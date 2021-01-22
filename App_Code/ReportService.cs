using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web;

/* Notes:
 * 
 
  
 */
public class ReportService
{

    public class RidesForVolunteer
    {
        public string Date { get; set; }
        public string OriginName { get; set; }
        public string DestinationName { get; set; }
        public string Time { get; set; }
        public string PatDisplayName { get; set; }
        public string Drivers { get; set; }
        public string Day { get; set; }
        public string Status { get; internal set; }
    }


    public class NameIDPair
    {
        public string Name { get; set; }
        public string ID { get; set; }
    }

    public class VolunteerPerRegion
    {

        public string Volunteer { get; set; }
        public string Region { get; set; }
    }

    public class VolunteerPerPatient
    {
        public string Volunteer { get; set; }
        public string Date { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }

    }

    public class VolunteerKM
    {
        public string Volunteer { get; set; }
        public string Date { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public string Patient { get; set; }

    }

    public class VolunteerInfo
    {
        public string FirstNameH { get; set; }
        public string LastNameH { get; set; }
        public string VolunteerIdentity { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string CityCityName { get; set; }
        public string JoinDate { get; set; }
        public string CellPhone { get; set; }

    }

    public class VolunteersPerMonthInfo
    {
        public string Year { get; set; }
        public string Month { get; set; }
        public string Count { get; set; }

    }

    public class SliceVolunteersPerMonthInfo
    {
        public string DisplayName { get; set; }
        public string City { get; set; }
        public string CellPhone { get; set; }
        public string JoinDate { get; set; }
        public string Jan { get; set; }
        public string Feb { get; set; }
        public string Mar { get; set; }
        public string Apr { get; set; }
        public string May { get; set; }
        public string Jun { get; set; }
        public string Jul { get; set; }
        public string Aug { get; set; }
        public string Sep { get; set; }
        public string Oct { get; set; }
        public string Nov { get; set; }
        public string Dec { get; set; }

    }

    public class SliceVolunteersCountInMonthInfo
    {
        public string Volunteer { get; set; }
        public string Count { get; set; }
    }


    public class INSERT_TO_NI_SQL_Objects
    {
        public string query { get; set; }

        public SqlParameter[] sqlParameters { get; set;  }
    }


    private DataTable getDriverByID(int driverID, DbService db)
    {
        string query = "select Id,DisplayName,CellPhone from Volunteer where Id = @ID";
        SqlCommand cmd = new SqlCommand(query);
        cmd.CommandType = CommandType.Text;
        cmd.Parameters.Add("@ID", SqlDbType.Int).Value = driverID;

        DataSet ds = db.GetDataSetBySqlCommand(cmd);
        DataTable dt = ds.Tables[0];
        return dt;
    }


    private DataTable getPickupForDriver(int driverID, string start_date, string end_date, DbService db)
    {
        string query = "SELECT* FROM RPView WHERE pickuptime < @end_date AND pickuptime >= @start_date AND MainDriver =  @ID";
        SqlCommand cmd = new SqlCommand(query);
        cmd.CommandType = CommandType.Text;
        cmd.Parameters.Add("@ID", SqlDbType.Int).Value = driverID;
        cmd.Parameters.Add("@start_date", SqlDbType.Date).Value = start_date;
        cmd.Parameters.Add("@end_date", SqlDbType.Date).Value = end_date;

        DataSet ds = db.GetDataSetBySqlCommand(cmd);
        DataTable dt = ds.Tables[0];
        return dt;
    }


