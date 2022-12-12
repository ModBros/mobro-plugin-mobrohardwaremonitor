using System;
using System.Collections.Generic;
using MoBro.Plugin.MoBroHardwareMonitor.Helper;
using MoBro.Plugin.SDK.Enums;
using MoBro.Plugin.SDK.Models;
using MoBro.Plugin.SDK.Models.Metrics;

namespace MoBro.Plugin.MoBroHardwareMonitor.Model.Stats;

internal readonly record struct SystemStats(
  DateTime SystemTime,
  DateTime DateTime
) : IMetricConvertible
{
  public IEnumerable<IMoBroItem> ToRegistrations()
  {
    // groups are registered in SystemInfo

    // register dynamic metrics
    yield return Builder.DynamicMetric(Ids.System.Time, CoreMetricType.DateTime, CoreCategory.System);
  }

  public IEnumerable<MetricValue> ToMetricValues()
  {
    yield return Builder.Value(Ids.System.Time, DateTime, SystemTime);
  }
}