using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for BroadCast
/// </summary>
public class BroadCast
{
    public static void BroadCast2Clients_driverHasAssigned2RidePat(RidePat rp)
    {
        if ((rp.Date - DateTime.Now).Days <= 30)
        {//case in this month → inform clients on manageRidPats.html
            IHubContext hubContext = GlobalHost.ConnectionManager.GetHubContext<RidePatHub>();
            hubContext.Clients.All.driverHasAssigned2RidePat(rp);
        }
    }

    public static void BroadCast2Clients_driverHasRemovedFromRidePat(RidePat rp)
    {
        if ((rp.Date - DateTime.Now).Days <= 30)
        {//case in this month → inform clients on manageRidPats.html
            IHubContext hubContext = GlobalHost.ConnectionManager.GetHubContext<RidePatHub>();
            hubContext.Clients.All.driverHasRemovedFromRidePat(rp);
        }
    }

}