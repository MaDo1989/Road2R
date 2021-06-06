using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class _default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        var myLog = new List<string>();
        myLog.Add(string.Format("{0} - Logging Started", DateTime.UtcNow));

        logListView.DataSource = myLog;
        logListView.DataBind();
    }
}