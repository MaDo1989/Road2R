using log4net;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Xml.Linq;

// <summary>
/// Summary description for Messege
/// </summary>

public class Message
{
    int msgID;
    int parentID;
    string type;
    string title;
    
    string msgContent;
    int ridePatID;
    string dateTime;
    int userID;
    string userNotes;
    bool isPush;
    bool isMail;
    bool isWhatsapp;
    string sender;

  

    private static readonly ILog Log =
            LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    public int MsgID
    {
        get
        {
            return msgID;
        }

        set
        {
            msgID = value;
        }
    }

    public int ParentID
    {
        get
        {
            return parentID;
        }

        set
        {
            parentID = value;
        }
    }

    public string Type
    {
        get
        {
            return type;
        }

        set
        {
            type = value;
        }
    }

    public string Title
    {
        get
        {
            return title;
        }

        set
        {
            title = value;
        }
    }



    public string MsgContent
    {
        get
        {
            return msgContent;
        }

        set
        {
            msgContent = value;
        }
    }

    public int RidePatID
    {
        get
        {
            return ridePatID;
        }

        set
        {
            ridePatID = value;
        }
    }

    public string DateTime
    {
        get
        {
            return dateTime;
        }

        set
        {
            dateTime = value;
        }
    }

    public int UserID
    {
        get
        {
            return userID;
        }

        set
        {
            userID = value;
        }
    }

    public string UserNotes
    {
        get
        {
            return userNotes;
        }

        set
        {
            userNotes = value;
        }
    }

    public bool IsPush
    {
        get
        {
            return isPush;
        }

        set
        {
            isPush = value;
        }
    }

    public bool IsMail
    {
        get
        {
            return isMail;
        }

        set
        {
            isMail = value;
        }
    }

    public bool IsWhatsapp
    {
        get
        {
            return isWhatsapp;
        }

        set
        {
            isWhatsapp = value;
        }
    }

    public string Sender
    {
        get
        {
            return sender;
        }

        set
        {
            sender = value;
        }
    }

    public Message()
    {
        //
        // TODO: Add constructor logic here
        //

    }



    public List<Message> getMessages(string displayName)
    {
        //displayName = displayName.Replace("'", "''");

        Volunteer v = new Volunteer();
        v.DisplayName = displayName;
        Volunteer volunteer = v.getVolunteer();
        int ID = volunteer.Id;
        string query = "select * from [Messages] where UserID= '" + ID + "'";
        List<Message> list = new List<Message>();
        DbService db = new DbService();
        DataSet ds = db.GetDataSetByQuery(query);

        foreach (DataRow dr in ds.Tables[0].Rows)
        {
            Message m = new Message();
            m.msgID = int.Parse(dr["MsgID"].ToString());
            m.parentID = int.Parse(dr["ParentID"].ToString());
            m.type = dr["Type"].ToString();
            m.title = dr["Title"].ToString();
            
            m.msgContent = dr["MsgContent"].ToString();
            m.ridePatID = int.Parse(dr["RidePatID"].ToString());
            m.dateTime = dr["DateTime"].ToString();
            m.userID = int.Parse(dr["UserID"].ToString());
            m.userNotes = dr["UserNotes"].ToString();
            m.isPush = Convert.ToBoolean(dr["IsPush"].ToString());
            m.isMail = Convert.ToBoolean(dr["IsMail"].ToString());
            m.isWhatsapp = Convert.ToBoolean(dr["IsWhatsapp"].ToString());
            m.Sender = dr["Sender"].ToString();
            list.Add(m);
        }
        
        return list;
    }


    public int insertMsg(int parentID, string type, string title, string msgContent, int ridePatID, DateTime dateTime, int userID, string userNotes, bool isPush, bool isMail, bool isWhatsapp, string sender)
    {

        DbService db = new DbService();
        SqlCommand cmd = new SqlCommand();
        cmd.CommandType = CommandType.Text;
        SqlParameter[] cmdParams = new SqlParameter[12];
        cmdParams[0] = cmd.Parameters.AddWithValue("@ParentID", parentID);
        cmdParams[1] = cmd.Parameters.AddWithValue("@Type", type);
        cmdParams[2] = cmd.Parameters.AddWithValue("@Title", title);
        cmdParams[3] = cmd.Parameters.AddWithValue("@MsgContent", msgContent);
        cmdParams[4] = cmd.Parameters.AddWithValue("@RidePatID", ridePatID);
        cmdParams[5] = cmd.Parameters.AddWithValue("@DateTime", dateTime);
        cmdParams[6] = cmd.Parameters.AddWithValue("@UserID", userID);
        cmdParams[7] = cmd.Parameters.AddWithValue("@UserNotes", userNotes);
        cmdParams[8] = cmd.Parameters.AddWithValue("@isPush", isPush);
        cmdParams[9] = cmd.Parameters.AddWithValue("@isMail", isMail);
        cmdParams[10] = cmd.Parameters.AddWithValue("@isWhatsapp", isWhatsapp);
        cmdParams[11] = cmd.Parameters.AddWithValue("@Sender", sender);
        string query = "insert into [Messages] OUTPUT inserted.MsgID values (@ParentID,@Type,@Title,@MsgContent,@RidePatID,@DateTime,@UserID,@UserNotes,@isPush,@isMail,@isWhatsapp,@Sender)";

        try
        {
            return int.Parse(db.GetObjectScalarByQuery(query, cmd.CommandType, cmdParams).ToString());
        }
        catch (Exception e)
        {
            //add to log
            throw new Exception("sender : " + sender + " ");
           // throw e;
        }
    }

