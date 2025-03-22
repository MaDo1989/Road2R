using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for CandidateV3: this class is the candidate class for the unity ride v3
/// 
/// </summary>
public class CandidateV3
{
    int id;
    string displayName;
    string cellPhone;
    DateTime joinDate;
    string cityCityName;
    int? availableSeats;
    int noOfDocumentedRides;
    DateTime? lastCallDateTime;
    int? lastRideInDays;
    int? nextRideInDays;
    float avgRidesPerWeekLast6Months;
    int amountOfRidesInThisPath;
    int amountOfRidesAreaToArea;
    int amountOfRidesOriginToArea;
    int amountOfRidesAtThisTime;
    int amountOfRidesAtThisDayWeek;
    int noOfDocumentedCalls;
    float lastRideinWeeks;
    
    float percentageOfRidesInThisPath;
    float percentageOfRidesAreaToArea;
    float percentageOfRidesOriginToArea;
    float percentageOfRidesAtThisTime;
    float percentageOfRidesAtThisDayWeek;
    
    float score;


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

    public string DisplayName
    {
        get
        {
            return displayName;
        }

        set
        {
            displayName = value;
        }
    }

    public string CellPhone
    {
        get
        {
            return cellPhone;
        }

        set
        {
            cellPhone = value;
        }
    }

    public DateTime JoinDate
    {
        get
        {
            return joinDate;
        }

        set
        {
            joinDate = value;
        }
    }

    public string CityCityName
    {
        get
        {
            return cityCityName;
        }

        set
        {
            cityCityName = value;
        }
    }

