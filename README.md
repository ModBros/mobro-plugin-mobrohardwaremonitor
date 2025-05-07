# mobro-plugin-mobrohardwaremonitor

![GitHub tag (latest by date)](https://img.shields.io/github/v/tag/ModBros/mobro-plugin-mobrohardwaremonitor?label=version)
![GitHub](https://img.shields.io/github/license/ModBros/mobro-plugin-mobrohardwaremonitor)
[![MoBro](https://img.shields.io/badge/-MoBro-red.svg)](https://mobro.app)
[![Discord](https://img.shields.io/discord/620204412706750466.svg?color=7389D8&labelColor=6A7EC2&logo=discord&logoColor=ffffff&style=flat-square)](https://discord.com/invite/DSNX4ds)

**Basic Hardware Data Plugin**

This plugin provides essential hardware metrics for [MoBro](https://mobro.app) that are standard on virtually every PC,
regardless of its specific hardware configuration.  
It does **not** offer hardware-specific details, such as individual core temperatures or loads.

All metrics are collected directly via Windows APIs or
the [LibreHardwareMonitorLib](https://www.nuget.org/packages/LibreHardwareMonitorLib/).

## Getting Started

Simply install the plugin in MoBro — no additional configuration required.

## Settings

This plugin offers customizable settings:

| Setting          | Default   | Description                                                                                                                     |
|------------------|-----------|---------------------------------------------------------------------------------------------------------------------------------|
| Update frequency | 1000 ms   | The interval (in milliseconds) for reading and updating metrics. Lower values provide faster updates but may increase CPU load. |
| Processor        | Enabled   | Toggle for including CPU metrics                                                                                                |
| Graphics card    | Enabled   | Toggle for including GPU metrics                                                                                                |
| Memory           | Enabled   | Toggle for including memory metrics                                                                                             |
| Processes        | 0         | Number of processes to monitor                                                                                                  |
| Processes sort   | CPU usage | Criteria to sort monitored processes                                                                                            |

## SDK

This plugin is built using the [MoBro Plugin SDK](https://github.com/ModBros/mobro-plugin-sdk).  
Developer documentation is available at [developer.mobro.app](https://developer.mobro.app).