    public void globalMessage(string message, string title)
    {
        //insert msg to db
        string sender = (string)HttpContext.Current.Session["loggedInName"];
        int msgID = insertMsg(0, "Global", title, message, 0, System.DateTime.Now, 0, "", true, false, false, sender);

        //get volunteers
        Volunteer v = new Volunteer();
        List<Volunteer> volunteersList = v.getVolunteersList(true);
        var data = new JObject();

        foreach (Volunteer V in volunteersList)
        {
            if (v.Device == "iOS")
            {
                //PUSH IOS
                data = new JObject();
                var notification = new JObject();
                notification.Add("title", title);
                notification.Add("body", message);
                data = new JObject();
                data.Add("msgID", msgID);
                data.Add("content-available", 1);
                //send push
                myPushNot pushIOS = new myPushNot();
                pushIOS.RunPushNotificationOne(V, data, notification);
            }
            else
            {
                data = new JObject();
                data.Add("message", message);
                data.Add("title", title);
                data.Add("msgID", msgID);
                data.Add("content-available", 1);
                //send push
                myPushNot pushANDROID = new myPushNot();
                pushANDROID.RunPushNotificationOne(V, data, null);

            }
        }
    }


    public void cancelRide(int ridePatID, Volunteer user)
    {
        //get ride details and generate msg
        RidePat rp = new RidePat();
        var abc = rp.GetRidePat(ridePatID);

        TimeZoneInfo sourceTimeZone = TimeZoneInfo.FindSystemTimeZoneById("UTC");
        TimeZoneInfo targetTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Israel Standard Time");
        var converted = TimeZoneInfo.ConvertTime(abc.Date, sourceTimeZone, targetTimeZone);
        string time = converted.ToShortTimeString();
        if (time == "22:14")
        {

            time = "אחה\"צ";
        }


        var msg = "בוטלה נסיעה מ" + abc.Origin.Name + " ל" + abc.Destination.Name + " בתאריך " + abc.Date.ToShortDateString() + ", בשעה " + time;
        //insert msg to db
        string sender = (string)HttpContext.Current.Session["loggedInName"];
        int msgID = insertMsg(0, "Cancel", "נסיעה בוטלה", msg, ridePatID, System.DateTime.Now, user.Id, "", true, false, false, sender);

        Volunteer V = new Volunteer();
        string device = V.getDeviceByID(user.Id);
        var data = new JObject();


        if (device == "iOS")
        {
            data = new JObject();
            //PUSH IOS
            var notification = new JObject();
            notification.Add("title", "נסיעה בוטלה");
            notification.Add("body", msg);
            data.Add("rideID", ridePatID);
            data.Add("status", "Canceled");
            data.Add("msgID", msgID);
            data.Add("content-available", 1);
            //send push
            myPushNot pushIOS = new myPushNot();
            pushIOS.RunPushNotificationOne(user, data, notification);
        }
        else
        {
            data = new JObject();
            //PUSH ANDROID
            data.Add("message", msg);
            data.Add("title", "נסיעה בוטלה");
            data.Add("rideID", ridePatID);
            data.Add("status", "Canceled");
            data.Add("msgID", msgID);
            data.Add("content-available", 1);
            //send push
            myPushNot pushANDROID = new myPushNot();
            pushANDROID.RunPushNotificationOne(user, data, null);

        }

    }

