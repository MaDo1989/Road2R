using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for RidePatPatientStatus
/// </summary>
public class RidePatPatientStatus
{
    public DateTime? EditTimeStamp { get; set; }
    public Constants.Enums.PatientStatus Status { get; set; }

}