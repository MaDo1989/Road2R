using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;

/// <summary>
/// Summary description for Customers
/// </summary>
public class Customers
{
    int customerID;
    string customerName;
    string customerContactName;
    string accountID;
    string active;
    string phone1;
    string phone2;
    string email;
    List<Addresses> addresses;
    Drivers preferedDrivers;
    PaymentTypes paymentType;
    string comments;
    string registrationNumber;
    string billingAddress;

    public int CustomerID
    {
        get
        {
            return customerID;
        }

        set
        {
            customerID = value;
        }
    }

    public string CustomerName
    {
        get
        {
            return customerName;
        }

        set
        {
            customerName = value;
        }
    }

    public string CustomerContactName
    {
        get
        {
            return customerContactName;
        }

        set
        {
            customerContactName = value;
        }
    }

    public string AccountID
    {
        get
        {
            return accountID;
        }

        set
        {
            accountID = value;
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

    public string Phone1
    {
        get
        {
            return phone1;
        }

        set
        {
            phone1 = value;
        }
    }

    public string Phone2
    {
        get
        {
            return phone2;
        }

        set
        {
            phone2 = value;
        }
    }

    public string Email
    {
        get
        {
            return email;
        }

        set
        {
            email = value;
        }
    }

    public List<Addresses> Addresses
    {
        get
        {
            return addresses;
        }

        set
        {
            addresses = value;
        }
    }

    public Drivers PreferedDrivers
    {
        get
        {
            return preferedDrivers;
        }

        set
        {
            preferedDrivers = value;
        }
    }

    public PaymentTypes PaymentType
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

    public string Comments
    {
        get
        {
            return comments;
        }

        set
        {
            comments = value;
        }
    }

    public string RegistrationNumber
    {
        get
        {
            return registrationNumber;
        }

        set
        {
            registrationNumber = value;
        }
    }

    public string BillingAddress
    {
        get
        {
            return billingAddress;
        }

        set
        {
            billingAddress = value;
        }
    }

    public Customers()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public Customers(int customerID, string customerName, string customerContactName, string accountID, string active, string phone1, string phone2, string email, List<Addresses> addresses, Drivers preferedDrivers, PaymentTypes paymentType, string comments, string registrationNumber, string billingAddress)
    {
        CustomerID = customerID;
        CustomerName = customerName;
        CustomerContactName = customerContactName;
        AccountID = accountID;
        Active = active;
        Phone1 = phone1;
        Phone2 = phone2;
        Email = email;
        Addresses = addresses;
        PreferedDrivers = preferedDrivers;
        PaymentType = paymentType;
        Comments = comments;
        RegistrationNumber = registrationNumber;
        BillingAddress = billingAddress;
    }

    public Customers(int customerID, string customerName, string customerContactName, string accountID, string phone1, string phone2, string email, Drivers preferedDrivers, PaymentTypes paymentType, string comments, string registrationNumber, string billingAddress)
    {
        CustomerID = customerID;
        CustomerName = customerName;
        CustomerContactName = customerContactName;
        AccountID = accountID;
        Phone1 = phone1;
        Phone2 = phone2;
        Email = email;
        PreferedDrivers = preferedDrivers;
        PaymentType = paymentType;
        Comments = comments;
        RegistrationNumber = registrationNumber;
        BillingAddress = billingAddress;
    }

    public List<Customers> getCustomersList(bool active)
    {
        #region DB functions
        string query = "select * from Customers c inner join PaymentTypes pt on c.PaymentTypeID=pt.PaymentTypeID";
        if (active)
        {
            query += " where c.Active = 'Y'";
        }

        query += " order by CustomerName";

        List<Customers> list = new List<Customers>();
        DbService db = new DbService();
        DataSet ds = db.GetDataSetByQuery(query);

        foreach (DataRow dr in ds.Tables[0].Rows)
        {
            Customers tmp = new Customers();
            tmp.CustomerID = (int)dr["CustomerID"];
            tmp.CustomerName = dr["CustomerName"].ToString();
            tmp.CustomerContactName = dr["ContactName"].ToString();
            tmp.AccountID = dr["AccountID"].ToString();
            tmp.Active = dr["Active"].ToString();
            tmp.Phone1 = dr["Phone1"].ToString();
            tmp.Phone2 = dr["Phone2"].ToString();
            tmp.Email = dr["Email"].ToString();
            tmp.PaymentType = new PaymentTypes((int)dr["PaymentTypeID"], dr["PaymentType"].ToString());
            tmp.Comments = dr["Comments"].ToString();
            tmp.RegistrationNumber = dr["CompanyRegistrationNumber"].ToString(); 

            list.Add(tmp);
        }
        #endregion

        return list;
    }


    public Customers getCustomer()
    {
        #region DB functions
        string query = "select c.CustomerID, c.CustomerName, c.ContactName, c.AccountID, c.Active, c.Phone1, c.Phone2, c.Email, c.PaymentTypeID, pt.PaymentType, c.Comments, c.CompanyRegistrationNumber, c.BillingAddress, c.PreferedDriverID, d.FirstName, d.LastName from Customers c inner join PaymentTypes pt on c.PaymentTypeID=pt.PaymentTypeID inner join Drivers d on c.PreferedDriverID = d.DriverID where CustomerID =" + CustomerID + "";
        Customers c = new Customers();
        DbService db = new DbService();
        DataSet ds = db.GetDataSetByQuery(query);

        foreach (DataRow dr in ds.Tables[0].Rows)
        {
            c.CustomerID = (int)dr["CustomerID"];
            c.CustomerName = dr["CustomerName"].ToString();
            c.CustomerContactName = dr["ContactName"].ToString();
            c.AccountID = dr["AccountID"].ToString();
            c.Active = dr["Active"].ToString();
            c.Phone1 = dr["Phone1"].ToString();
            c.Phone2 = dr["Phone2"].ToString();
            c.Email = dr["Email"].ToString();
            c.PaymentType = new PaymentTypes((int)dr["PaymentTypeID"], dr["PaymentType"].ToString());
            c.Comments = dr["Comments"].ToString();
            c.RegistrationNumber = dr["CompanyRegistrationNumber"].ToString();
            c.BillingAddress = dr["BillingAddress"].ToString();

            Drivers d = new Drivers();
            d.DriverID = (int)dr["PreferedDriverID"];
            d.FirstName = dr["FirstName"].ToString();
            d.LastName = dr["LastName"].ToString();
            c.PreferedDrivers = d;

            c.Addresses = new List<Addresses>();
            Addresses a = new Addresses();
            c.Addresses = a.getCustomerAddressesList(c.CustomerID);
        }
        #endregion

        return c;
    }

    public void setCustomer(string func)
    {
        DbService db = new DbService();
        string query = "";
        if (func == "edit")
        {
            query = "UPDATE Customers SET CustomerName = '" + CustomerName + "', ContactName = '" + CustomerContactName + "', AccountID = '" + AccountID + "', Phone1 = '" + Phone1 + "', Phone2 = '" + Phone2 + "', Email = '" + Email + "', PaymentTypeID = " + PaymentType.PaymentTypeID + ", Comments = '" + Comments + "', PreferedDriverID = " + PreferedDrivers.DriverID + ", CompanyRegistrationNumber = '" + RegistrationNumber + "', BillingAddress = '"+ BillingAddress +"' WHERE CustomerID = " + CustomerID;
        }
        else if (func == "new")
        {
            query = "insert into Customers values ('" + CustomerName + "','" + CustomerContactName + "','" + AccountID + "','Y','" + Phone1 + "','" + Phone2 + "','" + Email + "'," + PaymentType.PaymentTypeID + ",'" + Comments + "'," + PreferedDrivers.DriverID + ", '" + RegistrationNumber + "', '" + BillingAddress + "')";
        }
        db.ExecuteQuery(query);
    }

    public int getLastCustomerID()
    {
        int ID;

        string query = "select max(CustomerID) as 'id' from Customers";
        Customers c = new Customers();
        DbService db = new DbService();
        DataSet ds = db.GetDataSetByQuery(query);
        DataTable dt = ds.Tables[0];
        ID = Convert.ToInt32(dt.Rows[0]["id"]);

        return ID;
    }

    public void deactivateCustomer(string active)
    {
        DbService db = new DbService();
        db.ExecuteQuery("UPDATE Customers SET Active='" + active + "' WHERE CustomerID=" + CustomerID);
    }
}