using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for DestinationManager
/// </summary>
public class DestinationManager
{
    int id;
    string managerName;//שם מנהל יעד
    string managerLastName;//שם משפחה מנהל יעד
    string managerPhones;//טלפון מנהל יעד
    string managerPhones2;//טלפון מנהל יעד

    public DestinationManager()
    {
        //
        // TODO: Add constructor logic here
        //

    }
    public DestinationManager(string managerName, string managerLastName,string managerPhones,string managerPhones2)
    {
        ManagerName = managerName;
        ManagerLastName = managerLastName;
        ManagerPhones = managerPhones;
        ManagerPhones2 = managerPhones2;
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

    public int Id
    {
        get
        {
            return id;
        }

        set
        {
            id = value;
        }
    }

    public int setDestinationManager(DestinationManager m, string func)
    {
        int res = 0;
        DbService db = new DbService();
        SqlCommand cmd = new SqlCommand();
        cmd.CommandType = CommandType.Text;
        SqlParameter[] cmdParams = new SqlParameter[4];

        
        cmdParams[0] = cmd.Parameters.AddWithValue("@FirstName", m.ManagerName);
        cmdParams[1] = cmd.Parameters.AddWithValue("@LastName", m.ManagerLastName);
        cmdParams[2] = cmd.Parameters.AddWithValue("@Phone", m.ManagerPhones);
        cmdParams[3] = cmd.Parameters.AddWithValue("@Phone2", "");
       

        string query = "";
        if (func == "edit")
        {
            query = "update DestinationManagers set LastName=@LastName,";
            query += "Phone=@Phone, Phone2=@Phone2 where FirstName=@FirstName";

            res = db.ExecuteQuery(query, cmd.CommandType, cmdParams);

            if (res == 0)
            {
                query = "insert into DestinationManagers (FirstName, LastName, Phone, Phone2)";
                query += " values (@FirstName,@LastName,@Phone,@Phone2);SELECT SCOPE_IDENTITY();";
                db = new DbService();
                try
                {
                    db.ExecuteQuery(query, cmd.CommandType, cmdParams);
                }
                catch (SqlException e)
                {
                    throw;
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            }

        }
        else if (func == "new")
        {
            query = "insert into DestinationManagers (FirstName, LastName, Phone, Phone2)";
            query += " values (@FirstName,@LastName,@Phone,@Phone2);SELECT SCOPE_IDENTITY();";
            db = new DbService();
            try
            {
                db.ExecuteQuery(query, cmd.CommandType, cmdParams);
            }
            catch (SqlException e)
            {
                throw;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        int managerId = getDestinationManagerId();

        return managerId;
    }

    public int getDestinationManagerId()
    {
        #region DB functions
        SqlCommand cmd = new SqlCommand();
        cmd.CommandType = CommandType.Text;
        SqlParameter[] cmdParams = new SqlParameter[1];
        cmdParams[0] = cmd.Parameters.AddWithValue("name", ManagerName);
        string query = "select * from DestinationManagers where FirstName=@name";


        DestinationManager m = new DestinationManager();
        DbService db = new DbService();
        DataSet ds = db.GetDataSetByQuery(query, cmd.CommandType, cmdParams);
        DataRow dr = ds.Tables[0].Rows[0];

        m.Id = int.Parse(dr["Id"].ToString());
        #endregion

        return m.Id;
    }


}