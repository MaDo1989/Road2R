using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;

/// <summary>
/// Summary description for Orders
/// </summary>
public class Invoices
{
    int invoiceID;
    string comments;
    DateTime date;
    List<InvoiceLines> invoiceLines;
    InvoiceStatus invoiceStatus;
    Customers invoiceCustomer;


    public List<InvoiceLines> InvoiceLines
    {
        get { return invoiceLines; }
        set { invoiceLines = value; }
    }


    public int InvoiceID
    {
        get { return invoiceID; }
        set { invoiceID = value; }
    }

    public string Comments
    {
        get { return comments; }
        set { comments = value; }
    }

    public DateTime Date
    {
        get { return date; }
        set { date = value; }
    }

    public InvoiceStatus InvoiceStatus
    {
        get { return invoiceStatus; }
        set { invoiceStatus = value; }
    }

    public Customers InvoiceCustomer
    {
        get { return invoiceCustomer; }
        set { invoiceCustomer = value; }
    }

    public Invoices(){
        //empty
    }


    public Invoices(List<InvoiceLines> invoiceLines)
    {
        InvoiceLines = invoiceLines;
    }

    public void setInvoice(int orderID)
    {
        DbService db = new DbService();
        string queryCreate = "insert into Invoices (Date,InvoiceStatusID) VALUES (GETDATE(),1)";
        db.ExecuteQuery(queryCreate);

        DbService dbGetID = new DbService();
        string queryGetID = "select max(InvoiceID) as maxID from Invoices";
        DataSet dsGetID = dbGetID.GetDataSetByQuery(queryGetID);
        int InvoiceID = 0;
        int CustomerID = 0;
        foreach (DataRow dr in dsGetID.Tables[0].Rows) { InvoiceID = (int)dr["maxID"]; CustomerID = (int)dr["CustomerID"]; }

        DbService dbGetOrder = new DbService();
        string queryGetOrder = "select * from Orders where OrderID=" + orderID;
        DataSet dsGetOrder = dbGetOrder.GetDataSetByQuery(queryGetOrder);

        InvoiceLines il = new InvoiceLines();


        foreach (DataRow dr in dsGetOrder.Tables[0].Rows)
        { 
             il.OrderID = (int)dr["OrderID"];

             Services s = new Services();
             s.ServiceID = (int)dr["ServiceID"];
             il.OrderService = s;

             Addresses aFrom = new Addresses();
             aFrom.AddressID = (int)dr["ShipFromID"];
             il.ShipFrom = aFrom;

             Addresses aTo = new Addresses();
             aTo.AddressID = (int)dr["ShipToID"];
             il.ShipTo= aTo;

             il.OrderDate = (DateTime)dr["OrderDate"];

             il.TotalPrice = Convert.ToSingle(dr["TotalPrice"]);
        }
        string sqlFormattedDate = il.OrderDate.ToString("yyyy-MM-dd");

        DbService dbSetInvoice = new DbService();
        string querySetInvoice = "insert into InvoiceLines values (" + InvoiceID + "," + il.OrderService.ServiceID + "," + il.ShipFrom.AddressID + "," + il.ShipTo.AddressID + "," + il.TotalPrice + "," + il.OrderID + ",'" + sqlFormattedDate + "')";
        dbSetInvoice.ExecuteQuery(querySetInvoice);

        DbService dbSetInvoiceCustomer = new DbService();
        string querySetInvoiceCustomer = "insert into Invoices (CustomerID) values (" + CustomerID + ")";
        dbSetInvoice.ExecuteQuery(querySetInvoiceCustomer);

        DbService dbChangeOrderStatus = new DbService();
        string queryChangeOrderStatus = "UPDATE Orders SET OrderStatusID=6 where OrderID=" + orderID;
        dbChangeOrderStatus.ExecuteQuery(queryChangeOrderStatus);

    }

