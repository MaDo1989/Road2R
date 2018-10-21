﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using log4net;

public class LogEntry
{
    DateTime date;
    string severity;
    string message;
    int code;

    public DateTime Date { get; set; }
    public string Severity { get; set; }
    public string Message { get; set; }
    public int Code { get; set; }

    private static readonly ILog Log =
         LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    public LogEntry()
    {

    }
    public LogEntry(DateTime date, string severity, string message, int code)
    {
        Date = date;
        Severity = severity;
        Message = message;
        Code = code;
    }

    //public void Write(string str)
    //{
    //    Log.Error(str);
    //}

    public void Write()
    {
        try
        {
            DbService db = new DbService();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.Text;
            SqlParameter[] cmdParams = new SqlParameter[4];

            cmdParams[0] = cmd.Parameters.AddWithValue("@date", Date);
            cmdParams[1] = cmd.Parameters.AddWithValue("@severity", Severity);
            cmdParams[2] = cmd.Parameters.AddWithValue("@message", Message);
            cmdParams[3] = cmd.Parameters.AddWithValue("@code", Code);
            string query = "insert into [LogTable] ([date],[severity],[message],[code]) values (@date,@severity,@message,@code);";
            db.ExecuteQuery(query, cmd.CommandType, cmdParams);
        }
        catch (Exception)
        {

        }
    }

    //returns list of logs from db
    public List<LogEntry> GetLog(int hours)
    {
        List<LogEntry> el = new List<LogEntry>();
        string query = "";

        query = "select * from LogTable where dbo.LogTable.date >= DATEADD(HOUR, -" + hours.ToString() + ", GETDATE())";
        DbService db = new DbService();
        DataSet ds = db.GetDataSetByQuery(query);

        foreach (DataRow dr2 in ds.Tables[0].Rows)
        {
            LogEntry le = new LogEntry();
            le.Code = int.Parse(dr2["code"].ToString());
            le.Severity = (string)dr2["severity"].ToString();
            le.Message = (string)dr2["message"].ToString();
            le.Date = (DateTime)dr2["date"];

            el.Add(le);
        }

        return el;
    }
}