    public void cancelOneRide(int ridePatID, Volunteer user)
    {
        //get ride details and generate msg
        RidePat rp = new RidePat();
        var abc = rp.GetRidePat(ridePatID);

        TimeZoneInfo sourceTimeZone = TimeZoneInfo.FindSystemTimeZoneById("UTC");
        TimeZoneInfo targetTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Israel Standard Time");
        var converted = TimeZoneInfo.ConvertTime(abc.Date, sourceTimeZone, targetTimeZone);
        string time = converted.ToShortTimeString();
        if (time == "22:14")
        {

            time = "אחה\"צ";
        }


        var msg = " החולה " + abc.Pat.DisplayName + " ירד מההסעה מ" + abc.Origin.Name + " ל" + abc.Destination.Name + " בתאריך " + abc.Date.ToShortDateString() + ", בשעה " + time + ". אבל הנסיעה מתקיימת כי יש עדיין חולים אחרים על הסעה זו.";
        //insert msg to db
        string sender = (string)HttpContext.Current.Session["loggedInName"];
        int msgID = insertMsg(0, "Cancel", "נסיעה בוטלה", msg, ridePatID, System.DateTime.Now, user.Id, "", true, false, false, sender);

        Volunteer V = new Volunteer();
        string device = V.getDeviceByID(user.Id);
        var data = new JObject();


        if (device == "iOS")
        {
            data = new JObject();
            //PUSH IOS
            var notification = new JObject();
            notification.Add("title", "עדכון לגבי הסעה");
            notification.Add("body", msg);
            data.Add("rideID", ridePatID);
            data.Add("status", "Canceled");
            data.Add("msgID", msgID);
            data.Add("content-available", 1);
            //send push
            myPushNot pushIOS = new myPushNot();
            pushIOS.RunPushNotificationOne(user, data, notification);
        }
        else
        {
            data = new JObject();
            //PUSH ANDROID
            data.Add("message", msg);
            data.Add("title", "עדכון לגבי הסעה");
            data.Add("rideID", ridePatID);
            data.Add("status", "Canceled");
            data.Add("msgID", msgID);
            data.Add("content-available", 1);
            //send push
            myPushNot pushANDROID = new myPushNot();
            pushANDROID.RunPushNotificationOne(user, data, null);

        }

    }


    public void rideIsTomorrow(int ridePatID, Volunteer user)
    {
        //get ride details and generate msg
        RidePat rp = new RidePat();
        var abc = rp.GetRidePat(ridePatID);
        var msg = "מחר מתקיימת הסעה מ" + abc.Origin.Name + " ל" + abc.Destination.Name + ", בשעה " + abc.Date.ToShortTimeString();

        if (abc.Date.ToShortTimeString() == "22:14") msg = "מחר מתקיימת הסעה מ" + abc.Origin.Name + " ל" + abc.Destination.Name + " אחה\"צ";
        //insert msg to db
        string sender = (string)HttpContext.Current.Session["loggedInName"];
        int msgID = insertMsg(0, "Reminder", "תזכורת", msg, ridePatID, System.DateTime.Now, user.Id, "", true, false, false, sender);


        //PUSH ANDROID
        var data = new JObject();
        data.Add("message", msg);
        data.Add("title", "נסיעה קרובה");
        data.Add("rideID", ridePatID);
        data.Add("status", "Reminder");
        data.Add("msgID", msgID);
        data.Add("content-available", 1);
        //send push
        myPushNot pushANDROID = new myPushNot();
        pushANDROID.RunPushNotificationOne(user, data, null);

        //PUSH IOS
        var notification = new JObject();
        notification.Add("title", "נסיעה קרובה");
        notification.Add("body", msg);
        data = new JObject();
        data.Add("rideID", ridePatID);
        data.Add("status", "Reminder");
        data.Add("msgID", msgID);
        data.Add("content-available", 1);
        //send push
        myPushNot pushIOS = new myPushNot();
        pushIOS.RunPushNotificationOne(user, data, notification);
    }

