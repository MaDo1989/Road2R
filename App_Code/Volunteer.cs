using System;
using System.Collections.Generic;
using System.Configuration;
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
    string device;
    bool isAssistant;//האם עוזר?
    string englishName;//שם באנגלית
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
    int id;
    string regId;
    string lastModified;
    string volunteerIdentity;
    bool? knowsArabic;

    string englishFN;
    string englishLN;
    bool? isDriving;
    string howCanHelp;
    string feedback;
    string birthDate;
    bool? newsLetter;
    string refered;
    string roleInR2R;
    string joinYear;

    public List<RideStatus> Statusim { get; set; }

    public class RideStatus
    {
        string name;
        int id;

        public int Id { get; set; }
        public string Name { get; set; }
    }

    public string RegId { get; set; }

    public string Remarks { get; set; }

    //public DateTime BirthDate { get; set; }

    public DateTime JoinDate { get; set; }

    //public bool KnowsArabic { get; set; }

    public bool IsActive { get; set; }

    public List<string> PrefArea { get; set; }

    public List<string> PrefLocation { get; set; }

    public List<string[]> PrefTime { get; set; }

    public int AvailableSeats { get; set; }

    public string UserName { get; set; }

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

    //public string Birthdate
    //{
    //    get
    //    {
    //        return birthdate;
    //    }

    //    set
    //    {
    //        birthdate = value;
    //    }
    //}

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
    public int setVolunteerPrefs(int id, List<string> PrefLocation, List<string> PrefArea, List<string> PrefTime, int AvailableSeats)
    {
        string query = "";
        int res = 0;
        DbService db;
        SqlCommand cmd;


        //Delete previous preferences in DB
        db = new DbService();
        query = "delete from PreferedDay_Volunteer where VolunteerId=" + id + "; ";
        query += "delete from PreferredArea_Volunteer where VolunteerId = " + id + "; ";
        query += "delete from PreferredLocation_Volunteer where VolunteerId = " + id + "; ";
        db.ExecuteQuery(query);

        //foreach (string location in PrefLocation) //insert Location Preferences to DB
        //{
        //    db = new DbService();
        //    cmd = new SqlCommand();
        //    cmd.CommandType = CommandType.Text;
        //    SqlParameter[] locationParams = new SqlParameter[2];
        //    locationParams[0] = cmd.Parameters.AddWithValue("@location", location);
        //    locationParams[1] = cmd.Parameters.AddWithValue("@Id", id);
        //    query = "insert into PreferredLocation_Volunteer (PreferredLocation,VolunteerId) values (@location,@id);";
        //    res += db.ExecuteQuery(query, cmd.CommandType, locationParams);
        //}

        foreach (string area in PrefArea) //insert Area Preferences to DB
        {
            db = new DbService();
            cmd = new SqlCommand();
            cmd.CommandType = CommandType.Text;
            SqlParameter[] AreaParams = new SqlParameter[2];
            AreaParams[0] = cmd.Parameters.AddWithValue("@area", area);
            AreaParams[1] = cmd.Parameters.AddWithValue("@Id", id);
            query = "insert into PreferredArea_Volunteer (PreferredArea,VolunteerId) values (@area,@id);";
            res += db.ExecuteQuery(query, cmd.CommandType, AreaParams);
        }

        foreach (string shift in PrefTime) //insert Day&Shift Preferences to DB
        {
            string day = shift.Substring(shift.Length - 1);
            string finalShift = shift.Substring(0, shift.Length - 1);
            if (finalShift == "morning")
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
            shiftParams[2] = cmd.Parameters.AddWithValue("@Id", id);
            query = "insert into PreferedDay_Volunteer (PreferedDayDayInWeek,VolunteerId,Shift) values (@day,@id,@shift);";
            res += db.ExecuteQuery(query, cmd.CommandType, shiftParams);
        }

        db = new DbService();
        cmd = new SqlCommand();
        query = "update Volunteer set AvailableSeats=" + AvailableSeats + " where Id=" + id;
        res += db.ExecuteQuery(query);
        return res;
    }

    public Volunteer getVolunteerPrefs(int id)
    {
        PrefTime = new List<string[]>();
        PrefArea = new List<string>();
        PrefLocation = new List<string>();

        //Get Preferred Day & Shift for Volunteer
        string query = "select PreferedDayDayInWeek,Shift from PreferedDay_Volunteer where VolunteerId=" + id;
        DbService db = new DbService();
        DataSet ds = db.GetDataSetByQuery(query);
        foreach (DataRow dr in ds.Tables[0].Rows)
        {
            string[] DayShift = new string[2];
            DayShift[0] = dr["PreferedDayDayInWeek"].ToString();
            DayShift[1] = dr["Shift"].ToString();
            PrefTime.Add(DayShift);
        }

        //Get Preferred Area for Volunteer
        query = "select PreferredArea from PreferredArea_Volunteer where VolunteerId=" + id;
        db = new DbService();
        ds = db.GetDataSetByQuery(query);
        foreach (DataRow dr in ds.Tables[0].Rows)
        {
            string area = dr["PreferredArea"].ToString();
            PrefArea.Add(area);
        }

        //Get Preferred Location for Volunteer
        //query = "select PreferredLocation from PreferredLocation_Volunteer where VolunteerId=" + id;
        //db = new DbService();
        //ds = db.GetDataSetByQuery(query);
        //foreach (DataRow dr in ds.Tables[0].Rows)
        //{
        //    string location = dr["PreferredLocation"].ToString();
        //    PrefLocation.Add(location);
        //}

        //Get AvailabeSeats for Volunteer
        query = "select AvailableSeats from Volunteer where Id=" + id;
        db = new DbService();
        try
        {
            AvailableSeats = int.Parse(db.GetObjectScalarByQuery(query).ToString());

        }
        catch (Exception)
        {

        }

        return this;
    }



    internal List<Volunteer> getCoorList()
    {
        string query = "select * from VolunteerTypeView where VolunTypeType=N'רכז' or VolunTypeType=N'מנהל' and IsActive='true'";
        DbService db = new DbService();
        DataSet ds = db.GetDataSetByQuery(query);
        List<Volunteer> vl = new List<Volunteer>();
        foreach (DataRow dr in ds.Tables[0].Rows)
        {
            Volunteer v = new Volunteer();
            v.DisplayName = dr["DisplayName"].ToString();
            v.CellPhone = dr["CellPhone"].ToString();
            v.TypeVol = dr["VolunTypeType"].ToString();
            v.UserName = dr["UserName"].ToString();
            vl.Add(v);
        }
        return vl;
    }
    public List<Volunteer> getCoordinatorsList()
    {
        string query = "select * from VolunteerTypeView where VolunTypeType=N'רכז' and IsActive='true'";
        DbService db = new DbService();
        DataSet ds = db.GetDataSetByQuery(query);
        List<Volunteer> vl = new List<Volunteer>();
        foreach (DataRow dr in ds.Tables[0].Rows)
        {
            Volunteer v = new Volunteer();
            v.DisplayName = dr["DisplayName"].ToString();
            v.CellPhone = dr["CellPhone"].ToString();
            v.TypeVol = dr["VolunTypeType"].ToString();
            v.UserName = dr["UserName"].ToString();
            v.EnglishName = dr["EnglishName"].ToString();
            vl.Add(v);
        }
        return vl;
    }

    public List<string> getVolunteerTypes()
    {
        List<string> tl = new List<string>();
        string query = "select * from VolunType";
        DbService db = new DbService();
        DataSet ds = db.GetDataSetByQuery(query);
        foreach (DataRow dr in ds.Tables[0].Rows)
        {
            tl.Add(dr["Type"].ToString());
        }
        return tl;
    }

    public Volunteer getCoor(string userName)
    {
        string query = "select * from Volunteer where UserName=N'" + userName + "'";
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
    public Volunteer getVolunteerByCellphone(string cell)
    {
        DbService db = new DbService();
        cell = cell.Replace("-", "");
        string query = "select * from VolunteerTypeView where CellPhone = '" + cell + "'";
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
            //v.CellPhone = v.CellPhone.Replace("-", "");
            v.CellPhone2 = dr["CellPhone2"].ToString();
            v.HomePhone = dr["HomePhone"].ToString();
            v.City = dr["CityCityName"].ToString();
            v.Address = dr["Address"].ToString();
            v.Email = dr["Email"].ToString();
            v.RegId = dr["pnRegId"].ToString();
            //if (dr["BirthDate"].ToString() != "")
            //{
            //    v.BirthDate = Convert.ToDateTime(dr["BirthDate"].ToString());
            //}
            // v.BirthDate = Convert.ToDateTime(dr["BirthDate"].ToString());
            string date = dr["JoinDate"].ToString();
            if (date == "")
            {

            }
            else v.JoinDate = Convert.ToDateTime(dr["JoinDate"].ToString());
            bool ac = false;
            if (dr["IsActive"].ToString().ToLower() == "true")
            {
                ac = true;
            }
            v.IsActive = ac;
            bool arabic = false;
            if (dr["KnowsArabic"].ToString().ToLower() == "true")
            {
                arabic = true;
            }
            v.KnowsArabic = arabic;
            try
            {
                v.AvailableSeats = int.Parse(dr["AvailableSeats"].ToString());
            }
            catch (Exception)
            {

            }
            v.TypeVol = dr["VolunTypeType"].ToString();
            Volunteer v2 = new Volunteer();
            v2 = v2.getVolunteerPrefs(v.Id);
            v.PrefArea = v2.PrefArea;
            v.PrefTime = v2.PrefTime;
            v.PrefLocation = v2.PrefLocation;

            DbService db2 = new DbService();
            string query2 = "select * from status where StatusId > 99 and StatusId < 1000";
            DataSet ds2 = db2.GetDataSetByQuery(query2);
            v.Statusim = new List<RideStatus>();
            foreach (DataRow dr2 in ds2.Tables[0].Rows)
            {
                var stat = new RideStatus();
                stat.Id = int.Parse(dr2.ItemArray[1].ToString());
                stat.Name = dr2.ItemArray[0].ToString();
                v.Statusim.Add(stat);
            }
        }
        return v;

    }

    public Volunteer getVolunteerByMobile(string mobile)
    {
        Volunteer v = new Volunteer();
        int Id = -1;
        DbService db = new DbService();
        string query = "select Id,DisplayName from VolunteerTypeView where CellPhone = '" + mobile + "'";
        DataSet ds = db.GetDataSetByQuery(query);
        foreach (DataRow dr in ds.Tables[0].Rows)
        {
            v.Id = int.Parse(dr["Id"].ToString());
            v.displayName = dr["DisplayName"].ToString();
        }
        return v;
    }

    public Volunteer getVolunteerByMobile(string mobile, string regId, string device)
    {
        DbService db = new DbService();
        mobile = mobile.Replace("-", "");
        string query = "select * from VolunteerTypeView where CellPhone = '" + mobile + "'";
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
            //v.CellPhone = v.CellPhone.Replace("-", "");
            v.CellPhone2 = dr["CellPhone2"].ToString();
            v.HomePhone = dr["HomePhone"].ToString();
            v.City = dr["CityCityName"].ToString();
            v.Address = dr["Address"].ToString();
            v.Email = dr["Email"].ToString();
            //if (dr["BirthDate"].ToString() != "")
            //{
            //    v.BirthDate = Convert.ToDateTime(dr["BirthDate"].ToString());
            //}
            // v.BirthDate = Convert.ToDateTime(dr["BirthDate"].ToString());
            string date = dr["JoinDate"].ToString();
            if (date == "")
            {

            }
            else v.JoinDate = Convert.ToDateTime(dr["JoinDate"].ToString());
            bool ac = false;
            if (dr["IsActive"].ToString().ToLower() == "true")
            {
                ac = true;
            }
            v.IsActive = ac;
            bool arabic = false;
            if (dr["KnowsArabic"].ToString().ToLower() == "true")
            {
                arabic = true;
            }
            v.KnowsArabic = arabic;
            try
            {
                v.AvailableSeats = int.Parse(dr["AvailableSeats"].ToString());
            }
            catch (Exception)
            {

            }
            v.TypeVol = dr["VolunTypeType"].ToString();
            Volunteer v2 = new Volunteer();
            v2 = v2.getVolunteerPrefs(v.Id);
            v.PrefArea = v2.PrefArea;
            v.PrefTime = v2.PrefTime;
            v.PrefLocation = v2.PrefLocation;

            DbService db2 = new DbService();
            string query2 = "select * from status where StatusId > 99 and StatusId < 1000";
            DataSet ds2 = db2.GetDataSetByQuery(query2);
            v.Statusim = new List<RideStatus>();
            foreach (DataRow dr2 in ds2.Tables[0].Rows)
            {
                var stat = new RideStatus();
                stat.Id = int.Parse(dr2.ItemArray[1].ToString());
                stat.Name = dr2.ItemArray[0].ToString();
                v.Statusim.Add(stat);
            }
        }

        //dont update reg id and device type if user is rakaz on a mission
        if (regId != "i_am_spy")
        {
            //update reg id
            db = new DbService();
            var updateRegid = "update Volunteer set pnRegId=@REGID where Id=@ID";

            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.Text;

            SqlParameter[] cmdParams = new SqlParameter[2];
            cmdParams[0] = cmd.Parameters.AddWithValue("@REGID", regId);
            cmdParams[1] = cmd.Parameters.AddWithValue("@ID", v.Id);

            int result = db.ExecuteQuery(updateRegid, cmd.CommandType, cmdParams);

            //update device
            db = new DbService();
            var updateDevice = "update Volunteer set device=N'" + device + "' where Id=@ID";

            SqlCommand cmd1 = new SqlCommand();
            cmd1.CommandType = CommandType.Text;

            SqlParameter[] cmdParams1 = new SqlParameter[1];
            cmdParams1[0] = cmd1.Parameters.AddWithValue("@ID", v.Id);

            int result1 = db.ExecuteQuery(updateDevice, cmd1.CommandType, cmdParams1);
        }



        return v;

    }


    //public string JoinDate
    //{
    //    get
    //    {
    //        return joinDate;
    //    }

    //    set
    //    {
    //        joinDate = value;
    //    }
    //}

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

    public string Device
    {
        get
        {
            return device;
        }

        set
        {
            device = value;
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

    public bool IsAssistant
    {
        get
        {
            return isAssistant;
        }

        set
        {
            isAssistant = value;
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

    public string VolunteerIdentity
    {
        get
        {
            return volunteerIdentity;
        }

        set
        {
            volunteerIdentity = value;
        }
    }

    public bool? KnowsArabic
    {
        get
        {
            return knowsArabic;
        }

        set
        {
            knowsArabic = value;
        }
    }

    public string EnglishFN
    {
        get
        {
            return englishFN;
        }

        set
        {
            englishFN = value;
        }
    }

    public string EnglishLN
    {
        get
        {
            return englishLN;
        }

        set
        {
            englishLN = value;
        }
    }

    public bool? IsDriving
    {
        get
        {
            return isDriving;
        }

        set
        {
            isDriving = value;
        }
    }

    public string HowCanHelp
    {
        get
        {
            return howCanHelp;
        }

        set
        {
            howCanHelp = value;
        }
    }

    public string Feedback
    {
        get
        {
            return feedback;
        }

        set
        {
            feedback = value;
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

    public bool? NewsLetter
    {
        get
        {
            return newsLetter;
        }

        set
        {
            newsLetter = value;
        }
    }

    public string Refered
    {
        get
        {
            return refered;
        }

        set
        {
            refered = value;
        }
    }

    public string RoleInR2R
    {
        get
        {
            return roleInR2R;
        }

        set
        {
            roleInR2R = value;
        }
    }

    public string JoinYear
    {
        get
        {
            return joinYear;
        }

        set
        {
            joinYear = value;
        }
    }


    //public string KnowsArabic
    //{
    //    get
    //    {
    //        return KnowsArabic;
    //    }

    //    set
    //    {
    //        KnowsArabic = value;
    //    }
    //}

    public Volunteer()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    //public Volunteer(string _firstNameH, string __firstNameA, string _lastNameH, string _lastNameA, string _cellPhone, string _cellPhone2, string _homePhone,
    //    string _city, string _address, string _email, string _birthdate, string _joindate, string _status, string _gender, string _KnowsArabic,
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
    //    KnowsArabic = _KnowsArabic;
    //}

    //public Volunteer(string _displayName, string _firstNameA, string _firstNameH, string _lastNameH, string _lastNameA,
    //string _cellPhone, string _cellPhone2, string _homePhone, string _city, string _street, string _typeVol, string _email, string _preferDay1,
    //string _preferHour1, string _preferDay2, string _preferHour2, string _preferDay3, string _preferHour3, string _preferRoute1, string _preferRoute2,
    //string _preferRoute3, string _joinDate, string _statusVolunteer, string _KnowsArabic, string _birthdate, string _gender)
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
    //    KnowsArabic = _KnowsArabic;
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
        string query = "select * from VolunteerTypeView";
        if (active)
        {
            query += " where IsActive = 'True'";
        }

        query += " order by firstNameH";

        List<Volunteer> list = new List<Volunteer>();
        DbService db = new DbService();
        DataSet ds = db.GetDataSetByQuery(query);

        foreach (DataRow dr in ds.Tables[0].Rows)
        {
            Volunteer v = new Volunteer();
            v.Id = int.Parse(dr["Id"].ToString());
            v.DisplayName = dr["DisplayName"].ToString();
            v.FirstNameA = dr["FirstNameA"].ToString();
            v.FirstNameH = dr["FirstNameH"].ToString();
            v.LastNameH = dr["LastNameH"].ToString();
            v.LastNameA = dr["LastNameA"].ToString();
            v.CellPhone = dr["CellPhone"].ToString();
            v.CellPhone2 = dr["CellPhone2"].ToString();
            v.HomePhone = dr["HomePhone"].ToString();
            v.Remarks = dr["Remarks"].ToString();
            v.City = dr["CityCityName"].ToString();
            v.Address = dr["Address"].ToString();
            v.TypeVol = dr["VolunTypeType"].ToString();
            v.Email = dr["Email"].ToString();
            v.Device = dr["device"].ToString();
            //v.Day1 = dr["preferDay1"].ToString();
            //v.Hour1 = dr["preferHour1"].ToString();
            //v.Day2 = dr["preferDay2"].ToString();
            //v.Hour2 = dr["preferHour2"].ToString();
            //v.Day3 = dr["preferDay3"].ToString();
            //v.Hour3 = dr["preferHour3"].ToString();
            //v.PreferRoute1 = dr["preferRoute1"].ToString();
            //v.preferRoute2 = dr["preferRoute2"].ToString();
            //v.PreferRoute3 = dr["preferRoute3"].ToString();
            string date = dr["JoinDate"].ToString();
            bool isAssistant = Convert.ToBoolean(dr["isAssistant"].ToString());
            if (date == "")
            {

            }
            else v.JoinDate = Convert.ToDateTime(dr["JoinDate"].ToString());
            bool ac = false;
            if (dr["IsActive"].ToString().ToLower() == "true")
            {
                ac = true;
            }
            v.IsActive = ac;
            bool arabic = false;
            if (dr["KnowsArabic"].ToString().ToLower() == "true")
            {
                arabic = true;
            }
            v.KnowsArabic = arabic;
            v.Gender = dr["Gender"].ToString();
            v.RegId = dr["pnRegId"].ToString();

            v.EnglishName = dr["englishName"].ToString();
            v.LastModified = dr["lastModified"].ToString();

            list.Add(v);
        }
        #endregion

        return list;
    }

    public Volunteer getVolunteer()
    {
        #region DB functions
        SqlCommand cmd = new SqlCommand();
        cmd.CommandType = CommandType.Text;
        SqlParameter[] cmdParams = new SqlParameter[1];
        cmdParams[0] = cmd.Parameters.AddWithValue("name", displayName);
        string query = "select * from VolunteerTypeView where displayName=@name";
        Volunteer v = new Volunteer();
        DbService db = new DbService();
        DataSet ds = db.GetDataSetByQuery(query, true, cmd.CommandType, cmdParams);
        DataRow dr = ds.Tables[0].Rows[0];

        v.Id = int.Parse(dr["Id"].ToString());
        v.Remarks = dr["Remarks"].ToString();
        v.DisplayName = dr["DisplayName"].ToString();
        v.FirstNameA = dr["FirstNameA"].ToString();
        v.FirstNameH = dr["FirstNameH"].ToString();
        v.LastNameH = dr["LastNameH"].ToString();
        v.LastNameA = dr["LastNameA"].ToString();
        v.CellPhone = dr["CellPhone"].ToString();
        v.CellPhone2 = dr["CellPhone2"].ToString();
        v.HomePhone = dr["HomePhone"].ToString();
        v.City = dr["CityCityName"].ToString();
        v.Address = dr["Address"].ToString();
        v.Email = dr["Email"].ToString();
        v.EnglishName = dr["EnglishName"].ToString();
        v.VolunteerIdentity = dr["VolunteerIdentity"].ToString();
        string date = dr["JoinDate"].ToString();
        bool isAssistant = Convert.ToBoolean(dr["isAssistant"].ToString());
        if (date == "")
        {

        }
        else v.JoinDate = Convert.ToDateTime(dr["JoinDate"].ToString());
        bool ac = false;
        if (dr["IsActive"].ToString().ToLower() == "true")
        {
            ac = true;
        }
        v.IsActive = ac;
        bool arabic = false;
        if (dr["KnowsArabic"].ToString().ToLower() == "true")
        {
            arabic = true;
        }
        v.KnowsArabic = arabic;
        // v.BirthDate = Convert.ToDateTime(dr["BirthDate"].ToString());
        v.Gender = dr["Gender"].ToString();
        try
        {
            v.AvailableSeats = int.Parse(dr["AvailableSeats"].ToString());
        }
        catch (Exception)
        {

        }
        v.TypeVol = dr["VolunTypeType"].ToString();
        v.EnglishFN = dr["EnglishFN"].ToString();
        v.EnglishLN = dr["EnglishLN"].ToString();
        string bdate = dr["BirthDate"].ToString();
        if (bdate == "")
        {

        }
        else v.BirthDate = Convert.ToDateTime(dr["BirthDate"]).ToString("yyyy-MM-dd");
        if (dr["isDriving"].ToString() != "")
        {
            v.IsDriving = Convert.ToBoolean(dr["isDriving"].ToString());
        }
        else
        {
            v.IsDriving = null;
        }
        v.HowCanHelp = dr["HowCanHelp"].ToString();
        v.Feedback = dr["Feedback"].ToString();
        if (dr["newsLetter"].ToString() != "")
        {
            v.NewsLetter = Convert.ToBoolean(dr["newsLetter"].ToString());
        }
        else
        {
            v.NewsLetter = null;
        }
        v.Refered = dr["refered"].ToString();
        v.RoleInR2R = dr["roleInR2R"].ToString();

        Volunteer temp = new Volunteer();
        temp = getVolunteerPrefs(v.Id);

        v.PrefArea = temp.PrefArea;
        v.PrefTime = temp.PrefTime;
        v.PrefLocation = temp.PrefLocation;
        #endregion

        return v;
    }

   
    public Volunteer getVolunteerByDisplayName(string name)
    {
        #region DB functions
        SqlCommand cmd = new SqlCommand();
        cmd.CommandType = CommandType.Text;
        SqlParameter[] cmdParams = new SqlParameter[1];
        cmdParams[0] = cmd.Parameters.AddWithValue("name", name);
        string query = "select * from VolunteerTypeView where displayName=@name";
        Volunteer v = new Volunteer();
        DbService db = new DbService();
        DataSet ds = db.GetDataSetByQuery(query, true, cmd.CommandType, cmdParams);
        DataRow dr = ds.Tables[0].Rows[0];

        v.Id = int.Parse(dr["Id"].ToString());
        v.Remarks = dr["Remarks"].ToString();
        v.DisplayName = dr["DisplayName"].ToString();
        v.FirstNameA = dr["FirstNameA"].ToString();
        v.FirstNameH = dr["FirstNameH"].ToString();
        v.LastNameH = dr["LastNameH"].ToString();
        v.LastNameA = dr["LastNameA"].ToString();
        v.CellPhone = dr["CellPhone"].ToString();
        v.CellPhone2 = dr["CellPhone2"].ToString();
        v.HomePhone = dr["HomePhone"].ToString();
        v.City = dr["CityCityName"].ToString();
        v.RegId = dr["pnRegId"].ToString();
        v.Address = dr["Address"].ToString();
        v.Email = dr["Email"].ToString();
        v.VolunteerIdentity = dr["VolunteerIdentity"].ToString();

        //v.IsAssistant= Convert.ToBoolean(dr["isAssistant"].ToString());

        if (dr["device"] != null)
        {
            v.Device = dr["device"].ToString();
        }
        string date = dr["JoinDate"].ToString();
        if (date == "")
        {

        }
        else v.JoinDate = Convert.ToDateTime(dr["JoinDate"].ToString());
        bool ac = false;
        if (dr["IsActive"].ToString().ToLower() == "true")
        {
            ac = true;
        }
        v.IsActive = ac;
        bool arabic = false;
        if (dr["KnowsArabic"].ToString().ToLower() == "true")
        {
            arabic = true;
        }
        v.KnowsArabic = arabic;
        // v.BirthDate = Convert.ToDateTime(dr["BirthDate"].ToString());
        v.Gender = dr["Gender"].ToString();
        try
        {
            v.AvailableSeats = int.Parse(dr["AvailableSeats"].ToString());
        }
        catch (Exception)
        {

        }
        v.TypeVol = dr["VolunTypeType"].ToString();

        Volunteer temp = new Volunteer();
        temp = getVolunteerPrefs(v.Id);

        v.PrefArea = temp.PrefArea;
        v.PrefTime = temp.PrefTime;
        v.PrefLocation = temp.PrefLocation;
        #endregion

        return v;
    }
    public List<string> getPrefArea(int id)
    {
        List<string> areas = new List<string>();
        DbService db = new DbService();
        //Get Preferred Area for Volunteer
        string query = "select PreferredArea from PreferredArea_Volunteer where VolunteerId=" + id;
        DataSet ds = db.GetDataSetByQuery(query);
        foreach (DataRow dr in ds.Tables[0].Rows)
        {
            string area = dr["PreferredArea"].ToString();
            areas.Add(area);
        }
        return areas;

    }
    public Volunteer getVolunteerByID(int id)
    {
        #region DB functions
        SqlCommand cmd = new SqlCommand();
        cmd.CommandType = CommandType.Text;
        SqlParameter[] cmdParams = new SqlParameter[1];
        cmdParams[0] = cmd.Parameters.AddWithValue("id", id);
        string query = "select * from VolunteerTypeView where ID=@id";
        Volunteer v = new Volunteer();
        DbService db = new DbService();
        DataSet ds = db.GetDataSetByQuery(query, true, cmd.CommandType, cmdParams);
        DataRow dr = ds.Tables[0].Rows[0];

        v.Id = int.Parse(dr["Id"].ToString());
        v.Remarks = dr["Remarks"].ToString();
        v.DisplayName = dr["DisplayName"].ToString();
        v.FirstNameA = dr["FirstNameA"].ToString();
        v.FirstNameH = dr["FirstNameH"].ToString();
        v.LastNameH = dr["LastNameH"].ToString();
        v.LastNameA = dr["LastNameA"].ToString();
        v.CellPhone = dr["CellPhone"].ToString();
        v.CellPhone2 = dr["CellPhone2"].ToString();
        v.HomePhone = dr["HomePhone"].ToString();
        v.City = dr["CityCityName"].ToString();
        v.Address = dr["Address"].ToString();
        v.Email = dr["Email"].ToString();
        v.IsAssistant = Convert.ToBoolean(dr["isAssistant"].ToString());
        string date = dr["JoinDate"].ToString();
        if (date == "")
        {

        }
        else v.JoinDate = Convert.ToDateTime(dr["JoinDate"].ToString());
        bool ac = false;
        if (dr["IsActive"].ToString().ToLower() == "true")
        {
            ac = true;
        }
        v.IsActive = ac;
        bool arabic = false;
        if (dr["KnowsArabic"].ToString().ToLower() == "true")
        {
            arabic = true;
        }
        v.KnowsArabic = arabic;
        // v.BirthDate = Convert.ToDateTime(dr["BirthDate"].ToString());
        v.Gender = dr["Gender"].ToString();
        try
        {
            v.AvailableSeats = int.Parse(dr["AvailableSeats"].ToString());
        }
        catch (Exception)
        {

        }
        v.TypeVol = dr["VolunTypeType"].ToString();

        Volunteer temp = new Volunteer();
        temp = getVolunteerPrefs(v.Id);

        v.PrefArea = temp.PrefArea;
        v.PrefTime = temp.PrefTime;
        v.PrefLocation = temp.PrefLocation;
        #endregion

        return v;
    }

    public string GetVolunteerRegById(int id)
    {
        string query = "select pnRegID from Volunteer where Id ='" + id + "'";
        DbService db = new DbService();
        return db.GetObjectScalarByQuery(query).ToString();
    }

    public string getDeviceByID(int id)
    {
        string query = "select device from Volunteer where Id ='" + id + "'";
        DbService db = new DbService();
        return db.GetObjectScalarByQuery(query).ToString();
    }

    public void setVolunteer(Volunteer v, string func)
    {
        int res = 0;
        DbService db = new DbService();
        SqlCommand cmd = new SqlCommand();
        cmd.CommandType = CommandType.Text;
        SqlParameter[] cmdParams = new SqlParameter[24];
        cmdParams[0] = cmd.Parameters.AddWithValue("@address", v.Address);
        cmdParams[1] = cmd.Parameters.AddWithValue("@cell", v.CellPhone);
        cmdParams[2] = cmd.Parameters.AddWithValue("@cell2", v.CellPhone2);
        cmdParams[3] = cmd.Parameters.AddWithValue("@city", v.City);
        cmdParams[4] = cmd.Parameters.AddWithValue("@email", v.Email);
        cmdParams[5] = cmd.Parameters.AddWithValue("@firstNameA", v.FirstNameA);
        cmdParams[6] = cmd.Parameters.AddWithValue("@firstNameH", v.FirstNameH);
        cmdParams[7] = cmd.Parameters.AddWithValue("@gender", v.Gender);
        //cmdParams[8] = cmd.Parameters.AddWithValue("@phone", v.HomePhone);
        cmdParams[8] = cmd.Parameters.AddWithValue("@IsActive", v.IsActive);
        cmdParams[9] = cmd.Parameters.AddWithValue("@jDate", v.JoinDate);
        cmdParams[10] = cmd.Parameters.AddWithValue("@knowsArabic", v.KnowsArabic);
        cmdParams[11] = cmd.Parameters.AddWithValue("@lastNameA", v.LastNameA);
        cmdParams[12] = cmd.Parameters.AddWithValue("@lastNameH", v.LastNameH);
        cmdParams[13] = cmd.Parameters.AddWithValue("@volType", v.TypeVol);
        cmdParams[14] = cmd.Parameters.AddWithValue("@remarks", v.Remarks);
        cmdParams[15] = cmd.Parameters.AddWithValue("@displayName", v.DisplayName);
        cmdParams[16] = cmd.Parameters.AddWithValue("@UserName", v.CellPhone);
        cmdParams[17] = cmd.Parameters.AddWithValue("@englishName", v.EnglishName);
        cmdParams[18] = cmd.Parameters.AddWithValue("@isAssistant", v.IsAssistant);
        cmdParams[19] = cmd.Parameters.AddWithValue("@volunteerIdentity", v.VolunteerIdentity);

        cmdParams[20] = cmd.Parameters.AddWithValue("@englishFN", v.EnglishFN);
        cmdParams[21] = cmd.Parameters.AddWithValue("@englishLN", v.EnglishLN);
        cmdParams[22] = cmd.Parameters.AddWithValue("@birthDate", v.BirthDate);
        cmdParams[23] = cmd.Parameters.AddWithValue("@isDriving", v.IsDriving);
        //cmdParams[24] = cmd.Parameters.AddWithValue("@howCanHelp", v.HowCanHelp);
        //cmdParams[25] = cmd.Parameters.AddWithValue("@feedback", v.Feedback);
        //cmdParams[24] = cmd.Parameters.AddWithValue("@newsLetter", v.NewsLetter);

        

        string newName = v.FirstNameH + " " + v.LastNameH;
        newName = newName.Replace("'", "''");
        //cmdParams[1] = cmd.Parameters.AddWithValue("@bDay", v.BirthDate);
        //, HomePhone=@phone
        string query = "";
        if (func == "edit")
        {
            string password = ConfigurationManager.AppSettings["password"];
            if (v.TypeVol == "רכז" || v.TypeVol == "מנהל" || v.IsAssistant)
            {
                query = "update Volunteer set Address=@address, CellPhone=@cell,";
                query += "CellPhone2=@cell2, CityCityName=@city, Email=@email, FirstNameA=@firstNameA, FirstNameH=@firstNameH, VolunteerIdentity=@volunteerIdentity, ";
                query += "Gender=@gender, IsActive=@IsActive, JoinDate=@jDate, KnowsArabic=@knowsArabic, LastNameA=@lastNameA, ";
                query += "EnglishFN=@englishFN, EnglishLN=@englishLN, BirthDate=@birthDate, IsDriving=@isDriving, ";
                query += "LastNameH=@lastNameH,UserName=@UserName,Password='" + password + "', Remarks=@remarks,EnglishName=@englishName,isAssistant=@isAssistant,lastModified=DATEADD(hour, 2, SYSDATETIME()) where DisplayName=@displayName"; //, BirthDate=@bDay

            }
            else
            {
                query = "update Volunteer set Address=@address, CellPhone=@cell,";
                query += "CellPhone2=@cell2, CityCityName=@city, Email=@email, FirstNameA=@firstNameA, FirstNameH=@firstNameH, VolunteerIdentity=@volunteerIdentity, ";
                query += "Gender=@gender, IsActive=@IsActive, JoinDate=@jDate, KnowsArabic=@knowsArabic, LastNameA=@lastNameA, ";
                query += "EnglishFN=@englishFN, EnglishLN=@englishLN, BirthDate=@birthDate, IsDriving=@isDriving, ";
                query += "LastNameH=@lastNameH, Remarks=@remarks,EnglishName=@englishName,isAssistant=@isAssistant,lastModified=DATEADD(hour, 2, SYSDATETIME()) where DisplayName=@displayName"; //, BirthDate=@bDay
            }
            res = db.ExecuteQuery(query, cmd.CommandType, cmdParams);

            if (res == 0)
            {
                throw new Exception();
            }
            db = new DbService();
            try
            {
                query = "select Id from Volunteer where DisplayName=N'" + newName + "'";
                Id = int.Parse(db.GetObjectScalarByQuery(query).ToString());
            }
            catch (Exception)
            {
                query = "select Id from Volunteer where DisplayName=N'" + v.DisplayName + "'";
                Id = int.Parse(db.GetObjectScalarByQuery(query).ToString());
            }


            db = new DbService();
            query = "update VolunType_Volunteer set VolunTypeType=@volType where VolunteerId=" + Id;
            res = db.ExecuteQuery(query, cmd.CommandType, cmdParams);
            if (res == 0)
            {
                throw new Exception();
            }
        }
        else if (func == "new")
        {

            try
            {
                if (v.TypeVol == "רכז" || v.TypeVol == "מנהל" || v.IsAssistant)
                {
                    string password = ConfigurationManager.AppSettings["password"];
                    query = "insert into Volunteer (Address, CellPhone, CellPhone2, CityCityName, Email, FirstNameA, FirstNameH, Gender, IsActive, JoinDate, KnowsArabic, LastNameA, LastNameH, Remarks,EnglishName,isAssistant,UserName,Password,lastModified,EnglishFN, EnglishLN, BirthDate, IsDriving)";
                    query += " values (@address,@cell,@cell2,@city,@email,@firstNameA,@firstNameH,@gender,@IsActive,@jDate,@knowsArabic,@lastNameA,@lastNameH,@remarks,@englishName,@isAssistant,@UserName,'" + password + "',DATEADD(hour, 2, SYSDATETIME()),@englishFN, @englishLN, @birthDate, @isDriving);SELECT SCOPE_IDENTITY();";
                    
                }
                else
                {
                    query = "insert into Volunteer (Address, CellPhone, CellPhone2, CityCityName, Email, FirstNameA, FirstNameH, Gender, IsActive, JoinDate, KnowsArabic, LastNameA, LastNameH, Remarks,EnglishName,isAssistant,lastModified,EnglishFN, EnglishLN, BirthDate, IsDriving)";
                    query += " values (@address,@cell,@cell2,@city,@email,@firstNameA,@firstNameH,@gender,@IsActive,@jDate,@knowsArabic,@lastNameA,@lastNameH,@remarks,@englishName,@isAssistant,DATEADD(hour, 2, SYSDATETIME()),@englishFN, @englishLN, @birthDate, @isDriving);SELECT SCOPE_IDENTITY();";
                }
                db = new DbService();
                Id = int.Parse(db.GetObjectScalarByQuery(query, cmd.CommandType, cmdParams).ToString());

                query = "insert into VolunType_Volunteer (VolunTypeType,VolunteerId) values (@volType," + Id + ")";
                db = new DbService();
                db.ExecuteQuery(query, cmd.CommandType, cmdParams);
            }
            catch (SqlException ex)
            {
                throw new Exception("phone already exists");
            }
            catch (Exception e)
            {
                throw e;
            }

        }

    }

    

    public void deactivateCustomer(string active)
    {
        DbService db = new DbService();
        db.ExecuteQuery("UPDATE Volunteer SET IsActive='" + active + "', lastModified=DATEADD(hour, 2, SYSDATETIME()) WHERE displayName=N'" + DisplayName + "'");

    }

    //check volunteer spaces in car with volunteer fullName(displayName)
    public int spaceInCar(int driverId)
    {
        string query = "select * from Volunteer where Id = " + driverId;
        DbService db = new DbService();
        try
        {
            DataSet ds = db.GetDataSetByQuery(query);
            return int.Parse(ds.Tables[0].Rows[0]["AvailableSeats"].ToString());
        }
        catch (Exception ex)
        {

            throw;
        }

    }
    public void setUserPassword(string userName, string newPassword)
    {
        int res = 0;
        DbService db = new DbService();
        SqlCommand cmd = new SqlCommand();
        cmd.CommandType = CommandType.Text;
        SqlParameter[] cmdParams = new SqlParameter[1];

        cmdParams[0] = cmd.Parameters.AddWithValue("@password", newPassword);

        string query;
        query = "update Volunteer set Password=@password where UserName=N'" + userName + "'";

        res = db.ExecuteQuery(query, cmd.CommandType, cmdParams);

        if (res == 0)
        {
            throw new Exception();
        }

    }

    public Volunteer getVolunteerData() // function for volunteer data review
    {
        #region DB functions
        SqlCommand cmd = new SqlCommand();
        cmd.CommandType = CommandType.Text;
        SqlParameter[] cmdParams = new SqlParameter[1];
        cmdParams[0] = cmd.Parameters.AddWithValue("name", DisplayName);
        //string query = "select * from VolunteerData where displayName=@name";
        string query = "select * from VolunteerWI where displayName=@name";
        Volunteer v = new Volunteer();
        DbService db = new DbService();
        DataSet ds = db.GetDataSetByQuery(query, true, cmd.CommandType, cmdParams);
        DataRow dr = ds.Tables[0].Rows[0];

        v.Id = int.Parse(dr["Id"].ToString());
        v.FirstNameH = dr["FirstNameH"].ToString();
        v.LastNameH = dr["LastNameH"].ToString();
        v.EnglishFN = dr["EnglishFN"].ToString();
        v.EnglishLN = dr["EnglishLN"].ToString();
        v.VolunteerIdentity = dr["VolunteerIdentity"].ToString();
        v.CellPhone = dr["CellPhone"].ToString();
        v.Gender = dr["Gender"].ToString();
        v.City = dr["CityCityName"].ToString();
        v.Email = dr["Email"].ToString();
        string date = dr["BirthDate"].ToString();
        if (date == "")
        {

        }
        else v.BirthDate = Convert.ToDateTime(dr["BirthDate"]).ToString("yyyy-MM-dd");
        if (dr["isDriving"].ToString() != "")
        {
            v.IsDriving = Convert.ToBoolean(dr["isDriving"].ToString());
        }
        else
        {
            v.IsDriving = null;
        }
        v.HowCanHelp = dr["HowCanHelp"].ToString();
        v.Feedback = dr["Feedback"].ToString();
        v.Remarks = dr["Remarks"].ToString();
        if (dr["newsLetter"].ToString() != "")
        {
            v.NewsLetter = Convert.ToBoolean(dr["newsLetter"].ToString());
        }
        else
        {
            v.NewsLetter = null;
        }
        //v.Refered = dr["refered"].ToString();
        v.RoleInR2R = dr["roleInR2R"].ToString();
        if (dr["knowsArabic"].ToString() != "")
        {
            v.KnowsArabic = Convert.ToBoolean(dr["knowsArabic"].ToString());
        }
        else
        {
            v.KnowsArabic = null;
        }
        v.joinYear = dr["joinYear"].ToString();

        #endregion

        return v;
    }


    public void setVolunteerData(Volunteer v, string username)
    {
        int res = 0;
        DbService db = new DbService();
        SqlCommand cmd = new SqlCommand();
        cmd.CommandType = CommandType.Text;
        SqlParameter[] cmdParams = new SqlParameter[20];

        cmdParams[0] = cmd.Parameters.AddWithValue("@firstNameH", v.FirstNameH);
        cmdParams[1] = cmd.Parameters.AddWithValue("@lastNameH", v.LastNameH);
        cmdParams[2] = cmd.Parameters.AddWithValue("@englishFN", v.EnglishFN);
        cmdParams[3] = cmd.Parameters.AddWithValue("@englishLN", v.EnglishLN);
        cmdParams[4] = cmd.Parameters.AddWithValue("@volunteerIdentity", v.VolunteerIdentity);
        cmdParams[5] = cmd.Parameters.AddWithValue("@cell", v.CellPhone);
        cmdParams[6] = cmd.Parameters.AddWithValue("@gender", v.Gender);
        cmdParams[7] = cmd.Parameters.AddWithValue("@city", v.City);
        cmdParams[8] = cmd.Parameters.AddWithValue("@email", v.Email);
        cmdParams[9] = cmd.Parameters.AddWithValue("@bDay", v.BirthDate);
        cmdParams[10] = cmd.Parameters.AddWithValue("@isDriving", v.IsDriving);
        cmdParams[11] = cmd.Parameters.AddWithValue("@howCanHelp", v.HowCanHelp);
        cmdParams[12] = cmd.Parameters.AddWithValue("@feedback", v.Feedback);
        cmdParams[13] = cmd.Parameters.AddWithValue("@remarks", v.Remarks);
        cmdParams[14] = cmd.Parameters.AddWithValue("@newsLetter", v.NewsLetter);
        cmdParams[15] = cmd.Parameters.AddWithValue("@knowsArabic", v.KnowsArabic);
        cmdParams[16] = cmd.Parameters.AddWithValue("@displayName", v.DisplayName);
        cmdParams[17] = cmd.Parameters.AddWithValue("@englishName", v.EnglishName);
        cmdParams[18] = cmd.Parameters.AddWithValue("@username", username);
        cmdParams[19] = cmd.Parameters.AddWithValue("@joinYear", v.JoinYear);

        string query = "";

        //query = "update VolunteerData set FirstNameH=@firstNameH, LastNameH=@lastNameH, EnglishFN=@englishFN, EnglishLN=@englishLN, ";
        query = "update VolunteerWI set FirstNameH=@firstNameH, LastNameH=@lastNameH, EnglishFN=@englishFN, EnglishLN=@englishLN, ";
        query += "VolunteerIdentity=@volunteerIdentity, CellPhone=@cell, Gender=@gender, CityCityName=@city, Email=@email, ";
        query += "BirthDate=@bDay, IsDriving=@isDriving, HowCanHelp=@howCanHelp, Feedback=@feedback, Remarks=@remarks, NewsLetter=@newsLetter, KnowsArabic=@knowsArabic,";
        query += "DisplayName=@displayName, EnglishName=@englishName, JoinYear=@joinYear, lastModified=DATEADD(hour, 2, SYSDATETIME()) where DisplayName=@username";
        try
        {
            res = db.ExecuteQuery(query, cmd.CommandType, cmdParams);
        }
        catch (SqlException ex)
        {
            throw new Exception("phone already exists");
        }
        if (res == 0)
        {
            throw new Exception();
        }
    }

    public Volunteer getVolunteerExtendedByMobile(string mobile)
    {
        Volunteer v = new Volunteer();

        DbService db = new DbService();
        //string query = "select Id,DisplayName from VolunteerData where CellPhone = '" + mobile + "'";
        string query = "select Id,DisplayName from VolunteerWI where CellPhone = '" + mobile + "'";
        DataSet ds = db.GetDataSetByQuery(query);
        foreach (DataRow dr in ds.Tables[0].Rows)
        {
            v.Id = int.Parse(dr["Id"].ToString());
            v.DisplayName = dr["DisplayName"].ToString();
        }
        return v;
    }
}