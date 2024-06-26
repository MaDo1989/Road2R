﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web;

/* Notes:
 * 
Jan-2024:    migrating to "United" dtabase scheme
Given excistsing Report:   GetXXX
We copy it code so we will have two copies of the method
we rename one to be S_GetXXX - this is teh original implementation
We rename the other to be U_GetXXX - this will be the variant that use United scheme 
We implement original GetXXX as following:

    internal List<YYYY> GetXXXX(string start_date, string end_date)
    {
        List<YYYY> s = S_GetReportVolunteerWeekly(start_date, end_date);  // << original implementation
        List<YYYY> u = U_GetReportVolunteerWeekly(start_date, end_date);  // << New implementation using United
        if ( !compare_S_vs_U_results_ordered(s,u))                        // << Compare both lists (requires Equitable interfaces)
        {
            throw new Exception("GetXXXX mismatch");
        }
        return u;                                                         // return results
    }

    for Object type YYY to support comparing inlist, need to implement IEquatable:
    See implementation example in  class VolunteerPerRegion


  
 */
public class ReportService
{

    public class RidesForVolunteer : IEquatable<RidesForVolunteer>
    {
        public string Date { get; set; }
        public string OriginName { get; set; }
        public string DestinationName { get; set; }
        public string Time { get; set; }
        public string PatDisplayName { get; set; }
        public string Drivers { get; set; }
        public string Day { get; set; }
        public string Status { get; internal set; }

        public bool Equals(RidesForVolunteer other)
        {
            if (other == null)
                return false;

            return this.Date == other.Date && this.OriginName == other.OriginName
                && this.DestinationName == other.DestinationName
                && this.Time == other.Time
                && this.PatDisplayName == other.PatDisplayName
                && this.Drivers == other.Drivers
                && this.Day == other.Day
                && this.Status == other.Status;
        }
        public override bool Equals(object obj)
        {
            return Equals(obj as RidesForVolunteer);
        }
        public override int GetHashCode()
        {
            return Date.GetHashCode() ^ OriginName.GetHashCode() ^ Time.GetHashCode()
                    ^ PatDisplayName.GetHashCode() ^ Drivers.GetHashCode() 
                    ^ Day.GetHashCode() ^ Status.GetHashCode();
        }
    }


    public class NameIDPair
    {
        public string Name { get; set; }
        public string ID { get; set; }
    }

    public class VolunteerPerRegion : IEquatable<VolunteerPerRegion>
    {

        public string Volunteer { get; set; }
        public string Region { get; set; }

        public bool Equals(VolunteerPerRegion other)
        {
            if (other == null)
                return false;

            return this.Volunteer == other.Volunteer && this.Region == other.Region;
        }
        public override bool Equals(object obj)
        {
            return Equals(obj as VolunteerPerRegion);
        }
        public override int GetHashCode()
        {
            return Volunteer.GetHashCode() ^ Region.GetHashCode();
        }
    }

    public class VolunteerPerPatient : IEquatable<VolunteerPerPatient>
    {
        public string Volunteer { get; set; }
        public string Date { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }

