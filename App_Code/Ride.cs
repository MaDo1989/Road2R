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

    public Destination Origin { get; set; }

    public Destination Destination { get; set; }

    public List<RidePat> RidePats { get; set; }

    public List<Volunteer> Drivers { get; set; }

    public string Status { get; set; }

    public List<Ride> GetRidesForNotifyfull()
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
                        if (ridepat.RidePatNum == int.Parse(dr["ridePatNum"].ToString()))
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

    public List<Ride> GetRidesForNotify()
    {
        List<Ride> lr = new List<Ride>();

        //Ride 1
        Ride r1 = new Ride();
        r1.Origin = new Destination();
        r1.Origin.Name = "ארז";
        r1.Destination = new Destination();
        r1.Destination.Name = "הלל יפה";
        r1.RidePats = new List<RidePat>();
        r1.Drivers = new List<Volunteer>();
        
        

        //Volunteers-Drivers
        Volunteer v = new Volunteer();
        v.DisplayName = "אסף סלוק";
        v.CellPhone = "052-1111111";
        v.DriverType = "ראשי";
        v.pnRegId = "aaaaa";
        r1.Drivers.Add(v);
        Volunteer v2 = new Volunteer();
        v2.DisplayName = "מתן דורון";
        v2.CellPhone = "052-2222222";
        v2.DriverType = "גיבוי";
        v2.pnRegId = "mmmmm";
        r1.Drivers.Add(v2);

       
        r1.RidePats = new List<RidePat>();

        //RidePat
        RidePat rp = new RidePat();
        rp.Date = new DateTime(2018,4,1,6,0,0);
        rp.StartPlace = new Destination();
        rp.StartPlace.Name = "ארז";
        rp.Target = new Destination();
        rp.Target.Name = "הלל יפה";
        rp.Coordinator = new Volunteer();
        rp.Coordinator.DisplayName = "בני בורנפלד";
        rp.Coordinator.CellPhone = "050-0000000";
        rp.Coordinator.pnRegId = "bbbbb";

        //Patient
        rp.Pat = new Patient();
        rp.Pat.DisplayName = "וואליד באדיר";
        rp.Pat.CellPhone = "050-9999999";
        rp.Pat.Id = 1;
        rp.Pat.pnRegId = "wwwww";

        //Escort
        rp.Pat.EscortedList = new List<Escorted>();
        Escorted e = new Escorted();
        e.DisplayName = "מוחמד גאדיר";
        e.CellPhone = "050-8888888";

        rp.Pat.EscortedList.Add(e);

        //Equipment
        rp.Pat.Equipment = new List<string>();
        rp.Pat.Equipment.Add("כסא גלגלים");
        rp.Pat.Equipment.Add("קביים");

        r1.RidePats.Add(rp);
        lr.Add(r1);

        //Ride 2
        Ride r2 = new Ride();
        r2.Origin = new Destination();
        r2.Origin.Name = "ג'למה";
        r2.Destination = new Destination();
        r2.Destination.Name = "רמבם";
        r2.RidePats = new List<RidePat>();
        r2.Drivers = new List<Volunteer>();

        //Volunteers-Drivers
        Volunteer vv = new Volunteer();
        vv.DisplayName = "מסי";
        vv.CellPhone = "052-3333333";
        vv.DriverType = "ראשי";
        vv.pnRegId = "lllll";
        r2.Drivers.Add(vv);
        Volunteer vv2 = new Volunteer();
        vv2.DisplayName = "רונאלדו";
        vv2.CellPhone = "052-4444444";
        vv2.DriverType = "גיבוי";
        vv2.pnRegId = "cr7";
        r2.Drivers.Add(vv2);


        r2.RidePats = new List<RidePat>();
        
        //RidePat 1
        RidePat rp1 = new RidePat();
        rp1.Date = new DateTime(2018, 4, 5, 11, 0, 0);
        rp1.StartPlace = new Destination();
        rp1.StartPlace.Name = "ג'למה";
        rp1.Target = new Destination();
        rp1.Target.Name = "רמבם";
        rp1.Coordinator = new Volunteer();
        rp1.Coordinator.DisplayName = "בני בורנפלד";
        rp1.Coordinator.CellPhone = "050-0000000";
        rp1.Coordinator.pnRegId = "bbbbb";

        //Patient
        rp1.Pat = new Patient();
        rp1.Pat.DisplayName = "אדיר מילר";
        rp1.Pat.CellPhone = "050-7777777";
        rp1.Pat.Id = 2;
        rp1.Pat.pnRegId = "amama";

        //Escort
        rp1.Pat.EscortedList = new List<Escorted>();
        Escorted ee = new Escorted();
        ee.DisplayName = "שחר חסון";
        ee.CellPhone = "050-6666666";
       

        rp1.Pat.EscortedList = new List<Escorted>();
        Escorted ee2 = new Escorted();
        ee2.DisplayName = "קובי מימון";
        ee2.CellPhone = "050-5555555";
        

        rp1.Pat.EscortedList.Add(ee);
        rp1.Pat.EscortedList.Add(ee2);

        //Equipment
        rp1.Pat.Equipment = new List<string>();
        rp1.Pat.Equipment.Add("בוסטר");

        r2.RidePats.Add(rp1);

        //RidePat 2
        RidePat rp2 = new RidePat();
        rp2.Date = new DateTime(2018, 4, 5, 11, 0, 0);
        rp2.StartPlace = new Destination();
        rp2.StartPlace.Name = "ג'למה";
        rp2.Target = new Destination();
        rp2.Target.Name = "רמבם";
        rp2.Coordinator = new Volunteer();
        rp2.Coordinator.DisplayName = "בני בורנפלד";
        rp2.Coordinator.CellPhone = "050-0000000";
        rp2.Coordinator.pnRegId = "bbbbb";

        //Patient
        rp2.Pat = new Patient();
        rp2.Pat.DisplayName = "שלמה ארצי";
        rp2.Pat.CellPhone = "050-1234567";
        rp2.Pat.Id = 3;
        rp2.Pat.pnRegId = "sasas";

        //Escort
        rp2.Pat.EscortedList = new List<Escorted>();
        Escorted ee3 = new Escorted();
        ee3.DisplayName = "אבי סינגולדה";
        ee3.CellPhone = "050-0987654";

        rp2.Pat.EscortedList = new List<Escorted>();
        Escorted ee4 = new Escorted();
        ee4.DisplayName = "מאיר ישראל";
        ee4.CellPhone = "050-6758493";

        rp2.Pat.EscortedList.Add(ee3);
        rp2.Pat.EscortedList.Add(ee4);
        

        
        r2.RidePats.Add(rp2);

        lr.Add(r2);

        return lr;
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