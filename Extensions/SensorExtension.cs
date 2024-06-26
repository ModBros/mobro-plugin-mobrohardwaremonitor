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
    return sensors.FirstOrDefault(s => s.Is(type, labels))?.Value ?? -1;
  }
}