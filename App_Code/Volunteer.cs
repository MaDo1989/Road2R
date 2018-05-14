using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;

/// <summary>
/// Summary description for Volunteer
/// </summary>
public class Volunteer
{
    string displayName; //מזהה ייחודי
    string firstNameH;//שם פרטי בעברית
    string firstNameA;//שם פרטי בערבית
    string lastNameH;//שם משפחה בעברית
    string lastNameA;//שם משפחה בערבית
    string cellPhone;//טלפון נייד
    string cellPhone2;//טלפון נייד
    string homePhone;//טלפון
    string city;//יישוב
    string address;//רחוב
    string gender;//מין
    string typeVol;//סוג מתנדב
    string email;//אי מייל
    string birthdate;//תאריך לידה
    string preferRoute1;
    string preferRoute2;
    string preferRoute3;
    string day1;
    string day2;
    string day3;
    string hour1;
    string hour2;
    string hour3;
    string joinDate;//תאריך הצטרפות
    string status;//סטטוס
    string knowArabic;//יודע ערבית?
    int id;

    public List<string> PrefArea { get; set; }

    public List<string> PrefLocation { get; set; }

    public List<string[]> PrefTime { get; set; }

    public int AvailableSeats { get; set; }

    public string UserName { get; set; }

    public string pnRegId { get; set; }

    public string DriverType { get; set; } //Primary or Secondary

    public int Id { get; set; }

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

    public int setVolunteerPrefs(int Id, List<string> PrefLocation, List<string> PrefArea, List<string> PrefTime, int AvailableSeats)
    {
        string query = "";
        int res = 0;
        DbService db;
        SqlCommand cmd;

        foreach (string location in PrefLocation) //insert Location Preferences to DB
        {
            //Delete previous preferences in DB
            db = new DbService();
            query = "delete from PreferedDay_Volunteer where VolunteerId=" + id+";";
            query += "delete from PreferredArea_Volunteer where VolunteerId = " + id + ";";
            query+= "delete from PreferredLocation_Volunteer where VolunteerId = " + id + ";";
            db.ExecuteQuery(query);


            db = new DbService();
            cmd = new SqlCommand();
            cmd.CommandType = CommandType.Text;
            SqlParameter[] locationParams = new SqlParameter[2];
            locationParams[0] = cmd.Parameters.AddWithValue("@location", location);
            locationParams[1] = cmd.Parameters.AddWithValue("@Id", Id);
            query = "insert into PreferredLocation_Volunteer (PreferredLocation,VolunteerId) values (@location,@Id);";
            res += db.ExecuteQuery(query, cmd.CommandType, locationParams);
        }

        foreach (string area in PrefArea) //insert Area Preferences to DB
        {
            db = new DbService();
            cmd = new SqlCommand();
            cmd.CommandType = CommandType.Text;
            SqlParameter[] AreaParams = new SqlParameter[2];
            AreaParams[0] = cmd.Parameters.AddWithValue("@area", area);
            AreaParams[1] = cmd.Parameters.AddWithValue("@Id", Id);
            query = "insert into PreferredArea_Volunteer (PreferredArea,VolunteerId) values (@area,@Id);";
            res += db.ExecuteQuery(query, cmd.CommandType, AreaParams);
        }

        foreach (string shift in PrefTime) //insert Day&Shift Preferences to DB
        {
            string day = shift.Substring(shift.Length - 1);
            string finalShift = shift.Substring(0, shift.Length - 1);
            if (finalShift=="morning")
            {
                finalShift = "בוקר";
            }
            else
            {
                finalShift = "אחהצ";
            }
            switch (day)
            {
                case "A":
                    day = "ראשון";
                    break;
                case "B":
                    day = "שני";
                    break;
                case "C":
                    day = "שלישי";
                    break;
                case "D":
                    day = "רביעי";
                    break;
                case "E":
                    day = "חמישי";
                    break;
                case "F":
                    day = "שישי";
                    break;
                case "G":
                    day = "שבת";
                    break;
            }


            db = new DbService();
            cmd = new SqlCommand();
            cmd.CommandType = CommandType.Text;
            SqlParameter[] shiftParams = new SqlParameter[3];
            shiftParams[0] = cmd.Parameters.AddWithValue("@day", day);
            shiftParams[1] = cmd.Parameters.AddWithValue("@shift", finalShift);
            shiftParams[2] = cmd.Parameters.AddWithValue("@Id", Id);
            query = "insert into PreferedDay_Volunteer (PreferedDayDayInWeek,VolunteerId,Shift) values (@day,@Id,@shift);";
            res += db.ExecuteQuery(query, cmd.CommandType, shiftParams);
        }

        db = new DbService();
        cmd = new SqlCommand();
        query = "update Volunteer set AvailableSeats=" + AvailableSeats + " where Id=" + Id;
        res += db.ExecuteQuery(query);
        return res;
    }

