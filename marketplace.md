# MoBro Hardware Monitor

The universal monitoring plugin for every PC.  
This plugin provides a standardized set of essential hardware metrics that work across virtually any configuration,
ensuring you always have your core stats at your fingertips.

---

## Metrics

- **CPU**: Name, manufacturer, core counts, utilization, and temperatures.
- **GPU**: Full specs including driver version, memory usage, utilization, and thermal data.
- **Memory**: Capacity, manufacturer, frequency, and real-time RAM utilization.
- **System**: Detailed OS info, hostname, architecture, and current system time.
- **Process Monitoring**: Track top running processes by CPU or Memory usage.

---

## Setup

**Zero configuration required.**

1. **Install** via the MoBro Marketplace.
2. Ensure that PawnIO is installed (automatically included with MoBro).
3. The plugin will automatically begin querying your hardware and providing metrics.
4. Add your desired metrics to your MoBro dashboard.

**Note:** This plugin focuses on essential "standard" metrics.  
For highly detailed per-core data or specific motherboard sensors, check out our **LibreHardwareMonitor** plugin.

---

## Settings

Fine-tune the monitoring to balance performance and detail.

| Setting                 | Default   | Description                                                       |
|:------------------------|:----------|:------------------------------------------------------------------|
| **Update Frequency**    | `1000ms`  | How often metrics are refreshed. Lower values = smoother updates. |
| **Monitored Processes** | `0`       | Set the number of top processes to track (e.g., top 5).           |
| **Process Sorting**     | `CPU`     | Choose to sort processes by CPU usage or Memory usage.            |
| **Hardware Toggles**    | `Enabled` | Individually enable/disable CPU, GPU, or RAM monitoring.          |