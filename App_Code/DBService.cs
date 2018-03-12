using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

/// <summary>
/// Summary description for DbService
/// </summary>
public class DbService
{
    SqlTransaction tran;
    SqlCommand cmd;
    SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["db"].ConnectionString);

    SqlDataAdapter adp;

    public DbService()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public DataSet GetDataSetByQuery(string sqlQuery, CommandType cmdType = CommandType.Text, params SqlParameter[] parametersArray)
    {
        con.Open();
        cmd = new SqlCommand(sqlQuery, con);
        cmd.CommandType = cmdType;
        DataSet ds = new DataSet();
        adp = new SqlDataAdapter(cmd);

        foreach (SqlParameter s in parametersArray)
        {
            cmd.Parameters.AddWithValue(s.ParameterName, s.Value);

        }

        try
        {
            adp.Fill(ds);
        }
        catch (Exception )
        {
            //do something with the error
            ds = null;
        }
        finally
        {
            con.Close();
        }

        return ds;
    }

    public int ExecuteQuery(string sqlQuery, CommandType cmdType = CommandType.Text, params SqlParameter[] parametersArray)
    {
        int row_affected = 0;
        using (con)
        {

            con.Open();
            tran = con.BeginTransaction();

            cmd = new SqlCommand(sqlQuery, con, tran);
            cmd.CommandType = cmdType;

            foreach (SqlParameter s in parametersArray)
            {
                cmd.Parameters.AddWithValue(s.ParameterName, s.Value);
            }

            try
            {
                row_affected = cmd.ExecuteNonQuery();
                tran.Commit();
            }
            catch (Exception )
            {
                tran.Rollback();
            }
        }

        return row_affected;
    }


    public object GetObjectScalarByQuery(string sqlQuery, CommandType cmdType = CommandType.Text, params SqlParameter[] parametersArray)
    {
        cmd = new SqlCommand(sqlQuery, con);
        cmd.CommandType = cmdType;
        object res = null;

        foreach (SqlParameter s in parametersArray)
        {
            cmd.Parameters.AddWithValue(s.ParameterName, s.Value);
        }

        try
        {
            con.Open();
            res = cmd.ExecuteScalar();
        }
        catch
        {
            //do something with the error
        }
        finally
        {
            con.Close();
        }

        return res;
    }

    public DataTable getRidePat()
    {
        string cmdStr = "select * from RidePat";
        DataSet ds = new DataSet();
        try
        {
            adp = new SqlDataAdapter(cmdStr, con);
            
            adp.Fill(ds, "RidePat");
            
        }
        catch (Exception e)
        {
            e.Message.ToString();
        }
        return ds.Tables["RidePAt"];
    }
}