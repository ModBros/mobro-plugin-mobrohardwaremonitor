Basic hardware data plugin for MoBro.

This plugin provides standard hardware metrics available on every PC regardless of the actual hardware.  
This plugin does not provide hardware specific metrics like individual core temperatures, loads etc.

All metrics are read directly from APIs available within Windows or
the [LibreHardwareMonitorLib](https://www.nuget.org/packages/LibreHardwareMonitorLib/).

# Setup

Just install the plugin, and you're good to go.  
No further setup required.

# Metrics

Provides the following metrics:

## Processor

- Name - The name of the systems CPU
- Manufacturer - The manufacturer of the CPU
- Cores - The number of available cores
- Logical processors - The number of logical processors
- Max. clock speed - The maximum clock speed for this CPU
- Power usage - The current power usage of the processor
- Temperature - The current temperature of the processor
- Utilization - The total utilization of the processor

## Graphics card

- Name - The name of the GPU
- Manufacturer -The manufacturer of the GPU
- Driver - The currently installed driver
- Refresh rate - The current refresh rate of the display
- Horizontal resolution - The current horizontal resolution of the display
- Vertical resolution - The current vertical resolution of the display
- Core utilization - Current core utilization
- Memory utilization - Current memory utilization
- Power usage - The current power usage
- Temperature - The current temperature

## Memory

- Capacity - The capacity of this memory module
- Manufacturer - The manufacturer
- Frequency - The current frequency of this memory module
- Total capacity - The total memory capacity
- Available - Available memory
- Used - Memory currently in use
- Utilization - Current memory utilization

## System

- Name - The name of the operating system
- Version - The specific version of the operating system
- Type - The type of the operating system (32bit/64bit)
- Architecture - The platform architecture
- User - The user currently logged in
- Hostname - The hostname of the system
- System time - The current date and time of the system

## Processes

- Process name - The name of the process
- Cpu usage - The cpu usage of the process
- Memory usage - The memory usage of the process

# Settings

This plugin exposes the following settings:

| Setting          | Default   | Explanation                                                                                                                                                         |
|------------------|-----------|---------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| Update frequency | 1000 ms   | The frequency (in milliseconds) at which to read and update metrics from shared memory. Lower values will update metrics more frequently but may increase CPU load. |
| Processor        | enabled   | Whether to include CPU metrics                                                                                                                                      |
| Graphics card    | enabled   | Whether to include GPU metrics                                                                                                                                      |
| Memory           | enabled   | Whether to include memory metrics                                                                                                                                   |
| Processes        | 0         | How many processes to record                                                                                                                                        |
| Processes sort   | CPU usage | How to sort the recorded processes                                                                                                                                  |

