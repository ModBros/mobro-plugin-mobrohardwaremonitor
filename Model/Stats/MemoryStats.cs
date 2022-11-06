using System;
using System.Collections.Generic;
using MoBro.Plugin.MoBroHardwareMonitor.Helper;
using MoBro.Plugin.SDK.Enums;
using MoBro.Plugin.SDK.Models;
using MoBro.Plugin.SDK.Models.Metrics;

namespace MoBro.Plugin.MoBroHardwareMonitor.Model.Stats;

internal readonly record struct MemoryStats(
  ulong Capacity,
  ulong Available,
  ulong Used,
  double Usage,
  DateTime DateTime
) : IMetricConvertible
{
  public IEnumerable<IMoBroItem> ToRegistrations()
  {
    // groups are registered in MemoryInfo

    // register dynamic metrics
    yield return Builder.DynamicMetric(
      Ids.Memory.TotalCapacity, CoreMetricType.Data, CoreCategory.Ram, Ids.Groups.MemoryGroupOverall);
    yield return Builder.DynamicMetric(
      Ids.Memory.TotalAvailable, CoreMetricType.Data, CoreCategory.Ram, Ids.Groups.MemoryGroupOverall);
    yield return Builder.DynamicMetric(
      Ids.Memory.TotalUsed, CoreMetricType.Data, CoreCategory.Ram, Ids.Groups.MemoryGroupOverall);
    yield return Builder.DynamicMetric(
      Ids.Memory.TotalUsage, CoreMetricType.Usage, CoreCategory.Ram, Ids.Groups.MemoryGroupOverall);
  }

  public IEnumerable<IMetricValue> ToMetricValues()
  {
    yield return Builder.Value(Ids.Memory.TotalCapacity, DateTime, Capacity);
    yield return Builder.Value(Ids.Memory.TotalAvailable, DateTime, Available);
    yield return Builder.Value(Ids.Memory.TotalUsed, DateTime, Used);
    yield return Builder.Value(Ids.Memory.TotalUsage, DateTime, Usage);
  }
}