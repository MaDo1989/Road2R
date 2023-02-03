using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for Calculations
/// </summary>
public static class Calculations
{
    public static double? CalculateAge(DateTime? dateOfBirth)
    {
        double? age = null;
        DateTime now = DateTime.Now;
        if (dateOfBirth.HasValue)
        {
            DateTime currentDate = DateTime.Now;
            TimeSpan elapsedTime = currentDate - dateOfBirth.Value;
            age = elapsedTime.TotalDays / Constants.TimeUnits.DaysInAyear;
        }

        return age;
    }
}
