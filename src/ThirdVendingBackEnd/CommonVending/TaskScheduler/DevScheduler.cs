using System;
using System.Timers;
using CommonVending.Services;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace CommonVending.TaskScheduler
{
  public class DevScheduler : IDisposable
  {
    private const long MinuteInHour = 60;
    private const long MillisecondInMinute = 60 * 1000;
    private const long TicksInMillisecond = 10000;
    private const long TicksInHour = MinuteInHour * MillisecondInMinute * TicksInMillisecond;
    private long _nextIntervalTick;
    private Timer _timer;
    private readonly IEmailSender _sender = new AuthMessageSender();

    /// <inheritdoc />
    public void Dispose() { Stop(); }

    public void Start()
    {
      _timer = new Timer();
      _timer.Elapsed += TimerOnElapsed;
      _timer.AutoReset = false;
      _timer.Interval = GetInitialInterval();
      _timer.Start();
    }

    public void Stop()
    {
      if (_timer != null)
      {
        _timer.Elapsed -= TimerOnElapsed;
        _timer.Stop();
        _timer.Dispose();
      }
    }

    private double GetInitialInterval()
    {
      var now = DateTime.Now;
      double timeToNextHour = (((60 - now.Hour) * 1000 * 60) - ((60 - now.Second) * 1000) - now.Millisecond) + 15;
      _nextIntervalTick = now.Ticks + ((long) timeToNextHour * TicksInMillisecond);
      if ((timeToNextHour > 0) && (timeToNextHour < int.MaxValue)) return timeToNextHour;

      return MillisecondInMinute;
    }

    private double GetInterval()
    {
      try
      {
        _nextIntervalTick += TicksInHour;
        var interval = TicksToMs(_nextIntervalTick - DateTime.Now.Ticks);
        if ((interval > 0) && (interval < int.MaxValue)) return interval;

        return GetInitialInterval();
      }
      catch (Exception) { return GetInitialInterval(); }
    }

    private double TicksToMs(long ticks) { return ticks / (double) TicksInMillisecond; }
    
    private void TimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
    {
      _timer.Interval = GetInterval();
      _timer.Start();

      var dateNow = DateTime.Now;
      if (dateNow.Hour == 0)
      {
        var devices = DbProvider.DeviceDbProvider.GetDevices();
        foreach (var device in devices)
        {
          _sender.SendReport(device).Wait();
        }
      }
    }
  }
}
