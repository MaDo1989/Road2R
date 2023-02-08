using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;

/// <summary>
/// Summary description for Patient
/// </summary>
public class Patient
{
    string englishName;//שם באנגלית
    string displayName; //מזהה ייחודי
    string firstNameH;// שם פרטי בעברית
    string firstNameA;// שם פרטי בערבית
    string lastNameH;// שם משפחה בעברית
    string lastNameA;// שם משפחה בערבית
    string birthDate;// תאריך לידה
    string city;//יישוב
    Location barrier;//מחסום
    Location hospital;//בית חולים
    string department;//מחלקה
    string gender;//מין
    string cellPhone;//טלפון נייד
    string cellPhone1;//טלפון נייד1
    string homePhone;//טלפון
    string livingArea;//אזור (מחסום)
    List<Escorted> escortedList;//מלווים
    List<RidePat> ridePatList;//הסעות חולה
    string addition;//כלי עזר
    string history;//היסטוריה רפואית
    string status;//סטטוס
    string remarks;//הערות
    string numberOfEscort;
    string isAnonymous;
    int id;
    int patientIdentity;
    string lastModified;
    List<string> equipment;
    DbService dbs;

    public int? Age { get; set; }
    public Constants.Enums.Gender GenderAsEnum { get; set; }
    public bool IsActive { get; set; }

    public string pnRegId { get; set; }

    public int Id { get; set; }

    public List<string> Equipment { get; set; }

    public string DisplayNameA { get; set; }

    public string DisplayName
    {
        get
        {
            return displayName;
        }

        set
        {
            displayName = value;
        }
    }
    public RidePatPatientStatus RidePatPatientStatus { get; set; }


    public List<string> getEquipmentForPatient(string patient)
    {
        patient = patient.Replace("'", "''");
        Patient p = new Patient(patient);
        p.Equipment = new List<string>();
        string query = "select EquipmentName from EquipmentForPatientView where PatientName=N'" + patient + "'";
        DbService db = new DbService();
        DataSet ds = db.GetDataSetByQuery(query);
        foreach (DataRow dr in ds.Tables[0].Rows)
        {
            p.Equipment.Add(dr["EquipmentName"].ToString());
        }
        return p.Equipment;
    }


    public string FirstNameH
    {
        get
        {
            return firstNameH;
        }

        set
        {
            firstNameH = value;
        }
    }

    public string FirstNameA
    {
        get
        {
            return firstNameA;
        }

        set
        {
            firstNameA = value;
        }
    }

    public string LastNameH
    {
        get
        {
            return lastNameH;
        }

        set
        {
            lastNameH = value;
        }
    }

    public string LastNameA
    {
        get
        {
            return lastNameA;
        }

        set
        {
            lastNameA = value;
        }
    }

    public string BirthDate
    {
        get
        {
            return birthDate;
        }

        set
        {
            birthDate = value;
        }
    }

    public string City
    {
        get
        {
            return city;
        }

        set
        {
            city = value;
        }
    }

    public Location Barrier
    {
        get
        {
            return barrier;
        }

        set
        {
            barrier = value;
        }
    }

    public Location Hospital
    {
        get
        {
            return hospital;
        }

        set
        {
            hospital = value;
        }
    }

    public string Department
    {
        get
        {
            return department;
        }

        set
        {
            department = value;
        }
    }

    public string Gender
    {
        get
        {
            return gender;
        }

        set
        {
            gender = value;
        }
    }

    public string CellPhone
    {
        get
        {
            return cellPhone;
        }

        set
        {
            cellPhone = value;
        }
    }

    public string CellPhone1
    {
        get
        {
            return cellPhone1;
        }

        set
        {
            cellPhone1 = value;
        }
    }

    public string HomePhone
    {
        get
        {
            return homePhone;
        }

        set
        {
            homePhone = value;
        }
    }

    public string LivingArea
    {
        get
        {
            return livingArea;
        }

        set
        {
            livingArea = value;
        }
    }

    public List<Escorted> EscortedList
    {
        get
        {
            return escortedList;
        }

        set
        {
            escortedList = value;
        }
    }

    public List<RidePat> RidePatList
    {
        get
        {
            return ridePatList;
        }

        set
        {
            ridePatList = value;
        }
    }

    public string Addition
    {
        get
        {
            return addition;
        }

        set
        {
            addition = value;
        }
    }

    public string History
    {
        get
        {
            return history;
        }

        set
        {
            history = value;
        }
    }

    public string Status
    {
        get
        {
            return status;
        }

        set
        {
            status = value;
        }
    }

    public string Remarks
    {
        get
        {
            return remarks;
        }

        set
        {
            remarks = value;
        }
    }

    public string NumberOfEscort
    {
        get
        {
            return numberOfEscort;
        }

        set
        {
            numberOfEscort = value;
        }
    }

