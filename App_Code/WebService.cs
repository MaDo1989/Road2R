using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Services;
using System.Globalization;
using System.Web.Script.Services;

/// <summary>
/// Summary description for WebService
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
[System.Web.Script.Services.ScriptService]
public class WebService : System.Web.Services.WebService
{

    public WebService()
    {
        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    //----------------------Road to decovery-----------------------------------------------



    [WebMethod]
    public string getPatients(bool active)
    {
        JavaScriptSerializer j = new JavaScriptSerializer();
        Patient c = new Patient();
        List<Patient> patientsList = c.getPatientsList(active);
        return j.Serialize(patientsList);
    }

    [WebMethod]
    public string getPatientEscorted(string displayName)
    {
        JavaScriptSerializer j = new JavaScriptSerializer();
        Patient c = new Patient();
        List<Escorted> escortedsList = c.getescortedsList(displayName);
        return j.Serialize(escortedsList);
    }

    [WebMethod]
    public void deactivateEscorted(string displayName, string active)
    {
        Escorted c = new Escorted();
        c.DisplayName = displayName;
        c.deactivateEscorted(active);
    }


    [WebMethod]
    public void deactivatePatient(string displayName, string active)
    {
        Patient c = new Patient();
        c.DisplayName = displayName;
        c.deactivatePatient(active);
    }

    [WebMethod]
    public void setPatient(Patient patient, string func)
    {
        Patient p = new Patient();
        p = patient;
        p.setPatient(func);

    }
    
    [WebMethod]
    public void setEscorted(Escorted escorted, string func)
    {
        Escorted p = new Escorted();
        p = escorted;
        p.setEscorted(func);

    }

    [WebMethod]
    public string getEscorted(string displayName)
    {
        JavaScriptSerializer j = new JavaScriptSerializer();
        Escorted p = new Escorted();
        p.DisplayName = displayName;
        Escorted escorted = p.getEscorted();
        return j.Serialize(escorted);
    }
    [WebMethod]
    public string getPatient(string displayName)
    {
        JavaScriptSerializer j = new JavaScriptSerializer();
        Patient p = new Patient();
        p.DisplayName = displayName;
        Patient patient = p.getPatient();
        return j.Serialize(patient);
    }

    
    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string getRidePat(string test)
    {        
        RidePat rp = new RidePat();
       List<RidePat> r= rp.GetRidePat();
        JavaScriptSerializer j = new JavaScriptSerializer();
        return j.Serialize(r);
    }






    #region volunteers functions
    [WebMethod]
    public void deactivateVolunteer(string displayName, string active)
    {
        Volunteer c = new Volunteer();
        c.DisplayName = displayName;
        c.deactivateCustomer(active);
    }
    [WebMethod]
    public void setVolunteer(Volunteer volunteer, string func)
    {
        Volunteer v = new Volunteer();
        v = volunteer;
        v.setVolunteer(func);

    }


    [WebMethod]
    public string getVolunteers(bool active)
    {
        JavaScriptSerializer j = new JavaScriptSerializer();
        Volunteer c = new Volunteer();
        List<Volunteer> volunteersList = c.getVolunteersList(active);
        return j.Serialize(volunteersList);
    }



    [WebMethod]
    public string getVolunteer(string displayName)
    {
        JavaScriptSerializer j = new JavaScriptSerializer();
        Volunteer v = new Volunteer();
        v.DisplayName = displayName;
        Volunteer volunteer = v.getVolunteer();
        return j.Serialize(volunteer);
    }
    #endregion

    #region Destinations functions
    [WebMethod]
    public string getDestinationView(bool active)
    {
        JavaScriptSerializer j = new JavaScriptSerializer();
        Destination d = new Destination();
        List<Destination> destinationsList = d.getDestinationsListForView(active);
        return j.Serialize(destinationsList);
    }

    [WebMethod]
    public string getHospitalView(bool active)
    {
        JavaScriptSerializer j = new JavaScriptSerializer();
        Destination d = new Destination();
        List<Destination> hospitalList = d.getHospitalListForView(active);
        return j.Serialize(hospitalList);
    }

    [WebMethod]
    public string getBarrierView(bool active)
    {
        JavaScriptSerializer j = new JavaScriptSerializer();
        Destination d = new Destination();
        List<Destination> hospitalList = d.getBarrierListForView(active);
        return j.Serialize(hospitalList);
    }
    #endregion


    ///--------------------finish Road to decovery----------------------------------------

    #region login functions
    [WebMethod]
    public string loginUser(string uName, string password)
    {
        JavaScriptSerializer j = new JavaScriptSerializer();
        User u = new User(uName,password);
        bool userInDB = u.CheckLoginDetails();
        return j.Serialize(userInDB);
    }

    [WebMethod]
    public string loginDriver(string uName, string password)
    {
        JavaScriptSerializer j = new JavaScriptSerializer();
        Drivers d = new Drivers(uName, password);
        d = d.CheckLoginDetails();
        return j.Serialize(d);
    }
    #endregion

    #region orders functions

    
    [WebMethod]
    public string getOrderName()
    {
        JavaScriptSerializer j = new JavaScriptSerializer();
        Orders o = new Orders();
        string orderName = o.getOrderName();
        return j.Serialize(orderName);
    }

    //[WebMethod]
    //public string getOrders(bool active, int selectedOrdersStatus, string startDate, string endDate, int selectedCustomer, int selectedService)
    //{
    //    if (startDate == "")
    //    {
    //        startDate = "01/01/1990";
    //    }
    //    if (endDate == "")
    //    {
    //        endDate = "01/01/2990";
    //    }
    //    DateTime startDatedt = DateTime.ParseExact(startDate, "dd/MM/yyyy", null);
    //    DateTime endDatedt = DateTime.ParseExact(endDate, "dd/MM/yyyy", null);

    //    JavaScriptSerializer j = new JavaScriptSerializer();
    //    Orders o = new Orders();
    //    List<Orders> ordersList = o.getOrdersList(active, selectedOrdersStatus, startDatedt, endDatedt, selectedCustomer, selectedService);
    //    return j.Serialize(ordersList);
    //}

    //[WebMethod]
    //public string getOrdersView(bool active, int selectedOrdersStatus, string startDate, string endDate, int selectedCustomer, int selectedService)
    //{
    //    if (startDate == "")
    //    {
    //        startDate = "01/01/1990";
    //    }
    //    if (endDate == "")
    //    {
    //        endDate = "01/01/2990";
    //    }
    //    DateTime startDatedt = DateTime.ParseExact(startDate, "dd/MM/yyyy", null);
    //    DateTime endDatedt = DateTime.ParseExact(endDate, "dd/MM/yyyy", null);

    //    JavaScriptSerializer j = new JavaScriptSerializer();
    //    Orders o = new Orders();
    //    List<Orders> ordersList = o.getOrdersListForView(active, selectedOrdersStatus, startDatedt, endDatedt, selectedCustomer, selectedService);
    //    return j.Serialize(ordersList);
    //}

    //[WebMethod]
    //public string getOrdersDDL(bool active, int selectedOrdersStatus, string startDate, string endDate)
    //{
    //    if (startDate == "")
    //    {
    //        startDate = "01/01/1990";
    //    }
    //    if (endDate == "")
    //    {
    //        endDate = "01/01/2990";
    //    }
    //    DateTime startDatedt = DateTime.ParseExact(startDate, "dd/MM/yyyy", null);
    //    DateTime endDatedt = DateTime.ParseExact(endDate, "dd/MM/yyyy", null);

    //    JavaScriptSerializer j = new JavaScriptSerializer();
    //    Orders o = new Orders();
    //    List<Orders> ordersList = o.getOrdersListForView(active, selectedOrdersStatus, startDatedt, endDatedt);
    //    return j.Serialize(ordersList);
    //}

    //[WebMethod]
    //public string getOrders(bool active, int selectedOrdersStatus, string startDate, string endDate)
    //{
    //    if (startDate == "")
    //    {
    //        startDate = "01/01/1990";
    //    }
    //    if (endDate == "")
    //    {
    //        endDate = "01/01/2990";
    //    }
    //    DateTime startDatedt = DateTime.ParseExact(startDate, "dd/MM/yyyy", null);
    //    DateTime endDatedt = DateTime.ParseExact(endDate, "dd/MM/yyyy", null);

    //    JavaScriptSerializer j = new JavaScriptSerializer();
    //    Orders o = new Orders();
    //    List<Orders> ordersList = o.getOrdersList(active, selectedOrdersStatus, startDatedt, endDatedt);
    //    return j.Serialize(ordersList);
    //}

    //[WebMethod]
    //public string getOrdersForShifts(bool active, int selectedOrdersStatus, string startDate, string endDate, int selectedCustomer, int selectedService)
    //{
    //    if (startDate == "")
    //    {
    //        startDate = "01/01/1990";
    //    }
    //    if (endDate == "")
    //    {
    //        endDate = "01/01/2990";
    //    }
    //    DateTime startDatedt = DateTime.ParseExact(startDate, "dd/MM/yyyy", null);
    //    DateTime endDatedt = DateTime.ParseExact(endDate, "dd/MM/yyyy", null);

    //    JavaScriptSerializer j = new JavaScriptSerializer();
    //    Orders o = new Orders();
    //    List<Orders> ordersList = o.getOrdersForShiftsList(active, selectedOrdersStatus, startDatedt, endDatedt, selectedCustomer, selectedService);
    //    return j.Serialize(ordersList);
    //}

    [WebMethod]
    public string getDriverOrders(int driverID, int func)
    {
        JavaScriptSerializer j = new JavaScriptSerializer();
        Orders o = new Orders();
        List<Orders> ordersList = o.getDriverOrdersList(driverID, func);
        return j.Serialize(ordersList);
    }


    [WebMethod]
    public string getOrder(int orderID)
    {
        JavaScriptSerializer j = new JavaScriptSerializer();
        Orders o = new Orders();
        o.OrderID = orderID;
        Orders order = o.getOrder();
        return j.Serialize(order);
    }

    [WebMethod]
    public string copyOrder(int orderID, string orderName)
    {
        Orders o = new Orders();
        o.OrderID = orderID;
        Orders order = o.getOrder();
        order.OrderID = -1;
        order.OrderName = orderName;
        order.setOrder("new");

        Orders lastOrder = new Orders();
        string newOrderID = lastOrder.getLastOrderID();

        return newOrderID;
    }

    [WebMethod]
    public string getOrderLean(int orderID)
    {
        JavaScriptSerializer j = new JavaScriptSerializer();
        Orders o = new Orders();
        o.OrderID = orderID;
        Orders order = o.getOrderLean();
        return j.Serialize(order);
    }

    [WebMethod]
    public void setOrder(int orderID, string orderName, int customerID, string orderDate, int orderStatusID, string comments, int orderServiceID, int shipFromID, int shipToID, float totalPrice, float addTime, string container, float deliveryDuration, int orderLicenseTypeID, int orderCertificationTypeID, string func)
    {
        //, int inShift, int shiftSort, int truckID, int driverID, 
        DateTime dt;
        if (orderDate.Contains("."))
        {
            dt = DateTime.Parse(orderDate);
        }
        else
        {
            dt = DateTime.ParseExact(orderDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
        }

        Customers customer = new Customers();
        customer.CustomerID = customerID;

        OrderStatus orderStatus = new OrderStatus();
        orderStatus.OrderStatusID = orderStatusID;

        Services orderService = new Services();
        orderService.ServiceID = orderServiceID;

        Addresses shipFrom = new Addresses();
        shipFrom.AddressID = shipFromID;

        Addresses shipTo = new Addresses();
        shipTo.AddressID = shipToID;

        DriverLicenseTypes orderLicenseType = new DriverLicenseTypes();
        orderLicenseType.DriverLicenseTypeID = orderLicenseTypeID;

        DriverLicenseTypes orderCertificationType = new DriverLicenseTypes();
        orderCertificationType.DriverLicenseTypeID = orderCertificationTypeID;

        //Drivers noDriver = new Drivers();
        //noDriver.DriverID = driverID;

        //Trucks noTruck = new Trucks();
        //noTruck.TruckID = truckID;

        //, inShift, shiftSort, noTruck, noDriver
        Orders o = new Orders(orderID, orderName, customer, dt, orderStatus, comments, orderService, shipFrom, shipTo, totalPrice, addTime, container, deliveryDuration, orderLicenseType, orderCertificationType);
        o.setOrder(func);
   
    }

    [WebMethod]
    public void deactivateOrders(int orderID, int active)
    {
        Orders o = new Orders();
        o.OrderID = orderID;
        o.deactivateOrder(active);
    }
    #endregion

    #region invoice functions
    [WebMethod]
    public void setInvoice(int orderID)
    {
        Invoices i = new Invoices();
        i.setInvoice(orderID);
    }


    [WebMethod]
    public string showInvoice(int invoiceID)
    {
        JavaScriptSerializer j = new JavaScriptSerializer();
        Invoices os = new Invoices();
        Invoices invoiceData = os.showInvoice(invoiceID);
        return j.Serialize(invoiceData);
    }


    [WebMethod]
    public string getInvoiceStatus()
    {
        JavaScriptSerializer j = new JavaScriptSerializer();
        InvoiceStatus os = new InvoiceStatus();
        List<InvoiceStatus> invoiceStatusList = os.invoiceStatusList();
        return j.Serialize(invoiceStatusList);
    }

    [WebMethod]
    public string getInvoicesView(string startDate, string endDate, int selectedCustomer)
    {
        if (startDate == "")
        {
            startDate = "01/01/1990";
        }
        if (endDate == "")
        {
            endDate = "01/01/2990";
        }
        DateTime startDatedt = DateTime.ParseExact(startDate, "dd/MM/yyyy", null);
        DateTime endDatedt = DateTime.ParseExact(endDate, "dd/MM/yyyy", null);

        JavaScriptSerializer j = new JavaScriptSerializer();
        Invoices o = new Invoices();
        List<Invoices> invoicesList = o.getInvoiceListForView(startDatedt, endDatedt, selectedCustomer);
        return j.Serialize(invoicesList);
    }

    [WebMethod]
    public string createInvoice(string startDate, string endDate, int selectedCustomer)
    {
        if (startDate == "")
        {
            startDate = "01/01/1990";
        }
        if (endDate == "")
        {
            endDate = "01/01/2990";
        }
        DateTime startDatedt = DateTime.ParseExact(startDate, "dd/MM/yyyy", null);
        DateTime endDatedt = DateTime.ParseExact(endDate, "dd/MM/yyyy", null);

        JavaScriptSerializer j = new JavaScriptSerializer();
        Invoices i = new Invoices();
        int invoiceID = i.setInvoiceForCustomer(startDatedt, endDatedt, selectedCustomer);
        return j.Serialize(invoiceID);
    }

    [WebMethod]
    public string viewPreviewInvoice(string startDate, string endDate, int selectedCustomer)
    {
        if (startDate == "")
        {
            startDate = "01/01/1990";
        }
        if (endDate == "")
        {
            endDate = "01/01/2990";
        }
        DateTime startDatedt = DateTime.ParseExact(startDate, "dd/MM/yyyy", null);
        DateTime endDatedt = DateTime.ParseExact(endDate, "dd/MM/yyyy", null);

        JavaScriptSerializer j = new JavaScriptSerializer();
        Invoices i = new Invoices();
        Invoices invoiceList = i.viewInvoiceForCustomer(startDatedt, endDatedt, selectedCustomer);
        return j.Serialize(invoiceList);
    }

    [WebMethod]
    public string checkInvoice(string startDate, string endDate, int selectedCustomer)
    {
        if (startDate == "")
        {
            startDate = "01/01/1990";
        }
        if (endDate == "")
        {
            endDate = "01/01/2990";
        }
        DateTime startDatedt = DateTime.ParseExact(startDate, "dd/MM/yyyy", null);
        DateTime endDatedt = DateTime.ParseExact(endDate, "dd/MM/yyyy", null);

        JavaScriptSerializer j = new JavaScriptSerializer();
        Invoices o = new Invoices();
        string answer = o.checkInvoice(startDatedt, endDatedt, selectedCustomer);
        return j.Serialize(answer);
    }
    #endregion 

    #region orders status functions
    [WebMethod]
    public string getOrdersStatus()
    {
        JavaScriptSerializer j = new JavaScriptSerializer();
        OrderStatus os = new OrderStatus();
        List<OrderStatus> ordersStatusList = os.ordersStatusList();
        return j.Serialize(ordersStatusList);
    }
    #endregion

    #region customers functions
    [WebMethod]
    public string getCustomers(bool active)
    {
        JavaScriptSerializer j = new JavaScriptSerializer();
        Customers c = new Customers();
        List<Customers> customersList = c.getCustomersList(active);
        return j.Serialize(customersList);
    }

    [WebMethod]
    public string getCustomer(int customerID)
    {
        JavaScriptSerializer j = new JavaScriptSerializer();
        Customers c = new Customers();
        c.CustomerID = customerID;
        Customers customer = c.getCustomer();
        return j.Serialize(customer);
    }

    [WebMethod]
    public void setCustomer(int customerID, string customerName, string customerContactName, string accountID, string phone1, string phone2, string email, int preferedDriversID, int paymentTypeID, string comments, string registrationNumber, string billingAddress, string func, List<Addresses> addressList, int[] prevAddressesID)
    {
        Drivers preferedDriver = new Drivers();
        preferedDriver.DriverID = preferedDriversID;
        Customers c = new Customers(customerID, customerName, customerContactName, accountID, phone1, phone2, email, preferedDriver, new PaymentTypes(paymentTypeID, ""), comments, registrationNumber, billingAddress);
        c.setCustomer(func);

        List<int> currentAddressesID = new List<int>();
        //set Addresses
        foreach (var item in addressList)
        {
            if (item.AddressID != 0)
            {

                City ct = item.CustomerCity;
                item.CustomerCity.CityID = ct.getCityID();

                if (customerID == -1)
                {
                    Customers lastCustomer = new Customers();
                    customerID = lastCustomer.getLastCustomerID();
                }

                item.setAddress(customerID);

                if (item.AddressID != -1)
                {
                    currentAddressesID.Add(item.AddressID);
                }
            }
        }

        if (currentAddressesID.Count != prevAddressesID.Length)
        {
            List<int> deactiveAddresses = new List<int>();
            int count = currentAddressesID.Count;

            for (int i = 0; i < prevAddressesID.Length; i++)
            {
                count = currentAddressesID.Count;
                foreach (var item in currentAddressesID)
                {
                    if (prevAddressesID[i] != item)
                    {
                        count--;
                    }
                    else
                    {
                        break;
                    }
                }
                if (count == 0)
                {
                    deactiveAddresses.Add(prevAddressesID[i]);
                }
            }
            Addresses a = new Addresses();
            a.deactivateAddresses(deactiveAddresses);
        }
    }


    [WebMethod]
    public void deactivateCustomers(int customerID, string active)
    {
        Customers c = new Customers();
        c.CustomerID = customerID;
        c.deactivateCustomer(active);
    }
    #endregion


    #region services functions
    [WebMethod]
    public string getServices(bool active)
    {
        JavaScriptSerializer j = new JavaScriptSerializer();
        Services s = new Services();
        List<Services> servicesList = s.getServicesList(active);
        return j.Serialize(servicesList);
    }

    [WebMethod]
    public string getServicesDDL()
    {
        JavaScriptSerializer j = new JavaScriptSerializer();
        Services s = new Services();
        List<Services> servicesList = s.getServicesList(true);
        return j.Serialize(servicesList);
    }

    [WebMethod]
    public string getService(int serviceID)
    {
        JavaScriptSerializer j = new JavaScriptSerializer();
        Services s = new Services();
        s.ServiceID = serviceID;
        Services service = s.getService();
        return j.Serialize(service);
    }

    [WebMethod]
    public void setService(int serviceID, string service, string serviceDescription, float price, string func)
    {
        Services s = new Services(serviceID, service, serviceDescription, price);
        s.setService(func);
    }


    [WebMethod]
    public void deactivateService(int serviceID, string active)
    {
        Services s = new Services();
        s.ServiceID = serviceID;
        s.deactivateService(active);
    }
    #endregion

    #region trucks functions
    [WebMethod]
    public string getTrucks(bool active)
    {
        JavaScriptSerializer j = new JavaScriptSerializer();
        Trucks t = new Trucks();
        List<Trucks> trucksList = t.getTrucksList(active);
        return j.Serialize(trucksList);
    }

    [WebMethod]
    public string getTruck(int truckID)
    {
        JavaScriptSerializer j = new JavaScriptSerializer();
        Trucks t = new Trucks();
        t.TruckID = truckID;
        Trucks truck = t.getTruck();
        return j.Serialize(truck);
    }

    [WebMethod]
    public void setTruck(int truckID, int truckLicense, string manufacturer, string model, string kmToDate, string hand, float purchaseCost, string purchaseYear, int truckTypeID, string urea, string onRoadDate, string insuranceExpiredDate, string func)
    {
        DateTime dOnRoad;
        if (onRoadDate.Contains("."))
        {
            dOnRoad = DateTime.Parse(onRoadDate);
        }
        else
        {
            dOnRoad = DateTime.ParseExact(onRoadDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

        }

        DateTime dInsurance;
        if (insuranceExpiredDate.Contains("."))
        {
            dInsurance = DateTime.Parse(insuranceExpiredDate);
        }
        else
        {
            dInsurance = DateTime.ParseExact(insuranceExpiredDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

        }

        Trucks t = new Trucks(truckID, truckLicense, manufacturer, model, kmToDate, hand, purchaseCost, purchaseYear, new TruckTypes(truckTypeID,""), urea, dOnRoad, dInsurance);
        t.setTruck(func);
    }

    [WebMethod]
    public void deactivateTruck(int truckID, string active)
    {
        Trucks t = new Trucks();
        t.TruckID = truckID;
        t.deactivateTruck(active);
    }

    [WebMethod]
    public string getAvailableTrucks(string chosenDate)
    {
        DateTime selecetdDate = DateTime.ParseExact(chosenDate, "dd/MM/yyyy", null);

        JavaScriptSerializer j = new JavaScriptSerializer();
        Trucks t = new Trucks();
        List<Trucks> trucksList = t.getAvailableTrucksList(selecetdDate);
        return j.Serialize(trucksList);
    }
    #endregion

    #region truckTypes functions
    [WebMethod]
    public string getTruckTypes()
    {
        JavaScriptSerializer j = new JavaScriptSerializer();
        TruckTypes tp = new TruckTypes();
        List<TruckTypes> trucksTypesList = tp.getTruckTypesList();
        return j.Serialize(trucksTypesList);
    }
    #endregion

    #region truckHandlingTypes functions
    [WebMethod]
    public string getTruckHandlingTypes()
    {
        JavaScriptSerializer j = new JavaScriptSerializer();
        TruckHandlingTypes tht = new TruckHandlingTypes();
        List<TruckHandlingTypes> truckHandlingTypesList = tht.getTruckHandlingTypesList();
        return j.Serialize(truckHandlingTypesList);
    }
    #endregion

    #region truckHandlings functions
    [WebMethod]
    public string getTruckHandlings(bool active, int selectedTruckID, string startDate, string endDate)
    {
        if (startDate == "")
        {
            startDate = "01/01/1990";
        }

        if (endDate == "")
        {
            endDate = "01/01/2990";
        }

        DateTime sdt = DateTime.ParseExact(startDate, "dd/MM/yyyy", null);
        DateTime edt = DateTime.ParseExact(endDate, "dd/MM/yyyy", null);
        JavaScriptSerializer j = new JavaScriptSerializer();
        TruckHandlings t = new TruckHandlings();
        List<TruckHandlings> trucksHandlingsList = t.getTruckHandlingsList(active, selectedTruckID, sdt, edt);
        return j.Serialize(trucksHandlingsList);
    }

    [WebMethod]
    public string getTruckHandlingsPeriod(bool active, int selectedTruckID, int func)
    {
        JavaScriptSerializer j = new JavaScriptSerializer();
        TruckHandlings t = new TruckHandlings();
        List<TruckHandlings> trucksHandlingsList = t.getTruckHandlingsPeriodList(active, selectedTruckID ,func);
        return j.Serialize(trucksHandlingsList);
    }

    [WebMethod]
    public string getTruckHandling(int truckHandlingID)
    {
        JavaScriptSerializer j = new JavaScriptSerializer();
        TruckHandlings t = new TruckHandlings();
        t.TruckHandlingID = truckHandlingID;
        TruckHandlings truckHandling = t.getTruckHandling();
        return j.Serialize(truckHandling);
    }

    [WebMethod]
    public string getTruckLicenses(bool active)
    {
        JavaScriptSerializer j = new JavaScriptSerializer();
        TruckHandlings t = new TruckHandlings();
        List<TruckHandlings> trucksHandlingsList = t.getTruckLicenses(active);
        return j.Serialize(trucksHandlingsList);
    }

    [WebMethod]
    public void setTruckHandlingApp(int truckHandlingID, string truckHandlingDescription, string handlingProvider, int imgID, float cost, string date, int truckHandlingTypeID, int truckID, string func)
    {
        DateTime dt;
        if (date.Contains(".") || date.Contains("-"))
        {
            dt = DateTime.Parse(date);
        }
        else
        {
            dt = DateTime.ParseExact(date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
        }
        TruckHandlings t = new TruckHandlings(truckHandlingID, truckHandlingDescription, handlingProvider, cost, dt, new TruckHandlingTypes(truckHandlingTypeID, ""), new Trucks(truckID), imgID);
        t.setTruckHandlingApp(func);
    }


    [WebMethod]
    public void setTruckHandling(int truckHandlingID, string truckHandlingDescription, string handlingProvider, string url, float cost, string date, int truckHandlingTypeID, int truckID, string func)
    {
        DateTime dt;
        if (date.Contains(".") || date.Contains("-"))
        {
            dt = DateTime.Parse(date);
        }
        else
        {
            dt = DateTime.ParseExact(date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
        }
        TruckHandlings t = new TruckHandlings(truckHandlingID, truckHandlingDescription, handlingProvider, url, cost, dt, new TruckHandlingTypes(truckHandlingTypeID, ""), new Trucks(truckID));
        t.setTruckHandling(func);
    }

    [WebMethod]
    public void deactivateTruckHandlings(int truckHandlingID, string active)
    {
        TruckHandlings t = new TruckHandlings();
        t.TruckHandlingID = truckHandlingID;
        t.deactivateTruckHandlings(active);
    }
    #endregion

    #region drivers functions
    [WebMethod]
    public string getDrivers(bool active)
    {
        JavaScriptSerializer j = new JavaScriptSerializer();
        Drivers d = new Drivers();
        List<Drivers> driversList = d.getDriversList(active);
        return j.Serialize(driversList);
    }

    [WebMethod]
    public string getDriversView(bool active)
    {
        JavaScriptSerializer j = new JavaScriptSerializer();
        Drivers d = new Drivers();
        List<Drivers> driversList = d.getDriversListForView(active);
        return j.Serialize(driversList);
    }

    [WebMethod]
    public string getDriver(int driverID)
    {
        JavaScriptSerializer j = new JavaScriptSerializer();
        Drivers d = new Drivers();
        d.DriverID = driverID;
        Drivers driver = d.getDriver();
        return j.Serialize(driver);
    }

    [WebMethod]
    public void setDriver(int driverID, string driverNumber, string firstName, string lastName, string phone, string email, string accountID, string appPassword, string dateOfBirth, string driverLicenseExpiredDate, string driverCertificationExpiredDate, int cityID, int licenseTypeID, int certificationTypeID, int truckID, string func)
    {
        DateTime dBirth;
        if (dateOfBirth.Contains(".") || dateOfBirth.Contains("-"))
        {
            dBirth = DateTime.Parse(dateOfBirth);
        }
        else
        {
            dBirth = DateTime.ParseExact(dateOfBirth, "dd/MM/yyyy", CultureInfo.InvariantCulture);

        }

        DateTime dLExpired;
        if (driverLicenseExpiredDate.Contains(".") || driverLicenseExpiredDate.Contains("-"))
        {
            dLExpired = DateTime.Parse(driverLicenseExpiredDate);
        }
        else
        {
            dLExpired = DateTime.ParseExact(driverLicenseExpiredDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

        }

        DateTime dCExpired;
        if (driverCertificationExpiredDate.Contains(".") || driverCertificationExpiredDate.Contains("-"))
        {
            dCExpired = DateTime.Parse(driverCertificationExpiredDate);
        }
        else
        {
            dCExpired = DateTime.ParseExact(driverCertificationExpiredDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

        }

        Drivers d = new Drivers(driverID, driverNumber, firstName, lastName, phone, email, accountID, appPassword, dBirth, dLExpired, dCExpired, new City(cityID, "", "", -1), new DriverLicenseTypes(licenseTypeID, "", "", "רישיון"), new DriverLicenseTypes(certificationTypeID, "", "", "היתר"), new Trucks(truckID));
        d.setDriver(func);
    }

    [WebMethod]
    public void deactivateDriver(int driverID, string active)
    {
        Drivers d = new Drivers();
        d.DriverID = driverID;
        d.deactivateDriver(active);
    }

    [WebMethod]
    public string getAvailableDrivers(string chosenDate)
    {
        DateTime selecetdDate = DateTime.ParseExact(chosenDate, "dd/MM/yyyy", null);

        JavaScriptSerializer j = new JavaScriptSerializer();
        Drivers d = new Drivers();
        List<Drivers> driversList = d.getAvailableDriversList(selecetdDate);
        return j.Serialize(driversList);
    }
    #endregion

    #region cities functions
    [WebMethod]
    public string getCities()
    {
        JavaScriptSerializer j = new JavaScriptSerializer();
        City c = new City();
        List<City> citiesList = c.getCitiesList();
        return j.Serialize(citiesList);
    }

    #endregion

    #region addresses functions

    [WebMethod]
    public string getCustomerAddresses(int customerID)
    {
        JavaScriptSerializer j = new JavaScriptSerializer();
        Addresses a = new Addresses();
        List<Addresses> addressesList = a.getCustomerAddressesList(customerID);
        return j.Serialize(addressesList);
    }

    [WebMethod]
    public string getAddresses()
    {
        JavaScriptSerializer j = new JavaScriptSerializer();
        Addresses a = new Addresses();
        List<Addresses> addressesList = a.getAddressesList();
        return j.Serialize(addressesList);
    }

    #endregion

    #region paymentTypes functions
    [WebMethod]
    public string getPaymentTypes()
    {
        JavaScriptSerializer j = new JavaScriptSerializer();
        PaymentTypes pt = new PaymentTypes();
        List<PaymentTypes> paymentTypesList = pt.getPaymentTypesList();
        return j.Serialize(paymentTypesList);
    }

    #endregion

    #region driverConstraints functions

    [WebMethod]
    public string getDriverConstraints(bool active, int selectedDrivers, string startDate, string endDate)
    {
        if (startDate == "")
        {
            startDate = "01/01/1990";
        }
        if (endDate == "")
        {
            endDate = "01/01/2990";
        }
        DateTime startDatedt = DateTime.ParseExact(startDate, "dd/MM/yyyy", null);
        DateTime endDatedt = DateTime.ParseExact(endDate, "dd/MM/yyyy", null);


        JavaScriptSerializer j = new JavaScriptSerializer();
        DriverConstraints dc = new DriverConstraints();
        List<DriverConstraints> driverConstraintsList = dc.getDriverConstraintsList(active, selectedDrivers, startDatedt, endDatedt);
        return j.Serialize(driverConstraintsList);
    }

    [WebMethod]
    public string getDriverConstraintsView(bool active, int selectedDrivers, string startDate, string endDate)
    {
        if (startDate == "")
        {
            startDate = "01/01/1990";
        }
        if (endDate == "")
        {
            endDate = "01/01/2990";
        }
        DateTime startDatedt = DateTime.ParseExact(startDate, "dd/MM/yyyy", null);
        DateTime endDatedt = DateTime.ParseExact(endDate, "dd/MM/yyyy", null);


        JavaScriptSerializer j = new JavaScriptSerializer();
        DriverConstraints dc = new DriverConstraints();
        List<DriverConstraints> driverConstraintsList = dc.getDriverConstraintsListForView(active, selectedDrivers, startDatedt, endDatedt);
        return j.Serialize(driverConstraintsList);
    }

    [WebMethod]
    public void deactivateDriverConstraints(int driverConstraintID, string active)
    {
        DriverConstraints dc = new DriverConstraints();
        dc.DriverConstraintID = driverConstraintID;
        dc.deactivateDriverConstraints(active);
    }

    [WebMethod]
    public void setDriverConstraint(int driverConstraintID, string startdate, string enddate, int driverID, string comments, string func)
    {
        DateTime dtStart;
        DateTime dtFinish;
        if (startdate.Contains("."))
        {
            dtStart = DateTime.Parse(startdate);
        }
        else
        {
            dtStart = DateTime.ParseExact(startdate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
        }
        if (enddate.Contains("."))
        {
            dtFinish = DateTime.Parse(enddate);
        }
        else
        {
            dtFinish = DateTime.ParseExact(enddate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
        }
        
        DateTime dt = dtStart;
        while (dt <= dtFinish)
        {
            Drivers tmp = new Drivers();
            Drivers d = new Drivers();
            tmp.DriverID = driverID;
            d = tmp.getDriver();
            DriverConstraints dc = new DriverConstraints(driverConstraintID, dt, d, comments);
            dc.setDriverConstraint(func);
            dt = dt.AddDays(1);
        }

    }


    #endregion

    #region documentTypes functions
    [WebMethod]
    public string getDocumentTypes()
    {
        JavaScriptSerializer j = new JavaScriptSerializer();
        DocumentTypes dt = new DocumentTypes();
        List<DocumentTypes> documentsTypesList = dt.getDocumentTypesList();
        return j.Serialize(documentsTypesList);
    }
    #endregion

    #region driverLicenseTypes functions
    [WebMethod]
    public string getDriverLicenseTypes(string LicenseORCertification)
    {
        JavaScriptSerializer j = new JavaScriptSerializer();
        DriverLicenseTypes dlt = new DriverLicenseTypes();
        List<DriverLicenseTypes> driverLicenseTypesList = dlt.getDriverLicenseTypesList(LicenseORCertification);
        return j.Serialize(driverLicenseTypesList);
    }

    [WebMethod]
    public string getDriverLicenseTypesDDL()
    {
        JavaScriptSerializer j = new JavaScriptSerializer();
        DriverLicenseTypes dlt = new DriverLicenseTypes();
        List<DriverLicenseTypes> driverLicenseTypesList = dlt.getDriverLicenseTypesList();
        return j.Serialize(driverLicenseTypesList);
    }
    #endregion

    #region documents functions
    [WebMethod]
    public string getDocuments(bool active, int selectedDocumentTypeID, string startDate, string endDate)
    {
        if (startDate == "")
        {
            startDate = "01/01/1990";
        }

        if (endDate == "")
        {
            endDate = "01/01/2990";
        }

        DateTime sdt = DateTime.ParseExact(startDate, "dd/MM/yyyy", null);
        DateTime edt = DateTime.ParseExact(endDate, "dd/MM/yyyy", null);
        JavaScriptSerializer j = new JavaScriptSerializer();
        Documents d = new Documents();
        List<Documents> documentsList = d.getDocumentsList(active, selectedDocumentTypeID, sdt, edt);
        return j.Serialize(documentsList);
    }

    [WebMethod]
    public string getDocument(int documentID)
    {
        JavaScriptSerializer j = new JavaScriptSerializer();
        Documents d = new Documents();
        d.DocumentID = documentID;
        Documents document = d.getDocument();
        return j.Serialize(document);
    }

    

    [WebMethod]
    public void setDocumentApp(int documentID, string documentName, int imgID, float totalPrice, string comments, string date, int documentTypeID, int driverID, string containerID, int orderID, string func)
    {
        DateTime dt;
        if (date.Contains(".") || date.Contains("-"))
        {
            dt = DateTime.Parse(date);
        }
        else
        {
            dt = DateTime.ParseExact(date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
        }
        Drivers driver = new Drivers();
        driver.DriverID = driverID;
        Drivers sendBy = driver.getDriver();

        Orders order = new Orders();
        order.OrderID = orderID;
        Orders relatedOrder = order.getOrder();

        Documents d = new Documents(documentID, documentName,totalPrice, comments, containerID, dt, new DocumentTypes(documentTypeID, ""), sendBy, relatedOrder, imgID);
        d.setDocumentApp(func);
    }


    [WebMethod]
    public void setDocument(int documentID, string documentName, string url, float totalPrice, string comments, string date, int documentTypeID, int driverID, string containerID, int orderID, string func)
    {
        DateTime dt;
        if (date.Contains(".") || date.Contains("-"))
        {
            dt = DateTime.Parse(date);
        }
        else
        {
            dt = DateTime.ParseExact(date, "dd/MM/yyyy", CultureInfo.InvariantCulture);    
        }
        Drivers driver = new Drivers();
        driver.DriverID = driverID;
        Drivers sendBy = driver.getDriver();

        Orders order = new Orders();
        order.OrderID = orderID;
        Orders relatedOrder = order.getOrder();

        Documents d = new Documents(documentID, documentName, url, totalPrice, comments, containerID, dt, new DocumentTypes(documentTypeID, ""), sendBy, relatedOrder);
        d.setDocument(func);
    }

    [WebMethod]
    public void deactivateDocument(int documentID, string active)
    {
        Documents d = new Documents();
        d.DocumentID = documentID;
        d.deactivateDocument(active);
    }

    [WebMethod]
    public string getDriverDocumentsPeriod(int driverID, int func)
    {
        JavaScriptSerializer j = new JavaScriptSerializer();
        Documents d = new Documents();
        List<Documents> documentsList = d.getDriverDocumentsPeriodList(driverID, func);
        return j.Serialize(documentsList);
    }


    [WebMethod]
    public string getDriverDocuments(int driverID, int selectedDocumentTypeID, string startDate, string endDate)
    {
        if (startDate == "")
        {
            startDate = "01/01/1990";
        }

        if (endDate == "")
        {
            endDate = "01/01/2990";
        }

        DateTime sdt = DateTime.ParseExact(startDate, "dd/MM/yyyy", null);
        DateTime edt = DateTime.ParseExact(endDate, "dd/MM/yyyy", null);
        JavaScriptSerializer j = new JavaScriptSerializer();
        Documents d = new Documents();
        List<Documents> documentsList = d.getDriverDocumentsList(driverID, selectedDocumentTypeID, sdt, edt);
        return j.Serialize(documentsList);
    }

    #endregion

    #region priceList functions
    [WebMethod]
    public string getPriceLists(bool active)
    {

        JavaScriptSerializer j = new JavaScriptSerializer();
        PriceList pl = new PriceList();
        List<PriceList> priceListList = pl.getPriceListList(active);
        return j.Serialize(priceListList);
    }

    [WebMethod]
    public string getPriceListsView(bool active, int selectedCustomer, int selectedService)
    {
        JavaScriptSerializer j = new JavaScriptSerializer();
        PriceList pl = new PriceList();
        List<PriceList> priceListList = pl.getPriceListForView(active, selectedCustomer, selectedService);
        return j.Serialize(priceListList);
    }


    [WebMethod]
    public string getPriceList(int priceListID)
    {
        JavaScriptSerializer j = new JavaScriptSerializer();
        PriceList pl = new PriceList();
        pl.PriceListID = priceListID;
        PriceList priceList = pl.getPriceList();
        return j.Serialize(priceList);
    }

    [WebMethod]
    public string getPriceListUnknown(int customerID, int serviceID, int originID, int destinationID)
    {
        JavaScriptSerializer j = new JavaScriptSerializer();
        PriceList pl = new PriceList();
        Customers c = new Customers();
        pl.Customer = c;
        pl.Customer.CustomerID = customerID;
        Services s = new Services();
        pl.Service = s;
        pl.Service.ServiceID = serviceID;
        Addresses o = new Addresses();
        pl.Origin = o;
        pl.Origin.AddressID = originID;
        Addresses d = new Addresses();
        pl.Destination = d;
        pl.Destination.AddressID = destinationID;
        PriceList priceList = pl.getPriceList(true);
        return j.Serialize(priceList);
    }

    [WebMethod]
    public void setPriceList(int priceListID, int originID, int destinationID, float price, float deliveryTime, float deliveryDistance, int customerID, int serviceID, string func)
    {
        Addresses o = new Addresses();
        o.AddressID = originID;
        Addresses origin = o.getAddress();

        Addresses d = new Addresses();
        d.AddressID = destinationID;
        Addresses destination = d.getAddress();

        Customers c = new Customers();
        c.CustomerID = customerID;
        Customers customer = c.getCustomer();

        Services s = new Services();
        s.ServiceID = serviceID;
        Services service = s.getService();

        PriceList pl = new PriceList(priceListID, origin, destination, price, deliveryTime, deliveryDistance, customer, service);
        pl.setPriceList(func);
    }

    [WebMethod]
    public void deactivatePriceList(int priceListID, string active)
    {
        PriceList pl = new PriceList();
        pl.PriceListID = priceListID;
        pl.deactivatePriceList(active);
    }


    #endregion
    #region shiftOrganizer functions

    [WebMethod]
    public void saveShifts(List<Shifts> allDriversShifts, List<int> bankShift)
    {
        Orders o = new Orders();
        //o.saveOrderLineInShift(allDriversShifts);
        o.saveOrderInShift(allDriversShifts);
        //o.removeOrderLineInShift(bankShift);
        o.removeOrderInShift(bankShift);
    }
    #endregion

    #region imageTable functions

    [WebMethod]
    public string getLastDriverImgID(int driverID)
    {
        JavaScriptSerializer j = new JavaScriptSerializer();
        Images img = new Images();
        int imgID = img.getLastDriverImgID(driverID);
        return j.Serialize(imgID);
    }

    [WebMethod]
    public string getDocumentImage(int imgID)
    {
        JavaScriptSerializer j = new JavaScriptSerializer();
        Images img = new Images();
        string url = img.getDocumentImageURL(imgID);
        return j.Serialize(url);
    }
    #endregion
}
