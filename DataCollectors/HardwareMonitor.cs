using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using LibreHardwareMonitor.Hardware;
using MoBro.Plugin.MoBroHardwareMonitor.Model.Stats;
using MoBro.Plugin.SDK.Services;

namespace MoBro.Plugin.MoBroHardwareMonitor.DataCollectors;

internal class HardwareMonitor : IHardwareMonitor
{
  private static readonly TimeSpan LibreSensorScanInterval = TimeSpan.FromSeconds(10);
  private static readonly TimeSpan LibreSensorInitDuration = TimeSpan.FromMinutes(5);

  private readonly Dictionary<int, long> _processCpuTimeTicks = new();
  private readonly IMoBroScheduler _scheduler;
  private readonly Computer _computer;
  private readonly DateTime _startTime;

  private (IHardware, Dictionary<LibreSensor, ISensor?>)[] _cpuSensors;
  private (IHardware, Dictionary<LibreSensor, ISensor?>)[] _gpuSensors;

  private long _lastWallTimestamp;
  private bool _hasProcessBaseline;

  public HardwareMonitor(Computer computer, IMoBroScheduler scheduler)
  {
    _startTime = DateTime.UtcNow;
    _scheduler = scheduler;
    _computer = computer;

    lock (_computer)
    {
      _cpuSensors = ParseCpuSensors(computer);
      _gpuSensors = ParseGpuSensors(computer);
    }

    scheduler.OneOff(RescanLibreSensors, LibreSensorScanInterval);
  }

  public SystemStats GetSystem() => new(DateTime.UtcNow);

  public IEnumerable<ProcessorStats> GetProcessors()
  {
    lock (_computer)
    {
      var now = DateTime.UtcNow;
      for (var i = 0; i < _cpuSensors.Length; i++)
      {
        var (hardware, sensors) = _cpuSensors[i];
        hardware.Update();
        yield return new ProcessorStats(
          Index: i,
          Load: GetSensorValue(SensorType.Load, sensors[LibreSensor.CpuTotalUsage]),
          Temperature: GetSensorValue(SensorType.Temperature, sensors[LibreSensor.CpuTotalTemperature]),
          Power: GetSensorValue(SensorType.Power, sensors[LibreSensor.CpuTotalPower]),
          DateTime: now
        );
      }
    }
  }

  public MemoryStats GetMemory()
  {
    var now = DateTime.UtcNow;
    var info = GetMemoryInfo();

    var total = info.ullTotalPhys;
    var available = info.ullAvailPhys;
    var used = total >= available ? total - available : 0UL;

    var usage = total > 0
      ? (used / (double)total) * 100
      : 0;

    return new MemoryStats(
      Capacity: total,
      Available: available,
      Used: used,
      Usage: usage,
      now
    );
  }

  public IEnumerable<GraphicsStats> GetGraphics()
  {
    lock (_computer)
    {
      var now = DateTime.UtcNow;
      for (var i = 0; i < _gpuSensors.Length; i++)
      {
        var (hardware, sensors) = _gpuSensors[i];
        hardware.Update();
        yield return new GraphicsStats(
          Index: i,
          CoreLoad: GetSensorValue(SensorType.Load, sensors[LibreSensor.GpuUsageCore]),
          MemoryLoad: GetSensorValue(SensorType.Load, sensors[LibreSensor.GpuMemoryUsage]),
          MemoryCapacity: (long?)GetSensorValue(SensorType.SmallData, sensors[LibreSensor.GpuMemoryCapacity]),
          MemoryAvailable: (long?)GetSensorValue(SensorType.SmallData, sensors[LibreSensor.GpuMemoryAvailable]),
          MemoryUsed: (long?)GetSensorValue(SensorType.SmallData, sensors[LibreSensor.GpuMemoryUsed]),
          Temperature: GetSensorValue(SensorType.Temperature, sensors[LibreSensor.GpuTemperature]),
          Power: GetSensorValue(SensorType.Power, sensors[LibreSensor.GpuPower]),
          DateTime: now
        );
      }
    }
  }

