using System.Collections.Generic;
using System.Data;

/// <summary>
/// Summary description for Role
/// </summary>
public class Role
{

    string roleName;
    public Role()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public string RoleName
    {
        get
        {
            return roleName;
        }

        set
        {
            roleName = value;
        }
    }

    public List<Role> getRolesList()
    {
        #region DB functions
        string query = "select * from Roles order by role";

        List<Role> list = new List<Role>();
        DbService db = new DbService();
        DataSet ds = db.GetDataSetByQuery(query);

        foreach (DataRow dr in ds.Tables[0].Rows)
        {
            Role r = new Role();
            r.RoleName = dr["role"].ToString();
            list.Add(r);
        }
        #endregion

        return list;
    }
}