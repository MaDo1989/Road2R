using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;

/// <summary>
/// Summary description for Addresses
/// </summary>
public class Addresses
{
    int addressID;
    City customerCity;
    string addressName;
    string street;
    string number;
    string contactPerson;
    string contactPhone;
    string comments;
    #region Addresses geters and seters
    public int AddressID
    {
        get
        {
            return addressID;
        }

        set
        {
            addressID = value;
        }
    }

    public City CustomerCity
    {
        get
        {
            return customerCity;
        }

        set
        {
            customerCity = value;
        }
    }

    public string AddressName
    {
        get
        {
            return addressName;
        }

        set
        {
            addressName = value;
        }
    }

    public string Street
    {
        get
        {
            return street;
        }

        set
        {
            street = value;
        }
    }

    public string Number
    {
        get
        {
            return number;
        }

        set
        {
            number = value;
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

    public string ContactPerson
    {
        get
        {
            return contactPerson;
        }

        set
        {
            contactPerson = value;
        }
    }

    public string ContactPhone
    {
        get
        {
            return contactPhone;
        }

        set
        {
            contactPhone = value;
        }
    }
    #endregion

    #region Addressess Constructors
    public Addresses()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public Addresses(int addressID, City customerCity, string addressName, string street, string number, string contactPerson, string contactPhone, string comments)
    {
        AddressID = addressID;
        CustomerCity = customerCity;
        AddressName = addressName;
        Street = street;
        Number = number;
        ContactPerson = contactPerson;
        ContactPhone = contactPhone;
        Comments = comments;
    }
    #endregion

    #region Addresses Methods
    //returns a specific customer's addresses as a list
    public List<Addresses> getCustomerAddressesList(int customerID)
    {
        #region DB functions
        string query = "select * from Addresses a inner join City ct on ct.CityID=a.CityID where Active = 'Y' and CustomerID=" + customerID + " order by AddressName";
        List<Addresses> list = new List<Addresses>();
        DbService db = new DbService();
        DataSet ds = db.GetDataSetByQuery(query);

        foreach (DataRow dr in ds.Tables[0].Rows)
        {
            Addresses tmp = new Addresses();
            tmp.AddressID = (int)dr["AddressID"];
            tmp.CustomerCity = new City((int)dr["CityID"], dr["City"].ToString(), dr["Area"].ToString(), (int)dr["Zone"]);
            tmp.AddressName = dr["AddressName"].ToString();
            tmp.Street = dr["Street"].ToString();
            tmp.Number = dr["Number"].ToString();
            tmp.ContactPerson = dr["ContactPerson"].ToString();
            tmp.ContactPhone = dr["ContactPhone"].ToString();
            tmp.Comments = dr["Comments"].ToString();

            list.Add(tmp);
        }
        #endregion

        return list;
    }

    //returns all active addressess as a list
    public List<Addresses> getAddressesList()
    {
        #region DB functions
        string query = "select * from Addresses a inner join City ct on ct.CityID=a.CityID where Active = 'Y'";
        List<Addresses> list = new List<Addresses>();
        DbService db = new DbService();
        DataSet ds = db.GetDataSetByQuery(query);

        foreach (DataRow dr in ds.Tables[0].Rows)
        {
            Addresses tmp = new Addresses();
            tmp.AddressID = (int)dr["AddressID"];
            tmp.CustomerCity = new City((int)dr["CityID"], dr["City"].ToString(), dr["Area"].ToString(), (int)dr["Zone"]);
            tmp.AddressName = dr["AddressName"].ToString();
            tmp.Street = dr["Street"].ToString();
            tmp.Number = dr["Number"].ToString();
            tmp.ContactPerson = dr["ContactPerson"].ToString();
            tmp.ContactPhone = dr["ContactPhone"].ToString();
            tmp.Comments = dr["Comments"].ToString();

            list.Add(tmp);
        }
        #endregion

        return list;
    }

    //sets a new address for a given customer
    public void setAddress(int customerID)
    {
        DbService db = new DbService();
        string query = "";
        if (AddressID != -1)
        {
            query = "UPDATE Addresses SET CityID = " + CustomerCity.CityID + ", AddressName = '" + AddressName + "', Street = '" + Street + "', Number = '" + Number + "', Comments = '" + Comments + "', ContactPerson = '" + ContactPerson + "', ContactPhone = '" + ContactPhone + "' WHERE AddressID = " + AddressID;
        }
        else
        {
            query = "insert into Addresses values (" + customerID + "," + CustomerCity.CityID + ",'" + AddressName + "','" + Street + "','" + Number + "','" + Comments + "','Y', '" + ContactPerson + "', '" + ContactPhone + "')";
        }

        db.ExecuteQuery(query);
    }

    //updates the active state of a given list of customers' addresses
    public void deactivateAddresses(List<int> deactiveAddresses)
    {
        foreach (var item in deactiveAddresses)
        {
            DbService db = new DbService();
            db.ExecuteQuery("UPDATE Addresses SET Active='N' WHERE AddressID=" + item);
        }
    }

    //returns the id of an address by given name
    public int getAddressID()
    {
        #region DB functions
        string query = "select * from Addresses where AddressName = '" + AddressName + "'";

        List<Addresses> list = new List<Addresses>();
        DbService db = new DbService();
        DataSet ds = db.GetDataSetByQuery(query);

        int ID = 0;
        foreach (DataRow dr in ds.Tables[0].Rows)
        {
            ID = (int)dr["AddressID"];
        }
        #endregion

        return ID;
    }

    //returns a specific address by id
    public Addresses getAddress()
    {
        #region DB functions
        string query = "select * from Addresses a inner join City ct on ct.CityID=a.CityID where AddressID=" + addressID;

        Addresses a = new Addresses();
        DbService db = new DbService();
        DataSet ds = db.GetDataSetByQuery(query);

        foreach (DataRow dr in ds.Tables[0].Rows)
        {
            a.AddressID = (int)dr["AddressID"];
            a.CustomerCity = new City((int)dr["CityID"], dr["City"].ToString(), dr["Area"].ToString(), (int)dr["Zone"]);
            a.AddressName = dr["AddressName"].ToString();
            a.Street = dr["Street"].ToString();
            a.Number = dr["Number"].ToString();
            a.ContactPerson = dr["ContactPerson"].ToString();
            a.ContactPhone = dr["ContactPhone"].ToString();
            a.Comments = dr["Comments"].ToString();
        }
        #endregion

        return a;
    }
    #endregion
}