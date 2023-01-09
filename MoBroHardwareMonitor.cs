using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using MoBro.Plugin.MoBroHardwareMonitor.DataCollectors;
using MoBro.Plugin.MoBroHardwareMonitor.Model;
using MoBro.Plugin.SDK;

namespace MoBro.Plugin.MoBroHardwareMonitor;

public sealed class MoBroHardwareMonitor : IMoBroPlugin
{
  private static readonly TimeSpan UpdateInterval = TimeSpan.FromMilliseconds(1000);

  private readonly IHardwareInfoCollector _hardwareInfoCollector;
  private readonly IHardwareMonitor _hardwareMonitor;
  private readonly Timer _timer;
  private readonly IMoBroService _service;

  public MoBroHardwareMonitor(IMoBroService service)
  {
    _service = service;
    _hardwareInfoCollector = new HardwareInfoCollector();
    _hardwareMonitor = new HardwareMonitor();
    _timer = new Timer
    {
      Interval = UpdateInterval.TotalMilliseconds,
      AutoReset = true,
      Enabled = false
    };
    _timer.Elapsed += Update;
  }

  public void Init()
  {
    // static metrics
    Register(_hardwareInfoCollector.GetSystem());
    Register(_hardwareInfoCollector.GetProcessors());
    Register(_hardwareInfoCollector.GetGraphics());
    Register(_hardwareInfoCollector.GetMemory());

    // dynamic metrics
    Register(_hardwareMonitor.GetProcessor());
    Register(_hardwareMonitor.GetMemory());
    Register(_hardwareMonitor.GetGraphics());

    // start polling metric values
    _timer.Start();
  }

  public void Pause() => _timer.Stop();

  public void Resume() => _timer.Start();

  private void Update(object? sender, ElapsedEventArgs e)
  {
    try
    {
      _service.UpdateMetricValues(_hardwareMonitor.GetProcessor().ToMetricValues());
      _service.UpdateMetricValues(_hardwareMonitor.GetMemory().ToMetricValues());
      _service.UpdateMetricValues(_hardwareMonitor.GetGraphics().SelectMany(g => g.ToMetricValues()));
    }
    catch (Exception exception)
    {
      _service.NotifyError(exception);
    }
  }

  private void Register<T>(IEnumerable<T> convertibles) where T : IMetricConvertible
  {
    foreach (var metricConvertible in convertibles)
    {
      Register(metricConvertible);
    }
  }

  private void Register<T>(in T convertible) where T : IMetricConvertible
  {
    // register the metrics
    foreach (var moBroItem in convertible.ToRegistrations())
    {
      _service.RegisterItem(moBroItem);
    }

    // update values for the metrics
    foreach (var metricValue in convertible.ToMetricValues())
    {
      _service.UpdateMetricValue(metricValue);
    }
  }

  public void Dispose()
  {
  }
}