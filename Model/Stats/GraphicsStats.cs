using System;
using System.Collections.Generic;
using MoBro.Plugin.MoBroHardwareMonitor.Helper;
using MoBro.Plugin.SDK.Enums;
using MoBro.Plugin.SDK.Models;
using MoBro.Plugin.SDK.Models.Metrics;

namespace MoBro.Plugin.MoBroHardwareMonitor.Model.Stats;

internal readonly record struct GraphicsStats(
  int Index,
  double CoreLoad,
  double MemoryLoad,
  double Power,
  double Temperature,
  DateTime DateTime
) : IMetricConvertible
{
  public IEnumerable<IMoBroItem> ToRegistrations()
  {
    // groups are registered in GraphicsInfo

    // register dynamic metrics
    yield return Builder.DynamicMetric(
      Ids.Gpu.UsageCore, CoreMetricType.Usage, CoreCategory.Gpu, Ids.Groups.GpuGroupIndividual, Index);
    yield return Builder.DynamicMetric(
      Ids.Gpu.UsageMemory, CoreMetricType.Data, CoreCategory.Gpu, Ids.Groups.GpuGroupIndividual, Index);
    yield return Builder.DynamicMetric(
      Ids.Gpu.Power, CoreMetricType.Power, CoreCategory.Gpu, Ids.Groups.GpuGroupIndividual, Index);
    yield return Builder.DynamicMetric(
      Ids.Gpu.Temperature, CoreMetricType.Temperature, CoreCategory.Gpu, Ids.Groups.GpuGroupIndividual, Index);
  }

  public IEnumerable<MetricValue> ToMetricValues()
  {
    yield return Builder.Value(Ids.Gpu.UsageCore, DateTime, CoreLoad, Index);
    yield return Builder.Value(Ids.Gpu.UsageMemory, DateTime, MemoryLoad, Index);
    yield return Builder.Value(Ids.Gpu.Power, DateTime, Power, Index);
    yield return Builder.Value(Ids.Gpu.Temperature, DateTime, Temperature, Index);
  }
}