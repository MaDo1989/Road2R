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

    public int SetUnityRide(UnityRide unityride)
    {
        DBservice_Gilad dBservice = new DBservice_Gilad();
        return dBservice.SetUnityRide(unityride);
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
}