    public void driverCanceledRide(int ridePatID, Volunteer user)
    {
        //get ride details and generate message
        RidePat rp = new RidePat();
        var abc = rp.GetRidePat(ridePatID);
        Volunteer coor = new Volunteer();
        coor = abc.Coordinator.getVolunteerByDisplayName(abc.Coordinator.DisplayName);

        var message = "";
        if (user.Gender == "מתנדב")
        {
            message = "הנהג " + user.FirstNameH + " " + user.LastNameH + " ביטל את הנסיעה מ" + abc.Origin.Name + " ל" + abc.Destination.Name + " עם החולה " + abc.Pat.DisplayName + " שמתקיימת בזמן הקרוב";

        }
        else
        {
            message = "הנהגת " + user.FirstNameH + " " + user.LastNameH + " ביטלה את הנסיעה מ" + abc.Origin.Name + " ל" + abc.Destination.Name + " עם החולה " + abc.Pat.DisplayName + " שמתקיימת בזמן הקרוב";
        }
        //insert msg to db

        string sender;
        try { sender = (string)HttpContext.Current.Session["loggedInName"]; }
        catch
        {
            sender = "הנהג";
        }

        int msgID = insertMsg(0, "Canceled by driver", "נסיעה בוטלה על ידי נהג\\ת", message, ridePatID, System.DateTime.Now, user.Id, "", true, false, false, sender);


        var data = new JObject();

        if (coor.Device == "iOS")
        {
            //PUSH IOS
            data = new JObject();
            var notification = new JObject();
            notification.Add("title", "נסיעה בוטלה על ידי נהג\\ת");
            notification.Add("body", message);
            data.Add("rideID", ridePatID);
            data.Add("status", "Canceled");
            data.Add("msgID", msgID);
            data.Add("content-available", 1);
            //send push
            myPushNot pushIOS = new myPushNot();
            pushIOS.RunPushNotificationOne(coor, data, notification);
        }
        else
        {
            //PUSH ANDROID
            data = new JObject();
            data.Add("message", message);
            data.Add("title", "נסיעה בוטלה על ידי נהג\\ת");
            data.Add("rideID", ridePatID);
            data.Add("status", "Canceled");
            data.Add("msgID", msgID);
            data.Add("content-available", 1);
            //send push
            myPushNot pushANDROID = new myPushNot();
            pushANDROID.RunPushNotificationOne(coor, data, null);
        }
    }

    public void driverSignUpToCloseRide(int ridePatID, Volunteer user, bool isPrimary)
    {
        //get ride details and generate message
        RidePat rp = new RidePat();
        var abc = rp.GetRidePat(ridePatID);
        Volunteer coor = new Volunteer();
        coor = abc.Coordinator.getVolunteerByDisplayName(abc.Coordinator.DisplayName);
        string driverType = isPrimary ? "נהג ראשי" : "גיבוי";
        TimeZoneInfo sourceTimeZone = TimeZoneInfo.FindSystemTimeZoneById("UTC");
        TimeZoneInfo targetTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Israel Standard Time");
        var converted = TimeZoneInfo.ConvertTime(abc.Date, sourceTimeZone, targetTimeZone);
        string time = converted.ToShortTimeString();
        if (time == "22:14")
        {

            time = "אחה\"צ";
        }

        var message = "";
        if (user.Gender == "מתנדב")
        {
            message = "הנהג " + user.FirstNameH + " " + user.LastNameH + " נרשם להסעה מ" + abc.Origin.Name + " ל" + abc.Destination.Name + " עם החולה " + abc.Pat.DisplayName + " כ" + driverType + " בתאריך " + abc.Date.ToShortDateString() + " ושעה " + time + ".";
        }
        else
        {
            message = "הנהגת " + user.FirstNameH + " " + user.LastNameH + " נרשמה להסעה מ" + abc.Origin.Name + " ל" + abc.Destination.Name + " עם החולה " + abc.Pat.DisplayName + " כ" + driverType + " בתאריך " + abc.Date.ToShortDateString() + " ושעה " + time + ".";
        }

        //insert msg to db
        //string sender = (string)HttpContext.Current.Session["loggedInName"];
        string sender1;
        try
        {
             sender1 = (string)HttpContext.Current.Session["loggedInName"];
        }
        catch {
            sender1 = "הנהג";
        }




        int msgID;
        try
        {
            msgID = insertMsg(0, "sign by driver", "הרשמה להסעה קרובה", message, ridePatID, System.DateTime.Now, user.Id, "", true, false, false, sender1); //XXX
        }
        catch (Exception ex) {
            throw new Exception("I1: " + ex.Message);
        }


        var data = new JObject();

        if (coor.Device == "iOS")
        {
            //PUSH IOS
            data = new JObject();
            var notification = new JObject();
            notification.Add("title", "הרשמה להסעה קרובה");
            notification.Add("body", message);
            data.Add("rideID", ridePatID);
            data.Add("status", "SignUp");
            data.Add("msgID", msgID);
            data.Add("content-available", 1);
            //send push
            myPushNot pushIOS = new myPushNot();
            pushIOS.RunPushNotificationOne(coor, data, notification);
        }
        else
        {
            //PUSH ANDROID
            data = new JObject();
            data.Add("message", message);
            data.Add("title", "הרשמה להסעה קרובה");
            data.Add("rideID", ridePatID);
            data.Add("status", "SignUp");
            data.Add("msgID", msgID);
            data.Add("content-available", 1);
            //send push
            myPushNot pushANDROID = new myPushNot();
            pushANDROID.RunPushNotificationOne(coor, data, null);
        }
    }

