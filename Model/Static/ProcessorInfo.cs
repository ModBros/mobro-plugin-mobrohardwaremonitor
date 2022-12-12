using System;
using System.Collections.Generic;
using MoBro.Plugin.MoBroHardwareMonitor.Helper;
using MoBro.Plugin.SDK.Builders;
using MoBro.Plugin.SDK.Enums;
using MoBro.Plugin.SDK.Models;
using MoBro.Plugin.SDK.Models.Metrics;

namespace MoBro.Plugin.MoBroHardwareMonitor.Model.Static;

internal readonly record struct ProcessorInfo(
  int Index,
  string Name,
  string Manufacturer,
  uint Cores,
  uint LogicalProcessors,
  uint MaxClockSpeed,
  DateTime DateTime
) : IMetricConvertible
{
  public IEnumerable<IMoBroItem> ToRegistrations()
  {
    // register groups first
    yield return Builder.Group(Ids.Groups.CpuGroupIndividual, $"{Name} [{Index}]", null, Index);
    yield return Builder.Group(Ids.Groups.CpuGroupOverall, "CPU");

    // register static metrics
    yield return Builder.StaticMetric(
      Ids.Cpu.Name, CoreMetricType.Text, CoreCategory.Cpu, Ids.Groups.CpuGroupIndividual, Index);
    yield return Builder.StaticMetric(
      Ids.Cpu.Manufacturer, CoreMetricType.Text, CoreCategory.Cpu, Ids.Groups.CpuGroupIndividual, Index);
    yield return Builder.StaticMetric(
      Ids.Cpu.Cores, CoreMetricType.Numeric, CoreCategory.Cpu, Ids.Groups.CpuGroupIndividual, Index);
    yield return Builder.StaticMetric(
      Ids.Cpu.LogicalProcessors, CoreMetricType.Numeric, CoreCategory.Cpu, Ids.Groups.CpuGroupIndividual, Index);
    yield return Builder.StaticMetric(
      Ids.Cpu.MaxClockSpeed, CoreMetricType.Frequency, CoreCategory.Cpu, Ids.Groups.CpuGroupIndividual, Index);
  }

  public IEnumerable<MetricValue> ToMetricValues()
  {
    yield return Builder.Value(Ids.Cpu.Name, DateTime, Name, Index);
    yield return Builder.Value(Ids.Cpu.Manufacturer, DateTime, Manufacturer, Index);
    yield return Builder.Value(Ids.Cpu.Cores, DateTime, Cores, Index);
    yield return Builder.Value(Ids.Cpu.LogicalProcessors, DateTime, LogicalProcessors, Index);
    yield return Builder.Value(Ids.Cpu.MaxClockSpeed, DateTime, MaxClockSpeed, Index);
  }
}