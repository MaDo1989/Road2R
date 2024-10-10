using System;
using System.Collections.Generic;




public class CandidateV2
{
    public int? Id { get; set; }
    public string DisplayName { get; set; }
    public string CellPhone { get; set; }
    public string JoinDate { get; set; }
    public string CityCityName { get; set; }
    public int? AvailableSeats { get; set; }
    public int? NoOfDocumentedRides { get; set; }
    public float? SeniorityInYears { get; set; }
    public string LastCallDateTime { get; set; }

    public string Vtype { get; set; }
    public int? NoOfDocumentedCalls { get; set; }
    public int? LastRideInDays { get; set; }
    public int? NextRideInDays { get; set; }
    public int? NumOfRidesLast2Month { get; set; }
    public int? AmountOfRidesInThisPath { get; set; }
    public int? AmountOfRidesInOppositePath { get; set; }
    public int? AmountOfRides_OriginToArea { get; set; }
    public int? AmountOfRidesAtThisTime { get; set; }
    public int? AmountOfRidesAtThisDayWeek { get; set; }
    public int? AmountOfRidesFromRegionToDest { get; set; }
    public float? SumOfKM { get; set; }
    public float? Score { get; set; }

    public static List<float> NewbisWeights
    {
        get
        {
            return newbisWeights;
        }

        set
        {
            newbisWeights = value;
        }
    }

    public static List<float> RegularWeights
    {
        get
        {
            return regularWeights;
        }

        set
        {
            regularWeights = value;
        }
    }

    public static List<float> SuperWeights
    {
        get
        {
            return superWeights;
        }

        set
        {
            superWeights = value;
        }
    }

    private static List<float> newbisWeights = new List<float>
    {
        0f,    // C_NoOfDocumentedRides
        0.1f,  // C_SeniorityInYears
        0.02f, // C_LastRideInDays
        0.02f, // C_NextRideInDays
        3.5f,  // C_NumOfRidesLast2Month
        5f,    // C_AmountOfRidesInThisPath
        4f,    // C_AmountOfRidesInOppositePath
        3.5f,  // C_AmountOfRides_OriginToArea
        4.5f,  // C_AmountOfRidesAtThisTime
        5f,    // C_AmountOfRidesAtThisDayWeek
        3.5f,  // C_AmountOfRidesFromRegionToDest
        13f   // C_SumOfKM
    };
    private static List<float> regularWeights = new List<float>
    {
        0.5f,  // C_NoOfDocumentedRides
        0.25f, // C_SeniorityInYears
        0.1f,  // C_LastRideInDays
        0.1f,  // C_NextRideInDays
        0.5f,  // C_NumOfRidesLast2Month
        7.5f,  // C_AmountOfRidesInThisPath
        5.5f,    // C_AmountOfRidesInOppositePath
        3f,    // C_AmountOfRides_OriginToArea
        5f,    // C_AmountOfRidesAtThisTime
        2.5f,  // C_AmountOfRidesAtThisDayWeek
        3f,    // C_AmountOfRidesFromRegionToDest
        3f   // C_SumOfKM
    };

    private static List<float> superWeights = new List<float>
    {
        0.5f,  // C_NoOfDocumentedRides
        0.25f, // C_SeniorityInYears
        0.1f,  // C_LastRideInDays
        0.1f,  // C_NextRideInDays
        0.5f,  // C_NumOfRidesLast2Month
        9.5f,  // C_AmountOfRidesInThisPath
        7f,    // C_AmountOfRidesInOppositePath
        3f,    // C_AmountOfRides_OriginToArea
        5f,    // C_AmountOfRidesAtThisTime
        2.5f,  // C_AmountOfRidesAtThisDayWeek
        3f,    // C_AmountOfRidesFromRegionToDest
        2f   // C_SumOfKM
    };


    public CandidateV2()
    {

    }

    public List<CandidateV2> GetCandidateUnityRideV2(int rideNum,int mode)
    {
        DBservice_Gilad DB = new DBservice_Gilad();
        List<CandidateV2> list = DB.GetCandidateUnityRideV2(rideNum,mode);
        foreach (CandidateV2 c in list)
        {
            c.calcScore();
        }
        return list;


    
    }

    public static void UpdateWeights(List<float> newbis_W, List<float> regular_W, List<float> super_W)
    {
        newbisWeights = newbis_W;
        regularWeights = regular_W;
        superWeights = super_W;
    }

