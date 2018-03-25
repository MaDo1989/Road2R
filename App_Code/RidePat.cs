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
    Destination startPlace;//מקום התחלה
    Destination target;//מקום סיום
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

    public List<RidePat> GetRidePatEscortView()
    {
        string query = "select * from RidePatEscortView";
        DbService db = new DbService();
        DataSet ds = db.GetDataSetByQuery(query);
        List<RidePat> rpl = new List<RidePat>();
        bool exists;
        foreach (DataRow dr in ds.Tables[0].Rows)
        {
            exists = false;
            foreach (RidePat ride in rpl)
            {
                if (ride.RidePatNum == int.Parse(dr.ItemArray[0].ToString()) && dr.ItemArray[2].ToString() != "")
                {
                    Escorted es = new Escorted();
                    es.DisplayName = dr.ItemArray[2].ToString();
                    ride.pat.EscortedList.Add(es);
                    exists = true;
                    break;
                }
            }
            if (exists) continue;
            RidePat rp = new RidePat();
            rp.RidePatNum = int.Parse(dr.ItemArray[0].ToString());
            rp.pat = new Patient();
            rp.pat.DisplayName = dr.ItemArray[1].ToString();
            rp.pat.EscortedList = new List<Escorted>();
            if (dr.ItemArray[2].ToString() != "")
            {
                Escorted e = new Escorted();
                e.DisplayName = dr.ItemArray[2].ToString();
                rp.pat.EscortedList.Add(e);
            }

            Destination origin = new Destination();
            origin.Name = dr.ItemArray[3].ToString();
            rp.StartPlace = origin;
            Destination dest = new Destination();
            dest.Name = dr.ItemArray[4].ToString();
            rp.Target = dest;
            rp.Area = dr.ItemArray[5].ToString();
            rp.Shift = dr.ItemArray[6].ToString();
            rp.Date = Convert.ToDateTime(dr.ItemArray[7].ToString());
            rpl.Add(rp);
        }

        return rpl;
    }

    //public int DeleteRide(int ridePatId)
    //{
    //    string query = "update RidePat set statusRidePat='לא פעיל' where ridePatNum=" + ridePatId;
    //    DbService db = new DbService();
    //    int res = db.ExecuteQuery(query);

    //}

    public List<RidePat> GetMyRides(int volunteerId)
    {
        string query = "select * from RideView where DriverId=" + volunteerId;
        DbService db = new DbService();
        DataSet ds = db.GetDataSetByQuery(query);
        List<RidePat> rpl = new List<RidePat>();
        bool exists;
        foreach (DataRow dr in ds.Tables[0].Rows)
        {
            exists = false;
            foreach (RidePat ride in rpl)
            {
                if (ride.RidePatNum == int.Parse(dr["ridePatNum"].ToString()) && dr["Escort"].ToString() != "")
                {
                    Escorted es = new Escorted();
                    es.DisplayName = dr["Escort"].ToString();
                    ride.pat.EscortedList.Add(es);
                    exists = true;
                    break;
                }
            }
            if (exists) continue;
            RidePat rp = new RidePat();
            rp.RidePatNum = int.Parse(dr["ridePatNum"].ToString());
            rp.pat = new Patient();
            rp.pat.DisplayName = dr["patient"].ToString();
            rp.pat.EscortedList = new List<Escorted>();
            if (dr["Escort"].ToString() != "")
            {
                Escorted e = new Escorted();
                e.DisplayName = dr["Escort"].ToString();
                rp.pat.EscortedList.Add(e);
            }

            Destination origin = new Destination();
            origin.Name = dr["RidePatOrigin"].ToString();
            rp.StartPlace = origin;
            Destination dest = new Destination();
            dest.Name = dr["RidePatDestination"].ToString();
            rp.Target = dest;
            rp.Area = dr["RidePatArea"].ToString();
            rp.Shift = dr["RidePatShift"].ToString();
            rp.Date = Convert.ToDateTime(dr["RidePatDate"].ToString());
            rpl.Add(rp);
        }

        return rpl;
    }

   

    public int DeleteDriver(int ridePatId, int driverId)
    {
        int res = -1;
        bool primary = false;
        int rideNum = -1;
        int BackupDriver = -1;
        DbService db = new DbService();
        string query = "select * from RideView where RidePatNum=" + ridePatId;
        DataSet ds = db.GetDataSetByQuery(query);

        foreach (DataRow row in ds.Tables[0].Rows)
        {
            try
            {
                BackupDriver = int.Parse(row["BackupDriverId"].ToString());
            }
            catch (Exception)
            {

                BackupDriver = -1;
            }
            rideNum = int.Parse(row["RideNum"].ToString());
            if (driverId == int.Parse(row["DriverId"].ToString()))
            {
                primary = true;
            }
            else if (driverId == BackupDriver)
            {
                primary = false;
            }
        }
        if (primary && BackupDriver != -1)
        {
            query = "update Ride set DriverId=" + BackupDriver + ",BackupDriverId=null,statusRide= 'שובץ נהג' where RideNum=" + rideNum;
            DbService db2 = new DbService();
            res = db2.ExecuteQuery(query);
        }
        else if (primary && BackupDriver == -1)
        {
            query = "update Ride set DriverId=null,statusRide= 'לא פעילה' where RideNum=" + rideNum;
           DbService db3 = new DbService();
            res = db3.ExecuteQuery(query);
            query = "update RidePat set RideId=null where ridePatNum=" + ridePatId;
            DbService db4 = new DbService();
            res += db4.ExecuteQuery(query);
        }

        else if (!primary)
        {
            query = "update Ride set BackupDriverId=null, statusRide='שובץ נהג' where RideNum=" + rideNum;
            DbService db5 = new DbService();
            res = db5.ExecuteQuery(query);
        }

        return res;
    }

    public int SignDriver(int ridePatId, int ridePatId2, int driverId, bool primary)
    {
        DbService db = new DbService();
        string query = "select startPlace, finishPlace, dateRide from RidePat where ridePatNum=" + ridePatId;
        DataSet ds = db.GetDataSetByQuery(query);
        int res = -1;
        foreach (DataRow row in ds.Tables[0].Rows)//Origin and Destination are the same for RidePat and Ride.
        {
            RidePatNum = ridePatId;
            StartPlace = new Destination();
            StartPlace.Name = row["startPlace"].ToString();
            Target = new Destination();
            Target.Name = row["finishPlace"].ToString();
            //row["dateRide"].ToString("MM-dd-yyyy"))
            Date = Convert.ToDateTime(row["dateRide"].ToString());
            Day = Date.DayOfWeek.ToString();
            LeavingHour = Date.ToShortTimeString();
        }
        DbService db2 = new DbService();
        int RideId;
        if (primary)
        {
            query = "set dateformat dmy; insert into Ride (startPlace, finishPlace, dayRide, DateRide, hourRide, statusRide, DriverId) output inserted.RideNum values ('" + StartPlace.Name + "','" + Target.Name + "','" + Day + "','" + Date + "','" + LeavingHour + "','שובץ נהג'," + driverId + ")";
            RideId = int.Parse(db2.GetObjectScalarByQuery(query).ToString());

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
            query = "select RideNum from RideView where ridePatNum=" + ridePatId;
            DbService db5 = new DbService();
            RideId = int.Parse(db5.GetObjectScalarByQuery(query).ToString());
            query = "update Ride set statusRide='מלאה', BackupDriverId=" + driverId + "where RideNum=" + RideId;
            DbService db6 = new DbService();
            res = db6.ExecuteQuery(query);
        }

        return res;

    }

    //public Escorted Escorted1
    //{
    //    get
    //    {
    //        return escorted1;
    //    }

    //    set
    //    {
    //        escorted1 = value;
    //    }
    //}

    //public Escorted Escorted2
    //{
    //    get
    //    {
    //        return escorted2;
    //    }

    //    set
    //    {
    //        escorted2 = value;
    //    }
    //}

    //public Escorted Escorted3
    //{
    //    get
    //    {
    //        return escorted3;
    //    }

    //    set
    //    {
    //        escorted3 = value;
    //    }
    //}

    public Destination StartPlace
    {
        get
        {
            return startPlace;
        }

        set
        {
            startPlace = value;
        }
    }

    public Destination Target
    {
        get
        {
            return target;
        }

        set
        {
            target = value;
        }
    }

    //public string StartArea
    //{
    //    get
    //    {
    //        return startArea;
    //    }

    //    set
    //    {
    //        startArea = value;
    //    }
    //}

    //public string FinishArea
    //{
    //    get
    //    {
    //        return finishArea;
    //    }

    //    set
    //    {
    //        finishArea = value;
    //    }
    //}

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

    //public int Quantity
    //{
    //    get
    //    {
    //        return quantity;
    //    }

    //    set
    //    {
    //        quantity = value;
    //    }
    //}

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


    //public DataTable read()
    //{
    //    DBservices dbs = new DBservices();
    //    dbs = dbs.ReadFromDataBase("RoadDBconnectionString", "RidePat");
    //    return dbs.dt;
    //}
    //public DataTable getRidePat()
    //{
    //    string cmdStr = "select * from RidePat";
    //    DataSet ds = new DataSet();
    //    try
    //    {
    //        adp = new SqlDataAdapter(cmdStr, con);

    //        adp.Fill(ds, "RidePat");

    //    }
    //    catch (Exception e)
    //    {
    //        e.Message.ToString();
    //    }
    //    return ds.Tables["RidePAt"];
    //}

    //This method is used for שבץ אותי
    public List<RidePat> GetRidePatView(int volunteerId)//In case of coordinator will send -1 as ID.
    {
        string query = "select * from RidePatView where statusRide='שובץ נהג' or statusRide='פעילה' or statusRide='ממתינה לשיבוץ'"; //fix selection to show only relevant RidePats
        DbService db = new DbService();
        DataSet ds = db.GetDataSetByQuery(query);
        Ride ride = new Ride();
        ride.RidePats = new List<RidePat>();
        //List<RidePat> rpl = new List<RidePat>();
        bool exists;
        foreach (DataRow dr in ds.Tables[0].Rows)
        {
            try
            {
                if (volunteerId != -1)
                {
                    if (int.Parse(dr["DriverId"].ToString()) == volunteerId || int.Parse(dr["BackupDriverId"].ToString()) == volunteerId ) continue; //|| dr["statusRide"].ToString() == "מלאה" || dr["statusRide"].ToString() == "הסתיימה"
                }
            }
            catch (Exception)
            {

            }

            exists = false;
            foreach (RidePat ridePat in ride.RidePats)
            {


                if (ridePat.RidePatNum == int.Parse(dr["ridePatNum"].ToString()) && dr["Escort"].ToString() != "")
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
            rp.RidePatNum = int.Parse(dr["ridePatNum"].ToString());
            rp.pat = new Patient();
            rp.pat.DisplayName = dr["patient"].ToString();
            rp.pat.EscortedList = new List<Escorted>();
            if (dr["Escort"].ToString() != "")
            {
                Escorted e = new Escorted();
                e.DisplayName = dr["Escort"].ToString();
                rp.pat.EscortedList.Add(e);
            }

            Destination origin = new Destination();
            origin.Name = dr["RidePatOrigin"].ToString();
            rp.StartPlace = origin;
            Destination dest = new Destination();
            dest.Name = dr["RidePatDestination"].ToString();
            rp.Target = dest;
            rp.Area = dr["RidePatArea"].ToString();
            rp.Shift = dr["RidePatShift"].ToString();
            rp.Date = Convert.ToDateTime(dr["RidePatDate"].ToString());
            ride.RidePats.Add(rp);
            ride.Status = dr["statusRide"].ToString();
        }

        return ride.RidePats;
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
        //            rp.Target = new Destination();
        //            pat.DisplayName = row.ItemArray[1].ToString();
        //            pat.EscortedList = pat.getescortedsList(pat.DisplayName);
        //            rp.Pat = pat;
        //            rp.StartPlace.Name = row.ItemArray[2].ToString();
        //            rp.Target.Name = row.ItemArray[3].ToString();
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
}