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

    public List<Ride> GetMyRides(int volunteerId)
    {
        string query = "select * from RidePatView where DriverId=" + volunteerId;
        DbService db = new DbService();
        DataSet ds = db.GetDataSetByQuery(query);
        Ride r = new Ride();
        List<Ride> rl = new List<Ride>();
        r.RidePats = new List<RidePat>();
        //List<RidePat> rpl = r.RidePats;
        bool RidePatexists;
        bool RideExists;
        foreach (DataRow dr in ds.Tables[0].Rows)
        {
            RideExists = false;
            foreach (Ride ride in rl)
            {
                if (ride.Id == int.Parse(dr["RideId"].ToString()))
                {
                    RideExists = true;

                    RidePatexists = false;
                    foreach (RidePat ridePat in ride.RidePats)
                    {
                        if (ridePat.RidePatNum == int.Parse(dr["ridePatNum"].ToString())) RidePatexists = true;
                        if (RidePatexists && dr["Escort"].ToString() != "")
                        {
                            Escorted es = new Escorted();
                            es.DisplayName = dr["Escort"].ToString();
                            ridePat.Pat.EscortedList.Add(es);

                            break;
                        }
                    }
                    if (RidePatexists) continue;
                    RidePat rp2 = new RidePat();
                    //ride.RidePats = new List<RidePat>();
                    rp2.RidePatNum = int.Parse(dr["ridePatNum"].ToString());
                    rp2.Pat = new Patient();
                    rp2.Pat.DisplayName = dr["patient"].ToString();
                    rp2.Pat.EscortedList = new List<Escorted>();
                    if (dr["Escort"].ToString() != "")
                    {
                        Escorted e = new Escorted();
                        e.DisplayName = dr["Escort"].ToString();
                        rp2.Pat.EscortedList.Add(e);
                    }

                    Destination origin = new Destination();
                    origin.Name = dr["RidePatOrigin"].ToString();
                    rp2.StartPlace = origin;
                    Destination dest = new Destination();
                    dest.Name = dr["RidePatDestination"].ToString();
                    rp2.Target = dest;
                    rp2.Area = dr["RidePatArea"].ToString();
                    rp2.Shift = dr["RidePatShift"].ToString();
                    rp2.Date = Convert.ToDateTime(dr["RidePatDate"].ToString());
                    ride.RidePats.Add(rp2);
                }
            }
            if (RideExists) continue;
            Ride r2 = new Ride();
            r2.Id = int.Parse(dr["RideID"].ToString());
            r2.Status = dr["statusRide"].ToString();
            RidePat rp = new RidePat();
            r2.RidePats = new List<RidePat>();
            rp.RidePatNum = int.Parse(dr["ridePatNum"].ToString());
            rp.Pat = new Patient();
            rp.Pat.DisplayName = dr["patient"].ToString();
            rp.Pat.EscortedList = new List<Escorted>();
            if (dr["Escort"].ToString() != "")
            {
                Escorted e = new Escorted();
                e.DisplayName = dr["Escort"].ToString();
                rp.Pat.EscortedList.Add(e);
            }

            Destination origin2 = new Destination();
            origin2.Name = dr["RidePatOrigin"].ToString();
            rp.StartPlace = origin2;
            Destination dest2 = new Destination();
            dest2.Name = dr["RidePatDestination"].ToString();
            rp.Target = dest2;
            rp.Area = dr["RidePatArea"].ToString();
            rp.Shift = dr["RidePatShift"].ToString();
            rp.Date = Convert.ToDateTime(dr["RidePatDate"].ToString());
            r2.RidePats.Add(rp);
            rl.Add(r2);
        }

        return rl;
    }
}