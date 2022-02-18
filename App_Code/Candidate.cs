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
    public Candidate(int id, string displayName, bool isSuperDriver, List<int> ammountOfPathMatch,
                     int ammountOfMatchByDay, int ammountOfDissMatchByDay,
                     int ammountOfAfterNoonRides, int ammountOfMorningRides
                    ) : base(id, displayName)
    {
        IsSuperDriver = isSuperDriver;
        AmmountOfPathMatch = ammountOfPathMatch;
        AmmountOfMatchByDay = ammountOfMatchByDay;
        AmmountOfDissMatchByDay = ammountOfDissMatchByDay;
        AmmountOfAfterNoonRides = ammountOfAfterNoonRides;
        AmmountOfMorningRides = ammountOfMorningRides;
    }

    public bool IsSuperDriver { get; set; }
    public List<int> AmmountOfPathMatch { get; set; }
    public int AmmountOfMatchByDay { get; set; }
    public int AmmountOfDissMatchByDay { get; set; }
    public int AmmountOfAfterNoonRides { get; set; }
    public int AmmountOfMorningRides { get; set; }
    public int DaysSinceLastRide { get; set; }
    public int? DaysUntilNextRide { get; set; }
    public DateTime? LatestDocumentedCallDate { get; set; }
    public double SeniorityInYears { get; set; }

}