  public IEnumerable<TopProcessesStats> GetProcessesStats(int count, string sort)
  {
    var now = DateTime.UtcNow;
    var nowWallTimestamp = Stopwatch.GetTimestamp();

    var processes = Process.GetProcesses();
    var alivePids = new HashSet<int>(processes.Length);

    try
    {
      var topProcesses = processes
        .Select(p =>
        {
          alivePids.Add(p.Id);
          var cpu = GetCpuUsagePercent(nowWallTimestamp, p);
          return (Process: p, Cpu: cpu);
        })
        .OrderByDescending(t => sort?.ToLowerInvariant() switch
        {
          "cpu" => t.Cpu,
          "ram" => t.Process.PrivateMemorySize64,
          _ => t.Process.Id
        })
        .Take(count)
        .ToArray();

      // Update baseline AFTER sampling.
      _lastWallTimestamp = nowWallTimestamp;
      _hasProcessBaseline = true;

      // Prevent unbounded growth from exited processes.
      foreach (var pid in _processCpuTimeTicks.Keys.Where(pid => !alivePids.Contains(pid)).ToArray())
      {
        _processCpuTimeTicks.Remove(pid);
      }

      for (var i = 0; i < topProcesses.Length; i++)
      {
        yield return new TopProcessesStats(
          i,
          topProcesses[i].Process.ProcessName,
          topProcesses[i].Cpu,
          topProcesses[i].Process.PrivateMemorySize64,
          now
        );
      }
    }
    finally
    {
      foreach (var p in processes)
      {
        p.Dispose();
      }
    }
  }

  private double GetCpuUsagePercent(long nowWallTimestamp, Process process)
  {
    long cpuTimeTicks;
    try
    {
      cpuTimeTicks = process.TotalProcessorTime.Ticks; // 100ns units
    }
    catch (Exception)
    {
      return 0D;
    }

    if (!_hasProcessBaseline || _lastWallTimestamp == 0)
    {
      _processCpuTimeTicks[process.Id] = cpuTimeTicks;
      return 0D;
    }

    var wallDeltaTimestamp = nowWallTimestamp - _lastWallTimestamp;
    if (wallDeltaTimestamp <= 0)
    {
      _processCpuTimeTicks[process.Id] = cpuTimeTicks;
      return 0D;
    }

    var wallSeconds = wallDeltaTimestamp / (double)Stopwatch.Frequency;

    var cpuUsage = 0D;
    if (_processCpuTimeTicks.TryGetValue(process.Id, out var lastCpuTicks))
    {
      var cpuDeltaTicks = cpuTimeTicks - lastCpuTicks;
      if (cpuDeltaTicks > 0 && wallSeconds > 0)
      {
        var cpuSeconds = cpuDeltaTicks / (double)TimeSpan.TicksPerSecond;
        cpuUsage = (cpuSeconds / (wallSeconds * Environment.ProcessorCount)) * 100;
      }
    }

    _processCpuTimeTicks[process.Id] = cpuTimeTicks;
    return cpuUsage;
  }

  private void RescanLibreSensors()
  {
    lock (_computer)
    {
      _cpuSensors = ParseCpuSensors(_computer);
      _gpuSensors = ParseGpuSensors(_computer);
    }

    if (_startTime + LibreSensorInitDuration > DateTime.UtcNow)
    {
      _scheduler.OneOff(RescanLibreSensors, LibreSensorScanInterval);
    }
  }

  private static double? GetSensorValue(SensorType type, ISensor? sensor)
  {
    var value = sensor?.Value;

    if (value is null) return null;
    if (value == 0f) return 0D;

    var doubleVal = Convert.ToDouble(value);
    return type switch
    {
      SensorType.Throughput => doubleVal * 8, // bytes => bit
      SensorType.Clock => doubleVal * 1_000_000, // MHz => Hertz
      SensorType.SmallData => doubleVal * 1_000_000, // MB => Byte
      SensorType.Data => doubleVal * 1_000_000_000, // GB => Byte
      _ => doubleVal
    };
  }

