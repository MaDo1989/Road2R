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
using System.Configuration;
using System.Collections;
using System.Activities.Statements;

/// <summary>
/// Summary description for WebService
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
[System.Web.Script.Services.ScriptService]
public class WebService : System.Web.Services.WebService
{
    private static readonly ILog Log =
             LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    JavaScriptSerializer j;
    public WebService()
    {
        j = new JavaScriptSerializer();
        j.MaxJsonLength = int.MaxValue;
        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }


    //----------------------Road to Recovery-----------------------------------------------
    //[WebMethod(EnableSession = true)]
    //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    //public void test(Class1 c)
    //{
    //  //  Test test = t;

    //    //Dictionary<string, object> d = (Dictionary<string, object>)ridePat;
    //    // d = (Dictionary<string, object>)ridePat;
    //    // JavaScriptSerializer j = new JavaScriptSerializer();
    //    // Object[] o = j.Deserialize<object[]>(ridePat);
    //    // RidePat rp = new RidePat();
    //    //rp.setRidePat(d, func);
    //    // return j.Serialize(rp);
    //    var a = 0;
    //}

    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string isPrimaryStillCanceled(int driverID, int rideID)
    {
        try
        {
            Ride r = new Ride();
            bool res = r.isPrimaryStillCanceled(rideID, driverID);
            return j.Serialize(res);
        }
        catch (Exception ex)
        {
            Log.Error("Error in isPrimaryStillCanceled", ex);
            throw new Exception("שגיאה בבדיקה האם נהג ראשי מבוטל");
        }

    }

    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string backupToPrimary(int rideID, int driverID)
    {
        try
        {
            Ride r = new Ride();
            int res = r.backupToPrimary(rideID, driverID);
            return j.Serialize(res);
        }
        catch (Exception ex)
        {
            Log.Error("Error in backupToPrimary", ex);
            throw new Exception("שגיאה בעדכון נהג ראשי");
        }

    }

    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string getLocations()
    {
        try
        {
            List<Location> ll = new List<Location>();
            Location l = new Location();
            ll.AddRange(l.getHospitalListForView(true));
            ll.AddRange(l.getBarrierListForView(true));


            return j.Serialize(ll);
        }
        catch (Exception e)
        {
            Log.Error("Error in getLocations", e);
            throw new Exception("שגיאה בשליפת נתוני נקודות איסוף והורדה");
        }
    }
    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string getAreas()
    {
        try
        {
            List<string> areas = new List<string>();
            Location l = new Location();
            areas = l.getAreas();
            return j.Serialize(areas);
        }
        catch (Exception e)
        {
            Log.Error("Error in getAreas", e);
            throw new Exception("שגיאה בשליפת אזורים");
        }
    }
    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string ChangeRidepatAreas()
    {
        try
        {
            RidePat rp = new RidePat();
            rp.setRidepatsArea();
            return j.Serialize("");
        }
        catch (Exception e)
        {
            Log.Error("Error in getAreas", e);
            throw new Exception("שגיאה בשליפת אזורים");
        }
    }
    [WebMethod(EnableSession = true)]
    public int setVolunteerPrefs(int Id, List<string> PrefLocation, List<string> PrefArea, List<string> PrefTime, int AvailableSeats)
    {
        try
        {
            Volunteer v = new Volunteer();
            return v.setVolunteerPrefs(Id, PrefLocation, PrefArea, PrefTime, AvailableSeats);

        }
        catch (Exception ex)
        {
            Log.Error("Error in setVolunteerPrefs", ex);
            throw new Exception("שגיאה בעדכון העדפות המתנדב");
        }
    }

    [WebMethod(EnableSession = true)]
    public string getescortedsListMobile(string displayName, string patientCell)
    {
        try
        {
            Patient p = new Patient();
            List<Escorted> escortedsList = p.getescortedsListMobile(displayName, patientCell);
            return j.Serialize(escortedsList);
        }
        catch (Exception ex)
        {
            Log.Error("Error in getPatientEscorted", ex);
            throw new Exception("שגיאה בשליפת נתוני מלווים");
        }
    }
    [WebMethod(EnableSession = true)]
    public string getVolunteerPrefs(int Id)
    {
        try
        {
            Volunteer v = new Volunteer();
            v = v.getVolunteerPrefs(Id);
            return j.Serialize(v);
        }
        catch (Exception ex)
        {
            Log.Error("Error in getVolunteerPrefs", ex);
            throw new Exception("שגיאה בשליפת נתוני העדפות המתנדב");

        }
    }
    //לסדר
    [WebMethod(EnableSession = true)]
    public int setRidePat(RidePat RidePat, string func, bool isAnonymous, int numberOfRides, string repeatRideEvery)
    {
        try
        {
            RidePat rp = new RidePat();
            int res = rp.setRidePat(RidePat, func, isAnonymous, numberOfRides, repeatRideEvery);

            if (res > 0 && func == "delete")
            {
                string message = "";
                message = " נסיעה מספר " + RidePat.RidePatNum + " מ" + RidePat.Origin.Name + " ל" + RidePat.Destination.Name + " עם החולה " + RidePat.Pat.DisplayName + " בוטלה.";
                LogEntry le = new LogEntry(DateTime.Now, "info", message, 1);
            }
            return res;
        }
        catch (ArgumentException ex)
        {

            Log.Error("Error in setRidePat - same ride isue in date: ", ex);
            throw new Exception(j.Serialize(" ההסעה בתאריך " + ex.Message + " כבר קיימת.\nאנא בחר תאריך חדש."));
        }

        catch (Exception ex)
        {
            Log.Error("Error in setRidePat", ex);
            throw new Exception("שגיאה בפתיחה/עדכון/מחיקה של הסעה חדשה");

        }
    }

