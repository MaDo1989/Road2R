using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for Convertions
/// </summary>
public static class Convertions
{
    public static Constants.Enums.Gender ConvertStringToGender(string genderAsString)
    {
        Constants.Enums.Gender result;

        switch (genderAsString)
        {
            case Constants.Strings.Male:
                result = Constants.Enums.Gender.Male;
                break;

            case Constants.Strings.Female:
                result = Constants.Enums.Gender.Female;
                break;

            default:
                result = Constants.Enums.Gender.Empty;
                break;
        }


        return result;
    }

    public static Constants.Enums.PatientStatus ConvertStringToPatientStatus(string patientStatus)
    {
        Constants.Enums.PatientStatus result;
        switch (patientStatus)
        {
            case Constants.Strings.Finished:
                result = Constants.Enums.PatientStatus.Finished;
                break;

            case Constants.Strings.NotFinished:
                result = Constants.Enums.PatientStatus.NotFinished;
                break;

            default:
                result = Constants.Enums.PatientStatus.Empty;
                break;
        }

        return result;
    }
}