    public Invoices showInvoice(int invoiceID)
    {
        string InvoiceQuery = "" +
        "select " +
        "i.InvoiceID " +
        ",i.Date " +
        ",i.CustomerID " +
        ",c.CustomerName " +
        ",c.CompanyRegistrationNumber " +
        ",c.BillingAddress " +
        ",c.Phone1 " +
        "from Invoices i  " +
        "inner join Customers c on c.CustomerID=i.CustomerID where InvoiceID=" + invoiceID;

        DbService dbGetInvoice = new DbService();
        DataSet dsGetInvoice = dbGetInvoice.GetDataSetByQuery(InvoiceQuery);
        Invoices i = new Invoices();
        foreach (DataRow dr in dsGetInvoice.Tables[0].Rows)
        {
            i.InvoiceID = (int)dr["InvoiceID"];
            i.Date = (DateTime)dr["Date"];

            Customers c = new Customers();
            c.CustomerID = (int)dr["CustomerID"];
            c.CustomerName = dr["CustomerName"].ToString();
            c.RegistrationNumber = dr["CompanyRegistrationNumber"].ToString();
            c.BillingAddress = dr["BillingAddress"].ToString();
            c.Phone1 = dr["Phone1"].ToString();
            i.InvoiceCustomer = c;

        }

        string InvoiceLinesQuery = "" +
        "select " +
        "il.OrderID " +
        ",o.OrderName" +
        ",il.Date as OrderDate " +
        ",a1.AddressName 'From' " +
        ",a2.AddressName 'To' " +
        ",il.ShipFromID " +
        ",il.ShipToID " +
        ",s.Service " +
        ",il.ServiceID " +
        ",il.TotalPrice " +
        "from invoicelines il  " +
        "inner join Addresses a1 on a1.AddressID=il.ShipFromID " +
        "inner join Addresses a2 on a2.AddressID=il.ShipToID " +
        "inner join Orders o on o.OrderID=il.OrderID " +
        "inner join Services s on s.ServiceID=il.ServiceID where InvoiceID=" + invoiceID;

       
        DbService dbGetInvoiceLines = new DbService();
        DataSet dsGetInvoiceLines = dbGetInvoiceLines.GetDataSetByQuery(InvoiceLinesQuery);
        List<InvoiceLines> list = new List<InvoiceLines>();
        foreach (DataRow dr in dsGetInvoiceLines.Tables[0].Rows)
        {
            InvoiceLines tmp = new InvoiceLines();

            tmp.OrderID = (int)dr["OrderID"];
            tmp.OrderName = dr["OrderName"].ToString();
            tmp.OrderDate = (DateTime)dr["OrderDate"];
            tmp.TotalPrice = Convert.ToSingle(dr["TotalPrice"]);

            Addresses from = new Addresses();
            from.AddressName = dr["From"].ToString();
            from.AddressID = (int)dr["ShipFromID"];
            tmp.ShipFrom = from;

            Addresses to = new Addresses();
            to.AddressName = dr["From"].ToString();
            to.AddressID = (int)dr["ShipToID"];
            tmp.ShipTo = to;

            Services s = new Services();
            s.ServiceID = (int)dr["ServiceID"];
            s.Service = dr["Service"].ToString();
            tmp.OrderService = s;

            list.Add(tmp);
        }
        i.InvoiceLines = list;
        return i;
    }

    public List<Invoices> getInvoiceListForView(DateTime startDate, DateTime endDate, int selectedCustomer)
    {
        #region DB functions
        string sqlStartDate = startDate.ToString("yyyy-MM-dd");
        string sqlEndtDate = endDate.ToString("yyyy-MM-dd");

        string query = "select i.*, c.CustomerName from Invoices i inner join Customers c on c.CustomerID=i.CustomerID where 1=1";
        if (selectedCustomer != -1)
        {
            query += " and i.CustomerID =" + selectedCustomer;
        }
        if (startDate.Year != 1)
        {
            query += " and i.Date >= '" + sqlStartDate + "'";
            if (endDate.Year != 1)
            {
                query += " and i.Date <= '" + sqlEndtDate + "'";
            }
        }

        List<Invoices> list = new List<Invoices>();
        DbService db = new DbService();
        DataSet ds = db.GetDataSetByQuery(query);

        foreach (DataRow dr in ds.Tables[0].Rows)
        {
            Invoices tmp = new Invoices();
            tmp.InvoiceID = (int)dr["InvoiceID"];
            tmp.Date = (DateTime)dr["Date"];

            Customers c = new Customers();
            c.CustomerID = (int)dr["CustomerID"];
            c.CustomerName = dr["CustomerName"].ToString();
            tmp.InvoiceCustomer = c;


            list.Add(tmp);
        }
        #endregion

        return list;
    }

    public string checkInvoice(DateTime startDate, DateTime endDate, int selectedCustomer)
    {
        #region DB functions
        string sqlStartDate = startDate.ToString("yyyy-MM-dd");
        string sqlEndtDate = endDate.ToString("yyyy-MM-dd");

        string query = "select count(*) as Count from Orders where 1=1";
        if (selectedCustomer != -1)
        {
            query += " and CustomerID =" + selectedCustomer;
        }
        if (startDate.Year != 1)
        {
            query += " and OrderDate >= '" + sqlStartDate + "'";
            if (endDate.Year != 1)
            {
                query += " and OrderDate <= '" + sqlEndtDate + "'";
            }
        }

        int Check = 0;
        string answer = "";
        DbService db = new DbService();
        DataSet ds = db.GetDataSetByQuery(query);

        foreach (DataRow dr in ds.Tables[0].Rows) { Check = (int)dr["Count"]; }
        #endregion
        if (Check>=1)
        {
            answer = "OK";
        }
        else
	{
            answer="NotOK";
	}

        return answer;
    }

