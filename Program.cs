using System;
using System.Collections.Generic;
using MoBro.Plugin.MoBroHardwareMonitor;
using MoBro.Plugin.SDK;
using Serilog.Events;

var plugin = MoBroPluginBuilder
  .Create<MoBroHardwareMonitor>()
  .WithLogLevel(LogEventLevel.Debug)
  .WithSettings(new Dictionary<string, string>
  {
    ["cpu_metrics"] = "true",
    ["gpu_metrics"] = "true",
    ["ram_metrics"] = "true",
    ["num_processes"] = "5",
    ["processes_sort"] = "cpu"
  })
  .Build();

Console.ReadLine();