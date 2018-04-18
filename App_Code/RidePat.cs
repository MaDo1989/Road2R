using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

/// <summary>
/// Summary description for RidePat
/// </summary>
public class RidePat
{
    int ridePatNum;//מספר הסעה
    Patient pat;//חולה
    //Escorted escorted1;//נלקח מהמלווים של החולה
    //Escorted escorted2;//נלקח מהמלווים של החולה
    //Escorted escorted3;//נלקח מהמלווים של החולה
    Location origin;//מקום התחלה
    Location destination;//מקום סיום
    //string startArea;
    //string finishArea;
    string day;// יום
    DateTime date; //תאריך
    string leavingHour;//שעת יציאה
    //int quantity;//כמות מלווים
    string addition;//כלי עזר
    string rideType;//סוג הסעה
    Volunteer coordinator; //רכז אחראי
    string remark;//הערות
    string status;
    string area;
    string shift;

    public List<Volunteer> Drivers { get; set; }

    public string Shift { get; set; }

    public RidePat()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    //public RidePat(Patient _pat, Escorted _escorted1, Destination _startPlace, Destination _target,
    //   string _day, string _date, string _leavingHour, int _quantity, string _addition, string _rideType,
    //   Volunteer _coordinator, string _remark, string _status)
    //{
    //    Pat = _pat;
    //    Escorted1 = _escorted1;
    //    StartPlace = _startPlace;
    //    Target = _target;
    //    Day = _day;
    //    Date = _date;
    //    LeavingHour = _leavingHour;
    //    Quantity = _quantity;
    //    Addition = _addition;
    //    RideType = _rideType;
    //    Coordinator = _coordinator;
    //    Remark = _remark;
    //    Status = _status;
    //}
    //public RidePat(int _ridePatNum, Escorted _escorted1, Escorted _escorted2, Escorted _escorted3, Destination _startPlace, Destination _target,
    //   string _day, string _date, string _leavingHour, int _quantity, string _addition, string _rideType,
    //   Volunteer _coordinator, string _remark, string _status)
    //{
    //    RidePatNum = _ridePatNum;
    //    Escorted1 = _escorted1;
    //    Escorted2 = _escorted2;
    //    Escorted3 = _escorted3;
    //    StartPlace = _startPlace;
    //    Target = _target;
    //    Day = _day;
    //    Date = _date;
    //    LeavingHour = _leavingHour;
    //    Quantity = _quantity;
    //    Addition = _addition;
    //    RideType = _rideType;
    //    Coordinator = _coordinator;
    //    Remark = _remark;
    //    Status = _status;
    //}

    public int RidePatNum
    {
        get
        {
            return ridePatNum;
        }

        set
        {
            ridePatNum = value;
        }
    }

    public Patient Pat
    {
        get
        {
            return pat;
        }

        set
        {
            pat = value;
        }
    }

    public Location Origin
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

    public Location Destination
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

    public string Day
    {
        get
        {
            return day;
        }

        set
        {
            day = value;
        }
    }

    public DateTime Date
    {
        get
        {
            return date;
        }

        set
        {
            date = value;
        }
    }

    public string LeavingHour
    {
        get
        {
            return leavingHour;
        }

        set
        {
            leavingHour = value;
        }
    }

    public string Addition
    {
        get
        {
            return addition;
        }

        set
        {
            addition = value;
        }
    }

