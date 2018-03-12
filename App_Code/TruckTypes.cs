using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;


/// <summary>
/// Summary description for TruckTypes
/// </summary>
public class TruckTypes
{
    int truckTypeID;
    string truckType;

    public int TruckTypeID
    {
        get
        {
            return truckTypeID;
        }

        set
        {
            truckTypeID = value;
        }
    }

    public string TruckType
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

    public TruckTypes()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public TruckTypes(int truckTypeID, string truckType)
    {
        TruckTypeID = truckTypeID;
        TruckType = truckType;
    }

    
            public List<TruckTypes> getTruckTypesList()
    {
        #region DB functions
        string query = "select * from TruckTypes order by TruckType";

        List<TruckTypes> list = new List<TruckTypes>();
        DbService db = new DbService();
        DataSet ds = db.GetDataSetByQuery(query);

        foreach (DataRow dr in ds.Tables[0].Rows)
        {
            TruckTypes tmp = new TruckTypes((int)dr["TruckTypeID"], dr["TruckType"].ToString());
            list.Add(tmp);
        }
        #endregion

        return list;

    }
}