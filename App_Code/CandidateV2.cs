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

    public int? LastRideInDays { get; set; }
    public int? NextRideInDays { get; set; }
    public int? NumOfRidesLast2Month { get; set; }
    public int? AmountOfRidesInThisPath { get; set; }
    public int? AmountOfRidesInOppositePath { get; set; }
    public int? AmountOfRides_OriginToArea { get; set; }
    public int? AmountOfRidesAtThisTime { get; set; }
    public int? AmountOfRidesAtThisDayWeek { get; set; }
    public float? SumOfKM { get; set; }
    public float? Score { get; set; }


    public CandidateV2()
    {

    }

    public List<CandidateV2> GetCandidateUnityRideV2(int rideNum,int mode)
    {
        DBservice_Gilad DB = new DBservice_Gilad();
        List<CandidateV2> list = DB.GetCandidateUnityRideV2(rideNum,mode);
        foreach (CandidateV2 c in list)
        {
            c.calcScore(mode);
        }
        return list;


        //------------DECLARE @C_NoOfDocumentedRides FLOAT = 0.33;
        //------------DECLARE @C_SeniorityInYears FLOAT = 0.01;
        //------------DECLARE @C_LastRideInDays FLOAT = 0.2;
        //------------DECLARE @C_NextRideInDays FLOAT = 0.2;
        //------------DECLARE @C_NumOfRidesLast2Month FLOAT = 0.33;
        //------------DECLARE @C_AmountOfRidesInThisPath FLOAT = 0.5;
        //------------DECLARE @C_AmountOfRidesInOppositePath FLOAT = 0.25;

        //------------DECLARE @C_AmountOfRides_OriginToArea FLOAT = 0.33;
        //------------DECLARE @C_AmountOfRidesAtThisTime FLOAT = 0.33;
        //------------DECLARE @C_AmountOfRidesAtThisDayWeek FLOAT = 0.2;
        //------------DECLARE @C_SumOfKM FLOAT = 2.5
    }

    private void calcScore(int mode)
    {
        if (mode==0)
        {
            float C_NoOfDocumentedRides = 0.03f;
            float C_SeniorityInYears = 0.01f;
            float C_LastRideInDays = 0.002f;
            float C_NextRideInDays = 0.002f;
            float C_NumOfRidesLast2Month = 0.033f;
            float C_AmountOfRidesInThisPath = 0.5f;
            float C_AmountOfRidesInOppositePath = 0.025f;
            float C_AmountOfRides_OriginToArea = 0.033f;
            float C_AmountOfRidesAtThisTime = 0.1f;
            float C_AmountOfRidesAtThisDayWeek = 0.1f;
            float C_SumOfKM = 500.5f;

            float res = C_NoOfDocumentedRides * (this.NoOfDocumentedRides ?? 0);
            res += C_SeniorityInYears * (this.SeniorityInYears ?? 0);
            res += C_LastRideInDays * (this.LastRideInDays ?? 0);
            res += C_NextRideInDays * (this.NextRideInDays ?? 0);
            res += C_NumOfRidesLast2Month * (this.NumOfRidesLast2Month ?? 0);
            res += C_AmountOfRidesInThisPath * (this.AmountOfRidesInThisPath ?? 0);
            res += C_AmountOfRidesInOppositePath * (this.AmountOfRidesInOppositePath ?? 0);
            res += C_AmountOfRides_OriginToArea * (this.AmountOfRides_OriginToArea ?? 0);
            res += C_AmountOfRidesAtThisTime * (this.AmountOfRidesAtThisTime ?? 0);
            res += C_AmountOfRidesAtThisDayWeek * (this.AmountOfRidesAtThisDayWeek ?? 0);
            res += C_SumOfKM * ((1 / this.SumOfKM) ?? 0);
            this.Score = res;
        }
        else if (mode == 1)
        {
            float C_NoOfDocumentedRides = 0.303f;
            float C_SeniorityInYears = 0.15f;
            float C_LastRideInDays = 0.02f;
            float C_NextRideInDays = 0.2f;
            float C_NumOfRidesLast2Month = 2.33f;
            float C_AmountOfRidesInThisPath = 6.5f;
            float C_AmountOfRidesInOppositePath = 2.0025f;
            float C_AmountOfRides_OriginToArea = 1.4033f;
            float C_AmountOfRidesAtThisTime = 0.91f;
            float C_AmountOfRidesAtThisDayWeek = 0.31f;
            float C_SumOfKM = 0.15f;

            float res = C_NoOfDocumentedRides * (this.NoOfDocumentedRides ?? 0);
            res += C_SeniorityInYears * (this.SeniorityInYears ?? 0);
            res += C_LastRideInDays * (this.LastRideInDays ?? 0);
            res += C_NextRideInDays * (this.NextRideInDays ?? 0);
            res += C_NumOfRidesLast2Month * (this.NumOfRidesLast2Month ?? 0);
            res += C_AmountOfRidesInThisPath * (this.AmountOfRidesInThisPath ?? 0);
            res += C_AmountOfRidesInOppositePath * (this.AmountOfRidesInOppositePath ?? 0);
            res += C_AmountOfRides_OriginToArea * (this.AmountOfRides_OriginToArea ?? 0);
            res += C_AmountOfRidesAtThisTime * (this.AmountOfRidesAtThisTime ?? 0);
            res += C_AmountOfRidesAtThisDayWeek * (this.AmountOfRidesAtThisDayWeek ?? 0);
            res += C_SumOfKM * ((1 / this.SumOfKM) ?? 0);
            this.Score = res;
        }
        else if (mode ==2)
        {
            float C_NoOfDocumentedRides = 0.303f;
            float C_SeniorityInYears = 0.15f;
            float C_LastRideInDays = 0.02f;
            float C_NextRideInDays = 0.2f;
            float C_NumOfRidesLast2Month = 2.33f;
            float C_AmountOfRidesInThisPath = 8.5f;
            float C_AmountOfRidesInOppositePath = 3.0025f;
            float C_AmountOfRides_OriginToArea = 2.4033f;
            float C_AmountOfRidesAtThisTime = 0.91f;
            float C_AmountOfRidesAtThisDayWeek = 0.31f;
            float C_SumOfKM = 0.015f;

            float res = C_NoOfDocumentedRides * (this.NoOfDocumentedRides ?? 0);
            res += C_SeniorityInYears * (this.SeniorityInYears ?? 0);
            res += C_LastRideInDays * (this.LastRideInDays ?? 0);
            res += C_NextRideInDays * (this.NextRideInDays ?? 0);
            res += C_NumOfRidesLast2Month * (this.NumOfRidesLast2Month ?? 0);
            res += C_AmountOfRidesInThisPath * (this.AmountOfRidesInThisPath ?? 0);
            res += C_AmountOfRidesInOppositePath * (this.AmountOfRidesInOppositePath ?? 0);
            res += C_AmountOfRides_OriginToArea * (this.AmountOfRides_OriginToArea ?? 0);
            res += C_AmountOfRidesAtThisTime * (this.AmountOfRidesAtThisTime ?? 0);
            res += C_AmountOfRidesAtThisDayWeek * (this.AmountOfRidesAtThisDayWeek ?? 0);
            res += C_SumOfKM * ((1 / this.SumOfKM) ?? 0);
            this.Score = res;
        }





    }



    
}