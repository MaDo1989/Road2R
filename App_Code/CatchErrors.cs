using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for CatchErrors
/// </summary>
public class CatchErrors
{
    string query;
    DbService dbs;
    SqlCommand cmd;
    public CatchErrors(string whereCatched, string exceptionStackTrace, string exceptionMessage)
    {
        WhereCatched = whereCatched;
        ExceptionStackTrace = exceptionStackTrace;
        ExceptionMessage = exceptionMessage;
        DocumentThisExp();
    }
    public string WhereCatched { get; set; }
    public string ExceptionStackTrace { get; set; }
    public string ExceptionMessage { get; set; }

    private void DocumentThisExp()
    {
        SqlParameter[] cmdParams = new SqlParameter[3];
        cmd = new SqlCommand();

        cmdParams[0] = cmd.Parameters.AddWithValue("@WhereCatched", WhereCatched);
        cmdParams[1] = cmd.Parameters.AddWithValue("@ExceptionStackTrace", ExceptionStackTrace);
        cmdParams[2] = cmd.Parameters.AddWithValue("@ExceptionMessage", ExceptionMessage);

        query = "insert into CatchErros(WhereCatched, ExceptionStackTrace, ExceptionMessage) values (@WhereCatched, @ExceptionStackTrace, @ExceptionMessage)";
        dbs = new DbService();

        try
        {
            dbs.ExecuteQuery(query, CommandType.Text, cmdParams);
        }
        catch (Exception exp)
        {
            throw exp;
        }
    }


}