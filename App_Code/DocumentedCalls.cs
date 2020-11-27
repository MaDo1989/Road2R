using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for DocumentedCalls
/// </summary>
public class DocumentedCalls
{
    public DocumentedCalls(int callId, int driverId, int coordinatorId, DateTime callRecordedDate, DateTime callRecordedTime, string callContent)
    {
        CallId = callId;
        DriverId = driverId;
        CoordinatorId = coordinatorId;
        CallRecordedDate = callRecordedDate;
        CallRecordedTime = callRecordedTime;
        CallContent = callContent;
    }

    string query;
    DbService dbs;
    public int CallId { get; set; }
    public int DriverId { get; set; }
    public int CoordinatorId { get; set; }
    public DateTime CallRecordedDate { get; set; } // YYYY-MM-DD
    public DateTime CallRecordedTime { get; set; } // HH:MM
    public string CallContent { get; set; }
    //public List<DocumentedCalls> GetDocumentedCallsByDriverId(int driverId)
    //{

    //    try
    //    {
    //        dbs = new DbService();

    //    }
    //    catch (Exception ex)
    //    {

    //        throw new Exception(ex.Message);
    //    }

    //    throw new NotImplementedException();
    //}



    //#region insert query to copy past and then delete

    ///*
    // exec spDocumentedCalls_InsertCall
    //    @driverId=22658,
    //    @coordinatorId=22659,
    //    @callRecordedDate='2020-11-27',
    //    @callRecordedTime='09:15',
    //    @callContent=N'זוהי בדיקה',
    //    @callStatus=N'זוהי בדיקה '

    // */

    //#endregion

}