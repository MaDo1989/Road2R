using Microsoft.AspNet.SignalR;

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

    public void BroadCast_driverAssigningorUpdate(UnityRide ur)
    {
        Clients.All.driverHasAssigned2UnityRide(ur);
    }
}