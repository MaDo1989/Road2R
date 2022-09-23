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
    double lat;
    double lng;
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

    public double Lat
    {
        get
        {
            return lat;
        }

        set
        {
            lat = value;
        }
    }

    public double Lng
    {
        get
        {
            return lng;
        }

        set
        {
            lng = value;
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
      // CityID = cityID;
        CityName = cityName;
        //Area = area;
        //Zone = zone;
    }

    public City(string cityName)
    {
        CityName = cityName;
    }

    public City(string cityName, double lat, double lng)
    {
        this.cityName = cityName;
        this.Lat = lat;
        this.Lng = lng;
    }


    
    public List<City> getVolCitiesList()
    {
        cmd = new SqlCommand();
        dbs = new DbService();
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandText = "spCity_GetVolCities";
        List<City> cities = new List<City>();

        try
        {
            dbs = new DbService();
            sdr = dbs.GetDataReaderSP(cmd);
            while (sdr.Read())
            {
                City city = new City();
                city.CityName = Convert.ToString(sdr["CityName"]);
                if (sdr["lat"] != DBNull.Value)
                    city.Lat = Convert.ToDouble(sdr["lat"]);
                if (sdr["lng"] != DBNull.Value)
                    city.Lng = Convert.ToDouble(sdr["lng"]);
                cities.Add(city);
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }

        return cities;
    }

    
    public List<City> getUnmappedCitiesList()
    {
        cmd = new SqlCommand();
        dbs = new DbService();
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandText = "spCity_GetAllUnmappedCities";
        List<City> cities = new List<City>();

        try
        {
            dbs = new DbService();
            sdr = dbs.GetDataReaderSP(cmd);
            while (sdr.Read())
            {
                City city = new City();
                city.CityName = Convert.ToString(sdr["CityName"]);
                cities.Add(city);
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }

        return cities;
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
            while (sdr.Read())
            {
                City city = new City();
                city.CityName = Convert.ToString(sdr["CityName"]);

                if (sdr["lat"] != DBNull.Value)
                    city.Lat = Convert.ToDouble(sdr["lat"]);
                if (sdr["lng"] != DBNull.Value)
                    city.Lng = Convert.ToDouble(sdr["lng"]);

                cities.Add(city);
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }

        return cities;
    }

    public int write(List<City> cities)
    {
        DbService dbs = new DbService();
        return dbs.writeGoogleCities(cities);
    }

}