    public int setInvoiceForCustomer(DateTime startDate, DateTime endDate, int selectedCustomer)
    {
        string sqlStartDate = startDate.ToString("yyyy-MM-dd");
        string sqlEndtDate = endDate.ToString("yyyy-MM-dd");
        string orderQuery = "select OrderDate,ServiceID,ShipFromID,ShipToID,TotalPrice,CustomerID,OrderID from Orders O where 1=1 and CustomerID=" + selectedCustomer + " and OrderDate>='" + sqlStartDate + "' and OrderDate<='" + sqlEndtDate + "' and OrderStatusID=6";
        
        DbService dbGetOrder = new DbService();
        DataSet dsGetOrder = dbGetOrder.GetDataSetByQuery(orderQuery);


        DbService db = new DbService();
        string queryCreate = "insert into Invoices (Date,InvoiceStatusID,CustomerID) VALUES (GETDATE(),1," + selectedCustomer + ")";
        db.ExecuteQuery(queryCreate);

        DbService dbGetID = new DbService();
        string queryGetID = "select max(InvoiceID) as maxID from Invoices";
        DataSet dsGetID = dbGetID.GetDataSetByQuery(queryGetID);
        int InvoiceID = 0;
        foreach (DataRow dr in dsGetID.Tables[0].Rows) { InvoiceID = (int)dr["maxID"]; }


        List<InvoiceLines> list = new List<InvoiceLines>();

        foreach (DataRow dr in dsGetOrder.Tables[0].Rows)
        {
            DateTime tmp = (DateTime)dr["OrderDate"];
            string orderDate = tmp.ToString("yyyy-MM-dd");
            DbService dbSetInvoice = new DbService();
            float TotalPrice = Convert.ToSingle(dr["TotalPrice"]);
            string querySetInvoice = "insert into InvoiceLines values (" + InvoiceID + "," + (int)dr["ServiceID"] + "," + (int)dr["ShipFromID"] + "," + (int)dr["ShipToID"] + "," + TotalPrice + "," + (int)dr["OrderID"] + ",'" + orderDate + "')";
            dbSetInvoice.ExecuteQuery(querySetInvoice);

            DbService dbChangeStatus = new DbService();
            string queryChangeStatus = "update Orders SET OrderStatusId=7 where OrderID=" + (int)dr["OrderID"] ;
            dbChangeStatus.ExecuteQuery(queryChangeStatus);

        }

        return InvoiceID;

    }

    public Invoices viewInvoiceForCustomer(DateTime startDate, DateTime endDate, int selectedCustomer)
    {
        string sqlStartDate = startDate.ToString("yyyy-MM-dd");
        string sqlEndtDate = endDate.ToString("yyyy-MM-dd");
        //string orderQuery = "select OrderDate,ServiceID,ShipFromID,ShipToID,TotalPrice,c.CustomerID,c.CustomerName,OrderID from Orders o inner join Customers c on c.CustomerID=o.CustomerID where 1=1 and CustomerID=" + selectedCustomer + " and OrderDate>='" + sqlStartDate + "' and OrderDate<='" + sqlEndtDate + "' and OrderStatusID=6";

        string orderQuery = "select o.OrderID, o.OrderName, o.OrderDate, o.OrderStatusID, os.Status, o.CustomerID, c.CustomerName, o.ServiceID, s.Service, o.TotalPrice,o.ShipFromID,orig.AddressName 'ShipFrom',o.ShipToID,dest.AddressName 'ShipTo' from Orders o inner join OrderStatus os on o.OrderStatusID=os.StatusID inner join Customers c on c.CustomerID=o.CustomerID inner join Services s on s.ServiceID=o.ServiceID inner join Addresses orig on orig.AddressID = o.ShipFromID inner join Addresses dest on dest.AddressID = o.ShipToID where 1=1 and c.CustomerID=" + selectedCustomer + " and OrderDate>='" + sqlStartDate + "' and OrderDate<='" + sqlEndtDate + "' and OrderStatusID=6";
       

        DbService dbGetOrder = new DbService();
        DataSet dsGetOrder = dbGetOrder.GetDataSetByQuery(orderQuery);


        List<InvoiceLines> list = new List<InvoiceLines>();

        Invoices inv = new Invoices();
        

        foreach (DataRow dr in dsGetOrder.Tables[0].Rows)
        {

            Customers c = new Customers();
            c.CustomerName = dr["CustomerName"].ToString();
            c.CustomerID = (int)dr["CustomerID"];

            inv.InvoiceCustomer = c;

            InvoiceLines tmp = new InvoiceLines();
            tmp.OrderID = (int)dr["OrderID"];
            tmp.OrderName = dr["OrderName"].ToString();
            tmp.OrderDate = (DateTime)dr["OrderDate"];

            Services s = new Services();
            s.Service = dr["Service"].ToString();
            s.ServiceID = (int)dr["ServiceID"];
            tmp.OrderService = s;

            Addresses sf = new Addresses();
            sf.AddressID = (int)dr["ShipFromID"];
            sf.AddressName = dr["ShipFrom"].ToString();
            tmp.ShipFrom = sf;

            Addresses st = new Addresses();
            st.AddressID = (int)dr["ShipToID"];
            st.AddressName = dr["ShipTo"].ToString();
            tmp.ShipTo = st;

            tmp.TotalPrice = Convert.ToSingle(dr["TotalPrice"]);

            list.Add(tmp);
        }

        inv.InvoiceLines = list;

        return inv;

    }
}