    public void getVolunteerPrefs(int id)
    {
        string query = "select PreferedDayDayInWeek,Shift from PreferedDay_Volunteer where VolunteerId=" + id;
        DbService db = new DbService();
        DataSet ds = db.GetDataSetByQuery(query);
        foreach (DataRow dr in ds.Tables[0].Rows)
        {

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

    public string Address
    {
        get
        {
            return address;
        }

        set
        {
            address = value;
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

    public string TypeVol
    {
        get
        {
            return typeVol;
        }

        set
        {
            typeVol = value;
        }
    }

    public string Email
    {
        get
        {
            return email;
        }

        set
        {
            email = value;
        }
    }

    public string Birthdate
    {
        get
        {
            return birthdate;
        }

        set
        {
            birthdate = value;
        }
    }

    public string PreferRoute1
    {
        get
        {
            return preferRoute1;
        }

        set
        {
            preferRoute1 = value;
        }
    }

    public string PreferRoute2
    {
        get
        {
            return preferRoute2;
        }

        set
        {
            preferRoute2 = value;
        }
    }

    public string PreferRoute3
    {
        get
        {
            return preferRoute3;
        }

        set
        {
            preferRoute3 = value;
        }
    }

    public string Day1
    {
        get
        {
            return day1;
        }

        set
        {
            day1 = value;
        }
    }

    public string Day2
    {
        get
        {
            return day2;
        }

        set
        {
            day2 = value;
        }
    }

    public string Day3
    {
        get
        {
            return day3;
        }

        set
        {
            day3 = value;
        }
    }

    public string Hour1
    {
        get
        {
            return hour1;
        }

        set
        {
            hour1 = value;
        }
    }

    public string Hour2
    {
        get
        {
            return hour2;
        }

        set
        {
            hour2 = value;
        }
    }

    public string Hour3
    {
        get
        {
            return hour3;
        }

        set
        {
            hour3 = value;
        }
    }

    internal List<Volunteer> getCoorList()
    {
        string query = "select * from VolunteerTypeView where VolunTypeType='רכז' and IsActive='true'";
        DbService db = new DbService();
        DataSet ds = db.GetDataSetByQuery(query);
        List<Volunteer> vl = new List<Volunteer>();
        foreach (DataRow dr in ds.Tables[0].Rows)
        {
            Volunteer v = new Volunteer();
            v.DisplayName = dr["DisplayName"].ToString();
            v.CellPhone = dr["CellPhone"].ToString();
            v.TypeVol = "רכז";
            v.UserName = dr["UserName"].ToString();
            vl.Add(v);
        }
        return vl;

    }

    public Volunteer getCoor(string userName)
    {
        string query = "select * from Volunteer where UserName='" + userName + "'";
        DbService db = new DbService();
        DataSet ds = db.GetDataSetByQuery(query);
        Volunteer v = new Volunteer();
        try
        {
            DataRow dr = ds.Tables[0].Rows[0];

            v.DisplayName = dr["DisplayName"].ToString();
        }
        catch (Exception)
        {

        }

        return v;
    }


    public Volunteer getVolunteerByMobile(string mobile)
    {
        DbService db = new DbService();
        string query = "select * from VolunteerView where cellPhone = '" + mobile + "'";
        DataSet ds = db.GetDataSetByQuery(query);
        Volunteer v = new Volunteer();
        foreach (DataRow dr in ds.Tables[0].Rows)
        {
            v.Id = int.Parse(dr["Id"].ToString());
            v.DisplayName = dr["DisplayName"].ToString();
            v.FirstNameA = dr["FirstNameA"].ToString();
            v.FirstNameH = dr["FirstNameH"].ToString();
            v.LastNameH = dr["LastNameH"].ToString();
            v.LastNameA = dr["LastNameA"].ToString();
            v.CellPhone = dr["CellPhone"].ToString();
            v.CellPhone2 = dr["CellPhone2"].ToString();
            v.HomePhone = dr["HomePhone"].ToString();
            v.City = dr["City"].ToString();
            v.Address = dr["Address"].ToString();
            v.Email = dr["Email"].ToString();
            v.Birthdate = dr["BirthDate"].ToString();
            v.JoinDate = dr["JoinDate"].ToString();
            v.Status = dr["IsActive"].ToString();
            v.Gender = dr["Gender"].ToString();
            v.KnowArabic = dr["KnowsArabic"].ToString();
            //v.PreferRoute1 = dr["preferRoute1"].ToString();
            //v.PreferRoute2 = dr["preferRoute2"].ToString();
            //v.PreferRoute3 = dr["preferRoute3"].ToString();
            //v.Day1 = dr["preferDay1"].ToString();
            //v.Day2 = dr["preferDay2"].ToString();
            //v.Day3 = dr["preferDay3"].ToString();
            //v.Hour1 = dr["preferHour1"].ToString();
            //v.Hour2 = dr["preferHour2"].ToString();
            //v.Hour3 = dr["preferHour3"].ToString();
            v.TypeVol = dr["Type"].ToString();
        }
        return v;

    }



    public string JoinDate
    {
        get
        {
            return joinDate;
        }

        set
        {
            joinDate = value;
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

    public string KnowArabic
    {
        get
        {
            return knowArabic;
        }

        set
        {
            knowArabic = value;
        }
    }

    public Volunteer()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    //public Volunteer(string _firstNameH, string __firstNameA, string _lastNameH, string _lastNameA, string _cellPhone, string _cellPhone2, string _homePhone,
    //    string _city, string _address, string _email, string _birthdate, string _joindate, string _status, string _gender, string _knowArabic,
    //    string _preferRoute1, string _preferRoute2, string _preferRoute3, string _day1, string _day2, string _day3,
    //    string _hour1, string _hour2, string _hour3, string _typeVol)
    //{
    //    FirstNameH = _firstNameH;
    //    FirstNameA = __firstNameA;
    //    LastNameH = _lastNameH;
    //    LastNameA = _lastNameA;
    //    CellPhone = _cellPhone;
    //    CellPhone2 = _cellPhone2;
    //    HomePhone = _homePhone;
    //    City = _city;
    //    Address = _address;
    //    Email = _email;
    //    Birthdate = _birthdate;
    //    JoinDate = _joindate;
    //    Status = _status;
    //    Gender = _gender;
    //    //PreferRoute1 = _preferRoute1;
    //    //PreferRoute2 = _preferRoute2;
    //    //PreferRoute3 = _preferRoute3;
    //    //Day1 = _day1;
    //    //Hour1 = _hour1;
    //    //Day2 = _day2;
    //    //Hour2 = _hour2;
    //    //Day3 = _day3;
    //    //Hour3 = _hour3;
    //    TypeVol = _typeVol;
    //    KnowArabic = _knowArabic;
    //}

    //public Volunteer(string _displayName, string _firstNameA, string _firstNameH, string _lastNameH, string _lastNameA,
    //string _cellPhone, string _cellPhone2, string _homePhone, string _city, string _street, string _typeVol, string _email, string _preferDay1,
    //string _preferHour1, string _preferDay2, string _preferHour2, string _preferDay3, string _preferHour3, string _preferRoute1, string _preferRoute2,
    //string _preferRoute3, string _joinDate, string _statusVolunteer, string _knowArabic, string _birthdate, string _gender)
    //{
    //    DisplayName = _displayName;
    //    FirstNameH = _firstNameH;
    //    FirstNameA = _firstNameA;
    //    LastNameH = _lastNameH;
    //    LastNameA = _lastNameA;
    //    CellPhone = _cellPhone;
    //    CellPhone2 = _cellPhone2;
    //    HomePhone = _homePhone;
    //    City = _city;
    //    Address = _street;
    //    Email = _email;
    //    Birthdate = _birthdate;
    //    JoinDate = _joinDate;
    //    Status = _statusVolunteer;
    //    Gender = _gender;
    //    //PreferRoute1 = _preferRoute1;
    //    //PreferRoute2 = _preferRoute2;
    //    //PreferRoute3 = _preferRoute3;
    //    //Day1 = _preferDay1;
    //    //Hour1 = _preferHour1;
    //    //Day2 = _preferDay2;
    //    //Hour2 = _preferHour2;
    //    //Day3 = _preferDay3;
    //    //Hour3 = _preferHour3;
    //    TypeVol = _typeVol;
    //    KnowArabic = _knowArabic;
    //}

    public Volunteer(string _firstNameH, string _lastNameH, string _cellPhone)
    {
        FirstNameH = _firstNameH;
        LastNameH = _lastNameH;
        CellPhone = _cellPhone;
    }
    public Volunteer(string _displayName)
    {
        DisplayName = _displayName;
    }
    public Volunteer(string _displayName, string _firstNameH, string _lastNameH, string _cellPhone)
    {
        DisplayName = _displayName;
        FirstNameH = _firstNameH;
        LastNameH = _lastNameH;
        CellPhone = _cellPhone;
    }



    public List<Volunteer> getVolunteersList(bool active)
    {
        #region DB functions
        string query = "select * from Volunteer v";
        if (active)
        {
            query += " where v.statusVolunteer = 'פעיל'";
        }

        query += " order by firstNameH";

        List<Volunteer> list = new List<Volunteer>();
        DbService db = new DbService();
        DataSet ds = db.GetDataSetByQuery(query);

        foreach (DataRow dr in ds.Tables[0].Rows)
        {
            Volunteer v = new Volunteer();
            v.DisplayName = dr["displayName"].ToString();
            v.FirstNameA = dr["firstNameA"].ToString();
            v.FirstNameH = dr["firstNameH"].ToString();
            v.LastNameH = dr["lastNameH"].ToString();
            v.LastNameA = dr["lastNameA"].ToString();
            v.CellPhone = dr["cellPhone"].ToString();
            v.CellPhone2 = dr["cellPhone2"].ToString();
            v.HomePhone = dr["homePhone"].ToString();
            v.City = dr["city"].ToString();
            v.Address = dr["street"].ToString();
            v.TypeVol = dr["typeVol"].ToString();
            v.Email = dr["email"].ToString();
            v.Day1 = dr["preferDay1"].ToString();
            v.Hour1 = dr["preferHour1"].ToString();
            v.Day2 = dr["preferDay2"].ToString();
            v.Hour2 = dr["preferHour2"].ToString();
            v.Day3 = dr["preferDay3"].ToString();
            v.Hour3 = dr["preferHour3"].ToString();
            v.PreferRoute1 = dr["preferRoute1"].ToString();
            v.preferRoute2 = dr["preferRoute2"].ToString();
            v.PreferRoute3 = dr["preferRoute3"].ToString();
            v.JoinDate = dr["joinDate"].ToString();
            v.Status = dr["statusVolunteer"].ToString();
            v.KnowArabic = dr["knowArabic"].ToString();
            v.Birthdate = dr["birthdate"].ToString();
            v.Gender = dr["gender"].ToString();


            list.Add(v);
        }
        #endregion

        return list;
    }

    public Volunteer getVolunteer()
    {
        #region DB functions
        string query = "select displayName, firstNameA, firstNameH, lastNameH, lastNameA, cellPhone, cellPhone2, homePhone, city, street, email, birthdate, joinDate, statusVolunteer, gender, knowArabic, preferRoute1, preferRoute2, preferRoute3, preferDay1, preferDay2, preferDay3, preferHour1, preferHour2, preferHour3, typeVol from Volunteer where displayName ='" + displayName + "'";
        Volunteer v = new Volunteer();
        DbService db = new DbService();
        DataSet ds = db.GetDataSetByQuery(query);

        foreach (DataRow dr in ds.Tables[0].Rows)
        {
            v.DisplayName = dr["displayName"].ToString();
            v.FirstNameA = dr["firstNameA"].ToString();
            v.FirstNameH = dr["firstNameH"].ToString();
            v.LastNameH = dr["lastNameH"].ToString();
            v.LastNameA = dr["lastNameA"].ToString();
            v.CellPhone = dr["cellPhone"].ToString();
            v.CellPhone2 = dr["cellPhone2"].ToString();
            v.HomePhone = dr["homePhone"].ToString();
            v.City = dr["city"].ToString();
            v.Address = dr["street"].ToString();
            v.Email = dr["email"].ToString();

            v.Birthdate = dr["birthdate"].ToString();
            v.JoinDate = dr["joinDate"].ToString();
            v.Status = dr["statusVolunteer"].ToString();
            v.Gender = dr["gender"].ToString();
            v.KnowArabic = dr["knowArabic"].ToString();
            v.PreferRoute1 = dr["preferRoute1"].ToString();
            v.PreferRoute2 = dr["preferRoute2"].ToString();
            v.PreferRoute3 = dr["preferRoute3"].ToString();
            v.Day1 = dr["preferDay1"].ToString();
            v.Day2 = dr["preferDay2"].ToString();
            v.Day3 = dr["preferDay3"].ToString();
            v.Hour1 = dr["preferHour1"].ToString();
            v.Hour2 = dr["preferHour2"].ToString();
            v.Hour3 = dr["preferHour3"].ToString();
            v.TypeVol = dr["typeVol"].ToString();

        }
        #endregion

        return v;
    }


    public void setVolunteer(string func)
    {
        DbService db = new DbService();
        string query = "";
        if (func == "edit")
        {
            query = "UPDATE Volunteer SET displayName = '" + DisplayName + "', firstNameH = '" + FirstNameH + "', firstNameA = '" + FirstNameA + "', lastNameH = '" + LastNameH + "', lastNameA = '" + LastNameA + "', cellPhone = '" + CellPhone + "', cellPhone2 = '" + CellPhone2 +
            "', homePhone = '" + HomePhone + "', city = '" + City + "', street = '" + Address + "', email = '" + Email + "', birthdate = '" + Birthdate + "', joinDate = '" + JoinDate + "', statusVolunteer = '" + Status + "', gender = '" + Gender + "', knowArabic = '" + KnowArabic +
            "', preferRoute1 = '" + PreferRoute1 + "', preferRoute2 = '" + PreferRoute2 + "', preferRoute3 = '" + PreferRoute3 + "', preferDay1 = '" + Day1 + "', preferDay2 = '" + Day2 +
            "', preferDay3 = '" + Day3 + "', preferHour1 = '" + Hour1 + "', preferHour2 = '" + Hour2 + "', preferHour3 = '" + Hour3 + "', typeVol = '" + TypeVol + "' WHERE displayName = '" + DisplayName + "'";
        }
        else if (func == "new")
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Values('{0}', '{1}', '{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}', '{11}', '{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}','{22}','{23}','{24}','{25}')",
                DisplayName, FirstNameH, FirstNameA, LastNameH, LastNameA,
                CellPhone, CellPhone2, HomePhone, City, Address,
                Email, Birthdate, JoinDate, Status, Gender,
                KnowArabic, PreferRoute1, PreferRoute2, PreferRoute3, Day1,
                Day2, Day3, Hour1, Hour2, Hour3, TypeVol);
            String prefix = "INSERT INTO Volunteer " + "(displayName, firstNameH,firstNameA, lastNameH,lastNameA, cellPhone,cellPhone2,homePhone,city,street, email,birthdate ,joinDate ,statusVolunteer,gender,knowArabic,preferRoute1,preferRoute2,preferRoute3,preferDay1,preferDay2,preferDay3,preferHour1,preferHour2,preferHour3,typeVol)";
            query = prefix + sb.ToString();
            //query = "insert into Customers values ('" + CustomerName + "','" + CustomerContactName + "','" + AccountID + "','Y','" + Phone1 + "','" + Phone2 + "','" + Email + "'," + PaymentType.PaymentTypeID + ",'" + Comments + "'," + PreferedDrivers.DriverID + ", '" + RegistrationNumber + "', '" + BillingAddress + "')";
        }
        db.ExecuteQuery(query);
    }

    public void deactivateCustomer(string active)
    {
        DbService db = new DbService();
        db.ExecuteQuery("UPDATE Volunteer SET statusVolunteer='" + active + "' WHERE displayName='" + DisplayName + "'");
    }
}