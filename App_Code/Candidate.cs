using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for Candidate
/// </summary>
public class Candidate: Volunteer
{
    public Candidate()
    {

    }
    public Candidate(int id, string displayName, bool isSuperDriver, List<int> ammountOfPathMatch, int ammountOfMatchByDay, int ammountOfDissMatchByDay) : base(id, displayName)
    {
        IsSuperDriver = isSuperDriver;
        AmmountOfPathMatch = ammountOfPathMatch;
        AmmountOfMatchByDay = ammountOfMatchByDay;
        AmmountOfDissMatchByDay = ammountOfDissMatchByDay;
    }

    public bool IsSuperDriver { get; set; }
    public List<int> AmmountOfPathMatch { get; set; }
    public int AmmountOfMatchByDay { get; set; }
    public int AmmountOfDissMatchByDay { get; set; }

    
}