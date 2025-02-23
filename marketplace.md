Basic Hardware Data Plugin

This plugin provides essential hardware metrics that are standard on virtually every PC, regardless of its specific
hardware configuration.  
It does **not** offer hardware-specific details, such as individual core temperatures or loads.

All metrics are collected directly via Windows APIs or
the [LibreHardwareMonitorLib](https://www.nuget.org/packages/LibreHardwareMonitorLib/).

---

# Setup

Simply install the plugin, and you're all set — no additional configuration required.

---

# Metrics Overview

Provides the following metrics:

## Processor

- **Name**: The name of the system's CPU
- **Manufacturer**: The CPU manufacturer
- **Cores**: Number of physical cores
- **Logical processors**: Number of logical processors
- **Max. clock speed**: The CPU’s maximum clock speed
- **Power usage**: Current power consumption of the CPU
- **Temperature**: Current CPU temperature
- **Utilization**: Total CPU utilization

## Graphics Card

- **Name**: GPU name
- **Manufacturer**: GPU manufacturer
- **Driver**: Installed GPU driver
- **Refresh rate**: Current screen refresh rate
- **Horizontal resolution**: Display's horizontal resolution
- **Vertical resolution**: Display's vertical resolution
- **Core utilization**: GPU core usage percentage
- **Memory utilization**: GPU memory usage percentage
- **Power usage**: GPU’s current power consumption
- **Temperature**: Current GPU temperature
- **Used memory**: Memory currently in use
- **Total memory capacity**: GPU’s total memory capacity
- **Available memory**: GPU memory currently available

## Memory

- **Capacity**: The capacity of an individual memory module
- **Manufacturer**: Memory manufacturer
- **Frequency**: Current frequency of the memory
- **Total capacity**: Total memory available in the system
- **Available**: Unused memory
- **Used**: Memory currently being utilized
- **Utilization**: Percentage of memory in use

## System

- **Name**: Operating system name
- **Version**: Specific version of the operating system
- **Type**: Operating system type (32-bit/64-bit)
- **Architecture**: Platform architecture
- **User**: Currently logged-in user
- **Hostname**: System hostname
- **System time**: Current date and time on the system

## Processes

- **Process name**: Name of the running process
- **CPU usage**: CPU usage percentage of the process
- **Memory usage**: Memory usage of the process

---

# Settings

This plugin offers customizable settings:

| Setting          | Default   | Description                                                                                                                     |
|------------------|-----------|---------------------------------------------------------------------------------------------------------------------------------|
| Update frequency | 1000 ms   | The interval (in milliseconds) for reading and updating metrics. Lower values provide faster updates but may increase CPU load. |
| Processor        | Enabled   | Toggle for including CPU metrics                                                                                                |
| Graphics card    | Enabled   | Toggle for including GPU metrics                                                                                                |
| Memory           | Enabled   | Toggle for including memory metrics                                                                                             |
| Processes        | 0         | Number of processes to monitor                                                                                                  |
| Processes sort   | CPU usage | Criteria to sort monitored processes                                                                                            |
