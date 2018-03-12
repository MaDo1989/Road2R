using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;

/// <summary>
/// Summary description for Trucks
/// </summary>
public class Trucks
{
    int truckID;
    int truckLicense;
    string manufacturer;
    string model;
    string active;
    string kmToDate;
    string hand;
    float purchaseCost;
    string purchaseYear;
    TruckTypes truckType;
    DateTime onRoadDate;
    DateTime insuranceExpiredDate;
    string urea;

    public int TruckID
    {
        get
        {
            return truckID;
        }

        set
        {
            truckID = value;
        }
    }

    public int TruckLicense
    {
        get
        {
            return truckLicense;
        }

        set
        {
            truckLicense = value;
        }
    }

    public string Manufacturer
    {
        get
        {
            return manufacturer;
        }

        set
        {
            manufacturer = value;
        }
    }

    public string Model
    {
        get
        {
            return model;
        }

        set
        {
            model = value;
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

    public string KMToDate
    {
        get
        {
            return kmToDate;
        }

        set
        {
            kmToDate = value;
        }
    }

    public string Hand
    {
        get
        {
            return hand;
        }

        set
        {
            hand = value;
        }
    }

    public float PurchaseCost
    {
        get
        {
            return purchaseCost;
        }

        set
        {
            purchaseCost = value;
        }
    }

    public string PurchaseYear
    {
        get
        {
            return purchaseYear;
        }

        set
        {
            purchaseYear = value;
        }
    }

    public TruckTypes TruckType
    {
        get
        {
            return truckType;
        }

        set
        {
            truckType = value;
        }
    }

    public DateTime OnRoadDate
    {
        get
        {
            return onRoadDate;
        }

        set
        {
            onRoadDate = value;
        }
    }

    public DateTime InsuranceExpiredDate
    {
        get
        {
            return insuranceExpiredDate;
        }

        set
        {
            insuranceExpiredDate = value;
        }
    }

    public string Urea
    {
        get
        {
            return urea;
        }

        set
        {
            urea = value;
        }
    }

    public Trucks()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public Trucks(int truckID, int truckLicense, string manufacturer, string model, string active, string kmToDate, string hand, float purchaseCost, string purchaseYear, TruckTypes truckType, string urea, DateTime onRoadDate, DateTime insuranceExpiredDate)
    {
        TruckID = truckID;
        TruckLicense = truckLicense;
        Manufacturer = manufacturer;
        Model = model;
        Active = active;
        KMToDate = kmToDate;
        Hand = hand;
        PurchaseCost = purchaseCost;
        PurchaseYear = purchaseYear;
        TruckType = truckType;
        OnRoadDate = onRoadDate;
        InsuranceExpiredDate = insuranceExpiredDate;
        Urea = urea;
    }

    //without active
    public Trucks(int truckID, int truckLicense, string manufacturer, string model, string kmToDate, string hand, float purchaseCost, string purchaseYear, TruckTypes truckType, string urea, DateTime onRoadDate, DateTime insuranceExpiredDate)
    {
        TruckID = truckID;
        TruckLicense = truckLicense;
        Manufacturer = manufacturer;
        Model = model;
        KMToDate = kmToDate;
        Hand = hand;
        PurchaseCost = purchaseCost;
        PurchaseYear = purchaseYear;
        TruckType = truckType;
        OnRoadDate = onRoadDate;
        InsuranceExpiredDate = insuranceExpiredDate;
        Urea = urea;
    }

    public Trucks(int truckID)
    {
        TruckID = truckID;
    }

    public List<Trucks> getTrucksList(bool active)
    {
        #region DB functions
        string query = "select * from Trucks t inner join TruckTypes tp on t.TruckTypeID = tp.TruckTypeID";
        if (active)
        {
            query += " where Active = 'Y'";
        }

        query += "order by TruckLicense";

        List<Trucks> list = new List<Trucks>();
        DbService db = new DbService();
        DataSet ds = db.GetDataSetByQuery(query);

        foreach (DataRow dr in ds.Tables[0].Rows)
        {
            Trucks tmp = new Trucks();
            tmp.TruckID = (int)dr["TruckID"];
            tmp.TruckLicense = (int)dr["TruckLicense"];
            tmp.Manufacturer = dr["Manufacturer"].ToString();
            tmp.Model = dr["Model"].ToString();
            tmp.Active = dr["Active"].ToString();
            tmp.OnRoadDate = (DateTime)dr["OnRoadDate"];
            tmp.InsuranceExpiredDate = (DateTime)dr["InsuranceExpiredDate"];
            tmp.KMToDate = dr["KMToDate"].ToString();
            tmp.Hand = dr["Hand"].ToString();
            tmp.PurchaseCost = float.Parse(dr["PurchaseCost"].ToString());
            tmp.PurchaseYear = dr["PurchaseYear"].ToString();
            tmp.TruckType = new TruckTypes((int)dr["TruckTypeID"], dr["TruckType"].ToString());
            tmp.Urea = dr["Urea"].ToString();

            list.Add(tmp);
        }
        #endregion

        return list;

    }

    public Trucks getTruck()
    {
        #region DB functions
        string query = "select * from Trucks t inner join TruckTypes tp on t.TruckTypeID = tp.TruckTypeID where TruckID =" + TruckID + "";

        Trucks t = new Trucks();
        DbService db = new DbService();
        DataSet ds = db.GetDataSetByQuery(query);

        foreach (DataRow dr in ds.Tables[0].Rows)
        {
            t.TruckID = (int)dr["TruckID"];
            t.TruckLicense = (int)dr["TruckLicense"];
            t.Manufacturer = dr["Manufacturer"].ToString();
            t.Model = dr["Model"].ToString();
            t.Active = dr["Active"].ToString();
            t.OnRoadDate = (DateTime)dr["OnRoadDate"];
            t.InsuranceExpiredDate = (DateTime)dr["InsuranceExpiredDate"];
            t.KMToDate = dr["KMToDate"].ToString();
            t.Hand = dr["Hand"].ToString();
            t.PurchaseCost = float.Parse(dr["PurchaseCost"].ToString());
            t.PurchaseYear = dr["PurchaseYear"].ToString();
            t.TruckType = new TruckTypes((int)dr["TruckTypeID"], dr["TruckType"].ToString());
            t.Urea = dr["Urea"].ToString();
        }
        #endregion

        return t;
    }

    public void setTruck(string func)
    {
        string sqlFormattedOnRoadDate = OnRoadDate.ToString("yyyy-MM-dd");
        string sqlFormattedInsuranceExpiredDate = InsuranceExpiredDate.ToString("yyyy-MM-dd");

        DbService db = new DbService();
        string query = "";
        if (func == "edit")
        {
            query = "UPDATE Trucks SET TruckLicense = " + TruckLicense + ", Manufacturer = '" + Manufacturer + "', Model = '" + Model + "', KMToDate = '" + KMToDate + "', Hand = '" + Hand + "', PurchaseCost = " + PurchaseCost + ", PurchaseYear = '" + PurchaseYear + "', TruckTypeID = " + TruckType.TruckTypeID + ", Urea = '" + Urea + "', OnRoadDate = '" + sqlFormattedOnRoadDate + "', InsuranceExpiredDate = '" + sqlFormattedInsuranceExpiredDate + "' WHERE TruckID = " + TruckID;
        }
        else if (func == "new")
        {
            query = "insert into Trucks values (" + TruckLicense + ",'" + Manufacturer+"','" + Model + "','Y','" + KMToDate + "','"+Hand+"'," + PurchaseCost + ",'" + PurchaseYear+"'," + TruckType.TruckTypeID + ", '" + Urea + "', '" + sqlFormattedOnRoadDate + "', '" + sqlFormattedInsuranceExpiredDate + "')";
        }
        db.ExecuteQuery(query);
    }


    public void deactivateTruck(string active)
    {
        DbService db = new DbService();
        db.ExecuteQuery("UPDATE Trucks SET Active='" + active + "' WHERE TruckID=" + TruckID);
    }

    public List<Trucks> getAvailableTrucksList(DateTime selecetdDate)
    {
        #region DB functions
        string sqlDate = selecetdDate.ToString("yyyy-MM-dd");

        string query = "select t.TruckID, t.TruckLicense, t.Manufacturer, t.Model, t.Active, t.OnRoadDate, t.InsuranceExpiredDate, t.KMToDate, t.Hand, t.PurchaseCost, t.PurchaseYear, t.Urea, tp.TruckTypeID, tp.TruckType "
+ "from Trucks t inner join "
+ "TruckTypes tp on t.TruckTypeID = tp.TruckTypeID "
+ "left join "
+ "(select * "
+ "from TruckHandlings "
+ "where Active = 'Y' "
+ "and Date = '"+ sqlDate + "') as th on t.TruckID = th.TruckID "
+ "where th.Date is null "
+ "and t.Active = 'Y'";

        List<Trucks> list = new List<Trucks>();
        DbService db = new DbService();
        DataSet ds = db.GetDataSetByQuery(query);

        foreach (DataRow dr in ds.Tables[0].Rows)
        {
            Trucks tmp = new Trucks();
            tmp.TruckID = (int)dr["TruckID"];
            tmp.TruckLicense = (int)dr["TruckLicense"];
            tmp.Manufacturer = dr["Manufacturer"].ToString();
            tmp.Model = dr["Model"].ToString();
            tmp.Active = dr["Active"].ToString();
            tmp.OnRoadDate = (DateTime)dr["OnRoadDate"];
            tmp.InsuranceExpiredDate = (DateTime)dr["InsuranceExpiredDate"];
            tmp.KMToDate = dr["KMToDate"].ToString();
            tmp.Hand = dr["Hand"].ToString();
            tmp.PurchaseCost = float.Parse(dr["PurchaseCost"].ToString());
            tmp.PurchaseYear = dr["PurchaseYear"].ToString();
            tmp.TruckType = new TruckTypes((int)dr["TruckTypeID"], dr["TruckType"].ToString());
            tmp.Urea = dr["Urea"].ToString();

            list.Add(tmp);
        }
        #endregion

        return list;

    }

}