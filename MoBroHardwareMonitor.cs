using System;
using System.Collections.Generic;
using System.Linq;
using MoBro.Plugin.MoBroHardwareMonitor.DataCollectors;
using MoBro.Plugin.MoBroHardwareMonitor.Model;
using MoBro.Plugin.SDK;
using MoBro.Plugin.SDK.Services;

namespace MoBro.Plugin.MoBroHardwareMonitor;

public sealed class MoBroHardwareMonitor : IMoBroPlugin
{
  private static readonly TimeSpan UpdateInterval = TimeSpan.FromMilliseconds(1000);
  private static readonly TimeSpan InitialDelay = TimeSpan.FromSeconds(2);

  private readonly IHardwareInfoCollector _hardwareInfoCollector;
  private readonly IHardwareMonitor _hardwareMonitor;
  private readonly IMoBroService _service;
  private readonly IMoBroScheduler _scheduler;


  public MoBroHardwareMonitor(IMoBroService service, IMoBroScheduler scheduler)
  {
    _service = service;
    _scheduler = scheduler;
    _hardwareInfoCollector = new HardwareInfoCollector();
    _hardwareMonitor = new HardwareMonitor();
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
    _scheduler.Interval(Update, UpdateInterval, InitialDelay);
  }

  private void Update()
  {
    _service.UpdateMetricValues(_hardwareMonitor.GetProcessor().ToMetricValues());
    _service.UpdateMetricValues(_hardwareMonitor.GetMemory().ToMetricValues());
    _service.UpdateMetricValues(_hardwareMonitor.GetGraphics().SelectMany(g => g.ToMetricValues()));
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
      _service.Register(moBroItem);
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