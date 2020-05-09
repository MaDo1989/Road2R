using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web;

public class ReportService
{


    private DataTable getDriverByID(int driverID, DbService db)
    {
        string query = "select Id,DisplayName,CellPhone from Volunteer where Id = @ID";
        SqlCommand cmd = new SqlCommand(query);
        cmd.CommandType = CommandType.Text;
        cmd.Parameters.Add("@ID", SqlDbType.Int).Value = driverID;

        DataSet ds = db.GetDataSetBySqlCommand(cmd);
        DataTable dt = ds.Tables[0];
        return dt;
    }


    private DataTable getPickupForDriver(int driverID, string start_date, string end_date, DbService db)
    {
        string query = "SELECT* FROM RPView WHERE pickuptime < @end_date AND pickuptime >= @start_date AND MainDriver =  @ID";
        SqlCommand cmd = new SqlCommand(query);
        cmd.CommandType = CommandType.Text;
        cmd.Parameters.Add("@ID", SqlDbType.Int).Value = driverID;
        cmd.Parameters.Add("@start_date", SqlDbType.Date).Value = start_date;
        cmd.Parameters.Add("@end_date", SqlDbType.Date).Value = end_date;

        DataSet ds = db.GetDataSetBySqlCommand(cmd);
        DataTable dt = ds.Tables[0];
        return dt;
    }

    private DataTable getVolunteerRidesPerWeek(string start_date, string end_date, DbService db)
    {
        /*
         * SELECT RidePat.Patient , Volunteer.DisplayName, RidePat.Area ,RidePat.PickupTime, RidePat.MainDriver
FROM RidePat
INNER JOIN Volunteer ON RidePat.MainDriver=Volunteer.Id
AND RidePat.pickuptime < '2020-3-01'
AND RidePat.pickuptime >= '2020-1-01'

    */

        string query = @" SELECT RidePat.Patient , Volunteer.DisplayName, RidePat.Area ,RidePat.PickupTime, RidePat.MainDriver
                        FROM RidePat
                        INNER JOIN Volunteer ON RidePat.MainDriver=Volunteer.Id
                        AND RidePat.pickuptime < '2020-3-01'
                        AND RidePat.pickuptime >= '2020-1-01'";

        SqlCommand cmd = new SqlCommand(query);
        cmd.CommandType = CommandType.Text;
        //cmd.Parameters.Add("@ID", SqlDbType.Int).Value = driverID;
        //cmd.Parameters.Add("@start_date", SqlDbType.Date).Value = start_date;
        //cmd.Parameters.Add("@end_date", SqlDbType.Date).Value = end_date;

        DataSet ds = db.GetDataSetBySqlCommand(cmd);
        DataTable dt = ds.Tables[0];
        return dt;
    }

    private DataTable getRides()
    {
        DbService db = new DbService();
        SqlCommand cmd = new SqlCommand();
        //SqlParameter[] cmdparams= new SqlParameter[1];
        cmd.CommandType = CommandType.Text;
        //cmdparams[0] = cmd.Parameters.AddWithValue("id", id);
        string query = "select * from status_Ride order by Timestamp asc";
        DataSet ds = db.GetDataSetByQuery(query);
        DataTable dt = ds.Tables[0];
        return dt;
    }

    private DataTable getEscorts()
    {
        DbService db = new DbService();
        SqlCommand cmd = new SqlCommand();
        //SqlParameter[] cmdparams= new SqlParameter[1];
        cmd.CommandType = CommandType.Text;
        //cmdparams[0] = cmd.Parameters.AddWithValue("id", id);
        string query = "select * from RidePatEscortView";
        DataSet ds = db.GetDataSetByQuery(query);
        DataTable dt = ds.Tables[0];
        return dt;
    }