    public RidePat GetRidePat(int ridePatNum)
    {
        string query = "select * from RidePatView where RidePatNum=" + ridePatNum;
        DbService db = new DbService();
        DataSet ds = db.GetDataSetByQuery(query);
        RidePat rp = new RidePat();
        rp.pat = new Patient();
        DataRow dr = ds.Tables[0].Rows[0];

        rp.RidePatNum = int.Parse(dr["RidePatNum"].ToString());

        rp.pat.DisplayName = dr["Patient"].ToString();
        rp.pat.EscortedList = new List<Escorted>();
        Location origin = new Location();
        origin.Name = dr["RidePatOrigin"].ToString();
        rp.Origin = origin;
        Location dest = new Location();
        dest.Name = dr["RidePatDestination"].ToString();
        rp.Destination = dest;
        rp.Area = dr["RidePatArea"].ToString();
        rp.Shift = dr["RidePatShift"].ToString();
        rp.Date = Convert.ToDateTime(dr["RidePatPickupTime"].ToString());
        rp.Status = dr["RidePatStatus"].ToString();
        rp.Coordinator = new Volunteer();
        rp.Coordinator.DisplayName = dr["Coordinator"].ToString();
        foreach (DataRow r in ds.Tables[0].Rows)
        {
            if (r["Escort"].ToString() != "")
            {
                Escorted e = new Escorted();
                e.DisplayName = r["Escort"].ToString();
                rp.pat.EscortedList.Add(e);
            }
        }



        return rp;

    }

    //public List<RidePat> GetAllRidePats()
    //{

    //}

    public string RideType
    {
        get
        {
            return rideType;
        }

        set
        {
            rideType = value;
        }
    }

    public Volunteer Coordinator
    {
        get
        {
            return coordinator;
        }

        set
        {
            coordinator = value;
        }
    }

    public string Remark
    {
        get
        {
            return remark;
        }

        set
        {
            remark = value;
        }
    }

    public string Status
    {
        get
        {
            return status;
        }

        set
        {
            status = value;
        }
    }

    public string Area { get; set; }


    //This method is used for שבץ אותי
    public List<RidePat> GetRidePatView(int volunteerId)//In case of coordinator will send -1 as ID.
    {
        string query = "";
        if (volunteerId != -1)
            query = "select * from RidePatView where (RidePatStatus='שובץ נהג' or RidePatStatus='ממתינה לשיבוץ')";
        else query = "select * from RidePatView";
        DbService db = new DbService();
        DataSet ds = db.GetDataSetByQuery(query);
        // Ride ride = new Ride();
        // ride.RidePats = new List<RidePat>();
        List<RidePat> rpl = new List<RidePat>();
        bool exists;
        foreach (DataRow dr in ds.Tables[0].Rows)
        {

            if (dr["MainDriver"].ToString() == "" && dr["secondaryDriver"].ToString() == "")
            {

            }
            else if (dr["MainDriver"].ToString() != "" && dr["secondaryDriver"].ToString() == "")
            {
                if (int.Parse(dr["MainDriver"].ToString()) == volunteerId)
                    continue;
            }





            //try
            //{
            //    if (volunteerId != -1)
            //    {
            //        if (int.Parse(dr["DriverId"].ToString()) == volunteerId) continue; //|| dr["statusRide"].ToString() == "מלאה" || dr["statusRide"].ToString() == "הסתיימה"
            //    }
            //}
            //catch (Exception)
            //{
            //    //No driver was aasigned to RidePat, continue to show RidePats
            //}

            exists = false;
            foreach (RidePat ridePat in rpl)
            {


                if (ridePat.RidePatNum == int.Parse(dr["RidePatNum"].ToString()) && dr["Escort"].ToString() != "")
                {
                    Escorted es = new Escorted();
                    es.DisplayName = dr["Escort"].ToString();
                    ridePat.pat.EscortedList.Add(es);
                    exists = true;
                    break;
                }
            }
            if (exists) continue;
            RidePat rp = new RidePat();
            rp.RidePatNum = int.Parse(dr["RidePatNum"].ToString());
            rp.pat = new Patient();
            rp.pat.DisplayName = dr["Patient"].ToString();
            rp.pat.EscortedList = new List<Escorted>();
            if (dr["Escort"].ToString() != "")
            {
                Escorted e = new Escorted();
                e.DisplayName = dr["Escort"].ToString();
                rp.pat.EscortedList.Add(e);
            }

            Location origin = new Location();
            origin.Name = dr["RidePatOrigin"].ToString();
            rp.Origin = origin;
            Location dest = new Location();
            dest.Name = dr["RidePatDestination"].ToString();
            rp.Destination = dest;
            rp.Area = dr["RidePatArea"].ToString();
            rp.Shift = dr["RidePatShift"].ToString();
            rp.Date = Convert.ToDateTime(dr["RidePatPickupTime"].ToString());
            rp.Status = dr["RidePatStatus"].ToString();
            rpl.Add(rp);
            //ride.Status = dr["statusRide"].ToString();
        }

        return rpl;
        #region old
        //        DbService db = new DbService();
        //        string query = "select * from RidePat";
        //        DataSet ds = db.GetDataSetByQuery(query);
        //        List<RidePat> rpl = new List<RidePat>();
        //        foreach (DataRow row in ds.Tables[0].Rows)
        //        {
        //            RidePat rp = new RidePat();
        //            rp.RidePatNum = int.Parse(row.ItemArray[0].ToString());
        //            Patient pat = new Patient();
        //            rp.StartPlace = new Destination();
        //            rp.Destination = new Destination();
        //            pat.DisplayName = row.ItemArray[1].ToString();
        //            pat.EscortedList = pat.getescortedsList(pat.DisplayName);
        //            rp.Pat = pat;
        //            rp.StartPlace.Name = row.ItemArray[2].ToString();
        //            rp.Destination.Name = row.ItemArray[3].ToString();
        //            rp.Date = Convert.ToDateTime(row.ItemArray[5].ToString());
        //            rp.Day = rp.Date.DayOfWeek.ToString();
        //            //rp.LeavingHour = row.ItemArray[6].ToString();
        //            rp.Addition = row.ItemArray[8].ToString();
        //            rp.Area = row.ItemArray[13].ToString();
        //            rp.Shift = row.ItemArray[15].ToString();
        //            //if (row.ItemArray[13] != null)
        //            //{
        //            //    Escorted e = new Escorted();
        //            //    e.DisplayName = row.ItemArray[13].ToString();
        //            //    rp.escorted1 = e;
        //            //}
        //            //if (row.ItemArray[14] != null)
        //            //{
        //            //    Escorted e = new Escorted();
        //            //    e.DisplayName = row.ItemArray[14].ToString();
        //            //    rp.escorted2 = e;
        //            //}
        //            //if (row.ItemArray[15] != null)
        //            //{
        //            //    Escorted e = new Escorted();
        //            //    e.DisplayName = row.ItemArray[15].ToString();
        //            //    rp.escorted3 = e;
        //            //}
        //            rpl.Add(rp);
        //        }
        //        return rpl;
        #endregion
    }

