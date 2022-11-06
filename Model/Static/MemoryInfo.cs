using System;
using System.Collections.Generic;
using MoBro.Plugin.MoBroHardwareMonitor.Helper;
using MoBro.Plugin.SDK.Enums;
using MoBro.Plugin.SDK.Models;
using MoBro.Plugin.SDK.Models.Metrics;

namespace MoBro.Plugin.MoBroHardwareMonitor.Model.Static;

internal readonly record struct MemoryInfo(
  int Index,
  ulong Capacity,
  string Manufacturer,
  uint Frequency,
  DateTime DateTime
) : IMetricConvertible
{
  public IEnumerable<IMoBroItem> ToRegistrations()
  {
    // register groups first
    yield return Builder.Group(Ids.Groups.MemoryGroupIndividual, $"{Manufacturer} [{Index}]", null, Index);
    yield return Builder.Group(Ids.Groups.MemoryGroupOverall, "Memory");

    // register static metrics
    yield return Builder.StaticMetric(
      Ids.Memory.Capacity, CoreMetricType.Data, CoreCategory.Ram, Ids.Groups.MemoryGroupIndividual, Index);
    yield return Builder.StaticMetric(
      Ids.Memory.Manufacturer, CoreMetricType.Text, CoreCategory.Ram, Ids.Groups.MemoryGroupIndividual, Index);
    yield return Builder.StaticMetric(
      Ids.Memory.Frequency, CoreMetricType.Frequency, CoreCategory.Ram, Ids.Groups.MemoryGroupIndividual, Index);
  }

  public IEnumerable<IMetricValue> ToMetricValues()
  {
    yield return Builder.Value(Ids.Memory.Capacity, DateTime, Capacity, Index);
    yield return Builder.Value(Ids.Memory.Manufacturer, DateTime, Manufacturer, Index);
    yield return Builder.Value(Ids.Memory.Frequency, DateTime, Frequency, Index);
  }
}