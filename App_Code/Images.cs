using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;

/// <summary>
/// Summary description for Images
/// </summary>
public class Images
{
    int imageID;
    string url;

    public Images()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public int ImageID
    {
        get
        {
            return imageID;
        }

        set
        {
            imageID = value;
        }
    }

    public string Url
    {
        get
        {
            return url;
        }

        set
        {
            url = value;
        }
    }

    public int getLastDriverImgID(int driverID)
    {
        string convertedID = "";
        int ID;

        if (driverID<10)
        {
            convertedID = "100" + driverID.ToString();
        }
        else if (driverID<100)
        {
            convertedID = "10" + driverID.ToString();
        }
        else if (driverID <1000)
        {
            convertedID = "1" + driverID.ToString();
        }

        ID = Int32.Parse(convertedID);
        int newDriverID = Int32.Parse(convertedID);

        string query = "select count(ImageID) as 'id' from Images where ImageID like '" + newDriverID + "%'";
        DbService db = new DbService();
        DataSet ds = db.GetDataSetByQuery(query);
        DataTable dt = ds.Tables[0];
        if (Convert.ToInt32(dt.Rows[0]["id"]) != 0)
        {
            query = "select max(ImageID) as 'id' from Images where ImageID like '" + newDriverID + "%'";
            DbService db1 = new DbService();
            DataSet ds1 = db1.GetDataSetByQuery(query);
            DataTable dt1 = ds1.Tables[0];
            ID = Convert.ToInt32(dt1.Rows[0]["id"]) + 1;
        }
        else
        {
            ID = newDriverID * 10 + 1;
        }

        return ID;
    }
    public string getDocumentImageURL(int imgID)
    {

        #region DB functions
        string query = "select * from Images where ImageID =" + imgID + "";

        Images i = new Images();
        DbService db = new DbService();
        DataSet ds = db.GetDataSetByQuery(query);

        foreach (DataRow dr in ds.Tables[0].Rows)
        {
            i.ImageID = (int)dr["ImageID"];
            //i.Url = dr["ImageUrl"].ToString();
            i.Url = Convert.ToBase64String((byte[])(dr["ImageUrl"]));
        }
        #endregion

        return i.Url;
    }
}