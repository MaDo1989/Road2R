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
        public static readonly int NumOfDaysToThePast_CheckRides_Super = 2;
        public static readonly int NumOfDaysToTheFuture_CheckRides_Super = 2;
        public static readonly int AmountBottomLimitToBeSuperUserDriver = 60;
        public static readonly int AmountOfRidesInNewDriverTimeWindow = 3;
        public static readonly int NewDriverTimeWindow = 90;
    }

    public class TimeUnits
    {
        public static readonly int DaysInAyear = 365;
    }

    public class Enums
    {
        public enum Gender
        {
            Empty = -1,
            Female = 0,
            Male = 1
        }

        public enum PatientStatus
        {
            Empty = -1,
            NotFinished = 0,
            Finished = 1
        }
    }

    public class Strings
    {
        public const string Male = "חולֶה";
        public const string Female = "חולָה";
        public const string Finished = "Finished";
        public const string NotFinished = "Not Finished";

    }
}