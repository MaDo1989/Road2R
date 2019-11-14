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
    SqlConnection con;

    SqlDataAdapter adp;

    public DbService()
    {
        try
        {

            con = new SqlConnection(ConfigurationManager.ConnectionStrings["db"].ConnectionString);

        }
        catch (Exception ex)
        {

            throw ex;
        }
    }
    public void CloseConnection()
    {
        if (con.State == ConnectionState.Open)
        {
            con.Close();
        }
    }

    public DataSet GetDataSetByQuery(string sqlQuery,bool needToClose = true,CommandType cmdType = CommandType.Text, params SqlParameter[] parametersArray)
    {
        try
        {
            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }

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
            catch (Exception ex)
            {
                //do something with the error
                ds = null;
            }
            return ds;
        }
        catch (Exception e)
        {
            //Write exception to log
            throw e;
        }

        finally
        {
            if (needToClose)
            {
                con.Close();
            }
        }


    }

    public int ExecuteQuery(string sqlQuery, CommandType cmdType = CommandType.Text, params SqlParameter[] parametersArray)
    {
        int row_affected = 0;

        try
        {
            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }

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
            catch (Exception ex)
            {
                tran.Rollback();
                throw ex;
            }
            return row_affected;
        }
        finally
        {
            con.Close();
        }
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
            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }
            res = cmd.ExecuteScalar();
        }
        catch (Exception e)
        {
            throw e;
        }
        finally
        {
            con.Close();
        }

        return res;
    }


}