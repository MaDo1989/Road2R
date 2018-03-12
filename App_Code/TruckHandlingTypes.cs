using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;

/// <summary>
/// Summary description for TruckHandlingTypes
/// </summary>
public class TruckHandlingTypes
{
    int truckHandlingTypeID;
    string truckHandlingType;

    public int TruckHandlingTypeID
    {
        get
        {
            return truckHandlingTypeID;
        }

        set
        {
            truckHandlingTypeID = value;
        }
    }

    public string TruckHandlingType
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

    public TruckHandlingTypes()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public TruckHandlingTypes(int truckHandlingTypeID, string truckHandlingType)
    {
        TruckHandlingTypeID = truckHandlingTypeID;
        TruckHandlingType = truckHandlingType;
    }

    public List<TruckHandlingTypes> getTruckHandlingTypesList()
    {
        #region DB functions
        string query = "select * from TruckHandlingTypes order by TruckHandlingType";

        List<TruckHandlingTypes> list = new List<TruckHandlingTypes>();
        DbService db = new DbService();
        DataSet ds = db.GetDataSetByQuery(query);

        foreach (DataRow dr in ds.Tables[0].Rows)
        {
            TruckHandlingTypes tmp = new TruckHandlingTypes((int)dr["TruckHandlingTypeID"], dr["TruckHandlingType"].ToString());
            list.Add(tmp);
        }
        #endregion

        return list;

    }

}