using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
    bool isAnonymous;

    public string City { get; set; }


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

    public bool IsAnonymous
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

    public Escorted(Patient _pat, string _displayName, string _firstNameH, string _firstNameA, string _lastNameH, string _lastNameA,
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

    public Escorted(string _displayname, string _firstNameH, string _lastNameH, string _cellPhone)
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

    public void setEscortedStatus(string active)// change name to SetStatus
    {
        DbService db = new DbService();
        SqlCommand cmd = new SqlCommand();
        cmd.CommandType = CommandType.Text;
        SqlParameter[] cmdParams = new SqlParameter[2];
        cmdParams[0] = cmd.Parameters.AddWithValue("@displayName", DisplayName);
        cmdParams[1] = cmd.Parameters.AddWithValue("@status", active);
        string query = "UPDATE Escorted SET IsActive=@status WHERE displayName=@displayName";
        db.ExecuteQuery(query, cmd.CommandType, cmdParams);
    }

    public List<string> getContactType()
    {
        List<string> cl = new List<string>();
        string query = "select Name from ContactType";
        DbService db = new DbService();
        DataSet ds = db.GetDataSetByQuery(query);
        foreach (DataRow dr in ds.Tables[0].Rows)
        {
            ContactType = dr["Name"].ToString();
            cl.Add(ContactType);
        }
        return cl;
    }

    public Escorted getEscorted(string patientName)
    {
        #region DB functions
        SqlCommand cmd;
        SqlParameter[] cmdParams;
        // displayName = displayName.Replace("'", "''");
        string query = "select * from Escorted where DisplayName =@displayName"; //id,patient,displayName, firstNameH,firstNameA, lastNameH,lastNameA, cellPhone,cellPhone2,homePhone,city,statusEscorted, contactType,gender
        Escorted e = new Escorted();
        DbService db = new DbService();
        cmd = new SqlCommand();
        cmd.CommandType = CommandType.Text;
        cmdParams = new SqlParameter[1];
        cmdParams[0] = cmd.Parameters.AddWithValue("@displayName", displayName);
        DataSet ds = db.GetDataSetByQuery(query, true,cmd.CommandType, cmdParams);

        foreach (DataRow dr in ds.Tables[0].Rows)
        {
            e.Id = int.Parse(dr["Id"].ToString());
            //p.Pat = new Patient(dr["patient"].ToString());
            e.DisplayName = dr["DisplayName"].ToString();
            e.FirstNameA = dr["FirstNameA"].ToString();
            e.FirstNameH = dr["FirstNameH"].ToString();
            e.LastNameH = dr["LastNameH"].ToString();
            e.LastNameA = dr["LastNameA"].ToString();
            e.CellPhone = dr["CellPhone"].ToString();
            e.CellPhone2 = dr["CellPhone2"].ToString();
            e.HomePhone = dr["HomePhone"].ToString();
            e.City = dr["City"].ToString();
            //p.Status = dr["statusEscorted"].ToString();
            e.IsActive = Convert.ToBoolean(dr["IsActive"].ToString());
            // p.ContactType = dr["contactType"].ToString();
            e.Gender = dr["Gender"].ToString();
        }

        db = new DbService();
        cmd = new SqlCommand();
        cmd.CommandType = CommandType.Text;
        cmdParams = new SqlParameter[2];
        cmdParams[0] = cmd.Parameters.AddWithValue("@patient", patientName);
        cmdParams[1] = cmd.Parameters.AddWithValue("@escort", e.DisplayName);
        query = "select Relationship from RelationshipView where Patient=@patient and Escort=@escort";
        DataRow dr2 = db.GetDataSetByQuery(query, true,cmd.CommandType, cmdParams).Tables[0].Rows[0];
        e.ContactType = dr2["Relationship"].ToString();
        #endregion

        return e;
    }

    public void setEscorted(string func)
    {
        DbService db = new DbService();
        string query = "";
        SqlCommand cmd = new SqlCommand();
        cmd.CommandType = CommandType.Text;
        SqlParameter[] cmdParams = new SqlParameter[14];
        cmdParams[0] = cmd.Parameters.AddWithValue("@FirstNameH", FirstNameH);
        cmdParams[1] = cmd.Parameters.AddWithValue("@FirstNameA", FirstNameA);
        cmdParams[2] = cmd.Parameters.AddWithValue("@LastNameH", LastNameH);
        cmdParams[3] = cmd.Parameters.AddWithValue("@LastNameA", LastNameA);
        cmdParams[4] = cmd.Parameters.AddWithValue("@CellPhone", CellPhone);
        cmdParams[5] = cmd.Parameters.AddWithValue("@CellPhone2", CellPhone2);
        cmdParams[6] = cmd.Parameters.AddWithValue("@HomePhone", HomePhone);
        cmdParams[7] = cmd.Parameters.AddWithValue("@City", City);
        cmdParams[8] = cmd.Parameters.AddWithValue("@IsActive", IsActive);
        cmdParams[9] = cmd.Parameters.AddWithValue("@ContactType", ContactType);
        cmdParams[10] = cmd.Parameters.AddWithValue("@Gender", Gender);
        cmdParams[11] = cmd.Parameters.AddWithValue("@Id", Id);
        cmdParams[12] = cmd.Parameters.AddWithValue("@PatientId", Pat.Id);
        cmdParams[13] = cmd.Parameters.AddWithValue("@Relationship", 0);

        db = new DbService();
        query = "select Id from ContactType where Name=@ContactType";
        int Relationship = 0;
        try
        {
            Relationship = int.Parse(db.GetObjectScalarByQuery(query, cmd.CommandType, cmdParams).ToString());
            cmdParams[13] = cmd.Parameters.AddWithValue("@Relationship", Relationship);
        }
        catch (Exception e)
        {
            throw e;
        }


        if (func == "edit")
        {
            query = "update Escorted set FirstNameH=@FirstNameH,LastNameH=@LastNameH,FirstNameA=@FirstNameA,";
            query += "LastNameA=@LastNameA,CellPhone=@CellPhone,CellPhone2=@CellPhone2,HomePhone=@HomePhone,";
            query += "City=@City,IsActive=@IsActive,Gender=@Gender where Id=@Id";
            db = new DbService();
            int res = db.ExecuteQuery(query, cmd.CommandType, cmdParams);

            //if (res > 0)
            //{
            //    query = "delete from PatientEscort where PatientId=@PatientId and EscortId=@Id";
            //    db = new DbService();
            //    res = db.ExecuteQuery(query, cmd.CommandType, cmdParams);
            //}

            if (res > 0)
            {
                query = "update PatientEscort set ContactTypeId=@Relationship where PatientId=@PatientId and EscortId=@Id";
                db = new DbService();
                try
                {
                    db.ExecuteQuery(query, cmd.CommandType, cmdParams);
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        else if (func == "new")
        {
            query = "INSERT INTO Escorted (FirstNameH,LastNameH, CellPhone,CellPhone2,HomePhone,";
            query += "City,IsActive,Gender,FirstNameA,LastNameA)";
            query += " values (@FirstNameH,@LastNameH,@CellPhone,@CellPhone2,@HomePhone,@City,@IsActive,@Gender,@FirstNameA,@LastNameA); select SCOPE_IDENTITY()";
            db = new DbService();
            Id = int.Parse(db.GetObjectScalarByQuery(query, cmd.CommandType, cmdParams).ToString());
            cmdParams[11] = cmd.Parameters.AddWithValue("@Id", Id);
            query = "insert into PatientEscort (PatientId,EscortId,ContactTypeId) values (@PatientId,@Id,@Relationship)";
            db = new DbService();
            try
            {
                int res = db.ExecuteQuery(query, cmd.CommandType, cmdParams);
                if (res == 0) throw new Exception();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }

    //the function works only if the anonymous escort's id is: 22,23,25,26,28.
    public void setAnonymousEscorted(string func,int patientId,int numberOfEscort)
    {
        DbService db = new DbService();
        string query = "";
        SqlCommand cmd = new SqlCommand();
        cmd.CommandType = CommandType.Text;
        SqlParameter[] cmdParams = new SqlParameter[3];
        
        cmdParams[0] = cmd.Parameters.AddWithValue("@ContactType", 7);
        cmdParams[1] = cmd.Parameters.AddWithValue("@PatientId", patientId);

        if (func == "new")
        {
            for (int i = 1; i <= numberOfEscort; i++)
            {
                cmdParams[2] = cmd.Parameters.AddWithValue("@Id", i);
                query = "insert into PatientEscort (PatientId,EscortId,ContactTypeId) values (@PatientId,@Id,@ContactType);";
                db = new DbService();
                try
                {
                    int res = db.ExecuteQuery(query, cmd.CommandType, cmdParams);
                    if (res == 0) throw new Exception();
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
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