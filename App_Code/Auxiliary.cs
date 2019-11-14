using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for Auxiliary
/// </summary>
public class Auxiliary
{
    public Auxiliary()
    {
    }

    public string getDriverName(int driverId)
    {
        DbService db = new DbService();
        string query1 = "select Id,DisplayName,CellPhone from Volunteer where Id = " + driverId;
        DataSet ds1 = db.GetDataSetByQuery(query1);
        DataRow dr1 = ds1.Tables[0].Rows[0];

        return dr1[1].ToString();
    }

    //public string GetStatusName(string status)
    //{
    //    DbService db = new DbService();
    //    string query3 = "select StatusName from Status where StatusId = " + status;
    //    DataSet ds3 = db.GetDataSetByQuery(query3);
    //    DataRow dr3 = ds3.Tables[0].Rows[0];

    //    return dr3[0].ToString();
    //}
    public int GetDriverId(int rideId)
    {
        DbService db = new DbService();
        string query1 = "select MainDriver from RPView where RideNum = " + rideId;
        DataSet ds1 = db.GetDataSetByQuery(query1);
        DataRow dr1 = ds1.Tables[0].Rows[0];
       // coor = (string)dr1[1];
        return (int)dr1[0];
    }
    public string[] GetDriverAndCoordinatorByRidePat(int ridePatId)
    {
        DbService db = new DbService();
        string query1 = "select Coordinator,MainDriver from RPView where RidePatNum = " + ridePatId;
        DataSet ds1 = db.GetDataSetByQuery(query1);
        DataRow dr1 = ds1.Tables[0].Rows[0];

        if (dr1[1].ToString() == "")
        {
            string query2 = "select UserName from Volunteer where DisplayName = N'" + dr1[0] + "'";
            DataSet ds2 = db.GetDataSetByQuery(query2);
            DataRow dr2 = ds2.Tables[0].Rows[0];
        }
        else
        {
            string query2 = "select DisplayName from Volunteer where Id = " + dr1[1];
            DataSet ds2 = db.GetDataSetByQuery(query2);
            DataRow dr2 = ds2.Tables[0].Rows[0];
        }



        string query3 = "select UserName from Volunteer where DisplayName = N'" + dr1[0]+ "'";
        DataSet ds3 = db.GetDataSetByQuery(query3);
        DataRow dr3 = ds3.Tables[0].Rows[0];

        string[] names = { dr3[0].ToString(), dr1[0].ToString() };

        return names;
    }
    public string[] GetDriverAndCoordinatorByRide(int rideId)
    {
        DbService db = new DbService();
        string query1 = "select Coordinator,MainDriver from RPView where RideNum = " + rideId;
        DataSet ds1 = db.GetDataSetByQuery(query1);
        DataRow dr1 = ds1.Tables[0].Rows[0];


        string query2 = "select DisplayName from Volunteer where Id = " + dr1[1];
        DataSet ds2 = db.GetDataSetByQuery(query2);
        DataRow dr2 = ds2.Tables[0].Rows[0];

        string query3 = "select UserName from Volunteer where DisplayName = N'" + dr1[0] + "'";
        DataSet ds3 = db.GetDataSetByQuery(query3);
        DataRow dr3 = ds3.Tables[0].Rows[0];

        string[] names = { dr3[0].ToString(), dr2[0].ToString() };

        return names;
    }
    public List<string> getR2RServers()
    {
        return ConfigurationManager.AppSettings["AvailableServers"].Split(',').ToList();
    }

    public bool isProductionDatabase()
    {
        string dbName = ConfigurationManager.ConnectionStrings["db"].ConnectionString;
        return dbName.Contains("prod");
    }
}