    public int? AvailableSeats
    {
        get
        {
            return availableSeats;
        }

        set
        {
            availableSeats = value;
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

    public DateTime? LastCallDateTime
    {
        get
        {
            return lastCallDateTime;
        }

        set
        {
            lastCallDateTime = value;
        }
    }

    public int? LastRideInDays
    {
        get
        {
            return lastRideInDays;
        }

        set
        {
            lastRideInDays = value;
        }
    }

    public int? NextRideInDays
    {
        get
        {
            return nextRideInDays;
        }

        set
        {
            nextRideInDays = value;
        }
    }

    public float AvgRidesPerWeekLast6Months
    {
        get
        {
            return avgRidesPerWeekLast6Months;
        }

        set
        {
            avgRidesPerWeekLast6Months = value;
        }
    }

    public int AmountOfRidesInThisPath
    {
        get
        {
            return amountOfRidesInThisPath;
        }

        set
        {
            amountOfRidesInThisPath = value;
        }
    }

    public int AmountOfRidesAreaToArea
    {
        get
        {
            return amountOfRidesAreaToArea;
        }

        set
        {
            amountOfRidesAreaToArea = value;
        }
    }

    public int AmountOfRidesOriginToArea
    {
        get
        {
            return amountOfRidesOriginToArea;
        }

        set
        {
            amountOfRidesOriginToArea = value;
        }
    }

    public int AmountOfRidesAtThisTime
    {
        get
        {
            return amountOfRidesAtThisTime;
        }

        set
        {
            amountOfRidesAtThisTime = value;
        }
    }

    public int AmountOfRidesAtThisDayWeek
    {
        get
        {
            return amountOfRidesAtThisDayWeek;
        }

        set
        {
            amountOfRidesAtThisDayWeek = value;
        }
    }

    public int NoOfDocumentedCalls
    {
        get
        {
            return noOfDocumentedCalls;
        }

        set
        {
            noOfDocumentedCalls = value;
        }
    }

    public float Score
    {
        get
        {
            return score;
        }

        set
        {
            score = value;
        }
    }

    public float PercentageOfRidesInThisPath
    {
        get
        {
            return percentageOfRidesInThisPath;
        }

        set
        {
            percentageOfRidesInThisPath = value;
        }
    }

    public float PercentageOfRidesAreaToArea
    {
        get
        {
            return percentageOfRidesAreaToArea;
        }

        set
        {
            percentageOfRidesAreaToArea = value;
        }
    }

    public float PercentageOfRidesOriginToArea
    {
        get
        {
            return percentageOfRidesOriginToArea;
        }

        set
        {
            percentageOfRidesOriginToArea = value;
        }
    }

    public float PercentageOfRidesAtThisTime
    {
        get
        {
            return percentageOfRidesAtThisTime;
        }

        set
        {
            percentageOfRidesAtThisTime = value;
        }
    }

    public float PercentageOfRidesAtThisDayWeek
    {
        get
        {
            return percentageOfRidesAtThisDayWeek;
        }

        set
        {
            percentageOfRidesAtThisDayWeek = value;
        }
    }

    public float LastRideinWeeks
    {
        get
        {
            return lastRideinWeeks;
        }

        set
        {
            lastRideinWeeks = value;
        }
    }





    private List<CandidateV3> getCandidateFromDB_Raw(int ridepatnum)
    {
        DBservice_Gilad db = new DBservice_Gilad();
        List<CandidateV3> candidateList = db.GetCandidateUnityRideV3(ridepatnum);
        return candidateList;

    }

    public List<CandidateV3> GetCandidatesWithCalc(int ridepatnum)
    {
        List<CandidateV3> candidateList = getCandidateFromDB_Raw(ridepatnum);
        List<ScoreConfigDic> scoreDictionary = ScoreConfigDic.GetAllScoreConfigDictionary();

        List<ScoreConfigDic> p2pList = scoreDictionary.Where(x => x.Parameter == "point_to_point").ToList();
        List<ScoreConfigDic> a2aList = scoreDictionary.Where(x => x.Parameter == "area_to_area").ToList();
        List<ScoreConfigDic> p2aList = scoreDictionary.Where(x => x.Parameter == "point_to_area").ToList();
        List<ScoreConfigDic> dayWeekList = scoreDictionary.Where(x => x.Parameter == "this_day_week").ToList();
        List<ScoreConfigDic> timeInDayList = scoreDictionary.Where(x => x.Parameter == "this_time_inDay").ToList();

        List<ScoreConfigDic> timeFromLastRide = scoreDictionary.Where(x => x.Parameter == "Time_since_last_ride").ToList();
        List<ScoreConfigDic> AVG_rides_week = scoreDictionary.Where(x => x.Parameter == "AVG_rides_week").ToList();
        List<ScoreConfigDic> is_future_Ride = scoreDictionary.Where(x => x.Parameter == "is_future_Ride").ToList();

        foreach (var candid in candidateList)
        {
            candid.Score = 0;
            if (candid.NoOfDocumentedRides>0)
            {
                candid.PercentageOfRidesInThisPath = (float)candid.AmountOfRidesInThisPath / candid.NoOfDocumentedRides;
                candid.PercentageOfRidesAreaToArea = (float)candid.AmountOfRidesAreaToArea / candid.NoOfDocumentedRides;
                candid.PercentageOfRidesOriginToArea = (float)candid.AmountOfRidesOriginToArea / candid.NoOfDocumentedRides;
                candid.PercentageOfRidesAtThisTime = (float)candid.AmountOfRidesAtThisTime / candid.NoOfDocumentedRides;
                candid.PercentageOfRidesAtThisDayWeek = (float)candid.AmountOfRidesAtThisDayWeek / candid.NoOfDocumentedRides;

                candid.Score += GetAccurateScoringByPercentage(candid.PercentageOfRidesInThisPath, p2pList);
                candid.Score += GetAccurateScoringByPercentage(candid.PercentageOfRidesAreaToArea, a2aList);
                candid.Score += GetAccurateScoringByPercentage(candid.PercentageOfRidesOriginToArea, p2aList);
                candid.Score += GetAccurateScoringByPercentage(candid.PercentageOfRidesAtThisTime, timeInDayList);
                candid.Score += GetAccurateScoringByPercentage(candid.PercentageOfRidesAtThisDayWeek, dayWeekList);
            }

            if (candid.LastRideInDays!=null)
            {
                candid.LastRideinWeeks = (float)candid.LastRideInDays / 7;

            }



            candid.Score += GetScoringByValue(candid.LastRideinWeeks, timeFromLastRide);
            candid.Score += GetScoringByValue(candid.AvgRidesPerWeekLast6Months, AVG_rides_week);

            if (candid.NextRideInDays != null)
            {
                candid.Score += is_future_Ride[0].Score;
            }
            else
            {
                candid.Score += is_future_Ride[1].Score;
            }



        }

        return candidateList;

    }


    //this method will return the accurate score for the percentage for example:
    // percentage = 0.5 and the range is 0.4-0.6 and the score of this range is 3 so his accurate score will be
    // 3 + (0.5-0.4)/(0.6-0.4) * (3-1) = 4 (the 1 is the score of range 0.2-0.4)
    private float GetAccurateScoringByPercentage(float percentage, List<ScoreConfigDic> paramsList)
    {
        float AccurateScore = 0;
        float theScoreOfOneRowBefore = 0;
        foreach (var row in paramsList)
        {
            if (percentage >= row.MinRangeValue && percentage <= row.MaxRangeValue)
            {
                AccurateScore = row.Score + ((percentage - row.MinRangeValue) / (row.MaxRangeValue - row.MinRangeValue)) * row.Score; //(row.Score - theScoreOfOneRowBefore);
                return AccurateScore;
            }
            theScoreOfOneRowBefore = row.Score;
        }
        return AccurateScore;

    }

    private float GetScoringByValue(float value, List<ScoreConfigDic> paramsList)
    {
        
        foreach (var config in paramsList)
        {
            if (value >= config.MinRangeValue && value<=config.MaxRangeValue)
            {
                return config.Score;
            }
        }
        throw new Exception("value is not in any range, sonthing goes wrong in GetScoringByValue CandidateV3");

    }
}


