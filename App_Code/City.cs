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

    DbService dbs;
    SqlCommand cmd;
    SqlDataReader sdr;

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

    public int Zone { get; set; }
    public string Area { get; set; }

    public City()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public City(int cityID, string cityName, string area, int zone)
    {
      // CityID = cityID;
        CityName = cityName;
        //Area = area;
        //Zone = zone;
    }

    public City(string cityName)
    {
        CityName = cityName;
    }

    public List<City> getCitiesList()
    {
        cmd = new SqlCommand();
        dbs = new DbService();
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandText = "spCity_GetAllCities";
        List<City> cities = new List<City>();

        try
        {
            dbs = new DbService();
            sdr = dbs.GetDataReaderSP(cmd);
            City city;
            string cityName;
            while (sdr.Read())
            {
                cityName = Convert.ToString(sdr["CityName"]);
                city = new City(cityName);
                cities.Add(city);
            }
        }
        catch (Exception)
        {
            throw;
        }

        return cities;
    }

}