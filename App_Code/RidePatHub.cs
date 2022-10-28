using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
/*
     ===============================
     !!! PAY VERY GOOD ATTENTION !!!
     !!! CHANGES HERE MUST TAKE  !!!
     !!!   PLACE ON RELEVANT     !!!
     !!! PLACES ON CLIENT SIDE   !!!
     ===============================
*/
public class RidePatHub : Hub
{
    public void BroadCast_driverAssigning(RidePat rp)
    {
        Clients.All.driverHasAssigned2RidePat(rp);
    }

    public void BroadCast_driverRemoval(RidePat rp)
    {
        Clients.All.driverHasRemovedFromRidePat(rp);
    }

    public void BroadCast_ridePatUpdated(RidePat rp)
    {
        Clients.All.ridePatUpdated(rp);
    }
}