    public void changeAnonymousPatient(int ridePatID, Volunteer user)
    {
        //get ride details and generate msg
        RidePat rp = new RidePat();
        var abc = rp.GetRidePat(ridePatID);

        TimeZoneInfo sourceTimeZone = TimeZoneInfo.FindSystemTimeZoneById("UTC");
        TimeZoneInfo targetTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Israel Standard Time");
        var converted = TimeZoneInfo.ConvertTime(abc.Date, sourceTimeZone, targetTimeZone);
        string time = converted.ToShortTimeString();
        if (time == "22:14")
        {

            time = "אחה\"צ";
        }

        var msg = abc.Pat.DisplayName + " הינו החולה בנסיעה מ" + "" + abc.Origin.Name + " ל" + abc.Destination.Name + " בתאריך " + abc.Date.ToShortDateString() + ", בשעה " + time;
        //insert msg to db
        string sender = (string)HttpContext.Current.Session["loggedInName"];
        int msgID = insertMsg(0, "Anonymous Patient changed", "עדכון נתוני הסעה", msg, ridePatID, System.DateTime.Now, user.Id, "", true, false, false, sender);

        Volunteer V = new Volunteer();
        string device = V.getDeviceByID(user.Id);
        var data = new JObject();
        if (device == "iOS")
        {
            data = new JObject();
            //PUSH IOS
            var notification = new JObject();
            notification.Add("title", "עדכון נתוני הסעה");
            notification.Add("body", msg);
            data.Add("rideID", ridePatID);
            data.Add("status", "Anonymous Patient changed");
            data.Add("msgID", msgID);
            data.Add("content-available", 1);
            //send push
            myPushNot pushIOS = new myPushNot();
            pushIOS.RunPushNotificationOne(user, data, notification);
        }
        else
        {
            data = new JObject();
            //PUSH ANDROID
            data.Add("message", msg);
            data.Add("title", "עדכון נתוני הסעה");
            data.Add("rideID", ridePatID);
            data.Add("status", "Anonymous Patient changed");
            data.Add("msgID", msgID);
            data.Add("content-available", 1);
            //send push
            myPushNot pushANDROID = new myPushNot();
            pushANDROID.RunPushNotificationOne(user, data, null);
        }
    }

    public int backupToPrimary(int ridePatID)
    {
        string time = "";
        //get ride details and generate msg
        RidePat rp = new RidePat();
        rp = rp.GetRidePat(ridePatID);
        Volunteer v = new Volunteer();
        if (rp.Drivers[0].DriverType == "Secondary") v = rp.Drivers[0];
        else throw new Exception("The assigned driver is the primary driver for this ride");

        string device = v.getDeviceByID(v.Id);

        TimeZoneInfo sourceTimeZone = TimeZoneInfo.FindSystemTimeZoneById("UTC");
        TimeZoneInfo targetTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Israel Standard Time");
        var converted = TimeZoneInfo.ConvertTime(rp.Date, sourceTimeZone, targetTimeZone);
        time = converted.ToShortTimeString();
        if (time == "22:14")
        {

            time = "אחה\"צ";
        }

        string msg = "האם ברצונך להחליף את הנהג הראשי בנסיעה מ" + rp.Origin.Name + " ל" + rp.Destination.Name + " בתאריך " + rp.Date.ToShortDateString() + ", בשעה " + time + "?";

        //insert msg to db
        string sender = (string)HttpContext.Current.Session["loggedInName"];
        int msgID = insertMsg(0, "BackupToPrimary", "החלפת נהג ראשי", msg, ridePatID, System.DateTime.Now, v.Id, "", true, false, false, sender);

        var data = new JObject();
        if (device == "iOS")
        {
            data = new JObject();
            //PUSH IOS
            var notification = new JObject();
            notification.Add("title", "החלפת נהג ראשי");
            notification.Add("body", msg);
            data.Add("rideID", ridePatID);
            data.Add("status", "PrimaryCanceled");
            data.Add("msgID", msgID);
            data.Add("content-available", 1);
            //send push
            myPushNot pushIOS = new myPushNot();
            pushIOS.RunPushNotificationOne(v, data, notification);
        }
        else
        {
            data = new JObject();
            //PUSH ANDROID
            data.Add("message", msg);
            data.Add("title", "החלפת נהג ראשי");
            data.Add("rideID", ridePatID);
            data.Add("status", "PrimaryCanceled");
            data.Add("msgID", msgID);
            data.Add("content-available", 1);
            //send push
            myPushNot pushANDROID = new myPushNot();
            pushANDROID.RunPushNotificationOne(v, data, null);

        }

        return 1;
    }

