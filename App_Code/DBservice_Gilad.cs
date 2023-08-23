using System.Data.SqlClient;
using System.Data;
using System.Diagnostics;
using System.Collections.Generic;
using System.Xml.Linq;
using System;
using System.Configuration;
//using Microsoft.Extensions.Configuration;
using System.Collections;
using System.Web.UI.WebControls;
using log4net.Util.TypeConverters;
using System.Activities.Expressions;
using System.Linq;
//using static ReportService;

public class DBservice_Gilad
{
	public SqlConnection con;




	public List<Patient> GetPatinetsByActiveStatus(bool active)
	{

		Location tmp = new Location();
		Hashtable locations = tmp.getLocationsEnglishName();
		//SqlConnection con;
		SqlCommand cmd;
		try
		{
			con = new SqlConnection(ConfigurationManager.ConnectionStrings["db"].ConnectionString);
			con.Open();
		}

		catch (Exception ex)
		{
			// write to log
			throw (ex);
		}

		//String cStr = BuildUpdateCommand(student);      // helper method to build the insert string
		Dictionary<string, object> paramDic = new Dictionary<string, object>();
		paramDic.Add("@active", active);


		cmd = CreateCommandWithStoredProcedureGeneral("sp_PatientsAndEquipment_Gilad", con, paramDic);             // create the command


		List<Patient> PatientsList = new List<Patient>();

		try
		{


			SqlDataReader dataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

			while (dataReader.Read())
			{

				Patient OnePatient = new Patient();
				//quest.QuestionId = Convert.ToInt32(dataReader["questionId"]);
				//quest.QuestionBody = dataReader["questionBody"].ToString();
				//quest.QuestionCurrectAnswer = dataReader["questionCurrectAnswer"].ToString();
				//quest.QuestionDistractingA = dataReader["questionDistractingA"].ToString();
				//quest.QuestionDistractingB = dataReader["questionDistractingB"].ToString();
				//quest.QuestionDistractingC = dataReader["questionDistractingC"].ToString();
				//quest.QuestionCategory = dataReader["questionCategory"].ToString();
				//quest.QuestionDifficulty = Convert.ToInt32(dataReader["questionDifficulty"]);
				////quest.PhotoId = Convert.ToInt32(dataReader["photoId"]);

				//questlist.Add(quest);

				OnePatient.Id = int.Parse(dataReader["Id"].ToString());
				OnePatient.IsAnonymous = dataReader["IsAnonymous"].ToString();
				OnePatient.NumberOfEscort = dataReader["NumberOfEscort"].ToString();
				OnePatient.DisplayName = dataReader["DisplayName"].ToString();
				OnePatient.DisplayNameA = dataReader["DisplayNameA"].ToString();
				OnePatient.FirstNameA = dataReader["FirstNameA"].ToString();
				OnePatient.FirstNameH = dataReader["FirstNameH"].ToString();
				OnePatient.LastNameH = dataReader["LastNameH"].ToString();
				OnePatient.LastNameA = dataReader["LastNameA"].ToString();
				OnePatient.CellPhone = dataReader["CellPhone"].ToString();
				OnePatient.CellPhone1 = dataReader["CellPhone2"].ToString();
				OnePatient.HomePhone = dataReader["HomePhone"].ToString();
				OnePatient.City = dataReader["CityCityName"].ToString();
				OnePatient.LivingArea = dataReader["LivingArea"].ToString();
				OnePatient.IsActive = Convert.ToBoolean(dataReader["IsACtive"].ToString());
				OnePatient.BirthDate = dataReader["BirthDate"].ToString();
				OnePatient.History = dataReader["History"].ToString();
				OnePatient.Department = dataReader["Department"].ToString();
				if (dataReader["PatientIdentity"].ToString() == "")
				{
					OnePatient.PatientIdentity = 0;
				}
				else OnePatient.PatientIdentity = int.Parse(dataReader["PatientIdentity"].ToString());
				string barrier = dataReader["Barrier"].ToString();
				OnePatient.Barrier = new Location(barrier);
				if (locations[barrier] != null)
				{
					OnePatient.Barrier.EnglishName = locations[barrier].ToString();
				}
				else OnePatient.Barrier.EnglishName = "";
				string hospital = dataReader["Hospital"].ToString();
				OnePatient.Hospital = new Location();
				OnePatient.Hospital.Name = hospital;
				if (locations[hospital] != null)
				{
					OnePatient.Hospital.EnglishName = locations[hospital].ToString();
				}
				else OnePatient.Hospital.EnglishName = "";
				OnePatient.Gender = dataReader["Gender"].ToString();
				OnePatient.Remarks = dataReader["Remarks"].ToString();
				OnePatient.EnglishName = dataReader["EnglishName"].ToString();
				List <string> el = new List<string>();
				//get equipment for patient from the same view
				string e = dataReader["EquipmentName"].ToString();

				OnePatient.LastModified = dataReader["lastModified"].ToString();
				el.Add(e);
				OnePatient.Equipment = el;

				PatientsList.Add(OnePatient);
			}

			return PatientsList;
		}
		catch (Exception ex)
		{
			// write to log
			throw (ex);
		}

		finally
		{
			if (con != null)
			{
				// close the db connection
				con.Close();
			}
		}
	}

