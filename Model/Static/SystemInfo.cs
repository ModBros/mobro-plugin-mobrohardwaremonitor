using System;
using System.Collections.Generic;
using MoBro.Plugin.MoBroHardwareMonitor.Helper;
using MoBro.Plugin.SDK.Enums;
using MoBro.Plugin.SDK.Models;
using MoBro.Plugin.SDK.Models.Metrics;

namespace MoBro.Plugin.MoBroHardwareMonitor.Model.Static;

internal readonly record struct SystemInfo(
  string OsName,
  string OsVersion,
  string OsType,
  string OsArchitecture,
  string User,
  string HostName,
  DateTime DateTime
) : IMetricConvertible
{
  public IEnumerable<IMoBroItem> ToRegistrations()
  {
    yield return Builder.StaticMetric(Ids.System.OsName, CoreMetricType.Text, CoreCategory.System);
    yield return Builder.StaticMetric(Ids.System.OsVersion, CoreMetricType.Text, CoreCategory.System);
    yield return Builder.StaticMetric(Ids.System.OsType, CoreMetricType.Text, CoreCategory.System);
    yield return Builder.StaticMetric(Ids.System.OsArchitecture, CoreMetricType.Text, CoreCategory.System);
    yield return Builder.StaticMetric(Ids.System.User, CoreMetricType.Text, CoreCategory.System);
    yield return Builder.StaticMetric(Ids.System.Hostname, CoreMetricType.Text, CoreCategory.System);
  }

  public IEnumerable<MetricValue> ToMetricValues()
  {
    yield return Builder.Value(Ids.System.OsName, DateTime, OsName);
    yield return Builder.Value(Ids.System.OsVersion, DateTime, OsVersion);
    yield return Builder.Value(Ids.System.OsType, DateTime, OsType);
    yield return Builder.Value(Ids.System.OsArchitecture, DateTime, OsArchitecture);
    yield return Builder.Value(Ids.System.User, DateTime, User);
    yield return Builder.Value(Ids.System.Hostname, DateTime, HostName);
  }
}