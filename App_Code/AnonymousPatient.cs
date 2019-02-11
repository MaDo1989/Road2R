using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for AnonymousPatient
/// </summary>
public class AnonymousPatient
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
    int numberOfEscort;
    bool isAnonymous;
    public AnonymousPatient()
    {
        CellPhone = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff",CultureInfo.InvariantCulture);
        LastNameH = "";
        LastNameA = "";
        IsAnonymous = true;
        Remarks = "";
        History = "";
        Department = "";
        HomePhone = "";
        City = "";
        Gender = "";
        LastNameA = "";
        CellPhone1 = "";
        BirthDate = DateTime.Now.Year.ToString();
    }
    public AnonymousPatient(string _displayName)
    {
        DisplayName = _displayName;
    }
    #region
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

    public int NumberOfEscort
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

    #endregion

    //getAllEquipment() taken from patient, don't need it here.

    //new function to set an anonymous patient
    public void setAnonymousPatient(string func)
    {
        int res = 0;
        DbService db;
        SqlCommand cmd = new SqlCommand();
        cmd.CommandType = CommandType.Text;
        SqlParameter[] cmdParams = new SqlParameter[18];
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
        cmdParams[16] = cmd.Parameters.AddWithValue("@isAnonymous", IsAnonymous);
        cmdParams[17] = cmd.Parameters.AddWithValue("@numberOfEscort", NumberOfEscort);
        string query = "";
        if (func == "edit")
        {
            query = "UPDATE Patient SET FirstNameH=@firstNameH,FirstNameA=@firstNameA,LastNameH=@lastNameH,";
            query += "IsActive=@IsActive,";
            query += "Barrier=@barrier,Hospital=@hospital,IsAnonymous=@isAnonymous,NumberOfEscort=@numberOfEscort Where Id=" + Id;
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
            query += "HomePhone,CityCityName,IsActive,BirthDate,History,Department,Barrier,Hospital,Gender,Remarks,IsAnonymous,NumberOfEscort)";
            query += " values (@firstNameH,@firstNameA,@lastNameH,@lastNameA,";
            query += "@cellPhone,@cellPhone2,@homePhone,@city,@IsActive,@birthDate,";
            query += "@history,@department,@barrier,@hospital,@gender,@remarks,@isAnonymous,@numberOfEscort); select SCOPE_IDENTITY()";
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
            Escorted escort = new Escorted();
            try
            {
                //function to change when escort's ids will change!
                escort.setAnonymousEscorted(func, Id, NumberOfEscort);
            }
            catch (Exception ex)
            {

                throw new Exception("Problem with setAnonymousEscorted: " + ex.Message);
            }
        }
    }
    public List<string> getEquipmentForPatient(string anonymousPatient)
    {
        anonymousPatient = anonymousPatient.Replace("'", "''");
        AnonymousPatient p = new AnonymousPatient(anonymousPatient);
        p.Equipment = new List<string>();
        string query = "select EquipmentName from EquipmentForPatientView where PatientName=N'" + anonymousPatient + "'";
        DbService db = new DbService();
        DataSet ds = db.GetDataSetByQuery(query);
        foreach (DataRow dr in ds.Tables[0].Rows)
        {
            p.Equipment.Add(dr["EquipmentName"].ToString());
        }
        return p.Equipment;
    }

    public AnonymousPatient getAnonymousPatient()
    {
        #region DB functions
        displayName = displayName.Replace("'", "''");
        string query = "select * from Patient where displayName =N'" + displayName + "'";
        AnonymousPatient p = new AnonymousPatient();
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
            //p.HomePhone = dr["HomePhone"].ToString();
            p.City = dr["CityCityName"].ToString();
            p.LivingArea = dr["LivingArea"].ToString();
            //p.Status = dr["statusPatient"].ToString();
            p.IsActive = Convert.ToBoolean(dr["IsACtive"].ToString());
            //p.Addition = dr["addition"].ToString();
            //p.BirthDate = dr["BirthDate"].ToString();
           // p.History = dr["History"].ToString();
            //p.Department = dr["Department"].ToString();
            p.Barrier = new Location(dr["Barrier"].ToString());
            p.Hospital = new Location(dr["Hospital"].ToString());
            p.Gender = dr["Gender"].ToString();
            //p.Remarks = dr["Remarks"].ToString();     
            p.Equipment = p.getEquipmentForPatient(p.displayName);
            if (dr["NumberOfEscort"].ToString() == "")
            {
                p.NumberOfEscort = 0;
            }
            else p.NumberOfEscort = int.Parse(dr["NumberOfEscort"].ToString());
        }
        #endregion


        //Escorted es = new Escorted();
        //es.setAnonymousEscorted(func, p.NumberOfEscort);


        return p;
    }

    //check spaces in car in case of increasing escorts (for anonymous ride)
    public int checkSpaceInCar(int ridePatNum,int driverId)
    {
        //only if ride is not null.
        string query = "select * from RidePat where RidePatNum = " + ridePatNum;
        //count number of seats taken in each ridePat with the same ridepat num
        string query2 = "select * from PatientEscort_PatientInRide where [PatientInRide (RidePat)RidePatNum] = " +ridePatNum;

        List<int> numberOfRideParForRide = new List<int>();
        int numberOfSpaces = 0;
        //first check if we get a ride num for the ridePat
        try
        {
            DbService db = new DbService();
            DataSet ds = db.GetDataSetByQuery(query);
            if (ds.Tables[0].Rows.Count == 1)
            {
                //getting the ride id
                int rideId = int.Parse(ds.Tables[0].Rows[0]["RideId"].ToString());
                //selecting all ridePats numbers for the same ride id
                query = "select RidePatNum from RidePat where RideId = " + rideId;
                try
                {
                    ds = db.GetDataSetByQuery(query);

                    //adding each ridepat number into an array.
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        int ridePatFromTable = int.Parse(dr["RidePatNum"].ToString());
                        numberOfRideParForRide.Add(ridePatFromTable);
                    }
                    //counting the number of escorts in all ridepats assigned to a ride id
                    foreach (int ridePat in numberOfRideParForRide)
                    {
                        try
                        {
                            query2 = "select * from PatientEscort_PatientInRide where [PatientInRide (RidePat)RidePatNum] = " + ridePat;

                            ds = db.GetDataSetByQuery(query2);

                            //the number of escorts is as the number of rows in db.
                            if (ds == null)
                            {
                                numberOfSpaces = 0;
                            }
                            else numberOfSpaces = ds.Tables[0].Rows.Count;
                        }
                        catch (Exception ex)
                        {

                            throw;
                        }

                    }

                }
                catch (Exception ex)
                {

                    throw;
                }

            }
            //if the ride pat doesnt belong to a ride yet
            else
            {
                ds = db.GetDataSetByQuery(query2);
                //the number of escorts is as the number of rows in db.
                numberOfSpaces = ds.Tables[0].Rows.Count;
            }

        }
        catch (Exception ex)
        {

            throw;
        }
        Volunteer v = new Volunteer();
        int seats = v.spaceInCar(driverId) - numberOfSpaces;
        return seats;
    }


}