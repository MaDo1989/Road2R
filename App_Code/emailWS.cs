using System;
// REMEMBER TO ADD THIS NAMESPACE
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Web.Services;

/// <summary>
/// Summary description for WebService
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
[System.Web.Script.Services.ScriptService]
public class emailWS : System.Web.Services.WebService
{

    public emailWS()
    {

    }



    // REMEMBER TO ADD THOSE 2 ATTRIBUTES
    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    //--------------------------------------------------------------------------
    // send an email
    //--------------------------------------------------------------------------
    public string sendEmail(string type, string name, string phoneNumber, string message)
    {

        Email email = new Email();

        JavaScriptSerializer js = new JavaScriptSerializer();

        try
        {
            email.sendMessage(type, name, phoneNumber, message);
            return js.Serialize("message sent");
        }
        catch (Exception ex)
        {
            throw new Exception("Error in sending email: " + ex.Message);
        }

    }


}
