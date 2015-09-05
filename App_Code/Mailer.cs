using System;
using System.Data;
using System.Configuration;
using System.Net.Mail;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

/// <summary>
/// Summary description for Mailer
/// </summary>
public class Mailer
{
	public Mailer()
	{
		// TODO: Add constructor logic here
	}

    /// <summary>
    /// Sends an e-mail.
    /// </summary>
    /// <param name="senderEmail">E-mail address of the sender. This must be a valid e-mail account.</param>
    /// <param name="senderName">Friendly name of the sender</param>
    /// <param name="recipientEmail">E-mail address of the recipient</param>
    /// <param name="recipientName">Friendly name of the recipient</param>
    /// <param name="subject">Subject of the e-mail</param>
    /// <param name="body">Body of the e-mail</param>
    /// <param name="isBodyHtml">Should the e-mail body be send as html?</param>
    /// <param name="smtpServer">Name of the smtp server to use.</param>
    public void SendMail(string senderEmail, string senderName, string recipientEmail, 
                         string recipientName, string subject, string body, bool isBodyHtml, 
                         string smtpServer)
    {
        MailMessage mail = new MailMessage();
        mail.From = new MailAddress(senderEmail, senderName);
        mail.To.Add(new MailAddress(recipientEmail, recipientName));
        mail.Subject = subject;
        mail.Body = body;
        mail.IsBodyHtml = isBodyHtml;

        SmtpClient smtp = new SmtpClient(smtpServer);
        smtp.Send(mail);
    }
}
