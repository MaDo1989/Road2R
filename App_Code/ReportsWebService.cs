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
    public string GetReportVolunteerRides(int volunteerId, string start_date, string end_date)
    {
        try
        {
            HttpResponse response = GzipMe();
            //string AcceptEncoding = HttpContext.Current.Request.Headers["Accept-Encoding"];
            //if (AcceptEncoding.Contains("gzip"))
            //{
            //    HttpResponse Response = HttpContext.Current.Response;
            //    Response.Filter = new System.IO.Compression.GZipStream(Response.Filter, System.IO.Compression.CompressionMode.Compress);
            //    Response.Headers.Remove("Content-Encoding");
            //    Response.AppendHeader("Content-Encoding", "gzip");
            //}

            ReportService report = new ReportService();
            List<RidePat> r = report.GetReportVolunteerRides(volunteerId, start_date, end_date);
            j.MaxJsonLength = Int32.MaxValue;
            return j.Serialize(r);
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
    public string GetReportVolunteersPerPatient(int patient)
    {
        try
        {
            HttpResponse response = GzipMe();

            ReportService report = new ReportService();
            List<ReportService.VolunteerPerPatient> r = report.GetReportVolunteersPerPatient(patient);
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



}

