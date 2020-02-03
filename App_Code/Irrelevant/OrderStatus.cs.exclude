using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;

/// <summary>
/// Summary description for OrderStatus
/// </summary>
public class OrderStatus
{
    int orderStatusID;
    string orderStatusName;

    public OrderStatus()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public OrderStatus(int orderStatusID, string orderStatusName)
    {
        this.orderStatusID = orderStatusID;
        this.orderStatusName = orderStatusName;
    }

    public int OrderStatusID
    {
        get
        {
            return orderStatusID;
        }

        set
        {
            orderStatusID = value;
        }
    }

    public string OrderStatusName
    {
        get
        {
            return orderStatusName;
        }

        set
        {
            orderStatusName = value;
        }
    }

    public List<OrderStatus> ordersStatusList()
    {
        #region DB functions
        string query = "select * from OrderStatus";

        List<OrderStatus> list = new List<OrderStatus>();
        DbService db = new DbService();
        DataSet ds = db.GetDataSetByQuery(query);

        foreach (DataRow dr in ds.Tables[0].Rows)
        {
            OrderStatus tmp = new OrderStatus();
            tmp.OrderStatusID = (int)dr["StatusID"];
            tmp.OrderStatusName = dr["Status"].ToString();

            list.Add(tmp);
        }
        #endregion

        return list;
    }
}