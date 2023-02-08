using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for Calculations
/// </summary>
public static class Calculations
{
    public static int? CalculateAge(DateTime? dateOfBirth)
    {
        int? age = null;
        DateTime now = DateTime.Now;
        if (dateOfBirth.HasValue)
        {
            DateTime currentDate = DateTime.Now;
            age = currentDate.Year - dateOfBirth.Value.Year;
        }

        return age;
    }
}