    public List<RidePat> GetRidePatViewByTimeFilter_Gilad_DR(int from, int until)
    {
        
        //Gilad_gilad_
        //this method fetches ridepat which marked removed as well
        Location tmp = new Location();
        Hashtable locations = tmp.getLocationsEnglishName();

        //DataTable driverTable = getDriver();
        //DataTable equipmentTable = getEquipment();
        //DataTable rideTable = getRides();
        //DataTable escortTable = getEscorts();

        List<Escorted> el = new List<Escorted>();
        List<RidePat> rpl = new List<RidePat>();
        //string query = "exec spGet_rpview_ByTimeRange @from=" + from + ", @to=" + until;
        SqlCommand cmd;
        try
        {
            con = new SqlConnection(ConfigurationManager.ConnectionStrings["db"].ConnectionString);
            con.Open();
        }

        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }
        Dictionary<string, object> paramDic = new Dictionary<string, object>();
        paramDic.Add("@from", from);
        paramDic.Add("@to", until);
        cmd = CreateCommandWithStoredProcedureGeneral("spGet_rpview_ByTimeRange", con, paramDic);

        try
        {
            //DbService dbs = new DbService();
            //SqlDataReader sdr = dbs.GetDataReader(query);
            SqlDataReader dataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            while (dataReader.Read())
            {
                RidePat rp = new RidePat();
                rp.Coordinator = new Volunteer();
                rp.Coordinator.DisplayName = dataReader["Coordinator"].ToString();
                rp.Drivers = new List<Volunteer>();

                if (dataReader["MainDriver"].ToString() != "")
                {
                    Volunteer primary = new Volunteer();
                    primary.DriverType = "Primary";

                    primary.Id = int.Parse(dataReader["MainDriver"].ToString());

                    //string searchExpression = "Id = " + primary.Id;
                    //DataRow[] driverRow = driverTable.Select(searchExpression);
                    //primary.DisplayName = driverRow[0]["DisplayName"].ToString();
                    //primary.CellPhone = driverRow[0]["CellPhone"].ToString();

                    rp.Drivers.Add(primary);
                }

                rp.RidePatNum = int.Parse(dataReader["RidePatNum"].ToString());


                rp.RideNum = String.IsNullOrEmpty(dataReader["RideNum"].ToString()) ? -1 : (int)dataReader["RideNum"];

                rp.Pat = new Patient();
                rp.Pat.DisplayName = dataReader["DisplayName"].ToString();
                rp.Pat.EnglishName = dataReader["EnglishName"].ToString();
                rp.Pat.CellPhone = dataReader["CellPhone"].ToString();
                rp.Pat.IsAnonymous = dataReader["IsAnonymous"].ToString();

                rp.Pat.Id = int.Parse(dataReader["Id"].ToString());

                //rp.pat.Equipment = new List<string>();
                //string equipmentSearchExpression = "Id = " + rp.Pat.Id;
                //DataRow[] equipmentRow = equipmentTable.Select(equipmentSearchExpression);
                //foreach (DataRow row in equipmentRow)
                //{
                //    rp.pat.Equipment.Add(row.ItemArray[0].ToString());
                //}
                //rp.pat.EscortedList = new List<Escorted>();
                //string escortSearchExpression = "RidePatNum = " + rp.ridePatNum;
                //DataRow[] escortRow = escortTable.Select(escortSearchExpression);
                //foreach (DataRow row in escortRow)
                //{
                //    Escorted e = new Escorted();
                //    e.Id = int.Parse(row[0].ToString());
                //    e.DisplayName = row[1].ToString();
                //    rp.pat.EscortedList.Add(e);
                //}

                Location origin = new Location();
                origin.Name = dataReader["Origin"].ToString();
                if (locations[origin.Name] == null)
                {
                    origin.EnglishName = "";
                }
                else origin.EnglishName = locations[origin.Name].ToString();
                rp.Origin = origin;
                Location dest = new Location();
                dest.Name = dataReader["Destination"].ToString();
                if (locations[dest.Name] == null)
                {
                    dest.EnglishName = "";
                }
                else dest.EnglishName = locations[dest.Name].ToString();
                rp.Destination = dest;
                rp.Area = dataReader["Area"].ToString();
                rp.Shift = dataReader["Shift"].ToString();
                rp.Date = Convert.ToDateTime(dataReader["PickupTime"].ToString());
                rp.Status = dataReader["Status"].ToString();
                rp.LastModified =
                                  String.IsNullOrEmpty(dataReader["lastModified"].ToString()) ? null :
                                 (DateTime?)Convert.ToDateTime(dataReader["lastModified"].ToString());

                //if (rp.RideNum > 0 && rp.Status != "??? ????? ???? ??? ??? ?????") // if RidePat is assigned to a Ride - Take the Ride's status
                //{
                //    string searchExpression = "RideRideNum = " + rp.RideNum;
                //    DataRow[] rideRow = rideTable.Select(searchExpression);
                //    //rideRow = rideRow.OrderBy(x => x.TimeOfDay).ToList();
                //    rp.Statuses = new List<string>();
                //    foreach (DataRow status in rideRow)
                //    {
                //        rp.Statuses.Add(status.ItemArray[0].ToString());
                //    }
                //    try
                //    {
                //        rp.Status = rp.Statuses[rp.Statuses.Count - 1];
                //    }
                //    catch (Exception err)
                //    {

                //        throw err;
                //    }


                //}

                rpl.Add(rp);
            }
            return rpl;
        }
        catch (Exception ex)
        {
            throw new Exception("exception in DBservice_Gilad.cs ?  GetRidePatViewByTimeFilter_13/08" + ex);
        }
        finally
        {
            if (con != null)
            {
                // close the db connection
                con.Close();
            }
        }

    }

    public List<Absence> GetAbsenceByVolunteerId(int volunteerId)
    {
        SqlCommand cmd;
        try
        {
            con = new SqlConnection(ConfigurationManager.ConnectionStrings["db"].ConnectionString);
            con.Open();
        }

        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }
        Dictionary<string, object> paramDic = new Dictionary<string, object>();
        paramDic.Add("@id", volunteerId);
        cmd = CreateCommandWithStoredProcedureGeneral("GetVoluntterAbsenceById", con, paramDic);
        List<Absence> listToReturn = new List<Absence>();


        try
        {
            SqlDataReader dataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            while (dataReader.Read())
            {
                Absence absence = new Absence();
                absence.Id = Convert.ToInt32(dataReader["Id"]);
                absence.VolunteerId = Convert.ToInt32(dataReader["VolunteerId"]);
                absence.CoordinatorId = Convert.ToInt32(dataReader["CoordinatorId"]);
                absence.DaysToReturn = Convert.ToInt32(dataReader["DaysToReturn"]);
                absence.FromDate = Convert.ToDateTime(dataReader["FromDate"]);
                absence.UntilDate = Convert.ToDateTime(dataReader["UntilDate"]);
                absence.Cause = dataReader["Cause"].ToString();
                absence.Note = dataReader["Note"].ToString();
                absence.CoorName = dataReader["CoorName"].ToString();
                absence.AbsenceStatus = Convert.ToBoolean(dataReader["AbsenceStatus"]);
                absence.IsDeleted = Convert.ToBoolean(dataReader["isDeleted"]);
                listToReturn.Add(absence);



            }
            return listToReturn;
        }
        catch (Exception ex)
        {

            throw new Exception("exception in DBservice_Gilad.cs GetVoluntterAbsenceById sp -->" + ex);
        }
        finally 
        { 
            if (con != null)
            {
                con.Close();
            } 
        }
    }

    public int UpdateAbsenceById(int AbsenceId,int coorId, DateTime from, DateTime until , string cause,string note)
    {
        SqlCommand cmd;
        try
        {
            con = new SqlConnection(ConfigurationManager.ConnectionStrings["db"].ConnectionString);
            con.Open();
        }

        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }

        Dictionary<string, object> paramDic = new Dictionary<string, object>();
        paramDic.Add("@absenceId", AbsenceId);
        paramDic.Add("@fromDate", from);
        paramDic.Add("@untilDate", until);
        paramDic.Add("@cause", cause);
        paramDic.Add("@note", note);
        paramDic.Add("@coorId", coorId);
        cmd = CreateCommandWithStoredProcedureGeneral("updateAbsenceById", con, paramDic);
        int numEffected = 0;
        try
        {
             numEffected = cmd.ExecuteNonQuery();
            return numEffected;
        }
        
        catch (Exception ex)
        {

            throw new Exception("exception in DBservice_Gilad.cs updateAbsenceById sp -->" + ex);
        }
    }

    public int DeleteAbsenceById(int AbsenceId)
    {
        SqlCommand cmd;
        try
        {
            con = new SqlConnection(ConfigurationManager.ConnectionStrings["db"].ConnectionString);
            con.Open();
        }

        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }

        Dictionary<string, object> paramDic = new Dictionary<string, object>();
        paramDic.Add("@absenceId", AbsenceId);

        cmd = CreateCommandWithStoredProcedureGeneral("DeleteAbsenceById", con, paramDic);

        int numEffected = 0;
        try
        {
            numEffected = cmd.ExecuteNonQuery();
            return numEffected;
        }

        catch (Exception ex)
        {

            throw new Exception("exception in DBservice_Gilad.cs DeleteAbsenceById sp -->" + ex);
        }
    }

    public int InsertNewAbsence(int volunteerId, int coorId, DateTime from, DateTime until, string cause, string note)
    {
        SqlCommand cmd;
        try
        {
            con = new SqlConnection(ConfigurationManager.ConnectionStrings["db"].ConnectionString);
            con.Open();
        }

        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }

        Dictionary<string, object> paramDic = new Dictionary<string, object>();
        paramDic.Add("@volunteerId", volunteerId);
        paramDic.Add("@FromDate", from);
        paramDic.Add("@UntilDate", until);
        paramDic.Add("@Cause", cause);
        paramDic.Add("@Note", note);
        paramDic.Add("@CoordinatorId", coorId);
        cmd = CreateCommandWithStoredProcedureGeneral("InsertToAbsence", con, paramDic);
        int numEffected = 0;
        try
        {
            numEffected = cmd.ExecuteNonQuery();
            return numEffected;
        }

        catch (Exception ex)
        {

            throw new Exception("exception in DBservice_Gilad.cs InsertToAbsence sp -->" + ex);
        }
    }


    public List <Volunteer> getVolunteersList_V2_WebOnly_Gilad(bool active)
    {


        SqlCommand cmd;
        try
        {
            con = new SqlConnection(ConfigurationManager.ConnectionStrings["db"].ConnectionString);
            con.Open();
        }

        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }

        Dictionary<string, object> paramDic = new Dictionary<string, object>();
        paramDic.Add("@isActive", active);
        cmd = CreateCommandWithStoredProcedureGeneral("spVolunteerTypeView_GetVolunteersList_Gilad", con, paramDic);
        List<Volunteer> VolunteerList = new List<Volunteer>();
        try
        {
            City city = new City();
            Dictionary<string, string> nearByCities = city.getNearbyCities();

            SqlDataReader dataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            while (dataReader.Read())
            {
                Volunteer v = new Volunteer();
                v.Id = int.Parse(dataReader["Id"].ToString());
                v.DisplayName = dataReader["DisplayName"].ToString();
                v.FirstNameA = dataReader["FirstNameA"].ToString();
                v.FirstNameH = dataReader["FirstNameH"].ToString();
                v.LastNameH = dataReader["LastNameH"].ToString();
                v.LastNameA = dataReader["LastNameA"].ToString();
                v.CellPhone = dataReader["CellPhone"].ToString();
                v.CellPhone2 = dataReader["CellPhone2"].ToString();
                v.HomePhone = dataReader["HomePhone"].ToString();
                v.Remarks = dataReader["Remarks"].ToString();
                v.City = dataReader["CityCityName"].ToString();
                v.Address = dataReader["Address"].ToString();
                v.TypeVol = dataReader["VolunTypeType"].ToString();
                v.Email = dataReader["Email"].ToString();
                v.Device = dataReader["device"].ToString();
                v.NoOfDocumentedCalls = Convert.ToInt32(dataReader["NoOfDocumentedCalls"]);
                v.NoOfDocumentedRides = Convert.ToInt32(dataReader["NoOfDocumentedRides"]);
                v.NumOfRides_last2Months = Convert.ToInt32(dataReader["NumOfRides_last2Months"]);
                v.MostCommonPath = dataReader["mostCommonPath"].ToString();

                DateTime latestDrive;
                DateTime.TryParse(dataReader["latestDrive"].ToString(), out latestDrive);
                if (latestDrive == DateTime.MinValue)
                {
                    v.LatestDrive = null;
                }
                else
                {
                    v.LatestDrive = latestDrive;
                }
                if (dataReader["AbsenceStatus"].ToString() == "")
                {
                    v.AbsenceStatus = false;
                }
                else
                {
                    v.AbsenceStatus = Convert.ToBoolean(dataReader["AbsenceStatus"].ToString());
                }

                if (nearByCities.Keys.Contains(v.City))
                {
                    v.NearestBigCity = nearByCities[v.City];
                }


                string date = dataReader["JoinDate"].ToString();
                bool isAssistant = Convert.ToBoolean(dataReader["isAssistant"].ToString());
                if (date == "")
                {

                }
                else v.JoinDate = Convert.ToDateTime(dataReader["JoinDate"].ToString());
                bool ac = false;
                if (dataReader["IsActive"].ToString().ToLower() == "true")
                {
                    ac = true;
                }
                v.IsActive = ac;
                bool arabic = false;
                if (dataReader["KnowsArabic"].ToString().ToLower() == "true")
                {
                    arabic = true;
                }
                v.KnowsArabic = arabic;
                v.Gender = dataReader["Gender"].ToString();
                v.RegId = dataReader["pnRegId"].ToString();

                v.EnglishName = dataReader["englishName"].ToString();
                DateTime lastmodified;
                DateTime.TryParse(dataReader["lastModified"].ToString(), out lastmodified);
                v.DateTime_LastModified = lastmodified;

                if (dataReader["isDriving"].ToString() != "")
                {
                    v.IsDriving = Convert.ToBoolean(dataReader["isDriving"].ToString());
                }
                VolunteerList.Add(v);
            }
            return VolunteerList;
        }
        catch (Exception ex )
        {

            throw new Exception("exception in DBservice_Gilad.cs spVolunteerTypeView_GetVolunteersList_Gilad sp -->" + ex);
        }
        finally 
        {

            if (con != null)
            {
                // close the db connection
                con.Close();
            }

        }

    }


    // ---- Create Command with SP ---- \\ 
    private SqlCommand CreateCommandWithStoredProcedureGeneral(String spName, SqlConnection con, Dictionary<string, object> paramDic)
	{

		SqlCommand cmd = new SqlCommand(); // create the command object

		cmd.Connection = con;              // assign the connection to the command object

		cmd.CommandText = spName;      // can be Select, Insert, Update, Delete 

		cmd.CommandTimeout = 30;           // Time to wait for the execution' The default is 30 seconds

		cmd.CommandType = System.Data.CommandType.StoredProcedure; // the type of the command, can also be text

		if (paramDic != null)
			foreach (KeyValuePair<string, object> param in paramDic)
			{
				cmd.Parameters.AddWithValue(param.Key, param.Value);

			}


		return cmd;
	}
}