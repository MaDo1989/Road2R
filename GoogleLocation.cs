using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for GoogleLocation
/// </summary>
public class GoogleLocation
{

    string name;
    string type;
    string region;
    string googleName0;
    string googleName1;
    string googleName2;
    double lat;
    double lng;
    public GoogleLocation()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public GoogleLocation(string name, string type, string region, string googleName0, string googleName1, string googleName2, double lat, double lng)
    {
        this.name = name;
        this.type = type;
        this.region = region;
        this.googleName0 = googleName0;
        this.googleName1 = googleName1;
        this.googleName2 = googleName2;
        this.lat = lat;
        this.lng = lng;
    }

    public string Name { get => name; set => name = value; }
    public string Type { get => type; set => type = value; }
    public string Region { get => region; set => region = value; }
    public string GoogleName0 { get => googleName0; set => googleName0 = value; }
    public string GoogleName1 { get => googleName1; set => googleName1 = value; }
    public string GoogleName2 { get => googleName2; set => googleName2 = value; }
    public double Lat { get => lat; set => lat = value; }
    public double Lng { get => lng; set => lng = value; }

    static public List<string> readLocationsNamesByType(string locType)
    {
        {
            DbService db = new DbService();

            string where = "";

            if (locType != "הכל")
                where = " where [type] = N'" + locType + "'";


            string select = "SELECT name FROM location " + where;
            DataSet ds = db.GetDataSetByQuery(select);
            List<string> locations = ds.Tables[0].Rows.OfType<DataRow>().Select(dr => dr.Field<string>("Name")).ToList();
            return locations;
        }
    }


    public int write(List<GoogleLocation> gl)
    {
        DbService dbs = new DbService();
        return dbs.writeGoogleLocations(gl);
    }


}