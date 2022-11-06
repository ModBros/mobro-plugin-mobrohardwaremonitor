using System;
using System.Collections.Generic;
using MoBro.Plugin.MoBroHardwareMonitor.Helper;
using MoBro.Plugin.SDK.Enums;
using MoBro.Plugin.SDK.Models;
using MoBro.Plugin.SDK.Models.Metrics;

namespace MoBro.Plugin.MoBroHardwareMonitor.Model.Stats;

internal readonly record struct GraphicsStats(
  int Index,
  double Usage3D,
  ulong UsedMemory,
  DateTime DateTime
) : IMetricConvertible
{
  public IEnumerable<IMoBroItem> ToRegistrations()
  {
    // groups are registered in GraphicsInfo

    // register dynamic metrics
    yield return Builder.DynamicMetric(
      Ids.Gpu.Usage3D, CoreMetricType.Usage, CoreCategory.Gpu, Ids.Groups.GpuGroupIndividual, Index);
    yield return Builder.DynamicMetric(
      Ids.Gpu.UsedMemory, CoreMetricType.Data, CoreCategory.Gpu, Ids.Groups.GpuGroupIndividual, Index);
  }

  public IEnumerable<IMetricValue> ToMetricValues()
  {
    yield return Builder.Value(Ids.Gpu.Usage3D, DateTime, Usage3D, Index);
    yield return Builder.Value(Ids.Gpu.UsedMemory, DateTime, UsedMemory, Index);
  }
}