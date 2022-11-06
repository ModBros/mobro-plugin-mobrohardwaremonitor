using System.Collections.Generic;
using MoBro.Plugin.MoBroHardwareMonitor.Model.Stats;

namespace MoBro.Plugin.MoBroHardwareMonitor.DataCollectors;

internal interface IHardwareMonitor
{
  public ProcessorStats GetProcessor();
  public MemoryStats GetMemory();
  public IEnumerable<GraphicsStats> GetGraphics();
}