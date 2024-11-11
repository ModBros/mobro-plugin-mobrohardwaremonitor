using System;
using System.Collections.Generic;
using MoBro.Plugin.MoBroHardwareMonitor;
using MoBro.Plugin.SDK;
using Serilog.Events;

using var plugin = MoBroPluginBuilder
  .Create<Plugin>()
  .WithLogLevel(LogEventLevel.Debug)
  .WithSettings(new Dictionary<string, string>
  {
    ["update_frequency"] = "10000",
    ["cpu_metrics"] = "true",
    ["gpu_metrics"] = "true",
    ["ram_metrics"] = "true",
    ["num_processes"] = "0",
    ["processes_sort"] = "cpu"
  })
  .Build();

Console.ReadLine();