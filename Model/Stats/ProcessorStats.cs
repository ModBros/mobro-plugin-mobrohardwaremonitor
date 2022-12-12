using System;
using System.Collections.Generic;
using MoBro.Plugin.MoBroHardwareMonitor.Helper;
using MoBro.Plugin.SDK.Enums;
using MoBro.Plugin.SDK.Models;
using MoBro.Plugin.SDK.Models.Metrics;

namespace MoBro.Plugin.MoBroHardwareMonitor.Model.Stats;

internal readonly record struct ProcessorStats(
  double Usage,
  double Clock,
  DateTime DateTime
) : IMetricConvertible
{
  public IEnumerable<IMoBroItem> ToRegistrations()
  {
    // groups are registered in ProcessorInfo

    // register dynamic metrics
    yield return Builder.DynamicMetric(
      Ids.Cpu.TotalUsage, CoreMetricType.Usage, CoreCategory.Cpu, Ids.Groups.CpuGroupOverall);
    yield return Builder.DynamicMetric(
      Ids.Cpu.TotalClock, CoreMetricType.Frequency, CoreCategory.Cpu, Ids.Groups.CpuGroupOverall);
  }

  public IEnumerable<MetricValue> ToMetricValues()
  {
    yield return Builder.Value(Ids.Cpu.TotalUsage, DateTime, Usage);
    yield return Builder.Value(Ids.Cpu.TotalClock, DateTime, Clock);
  }
}