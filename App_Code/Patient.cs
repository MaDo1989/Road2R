using System;
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
    int id;
    List<string> equipment;

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

    public List<string> getEquipmentForPatient(string patient)
    {
        patient = patient.Replace("'", "''");
        Patient p = new Patient(patient);
        p.Equipment = new List<string>();
        string query = "select EquipmentName from EquipmentForPatientView where PatientName='" + patient + "'";
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

    //By Sufa
    public List<Patient> getPatientsList(bool active)
    {
        #region DB functions
        string query = "select * from Patient";
        if (active)
        {
            query += " where IsActive = 'true'";
        }

        query += " order by FirstNameH";

        List<Patient> list = new List<Patient>();
        DbService db = new DbService();
        DataSet ds = db.GetDataSetByQuery(query);

        foreach (DataRow dr in ds.Tables[0].Rows)
        {
            Patient p = new Patient();
            p.Id = int.Parse(dr["Id"].ToString());
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

            //set equipment
            List<string> el = new List<string>();
            db = new DbService();
            SqlCommand cmd= new SqlCommand();
            cmd.Parameters.AddWithValue("@displayName", p.DisplayName);
            cmd.CommandType = CommandType.Text;
            query = "select EquipmentName from EquipmentForPatientView where PatientName=@displayName";
            DataSet ds2 = db.GetDataSetByQuery(query,cmd.CommandType,cmd.Parameters[0]);
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

    public Patient getPatient()
    {
        #region DB functions
        displayName = displayName.Replace("'", "''");
        string query = "select * from Patient where displayName ='" + displayName + "'";
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
        db.ExecuteQuery("UPDATE Patient SET IsActive='" + active + "' WHERE DisplayName='" + DisplayName + "'");
    }

    public void setPatient(string func)
    {
        int res = 0;
        DbService db;
        SqlCommand cmd = new SqlCommand();
        cmd.CommandType = CommandType.Text;
        SqlParameter[] cmdParams = new SqlParameter[16];
        cmdParams[0] = cmd.Parameters.AddWithValue("@firstNameH", FirstNameH);
        cmdParams[1] = cmd.Parameters.AddWithValue("@lastNameH", LastNameH);
        cmdParams[2] = cmd.Parameters.AddWithValue("@firstNameA", FirstNameA);
        cmdParams[3] = cmd.Parameters.AddWithValue("@lastNameA", LastNameA);
        cmdParams[4] = cmd.Parameters.AddWithValue("@cellPhone", CellPhone);
        cmdParams[5] = cmd.Parameters.AddWithValue("@cellPhone2", CellPhone1);
        cmdParams[6] = cmd.Parameters.AddWithValue("@homePhone", HomePhone);
        cmdParams[7] = cmd.Parameters.AddWithValue("@city", City);
        cmdParams[8] = cmd.Parameters.AddWithValue("@IsActive", IsActive);
        cmdParams[9] = cmd.Parameters.AddWithValue("@birthDate", BirthDate);
        cmdParams[10] = cmd.Parameters.AddWithValue("@history", History);
        cmdParams[11] = cmd.Parameters.AddWithValue("@department", Department);
        cmdParams[12] = cmd.Parameters.AddWithValue("@barrier", Barrier.Name);
        cmdParams[13] = cmd.Parameters.AddWithValue("@hospital", Hospital.Name);
        cmdParams[14] = cmd.Parameters.AddWithValue("@gender", Gender);
        cmdParams[15] = cmd.Parameters.AddWithValue("@remarks", Remarks);
        string query = "";
        if (func == "edit")
        {
            query = "UPDATE Patient SET FirstNameH=@firstNameH,FirstNameA=@firstNameA,LastNameH=@lastNameH,";
            query += "CellPhone =@cellPhone,CellPhone2=@cellPhone2,HomePhone=@homePhone,CityCityName=@city,IsActive=@IsActive,BirthDate=@birthDate,";
            query += "History=@history,Department=@department,Barrier=@barrier,Hospital=@hospital,Gender=@gender,Remarks=@remarks Where Id=" + Id;
            db = new DbService();
            res = db.ExecuteQuery(query, cmd.CommandType, cmdParams);
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
            query = "insert into Patient (FirstNameH,FirstNameA,LastNameH,LastNameA,CellPhone,CellPhone2,";
            query += "HomePhone,CityCityName,IsActive,BirthDate,History,Department,Barrier,Hospital,Gender,Remarks)";
            query += " values (@firstNameH,@firstNameA,@lastNameH,@lastNameA,";
            query += "@cellPhone,@cellPhone2,@homePhone,@city,@IsActive,@birthDate,";
            query += "@history,@department,@barrier,@hospital,@gender,@remarks); select SCOPE_IDENTITY()";
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
        string query = "select * from PatientEscortView where PatientName='" + displayName + "'";
        if (caller == "ridePatForm") query += " and IsActive = 'True'";
        query += " order by EscortName";

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

            list.Add(e);
        }
        #endregion

        return list;
    }
}