using System;
using System.Net.NetworkInformation;
using System.Threading;
using MoBro.Plugin.MoBroHardwareMonitor.Model.Stats;

namespace MoBro.Plugin.MoBroHardwareMonitor.DataCollectors;

internal sealed class NetworkMonitor
{
  private readonly string _pingHost;
  private readonly int _pingTimeout;
  private readonly Lock _lock = new();

  // only accessed from UpdateTraffic
  private long _lastBytesReceived;
  private long _lastBytesSent;

  // shared between UpdateTraffic, UpdatePing and GetNetwork (guarded by _lock)
  private DateTime _lastSampleTime;
  private double _downloadSpeed;
  private double _uploadSpeed;
  private TimeSpan? _pingLatency;

  public NetworkMonitor(string pingHost, int pingTimeout)
  {
    _pingHost = pingHost;
    _pingTimeout = pingTimeout;

    (_lastBytesReceived, _lastBytesSent) = GetTotalBytes();
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
    var (currentReceived, currentSent) = GetTotalBytes();

    lock (_lock)
    {
      var elapsed = (now - _lastSampleTime).TotalSeconds;
      if (elapsed > 0)
      {
        // interfaces going up/down or counter resets can make the delta negative
        _downloadSpeed = Math.Max(0, currentReceived - _lastBytesReceived) / elapsed * 8;
        _uploadSpeed = Math.Max(0, currentSent - _lastBytesSent) / elapsed * 8;
      }

      _lastSampleTime = now;
    }

    _lastBytesReceived = currentReceived;
    _lastBytesSent = currentSent;
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

  private static (long received, long sent) GetTotalBytes()
  {
    long received = 0;
    long sent = 0;
    foreach (var nic in NetworkInterface.GetAllNetworkInterfaces())
    {
      if (nic.OperationalStatus != OperationalStatus.Up) continue;
      if (nic.NetworkInterfaceType == NetworkInterfaceType.Loopback) continue;
      var stats = nic.GetIPv4Statistics();
      received += stats.BytesReceived;
      sent += stats.BytesSent;
    }

    return (received, sent);
  }
}