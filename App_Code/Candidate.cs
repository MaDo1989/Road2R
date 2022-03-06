using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for Candidate
/// </summary>
public class Candidate : Volunteer
{
    public Candidate() { }
    public Candidate(int id, string displayName, int driverLevel, List<int> ammountOfPathMatch,
                     int ammountOfMatchByDay, int ammountOfDisMatchByDay,
                     int ammountOfMatchDayPart, int ammountOfDisMatchDayPart
                    ) : base(id, displayName)
    {
        DriverLevel = driverLevel;
        AmmountOfPathMatch = ammountOfPathMatch;
        AmmountOfMatchByDay = ammountOfMatchByDay;
        AmmountOfDisMatchByDay = ammountOfDisMatchByDay;
        AmmountOfMatchDayPart = ammountOfMatchDayPart;
        AmmountOfDisMatchDayPart = ammountOfDisMatchDayPart;
    }


    public int DriverLevel { get; set; }
    public double Score { get; set; }
    public bool IsSuperDriver { get; set; }
    public List<int> AmmountOfPathMatch { get; set; }
    public int AmmountOfMatchByDay { get; set; }
    public int AmmountOfDisMatchByDay { get; set; }
    public int AmmountOfMatchDayPart { get; set; }
    public int AmmountOfDisMatchDayPart { get; set; }
    public int DaysSinceLastRide { get; set; }
    public int? DaysUntilNextRide { get; set; }
    public DateTime? LatestDocumentedCallDate { get; set; }
    public double SeniorityInYears { get; set; }

}