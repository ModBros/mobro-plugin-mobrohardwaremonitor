using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using MoBro.Plugin.MoBroHardwareMonitor.DataCollectors;
using MoBro.Plugin.MoBroHardwareMonitor.Model;
using MoBro.Plugin.SDK;
using MoBro.Plugin.SDK.Models.Metrics;

namespace MoBro.Plugin.MoBroHardwareMonitor;

public sealed class MoBroHardwareMonitor : IMoBroPlugin
{
  private readonly IHardwareInfoCollector _hardwareInfoCollector;
  private readonly IHardwareMonitor _hardwareMonitor;

  private readonly Dictionary<string, object?> _staticValues = new();
  private readonly Dictionary<string, object?> _cpuValues = new();
  private readonly Dictionary<string, object?> _memoryValues = new();
  private readonly Dictionary<string, object?> _gpuValues = new();
  private readonly Dictionary<string, object?> _systemValues = new();

  public MoBroHardwareMonitor()
  {
    _hardwareInfoCollector = new HardwareInfoCollector();
    _hardwareMonitor = new HardwareMonitor();
  }

  public Task Init(IPluginSettings settings, IMoBro mobro)
  {
    _staticValues.Clear();
    _cpuValues.Clear();
    _memoryValues.Clear();
    _gpuValues.Clear();
    _systemValues.Clear();

    foreach (var metric in GetAllStatic().SelectMany(m => m.ToRegistrations()))
    {
      _staticValues.TryAdd(metric.Id, null);
      mobro.Register(metric);
    }

    foreach (var metric in _hardwareMonitor.GetProcessor().ToRegistrations())
    {
      _cpuValues.TryAdd(metric.Id, null);
      mobro.Register(metric);
    }

    foreach (var metric in _hardwareMonitor.GetMemory().ToRegistrations())
    {
      _memoryValues.TryAdd(metric.Id, null);
      mobro.Register(metric);
    }

    foreach (var metric in _hardwareMonitor.GetGraphics().SelectMany(m => m.ToRegistrations()))
    {
      _gpuValues.TryAdd(metric.Id, null);
      mobro.Register(metric);
    }

    foreach (var metric in _hardwareMonitor.GetSystem().ToRegistrations())
    {
      _systemValues.TryAdd(metric.Id, null);
      mobro.Register(metric);
    }

    return Task.CompletedTask;
  }

  public Task<IEnumerable<IMetricValue>> GetMetricValues(IList<string> ids)
  {
    var idSet = ids.ToImmutableHashSet();
    var loadStatic = _staticValues.Keys.Any(idSet.Contains);

    var metrics = loadStatic
      ? GetAllStatic().SelectMany(m => m.ToMetricValues())
      : Enumerable.Empty<IMetricValue>();

    return Task.FromResult(metrics
      .Concat(_cpuValues.Keys.Any(idSet.Contains)
        ? _hardwareMonitor.GetProcessor().ToMetricValues()
        : Enumerable.Empty<IMetricValue>()
      )
      .Concat(_memoryValues.Keys.Any(idSet.Contains)
        ? _hardwareMonitor.GetMemory().ToMetricValues()
        : Enumerable.Empty<IMetricValue>()
      )
      .Concat(_gpuValues.Keys.Any(idSet.Contains)
        ? _hardwareMonitor.GetGraphics().SelectMany(g => g.ToMetricValues())
        : Enumerable.Empty<IMetricValue>()
      )
      .Concat(_systemValues.Keys.Any(idSet.Contains)
        ? _hardwareMonitor.GetSystem().ToMetricValues()
        : Enumerable.Empty<IMetricValue>()
      )
      .Where(m => idSet.Contains(m.Id))
    );
  }

  private IEnumerable<IMetricConvertible> GetAllStatic()
  {
    yield return _hardwareInfoCollector.GetSystem();
    foreach (var m in _hardwareInfoCollector.GetProcessors()) yield return m;
    foreach (var m in _hardwareInfoCollector.GetGraphics()) yield return m;
    foreach (var m in _hardwareInfoCollector.GetMemory()) yield return m;
  }

  public void Dispose()
  {
  }
}