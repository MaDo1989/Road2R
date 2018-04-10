using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;

/// <summary>
/// Summary description for Escorted
/// </summary>
public class Escorted
{
    Patient pat;//חולה
    string displayName; //מזהה ייחודי
    string firstNameH;//שם פרטי עברית
    string firstNameA;//שם פרטי ערבית
    string lastNameH;// שם משפחה עברית
    string lastNameA;//שם משפחה ערבית
    string addrees;//כתובת
    string cellPhone;//טלפון נייד
    string cellPhone2;//טלפון 
    string homePhone;//טלפון בית
    string status;//סטטוס
    string contactType;//קרבה לחולה
    string gender;
    int id;


    public bool IsActive { get; set; }

    public int Id { get; set; }

    public Patient Pat
    {
        get
        {
            return pat;
        }

        set
        {
            pat = value;
        }
    }

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

    public string Addrees
    {
        get
        {
            return addrees;
        }

        set
        {
            addrees = value;
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

    public string CellPhone2
    {
        get
        {
            return cellPhone2;
        }

        set
        {
            cellPhone2 = value;
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

    public string ContactType
    {
        get
        {
            return contactType;
        }

        set
        {
            contactType = value;
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

    public Escorted(Patient _pat,string _displayName, string _firstNameH, string _firstNameA, string _lastNameH, string _lastNameA,
     string _addrees, string _cellPhone, string _cellPhone2, string _homePhone, string _status, string _contactType, string _gender)
    {
        Pat = _pat;
        DisplayName = _displayName;
        FirstNameA = _firstNameA;
        FirstNameH = _firstNameH;
        LastNameA = _lastNameA;
        LastNameH = _lastNameH;
        Addrees = _addrees;
        CellPhone = _cellPhone;
        CellPhone2 = _cellPhone2;
        HomePhone = _homePhone;
        Status = _status;
        ContactType = _contactType;
        Gender = _gender;
    }

    public Escorted(Patient _pat, string _firstNameH, string _firstNameA, string _lastNameH, string _lastNameA,
        string _addrees, string _cellPhone, string _cellPhone2, string _homePhone, string _status, string _contactType, string _gender)
    {
        Pat = _pat;
        FirstNameA = _firstNameA;
        FirstNameH = _firstNameH;
        LastNameA = _lastNameA;
        LastNameH = _lastNameH;
        Addrees = _addrees;
        CellPhone = _cellPhone;
        CellPhone2 = _cellPhone2;
        HomePhone = _homePhone;
        Status = _status;
        ContactType = _contactType;
        Gender = _gender;
    }
         public Escorted(string _displayName, string _firstNameH, string _firstNameA, string _lastNameH, string _lastNameA,
         string _cellPhone, string _cellPhone2, string _homePhone, string _addrees, string _status, string _contactType, string _gender)
    {

        DisplayName = _displayName;
        FirstNameA = _firstNameA;
        FirstNameH = _firstNameH;
        LastNameA = _lastNameA;
        LastNameH = _lastNameH;
        Addrees = _addrees;
        CellPhone = _cellPhone;
        CellPhone2 = _cellPhone2;
        HomePhone = _homePhone;
        Status = _status;
        ContactType = _contactType;
        Gender = _gender;
    }
    public Escorted()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public Escorted(string _displayname,string _firstNameH,string _lastNameH,string _cellPhone)
    {
        DisplayName = _displayname;
        FirstNameH = _firstNameH;
        LastNameH = _lastNameH;
        CellPhone = _cellPhone;
    }

    public Escorted(string _displayname)
    {
        DisplayName = _displayname;
    }

    public void deactivateEscorted(string active)// change name to SetStatus
    {
        DbService db = new DbService();
        db.ExecuteQuery("UPDATE Escorted SET statusEscorted='" + active + "' WHERE displayName='" + DisplayName + "'");
    }


    public Escorted getEscorted()
    {
        #region DB functions
        displayName = displayName.Replace("'", "''");
        string query = "select * from Escorted where DisplayName ='" + displayName + "'"; //id,patient,displayName, firstNameH,firstNameA, lastNameH,lastNameA, cellPhone,cellPhone2,homePhone,city,statusEscorted, contactType,gender
        Escorted p = new Escorted();
        DbService db = new DbService();
        DataSet ds = db.GetDataSetByQuery(query);

        foreach (DataRow dr in ds.Tables[0].Rows)
        {
            p.Id = int.Parse(dr["Id"].ToString());
            //p.Pat = new Patient(dr["patient"].ToString());
            p.DisplayName = dr["DisplayName"].ToString();
            p.FirstNameA = dr["FirstNameA"].ToString();
            p.FirstNameH = dr["FirstNameH"].ToString();
            p.LastNameH = dr["LastNameH"].ToString();
            p.LastNameA = dr["LastNameA"].ToString();
            p.CellPhone = dr["CellPhone"].ToString();
            p.CellPhone2 = dr["CellPhone2"].ToString();
            p.HomePhone = dr["HomePhone"].ToString();
            p.Addrees = dr["City"].ToString();
            //p.Status = dr["statusEscorted"].ToString();
            p.IsActive = Convert.ToBoolean(dr["IsActive"].ToString());
           // p.ContactType = dr["contactType"].ToString();
            p.Gender = dr["Gender"].ToString();

        }
        #endregion

        return p;
    }

    public void setEscorted(string func)
    {
        DbService db = new DbService();
        string query = "";
        displayName = displayName.Replace("'", "''");
        FirstNameH = FirstNameH.Replace("'", "''");
        LastNameH = LastNameH.Replace("'", "''");
        if (func == "edit")
        {
            query = "UPDATE Escorted SET displayName = '" + DisplayName + "', firstNameH = '" + FirstNameH + "', firstNameA = '" + FirstNameA + "', lastNameH = '" + LastNameH + "', lastNameA = '" + LastNameA + "', cellPhone = '" + CellPhone + "', cellPhone2 = " + CellPhone2 +
            ", homePhone = '" + HomePhone + "', city = '" + Addrees + "', statusEscorted = '" + Status + "', contactType = '" + ContactType + "', gender = '" + Gender + "' WHERE id = '" + Id + "'";
        }
        else if (func == "new")
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Values('{0}', '{1}', '{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}', '{11}')",
                DisplayName, FirstNameH, FirstNameA, LastNameH, LastNameA,
                CellPhone, CellPhone2, HomePhone, Addrees, Status,
                ContactType, Gender);
            String prefix = "INSERT INTO Escorted " + "(displayName, firstNameH,firstNameA, lastNameH,lastNameA, cellPhone,cellPhone2,homePhone,city,statusEscorted, contactType,gender)";
            query = prefix + sb.ToString();
            //query = "insert into Customers values ('" + CustomerName + "','" + CustomerContactName + "','" + AccountID + "','Y','" + Phone1 + "','" + Phone2 + "','" + Email + "'," + PaymentType.PaymentTypeID + ",'" + Comments + "'," + PreferedDrivers.DriverID + ", '" + RegistrationNumber + "', '" + BillingAddress + "')";
        }
        db.ExecuteQuery(query);
        string query2 = "select id from Escorted where displayName='" + displayName+"'";
        DbService db2 = new DbService();
        var Eid = db2.GetObjectScalarByQuery(query2);
        var Pid = pat.Id;
        DbService db3 = new DbService();
        string query3 = "insert into PatientEscort (PatientId,EscortId) values ('" + Pid +"','"+Eid+"')";
        db3.ExecuteQuery(query3);

    }

    //public DataTable read()
    //{
    //    DBservices dbs = new DBservices();
    //    dbs = dbs.ReadFromDataBase("RoadDBconnectionString", "Escorted");
    //    return dbs.dt;
    //}


    //    public List<Escorted> getListEscorted(string displayNamePat)
    //{
    //    DBservices dbs = new DBservices();
    //    List<Escorted> listE = new List<Escorted>();
    //    listE = dbs.getListEscorted("RoadDBconnectionString", "Escorted", displayNamePat);
    //    return listE;
    //}
}