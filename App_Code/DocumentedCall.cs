using System;
using System.Collections.Generic;
using System.Data.SqlClient;

/// <summary>
         /*
         =======================================================
         ||           ---!!!IMPORTANT NOTE!!!-- -             ||
         ||              ********************                 ||
         ||       for any use of this module                  ||
         ||       use the existing stored procedures          ||
         ||       as some of them has additional              ||
         ||       functionallity                              ||
         ||              ********************                 ||
         ||           ---!!!IMPORTANT NOTE!!!-- -             ||
         =======================================================
         */
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
    public DateTime FullDateStemp { get; set; } // for sorting in an accurate way
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
                documentedCall.FullDateStemp = documentedCall.CallRecordedDate + documentedCall.CallRecordedTime;
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
    public bool DocumentNewCall(DocumentedCall documentedCall)
    {
        //→receiving date as dd/mm/yyy and need to conert to yyyy-mm-dd so the db will write the correct date:


        query = "exec spDocumentedCall_InsertCall @driverId=" + documentedCall.DriverId;
        query += ", @coordinatorId=" + documentedCall.CoordinatorId + ", @callRecordedDate='" + documentedCall.CallRecordedDate.ToString("yyyy-MM-dd") + "',";
        query += "@callRecordedTime='" + documentedCall.CallRecordedTime + "', @callContent=N'" + documentedCall.CallContent + "'";

        try
        {
            dbs = new DbService();

            int res = dbs.ExecuteQuery(query);
            VolunteersCacheServiceManager.ClearCache();

            return res > 0;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }

    }

    public bool UpdateDocumentedCallField(string field2update, DocumentedCall documentedCall)
    {
        switch (field2update)
        {
            case "Date":
                query = "exec spDocumentedCall_UpdateCallRecordedDate @CallId = " + documentedCall.CallId + ", @newValue='" + documentedCall.CallRecordedDate + "'";
                break;
            case "Time":
                query = "exec spDocumentedCall_UpdateCallRecordedTime @CallId=" + documentedCall.CallId + ", @newValue='" + documentedCall.CallRecordedTime + "'";
                break;
            case "Content":
                query = "exec spDocumentedCall_UpdateCallContent @CallId=" + documentedCall.CallId + ", @newValue=N'" + documentedCall.CallContent + "'";
                break;
            default:
                throw new Exception("there was no valid field2update inserted to: UpdateDocumentedCallField(string field2update, DocumentedCall documentedCall)");
        }

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

}