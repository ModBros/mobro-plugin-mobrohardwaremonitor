namespace MoBro.Plugin.MoBroHardwareMonitor;

internal static class Ids
{
  internal static class Cpu
  {
    internal const string Name = "cpu.name";
    internal const string Manufacturer = "cpu.manufacturer";
    internal const string Cores = "cpu.cores";
    internal const string LogicalProcessors = "cpu.logicalprocessors";
    internal const string MaxClockSpeed = "cpu.maxclock";

    internal const string TotalUsage = "cpu.totalusage";
    internal const string TotalTemperature = "cpu.totaltemperature";
    internal const string TotalPower = "cpu.totalPower";
  }

  internal static class Gpu
  {
    internal const string Name = "gpu.name";
    internal const string Manufacturer = "gpu.manufacturer";
    internal const string Driver = "gpu.driver";
    internal const string RefreshRate = "gpu.refreshrate";
    internal const string HorizontalResolution = "gpu.hres";
    internal const string VerticalResolution = "gpu.vres";
    internal const string UsageCore = "gpu.usage.core";
    internal const string UsageMemory = "gpu.usage.memory";
    internal const string Power = "gpu.power";
    internal const string Temperature = "gpu.temperature";
  }

  internal static class Memory
  {
    internal const string Manufacturer = "memory.manufacturer";
    internal const string Capacity = "memory.capacity";
    internal const string Frequency = "memory.frequency";

    internal const string TotalCapacity = "memory.totalcapacity";
    internal const string TotalAvailable = "memory.totalavailable";
    internal const string TotalUsed = "memory.totalused";
    internal const string TotalUsage = "memory.totalusage";
  }

  internal static class System
  {
    internal const string OsName = "system.osname";
    internal const string OsVersion = "system.osversion";
    internal const string OsType = "system.ostype";
    internal const string OsArchitecture = "system.osarchitecture";
    internal const string User = "system.user";
    internal const string Hostname = "system.hostname";
    internal const string Time = "system.time";
    internal const string ProcessName = "system.process.name";
    internal const string ProcessCpu = "system.process.cpu";
    internal const string ProcessMemory = "system.process.memory";
  }

  internal static class Groups
  {
    internal const string CpuGroupIndividual = "group.cpu";
    internal const string GpuGroupIndividual = "group.gpu";
    internal const string MemoryGroupIndividual = "group.memory";
    internal const string MemoryGroupOverall = "group.memory.overall";
    internal const string ProcessGroupIndividual = "group.process";
  }
}