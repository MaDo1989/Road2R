using log4net;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Web.Services;

/// <summary>
/// Summary description for WebService
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
[System.Web.Script.Services.ScriptService]
public class ReportsWebService : System.Web.Services.WebService
{
    private static readonly ILog Log =
             LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    JavaScriptSerializer j;
    public ReportsWebService()
    {
        j = new JavaScriptSerializer();
        j.MaxJsonLength = int.MaxValue;
        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }


    //----------------------Road to Recovery-----------------------------------------------

    private HttpResponse GzipMe()
    {
        string AcceptEncoding = HttpContext.Current.Request.Headers["Accept-Encoding"];
        HttpResponse Response = HttpContext.Current.Response;
        if (AcceptEncoding.Contains("gzip"))
        {
            //HttpResponse Response = HttpContext.Current.Response;
            Response.Filter = new System.IO.Compression.GZipStream(Response.Filter, System.IO.Compression.CompressionMode.Compress);
            Response.Headers.Remove("Content-Encoding");
            Response.AppendHeader("Content-Encoding", "gzip");
        }
        return Response;
    }




    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public List<ReportService.RidesForVolunteer> GetReportVolunteerRides(int volunteerId, string start_date, string end_date)
    {
        try
        {
            HttpResponse response = GzipMe();

            ReportService report = new ReportService();
            List<ReportService.RidesForVolunteer> r = report.GetReportVolunteerRides(volunteerId, start_date, end_date);
            return r;
        }
        catch (Exception ex)
        {
            Log.Error("Error in GetReportVolunteerRides", ex);
            throw new Exception("שגיאה בשליפת נתוני הסעות");
        }

    }

    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string GetReportVolunteerWeekly(string start_date, string end_date)
    {
        try
        {
            HttpResponse response = GzipMe();

            ReportService report = new ReportService();
            List<ReportService.VolunteerPerRegion> r = report.GetReportVolunteerWeekly(start_date, end_date);
            j.MaxJsonLength = Int32.MaxValue;
            return j.Serialize(r);
        }
        catch (Exception ex)
        {
            Log.Error("Error in GetReportVolunteerWeekly", ex);
            throw new Exception("שגיאה בשליפת נתוני הסעות");
        }

    }

    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string GetReportVolunteerList(string start_date, string only_with_rides)
    {
        try
        {
            string cell_phone = (string)HttpContext.Current.Session["userSession"];
            HttpResponse response = GzipMe();
            ReportService report = new ReportService();
            List<ReportService.VolunteerInfo> r = report.GetReportVolunteerList(cell_phone, start_date, only_with_rides);
            j.MaxJsonLength = Int32.MaxValue;
            return j.Serialize(r);
        }
        catch (Exception ex)
        {
            Log.Error("Error in GetReportVolunteerList", ex);
            throw new Exception("שגיאה בשליפת נתוני הסעות");
        }

    }


    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string GetReportVolunteerPerMonth(string start_date)
    {
        try
        {
            HttpResponse response = GzipMe();

            ReportService report = new ReportService();
            List<ReportService.VolunteersPerMonthInfo> r = report.GetReportVolunteerPerMonth(start_date);
            j.MaxJsonLength = Int32.MaxValue;
            return j.Serialize(r);
        }
        catch (Exception ex)
        {
            Log.Error("Error in GetReportVolunteerPerMonth", ex);
            throw new Exception("שגיאה בשליפת נתוני הסעות");
        }

    }

    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public List<ReportService.SliceVolunteersPerMonthInfo> GetReportSliceVolunteerPerMonth(string start_date, string end_date)
    {
        try
        {
            HttpResponse response = GzipMe();

            ReportService report = new ReportService();
            List<ReportService.SliceVolunteersPerMonthInfo> r = report.GetReportSliceVolunteerPerMonth(start_date, end_date);
            return r;
        }
        catch (Exception ex)
        {
            Log.Error("Error in SliceVolunteersPerMonthInfo", ex);
            throw new Exception("שגיאה בשליפת נתוני הסעות");
        }

    }




    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public List<ReportService.SliceVolunteersCountInMonthInfo> GetReportSliceVolunteersCountInMonth(string start_date, string end_date)
    {
        try
        {
            HttpResponse response = GzipMe();

            ReportService report = new ReportService();
            List<ReportService.SliceVolunteersCountInMonthInfo> r = report.GetReportSliceVolunteersCountInMonth(start_date, end_date);
            return r;
        }
        catch (Exception ex)
        {
            Log.Error("Error in GetReportSliceVolunteersCountInMonth", ex);
            throw new Exception("שגיאה בשליפת נתוני הסעות");
        }

    }


    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public List<ReportService.VolunteerPerPatient> GetReportVolunteersPerPatient(int patient)
    {
        try
        {
            HttpResponse response = GzipMe();

            ReportService report = new ReportService();
            List<ReportService.VolunteerPerPatient> r = report.GetReportVolunteersPerPatient(patient);
            return r;
        }
        catch (Exception ex)
        {
            Log.Error("Error in GetReportVolunteerWeekly", ex);
            throw new Exception("שגיאה בשליפת נתוני הסעות");
        }

    }




    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string CommitReportedVolunteerListToNI_DB(string start_date, string only_with_rides)
    {
        try
        {
            string cell_phone = (string)HttpContext.Current.Session["userSession"];
            HttpResponse response = GzipMe();

            ReportService report = new ReportService();
            report.CommitReportedVolunteerListToNI_DB(cell_phone, start_date, only_with_rides);
            return "OK";
        }
        catch (Exception ex)
        {
            Log.Error("Error in CommitReportedVolunteerListToNI_DB", ex);
            throw new Exception("שגיאה בשליפת נתוני הסעות");
        }

    }


    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string GetReportVolunteersKM(string start_date, string end_date)
    {
        try
        {
            HttpResponse response = GzipMe();

            ReportService report = new ReportService();
            List<ReportService.VolunteerKM> r = report.GetReportVolunteersKM(start_date, end_date);
            j.MaxJsonLength = Int32.MaxValue;
            return j.Serialize(r);
        }
        catch (Exception ex)
        {
            Log.Error("Error in GetReportVolunteerWeekly", ex);
            throw new Exception("שגיאה בשליפת נתוני הסעות");
        }

    }


    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public List<ReportService.VolunteersInPeriod> GetReportSliceVolunteersInPeriod(string start_number, string end_number)
    {
        try
        {
            HttpResponse response = GzipMe();
            int delta_start = 0 - Int32.Parse(start_number);
            int delta_end = 0 - Int32.Parse(end_number);

            ReportService report = new ReportService();
            List<ReportService.VolunteersInPeriod> r = report.GetReportSliceVolunteersInPeriod(delta_start, delta_end);
            return r;
        }
        catch (Exception ex)
        {
            Log.Error("Error in GetReportSliceVolunteersInPeriod", ex);
            throw new Exception("שגיאה בשליפת נתוני הסעות");
        }

    }


    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string GetPatientsDisplayNames()
    {
        try
        {
            HttpResponse response = GzipMe();

            ReportService report = new ReportService();
            List<ReportService.NameIDPair> r = report.GetPatientsDisplayNames();
            j.MaxJsonLength = Int32.MaxValue;
            return j.Serialize(r);
        }
        catch (Exception ex)
        {
            Log.Error("Error in GetPatientsDisplayNames", ex);
            throw new Exception("שגיאה בשליפת נתוני הסעות");
        }

    }

    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public List<string> GetCurrentUserEntitlements()
    {
        try
        {
            string cell_phone = (string)HttpContext.Current.Session["userSession"];
            ReportService report = new ReportService();
            List<string> r = report.GetCurrentUserEntitlements(cell_phone);
            return r;
        }
        catch (Exception ex)
        {
            Log.Error("Error in GetUserEntitlements", ex);
            throw new Exception("שגיאה בשליפת נתוני הסעות");
        }
    }


    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public List<ReportService.CenterDailybyMonthInfo> GetReportCenterDailybyMonth(string start_date, string end_date)
    {
        try
        {
            HttpResponse response = GzipMe();

            ReportService report = new ReportService();
            List<ReportService.CenterDailybyMonthInfo> r = report.GetReportCenterDailybyMonth(start_date, end_date);
            return r;
        }
        catch (Exception ex)
        {
            Log.Error("Error in GetReportCenterDailybyMonth", ex);
            throw new Exception("שגיאה בשליפת נתוני הסעות");
        }

    }


    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public List<ReportService.CenterMonthlyByYearInfo> GetReportCenterMonthlyByYear(string start_date, string end_date)
    {
        try
        {
            HttpResponse response = GzipMe();

            ReportService report = new ReportService();
            List<ReportService.CenterMonthlyByYearInfo> r = report.GetReportCenterMonthlyByYear(start_date, end_date);
            return r;
        }
        catch (Exception ex)
        {
            Log.Error("Error in GetReportCenterMonthlyByYear", ex);
            throw new Exception("שגיאה בשליפת נתוני הסעות");
        }

    }


    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public List<ReportService.CenterPatientsRidesInfo> GetReportCenterPatientsRides(string volunteer, string start_date, string end_date,
        string origin, string destination)
    {
        try
        {
            HttpResponse response = GzipMe();

            ReportService report = new ReportService();
            List<ReportService.CenterPatientsRidesInfo> r = report.GetReportCenterPatientsRides(volunteer, start_date, end_date, origin, destination);
            return r;
        }
        catch (Exception ex)
        {
            Log.Error("Error in GetReportCenterPatientsRides", ex);
            throw new Exception("שגיאה בשליפת נתוני הסעות");
        }

    }




    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string GetReportCenterPatientsRidesCount(string volunteer, string start_date, string end_date,
        string origin, string destination)
    {
        try
        {
            HttpResponse response = GzipMe();

            ReportService report = new ReportService();
            string r = report.GetReportCenterPatientsRidesCount(volunteer, start_date, end_date, origin, destination);
            return r;
        }
        catch (Exception ex)
        {
            Log.Error("Error in GetReportCenterPatientsRidesCount", ex);
            throw new Exception("שגיאה בשליפת נתוני הסעות");
        }

    }


    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public List<string> GetReportHospitals()
    {
        try
        {
            HttpResponse response = GzipMe();

            ReportService report = new ReportService();
            List<string> r = report.GetReportHospitals();
            return r;
        }
        catch (Exception ex)
        {
            Log.Error("Error in GetReportHospitals", ex);
            throw new Exception("שגיאה בשליפת נתוני הסעות");
        }

    }

    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public List<string> GetReportBarriers()
    {
        try
        {
            HttpResponse response = GzipMe();

            ReportService report = new ReportService();
            List<string> r = report.GetReportBarriers();
            return r;
        }
        catch (Exception ex)
        {
            Log.Error("Error in GetReportBarriers", ex);
            throw new Exception("שגיאה בשליפת נתוני הסעות");
        }

    }

    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public List<string> GetReportLocations()
    {
        try
        {
            HttpResponse response = GzipMe();

            ReportService report = new ReportService();
            List<string> r = report.GetReportLocations();
            return r;
        }
        catch (Exception ex)
        {
            Log.Error("Error in GetReportLocations", ex);
            throw new Exception("שגיאה בשליפת נתוני הסעות");
        }

    }

    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public List<ReportService.MetricMonthlyInfo> GetReportWeeklyGraphMetrics(string start_date, string end_date)
    {
        try
        {
            HttpResponse response = GzipMe();

            ReportService report = new ReportService();
            List<ReportService.MetricMonthlyInfo> r = report.GetReportWeeklyGraphMetrics(start_date, end_date);
            return r;
        }
        catch (Exception ex)
        {
            Log.Error("Error in GetReportMonthlyMetrics", ex);
            throw new Exception("שגיאה בשליפת נתוני הסעות");
        }

    }



    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public List<ReportService.MetricMonthlyInfo> GetReportDailyDigestMetrics(string start_date, string end_date)
    {
        try
        {
            HttpResponse response = GzipMe();

            ReportService report = new ReportService();
            List<ReportService.MetricMonthlyInfo> r = report.GetReportDailyDigestMetrics(start_date, end_date);
            return r;
        }
        catch (Exception ex)
        {
            Log.Error("Error in GetReportDailyDigestMetrics", ex);
            throw new Exception("שגיאה בשליפת נתוני הסעות");
        }

    }

    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public List<ReportService.MetricMonthlyInfo> GetReportWithPeriodDigestMetrics(string start_date, string end_date,
        string prev_start, string prev_end, string span)
    {
        try
        {
            HttpResponse response = GzipMe();

            ReportService report = new ReportService();
            List<ReportService.MetricMonthlyInfo> r = report.GetReportWithPeriodDigestMetrics(start_date, end_date, prev_start, prev_end, span);
            return r;
        }
        catch (Exception ex)
        {
            Log.Error("Error in GetReportWithPeriodDigestMetrics", ex);
            throw new Exception("שגיאה בשליפת נתוני הסעות");
        }

    }



    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public ReportService.MetricMonthlyInfo GetReportNewDriversInRange(string start_date, string end_date)
    {
        try
        {
            HttpResponse response = GzipMe();

            ReportService report = new ReportService();
            ReportService.MetricMonthlyInfo r = report.GetReportNewDriversInRange(start_date, end_date);
            return r;
        }
        catch (Exception ex)
        {
            Log.Error("Error in GetReportNewDriversInRange", ex);
            throw new Exception("שגיאה בשליפת נתוני הסעות");
        }

    }

    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public List<ReportService.MetricMonthlyInfo> GetReportYearlyGraphMetrics(string start_date, string end_date)
    {
        try
        {
            HttpResponse response = GzipMe();

            ReportService report = new ReportService();
            List<ReportService.MetricMonthlyInfo> r = report.GetReportYearlyGraphMetrics(start_date, end_date);
            return r;
        }
        catch (Exception ex)
        {
            Log.Error("Error in GetReportMonthlyMetrics", ex);
            throw new Exception("שגיאה בשליפת נתוני הסעות");
        }

    }

    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public ReportService.MetricInfo GetReportRangeNeedDriversMetrics(string start_date, string end_date)
    {
        try
        {
            HttpResponse response = GzipMe();

            ReportService report = new ReportService();
            ReportService.MetricInfo r = report.GetReportRangeNeedDriversMetrics(start_date, end_date);
            return r;
        }
        catch (Exception ex)
        {
            Log.Error("Error in GetReportMonthlyMetrics", ex);
            throw new Exception("שגיאה בשליפת נתוני הסעות");
        }

    }

    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public List<ReportService.CenterTomorrowsRides> GetReportCenterTomorrowsRides(string start_date, string end_date)
    {
        try
        {
            HttpResponse response = GzipMe();

            ReportService report = new ReportService();
            List<ReportService.CenterTomorrowsRides> r = report.GetReportCenterTomorrowsRides(start_date, end_date);
            return r;
        }
        catch (Exception ex)
        {
            Log.Error("Error in GetReportCenterTomorrowsRides", ex);
            throw new Exception("שגיאה בשליפת נתוני הסעות");
        }

    }


}

