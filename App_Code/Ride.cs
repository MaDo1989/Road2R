using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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

    public Location Origin { get; set; }

    public Location Destination { get; set; }

    public List<RidePat> RidePats { get; set; }

    public List<Volunteer> Drivers { get; set; }

    public string Status { get; set; }

    public string DriverType { get; set; }

    public List<string> Statuses { get; set; }

    string query;
    DbService dbs;
    SqlCommand cmd;
    RidePat ridepat;
    public List<Ride> GetRidesForNotifyfull()
    {
        string query = "select * from RideViewForNotify where statusRide=N'שובץ נהג' or statusRide=N'מלאה'";
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
                        if (ridepat.RidePatNum == int.Parse(dr["RidePatNum"].ToString()))
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
                    rp.RidePatNum = int.Parse(dr["RidePatNum"].ToString());
                    rp.Pat = new Patient();
                    rp.Pat.DisplayName = dr["Patient"].ToString();
                    // rp.Pat.EscortedList = new List<Escorted>();
                    Location o = new Location();
                    o.Name = dr["Origin"].ToString();
                    rp.Origin = o;
                    Location destination = new Location();
                    destination.Name = dr["Destination"].ToString();
                    rp.Destination = destination;
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
            ridePat.RidePatNum = int.Parse(dr["RidePatNum"].ToString());
            ridePat.Pat = new Patient();
            ridePat.Pat.CellPhone = dr["PatientCell"].ToString();
            ridePat.Pat.DisplayName = dr["Patient"].ToString();
            // ridePat.Pat.EscortedList = new List<Escorted>();
            Location origin = new Location();
            origin.Name = dr["Origin"].ToString();
            ridePat.Origin = origin;
            Location dest = new Location();
            dest.Name = dr["Destination"].ToString();
            ridePat.Destination = dest;
            //ridePat.Area = dr["RidePatArea"].ToString();
            //ridePat.Shift = dr["RidePatShift"].ToString();
            ridePat.Date = Convert.ToDateTime(dr["dateRide"].ToString());
            r.RidePats = new List<RidePat>();
            r.RidePats.Add(ridePat);
            rides.Add(r);
        }

        return rides;
    }

    internal bool isPrimaryStillCanceled(int rideID, int driverID)
    {
        string query = "select * from Ride where RideNum = " + rideID + " AND SecondaryDriver = " + driverID + " AND MainDriver is null";
        DbService db = new DbService();
        DataSet ds = db.GetDataSetByQuery(query);

        if (ds.Tables[0].Rows[0].IsNull(0)) return false;
        else return true;
    }

    internal int backupToPrimary(int rideID, int driverID)
    {
        SqlCommand cmd = new SqlCommand();
        cmd.CommandType = CommandType.Text;
        SqlParameter[] cmdParams = new SqlParameter[2];
        cmdParams[0] = cmd.Parameters.AddWithValue("@rideID", rideID);
        cmdParams[1] = cmd.Parameters.AddWithValue("@driverID", driverID);
        string query = "update Ride set SecondaryDriver = null, MainDriver = @driverID where RideNum = @rideID";
        DbService db = new DbService();
        return db.ExecuteQuery(query, cmd.CommandType, cmdParams);
    }

    public int setStatus(int rideId, string status)
    {
        SqlCommand cmd = new SqlCommand();
        cmd.CommandType = CommandType.Text;
        SqlParameter[] cmdParams = new SqlParameter[3];
        cmdParams[0] = cmd.Parameters.AddWithValue("@rideId", rideId);
        cmdParams[1] = cmd.Parameters.AddWithValue("@status", status);
        cmdParams[2] = cmd.Parameters.AddWithValue("@time", DateTime.Now);
        string query = "insert into status_Ride (statusStatusName,RideRideNum,Timestamp) values (@status,@rideId,@time)";
        DbService db = new DbService();

        return db.ExecuteQuery(query, cmd.CommandType, cmdParams);
    }

    public int UpdateDriver(int rideNum,int ridePatId, int newDriverId, int assignedFromAppId)
    {
        query = "EXEC spRide_UpdateDriver @RideNum=" + rideNum + ", @NewDriverId=" + newDriverId + ", @AssignedFromAppId=" + assignedFromAppId;
        cmd = new SqlCommand();

        try
        {
            dbs = new DbService();
            int result = dbs.ExecuteQuery(query);
            ridepat = new RidePat();
            ridepat = ridepat.GetRidePat(ridePatId);
            BroadCast.BroadCast2Clients_driverHasAssigned2RidePat(ridepat);
            
            return result;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
        
    public List<Ride> GetMyRides(int volunteerId)
    {
        string query = "select * from RPView where MainDriver=" + volunteerId + " or secondaryDriver=" + volunteerId;
        DbService db = new DbService();
        DataSet ds = db.GetDataSetByQuery(query);
        DataSet EscortDS = new DataSet(); ;
        Ride r = new Ride();
        List<Ride> rl = new List<Ride>();
        r.RidePats = new List<RidePat>();
        RidePat rp2 = new RidePat();
        //List<RidePat> rpl = r.RidePats;
        bool RidePatexists;
        bool RideExists;
        try
        {
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                RideExists = false;
                foreach (Ride ride in rl)
                {
                    if (ride.Id == int.Parse(dr["RideNum"].ToString()))
                    {
                        RideExists = true;

                        RidePatexists = false;
                        foreach (RidePat ridePat in ride.RidePats)
                        {
                            if (ridePat.RidePatNum == int.Parse(dr["RidePatNum"].ToString())) RidePatexists = true;
                            if (RidePatexists && dr["Escort"].ToString() != "")
                            {
                                if (EscortDS.Tables[0].Rows.Count > 0)
                                {
                                    foreach (DataRow row in EscortDS.Tables[0].Rows)
                                    {
                                        Escorted e = new Escorted();
                                        e.DisplayName = row["DisplayName"].ToString();

                                        rp2.Pat.EscortedList.Add(e);
                                    }
                                }
                                break;
                            }
                        }
                        if (RidePatexists) continue;
                        rp2 = new RidePat();
                        //ride.RidePats = new List<RidePat>();
                        rp2.RidePatNum = int.Parse(dr["RidePatNum"].ToString());
                        rp2.Pat = new Patient();
                        rp2.Pat.DisplayName = dr["DisplayName"].ToString();
                        rp2.Pat.CellPhone = dr["CellPhone"].ToString();
                        rp2.Pat.Equipment = rp2.Pat.getEquipmentForPatient(rp2.Pat.DisplayName);
                        db = new DbService();

                        query = "select DisplayName,CellPhone from RidePatEscortView where RidePatNum=" + rp2.RidePatNum;
                        EscortDS = db.GetDataSetByQuery(query);
                        rp2.Pat.EscortedList = new List<Escorted>();
                        if (EscortDS.Tables[0].Rows.Count > 0)
                        {
                            foreach (DataRow row in EscortDS.Tables[0].Rows)
                            {
                                Escorted e = new Escorted();

                                e.DisplayName = row["DisplayName"].ToString();
                                e.CellPhone = row["CellPhone"].ToString();
                                rp2.Pat.EscortedList.Add(e);
                            }
                        }

                        Location origin = new Location();
                        origin.Name = dr["Origin"].ToString();
                        rp2.Origin = origin;
                        Location dest = new Location();
                        dest.Name = dr["Destination"].ToString();
                        rp2.Destination = dest;
                        rp2.Area = dr["Area"].ToString();
                        rp2.Shift = dr["Shift"].ToString();
                        rp2.Date = Convert.ToDateTime(dr["PickupTime"].ToString());
                        //adding anonymous
                        rp2.Pat.IsAnonymous = dr["IsAnonymous"].ToString();
                        ride.RidePats.Add(rp2);
                    }
                }

                if (RideExists) continue;
                Ride r2 = new Ride();

                if (dr["Maindriver"].ToString() == volunteerId.ToString())
                {
                    r2.DriverType = "Primary";
                }
                else
                {
                    r2.DriverType = "Secondary";
                }

                r2.Id = int.Parse(dr["RideNum"].ToString());
                query = "select statusStatusName from status_Ride where RideRideNum=" + r2.Id + " order by Timestamp desc";
                db = new DbService();
                r2.Statuses = new List<string>();
                foreach (DataRow status in db.GetDataSetByQuery(query).Tables[0].Rows)
                {
                    r2.Statuses.Add(status.ItemArray[0].ToString());
                }
                r2.Status = r2.Statuses[0]; db = new DbService();
                r2.Status = db.GetObjectScalarByQuery(query).ToString();
                RidePat rp = new RidePat();
                r2.RidePats = new List<RidePat>();
                rp.RidePatNum = int.Parse(dr["RidePatNum"].ToString());
                rp.Pat = new Patient();
                rp.Pat.DisplayName = dr["DisplayName"].ToString();
                rp.Pat.CellPhone = dr["CellPhone"].ToString();
                rp.Pat.Equipment = rp.Pat.getEquipmentForPatient(rp.Pat.DisplayName);
                rp.Pat.EscortedList = new List<Escorted>();
                db = new DbService();

                query = "select DisplayName,CellPhone from RidePatEscortView where RidePatNum=" + rp.RidePatNum;
                EscortDS = db.GetDataSetByQuery(query);
                rp.Pat.EscortedList = new List<Escorted>();
                if (EscortDS.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in EscortDS.Tables[0].Rows)
                    {
                        Escorted e = new Escorted();
                        e.DisplayName = row["DisplayName"].ToString();
                        e.CellPhone = row["CellPhone"].ToString();
                        rp.Pat.EscortedList.Add(e);
                    }
                }

                Location origin2 = new Location();
                origin2.Name = dr["Origin"].ToString();
                rp.Origin = origin2;
                Location dest2 = new Location();
                dest2.Name = dr["Destination"].ToString();
                rp.Destination = dest2;
                rp.Area = dr["Area"].ToString();
                rp.Shift = dr["Shift"].ToString();
                rp.Pat.IsAnonymous = dr["IsAnonymous"].ToString();
                rp.Date = Convert.ToDateTime(dr["PickupTime"].ToString());
                r2.RidePats.Add(rp);
                rl.Add(r2);
            }

            return rl;
        }
        catch (Exception e)
        {

            throw e;
        }
    }

    //Get My FUTURE RIDES
    public List<Ride> GetMyFutureRides(int volunteerId)
    {
        string query = "EXEC GetDriverFutureRides @driverId=" + volunteerId;
        DbService db = new DbService();
        DataSet ds = db.GetDataSetByQuery(query);
        DataSet EscortDS = new DataSet(); ;
        Ride r = new Ride();
        List<Ride> rl = new List<Ride>();
        r.RidePats = new List<RidePat>();
        RidePat rp2 = new RidePat();
        List<RidePat> rpl = r.RidePats;
        bool RidePatexists;
        bool RideExists;
        try
        {
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                RideExists = false;
                foreach (Ride ride in rl)
                {
                    if (ride.Id == int.Parse(dr["RideNum"].ToString()))
                    {
                        RideExists = true;

                        RidePatexists = false;
                        foreach (RidePat ridePat in ride.RidePats)
                        {
                            if (ridePat.RidePatNum == int.Parse(dr["RidePatNum"].ToString())) RidePatexists = true;
                            if (RidePatexists && dr["Escort"].ToString() != "")
                            {
                                if (EscortDS.Tables[0].Rows.Count > 0)
                                {
                                    foreach (DataRow row in EscortDS.Tables[0].Rows)
                                    {
                                        Escorted e = new Escorted();
                                        e.DisplayName = row["DisplayName"].ToString();
                                        rp2.Pat.EscortedList.Add(e);
                                    }
                                }
                                break;
                            }
                        }
                        if (RidePatexists) continue;
                        rp2 = new RidePat();
                        //ride.RidePats = new List<RidePat>();
                        rp2.RidePatNum = int.Parse(dr["RidePatNum"].ToString());
                        rp2.Pat = new Patient();
                        rp2.Pat.DisplayName = dr["DisplayName"].ToString();
                        rp2.Pat.CellPhone = dr["CellPhone"].ToString();
                        rp2.Pat.Equipment = rp2.Pat.getEquipmentForPatient(rp2.Pat.DisplayName);
                        db = new DbService();
                        query = "select DisplayName,CellPhone from RidePatEscortView where RidePatNum=" + rp2.RidePatNum;
                        EscortDS = db.GetDataSetByQuery(query);
                        rp2.Pat.EscortedList = new List<Escorted>();
                        if (EscortDS.Tables[0].Rows.Count > 0)
                        {
                            foreach (DataRow row in EscortDS.Tables[0].Rows)
                            {
                                Escorted e = new Escorted();
                                e.DisplayName = row["DisplayName"].ToString();
                                e.CellPhone = row["CellPhone"].ToString();
                                rp2.Pat.EscortedList.Add(e);
                            }
                        }

                        Location origin = new Location();
                        origin.Name = dr["Origin"].ToString();
                        rp2.Origin = origin;
                        Location dest = new Location();
                        dest.Name = dr["Destination"].ToString();
                        rp2.Destination = dest;
                        rp2.Area = dr["Area"].ToString();
                        rp2.Shift = dr["Shift"].ToString();
                        rp2.Date = Convert.ToDateTime(dr["PickupTime"].ToString());
                        //adding anonymous
                        rp2.Pat.IsAnonymous = dr["IsAnonymous"].ToString();
                        ride.RidePats.Add(rp2);
                    }
                }

                if (RideExists) continue;
                Ride r2 = new Ride();

                if (dr["Maindriver"].ToString() == volunteerId.ToString())
                {
                    r2.DriverType = "Primary";
                }
                else
                {
                    r2.DriverType = "Secondary";
                }

                r2.Id = int.Parse(dr["RideNum"].ToString());
                query = "select statusStatusName from status_Ride where RideRideNum=" + r2.Id + " order by Timestamp desc";
                db = new DbService();
                r2.Statuses = new List<string>();
                foreach (DataRow status in db.GetDataSetByQuery(query).Tables[0].Rows)
                {
                    r2.Statuses.Add(status.ItemArray[0].ToString());
                }
                r2.Status = r2.Statuses[0];
                db = new DbService();
                r2.Status = db.GetObjectScalarByQuery(query).ToString();
                RidePat rp = new RidePat();
                r2.RidePats = new List<RidePat>();
                rp.RidePatNum = int.Parse(dr["RidePatNum"].ToString());
                rp.Pat = new Patient();
                rp.Pat.DisplayName = dr["DisplayName"].ToString();
                rp.Pat.CellPhone = dr["CellPhone"].ToString();
                rp.Pat.Equipment = rp.Pat.getEquipmentForPatient(rp.Pat.DisplayName);
                rp.Pat.EscortedList = new List<Escorted>();
                db = new DbService();
                query = "select DisplayName,CellPhone from RidePatEscortView where RidePatNum=" + rp.RidePatNum;
                EscortDS = db.GetDataSetByQuery(query);
                rp.Pat.EscortedList = new List<Escorted>();
                if (EscortDS.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in EscortDS.Tables[0].Rows)
                    {
                        Escorted e = new Escorted();
                        e.DisplayName = row["DisplayName"].ToString();
                        e.CellPhone = row["CellPhone"].ToString();
                        rp.Pat.EscortedList.Add(e);
                    }
                }

                Location origin2 = new Location();
                origin2.Name = dr["Origin"].ToString();
                rp.Origin = origin2;
                Location dest2 = new Location();
                dest2.Name = dr["Destination"].ToString();
                rp.Destination = dest2;
                rp.Area = dr["Area"].ToString();
                rp.Shift = dr["Shift"].ToString();
                rp.Pat.IsAnonymous = dr["IsAnonymous"].ToString();
                rp.Date = Convert.ToDateTime(dr["PickupTime"].ToString());
                r2.RidePats.Add(rp);
                rl.Add(r2);
            }

            return rl;
        }
        catch (Exception e)
        {

            throw e;
        }
    }


    public List<Ride> GetMyPastRides(int volunteerId)
    {
        string query = "EXEC GetDriverPastRides @driverId=" + volunteerId;
        DbService db = new DbService();
        DataSet ds = db.GetDataSetByQuery(query);
        DataSet EscortDS = new DataSet(); ;
        Ride r = new Ride();
        List<Ride> rl = new List<Ride>();
        r.RidePats = new List<RidePat>();
        RidePat rp2 = new RidePat();
        //List<RidePat> rpl = r.RidePats;
        bool RidePatexists;
        bool RideExists;
        try
        {
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                RideExists = false;
                RidePatexists = false;
                foreach (Ride ride in rl)
                {
                    if (ride.Id == int.Parse(dr["RideNum"].ToString()))
                    {
                        RideExists = true;

                        RidePatexists = false;
                        foreach (RidePat ridePat in ride.RidePats)
                        {
                            if (ridePat.RidePatNum == int.Parse(dr["RidePatNum"].ToString())) RidePatexists = true;
                            //if (RidePatexists && dr["Escort"].ToString() != "")
                            //{
                            //    if (EscortDS.Tables[0].Rows.Count > 0)
                            //    {
                            //        foreach (DataRow row in EscortDS.Tables[0].Rows)
                            //        {
                            //            Escorted e = new Escorted();
                            //            e.DisplayName = row["DisplayName"].ToString();
                            //            rp2.Pat.EscortedList.Add(e);
                            //        }
                            //    }
                            //    break;
                            //}
                        }


                    }
                }
                if (RidePatexists) continue;

                //        rp2 = new RidePat();
                //        //ride.RidePats = new List<RidePat>();
                //        rp2.RidePatNum = int.Parse(dr["RidePatNum"].ToString());
                //        rp2.Pat = new Patient();
                //        rp2.Pat.DisplayName = dr["DisplayName"].ToString();
                //        rp2.Pat.CellPhone = dr["CellPhone"].ToString();
                //        rp2.Pat.Equipment = rp2.Pat.getEquipmentForPatient(rp2.Pat.DisplayName);
                //        db = new DbService();
                //        query = "select DisplayName from RidePatEscortView where RidePatNum=" + rp2.RidePatNum;
                //        EscortDS = db.GetDataSetByQuery(query);
                //        if (EscortDS.Tables[0].Rows.Count > 0)
                //        {
                //            rp2.Pat.EscortedList = new List<Escorted>();
                //            foreach (DataRow row in EscortDS.Tables[0].Rows)
                //            {
                //                Escorted e = new Escorted();
                //                e.DisplayName = row["DisplayName"].ToString();
                //                rp2.Pat.EscortedList.Add(e);
                //            }
                //        }

                //        Location origin = new Location();
                //        origin.Name = dr["Origin"].ToString();
                //        rp2.Origin = origin;
                //        Location dest = new Location();
                //        dest.Name = dr["Destination"].ToString();
                //        rp2.Destination = dest;
                //        rp2.Area = dr["Area"].ToString();
                //        rp2.Shift = dr["Shift"].ToString();
                //        rp2.Date = Convert.ToDateTime(dr["PickupTime"].ToString());
                //        //adding anonymous
                //        rp2.Pat.IsAnonymous = dr["IsAnonymous"].ToString();
                //        ride.RidePats.Add(rp2);
                //    }
                //}

                //if (RideExists) continue;
                Ride r2 = new Ride();

                if (dr["Maindriver"].ToString() == volunteerId.ToString())
                {
                    r2.DriverType = "Primary";
                }
                else
                {
                    r2.DriverType = "Secondary";
                }

                r2.Id = int.Parse(dr["RideNum"].ToString());
                query = "select statusStatusName from status_Ride where RideRideNum=" + r2.Id + " order by Timestamp desc";
                db = new DbService();
                r2.Statuses = new List<string>();
                foreach (DataRow status in db.GetDataSetByQuery(query).Tables[0].Rows)
                {
                    r2.Statuses.Add(status.ItemArray[0].ToString());
                }
                r2.Status = r2.Statuses[0];
                db = new DbService();
                r2.Status = db.GetObjectScalarByQuery(query).ToString();
                RidePat rp = new RidePat();
                r2.RidePats = new List<RidePat>();
                rp.RidePatNum = int.Parse(dr["RidePatNum"].ToString());
                rp.Pat = new Patient();
                rp.Pat.DisplayName = dr["DisplayName"].ToString();
                rp.Pat.CellPhone = dr["CellPhone"].ToString();
                rp.Pat.Equipment = rp.Pat.getEquipmentForPatient(rp.Pat.DisplayName);
                rp.Pat.EscortedList = new List<Escorted>();
                db = new DbService();
                query = "select DisplayName from RidePatEscortView where RidePatNum=" + rp.RidePatNum;
                EscortDS = db.GetDataSetByQuery(query);
                if (EscortDS.Tables[0].Rows.Count > 0)
                {
                    rp.Pat.EscortedList = new List<Escorted>();

                    foreach (DataRow row in EscortDS.Tables[0].Rows)
                    {
                        Escorted e = new Escorted();
                        e.DisplayName = row["DisplayName"].ToString();
                        rp.Pat.EscortedList.Add(e);
                    }
                }

                Location origin2 = new Location();
                origin2.Name = dr["Origin"].ToString();
                rp.Origin = origin2;
                Location dest2 = new Location();
                dest2.Name = dr["Destination"].ToString();
                rp.Destination = dest2;
                rp.Area = dr["Area"].ToString();
                rp.Shift = dr["Shift"].ToString();
                rp.Pat.IsAnonymous = dr["IsAnonymous"].ToString();
                rp.Date = Convert.ToDateTime(dr["PickupTime"].ToString());
                r2.RidePats.Add(rp);
                rl.Add(r2);
            }
            return rl;
        }
        catch (Exception e)
        {

            throw e;
        }
    }

    ////Get My Past rides
    //public List<Ride> GetMyPastRides(int volunteerId)
    //{
    //    string query = "select * from RPView where (MainDriver=" + volunteerId + " or secondaryDriver=" + volunteerId + ") and CONVERT(date,pickuptime)<CONVERT(DATE,getdate())";
    //    DbService db = new DbService();
    //    DataSet ds = db.GetDataSetByQuery(query);
    //    DataSet EscortDS = new DataSet(); ;
    //    Ride r = new Ride();
    //    List<Ride> rl = new List<Ride>();
    //    r.RidePats = new List<RidePat>();
    //    RidePat rp2 = new RidePat();
    //    //List<RidePat> rpl = r.RidePats;
    //    bool RidePatexists;
    //    bool RideExists;
    //    try
    //    {


    //        foreach (DataRow dr in ds.Tables[0].Rows)
    //        {
    //            RideExists = false;
    //            foreach (Ride ride in rl)
    //            {
    //                if (ride.Id == int.Parse(dr["RideNum"].ToString()))
    //                {
    //                    RideExists = true;

    //                    RidePatexists = false;
    //                    foreach (RidePat ridePat in ride.RidePats)
    //                    {
    //                        if (ridePat.RidePatNum == int.Parse(dr["RidePatNum"].ToString())) RidePatexists = true;
    //                        if (RidePatexists && dr["Escort"].ToString() != "")
    //                        {
    //                            if (EscortDS.Tables[0].Rows.Count > 0)
    //                            {
    //                                foreach (DataRow row in EscortDS.Tables[0].Rows)
    //                                {
    //                                    Escorted e = new Escorted();
    //                                    e.DisplayName = row["DisplayName"].ToString();
    //                                    rp2.Pat.EscortedList.Add(e);
    //                                }
    //                            }
    //                            break;
    //                        }
    //                    }
    //                    if (RidePatexists) continue;
    //                    rp2 = new RidePat();
    //                    //ride.RidePats = new List<RidePat>();
    //                    rp2.RidePatNum = int.Parse(dr["RidePatNum"].ToString());
    //                    rp2.Pat = new Patient();
    //                    rp2.Pat.DisplayName = dr["DisplayName"].ToString();
    //                    rp2.Pat.CellPhone = dr["CellPhone"].ToString();
    //                    rp2.Pat.Equipment = rp2.Pat.getEquipmentForPatient(rp2.Pat.DisplayName);
    //                    db = new DbService();
    //                    query = "select DisplayName from RidePatEscortView where RidePatNum=" + rp2.RidePatNum;
    //                    EscortDS = db.GetDataSetByQuery(query);
    //                    rp2.Pat.EscortedList = new List<Escorted>();
    //                    if (EscortDS.Tables[0].Rows.Count > 0)
    //                    {
    //                        foreach (DataRow row in EscortDS.Tables[0].Rows)
    //                        {
    //                            Escorted e = new Escorted();
    //                            e.DisplayName = row["DisplayName"].ToString();
    //                            rp2.Pat.EscortedList.Add(e);
    //                        }
    //                    }

    //                    Location origin = new Location();
    //                    origin.Name = dr["Origin"].ToString();
    //                    rp2.Origin = origin;
    //                    Location dest = new Location();
    //                    dest.Name = dr["Destination"].ToString();
    //                    rp2.Destination = dest;
    //                    rp2.Area = dr["Area"].ToString();
    //                    rp2.Shift = dr["Shift"].ToString();
    //                    rp2.Date = Convert.ToDateTime(dr["PickupTime"].ToString());
    //                    //adding anonymous
    //                    rp2.Pat.IsAnonymous = dr["IsAnonymous"].ToString();
    //                    ride.RidePats.Add(rp2);
    //                }
    //            }

    //            if (RideExists) continue;
    //            Ride r2 = new Ride();

    //            if (dr["Maindriver"].ToString() == volunteerId.ToString())
    //            {
    //                r2.DriverType = "Primary";
    //            }
    //            else
    //            {
    //                r2.DriverType = "Secondary";
    //            }

    //            r2.Id = int.Parse(dr["RideNum"].ToString());
    //            query = "select statusStatusName from status_Ride where RideRideNum=" + r2.Id + " order by Timestamp desc";
    //            db = new DbService();
    //            r2.Statuses = new List<string>();
    //            foreach (DataRow status in db.GetDataSetByQuery(query).Tables[0].Rows)
    //            {
    //                r2.Statuses.Add(status.ItemArray[0].ToString());
    //            }
    //            r2.Status = r2.Statuses[0];
    //            db = new DbService();
    //            r2.Status = db.GetObjectScalarByQuery(query).ToString();
    //            RidePat rp = new RidePat();
    //            r2.RidePats = new List<RidePat>();
    //            rp.RidePatNum = int.Parse(dr["RidePatNum"].ToString());
    //            rp.Pat = new Patient();
    //            rp.Pat.DisplayName = dr["DisplayName"].ToString();
    //            rp.Pat.CellPhone = dr["CellPhone"].ToString();
    //            rp.Pat.Equipment = rp.Pat.getEquipmentForPatient(rp.Pat.DisplayName);
    //            rp.Pat.EscortedList = new List<Escorted>();
    //            db = new DbService();
    //            query = "select DisplayName from RidePatEscortView where RidePatNum=" + rp.RidePatNum;
    //            EscortDS = db.GetDataSetByQuery(query);
    //            rp.Pat.EscortedList = new List<Escorted>();
    //            if (EscortDS.Tables[0].Rows.Count > 0)
    //            {
    //                foreach (DataRow row in EscortDS.Tables[0].Rows)
    //                {
    //                    Escorted e = new Escorted();
    //                    e.DisplayName = row["DisplayName"].ToString();
    //                    rp.Pat.EscortedList.Add(e);
    //                }
    //            }

    //            Location origin2 = new Location();
    //            origin2.Name = dr["Origin"].ToString();
    //            rp.Origin = origin2;
    //            Location dest2 = new Location();
    //            dest2.Name = dr["Destination"].ToString();
    //            rp.Destination = dest2;
    //            rp.Area = dr["Area"].ToString();
    //            rp.Shift = dr["Shift"].ToString();
    //            rp.Pat.IsAnonymous = dr["IsAnonymous"].ToString();
    //            rp.Date = Convert.ToDateTime(dr["PickupTime"].ToString());
    //            r2.RidePats.Add(rp);
    //            rl.Add(r2);
    //        }

    //        return rl;
    //    }
    //    catch (Exception e)
    //    {

    //        throw e;
    //    }
    //}


    public string GetDriverName(int rideId)
    {
        query = "exec VolunteerAndRide_GetDriverName @rideNum=" + rideId;

        try
        {
            dbs = new DbService();
            SqlDataReader sdr = dbs.GetDataReader(query);
            if (sdr.Read())
            {
                return Convert.ToString(sdr["DisplayName"]);
            }
            else
            {
                return "שגיאה בקריאת שם נהג";
            }
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
        finally
        {
            dbs.CloseConnection();
        }

    }

}