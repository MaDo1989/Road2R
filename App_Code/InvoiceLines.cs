using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;

/// <summary>
/// Summary description for Orders
/// </summary>
public class InvoiceLines
{
    int invoiceLineId;
    int orderID;
    string orderName;
    Services orderService;
    Addresses shipFrom;
    Addresses shipTo;
    DateTime orderDate;
    float totalPrice;

    public int InvoiceLineId
    {
        get { return invoiceLineId; }
        set { invoiceLineId = value; }
    }

    public int OrderID
    {
        get { return orderID; }
        set { orderID = value; }
    }

    public Services OrderService
    {
        get { return orderService; }
        set { orderService = value; }
    }

    public Addresses ShipFrom
    {
        get { return shipFrom; }
        set { shipFrom = value; }
    }

    public Addresses ShipTo
    {
        get { return shipTo; }
        set { shipTo = value; }
    }

    public DateTime OrderDate
    {
        get { return orderDate; }
        set { orderDate = value; }
    }
    public float TotalPrice
    {
        get { return totalPrice; }
        set { totalPrice = value; }
    }

    public string OrderName
    {
        get
        {
            return orderName;
        }

        set
        {
            orderName = value;
        }
    }

    public InvoiceLines()
    {
        //empty
    }
    public InvoiceLines(int orderID,Services orderService,Addresses shipFrom,Addresses shipTo,DateTime orderDate, float totalPrice)
    {
        OrderID = orderID;
        OrderService = orderService;
        ShipFrom = shipFrom;
        ShipTo = shipTo;
        OrderDate = orderDate;
        TotalPrice = totalPrice;
    }

    public InvoiceLines(int invoiceLineId, int orderID, string orderName, Services orderService, Addresses shipFrom, Addresses shipTo, DateTime orderDate, float totalPrice)
    {
        InvoiceLineId = invoiceLineId;
        OrderID = orderID;
        OrderName = orderName;
        OrderService = orderService;
        ShipFrom = shipFrom;
        ShipTo = shipTo;
        OrderDate = orderDate;
        TotalPrice = totalPrice;
    }
}