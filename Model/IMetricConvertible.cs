using System.Collections.Generic;
using MoBro.Plugin.SDK.Models;
using MoBro.Plugin.SDK.Models.Metrics;

namespace MoBro.Plugin.MoBroHardwareMonitor.Model;

internal interface IMetricConvertible
{
  IEnumerable<IMoBroItem> ToRegistrations();
  IEnumerable<MetricValue> ToMetricValues();
}