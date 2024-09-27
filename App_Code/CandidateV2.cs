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

    private void calcScore()
    {
        if (this.Vtype=="NEWBIS")
        {
            float C_NoOfDocumentedRides = 3f;
            float C_SeniorityInYears = 0.1f;
            float C_LastRideInDays = 0.02f;
            float C_NextRideInDays = 0.02f;
            float C_NumOfRidesLast2Month = 3.5f;
            float C_AmountOfRidesInThisPath = 5f;
            float C_AmountOfRidesInOppositePath = 4f;
            float C_AmountOfRides_OriginToArea = 3.5f;
            float C_AmountOfRidesAtThisTime = 4.5f;
            float C_AmountOfRidesAtThisDayWeek = 5f;
            float C_AmountOfRidesFromRegionToDest = 3.5f;
            float C_SumOfKM = 100f;

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
            res += (this.SumOfKM != null && this.SumOfKM.Value > 0 && C_SumOfKM != 0) ? (float)((Math.Log(this.SumOfKM.Value) / C_SumOfKM) * -1): 0;
            this.Score = res;
        }
        else if (this.Vtype=="REGULAR")
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
            float C_AmountOfRidesFromRegionToDest = 0.1f;
            float C_SumOfKM = 0.15f;

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
            res += (this.SumOfKM != null && this.SumOfKM.Value > 0 && C_SumOfKM != 0) ? (float)((Math.Log(this.SumOfKM.Value) / C_SumOfKM) * -1) : 0;
            this.Score = res;
        }
        else if (this.Vtype=="SUPER")
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
            float C_AmountOfRidesFromRegionToDest = 0.1f;
            float C_SumOfKM = 0.015f;

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
            res += (this.SumOfKM != null && this.SumOfKM.Value > 0 && C_SumOfKM != 0) ? (float)((Math.Log(this.SumOfKM.Value) / C_SumOfKM) * -1) : 0;
            this.Score = res;
        }





    }



    
}