    public void pushFromAssistant(int ridePatID, string cellphone, string msg)
    {
        RidePat rp = new RidePat();
        var abc = rp.GetRidePat(ridePatID);


        TimeSpan ts = abc.Date - System.DateTime.Now;

        // Difference in days.
        double differenceInDays = ts.TotalDays; // This is in double
        if (differenceInDays <= 2)
        {
            Volunteer V = new Volunteer();
            Volunteer user = V.getVolunteerByCellphone(cellphone);
            string device = V.getDeviceByID(user.Id);

            string sender = (string)HttpContext.Current.Session["loggedInName"];
            int msgID = insertMsg(0, "Change by assistant", "שינוי על ידי עוזר", msg, ridePatID, System.DateTime.Now, user.Id, "", true, false, false, sender);

            var data = new JObject();


            if (device == "iOS")
            {
                data = new JObject();
                //PUSH IOS
                var notification = new JObject();
                notification.Add("title", "שינוי ע\"י עוזר");
                notification.Add("body", msg);
                data.Add("rideID", ridePatID);
                data.Add("status", "Assistant change");
                data.Add("msgID", msgID);
                data.Add("content-available", 1);
                //send push
                myPushNot pushIOS = new myPushNot();
                pushIOS.RunPushNotificationOne(user, data, notification);
            }
            else
            {
                data = new JObject();
                //PUSH ANDROID
                data.Add("message", msg);
                data.Add("title", "שינוי ע\"י עוזר");
                data.Add("rideID", ridePatID);
                data.Add("status", "Assistant change");
                data.Add("msgID", msgID);
                data.Add("content-available", 1);
                //send push
                myPushNot pushANDROID = new myPushNot();
                pushANDROID.RunPushNotificationOne(user, data, null);

            }
        }

    }

    public void coordinatorCanceledRide(int ridePatID, Volunteer user)
    {
        //get ride details and generate message
        RidePat rp = new RidePat();
        var abc = rp.GetRidePat(ridePatID);
        Volunteer coor = new Volunteer();
        coor = abc.Coordinator.getVolunteerByDisplayName(abc.Coordinator.DisplayName);

        TimeZoneInfo sourceTimeZone = TimeZoneInfo.FindSystemTimeZoneById("UTC");
        TimeZoneInfo targetTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Israel Standard Time");
        var converted = TimeZoneInfo.ConvertTime(abc.Date, sourceTimeZone, targetTimeZone);
        string time = converted.ToShortTimeString();
        if (time == "22:14")
        {

            time = "אחה\"צ";
        }
        //abc.Date.ToShortDateString()
        var message = "נסיעה בוטלה על ידי הרכז/ת " + abc.Coordinator.DisplayName + "." + " הנסיעה בתאריך " + abc.Date.ToShortDateString() + " בשעה " + time + " עם החולה " + abc.Pat.DisplayName;
        int userId = 0;
        Volunteer V = new Volunteer();

        if (user != null)
        {
            V = V.getVolunteerByID(user.Id);
            userId = user.Id;
            message = "נסיעה בוטלה על ידי הרכז/ת " + abc.Coordinator.DisplayName + "." + " הנסיעה בתאריך " + abc.Date.ToShortDateString() + " בשעה " + time + " עם החולה " + abc.Pat.DisplayName + " על ידי הנהג/ת " + V.DisplayName;
        }

        //insert msg to db
        string sender = (string)HttpContext.Current.Session["loggedInName"];
        int msgID = insertMsg(0, "Canceled by coordinator", "נסיעה בוטלה על ידי רכז/ת", message, ridePatID, System.DateTime.Now, userId, "", true, false, false, sender);


        var data = new JObject();

        if (coor.Device == "iOS")
        {
            //PUSH IOS
            data = new JObject();
            var notification = new JObject();
            notification.Add("title", "נסיעה בוטלה על ידי רכז/ת");
            notification.Add("body", message);
            data.Add("rideID", ridePatID);
            data.Add("status", "Canceled");
            data.Add("msgID", msgID);
            data.Add("content-available", 1);
            //send push
            myPushNot pushIOS = new myPushNot();
            pushIOS.RunPushNotificationOne(coor, data, notification);
        }
        else
        {
            //PUSH ANDROID
            data = new JObject();
            data.Add("message", message);
            data.Add("title", "נסיעה בוטלה על ידי רכז/ת");
            data.Add("rideID", ridePatID);
            data.Add("status", "Canceled");
            data.Add("msgID", msgID);
            data.Add("content-available", 1);
            //send push
            myPushNot pushANDROID = new myPushNot();
            pushANDROID.RunPushNotificationOne(coor, data, null);
        }
    }

