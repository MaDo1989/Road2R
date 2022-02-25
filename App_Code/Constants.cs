using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for Constants
/// </summary>
public static class Constants
{

    public class Candidate
    {
        public static readonly int NumOfDaysToThePast = 180;
        public static readonly int NUmOfDaysToTheFuture = 0;
        public static readonly int NumOfDaysToThePast_CheckRides_Regular = 5;
        public static readonly int NumOfDaysToTheFuture_CheckRides_Regular = 5;
        public static readonly int NumOfDaysToThePast_CheckRides_Super = 1;
        public static readonly int NumOfDaysToTheFuture_CheckRides_Super = 1;
        public static readonly int AmountBottomLimitToBeSuperUserDriver = 60;
    }
}