﻿using System;
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
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json.Linq;

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
    public double CpuPrefTesting(int rounds)
    {

        Random r = new Random();
        double sum = 0;
        for (int i = 0; i < rounds; i++)
        {
            double num = r.Next(1, 2);
            sum += Math.Sin(num);
        }

        return sum;
    }


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
            List<Area> areas = new List<Area>();
            Location l = new Location();
            areas = l.getAreasAsClass();
            return j.Serialize(areas);
        }
        catch (Exception e)
        {
            Log.Error("Error in getAreas", e);
            throw new Exception("שגיאה בשליפת אזורים");
        }
    }

    [WebMethod(EnableSession = true)]
    public string GetRegions()
    {
        List<Region> regions;
        Region regionManager = new Region();
        try
        {
            regions = regionManager.GetAllRegions();
            return j.Serialize(regions);
        }
        catch (Exception e)
        {
            Log.Error("Error in GetRegions", e);
            throw new Exception("שגיאה בשליפת תתי אזורים");
        }
    }
    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string getAreas_AsLocationObj()
    {
        try
        {
            List<Location> areas = new Location().getAreas_AsLocationObj(); ;
            return j.Serialize(areas);
        }
        catch (Exception e)
        {
            Log.Error("Error in getAreas_AsLocationObj", e);
            throw new Exception("שגיאה בשליפת אזורים כאובייקט מיקום");
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
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string GetVolunteersDocumentedRides(int volunteerId)
    {
        try
        {
            List<RidePat> ridesRecords = new RidePat().GetVolunteersDocumentedRides(volunteerId);
            return j.Serialize(ridesRecords);
        }
        catch (Exception ex)
        {
            Log.Error("Error in GetVolunteersDocumentedRides", ex);
            throw new Exception("שגיאה בהבאת היסטוריית הסעות של מתנדב");
        }
    }


    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string GetVolunteersDocumentedUnityRides(int volunteerId)
    {
        try
        {
            UnityRide ur = new UnityRide();
            return j.Serialize(ur.GetUnityRidesByVolunteerId(volunteerId));
        }
        catch (Exception ex)
        {
            Log.Error("Error in GetVolunteersDocumentedUnityRides", ex);
            throw new Exception("שגיאה בהבאת היסטוריית הסעות של מתנדב");
        }
    }



    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string CheckFutureRides(int volunteerId)
    {
        try
        {
            Volunteer v = new Volunteer();
            return j.Serialize(v.hasFutureRides(volunteerId));
        }
        catch (Exception ex)
        {
            Log.Error("Error in CheckFutureRides", ex);
            throw new Exception("שגיאה בבדיקה נסיעות עתידיות");
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

//    {
//    "ridePatNum": 183192,
//    "numOfCandidates": 25,
//    "newFlag": false,
//    "dayInWeek": 3
//}

//Benny Candidates
[WebMethod(EnableSession = true)]
    public string GetCandidates(int ridePatNum, int numOfCandidates, bool newFlag, int dayInWeek)
    {

        try
        {
            CandidatesLogic cl = new CandidatesLogic();
            if (newFlag)
                return j.Serialize(cl.GetNewbiesCandidates(ridePatNum, numOfCandidates));
            else
                return j.Serialize(cl.GetCandidates(ridePatNum, numOfCandidates, dayInWeek));
        }
        catch (Exception ex)
        {
            Log.Error("Error in getCandidates", ex);
            throw new Exception("שגיאה בקבלת מועמדים לנסיעות: " + ex.Message);

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


    //Gilad try to set UnityRide 
    [WebMethod(EnableSession = true)]
    public int setUnityRide(UnityRide unityRide,string func,int numOfRide,string repeatEvery,bool firstTry)
    {
        try
        {
            int res = unityRide.SetUnityRide(unityRide,func,numOfRide,repeatEvery, firstTry);
            return res; 
        }
        catch (Exception ex)
        {

            throw ex;
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
            throw new Exception("שגיאה בפתיחה/עדכון/מחיקה של הסעה חדשה " + ex.Message);

        }
    }

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
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string getPatients_Gilad(string active = "true")
    {
        bool activeBool = Convert.ToBoolean(active);
        try
        {
            HttpResponse response = GzipMe();

            Patient p = new Patient();
            List<Patient> patientsList = p.GetPatientsList_Gilad(activeBool);
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

    [WebMethod(EnableSession = true)]
    public string GetPatients_slim(bool isActive)
    {
        try
        {
            HttpResponse response = GzipMe();
            Patient p = new Patient();
            List<Patient> patients = p.GetPatients_slim(isActive);
            j.MaxJsonLength = Int32.MaxValue;
            return j.Serialize(patients);
        }
        catch (Exception ex)
        {
            Log.Error("Error in GetPatients_slim", ex);
            throw ex;
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
    public string GetVolunteerById(int id)
    {
        try
        {
            Volunteer v = new Volunteer().getVolunteerByID(id);
            return j.Serialize(v);
        }
        catch (Exception ex)
        {
            Log.Error("Error in GetVolunteerById", ex);
            throw new Exception("שגיאה בשליפת נתוני מתנדב");

        }
    }

    [WebMethod(EnableSession = true)]
    public string GetVolunteerByMobile(string cellphone)
    {
        try
        {
            Volunteer volunteer = new Volunteer();
            volunteer = volunteer.GetVolunteerByMobile(cellphone);

            return j.Serialize(volunteer);
        }
        catch (Exception ex)
        {

            Log.Error("Error in GetVolunteerByCellphone", ex);
            throw ex;
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
    public void setEscortedStatus(int escortId, string active)// change name to SetStatus
    {
        try
        {
            Escorted c = new Escorted();
            c.Id = escortId;
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
            Log.Error("Error in setPatient XXXXXXX", ex);
            throw ex;
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
            //Escorted p = new Escorted();
            // p = escorted;
            escorted.setEscorted(func);
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
    public string GetEscortById(int id)
    {
        try
        {
            Escorted escort = new Escorted().GetEscortById(id);
            return j.Serialize(escort);
        }
        catch (Exception ex)
        {
            Log.Error("Error in GetEscortById", ex);
            throw new Exception("שגיאה בשליפת מלווה לפי מזהה");
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
    public string GetPatientById(int id)
    {
        try
        {
            Patient patient = new Patient().GetPatientById(id);
            return j.Serialize(patient);
        }
        catch (Exception ex)
        {
            Log.Error("Error in getPatientById", ex);
            throw new Exception("שגיאה בשליפת חולה לפי מזהה");
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
    public string CheckRideBeforePost(int volunteerId,DateTime start,DateTime end)
    {
        try
        {
            int res = Absence.checkRidesBeforePostAbsence(volunteerId, start, end);
            j.MaxJsonLength = Int32.MaxValue;
            return j.Serialize(res);
        }
        catch (Exception ex)
        {
            Log.Error("Error in CheckRideBeforePost ", ex);
            throw new Exception("שגיאה בבדיקת הסעות מול היעדרות");
        }
    }




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
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string GetRidePatViewByTimeFilter_Gilad(int from, int until, bool isDeletedtoShow)
    {
        //Gilad Update this with Data Reader only 

        try
        {
            HttpResponse response = GzipMe();
            List<RidePat> lrp = new RidePat().GetRifePatViewByTimeFilter_DR_Gilad(from, until, isDeletedtoShow);
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
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string Get_unityRide_ByTimeRange(int from, int until, bool isDeletedtoShow)
    {

        try
        {
            HttpResponse response = GzipMe();
            UnityRide ur = new UnityRide();
            List<UnityRide> list = new List<UnityRide>();
            list = ur.Get_unityRide_ByTimeRange(from, until, isDeletedtoShow);
            j.MaxJsonLength = Int32.MaxValue;
            return j.Serialize(list);
        }
        catch (Exception ex)
        {
            Log.Error("Error in Get_unityRide_ByTimeRange", ex);
            throw new Exception("שגיאה בייבוא נתונים לפי חתך זמנים");
        }
    }




    [WebMethod(EnableSession = true)]
    // [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public void ChangeArrayOF_RidePatStatuses(string newStatus, List<int> ridePatNums, DateTime clientUTCTimeStemp)
    {
        try
        {
            new RidePat().ChangeArrayOF_RidePatStatuses(newStatus, ridePatNums, clientUTCTimeStemp);
        }
        catch (Exception ex)
        {
            Log.Error("Error in ChangeArrayOF_RidePatStatuses", ex);
            throw new Exception("שגיאה במתודת שינוי סטטוס של ריד-פט");
        }
    }

    [WebMethod(EnableSession = true)]
    public bool recoverUnityRides(List<int> listIDs)
    {
        try
        {
            UnityRide ur = new UnityRide();
            return ur.recoverUnityRides(listIDs);
        }
        catch (Exception ex)
        {

            throw new Exception("error in recoverUnityRides API" +ex);
        }

    }

    //Gilad update 03/10/23
    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string GetUnityRide(int days)
    {
        try
        {
            HttpResponse response = GzipMe();
            UnityRide unityRide = new UnityRide();
            j.MaxJsonLength = Int32.MaxValue;
            return j.Serialize(unityRide.GetUnityRideView(days));


        }
        catch (Exception ex)
        {
            CatchErrors catchErrors = new CatchErrors("WebService: Exception in GetUnityRide", ex + " " + ex.Message + " " + ex.InnerException + " " + ex.Source, ex.StackTrace);
            Log.Error("Error in GetUnityRide", ex);
            throw new Exception("שגיאה בשליפת נתוני הסעות");
        }
    }

    //Gilad update 06/12/23 use for edit one ride
    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string GetUnityRideToEdit(int UnityRideId)
    {
        try
        {
            HttpResponse response = GzipMe();
            UnityRide unityRide = new UnityRide();
            j.MaxJsonLength = Int32.MaxValue;
            return j.Serialize(unityRide.GetUnityRide(UnityRideId));


        }
        catch (Exception ex)
        {
            CatchErrors catchErrors = new CatchErrors("WebService: Exception in GetUnityRide", ex + " " + ex.Message + " " + ex.InnerException + " " + ex.Source, ex.StackTrace);
            Log.Error("Error in GetUnityRide", ex);
            throw new Exception("שגיאה בשליפת נתוני הסעות");
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
            CatchErrors catchErrors = new CatchErrors("WebService: Exception in GetRidePatView", ex + " " + ex.Message + " " + ex.InnerException + " " + ex.Source, ex.StackTrace);
            Log.Error("Error in GetRidePatView", ex);
            throw new Exception("שגיאה בשליפת נתוני הסעות");
        }

    }

    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string GetWeeklyThanksforVolunteers()
    {
        try
        {
            HttpResponse response = GzipMe();
            UnityRide ur = new UnityRide();
            List <UnityRide> List = ur.GetWeeklyRidesForThanks();
            j.MaxJsonLength = Int32.MaxValue;
            return j.Serialize(List);


        }
        catch (Exception ex)
        {

            Log.Error("Error in GetWeeklyThanksforVolunteers", ex);
            throw new Exception("שגיאה בשליפת נתוני הסעות לתודות");
        }
    }
    //This method is used for שבץ אותי
    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string Get_Tomorrow_RidePatView_Gilad()
    {
        try
        {

            DBservice_Gilad db = new DBservice_Gilad();
            List<object> r = db.GetTomorrowRides();
            j.MaxJsonLength = Int32.MaxValue;
            return j.Serialize(r);
        }
        catch (Exception ex)
        {
            CatchErrors catchErrors = new CatchErrors("WebService: Exception in Get_Tomorrow_RidePatView_Gilad", ex + " " + ex.Message + " " + ex.InnerException + " " + ex.Source, ex.StackTrace);
            Log.Error("Error in GetRidePatView", ex);
            throw new Exception("שגיאה בשליפת נתוני הסעות של הבוקר Get_Tomorrow_RidePatView_Gilad");
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
    public string GetUnityRide_RidePat(int ridePatNum)
    {
        try
        {
            UnityRide ur = new UnityRide();
            return j.Serialize(ur.getUnityRideAsRP(ridePatNum));
        }
        catch (Exception ex)
        {
            Log.Error("Error in GetUnityRide_RidePat", ex);
            throw new Exception("שגיאה בשליפת נתוני הסעה");
        }

    }



    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string GetReturnRideUnityRide(int unityRideID)
    {
        try
        {
            UnityRide ur = new UnityRide();
            return j.Serialize(ur.GetReturnDrive(unityRideID));
        }
        catch (Exception ex)
        {
            Log.Error("Error in GetReturnRideUnityRide ", ex);
            throw new Exception("שגיאה בשליפת נתוני הסעה: GetReturnRideUnityRide");
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

    [WebMethod(EnableSession = true)]
    public void UpdateDriver(int rideNum, int ridePatId, int newDriverId, int assignedFromAppId)
    {
        try
        {
            Ride r = new Ride();
            r.UpdateDriver(rideNum, ridePatId, newDriverId, assignedFromAppId);
        }
        catch (Exception ex)
        {
            Log.Error("Error in UpdateDriver", ex);
            throw new Exception(":שגיאה בעדכון נהג להסעה זו " + rideNum);
        }
    }


    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string GetDriverName(int rideId)
    {
        try
        {
            return new Ride().GetDriverName(rideId);
        }
        catch (Exception ex)
        {
            Log.Error("Error in GetDriverName", ex);
            throw new Exception(" שגיאה בשליפת שם נהג מהסעה");
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
            v = v.GetVolunteerByMobile(mobile);

            return j.Serialize(v.DisplayName);
        }
        catch (Exception ex)
        {
            Log.Error("Error in CheckVolunteerByMobile", ex);
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
            Log.Error("Error in CheckVolunteerExtendedByMobile", ex);
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
    public string AssignRideToRidePatWithMobile(int ridePatId, string mobile, int assignedFromAppId) //Get RidePatId & UserId, Create a new Ride with this info - then return RideId
    {
        Volunteer v = new Volunteer();
        v = v.GetVolunteerByMobile(mobile);


        if (v.Id == 0)
            throw new Exception("user not found");
        try
        {
            Session["loggedInName"] = v.DisplayName;
            RidePat rp = new RidePat();
            int res = rp.AssignRideToRidePat(ridePatId, v.Id, "primary", assignedFromAppId);
            return j.Serialize(res);
        }
        catch (Exception ex)
        {
            throw new Exception("fail to assign");
        }

    }

    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string AssignRideToUnityRideWithMobile(int ridePatId, int userId, string driverType) //Get RidePatId & UserId, Create a new Ride with this info - then return RideId
    {
        try
        {
            UnityRide ur = new UnityRide();
            int res = ur.assignDriverMobile(ridePatId, userId);
            return j.Serialize(res);
        }
        catch (Exception ex)
        {
            throw new Exception("fail to assign");
        }

    }

    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string AssignRideToRidePat(int ridePatId, int userId, string driverType, int assignedFromAppId) //Get RidePatId & UserId, Create a new Ride with this info - then return RideId
    {
        Volunteer v = new Volunteer();
        v = v.getVolunteerByID(userId);
        Session["loggedInName"] = v.DisplayName; //14/11/2020 Yogev&Benny was an issue with that session of ther loggedInName

        try
        {
            RidePat rp = new RidePat();
            int res = rp.AssignRideToRidePat(ridePatId, userId, driverType, assignedFromAppId);
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
    public string AssignUpdateDriverToUnityRide(int UnityRideId, int DriverId,bool isDelete) //Get RidePatId & UserId, Create a new Ride with this info - then return RideId
    {
        

        try
        {
            UnityRide ur = new UnityRide();
            int res = ur.updateDriver(DriverId, UnityRideId, isDelete);
            return j.Serialize(res);

        }
        catch (Exception ex)
        {
            Log.Error("Error in AssignUpdateDriverToUnityRide", ex);
            throw new Exception(ex.Message);

        }

    }





    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public void deleteUnityRide(List<int>ListIDs) 
    {


        try
        {
            UnityRide ur = new UnityRide();
            ur.deleteUnityRide(ListIDs);

        }
        catch (Exception ex)
        {
            Log.Error("Error in deleteUnityRide", ex);
            throw new Exception(ex.Message);

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
                V = V.getVolunteerByID(driverId);
                m.driverCanceledRide(ridePatId, V);
            }
            return j.Serialize(res);
        }
        catch (Exception ex)
        {
            Log.Error("Error in LeaveRidePat", ex);
            throw new Exception("שגיאה בעת שנהג עזב נסיעה");
        }

    }


    [WebMethod(EnableSession = true, Description = "delete from only one unityRide")]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string LeaveUnityRide(int ridePatId, int rideId, int driverId)
    {
        try
        {
            //need to send push from here!

            //RidePat rp = new RidePat();
            //int res = rp.LeaveRidePat(ridePatId, rideId, driverId);
            UnityRide ur = new UnityRide();
            int res = ur.leaveUnityRideFromMobile(ridePatId, driverId);
            return j.Serialize(res);
        }
        catch (Exception ex)
        {
            Log.Error("Error in LeaveUnityRide", ex);
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
    public string SetVolunteerIsActive(string displayName, string active)
    {
        try
        {
            Volunteer v = new Volunteer();
            IsSuccessAndReason result = v.SetVolunteerIsActive(displayName, active == "true");

            return j.Serialize(result);

        }
        catch (Exception ex)
        {
            Log.Error("Error in SetVolunteerIsActive", ex);
            throw new Exception(" שגיאה בעת עדכון סטטוס מתנדב" + ex.Message);
        }
    }

    [WebMethod(EnableSession = true)]
    public void deactivateVolunteer(string displayName, string active)
    {
        try
        {
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
            throw new Exception("שגיאה ביצירת/עריכת מתנדב " + ex.Message);
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
            List<Volunteer> volunteersList = c.getVolunteersList_V2_WebOnly(active);
            return j.Serialize(volunteersList);
        }
        catch (Exception ex)
        {
            Log.Error("Error in getVolunteers", ex);
            throw new Exception("שגיאה בשליפת מתנדבים");
        }

    }


    [WebMethod(EnableSession = true)]
    public string getVolunteers_Gilad(bool active)
    {
        try
        {
            HttpResponse response = GzipMe();

            Volunteer c = new Volunteer();
            List<Volunteer> volunteersList = c.getVolunteersList_V2_WebOnly_Gilad(active);
            return j.Serialize(volunteersList);
        }
        catch (Exception ex)
        {
            Log.Error("Error in getVolunteers", ex);
            throw new Exception("שגיאה בשליפת מתנדבים אחרי עדכון של גלעד");
        }

    }

    //Gilad Touch here
    [WebMethod(EnableSession=true)]
    public string GetAbsenceByVolunteerId(int volunteerId)
    {
        try
        {
            Absence absence = new Absence();
            return j.Serialize(absence.GetAbsenceByVolunteerId(volunteerId));

        }
        catch (Exception ex)
        {
            Log.Error("Error in GetAbsenceByVolunteerId", ex);
            throw new Exception("שגיאה בשליפת היעדרויות"+ex.Message);
        }
    }
    [WebMethod(EnableSession = true)]
    public string UpdateAbsenceById(int AbsenceId, int coorId, DateTime from, DateTime until, string cause, string note)
    {
        try
        {
            Absence absence = new Absence();
            return j.Serialize( absence.UpdateAbsenceById(AbsenceId, coorId, from, until, cause, note));

        }
        catch (Exception ex)
        {
            Log.Error("Error in UpdateAbsenceById", ex);
            throw new Exception("שגיאה בעדכון היעדרות" + ex.Message);
        }
    }

    [WebMethod(EnableSession = true)]
    public string DeleteAbsenceById(int AbsenceId)
    {
        try
        {
            Absence absence = new Absence();
            return j.Serialize(absence.DeleteAbsenceById(AbsenceId));

        }
        catch (Exception ex)
        {
            Log.Error("Error in DeleteAbsenceById", ex);
            throw new Exception("שגיאה במחיקת היעדרות" + ex.Message);
        }
    }

    [WebMethod(EnableSession = true)]
    public string InsertNewAbsence(int volunteerId, int coorId, DateTime from, DateTime until, string cause, string note)
    {
        try
        {
            Absence absence = new Absence();
            return j.Serialize(absence.InsertNewAbsence(volunteerId,  coorId,  from,  until,  cause,  note));

        }
        catch (Exception ex)
        {
            Log.Error("Error in InsertNewAbsence", ex);
            throw new Exception("שגיאה בהוספת היעדרות" + ex.Message);
        }
    }

    [WebMethod(EnableSession = true)]
    public string GetDrivers(bool isActive, bool isDriving)
    {
        try
        {
            HttpResponse response = GzipMe();
            List<Volunteer> drivers = new Volunteer().GetDrivers(isActive, isDriving);
            return j.Serialize(drivers);
        }
        catch (Exception ex)
        {
            Log.Error("Error in getDrivers", ex);
            throw new Exception(" שגיאה בשליפת נהגים " + ex.Message);
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
            HttpResponse response = GzipMe();

            Location d = new Location();
            List<Location> destinationsList = d.getDestinationsListForView(active);
            //j.MaxJsonLength = int.MaxValue;
            j.MaxJsonLength = Int32.MaxValue;
            return j.Serialize(destinationsList);
        }
        catch (Exception ex)
        {
            Log.Error("Error in getDestinationView", ex);
            throw new Exception("שגיאה בשליפת נקודות איסוף והורדה " + ex.Message);
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
    public string getCoordinatorsList_version_02()
    {
        List<Volunteer> coordinators;
        try
        {
            coordinators = new Volunteer().getCoordinatorsList_version_02();
        }
        catch (Exception ex)
        {
            Log.Error("Error in getCoordinatorsList_version_02", ex);
            throw new Exception("Error in getCoordinatorsList_version_02");
        }
        return j.Serialize(coordinators);
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
    public string getUnmappedCities()
    {
        try
        {
            City c = new City();
            List<City> citiesList = c.getUnmappedCitiesList();
            return j.Serialize(citiesList);
        }
        catch (Exception ex)
        {
            Log.Error("Error in getUnmappedCitiesList", ex);
            throw new Exception("שגיאה בשליפת ערים getUnmappedCitiesList");
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
    public void setVolunteerYuval(Volunteer volunteer, List<Volunteer> coordinators, string instructions)
    {
        try
        {
            Volunteer v = volunteer;
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
            Log.Error("Error in WelcomePage2 method", ex);
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
    public string AssignMultiRideToRidePat(int ridePatId, int userId, string driverType, int numberOfRides, int assignedFromAppId) //Get RidePatId & UserId, Create a new Ride with this info - then return RideId
    {
        Volunteer v = new Volunteer();
        v = v.getVolunteerByID(userId);
        Session["loggedInName"] = v.DisplayName;

        try
        {
            int firstRide = ridePatId - numberOfRides + 1;
            RidePat rp = new RidePat();
            int res = rp.AssignMultiRideToRidePat(firstRide, userId, driverType, numberOfRides, assignedFromAppId);
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

    [WebMethod(EnableSession = true)]
    public void UpdateUnityRideTime(int unityRideId, DateTime pickupTime)
    {
        try
        {
            UnityRide unityRide = new UnityRide();
            unityRide.updateUnityRideTime(unityRideId, pickupTime);
        }
        catch (Exception ex)
        {
            Log.Error("Error in UpdateUnityRideTime", ex);
            throw new Exception(ex.Message);
        }
    }




    [WebMethod(EnableSession = true)]
    public void UpdateRidePatTime(int ridePatId, DateTime dateTime)
    {
        try
        {
            RidePat rp = new RidePat();
            rp.UpdateRidePatTime(ridePatId, dateTime);
        }
        catch (Exception ex)
        {
            Log.Error("Error in UpdateRidePatTime", ex);
            throw new Exception(ex.Message);
        }
    }


    #region DocumentedCall Module

    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    [WebMethod(EnableSession = true)]
    public string GetDocumentedCallsByDriverId(int driverId)
    {
        try
        {
            List<DocumentedCall> documentedCalls = new DocumentedCall().GetDocumentedCallsByDriverId(driverId);
            return j.Serialize(documentedCalls);
        }
        catch (Exception ex)
        {
            Log.Error("Error in GetDocumentedCallsByDriverId", ex);
            throw new Exception("Error in GetDocumentedCallsByDriverId ---> ex.Message: " + ex.Message);
        }

    }

    [WebMethod(EnableSession = true)]
    public bool ChangeDocumentedCallStatus(int callId, string newValue)
    {
        try
        {
            return new DocumentedCall().ChangeDocumentedCallStatus(callId, newValue);
        }
        catch (Exception ex)
        {
            Log.Error("Error in ChangeDocumentedCallStatus", ex);
            throw new Exception("Error in ChangeDocumentedCallStatus ---> ex.Message: " + ex.Message);
        }

    }

    [WebMethod(EnableSession = true)]
    public bool DocumentNewCall(DocumentedCall documentedCall)
    {
        if (documentedCall.CallContent.IndexOf("'") != -1)
        {
            documentedCall.CallContent = documentedCall.CallContent.Replace("'", "''");
        }
        try
        {
            return new DocumentedCall().DocumentNewCall(documentedCall);
        }
        catch (Exception ex)
        {
            Log.Error("Error in DocumentNewCall", ex);
            throw new Exception("Error in DocumentNewCall ---> ex.Message: " + ex.Message);
        }
    }



    [WebMethod(EnableSession = true)]
    public bool UpdateDocumentedCallField(string field2update, DocumentedCall documentedCall)
    {
        /*
         IMPORTANT NOTE!
            THIS METHOD HAS NOT TESTED YET DUE TO THIS ISSUE:
        "‏‏טופס הבדיקה זמין רק עבור פעולות שירות הכוללות סוגי נתונים בסיסיים כפרמטרים."
         WILL BE TESTED AS SOON AS THE HTML PAGE WILL BE UP
         */
        try
        {
            return new DocumentedCall().UpdateDocumentedCallField(field2update, documentedCall);
        }
        catch (Exception ex)
        {
            Log.Error("Error in UpdateDocumentedCallField", ex);
            throw new Exception("Error in UpdateDocumentedCallField ---> ex.Message: " + ex.Message);
        }
    }





    #endregion

    #region System_Log Module
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    [WebMethod(EnableSession = true)]
    public string GetLogs(string timeRange)
    {
        try
        {
            List<System_Log> logs = new System_Log().GetLogs(timeRange);
            return j.Serialize(logs);
        }
        catch (Exception ex)
        {
            Log.Error("Error in GetLogs", ex);
            throw new Exception("Error in GetLogs ---> ex.Message: " + ex.Message);
        }

    }
    #endregion


    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string readLocationsNamesByType(string locationType)
    {

        JavaScriptSerializer j = new JavaScriptSerializer();

        try
        {
            List<string> locations = Location.readLocationsNamesByType(locationType);

            return j.Serialize(locations);
        }
        catch (Exception e)
        {
            throw new Exception("שגיאה בשליפת נקודות");
        }

    }


    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]

    public string writeGoogleLocations(List<Location> googleLocations)
    {

        Location l = new Location();
        int numUpdated = l.write(googleLocations);

        JavaScriptSerializer j = new JavaScriptSerializer();
        return j.Serialize(numUpdated);
    }

    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string writeGoogleCities(List<City> googleCities)
    {

        City gc = new City();
        int numUpdated = gc.write(googleCities);

        JavaScriptSerializer j = new JavaScriptSerializer();
        return j.Serialize(numUpdated);
    }

    // return only cities where we have volunteers
    [WebMethod(EnableSession = true)]
    public string getVolCities()
    {
        try
        {
            City c = new City();
            List<City> citiesList = c.getVolCitiesList();
            return j.Serialize(citiesList);
        }
        catch (Exception ex)
        {
            Log.Error("Error in getVolCitiesList", ex);
            throw new Exception("getVolCitiesList error");
        }
    }

    [WebMethod(EnableSession = true)]
    public void TestConnections(int n)
    {
        DbService[] db = new DbService[n];
        for (int i = 0; i < n; i++)
        {
            db[i] = new DbService();
            db[i].con.Open();
            System.Threading.Thread.Sleep(50);
        }
    }


    [WebMethod(EnableSession = true)]
    public string GetTraces()
    {

        DbService dbsNoConnection = new DbService(true);
        List<string> result = dbsNoConnection.GetStackTraces();

        JavaScriptSerializer j = new JavaScriptSerializer();

        return j.Serialize(result);
    }

    [WebMethod(EnableSession = true)]
    public int GetTracesSizeInChars()
    {
        DbService dbsNoConnection = new DbService(true);
        List<string> result = dbsNoConnection.GetStackTraces();
        JavaScriptSerializer j = new JavaScriptSerializer();
        int res = j.Serialize(result).Length;

        return res;
    }


    [WebMethod(EnableSession = true)]
    public string ClearStackTraces()
    {
        DbService dbsNoConnection = new DbService(true);
        string result = dbsNoConnection.ClearStackTracesFile();
        JavaScriptSerializer j = new JavaScriptSerializer();

        return j.Serialize(result);
    }

    [WebMethod(EnableSession = true)]
    public string ClearSqlConnectionPool()
    {
        DbService dbs = new DbService();
        dbs.ClearSqlConnectionPool();

        return "finish";
    }

    [WebMethod(EnableSession = true)]
    public void mapNearestCities() {

        City c = new City();
        c.writeNearestMainCities();
    }

    [WebMethod(EnableSession = true)]
    public void UpdatePatientStatus(int patientId, int ridePatId, string patientStatus, DateTime? editTimeStamp)
    {
        try
        {
            RidePat rp = new RidePat();
            rp.UpdatePatientStatus(patientId, ridePatId, patientStatus, editTimeStamp);
        }
        catch (Exception ex)
        {
            Log.Error("Error in UpdatePatientStatus", ex);
            throw new Exception(ex.Message);
        }
    }


    [WebMethod(EnableSession = true)]
    public void UpdatePatientStatus_UnityRide(int patientId, int unityRideID, string patientStatus, DateTime? editTimeStamp)
    {
        try
        {
            UnityRide ur = new UnityRide();
            ur.updatePatientStatusandTime(patientId, unityRideID, patientStatus, editTimeStamp);
        }
        catch (Exception ex)
        {
            Log.Error("Error in UpdatePatientStatus", ex);
            throw new Exception(ex.Message);
        }
    }



    [WebMethod(EnableSession = true)]
    public void UpdateRidePatRemark(int ridePatId, string newRemark)
    {
        try
        {
            RidePat rp = new RidePat();
            rp.UpdateRidePatRemark(ridePatId, newRemark);
        }
        catch (Exception ex)
        {
            Log.Error("Error in UpdateRidePatRemark", ex);
            throw new Exception(ex.Message);
        }
    }


    [WebMethod(EnableSession = true)]
    public void UpdateUnityRideRemark(int UnityRideID, string newRemark)
    {
        try
        {
            UnityRide ur = new UnityRide();
            ur.updateRemark(UnityRideID, newRemark);
        }
        catch (Exception ex)
        {
            Log.Error("Error in UpdateRidePatRemark", ex);
            throw new Exception(ex.Message);
        }
    }
}




