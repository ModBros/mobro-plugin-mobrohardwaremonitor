using System.Linq;
using LibreHardwareMonitor.Hardware;

namespace MoBro.Plugin.MoBroHardwareMonitor.Extensions;

public static class SensorExtensions
{
  public static bool Is(this ISensor sensor, SensorType type, params string[] labels)
  {
    if (sensor.SensorType != type) return false;
    return labels.Length switch
    {
      0 => true,
      1 => sensor.Name.Trim().ToLower().Contains(labels[0]),
      _ => labels.Any(sensor.Name.Trim().ToLower().Contains)
    };
  }

  public static double ValueOf(this ISensor[] sensors, SensorType type, params string[] labels)
  {
    var value = sensors.FirstOrDefault(s => s.Is(type, labels))?.Value;
    if (value.HasValue)
    {
      return ConvertUnit(type, value.Value);
    }

    return -1;
  }

  private static double ConvertUnit(SensorType type, double value)
  {
    return type switch
    {
      SensorType.Throughput => value * 8, // bytes => bit
      SensorType.Clock => value * 1_000_000, // MHz => Hertz
      SensorType.SmallData => value * 1_000_000, // MB => Byte
      SensorType.Data => value * 1_000_000_000, // GB => Byte
      _ => value
    };
  }
}