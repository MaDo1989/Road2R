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
using System.Data.Common;
using System.Activities.Statements;
using System.Dynamic;
//using static System.Net.Mime.MediaTypeNames;
using System.IO;
using System.Web;
//using static Constants.Enums;
//using static ReportService;

public class DBservice_Gilad
{
	public SqlConnection con;

    public List<string> GetListOfEquipmentsForPAtient(int patientID)
    {
        List<string> list2Return = new List<string>();
        SqlCommand cmd2;
        Dictionary<string, object> paramDic2 = new Dictionary<string, object>();
        paramDic2.Add("@patientId", patientID);
        SqlConnection con2;
        try
        {
            con2 = new SqlConnection(ConfigurationManager.ConnectionStrings["db"].ConnectionString);
            con2.Open();
        }

        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }


        cmd2 = CreateCommandWithStoredProcedureGeneral("spGetEquipmentsForSpesificPatient", con2, paramDic2);
        try
        {
            SqlDataReader dataReader2 = cmd2.ExecuteReader(CommandBehavior.CloseConnection);
            while (dataReader2.Read())
            {
                string oneEquipment;
                oneEquipment = dataReader2["EquipmentName"].ToString();
                list2Return.Add(oneEquipment);
            }
            return list2Return;

        }
        catch (Exception ex)
        {

            throw (ex);
        }
        finally
        {
            if (con2 != null)
            {
                con2.Close();
            }

        }
        

    }

    public List<object> GetListOfEscortsByUnityRideId(int unityRideId)
    {
        List<object> list2Return = new List<object>();
        SqlCommand cmd2;
        Dictionary<string, object> paramDic2 = new Dictionary<string, object>();
        paramDic2.Add("@UnityRideId", unityRideId);
        SqlConnection con2;
        try
        {
            con2 = new SqlConnection(ConfigurationManager.ConnectionStrings["db"].ConnectionString);
            con2.Open();
        }

        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }


        cmd2 = CreateCommandWithStoredProcedureGeneral("spGetEscortByUnityRideId", con2, paramDic2);
        try
        {
            SqlDataReader dataReader2 = cmd2.ExecuteReader(CommandBehavior.CloseConnection);
            while (dataReader2.Read())
            {
                dynamic oneEscort = new ExpandoObject();
                oneEscort.Id = Convert.ToInt32(dataReader2["Id"]);
                oneEscort.DisplayName = dataReader2["DisplayName"].ToString();
                list2Return.Add(oneEscort);
            }
            return list2Return;

        }
        catch (Exception ex)
        {

            throw (ex);
        }
        finally
        {
            if (con2 != null)
            {
                con2.Close();
            }

        }


    }

    static public void WriteToErrorFile(string ancestors,string ExceptionString)
    {
        string Path = "\\log\\Errors.txt";
        string stackTracesfilePath = HttpContext.Current.Server.MapPath("~") + Path;
        DateTime currentDate = DateTime.Now;
        string formattedDate = currentDate.ToString("dd/MM/yyyy HH:mm:ss");
        string linebreak = "\n";
        ancestors = ancestors.Replace(" ", " -> ");
        string sep = "******************************";
        string txtToFile = sep + formattedDate + sep + linebreak;
        txtToFile += "FROM : " + linebreak + ancestors + linebreak;
        txtToFile += "The Error msg : " + linebreak + ExceptionString + linebreak + "END";






        try
        {
            using (StreamWriter writer = new StreamWriter(stackTracesfilePath,true))
            {
                // Write the text to the file
                writer.WriteLine(txtToFile);
            }
        }
        catch(Exception ex) { 
            throw new Exception("error in write to txt file "+ex.Message);
        }
    }
    public List<object> GetListOfEscortsByPatientId(int patientId)
    {
        List<object> list2Return = new List<object>();
        SqlCommand cmd2;
        Dictionary<string, object> paramDic2 = new Dictionary<string, object>();
        paramDic2.Add("@patientId", patientId);
        SqlConnection con2;
        try
        {
            con2 = new SqlConnection(ConfigurationManager.ConnectionStrings["db"].ConnectionString);
            con2.Open();
        }

        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }


        cmd2 = CreateCommandWithStoredProcedureGeneral("spGetEscortsByPatientId", con2, paramDic2);
        try
        {
            SqlDataReader dataReader2 = cmd2.ExecuteReader(CommandBehavior.CloseConnection);
            while (dataReader2.Read())
            {
                dynamic oneEscort = new ExpandoObject();
                oneEscort.Id = Convert.ToInt32(dataReader2["Id"]);
                oneEscort.DisplayName = dataReader2["DisplayName"].ToString();
                list2Return.Add(oneEscort);
            }
            return list2Return;

        }
        catch (Exception ex)
        {

            throw (ex);
        }
        finally
        {
            if (con2 != null)
            {
                con2.Close();
            }

        }


    }

    public List<UnityRide> GetRidesForRidePatView(int days)
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
        paramDic.Add("@days", days);

        cmd = CreateCommandWithStoredProcedureGeneral("spGetUnitedRides", con, paramDic);

        List <UnityRide> list2Return = new List<UnityRide>();
        try
        {


            SqlDataReader dataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            DateTime? nullableDateTime = null;

            while (dataReader.Read())
            {
                UnityRide oneRide = new UnityRide();
                oneRide.RidePatNum = Convert.ToInt32(dataReader["RidePatNum"]);
                oneRide.PatientName = dataReader["PatientName"].ToString();
                oneRide.PatientId = Convert.ToInt32(dataReader["PatientId"]);
                oneRide.PatientGender =Convert.ToInt32(Convertions.ConvertStringToGender(dataReader["PatientGender"].ToString()));
                oneRide.PatientCellPhone = dataReader["PatientCellPhone"].ToString();
                oneRide.PatientStatus = dataReader["PatientStatus"].ToString();
                if (oneRide.PatientStatus!="")
                {
                    oneRide.PatientStatusEditTime = Convert.ToDateTime(dataReader["patientStatusTime"]);

                }

                //another option*
                //if(dataReader.IsDBNull(dataReader.GetOrdinal("patientStatusTime"))){

                //}
                //else
                //{
                //    oneRide.PatientStatusEditTime = Convert.ToDateTime(dataReader["patientStatusTime"]);

                //}

                oneRide.PatientBirthdate = dataReader["PatientBirthDate"].ToString();
                DateTime? dateOfBirth = String.IsNullOrEmpty(dataReader["PatientBirthDate"].ToString()) ? null : (DateTime?)Convert.ToDateTime(dataReader["PatientBirthDate"].ToString());
                oneRide.PatientAge = Convert.ToInt32(Calculations.CalculateAge(dateOfBirth));

                oneRide.AmountOfEquipments = Convert.ToInt32(dataReader["AmountOfEquipments"]);
                if (oneRide.AmountOfEquipments > 0)
                {
                    oneRide.PatientEquipments = GetListOfEquipmentsForPAtient(oneRide.PatientId);
                }
                oneRide.AmountOfEscorts = Convert.ToInt32(dataReader["AmountOfEscorts"]);
                oneRide.Origin = dataReader["Origin"].ToString();
                oneRide.Destination = dataReader["Destination"].ToString();
                oneRide.PickupTime = Convert.ToDateTime(dataReader["pickupTime"]);
                oneRide.CoorName = dataReader["Coordinator"].ToString();
                oneRide.Remark = dataReader["Remark"].ToString();
                oneRide.Status = dataReader["Status"].ToString();
                oneRide.Area = dataReader["Area"].ToString();
                oneRide.Shift = dataReader["Shift"].ToString();
                oneRide.OnlyEscort = Convert.ToBoolean(dataReader["OnlyEscort"]);
                oneRide.LastModified = Convert.ToDateTime(dataReader["lastModified"]);
                oneRide.CoorId = Convert.ToInt32(dataReader["CoordinatorID"]);

                if (dataReader.IsDBNull(dataReader.GetOrdinal("MainDriver")))
                {
                    oneRide.MainDriver = -1;
                }
                else
                {
                    oneRide.MainDriver = Convert.ToInt32(dataReader["MainDriver"]);

                }
                oneRide.DriverName = dataReader["DriverName"].ToString();
                oneRide.DriverCellPhone = dataReader["DriverCellPhone"].ToString();
                if (dataReader.IsDBNull(dataReader.GetOrdinal("NoOfDocumentedRides")))
                {
                    oneRide.NoOfDocumentedRides = 0;
                }
                else
                {
                    oneRide.NoOfDocumentedRides = Convert.ToInt32(dataReader["NoOfDocumentedRides"]);

                }
                if (dataReader.IsDBNull(dataReader.GetOrdinal("IsAnonymous")))
                {
                    oneRide.IsAnonymous = false;
                }
                else
                {
                    oneRide.IsAnonymous = Convert.ToBoolean(dataReader["IsAnonymous"]);

                }
                oneRide.IsNewDriver = Convert.ToBoolean(dataReader["IsNewDriver"]);
                list2Return.Add(oneRide);

            }

            return list2Return;
        }
        catch (Exception ex)
        {
            WriteToErrorFile("GetUnityRide GetUnityRideView GetRidesForRidePatView spGetUnitedRides", ex.Message + ex.ToString());
            throw (new Exception(ex.Message));
        }

        finally
        {
            if (con != null)
            {
                con.Close();
            }
        }


    }


    public List<UnityRide> Get_unityRide_ByTimeRange(int from, int until, bool isDeletedtoShow)
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
        paramDic.Add("@from", from);
        paramDic.Add("@to", until);
        paramDic.Add("@isDeletedtoShow", isDeletedtoShow);
        cmd = CreateCommandWithStoredProcedureGeneral("spGet_UnityRide_ByTimeRange", con, paramDic);
        List<UnityRide> list2Return = new List<UnityRide>();

        try
        {


            SqlDataReader dataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            while (dataReader.Read())
            {
                UnityRide oneRide = new UnityRide();
                oneRide.RidePatNum = Convert.ToInt32(dataReader["RidePatNum"]);
                oneRide.PatientName = dataReader["PatientName"].ToString();
                oneRide.PatientId = Convert.ToInt32(dataReader["PatientId"]);
                //oneRide.PatientGender = Convert.ToInt32(Convertions.ConvertStringToGender(dataReader["PatientGender"].ToString()));
                //oneRide.PatientCellPhone = dataReader["PatientCellPhone"].ToString();
                //oneRide.PatientStatus = dataReader["PatientStatus"].ToString();
                //if (oneRide.PatientStatus != "")
                //{
                //    oneRide.PatientStatusEditTime = Convert.ToDateTime(dataReader["patientStatusTime"]);

                //}

                //oneRide.PatientBirthdate = dataReader["PatientBirthDate"].ToString();
                //DateTime? dateOfBirth = String.IsNullOrEmpty(dataReader["PatientBirthDate"].ToString()) ? null : (DateTime?)Convert.ToDateTime(dataReader["PatientBirthDate"].ToString());
                //oneRide.PatientAge = Convert.ToInt32(Calculations.CalculateAge(dateOfBirth));

                //oneRide.AmountOfEquipments = Convert.ToInt32(dataReader["AmountOfEquipments"]);
                //if (oneRide.AmountOfEquipments > 0)
                //{
                //    oneRide.PatientEquipments = GetListOfEquipmentsForPAtient(oneRide.PatientId);
                //}
                //oneRide.AmountOfEscorts = Convert.ToInt32(dataReader["AmountOfEscorts"]);
                oneRide.Origin = dataReader["Origin"].ToString();
                oneRide.Destination = dataReader["Destination"].ToString();
                oneRide.PickupTime = Convert.ToDateTime(dataReader["pickupTime"]);
                oneRide.CoorName = dataReader["Coordinator"].ToString();
                oneRide.Remark = dataReader["Remark"].ToString();
                oneRide.Status = dataReader["Status"].ToString();
                oneRide.Area = dataReader["Area"].ToString();
                oneRide.Shift = dataReader["Shift"].ToString();
                oneRide.OnlyEscort = Convert.ToBoolean(dataReader["OnlyEscort"]);
                oneRide.LastModified = Convert.ToDateTime(dataReader["lastModified"]);
                oneRide.CoorId = Convert.ToInt32(dataReader["CoordinatorID"]);

                if (dataReader.IsDBNull(dataReader.GetOrdinal("MainDriver")))
                {
                    oneRide.MainDriver = -1;
                }
                else
                {
                    oneRide.MainDriver = Convert.ToInt32(dataReader["MainDriver"]);

                }
                oneRide.DriverName = dataReader["DriverName"].ToString();
                oneRide.DriverCellPhone = dataReader["DriverCellPhone"].ToString();
                //if (dataReader.IsDBNull(dataReader.GetOrdinal("NoOfDocumentedRides")))
                //{
                //    oneRide.NoOfDocumentedRides = 0;
                //}
                //else
                //{
                //    oneRide.NoOfDocumentedRides = Convert.ToInt32(dataReader["NoOfDocumentedRides"]);

                //}
                if (dataReader.IsDBNull(dataReader.GetOrdinal("IsAnonymous")))
                {
                    oneRide.IsAnonymous = false;
                }
                else
                {
                    oneRide.IsAnonymous = Convert.ToBoolean(dataReader["IsAnonymous"]);

                }
                oneRide.IsNewDriver = Convert.ToBoolean(dataReader["IsNewDriver"]);
                list2Return.Add(oneRide);

            }

            return list2Return;
        }
        catch (Exception ex)
        {
            WriteToErrorFile("Get_unityRide_ByTimeRange Get_unityRide_ByTimeRange spGet_UnityRide_ByTimeRange", ex.Message +ex.ToString());
            throw new Exception("exception in DBservice_Gilad.cs ?  Get_unityRide_ByTimeRange" + ex);
        }

        finally
        {
            if (con != null)
            {
                con.Close();
            }
        }

    }


    public List <object> GetTomorrowRides()
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
        cmd = CreateCommandWithStoredProcedureGeneral("GetTomorrowRides_Gilad", con, null);
        List <object> list = new List<object>();

        try
        {


            SqlDataReader dataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

            while (dataReader.Read())
            {
                var rideFromRPview = new {
                    Id = Convert.ToInt32(dataReader["Id"]),
                    pickupTime = Convert.ToDateTime(dataReader["pickupTime"]),
                    pickupHour = dataReader["pickupHour"].ToString(),
                    OriginE = dataReader["OriginE"].ToString(),
                    DestinationE = dataReader["DestinationE"].ToString(),
                    numOfPass = Convert.ToInt32(dataReader["numOfPass"]),
                    patientEName = dataReader["EnglishName"].ToString(),
                    MainDriver = Convert.ToInt32(dataReader["MainDriver"]),

                };
                list.Add(rideFromRPview);
            }

            return list;
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

    public List <object> GetUnityRide(int UnityRideId)
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
        paramDic.Add("@UnityRideId", UnityRideId);
        cmd = CreateCommandWithStoredProcedureGeneral("GetUnityRide", con, paramDic);
        List<object> list = new List<object>();

        try
        {


            SqlDataReader dataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

            while (dataReader.Read())
            {
                dynamic OneRideTry = new ExpandoObject();


                OneRideTry.RidePatNum = Convert.ToInt32(dataReader["RidePatNum"]);
                OneRideTry.PatientName = dataReader["PatientName"].ToString();
                OneRideTry.PatientId = Convert.ToInt32(dataReader["PatientId"]);
                OneRideTry.AmountOfEscorts = Convert.ToInt32(dataReader["AmountOfEscorts"]);
                OneRideTry.AmountOfEquipments = Convert.ToInt32(dataReader["AmountOfEquipments"]);
                OneRideTry.Origin = dataReader["Origin"].ToString();
                OneRideTry.Destination = dataReader["Destination"].ToString();
                OneRideTry.PickupTime = Convert.ToDateTime(dataReader["PickupTime"].ToString());
                OneRideTry.Remark = dataReader["Remark"].ToString();
                OneRideTry.Status = dataReader["Status"].ToString();
                OneRideTry.OnlyEscort = Convert.ToBoolean(dataReader["OnlyEscort"]);
                OneRideTry.lastModified = Convert.ToDateTime(dataReader["lastModified"].ToString());
                OneRideTry.MainDriver = dataReader["MainDriver"] != DBNull.Value ? Convert.ToInt32(dataReader["MainDriver"]) : (int?)null;
                OneRideTry.DriverName = dataReader["DriverName"].ToString();
                OneRideTry.PatientEnglishName = dataReader["PatientEnglishName"].ToString();
                OneRideTry.PatientIdentity = dataReader["PatientIdentity"] != DBNull.Value ? Convert.ToInt32(dataReader["PatientIdentity"]) : (int?)null;
                OneRideTry.DisplayNameArabic = dataReader["DisplayNameArabic"].ToString();
                OneRideTry.Equipments = Convert.ToInt32(dataReader["AmountOfEquipments"]) > 0 ? GetListOfEquipmentsForPAtient(Convert.ToInt32(dataReader["PatientId"])) : new List<string>();
                OneRideTry.Escorts = OneRideTry.AmountOfEscorts>0? GetListOfEscortsByUnityRideId(OneRideTry.RidePatNum) : new List<string>();
                OneRideTry.IsAnonymous = dataReader["IsAnonymous"]!= DBNull.Value ? Convert.ToBoolean(dataReader["IsAnonymous"]) : false;
                OneRideTry.EscortList = GetListOfEscortsByPatientId(OneRideTry.PatientId);
                OneRideTry.CoorName = dataReader["Coordinator"].ToString();
                OneRideTry.CoorId = Convert.ToInt32(dataReader["CoordinatorID"]);
                list.Add(OneRideTry);
            }

            return list;
        }
        catch (Exception ex)
        {
            // write to log
            WriteToErrorFile("GetUnityRideToEdit GetUnityRideToEdit GetUnityRide", ex.Message + ex.ToString());
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

    public bool CheckValidDriverRides(int unityRideID,string DriverName,DateTime pickupTime) {
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
        paramDic.Add("@UnityRideID", unityRideID);
        paramDic.Add("@DriverName", DriverName);
        paramDic.Add("@pickupTime", pickupTime);
        cmd = CreateCommandWithStoredProcedureGeneral("spCheckValidDrive", con, paramDic);


        try
        {


            SqlDataReader dataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            int res = -1;
            while (dataReader.Read())
            {
                


                res = Convert.ToInt32(dataReader["res"]);

            }
            if (res>0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        catch (Exception ex)
        {
            // write to log
            WriteToErrorFile("setUnityRide SetUnityRide CheckValidDriverRides spCheckValidDrive", ex.Message + ex.ToString());
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


    public int recoverUnityRide(int unityRideID)
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
        paramDic.Add("@unityRideID", unityRideID);

        cmd = CreateCommandWithStoredProcedureGeneral("spRecoverUnityRide", con, paramDic);
        int res = -1;

        try
        {


            res = cmd.ExecuteNonQuery();
            return res;
         
        }
        catch (Exception ex)
        {
            // write to log
            WriteToErrorFile("recoverUnityRides recoverUnityRide spRecoverUnityRide",ex.Message + ex.ToString());
            throw new Exception("exception in DBservice_Gilad.cs -> recoverUnityRide " + ex);
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


    public int SetUnityRide(UnityRide unityRide)
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
        unityRide.Area = unityRide.NEWCheckLocationForUnityRideArea(unityRide.Origin, unityRide.Destination);
        Dictionary<string, object> paramDic = new Dictionary<string, object>();
        paramDic.Add("@patientName", unityRide.PatientName);
        paramDic.Add("@patientId", unityRide.PatientId);
        paramDic.Add("@origin", unityRide.Origin);
        paramDic.Add("@destination", unityRide.Destination);
        paramDic.Add("@pickupTime", unityRide.PickupTime);
        paramDic.Add("@remark", unityRide.Remark);
        paramDic.Add("@onlyEscort", unityRide.OnlyEscort);
        paramDic.Add("@area", unityRide.Area);
        paramDic.Add("@isAnonymous", unityRide.IsAnonymous);
        paramDic.Add("@coorName", unityRide.CoorName);
        paramDic.Add("@driverName", unityRide.DriverName==null?DBNull.Value.ToString(): unityRide.DriverName);
        paramDic.Add("@amountOfEscorts", unityRide.AmountOfEscorts);
        cmd = CreateCommandWithStoredProcedureGeneral("spSetNewUnityRide", con, paramDic);
        int res = 0;
        try
        {
            SqlDataReader dataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            while (dataReader.Read())
            {
                res = Convert.ToInt32(dataReader["RidePatNum"]);
            }
            return res;
        }
        catch (Exception ex)
        {
            WriteToErrorFile("setUnityRide SetUnityRide spSetNewUnityRide", ex.Message + ex.ToString());
            throw ex;
        }
        finally
        {
            if (con != null)
            {
                con.Close();
            }
        }


    }
    public int UpdateUnityRide(UnityRide unityRide)
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
        unityRide.Area = unityRide.NEWCheckLocationForUnityRideArea(unityRide.Origin, unityRide.Destination);
        Dictionary<string, object> paramDic = new Dictionary<string, object>();
        paramDic.Add("@patientName", unityRide.PatientName);
        paramDic.Add("@patientId", unityRide.PatientId);
        paramDic.Add("@origin", unityRide.Origin);
        paramDic.Add("@destination", unityRide.Destination);
        paramDic.Add("@pickupTime", unityRide.PickupTime);
        paramDic.Add("@remark", unityRide.Remark);
        paramDic.Add("@onlyEscort", unityRide.OnlyEscort);
        paramDic.Add("@area", unityRide.Area);
        paramDic.Add("@isAnonymous", unityRide.IsAnonymous);
        paramDic.Add("@coorName", unityRide.CoorName);
        paramDic.Add("@driverName", unityRide.DriverName == null ? DBNull.Value.ToString() : unityRide.DriverName);
        paramDic.Add("@amountOfEscorts", unityRide.AmountOfEscorts);
        paramDic.Add("@unityRideId", unityRide.RidePatNum);

        cmd = CreateCommandWithStoredProcedureGeneral("spUpdateRideInUnityRide", con, paramDic);
        int numEffected = 0;
        try
        {
            numEffected = cmd.ExecuteNonQuery();
            return numEffected;
        }
        catch (Exception ex)
        {
            WriteToErrorFile("setUnityRide SetUnityRide UpdateUnityRide spUpdateRideInUnityRide", ex.Message + ex.ToString());
            throw ex;
        }
        finally
        {
            if (con != null)
            {
                con.Close();
            }
        }


    }
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

    public List<RidePat> GetRidePatViewByTimeFilter_Gilad_DR(int from, int until,bool isDeletedtoShow)
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
        paramDic.Add("@isDeletedtoShow", isDeletedtoShow);
        cmd = CreateCommandWithStoredProcedureGeneral("spGet_rpview_ByTimeRange_Gilad", con, paramDic);

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
                    primary.DisplayName = dataReader["DriverName"].ToString();
                    primary.CellPhone = dataReader["DriverPhone"].ToString();

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

    public List<UnityRide> GetRidesByVolunteer(int volunteerID)
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

        string ancestors = "GetVolunteersDocumentedUnityRides GetUnityRidesByVolunteerId GetRidesByVolunteer spUnityRide_GetVolunteersRideHistory";
        Dictionary<string, object> paramDic = new Dictionary<string, object>();
        paramDic.Add("@volunteerId", volunteerID);
        cmd = CreateCommandWithStoredProcedureGeneral("spUnityRide_GetVolunteersRideHistory", con, paramDic);
        List<UnityRide> list2Return = new List<UnityRide>();
        try
        {
            SqlDataReader dataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            while (dataReader.Read())
            {
                UnityRide unityRide = new UnityRide();
                unityRide.RidePatNum = Convert.ToInt32(dataReader["RidePatNum"]);
                unityRide.PatientName = dataReader["PatientName"].ToString();
                unityRide.Remark = dataReader["Remark"].ToString();
                unityRide.Destination = dataReader["Destination"].ToString();
                unityRide.Origin = dataReader["Origin"].ToString();
                unityRide.PickupTime = Convert.ToDateTime(dataReader["pickupTime"]);
                list2Return.Add(unityRide);
            }
            return list2Return;
        }
        catch (Exception ex)
        {
            WriteToErrorFile(ancestors, "Error to get rides by volunteerId \n" + ex.Message + ex.ToString());
            throw new Exception("Error to get GetRidesByVolunteer dbservice_gilad" + ex.Message);
        }
        finally { 
            if (con != null) {

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
        SqlDataReader dataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
        int ScopeId = -1;

        try
        {
            while (dataReader.Read())
            {
                ScopeId = Convert.ToInt32(dataReader["AbsenceId"]);
            }
            return ScopeId;
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
                //v.FirstNameA = dataReader["FirstNameA"].ToString();
                v.FirstNameH = dataReader["FirstNameH"].ToString();
                v.LastNameH = dataReader["LastNameH"].ToString();
                v.EnglishName = dataReader["EnglishName"].ToString();
                //v.LastNameA = dataReader["LastNameA"].ToString();
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




    public UnityRide updateUnityRideTime(int unityRideNum, DateTime editedTime)
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
        paramDic.Add("@editedTime", editedTime);
        paramDic.Add("@unityRideId", unityRideNum);
        cmd = CreateCommandWithStoredProcedureGeneral("spUnityRide_UpdateDateAndTime", con, paramDic);
        UnityRide unityRide = new UnityRide();
        string ancestors = "UpdateUnityRideTime updateUnityRideTime spUnityRide_UpdateDateAndTime reciveUnityRideDB";
        unityRide = reciveUnityRideDB(cmd, ancestors);
        return unityRide;


    }

    public UnityRide updateRemark(int UnityRideID, string newRemark)
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
        paramDic.Add("@ridePatNum", UnityRideID);
        paramDic.Add("@newRemark", newRemark);
        cmd = CreateCommandWithStoredProcedureGeneral("spUnityRide_updateRemark", con, paramDic);
        UnityRide unityRide = new UnityRide();

        string ancestors = "UpdateUnityRideRemark updateRemark spUnityRide_updateRemark reciveUnityRideDB";
        unityRide = reciveUnityRideDB(cmd, ancestors);
        return unityRide;

    }

    public UnityRide updatePatientStatusAndTime(int patientId, int unityRideID, string patientStatus, DateTime? editTimeStamp)
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
        paramDic.Add("@PatientId", patientId);
        paramDic.Add("@RidePatNum", unityRideID);
        paramDic.Add("@PatientStatus", patientStatus);
        paramDic.Add("@EditTimeStamp", editTimeStamp);
        cmd = CreateCommandWithStoredProcedureGeneral("spUpdatePatientStatusUnityRide", con, paramDic);
        UnityRide unityRide = new UnityRide();
        string ancestors = "UpdatePatientStatus_UnityRide updatePatientStatusandTime updatePatientStatusAndTime spUpdatePatientStatusUnityRide reciveUnityRideDB";
        unityRide = reciveUnityRideDB(cmd, ancestors);
        return unityRide;

    }

    public UnityRide updateDriver(int driverId,int unityRideID)
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
        paramDic.Add("@driverID", driverId);
        paramDic.Add("@unityRideID", unityRideID);
        cmd = CreateCommandWithStoredProcedureGeneral("spUpdateDriverUnityRide", con, paramDic);
        UnityRide unityRide = new UnityRide();
        string ancestors = "AssignUpdateDriverToUnityRide updateDriver spUpdateDriverUnityRide reciveUnityRideDB";
        unityRide = reciveUnityRideDB(cmd, ancestors);
        return unityRide;
    }

    public UnityRide deleteUnityRide(int unityRideID)
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
        paramDic.Add("@unityRideID", unityRideID);
        cmd = CreateCommandWithStoredProcedureGeneral("spDeleteUnityRide", con, paramDic);
        UnityRide unityRide = new UnityRide();
        string ancestors = "deleteUnityRide deleteUnityRide spDeleteUnityRide reciveUnityRideDB";
        unityRide = reciveUnityRideDB(cmd, ancestors);
        return unityRide;
    }

    private UnityRide reciveUnityRideDB(SqlCommand cmd,string ancestors)
    {
        UnityRide unityRide = new UnityRide();
        try
        {
            SqlDataReader dataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            while (dataReader.Read())
            {
                int rideId = Convert.ToInt32(dataReader["RidePatNum"]);
                if (rideId > -1)
                {
                    unityRide.RidePatNum = rideId;
                    unityRide.PatientName = dataReader["PatientName"].ToString();
                    unityRide.PatientId = Convert.ToInt32(dataReader["PatientId"]);
                    unityRide.PatientGender = Convert.ToInt32(Convertions.ConvertStringToGender(dataReader["PatientGender"].ToString()));
                    unityRide.PatientCellPhone = dataReader["PatientCellPhone"].ToString();
                    unityRide.PatientStatus = dataReader["PatientStatus"].ToString();
                    if (unityRide.PatientStatus != "")
                    {
                        unityRide.PatientStatusEditTime = Convert.ToDateTime(dataReader["patientStatusTime"]);

                    }

                    unityRide.PatientBirthdate = dataReader["PatientBirthDate"].ToString();
                    DateTime? dateOfBirth = String.IsNullOrEmpty(dataReader["PatientBirthDate"].ToString()) ? null : (DateTime?)Convert.ToDateTime(dataReader["PatientBirthDate"].ToString());
                    unityRide.PatientAge = Convert.ToInt32(Calculations.CalculateAge(dateOfBirth));

                    unityRide.AmountOfEquipments = Convert.ToInt32(dataReader["AmountOfEquipments"]);
                    if (unityRide.AmountOfEquipments > 0)
                    {
                        unityRide.PatientEquipments = GetListOfEquipmentsForPAtient(unityRide.PatientId);
                    }
                    unityRide.AmountOfEscorts = Convert.ToInt32(dataReader["AmountOfEscorts"]);
                    unityRide.Origin = dataReader["Origin"].ToString();
                    unityRide.Destination = dataReader["Destination"].ToString();
                    unityRide.PickupTime = Convert.ToDateTime(dataReader["pickupTime"]);
                    unityRide.CoorName = dataReader["Coordinator"].ToString();
                    unityRide.Remark = dataReader["Remark"].ToString();
                    unityRide.Status = dataReader["Status"].ToString();
                    unityRide.Area = dataReader["Area"].ToString();
                    unityRide.Shift = dataReader["Shift"].ToString();
                    unityRide.OnlyEscort = Convert.ToBoolean(dataReader["OnlyEscort"]);
                    unityRide.LastModified = Convert.ToDateTime(dataReader["lastModified"]);
                    unityRide.CoorId = Convert.ToInt32(dataReader["CoordinatorID"]);

                    if (dataReader.IsDBNull(dataReader.GetOrdinal("MainDriver")))
                    {
                        unityRide.MainDriver = -1;
                    }
                    else
                    {
                        unityRide.MainDriver = Convert.ToInt32(dataReader["MainDriver"]);

                    }
                    unityRide.DriverName = dataReader["DriverName"].ToString();
                    unityRide.DriverCellPhone = dataReader["DriverCellPhone"].ToString();
                    if (dataReader.IsDBNull(dataReader.GetOrdinal("NoOfDocumentedRides")))
                    {
                        unityRide.NoOfDocumentedRides = 0;
                    }
                    else
                    {
                        unityRide.NoOfDocumentedRides = Convert.ToInt32(dataReader["NoOfDocumentedRides"]);

                    }
                    if (dataReader.IsDBNull(dataReader.GetOrdinal("IsAnonymous")))
                    {
                        unityRide.IsAnonymous = false;
                    }
                    else
                    {
                        unityRide.IsAnonymous = Convert.ToBoolean(dataReader["IsAnonymous"]);

                    }
                    unityRide.IsNewDriver = Convert.ToBoolean(dataReader["IsNewDriver"]);
                }

            }
            return unityRide;
        }
        catch (Exception ex)
        {
            WriteToErrorFile(ancestors,ex.Message+ex.ToString());
            throw new Exception("error in dbService_Gilad.cs in reciveUnityRideDB function -->" + ex.Message);

        }
        finally
        {
            if (con != null)
            {
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

        if (spName== "spGet_rpview_ByTimeRange_Gilad")
        {
            cmd.CommandTimeout = 85;  // spesific this sp taking almost 40 sec. that way change the timeout.
        }

        cmd.CommandType = System.Data.CommandType.StoredProcedure; // the type of the command, can also be text

		if (paramDic != null)
			foreach (KeyValuePair<string, object> param in paramDic)
			{
				cmd.Parameters.AddWithValue(param.Key, param.Value);

			}


		return cmd;
	}
}