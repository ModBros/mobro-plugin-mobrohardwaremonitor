using System;
using System.Collections.Generic;
using MoBro.Plugin.MoBroHardwareMonitor.Helper;
using MoBro.Plugin.SDK.Enums;
using MoBro.Plugin.SDK.Models;
using MoBro.Plugin.SDK.Models.Metrics;

namespace MoBro.Plugin.MoBroHardwareMonitor.Model.Stats;

internal readonly record struct TopProcessesStats(
  int Index,
  string Name,
  double CpuUsage,
  double MemoryUsage,
  DateTime DateTime
) : IMetricConvertible
{
  public IEnumerable<IMoBroItem> ToRegistrations()
  {
    // register group
    yield return Builder.Group(Ids.Groups.ProcessGroupIndividual, $"Top process [{Index}]", null, Index);

    // register metrics
    yield return Builder.DynamicMetric(Ids.System.ProcessName, CoreMetricType.Text, CoreCategory.System,
      Ids.Groups.ProcessGroupIndividual, Index);
    yield return Builder.DynamicMetric(Ids.System.ProcessCpu, CoreMetricType.Usage, CoreCategory.System,
      Ids.Groups.ProcessGroupIndividual, Index);
    yield return Builder.DynamicMetric(Ids.System.ProcessMemory, CoreMetricType.Data, CoreCategory.System,
      Ids.Groups.ProcessGroupIndividual, Index);
  }

  public IEnumerable<MetricValue> ToMetricValues()
  {
    yield return Builder.Value(Ids.System.ProcessName, DateTime, Name, Index);
    yield return Builder.Value(Ids.System.ProcessCpu, DateTime, CpuUsage, Index);
    yield return Builder.Value(Ids.System.ProcessMemory, DateTime, MemoryUsage, Index);
  }
}