    public int CombineRideRidePat(int rideId, int ridePatId)
    {
        string query = "select Status from RidePat where RidePatNum=" + ridePatId;
        DbService db = new DbService();
        DataSet ds = db.GetDataSetByQuery(query);
        DataRow dr = ds.Tables[0].Rows[0];
        if (dr["Status"].ToString() != "ממתינה לשיבוץ") return -1;

        string query2 = "update RidePat set RideId=" + rideId + " where RidePatNum=" + ridePatId;
        DbService db2 = new DbService();
        int res = db2.ExecuteQuery(query2);
        return res;
    }

    public int AssignRideToRidePat(int ridePatId, int userId)
    {
        int RideId = -1;
       
        string query = "select RideNum,RidePatOrigin,RidePatDestination,RidePatPickupTime,RidePatStatus,MainDriver,secondaryDriver from RidePatView where RidePatNum=" + ridePatId;
        DbService db = new DbService();
        DataSet ds = db.GetDataSetByQuery(query);
        DataRow dr = ds.Tables[0].Rows[0];
       
        Origin = new Location();
        Origin.Name = dr["RidePatOrigin"].ToString();
        Destination = new Location();
        Destination.Name = dr["RidePatDestination"].ToString();
        Date = Convert.ToDateTime(dr["RidePatPickupTime"].ToString());
        if (dr["RidePatStatus"].ToString() == "שובץ נהג וגיבוי") return -1;

        if (dr["RideNum"].ToString() != "")
        {
            RideId = int.Parse(dr["RideNum"].ToString());

           
            if (dr["MainDriver"].ToString() == "")
                query = "update Ride set MainDriver=" + userId + " where RideNum=" + RideId;
            else query = "update Ride set secondaryDriver=" + userId + " where RideNum=" + RideId;

            DbService db4 = new DbService();
        int res =    db4.ExecuteQuery(query);
            if (res <= 0) return -1;
        }
        else
        {
            string query2 = "set dateformat dmy; insert into Ride (Origin,Destination,Date,MainDriver) values ('" + Origin.Name + "','" + Destination.Name + "','" + Date + "'," + userId + ") SELECT SCOPE_IDENTITY()"; ;
            DbService db2 = new DbService();
            RideId = int.Parse(db2.GetObjectScalarByQuery(query2).ToString());
            if (RideId <= 0) return -1;
            string query3 = "update RidePat set RideId=" + RideId + " where RidePatNum=" + ridePatId;
            DbService db3 = new DbService();
            int res=db3.ExecuteQuery(query3);
            if (res <= 0) return -1;
        }




        return RideId;

    }

