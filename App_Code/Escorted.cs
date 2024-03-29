﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;

public class Escorted
{
    #region Private Fields
    Patient pat;//חולה
    string englishName;//שם באנגלית
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
    DbService dbs;
    #endregion Private Fields

    #region Public Properties
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
    public int Id1
    {
        get
        {
            return id;
        }

        set
        {
            id = value;
        }
    }
    #endregion Public Properties

    #region Constructors
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
    #endregion Constructors

    #region Get Methods
    public List<Escorted> getContactType()
    {
        List<Escorted> cl = new List<Escorted>();
        string query = "select Name,EnglishName from ContactType";
        DbService db = new DbService();
        DataSet ds = db.GetDataSetByQuery(query);
        foreach (DataRow dr in ds.Tables[0].Rows)
        {
            Escorted e = new Escorted();
            e.DisplayName = dr["Name"].ToString();
            e.EnglishName = dr["EnglishName"].ToString();
            cl.Add(e);
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
        DataSet ds = db.GetDataSetByQuery(query, true, cmd.CommandType, cmdParams);

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
            e.EnglishName = dr["EnglishName"].ToString();
        }

        db = new DbService();
        cmd = new SqlCommand();
        cmd.CommandType = CommandType.Text;
        cmdParams = new SqlParameter[2];
        cmdParams[0] = cmd.Parameters.AddWithValue("@patient", patientName);
        cmdParams[1] = cmd.Parameters.AddWithValue("@escort", e.DisplayName);
        query = "select Relationship from RelationshipView where Patient=@patient and Escort=@escort";
        DataRow dr2 = db.GetDataSetByQuery(query, true, cmd.CommandType, cmdParams).Tables[0].Rows[0];
        e.ContactType = dr2["Relationship"].ToString();
        #endregion

        return e;
    }

    public Escorted GetEscortById(int id)
    {
        string query = "exec spEscorted_GetEscortById @id=" + id;
        try
        {
            dbs = new DbService();
            SqlDataReader sdr = dbs.GetDataReader(query);
            Escorted escort = new Escorted(); 
            if (sdr.Read())
            {
                escort.Id = int.Parse(sdr["Id"].ToString());
                escort.DisplayName = sdr["DisplayName"].ToString();
                escort.FirstNameH = sdr["FirstNameH"].ToString();
                escort.LastNameH = sdr["LastNameH"].ToString();
                escort.CellPhone = sdr["CellPhone"].ToString();
                escort.IsAnonymous =  String.IsNullOrEmpty(sdr["IsAnonymous"].ToString());
                escort.IsActive =  String.IsNullOrEmpty(sdr["IsActive"].ToString());
            }
            return escort;
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

    #endregion Get Methods

    #region SetMethods
    public void setEscortedStatus(string active)
    {
        ChangeLastUpdateBy(Id);
        DbService db = new DbService();
        SqlCommand cmd = new SqlCommand();
        cmd.CommandType = CommandType.Text;
        SqlParameter[] cmdParams = new SqlParameter[2];
        cmdParams[0] = cmd.Parameters.AddWithValue("@escortId", Id);
        cmdParams[1] = cmd.Parameters.AddWithValue("@status", active);
        string query = "exec spEscorted_ToggleIsActive @id=@escortId, @isActive=@status";
        db.ExecuteQuery(query, cmd.CommandType, cmdParams);
    }
    public void setEscorted(string func)
    {
        ChangeLastUpdateBy(Id);
        DbService db = new DbService();
        string query = "";
        SqlCommand cmd = new SqlCommand();
        cmd.CommandType = CommandType.Text;
        SqlParameter[] cmdParams = new SqlParameter[16];
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
        cmdParams[14] = cmd.Parameters.AddWithValue("@EnglishName", EnglishName);
        cmdParams[15] = cmd.Parameters.AddWithValue("@IsAnonymous", IsAnonymous);

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

            //->
            //ID IS ALLREADY EXIST

            query = "update Escorted set FirstNameH=@FirstNameH,LastNameH=@LastNameH,FirstNameA=@FirstNameA,";
            query += "LastNameA=@LastNameA,CellPhone=@CellPhone,CellPhone2=@CellPhone2,HomePhone=@HomePhone,";
            query += "City=@City,IsActive=@IsActive,Gender=@Gender,EnglishName=@EnglishName where Id=@Id";
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
            query += "City,IsActive,Gender,FirstNameA,LastNameA,EnglishName, IsAnonymous)";
            query += " values (@FirstNameH,@LastNameH,@CellPhone,@CellPhone2,@HomePhone,@City,@IsActive,@Gender,@FirstNameA,@LastNameA,@EnglishName,@IsAnonymous);" +
                "select SCOPE_IDENTITY()";
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
    public void setAnonymousEscorted(string func, int patientId, int numberOfEscort)
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
    #endregion SetMethods


    #region ChangeLastUpdateBy

    /// <summary>
    /// ChangeLastUpdateBy is a private method for changing the lastUpdateBy in the Escorted table
    /// this functionality is for track who was the last one who change any field in a record of 
    /// Escorted table.
    /// ------------------------------------------------------
    /// the name of the last one who change a recorde is taken from the session
    /// </summary>
    private void ChangeLastUpdateBy(int id)
    {
        /*
         Notice!
            when user is not logged in propely
            the HttpContext is not accessable !
            therefore → loggedInName = "משתמש לא מזוהה";
            error CS0103: The name 'HttpContext' does not exist in the current context
         */
        string loggedInName = (string)HttpContext.Current.Session["loggedInName"];
        if (String.IsNullOrEmpty(loggedInName))
        {
            loggedInName = "משתמש לא מזוהה";
        }

        SqlCommand cmd = new SqlCommand();
        cmd.CommandType = CommandType.Text;
        SqlParameter[] cmdParams = new SqlParameter[2];

        cmdParams[0] = cmd.Parameters.AddWithValue("@loggedInName", loggedInName);
        cmdParams[1] = cmd.Parameters.AddWithValue("@Id", id);

        string query = "exec spEscorted_ChangeLastUpdateBy @lastUpdateBy=@loggedInName, @id=@Id";
        try
        {
            dbs = new DbService();
            dbs.ExecuteQuery(query, cmd.CommandType, cmdParams);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }

    }

    #endregion ChangeLastUpdateBy
}