    public List<RidePat> GetReportVolunteerRides(int volunteerId, string start_date, string end_date)
    {
        if (volunteerId <= 0)
        {
            throw new ArgumentException("Negative volunteerId is not supported");
        }

        RidePat helper_RidePat = new RidePat();

        DbService db = new DbService();
        Location tmp = new Location();
        Hashtable locations = tmp.getLocationsEnglishName();
        DataTable driverTable = getDriverByID(volunteerId, db);

        //TODO: re-implement getRides using sql-command
        DataTable rideTable = getRides();

        List<Escorted> el = new List<Escorted>();
        DataTable escortTable = getEscorts();


        DataTable pickupsTable = getPickupForDriver(volunteerId, start_date, end_date, db);
        List<RidePat> rpl = new List<RidePat>();

        int counter = 0;

        try
        {
            foreach (DataRow dr in pickupsTable.Rows)
            {
                try
                {
                    counter++;

                    RidePat rp = new RidePat();
                    rp.Coordinator = new Volunteer();
                    rp.Coordinator.DisplayName = dr["Coordinator"].ToString();
                    rp.Drivers = new List<Volunteer>();

                    if (dr["MainDriver"].ToString() != "")
                    {

                        Volunteer primary = new Volunteer();
                        primary.DriverType = "Primary";

                        primary.Id = int.Parse(dr["MainDriver"].ToString());
                        DataRow driverRow = driverTable.Rows[0];
                        primary.DisplayName = driverRow["DisplayName"].ToString();
                        primary.CellPhone = driverRow["CellPhone"].ToString();
                        rp.Drivers.Add(primary);
                    }


                    // if (numOfDrivers > 1)
                    // {
                    if (dr["secondaryDriver"].ToString() != "")
                    {
                        // Do we care about these for a report? 
                        if (false)
                        {
                            throw new Exception("Secondary Driver lookup not implemented yet");
                            Volunteer secondary = new Volunteer();
                            secondary.DriverType = "secondary";
                            secondary.Id = int.Parse(dr["secondaryDriver"].ToString());
                            string searchExpression = "Id = " + secondary.Id;
                            DataRow[] driverRow = driverTable.Select(searchExpression);
                            secondary.DisplayName = driverRow[0]["DisplayName"].ToString();
                            secondary.CellPhone = driverRow[0]["CellPhone"].ToString();
                            rp.Drivers.Add(secondary);
                        }
                    }

                    rp.RidePatNum = int.Parse(dr["RidePatNum"].ToString());
                    try
                    {
                        rp.RideNum = int.Parse(dr["RideNum"].ToString());
                    }
                    catch (Exception)
                    {

                    }

                    Patient thePatient = new Patient();
                    thePatient.DisplayName = dr["DisplayName"].ToString();
                    thePatient.EnglishName = dr["EnglishName"].ToString();
                    thePatient.CellPhone = dr["CellPhone"].ToString();
                    thePatient.IsAnonymous = dr["IsAnonymous"].ToString();
                    thePatient.Id = int.Parse(dr["Id"].ToString());

                    rp.Pat = thePatient;

                    rp.Pat.EscortedList = new List<Escorted>();
                    string escortSearchExpression = "RidePatNum = " + rp.RidePatNum;
                    DataRow[] escortRow = escortTable.Select(escortSearchExpression);
                    foreach (DataRow row in escortRow)
                    {
                        Escorted e = new Escorted();
                        e.Id = int.Parse(row[0].ToString());
                        e.DisplayName = row[1].ToString();
                        rp.Pat.EscortedList.Add(e);
                    }


                    Location origin = new Location();
                    origin.Name = dr["Origin"].ToString();
                    if (locations[origin.Name] == null)
                    {
                        origin.EnglishName = "";
                    }
                    else origin.EnglishName = locations[origin.Name].ToString();
                    rp.Origin = origin;
                    Location dest = new Location();
                    dest.Name = dr["Destination"].ToString();
                    if (locations[dest.Name] == null)
                    {
                        dest.EnglishName = "";
                    }
                    else dest.EnglishName = locations[dest.Name].ToString();
                    rp.Destination = dest;
                    rp.Area = dr["Area"].ToString();
                    rp.Shift = dr["Shift"].ToString();
                    rp.Date = Convert.ToDateTime(dr["PickupTime"].ToString());
                    rp.Status = dr["Status"].ToString();
                    if (rp.RideNum > 0) // if RidePat is assigned to a Ride - Take the Ride's status
                    {
                        string searchExpression = "RideRideNum = " + rp.RideNum;
                        DataRow[] rideRow = rideTable.Select(searchExpression);
                        //rideRow = rideRow.OrderBy(x => x.TimeOfDay).ToList();
                        rp.Statuses = new List<string>();
                        foreach (DataRow status in rideRow)
                        {
                            rp.Statuses.Add(status.ItemArray[0].ToString());
                        }
                        try
                        {
                            rp.Status = rp.Statuses[rp.Statuses.Count - 1];
                        }
                        catch (Exception err)
                        {

                            throw err;
                        }


                    }

                    rpl.Add(rp);
                }
                catch (Exception ex)
                {

                    throw ex;
                }


            }

            return rpl;
        }
        catch (Exception e)
        {
            throw e;
        }

    }


