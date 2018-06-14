using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;

/// <summary>
/// Summary description for User
/// </summary>
public class User
{
    int userID;
    string userName;
    string password;
    string userType;


    public int UserID
    {
        get
        {
            return userID;
        }

        set
        {
            userID = value;
        }
    }

    public string UserName
    {
        get
        {
            return userName;
        }

        set
        {
            userName = value;
        }
    }

    public string Password
    {
        get
        {
            return password;
        }

        set
        {
            password = value;
        }
    }

    public string UserType
    {
        get
        {
            return userType;
        }

        set
        {
            userType = value;
        }
    }

    public User()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public User(int userID, string userName, string password, string userType)
    {
        UserID = userID;
        UserName = userName;
        Password = password;
        UserType = userType;
    }

    public User(string userName, string password)
    {
        UserName = userName;
        Password = password;
    }

    public bool CheckLoginDetails()
    {
        #region DB functions
        string query = "select * from Volunteer where UserName ='" + userName + "'";

        DbService db = new DbService();
        DataSet ds = db.GetDataSetByQuery(query);
        DataTable dt = ds.Tables[0];
        bool userInDB = false;

        if (dt != null && dt.Rows.Count > 0)
        {
            DataRow dr = dt.Rows[0];
            if (dr["Password"].ToString() == Password)
            {
                userInDB = true;
            }
        }

        #endregion

        return userInDB;
    }

    public string getUserType(string userName)
    {
        SqlCommand cmd = new SqlCommand();
        cmd.CommandType = CommandType.Text;
        SqlParameter[] cmdParams = new SqlParameter[1];
        cmdParams[0] = cmd.Parameters.AddWithValue("@user", userName);
        string query = "select VolunTypeType from VolunteerTypeView where UserName =@user";
        DbService db = new DbService();
        DataSet ds = db.GetDataSetByQuery(query, cmd.CommandType, cmdParams);
        DataRow dr = ds.Tables[0].Rows[0];
        string type = dr["VolunTypeType"].ToString();
        return type;
    }
}