    //changed the return value from int to void for logentry
    [WebMethod(EnableSession = true)]
    public int setRideStatus(int rideId, string status)
    {
        Ride r = new Ride();

        //write to log
        Auxiliary a = new Auxiliary();
        string message = "";
        try
        {
            // string coor;
            int driverId = a.GetDriverId(rideId);
            if (driverId < 0)
            {
                message = " נסיעה מספר " + rideId + "שינתה סטטוס ל " + status;
            }
            else
            {
                message = "  נסיעה מספר " + rideId + " עם הנהג " + a.getDriverName(driverId) + " שינתה סטטוס ל" + status;
            }
            LogEntry le = new LogEntry(DateTime.Now, "שינוי סטטוס", message, 2, rideId, true);
            return r.setStatus(rideId, status);
        }
        catch (Exception ex)
        {
            Log.Error("Error in setRideStatus", ex);
            throw new Exception(" שגיאה בשינוי הסטטוס");
        }

    }




    //This method is used for שבץ אותי
    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]

    public string GetMessi(string active)
    {
        return active;
        //try
        //{
        //    HttpResponse response = GzipMe();
        //    //string AcceptEncoding = HttpContext.Current.Request.Headers["Accept-Encoding"];
        //    //if (AcceptEncoding.Contains("gzip"))
        //    //{
        //    //    HttpResponse Response = HttpContext.Current.Response;
        //    //    Response.Filter = new System.IO.Compression.GZipStream(Response.Filter, System.IO.Compression.CompressionMode.Compress);
        //    //    Response.Headers.Remove("Content-Encoding");
        //    //    Response.AppendHeader("Content-Encoding", "gzip");
        //    //}

        //    RidePat rp = new RidePat();
        //    List<RidePat> r = rp.GetRidePatView(volunteerId, maxDays);
        //    j.MaxJsonLength = Int32.MaxValue;
        //    return j.Serialize(r);
        //}
        //catch (Exception ex)
        //{
        //    Log.Error("Error in GetRidePatView", ex);
        //    throw new Exception("שגיאה בשליפת נתוני הסעות");
        //}

    }





    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string getPatients(string active = "true")
    {
        bool activeBool = Convert.ToBoolean(active);
        try
        {
            HttpResponse response = GzipMe();

            Patient c = new Patient();
            List<Patient> patientsList = c.getPatientsList(activeBool);
            //j.MaxJsonLength = int.MaxValue;
            j.MaxJsonLength = Int32.MaxValue;
            return j.Serialize(patientsList);
        }
        catch (Exception ex)
        {
            Log.Error("Error in getPatients", ex);
            //throw new Exception("שגיאה בשליפת נתוני חולים");
            throw ex;
        }

    }
    [WebMethod(EnableSession = true)]
    public string getPatients1()
    {
        try
        {
            Patient c = new Patient();
            List<Patient> patientsList = c.getPatientsList(true);
            return j.Serialize(patientsList);
        }
        catch (Exception ex)
        {
            Log.Error("Error in getPatients", ex);
            throw new Exception("שגיאה בשליפת נתוני חולים");
        }

    }

    [WebMethod(Description = "get patients with the same origin and destination")]
    public string getPatientsForAnonymous(bool active, string origin, string dest)
    {
        try
        {
            HttpResponse response = GzipMe();
            Patient c = new Patient();
            List<Patient> patientsList = c.getAnonymousPatientsList(active, origin, dest);
            return j.Serialize(patientsList);
        }
        catch (Exception ex)
        {
            Log.Error("Error in getPatientsForAnonymous", ex);
            throw new Exception("שגיאה בשליפת נתוני חולים");
        }

    }
    [WebMethod(EnableSession = true)]
    public string getAnonymousPatientsListForArea(bool active, string origin, string dest, string area)
    {
        try
        {
            HttpResponse response = GzipMe();
            Patient c = new Patient();
            List<Patient> patientsList = c.getAnonymousPatientsListForLocations(active, origin, dest, area);
            return j.Serialize(patientsList);
        }
        catch (Exception ex)
        {
            Log.Error("Error in getPatientsForAnonymous", ex);
            throw new Exception("שגיאה בשליפת נתוני חולים");
        }

    }
    [WebMethod(EnableSession = true)]
    public string getEquipmentForPatient(string patient)
    {
        try
        {
            Patient p = new Patient();
            p.Equipment = p.getEquipmentForPatient(patient);
            return j.Serialize(p.Equipment);

        }
        catch (Exception ex)
        {
            Log.Error("Error in getEquipmentForPatient", ex);
            throw new Exception("שגיאה בשליפת נתוני ציוד חולים");

        }
    }

    [WebMethod(EnableSession = true)]
    public string getCoorList()
    {
        try
        {
            Volunteer v = new Volunteer();
            List<Volunteer> vl = v.getCoorList();
            return j.Serialize(vl);
        }
        catch (Exception ex)
        {
            Log.Error("Error in getCoorList", ex);
            throw new Exception("שגיאה בשליפת נתוני רכזים");
        }

    }

    [WebMethod(EnableSession = true)]
    public string getCoor(string userName)
    {
        try
        {
            Volunteer v = new Volunteer();
            v = v.getCoor(userName);
            return j.Serialize(v);
        }
        catch (Exception ex)
        {
            Log.Error("Error in getCoor", ex);
            throw new Exception("שגיאה בשליפת נתוני רכז");
        }

    }

    [WebMethod(EnableSession = true)]
    public string getPatientEscorted(string displayName, string caller)
    {
        try
        {
            Patient p = new Patient();
            List<Escorted> escortedsList = p.getescortedsList(displayName, caller);
            return j.Serialize(escortedsList);
        }
        catch (Exception ex)
        {
            Log.Error("Error in getPatientEscorted", ex);
            throw new Exception("שגיאה בשליפת נתוני מלווים");
        }

    }
    [WebMethod(EnableSession = true)]
    public string GetVolunteerPrefArea(int Id)
    {
        try
        {
            Volunteer v = new Volunteer();
            List<string> areas = v.getPrefArea(Id);
            JavaScriptSerializer j = new JavaScriptSerializer();
            return j.Serialize(areas);
        }
        catch (Exception ex)
        {
            Log.Error("Error in GetVolunteerPrefArea", ex);
            throw new Exception("שגיאה בשליפת נתוני אזורים של המתנדב");

        }
    }

    [WebMethod(EnableSession = true)]
    public int getSpaceInCar(int ridePatNum, int driverId)
    {
        try
        {
            AnonymousPatient ap = new AnonymousPatient();
            int seats = ap.checkSpaceInCar(ridePatNum, driverId);
            return seats;
        }
        catch (Exception ex)
        {
            Log.Error("Error in getSpaceInCar", ex);
            throw new Exception("שגיאה בשליפת מקומות פנויים ברכב");
        }

    }

    [WebMethod(EnableSession = true)]
    public void setEscortedStatus(string displayName, string active)// change name to SetStatus
    {
        try
        {
            Escorted c = new Escorted();
            c.DisplayName = displayName;
            c.setEscortedStatus(active);
        }
        catch (Exception ex)
        {
            Log.Error("Error in setEscortedStatus", ex);
            throw new Exception("שגיאה בעדכון סטטוס מלווה");
        }

    }

    [WebMethod(Description = "Set Patient Status", EnableSession = true)]
    public void SetPatientStatus(string displayName, string active)
    {
        try
        {
            if (displayName.IndexOf("'") != -1)
            {
                displayName = displayName.Replace("'", "''");
            }
            Patient c = new Patient();
            c.DisplayName = displayName;
            c.SetPatientStatus(active);

            //write to log entry
            string message = "";
            if (active == "false")
            {
                message = " החולה " + displayName + " הפך ללא פעיל ";
            }
            else message = " החולה " + displayName + " הפך לפעיל ";
            LogEntry le = new LogEntry(DateTime.Now, "סטטוס חולה", message, 3);

        }
        catch (Exception ex)
        {
            Log.Error("Error in SetPatientStatus", ex);
            throw new Exception("שגיאה בעדכון סטטוס חולה");
        }
    }

    [WebMethod(EnableSession = true)]
    public void setPatient(Patient patient, string func)
    {
        try
        {
            Patient p = new Patient();
            p = patient;
            p.setPatient(func);
        }
        catch (Exception ex)
        {
            Log.Error("Error in setPatient", ex);
            throw new Exception("שגיאה בפתיחת חולה חדש");
        }

    }
    [WebMethod(EnableSession = true)]
    public void setAnonymousPatient(AnonymousPatient anonymousPatient, string func)
    {
        try
        {
            AnonymousPatient p = new AnonymousPatient();
            p = anonymousPatient;
            p.setAnonymousPatient(func);
        }
        catch (Exception ex)
        {
            Log.Error("Error in setAnonymousPatient", ex);
            throw new Exception("שגיאה בפתיחת חולה חדש");
        }

    }
    [WebMethod(EnableSession = true)]
    public void setAnonymousEscorted(string func, int patientId, int numberOfEscort)
    {
        try
        {
            Escorted escorted = new Escorted();
            escorted.setAnonymousEscorted(func, patientId, numberOfEscort);
        }
        catch (Exception ex)
        {
            Log.Error("Error in setAnonymousEscorted", ex);
            throw new Exception("שגיאה בפתיחת מלווים אנונימיים חדשים");
        }

    }
    [WebMethod(EnableSession = true)]
    public void setUserPassword(string userName, string password)
    {
        try
        {
            Volunteer v = new Volunteer();
            v.setUserPassword(userName, password);
        }
        catch (Exception ex)
        {
            Log.Error("Error in setUserPassword", ex);
            throw new Exception("שגיאה בהחלפת סיסמה");
        }

    }

    [WebMethod(EnableSession = true)]
    public void setEscorted(Escorted escorted, string func)
    {
        try
        {
            Escorted p = new Escorted();
            p = escorted;
            p.setEscorted(func);
        }
        catch (Exception ex)
        {
            Log.Error("Error in setEscorted", ex);
            throw new Exception("שגיאה בפתיחת מלווה חדש");
        }

    }
    [WebMethod(EnableSession = true)]
    public void setNewVersion(string userName, string google, string appstore, DateTime date, string version, bool mandatory)
    {
        try
        {
            //add global messege to all volunteers to download new app
            Version v = new Version();
            //add mandatory
            v.setNewVersion(userName, google, appstore, date, version, mandatory);

        }
        catch (Exception ex)
        {
            Log.Error("Error in setNewVersion", ex);
            throw new Exception("שגיאה בעדכון גרסה חדשה");
        }
    }

    [WebMethod(EnableSession = true)]
    public string getEscorted(string displayName, string patientName)
    {
        try
        {
            Escorted p = new Escorted();
            p.DisplayName = displayName;
            Escorted escorted = p.getEscorted(patientName);
            return j.Serialize(escorted);
        }
        catch (Exception ex)
        {
            Log.Error("Error in getEscorted", ex);
            throw new Exception("שגיאה בשליפת מלווים");
        }

    }

    [WebMethod(EnableSession = true)]
    public string getPatient(string displayName)
    {
        try
        {
            Patient p = new Patient();
            p.DisplayName = displayName;
            Patient patient = p.getPatient();
            return j.Serialize(patient);
        }
        catch (Exception ex)
        {
            Log.Error("Error in getPatient", ex);
            throw new Exception("שגיאה בשליפת חולה");
        }

    }
    [WebMethod(EnableSession = true)]
    public string getAnonymousPatient(string displayName)
    {
        try
        {
            AnonymousPatient p = new AnonymousPatient();
            p.DisplayName = displayName;
            AnonymousPatient patient = p.getAnonymousPatient();
            return j.Serialize(patient);
        }
        catch (Exception ex)
        {
            Log.Error("Error in getAnonymousPatient", ex);
            throw new Exception("שגיאה בשליפת חולה אנונימי");
        }

    }

    [WebMethod(EnableSession = true)]
    public string getContactType()
    {
        try
        {
            Escorted e = new Escorted();
            List<Escorted> cl = e.getContactType();
            return j.Serialize(cl);
        }
        catch (Exception ex)
        {
            Log.Error("Error in getContactType", ex);
            throw new Exception("שגיאה בשליפת קרבת מלווה");
        }

    }


    //[WebMethod(EnableSession = true)]
    //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    //public string getRidePatEscortView(string test)//Insert into same method as getRides.
    //{
    //    RidePat rp = new RidePat();
    //    //List<RidePat> r = rp.GetRidePat();
    //    List<RidePat> r = rp.GetRidePatEscortView();
    //    JavaScriptSerializer j = new JavaScriptSerializer();
    //    return j.Serialize(r);
    //}


    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string GetRidePatViewByTimeFilter(int from, int until)
    {
        try
        {
            List<RidePat> lrp = new RidePat().GetRidePatViewByTimeFilter(from, until);
            j.MaxJsonLength = Int32.MaxValue;
            return j.Serialize(lrp);
        }
        catch (Exception ex)
        {
            Log.Error("Error in GetRidePatViewByTimeFilter", ex);
            throw new Exception("שגיאה בייבוא נתונים לפי חתך זמנים");
        }
    }

    [WebMethod(EnableSession = true)]
    // [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public void ChangeArrayOF_RidePatStatuses(string newStatus, List<int> ridePatNums)
    {
        try
        {
            new RidePat().ChangeArrayOF_RidePatStatuses(newStatus, ridePatNums);
        }
        catch (Exception ex)
        {
            Log.Error("Error in ChangeArrayOF_RidePatStatuses", ex);
            throw new Exception("שגיאה במתודת שינוי סטטוס של ריד-פט");
        }
    }




    //This method is used for שבץ אותי
    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]

    public string GetRidePatView(int volunteerId, int maxDays)
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

            RidePat rp = new RidePat();
            List<RidePat> r = rp.GetRidePatView(volunteerId, maxDays);
            j.MaxJsonLength = Int32.MaxValue;
            return j.Serialize(r);
        }
        catch (Exception ex)
        {
            Log.Error("Error in GetRidePatView", ex);
            throw new Exception("שגיאה בשליפת נתוני הסעות");
        }

    }

    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string GetRidePat(int ridePatNum)
    {
        try
        {
            RidePat rp = new RidePat();
            rp = rp.GetRidePat(ridePatNum);
            return j.Serialize(rp);
        }
        catch (Exception ex)
        {
            Log.Error("Error in GetRidePat", ex);
            throw new Exception("שגיאה בשליפת נתוני הסעה");
        }

    }


    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string getMyRides(int volunteerId)
    {
        //RidePat rp = new RidePat();
        //List<RidePat> r = rp.GetRidePat();
        try
        {
            GzipMe();
            Ride r = new Ride();
            List<Ride> rl = r.GetMyFutureRides(volunteerId);
            return j.Serialize(rl);
        }
        catch (Exception ex)
        {
            Log.Error("Error in getMyRides", ex);
            throw new Exception("שגיאה בשליפת נתוני הסעות מתוכננות");
        }

    }

    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string getMyPastRides(int volunteerId)
    {
        //RidePat rp = new RidePat();
        //List<RidePat> r = rp.GetRidePat();
        try
        {
            GzipMe();
            Ride r = new Ride();
            List<Ride> rl = r.GetMyPastRides(volunteerId);
            return j.Serialize(rl);
        }
        catch (Exception ex)
        {
            Log.Error("Error in getMyRides", ex);
            throw new Exception(" שגיאה בשליפת נתוני הסעות עבר");
        }

    }

    //used for getting all versions of the app both in the appstore and in google play
    //the results order by DESC so if we want the latest version we get the first Version in the list.
    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string getVersions()
    {
        try
        {
            Version v = new Version();
            List<Version> vl = v.getVersions();
            return j.Serialize(vl);
        }
        catch (Exception ex)
        {
            Log.Error("Error in getVersions", ex);
            throw new Exception("שגיאה בשליפת נתוני גרסאות אפליקציה");
        }

    }

    //[WebMethod(EnableSession = true)]
    //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    //public string GetRidesForNotify()
    //{
    //    Ride r = new Ride();
    //    //List<RidePat> r = rp.GetRidePat();
    //    List<Ride> rl = r.GetRidesForNotify();
    //    JavaScriptSerializer j = new JavaScriptSerializer();
    //    return j.Serialize(rl);
    //}

    //[WebMethod(EnableSession = true)]
    //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    //public string DeleteRide(int RidePatId)
    //{
    //    RidePat rp = new RidePat();
    //    //List<RidePat> r = rp.GetRidePat();
    //    // int res = rp.DeleteRide(RidePatId);
    //    JavaScriptSerializer j = new JavaScriptSerializer();
    //    return j.Serialize("n");
    //}

    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string CheckUser(string mobile, string regId, string device)
    {
        try
        {
            Volunteer v = new Volunteer();
            v = v.getVolunteerByMobile(mobile, regId, device);

            User u = new User();
            string loggedInName = u.getUserNameByCellphone(mobile);

            Session["loggedInName"] = loggedInName;

            //XXX
            //throw new Exception("CU: " + (string)Session["loggedInName"]);            //HttpContext.Current.Session["loggedInName"] = mobile;
            return j.Serialize(v);
        }
        catch (Exception ex)
        {
            Log.Error("Error in CheckUser", ex);
            throw new Exception("שגיאה בבדיקת נתוני משתמש");
        }


    }

    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string CheckVolunteerByMobile(string mobile)
    {
        try
        {
            Volunteer v = new Volunteer();
            v = v.getVolunteerByMobile(mobile);

            return j.Serialize(v.DisplayName);
        }
        catch (Exception ex)
        {
            Log.Error("Error in CheckUser", ex);
            throw new Exception("שגיאה בבדיקת נתוני משתמש");
        }


    }

    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string CheckVolunteerExtendedByMobile(string mobile)
    {
        try
        {
            Volunteer v = new Volunteer();
            v = v.getVolunteerExtendedByMobile(mobile);

            return j.Serialize(v.DisplayName);
        }
        catch (Exception ex)
        {
            Log.Error("Error in CheckUser", ex);
            throw new Exception("שגיאה בבדיקת נתוני משתמש");
        }


    }

    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string SignDriver(int ridePatId, int ridePatId2, int driverId, bool primary)
    {
        try
        {
            RidePat rp = new RidePat();
            int res = rp.SignDriver(ridePatId, ridePatId2, driverId, primary);
            return j.Serialize(res);
        }
        catch (Exception ex)
        {
            Log.Error("Error in SignDriver", ex);
            if (ex.Message == "הנסיעה הזו בוטלה, תודה על הרצון לעזור.")
            {
                throw new Exception(ex.Message);
            }
            else throw new Exception("שגיאה ברישום נהג להסעה");
        }

    }


    [WebMethod(EnableSession = true, Description = "delete's driver from a ride, for example - two ridepats on the same ride")]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string DeleteDriver(int ridePatId, int driverId)
    {
        try
        {
            //Log entry inside DeleteDriver
            RidePat rp = new RidePat();
            int res = rp.DeleteDriver(ridePatId, driverId);
            if (res > 0)
            {
                Auxiliary a = new Auxiliary();
                string message = " הנהג/ת " + a.getDriverName(driverId) + " נמחק/ה מנסיעה מספר " + ridePatId.ToString();
                LogEntry le = new LogEntry(DateTime.Now, "מחיקת נהג/ת", message, 2, ridePatId, false);
                if (res == 911)
                {
                    //send push notification to coordinator phone
                    Message m = new Message();
                    //get driver details 
                    Volunteer V = new Volunteer();
                    V = V.getVolunteerByID(driverId);
                    m.driverCanceledRide(ridePatId, V);
                }
            }

            return j.Serialize(res);
        }
        catch (Exception ex)
        {
            Log.Error("Error in DeleteDriver", ex);
            throw new Exception(ex.Message);
        }

    }

    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string AssignRideToRidePatWithMobile(int ridePatId, string mobile, string fromDevice) //Get RidePatId & UserId, Create a new Ride with this info - then return RideId
    {
        Volunteer v = new Volunteer();
        v = v.getVolunteerByMobile(mobile);


        if (v.Id == 0)
            throw new Exception("user not found");
        try
        {
            Session["loggedInName"] = v.DisplayName;
            RidePat rp = new RidePat();
            int res = rp.AssignRideToRidePat(ridePatId, v.Id, "primary");
            return j.Serialize(res);
        }
        catch (Exception ex)
        {
            throw new Exception("faile to assign");
        }

    }


    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string AssignRideToRidePat(int ridePatId, int userId, string driverType) //Get RidePatId & UserId, Create a new Ride with this info - then return RideId
    {
        Volunteer v = new Volunteer();
        v = v.getVolunteerByID(userId);
        Session["loggedInName"] = v.DisplayName; //14/11/2020 Yogev&Benny was an issue with that session of ther loggedInName

        try
        {
            RidePat rp = new RidePat();
            int res = rp.AssignRideToRidePat(ridePatId, userId, driverType);
            return j.Serialize(res);
        }
        catch (Exception ex)
        {
            Log.Error("Error in AssignRideToRidePat", ex);
            if (ex.Message == "נסיעה זו בוטלה, תודה על הרצון לעזור")
            {
                throw new Exception(ex.Message);
            }

            //          else throw new Exception("שגיאה בצירוף הסעה לנסיעה");
            else throw ex;
        }

    }

    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string CombineRideRidePat(int rideId, int RidePatId) //Get RideId & RidePatId - combine them
    {
        try
        {
            RidePat rp = new RidePat();
            int res = rp.CombineRideRidePat(rideId, RidePatId);
            return j.Serialize(res);
        }
        catch (Exception ex)
        {
            Log.Error("Error in CombineRideRidePat", ex);
            throw new Exception("שגיאה במיזוג הסעה לנסיעה");
        }
    }



    [WebMethod(EnableSession = true, Description = "delete from only one ride pat")]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string LeaveRidePat(int ridePatId, int rideId, int driverId)
    {
        try
        {
            //need to send push from here!

            RidePat rp = new RidePat();
            int res = rp.LeaveRidePat(ridePatId, rideId, driverId);

            if (res > 0)
            {
                Auxiliary a = new Auxiliary();
                //write to log entry
                string message = " הנהג/ת " + a.getDriverName(driverId) + " נמחק/ה מנסיעה מספר " + ridePatId.ToString();
                LogEntry le = new LogEntry(DateTime.Now, "מחיקת נהג/ת", message, 2, ridePatId, false);
            }
            if (res == 911)
            {
                //send push notification to coordinator phone
                Message m = new Message();
                //get driver details 
                Volunteer V = new Volunteer();
                V.getVolunteerByID(driverId);
                m.driverCanceledRide(ridePatId, V.getVolunteerByID(driverId));
            }
            return j.Serialize(res);
        }
        catch (Exception ex)
        {
            Log.Error("Error in LeaveRidePat", ex);
            throw new Exception("שגיאה בעת שנהג עזב נסיעה");
        }

    }

    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string getAllStatus()
    {
        try
        {
            Status s = new Status();
            List<Status> sl = new List<Status>();
            sl = s.getAllStatus();
            return j.Serialize(sl);
        }
        catch (Exception ex)
        {
            Log.Error("Error in getAllStatus", ex);
            throw new Exception("שגיאה בשליפת סטטוסים");
        }

    }

    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string getAllEquipment()
    {
        try
        {
            Patient p = new Patient();
            List<string> el = p.getAllEquipment();
            return j.Serialize(el);
        }
        catch (Exception ex)
        {
            Log.Error("Error in getAllEquipment", ex);
            throw new Exception("שגיאה בשליפת ציוד");
        }

    }

    #region volunteers functions
    [WebMethod(EnableSession = true)]
    public void deactivateVolunteer(string displayName, string active)
    {
        try
        {
            if (displayName.IndexOf("'") != -1)
            {
                displayName = displayName.Replace("'", "''");
            }
            Volunteer v = new Volunteer();
            v.DisplayName = displayName;
            v.deactivateCustomer(active);

            //write to log
            string message = "";
            if (active == "false")
            {
                message = " המתנדב/ת " + displayName + " הפך ללא פעיל ";
            }
            else message = " המתנדב/ת " + displayName + " הפך לפעיל ";
            LogEntry le = new LogEntry(DateTime.Now, "סטטוס מתנדב", message, 4);
        }
        catch (Exception ex)
        {
            Log.Error("Error in deactivateVolunteer", ex);
            throw new Exception("שגיאה בעת עדכון סטטוס מתנדב");
        }

    }

    [WebMethod(EnableSession = true)]
    public void setVolunteer(Volunteer volunteer, string func)
    {
        try
        {
            Volunteer v = volunteer;
            v.setVolunteer(v, func);

        }
        catch (Exception ex)
        {
            Log.Error("Error in setVolunteer", ex);
            throw new Exception("שגיאה ביצירת מתנדב חדש");
        }

    }

    [WebMethod(EnableSession = true)]
    public void setVolunteerData(Volunteer volunteerExtended, string username)
    {
        try
        {
            Volunteer v = volunteerExtended;
            v.setVolunteerData(v, username);

        }
        catch (Exception ex)
        {
            Log.Error("Error in setVolunteerData", ex);
            throw new Exception("שגיאה בעריכת מתנדב");
        }

    }

    [WebMethod(EnableSession = true)]
    public void deactivateLocation(string displayName, string active)
    {
        try
        {
            if (displayName.IndexOf("'") != -1)
            {
                displayName = displayName.Replace("'", "''");
            }

            Location l = new Location();
            l.Name = displayName;

            l.deactivateLocation(active);

            //write to log
            string message = "";
            if (active == "false")
            {
                message = " המיקום " + l.Name + " הפך ללא פעיל ";
            }
            else message = " המיקום " + l.Name + " הפך לפעיל ";
            LogEntry le = new LogEntry(DateTime.Now, "סטטוס מיקום", message, 5);
        }
        catch (Exception ex)
        {
            Log.Error("Error in deactivateLocation", ex);
            throw new Exception("שגיאה בעדכון סטטוס מיקום");
        }

    }
    [WebMethod(EnableSession = true)]
    public void setLocation(Location location, string func)
    {
        try
        {
            Location l = location;
            l.setLocation(l, func);
        }
        catch (Exception ex)
        {
            Log.Error("Error in setLocation", ex);
            throw new Exception("שגיאה ביצירת  נקודת איסוף/הורדה");
        }

    }
    [WebMethod(EnableSession = true)]
    public string getLocation(string displayName)
    {
        try
        {
            Location l = new Location();
            l.Name = displayName;
            Location location = l.getLocation();
            return j.Serialize(location);
        }
        catch (Exception ex)
        {
            Log.Error("Error in getLocation", ex);
            throw new Exception("שגיאה בשליפת מיקום");
        }

    }

    [WebMethod(EnableSession = true)]
    public string getVolunteers(bool active)
    {
        try
        {
            HttpResponse response = GzipMe();

            Volunteer c = new Volunteer();
            List<Volunteer> volunteersList = c.getVolunteersList(active);
            return j.Serialize(volunteersList);
        }
        catch (Exception ex)
        {
            Log.Error("Error in getVolunteers", ex);
            throw new Exception("שגיאה בשליפת מתנדבים");
        }

    }

    [WebMethod(EnableSession = true)]
    public string getVolunteer(string displayName)
    {
        try
        {
            Volunteer v = new Volunteer();
            v.DisplayName = displayName;
            Volunteer volunteer = v.getVolunteer();
            return j.Serialize(volunteer);
        }
        catch (Exception ex)
        {
            Log.Error("Error in getVolunteer", ex);
            throw new Exception("שגיאה בשליפת מתנדב");
        }

    }

    [WebMethod(EnableSession = true)]
    // Benny changed the parameter from displayname to cellphone
    public string getVolunteerData(string cellphone) //function for data review of all volunteers
    {
        try
        {
            Volunteer v = new Volunteer();
            //v.DisplayName = displayName;
            v.CellPhone = cellphone;
            Volunteer volunteerExtended = v.getVolunteerData();

            return j.Serialize(volunteerExtended);
        }
        catch (Exception ex)
        {
            Log.Error("Error in getVolunteer", ex);
            throw new Exception("שגיאה בשליפת מתנדב");
        }

    }

    [WebMethod(EnableSession = true)]
    public string getVolunteerTypes()
    {
        try
        {
            Volunteer v = new Volunteer();

            List<string> types = v.getVolunteerTypes();
            return j.Serialize(types);
        }
        catch (Exception ex)
        {
            Log.Error("Error in getVolunteerTypes", ex);
            throw new Exception("שגיאה בשליפת סוגי מתנדב");
        }
    }
    #endregion

    #region Destinations functions
    [WebMethod(EnableSession = true)]
    public string getDestinationView(bool active)
    {
        try
        {
            Location d = new Location();
            List<Location> destinationsList = d.getDestinationsListForView(active);
            return j.Serialize(destinationsList);

        }
        catch (Exception ex)
        {
            Log.Error("Error in getDestinationView", ex);
            throw new Exception("שגיאה בשליפת נקודות איסוף והורדה");
        }
    }

    [WebMethod(EnableSession = true)]
    public string getHospitalView(bool active)
    {
        try
        {
            Location d = new Location();
            List<Location> hospitalList = d.getHospitalListForView(active);
            return j.Serialize(hospitalList);
        }
        catch (Exception ex)
        {
            Log.Error("Error in getHospitalView", ex);
            throw new Exception("שגיאה בשליפת נקודות איסוף והורדה");
        }

    }

    [WebMethod(EnableSession = true)]
    public string getBarrierView(bool active)
    {
        try
        {
            Location d = new Location();
            List<Location> hospitalList = d.getBarrierListForView(active);
            return j.Serialize(hospitalList);

        }
        catch (Exception ex)
        {
            Log.Error("Error in getBarrierView", ex);
            throw new Exception("שגיאה בשליפת מחסומים");
        }
    }

    [WebMethod(EnableSession = true)]
    public string confirmPush(int userId, int msgId, string status)
    {
        Message m = new Message();
        string sender = (string)HttpContext.Current.Session["loggedInName"];
        m.insertMsg(msgId, "", status, "", 0, DateTime.Now, userId, "", true, false, false, sender);

        return j.Serialize("ok");
    }
    [WebMethod(EnableSession = true)]
    public string pushAssistant(int ridepat, string cellphone, string msg)
    {

        Message m = new Message();
        m.pushFromAssistant(ridepat, cellphone, msg);
        //Email e = new Email();
        //e.sendMessage("Assistant change", "עוזר", cellphone, msg);
        return j.Serialize("ok");
    }
    #endregion

    #region login functions
    [WebMethod(EnableSession = true)]
    public string loginUser(string uName, string password)
    {
        bool userInDB;
        try
        {

            User u = new User(uName, password);
            userInDB = u.CheckLoginDetails();


            string loggedInName = u.getUserNameByCellphone(uName);

            Session["loggedInName"] = loggedInName;

            string loggedInCoord = (string)Session["loggedInName"];

        }
        catch (Exception ex)
        {

            throw ex;
        }
        HttpContext.Current.Session["userSession"] = uName;


        if (userInDB)
        {
            writeToLog("Successful login");
        }
        else
        {
            writeToLog("Login failed. password: " + password);
        }


        return j.Serialize(userInDB);
    }

    public void writeToLog(string str)
    {
        string user = (string)HttpContext.Current.Session["userSession"];
        Log.Error(str + " ;for user: " + user);
    }
    [WebMethod(EnableSession = true)]
    public string GetUserNameByCellphone(string uName)
    {
        string userInDB;
        try
        {

            User u = new User();
            userInDB = u.getUserNameByCellphone(uName);
        }
        catch (Exception ex)
        {

            throw ex;
        }
        return j.Serialize(userInDB);
    }
    [WebMethod(EnableSession = true)]
    public string GetUserEnglishNameByCellphone(string uName)
    {
        string userInDB;
        try
        {

            User u = new User();
            userInDB = u.getUserEnglishNameByCellphone(uName);
        }
        catch (Exception ex)
        {

            throw;
        }
        return j.Serialize(userInDB);
    }
    [WebMethod(EnableSession = true)]
    public string GetIsAssistantByCellphone(string uName)
    {
        bool userInDB;
        try
        {

            User u = new User();
            userInDB = u.GetIsAssistantByCellphone(uName);
        }
        catch (Exception ex)
        {

            throw;
        }
        return j.Serialize(userInDB);
    }
    [WebMethod(EnableSession = true)]
    public string GetCoordinatorsList()
    {
        List<Volunteer> coors;
        try
        {
            Volunteer v = new Volunteer();
            coors = v.getCoordinatorsList();

        }
        catch (Exception ex)
        {

            throw;
        }
        return j.Serialize(coors);
    }
    [WebMethod(EnableSession = true)]
    public void writeLog(string str)
    {
        for (int i = 0; i < 50; i++)
        {
            LogEntry le = new LogEntry(DateTime.Now, str, "error in something" + i.ToString(), i);


        }
    }

    [WebMethod(EnableSession = true)]
    public string getLog(int hours)
    {
        LogEntry le = new LogEntry();
        List<LogEntry> list = le.GetLog(hours);
        return j.Serialize(list);
    }

    //[WebMethod(EnableSession = true)]
    //public string getUserType(string user)
    //{
    //    JavaScriptSerializer j = new JavaScriptSerializer();
    //    User u = new User();
    //    u.UserName = user;
    //    u.UserType = u.getUserType(u.UserName);
    //    return j.Serialize(u.UserType);
    //}




    [WebMethod(EnableSession = true)]
    public string loginDriver(string uName, string password)
    {
        Drivers d = new Drivers(uName, password);
        d = d.CheckLoginDetails();
        return j.Serialize(d);
    }
    #endregion

    [WebMethod(EnableSession = true)]
    public string getCities()
    {
        try
        {
            City c = new City();
            List<City> citiesList = c.getCitiesList();
            return j.Serialize(citiesList);
        }
        catch (Exception ex)
        {
            Log.Error("Error in getCities", ex);
            throw new Exception("שגיאה בשליפת ערים");
        }

    }

    [WebMethod(EnableSession = true)]
    public int backupToPrimaryNotification(int ridePatId)
    {
        Message m = new Message();
        return m.backupToPrimary(ridePatId);
    }

    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string getR2RServers()
    {
        try
        {
            Auxiliary a = new Auxiliary();
            //a.getR2RServers()
            return j.Serialize(a.getR2RServers());
        }
        catch (Exception ex)
        {
            Log.Error("Error in getServers", ex);
            throw new Exception("שגיאה בשליפת שרתים");
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
    public string isProductionDatabase()
    {
        Auxiliary aux = new Auxiliary();
        bool ans = aux.isProductionDatabase();
        return j.Serialize(ans);
    }

    [WebMethod(EnableSession = true)]
    public string getMessages(string displayName)
    {
        try
        {
            Message m = new Message();
            List<Message> messages = m.getMessages(displayName);
            return j.Serialize(messages);
        }
        catch (Exception ex)
        {
            Log.Error("Error in getMessages", ex);
            throw new Exception("שגיאה בשליפת הודעות");
        }

    }

    [WebMethod(EnableSession = true)]
    public string getVolunteerDataTable()
    {
        try
        {
            HttpResponse response = GzipMe();
            Volunteer v = new Volunteer();
            List<Volunteer> VolunteersList = v.getVolunteerDataTable();
            return j.Serialize(VolunteersList);
        }
        catch (Exception ex)
        {
            Log.Error("Error in getVolunteerDataTable", ex);
            throw new Exception("שגיאה בשליפת טבלת מתנדבים");
        }

    }

    [WebMethod(EnableSession = true)]
    //public void setVolunteerYuval(Volunteer volunteer, string coorEmail, string coorName, string coorPhone, string instructions)
    public void setVolunteerYuval(Volunteer volunteer, List<Volunteer> coordinators, string instructions)
    {
        try
        {
            Volunteer v = volunteer;
            //v.setVolunteerYuval(v, coorEmail, coorName, coorPhone, instructions);
            v.setVolunteerYuval(v, coordinators, instructions);

        }
        catch (Exception ex)
        {
            Log.Error("Error in setVolunteerYuval", ex);
            if (ex.Message == "duplicate key")
            {
                throw new Exception("duplicate key");
            }
            else throw new Exception("שגיאה ביצירת מתנדב חדש");
        }

    }

    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string WelcomePage(string VolunteerMobile, List<string> CoordinatorMobiles) //For multiple coordinators
    {
        try
        {
            User u = new User();
            string VolunteerName = u.getUserNameByCellphone(VolunteerMobile);
            string CIOName = ConfigurationManager.AppSettings["CIOName"];
            string CIOPhone = ConfigurationManager.AppSettings["CIOPhone"];
            List<string> names = new List<string>();
            names.Add(VolunteerName);
            names.Add(CIOName);
            names.Add(CIOPhone);
            foreach (string item in CoordinatorMobiles)
            {
                string CoordinatorName = u.getUserNameByCellphone(item);
                names.Add(CoordinatorName);
            };



            return j.Serialize(names);
        }
        catch (Exception ex)
        {
            Log.Error("Error in WelcomePage method", ex);
            throw new Exception("שגיאה בבדיקת נתוני משתמש");
        }


    }

    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string WelcomePage2(string VolunteerMobile, string CoordinatorMobile) //For one coordinator
    {
        try
        {
            User u = new User();
            string VolunteerName = u.getUserNameByCellphone(VolunteerMobile);
            string CIOName = ConfigurationManager.AppSettings["CIOName"];
            string CIOPhone = ConfigurationManager.AppSettings["CIOPhone"];
            List<string> names = new List<string>();
            names.Add(VolunteerName);
            names.Add(CIOName);
            names.Add(CIOPhone);
            string CoordinatorName;
            if (CoordinatorMobile == "NoCoor")
            {
                CoordinatorName = "NoCoor";
            }
            else
            {
                CoordinatorName = u.getUserNameByCellphone(CoordinatorMobile);
            }
            names.Add(CoordinatorName);
            return j.Serialize(names);
        }
        catch (Exception ex)
        {
            Log.Error("Error in WelcomePage method", ex);
            throw new Exception("שגיאה בבדיקת נתוני משתמש");
        }


    }

    [WebMethod(EnableSession = true)]
    public string getRoles()
    {
        try
        {
            Role c = new Role();
            List<Role> rolesList = c.getRolesList();
            return j.Serialize(rolesList);
        }
        catch (Exception ex)
        {
            Log.Error("Error in getRoles", ex);
            throw new Exception("שגיאה בשליפת תפקידים");
        }

    }

    [WebMethod(EnableSession = true)]
    public bool CheckRidePat(RidePat RidePatBack)
    {
        /*OLD VERSION*/

        //RidePat r = new RidePat();

        //return r.CheckRidePat(RidePatBack, false);
        ////return j.Serialize(d);

        /*NEW VERSION 06/11/2020 YOGEV*/
        RidePat ridePatView = new RidePat().CheckRidePat_V2(RidePatBack, false);
        if (ridePatView.RidePatNum == 0)
        {
            return false;                            // case there is no return drive as such at all
        }
        else if (ridePatView.Status == "נמחקה")
        {
            return false;                           // case there is no return drive (there is one which MARKED deleted)
        }
        return true;                                // case there is return drive

    }

    [WebMethod(EnableSession = true)]
    public string getReturnRidePat(RidePat RidePat)
    {
        RidePat r = new RidePat();

        r = r.getReturnRidePat(RidePat, false);
        return j.Serialize(r);
    }

    [WebMethod(EnableSession = true)]
    public string changeRidePatStatus(string newStatus, string ridePatNum)
    {
        try
        {
            RidePat rp = new RidePat();

            return j.Serialize(rp.changeRidePatStatus(newStatus, ridePatNum));
        }
        catch (Exception ex)
        {
            Log.Error("Error in changeRidePatStatus", ex);
            throw new Exception("שגיאה בשינוי סטאטוס להסעה");
        }

    }

    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string AssignMultiRideToRidePat(int ridePatId, int userId, string driverType, int numberOfRides, string repeatRide) //Get RidePatId & UserId, Create a new Ride with this info - then return RideId
    {
        Volunteer v = new Volunteer();
        v = v.getVolunteerByID(userId);
        Session["loggedInName"] = v.DisplayName;

        try
        {
            int firstRide = ridePatId - numberOfRides + 1;
            RidePat rp = new RidePat();
            int res = rp.AssignMultiRideToRidePat(firstRide, userId, driverType, numberOfRides, repeatRide);
            return j.Serialize(res);
        }
        catch (Exception ex)
        {
            Log.Error("Error in AssignMultiRideToRidePat", ex);
            if (ex.Message == "נסיעה זו בוטלה, תודה על הרצון לעזור")
            {
                throw new Exception(ex.Message);
            }

            //          else throw new Exception("שגיאה בצירוף הסעה לנסיעה");
            else throw ex;
        }

    }
}

