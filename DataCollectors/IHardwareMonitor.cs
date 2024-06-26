using System.Collections.Generic;
using MoBro.Plugin.MoBroHardwareMonitor.Model.Stats;

namespace MoBro.Plugin.MoBroHardwareMonitor.DataCollectors;

internal interface IHardwareMonitor
{
  public SystemStats GetSystem();
  public IEnumerable<ProcessorStats> GetProcessors();
  public MemoryStats GetMemory();
  public IEnumerable<GraphicsStats> GetGraphics();
  public IEnumerable<TopProcessesStats> GetProcessesStats(int count, string sort);
}