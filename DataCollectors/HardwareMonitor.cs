using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using LibreHardwareMonitor.Hardware;
using MoBro.Plugin.MoBroHardwareMonitor.Extensions;
using MoBro.Plugin.MoBroHardwareMonitor.Model.Stats;

namespace MoBro.Plugin.MoBroHardwareMonitor.DataCollectors;

internal class HardwareMonitor : IHardwareMonitor
{
  private readonly IHardware[] _cpus;
  private readonly IHardware[] _gpus;

  private readonly Dictionary<int, double> _processCpuUsage = new();
  private long _lastClockTicks = long.MaxValue;

  public HardwareMonitor(Computer computer)
  {
    _cpus = computer.Hardware.Where(h => HardwareType.Cpu == h.HardwareType).ToArray();
    _gpus = computer.Hardware
      .Where(h => h.HardwareType is HardwareType.GpuAmd or HardwareType.GpuNvidia or HardwareType.GpuIntel)
      .ToArray();
  }

  public SystemStats GetSystem()
  {
    return new SystemStats(
      SystemTime: DateTime.Now,
      DateTime.UtcNow
    );
  }

  public IEnumerable<ProcessorStats> GetProcessors()
  {
    var now = DateTime.UtcNow;
    for (var i = 0; i < _cpus.Length; i++)
    {
      _cpus[i].Update();
      yield return new ProcessorStats(
        Index: i,
        Load: _cpus[i].Sensors.ValueOf(SensorType.Load, "total"),
        Temperature: _cpus[i].Sensors.ValueOf(SensorType.Temperature, "package"),
        Power: _cpus[i].Sensors.ValueOf(SensorType.Power, "package"),
        DateTime: now
      );
    }
  }

  public MemoryStats GetMemory()
  {
    var info = GetMemoryInfo();
    var ullUsedPhys = info.ullTotalPhys - info.ullAvailPhys;
    return new MemoryStats(
      Capacity: info.ullTotalPhys,
      Available: info.ullAvailPhys,
      Used: ullUsedPhys,
      Usage: (ullUsedPhys / (double)info.ullTotalPhys) * 100,
      DateTime.UtcNow
    );
  }

  public IEnumerable<GraphicsStats> GetGraphics()
  {
    var now = DateTime.UtcNow;
    for (var i = 0; i < _gpus.Length; i++)
    {
      _gpus[i].Update();
      yield return new GraphicsStats(
        Index: i,
        CoreLoad: _gpus[i].Sensors.ValueOf(SensorType.Load, "core"),
        MemoryLoad: _gpus[i].Sensors.ValueOf(SensorType.Load, "memory"),
        Temperature: _gpus[i].Sensors.ValueOf(SensorType.Temperature),
        Power: _gpus[i].Sensors.ValueOf(SensorType.Power),
        DateTime: now
      );
    }
  }

  public IEnumerable<TopProcessesStats> GetProcessesStats(int count, string sort)
  {
    var now = DateTime.UtcNow;
    var topProcesses = Process.GetProcesses()
      .Select(p => Tuple.Create(p, GetCpuUsage(now.Ticks, p)))
      .OrderByDescending(t => sort switch
      {
        "cpu" => t.Item2,
        "ram" => t.Item1.PrivateMemorySize64,
        _ => t.Item1.Id
      })
      .Take(count)
      .ToArray();

    _lastClockTicks = now.Ticks;

    for (var i = 0; i < topProcesses.Length; i++)
    {
      yield return new TopProcessesStats(
        i,
        topProcesses[i].Item1.ProcessName,
        topProcesses[i].Item2,
        topProcesses[i].Item1.PrivateMemorySize64,
        now
      );
    }
  }

  private double GetCpuUsage(long nowTicks, Process process)
  {
    long cpuTime;
    try
    {
      cpuTime = process.TotalProcessorTime.Ticks;
    }
    catch (Exception)
    {
      return 0D;
    }

    var cpuUsage = 0D;
    if (_processCpuUsage.TryGetValue(process.Id, out var lastCpuTime))
    {
      var cpuDelta = cpuTime - lastCpuTime;
      var wallClockDelta = nowTicks - _lastClockTicks;
      if (wallClockDelta > 0)
      {
        cpuUsage = cpuDelta / wallClockDelta / Environment.ProcessorCount;
      }
    }

    _processCpuUsage[process.Id] = cpuTime;
    return cpuUsage * 100;
  }

  private static MemoryInfo GetMemoryInfo()
  {
    var mi = new MemoryInfo();
    mi.dwLength = (uint)Marshal.SizeOf(mi);
    GlobalMemoryStatusEx(ref mi);
    return mi;
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
}