  private static (IHardware, Dictionary<LibreSensor, ISensor?>)[] ParseCpuSensors(Computer computer)
  {
    var cpus = computer.Hardware
      .Where(h => HardwareType.Cpu == h.HardwareType)
      .ToArray();
    var sensors = new ValueTuple<IHardware, Dictionary<LibreSensor, ISensor?>>[cpus.Length];
    for (var i = 0; i < cpus.Length; i++)
    {
      var hardware = cpus[i];
      hardware.Update();
      sensors[i] = (
        hardware,
        new Dictionary<LibreSensor, ISensor?>
        {
          { LibreSensor.CpuTotalUsage, GetSensor(hardware, SensorType.Load, "total") },
          { LibreSensor.CpuTotalTemperature, GetSensor(hardware, SensorType.Temperature, "package", "tctl", "tdie") },
          { LibreSensor.CpuTotalPower, GetSensor(hardware, SensorType.Power, "package", "total") }
        }
      );
    }

    return sensors;
  }

  private static ValueTuple<IHardware, Dictionary<LibreSensor, ISensor?>>[] ParseGpuSensors(Computer computer)
  {
    var gpus = computer.Hardware
      .Where(h => h.HardwareType is HardwareType.GpuAmd or HardwareType.GpuNvidia or HardwareType.GpuIntel)
      .ToArray();
    var sensors = new ValueTuple<IHardware, Dictionary<LibreSensor, ISensor?>>[gpus.Length];
    for (var i = 0; i < gpus.Length; i++)
    {
      var hardware = gpus[i];
      hardware.Update();
      sensors[i] = (
        hardware,
        new Dictionary<LibreSensor, ISensor?>
        {
          { LibreSensor.GpuUsageCore, GetSensor(hardware, SensorType.Load, "core") },
          { LibreSensor.GpuMemoryUsage, GetSensor(hardware, SensorType.Load, "memory") },
          { LibreSensor.GpuMemoryCapacity, GetSensor(hardware, SensorType.SmallData, "total") },
          { LibreSensor.GpuMemoryAvailable, GetSensor(hardware, SensorType.SmallData, "free", "available") },
          { LibreSensor.GpuMemoryUsed, GetSensor(hardware, SensorType.SmallData, "used") },
          { LibreSensor.GpuPower, GetSensor(hardware, SensorType.Power, "core", "hot spot") },
          { LibreSensor.GpuTemperature, GetSensor(hardware, SensorType.Temperature, "package", "total") },
        }
      );
    }

    return sensors;
  }

  private static ISensor? GetSensor(IHardware hardware, SensorType type, params string[] preferredNameFragments)
  {
    // Try to match preferred name fragments first (case-insensitive).
    if (preferredNameFragments.Length > 0)
    {
      foreach (var fragment in preferredNameFragments)
      {
        var sensor = hardware.Sensors.FirstOrDefault(s =>
          s.SensorType == type &&
          s.Value.HasValue &&
          s.Name.Contains(fragment, StringComparison.OrdinalIgnoreCase)
        );

        if (sensor?.Value != null) return sensor;
      }
    }

    // Fallback: first available sensor of that type.
    return hardware.Sensors.FirstOrDefault(s => s.SensorType == type && s.Value.HasValue);
  }

  private static MemoryInfo GetMemoryInfo()
  {
    var mi = new MemoryInfo();
    mi.dwLength = (uint)Marshal.SizeOf(mi);
    return !GlobalMemoryStatusEx(ref mi) ? default : mi;
  }

  [DllImport("kernel32.dll")]
  [return: MarshalAs(UnmanagedType.Bool)]
  private static extern bool GlobalMemoryStatusEx(ref MemoryInfo mi);

  [StructLayout(LayoutKind.Sequential)]
  private struct MemoryInfo
  {
    public uint dwLength; //Current structure size
    public uint dwMemoryLoad; //Current memory utilization
    public ulong ullTotalPhys; //Total physical memory size
    public ulong ullAvailPhys; //Available physical memory size
    public ulong ullTotalPageFile; //Total Exchange File Size
    public ulong ullAvailPageFile; //Total Exchange File Size
    public ulong ullTotalVirtual; //Total virtual memory size
    public ulong ullAvailVirtual; //Available virtual memory size
    public ulong ullAvailExtendedVirtual; //Keep this value always zero
  }

  private enum LibreSensor
  {
    CpuTotalUsage = 0,
    CpuTotalTemperature = 1,
    CpuTotalPower = 2,
    GpuUsageCore = 3,
    GpuMemoryUsage = 4,
    GpuMemoryCapacity = 5,
    GpuMemoryAvailable = 6,
    GpuMemoryUsed = 7,
    GpuPower = 8,
    GpuTemperature = 9
  }
}