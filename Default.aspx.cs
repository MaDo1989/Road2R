using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class _Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {


    }

    protected void cancelBTN_Click(object sender, EventArgs e)
    {
        var userID = int.Parse(UserTB.Text);
        var ridePatID = int.Parse(RideTB.Text);

        var v = new Volunteer();
        List<Volunteer> volunteersList = v.getVolunteersList(true);

        foreach (var user in volunteersList)
        {
            if (user.Id == userID)
            {
                Message m = new Message();
                m.cancelRide(ridePatID, user);
            }
        }
    }

    protected void backupToPrimary_Click(object sender, EventArgs e)
    {
        int ridePatID = int.Parse(TextBox3.Text);
        Message m = new Message();
        m.backupToPrimary(ridePatID);
    }


    protected void globalBTN_Click(object sender, EventArgs e)
    {
        string title = TextBox1.Text;
        string message = TextBox2.Text;

        Message m = new Message();
        m.globalMessage(message, title);
    }
}