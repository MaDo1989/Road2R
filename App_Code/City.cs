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
    bool isMain;
    string area;
    int zone;
    string nearestMainCity;
    static List<City> mainCities = new List<City>();

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

    public bool IsMain
    {
        get
        {
            return isMain;
        }

        set
        {
            isMain = value;
        }
    }

    public string NearestMainCity
    {
        get
        {
            return nearestMainCity;
        }

        set
        {
            nearestMainCity = value;
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
        cmd.CommandText = "spCity_GetAllCitiesWithLocations";
        List<City> cities = new List<City>();
        mainCities.Clear();
        try
        {
            dbs = new DbService();
            sdr = dbs.GetDataReaderSP(cmd);
            while (sdr.Read())
            {
                City city = new City();
                city.CityName = Convert.ToString(sdr["CityName"]);
                city.Lat = Convert.ToDouble(sdr["Lat"]);
                city.Lng = Convert.ToDouble(sdr["Lng"]);
                if (sdr["isMain"] != DBNull.Value)
                    city.IsMain = true;

                if (city.IsMain)
                    mainCities.Add(city);
                else
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
        finally {
            if (dbs.con != null)
                dbs.con.Close();
        }
        return cities;
    }




    public Dictionary<string, string> getNearbyCities() {

        Dictionary<string, string> dic = new Dictionary<string, string>();
        cmd = new SqlCommand();
        dbs = new DbService();
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandText = "spGetVolunteerCities";

        try
        {
            dbs = new DbService();
            sdr = dbs.GetDataReaderSP(cmd);
            while (sdr.Read())
            {
                City city = new City();
                city.CityName = Convert.ToString(sdr["CityName"]);
                city.nearestMainCity = Convert.ToString(sdr["mainCity"]);
                dic.Add(city.CityName, city.nearestMainCity);
            }
            return dic;
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally {
            if (dbs.con != null)
                dbs.con.Close();
        }
    }


    public int write(List<City> cities)
    {
        DbService dbs = new DbService();
        return dbs.writeGoogleCities(cities);
    }

    public int writeNearestMainCities() {

        // this returns all the cities and also populates the mainCities List
        List<City> cityList = getUnmappedCitiesList();

        int numCities = 0;

        // iterate over all the cities
        foreach (City c in cityList)
        {
            double dist = 1000000000000;
            foreach (City mc in mainCities) {
                double d = DistanceTo(c.Lat, c.Lng, mc.Lat, mc.Lng);
                if (d < dist) {
                    dist = d;
                    c.NearestMainCity = mc.cityName;
                }
            }
            // update the database



        }
        DbService db = new DbService();
        numCities = db.updateNearestCity(cityList);

        return numCities;

    }




    public static double DistanceTo(double lat1, double lon1, double lat2, double lon2, char unit = 'K')
    {
        double rlat1 = Math.PI * lat1 / 180;
        double rlat2 = Math.PI * lat2 / 180;
        double theta = lon1 - lon2;
        double rtheta = Math.PI * theta / 180;
        double dist =
            Math.Sin(rlat1) * Math.Sin(rlat2) + Math.Cos(rlat1) *
            Math.Cos(rlat2) * Math.Cos(rtheta);
        dist = Math.Acos(dist);
        dist = dist * 180 / Math.PI;
        dist = dist * 60 * 1.1515;

        switch (unit)
        {
            case 'K': //Kilometers -> default
                return dist * 1.609344;
            case 'N': //Nautical Miles 
                return dist * 0.8684;
            case 'M': //Miles
                return dist;
        }

        return dist;
    }

}