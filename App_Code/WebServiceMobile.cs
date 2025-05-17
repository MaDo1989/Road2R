using System;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Web.Services;

/// <summary>
/// Summary description for WebServiceMobile
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
[System.Web.Script.Services.ScriptService]
public class WebServiceMobile : System.Web.Services.WebService
{

    JavaScriptSerializer j;


    public WebServiceMobile()
    {
        j = new JavaScriptSerializer();
        j.MaxJsonLength = int.MaxValue;
        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string getPastRidesSlim(int volunteerId)
    {
        try
        {
            GzipMe();
            RideSlimExt r = new RideSlimExt();
            Object rl = r.GetPastRides(volunteerId);
            return j.Serialize(rl);
        }
        catch (Exception ex)
        {
            throw new Exception(" שגיאה בשליפת נתוני הסעות עבר");
        }
    }

    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string getMyRidesSlim(int volunteerId) // returns future rides only
    {
        try
        {
            GzipMe();
            RideSlimExt r = new RideSlimExt();
            Object rl = r.GetFutureRides(volunteerId);
            return j.Serialize(rl);
        }
        catch (Exception ex)
        {
            throw new Exception(" שגיאה בשליפת נתוני הסעות עבר");
        }
    }

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
    public string GetRidePatViewSlim(int maxDays)
    {
        try
        {
            HttpResponse response = GzipMe();

            RideSlimExt rp = new RideSlimExt();
            Object r = rp.GetRidePatView(maxDays);
            j.MaxJsonLength = Int32.MaxValue;
            return j.Serialize(r);
        }
        catch (Exception ex)
        {
            CatchErrors catchErrors = new CatchErrors("WebService: Exception in GetRidePatView", ex + " " + ex.Message + " " + ex.InnerException + " " + ex.Source, ex.StackTrace);
            //Log.Error("Error in GetRidePatView", ex);
            throw new Exception("שגיאה בשליפת נתוני הסעות");
        }

    }

}
