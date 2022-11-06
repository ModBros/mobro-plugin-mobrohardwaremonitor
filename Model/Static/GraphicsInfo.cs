using System;
using System.Collections.Generic;
using MoBro.Plugin.MoBroHardwareMonitor.Helper;
using MoBro.Plugin.SDK.Enums;
using MoBro.Plugin.SDK.Models;
using MoBro.Plugin.SDK.Models.Metrics;


namespace MoBro.Plugin.MoBroHardwareMonitor.Model.Static;

internal readonly record struct GraphicsInfo(
  int Index,
  string Name,
  string Manufacturer,
  string Driver,
  uint RefreshRate,
  uint HorizontalResolution,
  uint VerticalResolution,
  DateTime DateTime
) : IMetricConvertible
{
  public IEnumerable<IMoBroItem> ToRegistrations()
  {
    // register group first
    yield return Builder.Group(Ids.Groups.GpuGroupIndividual, $"{Name} [{Index}]", null, Index);

    // register static metrics
    yield return Builder.StaticMetric(
      Ids.Gpu.Name, CoreMetricType.Text, CoreCategory.Gpu, Ids.Groups.GpuGroupIndividual, Index);
    yield return Builder.StaticMetric(
      Ids.Gpu.Manufacturer, CoreMetricType.Text, CoreCategory.Gpu, Ids.Groups.GpuGroupIndividual, Index);
    yield return Builder.StaticMetric(
      Ids.Gpu.Driver, CoreMetricType.Text, CoreCategory.Gpu, Ids.Groups.GpuGroupIndividual, Index);
    yield return Builder.StaticMetric(
      Ids.Gpu.RefreshRate, CoreMetricType.Frequency, CoreCategory.Gpu, Ids.Groups.GpuGroupIndividual, Index);
    yield return Builder.StaticMetric(
      Ids.Gpu.HorizontalResolution, CoreMetricType.Numeric, CoreCategory.Gpu, Ids.Groups.GpuGroupIndividual, Index);
    yield return Builder.StaticMetric(
      Ids.Gpu.VerticalResolution, CoreMetricType.Numeric, CoreCategory.Gpu, Ids.Groups.GpuGroupIndividual, Index);
  }

  public IEnumerable<IMetricValue> ToMetricValues()
  {
    yield return Builder.Value(Ids.Gpu.Name, DateTime, Name, Index);
    yield return Builder.Value(Ids.Gpu.Manufacturer, DateTime, Manufacturer, Index);
    yield return Builder.Value(Ids.Gpu.Driver, DateTime, Driver, Index);
    yield return Builder.Value(Ids.Gpu.RefreshRate, DateTime, RefreshRate, Index);
    yield return Builder.Value(Ids.Gpu.HorizontalResolution, DateTime, HorizontalResolution, Index);
    yield return Builder.Value(Ids.Gpu.VerticalResolution, DateTime, VerticalResolution, Index);
  }
}