    public int LeaveRidePat(int ridePatId,int rideId,int driverId)
    {
        int res = -1;
        string driver = "";
        string query4 = "select MainDriver,secondaryDriver from Ride where RideNum=" + rideId;
        DbService db4 = new DbService();
        DataSet ds2 = db4.GetDataSetByQuery(query4);
        DataRow dr = ds2.Tables[0].Rows[0];
        if (dr["MainDriver"].ToString()==driverId.ToString())
        {
            driver = "MainDriver";
        }
        else if (dr["secondaryDriver"].ToString() == driverId.ToString())
        {
            driver = "secondaryDriver";
        }



        string query = "update RidePat set RideId=null where RidePatNum=" + ridePatId; //+"; update Ride set "+driver+" =null where RideNum="+rideId;
        DbService db = new DbService();
        res = db.ExecuteQuery(query);

        string query2 = "select RidePatNum from RidePat where RideId=" + rideId;
        DbService db2 = new DbService();
        DataSet ds = db2.GetDataSetByQuery(query2);
        if (ds.Tables[0].Rows.Count == 0)
        {
            string query3 = "update Ride set set " + driver + " =null where RideNum=" + rideId;
            DbService db3 = new DbService();
            res += db3.ExecuteQuery(query3);
        }
        return res;

    }

    public int DeleteDriver(int ridePatId, int driverId)
    {
        int res = -1;
        bool primary = false;
        int rideNum = -1;
        int BackupDriver = -1;
        DbService db = new DbService();
        string query = "select * from RidePatView where RidePatNum=" + ridePatId;
        DataSet ds = db.GetDataSetByQuery(query);
        DataRow row = ds.Tables[0].Rows[0];
        
            try
            {
                BackupDriver = int.Parse(row["secondaryDriver"].ToString());
            }
            catch (Exception)
            {

                BackupDriver = -1;
            }
            rideNum = int.Parse(row["RideNum"].ToString());
            if (driverId == int.Parse(row["MainDriver"].ToString()))
            {
                primary = true;
            }
            else if (driverId == BackupDriver)
            {
                primary = false;
            }
        
        if (primary)
        {
            query = "update Ride set MainDriver=null where RideNum=" + rideNum;
            DbService db2 = new DbService();
            res = db2.ExecuteQuery(query);
        }
        else
        {
            query = "update Ride set secondaryDriver=null where RideNum=" + rideNum;
            DbService db5 = new DbService();
            res = db5.ExecuteQuery(query);
        }

        return res;
    }

