using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;

/// <summary>
/// Summary description for TruckHandlings
/// </summary>
public class TruckHandlings
{
    int truckHandlingID;
    string truckHandlingDescription;
    string handlingProvider;
    string active;
    string url;
    float cost;
    DateTime date;
    TruckHandlingTypes truckHandlingType;
    Trucks truck;
    int imgID;

    public int TruckHandlingID
    {
        get
        {
            return truckHandlingID;
        }

        set
        {
            truckHandlingID = value;
        }
    }

    public string TruckHandlingDescription
    {
        get
        {
            return truckHandlingDescription;
        }

        set
        {
            truckHandlingDescription = value;
        }
    }

    public string HandlingProvider
    {
        get
        {
            return handlingProvider;
        }

        set
        {
            handlingProvider = value;
        }
    }

    public string Active
    {
        get
        {
            return active;
        }

        set
        {
            active = value;
        }
    }

    public float Cost
    {
        get
        {
            return cost;
        }

        set
        {
            cost = value;
        }
    }

    public DateTime Date
    {
        get
        {
            return date;
        }

        set
        {
            date = value;
        }
    }

    public TruckHandlingTypes TruckHandlingType
    {
        get
        {
            return truckHandlingType;
        }

        set
        {
            truckHandlingType = value;
        }
    }

