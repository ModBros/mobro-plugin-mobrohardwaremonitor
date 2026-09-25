using System;
using System.Collections.Generic;
using MoBro.Plugin.MoBroHardwareMonitor.Helper;
using MoBro.Plugin.SDK.Enums;
using MoBro.Plugin.SDK.Models;
using MoBro.Plugin.SDK.Models.Metrics;

namespace MoBro.Plugin.MoBroHardwareMonitor.Model.Stats;

internal readonly record struct NetworkStats(
  DateTime DateTime,
  string PingHost,
  TimeSpan? PingLatency,
  double DownloadSpeed,
  double UploadSpeed
) : IMetricConvertible
{
  public IEnumerable<IMoBroItem> ToRegistrations()
  {
    yield return Builder.StaticMetric(Ids.Network.PingHost, CoreMetricType.Text, CoreCategory.Network);
    yield return Builder.DynamicMetric(Ids.Network.PingLatency, CoreMetricType.Duration, CoreCategory.Network);
    yield return Builder.DynamicMetric(Ids.Network.DownloadSpeed, CoreMetricType.DataFlow, CoreCategory.Network);
    yield return Builder.DynamicMetric(Ids.Network.UploadSpeed, CoreMetricType.DataFlow, CoreCategory.Network);
  }

  public IEnumerable<MetricValue> ToMetricValues()
  {
    yield return Builder.Value(Ids.Network.PingHost, DateTime, PingHost);
    yield return Builder.Value(Ids.Network.PingLatency, DateTime, PingLatency);
    yield return Builder.Value(Ids.Network.DownloadSpeed, DateTime, DownloadSpeed);
    yield return Builder.Value(Ids.Network.UploadSpeed, DateTime, UploadSpeed);
  }
}