    //@@ TODO:  Maybe ths is not needed?
    private DataTable getVolunteerRidesPerWeek(string start_date, string end_date, DbService db)
    {
        /*
         * SELECT RidePat.Patient , Volunteer.DisplayName, RidePat.Area ,RidePat.PickupTime, RidePat.MainDriver
FROM RidePat
INNER JOIN Volunteer ON RidePat.MainDriver=Volunteer.Id
AND RidePat.pickuptime < '2020-3-01'
AND RidePat.pickuptime >= '2020-1-01'

    */

        string query = @" SELECT RidePat.Patient , Volunteer.DisplayName, RidePat.Area ,RidePat.PickupTime, RidePat.MainDriver
                        FROM RidePat
                        INNER JOIN Volunteer ON RidePat.MainDriver=Volunteer.Id
                        AND RidePat.pickuptime < '2020-3-01'
                        AND RidePat.pickuptime >= '2020-1-01'";

        SqlCommand cmd = new SqlCommand(query);
        cmd.CommandType = CommandType.Text;
        //cmd.Parameters.Add("@ID", SqlDbType.Int).Value = driverID;
        //cmd.Parameters.Add("@start_date", SqlDbType.Date).Value = start_date;
        //cmd.Parameters.Add("@end_date", SqlDbType.Date).Value = end_date;

        DataSet ds = db.GetDataSetBySqlCommand(cmd);
        DataTable dt = ds.Tables[0];
        return dt;
    }

    internal List<SliceVolunteersPerMonthInfo> GetReportSliceVolunteerPerMonth(string start_date, string end_date)
    {
        DbService db = new DbService();

        string query =
             @"select MainDriver, Volunteer.DisplayName as DisplayName, 
                Volunteer.CityCityName as CityName, Volunteer.CellPhone as CellPhone, 
                 convert(varchar, Volunteer.JoinDate, 103)  as JoinDate,
              sum(case when MONTH([pickuptime]) = '1' then 1 else 0 end) Jan,
              sum(case when MONTH([pickuptime]) = '2' then 1 else 0 end) Feb,
              sum(case when MONTH([pickuptime]) = '3' then 1 else 0 end) Mar,
              sum(case when MONTH([pickuptime]) = '4' then 1 else 0 end) Apr,
              sum(case when MONTH([pickuptime]) = '5' then 1 else 0 end) May,
              sum(case when MONTH([pickuptime]) = '6' then 1 else 0 end) Jun,
              sum(case when MONTH([pickuptime]) = '7' then 1 else 0 end) Jul,
              sum(case when MONTH([pickuptime]) = '8' then 1 else 0 end) Aug,
              sum(case when MONTH([pickuptime]) = '9' then 1 else 0 end) Sep,
              sum(case when MONTH([pickuptime]) = '10' then 1 else 0 end) Oct,
              sum(case when MONTH([pickuptime]) = '11' then 1 else 0 end) Nov,
              sum(case when MONTH([pickuptime]) = '12' then 1 else 0 end) Dec
            FROM RPView  rp
            INNER JOIN Volunteer on Volunteer.Id = rp.MainDriver 
            WHERE pickuptime >= @start_date 
            AND pickuptime <= @end_date
            Group BY MainDriver, Volunteer.DisplayName, Volunteer.CityCityName, Volunteer.CellPhone, Volunteer.JoinDate
            ";

        SqlCommand cmd = new SqlCommand(query);
        cmd.CommandType = CommandType.Text;
        cmd.Parameters.Add("@start_date", SqlDbType.Date).Value = start_date;
        cmd.Parameters.Add("@end_date", SqlDbType.Date).Value = end_date;

        DataSet ds = db.GetDataSetBySqlCommand(cmd);

        List<SliceVolunteersPerMonthInfo> result = new List<SliceVolunteersPerMonthInfo>();

        if (ds != null && ds.Tables.Count > 0 )
        {
            DataTable dt = ds.Tables[0];
            foreach (DataRow dr in dt.Rows)
            {
                SliceVolunteersPerMonthInfo obj = new SliceVolunteersPerMonthInfo();
                obj.DisplayName = dr["DisplayName"].ToString();
                obj.City = dr["CityName"].ToString();
                obj.CellPhone = dr["CellPhone"].ToString();
                obj.JoinDate = dr["JoinDate"].ToString();
                obj.Jan = dr["Jan"].ToString();
                obj.Feb = dr["Feb"].ToString();
                obj.Mar = dr["Mar"].ToString();
                obj.Apr = dr["Apr"].ToString();
                obj.May = dr["May"].ToString();
                obj.Jun = dr["Jun"].ToString();
                obj.Jul = dr["Jul"].ToString();
                obj.Aug = dr["Aug"].ToString();
                obj.Sep = dr["Sep"].ToString();
                obj.Oct = dr["Oct"].ToString();
                obj.Nov = dr["Nov"].ToString();
                obj.Dec = dr["Dec"].ToString();

                result.Add(obj);
            }
        }

        return result;
    }

