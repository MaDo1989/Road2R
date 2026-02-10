using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for Execution
/// </summary>
public class Execution
{
    public string UserPhone { get; set; }
    public string UserName { get; set; }
    public DateTime ExecutionTime { get; set; }
    public string SqlQuery { get; set; }
    public string AiDescription { get; set; }
    public string UserPrompt { get; set; }
    public string RelatedTables { get; set; }


    public Execution()
    {

    }

    public Execution(string userPhone, string userName, DateTime executionTime, string sqlQuery, string aiDescription, string userPrompt, string relatedTables)
    {
        UserPhone = userPhone;
        UserName = userName;
        ExecutionTime = executionTime;
        SqlQuery = sqlQuery;
        AiDescription = aiDescription;
        UserPrompt = userPrompt;
        RelatedTables = relatedTables;
    }
    static public int InsertExec(Execution exec)
    {
        if (exec !=null)
        {
            DBservice_Gilad dbs = new DBservice_Gilad();
            return dbs.SaveExecution(exec);
            
        }
        else
        {
            return -2;
        }
    }
}