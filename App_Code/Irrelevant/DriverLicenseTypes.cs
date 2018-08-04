using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;


/// <summary>
/// Summary description for DriverLicenseTypes
/// </summary>
public class DriverLicenseTypes
{
    int driverLicenseTypeID;
    string driverLicenseTypeName;
    string driverLicenseTypeDescription;
    string licenseOrCertification;

    public int DriverLicenseTypeID
    {
        get
        {
            return driverLicenseTypeID;
        }

        set
        {
            driverLicenseTypeID = value;
        }
    }

    public string DriverLicenseTypeName
    {
        get
        {
            return driverLicenseTypeName;
        }

        set
        {
            driverLicenseTypeName = value;
        }
    }

    public string DriverLicenseTypeDescription
    {
        get
        {
            return driverLicenseTypeDescription;
        }

        set
        {
            driverLicenseTypeDescription = value;
        }
    }

    public string LicenseOrCertification
    {
        get
        {
            return licenseOrCertification;
        }

        set
        {
            licenseOrCertification = value;
        }
    }

    public DriverLicenseTypes()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public DriverLicenseTypes(int driverLicenseTypeID, string driverLicenseTypeName, string driverLicenseTypeDescription, string licenseOrCertification)
    {
        DriverLicenseTypeID = driverLicenseTypeID;
        DriverLicenseTypeName = driverLicenseTypeName;
        DriverLicenseTypeDescription = driverLicenseTypeDescription;
        LicenseOrCertification = licenseOrCertification;
    }


    public List<DriverLicenseTypes> getDriverLicenseTypesList()
    {
        #region DB functions
        string query = "select * from DriverLicenseTypes";

        List<DriverLicenseTypes> list = new List<DriverLicenseTypes>();
        DbService db = new DbService();
        DataSet ds = db.GetDataSetByQuery(query);

        foreach (DataRow dr in ds.Tables[0].Rows)
        {
            DriverLicenseTypes tmp = new DriverLicenseTypes((int)dr["DriverLicenseTypeID"], dr["DriverLicenseTypeName"].ToString(), dr["DriverLicenseDescription"].ToString(), dr["PermissionORCertification"].ToString());
            list.Add(tmp);
        }
        #endregion

        return list;

    }

    public List<DriverLicenseTypes> getDriverLicenseTypesList(string LicenseORCertification)
    {
        #region DB functions
        string query = "select * from DriverLicenseTypes where PermissionORCertification = '"+ LicenseORCertification + "' order by DriverLicenseTypeName";

        List<DriverLicenseTypes> list = new List<DriverLicenseTypes>();
        DbService db = new DbService();
        DataSet ds = db.GetDataSetByQuery(query);

        foreach (DataRow dr in ds.Tables[0].Rows)
        {
            DriverLicenseTypes tmp = new DriverLicenseTypes((int)dr["DriverLicenseTypeID"], dr["DriverLicenseTypeName"].ToString(), dr["DriverLicenseDescription"].ToString(), dr["PermissionORCertification"].ToString());
            list.Add(tmp);
        }
        #endregion

        return list;

    }

    public DriverLicenseTypes getDriverLicenseType(int driverID, string licenseORCertification)
    {
        #region DB functions
        string query = "";
        if (licenseORCertification == "רישיון")
        {
            query = "select * from DriverLicenseTypes dlt inner join Drivers d on dlt.DriverLicenseTypeID = d.DriverLicenseID where d.DriverID =" + driverID + "";
        }

        else
        {
            query = "select * from DriverLicenseTypes dlt inner join Drivers d on dlt.DriverLicenseTypeID = d.DriverCertificationID where d.DriverID =" + driverID + "";
        }
        

        DriverLicenseTypes dlt = new DriverLicenseTypes();
        DbService db = new DbService();
        DataSet ds = db.GetDataSetByQuery(query);

        foreach (DataRow dr in ds.Tables[0].Rows)
        {
            dlt.DriverLicenseTypeID = (int)dr["DriverLicenseTypeID"];
            dlt.DriverLicenseTypeName = dr["DriverLicenseTypeName"].ToString();
            dlt.DriverLicenseTypeDescription = dr["DriverLicenseDescription"].ToString();
            dlt.LicenseOrCertification = dr["PermissionORCertification"].ToString();
        }
        #endregion

        return dlt;
    }

    public DriverLicenseTypes getDriverLicenseType()
    {
        #region DB functions
        string query = "";
        
        query = "select * from DriverLicenseTypes dlt where dlt.DriverLicenseTypeID =" + driverLicenseTypeID + "";


        DriverLicenseTypes dlt = new DriverLicenseTypes();
        DbService db = new DbService();
        DataSet ds = db.GetDataSetByQuery(query);

        foreach (DataRow dr in ds.Tables[0].Rows)
        {
            dlt.DriverLicenseTypeID = (int)dr["DriverLicenseTypeID"];
            dlt.DriverLicenseTypeName = dr["DriverLicenseTypeName"].ToString();
            dlt.DriverLicenseTypeDescription = dr["DriverLicenseDescription"].ToString();
            dlt.LicenseOrCertification = dr["PermissionORCertification"].ToString();
        }
        #endregion

        return dlt;
    }

    public int getDriverLicenseTypeID()
    {
        #region DB functions
        string query = "select * from DriverLicenseTypes where DriverLicenseTypeName = '" + DriverLicenseTypeName + "'";

        List<DriverLicenseTypes> list = new List<DriverLicenseTypes>();
        DbService db = new DbService();
        DataSet ds = db.GetDataSetByQuery(query);

        int ID = 0;
        foreach (DataRow dr in ds.Tables[0].Rows)
        {
            ID = (int)dr["DriverLicenseTypeID"];
        }
        #endregion

        return ID;
    }
}