using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for candidatesLogic
/// </summary>
public class CandidatesLogic
{
    DbService dbs;
    Dictionary<string, Candidate> candidates;


    public Dictionary<string, Candidate> GetCandidates(int ridePatNum)
    {

        string query = "exec spGetCandidatesForRidePat @RidePatNum=" + ridePatNum;
        query += ",@NumOfDaysToThePast=" + Constants.Candidate.NumOfDaysToThePast;
        query += ",@NUmOfDaysToTheFuture=" + Constants.Candidate.NUmOfDaysToTheFuture;
        query += ",@NumOfDaysToThePast_CheckRides_Regular=" + Constants.Candidate.NumOfDaysToThePast_CheckRides_Regular;
        query += ",@NumOfDaysToTheFuture_CheckRides_Regular=" + Constants.Candidate.NumOfDaysToTheFuture_CheckRides_Regular;
        query += ",@NumOfDaysToThePast_CheckRides_Super=" + Constants.Candidate.NumOfDaysToThePast_CheckRides_Super;
        query += ",@NumOfDaysToTheFuture_CheckRides_Super=" + Constants.Candidate.NumOfDaysToTheFuture_CheckRides_Super;
        query += ",@AmountBottomLimitToBeSuperUserDriver=" + Constants.Candidate.AmountBottomLimitToBeSuperUserDriver;

        try
        {
            dbs = new DbService();
            SqlDataReader sdr = dbs.GetDataReader(query);
            Candidate candidate;
            candidates = new Dictionary<string, Candidate>();
            List<int> ammountOfPathMatch;
            while (sdr.Read())
            {
                ammountOfPathMatch = new List<int>();
                ammountOfPathMatch.Add(Convert.ToInt32(sdr["AmmountOfPathMatchScoreOfType_0"]));
                ammountOfPathMatch.Add(Convert.ToInt32(sdr["AmmountOfPathMatchScoreOfType_1"]));
                ammountOfPathMatch.Add(Convert.ToInt32(sdr["AmmountOfPathMatchScoreOfType_2"]));
                ammountOfPathMatch.Add(Convert.ToInt32(sdr["AmmountOfPathMatchScoreOfType_3"]));
                ammountOfPathMatch.Add(Convert.ToInt32(sdr["AmmountOfPathMatchScoreOfType_4"]));

                candidate = new Candidate(
                    Convert.ToInt32(sdr["Id"]),
                    Convert.ToString(sdr["DisplayName"]),
                    Convert.ToBoolean(sdr["IsSuperDriver"]),
                    ammountOfPathMatch,
                    Convert.ToInt32(sdr["AmmountOfMatchByDay"]),
                    Convert.ToInt32(sdr["AmmountOfDissMatchByDay"])
                );
                candidates.Add(Convert.ToString(candidate.Id), candidate);
            }
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
        finally
        {
            dbs.CloseConnection();
        }

        return candidates;
    }
    //List<Candidate> superDriversList = new List<Candidate>();
    //List<Candidate> regularDriversList = new List<Candidate>();
    //List<Candidate> newDriversList = new List<Candidate>();
    //List<Candidate> newbies = new List<Candidate>();

    //public List<Candidate> Newbies
    //{
    //    get
    //    {
    //        return newbies;
    //    }

    //    set
    //    {
    //        newbies = value;
    //    }
    //}

    //public List<Candidate> SuperDriversList
    //{
    //    get
    //    {
    //        return superDriversList;
    //    }

    //    set
    //    {
    //        superDriversList = value;
    //    }
    //}

    //public List<Candidate> RegularDriversList
    //{
    //    get
    //    {
    //        return regularDriversList;
    //    }

    //    set
    //    {
    //        regularDriversList = value;
    //    }
    //}

    //public List<Candidate> NewDriversList
    //{
    //    get
    //    {
    //        return newDriversList;
    //    }

    //    set
    //    {
    //        newDriversList = value;
    //    }
    //}


    //public candidatesLogic()
    //{
    //    //
    //    // TODO: Add constructor logic here
    //    //
    //}

    //public candidatesLogic getCandidates(int ridePatNum) {
    //    candidatesLogic cl = new candidatesLogic();
    //    Candidate c = new Candidate();
    //    List<Candidate> canList = c.GetCandidates(ridePatNum);
    //  //  List<Candidate> newbies = c.GetNewCandidates(60);
    //    canList.AddRange(newbies);

    //    foreach (Candidate can in canList) {
    //        if (can.IsSuperUser)
    //            cl.SuperDriversList.Add(can);
    //        if (can.IsNewbie)
    //            cl.Newbies.Add(can);
    //        else
    //            cl.RegularDriversList.Add(can);
    //    }
    //    return cl;
    //}

}