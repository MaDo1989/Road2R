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
public class UnityRideHub : Hub
{
    public void BroadCast_UnityUpdated(UnityRide ur)
    {
       Clients.All.UnityRideUpdated(ur);
    }

}