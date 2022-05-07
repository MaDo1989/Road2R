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

    public class VolunteersInPeriod
    {
        public String Id { get; set; }
        public string Volunteer { get; set; }
        public string CityCityName { get; set; }
        public string CellPhone { get; set; }
    }

    public class CenterDailybyMonthInfo
    {
        public string Date { get; set; }
        public string PatientCount { get; set; }
        public string VolunteerCount { get; set; }
    }

    public class CenterMonthlyByYearInfo
    {
        public string PatientCount { get; set; }
        public string VolunteerCount { get; set; }
        public string Month { get; set; }
    }


    public class MetricInfo
    {
        public string MetricName { get; set; }
        public int Value1 { get; set; }
        public int Value2 { get; set; }
    }


    public class MetricMonthlyInfo
    {
        public string Day { get; set; }
        public string Rides { get; set; }
        public string Patients { get; set; }
        public string Volunteers { get; set; }
    }

    public class CenterPatientsRidesInfo
    {
        public string Volunteer { get; set; }
        public string Month { get; set; }
        public string Count { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
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

        // Inner select groups by time, origin&dest, to avoid counting the same ride multiple times 

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
             FROM (select MainDriver, PickupTime  from RPView r 
					where pickuptime >= @start_date 
					AND pickuptime <= @end_date
					AND MainDriver is not null
					GROUP BY MainDriver, PickupTime, Origin, Destination 
			   ) inner_select
            INNER JOIN Volunteer on Volunteer.Id = inner_select.MainDriver 
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

        // Inner-Select - Grouping by pickup time, dest & Orig is part of better accuracy
        // it avoids counting the same ride with multiple patients as 2 rides 
        string query =
@"select DisplayName, count(*) as COUNT_C   from 
(  SELECT Volunteer.DisplayName, RPView.PickupTime, RPView.Origin, RPView.Destination, count(*) as INNER_C
  FROM RPView 
  INNER JOIN Volunteer ON RPView.MainDriver=Volunteer.Id
  WHERE pickuptime < @end_date
  AND pickuptime >= @start_date
  and MainDriver is not null
  GROUP BY Volunteer.DisplayName, RPView.PickupTime, RPView.Origin, RPView.Destination 
  ) inner_select
GROUP BY inner_select.DisplayName
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

    private List<string> GetDistinctListOfField(string field, string table)
    {
        DbService db = new DbService();
        string query = string.Format("select DISTINCT {0} AS FIELD from {1}", field, table);

        SqlCommand cmd = new SqlCommand(query);
        cmd.CommandType = CommandType.Text;

        DataSet ds = db.GetDataSetBySqlCommand(cmd);
        DataTable dt = ds.Tables[0];

        List<string> result = new List<string>();

        foreach (DataRow dr in dt.Rows)
        {
            result.Add(dr["FIELD"].ToString());
        }

        return result;
    }


    internal List<string> GetReportHospitals()
    {
        return GetDistinctListOfField("Hospital", "Patient");
    }

    internal List<string> GetReportBarriers()
    {
        return GetDistinctListOfField("Barrier", "Patient");
    }

    internal List<string> GetReportLocations()
    {
        return GetDistinctListOfField("name", "Location");
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

    internal MetricMonthlyInfo GetReportNewDriversInRange(string start_date, string end_date)
    {
        DbService db = new DbService();

        string query =
             @"select  count (DISTINCT MainDriver)  AS COUNT_VOL
            from RPView
            where MainDriver is not null
            and pickuptime >= @start_date
            and pickuptime <= @end_date
            AND not MainDriver in (
	            select distinct MainDriver
	            from RPView
	            where MainDriver is not null
	            and pickuptime < @start_date)";

        SqlCommand cmd = new SqlCommand(query);
        cmd.CommandType = CommandType.Text;
        cmd.Parameters.Add("@start_date", SqlDbType.Date).Value = start_date;
        cmd.Parameters.Add("@end_date", SqlDbType.Date).Value = end_date;

        DataSet ds = db.GetDataSetBySqlCommand(cmd);
        DataTable dt = ds.Tables[0];

        MetricMonthlyInfo result = new MetricMonthlyInfo();

        DataRow dr = dt.Rows[0];
        result.Volunteers = dr["COUNT_VOL"].ToString();

        return result;
    }

    internal List<MetricMonthlyInfo> GetReportYearlyGraphMetrics(string start_date, string end_date)
    {
        DbService db = new DbService();

        /* The inner query allows identifying unique rides.
         * If we have same 'MainDriver, PickupTime, Origin, Destination', the ride is shared between patients
         * So we partition over that field combination and generate a runnign index (row_number) for each ride
         * Now to count just uniuqe ride, we put a CASE that gets '1' for each unique ride
         * The unique rides count is the SUM of the new case field
         */

        string query =
             @"SELECT  MONTH(pickuptime) as MONTH_C ,  count(DISTINCT DisplayName) as COUNT_PAT,  
                        count(*) as COUNT_DUPLICATE_RIDES, SUM(Unique_Drive_C) as COUNT_UNIQUE_RIDES, count(DISTINCT MainDriver) as COUNT_VOL
                FROM  (
	                select MainDriver  , PickupTime, Origin, Destination, DisplayName, 
	                ROW_NUMBER() OVER (PARTITION by MainDriver, PickupTime, Origin, Destination  ORDER BY PickupTime Asc) as row_num_C,
	                CASE WHEN ROW_NUMBER() OVER (PARTITION by MainDriver, PickupTime, Origin, Destination  ORDER BY PickupTime Asc) = '1' THEN 1
		                ELSE 0
	                END AS Unique_Drive_C
	                from RPView r 
	                where pickuptime >= @start_date 
	                AND pickuptime <= @end_date
	                AND MainDriver is not null
                ) s 
                GROUP BY MONTH(pickuptime)
                ORDER BY MONTH_C ASC";

        SqlCommand cmd = new SqlCommand(query);
        cmd.CommandType = CommandType.Text;
        cmd.Parameters.Add("@start_date", SqlDbType.Date).Value = start_date;
        cmd.Parameters.Add("@end_date", SqlDbType.Date).Value = end_date;

        DataSet ds = db.GetDataSetBySqlCommand(cmd);
        DataTable dt = ds.Tables[0];

        List<MetricMonthlyInfo> result = new List<MetricMonthlyInfo>();

        foreach (DataRow dr in dt.Rows)
        {
            MetricMonthlyInfo obj = new MetricMonthlyInfo();
            obj.Day = dr["MONTH_C"].ToString();
            obj.Rides = dr["COUNT_UNIQUE_RIDES"].ToString();
            obj.Patients = dr["COUNT_PAT"].ToString();
            obj.Volunteers = dr["COUNT_VOL"].ToString();
            result.Add(obj);
        }

        return result;
    }

    internal MetricInfo GetReportRangeNeedDriversMetrics(string start_date, string end_date)
    {
        DbService db = new DbService();

        string query =
             @"select ID from RPView
                where MainDriver is NULL
                and pickuptime > @start_date
                and pickuptime < @end_date
                and Status = N'ממתינה לשיבוץ' ";

        SqlCommand cmd = new SqlCommand(query);
        cmd.CommandType = CommandType.Text;
        cmd.Parameters.Add("@start_date", SqlDbType.Date).Value = start_date;
        cmd.Parameters.Add("@end_date", SqlDbType.Date).Value = end_date;

        DataSet ds = db.GetDataSetBySqlCommand(cmd);
        DataTable dt = ds.Tables[0];


        int count = 0;
        HashSet<string> uniqueIDs = new HashSet<string>();

        foreach (DataRow dr in dt.Rows)
        {
            uniqueIDs.Add(dr["ID"].ToString());
            count++;
        }

        MetricInfo result = new MetricInfo
        {
            MetricName = "NeedDrivers",
            Value1 = count,
            Value2 = uniqueIDs.Count
        };
        return result;
    }

    internal List<MetricMonthlyInfo> GetReportRangeDigestMetrics(string start_date, string end_date, string prev_start, string prev_end, string query)
    {
        DbService db = new DbService();

        SqlCommand cmd = new SqlCommand(query);
        cmd.CommandType = CommandType.Text;
        cmd.Parameters.Add("@start_date", SqlDbType.Date).Value = start_date;
        cmd.Parameters.Add("@end_date", SqlDbType.Date).Value = end_date;
        if (!prev_start.Equals("NA"))
        {
            cmd.Parameters.Add("@prev_start", SqlDbType.Date).Value = prev_start;
            cmd.Parameters.Add("@prev_end", SqlDbType.Date).Value = prev_end;
        }

        DataSet ds = db.GetDataSetBySqlCommand(cmd);
        DataTable dt = ds.Tables[0];

        List<MetricMonthlyInfo> result = new List<MetricMonthlyInfo>();

        foreach (DataRow dr in dt.Rows)
        {
            MetricMonthlyInfo obj = new MetricMonthlyInfo();
            obj.Day = dr["SPAN_C"].ToString();
            obj.Rides = dr["COUNT_UNIQUE_RIDES"].ToString();
            obj.Patients = dr["COUNT_PAT"].ToString();
            obj.Volunteers = dr["COUNT_VOL"].ToString();
            result.Add(obj);
        }

        return result;
    }
    
    internal List<MetricMonthlyInfo> GetReportMonthlyDigestMetrics(string start_date, string end_date)
    {
        string query =
             @"SELECT 1 AS SPAN_C, count(DISTINCT DisplayName) as COUNT_PAT, SUM(Unique_Drive_C) as COUNT_UNIQUE_RIDES, count(DISTINCT MainDriver) as COUNT_VOL     
                FROM  (
	                select MainDriver  , PickupTime, Origin, Destination, DisplayName, 
	                CASE WHEN ROW_NUMBER() OVER (PARTITION by MainDriver, PickupTime, Origin, Destination  ORDER BY PickupTime Asc) = '1' THEN 1
		                ELSE 0
	                END AS Unique_Drive_C
	                from RPView r 
	                where pickuptime >= @start_date 
	                AND pickuptime <= @end_date
	                AND MainDriver is not null
                )  s";

        return GetReportRangeDigestMetrics(start_date, end_date, "NA", "NA", query);
    }


    // pickuptime >= @start_date      AND pickuptime <= @end_date

    internal string buildTimeCondition(string field_name, string prev_start)
    {
        if (prev_start.Equals("NA"))
        {
            return string.Format("{0} >= @start_date  AND {0} <= @end_date", field_name);
        }
        else
        {
            return string.Format("( ({0} >= @start_date  AND {0} <= @end_date) OR ( {0} >= @prev_start  AND {0} <= @prev_end) )", field_name);
        }
    }

    internal List<MetricMonthlyInfo> GetReportWithPeriodDigestMetrics(string start_date, string end_date, string prev_start, string prev_end, string span)
    {
        string span_column;
        if (span.Equals("WEEK"))
        {
            span_column = "CONCAT (YEAR(PickupTime), '-', DATEPART(week, PickupTime) )";  // YYYY-WKNUM
        }
        else if (span.Equals("MONTH"))
        {
            span_column = "CONVERT(nvarchar(7), PickupTime, 23)";  // YYYY-MM
        }
        else if (span.Equals("YEAR"))
        {
            span_column = "YEAR(PickupTime)";
        }
        else
        {
            return null;
        }

        string pickup_time_condition = buildTimeCondition("PickupTime", prev_start);


        string query = string.Format(@"SELECT {0} AS SPAN_C, count(DISTINCT DisplayName) as COUNT_PAT, SUM(Unique_Drive_C) as COUNT_UNIQUE_RIDES, count(DISTINCT MainDriver) as COUNT_VOL     
                FROM  (
	                select MainDriver  , PickupTime, Origin, Destination, DisplayName, 
	                CASE WHEN ROW_NUMBER() OVER (PARTITION by MainDriver, PickupTime, Origin, Destination  ORDER BY PickupTime Asc) = '1' THEN 1
		                ELSE 0
	                END AS Unique_Drive_C
	                from RPView r 
	                where {1} AND MainDriver is not null
                )  s
                GROUP BY {0} ORDER BY {0} ASC",
                span_column, pickup_time_condition);

        return GetReportRangeDigestMetrics(start_date, end_date, prev_start, prev_end, query);
    }

    internal List<MetricMonthlyInfo> GetReportDailyDigestMetrics(string start_date, string end_date)
    {
        // Gets info also on rides without an allocted driver
        string query =
             @"SELECT  1 AS SPAN_C, count(DISTINCT DisplayName) as COUNT_PAT, SUM(Unique_Drive_C) as COUNT_UNIQUE_RIDES, count(DISTINCT MainDriver) as COUNT_VOL
                FROM (
	                Select  MainDriver  , PickupTime, Origin, Destination, DisplayName, 
                    CASE WHEN ROW_NUMBER() OVER (PARTITION by MainDriver, PickupTime, Origin, Destination  ORDER BY PickupTime Asc) = '1' THEN 1
	                    ELSE 0
                    END AS Unique_Drive_C
                    FROM RPView 
                    WHERE  Status != N'נמחקה'
                    AND RPView.pickuptime > @start_date
                    AND RPView.pickuptime < @end_date
                    ) s";

        return GetReportRangeDigestMetrics(start_date, end_date, "NA", "NA", query);
    }


    // Returns the day info as MM-dd string
    // Function Name is incorrect, is used by Month Graph as well.
    // This allows optimizations - all 3 weeks spans in one call, 2 months in one call etc..
    internal List<ReportService.MetricMonthlyInfo> GetReportWeeklyGraphMetrics(string start_date, string end_date)
    {
        DbService db = new DbService();

        string query =
             @"SELECT FORMAT (PickupTime, 'yyyy-MM-dd') as DAY_C ,  count(DISTINCT DisplayName) as COUNT_PAT, SUM(Unique_Drive_C) as COUNT_UNIQUE_RIDES, count(DISTINCT MainDriver) as COUNT_VOL
                FROM (
	                Select  MainDriver  , PickupTime, Origin, Destination, DisplayName, 
                    CASE 
                        WHEN ROW_NUMBER() OVER (PARTITION by MainDriver, PickupTime, Origin, Destination  ORDER BY PickupTime Asc) = '1' THEN 1
	                    ELSE 0
                    END AS Unique_Drive_C
                    FROM RPView 
                    WHERE MainDriver is not null
                    AND RPView.pickuptime > @start_date
                    AND RPView.pickuptime < @end_date
                ) s 
            GROUP BY FORMAT (PickupTime, 'yyyy-MM-dd')
            ORDER BY DAY_C ASC					
            ";

        SqlCommand cmd = new SqlCommand(query);
        cmd.CommandType = CommandType.Text;
        cmd.Parameters.Add("@start_date", SqlDbType.Date).Value = start_date;
        cmd.Parameters.Add("@end_date", SqlDbType.Date).Value = end_date;

        DataSet ds = db.GetDataSetBySqlCommand(cmd);
        DataTable dt = ds.Tables[0];

        List<MetricMonthlyInfo> result = new List<MetricMonthlyInfo>();

        foreach (DataRow dr in dt.Rows)
        {
            MetricMonthlyInfo obj = new MetricMonthlyInfo();
            obj.Day = dr["DAY_C"].ToString();
            obj.Rides = dr["COUNT_UNIQUE_RIDES"].ToString();
            obj.Patients = dr["COUNT_PAT"].ToString();
            obj.Volunteers = dr["COUNT_VOL"].ToString();
            result.Add(obj);
        }

        return result;

    }


    internal List<VolunteersPerMonthInfo> GetReportVolunteerPerMonth(string start_date)
    {
        DbService db = new DbService();

        string query =
             @"SELECT count(DISTINCT MainDriver )as COUNT_G, YEAR(PickupTime) as YEAR_G, MONTH(PickupTime) as MONTH_G
                FROM RPView r  
                where PickupTime >= @start_date
                and PickupTime <= CURRENT_TIMESTAMP
                GROUP BY YEAR(PickupTime), MONTH(PickupTime) 
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



    internal List<VolunteersInPeriod> GetReportSliceVolunteersInPeriod(int delta_start, int delta_end)
    {
        DbService db = new DbService();
        List<VolunteersInPeriod> result = new List<VolunteersInPeriod>();

        string query =
@"select DISTINCT rp.MainDriver, Volunteer.DisplayName, Volunteer.CityCityName, Volunteer.CellPhone
FROM RPView  rp
INNER JOIN Volunteer on Volunteer.Id = rp.MainDriver 
where CONVERT(date, pickuptime) > CONVERT(date, dateadd(day, @delta_start, getdate()))
and pickuptime <= getdate() 
and not  MainDriver in ( select DISTINCT MainDriver from RPView 
where CONVERT(date, pickuptime) > CONVERT(date, dateadd(day, @delta_end, getdate()))
and pickuptime <= getdate() 
and MainDriver is not NULL)
";

        SqlCommand cmd = new SqlCommand(query);
        cmd.CommandType = CommandType.Text;
        cmd.Parameters.Add("@delta_start", SqlDbType.Int).Value = delta_start;
        cmd.Parameters.Add("@delta_end", SqlDbType.Int).Value = delta_end;

        DataSet ds = db.GetDataSetBySqlCommand(cmd);

        if (ds != null && ds.Tables.Count > 0)
        {
            DataTable dt = ds.Tables[0];

            foreach (DataRow dr in dt.Rows)
            {
                VolunteersInPeriod obj = new VolunteersInPeriod();
                obj.Id = dr["MainDriver"].ToString();
                obj.Volunteer = dr["DisplayName"].ToString();
                obj.CellPhone = dr["CellPhone"].ToString();
                obj.CityCityName = dr["CityCityName"].ToString();
                result.Add(obj);
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

        //TODO: re-implement using sql-command
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

    internal List<CenterDailybyMonthInfo> GetReportCenterDailybyMonth(string start_date, string end_date)
    {
        DbService db = new DbService();

        string query =
@"select  count(DISTINCT rp.MainDriver) AS Drivers, count(DISTINCT rp.Id) As Patients, CONVERT(date, pickuptime) as DayInMonth
FROM RPView  rp
where pickuptime >= @start_date 
and pickuptime <= @end_date
and rp.MainDriver is not null
group by CONVERT(date, pickuptime) ";

        SqlCommand cmd = new SqlCommand(query);
        cmd.CommandType = CommandType.Text;
        cmd.Parameters.Add("@start_date", SqlDbType.Date).Value = start_date;
        cmd.Parameters.Add("@end_date", SqlDbType.Date).Value = end_date;

        DataSet ds = db.GetDataSetBySqlCommand(cmd);
        DataTable dt = ds.Tables[0];

        List<CenterDailybyMonthInfo> result = new List<CenterDailybyMonthInfo>();

        foreach (DataRow dr in dt.Rows)
        {
            CenterDailybyMonthInfo obj = new CenterDailybyMonthInfo();
            obj.Date = dr["DayInMonth"].ToString();
            obj.VolunteerCount = dr["Drivers"].ToString();
            obj.PatientCount = dr["Patients"].ToString();
            result.Add(obj);
        }

        return result;
    }


    internal List<CenterMonthlyByYearInfo> GetReportCenterMonthlyByYear(string start_date, string end_date)
    {
        DbService db = new DbService();

        string query =
@"select count(DISTINCT Id )as COUNT_PATS, count(DISTINCT MainDriver )as COUNT_DRIVERS,  MONTH(PickupTime) as MONTH_G
FROM RPView r 
where PickupTime >= @start_date
and PickupTime <= @end_date
and RideNum  is not null
and MainDriver is not null
GROUP BY  MONTH(PickupTime)                 
ORDER  BY MONTH_G";

        SqlCommand cmd = new SqlCommand(query);
        cmd.CommandType = CommandType.Text;
        cmd.Parameters.Add("@start_date", SqlDbType.Date).Value = start_date;
        cmd.Parameters.Add("@end_date", SqlDbType.Date).Value = end_date;

        DataSet ds = db.GetDataSetBySqlCommand(cmd);
        DataTable dt = ds.Tables[0];

        List<CenterMonthlyByYearInfo> result = new List<CenterMonthlyByYearInfo>();

        foreach (DataRow dr in dt.Rows)
        {
            CenterMonthlyByYearInfo obj = new CenterMonthlyByYearInfo();
            obj.PatientCount = dr["COUNT_PATS"].ToString();
            obj.VolunteerCount = dr["COUNT_DRIVERS"].ToString();
            obj.Month = dr["MONTH_G"].ToString();
            result.Add(obj);
        }

        return result;
    }


    internal string build_condition_ReportCenterPatientsRides(string volunteer, string origin, string destination)
    {
        string condition = "";
        if (!origin.Equals("*"))
        {
            condition = "AND Origin = @Origin";
        }
        if (!destination.Equals("*"))
        {
            condition += " AND Destination = @Destination";
        }

        if (volunteer.Equals("*"))
        {
            condition +=  " AND MainDriver is not null";
        }
        else
        {
            condition += " AND maindriver=@volunteerID";
        }
        return condition;
    }


    internal SqlCommand build_command_ReportCenterPatientsRides(string volunteer, string start_date, string end_date,
        string origin, string destination)
    {
        SqlCommand cmd = new SqlCommand();
        cmd.CommandType = CommandType.Text;
        if (!volunteer.Equals("*"))
        {
            cmd.Parameters.Add("@volunteerID", SqlDbType.Int).Value = volunteer;
        }
        cmd.Parameters.Add("@start_date", SqlDbType.Date).Value = start_date;
        cmd.Parameters.Add("@end_date", SqlDbType.Date).Value = end_date;
        cmd.Parameters.Add("@origin", SqlDbType.NVarChar).Value = origin;
        cmd.Parameters.Add("@destination", SqlDbType.NVarChar).Value = destination;

        return cmd;
    }

        internal List<CenterPatientsRidesInfo> GetReportCenterPatientsRides(string volunteer, string start_date, string end_date,
        string origin, string destination)
    {
        DbService db = new DbService();
        SqlCommand cmd = build_command_ReportCenterPatientsRides(volunteer, start_date, end_date, origin, destination);

        string condition = build_condition_ReportCenterPatientsRides(volunteer, origin, destination);

        string query =
        @"select
            FORMAT (PICKUP_TIME_C, 'MM-yy') AS MONTH_C, Origin , Destination, DISPLAY_NAME_C, count(*) AS COUNT_C
            from 
            (  select rp.PickupTime AS PICKUP_TIME_C, Origin , Destination,  Volunteer.DisplayName AS DISPLAY_NAME_C
	            FROM RPView rp 
	            INNER JOIN Patient p on rp.Id = p.Id
	            INNER JOIN Volunteer  ON rp.MainDriver=Volunteer.Id
	            where pickuptime > @start_date
                AND pickuptime < @end_date " +
                condition +
                @" GROUP BY rp.PickupTime, Origin , Destination, Volunteer.DisplayName
            ) s
            GROUP BY FORMAT (PICKUP_TIME_C, 'MM-yy'),  Origin , Destination, DISPLAY_NAME_C
            order by MONTH_C ASC";

        cmd.CommandText = query;

        DataSet ds = db.GetDataSetBySqlCommand(cmd);
        DataTable dt = ds.Tables[0];

        List<CenterPatientsRidesInfo> result = new List<CenterPatientsRidesInfo>();

        foreach (DataRow dr in dt.Rows)
        {
            CenterPatientsRidesInfo obj = new CenterPatientsRidesInfo();
            obj.Volunteer = dr["DISPLAY_NAME_C"].ToString();
            obj.Month = dr["MONTH_C"].ToString();
            obj.Origin = dr["Origin"].ToString();
            obj.Destination = dr["Destination"].ToString();
            obj.Count = dr["COUNT_C"].ToString();
            result.Add(obj);
        }

        return result;
    }


    internal string GetReportCenterPatientsRidesCount(string volunteer, string start_date, string end_date,
string origin, string destination)
    {
        DbService db = new DbService();
        SqlCommand cmd = build_command_ReportCenterPatientsRides(volunteer, start_date, end_date, origin, destination);

        string condition = build_condition_ReportCenterPatientsRides(volunteer, origin, destination);

        string query =
        @"select count(rp.Id) , count(DISTINCT rp.Id) AS PAT_COUNT
          from RPView rp
	      INNER JOIN Patient p on rp.Id = p.Id
	      where pickuptime > @start_date
	      AND pickuptime < @end_date  
          " + condition;

        cmd.CommandText = query;

        DataSet ds = db.GetDataSetBySqlCommand(cmd);
        DataTable dt = ds.Tables[0];

        string result = dt.Rows[0]["PAT_COUNT"].ToString();
        return result;
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