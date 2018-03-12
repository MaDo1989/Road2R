using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;

/// <summary>
/// Summary description for PriceList
/// </summary>
public class PriceList
{
    int priceListID;
    Addresses origin;
    Addresses destination;
    float price;
    float deliveryTime;
    float deliveryDistance;
    Customers customer;
    Services service;
    string active;

    public int PriceListID
    {
        get
        {
            return priceListID;
        }

        set
        {
            priceListID = value;
        }
    }

    public Addresses Origin
    {
        get
        {
            return origin;
        }

        set
        {
            origin = value;
        }
    }

    public Addresses Destination
    {
        get
        {
            return destination;
        }

        set
        {
            destination = value;
        }
    }

    public float Price
    {
        get
        {
            return price;
        }

        set
        {
            price = value;
        }
    }

    public float DeliveryTime
    {
        get
        {
            return deliveryTime;
        }

        set
        {
            deliveryTime = value;
        }
    }

    public float DeliveryDistance
    {
        get
        {
            return deliveryDistance;
        }

        set
        {
            deliveryDistance = value;
        }
    }

    public Customers Customer
    {
        get
        {
            return customer;
        }

        set
        {
            customer = value;
        }
    }

    public Services Service
    {
        get
        {
            return service;
        }

        set
        {
            service = value;
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

    public PriceList()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public PriceList(int priceListID, Addresses origin, Addresses destination, float price, float deliveryTime, float deliveryDistance, Customers customer, Services service)
    {
        PriceListID = priceListID;
        Origin = origin;
        Destination = destination;
        Price = price;
        DeliveryTime = deliveryTime;
        DeliveryDistance = deliveryDistance;
        Customer = customer;
        Service = service;
    }

    public PriceList(int priceListID, Addresses origin, Addresses destination, float price, float deliveryTime, float deliveryDistance, Customers customer, Services service, string active)
    {
        PriceListID = priceListID;
        Origin = origin;
        Destination = destination;
        Price = price;
        DeliveryTime = deliveryTime;
        DeliveryDistance = deliveryDistance;
        Customer = customer;
        Service = service;
        Active = active;
    }


    public List<PriceList> getPriceListList(bool active)
    {

        #region DB functions
        string query = "select pl.Active, pl.PriceListID ,c.CustomerName ,s.Service ,pl.Price ,o.AddressName as 'origin' ,d.AddressName as 'destination' from PriceList pl inner join Customers c on pl.CustomerID = c.CustomerID inner join Services s on pl.ServiceID = s.ServiceID inner join Addresses o on pl.OriginID = o.AddressID inner join Addresses d on pl.DestinationID = d.AddressID";
        //string query = "select * from PriceList pl";
        if (active)
        {
            query += " where pl.Active = 'Y'";
        }

        List<PriceList> list = new List<PriceList>();
        DbService db = new DbService();
        DataSet ds = db.GetDataSetByQuery(query);

        foreach (DataRow dr in ds.Tables[0].Rows)
        {

            PriceList tmp = new PriceList();
            tmp.PriceListID = (int)dr["PriceListID"];
            //tmp.DeliveryTime = Convert.ToSingle(dr["DeliveryDuration"]);
            //tmp.DeliveryDistance = Convert.ToSingle(dr["DeliveryDistance"]);
            tmp.Price = Convert.ToSingle(dr["Price"]);
            tmp.Active = dr["Active"].ToString();

            Customers c = new Customers();
            //c.CustomerID = (int)dr["CustomerID"];
            //tmp.Customer = c.getCustomer();
            c.CustomerName = dr["CustomerName"].ToString();
            tmp.Customer = c;

            Services s = new Services();
            //s.ServiceID = (int)dr["ServiceID"];
            //tmp.Service = s.getService();
            s.Service = dr["Service"].ToString();
            tmp.Service = s;

            Addresses origin = new Addresses();
            //origin.AddressID = (int)dr["OriginID"];
            //tmp.Origin = origin.getAddress();
            origin.AddressName = dr["origin"].ToString();
            tmp.Origin = origin;

            Addresses destination = new Addresses();
            //destination.AddressID = (int)dr["DestinationID"];
            //tmp.destination = destination.getAddress();
            destination.AddressName = dr["destination"].ToString();
            tmp.Destination = destination;

            list.Add(tmp);
        }
        #endregion

        return list;

    }

    public List<PriceList> getPriceListForView(bool active, int selectedCustomer, int selectedService)
    {
        #region DB functions
        string query = "select pl.Active, pl.PriceListID ,c.CustomerName ,s.Service ,pl.Price ,o.AddressName as 'origin' ,d.AddressName as 'destination' from PriceList pl inner join Customers c on pl.CustomerID = c.CustomerID inner join Services s on pl.ServiceID = s.ServiceID inner join Addresses o on pl.OriginID = o.AddressID inner join Addresses d on pl.DestinationID = d.AddressID";
        //string query = "select * from PriceList pl";
        if (active)
        {
            query += " where pl.Active = 'Y'";
        }
        if (selectedCustomer != -1)
        {
            query += " and pl.CustomerID =" + selectedCustomer;
        }
        if (selectedService != -1)
        {
            query += " and pl.ServiceID =" + selectedService;
        }

        List<PriceList> list = new List<PriceList>();
        DbService db = new DbService();
        DataSet ds = db.GetDataSetByQuery(query);

        foreach (DataRow dr in ds.Tables[0].Rows)
        {

            PriceList tmp = new PriceList();
            tmp.PriceListID = (int)dr["PriceListID"];
            //tmp.DeliveryTime = Convert.ToSingle(dr["DeliveryDuration"]);
            //tmp.DeliveryDistance = Convert.ToSingle(dr["DeliveryDistance"]);
            tmp.Price = Convert.ToSingle(dr["Price"]);
            tmp.Active = dr["Active"].ToString();

            Customers c = new Customers();
            //c.CustomerID = (int)dr["CustomerID"];
            //tmp.Customer = c.getCustomer();
            c.CustomerName = dr["CustomerName"].ToString();
            tmp.Customer = c;

            Services s = new Services();
            //s.ServiceID = (int)dr["ServiceID"];
            //tmp.Service = s.getService();
            s.Service = dr["Service"].ToString();
            tmp.Service = s;

            Addresses origin = new Addresses();
            //origin.AddressID = (int)dr["OriginID"];
            //tmp.Origin = origin.getAddress();
            origin.AddressName = dr["origin"].ToString();
            tmp.Origin = origin;

            Addresses destination = new Addresses();
            //destination.AddressID = (int)dr["DestinationID"];
            //tmp.destination = destination.getAddress();
            destination.AddressName = dr["destination"].ToString();
            tmp.Destination = destination;

            list.Add(tmp);
        }
        #endregion

        return list;
    }


    public PriceList getPriceList()
    {
        #region DB functions
        string query = "select pl.PriceListID, pl.DeliveryDuration, pl.DeliveryDistance, pl.Price, pl.Active, pl.CustomerID, c.CustomerName, pl.ServiceID, s.Service, pl.OriginID, orig.AddressName as 'Origin', pl.DestinationID, dest.AddressName as 'Destination' from PriceList pl inner join Customers c on pl.CustomerID = c.CustomerID inner join Services s on pl.ServiceID = s.ServiceID inner join Addresses orig on pl.OriginID = orig.AddressID inner join Addresses dest on pl.DestinationID = dest.AddressID where pl.PriceListID =" + PriceListID + "";

        PriceList pl = new PriceList();
        DbService db = new DbService();
        DataSet ds = db.GetDataSetByQuery(query);

        foreach (DataRow dr in ds.Tables[0].Rows)
        { 
            pl.PriceListID = (int)dr["PriceListID"];
            pl.DeliveryTime = Convert.ToSingle(dr["DeliveryDuration"]);
            pl.DeliveryDistance = Convert.ToSingle(dr["DeliveryDistance"]);
            pl.Price = Convert.ToSingle(dr["Price"]);
            pl.Active = dr["Active"].ToString();

            Customers c = new Customers();
            c.CustomerID = (int)dr["CustomerID"];
            c.CustomerName = dr["CustomerName"].ToString();
            pl.Customer = c;
            //pl.Customer = c.getCustomer();

            Services s = new Services();
            s.ServiceID = (int)dr["ServiceID"];
            s.Service = dr["Service"].ToString();
            pl.Service = s;
            //pl.Service = s.getService();

            Addresses origin = new Addresses();
            origin.AddressID = (int)dr["OriginID"];
            origin.AddressName = dr["Origin"].ToString();
            pl.Origin = origin;
            //pl.Origin = origin.getAddress();

            Addresses destination = new Addresses();
            destination.AddressID = (int)dr["DestinationID"];
            destination.AddressName = dr["Destination"].ToString();
            pl.destination = destination;
            //pl.destination = destination.getAddress();

        }
        #endregion

        return pl;
    }

    public PriceList getPriceList(bool fromOrders)
    {
        #region DB functions
        string query = "select * from PriceList pl where pl.CustomerID =" + Customer.CustomerID + " and pl.ServiceID =" + Service.ServiceID + " and pl.OriginID =" + Origin.AddressID + " and pl.DestinationID =" + Destination.AddressID + " and Active = 'Y'";

        PriceList pl = new PriceList();
        DbService db = new DbService();
        DataSet ds = db.GetDataSetByQuery(query);

        foreach (DataRow dr in ds.Tables[0].Rows)
        {
            pl.PriceListID = (int)dr["PriceListID"];
            pl.DeliveryTime = Convert.ToSingle(dr["DeliveryDuration"]);
            pl.DeliveryDistance = Convert.ToSingle(dr["DeliveryDistance"]);
            pl.Price = Convert.ToSingle(dr["Price"]);
            pl.Active = dr["Active"].ToString();

            Customers c = new Customers();
            c.CustomerID = (int)dr["CustomerID"];
            pl.Customer = c.getCustomer();

            Services s = new Services();
            s.ServiceID = (int)dr["ServiceID"];
            pl.Service = s.getService();

            Addresses origin = new Addresses();
            origin.AddressID = (int)dr["OriginID"];
            pl.Origin = origin.getAddress();

            Addresses destination = new Addresses();
            destination.AddressID = (int)dr["DestinationID"];
            pl.destination = destination.getAddress();

        }
        #endregion

        return pl;
    }

    public void setPriceList(string func)
    {

        DbService db = new DbService();
        string query = "";
        if (func == "edit")
        {
            query = "UPDATE PriceList SET OriginID = " + Origin.AddressID + ", DestinationID = " + Destination.AddressID + ", CustomerID = " + Customer.CustomerID + ", ServiceID = " + Service.ServiceID + ", DeliveryDuration = " + DeliveryTime + ", DeliveryDistance = " + DeliveryDistance + ", Price = " + Price + " WHERE PriceListID = " + PriceListID;
        }
        else if (func == "new")
        {
            query = "insert into PriceList values (" + Origin.AddressID + "," + Destination.AddressID + "," + Customer.CustomerID + "," + Service.ServiceID + "," + DeliveryTime + "," + DeliveryDistance + "," + Price + ",'Y')";
        }
        db.ExecuteQuery(query);
    }


    public void deactivatePriceList(string active)
    {
        DbService db = new DbService();
        db.ExecuteQuery("UPDATE PriceList SET Active='" + active + "' WHERE PriceListID=" + PriceListID);
    }
}