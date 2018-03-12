<%@ WebHandler Language="C#" Class="Handler" %>

using System;
using System.Web;
using System.Collections.Specialized;
using System.IO;
public class Handler : IHttpHandler {

    public void ProcessRequest (HttpContext context) {

        HttpPostedFile f = context.Request.Files["file"];
        NameValueCollection c = context.Request.Params;

        string id = c["id"];
        string associatedTbl = c["imageTable"];
        string func = c["imgFunc"];


        // דרך ראשונה- לשמור את התמונה כקובץ
        //f.SaveAs(context.Server.MapPath("~/") + func + ".jpg");


        // דרך שניה - לשמור כקובץ בינארי מקס ואז את זה צריך לדחוף לדאטה בייס
        Stream s = f.InputStream;
        BinaryReader br = new BinaryReader(s);
        byte[] b = br.ReadBytes((int)s.Length);

        DbService db = new DbService();

        if (func == "New")
        {
            db.ExecuteQuery("insert into Images values(@imgID,CONVERT(varbinary(max),@img),@tbl)", System.Data.CommandType.Text, new System.Data.SqlClient.SqlParameter("@img", b), new System.Data.SqlClient.SqlParameter("@imgID", id), new System.Data.SqlClient.SqlParameter("@tbl", associatedTbl));
        }
        else if (func == "Edit")
        {
            db.ExecuteQuery("update Images set ImageUrl = CONVERT(varbinary(max),@img) where ImageID = @imgID", System.Data.CommandType.Text, new System.Data.SqlClient.SqlParameter("@img", b), new System.Data.SqlClient.SqlParameter("@imgID", id));
        }

    }

    public bool IsReusable {
        get {
            return false;
        }
    }

}