    internal List<ReportService.SliceVolunteersCountInMonthInfo> GetReportSliceVolunteersCountInMonth(string start_date, string end_date)
    {
        DbService db = new DbService();

        string query =
@"SELECT Volunteer.DisplayName , count(*) as COUNT_C  
FROM RPView 
INNER JOIN Volunteer ON RPView.MainDriver=Volunteer.Id
WHERE pickuptime < @end_date
AND pickuptime >= @start_date
and MainDriver is not null
GROUP BY Volunteer.DisplayName 
";
        SqlCommand cmd = new SqlCommand(query);
        cmd.CommandType = CommandType.Text;
        cmd.Parameters.Add("@start_date", SqlDbType.Date).Value = start_date;
        cmd.Parameters.Add("@end_date", SqlDbType.Date).Value = end_date;

        DataSet ds = db.GetDataSetBySqlCommand(cmd);
        DataTable dt = ds.Tables[0];

        List<SliceVolunteersCountInMonthInfo> result = new List<SliceVolunteersCountInMonthInfo>();

        foreach (DataRow dr in dt.Rows)
        {
            SliceVolunteersCountInMonthInfo obj = new SliceVolunteersCountInMonthInfo();
            obj.Volunteer = dr["DisplayName"].ToString();
            obj.Count = dr["COUNT_C"].ToString();
            result.Add(obj);
        }

        return result;
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

    internal string CommitReportedVolunteerListToNI_DB(string cell_phone, string start_date, string only_with_rides)
    {
        // This service is not to be used by everybody, check if user is entitled for it
        List<string> permissions = this.GetCurrentUserEntitlements(cell_phone);
        if (!permissions.Contains("Record_NI_report"))
        {
            return "UnAuthorized";   // Empty results - TODO: 404
        }


        DbService db = new DbService();

        INSERT_TO_NI_SQL_Objects objs = GetQueryTextForINSERT_NIVolunteerList(start_date, only_with_rides);

        db.ExecuteQuery(objs.query, CommandType.Text, objs.sqlParameters);

        return "OK";

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

    internal List<VolunteerPerRegion> GetReportVolunteerWeekly(string start_date, string end_date)
    {
        DbService db = new DbService();

        string query =
             @"select DISTINCT DisplayName,  Area
            From(SELECT Volunteer.DisplayName, RPView.Area
            FROM RPView
            INNER JOIN Volunteer ON RPView.MainDriver = Volunteer.Id
            AND RPView.pickuptime <  @end_date
            AND RPView.pickuptime >=  @start_date) AS BUFF
            ORDER BY Area ASC";

        SqlCommand cmd = new SqlCommand(query);
        cmd.CommandType = CommandType.Text;
        cmd.Parameters.Add("@start_date", SqlDbType.Date).Value = start_date;
        cmd.Parameters.Add("@end_date", SqlDbType.Date).Value = end_date;

        DataSet ds = db.GetDataSetBySqlCommand(cmd);
        DataTable dt = ds.Tables[0];

        List<VolunteerPerRegion> result = new List<VolunteerPerRegion>();

        foreach (DataRow dr in dt.Rows)
        {
            VolunteerPerRegion obj = new VolunteerPerRegion();
            obj.Volunteer = dr["DisplayName"].ToString();
            obj.Region = dr["Area"].ToString();
            result.Add(obj);
        }

        return result;
    }

    internal List<VolunteersPerMonthInfo> GetReportVolunteerPerMonth(string start_date)
    {
        DbService db = new DbService();

        string query =
             @"SELECT count(DISTINCT MainDriver )as COUNT_G, YEAR(date) as YEAR_G, MONTH(date) as MONTH_G
                FROM Ride 
                where date >= @start_date
                and date <= CURRENT_TIMESTAMP
                GROUP BY YEAR(Date), MONTH(Date) 
                ORDER  BY YEAR_G, MONTH_G ASC";

        SqlCommand cmd = new SqlCommand(query);
        cmd.CommandType = CommandType.Text;
        cmd.Parameters.Add("@start_date", SqlDbType.Date).Value = start_date;

        DataSet ds = db.GetDataSetBySqlCommand(cmd);
        DataTable dt = ds.Tables[0];

        List<VolunteersPerMonthInfo> result = new List<VolunteersPerMonthInfo>();

        foreach (DataRow dr in dt.Rows)
        {
            VolunteersPerMonthInfo obj = new VolunteersPerMonthInfo();
            obj.Year = dr["YEAR_G"].ToString();
            obj.Month = dr["MONTH_G"].ToString();
            obj.Count = dr["COUNT_G"].ToString();
            result.Add(obj);
        }

        return result;


    }

    internal string pad_with_zeros(string id)
    {
        int pad = 9 - id.Length;
        if (pad > 0 && pad != 9)
        {
            return new string('0', pad) + id;
        }
        return id;
    }


    // Reusable code, for the logic to display (orupdate) the NI report volunteers
    // Used both form the report Table, and also in the INSERT to the DeliveredNIReport
    internal string getQueryBodyFor_NIVolunteerList(string start_date, string only_with_rides)
    {
        string result;
        if (start_date.Equals("NONE"))
        {
            result = @"from Volunteer v
                INNER JOIN RPView rp ON rp.MainDriver=v.Id
                where IsActive='true' 
                and not v.Id in (select distinct DriverId from DeliveredNIReport )";

            if (only_with_rides.Equals("True"))
            {
                result += " and rp.pickuptime >= '2000-01-01'";  // From begining of time
            }

        }
        else
        {
            result = @"from Volunteer v
                INNER JOIN RPView rp ON rp.MainDriver=v.Id
                where IsActive='true' 
                and (JoinDate >= @start_date )
                and not v.Id in (select distinct DriverId from DeliveredNIReport )";

            if (only_with_rides.Equals("True"))
            {
               result += " and rp.pickuptime >= @start_date";
            }
        }

        return result;

    }

        // The interface to execute statements on DbService differes than the one to Query
        // It should sometimes return also an array of parameters
        internal INSERT_TO_NI_SQL_Objects GetQueryTextForINSERT_NIVolunteerList(string start_date, string only_with_rides)
    {
        string insert_header = @"INSERT into DeliveredNIReport   (DriverId, ReportDate) 
                                SELECT  DISTINCT v.Id, CAST(GETDATE() AS Date) ";

        string query_body = getQueryBodyFor_NIVolunteerList(start_date, only_with_rides);

        INSERT_TO_NI_SQL_Objects objs = new INSERT_TO_NI_SQL_Objects();
        objs.query = insert_header + query_body;

        if (start_date.Equals("NONE"))
        {
            objs.sqlParameters = new SqlParameter[0];   // Make downstream DbService Execute happy
        }
        else
        {
            objs.sqlParameters = new SqlParameter[1];
            objs.sqlParameters[0] = new SqlParameter("@start_date", SqlDbType.Date);
            objs.sqlParameters[0].Value = start_date;
        }

        return objs;
    }


        internal SqlCommand BuildSqlCommandForNIVolunteerList(string start_date, string only_with_rides)
    {
        SqlCommand cmd;

        /* ATTENTION - If you change here, review  also GetQueryTextForINSERT_NIVolunteerList */

        string header = "select DISTINCT v.ID, FirstNameH, LastNameH , VolunteerIdentity , Email, Address, CityCityName, JoinDate, v.CellPhone ";

        string query_body = getQueryBodyFor_NIVolunteerList(start_date, only_with_rides);

        cmd = new SqlCommand(header + query_body);
        cmd.CommandType = CommandType.Text;

        if (!start_date.Equals("NONE"))
        {
            cmd.Parameters.Add("@start_date", SqlDbType.Date).Value = start_date;
        }

        return cmd;
    }

    internal List<VolunteerInfo> GetReportVolunteerList(string cell_phone, string start_date, string only_with_rides)
    {
        List<VolunteerInfo> result = new List<VolunteerInfo>();

        // This service is not to be used by everybody, check if user is entitled for it
        List<string> permissions = this.GetCurrentUserEntitlements(cell_phone);
        if ( !permissions.Contains("Record_NI_report"))
        {
            return result;   // Empty results - TODO: 404
        }


        DbService db = new DbService();
        SqlCommand cmd = BuildSqlCommandForNIVolunteerList(start_date, only_with_rides);
        DataSet ds = db.GetDataSetBySqlCommand(cmd);
        DataTable dt = ds.Tables[0];

        foreach (DataRow dr in dt.Rows)
        {
            VolunteerInfo obj = new VolunteerInfo();
            obj.FirstNameH = dr["FirstNameH"].ToString();
            obj.LastNameH = dr["LastNameH"].ToString();
            string id = dr["VolunteerIdentity"].ToString();
            obj.VolunteerIdentity = pad_with_zeros(id);
            obj.Email = dr["Email"].ToString();
            obj.CellPhone = dr["CellPhone"].ToString();
            obj.Address = dr["Address"].ToString();
            obj.CityCityName = dr["CityCityName"].ToString();
            if ( dr.IsNull("JoinDate") )
            {
                obj.JoinDate = DateTime.Now.ToString("dd/MM/yyyy");
            } 
            else
            {
                obj.JoinDate = ((DateTime)dr["JoinDate"]).ToString("dd/MM/yyyy");
            }
            result.Add(obj);
        }

        return result;
    }



    internal List<VolunteerKM> GetReportVolunteersKM(string start_date, string end_date)
    {
        DbService db = new DbService();

        string query =
 @"select convert(varchar, PickupTime, 103) AS PickupTime, Origin, Destination, Volunteer.DisplayName, PatName
from 
( SELECT pickuptime, Origin, Destination, MainDriver, DisplayName AS PatName FROM RPView 
WHERE pickuptime >= '2019-1-01'
and MainDriver is not null
AND RPView.pickuptime < @end_date
AND RPView.pickuptime >= @start_date
) AS BUFF
INNER JOIN Volunteer ON MainDriver=Volunteer.Id
ORDER BY Volunteer.DisplayName ASC
";

        SqlCommand cmd = new SqlCommand(query);
        cmd.CommandType = CommandType.Text;
        cmd.Parameters.Add("@start_date", SqlDbType.Date).Value = start_date;
        cmd.Parameters.Add("@end_date", SqlDbType.Date).Value = end_date;

        DataSet ds = db.GetDataSetBySqlCommand(cmd);
        DataTable dt = ds.Tables[0];

        List<VolunteerKM> result = new List<ReportService.VolunteerKM>();

        foreach (DataRow dr in dt.Rows)
        {
            ReportService.VolunteerKM obj = new ReportService.VolunteerKM();
            obj.Volunteer = dr["DisplayName"].ToString();
            obj.Patient = dr["PatName"].ToString();
            obj.Origin = dr["Origin"].ToString();
            obj.Destination = dr["Destination"].ToString();
            obj.Date = dr["PickupTime"].ToString();
            result.Add(obj);
        }

        return result;
    }

    internal List<NameIDPair> GetPatientsDisplayNames()
    {
        DbService db = new DbService();

        string query =
 @"select Id, DisplayName 
FROM Patient
order BY DisplayName ASC
";

        SqlCommand cmd = new SqlCommand(query);
        cmd.CommandType = CommandType.Text;

        DataSet ds = db.GetDataSetBySqlCommand(cmd);
        DataTable dt = ds.Tables[0];

        List<NameIDPair> result = new List<NameIDPair>();

        foreach (DataRow dr in dt.Rows)
        {
            NameIDPair obj = new NameIDPair();
            obj.Name = dr["DisplayName"].ToString();
            obj.ID = dr["Id"].ToString();
            result.Add(obj);
        }

        return result;
    }

    // Purpose: Let the web app know what  is applicable for this user
    // Returns: List of special priviliages. Can be empty if user has nothing special         
    // Usage:
    //       1. Who can "click the button" that records in DB the National Insurance report
    internal List<string> GetCurrentUserEntitlements(string cell_phone)
    {
        List<string> result = new List<string>();
        if (!String.IsNullOrEmpty(cell_phone))
        {
            DbService db = new DbService();

            string query = @"select Permission FROM ReportPermissions WHERE Cellphone = @cell_phone";

            SqlCommand cmd = new SqlCommand(query);
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.Add("@cell_phone", SqlDbType.NVarChar).Value = cell_phone;

            DataSet ds = db.GetDataSetBySqlCommand(cmd);
            DataTable dt = ds.Tables[0];

            foreach (DataRow dr in dt.Rows)
            {
                result.Add(dr["Permission"].ToString());
            }
        }
        return result;
    }


    internal List<VolunteerPerPatient> GetReportVolunteersPerPatient(int patient)
    {
        DbService db = new DbService();

        string query =
@"SELECT convert(varchar, PickupTime, 103) AS PickupTime, Origin, Destination, Volunteer.DisplayName
From (
SELECT pickuptime, Origin, Destination, MainDriver FROM RPView  
WHERE pickuptime >= '2019-1-01'
and MainDriver is not null
and Id = @patient
) as BUFF
INNER JOIN Volunteer ON BUFF.MainDriver=Volunteer.Id";

        SqlCommand cmd = new SqlCommand(query);
        cmd.CommandType = CommandType.Text;
        cmd.Parameters.Add("@patient", SqlDbType.Int).Value = patient;

        DataSet ds = db.GetDataSetBySqlCommand(cmd);
        DataTable dt = ds.Tables[0];

        List<VolunteerPerPatient> result = new List<VolunteerPerPatient>();

        foreach (DataRow dr in dt.Rows)
        {
            VolunteerPerPatient obj = new VolunteerPerPatient();
            obj.Volunteer = dr["DisplayName"].ToString();
            obj.Origin = dr["Origin"].ToString();
            obj.Destination = dr["Destination"].ToString();
            obj.Date = dr["PickupTime"].ToString();
            result.Add(obj);
        }

        return result;
    }

    public List<RidesForVolunteer> GetReportVolunteerRides(int volunteerId, string start_date, string end_date)
    {
        if (volunteerId <= 0)
        {
            throw new ArgumentException("Negative volunteerId is not supported");
        }

        RidePat helper_RidePat = new RidePat();

        DbService db = new DbService();
        Location tmp = new Location();
        Hashtable locations = tmp.getLocationsEnglishName();
        DataTable driverTable = getDriverByID(volunteerId, db);

        //TODO: re-implement getRides using sql-command
        DataTable rideTable = getRides();

        List<Escorted> el = new List<Escorted>();
        DataTable escortTable = getEscorts();


        DataTable pickupsTable = getPickupForDriver(volunteerId, start_date, end_date, db);

        List<RidesForVolunteer> result = new List<RidesForVolunteer>();

        try
        {
            foreach (DataRow dr in pickupsTable.Rows)
            {
                try
                {
                    RidesForVolunteer obj = new RidesForVolunteer();
                    obj.PatDisplayName = dr["DisplayName"].ToString();
                    obj.OriginName = dr["Origin"].ToString();
                    obj.DestinationName = dr["Destination"].ToString();
                    obj.Date = dr["PickupTime"].ToString();
                    obj.Status = dr["Status"].ToString();

                    result.Add(obj);
                }
                catch (Exception ex)
                {

                    throw ex;
                }


            }

            return result;
        }
        catch (Exception e)
        {
            throw e;
        }

    }


    //@@ TODO:  See notes on this method name in reports.js
    public List<RidePat> GetReportRidesWeeklyPerRegion(string start_date, string end_date)
    {
        DbService db = new DbService();
        Location tmp = new Location();
        Hashtable locations = tmp.getLocationsEnglishName();


        DataTable pickupsTable = getVolunteerRidesPerWeek(start_date, end_date, db);
        List<RidePat> rpl = new List<RidePat>();

        int counter = 0;

        try
        {
            foreach (DataRow dr in pickupsTable.Rows)
            {
                try
                {
                    counter++;

                    RidePat rp = new RidePat();
                    rp.Coordinator = new Volunteer();
                    rp.Coordinator.DisplayName = dr["Coordinator"].ToString();
                    rp.Drivers = new List<Volunteer>();

                    if (dr["MainDriver"].ToString() != "")
                    {

                        Volunteer primary = new Volunteer();
                        primary.DriverType = "Primary";
/* @@
                        primary.Id = int.Parse(dr["MainDriver"].ToString());
                        DataRow driverRow = driverTable.Rows[0];
                        primary.DisplayName = driverRow["DisplayName"].ToString();
                        primary.CellPhone = driverRow["CellPhone"].ToString();
                        rp.Drivers.Add(primary);
@@ */
    }


                    // if (numOfDrivers > 1)
                    // {
                    if (dr["secondaryDriver"].ToString() != "")
                    {
                        // Do we care about these for a report? 
                        if (false)
                        {
                            throw new Exception("Secondary Driver lookup not implemented yet");
                            Volunteer secondary = new Volunteer();
                            secondary.DriverType = "secondary";
                            secondary.Id = int.Parse(dr["secondaryDriver"].ToString());
                            string searchExpression = "Id = " + secondary.Id;
/* @@ 
                            DataRow[] driverRow = driverTable.Select(searchExpression);
                            secondary.DisplayName = driverRow[0]["DisplayName"].ToString();
                            secondary.CellPhone = driverRow[0]["CellPhone"].ToString();
                            rp.Drivers.Add(secondary);

    @@ */
                        }
                    }

                    rp.RidePatNum = int.Parse(dr["RidePatNum"].ToString());
                    try
                    {
                        rp.RideNum = int.Parse(dr["RideNum"].ToString());
                    }
                    catch (Exception)
                    {

                    }

                    Patient thePatient = new Patient();
                    thePatient.DisplayName = dr["DisplayName"].ToString();
                    thePatient.EnglishName = dr["EnglishName"].ToString();
                    thePatient.CellPhone = dr["CellPhone"].ToString();
                    thePatient.IsAnonymous = dr["IsAnonymous"].ToString();
                    thePatient.Id = int.Parse(dr["Id"].ToString());

                    rp.Pat = thePatient;

                    rp.Pat.EscortedList = new List<Escorted>();
/* @@
                    string escortSearchExpression = "RidePatNum = " + rp.RidePatNum;
                    DataRow[] escortRow = escortTable.Select(escortSearchExpression);
                    foreach (DataRow row in escortRow)
                    {
                        Escorted e = new Escorted();
                        e.Id = int.Parse(row[0].ToString());
                        e.DisplayName = row[1].ToString();
                        rp.Pat.EscortedList.Add(e);
                    }
                    @@ */



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
                    if (rp.RideNum > 0) // if RidePat is assigned to a Ride - Take the Ride's status
                    {
/* @@ 
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

@@ */
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

    }

}