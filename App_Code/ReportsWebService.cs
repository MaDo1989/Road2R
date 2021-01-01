using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Services;
using System.Globalization;
using System.Web.Script.Services;
using log4net;
using System.Web.UI;

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
    public string GetReportVolunteerList(string start_date, string config)
    {
        try
        {
            string cell_phone = (string)HttpContext.Current.Session["userSession"];
            HttpResponse response = GzipMe();
            ReportService report = new ReportService();
            List<ReportService.VolunteerInfo> r = report.GetReportVolunteerList(cell_phone, start_date, config);
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
    public string CommitReportedVolunteerListToNI_DB(string start_date, string config)
    {
        try
        {
            string cell_phone = (string)HttpContext.Current.Session["userSession"];
            HttpResponse response = GzipMe();

            ReportService report = new ReportService();
            report.CommitReportedVolunteerListToNI_DB(cell_phone, start_date, config);
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

}

