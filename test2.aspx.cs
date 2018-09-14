using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using log4net;

public partial class test2 : System.Web.UI.Page
{
    private static readonly ILog Log =
                LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            string user = (string)HttpContext.Current.Session["userSession"];
            Log.Error("this is a new error message, user: "+user);

        }
        catch (Exception ex)
        {
            Response.Write(ex.Message);
        }
    }
}