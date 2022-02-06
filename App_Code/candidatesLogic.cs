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
    List<Candidate> newbies = new List<Candidate>();

    public List<Candidate> Newbies
    {
        get
        {
            return newbies;
        }

        set
        {
            newbies = value;
        }
    }

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
        List<Candidate> newbies = c.GetNewCandidates(60);
        canList.AddRange(newbies);

        foreach (Candidate can in canList) {
            if (can.IsSuperUser)
                cl.SuperDriversList.Add(can);
            if (can.IsNewbie)
                cl.Newbies.Add(can);
            else
                cl.RegularDriversList.Add(can);
        }
        return cl;
    }
     
}