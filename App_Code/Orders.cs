using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;

/// <summary>
/// Summary description for Orders
/// </summary>
public class Orders
{
    int orderID;
    string orderName;
    Customers customer;
    DateTime orderDate;
    OrderStatus orderStatus;
    string comments;
    Services orderService;
    Addresses shipFrom;
    Addresses shipTo;
    float totalPrice;
    float addTime;
    string container;
    DriverLicenseTypes orderLicenseType;
    DriverLicenseTypes orderCertificationType;
    string active;
    string inShift;
    int shiftSort;
    Trucks truck;
    Drivers driver;
    float deliveryDuration;

    public Customers Customer
    {
        get { return customer; }
        set { customer = value; }
    }
    public DateTime OrderDate
    {
        get { return orderDate; }
        set { orderDate = value; }
    }
    public OrderStatus OrderStatus
    {
        get { return orderStatus; }
        set { orderStatus = value; }
    }
    public string Comments
    {
        get { return comments; }
        set { comments = value; }
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
    public float TotalPrice
    {
        get { return totalPrice; }
        set { totalPrice = value; }
    }
    public float AddTime
    {
        get { return addTime; }
        set { addTime = value; }
    }
    public string Container
    {
        get { return container; }
        set { container = value; }
    }
    public string InShift
    {
        get { return inShift; }
        set { inShift = value; }
    }
    public int ShiftSort
    {
        get { return shiftSort; }
        set { shiftSort = value; }
    }
    public Trucks Truck
    {
        get { return truck; }
        set { truck = value; }
    }
    public Drivers Driver
    {
        get { return driver; }
        set { driver = value; }
    }
    public string Active
    {
        get { return active; }
        set { active = value; }
    }
    public int OrderID
    {
        get { return orderID; }
        set { orderID = value; }
    }
    public string OrderName
    {
        get { return orderName; }
        set { orderName = value; }
    }

    public DriverLicenseTypes OrderLicenseType
    {
        get
        {
            return orderLicenseType;
        }

        set
        {
            orderLicenseType = value;
        }
    }

    public DriverLicenseTypes OrderCertificationType
    {
        get
        {
            return orderCertificationType;
        }

        set
        {
            orderCertificationType = value;
        }
    }

    public float DeliveryDuration
    {
        get
        {
            return deliveryDuration;
        }

        set
        {
            deliveryDuration = value;
        }
    }

    public Orders()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public Orders(int orderID, string orderName, Customers customer, DateTime orderDate,OrderStatus orderStatus, string comments, Services orderService, Addresses shipFrom, Addresses shipTo, float totalPrice, float addTime, string container, float deliveryDuration, DriverLicenseTypes orderLicenseType, DriverLicenseTypes orderCertificationType, string inShift, int shiftSort, Trucks truck, string active, Drivers driver)
    {
        OrderID = orderID;
        OrderName = orderName;
        Customer = customer;
        OrderDate = orderDate;
        OrderStatus = orderStatus;
        Comments = comments;
        OrderService = orderService;
        ShipFrom = shipFrom;
        ShipTo = shipTo;
        TotalPrice = totalPrice;
        AddTime = addTime;
        Container = container;
        OrderLicenseType = orderLicenseType;
        OrderCertificationType = orderCertificationType;
        InShift = inShift;
        ShiftSort = shiftSort;
        Truck = truck;
        Active = active;
        Driver = driver;
        DeliveryDuration = deliveryDuration;
    }

    public Orders(int orderID, string orderName, Customers customer, DateTime orderDate, OrderStatus orderStatus, string comments, Services orderService, Addresses shipFrom, Addresses shipTo, float totalPrice, float addTime, string container, float deliveryDuration, DriverLicenseTypes orderLicenseType, DriverLicenseTypes orderCertificationType, string inShift, int shiftSort, Trucks truck, Drivers driver)
    {
        OrderID = orderID;
        OrderName = orderName;
        Customer = customer;
        OrderDate = orderDate;
        OrderStatus = orderStatus;
        Comments = comments;
        OrderService = orderService;
        ShipFrom = shipFrom;
        ShipTo = shipTo;
        TotalPrice = totalPrice;
        AddTime = addTime;
        Container = container;
        OrderLicenseType = orderLicenseType;
        OrderCertificationType = orderCertificationType;
        InShift = inShift;
        ShiftSort = shiftSort;
        Truck = truck;
        Driver = driver;
        DeliveryDuration = deliveryDuration;
    }

    public Orders(int orderID, string orderName, Customers customer, DateTime orderDate, OrderStatus orderStatus, string comments, Services orderService, Addresses shipFrom, Addresses shipTo, float totalPrice, float addTime, string container, float deliveryDuration, DriverLicenseTypes orderLicenseType, DriverLicenseTypes orderCertificationType)
    {
        OrderID = orderID;
        OrderName = orderName;
        Customer = customer;
        OrderDate = orderDate;
        OrderStatus = orderStatus;
        Comments = comments;
        OrderService = orderService;
        ShipFrom = shipFrom;
        ShipTo = shipTo;
        TotalPrice = totalPrice;
        AddTime = addTime;
        Container = container;
        OrderLicenseType = orderLicenseType;
        OrderCertificationType = orderCertificationType;
        DeliveryDuration = deliveryDuration;
    }

    public List<Orders> getOrdersList(bool active, int selectedOrdersStatus, DateTime startDate, DateTime endDate, int selectedCustomer, int selectedService)
    {
        #region DB functions
        string sqlStartDate = startDate.ToString("yyyy-MM-dd");
        string sqlEndtDate = endDate.ToString("yyyy-MM-dd");

        string query = "select * from Orders o inner join OrderStatus os on o.OrderStatusID=os.StatusID";
        if (active)
        {
            query += " where os.Status != 'מבוטלת'";
        }
        if (selectedOrdersStatus != -1)
        {
            query += " and os.StatusID =" + selectedOrdersStatus;
        }
        if (selectedCustomer != -1)
        {
            query += " and o.CustomerID =" + selectedCustomer;
        }
        if (selectedService != -1)
        {
            query += " and o.ServiceID =" + selectedService;
        }
        if (startDate.Year != 1)
        {
            query += " and o.OrderDate >= '" + sqlStartDate + "'";
            if (endDate.Year != 1)
            {
                query += " and o.OrderDate <= '" + sqlEndtDate + "'";
            }
        }

        List<Orders> list = new List<Orders>();
        DbService db = new DbService();
        DataSet ds = db.GetDataSetByQuery(query);

        foreach (DataRow dr in ds.Tables[0].Rows)
        {
            Orders tmp = new Orders();
            tmp.OrderID = (int)dr["OrderID"];
            tmp.OrderName = dr["OrderName"].ToString();
            tmp.OrderDate = (DateTime)dr["OrderDate"];
            tmp.OrderStatus = new OrderStatus((int)dr["OrderStatusID"], dr["Status"].ToString());
            tmp.Comments = dr["Comments"].ToString();
            tmp.TotalPrice = Convert.ToSingle(dr["TotalPrice"]);
            tmp.AddTime = Convert.ToSingle(dr["AddTime"]);
            tmp.Container = dr["Container"].ToString();
            tmp.DeliveryDuration = Convert.ToSingle(dr["DeliveryDuration"]);

            Customers c = new Customers();
            c.CustomerID = (int)dr["CustomerID"];
            tmp.Customer = c.getCustomer();

            Services s = new Services();
            s.ServiceID = (int)dr["ServiceID"];
            tmp.OrderService = s.getService();

            Addresses sf = new Addresses();
            sf.AddressID = (int)dr["ShipFromID"];
            tmp.ShipFrom = sf.getAddress();

            Addresses st = new Addresses();
            st.AddressID = (int)dr["ShipToID"];
            tmp.ShipTo = st.getAddress();

            DriverLicenseTypes dlt = new DriverLicenseTypes();
            dlt.DriverLicenseTypeID = (int)dr["DriverLicenseTypeID"];
            tmp.OrderLicenseType = dlt.getDriverLicenseType();

            DriverLicenseTypes dct = new DriverLicenseTypes();
            dct.DriverLicenseTypeID = (int)dr["DriverCertificationTypeID"];
            tmp.OrderCertificationType = dct.getDriverLicenseType();

            list.Add(tmp);
        }
        #endregion

        return list;
    }

    public List<Orders> getOrdersList(bool active, int selectedOrdersStatus, DateTime startDate, DateTime endDate)
    {
        #region DB functions
        string sqlStartDate = startDate.ToString("yyyy-MM-dd");
        string sqlEndtDate = endDate.ToString("yyyy-MM-dd");

        string query = "select * from Orders o inner join OrderStatus os on o.OrderStatusID=os.StatusID";
        if (active)
        {
            query += " where os.Status != 'מבוטלת'";
        }
        if (selectedOrdersStatus != -1)
        {
            query += " and os.StatusID =" + selectedOrdersStatus;
        }
        if (startDate.Year != 1)
        {
            query += " and o.OrderDate >= '" + sqlStartDate + "'";
            if (endDate.Year != 1)
            {
                query += " and o.OrderDate <= '" + sqlEndtDate + "'";
            }
        }

        List<Orders> list = new List<Orders>();
        DbService db = new DbService();
        DataSet ds = db.GetDataSetByQuery(query);

        foreach (DataRow dr in ds.Tables[0].Rows)
        {
            Orders tmp = new Orders();
            tmp.OrderID = (int)dr["OrderID"];
            tmp.OrderName = dr["OrderName"].ToString();
            tmp.OrderDate = (DateTime)dr["OrderDate"];
            tmp.OrderStatus = new OrderStatus((int)dr["OrderStatusID"], dr["Status"].ToString());
            tmp.Comments = dr["Comments"].ToString();
            tmp.TotalPrice = Convert.ToSingle(dr["TotalPrice"]);
            tmp.AddTime = Convert.ToSingle(dr["AddTime"]);
            tmp.Container = dr["Container"].ToString();
            tmp.DeliveryDuration = Convert.ToSingle(dr["DeliveryDuration"]);

            Customers c = new Customers();
            c.CustomerID = (int)dr["CustomerID"];
            tmp.Customer = c.getCustomer();

            Services s = new Services();
            s.ServiceID = (int)dr["ServiceID"];
            tmp.OrderService = s.getService();

            Addresses sf = new Addresses();
            sf.AddressID = (int)dr["ShipFromID"];
            tmp.ShipFrom = sf.getAddress();

            Addresses st = new Addresses();
            st.AddressID = (int)dr["ShipToID"];
            tmp.ShipTo = st.getAddress();

            DriverLicenseTypes dlt = new DriverLicenseTypes();
            dlt.DriverLicenseTypeID = (int)dr["DriverLicenseTypeID"];
            tmp.OrderLicenseType = dlt.getDriverLicenseType();

            DriverLicenseTypes dct = new DriverLicenseTypes();
            dct.DriverLicenseTypeID = (int)dr["DriverCertificationTypeID"];
            tmp.OrderCertificationType = dct.getDriverLicenseType();

            list.Add(tmp);
        }
        #endregion

        return list;
    }

    public List<Orders> getOrdersForShiftsList(bool active, int selectedOrdersStatus, DateTime startDate, DateTime endDate, int selectedCustomer, int selectedService)
    {
        #region DB functions
        string sqlStartDate = startDate.ToString("yyyy-MM-dd");
        string sqlEndtDate = endDate.ToString("yyyy-MM-dd");

        string query = "select o.OrderID, o.OrderName, o.AddTime,o.DeliveryDuration, o.Container, o.InShift, o.ShiftSort, o.DriverID, o.TruckID, o.OrderDate, o.OrderStatusID, os.Status, o.Comments, o.CustomerID, c.CustomerName, o.ServiceID, s.Service, o.TotalPrice,o.ShipFromID,orig.AddressName 'ShipFrom',origc.Zone 'ShipFromZone',o.ShipToID,dest.AddressName 'ShipTo',destc.Zone 'ShipToZone',o.DriverLicenseTypeID,dl.DriverLicenseTypeName as 'dlName',o.DriverCertificationTypeID,dc.DriverLicenseTypeName as 'dcName' ,dl.DriverLicenseDescription as 'dlDesc',dc.DriverLicenseDescription as 'dcDesc' ,c.PreferedDriverID from Orders o inner join OrderStatus os on o.OrderStatusID=os.StatusID inner join Customers c on c.CustomerID=o.CustomerID inner join Services s on s.ServiceID=o.ServiceID inner join Addresses orig on orig.AddressID = o.ShipFromID inner join City origc on orig.CityID = origc.CityID inner join Addresses dest on dest.AddressID = o.ShipToID inner join City destc on orig.CityID = destc.CityID inner join DriverLicenseTypes dl on o.DriverLicenseTypeID = dl.DriverLicenseTypeID inner join DriverLicenseTypes dc on o.DriverCertificationTypeID = dc.DriverLicenseTypeID inner join Drivers pd on pd.DriverID = c.PreferedDriverID where 1=1";
        if (active)
        {
            query += " and os.Status != 'מבוטלת'";
        }
        if (selectedOrdersStatus != -1)
        {
            query += " and os.StatusID =" + selectedOrdersStatus;
        }
        if (selectedCustomer != -1)
        {
            query += " and o.CustomerID =" + selectedCustomer;
        }
        if (selectedService != -1)
        {
            query += " and o.ServiceID =" + selectedService;
        }
        if (startDate.Year != 1)
        {
            query += " and o.OrderDate >= '" + sqlStartDate + "'";
            if (endDate.Year != 1)
            {
                query += " and o.OrderDate <= '" + sqlEndtDate + "'";
            }
        }

        //query += "and o.OrderDate = '2017-06-01'";

        List<Orders> list = new List<Orders>();
        DbService db = new DbService();
        DataSet ds = db.GetDataSetByQuery(query);

        foreach (DataRow dr in ds.Tables[0].Rows)
        {
            Orders tmp = new Orders();
            tmp.OrderID = (int)dr["OrderID"];
            tmp.OrderName = dr["OrderName"].ToString();
            tmp.OrderDate = (DateTime)dr["OrderDate"];
            tmp.OrderStatus = new OrderStatus((int)dr["OrderStatusID"], dr["Status"].ToString());
            tmp.Comments = dr["Comments"].ToString();
            tmp.AddTime = Convert.ToSingle(dr["AddTime"]);
            tmp.DeliveryDuration = Convert.ToSingle(dr["DeliveryDuration"]);
            tmp.Container = dr["Container"].ToString();
            tmp.InShift = dr["InShift"].ToString();

            if (tmp.InShift == "Y")
            {
                tmp.ShiftSort = (int)dr["ShiftSort"];
                Drivers d = new Drivers();
                d.DriverID = (int)dr["DriverID"];
                tmp.Driver = d;

                Trucks t = new Trucks();
                t.TruckID = (int)dr["TruckID"];
                tmp.Truck = t;
            }

            Customers c = new Customers();
            c.CustomerName = dr["CustomerName"].ToString();
            c.CustomerID = (int)dr["CustomerID"];
            Drivers pd = new Drivers();
            pd.DriverID = (int)dr["PreferedDriverID"];
            c.PreferedDrivers = pd;
            tmp.Customer = c;

            Services s = new Services();
            s.Service = dr["Service"].ToString();
            s.ServiceID = (int)dr["ServiceID"];
            tmp.OrderService = s;

            Addresses sf = new Addresses();
            sf.AddressID = (int)dr["ShipFromID"];
            sf.AddressName = dr["ShipFrom"].ToString();
            City csf = new City();
            csf.Zone = (int)dr["ShipFromZone"];
            sf.CustomerCity = csf;
            tmp.ShipFrom = sf;

            Addresses st = new Addresses();
            st.AddressID = (int)dr["ShipToID"];
            st.AddressName = dr["ShipTo"].ToString();
            City cst = new City();
            cst.Zone = (int)dr["ShipToZone"];
            st.CustomerCity = cst;
            tmp.ShipTo = st;

            DriverLicenseTypes dl = new DriverLicenseTypes();
            dl.DriverLicenseTypeID = (int)dr["DriverLicenseTypeID"];
            dl.DriverLicenseTypeName = dr["dlName"].ToString();
            dl.DriverLicenseTypeDescription = dr["dlDesc"].ToString();
            tmp.OrderLicenseType = dl;

            DriverLicenseTypes dc = new DriverLicenseTypes();
            dc.DriverLicenseTypeID = (int)dr["DriverCertificationTypeID"];
            dc.DriverLicenseTypeName = dr["dcName"].ToString();
            dc.DriverLicenseTypeDescription = dr["dcDesc"].ToString();
            tmp.OrderCertificationType = dl;

            tmp.TotalPrice = Convert.ToSingle(dr["TotalPrice"]);

            list.Add(tmp);
        }
        #endregion

        return list;
    }

    public List<Orders> getDriverOrdersList(int driverID, int func)
    {
        #region DB functions
        string query = "select *"
        + " from Orders o"
        + " inner join OrderStatus os on o.OrderStatusID=os.StatusID"
        + " where 1=1"
        + " and o.InShift='Y'"
        + " and o.DriverID=" + driverID;

        #region selecting Period

        if (func == 1)
        {
            query += " and o.OrderDate = convert(date, GETDATE())";
        }

        if (func == 2)
        {
            query += " and o.OrderDate = convert(date,DATEADD(day, 1, GETDATE()))";
        }

        if (func == 3)
        {
            query += " and o.OrderDate >= convert(date, DATEADD(day, -7, GETDATE())) and o.OrderDate <= convert(date, GETDATE()) ";
        }

        if (func == 4)
        {
            query += " and o.OrderDate >= convert(date, DATEADD(day, -30, GETDATE())) and o.OrderDate <= convert(date, GETDATE()) ";
        }

        #endregion

        query += " order by o.OrderDate desc, o.ShiftSort asc";

        List<Orders> list = new List<Orders>();
        DbService db = new DbService();
        DataSet ds = db.GetDataSetByQuery(query);

        foreach (DataRow dr in ds.Tables[0].Rows)
        {
            Orders tmp = new Orders();
            tmp.OrderID = (int)dr["OrderID"];
            tmp.OrderName = dr["OrderName"].ToString();
            tmp.OrderDate = (DateTime)dr["OrderDate"];
            tmp.OrderStatus = new OrderStatus((int)dr["OrderStatusID"], dr["Status"].ToString());
            tmp.Comments = dr["Comments"].ToString();
            tmp.TotalPrice = Convert.ToSingle(dr["TotalPrice"]);
            tmp.AddTime = Convert.ToSingle(dr["AddTime"]);
            tmp.Container = dr["Container"].ToString();
            tmp.InShift = dr["InShift"].ToString();
            tmp.DeliveryDuration = Convert.ToSingle(dr["DeliveryDuration"]);

            if (tmp.InShift == "Y")
            {
                tmp.ShiftSort = (int)dr["ShiftSort"];
                Drivers d = new Drivers();
                d.DriverID = (int)dr["DriverID"];
                tmp.Driver = d.getDriver();
                Trucks t = new Trucks();
                t.TruckID = (int)dr["TruckID"];
                tmp.Truck = t.getTruck();
            }
            Customers c = new Customers();
            c.CustomerID = (int)dr["CustomerID"];
            tmp.Customer = c.getCustomer();
            Services s = new Services();
            s.ServiceID = (int)dr["ServiceID"];
            tmp.OrderService = s.getService();
            Addresses sf = new Addresses();
            sf.AddressID = (int)dr["ShipFromID"];
            tmp.ShipFrom = sf.getAddress();
            Addresses st = new Addresses();
            st.AddressID = (int)dr["ShipToID"];
            tmp.ShipTo = st.getAddress();
            DriverLicenseTypes dlt = new DriverLicenseTypes();
            dlt.DriverLicenseTypeID = (int)dr["DriverLicenseTypeID"];
            tmp.OrderLicenseType = dlt.getDriverLicenseType();
            DriverLicenseTypes dct = new DriverLicenseTypes();
            dct.DriverLicenseTypeID = (int)dr["DriverCertificationTypeID"];
            tmp.OrderCertificationType = dct.getDriverLicenseType();
            list.Add(tmp);
        }
        #endregion
        return list;
    }

    public void deactivateOrder(int active)
    {
        DbService db = new DbService();
        db.ExecuteQuery("UPDATE Orders SET OrderStatusID=" + active + " WHERE OrderID=" + OrderID);
    }


    public string getLastOrderID()
    {
        int ID;

        string query = "select max(OrderID) as 'id' from Orders";
        Orders o = new Orders();
        DbService db = new DbService();
        DataSet ds = db.GetDataSetByQuery(query);
        DataTable dt = ds.Tables[0];
        ID = Convert.ToInt32(dt.Rows[0]["id"]);

        return Convert.ToString(ID);
    }

    public Orders getOrder()
    {
        #region DB functions
        string query = "select * from Orders o inner join OrderStatus os on o.OrderStatusID=os.StatusID where OrderID =" + OrderID + "";

        Orders o = new Orders();
        DbService db = new DbService();
        DataSet ds = db.GetDataSetByQuery(query);

        foreach (DataRow dr in ds.Tables[0].Rows)
        {
            o.OrderID = (int)dr["OrderID"];
            o.OrderName = dr["OrderName"].ToString();
            o.OrderDate = (DateTime)dr["OrderDate"];
            o.OrderStatus = new OrderStatus((int)dr["OrderStatusID"], dr["Status"].ToString());
            o.Comments = dr["Comments"].ToString();
            o.TotalPrice = Convert.ToSingle(dr["TotalPrice"]);
            o.AddTime = Convert.ToSingle(dr["AddTime"]);
            o.Container = dr["Container"].ToString();
            o.DeliveryDuration = Convert.ToSingle(dr["DeliveryDuration"]);

            Customers c = new Customers();
            c.CustomerID = (int)dr["CustomerID"];
            o.Customer = c.getCustomer();

            Services s = new Services();
            s.ServiceID = (int)dr["ServiceID"];
            o.OrderService = s.getService();

            Addresses sf = new Addresses();
            sf.AddressID = (int)dr["ShipFromID"];
            o.ShipFrom = sf.getAddress();

            Addresses st = new Addresses();
            st.AddressID = (int)dr["ShipToID"];
            o.ShipTo = st.getAddress();

            DriverLicenseTypes dlt = new DriverLicenseTypes();
            dlt.DriverLicenseTypeID = (int)dr["DriverLicenseTypeID"];
            o.OrderLicenseType = dlt.getDriverLicenseType();

            DriverLicenseTypes dct = new DriverLicenseTypes();
            dct.DriverLicenseTypeID = (int)dr["DriverCertificationTypeID"];
            o.OrderCertificationType = dct.getDriverLicenseType();

            //DriverLicenseTypes dlt = new DriverLicenseTypes();
            //o.OrderCertificationType = dlt.getDriverLicenseType(o.OrderCertificationType.DriverLicenseTypeID, "היתר");
            //o.OrderLicenseType = dlt.getDriverLicenseType(o.OrderLicenseType.DriverLicenseTypeID, "רישיון");


        }
        #endregion

        return o;
    }

    public Orders getOrderLean()
    {
        #region DB functions
        string query = "select o.OrderID, o.OrderName, o.OrderDate, o.OrderStatusID, os.Status, o.Comments, o.TotalPrice, o.AddTime, o.Container, o.DeliveryDuration, o.CustomerID, c.CustomerName, o.ServiceID, s.Service, o.ShipFromID, orig.AddressName as 'Origin', o.ShipToID, dest.AddressName as 'Destination', o.DriverLicenseTypeID, lic.DriverLicenseTypeName as 'License', lic.DriverLicenseDescription as 'LicenseDescription', o.DriverCertificationTypeID, cert.DriverLicenseTypeName as 'Certification', cert.DriverLicenseDescription as 'CertificationDescription' from Orders o inner join OrderStatus os on o.OrderStatusID=os.StatusID inner join Customers c on o.CustomerID = c.CustomerID inner join Services s on o.ServiceID = s.ServiceID inner join Addresses orig on o.ShipFromID = orig.AddressID inner join Addresses dest on o.ShipToID = dest.AddressID inner join DriverLicenseTypes lic on o.DriverLicenseTypeID = lic.DriverLicenseTypeID inner join DriverLicenseTypes cert on o.DriverCertificationTypeID = cert.DriverLicenseTypeID where OrderID =" + OrderID + "";

        Orders o = new Orders();
        DbService db = new DbService();
        DataSet ds = db.GetDataSetByQuery(query);

        foreach (DataRow dr in ds.Tables[0].Rows)
        {
            o.OrderID = (int)dr["OrderID"];
            o.OrderName = dr["OrderName"].ToString();
            o.OrderDate = (DateTime)dr["OrderDate"];
            o.OrderStatus = new OrderStatus((int)dr["OrderStatusID"], dr["Status"].ToString());
            o.Comments = dr["Comments"].ToString();
            o.TotalPrice = Convert.ToSingle(dr["TotalPrice"]);
            o.AddTime = Convert.ToSingle(dr["AddTime"]);
            o.Container = dr["Container"].ToString();
            try { o.DeliveryDuration = Convert.ToSingle(dr["DeliveryDuration"]); }
            catch { o.DeliveryDuration = 0; }

            Customers c = new Customers();
            c.CustomerID = (int)dr["CustomerID"];
            c.CustomerName = dr["CustomerName"].ToString();
            o.Customer = c;

            Services s = new Services();
            s.ServiceID = (int)dr["ServiceID"];
            s.Service = dr["Service"].ToString();
            o.OrderService = s;

            Addresses sf = new Addresses();
            sf.AddressID = (int)dr["ShipFromID"];
            sf.AddressName = dr["Origin"].ToString();
            o.ShipFrom = sf;

            Addresses st = new Addresses();
            st.AddressID = (int)dr["ShipToID"];
            st.AddressName = dr["Destination"].ToString();
            o.ShipTo = st;

            DriverLicenseTypes dlt = new DriverLicenseTypes();
            dlt.DriverLicenseTypeID = (int)dr["DriverLicenseTypeID"];
            dlt.DriverLicenseTypeName = dr["License"].ToString();
            dlt.DriverLicenseTypeDescription = dr["LicenseDescription"].ToString();
            o.OrderLicenseType = dlt;

            DriverLicenseTypes dct = new DriverLicenseTypes();
            dct.DriverLicenseTypeID = (int)dr["DriverCertificationTypeID"];
            dct.DriverLicenseTypeName = dr["Certification"].ToString();
            dct.DriverLicenseTypeDescription = dr["CertificationDescription"].ToString();
            o.OrderCertificationType = dct;

            //DriverLicenseTypes dlt = new DriverLicenseTypes();
            //o.OrderCertificationType = dlt.getDriverLicenseType(o.OrderCertificationType.DriverLicenseTypeID, "היתר");
            //o.OrderLicenseType = dlt.getDriverLicenseType(o.OrderLicenseType.DriverLicenseTypeID, "רישיון");


        }
        #endregion

        return o;
    }

    public void setOrder(string func)
    {

        string sqlFormattedDate = OrderDate.ToString("yyyy-MM-dd");

        DbService db = new DbService();
        string query = "";
        if (func == "edit")
        {
            query = "UPDATE Orders SET OrderName = '" + OrderName + "', CustomerID = " + Customer.CustomerID + ", OrderDate = '" + sqlFormattedDate + "', TotalPrice = " + TotalPrice + ", Comments = '" + Comments + "', ShipFromID = " + ShipFrom.AddressID + " , AddTime = " + AddTime + " , Container = '" + Container + "' , DriverLicenseTypeID = " + OrderLicenseType.DriverLicenseTypeID + ", ServiceID = " + OrderService.ServiceID + " , ShipToID = " + ShipTo.AddressID + ", DriverCertificationTypeID = " + OrderCertificationType.DriverLicenseTypeID + " ,DeliveryDuration = " + DeliveryDuration + " WHERE OrderID = " + OrderID;
        }
        else if (func == "new")
        {
            query = "insert into Orders values ('" + OrderName + "'," + Customer.CustomerID + ",'" + sqlFormattedDate + "'," + 1 + ",'" + Comments + "', " + ShipFrom.AddressID + ", " + AddTime + ", '" + Container + "', " + OrderLicenseType.DriverLicenseTypeID + ", '" + "N" + "',null,null,null, " + OrderService.ServiceID + ", " + ShipTo.AddressID + "," + TotalPrice + ", " + OrderCertificationType.DriverLicenseTypeID + ", " + DeliveryDuration + ")";
        }
        db.ExecuteQuery(query);
    }

    public void saveOrderInShift(List<Shifts> allDriversShifts)
    {
        int sortNum = 1;
        foreach (var item in allDriversShifts)
        {
            foreach (var order in item.DriverShiftIDs)
            {
                DbService db = new DbService();
                string query = "";
                query = "UPDATE Orders SET InShift = 'Y', OrderStatusID = 3, ShiftSort = '" + sortNum + "', TruckID = " + item.TruckID + ", DriverID = " + item.DriverID + " WHERE OrderID = " + order;
                sortNum++;
                db.ExecuteQuery(query);
            }
            sortNum = 1;
        }
    }

    public void removeOrderInShift(List<int> bankShift)
    {
        foreach (var order in bankShift)
        {
            DbService db = new DbService();
            string query = "";
            query = "UPDATE Orders SET InShift = 'N',OrderStatusID = 1, ShiftSort = NULL, TruckID = NULL, DriverID = NULL WHERE OrderID = " + order;
            db.ExecuteQuery(query);
        }
    }

    public List<Orders> getOrdersListForView(bool active, int selectedOrdersStatus, DateTime startDate, DateTime endDate, int selectedCustomer, int selectedService)
    {
        #region DB functions
        string sqlStartDate = startDate.ToString("yyyy-MM-dd");
        string sqlEndtDate = endDate.ToString("yyyy-MM-dd");

        string query = "select o.OrderID, o.OrderName, o.OrderDate, os.Status, c.CustomerName, s.Service, o.TotalPrice  from Orders o inner join OrderStatus os on o.OrderStatusID=os.StatusID inner join Customers c on c.CustomerID=o.CustomerID inner join Services s on s.ServiceID=o.ServiceID where 1=1";
        if (active)
        {
            query += " and os.Status != 'מבוטלת'";
        }
        if (selectedOrdersStatus != -1)
        {
            query += " and os.StatusID =" + selectedOrdersStatus;
        }
        if (selectedCustomer != -1)
        {
            query += " and o.CustomerID =" + selectedCustomer;
        }
        if (selectedService != -1)
        {
            query += " and o.ServiceID =" + selectedService;
        }
        if (startDate.Year != 1)
        {
            query += " and o.OrderDate >= '" + sqlStartDate + "'";
            if (endDate.Year != 1)
            {
                query += " and o.OrderDate <= '" + sqlEndtDate + "'";
            }
        }

        List<Orders> list = new List<Orders>();
        DbService db = new DbService();
        DataSet ds = db.GetDataSetByQuery(query);

        foreach (DataRow dr in ds.Tables[0].Rows)
        {
            Orders tmp = new Orders();
            tmp.OrderID = (int)dr["OrderID"];
            tmp.OrderName = dr["OrderName"].ToString();
            tmp.OrderDate = (DateTime)dr["OrderDate"];
            tmp.OrderStatus = new OrderStatus(-1, dr["Status"].ToString());
            Customers c = new Customers();
            c.CustomerName = dr["CustomerName"].ToString();
            tmp.Customer = c;
            Services s = new Services();
            s.Service = dr["Service"].ToString();
            tmp.OrderService = s;
            tmp.TotalPrice = Convert.ToSingle(dr["TotalPrice"]);

            list.Add(tmp);
        }
        #endregion

        return list;
    }

    public List<Orders> getOrdersListForView(bool active, int selectedOrdersStatus, DateTime startDate, DateTime endDate)
    {
        #region DB functions
        string sqlStartDate = startDate.ToString("yyyy-MM-dd");
        string sqlEndtDate = endDate.ToString("yyyy-MM-dd");

        string query = "select o.OrderID, o.OrderName, o.OrderDate, os.Status, c.CustomerName, s.Service, o.TotalPrice  from Orders o inner join OrderStatus os on o.OrderStatusID=os.StatusID inner join Customers c on c.CustomerID=o.CustomerID inner join Services s on s.ServiceID=o.ServiceID where 1=1";
        if (active)
        {
            query += " and os.Status != 'מבוטלת'";
        }
        if (selectedOrdersStatus != -1)
        {
            query += " and os.StatusID =" + selectedOrdersStatus;
        }
        if (startDate.Year != 1)
        {
            query += " and o.OrderDate >= '" + sqlStartDate + "'";
            if (endDate.Year != 1)
            {
                query += " and o.OrderDate <= '" + sqlEndtDate + "'";
            }
        }

        query += "order by OrderName";

        List<Orders> list = new List<Orders>();
        DbService db = new DbService();
        DataSet ds = db.GetDataSetByQuery(query);

        foreach (DataRow dr in ds.Tables[0].Rows)
        {
            Orders tmp = new Orders();
            tmp.OrderID = (int)dr["OrderID"];
            tmp.OrderName = dr["OrderName"].ToString();
            tmp.OrderDate = (DateTime)dr["OrderDate"];
            tmp.OrderStatus = new OrderStatus(-1, dr["Status"].ToString());
            
            tmp.TotalPrice = Convert.ToSingle(dr["TotalPrice"]);

            list.Add(tmp);
        }
        #endregion

        return list;
    }

    public string getOrderName()
    {
        #region DB functions
        DbService dbGetID = new DbService();
        string queryGetID = "select MAX(SUBSTRING ( OrderName ,8 , 5)) as '5Dig','SHN' + SUBSTRING(CONVERT(nvarchar(10), DATEPART(YY, CONVERT(date, GETDATE()))), 3, 2) + CASE WHEN DATEPART(MONTH, CONVERT(date, GETDATE())) < 10 THEN '0' + CONVERT(nvarchar(10), DATEPART(MONTH, CONVERT(date, GETDATE()))) ELSE CONVERT(nvarchar(10), DATEPART(MONTH, CONVERT(date, GETDATE()))) END as 'initialName' from Orders where 1=1 and OrderName like '%' + SUBSTRING(CONVERT(nvarchar(10), DATEPART(YY, CONVERT(date, GETDATE()))), 3, 2) + CASE WHEN DATEPART(MONTH, CONVERT(date, GETDATE())) < 10 THEN '0' + CONVERT(nvarchar(10), DATEPART(MONTH, CONVERT(date, GETDATE()))) ELSE CONVERT(nvarchar(10), DATEPART(MONTH, CONVERT(date, GETDATE()))) END + '%' ";
        DataSet dsGetID = dbGetID.GetDataSetByQuery(queryGetID);
        string Dig = "";
        string initialName = "";
        foreach (DataRow dr in dsGetID.Tables[0].Rows)
        {
            Dig = dr["5Dig"].ToString();
            initialName = dr["initialName"].ToString();
        }
        #endregion

        if (Dig == "")
        {
            Dig = "0";
        }

        int maxNum = Int32.Parse(Dig);
        maxNum++;

        string orderName = "";

        if (maxNum < 10)
        {
            orderName = initialName + "0000" + maxNum.ToString();
        }
        else if (maxNum < 100)
        {
            orderName = initialName + "000" + maxNum.ToString();
        }
        else if (maxNum < 1000)
        {
            orderName = initialName + "00" + maxNum.ToString();
        }
        else if (maxNum < 10000)
        {
            orderName = initialName + "0" + maxNum.ToString();
        }
        else
        {
            orderName = initialName + maxNum.ToString();
        }

        return orderName;
    }
}