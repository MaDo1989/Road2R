using System;
using System.Activities.Expressions;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web;

public class UnityRide
{
    int ridePatNum;
	string patientName;
    int patientId;
    int patientGender;
    string patientStatus;
    DateTime patientStatusEditTime;
    string patientBirthdate;
    int patientAge;
	string patientCellPhone;
    int amountOfEscorts;
    int amountOfEquipments;
    List<string> patientEquipments;
    string origin;
	string destination;
	DateTime pickupTime;
	string coorName;
	string remark;
	string status;
	string area;
	string shift;
	bool onlyEscort;
    DateTime lastModified;
    int coorId;
    int mainDriver;
    string driverName;
    string driverCellPhone;
    int noOfDocumentedRides;
    bool isAnonymous;
    bool isNewDriver;

    public int RidePatNum
    {
        get
        {
            return ridePatNum;
        }

        set
        {
            ridePatNum = value;
        }
    }

    public string PatientName
    {
        get
        {
            return patientName;
        }

        set
        {
            patientName = value;
        }
    }

    public string PatientCellPhone
    {
        get
        {
            return patientCellPhone;
        }

        set
        {
            patientCellPhone = value;
        }
    }

    public string Origin
    {
        get
        {
            return origin;
        }

        set
        {
            origin = value;
        }
    }

    public string Destination
    {
        get
        {
            return destination;
        }

        set
        {
            destination = value;
        }
    }

    public DateTime PickupTime
    {
        get
        {
            return pickupTime;
        }

        set
        {
            pickupTime = value;
        }
    }

    public string CoorName
    {
        get
        {
            return coorName;
        }

        set
        {
            coorName = value;
        }
    }

    public string Remark
    {
        get
        {
            return remark;
        }

        set
        {
            remark = value;
        }
    }

    public string Status
    {
        get
        {
            return status;
        }

        set
        {
            status = value;
        }
    }

    public string Area
    {
        get
        {
            return area;
        }

        set
        {
            area = value;
        }
    }

    public string Shift
    {
        get
        {
            return shift;
        }

        set
        {
            shift = value;
        }
    }

    public bool OnlyEscort
    {
        get
        {
            return onlyEscort;
        }

        set
        {
            onlyEscort = value;
        }
    }

    public DateTime LastModified
    {
        get
        {
            return lastModified;
        }

        set
        {
            lastModified = value;
        }
    }

    public int CoorId
    {
        get
        {
            return coorId;
        }

        set
        {
            coorId = value;
        }
    }

    public int MainDriver
    {
        get
        {
            return mainDriver;
        }

        set
        {
            mainDriver = value;
        }
    }

    public string DriverName
    {
        get
        {
            return driverName;
        }

        set
        {
            driverName = value;
        }
    }

    public string DriverCellPhone
    {
        get
        {
            return driverCellPhone;
        }

        set
        {
            driverCellPhone = value;
        }
    }

    public int NoOfDocumentedRides
    {
        get
        {
            return noOfDocumentedRides;
        }

        set
        {
            noOfDocumentedRides = value;
        }
    }

    public bool IsAnonymous
    {
        get
        {
            return isAnonymous;
        }

        set
        {
            isAnonymous = value;
        }
    }

    public bool IsNewDriver
    {
        get
        {
            return isNewDriver;
        }

        set
        {
            isNewDriver = value;
        }
    }

    public int PatientId
    {
        get
        {
            return patientId;
        }

        set
        {
            patientId = value;
        }
    }

    public string PatientStatus
    {
        get
        {
            return patientStatus;
        }

        set
        {
            patientStatus = value;
        }
    }

    public string PatientBirthdate
    {
        get
        {
            return patientBirthdate;
        }

        set
        {
            patientBirthdate = value;
        }
    }

    public int AmountOfEscorts
    {
        get
        {
            return amountOfEscorts;
        }

        set
        {
            amountOfEscorts = value;
        }
    }

    public int AmountOfEquipments
    {
        get
        {
            return amountOfEquipments;
        }

        set
        {
            amountOfEquipments = value;
        }
    }

    public List<string> PatientEquipments
    {
        get
        {
            return patientEquipments;
        }

        set
        {
            patientEquipments = value;
        }
    }

    public DateTime PatientStatusEditTime
    {
        get
        {
            return patientStatusEditTime;
        }

        set
        {
            patientStatusEditTime = value;
        }
    }

    public int PatientGender
    {
        get
        {
            return patientGender;
        }

        set
        {
            patientGender = value;
        }
    }

    public int PatientAge
    {
        get
        {
            return patientAge;
        }

        set
        {
            patientAge = value;
        }
    }

    public List<UnityRide> GetUnityRideView(int days)
    {
        DBservice_Gilad dBservice_Gilad = new DBservice_Gilad();
        return dBservice_Gilad.GetRidesForRidePatView(days);
    }
    public List<object> GetUnityRide(int UnityRideId)
    {
        DBservice_Gilad dBservice = new DBservice_Gilad();
        return dBservice.GetUnityRide(UnityRideId);
    }

    public int SetUnityRide(UnityRide unityride, string func, int numOfRide, string repeatEvery,bool firstTry)
    {
        DBservice_Gilad dBservice = new DBservice_Gilad();
        if (dBservice.CheckValidDriverRides(unityride.RidePatNum,unityride.DriverName, unityride.PickupTime)==true && firstTry == true)
        {
            return -5;
        }

        if (func=="new")
        {
            if (numOfRide==1)
            {
                return dBservice.SetUnityRide(unityride);

            }
            else
            {
                List<DateTime> dateList = new List<DateTime>();
                dateList = BuildFutureRidesDates(unityride.pickupTime, repeatEvery, numOfRide);
                for (int i = 0; i < dateList.Count; i++)
                {
                    int res = 0;
                    unityride.PickupTime = dateList[i];
                    res=dBservice.SetUnityRide(unityride);
                    if (i==dateList.Count-1)
                    {
                        return res;
                    }
                }
            }


        }
        else if(func=="edit"){
            return dBservice.UpdateUnityRide(unityride);
        }
        // if didnt send func name its an error = -9 ;
        return -9;
    }

