using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;

/// <summary>
/// Summary description for Drivers
/// </summary>
public class DriverConstraints
{
    int driverConstraintID;
    DateTime date;
    Drivers driver;
    string comments;
    string active;



    public int DriverConstraintID
    {
        get { return driverConstraintID; }
        set { driverConstraintID = value; }
    }
    public DateTime Date
    {
        get { return date; }
        set { date = value; }
    }
    public Drivers Driver
    {
        get { return driver; }
        set { driver = value; }
    }
    public string Comments
    {
        get { return comments; }
        set { comments = value; }
    }
    public string Active
    {
        get { return active; }
        set { active = value; }
    }

    public DriverConstraints()
    {
        //empty constractor
    }
    public DriverConstraints(int driverConstraintID, DateTime date, Drivers driver, string comments)
    {
        DriverConstraintID = driverConstraintID;
        Date = date;
        Driver = driver;
        Comments = comments;
        Active = active;
    }


    public List<DriverConstraints> getDriverConstraintsList(bool active, int selectedDriver, DateTime startDate, DateTime endDate)
    {
        #region DB functions
        ////DateTime dt = DateTime.ParseExact(date, "dd/MM/yyyy", null);
        string sqlStartDate = startDate.ToString("yyyy-MM-dd");
        string sqlEndtDate = endDate.ToString("yyyy-MM-dd");

        string query = "select * from DriverConstraints c inner join Drivers d on d.DriverID = c.DriverID where 1=1";
        if (active)
        {
            query += " and c.Active = 'Y'";
        }
        if (selectedDriver != -1)
        {
            query += " and d.DriverID =" + selectedDriver;
        }
        if (startDate.Year != 1)
        {
            query += " and c.Date >= '" + sqlStartDate + "'";
            if (endDate.Year != 1)
            {
                query += " and c.Date <= '" + sqlEndtDate + "'";
            }
        }

        List<DriverConstraints> list = new List<DriverConstraints>();
        DbService db = new DbService();
        DataSet ds = db.GetDataSetByQuery(query);

        foreach (DataRow dr in ds.Tables[0].Rows)
        {

            DriverConstraints tmp = new DriverConstraints();
            tmp.DriverConstraintID = (int)dr["DriverConstraintID"];
            tmp.Date = (DateTime)dr["Date"];
            Drivers d = new Drivers();
            d.DriverID = (int)dr["DriverID"];
            tmp.Driver = d.getDriver();
            tmp.Comments = dr["Comments"].ToString();
            tmp.Active = dr["Active"].ToString();

            list.Add(tmp);
        }
        #endregion
        return list;
    }

    public List<DriverConstraints> getDriverConstraintsListForView(bool active, int selectedDriver, DateTime startDate, DateTime endDate)
    {
        #region DB functions
        ////DateTime dt = DateTime.ParseExact(date, "dd/MM/yyyy", null);
        string sqlStartDate = startDate.ToString("yyyy-MM-dd");
        string sqlEndtDate = endDate.ToString("yyyy-MM-dd");

        string query = "select c.DriverConstraintID, c.Date, d.FirstName, d.LastName, c.Comments, c.Active from DriverConstraints c inner join Drivers d on c.DriverID = d.DriverID where 1=1";
        if (active)
        {
            query += " and c.Active = 'Y'";
        }
        if (selectedDriver != -1)
        {
            query += " and d.DriverID =" + selectedDriver;
        }
        if (startDate.Year != 1)
        {
            query += " and c.Date >= '" + sqlStartDate + "'";
            if (endDate.Year != 1)
            {
                query += " and c.Date <= '" + sqlEndtDate + "'";
            }
        }

        List<DriverConstraints> list = new List<DriverConstraints>();
        DbService db = new DbService();
        DataSet ds = db.GetDataSetByQuery(query);

        foreach (DataRow dr in ds.Tables[0].Rows)
        {

            DriverConstraints tmp = new DriverConstraints();
            tmp.DriverConstraintID = (int)dr["DriverConstraintID"];
            tmp.Date = (DateTime)dr["Date"];
            Drivers d = new Drivers();
            d.FirstName = dr["FirstName"].ToString();
            d.LastName = dr["LastName"].ToString();
            tmp.Driver = d;
            tmp.Comments = dr["Comments"].ToString();
            tmp.Active = dr["Active"].ToString();

            list.Add(tmp);
        }
        #endregion
        return list;
    }


    public void deactivateDriverConstraints(string active)
    {
        DbService db = new DbService();
        db.ExecuteQuery("UPDATE DriverConstraints SET Active='" + active + "' WHERE DriverConstraintID=" + DriverConstraintID);
    }

    public void setDriverConstraint(string func)
    {
        DbService db = new DbService();
        string query = "";
        string sqlDate = Date.ToString("yyyy-MM-dd");
        if (func == "edit")
        {
            query = "UPDATE DriverConstraints SET Date = Convert(varchar,'" + sqlDate + "',103), DriverID = " + Driver.DriverID + ", Comments = '" + Comments + "' WHERE DriverConstraintID = " + DriverConstraintID;
        }
        else if (func == "new")
        {
            query = "insert into DriverConstraints values ('" + sqlDate + "'," + Driver.DriverID + ",'" + Comments + "','Y')";
        }
        db.ExecuteQuery(query);
    }

}
