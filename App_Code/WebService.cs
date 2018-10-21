using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Services;
using System.Globalization;
using System.Web.Script.Services;
using log4net;

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
        Ride r = new Ride();
        bool res = r.isPrimaryStillCanceled(rideID, driverID);
        JavaScriptSerializer j = new JavaScriptSerializer();
        return j.Serialize(res);
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string backupToPrimary(int rideID, int driverID)
    {
        Ride r = new Ride();
        int res = r.backupToPrimary(rideID, driverID);
        JavaScriptSerializer j = new JavaScriptSerializer();
        return j.Serialize(res);
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
            throw new Exception("Error in getLocations: " + e.Message);
        }
    }

    [WebMethod]
    public int setVolunteerPrefs(int Id, List<string> PrefLocation, List<string> PrefArea, List<string> PrefTime, int AvailableSeats)
    {
        Volunteer v = new Volunteer();
        return v.setVolunteerPrefs(Id, PrefLocation, PrefArea, PrefTime, AvailableSeats);
    }


    [WebMethod]
    public string getVolunteerPrefs(int Id)
    {
        Volunteer v = new Volunteer();
        v = v.getVolunteerPrefs(Id);
        JavaScriptSerializer j = new JavaScriptSerializer();
        return j.Serialize(v);
    }

    [WebMethod]
    public int setRidePat(RidePat RidePat, string func)
    {
        RidePat rp = new RidePat();
        return rp.setRidePat(RidePat, func);
    }

    [WebMethod]
    public int setRideStatus(int rideId, string status)
    {
        Ride r = new Ride();
        return r.setStatus(rideId, status);
    }


    [WebMethod]
    public string getPatients(bool active)
    {
        JavaScriptSerializer j = new JavaScriptSerializer();
        Patient c = new Patient();
        List<Patient> patientsList = c.getPatientsList(active);
        return j.Serialize(patientsList);
    }

    [WebMethod]
    public string getEquipmentForPatient(string patient)
    {
        JavaScriptSerializer j = new JavaScriptSerializer();
        Patient p = new Patient();
        p.Equipment = p.getEquipmentForPatient(patient);
        return j.Serialize(p.Equipment);
    }

    [WebMethod]
    public string getCoorList()
    {
        JavaScriptSerializer j = new JavaScriptSerializer();
        Volunteer v = new Volunteer();
        List<Volunteer> vl = v.getCoorList();
        return j.Serialize(vl);
    }

    [WebMethod]
    public string getCoor(string userName)
    {
        JavaScriptSerializer j = new JavaScriptSerializer();
        Volunteer v = new Volunteer();
        v = v.getCoor(userName);
        return j.Serialize(v);
    }

    [WebMethod]
    public string getPatientEscorted(string displayName, string caller)
    {
        JavaScriptSerializer j = new JavaScriptSerializer();
        Patient p = new Patient();
        List<Escorted> escortedsList = p.getescortedsList(displayName, caller);
        return j.Serialize(escortedsList);
    }

    [WebMethod]
    public void setEscortedStatus(string displayName, string active)// change name to SetStatus
    {
        Escorted c = new Escorted();
        c.DisplayName = displayName;
        c.setEscortedStatus(active);
    }

    [WebMethod]
    public void SetPatientStatus(string displayName, string active)
    {
        Patient c = new Patient();
        c.DisplayName = displayName;
        c.SetPatientStatus(active);
    }

    [WebMethod]
    public void setPatient(Patient patient, string func)
    {
        Patient p = new Patient();
        p = patient;
        p.setPatient(func);
    }

    [WebMethod]
    public void setEscorted(Escorted escorted, string func)
    {
        Escorted p = new Escorted();
        p = escorted;
        p.setEscorted(func);
    }

    [WebMethod]
    public string getEscorted(string displayName, string patientName)
    {
        JavaScriptSerializer j = new JavaScriptSerializer();
        Escorted p = new Escorted();
        p.DisplayName = displayName;
        Escorted escorted = p.getEscorted(patientName);
        return j.Serialize(escorted);
    }

    [WebMethod]
    public string getPatient(string displayName)
    {
        JavaScriptSerializer j = new JavaScriptSerializer();
        Patient p = new Patient();
        p.DisplayName = displayName;
        Patient patient = p.getPatient();
        return j.Serialize(patient);
    }

    [WebMethod]
    public string getContactType()
    {
        JavaScriptSerializer j = new JavaScriptSerializer();
        Escorted e = new Escorted();
        List<string> cl = e.getContactType();
        return j.Serialize(cl);
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
        string test = (string)HttpContext.Current.Session["userSession"];

        RidePat rp = new RidePat();
        List<RidePat> r = rp.GetRidePatView(volunteerId);
        JavaScriptSerializer j = new JavaScriptSerializer();
        j.MaxJsonLength=Int32.MaxValue;
        return j.Serialize(r);
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string GetRidePat(int ridePatNum)
    {
        RidePat rp = new RidePat();
        rp = rp.GetRidePat(ridePatNum);
        JavaScriptSerializer j = new JavaScriptSerializer();
        return j.Serialize(rp);
    }


    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string getMyRides(int volunteerId)
    {
        //RidePat rp = new RidePat();
        //List<RidePat> r = rp.GetRidePat();
        Ride r = new Ride();
        List<Ride> rl = r.GetMyRides(volunteerId);
        JavaScriptSerializer j = new JavaScriptSerializer();
        return j.Serialize(rl);
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
        catch (Exception e)
        {
            throw new Exception("Error in CheckUser: " + e.Message);
        }


    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string SignDriver(int ridePatId, int ridePatId2, int driverId, bool primary)
    {
        RidePat rp = new RidePat();
        int res = rp.SignDriver(ridePatId, ridePatId2, driverId, primary);
        JavaScriptSerializer j = new JavaScriptSerializer();
        return j.Serialize(res);
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string DeleteDriver(int ridePatId, int driverId)
    {
        RidePat rp = new RidePat();
        int res = rp.DeleteDriver(ridePatId, driverId);
        JavaScriptSerializer j = new JavaScriptSerializer();
        return j.Serialize(res);
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string AssignRideToRidePat(int ridePatId, int userId, string driverType) //Get RidePatId & UserId, Create a new Ride with this info - then return RideId
    {
        RidePat rp = new RidePat();
        int res = rp.AssignRideToRidePat(ridePatId, userId, driverType);
        JavaScriptSerializer j = new JavaScriptSerializer();
        return j.Serialize(res);
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string CombineRideRidePat(int rideId, int RidePatId) //Get RideId & RidePatId - combine them
    {
        RidePat rp = new RidePat();
        int res = rp.CombineRideRidePat(rideId, RidePatId);
        JavaScriptSerializer j = new JavaScriptSerializer();
        return j.Serialize(res);
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string LeaveRidePat(int ridePatId, int rideId, int driverId)
    {
        RidePat rp = new RidePat();
        int res = rp.LeaveRidePat(ridePatId, rideId, driverId);
        JavaScriptSerializer j = new JavaScriptSerializer();
        return j.Serialize(res);
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string getAllStatus()
    {
        Status s = new Status();
        List<Status> sl = new List<Status>();
        sl = s.getAllStatus();
        JavaScriptSerializer j = new JavaScriptSerializer();
        return j.Serialize(sl);
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string getAllEquipment()
    {
        Patient p = new Patient();
        List<string> el = p.getAllEquipment();
        JavaScriptSerializer j = new JavaScriptSerializer();
        return j.Serialize(el);
    }

    #region volunteers functions
    [WebMethod]
    public void deactivateVolunteer(string displayName, string active)
    {
        Volunteer v = new Volunteer();
        v.DisplayName = displayName;
        v.deactivateCustomer(active);
    }

    [WebMethod]
    public void setVolunteer(Volunteer volunteer, string func)
    {
        Volunteer v = volunteer;
        v.setVolunteer(v, func);

    }

    [WebMethod]
    public void deactivateLocation(string displayName, string active)
    {
        Location l = new Location();
        l.Name = displayName;
        l.deactivateLocation(active);
    }
    [WebMethod]
    public void setLocation(Location location, string func)
    {
        Location l = location;
        l.setLocation(l, func);
    }
    [WebMethod]
    public string getLocation(string displayName)
    {
        JavaScriptSerializer j = new JavaScriptSerializer();
        Location l = new Location();
        l.Name = displayName;
        Location location = l.getLocation();
        return j.Serialize(location);
    }

    [WebMethod]
    public string getVolunteers(bool active)
    {
        JavaScriptSerializer j = new JavaScriptSerializer();
        Volunteer c = new Volunteer();
        List<Volunteer> volunteersList = c.getVolunteersList(active);
        return j.Serialize(volunteersList);
    }

    [WebMethod]
    public string getVolunteer(string displayName)
    {
        JavaScriptSerializer j = new JavaScriptSerializer();
        Volunteer v = new Volunteer();
        v.DisplayName = displayName;
        Volunteer volunteer = v.getVolunteer();
        return j.Serialize(volunteer);
    }

    [WebMethod]
    public string getVolunteerTypes()
    {
        JavaScriptSerializer j = new JavaScriptSerializer();
        Volunteer v = new Volunteer();

        List<string> types = v.getVolunteerTypes();
        return j.Serialize(types);
    }
    #endregion

    #region Destinations functions
    [WebMethod]
    public string getDestinationView(bool active)
    {
        JavaScriptSerializer j = new JavaScriptSerializer();
        Location d = new Location();
        List<Location> destinationsList = d.getDestinationsListForView(active);
        return j.Serialize(destinationsList);
    }

    [WebMethod]
    public string getHospitalView(bool active)
    {
        JavaScriptSerializer j = new JavaScriptSerializer();
        Location d = new Location();
        List<Location> hospitalList = d.getHospitalListForView(active);
        return j.Serialize(hospitalList);
    }

    [WebMethod]
    public string getBarrierView(bool active)
    {
        JavaScriptSerializer j = new JavaScriptSerializer();
        Location d = new Location();
        List<Location> hospitalList = d.getBarrierListForView(active);
        return j.Serialize(hospitalList);
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
        catch (Exception)
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
<<<<<<< HEAD
=======

    [WebMethod]
    public void writeLog(string str)
    {
        for (int i = 0; i < 50; i++)
        {
            LogEntry le = new LogEntry(DateTime.Now, str, "error in something" + i.ToString(), i);
            le.Write();
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


>>>>>>> sapir

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
        JavaScriptSerializer j = new JavaScriptSerializer();
        City c = new City();
        List<City> citiesList = c.getCitiesList();
        return j.Serialize(citiesList);
    }

    [WebMethod]
    public int backupToPrimaryNotification(int ridePatId)
    {       
                Message m = new Message();
               return m.backupToPrimary(ridePatId);               
    }

}
