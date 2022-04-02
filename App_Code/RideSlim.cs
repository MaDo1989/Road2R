using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;

/// <summary>
/// Summary description for RideSlim
/// </summary>
public class RideSlim
{

    string patientName;
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

    public RideSlim(string patientName, string driverName, int driverId, string origin, string destination, DateTime pickUpTime, int id)
    {
        this.patientName = patientName;
        this.driverName = driverName;
        this.driverId = driverId;
        this.origin = origin;
        this.destination = destination;
        this.pickUpTime = pickUpTime;
        this.id = id;
    }

    public List<RideSlim> GetPastRides(int volunteerID) {

        List<RideSlim> pastRides = new List<RideSlim>();

        DbService db = new DbService();
        SqlCommand cmd = new SqlCommand();
        cmd.CommandType = CommandType.Text;
        cmd.Parameters.AddWithValue("@volunteerId", volunteerID);
        cmd.CommandText = "exec spRideAndRidePat_GetVolunteersRideHistory volunteerID = @volunteerId";
        SqlDataReader dr = db.GetDataReader(cmd);
        while (dr.Read()) {
            Id = Convert.ToInt32(dr["id"]);
            Destination = dr["destination"].ToString();
            Origin = dr["origin"].ToString();
            PickUpTime = Convert.ToDateTime(dr["PickUpTime"].ToString());
            patientName = dr["patient"].ToString();
            pastRides.Add(new RideSlim(patientName, "", volunteerID, origin, destination, PickUpTime, id));
        }
        return pastRides;
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
}