        public bool Equals(VolunteerPerPatient other)
        {
            if (other == null)
                return false;

            return this.Volunteer == other.Volunteer && this.Date == other.Date
                && this.Origin == other.Origin && this.Destination == other.Destination;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as VolunteerPerPatient);
        }
        public override int GetHashCode()
        {
            return Volunteer.GetHashCode() ^ Date.GetHashCode() ^ Origin.GetHashCode() ^ Destination.GetHashCode();
        }

    }

    public class VolunteerKM : IEquatable<VolunteerKM>
    {
        public string Volunteer { get; set; }
        public string Date { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public string Patient { get; set; }

        public bool Equals(VolunteerKM other)
        {
            if (other == null)
                return false;

            return this.Volunteer == other.Volunteer && this.Date == other.Date
                && this.Origin == other.Origin && this.Destination == other.Destination
                && this.Patient == other.Patient;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as VolunteerKM);
        }
        public override int GetHashCode()
        {
            return Volunteer.GetHashCode() ^ Date.GetHashCode() ^ Origin.GetHashCode()
                ^ Destination.GetHashCode() ^ Patient.GetHashCode();
        }
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

    public class VolunteersPerMonthInfo : IEquatable<VolunteersPerMonthInfo>
    {
        public string Year { get; set; }
        public string Month { get; set; }
        public string Count { get; set; }

        public bool Equals(VolunteersPerMonthInfo other)
        {
            if (other == null)
                return false;

            return this.Year == other.Year && this.Month == other.Month
                && this.Count == other.Count;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as VolunteersPerMonthInfo);
        }
        public override int GetHashCode()
        {
            return Year.GetHashCode() ^ Month.GetHashCode() ^ Count.GetHashCode();
        }
    }

    public class SliceVolunteersPerMonthInfo : IEquatable<SliceVolunteersPerMonthInfo>
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

        public bool Equals(SliceVolunteersPerMonthInfo other)
        {
            if (other == null)
                return false;

            return this.DisplayName == other.DisplayName && this.City == other.City
                && this.CellPhone == other.CellPhone && this.JoinDate == other.JoinDate
                && this.Jan == other.Jan && this.Feb == other.Feb && this.Mar == other.Mar
                && this.Apr == other.Apr && this.May == other.May && this.Jun == other.Jun
                && this.Jul == other.Jul && this.Aug == other.Aug && this.Sep == other.Sep
                && this.Oct == other.Oct && this.Nov == other.Nov && this.Dec == other.Dec;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as SliceVolunteersPerMonthInfo);
        }
        public override int GetHashCode()
        {
            return this.DisplayName.GetHashCode() ^ this.City.GetHashCode() 
                ^ this.CellPhone.GetHashCode() ^ this.JoinDate.GetHashCode()
                ^ this.Jan.GetHashCode() ^ this.Feb.GetHashCode() ^ this.Mar.GetHashCode()
                ^ this.Apr.GetHashCode() ^ this.May.GetHashCode() ^ this.Jun.GetHashCode()
                ^ this.Jul.GetHashCode() ^ this.Aug.GetHashCode() ^ this.Sep.GetHashCode()
                ^ this.Oct.GetHashCode() ^ this.Nov.GetHashCode() ^ this.Dec.GetHashCode();
        }
    }

    public class SliceVolunteersCountInMonthInfo  : IEquatable<SliceVolunteersCountInMonthInfo>
    {
        public string Volunteer { get; set; }
        public string Count { get; set; }
        public bool Equals(SliceVolunteersCountInMonthInfo other)
        {
            if (other == null)
                return false;

            return this.Volunteer == other.Volunteer && this.Count == other.Count;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as SliceVolunteersCountInMonthInfo);
        }
        public override int GetHashCode()
        {
            return Volunteer.GetHashCode() ^ Count.GetHashCode();
        }

    }


    public class INSERT_TO_NI_SQL_Objects
    {
        public string query { get; set; }

        public SqlParameter[] sqlParameters { get; set; }
    }

    public class VolunteersInPeriod : IEquatable<VolunteersInPeriod>
    {
        public String Id { get; set; }
        public string Volunteer { get; set; }
        public string CityCityName { get; set; }
        public string CellPhone { get; set; }

        public bool Equals(VolunteersInPeriod other)
        {
            if (other == null)
                return false;

            return this.Id == other.Id && this.Volunteer == other.Volunteer
                && this.CityCityName == other.CityCityName && this.CellPhone == other.CellPhone;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as VolunteersInPeriod);
        }
        public override int GetHashCode()
        {
            return Id.GetHashCode() ^ Volunteer.GetHashCode() 
                ^ CityCityName.GetHashCode() ^ CellPhone.GetHashCode();
        }
    }

    public class CenterDailybyMonthInfo : IEquatable<CenterDailybyMonthInfo>
    {
        public string Date { get; set; }
        public string PatientCount { get; set; }
        public string VolunteerCount { get; set; }
        public bool Equals(CenterDailybyMonthInfo other)
        {
            if (other == null)
                return false;

            return this.Date == other.Date && this.PatientCount == other.PatientCount
                && this.VolunteerCount == other.VolunteerCount;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as CenterDailybyMonthInfo);
        }
        public override int GetHashCode()
        {
            return Date.GetHashCode() ^ PatientCount.GetHashCode() ^ VolunteerCount.GetHashCode();
        }
    }

    public class CenterMonthlyByYearInfo : IEquatable<CenterMonthlyByYearInfo>
    {
        public string PatientCount { get; set; }
        public string VolunteerCount { get; set; }
        public string RidesCount { get; set; }
        public string DemandsCount { get; set; }
        public string Month { get; set; }

        public bool Equals(CenterMonthlyByYearInfo other)
        {
            if (other == null)
                return false;

            return this.RidesCount == other.RidesCount && this.PatientCount == other.PatientCount
                && this.VolunteerCount == other.VolunteerCount && this.DemandsCount == other.DemandsCount
                && this.Month == other.Month;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as CenterMonthlyByYearInfo);
        }
        public override int GetHashCode()
        {
            return RidesCount.GetHashCode() ^ PatientCount.GetHashCode() ^ VolunteerCount.GetHashCode()
                ^ DemandsCount.GetHashCode() ^ Month.GetHashCode();
        }

    }


    public class MetricInfo
    {
        public string MetricName { get; set; }
        public int Value1 { get; set; }
        public int Value2 { get; set; }
    }


    public class MetricMonthlyInfo : IEquatable<MetricMonthlyInfo>
    {
        public string Day { get; set; }
        public string Rides { get; set; }
        public string Patients { get; set; }
        public string Volunteers { get; set; }
        public string Demands { get; set; }

        public bool Equals(MetricMonthlyInfo other)
        {
            if (other == null)
                return false;

            return this.Day == other.Day && this.Rides == other.Rides
                && this.Patients == other.Patients && this.Volunteers == other.Volunteers
                && this.Demands == other.Demands;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as MetricMonthlyInfo);
        }
        public override int GetHashCode()
        {
            return Day.GetHashCode() ^ Rides.GetHashCode() ^ Patients.GetHashCode()
                ^ Volunteers.GetHashCode() ^ Demands.GetHashCode();
        }
    }

    public class CenterPatientsRidesInfo : IEquatable<CenterPatientsRidesInfo>
    {
        public string Volunteer { get; set; }
        public string Month { get; set; }
        public string Count { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public bool Equals(CenterPatientsRidesInfo other)
        {
            if (other == null)
                return false;

            return this.Volunteer == other.Volunteer && this.Month == other.Month
                && this.Count == other.Count && this.Origin == other.Origin
                && this.Destination == other.Destination;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as CenterPatientsRidesInfo);
        }
        public override int GetHashCode()
        {
            return Volunteer.GetHashCode() ^ Month.GetHashCode() ^ Count.GetHashCode()
                ^ Origin.GetHashCode() ^ Destination.GetHashCode();
        }
    }

    public class CenterTomorrowsRides : IEquatable<CenterTomorrowsRides>
    {
        public string Patient { get; set; }
        public string DriverID { get; set; }
        public string Pickuptime { get; set; }
        public string EscortCount { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }

        public bool Equals(CenterTomorrowsRides other)
        {
            if (other == null)
                return false;

            return this.Patient == other.Patient && this.DriverID == other.DriverID
                && this.Origin == other.Origin && this.Destination == other.Destination
                && this.Pickuptime == other.Pickuptime && this.EscortCount == other.EscortCount;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as CenterTomorrowsRides);
        }
        public override int GetHashCode()
        {
            return DriverID.GetHashCode() ^ Origin.GetHashCode()
                ^ Destination.GetHashCode() ^ Patient.GetHashCode()
                ^ Pickuptime.GetHashCode() ^ EscortCount.GetHashCode();
        }

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

    internal List<SliceVolunteersPerMonthInfo> S_GetReportSliceVolunteerPerMonth(string start_date, string end_date)
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

        if (ds != null && ds.Tables.Count > 0)
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

    internal List<SliceVolunteersPerMonthInfo> U_GetReportSliceVolunteerPerMonth(string start_date, string end_date)
    {
        DBservice_Gilad db = new DBservice_Gilad();

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
             FROM (select MainDriver, PickupTime  from UnityRide ur 
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

        SqlDataReader reader = db.GetDataReaderBySqlCommand(cmd);

        List<SliceVolunteersPerMonthInfo> result = new List<SliceVolunteersPerMonthInfo>();

        while (reader.Read())
        {
            SliceVolunteersPerMonthInfo obj = new SliceVolunteersPerMonthInfo();
                obj.DisplayName = reader["DisplayName"].ToString();
                obj.City = reader["CityName"].ToString();
                obj.CellPhone = reader["CellPhone"].ToString();
                obj.JoinDate = reader["JoinDate"].ToString();
                obj.Jan = reader["Jan"].ToString();
                obj.Feb = reader["Feb"].ToString();
                obj.Mar = reader["Mar"].ToString();
                obj.Apr = reader["Apr"].ToString();
                obj.May = reader["May"].ToString();
                obj.Jun = reader["Jun"].ToString();
                obj.Jul = reader["Jul"].ToString();
                obj.Aug = reader["Aug"].ToString();
                obj.Sep = reader["Sep"].ToString();
                obj.Oct = reader["Oct"].ToString();
                obj.Nov = reader["Nov"].ToString();
                obj.Dec = reader["Dec"].ToString();

                result.Add(obj);
        }
        reader.Close();
        return result;
    }

    internal List<SliceVolunteersPerMonthInfo> GetReportSliceVolunteerPerMonth(string start_date, string end_date)
    {
        List<SliceVolunteersPerMonthInfo> u = U_GetReportSliceVolunteerPerMonth(start_date, end_date);
        return u;
    }

    // מתנדבים מסיעים בחתך חודשי   : פילוחים
    internal List<ReportService.SliceVolunteersCountInMonthInfo> S_GetReportSliceVolunteersCountInMonth(string start_date, string end_date)
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

    // מתנדבים מסיעים בחתך חודשי   : פילוחים
    internal List<ReportService.SliceVolunteersCountInMonthInfo> U_GetReportSliceVolunteersCountInMonth(string start_date, string end_date)
    {
        DBservice_Gilad db = new DBservice_Gilad();

        // Inner-Select - Grouping by pickup time, dest & Orig is part of better accuracy
        // it avoids counting the same ride with multiple patients as 2 rides 
        string query =
            @"Select DisplayName, count(*) as COUNT_C   FROM  
                (SELECT DriverName AS DisplayName, PickupTime, Origin, Destination, count(*) as INNER_C
                  FROM UnityRide ur  
                  WHERE pickuptime <  @end_date
                  AND pickuptime >= @start_date
                  and MainDriver is not null
                  GROUP BY DriverName, PickupTime, Origin, Destination 
                  ) inner_select
                GROUP BY inner_select.DisplayName
            ";
        SqlCommand cmd = new SqlCommand(query);
        cmd.CommandType = CommandType.Text;
        cmd.Parameters.Add("@start_date", SqlDbType.Date).Value = start_date;
        cmd.Parameters.Add("@end_date", SqlDbType.Date).Value = end_date;

        SqlDataReader reader = db.GetDataReaderBySqlCommand(cmd);

        List<SliceVolunteersCountInMonthInfo> result = new List<SliceVolunteersCountInMonthInfo>();

        while (reader.Read())
        {
            SliceVolunteersCountInMonthInfo obj = new SliceVolunteersCountInMonthInfo();
            obj.Volunteer = reader["DisplayName"].ToString();
            obj.Count = reader["COUNT_C"].ToString();
            result.Add(obj);
        }
        reader.Close();
        return result;
    }

    // מתנדבים מסיעים בחתך חודשי   : פילוחים
    internal List<ReportService.SliceVolunteersCountInMonthInfo> GetReportSliceVolunteersCountInMonth(string start_date, string end_date)
    {
        List<SliceVolunteersCountInMonthInfo> u = U_GetReportSliceVolunteersCountInMonth(start_date, end_date);  // << New implementation using United
        return u;                                                         // return results
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

    private List<string> S_GetDistinctListOfField(string field, string table)
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

    private List<string> U_GetDistinctListOfField(string field, string table)
    {
        DBservice_Gilad db = new DBservice_Gilad();
        string query = string.Format("select DISTINCT {0} AS FIELD from {1}", field, table);

        SqlCommand cmd = new SqlCommand(query);
        cmd.CommandType = CommandType.Text;

        SqlDataReader reader = db.GetDataReaderBySqlCommand(cmd);

        List<string> result = new List<string>();

        while (reader.Read())
        {
            result.Add(reader["FIELD"].ToString());
        }
        reader.Close();
        return result;
    }

    // מרכז תיאום - הסעת חולים  לעמותה
    private List<string> GetDistinctListOfField(string field, string table)
    {
        List<string> u = U_GetDistinctListOfField(field, table);  // << New implementation using United
        return u;                                                         // return results
    }

    internal List<string> GetReportHospitals()
    {
        return GetDistinctListOfField("Hospital", "Patient");
    }

    internal List<string> GetReportBarriers()
    {
        return GetDistinctListOfField("Barrier", "Patient");
    }

    // מרכז תיאום - הסעת חולים  לעמותה
    internal List<string> GetReportLocations()
    {
        return GetDistinctListOfField("name", "Location");
    }

    internal List<VolunteerPerRegion> S_GetReportVolunteerWeekly(string start_date, string end_date)
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

    internal List<VolunteerPerRegion> U_GetReportVolunteerWeekly(string start_date, string end_date)
    {
        DBservice_Gilad db = new DBservice_Gilad();
        string query =
            @"select DISTINCT DriverName AS DisplayName, Area
                from UnityRide
                Where Pickuptime < @end_date
                AND pickuptime >=  @start_date
                AND status not like N'נמחקה'
                and DriverName is not null
                ORDER BY Area ASC";

        SqlCommand cmd = new SqlCommand(query);
        cmd.CommandType = CommandType.Text;
        cmd.Parameters.Add("@start_date", SqlDbType.Date).Value = start_date;
        cmd.Parameters.Add("@end_date", SqlDbType.Date).Value = end_date;

        SqlDataReader reader = db.GetDataReaderBySqlCommand(cmd);

        List<VolunteerPerRegion> result = new List<VolunteerPerRegion>();

        while (reader.Read())
        {
            VolunteerPerRegion obj = new VolunteerPerRegion();
            obj.Volunteer = reader["DisplayName"].ToString();
            obj.Region = reader["Area"].ToString();
            result.Add(obj);
        }
        reader.Close();
        return result;
    }

    internal List<VolunteerPerRegion> GetReportVolunteerWeekly(string start_date, string end_date)
    {
        List<VolunteerPerRegion> u = U_GetReportVolunteerWeekly(start_date, end_date);
        return u;
    }

    // DSB - New drivers - 3 different spans
    internal MetricMonthlyInfo U_GetReportNewDriversInRange(string start_date, string end_date)
    {
        DBservice_Gilad db = new DBservice_Gilad();

        string query =
             @"select  count (DISTINCT MainDriver)  AS COUNT_VOL
                from ( 
                    select MainDriver, SUM(IS_OLD) AS COUNT_OLD_DRIVES
                    from
                    ( 
                        select MainDriver, 
	                        CASE WHEN pickuptime < @start_date THEN 1
		                    ELSE 0
	                        END AS IS_OLD
	                    from UnityRide ur 
	                    where pickuptime <= @end_date
	                    AND MainDriver is not null 
                    ) s
                    Group By MainDriver	 
                ) s2
                where COUNT_OLD_DRIVES = '0'
	            order by COUNT_VOL ASC";

        SqlCommand cmd = new SqlCommand(query);
        cmd.CommandType = CommandType.Text;
        cmd.Parameters.Add("@start_date", SqlDbType.Date).Value = start_date;
        cmd.Parameters.Add("@end_date", SqlDbType.Date).Value = end_date;

        SqlDataReader reader = db.GetDataReaderBySqlCommand(cmd);

        MetricMonthlyInfo result = new MetricMonthlyInfo();
        while (reader.Read())
        {
            result.Volunteers = reader["COUNT_VOL"].ToString();
        }

        reader.Close();

        return result;
    }


    // DSB - New drivers - 3 different spans
    // Not bothering keeping the S_ variant as perf is bad.
    internal MetricMonthlyInfo GetReportNewDriversInRange(string start_date, string end_date)
    {
        return U_GetReportNewDriversInRange(start_date, end_date);
    }


    internal List<MetricMonthlyInfo> S_GetReportYearlyGraphMetrics(string start_date, string end_date)
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


    internal List<MetricMonthlyInfo> U_GetReportYearlyGraphMetrics(string start_date, string end_date)
    {
        DBservice_Gilad db = new DBservice_Gilad();

        /* The inner query allows identifying unique rides.
         * If we have same 'MainDriver, PickupTime, Origin, Destination', the ride is shared between patients
         * So we partition over that field combination and generate a runnign index (row_number) for each ride
         * Now to count just uniuqe ride, we put a CASE that gets '1' for each unique ride
         * The unique rides count is the SUM of the new case field
         */

        string query =
             @"SELECT  MONTH(pickuptime) as MONTH_C ,  count(DISTINCT PatientName) as COUNT_PAT,  
                        count(*) as COUNT_DUPLICATE_RIDES, SUM(Unique_Drive_C) as COUNT_UNIQUE_RIDES, count(DISTINCT MainDriver) as COUNT_VOL
                FROM  (
	                select MainDriver  , PickupTime, Origin, Destination, PatientName, 
	                ROW_NUMBER() OVER (PARTITION by MainDriver, PickupTime, Origin, Destination  ORDER BY PickupTime Asc) as row_num_C,
	                CASE WHEN ROW_NUMBER() OVER (PARTITION by MainDriver, PickupTime, Origin, Destination  ORDER BY PickupTime Asc) = '1' THEN 1
		                ELSE 0
	                END AS Unique_Drive_C
	                from UnityRide ur
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

        SqlDataReader reader = db.GetDataReaderBySqlCommand(cmd);

        List<MetricMonthlyInfo> result = new List<MetricMonthlyInfo>();

        while (reader.Read())
        {
            MetricMonthlyInfo obj = new MetricMonthlyInfo();
            obj.Day = reader["MONTH_C"].ToString();
            obj.Rides = reader["COUNT_UNIQUE_RIDES"].ToString();
            obj.Patients = reader["COUNT_PAT"].ToString();
            obj.Volunteers = reader["COUNT_VOL"].ToString();
            result.Add(obj);
        }
        reader.Close();
        return result;
    }

    internal List<MetricMonthlyInfo> GetReportYearlyGraphMetrics(string start_date, string end_date)
    {
        List<MetricMonthlyInfo> u = U_GetReportYearlyGraphMetrics(start_date, end_date);  // << New implementation using United
        return u;                                                         // return results
    }

    internal MetricInfo S_GetReportRangeNeedDriversMetrics(string start_date, string end_date)
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

    // TODO:Unity:   This is not returning same results as RPView. Ask Gilad if he cleaned UnityRide
    internal MetricInfo U_GetReportRangeNeedDriversMetrics(string start_date, string end_date)
    {
        DBservice_Gilad db = new DBservice_Gilad();

        string query =
             @"select RidePatNum AS ID from UnityRide
                where MainDriver is NULL
                and pickuptime > @start_date
                and pickuptime < @end_date
                and Status = N'ממתינה לשיבוץ' ";

        SqlCommand cmd = new SqlCommand(query);
        cmd.CommandType = CommandType.Text;
        cmd.Parameters.Add("@start_date", SqlDbType.Date).Value = start_date;
        cmd.Parameters.Add("@end_date", SqlDbType.Date).Value = end_date;

        SqlDataReader reader = db.GetDataReaderBySqlCommand(cmd);

        int count = 0;
        HashSet<string> uniqueIDs = new HashSet<string>();

        while (reader.Read())
        {
            uniqueIDs.Add(reader["ID"].ToString());
            count++;
        }
        reader.Close();

        MetricInfo result = new MetricInfo
        {
            MetricName = "NeedDrivers",
            Value1 = count,
            Value2 = uniqueIDs.Count
        };
        return result;
    }

    internal MetricInfo GetReportRangeNeedDriversMetrics(string start_date, string end_date)
    {
        MetricInfo u = U_GetReportRangeNeedDriversMetrics(start_date, end_date);  // << New implementation using United

        return u;

    }

    internal List<MetricMonthlyInfo> S_GetReportRangeDigestMetrics(string start_date, string end_date, string prev_start, string prev_end, string query)
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

        bool with_demands = query.Contains("COUNT_DEMAND_RAW");
        List<MetricMonthlyInfo> result = new List<MetricMonthlyInfo>();

        foreach (DataRow dr in dt.Rows)
        {
            MetricMonthlyInfo obj = new MetricMonthlyInfo();
            obj.Day = dr["SPAN_C"].ToString();
            obj.Rides = dr["COUNT_UNIQUE_RIDES"].ToString();
            obj.Patients = dr["COUNT_PAT"].ToString();
            obj.Volunteers = dr["COUNT_VOL"].ToString();
            if (with_demands)
            {
                int raw_count = helper_unsafe_ParseIntger(dr["COUNT_DEMAND_RAW"].ToString());
                int actual_count = raw_count - helper_unsafe_ParseIntger(dr["MULTI_SEG_TO_SUBSTRACT_C"].ToString());
                obj.Demands = actual_count.ToString(); 
            }
            result.Add(obj);
        }

        return result;
    }


    internal List<MetricMonthlyInfo> U_GetReportRangeDigestMetrics(string start_date, string end_date, string prev_start, string prev_end, string query)
    {
        DBservice_Gilad db = new DBservice_Gilad();

        SqlCommand cmd = new SqlCommand(query);
        cmd.CommandType = CommandType.Text;
        cmd.Parameters.Add("@start_date", SqlDbType.Date).Value = start_date;
        cmd.Parameters.Add("@end_date", SqlDbType.Date).Value = end_date;
        if (!prev_start.Equals("NA"))
        {
            cmd.Parameters.Add("@prev_start", SqlDbType.Date).Value = prev_start;
            cmd.Parameters.Add("@prev_end", SqlDbType.Date).Value = prev_end;
        }

        SqlDataReader reader = db.GetDataReaderBySqlCommand(cmd);

        bool with_demands = query.Contains("COUNT_DEMAND_RAW");
        List<MetricMonthlyInfo> result = new List<MetricMonthlyInfo>();

        while (reader.Read())
        {
            MetricMonthlyInfo obj = new MetricMonthlyInfo();
            obj.Day = reader["SPAN_C"].ToString();
            obj.Rides = reader["COUNT_UNIQUE_RIDES"].ToString();
            obj.Patients = reader["COUNT_PAT"].ToString();
            obj.Volunteers = reader["COUNT_VOL"].ToString();
            if (with_demands)
            {
                int raw_count = helper_unsafe_ParseIntger(reader["COUNT_DEMAND_RAW"].ToString());
                int actual_count = raw_count - helper_unsafe_ParseIntger(reader["MULTI_SEG_TO_SUBSTRACT_C"].ToString());
                obj.Demands = actual_count.ToString();
            }
            result.Add(obj);
        }

        reader.Close();
        return result;
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

    // Note: This is used in rows for Week/Month & Year in Dashboard.
    // Recheck all three spans after changing
    internal List<MetricMonthlyInfo> S_GetReportWithPeriodDigestMetrics(string start_date, string end_date, string prev_start, string prev_end, string span)
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


        string query = string.Format(@"SELECT {0} AS SPAN_C, count(DISTINCT DisplayName) as COUNT_PAT, SUM(Unique_Drive_C) as COUNT_UNIQUE_RIDES, 
                count(DISTINCT MainDriver) as COUNT_VOL, count(DisplayName) as COUNT_DEMAND_RAW, SUM(MULTI_SEG_TO_SUBSTRACT) AS MULTI_SEG_TO_SUBSTRACT_C
                FROM  (
	                select MainDriver  , PickupTime, Origin, Destination, DisplayName, 
	                CASE WHEN ROW_NUMBER() OVER (PARTITION by MainDriver, PickupTime, Origin, Destination  ORDER BY PickupTime Asc) = '1' THEN 1
		                ELSE 0
	                END AS Unique_Drive_C,
					CASE WHEN ROW_NUMBER() OVER (PARTITION by FORMAT (PickupTime , 'yyyy-MM-dd'), DisplayName  ORDER BY PickupTime Asc)  = '3' THEN 1
						ELSE 0
					END AS MULTI_SEG_TO_SUBSTRACT 
	                from RPView r 
	                where {1} AND MainDriver is not null
                )  s
                GROUP BY {0} ORDER BY {0} ASC",
                span_column, pickup_time_condition);

        return S_GetReportRangeDigestMetrics(start_date, end_date, prev_start, prev_end, query);
    }

    // Note: This is used in rows for Week/Month & Year in Dashboard.
    // Recheck all three spans after changing
    internal List<MetricMonthlyInfo> U_GetReportWithPeriodDigestMetrics(string start_date, string end_date, string prev_start, string prev_end, string span)
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


        string query = string.Format(@"SELECT {0} AS SPAN_C, count(DISTINCT PatientName) as COUNT_PAT, SUM(Unique_Drive_C) as COUNT_UNIQUE_RIDES, 
                count(DISTINCT MainDriver) as COUNT_VOL, count(PatientName) as COUNT_DEMAND_RAW, SUM(MULTI_SEG_TO_SUBSTRACT) AS MULTI_SEG_TO_SUBSTRACT_C
                FROM  (
	                select MainDriver  , PickupTime, Origin, Destination, PatientName, 
	                CASE WHEN ROW_NUMBER() OVER (PARTITION by MainDriver, PickupTime, Origin, Destination  ORDER BY PickupTime Asc) = '1' THEN 1
		                ELSE 0
	                END AS Unique_Drive_C,
					CASE WHEN ROW_NUMBER() OVER (PARTITION by FORMAT (PickupTime , 'yyyy-MM-dd'), PatientName  ORDER BY PickupTime Asc)  = '3' THEN 1
						ELSE 0
					END AS MULTI_SEG_TO_SUBSTRACT 
	                from UnityRide
	                where {1} AND MainDriver is not null
                )  s
                GROUP BY {0} ORDER BY {0} ASC",
                span_column, pickup_time_condition);

        return U_GetReportRangeDigestMetrics(start_date, end_date, prev_start, prev_end, query);
    }

    internal List<MetricMonthlyInfo> GetReportWithPeriodDigestMetrics(string start_date, string end_date, string prev_start, string prev_end, string span)
    {
        List<MetricMonthlyInfo> u = U_GetReportWithPeriodDigestMetrics(start_date, end_date, prev_start, prev_end, span);  // << New implementation using United
        return u;                                                         // return results
    }



    // Called from dashboard daily rows:   start_current_daily_totals() */
    internal List<MetricMonthlyInfo> S_GetReportDailyDigestMetrics(string start_date, string end_date)
    {
        // Gets info also on rides without an allocted driver
        string query =
             @"SELECT  1 AS SPAN_C, count(DISTINCT DisplayName) as COUNT_PAT, SUM(Unique_Drive_C) as COUNT_UNIQUE_RIDES, 
                    count(DISTINCT MainDriver) as COUNT_VOL, count(DisplayName) as COUNT_DEMAND_RAW, SUM(MULTI_SEG_TO_SUBSTRACT) AS MULTI_SEG_TO_SUBSTRACT_C
                FROM (
	                Select  MainDriver  , PickupTime, Origin, Destination, DisplayName, 
                    CASE WHEN ROW_NUMBER() OVER (PARTITION by MainDriver, PickupTime, Origin, Destination  ORDER BY PickupTime Asc) = '1' THEN 1
	                    ELSE 0
                    END AS Unique_Drive_C,
					CASE WHEN ROW_NUMBER() OVER (PARTITION by FORMAT (PickupTime , 'yyyy-MM-dd'), DisplayName  ORDER BY PickupTime Asc)  = '3' THEN 1
						ELSE 0
					END AS MULTI_SEG_TO_SUBSTRACT 
                    FROM RPView 
                    WHERE  Status != N'נמחקה'
                    AND RPView.pickuptime > @start_date
                    AND RPView.pickuptime < @end_date
                    ) s";

        return S_GetReportRangeDigestMetrics(start_date, end_date, "NA", "NA", query);
    }


    // Called from dashboard daily rows:   start_current_daily_totals() */
    internal List<MetricMonthlyInfo> U_GetReportDailyDigestMetrics(string start_date, string end_date)
    {
        // Gets info also on rides without an allocted driver
        string query =
             @"SELECT  1 AS SPAN_C, count(DISTINCT DisplayName) as COUNT_PAT, SUM(Unique_Drive_C) as COUNT_UNIQUE_RIDES, 
                    count(DISTINCT MainDriver) as COUNT_VOL, count(DisplayName) as COUNT_DEMAND_RAW, SUM(MULTI_SEG_TO_SUBSTRACT) AS MULTI_SEG_TO_SUBSTRACT_C
               FROM (
                    Select  MainDriver  , PickupTime, Origin, Destination, PatientName AS DisplayName, 
                        CASE WHEN ROW_NUMBER() OVER (PARTITION by MainDriver, PickupTime, Origin, Destination  ORDER BY PickupTime Asc) = '1' THEN 1
                        ELSE 0
                        END AS Unique_Drive_C,
			            CASE WHEN ROW_NUMBER() OVER (PARTITION by FORMAT (PickupTime , 'yyyy-MM-dd'), PatientName  ORDER BY PickupTime Asc)  = '3' THEN 1
				        ELSE 0
			            END AS MULTI_SEG_TO_SUBSTRACT 
                    FROM UnityRide ur  
                    WHERE  Status != N'נמחקה'
                    AND ur.pickuptime > @start_date
                    AND ur.pickuptime < @end_date
                    ) s";

        return U_GetReportRangeDigestMetrics(start_date, end_date, "NA", "NA", query);
    }

    internal List<MetricMonthlyInfo> GetReportDailyDigestMetrics(string start_date, string end_date)
    {
        List<MetricMonthlyInfo> u = U_GetReportDailyDigestMetrics(start_date, end_date);  // << New implementation using United
        return u;                                                         // return results
    }

    // Returns the day info as MM-dd string
    // Function Name is incorrect, is used by Month Graph as well.
    // This allows optimizations - all 3 weeks spans in one call, 2 months in one call etc..
    internal List<ReportService.MetricMonthlyInfo> S_GetReportWeeklyGraphMetrics(string start_date, string end_date)
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


    // Returns the day info as MM-dd string
    // Function Name is incorrect, is used by Month Graph as well.
    // This allows optimizations - all 3 weeks spans in one call, 2 months in one call etc..
    internal List<ReportService.MetricMonthlyInfo> U_GetReportWeeklyGraphMetrics(string start_date, string end_date)
    {
        DBservice_Gilad db = new DBservice_Gilad();

        string query =
      @"SELECT FORMAT (PickupTime, 'yyyy-MM-dd') as DAY_C ,  count(DISTINCT PatientName) as COUNT_PAT, SUM(Unique_Drive_C) as COUNT_UNIQUE_RIDES, count(DISTINCT MainDriver) as COUNT_VOL
        FROM (
            Select  MainDriver  , PickupTime, Origin, Destination, PatientName, 
            CASE 
                WHEN ROW_NUMBER() OVER (PARTITION by MainDriver, PickupTime, Origin, Destination  ORDER BY PickupTime Asc) = '1' THEN 1
                ELSE 0
            END AS Unique_Drive_C
            FROM UnityRide ur  
            WHERE MainDriver is not null
            AND ur.pickuptime > @start_date
            AND ur.pickuptime < @end_date
        ) s 
        GROUP BY FORMAT (PickupTime, 'yyyy-MM-dd')
        ORDER BY DAY_C ASC";

        SqlCommand cmd = new SqlCommand(query);
        cmd.CommandType = CommandType.Text;
        cmd.Parameters.Add("@start_date", SqlDbType.Date).Value = start_date;
        cmd.Parameters.Add("@end_date", SqlDbType.Date).Value = end_date;

        SqlDataReader reader = db.GetDataReaderBySqlCommand(cmd);

        List<MetricMonthlyInfo> result = new List<MetricMonthlyInfo>();

        while (reader.Read())
        {
            MetricMonthlyInfo obj = new MetricMonthlyInfo();
            obj.Day = reader["DAY_C"].ToString();
            obj.Rides = reader["COUNT_UNIQUE_RIDES"].ToString();
            obj.Patients = reader["COUNT_PAT"].ToString();
            obj.Volunteers = reader["COUNT_VOL"].ToString();
            result.Add(obj);
        }
        reader.Close();
        return result;

    }

    internal List<ReportService.MetricMonthlyInfo> GetReportWeeklyGraphMetrics(string start_date, string end_date)
    {
        List<ReportService.MetricMonthlyInfo> u = U_GetReportWeeklyGraphMetrics(start_date, end_date);  // << New implementation using United
        return u;                                                         // return results
    }

    // מספר המתנדבים המסיעים פר חודש : לעמותה
    internal List<VolunteersPerMonthInfo> S_GetReportVolunteerPerMonth(string start_date)
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

    // מספר המתנדבים המסיעים פר חודש : לעמותה
    internal List<VolunteersPerMonthInfo> U_GetReportVolunteerPerMonth(string start_date)
    {
        DBservice_Gilad db = new DBservice_Gilad();

        string query =
             @"SELECT count(DISTINCT MainDriver )as COUNT_G, YEAR(PickupTime) as YEAR_G, MONTH(PickupTime) as MONTH_G
                FROM UnityRide
                where PickupTime >= @start_date
                and PickupTime <= CURRENT_TIMESTAMP
                GROUP BY YEAR(PickupTime), MONTH(PickupTime) 
                ORDER  BY YEAR_G, MONTH_G ASC";

        SqlCommand cmd = new SqlCommand(query);
        cmd.CommandType = CommandType.Text;
        cmd.Parameters.Add("@start_date", SqlDbType.Date).Value = start_date;

        SqlDataReader reader = db.GetDataReaderBySqlCommand(cmd);
        
        List<VolunteersPerMonthInfo> result = new List<VolunteersPerMonthInfo>();

        while (reader.Read())
        {
            VolunteersPerMonthInfo obj = new VolunteersPerMonthInfo();
            obj.Year = reader["YEAR_G"].ToString();
            obj.Month = reader["MONTH_G"].ToString();
            obj.Count = reader["COUNT_G"].ToString();
            result.Add(obj);
        }
        reader.Close();
        return result;
    }

    internal List<VolunteersPerMonthInfo> GetReportVolunteerPerMonth(string start_date)
    {
        List<VolunteersPerMonthInfo> u = U_GetReportVolunteerPerMonth(start_date);  // << New implementation using United

        return u;                                                         // return results

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

    // מיוחדים : רשימת מתנדבים לביטוח לאומי
    internal List<VolunteerInfo> GetReportVolunteerList(string cell_phone, string start_date, string only_with_rides)
    {
        List<VolunteerInfo> result = new List<VolunteerInfo>();

        // This service is not to be used by everybody, check if user is entitled for it
        List<string> permissions = this.GetCurrentUserEntitlements(cell_phone);
        if (!permissions.Contains("Record_NI_report"))
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
            if (dr.IsNull("JoinDate"))
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



    //  מתנדבים – שנתי : לעמותה
    internal List<VolunteerKM> S_GetReportVolunteersKM(string start_date, string end_date)
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


    //  מתנדבים – שנתי : לעמותה
    internal List<VolunteerKM> U_GetReportVolunteersKM(string start_date, string end_date)
    {
        DBservice_Gilad db = new DBservice_Gilad();

        string query =
             @"SELECT convert(varchar, PickupTime, 103) AS PickupTime, Origin, Destination, DriverName AS DisplayName, PatientName  AS PatName  
            FROM UnityRide ur  
            WHERE pickuptime >= '2019-1-01'
                and MainDriver is not null
                AND pickuptime < @end_date
                AND pickuptime >= @start_date
            ORDER BY DisplayName ASC
            ";

        SqlCommand cmd = new SqlCommand(query);
        cmd.CommandType = CommandType.Text;
        cmd.Parameters.Add("@start_date", SqlDbType.Date).Value = start_date;
        cmd.Parameters.Add("@end_date", SqlDbType.Date).Value = end_date;

        SqlDataReader reader = db.GetDataReaderBySqlCommand(cmd);
        

        List<VolunteerKM> result = new List<ReportService.VolunteerKM>();

        while (reader.Read())
        {
            ReportService.VolunteerKM obj = new ReportService.VolunteerKM();
            obj.Volunteer = reader["DisplayName"].ToString();
            obj.Patient = reader["PatName"].ToString();
            obj.Origin = reader["Origin"].ToString();
            obj.Destination = reader["Destination"].ToString();
            obj.Date = reader["PickupTime"].ToString();
            result.Add(obj);
        }

        return result;
    }


    internal List<VolunteerKM> GetReportVolunteersKM(string start_date, string end_date)
    {
        List<VolunteerKM> u = U_GetReportVolunteersKM(start_date, end_date);  // << New implementation using United
        return u;   
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


    //  הסעות בתקופה האחרונה   פילוחים
    internal List<VolunteersInPeriod> S_GetReportSliceVolunteersInPeriod(int delta_start, int delta_end)
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

    //  הסעות בתקופה האחרונה   פילוחים
    internal List<VolunteersInPeriod> U_GetReportSliceVolunteersInPeriod(int delta_start, int delta_end)
    {
        DBservice_Gilad db = new DBservice_Gilad();
        List<VolunteersInPeriod> result = new List<VolunteersInPeriod>();

        string query =
@"select  DISTINCT s2.MainDriver, Volunteer.DisplayName, Volunteer.CityCityName, Volunteer.CellPhone
from 
(select MainDriver, SUM(NEW) AS NEW_CNT, SUM(OLD) AS OLD_CNT from 
	(select MainDriver,
		case when CONVERT(date, pickuptime) > CONVERT(date, dateadd(day, @delta_end, getdate())) then 1 else 0 end AS NEW,
		case when CONVERT(date, pickuptime) > CONVERT(date, dateadd(day, @delta_start, getdate())) then 1 else 0 end AS OLD
		from UnityRide ur2  
		where pickuptime <= getdate() 
		and MainDriver is not NULL) S
	GROUP BY MainDriver) s2
INNER JOIN Volunteer on Volunteer.Id = s2.MainDriver 
where NEW_CNT = '0' and not OLD_CNT = '0'
";

        SqlCommand cmd = new SqlCommand(query);
        cmd.CommandType = CommandType.Text;
        cmd.Parameters.Add("@delta_start", SqlDbType.Int).Value = delta_start;
        cmd.Parameters.Add("@delta_end", SqlDbType.Int).Value = delta_end;

        SqlDataReader reader = db.GetDataReaderBySqlCommand(cmd);

        while (reader.Read())
        {
            VolunteersInPeriod obj = new VolunteersInPeriod();
            obj.Id = reader["MainDriver"].ToString();
            obj.Volunteer = reader["DisplayName"].ToString();
            obj.CellPhone = reader["CellPhone"].ToString();
            obj.CityCityName = reader["CityCityName"].ToString();
            result.Add(obj);
        }
        reader.Close();
        return result;
    }

    internal List<VolunteersInPeriod> GetReportSliceVolunteersInPeriod(int delta_start, int delta_end)
    {
        List<VolunteersInPeriod> u = U_GetReportSliceVolunteersInPeriod(delta_start, delta_end);  // << New implementation using United
        return u;                                                         // return results
    }


    internal List<VolunteerPerPatient> S_GetReportVolunteersPerPatient(int patient)
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

    internal List<VolunteerPerPatient> U_GetReportVolunteersPerPatient(int patient)
    {
        DBservice_Gilad db = new DBservice_Gilad();

        string query =
            @"SELECT convert(varchar, PickupTime, 103) AS PickupTime, Origin, Destination, DriverName AS DisplayName
            From UnityRide
            WHERE pickuptime >= '2019-1-01'
            and MainDriver is not null
            and PatientId  =  @patient
            ";

        SqlCommand cmd = new SqlCommand(query);
        cmd.CommandType = CommandType.Text;
        cmd.Parameters.Add("@patient", SqlDbType.Int).Value = patient;

        SqlDataReader reader = db.GetDataReaderBySqlCommand(cmd);

        List<VolunteerPerPatient> result = new List<VolunteerPerPatient>();

        while (reader.Read())
        {
            VolunteerPerPatient obj = new VolunteerPerPatient();
            obj.Volunteer = reader["DisplayName"].ToString();
            obj.Origin = reader["Origin"].ToString();
            obj.Destination = reader["Destination"].ToString();
            obj.Date = reader["PickupTime"].ToString();
            result.Add(obj);
        }

        reader.Close();
        return result;
    }

    internal List<VolunteerPerPatient> GetReportVolunteersPerPatient(int patient)
    {
        List<VolunteerPerPatient> u = U_GetReportVolunteersPerPatient(patient);
        return u;
    }

    public List<RidesForVolunteer> S_GetReportVolunteerRides(int volunteerId, string start_date, string end_date)
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

    public List<RidesForVolunteer> U_GetReportVolunteerRides(int volunteerId, string start_date, string end_date)
    {
        if (volunteerId <= 0)
        {
            throw new ArgumentException("Negative volunteerId is not supported");
        }

        DBservice_Gilad db = new DBservice_Gilad();

        string query = @"SELECT ridepatNum , patientName, Origin, Destination ,PickupTime, Status
                        from UnityRide 
                        WHERE PickupTime < @end_date AND pickuptime >= @start_date 
                        AND MainDriver =  @ID 
                        AND status not like N'נמחקה'";
        SqlCommand cmd = new SqlCommand(query);
        cmd.CommandType = CommandType.Text;
        cmd.Parameters.Add("@ID", SqlDbType.Int).Value = volunteerId;
        cmd.Parameters.Add("@start_date", SqlDbType.Date).Value = start_date;
        cmd.Parameters.Add("@end_date", SqlDbType.Date).Value = end_date;

        SqlDataReader reader = db.GetDataReaderBySqlCommand(cmd);

        List<RidesForVolunteer> result = new List<RidesForVolunteer>();

        try
        {
            while (reader.Read())
            {
                try
                {
                    RidesForVolunteer obj = new RidesForVolunteer();
                    obj.PatDisplayName = reader["patientName"].ToString();
                    obj.OriginName = reader["Origin"].ToString();
                    obj.DestinationName = reader["Destination"].ToString();
                    obj.Date = reader["PickupTime"].ToString();
                    obj.Status = reader["Status"].ToString();

                    result.Add(obj);
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            }

            reader.Close();
            return result;
        }
        catch (Exception e)
        {
            throw e;
        }

    }


    internal bool compare_S_vs_U_results_unordered<T>(List<T> s, List<T> u)
    {
        if (s.Count != u.Count)
        {
            return false;
        }

        HashSet<T> u_set = new HashSet<T>(u);
        foreach (var elem in s)
        {
            if (!u_set.Contains(elem))
            {
                return false;
            }
        }
        return true;
    }


    internal bool compare_S_vs_U_results_ordered<T>(List<T> s, List<T> u)
    {
        return Enumerable.SequenceEqual(s, u);
    }

    public List<RidesForVolunteer> GetReportVolunteerRides(int volunteerId, string start_date, string end_date)

    {
        List<RidesForVolunteer> u_result = U_GetReportVolunteerRides(volunteerId, start_date, end_date);
        return u_result;
    }

    //  מרכז תיאום - מבט יומי :לעמותה
    internal List<CenterDailybyMonthInfo> S_GetReportCenterDailybyMonth(string start_date, string end_date)
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

    //  מרכז תיאום - מבט יומי :לעמותה
    internal List<CenterDailybyMonthInfo> U_GetReportCenterDailybyMonth(string start_date, string end_date)
    {
        DBservice_Gilad db = new DBservice_Gilad();

        string query =
        @"select  count(DISTINCT MainDriver) AS Drivers, count(DISTINCT PatientId ) As Patients, CONVERT(date, pickuptime) as DayInMonth
            FROM UnityRide ur  
            where pickuptime >=  @start_date 
                and pickuptime <=  @end_date
                and MainDriver is not null
            group by CONVERT(date, pickuptime)";

        SqlCommand cmd = new SqlCommand(query);
        cmd.CommandType = CommandType.Text;
        cmd.Parameters.Add("@start_date", SqlDbType.Date).Value = start_date;
        cmd.Parameters.Add("@end_date", SqlDbType.Date).Value = end_date;

        SqlDataReader reader = db.GetDataReaderBySqlCommand(cmd);

        List<CenterDailybyMonthInfo> result = new List<CenterDailybyMonthInfo>();

        while (reader.Read())
        {
            CenterDailybyMonthInfo obj = new CenterDailybyMonthInfo();
            obj.Date = reader["DayInMonth"].ToString();
            obj.VolunteerCount = reader["Drivers"].ToString();
            obj.PatientCount = reader["Patients"].ToString();
            result.Add(obj);
        }

        reader.Close();
        return result;
    }


    internal List<CenterDailybyMonthInfo> GetReportCenterDailybyMonth(string start_date, string end_date)
    {
        {
            List<CenterDailybyMonthInfo> u = U_GetReportCenterDailybyMonth(start_date, end_date);  // << New implementation using United
            return u;                                                         // return results
        }
    }

    //  מרכז תיאום - מבט חודשי : לעמותה
    internal List<CenterMonthlyByYearInfo> S_GetReportCenterMonthlyByYear(string start_date, string end_date)
    {
        DbService db = new DbService();

        string query = @"SELECT  
        MONTH(pickuptime) as MONTH_C, count(DISTINCT DisplayName) as COUNT_PAT,
        count(DISTINCT MainDriver) as COUNT_VOL, SUM(Unique_Drive_C) as COUNT_UNIQUE_RIDE,
        count(DisplayName) as COUNT_DEMAND_RAW, SUM(MULTI_SEG_TO_SUBSTRACT) AS MULTI_SEG_TO_SUBSTRACT_C
        FROM (
	           select MainDriver  , PickupTime, Origin, Destination, DisplayName, 
	              ROW_NUMBER() OVER (PARTITION by MainDriver, PickupTime, Origin, Destination  ORDER BY PickupTime Asc) as row_num_C,
	              CASE WHEN ROW_NUMBER() OVER (PARTITION by MainDriver, PickupTime, Origin, Destination  ORDER BY PickupTime Asc) = '1' THEN 1
		              ELSE 0
	              END AS Unique_Drive_C,
  				  CASE WHEN ROW_NUMBER() OVER (PARTITION by FORMAT (PickupTime , 'yyyy-MM-dd'), DisplayName  ORDER BY PickupTime Asc)  = '3' THEN 1
					ELSE 0
				  END AS MULTI_SEG_TO_SUBSTRACT 
	              from RPView r 
	              where pickuptime >= @start_date 
	                AND pickuptime <= @end_date
	                AND MainDriver is not null
	                AND DisplayName not like N'אנונימי%'
              ) s 
        GROUP BY MONTH(pickuptime)
        ORDER BY MONTH_C ASC";

        SqlCommand cmd = new SqlCommand(query);
        cmd.CommandType = CommandType.Text;
        cmd.Parameters.Add("@start_date", SqlDbType.Date).Value = start_date;
        cmd.Parameters.Add("@end_date", SqlDbType.Date).Value = end_date;

        DataSet ds = db.GetDataSetBySqlCommand(cmd);
        DataTable dt = ds.Tables[0];

        //  call another query so we know how many multi-segment rides we had
        //  IDictionary<string, int>   multiSegMonthDB = secondary_query_GetMultiSegmentRidesMonthly(start_date, end_date);

        List<CenterMonthlyByYearInfo> result = new List<CenterMonthlyByYearInfo>();

        foreach (DataRow dr in dt.Rows)
        {
            CenterMonthlyByYearInfo obj = new CenterMonthlyByYearInfo();
            obj.PatientCount = dr["COUNT_PAT"].ToString();
            obj.VolunteerCount = dr["COUNT_VOL"].ToString();
            obj.RidesCount = dr["COUNT_UNIQUE_RIDE"].ToString();
            obj.Month = dr["MONTH_C"].ToString();

            // substract the number of multi-seg rides
            int orig_count = helper_unsafe_ParseIntger(dr["COUNT_DEMAND_RAW"].ToString());
            int new_value = orig_count - helper_unsafe_ParseIntger(dr["MULTI_SEG_TO_SUBSTRACT_C"].ToString());
            obj.DemandsCount = new_value.ToString();
            result.Add(obj);
        }

        return result;
    }

    //  מרכז תיאום - מבט חודשי : לעמותה
    internal List<CenterMonthlyByYearInfo> U_GetReportCenterMonthlyByYear(string start_date, string end_date)
    {
        DBservice_Gilad db = new DBservice_Gilad();

        string query = @"SELECT  
        MONTH(pickuptime) as MONTH_C, count(DISTINCT PatientName) as COUNT_PAT,
        count(DISTINCT MainDriver) as COUNT_VOL, SUM(Unique_Drive_C) as COUNT_UNIQUE_RIDE,
        count(PatientName) as COUNT_DEMAND_RAW, SUM(MULTI_SEG_TO_SUBSTRACT) AS MULTI_SEG_TO_SUBSTRACT_C
        FROM (
	           select MainDriver  , PickupTime, Origin, Destination, PatientName, 
	              ROW_NUMBER() OVER (PARTITION by MainDriver, PickupTime, Origin, Destination  ORDER BY PickupTime Asc) as row_num_C,
	              CASE WHEN ROW_NUMBER() OVER (PARTITION by MainDriver, PickupTime, Origin, Destination  ORDER BY PickupTime Asc) = '1' THEN 1
		              ELSE 0
	              END AS Unique_Drive_C,
  				  CASE WHEN ROW_NUMBER() OVER (PARTITION by FORMAT (PickupTime , 'yyyy-MM-dd'), PatientName  ORDER BY PickupTime Asc)  = '3' THEN 1
					ELSE 0
				  END AS MULTI_SEG_TO_SUBSTRACT 
	              from UnityRide ur 
	              where pickuptime >= @start_date 
	                AND pickuptime <= @end_date 
	                AND MainDriver is not null
	                AND PatientName not like N'אנונימי%'
              ) s 
        GROUP BY MONTH(pickuptime)
        ORDER BY MONTH_C ASC";

        SqlCommand cmd = new SqlCommand(query);
        cmd.CommandType = CommandType.Text;
        cmd.Parameters.Add("@start_date", SqlDbType.Date).Value = start_date;
        cmd.Parameters.Add("@end_date", SqlDbType.Date).Value = end_date;

        SqlDataReader reader = db.GetDataReaderBySqlCommand(cmd);

        //  call another query so we know how many multi-segment rides we had
        //  IDictionary<string, int>   multiSegMonthDB = secondary_query_GetMultiSegmentRidesMonthly(start_date, end_date);

        List<CenterMonthlyByYearInfo> result = new List<CenterMonthlyByYearInfo>();

        while (reader.Read())
        {
            CenterMonthlyByYearInfo obj = new CenterMonthlyByYearInfo();
            obj.PatientCount = reader["COUNT_PAT"].ToString();
            obj.VolunteerCount = reader["COUNT_VOL"].ToString();
            obj.RidesCount = reader["COUNT_UNIQUE_RIDE"].ToString();
            obj.Month = reader["MONTH_C"].ToString();

            // substract the number of multi-seg rides
            int orig_count = helper_unsafe_ParseIntger(reader["COUNT_DEMAND_RAW"].ToString());
            int new_value = orig_count - helper_unsafe_ParseIntger(reader["MULTI_SEG_TO_SUBSTRACT_C"].ToString());
            obj.DemandsCount = new_value.ToString();
            result.Add(obj);
        }

        reader.Close();
        return result;
    }

    internal List<CenterMonthlyByYearInfo> GetReportCenterMonthlyByYear(string start_date, string end_date)
    {
        List<CenterMonthlyByYearInfo> u = U_GetReportCenterMonthlyByYear(start_date, end_date);  // << New implementation using United
        return u;                                                         // return results

    }

    /* Purpose: calculate how many cases we have of a patient being driven over 2 times a day, e.g.
     *  Barrier --> Some-handover-location -> Hospital  -->  Barrier
     *  e.g.  אייל -- גן שמואל -- רמב"ם -- אייל 
     *  Currently not being used, as we are testing MULTI_SEG_TO_SUBSTRACT_C approach.
     */
    internal IDictionary<string, int> secondary_query_GetMultiSegmentRidesMonthly(string start_date, string end_date)
    {
        DbService db = new DbService();

        string query = @"select  
            MONTH(DATE_C) as MONTH_C, count(* ) AS COUNT_MULTI_SEG
            FROM (
                Select FORMAT (PickupTime, 'yyyy-MM-dd') AS DATE_C, DisplayName, COUNT(*) AS COUNT_C
                from RPView r
                where MainDriver  is Not NULL
                and PickupTime > @start_date
                and PickupTime<@end_date
                AND DisplayName not like N'אנונימי%'
                GROUP BY FORMAT (PickupTime, 'yyyy-MM-dd'), DisplayName
            ) S
            where  COUNT_C > 2
            GROUP BY MONTH(DATE_C)
            ORDER BY MONTH_C ASC
            ";
        SqlCommand cmd = new SqlCommand(query);
        cmd.CommandType = CommandType.Text;
        cmd.Parameters.Add("@start_date", SqlDbType.Date).Value = start_date;
        cmd.Parameters.Add("@end_date", SqlDbType.Date).Value = end_date;

        DataSet ds = db.GetDataSetBySqlCommand(cmd);


        IDictionary<string, int> monthDB = new Dictionary<string, int>();
        DataTable dt = ds.Tables[0];
        foreach (DataRow dr in dt.Rows)
        {
            int count = helper_unsafe_ParseIntger(dr["COUNT_MULTI_SEG"].ToString());
            monthDB.Add(dr["MONTH_C"].ToString(), count);
        }


        return monthDB;
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
            condition += " AND MainDriver is not null";
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

    // מרכז תיאום - הסעת חולים : לעמותה
    internal List<CenterPatientsRidesInfo> S_GetReportCenterPatientsRides(string volunteer, string start_date, string end_date,
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

    // מרכז תיאום - הסעת חולים : לעמותה
    internal List<CenterPatientsRidesInfo> U_GetReportCenterPatientsRides(string volunteer, string start_date, string end_date,
    string origin, string destination)
    {
        DBservice_Gilad db = new DBservice_Gilad();
        SqlCommand cmd = build_command_ReportCenterPatientsRides(volunteer, start_date, end_date, origin, destination);

        string condition = build_condition_ReportCenterPatientsRides(volunteer, origin, destination);

        string query =
        @"select
            FORMAT (PICKUP_TIME_C, 'MM-yy') AS MONTH_C, Origin , Destination, DISPLAY_NAME_C, count(*) AS COUNT_C
            from 
            (  select PickupTime AS PICKUP_TIME_C, Origin , Destination,  DriverName AS DISPLAY_NAME_C
	            FROM UnityRide ur  
	            where pickuptime > @start_date
                AND pickuptime < @end_date " +
                condition +
                @" GROUP BY PickupTime, Origin , Destination, DriverName
            ) s
            GROUP BY FORMAT (PICKUP_TIME_C, 'MM-yy'),  Origin , Destination, DISPLAY_NAME_C
            order by MONTH_C ASC";

        cmd.CommandText = query;

        SqlDataReader reader = db.GetDataReaderBySqlCommand(cmd);
        
        List<CenterPatientsRidesInfo> result = new List<CenterPatientsRidesInfo>();

        while (reader.Read())
        {
            CenterPatientsRidesInfo obj = new CenterPatientsRidesInfo();
            obj.Volunteer = reader["DISPLAY_NAME_C"].ToString();
            obj.Month = reader["MONTH_C"].ToString();
            obj.Origin = reader["Origin"].ToString();
            obj.Destination = reader["Destination"].ToString();
            obj.Count = reader["COUNT_C"].ToString();
            result.Add(obj);
        }

        reader.Close();
        return result;
    }

    // מרכז תיאום - הסעת חולים : לעמותה
    internal List<CenterPatientsRidesInfo> GetReportCenterPatientsRides(string volunteer, string start_date, string end_date,
    string origin, string destination)
    {
        List<CenterPatientsRidesInfo> u = U_GetReportCenterPatientsRides(volunteer, start_date, end_date, origin, destination);  
        return u;                                                         // return results
    }


    // מרכז תיאום - הסעת חולים: לעמותה
    internal string S_GetReportCenterPatientsRidesCount(string volunteer, string start_date, string end_date,
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

    // מרכז תיאום - הסעת חולים: לעמותה
    internal string U_GetReportCenterPatientsRidesCount(string volunteer, string start_date, string end_date,
string origin, string destination)
    {
        DBservice_Gilad db = new DBservice_Gilad();
        SqlCommand cmd = build_command_ReportCenterPatientsRides(volunteer, start_date, end_date, origin, destination);

        string condition = build_condition_ReportCenterPatientsRides(volunteer, origin, destination);

        string query =
        @"select count(PatientId) , count(DISTINCT PatientId) AS PAT_COUNT
          from UnityRide ur  
	      where pickuptime > @start_date
	      AND pickuptime < @end_date  
          " + condition;

        cmd.CommandText = query;

        SqlDataReader reader = db.GetDataReaderBySqlCommand(cmd);
        reader.Read();
        string result = reader["PAT_COUNT"].ToString();
        reader.Close();

        return result;
    }

    internal string GetReportCenterPatientsRidesCount(string volunteer, string start_date, string end_date,
string origin, string destination)
    {
        string u = U_GetReportCenterPatientsRidesCount(volunteer, start_date, end_date, origin, destination);
        return u;      
    }

    private int helper_unsafe_ParseIntger(string s)
    {
        int result = 0;
        try
        {
            result = int.Parse(s);
        }
        catch (Exception)
        {

        }
        return result;
    }

    // מרכז תיאום - הסעות של מחר : לעמותה
    internal List<ReportService.CenterTomorrowsRides> S_GetReportCenterTomorrowsRides(string start_date, string end_date)
    {
        DbService db = new DbService();

        // Create Temporary Table, counting escorts per Ride
        string query = 
            @"select RidePatNum, COUNT(*) AS COUNT_C INTO #ESCORTS_PER_RIDE
                from RidePatEscortView
                GROUP BY RidePatNum ";
        db.GetDataSetByQuery(query,false);  // do not close the connection.

        // Find the records, using LEFT joins to get English names of Orig/Dest
        // Also use the temporary table to count escorts per ride
        query =
             @"select Pickuptime, MainDriver, r.EnglishName, l1.EnglishName as ORIGIN_C, l2.EnglishName AS DEST_C, escorts.COUNT_C AS ESCORTS_C
                from RPView r 
                LEFT JOIN Location l1  ON r.Origin = l1.Name 
                LEFT JOIN Location l2  ON r.Destination = l2.Name 
                LEFT JOIN #ESCORTS_PER_RIDE escorts ON escorts.RidePatNum = r.RidePatNum 
                WHERE MainDriver is not null
                AND pickuptime > @start_date
                AND pickuptime < @end_date
                ORDER BY PickupTime, MainDriver, r.EnglishName ASC";

        SqlCommand cmd = new SqlCommand(query);
        cmd.CommandType = CommandType.Text;
        cmd.Parameters.Add("@start_date", SqlDbType.Date).Value = start_date;
        cmd.Parameters.Add("@end_date", SqlDbType.Date).Value = end_date;

        DataSet ds = db.GetDataSetBySqlCommand(cmd);
        DataTable dt = ds.Tables[0];

        List<ReportService.CenterTomorrowsRides> result = new List<ReportService.CenterTomorrowsRides>();
        foreach (DataRow dr in dt.Rows)
        {
            CenterTomorrowsRides obj = new CenterTomorrowsRides();
            obj.Patient = dr["EnglishName"].ToString();
            obj.Origin = dr["ORIGIN_C"].ToString();
            obj.Destination = dr["DEST_C"].ToString();
            obj.EscortCount = dr["ESCORTS_C"].ToString();
            obj.Pickuptime = dr["Pickuptime"].ToString();
            obj.DriverID = dr["MainDriver"].ToString();
            result.Add(obj);
        }

        db.CloseConnection(); // force freeing teh temporary table
        return result;
    }

    // מרכז תיאום - הסעות של מחר : לעמותה
    internal List<ReportService.CenterTomorrowsRides> U_GetReportCenterTomorrowsRides(string start_date, string end_date)
    {
        DBservice_Gilad db = new DBservice_Gilad();


        // Find the records, using LEFT joins to get English names of Orig/Dest
        string query =
           @"Select Pickuptime, MainDriver, p.EnglishName, l1.EnglishName as ORIGIN_C, l2.EnglishName AS DEST_C, ur.AmountOfEscorts  AS ESCORTS_C
                from UnityRide ur 
                LEFT JOIN Location l1  ON ur.Origin = l1.Name 
                LEFT JOIN Location l2  ON ur.Destination = l2.Name 
                LEFT JOIN Patient p ON ur.PatientId = p.Id 
                WHERE MainDriver is not null
                 AND pickuptime > @start_date
                 AND pickuptime < @end_date
                ORDER BY PickupTime, MainDriver, p.EnglishName ASC";

        SqlCommand cmd = new SqlCommand(query);
        cmd.CommandType = CommandType.Text;
        cmd.Parameters.Add("@start_date", SqlDbType.Date).Value = start_date;
        cmd.Parameters.Add("@end_date", SqlDbType.Date).Value = end_date;

        SqlDataReader reader = db.GetDataReaderBySqlCommand(cmd);

        List<ReportService.CenterTomorrowsRides> result = new List<ReportService.CenterTomorrowsRides>();
        while (reader.Read())
        {
            CenterTomorrowsRides obj = new CenterTomorrowsRides();
            obj.Patient = reader["EnglishName"].ToString();
            obj.Origin = reader["ORIGIN_C"].ToString();
            obj.Destination = reader["DEST_C"].ToString();
            obj.EscortCount = reader["ESCORTS_C"].ToString();
            obj.Pickuptime = reader["Pickuptime"].ToString();
            obj.DriverID = reader["MainDriver"].ToString();
            if (obj.EscortCount.Equals("0"))
            {
                // so it can be the same in comparison
                obj.EscortCount = "";
            }
            result.Add(obj);
        }

        reader.Close();
        return result;
    }

    internal List<ReportService.CenterTomorrowsRides> GetReportCenterTomorrowsRides(string start_date, string end_date)
    {
        List<CenterTomorrowsRides> u = U_GetReportCenterTomorrowsRides(start_date, end_date);  // << New implementation using United
        return u;                                                         // return results
    }


   
}