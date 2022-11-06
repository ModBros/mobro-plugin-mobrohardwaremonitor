using System.Collections.Generic;
using MoBro.Plugin.MoBroHardwareMonitor.Model.Static;

namespace MoBro.Plugin.MoBroHardwareMonitor.DataCollectors;

internal interface IHardwareInfoCollector
{
  public IEnumerable<GraphicsInfo> GetGraphics();
  public IEnumerable<ProcessorInfo> GetProcessors();
  public IEnumerable<MemoryInfo> GetMemory();
  public SystemInfo GetSystem();
}