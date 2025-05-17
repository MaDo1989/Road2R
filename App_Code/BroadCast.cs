using Microsoft.AspNet.SignalR;
using System;

/// <summary>
/// Summary description for BroadCast
/// </summary>
public class BroadCast
{
    public static void BroadCast2Clients_driverHasAssigned2RidePat(RidePat rp)
    {
        if (ShouldClientsBeUpdated(rp.Date, 30))
        {
            IHubContext hubContext = GlobalHost.ConnectionManager.GetHubContext<RidePatHub>();
            hubContext.Clients.All.driverHasAssigned2RidePat(rp);
        }
    }


    public static void BroadCast2Clients_driverHasAssigned2UnityRide(UnityRide ur)
    {
        if (ShouldClientsBeUpdated(ur.PickupTime, 30))
        {
            IHubContext hubContext = GlobalHost.ConnectionManager.GetHubContext<UnityRideHub>();
            hubContext.Clients.All.driverHasAssigned2UnityRide(ur);
        }
    }



    public static void BroadCast2Clients_driverHasRemovedFromRidePat(RidePat rp)
    {
        if (ShouldClientsBeUpdated(rp.Date, 30))
        {
            IHubContext hubContext = GlobalHost.ConnectionManager.GetHubContext<RidePatHub>();
            hubContext.Clients.All.driverHasRemovedFromRidePat(rp);
        }
    }

    public static void BroadCast2Clients_ridePatUpdated(RidePat rp)
    {
        if (ShouldClientsBeUpdated(rp.Date, 30))
        {
            IHubContext hubContext = GlobalHost.ConnectionManager.GetHubContext<RidePatHub>();
            hubContext.Clients.All.ridePatUpdated(rp);
        }
    }
    public static void BroadCast2Clients_UnityRideUpdated(UnityRide ur)
    {
        if (ShouldClientsBeUpdated(ur.PickupTime, 30))
        {
            IHubContext hubContext = GlobalHost.ConnectionManager.GetHubContext<UnityRideHub>();
            hubContext.Clients.All.UnityRideUpdated(ur);
        }
    }

    private static bool ShouldClientsBeUpdated(DateTime rpDate, int maxNumOfDays)
    {
        bool shouldClientsBeUpdated = (rpDate - DateTime.Now).Days <= maxNumOfDays;

        return shouldClientsBeUpdated;
    }
}