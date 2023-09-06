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
    yield return Builder.Value(Ids.Cpu.Name, DateTime, SanitizedName(), Index);
    yield return Builder.Value(Ids.Cpu.Manufacturer, DateTime, SanitizedManufacturerName(), Index);
    yield return Builder.Value(Ids.Cpu.Cores, DateTime, Cores, Index);
    yield return Builder.Value(Ids.Cpu.LogicalProcessors, DateTime, LogicalProcessors, Index);
    yield return Builder.Value(Ids.Cpu.MaxClockSpeed, DateTime, MaxClockSpeed, Index);
  }

  private string SanitizedManufacturerName() => Manufacturer.Trim() switch
  {
    "GenuineIntel" => "Intel",
    "AuthenticAMD" => "AMD",
    _ => Manufacturer
  };

  private string SanitizedName()
  {
    StringBuilder builder = new(Name);
    builder.Replace("(R)", string.Empty);
    builder.Replace("(TM)", string.Empty);
    builder.Replace("(tm)", string.Empty);
    builder.Replace("CPU", string.Empty);
    builder.Replace("Dual-Core Processor", string.Empty);
    builder.Replace("Triple-Core Processor", string.Empty);
    builder.Replace("Quad-Core Processor", string.Empty);
    builder.Replace("Six-Core Processor", string.Empty);
    builder.Replace("Eight-Core Processor", string.Empty);
    builder.Replace("Twelve-Core Processor", string.Empty);
    builder.Replace("Sixteen-Core Processor", string.Empty);
    builder.Replace("6-Core Processor", string.Empty);
    builder.Replace("8-Core Processor", string.Empty);
    builder.Replace("12-Core Processor", string.Empty);
    builder.Replace("16-Core Processor", string.Empty);
    builder.Replace("24-Core Processor", string.Empty);
    builder.Replace("32-Core Processor", string.Empty);
    builder.Replace("64-Core Processor", string.Empty);
    builder.Replace("  ", " ");

    var sanitizedName = builder.ToString();
    return sanitizedName.Contains('@') 
      ? sanitizedName.Remove(sanitizedName.LastIndexOf('@')).Trim() 
      : sanitizedName.Trim();
  }
}