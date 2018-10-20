using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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


    public bool OnlyEscort { get; set; }

    public List<Escorted> Escorts { get; set; }

    public int RideNum { get; set; }

    public List<Volunteer> Drivers { get; set; }

    public string Shift { get; set; }

    public List<string> Statuses { get; set; }

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

    public RidePat()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public int setRidePat(RidePat ridePat, string func)
    {
        DbService db = new DbService();
        SqlCommand cmd = new SqlCommand();
        cmd.CommandType = CommandType.Text;
        SqlParameter[] cmdParams = new SqlParameter[7];
        try
        {
            Pat = ridePat.Pat;
            Location origin = new Location();
            origin.Name = ridePat.Origin.Name;
            Location destination = new Location();
            destination.Name = ridePat.Destination.Name;
            Date = ridePat.Date;
            Coordinator = new Volunteer();
            Coordinator.DisplayName = ridePat.Coordinator.DisplayName;
            Remark = ridePat.Remark;
            Escorts = ridePat.Escorts;
            OnlyEscort = ridePat.OnlyEscort;

            cmdParams[0] = cmd.Parameters.AddWithValue("@pat", Pat.DisplayName);
            cmdParams[1] = cmd.Parameters.AddWithValue("@origin", origin.Name);
            cmdParams[2] = cmd.Parameters.AddWithValue("@destination", destination.Name);
            cmdParams[3] = cmd.Parameters.AddWithValue("@date", Date);
            cmdParams[4] = cmd.Parameters.AddWithValue("@remark", Remark);
            cmdParams[5] = cmd.Parameters.AddWithValue("@onlyEscort", OnlyEscort);
        }
        catch (Exception)
        {

        }

        if (func == "new") //Insert new RidePat to DB
        {
            cmdParams[6] = cmd.Parameters.AddWithValue("@coordinator", Coordinator.DisplayName);

            string query = "insert into RidePat (Patient,Origin,Destination,PickupTime,Coordinator,Remark,OnlyEscort) values (@pat,@origin,@destination,@date,@coordinator,@remark,@onlyEscort);SELECT SCOPE_IDENTITY();";
            RidePatNum = int.Parse(db.GetObjectScalarByQuery(query, cmd.CommandType, cmdParams).ToString());

            if (Escorts.Count > 0 && RidePatNum != 0)
            {
                string query2 = "";
                SqlCommand cmd2 = new SqlCommand();
                cmd2.CommandType = CommandType.Text;
                SqlParameter[] cmdParams2 = new SqlParameter[3];
                cmdParams2[0] = cmd2.Parameters.AddWithValue("@pat", Pat.Id);
                cmdParams2[1] = cmd2.Parameters.AddWithValue("@ridePatNum", RidePatNum);
                foreach (Escorted e in Escorts)
                {
                    cmdParams2[2] = cmd2.Parameters.AddWithValue("@Escort", e.Id);
                    query2 += "insert into [PatientEscort_PatientInRide (RidePat)] ([PatientEscortPatientId],[PatientEscortEscortId],[PatientInRide (RidePat)RidePatNum]) values (@pat,@Escort,@ridePatNum);";
                }
                DbService db2 = new DbService();
                try
                {
                    // db2.ExecuteQuery(query2);
                    db2.ExecuteQuery(query2, cmd2.CommandType, cmdParams2);
                }
                catch (Exception e)
                {
                    throw e;

                }
            }
        }
        else if (func == "edit") //Edit existing RidePat in DB
        {
            RidePatNum = ridePat.RidePatNum;
            string query = "select Status from RidePat where RidePatNum=" + RidePatNum;
            Status = db.GetObjectScalarByQuery(query).ToString();
            if (Status != "ממתינה לשיבוץ") throw new Exception("נסיעה זו כבר הוקצתה לנהג ואין אפשרות לערוך אותה");
            cmdParams[6] = cmd.Parameters.AddWithValue("@ridePatNum", RidePatNum);
            query = "update RidePat set Patient=@pat,Origin=@origin,Destination=@destination,PickupTime=@date,Remark=@remark,OnlyEscort=@onlyEscort where RidePatNum=@ridePatNum";
            int res = db.ExecuteQuery(query, cmd.CommandType, cmdParams);

            if (res > 0)
            {
                string query3 = "delete from [PatientEscort_PatientInRide (RidePat)] where [PatientInRide (RidePat)RidePatNum]=" + RidePatNum;
                DbService db3 = new DbService();
                res += db3.ExecuteQuery(query3);
            }

            if (Escorts.Count > 0 && res > 0)
            {


                string query2 = "";
                SqlCommand cmd2 = new SqlCommand();
                cmd2.CommandType = CommandType.Text;
                SqlParameter[] cmdParams2 = new SqlParameter[3];

                foreach (Escorted e in Escorts)
                {
                    cmdParams2[0] = cmd2.Parameters.AddWithValue("@pat", Pat.Id);
                    cmdParams2[1] = cmd2.Parameters.AddWithValue("@ridePatNum", RidePatNum);
                    cmdParams2[2] = cmd2.Parameters.AddWithValue("@Escort", e.Id);
                    query2 = "insert into [PatientEscort_PatientInRide (RidePat)] ([PatientEscortPatientId],[PatientEscortEscortId],[PatientInRide (RidePat)RidePatNum]) values (@pat,@Escort,@ridePatNum);";
                    DbService db2 = new DbService();
                    try
                    {
                        // db2.ExecuteQuery(query2);
                        res += db2.ExecuteQuery(query2, cmd2.CommandType, cmdParams2);
                    }
                    catch (Exception ex)
                    {
                        throw ex;

                    }
                }

            }
            return res;
        }
        else if (func == "delete")
        {


            RidePatNum = ridePat.RidePatNum;
            Ride r = new Ride();
            RidePat rp = GetRidePat(ridePatNum);
            if (rp.Drivers != null)
            {
                foreach (Volunteer driver in rp.Drivers)
                {
                    Message m = new Message();
                    m.cancelRide(RidePatNum, driver);
                }
            }


            //  m.cancelRide(ridePatNum, dr);
            db = new DbService();
            string query = "delete from [PatientEscort_PatientInRide (RidePat)] where [PatientInRide (RidePat)RidePatNum]=" + RidePatNum;
            int res = db.ExecuteQuery(query);

            db = new DbService();
            query = "delete from RidePat where RidePatNum=" + RidePatNum;
            res += db.ExecuteQuery(query);




            return res;

        }
        return RidePatNum;

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



    public RidePat GetRidePat(int ridePatNum)
    {
        string query = "select * from RPView where RidePatNum=" + ridePatNum;
        DbService db = new DbService();
        DataSet ds = db.GetDataSetByQuery(query);
        RidePat rp = new RidePat();
        rp.pat = new Patient();
        DataRow dr = ds.Tables[0].Rows[0];

        rp.RidePatNum = int.Parse(dr["RidePatNum"].ToString());
        rp.OnlyEscort = Convert.ToBoolean(dr["OnlyEscort"].ToString());
        rp.pat.DisplayName = dr["DisplayName"].ToString();
        rp.Drivers = new List<Volunteer>();
        if (dr["MainDriver"].ToString() != "")
        {
            Volunteer v1 = new Volunteer();
            v1.Id = int.Parse(dr["MainDriver"].ToString());
            v1.RegId = v1.GetVolunteerRegById(v1.Id);
            v1.DriverType = "Primary";
            rp.Drivers.Add(v1);

        }
        if (dr["secondaryDriver"].ToString() != "")
        {
            Volunteer v2 = new Volunteer();
            v2.Id = int.Parse(dr["secondaryDriver"].ToString());
            v2.RegId = v2.GetVolunteerRegById(v2.Id);
            v2.DriverType = "Secondary";
            rp.Drivers.Add(v2);
        }
        //rp.pat.EscortedList = new List<Escorted>();
        rp.Escorts = new List<Escorted>();
        Location origin = new Location();
        origin.Name = dr["Origin"].ToString();
        rp.Origin = origin;
        Location dest = new Location();
        dest.Name = dr["Destination"].ToString();
        rp.Destination = dest;
        rp.Area = dr["Area"].ToString();
        rp.Shift = dr["Shift"].ToString();
        rp.Date = Convert.ToDateTime(dr["PickupTime"].ToString());
        rp.Status = dr["Status"].ToString();
        rp.Coordinator = new Volunteer();
        rp.Coordinator.DisplayName = dr["Coordinator"].ToString();
        rp.Remark = dr["Remark"].ToString();
        string query2 = "select DisplayName from RidePatEscortView where RidePatNum=" + ridePatNum;
        DbService db2 = new DbService();
        DataSet ds2 = db2.GetDataSetByQuery(query2);
        foreach (DataRow r in ds2.Tables[0].Rows)
        {
            if (r["DisplayName"].ToString() != "")
            {
                Escorted e = new Escorted();
                e.DisplayName = r["DisplayName"].ToString();
                //rp.pat.EscortedList.Add(e);

                rp.Escorts.Add(e);
            }
        }

        return rp;

    }

    //public List<RidePat> GetAllRidePats()
    //{

    //}




    //This method is used for שבץ אותי
    public List<RidePat> GetRidePatView(int volunteerId) //VolunteerId - 1 means get ALL FUTURE ridePats // VolunteerId -2 means get ALL ridePats
    {
        List<Escorted> el = new List<Escorted>();
        string query = "";

        if (volunteerId == -1)
            query = "select * from RPView where PickupTime>= getdate()"; // Get ALL FUTURE RidePats, even if cancelled
        else if (volunteerId == -2)
            query = "select * from RPView"; //get ALL ridePats
        else
            query = "select * from RPView where (Status<>'הסתיימה' or Status<>'בוטלה')"; //Get ALL ACTIVE RidePats (used by mobile app)

        DbService db = new DbService();
        DataSet ds = db.GetDataSetByQuery(query);
        // Ride ride = new Ride();
        // ride.RidePats = new List<RidePat>();
        List<RidePat> rpl = new List<RidePat>();
        // int numOfDrivers = 0;

        string query2 = "select * from RidePatEscortView";
        DbService db2 = new DbService();
        try
        {
            DataSet ds2 = db2.GetDataSetByQuery(query2);

            foreach (DataRow dr2 in ds2.Tables[0].Rows)
            {
                Escorted e = new Escorted();
                e.Id = int.Parse(dr2["RidePatNum"].ToString());
                e.DisplayName = dr2["DisplayName"].ToString();
                el.Add(e);

            }

        }
        catch (Exception)
        {

        }
        int counter = 0;
        try
        {

            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                counter++;
                //if (dr["MainDriver"].ToString() == "" && dr["secondaryDriver"].ToString() == "")
                //{
                //    numOfDrivers = 0;
                //}
                //else if (dr["MainDriver"].ToString() != "" && dr["secondaryDriver"].ToString() == "")
                //{
                //    numOfDrivers = 1;

                //}
                //else numOfDrivers = 2;

                if ((dr["MainDriver"].ToString() != "" && int.Parse(dr["MainDriver"].ToString()) == volunteerId) || (dr["secondaryDriver"].ToString() != "" && int.Parse(dr["secondaryDriver"].ToString()) == volunteerId))
                    continue;

                RidePat rp = new RidePat();
                rp.Coordinator = new Volunteer();
                rp.Coordinator.DisplayName = dr["Coordinator"].ToString();
                rp.Drivers = new List<Volunteer>();
                // if (numOfDrivers != 0)
                //  {
                if (dr["MainDriver"].ToString() != "")
                {

                    Volunteer primary = new Volunteer();
                    primary.DriverType = "Primary";

                    primary.Id = int.Parse(dr["MainDriver"].ToString());
                    string query3 = "select DisplayName,CellPhone from Volunteer where Id=" + primary.Id;
                    DbService db3 = new DbService();
                    //primary.DisplayName = db3.GetObjectScalarByQuery(query3).ToString();
                    DataSet driverSet = db3.GetDataSetByQuery(query3);
                    DataRow driverRow = driverSet.Tables[0].Rows[0];
                    primary.DisplayName = driverRow["DisplayName"].ToString();
                    primary.CellPhone = driverRow["CellPhone"].ToString();
                    rp.Drivers.Add(primary);
                }


                // if (numOfDrivers > 1)
                // {
                if (dr["secondaryDriver"].ToString() != "")
                {
                    Volunteer secondary = new Volunteer();
                    secondary.DriverType = "secondary";
                    secondary.Id = int.Parse(dr["secondaryDriver"].ToString());
                    string query4 = "select DisplayName,Cellphone from Volunteer where Id=" + secondary.Id;
                    DbService db4 = new DbService();
                    // secondary.DisplayName = db4.GetObjectScalarByQuery(query4).ToString();
                    DataSet driverSet = db4.GetDataSetByQuery(query4);
                    DataRow driverRow = driverSet.Tables[0].Rows[0];
                    secondary.DisplayName = driverRow["DisplayName"].ToString();
                    secondary.CellPhone = driverRow["CellPhone"].ToString();
                    rp.Drivers.Add(secondary);
                }
                //  }
                //    }

                rp.RidePatNum = int.Parse(dr["RidePatNum"].ToString());
                try
                {
                    rp.RideNum = int.Parse(dr["RideNum"].ToString());
                }
                catch (Exception)
                {

                }

                rp.pat = new Patient();

                rp.pat.DisplayName = dr["DisplayName"].ToString();
                rp.pat.CellPhone = dr["CellPhone"].ToString();
                rp.pat.Equipment = rp.Pat.getEquipmentForPatient(rp.pat.DisplayName);
                rp.pat.EscortedList = new List<Escorted>();
                foreach (Escorted e in el)
                {
                    if (e.Id == rp.RidePatNum)
                    {
                        rp.pat.EscortedList.Add(e);
                    }
                }


                Location origin = new Location();
                origin.Name = dr["Origin"].ToString();
                rp.Origin = origin;
                Location dest = new Location();
                dest.Name = dr["Destination"].ToString();
                rp.Destination = dest;
                rp.Area = dr["Area"].ToString();
                rp.Shift = dr["Shift"].ToString();
                rp.Date = Convert.ToDateTime(dr["PickupTime"].ToString());
                rp.Status = dr["Status"].ToString();
                if (rp.RideNum > 0) // if RidePat is assigned to a Ride - Take the Ride's status
                {
                    query = "select statusStatusName from status_Ride where RideRideNum=" + rp.RideNum + " order by Timestamp desc";
                    db = new DbService();
                    rp.Statuses = new List<string>();
                    foreach (DataRow status in db.GetDataSetByQuery(query).Tables[0].Rows)
                    {
                        rp.Statuses.Add(status.ItemArray[0].ToString());
                    }
                    rp.Status = rp.Statuses[0];

                }
                rpl.Add(rp);

            }

            return rpl;
        }
        catch (Exception)
        {
            throw;
        }
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

    public int AssignRideToRidePat(int ridePatId, int userId, string driverType)
    {
        int RideId = -1;

        string query = "select RideNum,Origin,Destination,PickupTime,Status,MainDriver,secondaryDriver from RPView where RidePatNum=" + ridePatId;
        DbService db = new DbService();
        DataSet ds = db.GetDataSetByQuery(query);
        if (ds.Tables[0].Rows.Count == 0)
        {
            //There is no RidePat with that ID
            throw new Exception("הנסיעה אליה נרשמתם בוטלה");
        }
        DataRow dr = ds.Tables[0].Rows[0];

        Origin = new Location();
        Origin.Name = dr["Origin"].ToString();
        Destination = new Location();
        Destination.Name = dr["Destination"].ToString();
        Date = Convert.ToDateTime(dr["PickupTime"].ToString());
        if (dr["Status"].ToString() == "שובץ נהג וגיבוי") throw new Exception("הנסיעה אליה נרשמתם כבר מלאה");

        if (dr["RideNum"].ToString() != "") //Ride aleady exists
        {
            RideId = int.Parse(dr["RideNum"].ToString());


            if (dr["MainDriver"].ToString() == "") //No main driver is assigned to this ride
            {
                if (driverType == "primary")
                    query = "update Ride set MainDriver=" + userId + " where RideNum=" + RideId; //assign a main driver to this ride
            }
            //A main driver IS assigned to this ride
            else //if (dr["MainDriver"].ToString() != userId.ToString()) //Check that the current user is not already assigned as primary to this ride
            {
                if (driverType == "primary") throw new Exception("לנסיעה זו כבר שובץ נהג ראשי. באפשרותכם להירשם אליה כגיבוי"); //The driver asked to be assigned as primary and there already is a primary

                if (dr["secondaryDriver"].ToString() != "") throw new Exception("הנסיעה אליה נרשמתם כבר מלאה"); //The driver asked to be assigned as secondary and there already is a secondary

                query = "update Ride set secondaryDriver=" + userId + " where RideNum=" + RideId; //Assign a secondary driver to this ride
            }
            DbService db4 = new DbService();
            int res = db4.ExecuteQuery(query);
            if (res <= 0) return -1;
        }
        else // New ride
        {
            SqlCommand cmd = new SqlCommand();
            SqlParameter[] cmdparams = new SqlParameter[4];
            cmdparams[0] = cmd.Parameters.AddWithValue("@origin", Origin.Name);
            cmdparams[1] = cmd.Parameters.AddWithValue("@dest", Destination.Name);
            cmdparams[2] = cmd.Parameters.AddWithValue("@date", Date);
            cmdparams[3] = cmd.Parameters.AddWithValue("@mainDriver", userId);

            string query2 = "set dateformat dmy; insert into Ride (Origin,Destination,Date,MainDriver) values (@origin,@dest,@date,@mainDriver) SELECT SCOPE_IDENTITY()";
            DbService db2 = new DbService();
            RideId = int.Parse(db2.GetObjectScalarByQuery(query2, cmd.CommandType, cmdparams).ToString());
            if (RideId <= 0) return -1;
            string query3 = "update RidePat set RideId=" + RideId + " where RidePatNum=" + ridePatId;
            DbService db3 = new DbService();
            int res = db3.ExecuteQuery(query3);
            if (res <= 0) return -1;
        }




        return RideId;

    }

    public int LeaveRidePat(int ridePatId, int rideId, int driverId)
    {
        int res = -1;
        string driver = "";
        string query4 = "select MainDriver,secondaryDriver from Ride where RideNum=" + rideId;
        DbService db4 = new DbService();
        DataSet ds2 = db4.GetDataSetByQuery(query4);
        DataRow dr = ds2.Tables[0].Rows[0];
        if (dr["MainDriver"].ToString() == driverId.ToString())
        {
            driver = "MainDriver";
        }
        else if (dr["secondaryDriver"].ToString() == driverId.ToString())
        {
            driver = "secondaryDriver";
        }


        if (driver == "secondaryDriver")
        {
            string query = "update Ride set secondaryDriver=null where RideNum=" + rideId;
            DbService db = new DbService();
            res = db.ExecuteQuery(query);
        }
        else
        {
            // string query = "update RidePat set RideId=null where RidePatNum=" + ridePatId; //+"; update Ride set "+driver+" =null where RideNum="+rideId;
            string query = "update Ride set MainDriver=null where RideNum=" + rideId;
            DbService db = new DbService();
            res = db.ExecuteQuery(query);
        }

        //string query2 = "select RidePatNum from RidePat where RideId=" + rideId;
        //DbService db2 = new DbService();
        //DataSet ds = db2.GetDataSetByQuery(query2);
        //if (ds.Tables[0].Rows.Count == 0)
        //{
        //    string query3 = "update Ride set " + driver + " =null where RideNum=" + rideId;
        //    DbService db3 = new DbService();
        //    res += db3.ExecuteQuery(query3);
        //}
        return res;

    }

    public int DeleteDriver(int ridePatId, int driverId)
    {
        int res = -1;
        bool primary = false;
        int rideNum = -1;
        int BackupDriver = -1;
        DbService db = new DbService();
        string query = "select * from RPView where RidePatNum=" + ridePatId;
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
            query = "select RideNum from RPView where RidePatNum=" + ridePatId;
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