    public void changeInRide(int ridePatID, Volunteer user)
    {
        //get ride details and generate msg
        RidePat rp = new RidePat();
        var abc = rp.GetRidePat(ridePatID);

        TimeZoneInfo sourceTimeZone = TimeZoneInfo.FindSystemTimeZoneById("UTC");
        TimeZoneInfo targetTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Israel Standard Time");
        var converted = TimeZoneInfo.ConvertTime(abc.Date, sourceTimeZone, targetTimeZone);
        string time = converted.ToShortTimeString();
        if (time == "22:14")
        {

            time = "אחה\"צ";
        }

        string escortsStr = "";
        if (abc.Escorts.Count > 0)
        {
            if (abc.Escorts.Count == 1)
            {
                escortsStr = " עם מלווה";
            }
            else
            {
                escortsStr = " עם " + abc.Escorts.Count + " מלווים";
            }

        }

        string displayName = abc.Pat.DisplayName;
        if (abc.Pat.IsAnonymous == "True")
        {
            displayName = "חולה";
        }

        var msg = "בוצע שינוי בהסעה שנרשמת אליה. לאחר השינוי, " + displayName + escortsStr + ", מ" + abc.Origin.Name + " ל" + abc.Destination.Name + " ב-" + abc.Date.Day + "/" + abc.Date.Month + ", בשעה " + time;

        //XXX
        //insert msg to db
        //int msgID = insertMsg(0, "Anonymous Patient changed", "עדכון שם חולה בהסעה", msg, ridePatID, DateTime.Now, user.Id, "", true, false, false);
        string sender = (string)HttpContext.Current.Session["loggedInName"];
        int msgID = insertMsg(0, "Changes in ride", "שינויים בהסעה", msg, ridePatID, System.DateTime.Now, user.Id, "", true, false, false, sender);


        Volunteer V = new Volunteer();
        string device = V.getDeviceByID(user.Id);
        var data = new JObject();
        if (device == "iOS")
        {
            data = new JObject();
            //PUSH IOS
            var notification = new JObject();
            notification.Add("title", "שינויים בהסעה");
            notification.Add("body", msg);
            data.Add("rideID", ridePatID);
            data.Add("status", "Changes in ride");
            data.Add("msgID", msgID);
            data.Add("content-available", 1);
            //send push
            myPushNot pushIOS = new myPushNot();
            pushIOS.RunPushNotificationOne(user, data, notification);
        }
        else
        {
            data = new JObject();
            //PUSH ANDROID
            data.Add("message", msg);
            data.Add("title", "שינויים בהסעה");
            data.Add("rideID", ridePatID);
            data.Add("status", "Changes in ride");
            data.Add("msgID", msgID);
            data.Add("content-available", 1);
            //send push
            myPushNot pushANDROID = new myPushNot();
            pushANDROID.RunPushNotificationOne(user, data, null);
        }
    }

    public void driverAddedToRide(int ridePatID, Volunteer user)
    {
        
        //get ride details and generate message
        RidePat rp = new RidePat();
        var abc = rp.GetRidePat(ridePatID);
        //Volunteer coor = new Volunteer();
        //coor = abc.Coordinator.getVolunteerByDisplayName(abc.Coordinator.DisplayName);

        TimeZoneInfo sourceTimeZone = TimeZoneInfo.FindSystemTimeZoneById("UTC");
        TimeZoneInfo targetTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Israel Standard Time");
        var converted = TimeZoneInfo.ConvertTime(abc.Date, sourceTimeZone, targetTimeZone);
        string time = converted.ToShortTimeString();
        if (time == "22:14")
        {

            time = "אחה\"צ";
        }
        //abc.Date.ToShortDateString()

        string escortsStr = "";
        if (abc.Escorts.Count > 0)
        {
            if (abc.Escorts.Count == 1)
            {
                escortsStr = " עם מלווה";
            }
            else
            {
                escortsStr = " עם " + abc.Escorts.Count + " מלווים";
            }

        }

        string displayName = abc.Pat.DisplayName;
        if (abc.Pat.IsAnonymous == "True")
        {
            displayName = "חולה";
        }



        var message = "שובצת לנסיעה: " + displayName + escortsStr + ", מ" + abc.Origin.Name + " ל" + abc.Destination.Name + " ב-" + abc.Date.Day + "/" + abc.Date.Month + ", בשעה " + time;

        int userId = 0;
        Volunteer V = new Volunteer();

        //if (user != null)
        //{
        //    V = V.getVolunteerByID(user.Id);
        //    userId = user.Id;
        //    message = "נסיעה בוטלה על ידי הרכז/ת " + abc.Coordinator.DisplayName + "." + " הנסיעה בתאריך " + abc.Date.ToShortDateString() + " בשעה " + time + " עם החולה " + abc.Pat.DisplayName + " על ידי הנהג/ת " + V.DisplayName;
        //}

        //insert msg to db

        string sender1;
        try {  sender1 = (string)HttpContext.Current.Session["loggedInName"]; }
        catch {
            sender1 = "הנהג";
        }



        int msgID;
        try
        {
            msgID = insertMsg(0, "You have been listed for a ride", "שובצת לנסיעה", message, ridePatID, System.DateTime.Now, user.Id, "", true, false, false, sender1);
        }
        catch (Exception ex) {
            throw new Exception("insertMsg Failed: sender2: " + sender1);
        }

        var data = new JObject();

        if (user.Device == "iOS")
        {
            //PUSH IOS
            data = new JObject();
            var notification = new JObject();
            notification.Add("title", "שובצת לנסיעה");
            notification.Add("body", message);
            data.Add("rideID", ridePatID);
            data.Add("status", "Canceled");
            data.Add("msgID", msgID);
            data.Add("content-available", 1);
            //send push
            myPushNot pushIOS = new myPushNot();
            pushIOS.RunPushNotificationOne(user, data, notification);
        }
        else
        {
            //PUSH ANDROID
            data = new JObject();
            data.Add("message", message);
            data.Add("title", "שובצת לנסיעה");
            data.Add("rideID", ridePatID);
            data.Add("status", "Canceled");
            data.Add("msgID", msgID);
            data.Add("content-available", 1);
            //send push
            myPushNot pushANDROID = new myPushNot();
            pushANDROID.RunPushNotificationOne(user, data, null);
        }
    }

