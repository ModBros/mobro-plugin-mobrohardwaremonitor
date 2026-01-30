using System;
using System.Collections.Generic;
using System.Linq;
using LibreHardwareMonitor.Hardware;
using MoBro.Plugin.MoBroHardwareMonitor.DataCollectors;
using MoBro.Plugin.MoBroHardwareMonitor.Helper;
using MoBro.Plugin.MoBroHardwareMonitor.Model;
using MoBro.Plugin.SDK;
using MoBro.Plugin.SDK.Services;

namespace MoBro.Plugin.MoBroHardwareMonitor;

public sealed class Plugin : IMoBroPlugin, IDisposable
{
  private static readonly TimeSpan InitialDelay = TimeSpan.FromSeconds(2);
  private const int DefaultUpdateFrequencyMs = 1000;

  private readonly IMoBroService _service;
  private readonly IMoBroScheduler _scheduler;

  private IHardwareInfoCollector? _hardwareInfoCollector;
  private IHardwareMonitor? _hardwareMonitor;
  private Computer? _computer;

  private readonly int _updateFrequency;
  private readonly bool _monitorCpu;
  private readonly bool _monitorGpu;
  private readonly bool _monitorRam;
  private readonly int _numProcesses;
  private readonly string _processesSort;

  public Plugin(IMoBroService service, IMoBroSettings settings, IMoBroScheduler scheduler)
  {
    _service = service;
    _scheduler = scheduler;

    _updateFrequency = settings.GetValue("update_frequency", DefaultUpdateFrequencyMs);
    _monitorCpu = settings.GetValue<bool>("cpu_metrics", true);
    _monitorGpu = settings.GetValue<bool>("gpu_metrics", true);
    _monitorRam = settings.GetValue<bool>("ram_metrics", true);
    _numProcesses = settings.GetValue<int>("num_processes", 0);
    _processesSort = settings.GetValue<string>("processes_sort", "cpu");
  }

  public void Init()
  {
    // check PawnIO status 
    _service.SetDependencyStatus("pawnio", PawnIo.GetStatus());

    // init LibreHardwareMonitor
    _computer = new Computer
    {
      IsCpuEnabled = _monitorCpu,
      IsGpuEnabled = _monitorGpu,
      IsMemoryEnabled = false,
      IsMotherboardEnabled = false,
      IsStorageEnabled = false,
      IsNetworkEnabled = false,
      IsControllerEnabled = false,
      IsPsuEnabled = false,
      IsBatteryEnabled = false
    };
    _computer.Open();

    _hardwareInfoCollector = new HardwareInfoCollector();
    _hardwareMonitor = new HardwareMonitor(_computer);

    // static metrics
    Register(_hardwareInfoCollector.GetSystem());
    Register(_hardwareInfoCollector.GetProcessors());
    Register(_hardwareInfoCollector.GetGraphics());
    Register(_hardwareInfoCollector.GetMemory());

    // dynamic metrics
    Register(_hardwareMonitor.GetSystem());
    if (_monitorCpu) Register(_hardwareMonitor.GetProcessors());
    if (_monitorGpu) Register(_hardwareMonitor.GetGraphics());
    if (_monitorRam) Register(_hardwareMonitor.GetMemory());
    if (_numProcesses > 0) Register(_hardwareMonitor.GetProcessesStats(_numProcesses, _processesSort));

    // start polling metric values
    _scheduler.Interval(Update, TimeSpan.FromMilliseconds(_updateFrequency), InitialDelay);
  }

  private void Update()
  {
    if (_hardwareMonitor is null) return;

    _service.UpdateMetricValues(_hardwareMonitor.GetSystem().ToMetricValues());

    if (_monitorCpu) _service.UpdateMetricValues(_hardwareMonitor.GetProcessors().SelectMany(c => c.ToMetricValues()));
    if (_monitorGpu) _service.UpdateMetricValues(_hardwareMonitor.GetGraphics().SelectMany(g => g.ToMetricValues()));
    if (_monitorRam) _service.UpdateMetricValues(_hardwareMonitor.GetMemory().ToMetricValues());
    if (_numProcesses > 0) _service.UpdateMetricValues(_hardwareMonitor.GetProcessesStats(_numProcesses, _processesSort) .SelectMany(g => g.ToMetricValues()));
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
    _computer?.Close();
  }
}