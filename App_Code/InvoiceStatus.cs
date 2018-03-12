using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;

/// <summary>
/// Summary description for OrderStatus
/// </summary>
public class InvoiceStatus
{
    int invoiceStatusID;
    string invoiceStatusName;

    public InvoiceStatus()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public InvoiceStatus(int invoiceStatusID, string invoiceStatusName)
    {
        this.invoiceStatusID = invoiceStatusID;
        this.invoiceStatusName = invoiceStatusName;
    }

    public int InvoiceStatusID
    {
        get
        {
            return invoiceStatusID;
        }

        set
        {
            invoiceStatusID = value;
        }
    }

    public string InvoiceStatusName
    {
        get
        {
            return invoiceStatusName;
        }

        set
        {
            invoiceStatusName = value;
        }
    }

    public List<InvoiceStatus> invoiceStatusList()
    {
        #region DB functions
        string query = "select * from InvoiceStatus";

        List<InvoiceStatus> list = new List<InvoiceStatus>();
        DbService db = new DbService();
        DataSet ds = db.GetDataSetByQuery(query);

        foreach (DataRow dr in ds.Tables[0].Rows)
        {
            InvoiceStatus tmp = new InvoiceStatus();
            tmp.InvoiceStatusID = (int)dr["StatusID"];
            tmp.InvoiceStatusName = dr["Status"].ToString();

            list.Add(tmp);
        }
        #endregion

        return list;
    }
}