using System;
using System.Collections.Generic;
using MoBro.Plugin.MoBroHardwareMonitor.Helper;
using MoBro.Plugin.SDK.Enums;
using MoBro.Plugin.SDK.Models;
using MoBro.Plugin.SDK.Models.Metrics;

namespace MoBro.Plugin.MoBroHardwareMonitor.Model.Stats;

internal readonly record struct ProcessorStats(
  int Index,
  double Load,
  double Temperature,
  double Power,
  DateTime DateTime
) : IMetricConvertible
{
  public IEnumerable<IMoBroItem> ToRegistrations()
  {
    // groups are registered in ProcessorInfo

    // register dynamic metrics
    yield return Builder.DynamicMetric(
      Ids.Cpu.TotalUsage, CoreMetricType.Usage, CoreCategory.Cpu, Ids.Groups.CpuGroupIndividual, Index);
    yield return Builder.DynamicMetric(
      Ids.Cpu.TotalTemperature, CoreMetricType.Temperature, CoreCategory.Cpu, Ids.Groups.CpuGroupIndividual, Index);
    yield return Builder.DynamicMetric(
      Ids.Cpu.TotalPower, CoreMetricType.Power, CoreCategory.Cpu, Ids.Groups.CpuGroupIndividual, Index);
  }

  public IEnumerable<MetricValue> ToMetricValues()
  {
    yield return Builder.Value(Ids.Cpu.TotalUsage, DateTime, Load, Index);
    yield return Builder.Value(Ids.Cpu.TotalTemperature, DateTime, Temperature, Index);
    yield return Builder.Value(Ids.Cpu.TotalPower, DateTime, Power, Index);
  }
}