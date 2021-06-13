using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public class TesterHub : Hub
{
    public void BroadCastNotification(string notification)
    {
        Clients.All.spreadtheWord(notification);
    }
}
