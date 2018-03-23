using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for Ride
/// </summary>
public class Ride
{
    public Ride()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public int Id { get; set; }

    public List<RidePat> RidePats { get; set; }

    public List<Volunteer> Drivers { get; set; }

    public string Status { get; set; }

    public List<Ride> GetRidesForNotify()
    {
        string query = "select * from RideViewForNotify where statusRide='שובץ נהג' or statusRide='מלאה'";
        DbService db = new DbService();
        DataSet ds = db.GetDataSetByQuery(query);
        List<Ride> rides = new List<Ride>();
        RidePats = new List<RidePat>();
        bool RidePatexists;
        bool RideExists;
        foreach (DataRow dr in ds.Tables[0].Rows)
        {

            RideExists = false;

            foreach (Ride ride in rides)
            {
                RidePatexists = false;
                if (ride.Id == int.Parse(dr["RideNum"].ToString()))
                {
                    RideExists = true;
                    foreach (RidePat ridepat in ride.RidePats)
                    {
                        if (ridepat.RidePatNum==int.Parse(dr["ridePatNum"].ToString()))
                        {
                            RidePatexists = true;
                            break;
                        }
                    }
                    if (ride.Drivers.Count != 2)
                    {
                        if (ride.Drivers[0].DisplayName != dr["DriverName"].ToString())
                        {
                            Volunteer d = new Volunteer();
                            d.DisplayName = dr["DriverName"].ToString();
                            d.DriverType = dr["DriverPosition"].ToString();
                            d.CellPhone = dr["DriverCell"].ToString();
                            ride.Drivers.Add(d);
                        }
                    }
                    if (RidePatexists) continue;
                    RidePat rp = new RidePat();
                    rp.RidePatNum = int.Parse(dr["ridePatNum"].ToString());
                    rp.Pat = new Patient();
                    rp.Pat.DisplayName = dr["patient"].ToString();
                    // rp.Pat.EscortedList = new List<Escorted>();
                    Destination o = new Destination();
                    o.Name = dr["Origin"].ToString();
                    rp.StartPlace = o;
                    Destination destination = new Destination();
                    destination.Name = dr["Destination"].ToString();
                    rp.Target = destination;
                    //rp.Area = dr["RidePatArea"].ToString();
                    //rp.Shift = dr["RidePatShift"].ToString();
                    rp.Date = Convert.ToDateTime(dr["dateRide"].ToString());
                    rp.Pat.CellPhone = dr["PatientCell"].ToString();
                    
                    ride.RidePats.Add(rp);
                    
                }
            }
            if (RideExists) continue;
            Ride r = new Ride();
            r.Status = dr["statusRide"].ToString();
            r.Drivers = new List<Volunteer>();
            Volunteer driver = new Volunteer();
            driver.DisplayName = dr["DriverName"].ToString();
            driver.DriverType = dr["DriverPosition"].ToString();
            driver.CellPhone = dr["DriverCell"].ToString();
            r.Drivers.Add(driver);
            r.Id = int.Parse(dr["RideNum"].ToString());
            RidePat ridePat = new RidePat();
            ridePat.RidePatNum = int.Parse(dr["ridePatNum"].ToString());
            ridePat.Pat = new Patient();
            ridePat.Pat.CellPhone = dr["PatientCell"].ToString();
            ridePat.Pat.DisplayName = dr["patient"].ToString();
            // ridePat.Pat.EscortedList = new List<Escorted>();
            Destination origin = new Destination();
            origin.Name = dr["Origin"].ToString();
            ridePat.StartPlace = origin;
            Destination dest = new Destination();
            dest.Name = dr["Destination"].ToString();
            ridePat.Target = dest;
            //ridePat.Area = dr["RidePatArea"].ToString();
            //ridePat.Shift = dr["RidePatShift"].ToString();
            ridePat.Date = Convert.ToDateTime(dr["dateRide"].ToString());
            r.RidePats = new List<RidePat>();
            r.RidePats.Add(ridePat);
            rides.Add(r);
        }

        return rides;
    }
}