    private void calcScore()
    {
        if (this.Vtype=="NEWBIS")
        {
            float C_NoOfDocumentedRides = newbisWeights[0];
            float C_SeniorityInYears = newbisWeights[1];
            float C_LastRideInDays = newbisWeights[2];
            float C_NextRideInDays = newbisWeights[3];
            float C_NumOfRidesLast2Month = newbisWeights[4];
            float C_AmountOfRidesInThisPath = newbisWeights[5];
            float C_AmountOfRidesInOppositePath = newbisWeights[6];
            float C_AmountOfRides_OriginToArea = newbisWeights[7];
            float C_AmountOfRidesAtThisTime = newbisWeights[8];
            float C_AmountOfRidesAtThisDayWeek = newbisWeights[9];
            float C_AmountOfRidesFromRegionToDest = newbisWeights[10];
            float C_SumOfKM = newbisWeights[11];

            float res = C_NoOfDocumentedRides * (this.NoOfDocumentedRides != null ? (float)Math.Log(this.NoOfDocumentedRides.Value + 1) : 0);
            res += C_SeniorityInYears * (this.SeniorityInYears != null ? (float)Math.Log(this.SeniorityInYears.Value + 1) : 0);
            res += C_LastRideInDays * (this.LastRideInDays != null ? (float)Math.Log(this.LastRideInDays.Value + 1) : (float)Math.Log(365));
            res += C_NextRideInDays * (this.NextRideInDays != null ? (float)Math.Log(this.NextRideInDays.Value + 1) : (float)Math.Log(365));
            res += C_NumOfRidesLast2Month * (this.NumOfRidesLast2Month != null ? (float)Math.Log(this.NumOfRidesLast2Month.Value + 1) : 0);
            res += C_AmountOfRidesInThisPath * (this.AmountOfRidesInThisPath != null ? (float)Math.Log(this.AmountOfRidesInThisPath.Value + 1) : 0);
            res += C_AmountOfRidesInOppositePath * (this.AmountOfRidesInOppositePath != null ? (float)Math.Log(this.AmountOfRidesInOppositePath.Value + 1) : 0);
            res += C_AmountOfRides_OriginToArea * (this.AmountOfRides_OriginToArea != null ? (float)Math.Log(this.AmountOfRides_OriginToArea.Value + 1) : 0);
            res += C_AmountOfRidesAtThisTime * (this.AmountOfRidesAtThisTime != null ? (float)Math.Log(this.AmountOfRidesAtThisTime.Value + 1) : 0);
            res += C_AmountOfRidesAtThisDayWeek * (this.AmountOfRidesAtThisDayWeek != null ? (float)Math.Log(this.AmountOfRidesAtThisDayWeek.Value + 1) : 0);
            res += C_AmountOfRidesFromRegionToDest * (this.AmountOfRidesFromRegionToDest != null ? (float)Math.Log(this.AmountOfRidesFromRegionToDest.Value + 1) : 0);
            res += (this.SumOfKM != null && this.SumOfKM.Value > 0 && C_SumOfKM != 0) ? (float)((Math.Log(this.SumOfKM.Value / 100) ) * (-1* C_SumOfKM)): 0;
            this.Score = res;
        }
        else if (this.Vtype=="REGULAR")
        {
            float C_NoOfDocumentedRides = regularWeights[0];
            float C_SeniorityInYears = regularWeights[1];
            float C_LastRideInDays = regularWeights[2];
            float C_NextRideInDays = regularWeights[3];
            float C_NumOfRidesLast2Month = regularWeights[4];
            float C_AmountOfRidesInThisPath = regularWeights[5];
            float C_AmountOfRidesInOppositePath = regularWeights[6];
            float C_AmountOfRides_OriginToArea = regularWeights[7];
            float C_AmountOfRidesAtThisTime = regularWeights[8];
            float C_AmountOfRidesAtThisDayWeek = regularWeights[9];
            float C_AmountOfRidesFromRegionToDest = regularWeights[10];
            float C_SumOfKM = regularWeights[11];

            float res = C_NoOfDocumentedRides * (this.NoOfDocumentedRides != null ? (float)Math.Log(this.NoOfDocumentedRides.Value + 1) : 0);
            res += C_SeniorityInYears * (this.SeniorityInYears != null ? (float)Math.Log(this.SeniorityInYears.Value + 1) : 0);
            res += C_LastRideInDays * (this.LastRideInDays != null ? (float)Math.Log(this.LastRideInDays.Value + 1) : (float)Math.Log(365));
            res += C_NextRideInDays * (this.NextRideInDays != null ? (float)Math.Log(this.NextRideInDays.Value + 1) : (float)Math.Log(365));
            res += C_NumOfRidesLast2Month * (this.NumOfRidesLast2Month != null ? (float)Math.Log(this.NumOfRidesLast2Month.Value + 1) : 0);
            res += C_AmountOfRidesInThisPath * (this.AmountOfRidesInThisPath != null ? (float)Math.Log(this.AmountOfRidesInThisPath.Value + 1) : 0);
            res += C_AmountOfRidesInOppositePath * (this.AmountOfRidesInOppositePath != null ? (float)Math.Log(this.AmountOfRidesInOppositePath.Value + 1) : 0);
            res += C_AmountOfRides_OriginToArea * (this.AmountOfRides_OriginToArea != null ? (float)Math.Log(this.AmountOfRides_OriginToArea.Value + 1) : 0);
            res += C_AmountOfRidesAtThisTime * (this.AmountOfRidesAtThisTime != null ? (float)Math.Log(this.AmountOfRidesAtThisTime.Value + 1) : 0);
            res += C_AmountOfRidesAtThisDayWeek * (this.AmountOfRidesAtThisDayWeek != null ? (float)Math.Log(this.AmountOfRidesAtThisDayWeek.Value + 1) : 0);
            res += C_AmountOfRidesFromRegionToDest * (this.AmountOfRidesFromRegionToDest != null ? (float)Math.Log(this.AmountOfRidesFromRegionToDest.Value + 1) : 0);
            res += (this.SumOfKM != null && this.SumOfKM.Value > 0 && C_SumOfKM != 0) ? (float)((Math.Log(this.SumOfKM.Value / 100) ) * (-1 * C_SumOfKM)) : 0;
            this.Score = res;
        }
        else if (this.Vtype=="SUPER")
        {
            float C_NoOfDocumentedRides = superWeights[0];
            float C_SeniorityInYears = superWeights[1];
            float C_LastRideInDays = superWeights[2];
            float C_NextRideInDays = superWeights[3];
            float C_NumOfRidesLast2Month = superWeights[4];
            float C_AmountOfRidesInThisPath = superWeights[5];
            float C_AmountOfRidesInOppositePath = superWeights[6];
            float C_AmountOfRides_OriginToArea = superWeights[7];
            float C_AmountOfRidesAtThisTime = superWeights[8];
            float C_AmountOfRidesAtThisDayWeek = superWeights[9];
            float C_AmountOfRidesFromRegionToDest = superWeights[10];
            float C_SumOfKM = superWeights[11];

            float res = C_NoOfDocumentedRides * (this.NoOfDocumentedRides != null ? (float)Math.Log(this.NoOfDocumentedRides.Value + 1) : 0);
            res += C_SeniorityInYears * (this.SeniorityInYears != null ? (float)Math.Log(this.SeniorityInYears.Value + 1) : 0);
            res += C_LastRideInDays * (this.LastRideInDays != null ? (float)Math.Log(this.LastRideInDays.Value + 1) : (float)Math.Log(365));
            res += C_NextRideInDays * (this.NextRideInDays != null ? (float)Math.Log(this.NextRideInDays.Value + 1) : (float)Math.Log(365));
            res += C_NumOfRidesLast2Month * (this.NumOfRidesLast2Month != null ? (float)Math.Log(this.NumOfRidesLast2Month.Value + 1) : 0);
            res += C_AmountOfRidesInThisPath * (this.AmountOfRidesInThisPath != null ? (float)Math.Log(this.AmountOfRidesInThisPath.Value + 1) : 0);
            res += C_AmountOfRidesInOppositePath * (this.AmountOfRidesInOppositePath != null ? (float)Math.Log(this.AmountOfRidesInOppositePath.Value + 1) : 0);
            res += C_AmountOfRides_OriginToArea * (this.AmountOfRides_OriginToArea != null ? (float)Math.Log(this.AmountOfRides_OriginToArea.Value + 1) : 0);
            res += C_AmountOfRidesAtThisTime * (this.AmountOfRidesAtThisTime != null ? (float)Math.Log(this.AmountOfRidesAtThisTime.Value + 1) : 0);
            res += C_AmountOfRidesAtThisDayWeek * (this.AmountOfRidesAtThisDayWeek != null ? (float)Math.Log(this.AmountOfRidesAtThisDayWeek.Value + 1) : 0);
            res += C_AmountOfRidesFromRegionToDest * (this.AmountOfRidesFromRegionToDest != null ? (float)Math.Log(this.AmountOfRidesFromRegionToDest.Value + 1) : 0);
            res += (this.SumOfKM != null && this.SumOfKM.Value > 0 && C_SumOfKM != 0) ? (float)((Math.Log(this.SumOfKM.Value / 100) ) * (-1 * C_SumOfKM)) : 0;
            this.Score = res;
        }





    }


    public static Tuple<List<float>, List<float>, List<float>> GetWeights()
    {
        return Tuple.Create(newbisWeights, regularWeights, superWeights);
    }


    //public static List<float> ConvertToLongListWeights() { 
    //    Tuple<List<float>, List<float>, List<float>> weights = GetWeights();
    //    List<float> res = new List<float>();
    //    res.AddRange(weights.Item1);
    //    res.AddRange(weights.Item2);
    //    res.AddRange(weights.Item3);
    //    return res;
    
    
    //}

}