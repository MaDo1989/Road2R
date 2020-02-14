using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

/// <summary>
/// 
/// </summary>
public class Version
{
    public string UserName { get; set; }
    public string GoogleStoreURL { get; set; }
    public string AppStoreURL { get; set; }
    public DateTime RealseDate { get; set; }
    public DateTime UpdateDate { get; set; }
    public string VersionName { get; set; }
    public bool IsMandatory { get; set; }
    public Version()  {   }
    public Version(string userName,string google,string appstore,DateTime date,string version,bool mandatory)
    {
        UserName = userName;
        GoogleStoreURL = google;
        AppStoreURL = appstore;
        VersionName = version;
        RealseDate = date;
        IsMandatory = mandatory;
    }

    public void setNewVersion(string userName, string google, string appstore, DateTime date, string version,bool mandatory)
    {
        int Id;
        DbService db = new DbService();
        SqlCommand cmd = new SqlCommand();
        cmd.CommandType = CommandType.Text;
        SqlParameter[] cmdParams = new SqlParameter[6];

        string versionCheck = version.Replace(".","");

        try
        {
            List<Version> lv =  this.getVersions();
            if (int.Parse(lv[0].VersionName)>=int.Parse(versionCheck))
            {
                return;
            } 
        }
        catch (Exception)
        {

            
        }


        cmdParams[0] = cmd.Parameters.AddWithValue("@UserName", userName);
        cmdParams[1] = cmd.Parameters.AddWithValue("@GoogleStoreURL", google);
        cmdParams[2] = cmd.Parameters.AddWithValue("@AppStoreURL", appstore);
        cmdParams[3] = cmd.Parameters.AddWithValue("@VersionName", version);
        cmdParams[4] = cmd.Parameters.AddWithValue("@RealseDate", date);
        cmdParams[5] = cmd.Parameters.AddWithValue("@IsMandatory", mandatory);

        string query = "";
        query = "INSERT INTO Version (UserName,VersionName, GoogleStoreURL,AppStoreURL,RealseDate,IsMandatory)";
        query += " values (@UserName,@VersionName,@GoogleStoreURL,@AppStoreURL,@RealseDate,@IsMandatory); select SCOPE_IDENTITY()";
        db = new DbService();
        try
        {
            
            //getting the id of this version.
            Id = int.Parse(db.GetObjectScalarByQuery(query, cmd.CommandType, cmdParams).ToString());

            //Regex r = new Regex(@"(https?://[^\s]+)");
            //google = r.Replace(google, "<a href=\"$1\">Google Play</a>");
            //Regex r1 = new Regex(@"(https?://[^\s]+)");
            //appstore = r1.Replace(appstore, "<a href=\"$1\">Appstore</a>");
            //Message m = new Message();
            //string msg = "עידכון גרסה,משתמשי אנדרואיד: " + google + "\nמשתמשי אפל: "+ appstore;
            //m.globalMessage(msg,"עדכון גרסה");
        }
        catch (SqlException ex)
        {
            throw;
        }

    }
    public List<Version> getVersions()
    {
        #region DB functions
        string query = "select top 1 * from version order by Date DESC";
        List<Version> versionList = new List<Version>();
        DbService db = new DbService();
        DataSet ds = db.GetDataSetByQuery(query);

        foreach (DataRow dr in ds.Tables[0].Rows)
        {
            Version tmp = new Version();
            tmp.UserName = dr["UserName"].ToString();
            tmp.VersionName = dr["VersionName"].ToString();
            tmp.GoogleStoreURL = dr["GoogleStoreURL"].ToString();
            tmp.AppStoreURL = dr["AppStoreURL"].ToString();
            tmp.RealseDate = Convert.ToDateTime(dr["RealseDate"].ToString());
            tmp.UpdateDate = Convert.ToDateTime(dr["Date"].ToString());
            tmp.IsMandatory = Convert.ToBoolean(dr["IsMandatory"].ToString());

            versionList.Add(tmp);
        }
        #endregion

        return versionList;

    }

}