﻿using Microsoft.AspNet.SignalR;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
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
    DateTime? lastModified;
    DbService dbs;
    SqlDataReader sdr;

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

    public DateTime? LastModified
    {
        get
        {
            return lastModified;
        }

        set
        {
            lastModified = value;
        }
    }
    public RidePat()
    {
        //
        // TODO: Add constructor logic here
        //
    }
    private string NEWCheckLocationForRidepatArea(string origin, string destination)
    {
        //ארז – ירושלים
        //ארז - מרכז
        //ארז - צפון
        //תרקומיא – מרכז
        //אזור המרכז
        //מרכז - ירושלים
        //מרכז - צפון
        //אזור הצפון
        Location l = new Location();
        string originArea = l.GetAreaForPoint(origin);
        string destinationArea = l.GetAreaForPoint(destination);
        List<string> allAreas = l.getAreas();
        string rideArea = originArea + " - " + destinationArea;
        if (originArea == destinationArea)
        {
            rideArea = originArea;
        }
        else if (!allAreas.Contains(rideArea))
        {
            rideArea = destinationArea + " - " + originArea;
            if (!allAreas.Contains(rideArea))
            {
                throw new ArgumentException("area not undefined");
            }
        }
        return rideArea;
    }
    //need to change....
    private string CheckLocationForRidepatArea(string origin, string destination)
    {
        //ארז – ירושלים
        //ארז - מרכז
        //ארז - צפון
        //תרקומיא – מרכז
        //אזור המרכז
        //מרכז - ירושליים
        //צפון - מרכז
        //אזור הצפון


        //        דרום
        //מרכז
        //מרכז - ירושליים
        //מרכז - דרום
        //צפון
        //צפון - מרכז

        //get locations area
        Location l = new Location();
        string originArea = l.GetAreaForPoint(origin);
        string destinationArea = l.GetAreaForPoint(destination);

        string areaForRidepat = originArea;
        if (origin == "ארז")
        {
            switch (destinationArea)
            {
                case "מרכז":
                case "מרכז-דרום":
                    areaForRidepat = "ארז - מרכז";
                    break;
                case "מרכז - ירושליים":
                case "דרום":
                    areaForRidepat = "ארז – ירושלים";
                    break;
                case "צפון":
                case "צפון-מרכז":
                    areaForRidepat = "ארז - צפון";
                    break;
                default:
                    areaForRidepat = originArea; ;
                    break;
            }
        }
        else if (destination == "ארז")
        {
            switch (originArea)
            {
                case "מרכז":
                case "מרכז-דרום":
                case "צפון-מרכז":
                    areaForRidepat = "ארז - מרכז";
                    break;
                case "מרכז - ירושליים":
                case "דרום":
                    areaForRidepat = "ארז – ירושלים";
                    break;
                case "צפון":
                    areaForRidepat = "ארז - צפון";
                    break;
                default:
                    areaForRidepat = originArea; ;
                    break;
            }
        }
        else if (origin == "תרקומיא" || destination == "תרקומיא")
        {
            areaForRidepat = "תרקומיא – מרכז";
        }
        else if ((originArea == "מרכז" && destinationArea == "צפון") || (destinationArea == "מרכז" && originArea == "צפון"))
        {
            areaForRidepat = "צפון-מרכז";
        }
        return areaForRidepat;
    }

    public List<RidePat> GetVolunteersDocumentedRides(int volunteerId)
    {
        string query = "exec spRideAndRidePat_GetVolunteersRideHistory @volunteerID =" + volunteerId;
        List<RidePat> thisVolunteerRides = new List<RidePat>();
        RidePat ridePat;
        try
        {
            dbs = new DbService();
            SqlDataReader sdr = dbs.GetDataReader(query);
            while (sdr.Read())
            {
                ridePat = new RidePat();
                ridePat.RidePatNum = Convert.ToInt32(sdr["RidePatNum"]);
                ridePat.Pat = new Patient(Convert.ToString(sdr["Patient"]));
                ridePat.Remark = Convert.ToString(sdr["Remark"]);
                ridePat.Origin = new Location(Convert.ToString(sdr["Origin"]));
                ridePat.Destination = new Location(Convert.ToString(sdr["Destination"]));
                ridePat.Date = Convert.ToDateTime(sdr["PickupTime"]);
                thisVolunteerRides.Add(ridePat);
            }
            return thisVolunteerRides;
        }
        catch (Exception ex)
        {
            throw new Exception("Exception in RidePat.cs → GetVolunteersRideHistory " + ex.Message);
        }
        finally
        {
            dbs.CloseConnection();
        }
    }

    //לשנות את  isAnonymous

    public int setRidePat(RidePat ridePat, string func, bool isAnonymous, int numberOfRides, string repeatRideEvery)
    {
        DateTime timeRightNow = DateTime.Now;
        DbService db = new DbService();
        SqlCommand cmd = new SqlCommand();
        cmd.CommandType = CommandType.Text;
        SqlParameter[] cmdParams = new SqlParameter[10];
        bool sendMessage = true;

        try
        {
            Pat = ridePat.Pat;
            Location origin = new Location();
            origin.Name = ridePat.Origin.Name;
            Location destination = new Location();
            destination.Name = ridePat.Destination.Name;
            Area = NEWCheckLocationForRidepatArea(origin.Name, destination.Name);
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
            cmdParams[7] = cmd.Parameters.AddWithValue("@Area", Area);
            cmdParams[9] = cmd.Parameters.AddWithValue("@lastModified", ridePat.LastModified);

        }
        catch (Exception ex)
        {

        }

        if (func == "new") //Insert new RidePat to DB
        {
            List<DateTime> listOfDatesAfterUTCfix = numberOfRides > 1 ?
                BuildFutureRidesDates(Date, repeatRideEvery, numberOfRides)
                : new List<DateTime>() { Date };

            for (int i = 0; i < numberOfRides; i++)
            {
                ridePat.Date = listOfDatesAfterUTCfix[i];
                cmdParams[3] = cmd.Parameters.AddWithValue("@date", listOfDatesAfterUTCfix[i]);

                RidePat ridePatView = CheckRidePat_V2(ridePat, isAnonymous);

                if (ridePatView.RidePatNum != 0 && ridePatView.Status != "נמחקה" && !isAnonymous)
                {
                    return 1; // there is an issue - don't create new drive 
                }
                else if (ridePatView.Status == "נמחקה")
                {
                    // recover the ridepat from נמחקה to ממתינה לשיבוץ
                    changeRidePatStatus("ממתינה לשיבוץ", ridePatView.ridePatNum.ToString());
                    ridePat.ridePatNum = ridePatView.ridePatNum;

                    //recursive call ↓ -  YOGEV
                    setRidePat(ridePat, "edit", isAnonymous, numberOfRides, repeatRideEvery); //when recover a ridepat we should consider all the new parameters
                    //recursive call ↑ -  YOGEV

                    //multipling in -1 = case ==>  נסיעה שוחזרה
                    return -1 * ridePatView.ridePatNum;
                }

                cmdParams[6] = cmd.Parameters.AddWithValue("@coordinator", Coordinator.DisplayName);
                User u = new User();
                string CoordinatorID = u.getIdByUserName(Coordinator.DisplayName);
                cmdParams[8] = cmd.Parameters.AddWithValue("@coordinatorID", CoordinatorID);

                string query = "insert into RidePat (Patient,Origin,Destination,PickupTime,Coordinator,Remark,OnlyEscort,Area,CoordinatorId,lastModified) values (@pat,@origin,@destination,@date,@coordinator,@remark,@onlyEscort,@Area,@coordinatorID,@lastModified);SELECT SCOPE_IDENTITY();";
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
                        query2 = "insert into [PatientEscort_PatientInRide (RidePat)] ([PatientEscortPatientId],[PatientEscortEscortId],[PatientInRide (RidePat)RidePatNum]) values (@pat,@Escort,@ridePatNum);";
                        DbService db2 = new DbService();
                        try
                        {
                            // db2.ExecuteQuery(query2);
                            db2.ExecuteQuery(query2, cmd2.CommandType, cmdParams2);
                        }
                        catch (Exception ex)
                        {
                            throw ex;

                        }
                    }
                }

            }

        }
        else if (func == "edit") //Edit existing RidePat in DB
        {
            RidePatNum = ridePat.RidePatNum;
            RidePat ridePatPreEdit = GetRidePat(RidePatNum);
            bool isTargetPatientAnonumous = isAnonymous;
            if (ridePatPreEdit.Pat.IsAnonymous == bool.TrueString && !isTargetPatientAnonumous)
            {
                RidePat ridePatView = CheckRidePat_V2(ridePat, isAnonymous);
                if (ridePatView.RidePatNum != 0 && !isAnonymous)
                {
                    return 1; // there is an issue - don't create new drive 
                }
            }

            //SET THE COORDINATOR NAME IN RIDEPAT TABLE TO THE LAST ONE WHO TOUCHED THIS RIDEPAT 
            ChangeCoordinatoor(RidePatNum);

            if (ridePatPreEdit.Pat.DisplayName == ridePat.Pat.DisplayName && ridePatPreEdit.Origin.Name == ridePat.Origin.Name && ridePatPreEdit.Destination.Name == ridePat.Destination.Name && ridePatPreEdit.Date.TimeOfDay == ridePat.Date.TimeOfDay)
            {
                sendMessage = false;
            }

            cmdParams[6] = cmd.Parameters.AddWithValue("@ridePatNum", RidePatNum);

            cmdParams[8] = cmd.Parameters.AddWithValue("@coordinatorID", -1); //THIS IS JUST SO YOU DONT GET EXCEPTION

            string query = "update RidePat set Patient=@pat,Origin=@origin,Destination=@destination,PickupTime=@date,Remark=@remark,OnlyEscort=@onlyEscort,Area=@Area,lastModified=@lastModified where RidePatNum=@ridePatNum ";
            query += "DECLARE @RIDEID INT = (select RideId from RidePat where RidePatNum=@ridePatNum) ";
            query += "if (select RideNum from ride where RideNum=@RIDEID) is not null ";
            query += "begin ";
            query += "update ride set Origin=@origin, Destination=@destination where RideNum=@RIDEID ";
            query += "end";

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
            RidePatNum = ridePat.RidePatNum;
            Ride r = new Ride();
            RidePat rp = GetRidePat(RidePatNum);

            if (isAnonymous && Pat.DisplayName.IndexOf("אנונימי") == -1 && (Date > timeRightNow))
            {

                foreach (Volunteer driver in rp.Drivers)
                {
                    Message m = new Message();
                    m.changeAnonymousPatient(RidePatNum, driver);
                }

            }
            else if (rp.Drivers != null && (Date > timeRightNow) && sendMessage)
            {
                foreach (Volunteer driver in rp.Drivers)
                {
                    Message m = new Message();
                    m.changeInRide(RidePatNum, driver);
                }
            }

            return RidePatNum;
        }
        else if (func == "delete")
        {

            RidePatNum = ridePat.RidePatNum;
            Ride r = new Ride();
            RidePat rp = GetRidePat(ridePatNum);
            bool anotherRide;
            if (rp.RideNum == -1)
            {
                anotherRide = false;
            }
            else anotherRide = IsThereAnotherRidePat(rp);
            if (rp.Drivers != null && (Date > timeRightNow))
            {
                foreach (Volunteer driver in rp.Drivers)
                {
                    Message m = new Message();
                    if (anotherRide)
                    {
                        m.cancelOneRide(RidePatNum, driver);
                    }
                    else
                    {
                        m.cancelRide(RidePatNum, driver);
                    }

                }
            }
            if (!isAnonymous && (Date > timeRightNow))
            {
                Message m = new Message();
                if (rp.Drivers.Count >= 1)
                {
                    m.coordinatorCanceledRide(RidePatNum, rp.Drivers[0]);
                }
                else m.coordinatorCanceledRide(RidePatNum, null);
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

    private List<DateTime> BuildFutureRidesDates(DateTime date, string repeatRideEvery, int numberOfRides)
    {
        List<DateTime> listOfDatesAfterUTCfix = new List<DateTime>();
        listOfDatesAfterUTCfix.Add(date);
        DateTime firstDate = date;

        DateTime dateAfterIncrement;
        for (int i = 1; i < numberOfRides; i++)
        {
            dateAfterIncrement = repeatRideEvery == "כל שבוע" ? date.AddDays(7) : date.AddDays(1);
            listOfDatesAfterUTCfix.Add(dateAfterIncrement);
            date = listOfDatesAfterUTCfix[i];
        }

        TimeZoneInfo israelTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Israel Standard Time");
        bool isFirstRideDateDayLightSaving = israelTimeZone.IsDaylightSavingTime(firstDate);

        for (int i = 1; i < listOfDatesAfterUTCfix.Count; i++)
        {
            if (isFirstRideDateDayLightSaving && !israelTimeZone.IsDaylightSavingTime(listOfDatesAfterUTCfix[i]))
            {
                listOfDatesAfterUTCfix[i] = listOfDatesAfterUTCfix[i].AddHours(1);
            }
            else if (!isFirstRideDateDayLightSaving && israelTimeZone.IsDaylightSavingTime(listOfDatesAfterUTCfix[i]))
            {
                listOfDatesAfterUTCfix[i] = listOfDatesAfterUTCfix[i].AddHours(-1);
            }
        }
        return listOfDatesAfterUTCfix;
    }
    public bool IsThereAnotherRidePat(RidePat rp)
    {
        //TimeZoneInfo sourceTimeZone = TimeZoneInfo.FindSystemTimeZoneById("UTC");
        var date = rp.Date.ToString("yyyy-MM-dd");
        var time = rp.Date.ToShortTimeString();
        Volunteer v = new Volunteer();
        int c = rp.Drivers.Count;
        if (c != 0)
        {
            v = rp.Drivers[0];
            if (v == null)
            {
                v = rp.Drivers[1];
            }
        }


        string query = "select * from rpview where Origin=N'" + rp.Origin.Name.Replace("'", "''") + "' and Destination=N'" + rp.Destination.Name.Replace("'", "''") + "' and pickuptime='" + date + " " + time + "' and (MainDriver=" + v.Id + " or SecondaryDriver=" + v.Id + ")";
        DbService db = new DbService();
        DataSet ds = db.GetDataSetByQuery(query);
        if (ds.Tables[0].Rows.Count == 1)
        {
            return false;
        }
        return true;
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

    public RidePat CheckRidePat_V2(RidePat ridePat, bool isAnonymous)
    {
        string[] date = ridePat.Date.ToString().Split(' ');
        DateTime date1 = Convert.ToDateTime(date[0]);
        if (!isAnonymous)
        {
            string origin = ridePat.Origin.Name.Replace("'", "''");
            string dest = ridePat.Destination.Name.Replace("'", "''");
            string patName = ridePat.Pat.DisplayName.Replace("'", "''");

            string query = "select * from RPView where Origin=N'" + origin + "' and Destination=N'" + dest + "' and cast(PickupTime as date)='" + date1.ToString("yyyy-MM-dd") + "' and DisplayName=N'" + patName + "'";
            DbService db = new DbService();
            DataSet ds = db.GetDataSetByQuery(query);
            if (ds.Tables[0].Rows.Count == 0)
            {
                return new RidePat();
            }
            DataRow dr = ds.Tables[0].Rows[0];

            RidePat rp2return = new RidePat();

            rp2return.RidePatNum = Convert.ToInt32(dr["RidePatNum"]);
            rp2return.Status = Convert.ToString(dr["Status"]);
            return rp2return;
        }
        else return new RidePat();
    }

    /*Yogev - there is no more need in this method
    public bool CheckRidePat(RidePat ridePat, bool isAnonymous)
    {
        string[] date = ridePat.Date.ToString().Split(' ');
        DateTime date1 = Convert.ToDateTime(date[0]);
        if (!isAnonymous)
        {
            string origin = ridePat.Origin.Name.Replace("'", "''");
            string dest = ridePat.Destination.Name.Replace("'", "''");
            string patName = ridePat.Pat.DisplayName.Replace("'", "''");

            string query = "select * from RPView where Origin=N'" + origin + "' and Destination=N'" + dest + "' and cast(PickupTime as date)='" + date1.ToString("yyyy-MM-dd") + "' and DisplayName=N'" + patName + "'";
            DbService db = new DbService();
            DataSet ds = db.GetDataSetByQuery(query);
            if (ds.Tables[0].Rows.Count == 0)
            {
                return false;
            }
            return true;
        }
        else return false;
    }

     */
    public RidePat GetRidePat(int ridePatNum)
    {
        try
        {


            Location tmp = new Location();
            Hashtable locations = tmp.getLocationsEnglishName();
            string query = "select * from RPView where RidePatNum=" + ridePatNum;
            DbService db = new DbService();
            DataSet ds = db.GetDataSetByQuery(query);
            RidePat rp = new RidePat();
            rp.pat = new Patient();
            DataRow dr = ds.Tables[0].Rows[0];

            rp.RidePatNum = int.Parse(dr["RidePatNum"].ToString());
            if (dr["RideNum"].ToString() != null)
            {
                if (dr["RideNum"].ToString() != "")
                {
                    rp.RideNum = int.Parse(dr["RideNum"].ToString());
                }
            }
            else rp.RideNum = -1;
            rp.OnlyEscort = Convert.ToBoolean(dr["OnlyEscort"].ToString());
            rp.Pat = rp.Pat.GetPatientById(Convert.ToInt32(dr["Id"]), ridePatNum);
            rp.Drivers = new List<Volunteer>();
            if (dr["MainDriver"].ToString() != "")
            {
                Volunteer mainDriver = new Volunteer();
                mainDriver.Id = int.Parse(dr["MainDriver"].ToString());
                mainDriver = mainDriver.getVolunteerByID(mainDriver.Id);
                mainDriver.RegId = mainDriver.GetVolunteerRegById(mainDriver.Id);
                int numberOfRides = 0;
                int.TryParse(dr["NoOfDocumentedRides"].ToString(), out numberOfRides);
                mainDriver.NoOfDocumentedRides = numberOfRides;
                mainDriver.DriverType = "Primary";

                if (dr["IsNewDriver"].ToString() != "")
                {
                    mainDriver.IsNewDriver = dr["IsNewDriver"].ToString() == "1";
                }

                rp.Drivers.Add(mainDriver);
            }

            //NOT A SUPPORTED FEATCHER ANY MORE
            //if (dr["secondaryDriver"].ToString() != "")
            //{
            //    Volunteer v2 = new Volunteer();
            //    v2.Id = int.Parse(dr["secondaryDriver"].ToString());
            //    v2.RegId = v2.GetVolunteerRegById(v2.Id);
            //    v2.DriverType = "Secondary";
            //    rp.Drivers.Add(v2);
            //}

            Location origin = new Location();
            origin.Name = dr["Origin"].ToString();
            origin = origin.getLocation();
            if (locations[origin.Name] == null)
            {
                origin.EnglishName = "";
            }
            else origin.EnglishName = locations[origin.Name].ToString();
            rp.Origin = origin;

            Location dest = new Location();
            dest.Name = dr["Destination"].ToString();
            dest = dest.getLocation();
            if (locations[dest.Name] == null)
            {
                dest.EnglishName = "";
            }
            else dest.EnglishName = locations[dest.Name].ToString();
            rp.Destination = dest;
            rp.Area = dr["Area"].ToString();
            rp.Shift = dr["Shift"].ToString();
            rp.Date = Convert.ToDateTime(dr["PickupTime"].ToString());
            rp.Status = dr["Status"].ToString();
            rp.Coordinator = new Volunteer();
            rp.Coordinator.DisplayName = dr["Coordinator"].ToString();
            rp.Remark = dr["Remark"].ToString();
            rp.LastModified =
                                String.IsNullOrEmpty(dr["lastModified"].ToString()) ? null :
                                (DateTime?)Convert.ToDateTime(dr["lastModified"].ToString());

            string query2 = "select DisplayName,Id from RidePatEscortView where RidePatNum=" + ridePatNum;
            DbService db2 = new DbService();
            DataSet ds2 = db2.GetDataSetByQuery(query2);
            rp.Escorts = new List<Escorted>();

            //rp.pat.EscortedList = new List<Escorted>();
            foreach (DataRow r in ds2.Tables[0].Rows)
            {
                if (r["DisplayName"].ToString() != "")
                {
                    Escorted e = new Escorted();
                    e.DisplayName = r["DisplayName"].ToString();
                    e.Id = (int)r["Id"];


                    /// rp.pat.EscortedList are the escorts a patient has in general
                    /// &
                    /// rp.Escorts are escorts in this particular ridepat
                    ///


                    //rp.pat.EscortedList.Add(e);
                    rp.Escorts.Add(e);
                }
            }

            return rp;
        }
        catch (Exception ex)
        {
            throw ex;
        }

    }


    //public List<RidePat> GetAllRidePats()
    //{

    //}

    private DataTable getDriver()
    {
        DbService db = new DbService();
        SqlCommand cmd = new SqlCommand();
        //SqlParameter[] cmdparams= new SqlParameter[1];
        cmd.CommandType = CommandType.Text;
        //cmdparams[0] = cmd.Parameters.AddWithValue("id", id);
        string query = "select Id, DisplayName, CellPhone, EnglishFN, EnglishLN, NoOfDocumentedRides from Volunteer";
        DataSet ds = db.GetDataSetByQuery(query);
        DataTable dt = ds.Tables[0];
        return dt;
    }

    private DataTable GetDriver_V2()
    {
        DbService db = new DbService();
        SqlCommand cmd = new SqlCommand();
        cmd.CommandType = CommandType.Text;
        string query = "EXEC spVolunteer_GetDrivers @isActive=1, @isDriving=1";
        DataSet ds = db.GetDataSetByQuery(query);
        DataTable dt = ds.Tables[0];

        return dt;
    }

    private DataTable getRideStatus()
    {
        DbService db = new DbService();
        SqlCommand cmd = new SqlCommand();
        //SqlParameter[] cmdparams= new SqlParameter[1];
        cmd.CommandType = CommandType.Text;
        //cmdparams[0] = cmd.Parameters.AddWithValue("id", id);
        string query = "select Id,DisplayName,CellPhone from Volunteer";
        DataSet ds = db.GetDataSetByQuery(query);
        DataTable dt = ds.Tables[0];
        return dt;
    }

    private DataTable getEquipment()
    {
        DbService db = new DbService();
        SqlCommand cmd = new SqlCommand();
        //SqlParameter[] cmdparams= new SqlParameter[1];
        cmd.CommandType = CommandType.Text;
        //cmdparams[0] = cmd.Parameters.AddWithValue("id", id);
        string query = "select * from EquipmentForPatientView";
        DataSet ds = db.GetDataSetByQuery(query);
        DataTable dt = ds.Tables[0];
        return dt;
    }

    private DataTable getRides()
    {
        DbService db = new DbService();
        SqlCommand cmd = new SqlCommand();
        //SqlParameter[] cmdparams= new SqlParameter[1];
        cmd.CommandType = CommandType.Text;
        //cmdparams[0] = cmd.Parameters.AddWithValue("id", id);
        string query = "select * from status_Ride order by Timestamp asc";
        DataSet ds = db.GetDataSetByQuery(query);
        DataTable dt = ds.Tables[0];
        return dt;
    }

    private DataTable getEscorts()
    {
        DbService db = new DbService();
        SqlCommand cmd = new SqlCommand();
        //SqlParameter[] cmdparams= new SqlParameter[1];
        cmd.CommandType = CommandType.Text;
        //cmdparams[0] = cmd.Parameters.AddWithValue("id", id);
        string query = "select * from RidePatEscortView";
        DataSet ds = db.GetDataSetByQuery(query);
        DataTable dt = ds.Tables[0];
        return dt;
    }


    //This method is used for שבץ אותי in mobile app & manageRidepats
    public List<RidePat> GetRidePatView(int volunteerId, int maxDays) //VolunteerId - 1 means get ALL FUTURE ridePats // VolunteerId -2 means get ALL ridePats
    {

        Location tmp = new Location();
        Hashtable locations = tmp.getLocationsEnglishName();
        DataTable driverTable = GetDriver_V2();
        DataTable equipmentTable = getEquipment();
        DataTable rideTable = getRides();
        DataTable escortTable = getEscorts();
        List<Escorted> el = new List<Escorted>();
        string query = "";

        if (volunteerId == -1)
        {
            if (maxDays == -1)
            {

                //query = "select * from RPView where Convert(date,pickuptime)>=CONVERT(date, getdate()) and Status!=N'הגענו ליעד';"; // Get ALL FUTURE RidePats, even if cancelled
                query = "select * from RPView where Convert(date,pickuptime)>=CONVERT(date, getdate()) and Status <> N'נמחקה'; "; // Get ALL FUTURE RidePats, even if cancelled

            }

            //else query = "select * from RPView where DATEDIFF(day,getdate(),pickuptime)<=" + maxDays + " and Convert(date,pickuptime)>=CONVERT(date, getdate()) and Status!=N'הגענו ליעד';";
            else query = "select * from RPView where DATEDIFF(day,getdate(),pickuptime)<=" + maxDays + " and Convert(date,pickuptime)>=CONVERT(date, getdate()) and Status <> N'נמחקה'; ";
        }
        else if (volunteerId == -2)
        {
            if (maxDays == -2) // get this week
            {
                var thisSunday = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);

                var endDate = thisSunday.AddDays(7).Date;

                string thisSundayString = thisSunday.ToString("yyyy-MM-dd");
                string endDateString = endDate.ToString("yyyy-MM-dd");

                query = "select * from RPView where pickuptime >= Convert(datetime,'" + thisSundayString + "') AND pickuptime < Convert(datetime,'" + endDateString + "') and Status <> N'נמחקה';";
            }
            else if (maxDays == -3) // get next week
            {
                var thisSunday = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);

                var nextSunday = thisSunday.AddDays(7).Date;

                var endDate = nextSunday.AddDays(7).Date;

                string nextSundayString = nextSunday.ToString("yyyy-MM-dd");
                string endDateString = endDate.ToString("yyyy-MM-dd");

                query = "select * from RPView where pickuptime >= Convert(datetime,'" + nextSundayString + "') AND pickuptime < Convert(datetime,'" + endDateString + "') and Status <> N'נמחקה';";
            }
            //Yogev ↓
            else query = "select * from RPView where PickupTime between getDate()-100 and getDate()+50 and Status <> N'נמחקה'";
            // query = "select * from RPView"; //get ALL ridePats - MAKES THE WEB APP REALY SLOW
            //Yogev ↑
        }
        else
        {
            //query = "select * from RPView where (Status<>N'הסתיימה' or Status<>N'בוטלה') and PickupTime>= getdate()"; //Get ALL ACTIVE RidePats (used by mobile app)
            if (maxDays != -1)
            {
                query = "select * from RPView where (Status=N'שובץ נהג' or Status=N'ממתינה לשיבוץ' or Status=N'שובץ גיבוי') and DATEDIFF(day,getdate(),pickuptime)<=" + maxDays + " and pickuptime>=getdate() and Status <> N'נמחקה'"; //Get ALL ACTIVE RidePats (used by mobile app) where max days=30
            }
            else query = "select * from RPView where (Status=N'שובץ נהג' or Status=N'ממתינה לשיבוץ' or Status=N'שובץ גיבוי') and PickupTime>= getdate() and Status <> N'נמחקה'"; //Get ALL ACTIVE RidePats (used by mobile app)
        }
        DbService db = new DbService();
        DataSet ds = db.GetDataSetByQuery(query);
        // Ride ride = new Ride();
        // ride.RidePats = new List<RidePat>();
        List<RidePat> rpl = new List<RidePat>();

        int counter = 0;
        try
        {
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                try
                {
                    counter++;
                    // if voluneer is assigned to this ridePat, don't return it (for mobile app)
                    if ((dr["MainDriver"].ToString() != "" && int.Parse(dr["MainDriver"].ToString()) == volunteerId) || (dr["secondaryDriver"].ToString() != "" && int.Parse(dr["secondaryDriver"].ToString()) == volunteerId))
                        continue;

                    RidePat rp = new RidePat();
                    rp.Coordinator = new Volunteer();
                    rp.Coordinator.DisplayName = dr["Coordinator"].ToString();
                    rp.Remark = dr["Remark"].ToString();
                    rp.Drivers = new List<Volunteer>();
                    // if (numOfDrivers != 0)
                    //  {

                    if (dr["MainDriver"].ToString() != "")
                    {

                        Volunteer primary = new Volunteer();
                        primary.DriverType = "Primary";

                        primary.Id = int.Parse(dr["MainDriver"].ToString());
                        string searchExpression = "Id = " + primary.Id;
                        DataRow[] driverRow = driverTable.Select(searchExpression);
                        primary.DisplayName = driverRow[0]["DisplayName"].ToString();
                        primary.CellPhone = driverRow[0]["CellPhone"].ToString();
                        primary.EnglishFN =
                            String.IsNullOrEmpty(Convert.ToString(driverRow[0]["EnglishFN"])) ?
                            "" :
                            Convert.ToString(driverRow[0]["EnglishFN"]);
                        primary.EnglishLN =
                            String.IsNullOrEmpty(Convert.ToString(driverRow[0]["EnglishLN"])) ?
                            "" :
                            Convert.ToString(driverRow[0]["EnglishLN"]);

                        int numberOfRides = 0;
                        int.TryParse(driverRow[0]["NoOfDocumentedRides"].ToString(), out numberOfRides);
                        primary.NoOfDocumentedRides = numberOfRides;

                        primary.IsNewDriver = driverRow[0]["IsNewDriver"].ToString() == "1";

                        rp.Drivers.Add(primary);
                    }


                    // if (numOfDrivers > 1)
                    // {



                    //Yogev ↓

                    #region comment out secondaryDriver
                    /* 
                    if (dr["secondaryDriver"].ToString() != "")
                    {
                        Volunteer secondary = new Volunteer();
                        secondary.DriverType = "secondary";
                        secondary.Id = int.Parse(dr["secondaryDriver"].ToString());
                        string searchExpression = "Id = " + secondary.Id;
                        DataRow[] driverRow = driverTable.Select(searchExpression);
                        secondary.DisplayName = driverRow[0]["DisplayName"].ToString();
                        secondary.CellPhone = driverRow[0]["CellPhone"].ToString();
                        rp.Drivers.Add(secondary);
                    }
                     */
                    #endregion


                    rp.RidePatNum = int.Parse(dr["RidePatNum"].ToString());


                    rp.RideNum = String.IsNullOrEmpty(dr["RideNum"].ToString()) ? -1 : (int)dr["RideNum"];

                    //try
                    //{
                    //    rp.RideNum = int.Parse(dr["RideNum"].ToString());
                    //}
                    //catch (Exception)
                    //{

                    //}

                    //Yogev ↑
                    rp.pat = new Patient();
                    rp.pat.DisplayName = dr["DisplayName"].ToString();
                    rp.pat.EnglishName = dr["EnglishName"].ToString();
                    rp.pat.CellPhone = dr["CellPhone"].ToString();
                    rp.pat.IsAnonymous = dr["IsAnonymous"].ToString();
                    string gender = dr["Gender"].ToString();
                    rp.Pat.GenderAsEnum = Convertions.ConvertStringToGender(gender);
                    DateTime? dateOfBirth = String.IsNullOrEmpty(dr["BirthDate"].ToString()) ? null : (DateTime?)Convert.ToDateTime(dr["BirthDate"].ToString());
                    rp.Pat.Age = Calculations.CalculateAge(dateOfBirth);
                    rp.Pat.RidePatPatientStatus = new RidePatPatientStatus();
                    string patientStatus = dr["PatientStatus"].ToString();
                    rp.Pat.RidePatPatientStatus.Status = Convertions.ConvertStringToPatientStatus(patientStatus);
                    rp.Pat.RidePatPatientStatus.EditTimeStamp = String.IsNullOrEmpty(dr["EditTimeStamp"].ToString()) ? null : (DateTime?)Convert.ToDateTime(dr["EditTimeStamp"].ToString());

                    rp.pat.Id = int.Parse(dr["Id"].ToString());
                    rp.pat.Equipment = new List<string>();
                    string equipmentSearchExpression = "Id = " + rp.Pat.Id;
                    DataRow[] equipmentRow = equipmentTable.Select(equipmentSearchExpression);
                    foreach (DataRow row in equipmentRow)
                    {
                        rp.pat.Equipment.Add(row.ItemArray[0].ToString());
                    }

                    rp.pat.EscortedList = new List<Escorted>();
                    rp.Escorts = new List<Escorted>();
                    string escortSearchExpression = "RidePatNum = " + rp.ridePatNum;
                    DataRow[] escortRow = escortTable.Select(escortSearchExpression);

                    foreach (DataRow row in escortRow)
                    {
                        Escorted e = new Escorted();
                        e.Id = int.Parse(row["Id"].ToString());
                        e.DisplayName = row["DisplayName"].ToString();
                        e.CellPhone = row["CellPhone"].ToString();
                        // Benny's Fix 22/9/2022
                        //e.IsAnonymous = String.IsNullOrEmpty(row["IsAnonymous"].ToString()) ? false : true;
                        if (String.IsNullOrEmpty(row["IsAnonymous"].ToString()))
                        {
                            e.IsAnonymous = false;
                        }
                        else
                        {
                            if (row["IsAnonymous"].ToString() == "1")
                                e.IsAnonymous = true;
                            else
                                e.IsAnonymous = false;
                        }
                        // End Benny's fix

                        rp.Escorts.Add(e);
                        rp.pat.EscortedList.Add(e);

                    }

                    Location origin = new Location();
                    origin.Name = dr["Origin"].ToString();
                    if (locations[origin.Name] == null)
                    {
                        origin.EnglishName = "";
                    }
                    else origin.EnglishName = locations[origin.Name].ToString();
                    rp.Origin = origin;
                    Location dest = new Location();
                    dest.Name = dr["Destination"].ToString();
                    if (locations[dest.Name] == null)
                    {
                        dest.EnglishName = "";
                    }
                    else dest.EnglishName = locations[dest.Name].ToString();
                    rp.Destination = dest;
                    rp.Area = dr["Area"].ToString();
                    rp.Shift = dr["Shift"].ToString();
                    rp.Date = Convert.ToDateTime(dr["PickupTime"].ToString());
                    rp.Status = dr["Status"].ToString();

                    bool result;
                    Boolean.TryParse(dr["OnlyEscort"].ToString(), out result);
                    rp.OnlyEscort = result;

                    rp.LastModified =
                                       String.IsNullOrEmpty(dr["lastModified"].ToString()) ? null :
                                       (DateTime?)Convert.ToDateTime(dr["lastModified"].ToString());

                    if (rp.RideNum > 0 && rp.Status != "אין נסיעת הלוך ויש נהג משובץ") // if RidePat is assigned to a Ride - Take the Ride's status
                    {
                        string searchExpression = "RideRideNum = " + rp.RideNum;
                        DataRow[] rideRow = rideTable.Select(searchExpression);
                        //rideRow = rideRow.OrderBy(x => x.TimeOfDay).ToList();
                        rp.Statuses = new List<string>();
                        foreach (DataRow status in rideRow)
                        {
                            rp.Statuses.Add(status.ItemArray[0].ToString());
                        }
                        try
                        {
                            rp.Status = rp.Statuses[rp.Statuses.Count - 1];
                        }
                        catch (Exception err)
                        {

                            throw err;
                        }


                    }
                    rpl.Add(rp);
                }
                catch (Exception ex)
                {

                    throw ex;
                }


            }
            return rpl;
        }
        catch (Exception e)
        {
            throw e;
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

    public List<RidePat> GetRidePatViewByTimeFilter(int from, int until)
    {
        //this method fetches ridepat which marked removed as well
        Location tmp = new Location();
        Hashtable locations = tmp.getLocationsEnglishName();
        DataTable driverTable = getDriver();
        DataTable equipmentTable = getEquipment();
        DataTable rideTable = getRides();
        DataTable escortTable = getEscorts();
        List<Escorted> el = new List<Escorted>();
        List<RidePat> rpl = new List<RidePat>();
        string query = "exec spGet_rpview_ByTimeRange @from=" + from + ", @to=" + until;


        try
        {
            dbs = new DbService();
            SqlDataReader sdr = dbs.GetDataReader(query);
            while (sdr.Read())
            {
                RidePat rp = new RidePat();
                rp.Coordinator = new Volunteer();
                rp.Coordinator.DisplayName = sdr["Coordinator"].ToString();
                rp.Drivers = new List<Volunteer>();

                if (sdr["MainDriver"].ToString() != "")
                {
                    Volunteer primary = new Volunteer();
                    primary.DriverType = "Primary";

                    primary.Id = int.Parse(sdr["MainDriver"].ToString());
                    string searchExpression = "Id = " + primary.Id;
                    DataRow[] driverRow = driverTable.Select(searchExpression);
                    primary.DisplayName = driverRow[0]["DisplayName"].ToString();
                    primary.CellPhone = driverRow[0]["CellPhone"].ToString();
                    rp.Drivers.Add(primary);
                }

                rp.RidePatNum = int.Parse(sdr["RidePatNum"].ToString());


                rp.RideNum = String.IsNullOrEmpty(sdr["RideNum"].ToString()) ? -1 : (int)sdr["RideNum"];

                rp.pat = new Patient();
                rp.pat.DisplayName = sdr["DisplayName"].ToString();
                rp.pat.EnglishName = sdr["EnglishName"].ToString();
                rp.pat.CellPhone = sdr["CellPhone"].ToString();
                rp.pat.IsAnonymous = sdr["IsAnonymous"].ToString();

                rp.pat.Id = int.Parse(sdr["Id"].ToString());
                rp.pat.Equipment = new List<string>();
                string equipmentSearchExpression = "Id = " + rp.Pat.Id;
                DataRow[] equipmentRow = equipmentTable.Select(equipmentSearchExpression);
                foreach (DataRow row in equipmentRow)
                {
                    rp.pat.Equipment.Add(row.ItemArray[0].ToString());
                }
                rp.pat.EscortedList = new List<Escorted>();
                string escortSearchExpression = "RidePatNum = " + rp.ridePatNum;
                DataRow[] escortRow = escortTable.Select(escortSearchExpression);
                foreach (DataRow row in escortRow)
                {
                    Escorted e = new Escorted();
                    e.Id = int.Parse(row[0].ToString());
                    e.DisplayName = row[1].ToString();
                    rp.pat.EscortedList.Add(e);
                }

                Location origin = new Location();
                origin.Name = sdr["Origin"].ToString();
                if (locations[origin.Name] == null)
                {
                    origin.EnglishName = "";
                }
                else origin.EnglishName = locations[origin.Name].ToString();
                rp.Origin = origin;
                Location dest = new Location();
                dest.Name = sdr["Destination"].ToString();
                if (locations[dest.Name] == null)
                {
                    dest.EnglishName = "";
                }
                else dest.EnglishName = locations[dest.Name].ToString();
                rp.Destination = dest;
                rp.Area = sdr["Area"].ToString();
                rp.Shift = sdr["Shift"].ToString();
                rp.Date = Convert.ToDateTime(sdr["PickupTime"].ToString());
                rp.Status = sdr["Status"].ToString();
                rp.LastModified =
                                  String.IsNullOrEmpty(sdr["lastModified"].ToString()) ? null :
                                 (DateTime?)Convert.ToDateTime(sdr["lastModified"].ToString());

                if (rp.RideNum > 0 && rp.Status != "אין נסיעת הלוך ויש נהג משובץ") // if RidePat is assigned to a Ride - Take the Ride's status
                {
                    string searchExpression = "RideRideNum = " + rp.RideNum;
                    DataRow[] rideRow = rideTable.Select(searchExpression);
                    //rideRow = rideRow.OrderBy(x => x.TimeOfDay).ToList();
                    rp.Statuses = new List<string>();
                    foreach (DataRow status in rideRow)
                    {
                        rp.Statuses.Add(status.ItemArray[0].ToString());
                    }
                    try
                    {
                        rp.Status = rp.Statuses[rp.Statuses.Count - 1];
                    }
                    catch (Exception err)
                    {

                        throw err;
                    }


                }

                rpl.Add(rp);
            }
            return rpl;
        }
        catch (Exception ex)
        {
            throw new Exception("exception in RidePat.cs →  GetRidePatViewByTimeFilter" + ex);
        }
        finally
        {
            dbs.CloseConnection();
        }

    }

    public List <RidePat> GetRifePatViewByTimeFilter_DR_Gilad(int from,int until, bool isDeletedtoShow)
    {
        DBservice_Gilad dBservice_Gilad = new DBservice_Gilad();
        return dBservice_Gilad.GetRidePatViewByTimeFilter_Gilad_DR(from, until, isDeletedtoShow);
    }


    public List<Volunteer> GetRidePatViewForTomorrow(ref List<int> ridesId)
    {
        #region Database
        DataTable driverTable = getDriver();

        string query = "select RidePatNum,MainDriver from rpview where pickuptime>GETDATE() and datediff(hour,getdate(),pickuptime)<=24";

        List<Volunteer> volunteerListForNotification = new List<Volunteer>();
        List<RidePat> rp = new List<RidePat>();

        DbService db = new DbService();
        DataSet ds = db.GetDataSetByQuery(query);

        try
        {
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                // if voluneer is assigned to this ridePat, don't return it (for mobile app)
                if ((dr["MainDriver"].ToString() != "") /*|| (dr["secondaryDriver"].ToString() != "")*/)
                {
                    if ((dr["MainDriver"].ToString() != ""))
                    {
                        Volunteer primary = new Volunteer();
                        // RidePat ridep = new RidePat();
                        primary.DriverType = "Primary";

                        primary.Id = int.Parse(dr["MainDriver"].ToString());
                        string searchExpression = "Id = " + primary.Id;
                        DataRow[] driverRow = driverTable.Select(searchExpression);
                        primary.DisplayName = driverRow[0]["DisplayName"].ToString();
                        // primary.Gender = driverRow[0]["Gender"].ToString();
                        primary.CellPhone = driverRow[0]["CellPhone"].ToString();
                        primary.RegId = driverRow[0]["pnRegId"].ToString();

                        // ridep.

                        volunteerListForNotification.Add(primary);
                        ridesId.Add(int.Parse(dr["RidePatNum"].ToString()));

                    }
                }

            }

            return volunteerListForNotification;
        }
        catch (Exception e)
        {
            throw e;
        }
        #endregion
    }

    public List<RidePat> getRidepats() //Very strange method. hopefully no body use it. (area<>N'מרכז' hard coded).
    {
        string query = "select * from ridepat where pickuptime>=getdate() and area<>N'מרכז'"; // Get ALL FUTURE RidePats, even if cancelled
        List<RidePat> ridePats = new List<RidePat>();
        DbService db = new DbService();
        DataSet ds = db.GetDataSetByQuery(query);
        foreach (DataRow dr in ds.Tables[0].Rows)
        {
            RidePat rp = new RidePat();
            rp.RidePatNum = (int)dr["RidePatNum"];
            Location origin = new Location();
            origin.Name = dr["Origin"].ToString();
            Location destination = new Location();
            destination.Name = dr["Destination"].ToString();
            rp.Origin = origin;
            rp.Destination = destination;
            ridePats.Add(rp);
        }
        return ridePats;
    }
    public void setRidepatsArea()
    {
        List<RidePat> ridePats = getRidepats();
        DbService db = new DbService();
        foreach (RidePat rp in ridePats)
        {
            try
            {
                string area = NEWCheckLocationForRidepatArea(rp.Origin.Name, rp.Destination.Name);
                string query = "update ridepat set Area=" + "N'" + area + "'" + " WHERE ridepatnum=" + rp.RidePatNum;
                int res = db.ExecuteQuery(query);
            }
            catch (Exception ex)
            {

                throw;
            }

        }
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

    public int AssignRideToRidePat(int ridePatId, int userId, string driverType, int assignedFromAppId)
    {
        int createRideId = -1;
        SqlCommand cmd = new SqlCommand();
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandText = "spRideAndRidePat_AssignDriver";
        cmd.Parameters.AddWithValue("@ridePatId", ridePatId);
        cmd.Parameters.AddWithValue("@driverId", userId);
        cmd.Parameters.AddWithValue("@assignedFromAppId", assignedFromAppId);

        try
        {
            dbs = new DbService();
            sdr = dbs.GetDataReaderSP(cmd);

            if (sdr.Read())
            {

                bool isError = Convert.ToBoolean(Convert.ToInt32(sdr["IsError"]));
                if (isError)
                {//errors are managed in the sp
                    string errorMessage = sdr["Message"].ToString();
                    throw new Exception(errorMessage);
                }

                createRideId = Convert.ToInt32(sdr["RideId"].ToString());
            }

            RidePat rp = GetRidePat(ridePatId);
            BroadCast.BroadCast2Clients_driverHasAssigned2RidePat(rp);

        }
        catch (Exception e)
        {
            throw e;
        }
        finally
        {
            sdr.Close();
        }

        return createRideId;
    }
    public int LeaveRidePat(int ridePatId, int rideId, int driverId)
    {//signalR implemnted in this method
        int res = -1;
        DateTime timeRightNow = DateTime.Now;
        string query = "select * from RPView where RidePatNum=" + ridePatId;
        DbService db4 = new DbService();
        DataSet ds2 = db4.GetDataSetByQuery(query);
        DataRow dr = ds2.Tables[0].Rows[0];

        DateTime pickupTime = new DateTime();
        DateTime timeRightAboutNow = new DateTime();
        timeRightAboutNow = DateTime.Now;
        int hours = 0;

        pickupTime = DateTime.Parse(dr["PickupTime"].ToString());
        TimeSpan difference = pickupTime - DateTime.Now;
        hours = difference.Hours;

        string query2 = "update Ride set MainDriver=null where RideNum=" + rideId;
        DbService db = new DbService();
        res = db.ExecuteQuery(query2);

        if (hours <= 24 && (pickupTime > timeRightNow))
        {

            //it's less than 24 to the ride
            res = 911;
        }
        RidePat rp = GetRidePat(ridePatId);
        BroadCast.BroadCast2Clients_driverHasRemovedFromRidePat(rp);

        return res;
    }
    public int DeleteDriver(int ridePatId, int driverId)
    {
        //NEED TO: update ridepat --> rideid=null
        //if thers is a secondary OR maindriver --> 
        //if primary canceled: create a new ride with SignMe
        //if secondery canceled: create a new ride but take all statuses.

        int res = -1;
        bool primary = false;
        int rideNum = -1;
        int BackupDriver = -1;
        DateTime pickupTime = new DateTime();
        DateTime timeRightAboutNow = new DateTime();
        timeRightAboutNow = DateTime.Now;
        DbService db = new DbService();
        int hours = 0;

        string query = "select * from RPView where RidePatNum=" + ridePatId;
        DataSet ds = db.GetDataSetByQuery(query);
        DataRow row = ds.Tables[0].Rows[0];
        bool is24Hours = false;
        try
        {
            pickupTime = DateTime.Parse(row["PickupTime"].ToString());
            TimeSpan difference = pickupTime - DateTime.Now;
            hours = difference.Hours;
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

            //if there is a secondery driver on this ride also
            if (BackupDriver != -1)
            {

            }
            //query = "update ridepat set RideId=null where ridepatnum =" + ridePatId;
            DbService db2 = new DbService();
            res = db2.ExecuteQuery(query);
        }
        else
        {
            query = "update Ride set secondaryDriver=null where RideNum=" + rideNum;
            //query = "update ridepat set RideId=null where ridepatnum =" + ridePatId;
            DbService db5 = new DbService();
            res = db5.ExecuteQuery(query);
        }
        if (hours <= 24)
        {
            is24Hours = true;
            //call the police!! it's less than 24 to the ride
            res = 911;
        }

        return res;
    }
    public int SignDriver(int ridePatId, int ridePatId2, int driverId, bool primary)
    {
        DbService db = new DbService();
        string query = "select Origin, Destination, PickupTime from RidePat where ridePatNum=" + ridePatId;
        DataSet ds = db.GetDataSetByQuery(query);
        if (ds.Tables[0].Rows.Count == 0)
        {
            throw new Exception("הנסיעה הזו בוטלה, תודה על הרצון לעזור.");
        }
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
            query = "set dateformat dmy; insert into Ride (Origin, Destination, Date, MainDriver) values (N'" + Origin.Name + "',N'" + Destination.Name + "',N'" + Date + "', " + driverId + ") SELECT SCOPE_IDENTITY()";
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
        Message m = new Message();
        Volunteer v = new Volunteer();
        TimeSpan hourDiff = Date - DateTime.Now;

        if (hourDiff.Hours <= 12)
        {
            m.driverSignUpToCloseRide(ridePatId, v.getVolunteerByID(driverId), primary);
            if (ridePatId2 != ridePatId && ridePatId2 != 0)
            {
                m.driverSignUpToCloseRide(ridePatId2, v.getVolunteerByID(driverId), primary);
            }
        }


        return res;

    }

    public void UpdateRidePatTime(int ridePatId, DateTime dateTime)
    {
        RidePat updatedRidePat = null;
        try
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "spRidePatAndRide_UpdateDateAndTime";
            cmd.Parameters.AddWithValue("@ridePatId", ridePatId);
            cmd.Parameters.AddWithValue("@editedTime", dateTime);
            dbs = new DbService();
            sdr = dbs.GetDataReaderSP(cmd);
            updatedRidePat = new RidePat();
            updatedRidePat = GetRidePat(ridePatId);
            BroadCast.BroadCast2Clients_ridePatUpdated(updatedRidePat);
        }
        catch (Exception)
        {
            throw;
        }
        finally
        {
            if (sdr != null)
            {
                sdr.Close();
            }
        }
    }

    public RidePat getReturnRidePat(RidePat ridePat, bool isAnonymous)
    {
        Location tmp = new Location();
        Hashtable locations = tmp.getLocationsEnglishName();
        string[] date = ridePat.Date.ToString().Split(' ');
        DateTime date1 = Convert.ToDateTime(date[0]);
        if (!isAnonymous)
        {
            //Switching origin and destination:
            string originName = ridePat.Destination.Name.Replace("'", "''");
            string destName = ridePat.Origin.Name.Replace("'", "''");
            string patName = ridePat.Pat.DisplayName.Replace("'", "''");

            string query = "select * from RPView where Origin=N'" + originName + "' and Destination=N'" + destName + "' and cast(PickupTime as date)='" + date1.ToString("yyyy-MM-dd") + "' and DisplayName=N'" + patName + "'";
            DbService db = new DbService();
            DataSet ds = db.GetDataSetByQuery(query);
            if (ds.Tables[0].Rows.Count == 0)
            {
                return null;
            }

            RidePat rp = new RidePat();
            rp.pat = new Patient();
            DataRow dr = ds.Tables[0].Rows[0];

            rp.RidePatNum = int.Parse(dr["RidePatNum"].ToString());
            if (dr["RideNum"].ToString() != null)
            {
                if (dr["RideNum"].ToString() != "")
                {
                    rp.RideNum = int.Parse(dr["RideNum"].ToString());
                }
            }
            else rp.RideNum = -1;
            rp.OnlyEscort = Convert.ToBoolean(dr["OnlyEscort"].ToString());
            rp.pat.DisplayName = dr["DisplayName"].ToString();
            rp.pat.IsAnonymous = dr["IsAnonymous"].ToString();
            rp.Drivers = new List<Volunteer>();
            if (dr["MainDriver"].ToString() != "")
            {
                Volunteer v1 = new Volunteer();
                v1.Id = int.Parse(dr["MainDriver"].ToString());
                v1.RegId = v1.GetVolunteerRegById(v1.Id);
                v1.DriverType = "Primary";

                string query3 = "select DisplayName from volunteer where id=" + v1.Id;
                DbService db3 = new DbService();
                DataSet ds3 = db3.GetDataSetByQuery(query3);
                if (ds3.Tables[0].Rows.Count != 0)
                {
                    DataRow dr3 = ds3.Tables[0].Rows[0];
                    v1.DisplayName = dr3["DisplayName"].ToString();
                }

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
            if (locations[origin.Name] == null)
            {
                origin.EnglishName = "";
            }
            else origin.EnglishName = locations[origin.Name].ToString();
            rp.Origin = origin;
            Location dest = new Location();
            dest.Name = dr["Destination"].ToString();
            if (locations[dest.Name] == null)
            {
                dest.EnglishName = "";
            }
            else dest.EnglishName = locations[dest.Name].ToString();
            rp.Destination = dest;
            rp.Area = dr["Area"].ToString();
            rp.Shift = dr["Shift"].ToString();
            rp.Date = Convert.ToDateTime(dr["PickupTime"].ToString());
            rp.Status = dr["Status"].ToString();
            rp.Coordinator = new Volunteer();
            rp.Coordinator.DisplayName = dr["Coordinator"].ToString();
            rp.Remark = dr["Remark"].ToString();

            rp.LastModified =
                            String.IsNullOrEmpty(dr["lastModified"].ToString()) ? null :
                            (DateTime?)Convert.ToDateTime(dr["lastModified"].ToString());

            string query2 = "select DisplayName,Id from RidePatEscortView where RidePatNum=" + rp.ridePatNum;
            DbService db2 = new DbService();
            DataSet ds2 = db2.GetDataSetByQuery(query2);
            foreach (DataRow r in ds2.Tables[0].Rows)
            {
                if (r["DisplayName"].ToString() != "")
                {
                    Escorted e = new Escorted();
                    e.DisplayName = r["DisplayName"].ToString();
                    //rp.pat.EscortedList.Add(e);
                    e.Id = (int)r["Id"];
                    rp.Escorts.Add(e);
                }
            }

            return rp;

        }
        else return null;
    }

    public int changeRidePatStatus(string newStatus, string ridePatNum)
    {
        SqlCommand cmd = new SqlCommand();
        cmd.CommandType = CommandType.Text;
        SqlParameter[] cmdParams = new SqlParameter[2];
        cmdParams[0] = cmd.Parameters.AddWithValue("@ridePatNum", ridePatNum);
        cmdParams[1] = cmd.Parameters.AddWithValue("@newStatus", newStatus);


        string query = "update RidePat set status = @newStatus where RidePatNum = @ridePatNum";

        DbService db = new DbService();

        return db.ExecuteQuery(query, cmd.CommandType, cmdParams);
    }

    public void ChangeArrayOF_RidePatStatuses(string new_status, List<int> ridePatNums, DateTime clientUTCTimeStemp)
    {
        /*
         * 27/04/2021 Yogev ! *
         this design is not best practise at all but due to lack of time it stais this way for now
         we should seek a way to send array of ids from here and create procedure will get all of them
         and apply all the changes to them in it (where id in (...)
        */

        //signalr goes in this method

        DbService dbs;
        SqlParameter[] cmdparams = new SqlParameter[3];
        SqlCommand cmd = new SqlCommand();
        cmdparams[0] = cmd.Parameters.AddWithValue("@new_Status", new_status);
        cmdparams[1] = cmd.Parameters.AddWithValue("@clientUTCTimeStemp", clientUTCTimeStemp);

        string query = "exec SpRidePat_UpdateStatus @newStatus=@new_Status, @ridePatNum=@ridePatNum, @lastmodified=@clientUTCTimeStemp";
        try
        {
            dbs = new DbService();
            for (int i = 0; i < ridePatNums.Count; i++)
            {
                cmdparams[2] = cmd.Parameters.AddWithValue("@ridePatNum", ridePatNums[i]);
                dbs.ExecuteQuery(query, CommandType.Text, cmdparams);
            }
        }
        catch (Exception e)
        {
            throw e;
        }

        #region old way of run the query and inform driver and coordinator
        /*
DbService dbs = new DbService();
Message msg = new Message();
Volunteer driver2inform = new Volunteer();

LogEntry log;
string query = "";

string logMsg = "נסיעה מספר ";
try
{
    for (int i = 0; i < ridePatNums.Count; i++)
    {
        logMsg += ridePatNums[i] + " ";
        query = "exec SpRidePat_UpdateStatus @newStatus=N'" + new_status + "',@ridePatNum=" + ridePatNums[i];

        //at the time twe worked with push notifications we worked with datareader here

        //if the query returns a value → then there is a driver to inform using  push notifications

        //this is no longer the case but SpRidePat_UpdateStatus still returns driver2inform {Id, DisplayName, RegId}


        SqlDataReader sdr = dbs.GetDataReader(query);
        if (sdr.Read())         
        {
            //Update driver:
            driver2inform.Id = Convert.ToInt32(sdr["MainDriver"]);
            driver2inform.DisplayName = Convert.ToString(sdr["DisplayName"]);
            driver2inform.RegId = Convert.ToString(sdr["pnRegId"]);

            msg.cancelRide(ridePatNums[i], driver2inform);               // inform driver
            msg.coordinatorCanceledRide(ridePatNums[i], driver2inform); //inform coordinator  -  this method fetch the coordinator to inform to by the ridePatNums and the driver2inform id

            logMsg += "עם הנהג " + driver2inform.DisplayName + "שינתה סטטוס ל " + new_status;
            log = new LogEntry(DateTime.Now, "שינוי סטטוס", logMsg, 2, ridePatNums[i], false);

        }
        else
        {
            logMsg += "ללא נהג שינתה סטטוס ל" + new_status;
            log = new LogEntry(DateTime.Now, "שינוי סטטוס", logMsg, 2, ridePatNums[i], false);
        }

        dbs.CloseConnection();

    }
}
catch (Exception e)
{
    throw e;
}
finally
{
    dbs.CloseConnection();
}
         */
        #endregion
    }

    public int AssignMultiRideToRidePat(int ridePatId, int userId, string driverType, int numberOfRides, int assignedFromAppId)
    {
        int RideId = -1;
        for (int i = 0; i < numberOfRides; i++)
        {
            RideId = -1;

            DateTime timeRightNow = DateTime.Now;
            string query = "select RideNum,Origin,Destination,PickupTime,Status,MainDriver,secondaryDriver from RPView where RidePatNum=" + ridePatId;
            DbService db = new DbService();
            DataSet ds = db.GetDataSetByQuery(query);
            if (ds.Tables[0].Rows.Count == 0)
            {
                //There is no RidePat with that ID
                throw new Exception("נסיעה זו בוטלה, תודה על הרצון לעזור");
            }
            DataRow dr = ds.Tables[0].Rows[0];

            Origin = new Location();
            Origin.Name = dr["Origin"].ToString();
            Destination = new Location();
            Destination.Name = dr["Destination"].ToString();
            Date = Convert.ToDateTime(dr["PickupTime"].ToString());
            if (dr["Status"].ToString() == "שובץ נהג וגיבוי") throw new Exception("הנסיעה אליה נרשמתם כבר מלאה");
            //XXX DOESNT GO HERE WITH MY LOGIC (always ELSE):
            if (dr["RideNum"].ToString() != "") //Ride aleady exists
            {
                RideId = int.Parse(dr["RideNum"].ToString());


                if (dr["MainDriver"].ToString() == "") //No main driver is assigned to this ride
                {
                    if (driverType == "primary")
                        query = "update Ride set MainDriver=" + userId + ", AssignedFromAppId=" + assignedFromAppId + " where RideNum=" + RideId; //assign a main driver to this ride
                }

                DbService db4 = new DbService();
                int res = db4.ExecuteQuery(query);
                if (res <= 0) return -1;
            }
            else // New ride
            {
                SqlCommand cmd = new SqlCommand();
                SqlParameter[] cmdparams = new SqlParameter[5];
                cmdparams[0] = cmd.Parameters.AddWithValue("@origin", Origin.Name);
                cmdparams[1] = cmd.Parameters.AddWithValue("@dest", Destination.Name);
                cmdparams[2] = cmd.Parameters.AddWithValue("@date", Date);
                cmdparams[3] = cmd.Parameters.AddWithValue("@mainDriver", userId);
                cmdparams[4] = cmd.Parameters.AddWithValue("@assignedFromAppId", assignedFromAppId);

                string query2 = "set dateformat dmy; insert into Ride (Origin,Destination,Date,MainDriver,AssignedFromAppId) values (@origin,@dest,@date,@mainDriver,@assignedFromAppId) SELECT SCOPE_IDENTITY()";
                DbService db2 = new DbService();
                RideId = int.Parse(db2.GetObjectScalarByQuery(query2, cmd.CommandType, cmdparams).ToString());
                if (RideId <= 0) return -1;
                string query3 = "update RidePat set RideId=" + RideId + " where RidePatNum=" + ridePatId;
                DbService db3 = new DbService();
                int res = db3.ExecuteQuery(query3);
                if (res <= 0) return -1;
            }

            Message m = new Message();
            Volunteer v = new Volunteer();
            TimeSpan hourDiff = Date - DateTime.Now;

            if (hourDiff.Hours <= 12 && (Date > timeRightNow))
            {
                bool primary = false;
                if (driverType == "primary")
                {
                    primary = true;
                }
                try
                {
                    m.driverSignUpToCloseRide(ridePatId, v.getVolunteerByID(userId), primary);
                }
                catch (Exception ex)
                {
                    throw new Exception("A1 " + ex.Message);
                }
            }

            //HERE!

            RidePat rp = GetRidePat(ridePatId);
            RidePatNum = rp.RidePatNum;
            if (Date > timeRightNow)
            {
                try
                {
                    m.driverAddedToRide(RidePatNum, rp.Drivers[0]);
                }
                catch (Exception ex)
                {
                    throw new Exception("A2 " + ex.Message);
                }

            }


            ridePatId++;
        }

        return RideId;

    }

    public void UpdatePatientStatus(int patientId, int ridePatId, string patientStatus, DateTime? editTimeStamp)
    {
        try
        {
            string sqlDate = "NULL";

            if (editTimeStamp.HasValue)
            {
                sqlDate = editTimeStamp.Value.Year + "-" + editTimeStamp.Value.Month + "-" + editTimeStamp.Value.Day;
                sqlDate += " " + editTimeStamp.Value.Hour + ":" + editTimeStamp.Value.Minute;
            }

            string query = "EXEC spRidePatPatientStatus_TogglePatientStatus @PatientId=" + patientId + ", @RidePatNum=" + ridePatId;
            query += ", @PatientStatus='" + patientStatus + "', @EditTimeStamp='" + sqlDate + "'";

            try
            {
                dbs = new DbService();
                dbs.ExecuteQuery(query);
                RidePat updatedRidePat = GetRidePat(ridePatId);
                BroadCast.BroadCast2Clients_ridePatUpdated(updatedRidePat);
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
        catch (Exception ex)
        {
            throw;
        }
    }

    public void UpdateRidePatRemark(int ridePatId, string newRemark)
    {
        try
        {
            newRemark = FixApostrophe(newRemark);
            string query = "exec spRidePat_UpdateRemark @ridePatNum=" + ridePatId + ", @newRemark = N'" + newRemark + "'";
            dbs = new DbService();
            dbs.ExecuteQuery(query);
            RidePat rp = GetRidePat(ridePatId);
            BroadCast.BroadCast2Clients_ridePatUpdated(rp);
        }
        catch (Exception)
        {
            throw;
        }
    }

    /// <summary>
    /// ChangeCoordinatoor is a private method for changing the coordinator NAME! in the ridepat table
    /// this functionality is for track who was the last coordinator who change any field in a record of 
    /// ridepat table.
    /// ------------------------------------------------------
    /// the name of the coordinator is taken from the session
    /// </summary>
    private void ChangeCoordinatoor(int ridePatNum)
    {
        string loggedInName = String.IsNullOrEmpty((string)HttpContext.Current.Session["loggedInName"]) ? "sessionWasEmpty" :
            FixApostrophe((string)HttpContext.Current.Session["loggedInName"]);

        string query = "exec spRidePat_ChangeCoordinatorName @coordinatorName=N'" + loggedInName + "', @RidePatNum=" + ridePatNum;
        SqlCommand cmd = new SqlCommand();

        try
        {
            dbs = new DbService();
            dbs.ExecuteQuery(query);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }

    }

    private string FixApostrophe(string strWithChopchick)
    {
        return strWithChopchick.Replace("'", "''");
    }

}