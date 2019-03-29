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
public class WebService : System.Web.Services.WebService
{
    private static readonly ILog Log =
             LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    public WebService()
    {
        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    //----------------------Road to Recovery-----------------------------------------------
    //[WebMethod]
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

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string isPrimaryStillCanceled(int driverID, int rideID)
    {
        try
        {
            Ride r = new Ride();
            bool res = r.isPrimaryStillCanceled(rideID, driverID);
            JavaScriptSerializer j = new JavaScriptSerializer();
            return j.Serialize(res);
        }
        catch (Exception ex)
        {
            Log.Error("Error in isPrimaryStillCanceled", ex);
            throw new Exception("שגיאה בבדיקה האם נהג ראשי מבוטל");
        }
        
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string backupToPrimary(int rideID, int driverID)
    {
        try
        {
            Ride r = new Ride();
            int res = r.backupToPrimary(rideID, driverID);
            JavaScriptSerializer j = new JavaScriptSerializer();
            return j.Serialize(res);
        }
        catch (Exception ex)
        {
            Log.Error("Error in backupToPrimary", ex);
            throw new Exception("שגיאה בעדכון נהג ראשי");
        }
        
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string getLocations()
    {
        try
        {
            List<Location> ll = new List<Location>();
            Location l = new Location();
            ll.AddRange(l.getHospitalListForView(true));
            ll.AddRange(l.getBarrierListForView(true));

            JavaScriptSerializer j = new JavaScriptSerializer();
            return j.Serialize(ll);
        }
        catch (Exception e)
        {
            Log.Error("Error in getLocations", e);
            throw new Exception("שגיאה בשליפת נתוני נקודות איסוף והורדה");
        }
    }

    [WebMethod]
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

    [WebMethod]
    public string getescortedsListMobile(string displayName, string patientCell)
    {
        try
        {
            JavaScriptSerializer j = new JavaScriptSerializer();
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
    [WebMethod]
    public string getVolunteerPrefs(int Id)
    {
        try
        {
            Volunteer v = new Volunteer();
            v = v.getVolunteerPrefs(Id);
            JavaScriptSerializer j = new JavaScriptSerializer();
            return j.Serialize(v);
        }
        catch(Exception ex)
        {
            Log.Error("Error in getVolunteerPrefs", ex);
            throw new Exception("שגיאה בשליפת נתוני העדפות המתנדב");

        }
    }
    //לסדר
    [WebMethod(EnableSession = true)]
    public int setRidePat(RidePat RidePat, string func, bool isAnonymous)
    {
        try
        {
            RidePat rp = new RidePat();
            int res = rp.setRidePat(RidePat, func,isAnonymous);
            //write to log on delete 
            if (res > 0 && func == "delete")
            {
                string message = "";
                message = " נסיעה מספר " + RidePat.RidePatNum + " מ"+RidePat.Origin.Name+" ל"+RidePat.Destination.Name+" עם החולה "+RidePat.Pat.DisplayName+" בוטלה.";
                LogEntry le = new LogEntry(DateTime.Now, "info", message, 1);
            }
            return res;
        }
        catch(Exception ex)
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
                message = "  נסיעה מספר " + rideId + " עם הנהג " + a.getDriverName(driverId) +  " שינתה סטטוס ל" + status;
            }
            LogEntry le = new LogEntry(DateTime.Now, "שינוי סטטוס", message, 2, rideId, true);
            return r.setStatus(rideId, status);
        }
        catch (Exception ex)
        {
            Log.Error("Error in setRideStatus",ex);
            throw new Exception(" שגיאה בשינוי הסטטוס");
        }

    }

    [WebMethod]
    public string getPatients(bool active)
    {
        try
        {
            JavaScriptSerializer j = new JavaScriptSerializer();
            Patient c = new Patient();
            List<Patient> patientsList = c.getPatientsList(active);
            return j.Serialize(patientsList);
        }
        catch (Exception ex)
        {
            Log.Error("Error in getPatients", ex);
            throw new Exception("שגיאה בשליפת נתוני חולים");
        }

    }
    
    [WebMethod(Description ="get patients with the same origin and destination")]
    public string getPatientsForAnonymous(bool active,string origin,string dest)
    {
        try
        {
            JavaScriptSerializer j = new JavaScriptSerializer();
            Patient c = new Patient();
            List<Patient> patientsList = c.getAnonymousPatientsList(active,origin,dest);
            return j.Serialize(patientsList);
        }
        catch (Exception ex)
        {
            Log.Error("Error in getPatientsForAnonymous", ex);
            throw new Exception("שגיאה בשליפת נתוני חולים");
        }

    }

    [WebMethod]
    public string getEquipmentForPatient(string patient)
    {
        try
        {
            JavaScriptSerializer j = new JavaScriptSerializer();
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

    [WebMethod]
    public string getCoorList()
    {
        try
        {
            JavaScriptSerializer j = new JavaScriptSerializer();
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

    [WebMethod]
    public string getCoor(string userName)
    {
        try
        {
            JavaScriptSerializer j = new JavaScriptSerializer();
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

    [WebMethod]
    public string getPatientEscorted(string displayName, string caller)
    {
        try
        {
            JavaScriptSerializer j = new JavaScriptSerializer();
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
    [WebMethod]
    public int getSpaceInCar(int ridePatNum, int driverId)
    {
        try
        {
            JavaScriptSerializer j = new JavaScriptSerializer();
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

    [WebMethod]
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

    [WebMethod(Description = "Set Patient Status",EnableSession =true)]
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

    [WebMethod]
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
    [WebMethod]
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
    [WebMethod]
    public void setUserPassword(string userName, string password)
    {
        try
        {
            Volunteer v = new Volunteer();
            v.setUserPassword(userName,password);
        }
        catch (Exception ex)
        {
            Log.Error("Error in setUserPassword", ex);
            throw new Exception("שגיאה בהחלפת סיסמה");
        }

    }

    [WebMethod]
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
    [WebMethod]
    public void setNewVersion(string userName, string google, string appstore, DateTime date, string version,bool mandatory)
    {
        try
        {
            Version v = new Version();
            //add mandatory
            v.setNewVersion(userName, google, appstore, date, version,mandatory);
        }
        catch (Exception ex)
        {
            Log.Error("Error in setNewVersion", ex);
            throw new Exception("שגיאה בעדכון גרסה חדשה");
        }
    }

    [WebMethod]
    public string getEscorted(string displayName, string patientName)
    {
        try
        {
            JavaScriptSerializer j = new JavaScriptSerializer();
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

    [WebMethod]
    public string getPatient(string displayName)
    {
        try
        {
            JavaScriptSerializer j = new JavaScriptSerializer();
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
    [WebMethod]
    public string getAnonymousPatient(string displayName)
    {
        try
        {
            JavaScriptSerializer j = new JavaScriptSerializer();
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

    [WebMethod]
    public string getContactType()
    {
        try
        {
            JavaScriptSerializer j = new JavaScriptSerializer();
            Escorted e = new Escorted();
            List<string> cl = e.getContactType();
            return j.Serialize(cl);
        }
        catch (Exception ex)
        {
            Log.Error("Error in getContactType", ex);
            throw new Exception("שגיאה בשליפת קרבת מלווה");
        }

    }


    //[WebMethod]
    //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    //public string getRidePatEscortView(string test)//Insert into same method as getRides.
    //{
    //    RidePat rp = new RidePat();
    //    //List<RidePat> r = rp.GetRidePat();
    //    List<RidePat> r = rp.GetRidePatEscortView();
    //    JavaScriptSerializer j = new JavaScriptSerializer();
    //    return j.Serialize(r);
    //}

    //This method is used for שבץ אותי
    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string GetRidePatView(int volunteerId)
    {
        try
        {
            //string test = (string)HttpContext.Current.Session["userSession"];

            RidePat rp = new RidePat();
            List<RidePat> r = rp.GetRidePatView(volunteerId);
            JavaScriptSerializer j = new JavaScriptSerializer();
            j.MaxJsonLength = Int32.MaxValue;
            return j.Serialize(r);
        }
        catch (Exception ex)
        {
            Log.Error("Error in GetRidePatView", ex);
            throw new Exception("שגיאה בשליפת נתוני הסעות");
        }

    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string GetRidePat(int ridePatNum)
    {
        try
        {
            RidePat rp = new RidePat();
            rp = rp.GetRidePat(ridePatNum);
            JavaScriptSerializer j = new JavaScriptSerializer();
            return j.Serialize(rp);
        }
        catch (Exception ex)
        {
            Log.Error("Error in GetRidePat", ex);
            throw new Exception("שגיאה בשליפת נתוני הסעה");
        }

    }


    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string getMyRides(int volunteerId)
    {
        //RidePat rp = new RidePat();
        //List<RidePat> r = rp.GetRidePat();
        try
        {
            Ride r = new Ride();
            List<Ride> rl = r.GetMyRides(volunteerId);
            JavaScriptSerializer j = new JavaScriptSerializer();
            return j.Serialize(rl);
        }
        catch (Exception ex)
        {
            Log.Error("Error in getMyRides", ex);
            throw new Exception("שגיאה בשליפת נתוני הסעות");
        }

    }

    //used for getting all versions of the app both in the appstore and in google play
    //the results order by DESC so if we want the latest version we get the first Version in the list.
    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string getVersions()
    {
        try
        {
            Version v = new Version();
            List<Version> vl = v.getVersions();
            JavaScriptSerializer j = new JavaScriptSerializer();
            return j.Serialize(vl);
        }
        catch (Exception ex)
        {
            Log.Error("Error in getVersions", ex);
            throw new Exception("שגיאה בשליפת נתוני גרסאות אפליקציה");
        }

    }

    //[WebMethod]
    //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    //public string GetRidesForNotify()
    //{
    //    Ride r = new Ride();
    //    //List<RidePat> r = rp.GetRidePat();
    //    List<Ride> rl = r.GetRidesForNotify();
    //    JavaScriptSerializer j = new JavaScriptSerializer();
    //    return j.Serialize(rl);
    //}

    //[WebMethod]
    //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    //public string DeleteRide(int RidePatId)
    //{
    //    RidePat rp = new RidePat();
    //    //List<RidePat> r = rp.GetRidePat();
    //    // int res = rp.DeleteRide(RidePatId);
    //    JavaScriptSerializer j = new JavaScriptSerializer();
    //    return j.Serialize("n");
    //}

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string CheckUser(string mobile, string regId)
    {
        try
        {
            Volunteer v = new Volunteer();
            v = v.getVolunteerByMobile(mobile, regId);
            JavaScriptSerializer j = new JavaScriptSerializer();
            return j.Serialize(v);
        }
        catch (Exception ex)
        {
            Log.Error("Error in CheckUser", ex);
            throw new Exception("שגיאה בבדיקת נתוני משתמש");
        }


    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string SignDriver(int ridePatId, int ridePatId2, int driverId, bool primary)
    {
        try
        {
            RidePat rp = new RidePat();
            int res = rp.SignDriver(ridePatId, ridePatId2, driverId, primary);
            JavaScriptSerializer j = new JavaScriptSerializer();
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
 

    [WebMethod(EnableSession = true,Description = "delete's driver from a ride, for example - two ridepats on the same ride")]
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
                    V.getVolunteerByID(driverId);
                    m.driverCanceledRide(ridePatId,V.getVolunteerByID(driverId));
                }
            }

            JavaScriptSerializer j = new JavaScriptSerializer();
            return j.Serialize(res);
        }
        catch (Exception ex)
        {
            Log.Error("Error in DeleteDriver", ex);
            throw new Exception(ex.Message);
        }

    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string AssignRideToRidePat(int ridePatId, int userId, string driverType) //Get RidePatId & UserId, Create a new Ride with this info - then return RideId
    {
        try
        {
            RidePat rp = new RidePat();
            int res = rp.AssignRideToRidePat(ridePatId, userId, driverType);
            JavaScriptSerializer j = new JavaScriptSerializer();
            return j.Serialize(res);
        }
        catch (Exception ex)
        {
            Log.Error("Error in AssignRideToRidePat", ex);
            if (ex.Message == "נסיעה זו בוטלה, תודה על הרצון לעזור")
            {
                throw new Exception(ex.Message);
            }
            else throw new Exception("שגיאה בצירוף הסעה לנסיעה");
        }

    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string CombineRideRidePat(int rideId, int RidePatId) //Get RideId & RidePatId - combine them
    {
        try
        {
            RidePat rp = new RidePat();
            int res = rp.CombineRideRidePat(rideId, RidePatId);
            JavaScriptSerializer j = new JavaScriptSerializer();
            return j.Serialize(res);
        }
        catch (Exception ex)
        {
            Log.Error("Error in CombineRideRidePat", ex);
            throw new Exception("שגיאה במיזוג הסעה לנסיעה");
        }
    }



    [WebMethod(EnableSession =true,Description = "delete from only one ride pat")]
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
            JavaScriptSerializer j = new JavaScriptSerializer();
            return j.Serialize(res);
        }
        catch (Exception ex)
        {
            Log.Error("Error in LeaveRidePat", ex);
            throw new Exception("שגיאה בעת שנהג עזב נסיעה");
        }

    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string getAllStatus()
    {
        try
        {
            Status s = new Status();
            List<Status> sl = new List<Status>();
            sl = s.getAllStatus();
            JavaScriptSerializer j = new JavaScriptSerializer();
            return j.Serialize(sl);
        }
        catch (Exception ex)
        {
            Log.Error("Error in getAllStatus", ex);
            throw new Exception("שגיאה בשליפת סטטוסים");
        }

    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string getAllEquipment()
    {
        try
        {
            Patient p = new Patient();
            List<string> el = p.getAllEquipment();
            JavaScriptSerializer j = new JavaScriptSerializer();
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

    [WebMethod]
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
    [WebMethod]
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
    [WebMethod]
    public string getLocation(string displayName)
    {
        try
        {
            JavaScriptSerializer j = new JavaScriptSerializer();
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

    [WebMethod]
    public string getVolunteers(bool active)
    {
        try
        {
            JavaScriptSerializer j = new JavaScriptSerializer();
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

    [WebMethod]
    public string getVolunteer(string displayName)
    {
        try
        {
            JavaScriptSerializer j = new JavaScriptSerializer();
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

    [WebMethod]
    public string getVolunteerTypes()
    {
        try
        {
            JavaScriptSerializer j = new JavaScriptSerializer();
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
    [WebMethod]
    public string getDestinationView(bool active)
    {
        try
        {
            JavaScriptSerializer j = new JavaScriptSerializer();
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

    [WebMethod]
    public string getHospitalView(bool active)
    {
        try
        {
            JavaScriptSerializer j = new JavaScriptSerializer();
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

    [WebMethod]
    public string getBarrierView(bool active)
    {
        try
        {
            JavaScriptSerializer j = new JavaScriptSerializer();
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

    [WebMethod]
    public string confirmPush(int userId, int msgId, string status)
    {
        Message m = new Message();
        m.insertMsg(msgId, "", status, "", 0, DateTime.Now, userId, "", true, false, false);

        JavaScriptSerializer j = new JavaScriptSerializer();
        return j.Serialize("ok");
    }
    #endregion

    #region login functions
    [WebMethod(EnableSession = true)]
    public string loginUser(string uName, string password)
    {
        JavaScriptSerializer j = new JavaScriptSerializer();
        bool userInDB;
        try
        {

            User u = new User(uName, password);
            userInDB = u.CheckLoginDetails();
        }
        catch (Exception ex)
        {

            throw;
        }
        HttpContext.Current.Session["userSession"] = uName;

        writeToLog("Successful login");


        return j.Serialize(userInDB);
    }

    public void writeToLog(string str)
    {
        string user = (string)HttpContext.Current.Session["userSession"];
        Log.Error(str + " ;for user: " + user);
    }
    [WebMethod]
    public string GetUserNameByCellphone(string uName)
    {
        string userInDB;
        JavaScriptSerializer j = new JavaScriptSerializer();
        try
        {

            User u = new User();
            userInDB = u.getUserNameByCellphone(uName);
        }
        catch (Exception ex)
        {

            throw;
        }
        return j.Serialize(userInDB);
    }
    [WebMethod]
    public void writeLog(string str)
    {
        for (int i = 0; i < 50; i++)
        {
            LogEntry le = new LogEntry(DateTime.Now, str, "error in something" + i.ToString(), i);
           

        }
    }

    [WebMethod]
    public string getLog(int hours)
    {
        JavaScriptSerializer j = new JavaScriptSerializer();
        LogEntry le = new LogEntry();
        List<LogEntry> list = le.GetLog(hours);
        return j.Serialize(list);
    }

    //[WebMethod]
    //public string getUserType(string user)
    //{
    //    JavaScriptSerializer j = new JavaScriptSerializer();
    //    User u = new User();
    //    u.UserName = user;
    //    u.UserType = u.getUserType(u.UserName);
    //    return j.Serialize(u.UserType);
    //}




    [WebMethod]
    public string loginDriver(string uName, string password)
    {
        JavaScriptSerializer j = new JavaScriptSerializer();
        Drivers d = new Drivers(uName, password);
        d = d.CheckLoginDetails();
        return j.Serialize(d);
    }
    #endregion

    [WebMethod]
    public string getCities()
    {
        try
        {
            JavaScriptSerializer j = new JavaScriptSerializer();
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

    [WebMethod]
    public int backupToPrimaryNotification(int ridePatId)
    {
        Message m = new Message();
        return m.backupToPrimary(ridePatId);
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string getR2RServers()
    {
        try
        {
            Auxiliary a = new Auxiliary();
            JavaScriptSerializer j = new JavaScriptSerializer();
            //a.getR2RServers()
            return j.Serialize(a.getR2RServers());
        }
        catch (Exception ex)
        {
            Log.Error("Error in getServers", ex);
            throw new Exception("שגיאה בשליפת שרתים");
        }
    }
}

