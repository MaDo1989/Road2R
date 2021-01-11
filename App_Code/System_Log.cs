using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

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
    public string NewValue { get; set; }
    public DateTime DateAdded { get; set; } // YYYY-MM-DD
    public TimeSpan TimeAdded { get; set; } // HH:MM:SS

    string query;
    DbService dbs;

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
                log.OldValue = Convert.ToString(sdr["OldValue"]);
                log.NewValue = Convert.ToString(sdr["NewValue"]);
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