    public void driverRemovedFromRide(int ridePatID, Volunteer user)
    {
        //get ride details and generate message
        RidePat rp = new RidePat();
        var abc = rp.GetRidePat(ridePatID);
        //Volunteer coor = new Volunteer();
        //coor = abc.Coordinator.getVolunteerByDisplayName(abc.Coordinator.DisplayName);

        TimeZoneInfo sourceTimeZone = TimeZoneInfo.FindSystemTimeZoneById("UTC");
        TimeZoneInfo targetTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Israel Standard Time");
        var converted = TimeZoneInfo.ConvertTime(abc.Date, sourceTimeZone, targetTimeZone);
        string time = converted.ToShortTimeString();
        if (time == "22:14")
        {

            time = "אחה\"צ";
        }
        //abc.Date.ToShortDateString()

        string escortsStr = "";
        if (abc.Escorts.Count > 0)
        {
            if (abc.Escorts.Count == 1)
            {
                escortsStr = " עם מלווה";
            }
            else
            {
                escortsStr = " עם " + abc.Escorts.Count + " מלווים";
            }

        }

        string displayName = abc.Pat.DisplayName;
        if (abc.Pat.IsAnonymous == "True")
        {
            displayName = "חולה";
        }


        var message = "בוטלה הנסיעה: " + displayName + escortsStr + ", מ" + abc.Origin.Name + " ל" + abc.Destination.Name + " ב-" + abc.Date.Day + "/" + abc.Date.Month + ", בשעה " + time;

        //int userId = 0;
        Volunteer V = new Volunteer();

        //if (user != null)
        //{
        //    V = V.getVolunteerByID(user.Id);
        //    userId = user.Id;
        //    message = "נסיעה בוטלה על ידי הרכז/ת " + abc.Coordinator.DisplayName + "." + " הנסיעה בתאריך " + abc.Date.ToShortDateString() + " בשעה " + time + " עם החולה " + abc.Pat.DisplayName + " על ידי הנהג/ת " + V.DisplayName;
        //}

        //insert msg to db
        string sender = "unknown";
        if (HttpContext.Current.Session["loggedInName"] != null)
        {
            sender = (string)HttpContext.Current.Session["loggedInName"];

        }
        


        int msgID = insertMsg(0, "Ride canceled", "נסיעה בוטלה", message, ridePatID, System.DateTime.Now, user.Id, "", true, false, false, sender);


        var data = new JObject();

        if (user.Device == "iOS")
        {
            //PUSH IOS
            data = new JObject();
            var notification = new JObject();
            notification.Add("title", "נסיעה בוטלה");
            notification.Add("body", message);
            data.Add("rideID", ridePatID);
            data.Add("status", "Canceled");
            data.Add("msgID", msgID);
            data.Add("content-available", 1);
            //send push
            myPushNot pushIOS = new myPushNot();
            pushIOS.RunPushNotificationOne(user, data, notification);
        }
        else
        {
            //PUSH ANDROID
            data = new JObject();
            data.Add("message", message);
            data.Add("title", "נסיעה בוטלה");
            data.Add("rideID", ridePatID);
            data.Add("status", "Canceled");
            data.Add("msgID", msgID);
            data.Add("content-available", 1);
            //send push
            myPushNot pushANDROID = new myPushNot();
            pushANDROID.RunPushNotificationOne(user, data, null);
        }
    }
}