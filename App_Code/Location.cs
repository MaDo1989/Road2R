using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

/// <summary>
/// Summary description for Destination
/// </summary>
public class Location
{
    public Location()
    {
        //
        // TODO: Add constructor logic here
        //
    }
    string type;//סוג יעד
    string name;//שם
    string area;//אזור
    string direction;//הוראות הגעה
    Volunteer responsible;//רכז אחראי
    string status;//סטטוס יעד
    string remarks;//הערות
    string managerName;//שם מנהל יעד
    string managerLastName;//שם משפחה מנהל יעד
    string managerPhones;//טלפון מנהל יעד
    string managerPhones2;//טלפון מנהל יעד

    public bool IsActive { get; set; }

    public string Type
    {
        get
        {
            return type;
        }

        set
        {
            type = value;
        }
    }

    public string Name
    {
        get
        {
            return name;
        }

        set
        {
            name = value;
        }
    }

    public string Area
    {
        get
        {
            return area;
        }

        set
        {
            area = value;
        }
    }

    public string Direction
    {
        get
        {
            return direction;
        }

        set
        {
            direction = value;
        }
    }

    public Volunteer Responsible
    {
        get
        {
            return responsible;
        }

        set
        {
            responsible = value;
        }
    }

    public string Status
    {
        get
        {
            return status;
        }

        set
        {
            status = value;
        }
    }

    public string Remarks
    {
        get
        {
            return remarks;
        }

        set
        {
            remarks = value;
        }
    }

    public string ManagerName
    {
        get
        {
            return managerName;
        }

        set
        {
            managerName = value;
        }
    }

    public string ManagerLastName
    {
        get
        {
            return managerLastName;
        }

        set
        {
            managerLastName = value;
        }
    }

    public string ManagerPhones
    {
        get
        {
            return managerPhones;
        }

        set
        {
            managerPhones = value;
        }
    }

    public string ManagerPhones2
    {
        get
        {
            return managerPhones2;
        }

        set
        {
            managerPhones2 = value;
        }
    }

    public Location(string _type, string _name, string _area, string _direction, Volunteer _responsible, string _status,
        string _remarks, string _managerName, string _managerLastName, string _managerPhones, string _managerPhones2)
    {
        Type = _type;
        Name = _name;
        Area = _area;
        Direction = _direction;
        Responsible = _responsible;
        Status = _status;
        Remarks = _remarks;
        ManagerName = _managerName;
        ManagerLastName = _managerLastName;
        ManagerPhones = _managerPhones;
        ManagerPhones2 = _managerPhones2;

    }

    public Location(string _name)
    {
        Name = _name;
    }

    //public DataTable read()
    //{
    //    DBservices dbs = new DBservices();
    //    dbs = dbs.ReadFromDataBase("RoadDBconnectionString", "Destination");
    //    return dbs.dt;
    //}

    //public List<Destination> getListdestination()
    //{
    //    DBservices dbs = new DBservices();
    //    List<Destination> listd = new List<Destination>();
    //    listd = dbs.getListdestination("RoadDBconnectionString", "Destination");
    //    return listd;
    //}

    public List<Location> getDestinationsListForView(bool active)
    {
        #region DB functions
        string query = "select * from Location ";
        query += "order by name";

        List<Location> list = new List<Location>();
        DbService db = new DbService();
        DataSet ds = db.GetDataSetByQuery(query);

        foreach (DataRow dr in ds.Tables[0].Rows)
        {

            Location l = new Location();
            l.Name = dr["Name"].ToString();
            l.Type = dr["Type"].ToString();
            l.Area = dr["Area"].ToString();
            //tmp.Direction = dr["direction"].ToString();
            l.Responsible = new Volunteer(dr["Responsible"].ToString());
            l.IsActive =Convert.ToBoolean( dr["IsActive"].ToString());
            l.Remarks = dr["Remarks"].ToString();
            //l.ManagerName = dr["DestinationManager"].ToString();
            //l.ManagerLastName = dr["managerLastName"].ToString();
            //l.ManagerPhones = dr["managerPhones1"].ToString();
            //l.ManagerPhones2 = dr["managerPhones2"].ToString();
            list.Add(l);
        }
        #endregion

        return list;

    }

    public List<Location> getHospitalListForView(bool active)
    {
        #region DB functions
        string query = "select * from Location where Type='בית חולים'";
        query += "order by name";

        List<Location> list = new List<Location>();
        DbService db = new DbService();
        DataSet ds = db.GetDataSetByQuery(query);

        foreach (DataRow dr in ds.Tables[0].Rows)
        {

            Location tmp = new Location();
            tmp.Name = dr["Name"].ToString();
            tmp.Type = dr["Type"].ToString();
            tmp.Area = dr["Area"].ToString();
            //tmp.Direction = dr["direction"].ToString();
            tmp.Responsible = new Volunteer(dr["Responsible"].ToString());
            tmp.IsActive = Convert.ToBoolean(dr["IsActive"].ToString());
            tmp.Remarks = dr["Remarks"].ToString();
            //tmp.ManagerName = dr["managerName"].ToString();
            //tmp.ManagerLastName = dr["managerLastName"].ToString();
            //tmp.ManagerPhones = dr["managerPhones1"].ToString();
            //tmp.ManagerPhones2 = dr["managerPhones2"].ToString();
            list.Add(tmp);
        }
        #endregion

        return list;

    }

    public List<Location> getBarrierListForView(bool active)
    {
        #region DB functions
        string query = "select * from Location where Type='מחסום'";
        query += "order by name";

        List<Location> list = new List<Location>();
        DbService db = new DbService();
        DataSet ds = db.GetDataSetByQuery(query);

        foreach (DataRow dr in ds.Tables[0].Rows)
        {

            Location tmp = new Location();
            tmp.Name = dr["Name"].ToString();
            tmp.Type = dr["Type"].ToString();
            tmp.Area = dr["Area"].ToString();
            //tmp.Direction = dr["direction"].ToString();
            tmp.Responsible = new Volunteer(dr["Responsible"].ToString());
            tmp.IsActive = Convert.ToBoolean(dr["IsActive"].ToString());
            tmp.Remarks = dr["Remarks"].ToString();
            //tmp.ManagerName = dr["managerName"].ToString();
            //tmp.ManagerLastName = dr["managerLastName"].ToString();
            //tmp.ManagerPhones = (int)dr["managerPhones1"];
            //tmp.ManagerPhones2 = (int)dr["managerPhones2"];
            //tmp.ManagerPhones = dr["managerPhones1"].ToString();
            //tmp.ManagerPhones2 = dr["managerPhones2"].ToString();
            list.Add(tmp);
        }
        #endregion

        return list;

    }
}