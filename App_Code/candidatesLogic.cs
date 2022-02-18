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
                    Convert.ToInt32(sdr["AmmountOfDissMatchByDay"]),
                    Convert.ToInt32(sdr["AmmountOfAfterNoonRides"]),
                    Convert.ToInt32(sdr["AmmountOfMorningRides"])
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
   
}