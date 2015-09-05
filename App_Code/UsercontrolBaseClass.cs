using System;
using System.Configuration;
using System.Data;
using System.Net.Mail;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

/// <summary>
/// Summary description for UsercontrolBaseClass
/// </summary>
public class UsercontrolBaseClass : System.Web.UI.UserControl
{
	public UsercontrolBaseClass()
	{

    }

    public string ValidateInput(string invalidInput)
    {
        invalidInput = Server.HtmlEncode(invalidInput);
        return invalidInput;
    }
    public void SendMail(string senderEmail, string senderName, string recipientEmail,
                         string recipientName, string subject, string body, bool isBodyHtml,
                         string smtpServer)
    {
        Mailer mailer = new Mailer();
        mailer.SendMail(senderEmail, senderName, recipientEmail, 
                        recipientName, subject, body, isBodyHtml, 
                        smtpServer);
    }

    public void LogError(Exception ex)
    {
        BaseClass baseclass = new BaseClass();
        baseclass.LogError(ex);
    }

}