    public Trucks Truck
    {
        get
        {
            return truck;
        }

        set
        {
            truck = value;
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

    public int ImgID
    {
        get
        {
            return imgID;
        }

        set
        {
            imgID = value;
        }
    }

    public TruckHandlings()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public TruckHandlings(int truckHandlingID, string truckHandlingDescription, string handlingProvider, string active, string url, float cost, DateTime date, TruckHandlingTypes truckHandlingType, Trucks truck)
    {
        TruckHandlingID = truckHandlingID;
        TruckHandlingDescription = truckHandlingDescription;
        HandlingProvider = handlingProvider;
        Active = active;
        Url = url;
        Cost = cost;
        Date = date;
        TruckHandlingType = truckHandlingType;
        Truck = truck;
    }

    //without active
    public TruckHandlings(int truckHandlingID, string truckHandlingDescription, string handlingProvider, string url, float cost, DateTime date, TruckHandlingTypes truckHandlingType, Trucks truck)
    {
        TruckHandlingID = truckHandlingID;
        TruckHandlingDescription = truckHandlingDescription;
        HandlingProvider = handlingProvider;
        Url = url;
        Cost = cost;
        Date = date;
        TruckHandlingType = truckHandlingType;
        Truck = truck;
    }

    //without active
    public TruckHandlings(int truckHandlingID, string truckHandlingDescription, string handlingProvider, float cost, DateTime date, TruckHandlingTypes truckHandlingType, Trucks truck, int imgID)
    {
        TruckHandlingID = truckHandlingID;
        TruckHandlingDescription = truckHandlingDescription;
        HandlingProvider = handlingProvider;
        Active = active;
        Cost = cost;
        Date = date;
        TruckHandlingType = truckHandlingType;
        Truck = truck;
        ImgID = imgID;
    }

    public List<TruckHandlings> getTruckHandlingsList(bool active, int selectedTruckID, DateTime startDate, DateTime endDate)
    {
        #region DB functions
        string sqlStartDate = startDate.ToString("yyyy-MM-dd");
        string sqlEndDate = endDate.ToString("yyyy-MM-dd");

        string query = "select * from TruckHandlings th inner join TruckHandlingTypes tht on th.TruckHandlingTypeID=tht.TruckHandlingTypeID inner join Trucks t on th.TruckID=t.TruckID where 1=1";
        if (active)
        {
            query += " and th.Active = 'Y'";
        }
        if (selectedTruckID != -1)
        {
            query += " and th.TruckID =" + selectedTruckID;
        }
        if (startDate.Year != 1)
        {
            query += " and th.Date >= '" + sqlStartDate + "'";
            if (endDate.Year != 1)
            {
                query += " and th.Date <= '" + sqlEndDate + "'";
            }
        }

        query += " order by th.Date desc";

        List<TruckHandlings> list = new List<TruckHandlings>();
        DbService db = new DbService();
        DataSet ds = db.GetDataSetByQuery(query);

        foreach (DataRow dr in ds.Tables[0].Rows)
        {
            TruckHandlings tmp = new TruckHandlings();
            tmp.TruckHandlingID = (int)dr["TruckHandlingID"];
            tmp.TruckHandlingDescription = dr["TruckHandlingDescription"].ToString();
            tmp.HandlingProvider = dr["HandlingProvider"].ToString();
            tmp.Active = dr["Active"].ToString();
            if (dr["HasImg"].ToString() == "Y")
            {
                try { tmp.ImgID = (int)dr["ImgID"]; }
                catch { tmp.ImgID = 0; }
               
            }
            tmp.Cost = float.Parse(dr["Cost"].ToString());
            tmp.Date = (DateTime)dr["Date"];
            tmp.TruckHandlingType = new TruckHandlingTypes((int)dr["TruckHandlingTypeID"], dr["TruckHandlingType"].ToString());
            tmp.Truck = new Trucks();
            tmp.Truck.TruckID = (int)dr["TruckID"];
            tmp.Truck.TruckLicense = (int)dr["TruckLicense"];

            list.Add(tmp);
        }
        #endregion

        return list;
    }

    public List<TruckHandlings> getTruckHandlingsPeriodList(bool active, int selectedTruckID, int func)
    {
        #region DB functions

        string query = "select * from TruckHandlings th inner join TruckHandlingTypes tht on th.TruckHandlingTypeID=tht.TruckHandlingTypeID inner join Trucks t on th.TruckID=t.TruckID where 1=1";
        if (active)
        {
            query += " and th.Active = 'Y'";
        }
        if (selectedTruckID != -1)
        {
            query += " and th.TruckID =" + selectedTruckID;
        }

        #region selecting Period

        if (func == 1)
        {
            query += " and th.Date >= convert(date, GETDATE())"
                + " order by th.Date asc";
        }

        if (func == 2)
        {
            query += " and th.Date < DATEADD(mm, DATEDIFF(mm, 0, GETDATE()) +1, 0) and th.Date >= DATEADD(mm, DATEDIFF(mm, 0, GETDATE()) -0, 0)"
                + " order by th.Date asc";
        }

        if (func == 3)
        {
            query += " and th.Date <= convert(date, GETDATE())"
                + " order by th.Date desc";
        }

        #endregion

        List<TruckHandlings> list = new List<TruckHandlings>();
        DbService db = new DbService();
        DataSet ds = db.GetDataSetByQuery(query);

        foreach (DataRow dr in ds.Tables[0].Rows)
        {
            TruckHandlings tmp = new TruckHandlings();
            tmp.TruckHandlingID = (int)dr["TruckHandlingID"];
            tmp.TruckHandlingDescription = dr["TruckHandlingDescription"].ToString();
            tmp.HandlingProvider = dr["HandlingProvider"].ToString();
            tmp.Active = dr["Active"].ToString();
            if (dr["HasImg"].ToString() == "Y")
            {
                tmp.ImgID = (int)dr["ImgID"];
            }
            tmp.Cost = float.Parse(dr["Cost"].ToString());
            tmp.Date = (DateTime)dr["Date"];
            tmp.TruckHandlingType = new TruckHandlingTypes((int)dr["TruckHandlingTypeID"], dr["TruckHandlingType"].ToString());
            tmp.Truck = new Trucks();
            tmp.Truck.TruckID = (int)dr["TruckID"];
            tmp.Truck.TruckLicense = (int)dr["TruckLicense"];

            list.Add(tmp);
        }
        #endregion

        return list;
    }

    public TruckHandlings getTruckHandling()
    {
        #region DB functions
        string query = "select * from TruckHandlings th inner join TruckHandlingTypes tht on th.TruckHandlingTypeID=tht.TruckHandlingTypeID inner join Trucks t on th.TruckID=t.TruckID where th.TruckHandlingID =" + TruckHandlingID;

        TruckHandlings t = new TruckHandlings();
        DbService db = new DbService();
        DataSet ds = db.GetDataSetByQuery(query);

        foreach (DataRow dr in ds.Tables[0].Rows)
        {
            t.TruckHandlingID = (int)dr["TruckHandlingID"];
            t.TruckHandlingDescription = dr["TruckHandlingDescription"].ToString();
            t.HandlingProvider = dr["HandlingProvider"].ToString();
            t.Active = dr["Active"].ToString();
            if (dr["HasImg"].ToString() == "Y")
            {
                t.ImgID = (int)dr["ImgID"];
            }
            t.Cost = float.Parse(dr["Cost"].ToString());
            t.Date = (DateTime)dr["Date"];
            t.TruckHandlingType = new TruckHandlingTypes((int)dr["TruckHandlingTypeID"], dr["TruckHandlingType"].ToString());
            t.Truck = new Trucks();
            t.Truck.TruckID = (int)dr["TruckID"];
            t.Truck.TruckLicense = (int)dr["TruckLicense"];
        }
        #endregion

        return t;
    }


    public List<TruckHandlings> getTruckLicenses(bool active)
    {
        #region DB functions
        string query = "select * from TruckHandlings th inner join TruckHandlingTypes tht on th.TruckHandlingTypeID=tht.TruckHandlingTypeID inner join Trucks t on th.TruckID=t.TruckID where 1=1";
        if (active)
        {
            query += " and th.Active = 'Y'";
        }

        List<TruckHandlings> list = new List<TruckHandlings>();
        DbService db = new DbService();
        DataSet ds = db.GetDataSetByQuery(query);

        foreach (DataRow dr in ds.Tables[0].Rows)
        {
            TruckHandlings tmp = new TruckHandlings();
            tmp.TruckHandlingID = (int)dr["TruckHandlingID"];
            tmp.TruckHandlingDescription = dr["TruckHandlingDescription"].ToString();
            tmp.HandlingProvider = dr["HandlingProvider"].ToString();
            tmp.Active = dr["Active"].ToString();
            //tmp.Url = dr["Url"].ToString();
            tmp.Cost = float.Parse(dr["Cost"].ToString());
            tmp.Date = (DateTime)dr["Date"];
            tmp.TruckHandlingType = new TruckHandlingTypes((int)dr["TruckHandlingTypeID"], dr["TruckHandlingType"].ToString());
            tmp.Truck = new Trucks();
            tmp.Truck.TruckID = (int)dr["TruckID"];
            tmp.Truck.TruckLicense = (int)dr["TruckLicense"];

            list.Add(tmp);
        }
        #endregion

        return list;
    }


    public void setTruckHandling(string func)
    {
        string sqlFormattedDate = Date.ToString("yyyy-MM-dd");

        DbService db = new DbService();
        string query = "";
        if (func == "edit")
        {
            query = "UPDATE TruckHandlings SET TruckHandlingDescription = '" + TruckHandlingDescription + "', HandlingProvider = '" + HandlingProvider + "', Cost = " + Cost + ", Date = '" + sqlFormattedDate + "', TruckHandlingTypeID = " + TruckHandlingType.TruckHandlingTypeID + ", TruckID = " + Truck.TruckID + " WHERE TruckHandlingID = " + TruckHandlingID;
        }
        else if (func == "new")
        {
            query = "insert into TruckHandlings values ('" + TruckHandlingDescription + "','" + HandlingProvider + "','Y'," + Cost + ",'" + sqlFormattedDate + "'," + TruckHandlingType.TruckHandlingTypeID + "," + Truck.TruckID + ", NULL,'N')";
        }
        db.ExecuteQuery(query);
    }

    
     public void setTruckHandlingApp(string func)
    {
        string sqlFormattedDate = Date.ToString("yyyy-MM-dd");

        DbService db = new DbService();
        string query = "";
        if (func == "edit")
        {
            query = "UPDATE TruckHandlings SET TruckHandlingDescription = '" + TruckHandlingDescription + "', HandlingProvider = '" + HandlingProvider + "', Cost = " + Cost + ", Date = '" + sqlFormattedDate + "', TruckHandlingTypeID = " + TruckHandlingType.TruckHandlingTypeID + ", TruckID = " + Truck.TruckID + ", ImgID = " + ImgID + ", HasImg='Y' WHERE TruckHandlingID = " + TruckHandlingID;
        }
        else if (func == "new")
        {
            query = "insert into TruckHandlings values ('" + TruckHandlingDescription + "','" + HandlingProvider + "','Y'," + Cost + ",'" + sqlFormattedDate + "'," + TruckHandlingType.TruckHandlingTypeID + "," + Truck.TruckID + "," + ImgID + ",'Y')";
        }
        db.ExecuteQuery(query);
    }


    public void deactivateTruckHandlings(string active)
    {
        DbService db = new DbService();
        db.ExecuteQuery("UPDATE TruckHandlings SET Active='" + active + "' WHERE TruckHandlingID=" + TruckHandlingID);
    }

}