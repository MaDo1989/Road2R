using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Net.Mail;
using System.Web.Security;
using System.Configuration;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Net;
using System.Text;
 

/// <summary>
/// Summary description for Email
/// </summary>
public class Email
{

    Dictionary<string, string> subjects = new Dictionary<string, string>{
      {"problem","בעיה מדווחת מהאפליקציה"},
      {"registration","בקשה לרישום"}
    };

	public Email()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    private void sendMail(MailMessage message){
            SmtpClient smtp = new SmtpClient();
            if(smtp.EnableSsl == true)
                ServicePointManager.ServerCertificateValidationCallback = delegate(object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
            smtp.Send(message);
    }
  
    public void sendMessage(string type, string userName, string phoneNumber, string messageText) {

        MailMessage message = new MailMessage();
        string systemMail = ConfigurationManager.AppSettings["systemMail"];
        string adminMail = ConfigurationManager.AppSettings["adminMail"];
        message.To.Add(new MailAddress(adminMail));

        message.IsBodyHtml = true;
        try
        {
            message.Subject = subjects[type];
        }
        catch (Exception ex) {
            throw new Exception("no such type: " + type + ", " + ex.Message);
        }
        StringBuilder sb = new StringBuilder();
        //sb.AppendFormat(" {שם: {0} מספר : {1", userName, phoneNumber);
        sb.AppendFormat("{0} {1}", userName, phoneNumber);
        if (type == "problem")
        {
            sb.AppendFormat("<BR/>");
            sb.AppendFormat("{0}", messageText);
        }
        message.Body = sb.ToString();
        try
        {
            sendMail(message);
        }
        catch (Exception ex)
        {
            throw (ex);
        }

    }

}
