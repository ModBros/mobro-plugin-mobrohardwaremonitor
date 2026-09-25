using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Threading;
using MoBro.Plugin.MoBroHardwareMonitor.Model.Stats;

namespace MoBro.Plugin.MoBroHardwareMonitor.DataCollectors;

internal sealed class NetworkMonitor
{
  private readonly string _pingHost;
  private readonly int _pingTimeout;
  private readonly Lock _lock = new();

  // only accessed from UpdateTraffic (keyed by interface id)
  private Dictionary<string, (long Received, long Sent)> _lastBytes;

  // shared between UpdateTraffic, UpdatePing and GetNetwork (guarded by _lock)
  private DateTime _lastSampleTime;
  private double _downloadSpeed;
  private double _uploadSpeed;
  private TimeSpan? _pingLatency;

  public NetworkMonitor(string pingHost, int pingTimeout)
  {
    _pingHost = pingHost;
    _pingTimeout = pingTimeout;

    _lastBytes = GetBytesPerInterface();
    _lastSampleTime = DateTime.UtcNow;
  }

  public NetworkStats GetNetwork()
  {
    lock (_lock)
    {
      return new NetworkStats(_lastSampleTime, _pingHost, _pingLatency, _downloadSpeed, _uploadSpeed);
    }
  }

  public void UpdateTraffic()
  {
    var now = DateTime.UtcNow;
    var currentBytes = GetBytesPerInterface();

    // sum up the deltas per interface, so interfaces coming up or going down between samples
    // don't cause spikes, and counter resets don't produce negative values
    long receivedDelta = 0;
    long sentDelta = 0;
    foreach (var (id, current) in currentBytes)
    {
      if (!_lastBytes.TryGetValue(id, out var last)) continue;
      receivedDelta += Math.Max(0, current.Received - last.Received);
      sentDelta += Math.Max(0, current.Sent - last.Sent);
    }

    lock (_lock)
    {
      var elapsed = (now - _lastSampleTime).TotalSeconds;
      if (elapsed > 0)
      {
        _downloadSpeed = receivedDelta / elapsed * 8;
        _uploadSpeed = sentDelta / elapsed * 8;
      }

      _lastSampleTime = now;
    }

    _lastBytes = currentBytes;
  }

  public void UpdatePing()
  {
    TimeSpan? pingLatency;
    try
    {
      using var ping = new Ping();
      var reply = ping.Send(_pingHost, _pingTimeout);
      if (reply.Status == IPStatus.Success)
      {
        pingLatency = TimeSpan.FromMilliseconds(reply.RoundtripTime);
      }
      else
      {
        pingLatency = null;
      }
    }
    catch (Exception)
    {
      pingLatency = null;
    }

    lock (_lock)
    {
      _pingLatency = pingLatency;
    }
  }

  private static Dictionary<string, (long Received, long Sent)> GetBytesPerInterface()
  {
    var bytes = new Dictionary<string, (long Received, long Sent)>();
    foreach (var nic in NetworkInterface.GetAllNetworkInterfaces())
    {
      if (nic.OperationalStatus != OperationalStatus.Up) continue;
      if (nic.NetworkInterfaceType == NetworkInterfaceType.Loopback) continue;
      // skip NDIS filter pseudo-interfaces (e.g. "Ethernet-QoS Packet Scheduler-0000") that have no IP stack bound
      // but report the same counters as the adapter they are attached to (would count traffic multiple times)
      if (!nic.Supports(NetworkInterfaceComponent.IPv4) && !nic.Supports(NetworkInterfaceComponent.IPv6)) continue;
      var stats = nic.GetIPStatistics();
      bytes[nic.Id] = (stats.BytesReceived, stats.BytesSent);
    }

    return bytes;
  }
}
