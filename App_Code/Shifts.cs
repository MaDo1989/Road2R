using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for Shifts
/// </summary>
public class Shifts
{
    int driverID;
    int truckID;
    List<int> driverShiftIDs;

    public int DriverID
    {
        get
        {
            return driverID;
        }

        set
        {
            driverID = value;
        }
    }



    public List<int> DriverShiftIDs
    {
        get
        {
            return driverShiftIDs;
        }

        set
        {
            driverShiftIDs = value;
        }
    }

    public int TruckID
    {
        get
        {
            return truckID;
        }

        set
        {
            truckID = value;
        }
    }

    public Shifts()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public Shifts(int driverID, int truckID, List<int> driverShiftIDs)
    {
       DriverID = driverID;
       TruckID = truckID;
       DriverShiftIDs = driverShiftIDs;
    }
}