    public bool recoverUnityRides(List<int> ListIDs)
    {
        DBservice_Gilad db = new DBservice_Gilad();
        bool indcation = false;
        for (int i = 0; i < ListIDs.Count; i++)
        {
            int res = db.recoverUnityRide(ListIDs[i]);
           
            if (res >0)
            {
              indcation = true;
            }
            else
            {
              indcation = false;
            }
            
        }
        return indcation;
    }

    public string NEWCheckLocationForUnityRideArea(string origin, string destination)
    {
       
        Location l = new Location();
        string originArea = l.GetAreaForPoint(origin);
        string destinationArea = l.GetAreaForPoint(destination);
        List<string> allAreas = l.getAreas();
        string rideArea = originArea + " - " + destinationArea;
        if (originArea == destinationArea)
        {
            rideArea = originArea;
        }
        else if (!allAreas.Contains(rideArea))
        {
            rideArea = destinationArea + " - " + originArea;
            if (!allAreas.Contains(rideArea))
            {
                throw new ArgumentException("area not undefined");
            }
        }
        return rideArea;
    }

    public void updateUnityRideTime(int unityRideNum, DateTime editedTime)
    {
        DBservice_Gilad db = new DBservice_Gilad();
        UnityRide unityRide = new UnityRide();
        unityRide = db.updateUnityRideTime(unityRideNum, editedTime);
        if(unityRide.RidePatNum!=-1)
        {
            BroadCast.BroadCast2Clients_UnityRideUpdated(unityRide);
        }
    }

    public void updateRemark(int UnityRideID, string newRemark)
    {
        DBservice_Gilad dBservice = new DBservice_Gilad();
        UnityRide returnride = new UnityRide();
        returnride = dBservice.updateRemark(UnityRideID, newRemark);
        if (returnride.RidePatNum!=-1)
        {
            BroadCast.BroadCast2Clients_UnityRideUpdated(returnride);

        }
    }

    public void updatePatientStatusandTime (int patientId, int unityRideID, string patientStatus, DateTime? editTimeStamp)
    {
        DBservice_Gilad db = new DBservice_Gilad();
        UnityRide ur = new UnityRide();
        ur = db.updatePatientStatusAndTime(patientId, unityRideID, patientStatus, editTimeStamp);
        BroadCast.BroadCast2Clients_UnityRideUpdated(ur);
    }

    public void updateDriver(int driverId, int unityRideId, bool isDelete)
    {
        DBservice_Gilad db = new DBservice_Gilad();
        UnityRide ur = new UnityRide();
        if (isDelete)
        {
            ur = db.updateDriver(-1, unityRideId);
            //send push notification to coordinator phone
            Message m = new Message();
            //get driver details 
            Volunteer V = new Volunteer();
            V = V.getVolunteerByID(driverId);
            m.driverCanceledRide(unityRideId, V);

        }
        else
        {
            ur = db.updateDriver(driverId, unityRideId);

        }
        BroadCast.BroadCast2Clients_UnityRideUpdated(ur);

    }

    public void deleteUnityRide(List<int> listIDs)
    {
        DBservice_Gilad dBservice = new DBservice_Gilad();
        UnityRide ur = new UnityRide();
        if (listIDs.Count>1)
        {
            for (int i = 0; i < listIDs.Count; i++)
            {
                dBservice.deleteUnityRide(listIDs[i]);
            }
        }
        else
        {
            ur = dBservice.deleteUnityRide(listIDs[0]);
            BroadCast.BroadCast2Clients_UnityRideUpdated(ur);
        }


    }

    public List<UnityRide> Get_unityRide_ByTimeRange(int from, int until, bool isDeletedtoShow)
    {
        DBservice_Gilad db = new DBservice_Gilad();
        return db.Get_unityRide_ByTimeRange(from, until, isDeletedtoShow);
    }

    private List<DateTime> BuildFutureRidesDates(DateTime date, string repeatRideEvery, int numberOfRides)
    {
        List<DateTime> listOfDatesAfterUTCfix = new List<DateTime>();
        listOfDatesAfterUTCfix.Add(date);
        DateTime firstDate = date;

        DateTime dateAfterIncrement;
        for (int i = 1; i < numberOfRides; i++)
        {
            dateAfterIncrement = repeatRideEvery == "כל שבוע" ? date.AddDays(7) : date.AddDays(1);
            listOfDatesAfterUTCfix.Add(dateAfterIncrement);
            date = listOfDatesAfterUTCfix[i];
        }

        TimeZoneInfo israelTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Israel Standard Time");
        bool isFirstRideDateDayLightSaving = israelTimeZone.IsDaylightSavingTime(firstDate);

        for (int i = 1; i < listOfDatesAfterUTCfix.Count; i++)
        {
            if (isFirstRideDateDayLightSaving && !israelTimeZone.IsDaylightSavingTime(listOfDatesAfterUTCfix[i]))
            {
                listOfDatesAfterUTCfix[i] = listOfDatesAfterUTCfix[i].AddHours(1);
            }
            else if (!isFirstRideDateDayLightSaving && israelTimeZone.IsDaylightSavingTime(listOfDatesAfterUTCfix[i]))
            {
                listOfDatesAfterUTCfix[i] = listOfDatesAfterUTCfix[i].AddHours(-1);
            }
        }
        return listOfDatesAfterUTCfix;
    }
}