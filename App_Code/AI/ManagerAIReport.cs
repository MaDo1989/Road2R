using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for ManagerAIReport
/// </summary>
public class ManagerAIReport
{
    public ManagerAIReport()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public List<object> GetAIReport(string query,List<string> tableNames)
    {
        DBstracture dbstracture = new DBstracture();
        var cols = dbstracture.GetStractureFromDB(tableNames);
    } 
}