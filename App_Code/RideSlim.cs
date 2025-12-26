using System;

/// <summary>
/// Summary description for RideSlim
/// </summary>
public class RideSlim
{

    string patientName;
    string cellPhone;
    string driverName;
    int driverId;
    string origin;
    string destination;
    DateTime pickUpTime;
    int id;

    public RideSlim()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public RideSlim(string patientName, string driverName, int driverId, string origin, string destination, DateTime pickUpTime, int id, string cellPhone)
    {
        this.patientName = patientName;
        this.driverName = driverName;
        this.driverId = driverId;
        this.origin = origin;
        this.destination = destination;
        this.pickUpTime = pickUpTime;
        this.id = id;
        this.cellPhone = cellPhone;
    }

    public string PatientName
    {
        get
        {
            return patientName;
        }

        set
        {
            patientName = value;
        }
    }

    public string DriverName
    {
        get
        {
            return driverName;
        }

        set
        {
            driverName = value;
        }
    }

    public int DriverId
    {
        get
        {
            return driverId;
        }

        set
        {
            driverId = value;
        }
    }

    public string Origin
    {
        get
        {
            return origin;
        }

        set
        {
            origin = value;
        }
    }

    public string Destination
    {
        get
        {
            return destination;
        }

        set
        {
            destination = value;
        }
    }

    public DateTime PickUpTime
    {
        get
        {
            return pickUpTime;
        }

        set
        {
            pickUpTime = value;
        }
    }

    public int Id
    {
        get
        {
            return id;
        }

        set
        {
            id = value;
        }
    }

    public string CellPhone
    {
        get
        {
            return cellPhone;
        }

        set
        {
            cellPhone = value;
        }
    }
}