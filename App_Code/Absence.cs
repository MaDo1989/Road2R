using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;


public class Absence
{
    int id;
    int volunteerId;
    int coordinatorId;
    int daysToReturn;
    DateTime fromDate;
    DateTime untilDate;
    string cause;
    string note;
    string coorName;
    bool absenceStatus;
    bool isDeleted;

    public int Id
    {
        get
        {
            return id;
        }

        set
        {
            id = value;
        }
    }

    public int VolunteerId
    {
        get
        {
            return volunteerId;
        }

        set
        {
            volunteerId = value;
        }
    }

    public int CoordinatorId
    {
        get
        {
            return coordinatorId;
        }

        set
        {
            coordinatorId = value;
        }
    }

    public DateTime FromDate
    {
        get
        {
            return fromDate;
        }

        set
        {
            fromDate = value;
        }
    }

    public DateTime UntilDate
    {
        get
        {
            return untilDate;
        }

        set
        {
            untilDate = value;
        }
    }

    public string Cause
    {
        get
        {
            return cause;
        }

        set
        {
            cause = value;
        }
    }

    public string Note
    {
        get
        {
            return note;
        }

        set
        {
            note = value;
        }
    }

    public bool AbsenceStatus
    {
        get
        {
            return absenceStatus;
        }

        set
        {
            absenceStatus = value;
        }
    }

    public bool IsDeleted
    {
        get
        {
            return isDeleted;
        }

        set
        {
            isDeleted = value;
        }
    }

    public int DaysToReturn
    {
        get
        {
            return daysToReturn;
        }

        set
        {
            daysToReturn = value;
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

    public List<Absence> GetAbsenceByVolunteerId(int volunteerId)
    {
        DBservice_Gilad dBservice_Gilad =new DBservice_Gilad();
        return dBservice_Gilad.GetAbsenceByVolunteerId(volunteerId);
    }

    static public int checkRidesBeforePostAbsence(int volunteerId,DateTime startDate, DateTime endDate)
    {
        DBservice_Gilad db = new DBservice_Gilad();
        return db.hasFutureRidesByDates(volunteerId, startDate, endDate);
    }

    public int UpdateAbsenceById(int AbsenceId, int coorId, DateTime from, DateTime until, string cause, string note)
    {
        DBservice_Gilad dBservice =new DBservice_Gilad();
        return dBservice.UpdateAbsenceById(AbsenceId, coorId, from, until, cause, note);
    }

    public int DeleteAbsenceById(int AbsenceId)
    {
        DBservice_Gilad dBservice =new DBservice_Gilad();
        return dBservice.DeleteAbsenceById(AbsenceId);
    }

    public int InsertNewAbsence(int volunteerId, int coorId, DateTime from, DateTime until, string cause, string note)
    {
        DBservice_Gilad dBservice = new DBservice_Gilad();
        return dBservice.InsertNewAbsence(volunteerId, coorId, from, until, cause, note);
    }
}