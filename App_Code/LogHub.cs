using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public class LogHub : Hub
{
    public static readonly System.Timers.Timer _Timer = new System.Timers.Timer();

    static LogHub()
    {
        _Timer.Interval = 5000;
        _Timer.Elapsed += TimerElapsed;
        _Timer.Start();
    }

    static void TimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
    {
        var hub = GlobalHost.ConnectionManager.GetHubContext("LogHub");
        hub.Clients.All.logMessage(string.Format("{0} - Still running", DateTime.UtcNow));
    }
}
