using System;
using System.Collections.Generic;
using System.Data.SqlClient;

/// <summary>
/// this class is for GET the logs from the table LogTable_AutoTrackChanges
/// and its records inserts to it via triggers in the app DB 
/// </summary>
public class System_Log
{
    public int Id { get; set; }
    public string WhoChanged { get; set; }
    public string TableName { get; set; }
    public int Recorde_UniqueId { get; set; }
    public string ColumnName { get; set; }
    public string OldValue { get; set; }
    public DateTime OldValue_AsDate { get; set; }
    public string NewValue { get; set; }
    public DateTime NewValue_AsDate { get; set; }
    public DateTime DateAdded { get; set; } // YYYY-MM-DD
    public TimeSpan TimeAdded { get; set; } // HH:MM:SS

    string query;
    DbService dbs;
    bool valueIsDate;
    DateTime temp4values_asDate;

    public List<System_Log> GetLogs(string timeRange)
    {
        /*day week month year*/

        query = "exec spLogTable_AutoTrackChanges_GetLogs @timeRange='" + timeRange + "'";
        try
        {
            dbs = new DbService();
            List<System_Log> logs = new List<System_Log>();
            SqlDataReader sdr = dbs.GetDataReader(query);
            System_Log log;
            while (sdr.Read())
            {
                log = new System_Log();
                log.Id = Convert.ToInt32(sdr["Id"]);
                log.WhoChanged = Convert.ToString(sdr["WhoChanged"]);
                log.TableName = Convert.ToString(sdr["TableName"]);
                log.Recorde_UniqueId = Convert.ToInt32(sdr["Recorde_UniqueId"]);
                log.ColumnName = Convert.ToString(sdr["ColumnName"]);

                valueIsDate = DateTime.TryParse(Convert.ToString(sdr["OldValue"]), out temp4values_asDate);
                if (valueIsDate) { log.OldValue_AsDate = temp4values_asDate; }
                else log.OldValue = Convert.ToString(sdr["OldValue"]);

                valueIsDate = DateTime.TryParse(Convert.ToString(sdr["NewValue"]), out temp4values_asDate);
                if (valueIsDate) { log.NewValue_AsDate = temp4values_asDate; }
                else log.NewValue = Convert.ToString(sdr["NewValue"]);

                log.DateAdded = Convert.ToDateTime(Convert.ToString(sdr["DateAdded"]));
                log.TimeAdded = TimeSpan.Parse(Convert.ToString(sdr["TimeAdded"]));

                logs.Add(log);
            }
            return logs;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
        finally
        {
            dbs.CloseConnection();
        }
    }
}