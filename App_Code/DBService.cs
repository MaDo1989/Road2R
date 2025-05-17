using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;


/// <summary>
/// Summary description for DbService
/// </summary>
public class DbService : IDisposable
{
    SqlTransaction tran;
    SqlCommand cmd;
    public SqlConnection con;
    public List<string> stackTraces;
    static int counter = 1;
    StackTrace stackTrace;
    static string path = "\\log\\StackTraces.txt";
    string stackTracesfilePath = HttpContext.Current.Server.MapPath("~") + path;

    SqlDataAdapter adp;
    public DbService(bool noConnectopnInstance)
    {

    }
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
        finally
        {
            try
            {
                LogToTextFile();
            }
            catch (Exception)
            {
                //the catch is empty in order not to crash the system!
            }
        }
    }

    private void LogToTextFile()
    {
        stackTrace = new StackTrace();
        string result = stackTrace.ToString();
        int indexOfSystem = result.IndexOf("System");
        result = result.Substring(0, indexOfSystem);
        stackTraces = GetStackTraces();
        stackTraces.Add(counter++.ToString() + ") " + DateTime.Now + " " + result + " at datetime: " + Environment.NewLine);

        File.WriteAllLines(stackTracesfilePath, stackTraces);
    }

    public List<string> GetStackTraces()
    {
        List<string> stackTraces;
        if (File.Exists(stackTracesfilePath))
        {
            stackTraces = File.ReadAllLines(stackTracesfilePath).ToList();
        }
        else
        {
            stackTraces = new List<string>();
        }

        return stackTraces;
    }

    public string ClearStackTracesFile()
    {
        string result = "";
        if (File.Exists(stackTracesfilePath))
        {
            File.Delete(stackTracesfilePath);
            result = "stack trace initialized";
        }
        else
        {
            result = "There is no file found to initialize";
        }

        return "stack trace initialized";
    }
    public void CloseConnection()
    {
        if (con.State == ConnectionState.Open)
        {
            con.Close();
        }
    }

    public DataSet GetDataSetByQuery(string sqlQuery, bool needToClose = true, CommandType cmdType = CommandType.Text, params SqlParameter[] parametersArray)
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


    public SqlDataReader GetDataReader(SqlCommand command)
    {

        SqlDataReader dr = null;

        try
        {
            if (con == null)
                con = new SqlConnection();

            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }

            command.Connection = con;

            try
            {
                dr = command.ExecuteReader();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dr;
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


    // Allows caller to provide a prepared command, that has the parameters already added
    public DataSet GetDataSetBySqlCommand(SqlCommand cmd)
    {
        try
        {
            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }

            cmd.Connection = con;
            DataSet ds = new DataSet();
            adp = new SqlDataAdapter(cmd);

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
            SqlConnection.ClearPool(con);
            CloseConnection();
        }

    }

    //Yogev ↓
    public SqlDataReader GetDataReader(string query)
    {
        /*


        !!!

        I intentionally do not use finally and then close con here
        BE AWARE !
        IF USE THIS METHOD CLOSE THE CONNECTION VIA CloseConnection() METHOD
        FROM WHERE YOU USE IT !

        !!!

         */
        try
        {
            if (con.State == ConnectionState.Closed) { con.Open(); }
            cmd = new SqlCommand(query, con);
            return cmd.ExecuteReader();
        }
        catch (Exception ex)
        {
            throw new Exception("exception in DBService →  GetDataReader(string query) " + ex);
        }
        /*

        !!!
        
        I intentionally do not use finally and then close con here
        BE AWARE !
        IF USE THIS METHOD CLOSE THE CONNECTION VIA CloseConnection() METHOD
        FROM WHERE YOU USE IT !
        
        !!!


         */
    }


    public SqlDataReader GetDataReaderSP(SqlCommand cmd)
    {
        /*


        !!!

        I intentionally do not use finally and then close con here
        BE AWARE !
        IF USE THIS METHOD CLOSE THE CONNECTION VIA CloseConnection() METHOD
        FROM WHERE YOU USE IT !

        !!!

         */
        try
        {
            if (con.State == ConnectionState.Closed) { con.Open(); cmd.Connection = con; }
            return cmd.ExecuteReader();
        }
        catch (Exception ex)
        {
            throw new Exception("exception in DBService →  GetDataReader(string query) " + ex);
        }
        /*

        !!!
        
        I intentionally do not use finally and then close con here
        BE AWARE !
        IF USE THIS METHOD CLOSE THE CONNECTION VIA CloseConnection() METHOD
        FROM WHERE YOU USE IT !
        
        !!!


         */
    }

    public void Dispose()
    {
        CloseConnection();
    }
    //Yogev ↑


    public int writeGoogleLocations(List<Location> locations)
    {

        int row_affected = 0;

        try
        {
            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }



            foreach (Location gl in locations)
            {

                using (SqlCommand cmd = new SqlCommand("spUpdateGoogleLocation", con))
                {
                    cmd.Parameters.AddWithValue("@name", gl.Name);
                    cmd.Parameters.AddWithValue("@lat", gl.Lat);
                    cmd.Parameters.AddWithValue("@lng", gl.Lng);
                    cmd.CommandType = CommandType.StoredProcedure;
                    row_affected += cmd.ExecuteNonQuery();
                }
            }

            return row_affected;

        }

        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            con.Close();
        }

    }

    public int writeGoogleCities(List<City> cities)
    {

        int row_affected = 0;

        try
        {
            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }

            foreach (City c in cities)
            {

                using (SqlCommand cmd = new SqlCommand("spUpdateGoogleCity", con))
                {

                    cmd.Parameters.AddWithValue("@name", c.CityName);
                    cmd.Parameters.AddWithValue("@lat", c.Lat);
                    cmd.Parameters.AddWithValue("@lng", c.Lng);
                    cmd.CommandType = CommandType.StoredProcedure;
                    row_affected += cmd.ExecuteNonQuery();
                }
            }

            return row_affected;

        }

        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            con.Close();
        }

    }

    public void ClearSqlConnectionPool()
    {
        if (con != null)
        {
            con.Dispose();
        }
    }


    public int updateNearestCity(List<City> cities)
    {

        int row_affected = 0;

        try
        {
            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }

            foreach (City c in cities)
            {

                using (SqlCommand cmd = new SqlCommand("spUpdateNearestMainCity", con))
                {
                    cmd.Parameters.AddWithValue("@cityName", c.CityName);
                    cmd.Parameters.AddWithValue("@mainCity", c.NearestMainCity);
                    cmd.CommandType = CommandType.StoredProcedure;
                    row_affected += cmd.ExecuteNonQuery();
                }
            }

            return row_affected;

        }

        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            con.Close();
        }

    }
}