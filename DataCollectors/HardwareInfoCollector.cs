using System;
using System.Collections.Generic;
using System.Management;
using Microsoft.Win32;
using MoBro.Plugin.MoBroHardwareMonitor.Model.Static;

namespace MoBro.Plugin.MoBroHardwareMonitor.DataCollectors;

internal class HardwareInfoCollector : IHardwareInfoCollector
{
  private const string ManagementScope = "root\\cimv2";

  private const string MemoryQuery = "SELECT Capacity, Manufacturer, Speed FROM Win32_PhysicalMemory";
  private const string ProcessorQuery = "SELECT Name, Manufacturer, NumberOfCores, NumberOfLogicalProcessors, MaxClockSpeed FROM Win32_Processor";
  private const string GraphicsQuery = "SELECT Name, AdapterCompatibility, DriverVersion, CurrentRefreshRate, CurrentHorizontalResolution, CurrentVerticalResolution FROM Win32_VideoController";

  private readonly EnumerationOptions _enumerationOptions = new()
  {
    ReturnImmediately = true,
    Rewindable = false,
    Timeout = TimeSpan.FromSeconds(5)
  };

  public IEnumerable<GraphicsInfo> GetGraphics()
  {
    var now = DateTime.UtcNow;
    var count = 0;
    foreach (var mo in QueryWmi(GraphicsQuery))
    {
      yield return new GraphicsInfo(
        count++,
        GetStringValue(mo["Name"]),
        GetStringValue(mo["AdapterCompatibility"]),
        GetStringValue(mo["DriverVersion"]),
        GetValue<uint>(mo["CurrentRefreshRate"]),
        GetValue<uint>(mo["CurrentHorizontalResolution"]),
        GetValue<uint>(mo["CurrentVerticalResolution"]),
        now
      );
    }
  }

  public IEnumerable<ProcessorInfo> GetProcessors()
  {
    var now = DateTime.UtcNow;
    var count = 0;
    foreach (var mo in QueryWmi(ProcessorQuery))
    {
      yield return new ProcessorInfo(
        count++,
        GetStringValue(mo["Name"]),
        GetStringValue(mo["Manufacturer"]),
        GetValue<uint>(mo["NumberOfCores"]),
        GetValue<uint>(mo["NumberOfLogicalProcessors"]),
        GetValue<uint>(mo["MaxClockSpeed"]) * 1_000_000, // convert to Hz
        now
      );
    }
  }

  public SystemInfo GetSystem()
  {
    const string hklmWinNtCurrent = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion";
    var osName = Registry.GetValue(hklmWinNtCurrent, "productName", "")?.ToString();
    // var osRelease = Registry.GetValue(HKLMWinNTCurrent, "ReleaseId", "")?.ToString();
    var osVersion = Environment.OSVersion.Version.ToString();
    var osType = Environment.Is64BitOperatingSystem ? "64-bit" : "32-bit";
    // var osBuild = Registry.GetValue(HKLMWinNTCurrent, "CurrentBuildNumber", "")?.ToString();

    return new SystemInfo(
      osName ?? "Unknown",
      osVersion,
      osType,
      Environment.UserName,
      Environment.MachineName,
      DateTime.UtcNow
    );
  }

  public IEnumerable<MemoryInfo> GetMemory()
  {
    var now = DateTime.UtcNow;
    var count = 0;
    foreach (var mo in QueryWmi(MemoryQuery))
    {
      yield return new MemoryInfo(
        count++,
        GetValue<ulong>(mo["Capacity"]),
        GetStringValue(mo["Manufacturer"]),
        GetValue<uint>(mo["Speed"]) * 1000, // convert to Hz
        now
      );
    }
  }

  private ManagementObjectCollection QueryWmi(string query)
  {
    using var mos = new ManagementObjectSearcher(ManagementScope, query, _enumerationOptions);
    return mos.Get();
  }

  private static string GetStringValue(object? obj)
  {
    return obj is string str ? str.Trim() : string.Empty;
  }

  private static T GetValue<T>(object? obj) where T : struct
  {
    return obj is null ? default : (T)obj;
  }
}