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
        const string query = "exec spLogTable_AutoTrackChanges_GetLogs @timeRange";

        try
        {
            dbs = new DbService();
            List<System_Log> logs = new List<System_Log>();

            dbs.con.Open();

            using (SqlCommand cmd = new SqlCommand(query, dbs.con))
            {
                cmd.Parameters.AddWithValue("@timeRange", timeRange);

                using (SqlDataReader sdr = cmd.ExecuteReader())
                {
                    int idIdx = sdr.GetOrdinal("Id");
                    int whoChangedIdx = sdr.GetOrdinal("WhoChanged");
                    int tableNameIdx = sdr.GetOrdinal("TableName");
                    int recordeUniqueIdIdx = sdr.GetOrdinal("Recorde_UniqueId");
                    int columnNameIdx = sdr.GetOrdinal("ColumnName");
                    int oldValueIdx = sdr.GetOrdinal("OldValue");
                    int newValueIdx = sdr.GetOrdinal("NewValue");
                    int dateAddedIdx = sdr.GetOrdinal("DateAdded");
                    int timeAddedIdx = sdr.GetOrdinal("TimeAdded");

                    DateTime tempDate;
                    string tempVal;

                    while (sdr.Read())
                    {
                        System_Log log = new System_Log();

                        log.Id = sdr.GetInt32(idIdx);
                        log.WhoChanged = sdr.GetString(whoChangedIdx);
                        log.TableName = sdr.GetString(tableNameIdx);
                        log.Recorde_UniqueId = sdr.GetInt32(recordeUniqueIdIdx);
                        log.ColumnName = sdr.GetString(columnNameIdx);
                        log.DateAdded = sdr.GetDateTime(dateAddedIdx);
                        log.TimeAdded = sdr.GetTimeSpan(timeAddedIdx);

                        if (!sdr.IsDBNull(oldValueIdx))
                        {
                            tempVal = sdr.GetString(oldValueIdx);
                            if (DateTime.TryParse(tempVal, out tempDate))
                                log.OldValue_AsDate = tempDate;
                            else
                                log.OldValue = tempVal;
                        }

                        if (!sdr.IsDBNull(newValueIdx))
                        {
                            tempVal = sdr.GetString(newValueIdx);
                            if (DateTime.TryParse(tempVal, out tempDate))
                                log.NewValue_AsDate = tempDate;
                            else
                                log.NewValue = tempVal;
                        }

                        logs.Add(log);
                    }
                }
            }
            return logs;
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            dbs.CloseConnection();
        }
    }
}