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
    Dictionary<string, Candidate> newbies;

    enum  Level
    {
        Newbee,
        Regular,
        Super
    }

    public Dictionary<string, Candidate> GetCandidates(int ridePatNum, int numOfCandidates, int dayInWeek)
    {

        Dictionary<string, Candidate> newbies;

        string query = "exec spGetCandidatesForRidePat @RidePatNum=" + ridePatNum;
        query += ",@NumOfDaysToThePast=" + Constants.Candidate.NumOfDaysToThePast;
        query += ",@NUmOfDaysToTheFuture=" + Constants.Candidate.NUmOfDaysToTheFuture;
        query += ",@NumOfDaysToThePast_CheckRides_Regular=" + Constants.Candidate.NumOfDaysToThePast_CheckRides_Regular;
        query += ",@NumOfDaysToTheFuture_CheckRides_Regular=" + Constants.Candidate.NumOfDaysToTheFuture_CheckRides_Regular;
        query += ",@NumOfDaysToThePast_CheckRides_Super=" + Constants.Candidate.NumOfDaysToThePast_CheckRides_Super;
        query += ",@NumOfDaysToTheFuture_CheckRides_Super=" + Constants.Candidate.NumOfDaysToTheFuture_CheckRides_Super;
        query += ",@AmountBottomLimitToBeSuperUserDriver=" + Constants.Candidate.AmountBottomLimitToBeSuperUserDriver;
        query += ",@AmountOfRidesInNewDriverTimeWindow=" + Constants.Candidate.AmountOfRidesInNewDriverTimeWindow;
        query += ",@NewDriverTimeWindow=" + Constants.Candidate.NewDriverTimeWindow;

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

                bool superUser = Convert.ToBoolean(sdr["IsSuperDriver"]);
                int driverLevel = (int) Level.Regular;
                if (superUser) driverLevel = (int) Level.Super;
                

                candidate = new Candidate(
                    Convert.ToInt32(sdr["Id"]),
                    Convert.ToString(sdr["DisplayName"]),
                    Convert.ToInt32(sdr["NoOfDocumentedCalls"]),
                    driverLevel,
                    ammountOfPathMatch,
                    Convert.ToInt32(sdr["AmmountOfMatchByDay"]),
                    Convert.ToInt32(sdr["AmmountOfDisMatchByDay"]),
                    Convert.ToInt32(sdr["AmmountOfMatchDayPart"]),
                    Convert.ToInt32(sdr["AmmountOfDisMatchDayPart"])
                );
                candidates.Add(Convert.ToString(candidate.Id), candidate);
            }
            newbies = GetNewbiesCandidates(ridePatNum,1000); // 100 is just a big number to get all the new drivers

        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
        finally
        {
            dbs.CloseConnection();
        }

        // ADDED BY BENNY
        // THE LOGIC OF CHOOSING CANDIDATES

       

        foreach (KeyValuePair<string,Candidate> kv in newbies) // add the newbies to the candidates
        {
            candidates.Add(kv.Key, kv.Value);
        }

        Dictionary<string, Candidate> bestCandidates = selectBestCandidates(candidates, true, numOfCandidates, dayInWeek);

        return FillExtraDetails(bestCandidates);
    }

    public Dictionary<string, Candidate> GetNewbiesCandidates(int ridePatNum, int numOfCandidates)
    {
        string query = "exec spGetNoobsCandidatesForRidePat @RidePatNum=" + ridePatNum;
        query += ",@NumOfDaysToThePast=" + Constants.Candidate.NumOfDaysToThePast;
        query += ",@NUmOfDaysToTheFuture=" + Constants.Candidate.NUmOfDaysToTheFuture;
        query += ",@NumOfDaysToThePast_CheckRides_Regular=" + Constants.Candidate.NumOfDaysToThePast_CheckRides_Regular;
        query += ",@NumOfDaysToTheFuture_CheckRides_Regular=" + Constants.Candidate.NumOfDaysToTheFuture_CheckRides_Regular;
        query += ",@NumOfDaysToThePast_CheckRides_Super=" + Constants.Candidate.NumOfDaysToThePast_CheckRides_Super;
        query += ",@NumOfDaysToTheFuture_CheckRides_Super=" + Constants.Candidate.NumOfDaysToTheFuture_CheckRides_Super;
        query += ",@AmountBottomLimitToBeSuperUserDriver=" + Constants.Candidate.AmountBottomLimitToBeSuperUserDriver;
        query += ",@AmountOfRidesInNewDriverTimeWindow=" + Constants.Candidate.AmountOfRidesInNewDriverTimeWindow;
        query += ",@NewDriverTimeWindow=" + Constants.Candidate.NewDriverTimeWindow;

        try
        {
            dbs = new DbService();
            SqlDataReader sdr = dbs.GetDataReader(query);
            Candidate candidate;
            newbies = new Dictionary<string, Candidate>();
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
                    Convert.ToInt32(sdr["NoOfDocumentedCalls"]),
                    (int) Level.Newbee,
                    ammountOfPathMatch,
                    Convert.ToInt32(sdr["AmmountOfMatchByDay"]),
                    Convert.ToInt32(sdr["AmmountOfDisMatchByDay"]),
                    Convert.ToInt32(sdr["AmmountOfMatchDayPart"]),
                    Convert.ToInt32(sdr["AmmountOfDisMatchDayPart"])
                );
                
                newbies.Add(Convert.ToString(candidate.Id), candidate);
            }
            return newbies;
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

    private Dictionary<string, Candidate> selectBestCandidates(Dictionary<string, Candidate> candidates,bool rand, int numOfCandidates, int dayInWeek) {

        Dictionary<string, double> weights;
        if (dayInWeek < 5) 
        {
            weights = new Dictionary<string, double>{
            {"point2point",50 },
            {"oposite",20 },
            {"point2area",20 },
            {"area2area",8 },
            {"otherAears",2 },
            {"routeWeight",0.7 },
            {"sameDay", 40 },
            {"DifferentDays",10 },
            {"SameDayPart",40},
            {"DifferentDayPart",10},
            {"timeWeight", 0.3 }
        };
        }
        else // friday of saturday
        {
            weights = new Dictionary<string, double>{
            {"point2point",50 },
            {"oposite",20 },
            {"point2area",20 },
            {"area2area",8 },
            {"otherAears",2 },
            {"routeWeight",0.3 },
            {"sameDay", 70 },
            {"DifferentDays",5 },
            {"SameDayPart",20},
            {"DifferentDayPart",5},
            {"timeWeight", 0.7 }
        };
        }



        Dictionary<string, Candidate> super  = new Dictionary<string, Candidate>();
        Dictionary<string, Candidate> regular = new Dictionary<string, Candidate>();
        Dictionary<string, Candidate> newbee = new Dictionary<string, Candidate>();



        // seperate them to super volunteers and regular
        foreach (KeyValuePair<string, Candidate> kv in candidates) {
            if (kv.Value.DriverLevel == (int)Level.Super) super.Add(kv.Key, kv.Value);
            else if (kv.Value.DriverLevel == (int)Level.Regular) regular.Add(kv.Key, kv.Value);
            else if (kv.Value.DriverLevel == (int)Level.Newbee) newbee.Add(kv.Key, kv.Value);
            else throw new Exception("must be Super or Regular or Newbee");

        }

        Dictionary<string, double> superScore = calculateScore(super, weights);
        Dictionary<string, double> regularScore = calculateScore(regular, weights);
        Dictionary<string, double> newbeeScore = calculateScore(newbee, weights);


        Dictionary<string,double> topSuper = selectTop(superScore, numOfCandidates);
        Dictionary<string, double> topRegular = selectTop(regularScore, numOfCandidates);
        Dictionary<string, double> topNewbee = selectTop(newbeeScore, 1000);

        Dictionary<string, Candidate> topCandidates = new Dictionary<string, Candidate>();

        foreach (KeyValuePair<string, double> kv in topSuper) {
            candidates[kv.Key].Score = kv.Value;
            topCandidates.Add(kv.Key, candidates[kv.Key]);
            }
            
        foreach (KeyValuePair<string, double> kv in topRegular) {
            candidates[kv.Key].Score = kv.Value;
            topCandidates.Add(kv.Key, candidates[kv.Key]);
            }

        foreach (KeyValuePair<string, double> kv in topNewbee)
        {
            candidates[kv.Key].Score = kv.Value;
            topCandidates.Add(kv.Key, candidates[kv.Key]);
        }

        return topCandidates;
    }

    private Dictionary<string,double> selectTop(Dictionary<string, double> scores, int num) {
        var sortedDict = (from entry in scores orderby entry.Value descending select entry)
            .ToDictionary(pair => pair.Key, pair => pair.Value).Take(num);

        Dictionary<string, double> res = new Dictionary<string, double>();
        foreach (KeyValuePair<string, double> kv in sortedDict) {
            res.Add(kv.Key, kv.Value);
        }
        return res;
    }

    private Dictionary<string, double> calculateScore(Dictionary<string, Candidate> candidates, Dictionary<string, double> weights) {


        Dictionary<string, double> score = new Dictionary<string, double>();

        foreach (KeyValuePair<string, Candidate> kv in candidates)
        {
            Candidate c = kv.Value;
            int totalDrives = c.AmmountOfDisMatchByDay + c.AmmountOfMatchByDay; // I will use Mornings as the rightones
            double routeScore = Math.Log(c.AmmountOfPathMatch[0] + 1 ,2) * weights["otherAears"] +
                           Math.Log(c.AmmountOfPathMatch[1] + 1, 2) * weights["area2area"] +
                           Math.Log(c.AmmountOfPathMatch[2] + 1, 2) * weights["point2area"] +
                           Math.Log(c.AmmountOfPathMatch[3] + 1, 2) * weights["oposite"] +
                           Math.Log(c.AmmountOfPathMatch[4] + 1, 2) * weights["point2point"];

            double timeScore = Math.Log(c.AmmountOfMatchByDay + 1, 2) * weights["sameDay"] +
                               Math.Log(c.AmmountOfDisMatchByDay + 1, 2) * weights["DifferentDays"] +
                               Math.Log(c.AmmountOfMatchDayPart + 1, 2) * weights["SameDayPart"] +
                               Math.Log(c.AmmountOfDisMatchDayPart + 1, 2) * weights["DifferentDayPart"];
            routeScore = Math.Pow(routeScore, weights["routeWeight"]);
            timeScore  = Math.Pow(timeScore, weights["timeWeight"]);
            score.Add(kv.Key, routeScore * timeScore);
        }
        return score;
    }

    public Dictionary<string, Candidate> FillExtraDetails(Dictionary<string, Candidate> candidates)
    {
        HashSet<string> ids = candidates.Select(i => i.Key).ToHashSet();
        string query = "DECLARE @CandidatesIds [IntList] INSERT INTO @CandidatesIds VALUES";

        foreach (string id in ids)
        {
            query += "(" + id + ")";
            query += ids.Last() != id ? "," : "";
        }

        query += "exec spGetCandidatesDetails @IDs=@CandidatesIds";

        try
        {
            dbs = new DbService();
            SqlDataReader sdr = dbs.GetDataReader(query);
            string idPointer;

            while (sdr.Read())
            {
                idPointer = Convert.ToString(sdr["Id"]);
                candidates[idPointer].CellPhone = Convert.ToString(sdr["CellPhone"]);

                candidates[idPointer].City = String.IsNullOrEmpty(sdr["CityCityName"].ToString()) ? null :
                                                          Convert.ToString(sdr["CityCityName"]);

                candidates[idPointer].DaysSinceLastRide = String.IsNullOrEmpty(sdr["DaysSinceLastRide"].ToString()) ? null :
                                                         (int?) Convert.ToInt32(sdr["DaysSinceLastRide"]);
                
                candidates[idPointer].NumOfRides_last2Months = Convert.ToInt32(sdr["NumOfRides_last2Months"]);
                
                candidates[idPointer].DaysUntilNextRide = String.IsNullOrEmpty(sdr["DaysUntilNextRide"].ToString()) ? null :
                                                          (int?)Convert.ToInt32(sdr["DaysUntilNextRide"]);
                
                candidates[idPointer].LatestDocumentedCallDate = String.IsNullOrEmpty(sdr["LatestDocumentedCallDate"].ToString()) ? null :
                                                                 (DateTime?)Convert.ToDateTime(sdr["LatestDocumentedCallDate"].ToString());
                
                candidates[idPointer].SeniorityInYears = Convert.ToDouble(sdr["SeniorityInYears"]);
            }

            return candidates;
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
 