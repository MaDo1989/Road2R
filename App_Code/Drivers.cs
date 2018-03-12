using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;

/// <summary>
/// Summary description for Drivers
/// </summary>
public class Drivers
{
    int driverID;
    string driverNumber;
    string firstName;
    string lastName;
    string active;
    string phone;
    string email;
    string accountID;
    string appPassword;
    DateTime dateOfBirth;
    DateTime driverLicenseExpiredDate;
    DateTime driverCertificationExpiredDate;
    City cityLiving;
    DriverLicenseTypes licenseType;
    DriverLicenseTypes certificationType;
    Trucks driverTruck;

    public int DriverID
    {
        get
        {
            return driverID;
        }

        set
        {
            driverID = value;
        }
    }

    public string DriverNumber
    {
        get
        {
            return driverNumber;
        }

        set
        {
            driverNumber = value;
        }
    }

    public string FirstName
    {
        get
        {
            return firstName;
        }

        set
        {
            firstName = value;
        }
    }

    public string LastName
    {
        get
        {
            return lastName;
        }

        set
        {
            lastName = value;
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

    public string Phone
    {
        get
        {
            return phone;
        }

        set
        {
            phone = value;
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

    public string AppPassword
    {
        get
        {
            return appPassword;
        }

        set
        {
            appPassword = value;
        }
    }

    public City CityLiving
    {
        get
        {
            return cityLiving;
        }

        set
        {
            cityLiving = value;
        }
    }

    public DriverLicenseTypes LicenseType
    {
        get
        {
            return licenseType;
        }

        set
        {
            licenseType = value;
        }
    }

    public DriverLicenseTypes CertificationType
    {
        get
        {
            return certificationType;
        }

        set
        {
            certificationType = value;
        }
    }

    public Trucks DriverTruck
    {
        get
        {
            return driverTruck;
        }

        set
        {
            driverTruck = value;
        }
    }

    public DateTime DateOfBirth
    {
        get
        {
            return dateOfBirth;
        }

        set
        {
            dateOfBirth = value;
        }
    }

    public DateTime DriverLicenseExpiredDate
    {
        get
        {
            return driverLicenseExpiredDate;
        }

        set
        {
            driverLicenseExpiredDate = value;
        }
    }

    public DateTime DriverCertificationExpiredDate
    {
        get
        {
            return driverCertificationExpiredDate;
        }

        set
        {
            driverCertificationExpiredDate = value;
        }
    }

    public Drivers()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public Drivers(int driverID, string driverNumber, string firstName, string lastName, string active, string phone, string email, string accountID, string appPassword, DateTime dateOfBirth, DateTime driverLicenseExpiredDate, DateTime driverCertificationExpiredDate, City cityLiving, DriverLicenseTypes licenseType, DriverLicenseTypes certificationType, Trucks driverTruck)
    {
        DriverID = driverID;
        DriverNumber = driverNumber;
        FirstName = firstName;
        LastName = lastName;
        Active = active;
        Phone = phone;
        Email = email;
        AccountID = accountID;
        AppPassword = appPassword;
        CityLiving = cityLiving;
        LicenseType = licenseType;
        CertificationType = certificationType;
        DriverTruck = driverTruck;
        DateOfBirth = dateOfBirth;
        DriverLicenseExpiredDate = driverLicenseExpiredDate;
        DriverCertificationExpiredDate = driverCertificationExpiredDate;
    }

    public Drivers(int driverID, string driverNumber, string firstName, string lastName, string phone, string email, string accountID, string appPassword, DateTime dateOfBirth, DateTime driverLicenseExpiredDate, DateTime driverCertificationExpiredDate, City cityLiving, DriverLicenseTypes licenseType, DriverLicenseTypes certificationType, Trucks driverTruck)
    {
        DriverID = driverID;
        DriverNumber = driverNumber;
        FirstName = firstName;
        LastName = lastName;
        Phone = phone;
        Email = email;
        AccountID = accountID;
        AppPassword = appPassword;
        CityLiving = cityLiving;
        LicenseType = licenseType;
        CertificationType = certificationType;
        DriverTruck = driverTruck;
        DateOfBirth = dateOfBirth;
        DriverLicenseExpiredDate = driverLicenseExpiredDate;
        DriverCertificationExpiredDate = driverCertificationExpiredDate;
    }

    public List<Drivers> getDriversListForView(bool active)
    {
        #region DB functions
        string query = "select d.Active, d.DriverID ,d.FirstName ,d.LastName ,d.DriverNumber ,c.City ,d.Phone from Drivers d inner join City c on d.CityID = c.CityID where d.FirstName != 'בחר'";
        if (active)
        {
            query += " and d.Active = 'Y'";
        }
        query += "order by FirstName";

        List<Drivers> list = new List<Drivers>();
        DbService db = new DbService();
        DataSet ds = db.GetDataSetByQuery(query);

        foreach (DataRow dr in ds.Tables[0].Rows)
        {

            Drivers tmp = new Drivers();
            tmp.DriverID = (int)dr["DriverID"];
            tmp.DriverNumber = dr["DriverNumber"].ToString();
            tmp.FirstName = dr["FirstName"].ToString();
            tmp.LastName = dr["LastName"].ToString();
            tmp.Phone = dr["Phone"].ToString();
            tmp.Active = dr["Active"].ToString();
            //tmp.Email = dr["Email"].ToString();
            //tmp.AccountID = dr["AccountID"].ToString();
            //tmp.AppPassword = dr["AppPassword"].ToString();
            City c = new City();
            c.CityName = dr["City"].ToString();
            tmp.CityLiving = c;
            //tmp.CityLiving = new City((int)dr["CityID"], dr["City"].ToString(), dr["Area"].ToString());
            //tmp.DriverTruck = new Trucks((int)dr["TruckID"], (int)dr["TruckLicense"], dr["Manufacturer"].ToString(), dr["Model"].ToString(), dr["KMToDate"].ToString(), dr["Hand"].ToString(), float.Parse(dr["PurchaseCost"].ToString()), dr["PurchaseYear"].ToString(), new TruckTypes((int)dr["TruckTypeID"], dr["TruckType"].ToString()), dr["Urea"].ToString(), (DateTime)dr["OnRoadDate"], (DateTime)dr["InsuranceExpiredDate"]);

            //DriverLicenseTypes dlt = new DriverLicenseTypes();
            //tmp.CertificationType = dlt.getDriverLicenseType(tmp.DriverID, "היתר");
            //tmp.LicenseType = dlt.getDriverLicenseType(tmp.DriverID, "רישיון");

            list.Add(tmp);
        }
        #endregion

        return list;

    }

    public List<Drivers> getDriversList(bool active)
    {
        #region DB functions
        string query = "select * from Drivers d inner join City c on d.CityID = c.CityID inner join Trucks t on d.TruckID=t.TruckID inner join TruckTypes tp on t.TruckTypeID = tp.TruckTypeID where d.FirstName != 'בחר'";
        if (active)
        {
            query += " and d.Active = 'Y'";
        }

        query += " order by FirstName";

        List<Drivers> list = new List<Drivers>();
        DbService db = new DbService();
        DataSet ds = db.GetDataSetByQuery(query);

        foreach (DataRow dr in ds.Tables[0].Rows)
        {

            Drivers tmp = new Drivers();
            tmp.DriverID = (int)dr["DriverID"];
            tmp.DriverNumber = dr["DriverNumber"].ToString();
            tmp.FirstName = dr["FirstName"].ToString();
            tmp.LastName = dr["LastName"].ToString();
            tmp.Phone = dr["Phone"].ToString();
            tmp.Active = dr["Active"].ToString();
            tmp.Email = dr["Email"].ToString();
            tmp.AccountID = dr["AccountID"].ToString();
            tmp.AppPassword = dr["AppPassword"].ToString();
            tmp.CityLiving = new City((int)dr["CityID"], dr["City"].ToString(), dr["Area"].ToString(), (int)dr["Zone"]);
            tmp.DriverTruck = new Trucks((int)dr["TruckID"], (int)dr["TruckLicense"], dr["Manufacturer"].ToString(), dr["Model"].ToString(), dr["KMToDate"].ToString(), dr["Hand"].ToString(), float.Parse(dr["PurchaseCost"].ToString()), dr["PurchaseYear"].ToString(), new TruckTypes((int)dr["TruckTypeID"], dr["TruckType"].ToString()), dr["Urea"].ToString(), (DateTime)dr["OnRoadDate"], (DateTime)dr["InsuranceExpiredDate"]);

            DriverLicenseTypes dlt = new DriverLicenseTypes();
            tmp.CertificationType = dlt.getDriverLicenseType(tmp.DriverID, "היתר");
            tmp.LicenseType = dlt.getDriverLicenseType(tmp.DriverID, "רישיון");

            list.Add(tmp);
        }
        #endregion

        return list;

    }


    public Drivers getDriver()
    {
        #region DB functions
        string query = "select * from Drivers d inner join City c on d.CityID = c.CityID inner join Trucks t on d.TruckID=t.TruckID inner join TruckTypes tp on t.TruckTypeID = tp.TruckTypeID where d.DriverID = " + DriverID + "";

        Drivers d = new Drivers();
        DbService db = new DbService();
        DataSet ds = db.GetDataSetByQuery(query);

        foreach (DataRow dr in ds.Tables[0].Rows)
        {
            d.DriverID = (int)dr["DriverID"];
            d.DriverNumber = dr["DriverNumber"].ToString();
            d.FirstName = dr["FirstName"].ToString();
            d.LastName = dr["LastName"].ToString();
            d.Phone = dr["Phone"].ToString();
            d.Active = dr["Active"].ToString();
            d.Email = dr["Email"].ToString();
            d.AccountID = dr["AccountID"].ToString();
            d.AppPassword = dr["AppPassword"].ToString();
            d.CityLiving = new City((int)dr["CityID"], dr["City"].ToString(), dr["Area"].ToString(), (int)dr["Zone"]);
            d.DriverTruck = new Trucks((int)dr["TruckID"], (int)dr["TruckLicense"], dr["Manufacturer"].ToString(), dr["Model"].ToString(), dr["KMToDate"].ToString(), dr["Hand"].ToString(), float.Parse(dr["PurchaseCost"].ToString()), dr["PurchaseYear"].ToString(), new TruckTypes((int)dr["TruckTypeID"], dr["TruckType"].ToString()), dr["Urea"].ToString(), (DateTime)dr["OnRoadDate"], (DateTime)dr["InsuranceExpiredDate"]);
            try
            {
                d.DateOfBirth = (DateTime)dr["DateOfBirth"];
            }
            catch
            {
                d.DateOfBirth = new DateTime(1900, 01, 01);
            }
            try
            {
                d.DriverLicenseExpiredDate = (DateTime)dr["DriverLicenseExpiredDate"];
            }
            catch
            {
                d.DriverLicenseExpiredDate = new DateTime(1900, 01, 01);
            }
            try
            {
                d.DriverCertificationExpiredDate = (DateTime)dr["DriverCertificationExpiredDate"];
            }
            catch
            {
                d.DriverCertificationExpiredDate = new DateTime(1900, 01, 01);
            }

            DriverLicenseTypes dlt = new DriverLicenseTypes();
            d.CertificationType = dlt.getDriverLicenseType(d.DriverID, "היתר");
            d.LicenseType = dlt.getDriverLicenseType(d.DriverID, "רישיון");
        }
        #endregion

        return d;
    }

    public void setDriver(string func)
    {
        string sqlFormattedDateOfBirth = DateOfBirth.ToString("yyyy-MM-dd");
        string sqlFormattedDriverLicenseExpiredDate = DriverLicenseExpiredDate.ToString("yyyy-MM-dd");
        string sqlFormattedDriverCertificationExpiredDate = DriverCertificationExpiredDate.ToString("yyyy-MM-dd");

        DbService db = new DbService();
        string query = "";
        if (func == "edit")
        {
            query = "UPDATE Drivers SET DriverNumber = '" + DriverNumber + "', FirstName = '" + FirstName + "', LastName = '" + LastName + "', Phone = '" + Phone + "', Email = '" + Email + "', AccountID = '" + AccountID + "', AppPassword = '" + AppPassword + "', CityID = " + CityLiving.CityID + ", TruckID = " + DriverTruck.TruckID + ", DriverLicenseID = " + LicenseType.DriverLicenseTypeID + ", DriverCertificationID = " + CertificationType.DriverLicenseTypeID + ", DateOfBirth = '" + sqlFormattedDateOfBirth + "', DriverLicenseExpiredDate = '" + sqlFormattedDriverLicenseExpiredDate + "', DriverCertificationExpiredDate = '" + sqlFormattedDriverCertificationExpiredDate + "' WHERE DriverID = " + DriverID;
        }
        else if (func == "new")
        {
            query = "insert into Drivers values ('" + DriverNumber + "','" + FirstName + "','" + LastName + "','" + Phone + "','" + Email + "','Y','" + AccountID + "','" + AppPassword + "'," + CityLiving.CityID + ", " + DriverTruck.TruckID + ", " + LicenseType.DriverLicenseTypeID + ", " + CertificationType.DriverLicenseTypeID + ", '" + sqlFormattedDateOfBirth + "', '" + sqlFormattedDriverLicenseExpiredDate + "', '" + sqlFormattedDriverCertificationExpiredDate + "')";
        }
        db.ExecuteQuery(query);
    }


    public void deactivateDriver(string active)
    {
        DbService db = new DbService();
        db.ExecuteQuery("UPDATE Drivers SET Active='" + active + "' WHERE DriverID=" + DriverID);
    }

    public List<Drivers> getAvailableDriversList(DateTime selecetdDate)
    {
        #region DB functions
        string sqlDate = selecetdDate.ToString("yyyy-MM-dd");

        string query = "select * "
        + "from Drivers d left join "
+ "(select * "
+ "from DriverConstraints "
+ "where Date ='" + sqlDate+"' "
+ "and Active = 'Y') as dc on d.DriverID = dc.DriverID "
+ "inner join City c on d.CityID = c.CityID inner join Trucks t on d.TruckID=t.TruckID inner join TruckTypes tp on t.TruckTypeID = tp.TruckTypeID "
+ "where d.Active = 'Y' "
+ "and dc.Date is null "
+ "and d.FirstName != 'בחר'";

        List<Drivers> list = new List<Drivers>();
        DbService db = new DbService();
        DataSet ds = db.GetDataSetByQuery(query);

        foreach (DataRow dr in ds.Tables[0].Rows)
        {

            Drivers tmp = new Drivers();
            tmp.DriverID = (int)dr["DriverID"];
            tmp.DriverNumber = dr["DriverNumber"].ToString();
            tmp.FirstName = dr["FirstName"].ToString();
            tmp.LastName = dr["LastName"].ToString();
            tmp.Phone = dr["Phone"].ToString();
            tmp.Active = dr["Active"].ToString();
            tmp.Email = dr["Email"].ToString();
            tmp.AccountID = dr["AccountID"].ToString();
            tmp.AppPassword = dr["AppPassword"].ToString();
            tmp.CityLiving = new City((int)dr["CityID"], dr["City"].ToString(), dr["Area"].ToString(), (int)dr["Zone"]);
            tmp.DriverTruck = new Trucks((int)dr["TruckID"], (int)dr["TruckLicense"], dr["Manufacturer"].ToString(), dr["Model"].ToString(), dr["KMToDate"].ToString(), dr["Hand"].ToString(), float.Parse(dr["PurchaseCost"].ToString()), dr["PurchaseYear"].ToString(), new TruckTypes((int)dr["TruckTypeID"], dr["TruckType"].ToString()), dr["Urea"].ToString(), (DateTime)dr["OnRoadDate"], (DateTime)dr["InsuranceExpiredDate"]);
            try
            {
                tmp.DateOfBirth = (DateTime)dr["DateOfBirth"];
            }
            catch
            {
                tmp.DateOfBirth = new DateTime(1900, 01, 01);
            }
            try
            {
                tmp.DriverLicenseExpiredDate = (DateTime)dr["DriverLicenseExpiredDate"];
            }
            catch
            {
                tmp.DriverLicenseExpiredDate = new DateTime(1900, 01, 01);
            }
            try
            {
                tmp.DriverCertificationExpiredDate = (DateTime)dr["DriverCertificationExpiredDate"];
            }
            catch
            {
                tmp.DriverCertificationExpiredDate = new DateTime(1900, 01, 01);
            }

            DriverLicenseTypes dlt = new DriverLicenseTypes();
            tmp.CertificationType = dlt.getDriverLicenseType(tmp.DriverID, "היתר");
            tmp.LicenseType = dlt.getDriverLicenseType(tmp.DriverID, "רישיון");

            list.Add(tmp);
        }
        #endregion

        return list;

    }

    public Drivers(string driverNumber, string appPassword)
    {
        DriverNumber = driverNumber;
        AppPassword = appPassword;
    }

    public Drivers CheckLoginDetails()
    {
        #region DB functions
        string query = "select * from Drivers where DriverNumber ='" + DriverNumber + "' and AppPassword='" + AppPassword + "' and Active='Y'";

        DbService db = new DbService();
        DataSet ds = db.GetDataSetByQuery(query);
        DataTable dt = ds.Tables[0];
        Drivers d = new Drivers();

        if (dt != null && dt.Rows.Count > 0)
        {
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                d.DriverID = (int)dr["DriverID"];
                d.DriverTruck = new Trucks((int)dr["TruckID"]);
            }
        }

        #endregion

        return d;
    }

}
