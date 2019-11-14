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
    string englishName;
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
        string query = "select * from Volunteer where UserName =N'" + userName + "'";

        DbService db = new DbService();
        DataSet ds = db.GetDataSetByQuery(query);
        DataTable dt = ds.Tables[0];
        bool userInDB = false;

        if (dt != null && dt.Rows.Count > 0)
        {
            DataRow dr = dt.Rows[0];
            if (dr["Password"].ToString() == Password && (bool)dr["IsActive"])
            {
                userInDB = true;
            }
        }

        #endregion

        return userInDB;
    }
    public string getUserNameByCellphone(string cellphone)
    {
        #region DB functions
        string query = "select displayName,englishname from Volunteer where Cellphone ='" + cellphone + "'";

        DbService db = new DbService();
        DataSet ds = db.GetDataSetByQuery(query);
        DataRow dr = ds.Tables[0].Rows[0];
        string userName = dr["DisplayName"].ToString();
        string userEnglishName = dr["EnglishName"].ToString();
        //User u = new User();
        //u.EnglishName = userEnglishName;
        //u.UserName = userName;
        return userName;
        #endregion
    }
    public string getUserEnglishNameByCellphone(string cellphone)
    {
        #region DB functions
        string query = "select displayName,englishname from Volunteer where Cellphone ='" + cellphone + "'";

        DbService db = new DbService();
        DataSet ds = db.GetDataSetByQuery(query);
        DataRow dr = ds.Tables[0].Rows[0];
        string userName = dr["DisplayName"].ToString();
        string userEnglishName = dr["EnglishName"].ToString();
        return userEnglishName;
        #endregion
    }
    public bool GetIsAssistantByCellphone(string cellphone)
    {
        #region DB functions
        string query = "select isAssistant from Volunteer where Cellphone ='" + cellphone + "'";

        DbService db = new DbService();
        DataSet ds = db.GetDataSetByQuery(query);
        DataRow dr = ds.Tables[0].Rows[0];
        bool isAssistant = Convert.ToBoolean(dr["isAssistant"].ToString());
        return isAssistant;
        #endregion
    }
    public string getUserType(string userName)
    {
        SqlCommand cmd = new SqlCommand();
        cmd.CommandType = CommandType.Text;
        SqlParameter[] cmdParams = new SqlParameter[1];
        cmdParams[0] = cmd.Parameters.AddWithValue("@user", userName);
        string query = "select VolunTypeType from VolunteerTypeView where UserName =@user";
        DbService db = new DbService();
        DataSet ds = db.GetDataSetByQuery(query,true, cmd.CommandType, cmdParams);
        DataRow dr = ds.Tables[0].Rows[0];
        string type = dr["VolunTypeType"].ToString();
        return type;
    }
}