    public string IsAnonymous
    {
        get
        {
            return isAnonymous;
        }

        set
        {
            isAnonymous = value;
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

    public int PatientIdentity
    {
        get
        {
            return patientIdentity;
        }

        set
        {
            patientIdentity = value;
        }
    }

    public string LastModified
    {
        get
        {
            return lastModified;
        }

        set
        {
            lastModified = value;
        }
    }

    public Patient()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public Patient(string _firstNameH, string _lastNameH, string _cellPhone)
    {
        FirstNameH = _firstNameH;
        LastNameH = _lastNameH;
        CellPhone = _cellPhone;
    }

    public Patient(string _displayName, string _firstNameH, string _lastNameH, string _cellPhone)
    {
        DisplayName = _displayName;
        FirstNameH = _firstNameH;
        LastNameH = _lastNameH;
        CellPhone = _cellPhone;
    }
    public Patient(string _displayName)
    {
        DisplayName = _displayName;
    }


    public Patient(string _firstNameH, string _firstNameA, string _lastNameH, string _lastNameA, string _cellPhone, string _cellPhone1, string _homePhone,
      string _city, string _gender, string _birthdate, Location _barrier, Location _hospital,
      string _department, string _area, string _status, string _addition, string _history, string _remarks)
    {
        FirstNameH = _firstNameH;
        LastNameH = _lastNameH;
        FirstNameA = _firstNameA;
        LastNameA = _lastNameA;
        CellPhone = _cellPhone;
        CellPhone1 = _cellPhone1;
        HomePhone = _homePhone;
        City = _city;
        Gender = _gender;
        BirthDate = _birthdate;
        Barrier = _barrier;
        Hospital = _hospital;
        Department = _department;
        LivingArea = _area;
        Status = _status;
        Addition = _addition;
        Remarks = _remarks;
        History = _history;
    }

    public Patient(string _displayName, string _firstNameH, string _firstNameA, string _lastNameH, string _lastNameA, string _cellPhone,
        string _cellPhone1, string _homePhone, string _city, string _gender, string _birthdate, Location _barrier, Location _hospital,
      string _department, string _area, string _status, string _addition, string _history, string _remarks)
    {
        DisplayName = _displayName;
        FirstNameH = _firstNameH;
        LastNameH = _lastNameH;
        FirstNameA = _firstNameA;
        LastNameA = _lastNameA;
        CellPhone = _cellPhone;
        CellPhone1 = _cellPhone1;
        HomePhone = _homePhone;
        City = _city;
        Gender = _gender;
        BirthDate = _birthdate;
        Barrier = _barrier;
        Hospital = _hospital;
        Department = _department;
        LivingArea = _area;
        Status = _status;
        Addition = _addition;
        Remarks = _remarks;
        History = _history;
    }

    public List<Patient> GetPatients_slim(bool isActive)
    {
        List<Patient> patients = new List<Patient>();
        Patient patient;
        string query = "exec spPatient_GetPatients_slim @IsActive=" + isActive;

        try
        {
            dbs = new DbService();
            SqlDataReader sdr = dbs.GetDataReader(query);

            while (sdr.Read())
            {
                patient = new Patient();
                patient.Id = Convert.ToInt32(sdr["Id"]);
                patient.PatientIdentity = String.IsNullOrEmpty(sdr["PatientIdentity"].ToString()) ? 0 : Convert.ToInt32(sdr["PatientIdentity"]);
                patient.DisplayName = String.IsNullOrEmpty(sdr["DisplayName"].ToString()) ? "" : Convert.ToString(sdr["DisplayName"]);
                patient.EnglishName = String.IsNullOrEmpty(sdr["EnglishName"].ToString()) ? "" : Convert.ToString(sdr["EnglishName"]);
                patient.IsAnonymous = String.IsNullOrEmpty(sdr["IsAnonymous"].ToString()) ? "False" : Convert.ToString(sdr["IsAnonymous"]);

                patients.Add(patient);
            }
            return patients;
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            dbs.CloseConnection();
        }
    }

    //By Sufa
    //public List<Patient> getPatientsList(bool active)
    //{
    //    #region DB functions
    //    string query = "select * from Patient";
    //    if (active)
    //    {
    //        query += " where IsActive = 'true'";
    //    }

    //    query += " order by FirstNameH";

    //    List<Patient> list = new List<Patient>();
    //    DbService db = new DbService();
    //    DataSet ds = db.GetDataSetByQuery(query);

    //    foreach (DataRow dr in ds.Tables[0].Rows)
    //    {
    //        Patient p = new Patient();
    //        p.Id = int.Parse(dr["Id"].ToString());
    //        p.IsAnonymous = dr["IsAnonymous"].ToString();
    //        p.NumberOfEscort = dr["NumberOfEscort"].ToString();
    //        p.DisplayName = dr["DisplayName"].ToString();
    //        p.DisplayNameA = dr["DisplayNameA"].ToString();
    //        p.FirstNameA = dr["FirstNameA"].ToString();
    //        p.FirstNameH = dr["FirstNameH"].ToString();
    //        p.LastNameH = dr["LastNameH"].ToString();
    //        p.LastNameA = dr["LastNameA"].ToString();
    //        p.CellPhone = dr["CellPhone"].ToString();
    //        p.CellPhone1 = dr["CellPhone2"].ToString();
    //        p.HomePhone = dr["HomePhone"].ToString();
    //        p.City = dr["CityCityName"].ToString();
    //        p.LivingArea = dr["LivingArea"].ToString();
    //        //p.Status = dr["statusPatient"].ToString();
    //        p.IsActive = Convert.ToBoolean(dr["IsACtive"].ToString());
    //        //p.Addition = dr["addition"].ToString();

    //        p.BirthDate = dr["BirthDate"].ToString();
    //        p.History = dr["History"].ToString();
    //        p.Department = dr["Department"].ToString();
    //        p.Barrier = new Location(dr["Barrier"].ToString());
    //        p.Hospital = new Location(dr["Hospital"].ToString());
    //        p.Gender = dr["Gender"].ToString();
    //        p.Remarks = dr["Remarks"].ToString();


    //        //set equipment
    //        List<string> el = new List<string>();
    //        //db = new DbService();
    //        SqlCommand cmd = new SqlCommand();
    //        cmd.Parameters.AddWithValue("@displayName", p.DisplayName);
    //        cmd.CommandType = CommandType.Text;
    //        query = "select EquipmentName from EquipmentForPatientView where id=" + p.Id;
    //        DataSet ds2 = db.GetDataSetByQuery(query, false, cmd.CommandType, cmd.Parameters[0]);
    //        foreach (DataRow row in ds2.Tables[0].Rows)
    //        {
    //            string e = row["EquipmentName"].ToString();
    //            el.Add(e);
    //        }
    //        p.Equipment = el;

    //        list.Add(p);
    //    }
    //    db.CloseConnection();
    //    #endregion

    //    return list;
    //}
    public List<Patient> getPatientsList(bool active)
    {
        #region DB functions
        Location tmp = new Location();
        Hashtable locations = tmp.getLocationsEnglishName();

        string query = "select * from PatientsAndEquipmentView";
        if (active)
        {
            query += " where IsActive = 'true'";
        }

        query += " order by Id";

        List<Patient> list = new List<Patient>();
        List<string> el = new List<string>();
        Patient p = new Patient();
        int tempID = 0;

        DbService db = new DbService();
        DataSet ds = db.GetDataSetByQuery(query);
        foreach (DataRow dr in ds.Tables[0].Rows)
        {
            //new patient from view (patients in this view repeat themselves)
            if (tempID != int.Parse(dr["Id"].ToString()))
            {
                try
                {
                    //the first patient is null so we don't add him/her
                    if (tempID != 0)
                    {
                        //adding the last patient to the list.
                        list.Add(p);
                        p = new Patient();
                    }
                    p.Id = int.Parse(dr["Id"].ToString());
                    p.IsAnonymous = dr["IsAnonymous"].ToString();
                    p.NumberOfEscort = dr["NumberOfEscort"].ToString();
                    p.DisplayName = dr["DisplayName"].ToString();
                    p.DisplayNameA = dr["DisplayNameA"].ToString();
                    p.FirstNameA = dr["FirstNameA"].ToString();
                    p.FirstNameH = dr["FirstNameH"].ToString();
                    p.LastNameH = dr["LastNameH"].ToString();
                    p.LastNameA = dr["LastNameA"].ToString();
                    p.CellPhone = dr["CellPhone"].ToString();
                    p.CellPhone1 = dr["CellPhone2"].ToString();
                    p.HomePhone = dr["HomePhone"].ToString();
                    p.City = dr["CityCityName"].ToString();
                    p.LivingArea = dr["LivingArea"].ToString();
                    p.IsActive = Convert.ToBoolean(dr["IsACtive"].ToString());
                    p.BirthDate = dr["BirthDate"].ToString();
                    p.History = dr["History"].ToString();
                    p.Department = dr["Department"].ToString();
                    if (dr["PatientIdentity"].ToString() == "")
                    {
                        p.PatientIdentity = 0;
                    }
                    else p.PatientIdentity = int.Parse(dr["PatientIdentity"].ToString());
                    string barrier = dr["Barrier"].ToString();
                    p.Barrier = new Location(barrier);
                    if (locations[barrier] != null)
                    {
                        p.Barrier.EnglishName = locations[barrier].ToString();
                    }
                    else p.Barrier.EnglishName = "";
                    string hospital = dr["Hospital"].ToString();
                    p.Hospital = new Location();
                    p.Hospital.Name = hospital;
                    if (locations[hospital] != null)
                    {
                        p.Hospital.EnglishName = locations[hospital].ToString();
                    }
                    else p.Hospital.EnglishName = "";
                    p.Gender = dr["Gender"].ToString();
                    p.Remarks = dr["Remarks"].ToString();
                    p.EnglishName = dr["EnglishName"].ToString();
                    el = new List<string>();
                    //get equipment for patient from the same view
                    string e = dr["EquipmentName"].ToString();

                    p.LastModified = dr["lastModified"].ToString();
                    el.Add(e);
                    p.Equipment = el;

                    tempID = int.Parse(dr["Id"].ToString());
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            }
        }
        #endregion
        list.Add(p);
        return list;
    }

    //this function is for generating patients after anonymous patient is set. only patients with same locations will be assingned.
    public List<Patient> getAnonymousPatientsList(bool active, string origin, string dest)
    {
        #region DB functions
        string query = "select * from Patient where Barrier = N'" + origin + "' and Hospital = N'" + dest + "' OR Barrier = N'" + dest + "' and Hospital = N'" + origin + "'";
        if (active)
        {
            query += " and IsActive = 'true'";
        }

        query += " order by FirstNameH";

        List<Patient> list = new List<Patient>();
        DbService db = new DbService();
        DataSet ds = db.GetDataSetByQuery(query);

        foreach (DataRow dr in ds.Tables[0].Rows)
        {
            Patient p = new Patient();
            p.Id = int.Parse(dr["Id"].ToString());
            p.IsAnonymous = dr["IsAnonymous"].ToString();
            p.NumberOfEscort = dr["NumberOfEscort"].ToString();
            p.DisplayName = dr["DisplayName"].ToString();
            p.DisplayNameA = dr["DisplayNameA"].ToString();
            p.FirstNameA = dr["FirstNameA"].ToString();
            p.FirstNameH = dr["FirstNameH"].ToString();
            p.LastNameH = dr["LastNameH"].ToString();
            p.LastNameA = dr["LastNameA"].ToString();
            p.CellPhone = dr["CellPhone"].ToString();
            p.CellPhone1 = dr["CellPhone2"].ToString();
            p.HomePhone = dr["HomePhone"].ToString();
            p.City = dr["CityCityName"].ToString();
            p.LivingArea = dr["LivingArea"].ToString();
            //p.Status = dr["statusPatient"].ToString();
            p.IsActive = Convert.ToBoolean(dr["IsACtive"].ToString());
            //p.Addition = dr["addition"].ToString();

            p.BirthDate = dr["BirthDate"].ToString();
            p.History = dr["History"].ToString();
            p.Department = dr["Department"].ToString();
            p.Barrier = new Location(dr["Barrier"].ToString());
            p.Hospital = new Location(dr["Hospital"].ToString());
            p.Gender = dr["Gender"].ToString();
            p.Remarks = dr["Remarks"].ToString();
            p.EnglishName = dr["EnglishName"].ToString();
            if (dr["PatientIdentity"].ToString() == "")
            {
                p.PatientIdentity = 0;
            }
            else p.PatientIdentity = int.Parse(dr["PatientIdentity"].ToString());

            //set equipment
            List<string> el = new List<string>();
            db = new DbService();
            SqlCommand cmd = new SqlCommand();
            cmd.Parameters.AddWithValue("@displayName", p.DisplayName);
            cmd.CommandType = CommandType.Text;
            query = "select EquipmentName from EquipmentForPatientView where PatientName=@displayName";
            DataSet ds2 = db.GetDataSetByQuery(query, true, cmd.CommandType, cmd.Parameters[0]);
            foreach (DataRow row in ds2.Tables[0].Rows)
            {
                string e = row["EquipmentName"].ToString();
                el.Add(e);
            }
            p.Equipment = el;

            list.Add(p);
        }
        #endregion

        return list;
    }
    public List<string> GetLocationsForArea(string areaName)
    {
        List<string> locations = new List<string>();
        string query = "select * from Location where Area=N'" + areaName + "'";
        DbService db = new DbService();
        DataSet ds = db.GetDataSetByQuery(query);
        foreach (DataRow dr in ds.Tables[0].Rows)
        {
            locations.Add(dr["Name"].ToString());
        }

        return locations;
    }


    private DataTable getPatientsOnTheSamePath(string origin, string destination)
    {
        Location loc = new Location();
        string originArea = loc.GetAreaForPoint(origin);
        string destinationArea = loc.GetAreaForPoint(destination);
        string query = "select p.* from Patient p join location lo on p.Barrier = lo.Name join location ld on p.Hospital = ld.Name where(lo.Area = N'" + originArea + "' and ld.Area = N'" + destinationArea + "') or(lo.Area = N'" + destinationArea + "' and ld.Area = N'" + originArea + "') and (p.IsActive = 'true')";
        DbService db = new DbService();
        DataSet ds = db.GetDataSetByQuery(query);
        return ds.Tables[0];
    }

    public List<Patient> getAnonymousPatientsListForLocations(bool active, string origin, string dest, string area)
    {
        //change to query with "in" 
        #region DB functions

        /*
        string query = "";
        string text = "";

        
        for (int i = 0; i < locations.Count; i++)
        {
            text = locations[i].Replace("'", "''");
            if (i == 0)
            {
                query = "select * from Patient where ((Barrier = N'" + text + "' OR ";
            }
            if (i == locations.Count - 1)
            {
                query += "Barrier = N'" + text + "') and Hospital = N'" + dest + "') OR ((Hospital = N'" + text + "' OR ";
            }
            else query += "Barrier = N'" + text + "' OR ";
        }
        for (int i = 0; i < locations.Count; i++)
        {
            text = locations[i].Replace("'", "''");
            if (i == locations.Count - 1)
            {
                query += "Hospital = N'" + text + "') and Barrier = N'" + origin + "') OR ((Hospital = N'" + text + "' OR ";
            }
            else query += "Hospital = N'" + text + "' OR ";
        }
        for (int i = 0; i < locations.Count; i++)
        {
            text = locations[i].Replace("'", "''");
            if (i == locations.Count - 1)
            {
                query += "Hospital = N'" + text + "') and Barrier = N'" + dest + "') OR ((Barrier = N'" + text + "' OR ";
            }
            else query += "Hospital = N'" + text + "' OR ";
        }
        for (int i = 0; i < locations.Count; i++)
        {
            text = locations[i].Replace("'", "''");
            if (i == locations.Count - 1)
            {
                query += "Barrier = N'" + text + "') and Hospital = N'" + origin + "')";
            }
            else query += "Barrier = N'" + text + "' OR ";
        }
        query += " OR (Barrier = N'" + origin + "' and Hospital = N'" + dest + "') OR (Barrier = N'" + dest + "' and Hospital = N'" + origin + "')";
        if (active)
        {
            query += " and IsActive = 'true'";
        }
        query += " order by FirstNameH";
        */





        //DbService db = new DbService();

        //DataSet ds = db.GetDataSetByQuery(query);

        DataTable dt = getPatientsOnTheSamePath(origin, dest);

        List<Patient> list = new List<Patient>();

        foreach (DataRow dr in dt.Rows)
        {
            Patient p = new Patient();
            p.Id = int.Parse(dr["Id"].ToString());
            p.IsAnonymous = dr["IsAnonymous"].ToString();
            p.NumberOfEscort = dr["NumberOfEscort"].ToString();
            p.DisplayName = dr["DisplayName"].ToString();
            p.DisplayNameA = dr["DisplayNameA"].ToString();
            p.FirstNameA = dr["FirstNameA"].ToString();
            p.FirstNameH = dr["FirstNameH"].ToString();
            p.LastNameH = dr["LastNameH"].ToString();
            p.LastNameA = dr["LastNameA"].ToString();
            p.CellPhone = dr["CellPhone"].ToString();
            p.CellPhone1 = dr["CellPhone2"].ToString();
            p.HomePhone = dr["HomePhone"].ToString();
            p.City = dr["CityCityName"].ToString();
            p.LivingArea = dr["LivingArea"].ToString();
            //p.Status = dr["statusPatient"].ToString();
            p.IsActive = Convert.ToBoolean(dr["IsACtive"].ToString());
            //p.Addition = dr["addition"].ToString();

            p.BirthDate = dr["BirthDate"].ToString();
            p.History = dr["History"].ToString();
            p.Department = dr["Department"].ToString();
            p.Barrier = new Location(dr["Barrier"].ToString());
            p.Hospital = new Location(dr["Hospital"].ToString());
            p.Gender = dr["Gender"].ToString();
            p.Remarks = dr["Remarks"].ToString();
            p.EnglishName = dr["EnglishName"].ToString();
            if (dr["PatientIdentity"].ToString() == "")
            {
                p.PatientIdentity = 0;
            }
            else p.PatientIdentity = int.Parse(dr["PatientIdentity"].ToString());

            ////set equipment
            //List<string> el = new List<string>();
            //db = new DbService();
            //SqlCommand cmd = new SqlCommand();
            //cmd.Parameters.AddWithValue("@displayName", p.DisplayName);
            //cmd.CommandType = CommandType.Text;
            //query = "select EquipmentName from EquipmentForPatientView where PatientName=@displayName";
            //DataSet ds2 = db.GetDataSetByQuery(query, true, cmd.CommandType, cmd.Parameters[0]);
            //foreach (DataRow row in ds2.Tables[0].Rows)
            //{
            //    string e = row["EquipmentName"].ToString();
            //    el.Add(e);
            //}
            //p.Equipment = el;

            list.Add(p);
        }
        #endregion

        list = getEquipmentForAnon(list);

        return list;
    }

    public Patient GetPatientById(int id, int ridePatNum = -1)
    {
        string query;
        if (ridePatNum != -1)
        {
            query = "SELECT P.*, RPPS.RidePatNum as RidePatPatientStatus_RidePatNum, RPPS.PatientStatus, RPPS.EditTimeStamp FROM Patient P ";
            query += "LEFT JOIN RidePatPatientStatus RPPS ON P.Id = RPPS.PatientId AND RPPS.RidePatNum=" + ridePatNum + " WHERE P.Id=" + id;
        }
        else
        {
            query = "SELECT * FROM Patient where Id=" + id;
        }


        try
        {
            dbs = new DbService();
            SqlDataReader sdr = dbs.GetDataReader(query);
            Patient p = new Patient();
            if (sdr.Read())
            {
                p.Id = int.Parse(sdr["Id"].ToString());
                p.DisplayName = sdr["DisplayName"].ToString();
                p.FirstNameA = sdr["FirstNameA"].ToString();
                p.FirstNameH = sdr["FirstNameH"].ToString();
                p.LastNameH = sdr["LastNameH"].ToString();
                p.LastNameA = sdr["LastNameA"].ToString();
                p.CellPhone = sdr["CellPhone"].ToString();
                p.CellPhone1 = sdr["CellPhone2"].ToString();
                p.HomePhone = sdr["HomePhone"].ToString();
                p.City = sdr["CityCityName"].ToString();
                p.LivingArea = sdr["LivingArea"].ToString();
                p.IsActive = Convert.ToBoolean(sdr["IsACtive"].ToString());
                p.BirthDate = sdr["BirthDate"].ToString();
                p.History = sdr["History"].ToString();
                p.Department = sdr["Department"].ToString();
                p.Barrier = new Location(sdr["Barrier"].ToString());
                p.Hospital = new Location(sdr["Hospital"].ToString());
                p.Barrier = p.Barrier.getLocation(); //get the full discription of a location
                p.Hospital = p.Hospital.getLocation(); //get the full discription of a location
                p.Gender = sdr["Gender"].ToString();
                p.GenderAsEnum = Convertions.ConvertStringToGender(p.Gender);
                p.Remarks = sdr["Remarks"].ToString();
                p.EnglishName = sdr["EnglishName"].ToString();
                p.IsAnonymous = String.IsNullOrEmpty(sdr["IsAnonymous"].ToString()) ? "" : sdr["IsAnonymous"].ToString();

                int ridePatPatientStatus_RidePatNum;
                bool isExistsRidePatPatientStatus = false;
                if (HasColumn(sdr, "RidePatPatientStatus_RidePatNum"))
                     isExistsRidePatPatientStatus = int.TryParse(sdr["RidePatPatientStatus_RidePatNum"].ToString(), out ridePatPatientStatus_RidePatNum);
                if (isExistsRidePatPatientStatus)
                {
                    string patientStatus = sdr["PatientStatus"].ToString();
                    p.RidePatPatientStatus = new RidePatPatientStatus();
                    
                    p.RidePatPatientStatus.Status = Convertions.ConvertStringToPatientStatus(patientStatus);
                    p.RidePatPatientStatus.EditTimeStamp = String.IsNullOrEmpty(sdr["EditTimeStamp"].ToString()) ? null : (DateTime?)Convert.ToDateTime(sdr["EditTimeStamp"].ToString());

                }
                if (sdr["PatientIdentity"].ToString() == "")
                {
                    p.PatientIdentity = 0;
                }
                else p.PatientIdentity = int.Parse(sdr["PatientIdentity"].ToString());

                p.Equipment = p.getEquipmentForPatient(p.displayName);
                p.EscortedList = getescortedsList(p.DisplayName, "ridePatForm");
            }
            return p;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
        finally
        {
            dbs.CloseConnection();
        }
    }


    //-------------------------------------------------------
    //-- Benny optimization
    //----------------------------------------

    private List<Patient> getEquipmentForAnon(List<Patient> pList)
    {



        //set equipment
        List<string> el = new List<string>();
        DbService db = new DbService();
        SqlCommand cmd = new SqlCommand();
        cmd.CommandType = CommandType.Text;
        string query = "select id, EquipmentName from EquipmentForPatientView";
        DataSet ds2 = db.GetDataSetByQuery(query, true, cmd.CommandType);

        Dictionary<int, List<string>> elist = new Dictionary<int, List<string>>();

        foreach (DataRow row in ds2.Tables[0].Rows)
        {
            string e = row["EquipmentName"].ToString();
            int id = Convert.ToInt32(row["Id"]);
            if (!elist.ContainsKey(id))
            {
                elist.Add(id, new List<string>());
            }
            elist[id].Add(e);
        }

        for (int i = 0; i < pList.Count; i++)
        {
            if (elist.ContainsKey(pList[i].Id))
            {
                pList[i].equipment = elist[pList[i].Id];
            }
        }

        return pList;

    }


    public Patient getPatient()
    {
        #region DB functions
        displayName = displayName.Replace("'", "''");
        string query = "select * from Patient where displayName =N'" + displayName + "'";
        Patient p = new Patient();
        DbService db = new DbService();
        DataSet ds = db.GetDataSetByQuery(query);

        foreach (DataRow dr in ds.Tables[0].Rows)
        {
            p.Id = int.Parse(dr["Id"].ToString());
            p.DisplayName = dr["DisplayName"].ToString();
            p.FirstNameA = dr["FirstNameA"].ToString();
            p.FirstNameH = dr["FirstNameH"].ToString();
            p.LastNameH = dr["LastNameH"].ToString();
            p.LastNameA = dr["LastNameA"].ToString();
            p.CellPhone = dr["CellPhone"].ToString();
            p.CellPhone1 = dr["CellPhone2"].ToString();
            p.HomePhone = dr["HomePhone"].ToString();
            p.City = dr["CityCityName"].ToString();
            p.LivingArea = dr["LivingArea"].ToString();
            p.IsActive = Convert.ToBoolean(dr["IsACtive"].ToString());
            p.BirthDate = dr["BirthDate"].ToString();
            p.History = dr["History"].ToString();
            p.Department = dr["Department"].ToString();
            p.Barrier = new Location(dr["Barrier"].ToString());
            p.Hospital = new Location(dr["Hospital"].ToString());
            p.Gender = dr["Gender"].ToString();
            p.Remarks = dr["Remarks"].ToString();
            p.EnglishName = dr["EnglishName"].ToString();
            if (dr["PatientIdentity"].ToString() == "")
            {
                p.PatientIdentity = 0;
            }
            else p.PatientIdentity = int.Parse(dr["PatientIdentity"].ToString());

            p.Equipment = p.getEquipmentForPatient(p.displayName);
        }
        #endregion


        return p;
    }

    public List<string> getAllEquipment()
    {
        List<string> ls = new List<string>();
        string query = "select * from Equipment";
        DbService db = new DbService();
        DataSet ds = db.GetDataSetByQuery(query);
        foreach (DataRow dr in ds.Tables[0].Rows)
        {
            ls.Add(dr["Name"].ToString());
        }
        return ls;
    }

    public void SetPatientStatus(string active)
    {
        DbService db = new DbService();
        ChangeLastUpdateBy(0);
        db.ExecuteQuery("UPDATE Patient SET IsActive='" + active + "', lastModified=DATEADD(hour, 2, SYSDATETIME()) WHERE DisplayName=N'" + DisplayName + "'");

    }

    public void setPatient(string func)
    {
        //patient id
        int res = 0;
        DbService db;
        SqlCommand cmd = new SqlCommand();
        cmd.CommandType = CommandType.Text;
        SqlParameter[] cmdParams = new SqlParameter[19];

        cmdParams[0] = cmd.Parameters.AddWithValue("@firstNameH", FirstNameH);
        cmdParams[1] = cmd.Parameters.AddWithValue("@lastNameH", LastNameH);
        cmdParams[2] = cmd.Parameters.AddWithValue("@firstNameA", FirstNameA);
        cmdParams[3] = cmd.Parameters.AddWithValue("@lastNameA", LastNameA);
        cmdParams[4] = cmd.Parameters.AddWithValue("@cellPhone", CellPhone);
        cmdParams[5] = cmd.Parameters.AddWithValue("@cellPhone2", CellPhone1);
        cmdParams[6] = cmd.Parameters.AddWithValue("@homePhone", HomePhone);
        cmdParams[7] = cmd.Parameters.AddWithValue("@city", City);
        cmdParams[8] = cmd.Parameters.AddWithValue("@IsActive", IsActive);

        if (BirthDate == "")
            cmdParams[9] = cmd.Parameters.AddWithValue("@birthDate", DBNull.Value);
        else
            cmdParams[9] = cmd.Parameters.AddWithValue("@birthDate", BirthDate);

        cmdParams[10] = cmd.Parameters.AddWithValue("@history", History);
        cmdParams[11] = cmd.Parameters.AddWithValue("@department", Department);
        cmdParams[12] = cmd.Parameters.AddWithValue("@barrier", Barrier.Name);
        cmdParams[13] = cmd.Parameters.AddWithValue("@hospital", Hospital.Name);
        cmdParams[14] = cmd.Parameters.AddWithValue("@gender", Gender);
        cmdParams[15] = cmd.Parameters.AddWithValue("@remarks", Remarks);


        FirstNameH = FirstNameH.Trim();
        LastNameH = LastNameH.Trim();

        //handle situation when user insert full name twice. once to firstname and seconde to lastname
        if (FirstNameH == LastNameH)
        {
            if (firstNameA.Contains(" ") && lastNameH.Contains(" "))
            {
                FirstNameH = firstNameH.Substring(0, firstNameH.IndexOf(" "));
                LastNameH = LastNameH.Substring(LastNameH.IndexOf(" ") + 1, LastNameH.Length - FirstNameH.Length - 1);
            }
        }

        string displayName = FirstNameH + " " + LastNameH;



        cmdParams[16] = cmd.Parameters.AddWithValue("@displayName", displayName.Trim());
        cmdParams[17] = cmd.Parameters.AddWithValue("@englishName", EnglishName);
        cmdParams[18] = cmd.Parameters.AddWithValue("@patientIdentity", PatientIdentity);

        string query = "";
        if (func == "edit")
        {
            ChangeLastUpdateBy(Id);
            cmdParams[16].ToString().Trim();
            query = "UPDATE Patient SET FirstNameH=@firstNameH,FirstNameA=@firstNameA,LastNameH=@lastNameH,";
            query += "CellPhone=@cellPhone,CellPhone2=@cellPhone2,CityCityName=@city,IsActive=@IsActive,BirthDate=@birthDate,";
            query += "HomePhone=@homePhone,History=@history,Department=@department,Barrier=@barrier,Hospital=@hospital,Gender=@gender,Remarks=@remarks, DisplayName=@displayName,EnglishName=@englishName,PatientIdentity=@patientIdentity,lastModified=DATEADD(hour, 2, SYSDATETIME()) Where Id=" + Id;
            db = new DbService();

            try
            {
                res = db.ExecuteQuery(query, cmd.CommandType, cmdParams);
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627)
                {
                    // Violation in unique constraint
                    displayName += "_" + CellPhone;
                    cmdParams[16] = cmd.Parameters.AddWithValue("@displayName", displayName.Trim());
                    res = db.ExecuteQuery(query, cmd.CommandType, cmdParams);
                }
            }

            if (res > 0)
            {
                query = "delete from Equipment_Patient where PatientId=" + Id;
                db = new DbService();
                db.ExecuteQuery(query);
            }
            if (Equipment.Count > 0)
            {
                foreach (string e in Equipment)
                {
                    SqlParameter[] cmdParams2 = new SqlParameter[2];
                    cmdParams2[0] = cmd.Parameters.AddWithValue("@equipment", e);
                    cmdParams2[1] = cmd.Parameters.AddWithValue("@id", Id);
                    query = "insert into Equipment_Patient (EquipmentId,PatientId) values (@equipment,@id)";
                    db = new DbService();
                    db.ExecuteQuery(query, cmd.CommandType, cmdParams2);
                }

            }


        }
        else if (func == "new")
        {
            query = "insert into Patient (FirstNameH,FirstNameA,LastNameH,LastNameA,CellPhone,CellPhone2, HomePhone,";
            query += "CityCityName,IsActive,BirthDate,History,Department,Barrier,Hospital,Gender,Remarks,EnglishName,PatientIdentity,lastModified)";
            query += " values (@firstNameH,@firstNameA,@lastNameH,@lastNameA,";
            query += "@cellPhone,@cellPhone2,@homephone,@city,@IsActive,@birthDate,";
            query += "@history,@department,@barrier,@hospital,@gender,@remarks,@englishName,@patientIdentity,DATEADD(hour, 2, SYSDATETIME())); select SCOPE_IDENTITY()";
            db = new DbService();
            Id = int.Parse(db.GetObjectScalarByQuery(query, cmd.CommandType, cmdParams).ToString());


            if (Equipment.Count > 0 && Id != 0)
            {
                foreach (string e in Equipment)
                {
                    SqlParameter[] cmdParams2 = new SqlParameter[2];
                    cmdParams2[0] = cmd.Parameters.AddWithValue("@equipment", e);
                    cmdParams2[1] = cmd.Parameters.AddWithValue("@id", Id);
                    query = "insert into Equipment_Patient (EquipmentId,PatientId) values (@equipment,@id)";
                    db = new DbService();
                    db.ExecuteQuery(query, cmd.CommandType, cmdParams2);
                }

            }
            // query = prefix + sb.ToString();
            //query = "insert into Customers values ('" + CustomerName + "','" + CustomerContactName + "','" + AccountID + "','Y','" + Phone1 + "','" + Phone2 + "','" + Email + "'," + PaymentType.PaymentTypeID + ",'" + Comments + "'," + PreferedDrivers.DriverID + ", '" + RegistrationNumber + "', '" + BillingAddress + "')";
        }
        //db.ExecuteQuery(query);
    }

    //By Sufa & Matan --Get a list of all Escorts for Patient.
    public List<Escorted> getescortedsList(string displayName, string caller)
    {
        #region DB functions
        displayName = displayName.Replace("'", "''");
        string query = "select * from PatientEscortView where PatientName= N'" + displayName + "'";
        if (caller == "ridePatForm") query += " and IsActive = 'True'";
        //query += " order by EscortName";

        List<Escorted> list = new List<Escorted>();
        DbService db = new DbService();
        DataSet ds = db.GetDataSetByQuery(query);

        foreach (DataRow dr in ds.Tables[0].Rows)
        {
            Escorted e = new Escorted();
            // e.Pat = new Patient(dr["PatientName"].ToString()); //new Patient(dr["patient"].ToString());
            e.Id = int.Parse(dr["EscortId"].ToString());
            e.DisplayName = dr["EscortName"].ToString();
            e.FirstNameA = dr["FirstNameA"].ToString();
            e.FirstNameH = dr["FirstNameH"].ToString();
            e.LastNameH = dr["LastNameH"].ToString();
            e.LastNameA = dr["LastNameA"].ToString();
            e.CellPhone = dr["CellPhone"].ToString();
            e.CellPhone2 = dr["CellPhone2"].ToString();
            e.HomePhone = dr["HomePhone"].ToString();
            e.IsActive = Convert.ToBoolean(dr["IsActive"].ToString());
            // e.ContactType = dr["ContactType"].ToString();
            e.Gender = dr["Gender"].ToString();
            e.City = dr["City"].ToString();
            e.ContactType = dr["Relationship"].ToString();
            e.EnglishName = dr["EnglishName"].ToString();

            list.Add(e);
        }
        #endregion

        return list;
    }
    public List<Escorted> getescortedsListMobile(string displayName, string patientCell)
    {
        #region DB functions
        displayName = displayName.Replace("'", "''");
        string query = "select * from PatientEscortView where PatientName= N'" + displayName + "' and PatientCell='" + patientCell + "'";
        query += " and IsActive = 'True'";
        //query += " order by EscortName";

        List<Escorted> list = new List<Escorted>();
        DbService db = new DbService();
        DataSet ds = db.GetDataSetByQuery(query);

        foreach (DataRow dr in ds.Tables[0].Rows)
        {
            Escorted e = new Escorted();
            e.Pat = new Patient(dr["PatientName"].ToString()); //new Patient(dr["patient"].ToString());
            e.Id = int.Parse(dr["EscortId"].ToString());
            e.DisplayName = dr["EscortName"].ToString();
            e.FirstNameA = dr["FirstNameA"].ToString();
            e.FirstNameH = dr["FirstNameH"].ToString();
            e.LastNameH = dr["LastNameH"].ToString();
            e.LastNameA = dr["LastNameA"].ToString();
            e.CellPhone = dr["CellPhone"].ToString();
            e.CellPhone2 = dr["CellPhone2"].ToString();
            e.HomePhone = dr["HomePhone"].ToString();
            e.IsActive = Convert.ToBoolean(dr["IsActive"].ToString());
            // e.ContactType = dr["ContactType"].ToString();
            e.Gender = dr["Gender"].ToString();
            e.City = dr["City"].ToString();
            e.ContactType = dr["Relationship"].ToString();
            e.EnglishName = dr["EnglishName"].ToString();

            list.Add(e);
        }
        #endregion

        return list;
    }


    /// <summary>
    /// ChangeLastUpdateBy is a private method for changing the lastUpdateBy in the Patient table
    /// this functionality is for track who was the last one who change any field in a record of 
    /// Patient table.
    /// ------------------------------------------------------
    /// the name of the last one who change a recorde is taken from the session
    /// </summary>
    private void ChangeLastUpdateBy(int patientId)
    {
        string loggedInName = (string)HttpContext.Current.Session["loggedInName"];

        if (patientId == 0 && DisplayName.Length > 0)
        {
            Patient p = getPatient();
            patientId = p.Id;
        }

        string query = query = "exec spPatient_ChangeLastUpdateBy @lastUpdateBy=N'" + loggedInName + "', @id=" + patientId;
        SqlCommand cmd = new SqlCommand();

        try
        {
            dbs = new DbService();
            dbs.ExecuteQuery(query);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }

    }

    public static bool HasColumn(SqlDataReader Reader, string ColumnName)
    {
        foreach (DataRow row in Reader.GetSchemaTable().Rows)
        {
            if (row["ColumnName"].ToString() == ColumnName)
                return true;
        } //Still here? Column not found. 
        return false;
    }

}