    public List<RidePat> GetReportRidesWeeklyPerRegion(string start_date, string end_date)
    {
        DbService db = new DbService();
        Location tmp = new Location();
        Hashtable locations = tmp.getLocationsEnglishName();


        DataTable pickupsTable = getVolunteerRidesPerWeek(start_date, end_date, db);
        List<RidePat> rpl = new List<RidePat>();

        int counter = 0;

        try
        {
            foreach (DataRow dr in pickupsTable.Rows)
            {
                try
                {
                    counter++;

                    RidePat rp = new RidePat();
                    rp.Coordinator = new Volunteer();
                    rp.Coordinator.DisplayName = dr["Coordinator"].ToString();
                    rp.Drivers = new List<Volunteer>();

                    if (dr["MainDriver"].ToString() != "")
                    {

                        Volunteer primary = new Volunteer();
                        primary.DriverType = "Primary";

                        primary.Id = int.Parse(dr["MainDriver"].ToString());
                        DataRow driverRow = driverTable.Rows[0];
                        primary.DisplayName = driverRow["DisplayName"].ToString();
                        primary.CellPhone = driverRow["CellPhone"].ToString();
                        rp.Drivers.Add(primary);
                    }


                    // if (numOfDrivers > 1)
                    // {
                    if (dr["secondaryDriver"].ToString() != "")
                    {
                        // Do we care about these for a report? 
                        if (false)
                        {
                            throw new Exception("Secondary Driver lookup not implemented yet");
                            Volunteer secondary = new Volunteer();
                            secondary.DriverType = "secondary";
                            secondary.Id = int.Parse(dr["secondaryDriver"].ToString());
                            string searchExpression = "Id = " + secondary.Id;
                            DataRow[] driverRow = driverTable.Select(searchExpression);
                            secondary.DisplayName = driverRow[0]["DisplayName"].ToString();
                            secondary.CellPhone = driverRow[0]["CellPhone"].ToString();
                            rp.Drivers.Add(secondary);
                        }
                    }

                    rp.RidePatNum = int.Parse(dr["RidePatNum"].ToString());
                    try
                    {
                        rp.RideNum = int.Parse(dr["RideNum"].ToString());
                    }
                    catch (Exception)
                    {

                    }

                    Patient thePatient = new Patient();
                    thePatient.DisplayName = dr["DisplayName"].ToString();
                    thePatient.EnglishName = dr["EnglishName"].ToString();
                    thePatient.CellPhone = dr["CellPhone"].ToString();
                    thePatient.IsAnonymous = dr["IsAnonymous"].ToString();
                    thePatient.Id = int.Parse(dr["Id"].ToString());

                    rp.Pat = thePatient;

                    rp.Pat.EscortedList = new List<Escorted>();
                    string escortSearchExpression = "RidePatNum = " + rp.RidePatNum;
                    DataRow[] escortRow = escortTable.Select(escortSearchExpression);
                    foreach (DataRow row in escortRow)
                    {
                        Escorted e = new Escorted();
                        e.Id = int.Parse(row[0].ToString());
                        e.DisplayName = row[1].ToString();
                        rp.Pat.EscortedList.Add(e);
                    }


                    Location origin = new Location();
                    origin.Name = dr["Origin"].ToString();
                    if (locations[origin.Name] == null)
                    {
                        origin.EnglishName = "";
                    }
                    else origin.EnglishName = locations[origin.Name].ToString();
                    rp.Origin = origin;
                    Location dest = new Location();
                    dest.Name = dr["Destination"].ToString();
                    if (locations[dest.Name] == null)
                    {
                        dest.EnglishName = "";
                    }
                    else dest.EnglishName = locations[dest.Name].ToString();
                    rp.Destination = dest;
                    rp.Area = dr["Area"].ToString();
                    rp.Shift = dr["Shift"].ToString();
                    rp.Date = Convert.ToDateTime(dr["PickupTime"].ToString());
                    rp.Status = dr["Status"].ToString();
                    if (rp.RideNum > 0) // if RidePat is assigned to a Ride - Take the Ride's status
                    {
                        string searchExpression = "RideRideNum = " + rp.RideNum;
                        DataRow[] rideRow = rideTable.Select(searchExpression);
                        //rideRow = rideRow.OrderBy(x => x.TimeOfDay).ToList();
                        rp.Statuses = new List<string>();
                        foreach (DataRow status in rideRow)
                        {
                            rp.Statuses.Add(status.ItemArray[0].ToString());
                        }
                        try
                        {
                            rp.Status = rp.Statuses[rp.Statuses.Count - 1];
                        }
                        catch (Exception err)
                        {

                            throw err;
                        }


                    }

                    rpl.Add(rp);
                }
                catch (Exception ex)
                {

                    throw ex;
                }


            }

            return rpl;
        }
        catch (Exception e)
        {
            throw e;
        }

    }

}