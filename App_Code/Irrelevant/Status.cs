using System.Collections.Generic;
using System.Data;

/// <summary>
/// Summary description for Status
/// </summary>
public class Status
{
    public Status()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public string Name { get; set; }

    public List<Status> getAllStatus()
    {
        List<Status> ls = new List<Status>();
        DbService db = new DbService();
        string query = "select * from status";
        DataSet ds = db.GetDataSetByQuery(query);
        foreach (DataRow dr in ds.Tables[0].Rows)
        {
            Status s = new Status();
            s.Name = dr["StatusName"].ToString();
            ls.Add(s);
        }
        return ls;
    }
}