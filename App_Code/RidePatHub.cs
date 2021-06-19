using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public class RidePatHub : Hub
{
    public void BroadCast_driverAssigning(RidePat rp)
    {
        Clients.All.ridePatHasUpdated(rp);
    }

}
