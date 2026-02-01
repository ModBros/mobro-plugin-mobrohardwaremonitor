using System;
using MoBro.Plugin.SDK.Builders;
using MoBro.Plugin.SDK.Enums;
using MoBro.Plugin.SDK.Models.Categories;
using MoBro.Plugin.SDK.Models.Metrics;

namespace MoBro.Plugin.MoBroHardwareMonitor.Helper;

internal class Builder
{
  public static Metric StaticMetric(
    string id,
    CoreMetricType type,
    CoreCategory category,
    string? groupId = null,
    int? index = null
  )
  {
    return MoBroItem
      .CreateMetric()
      .WithId(Id(id, index)!)
      .WithLabel($"metrics.{id}.title", $"metrics.{id}.desc")
      .OfType(type)
      .OfCategory(category)
      .OfGroup(Id(groupId, index))
      .AsStaticValue()
      .Build();
  }

  public static Metric DynamicMetric(
    string id,
    CoreMetricType type,
    CoreCategory category,
    string? groupId = null,
    int? index = null
  )
  {
    return MoBroItem
      .CreateMetric()
      .WithId(Id(id, index)!)
      .WithLabel($"metrics.{id}.title", $"metrics.{id}.desc")
      .OfType(type)
      .OfCategory(category)
      .OfGroup(Id(groupId, index))
      .AsDynamicValue()
      .Build();
  }

  public static Group Group(string id, string label, string? desc = null, int? index = null)
  {
    return MoBroItem
      .CreateGroup()
      .WithId(Id(id, index)!)
      .WithLabel(label, desc)
      .Build();
  }

  public static MetricValue Value(string id, DateTime dateTime, object? value, int? index = null)
  {
    return new MetricValue(Id(id, index)!, dateTime, value);
  }

  private static string? Id(string? id, int? index)
  {
    if (id == null) return null;
    if (index == null) return id;

    return $"{id}.{index}";
  }
}