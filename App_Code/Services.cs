using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;


/// <summary>
/// Summary description for Services
/// </summary>
public class Services
{
    int serviceID;
    string service;
    string serviceDescription;
    float price;
    string active;

    public int ServiceID
    {
        get
        {
            return serviceID;
        }

        set
        {
            serviceID = value;
        }
    }

    public string Service
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

    public string ServiceDescription
    {
        get
        {
            return serviceDescription;
        }

        set
        {
            serviceDescription = value;
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

    public Services()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public Services(int serviceID, string service, string serviceDescription, float price)
    {
        ServiceID = serviceID;
        Service = service;
        ServiceDescription = serviceDescription;
        Price = price;
    }

    public Services(string service, string serviceDescription, float price)
    {
        Service = service;
        ServiceDescription = serviceDescription;
        Price = price;
    }

    public List<Services> getServicesList(bool active)
    {
        #region DB functions
        string query = "select * from Services";
        if (active)
        {
            query += " where Active = 'Y'";
        }

        query += " order by Service";
        
        List<Services> list = new List<Services>();
        DbService db = new DbService();
        DataSet ds = db.GetDataSetByQuery(query);

        foreach (DataRow dr in ds.Tables[0].Rows)
        {
            Services tmp = new Services();
            tmp.ServiceID = (int)dr["ServiceID"];
            tmp.Service = dr["Service"].ToString();
            tmp.ServiceDescription = dr["ServiceDescription"].ToString();
            tmp.Price = float.Parse(dr["Price"].ToString());
            tmp.Active = dr["Active"].ToString();
           
            list.Add(tmp);
        }
        #endregion

        return list;
    }

    public Services getService()
    {
        #region DB functions
        string query = "select * from Services where ServiceID ="+ServiceID+"";

        Services s = new Services();
        DbService db = new DbService();
        DataSet ds = db.GetDataSetByQuery(query);

        foreach (DataRow dr in ds.Tables[0].Rows)
        {
            s.ServiceID = (int)dr["ServiceID"];
            s.Service = dr["Service"].ToString();
            s.ServiceDescription = dr["ServiceDescription"].ToString();
            s.Price = float.Parse(dr["Price"].ToString());
            s.Active = dr["Active"].ToString();
        }
        #endregion

        return s;
    }


    public void setService (string func)
    {
        DbService db = new DbService();
        string query = "";
        if (func == "edit")
        {
            query = "UPDATE Services SET Service = '" + Service + "', ServiceDescription = '" + ServiceDescription + "', Price = " + Price + " WHERE ServiceID = " + ServiceID;
        }
        else if(func == "new")
        {
            query = "insert into Services values ('"+Service+"','"+ServiceDescription+"',"+Price+",'Y')";
        }
        db.ExecuteQuery(query);
    }

    public void deactivateService(string active)
    {
        DbService db = new DbService();
        db.ExecuteQuery("UPDATE Services SET Active='" + active + "' WHERE ServiceID="+ServiceID);
    }

    public int getServiceID()
    {
        #region DB functions
        string query = "select * from Services where Service = '" + Service + "'";

        List<Services> list = new List<Services>();
        DbService db = new DbService();
        DataSet ds = db.GetDataSetByQuery(query);

        int ID = 0;
        foreach (DataRow dr in ds.Tables[0].Rows)
        {
            ID = (int)dr["ServiceID"];
        }
        #endregion

        return ID;
    }

}