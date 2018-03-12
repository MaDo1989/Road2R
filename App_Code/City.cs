using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;

/// <summary>
/// Summary description for City
/// </summary>
public class City
{
    int cityID;
    string cityName;
    string area;
    int zone;

    public int CityID
    {
        get
        {
            return cityID;
        }

        set
        {
            cityID = value;
        }
    }

    public string CityName
    {
        get
        {
            return cityName;
        }

        set
        {
            cityName = value;
        }
    }

    public string Area
    {
        get
        {
            return area;
        }

        set
        {
            area = value;
        }
    }

    public int Zone
    {
        get
        {
            return zone;
        }

        set
        {
            zone = value;
        }
    }

    public City()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public City(int cityID, string cityName, string area, int zone)
    {
        CityID = cityID;
        CityName = cityName;
        Area = area;
        Zone = zone;
    }

    public List<City> getCitiesList()
    {
        #region DB functions
        string query = "select * from City order by City";

        List<City> list = new List<City>();
        DbService db = new DbService();
        DataSet ds = db.GetDataSetByQuery(query);

        foreach (DataRow dr in ds.Tables[0].Rows)
        {
            //לשנות ל-zone
            City tmp = new City((int)dr["CityID"], dr["City"].ToString(), dr["Area"].ToString(), (int)dr["Zone"]);
            list.Add(tmp);
        }
        #endregion

        return list;
    }

    public int getCityID()
    {
        #region DB functions
        string query = "select * from City where City = '" + CityName + "'";

        List<City> list = new List<City>();
        DbService db = new DbService();
        DataSet ds = db.GetDataSetByQuery(query);

        int ID = 0;
        foreach (DataRow dr in ds.Tables[0].Rows)
        {
            ID = (int)dr["CityID"];
        }
        #endregion

        return ID;
    }

}