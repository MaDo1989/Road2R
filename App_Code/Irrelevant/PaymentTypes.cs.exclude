using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;

/// <summary>
/// Summary description for PaymentTypes
/// </summary>
public class PaymentTypes
{
    int paymentTypeID;
    string paymentType;

    public int PaymentTypeID
    {
        get
        {
            return paymentTypeID;
        }

        set
        {
            paymentTypeID = value;
        }
    }

    public string PaymentType
    {
        get
        {
            return paymentType;
        }

        set
        {
            paymentType = value;
        }
    }

    public PaymentTypes()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public PaymentTypes(int paymentTypeID, string paymentType)
    {
        PaymentTypeID = paymentTypeID;
        PaymentType = paymentType;
    }

    public List<PaymentTypes> getPaymentTypesList ()
    {
        #region DB functions
        string query = "select * from PaymentTypes order by PaymentType ";

        List<PaymentTypes> list = new List<PaymentTypes>();
        DbService db = new DbService();
        DataSet ds = db.GetDataSetByQuery(query);

        foreach (DataRow dr in ds.Tables[0].Rows)
        {
            PaymentTypes tmp = new PaymentTypes((int)dr["PaymentTypeID"], dr["PaymentType"].ToString());
            list.Add(tmp);
        }
        #endregion

        return list;
    }
}