using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using GoogleApi.Entities.Common;
using GoogleApi.Test.Maps.Geocoding.Address;

/// <summary>
/// Summary description for GoogleCity
/// </summary>
public class GoogleCity
{

    double lat;
    double lng;
    string name;
    string googleName0;
    string googleName1;
    string googleName2;



    public GoogleCity()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public GoogleCity(double lat, double lng, string name, string googleName0, string googleName1, string googleName2)
    {
        this.lat = lat;
        this.lng = lng;
        this.name = name;
        this.googleName0 = googleName0;
        this.googleName1 = googleName1;
        this.googleName2 = googleName2;
    }

    static public List<string> readCities() {
        DbService db = new DbService();
        string select = "SELECT cityname FROM city WHERE cityname <> ''";
        DataSet ds = db.GetDataSetByQuery(select);
        List<string> cities = ds.Tables[0].Rows.OfType<DataRow>().Select(dr => dr.Field<string>("cityname")).ToList();
        return cities;
    }

    public int write(List<GoogleCity> cities) {
        DbService dbs = new DbService();
        return dbs.writeGoogleCities(cities);
    }

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

    public string Name
    {
        get
        {
            return name;
        }

        set
        {
            name = value;
        }
    }

    public string GoogleName0
    {
        get
        {
            return googleName0;
        }

        set
        {
            googleName0 = value;
        }
    }

    public string GoogleName1
    {
        get
        {
            return googleName1;
        }

        set
        {
            googleName1 = value;
        }
    }

    public string GoogleName2
    {
        get
        {
            return googleName2;
        }

        set
        {
            googleName2 = value;
        }
    }
}