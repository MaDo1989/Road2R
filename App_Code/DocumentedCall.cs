using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for DocumentedCalls
/// </summary>
public class DocumentedCall
{
    //private fields
    string query;
    DbService dbs;

    public int CallId { get; set; }
    public int DriverId { get; set; }
    public int CoordinatorId { get; set; }
    public DateTime CallRecordedDate { get; set; } // YYYY-MM-DD
    public TimeSpan CallRecordedTime { get; set; } // HH:MM
    public string CallContent { get; set; }
    public string CallStatus { get; set; }
    public string DriverName { get; set; }              //comes from the DocumentedCallView
    public string CoordinatorName { get; set; }         //comes from the DocumentedCallView


    public List<DocumentedCall> GetDocumentedCallsByDriverId(int driverId)
    {
        List<DocumentedCall> documentedCalls = new List<DocumentedCall>();
        DocumentedCall documentedCall;
        query = "exec spDocumentedCallView_GetDocumentedCallsByDriverId @driverId=" + driverId; //this sp is allready taking care for not fetching calls which marked deleted.
        try
        {
            dbs = new DbService();
            SqlDataReader sdr = dbs.GetDataReader(query);
            while (sdr.Read())
            {
                documentedCall = new DocumentedCall();
                documentedCall.CallId = Convert.ToInt32(sdr["CallId"]);
                documentedCall.DriverId = Convert.ToInt32(sdr["DriverId"]);
                documentedCall.CoordinatorId = Convert.ToInt32(sdr["CoordinatorId"]);
                documentedCall.CallRecordedDate = Convert.ToDateTime(Convert.ToString(sdr["CallRecordedDate"]));
                documentedCall.CallRecordedTime = TimeSpan.Parse(Convert.ToString(sdr["CallRecordedTime"]));
                documentedCall.CallContent = Convert.ToString(sdr["CallContent"]);
                documentedCall.DriverName = Convert.ToString(sdr["DriverName"]);
                documentedCall.CoordinatorName = Convert.ToString(sdr["CoordinatorName"]);
                documentedCalls.Add(documentedCall);
            }
            return documentedCalls;
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


    public bool ChangeDocumentedCallStatus(int callId, string newValue)
    {
        query = "exec spDocumentedCall_UpdateCallStatus @CallId = " + callId + ", @newValue = N'" + newValue + "'";

        try
        {
            dbs = new DbService();

            int res = dbs.ExecuteQuery(query);

            return res > 0;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }

    }
    public bool DocumentNewCall(int driverId, int coordinatorId, DateTime callRecordedDate, TimeSpan callRecordedTime, string callContent)
    {
        query = "exec spDocumentedCall_InsertCall @driverId=" + driverId;
        query += ", @coordinatorId=" + coordinatorId + ", @callRecordedDate='" + callRecordedDate + "',";
        query += "@callRecordedTime='" + callRecordedTime + "', @callContent=N'" + callContent + "'";

        try
        {
            dbs = new DbService();

            int res = dbs.ExecuteQuery(query);

            return res > 0;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }

    }

    public bool UpdateDocumentedCallField(int callId, string feild, string newValue)
    {
        throw new NotImplementedException();
    }

}