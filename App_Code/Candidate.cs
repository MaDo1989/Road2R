using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for Candidate
/// </summary>
public class Candidate
{
    //here I stop and went to develop sort by regions

    string DisplayName;
    short maxPathMatch;
    bool isSuperUser;
    bool dayMatch;
    short closestRide;




    public short MaxPathMatch
    {
        get
        {
            return maxPathMatch;
        }

        set
        {
            maxPathMatch = value;
        }
    }

    public bool IsSuperUser
    {
        get
        {
            return isSuperUser;
        }

        set
        {
            isSuperUser = value;
        }
    }

    public bool DayMatch
    {
        get
        {
            return dayMatch;
        }

        set
        {
            dayMatch = value;
        }
    }

    public short ClosestRide
    {
        get
        {
            return closestRide;
        }

        set
        {
            closestRide = value;
        }
    }

    public string DisplayName1
    {
        get
        {
            return DisplayName;
        }

        set
        {
            DisplayName = value;
        }
    }



    public List<Candidate> GetCandidates(int ridePatNum) {

        string query = "exec spGetRideCandidates @ridePatNum=" + ridePatNum;
        List<Candidate> candidatesList = new List<Candidate>();
        
        DbService dbs = new DbService();

        try
        {
            SqlDataReader sdr = dbs.GetDataReader(query);
            while (sdr.Read())
            {
                Candidate c = new Candidate();
                c.DisplayName = sdr["DisplayName"].ToString();
                //c.CellPhone = sdr["CellPhone"].ToString();
                c.IsSuperUser = Convert.ToBoolean(sdr["superUser"]);
                c.DayMatch = Convert.ToBoolean(sdr["dayMatch"]);
                c.MaxPathMatch = Convert.ToInt16(sdr["maxPathMatch"]);
                c.ClosestRide = Convert.ToInt16(sdr["closestRideInDays"]);
                candidatesList.Add(c);
            }
            return candidatesList;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
        finally
        {
            dbs.CloseConnection();
        }


    }


}