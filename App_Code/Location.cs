using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
    string englishName;//שם באנגלית
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

    public string EnglishName
    {
        get
        {
            return englishName;
        }

        set
        {
            englishName = value;
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
    public Location(string _name,string _englishName)
    {
        Name = _name;
        EnglishName = _englishName;
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
        
        string query = "select * from Location";
        if (active)
        {
            query += " where IsActive = '" + active + "' order by EnglishName";
        }
        else query += "order by englishName";

        List<Location> list = new List<Location>();
        DbService db = new DbService();
        DataSet ds = db.GetDataSetByQuery(query);

        SqlCommand cmd2 = new SqlCommand();
        cmd2.CommandType = CommandType.Text;
        SqlParameter[] cmdParams2 = new SqlParameter[1];

        foreach (DataRow dr in ds.Tables[0].Rows)
        {

            Location l = new Location();
            l.Name = dr["Name"].ToString();
            l.Type = dr["Type"].ToString();
            l.Area = dr["Area"].ToString();
            l.Direction = dr["adress"].ToString();
            l.Responsible = new Volunteer(dr["Responsible"].ToString());
            l.IsActive =Convert.ToBoolean( dr["IsActive"].ToString());
            l.Remarks = dr["Remarks"].ToString();
            l.EnglishName = dr["EnglishName"].ToString();
            if (dr["DestinationManager"].ToString() != "")
            {
                int managerId = int.Parse(dr["DestinationManager"].ToString());
                cmdParams2[0] = cmd2.Parameters.AddWithValue("Id", managerId);
                string query2 = "select * from DestinationManagers where Id=@Id";
                DestinationManager m = new DestinationManager();
                DbService db2 = new DbService();
                DataSet ds2 = db2.GetDataSetByQuery(query2, true,cmd2.CommandType, cmdParams2);
                DataRow dr2 = ds2.Tables[0].Rows[0];
                l.ManagerName = dr2["FirstName"].ToString();
                l.ManagerLastName = dr2["LastName"].ToString();
                l.managerPhones = dr2["Phone"].ToString();
            }

            list.Add(l);
        }
        #endregion

        return list;

    }

    public List<Location> getHospitalListForView(bool active)
    {
        #region DB functions
        string query = "select * from Location where Type=N'בית חולים'";
        query += " and IsActive = 'True'";
        query += " order by englishname";

        List<Location> list = new List<Location>();
        DbService db = new DbService();
        DataSet ds = db.GetDataSetByQuery(query);

        foreach (DataRow dr in ds.Tables[0].Rows)
        {

            Location tmp = new Location();
            tmp.Name = dr["Name"].ToString();
            tmp.EnglishName = dr["EnglishName"].ToString();
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

    public List<string> getAreas()
    {
        List<string> areas = new List<string>();
        string query = "select * from Area order by AreaName";
        DbService db = new DbService();
        DataSet ds = db.GetDataSetByQuery(query);

        foreach (DataRow dr in ds.Tables[0].Rows)
        {
            string tmp = dr["AreaName"].ToString();
            areas.Add(tmp);
        }
        return areas;
    }

    public List<Location> getBarrierListForView(bool active)
    {
        #region DB functions
        // where Type='מחסום'
        string query = "select * from Location where Type=N'מחסום'";
        //query += " and IsActive = 'True'";
        query += " order by englishname";

        List<Location> list = new List<Location>();
        DbService db = new DbService();
        DataSet ds = db.GetDataSetByQuery(query);

        foreach (DataRow dr in ds.Tables[0].Rows)
        {

            Location tmp = new Location();
            tmp.Name = dr["Name"].ToString();
            tmp.EnglishName = dr["EnglishName"].ToString();
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
    public void deactivateLocation(string active)
    {
        DbService db = new DbService();
        db.ExecuteQuery("UPDATE Location SET IsActive='" + active + "' WHERE Name=N'" + Name + "'");
    }


    public Location getLocation()
    {
        #region DB functions
        SqlCommand cmd = new SqlCommand();
        cmd.CommandType = CommandType.Text;
        SqlParameter[] cmdParams = new SqlParameter[1];
        cmdParams[0] = cmd.Parameters.AddWithValue("name", Name);
        string query = "select * from Location where Name=@name";
        Location l = new Location();
        DbService db = new DbService();
        DataSet ds = db.GetDataSetByQuery(query,true, cmd.CommandType, cmdParams);
        DataRow dr = ds.Tables[0].Rows[0];

        l.Type = dr["Type"].ToString();
        l.Name = dr["Name"].ToString();
        l.Area = dr["Area"].ToString();
        l.Direction = dr["adress"].ToString();
        //l.Responsible = (Volunteer)dr["Responsible"];
        l.IsActive = bool.Parse(dr["IsActive"].ToString());
        l.Remarks = dr["Remarks"].ToString();
        l.EnglishName = dr["EnglishName"].ToString();
        if (dr["DestinationManager"].ToString() != "")
        {
            int managerId = int.Parse(dr["DestinationManager"].ToString());
            SqlCommand cmd2 = new SqlCommand();
            cmd2.CommandType = CommandType.Text;
            SqlParameter[] cmdParams2 = new SqlParameter[1];
            cmdParams2[0] = cmd2.Parameters.AddWithValue("Id", managerId);
            string query2 = "select * from DestinationManagers where Id=@Id";

            DestinationManager m = new DestinationManager();
            DbService db2 = new DbService();
            DataSet ds2 = db2.GetDataSetByQuery(query2, true,cmd2.CommandType, cmdParams2);
            DataRow dr2 = ds2.Tables[0].Rows[0];
            l.ManagerName = dr2["FirstName"].ToString();
            l.ManagerLastName = dr2["LastName"].ToString();
            l.managerPhones = dr2["Phone"].ToString();
           
        }
        #endregion

        return l;
    }

    public Hashtable getLocationsEnglishName()
    {
       
        string query = "select Name,EnglishName from Location ";
        Hashtable list = new Hashtable();
        DbService db = new DbService();
        DataSet ds = db.GetDataSetByQuery(query);

        foreach (DataRow dr in ds.Tables[0].Rows)
        {

            Location tmp = new Location();
            tmp.Name = dr["Name"].ToString();
            tmp.EnglishName = dr["EnglishName"].ToString();

            list[tmp.Name] = tmp.EnglishName;
        }
        return list;
    }

    public void setLocation(Location v, string func)
    {
        #region DB functions
        int res = 0;
        DbService db = new DbService();
        SqlCommand cmd = new SqlCommand();
        cmd.CommandType = CommandType.Text;
        SqlParameter[] cmdParams = new SqlParameter[9];
        
        //getting the index for the destination manager
        DestinationManager m = new DestinationManager(v.ManagerName,v.ManagerLastName,v.ManagerPhones,v.ManagerPhones2);
        int managerId = m.setDestinationManager(m, func);

        cmdParams[0] = cmd.Parameters.AddWithValue("@type", v.Type);
        cmdParams[1] = cmd.Parameters.AddWithValue("@name", v.Name);
        cmdParams[2] = cmd.Parameters.AddWithValue("@area", v.Area);
        cmdParams[3] = cmd.Parameters.AddWithValue("@adress", v.Direction);
        //cmdParams[4] = cmd.Parameters.AddWithValue("@responsible", null);
        cmdParams[4] = cmd.Parameters.AddWithValue("@isActive", v.Status);
        cmdParams[5] = cmd.Parameters.AddWithValue("@remarks", v.Remarks);
        cmdParams[6] = cmd.Parameters.AddWithValue("@DestinationManager", managerId);
        cmdParams[7] = cmd.Parameters.AddWithValue("@cityCityName", "אביחיל");
        cmdParams[8] = cmd.Parameters.AddWithValue("@EnglishName", EnglishName);

        string query = "";
        if (func == "edit")
        {
            

            query = "update Location set Type=@type, Name=@name,";
            query += "Area=@area, Adress=@adress, IsActive=@IsActive, Remarks=@remarks, ";
            query += "DestinationManager=@DestinationManager, CityCityName=@cityCityName,EnglishName=@EnglishName where Name=@name"; 

            res = db.ExecuteQuery(query, cmd.CommandType, cmdParams);

            if (res == 0)
            {
                throw new Exception("שם האזור לא תקין");
            }
          
        }
        else if (func == "new")
        {
            query = "insert into Location (Type, Name, Area, Adress, IsActive, Remarks, DestinationManager, CityCityName,EnglishName)";
            query += " values (@type,@name,@area,@adress,@IsActive,@remarks,@DestinationManager,@cityCityName,@EnglishName);SELECT SCOPE_IDENTITY();";
            db = new DbService();
            try
            {
                db.ExecuteQuery(query, cmd.CommandType, cmdParams);
            }
            catch (SqlException e)
            {
                throw e;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        #endregion

    }

}