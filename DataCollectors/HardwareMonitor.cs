using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using MoBro.Plugin.MoBroHardwareMonitor.Model.Stats;

namespace MoBro.Plugin.MoBroHardwareMonitor.DataCollectors;

internal class HardwareMonitor : IHardwareMonitor
{
  private static readonly Regex GpuIdxRegex = new(@"\w+phys_(\d)\w*", RegexOptions.Compiled);

  private readonly PerformanceCounter _cpuUsageCounter;
  private readonly PerformanceCounter _cpuClockCounter;

  private readonly Dictionary<int, List<PerformanceCounter>> _gpuUsage3DCounters;
  private readonly Dictionary<int, List<PerformanceCounter>> _gpuMemoryCounters;

  public HardwareMonitor()
  {
    _cpuUsageCounter = new PerformanceCounter("Processor Information", "% Processor Time", "_Total", true);
    _cpuClockCounter = new PerformanceCounter("Processor Information", "Processor Frequency", "_Total", true);

    _gpuUsage3DCounters = GroupGpuCounters("GPU Engine", "Utilization Percentage", "engtype_3D");
    _gpuMemoryCounters = GroupGpuCounters("GPU Adapter Memory", "Dedicated Usage");
  }

  public ProcessorStats GetProcessor()
  {
    return new ProcessorStats(
      Usage: _cpuUsageCounter.NextValue(),
      Clock: _cpuClockCounter.NextValue(),
      DateTime.UtcNow
    );
  }

  public MemoryStats GetMemory()
  {
    var info = GetMemoryInfo();
    var ullUsedPhys = info.ullTotalPhys - info.ullAvailPhys;
    return new MemoryStats(
      Capacity: info.ullTotalPhys,
      Available: info.ullAvailPhys,
      Used: ullUsedPhys,
      Usage: ullUsedPhys / (double)info.ullTotalPhys,
      DateTime.UtcNow
    );
  }

  public IEnumerable<GraphicsStats> GetGraphics()
  {
    var now = DateTime.UtcNow;
    return _gpuUsage3DCounters.Keys.Select(idx =>
    {
      var usage3D = 0D;
      var usageMemory = 0UL;
      if (_gpuUsage3DCounters.TryGetValue(idx, out var uCounters))
      {
        foreach (var counter in uCounters)
        {
          try
          {
            usage3D += counter.NextValue();
          }
          catch (Exception)
          {
            // ignored
          }
        }
      }

      if (_gpuMemoryCounters.TryGetValue(idx, out var mCounters))
      {
        foreach (var c in mCounters)
        {
          try
          {
            usageMemory += (ulong)c.NextValue();
          }
          catch (Exception)
          {
            // ignored
          }
        }
      }

      return new GraphicsStats(
        Index: idx,
        Usage3D: usage3D,
        UsedMemory: usageMemory,
        now
      );
    });
  }

  private Dictionary<int, List<PerformanceCounter>> GroupGpuCounters(string category, string counter,
    string? instanceEndsWith = null)
  {
    Dictionary<int, List<PerformanceCounter>> groupedCounters = new();


    if (!PerformanceCounterCategory.Exists(category) ||
        !PerformanceCounterCategory.CounterExists(counter, category)) return groupedCounters;

    var perfCounterCategory = new PerformanceCounterCategory(category);
    foreach (var counterName in perfCounterCategory.GetInstanceNames())
    {
      if (instanceEndsWith != null && !counterName.EndsWith(instanceEndsWith)) continue;

      var match = GpuIdxRegex.Match(counterName);
      if (!match.Success) continue;
      var idx = int.Parse(match.Groups[1].ToString());
      if (!groupedCounters.TryGetValue(idx, out var list))
      {
        groupedCounters[idx] = list = new List<PerformanceCounter>();
      }

      list.AddRange(perfCounterCategory
        .GetCounters(counterName)
        .Where(c => c.CounterName == counter)
      );
    }

    return groupedCounters;
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