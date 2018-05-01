using System;
using System.Collections.Generic;
using System.Data;
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

            list.Add(p);
        }
        #endregion

        return list;
    }

    public Patient getPatient()
    {
        #region DB functions
        displayName = displayName.Replace("'", "''");
        string query = "select Id,DisplayName, FirstNameA, FirstNameH, LastNameH, LastNameA, CellPhone, CellPhone2, HomePhone, CityCityName, LivingArea, IsActive, BirthDate, History, Department, Barrier, Hospital, Gender, Remarks from Patient where displayName ='" + displayName + "'";
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

        }
        #endregion

        return p;
    }

    public void SetPatientStatus(string active)
    {
        DbService db = new DbService();
        db.ExecuteQuery("UPDATE Patient SET IsActive='" + active + "' WHERE DisplayName='" + DisplayName + "'");
    }

    public void setPatient(string func)
    {
        DbService db = new DbService();
        string query = "";
        if (func == "edit")
        {
            query = "UPDATE Patient SET displayName = '" + DisplayName + "', firstNameH = '" + FirstNameH + "', firstNameA = '" + FirstNameA + "', lastNameH = '" + LastNameH + "', lastNameA = '" + LastNameA + "', cellPhone = '" + CellPhone + "', cellPhone2 = '" + CellPhone1 +
            "', homePhone = '" + HomePhone + "', city = '" + City + "', livingArea = '" + LivingArea + "', statusPatient = '" + Status + "', addition = '" + Addition + "', birthdate = '" + BirthDate + "', history = '" + History + "', department = '" + Department + "', barrier = '" + Barrier.Name +
            "', hospital = '" + Hospital.Name + "', gender = '" + Gender + "', remarks = '" + Remarks + "' WHERE displayName = '" + DisplayName + "'";
        }
        else if (func == "new")
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Values('{0}', '{1}', '{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}', '{11}', '{12}','{13}','{14}','{15}','{16}','{17}','{18}')",
                DisplayName, FirstNameH, FirstNameA, LastNameH, LastNameA,
                CellPhone, CellPhone1, HomePhone, City, LivingArea,
                Status, Addition, BirthDate, History, Department,
                Barrier.Name, Hospital.Name, Gender, Remarks);
            String prefix = "INSERT INTO Patient " + "(displayName, firstNameH,firstNameA, lastNameH,lastNameA, cellPhone,cellPhone2,homePhone,city,livingArea, statusPatient,addition ,birthdate ,history,department,barrier,hospital,gender,remarks)";
            query = prefix + sb.ToString();
            //query = "insert into Customers values ('" + CustomerName + "','" + CustomerContactName + "','" + AccountID + "','Y','" + Phone1 + "','" + Phone2 + "','" + Email + "'," + PaymentType.PaymentTypeID + ",'" + Comments + "'," + PreferedDrivers.DriverID + ", '" + RegistrationNumber + "', '" + BillingAddress + "')";
        }
        db.ExecuteQuery(query);
    }

    //By Sufa & Matan --Get a list of all Escorts for Patient.
    public List<Escorted> getescortedsList(string displayName)
    {
        #region DB functions
        displayName = displayName.Replace("'", "''");
        string query = "select * from PatientEscortView where PatientName='" + displayName + "'";

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
            e.ContactType = dr["ContactType"].ToString();
            e.Gender = dr["Gender"].ToString();
            e.Addrees = dr["City"].ToString();

            list.Add(e);
        }
        #endregion

        return list;
    }
}