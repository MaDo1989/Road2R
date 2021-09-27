using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for candidatesLogic
/// </summary>
public class candidatesLogic
{

    List<Candidate> superDriversList = new List<Candidate>();
    List<Candidate> regularDriversList = new List<Candidate>();
    List<Candidate> newDriversList = new List<Candidate>();

    public List<Candidate> SuperDriversList
    {
        get
        {
            return superDriversList;
        }

        set
        {
            superDriversList = value;
        }
    }

    public List<Candidate> RegularDriversList
    {
        get
        {
            return regularDriversList;
        }

        set
        {
            regularDriversList = value;
        }
    }

    public List<Candidate> NewDriversList
    {
        get
        {
            return newDriversList;
        }

        set
        {
            newDriversList = value;
        }
    }


    public candidatesLogic()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public candidatesLogic getCandidates(int ridePatNum) {
        candidatesLogic cl = new candidatesLogic();
        Candidate c = new Candidate();
        List<Candidate> canList = c.GetCandidates(ridePatNum);
        foreach (Candidate can in canList) {
            if (can.IsSuperUser)
                cl.SuperDriversList.Add(can);
            else
                cl.RegularDriversList.Add(can);
        }
        return cl;
    }
}