    public int SignDriver(int ridePatId, int ridePatId2, int driverId, bool primary)
    {
        DbService db = new DbService();
        string query = "select Origin, Destination, PickupTime from RidePat where ridePatNum=" + ridePatId;
        DataSet ds = db.GetDataSetByQuery(query);
        int res = -1;
        foreach (DataRow row in ds.Tables[0].Rows)//Origin and Destination are the same for RidePat and Ride.
        {
            RidePatNum = ridePatId;
            Origin = new Location();
            Origin.Name = row["Origin"].ToString();
            Destination = new Location();
            Destination.Name = row["Destination"].ToString();
            //row["dateRide"].ToString("MM-dd-yyyy"))
            Date = Convert.ToDateTime(row["PickupTime"].ToString());
            Day = Date.DayOfWeek.ToString();
            //LeavingHour = Date.ToShortTimeString();
        }
        DbService db2 = new DbService();
        int RideId = -1;
        if (primary)
        {
            Origin.Name = Origin.Name.Replace("'", "''");
            Destination.Name = Destination.Name.Replace("'", "''");
            query = "set dateformat dmy; insert into Ride (Origin, Destination, Date, MainDriver) values ('" + Origin.Name + "','" + Destination.Name + "','" + Date + "', " + driverId + ") SELECT SCOPE_IDENTITY()";
            RideId = int.Parse(db2.GetObjectScalarByQuery(query).ToString()); //Insert and get the new RideId



            DbService db3 = new DbService();
            query = "update RidePat set RideId=" + RideId + " where ridePatNum=" + RidePatNum;
            res = db3.ExecuteQuery(query);
            if (ridePatId2 != -1)
            {
                query = "update RidePat set RideId=" + RideId + " where ridePatNum=" + ridePatId2;
                DbService db4 = new DbService();
                res += db4.ExecuteQuery(query);
            }
        }
        else
        {
            query = "select RideNum from RidePatView where RidePatNum=" + ridePatId;
            DbService db5 = new DbService();
            RideId = int.Parse(db5.GetObjectScalarByQuery(query).ToString());
            query = "update Ride set secondaryDriver=" + driverId + " where RideNum=" + RideId;
            DbService db6 = new DbService();
            res = db6.ExecuteQuery(query);
        }

        return res;

    }


    //Irrelevant
    #region GetRidePatEscortView
    //public List<RidePat> GetRidePatEscortView()
    //{
    //    string query = "select * from RidePatEscortView";
    //    DbService db = new DbService();
    //    DataSet ds = db.GetDataSetByQuery(query);
    //    List<RidePat> rpl = new List<RidePat>();
    //    bool exists;
    //    foreach (DataRow dr in ds.Tables[0].Rows)
    //    {
    //        exists = false;
    //        foreach (RidePat ride in rpl)
    //        {
    //            if (ride.RidePatNum == int.Parse(dr.ItemArray[0].ToString()) && dr.ItemArray[2].ToString() != "")
    //            {
    //                Escorted es = new Escorted();
    //                es.DisplayName = dr.ItemArray[2].ToString();
    //                ride.pat.EscortedList.Add(es);
    //                exists = true;
    //                break;
    //            }
    //        }
    //        if (exists) continue;
    //        RidePat rp = new RidePat();
    //        rp.RidePatNum = int.Parse(dr.ItemArray[0].ToString());
    //        rp.pat = new Patient();
    //        rp.pat.DisplayName = dr.ItemArray[1].ToString();
    //        rp.pat.EscortedList = new List<Escorted>();
    //        if (dr.ItemArray[2].ToString() != "")
    //        {
    //            Escorted e = new Escorted();
    //            e.DisplayName = dr.ItemArray[2].ToString();
    //            rp.pat.EscortedList.Add(e);
    //        }

    //        Destination origin = new Destination();
    //        origin.Name = dr.ItemArray[3].ToString();
    //        rp.StartPlace = origin;
    //        Destination dest = new Destination();
    //        dest.Name = dr.ItemArray[4].ToString();
    //        rp.Destination = dest;
    //        rp.Area = dr.ItemArray[5].ToString();
    //        rp.Shift = dr.ItemArray[6].ToString();
    //        rp.Date = Convert.ToDateTime(dr.ItemArray[7].ToString());
    //        rpl.Add(rp);
    //    }

    //    return rpl;
    //}
    #endregion
}