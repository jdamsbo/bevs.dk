using System;
using System.Data;
using System.Data.OleDb;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using GotDotNet.ApplicationBlocks.Data;
using MySql.Data.MySqlClient;


public class Logger
{

    public enum StorageType
    {
        SQLServer,
        MySQL,
        Access,
        File
    }

    private StorageType _storage;

    public Logger()
    {
        // TODO: Add constructor logic here
    }

    public StorageType Storage
    {
        get { return _storage; }
        set { _storage = value; }
    }

    public string strSQL;

    /// <summary>
    /// Logs exceptions to the preferred database
    /// </summary>
    /// <param name="ex">The exception</param>
    /// <param name="_dsn">The database connection string</param>
    /// <param name="page">The Page object</param>
    /// <param name="showCustomErrorMessageOnPage">If set, this displays the full error text on the page to a Literal control with id="litError"</param>
    /// <example>Logger logger = new Logger();
    /// logger.Storage = Logger.StorageType.MySQL;
    /// logger.LogError(ex, _dsn, Page, false);</example>
    public void LogError(Exception ex, string _dsn, Page page, bool showCustomErrorMessageOnPage)
    {
        LogError(ex, _dsn, page, showCustomErrorMessageOnPage, false, string.Empty, string.Empty,
                    string.Empty, string.Empty, string.Empty, string.Empty);
    }

    /// <summary>
    /// Logs exceptions to the preferred database
    /// </summary>
    /// <param name="ex">The exception</param>
    /// <param name="_dsn">The database connection string</param>
    /// <param name="page">The Page object</param>
    /// <param name="showCustomErrorMessageOnPage">If set, this displays the full error text on the page to a Literal control with id="litError"</param>
    /// <param name="isSendErrorMail">Send error mail?</param>
    /// <param name="recipientEmail">Recipient's e-mail address</param>
    /// <param name="recipientName">Recipient's name</param>
    /// <param name="senderEmail">Sender's e-mail address</param>
    /// <param name="senderName">Sender's name</param>
    /// <param name="smtpServer">Name of smtp server</param>
    /// <param name="applicationName">Name of application</param>
    /// <example>Logger logger = new Logger();
    /// logger.Storage = Logger.StorageType.MySQL;
    /// logger.LogError(ex, _dsn, Page, false, true, "john@dot.com", "John Dot", "bevs@bevs.dk", "Bevs", "smtp.tele2.dk", "bogen.dk");</example>
    public void LogError(Exception ex, string _dsn, Page page, bool showCustomErrorMessageOnPage,
                            bool isSendErrorMail, string recipientEmail, string recipientName, string senderEmail,
                            string senderName, string smtpServer, string applicationName)
    {
        string FullErrorText = string.Empty;
        MySqlCommand mysql = null;
        MySqlDataReader reader = null;
        MySqlConnection conn = null;
        OleDb ole = null;
        SqlServer sql = null;
        IDataParameter[] p = null;

        FullErrorText = WriteEntry("ErrorMessage", ex.Message.ToString());
        FullErrorText += WriteEntry("GetBaseException", ex.GetBaseException().ToString());
        if (ex.InnerException != null)
            { FullErrorText += WriteEntry("InnerException", ex.InnerException.ToString()); }
        FullErrorText += WriteEntry("StackTrace", ex.StackTrace.ToString());
        FullErrorText += WriteEntry("GetType", ex.GetType().Name);
        FullErrorText += WriteEntry("HTTP_REFERER", HttpContext.Current.Request.ServerVariables["HTTP_REFERER"]);
        FullErrorText += WriteEntry("SCRIPT_NAME", HttpContext.Current.Request.ServerVariables["SCRIPT_NAME"]);
        FullErrorText += WriteEntry("Server.MachineName", HttpContext.Current.Server.MachineName);
        FullErrorText += WriteEntry("ErrorSource", ex.Source);

        FullErrorText += WriteHeading("Session(s)");
        try
        {
            foreach (object o in page.Session)
            {
                try { FullErrorText += WriteValue(o.ToString() + ": " + page.Session[o.ToString()]); }
                catch
                {
                    try { FullErrorText += WriteValue(o.GetType().Name + ": " + o.GetType().ToString()); }
                    catch (Exception ex2) { FullErrorText += WriteValue("Not able to read Session element due to exception: " + ex2.ToString()); }
                }
            }
        }
        catch { FullErrorText += "..on Session object"; }

        FullErrorText += WriteHeading("Application(s)");
        foreach (object o in HttpContext.Current.Application)
        {
            try { FullErrorText += WriteValue(o.ToString() +": "+ HttpContext.Current.Application[o.ToString()]); }
            catch
            {
                try { FullErrorText += WriteValue(o.GetType().Name + ": " + o.GetType().ToString()); }
                catch (Exception ex2) { FullErrorText += WriteValue("Not able to read Application element due to exception: "+ ex2.ToString()); }
            }
        }

        FullErrorText += WriteHeading("Querystring");
        foreach (object o in HttpContext.Current.Request.QueryString)
        {
            try { FullErrorText += WriteValue(o.ToString() +": "+ HttpContext.Current.Request.QueryString[o.ToString()]); }
            catch (Exception ex2) { FullErrorText += WriteValue("Not able to read Query string element due to exception: " + ex2.ToString()); }
        }

        FullErrorText += WriteHeading("Form");
        foreach (object o in HttpContext.Current.Request.Form)
        {
            try { FullErrorText += WriteValue(o.ToString() + ": " + HttpContext.Current.Request.Form[o.ToString()]); }
            catch (Exception ex2) { FullErrorText += WriteValue("Not able to read Form element due to exception: " + ex2.ToString()); }
        }

                if (_storage == StorageType.Access)
                {
                    strSQL =  " INSERT INTO [Errors]";
					strSQL += "			  ( Message, ";
					strSQL += "				Page, ";
					strSQL += "				FullErrorText, ";
					strSQL += "				ErrorDate )";
					strSQL += "	   VALUES ( ?Message, ";
					strSQL += "				?Page, ";
					strSQL += "				?FullErrorText, ";
                    strSQL += "				?ErrorDate ) ";

                    using (conn = new MySqlConnection(_dsn))
                    {
                        using (mysql = new MySqlCommand(strSQL, conn))
                        {
                            mysql.Parameters.Add("?Message", MySqlDbType.VarChar, 250).Value = "SendMail";
                            mysql.Parameters.Add("?Page", MySqlDbType.VarChar, 500).Value = "SendMail";
                            mysql.Parameters.Add("?FullErrorText", MySqlDbType.VarChar, 500).Value = FullErrorText;
                            mysql.Parameters.Add("?ErrorDate", MySqlDbType.DateTime).Value = DateTime.Now;

                            conn.Open();
                            mysql.ExecuteNonQuery();
                        }
                    }

                }
                else if (_storage == StorageType.SQLServer)
                {
                    strSQL = @" INSERT INTO [Errors]
										(Message, 
										 Page, 
										 FullErrorText, 
										 ErrorDate )
								 VALUES (@Message, 
										 @Page, 
										 @FullErrorText, 
										 @ErrorDate ) ";
                    sql = new SqlServer();
                    p = new IDataParameter[4];
                    p[0] = sql.GetParameter("@Message", "SendMail");
                    p[1] = sql.GetParameter("@Page", "SendMail");
                    p[2] = sql.GetParameter("@FullErrorText", FullErrorText);
                    p[3] = sql.GetParameter("@ErrorDate", DateTime.Now.ToString());
                    sql.ExecuteNonQuery(_dsn, System.Data.CommandType.Text, strSQL, p);
                }
                else if (_storage == StorageType.MySQL)
                {
                    try
                    {
                        strSQL = @"INSERT INTO errors 
                                   (ID,
                                    Message, 
                                    Page, 
                                    FullErrorText, 
                                    ErrorDate) 
                            VALUES (?ID,
                                    ?Message, 
                                    ?Page, 
                                    ?FullErrorText, 
                                    ?ErrorDate) ";

                        conn = new MySqlConnection(_dsn);
                        mysql = new MySqlCommand(strSQL, conn);

                        Guid errorID = Guid.NewGuid();
                        mysql.Parameters.Add("?ID", MySqlDbType.VarChar, 36).Value = errorID;
                        mysql.Parameters.Add("?Message", MySqlDbType.VarChar, 250).Value = "SendMail";
                        mysql.Parameters.Add("?Page", MySqlDbType.VarChar, 250).Value = "SendMail";
                        mysql.Parameters.Add("?FullErrorText", MySqlDbType.VarChar, 7000).Value = FullErrorText;
                        mysql.Parameters.Add("?ErrorDate", MySqlDbType.DateTime).Value = DateTime.Now.ToString();

                        conn.Open();
                        mysql.ExecuteNonQuery();
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
                else if (_storage == StorageType.File)
                    WriteErrorToLogFile(FullErrorText);
            

        /*catch (Exception ex2)
            {
                FullErrorText += WriteHeading("Exception occurred (DatabaseType="+ _database +")");
                FullErrorText += WriteValue("Database insertion could not be completed due to exception:");
                FullErrorText += WriteValue(ex2.ToString());
                FullErrorText += WriteHeading("strSQL");
                FullErrorText += WriteValue(strSQL);
            }*/
        


        if (isSendErrorMail && !String.IsNullOrEmpty(smtpServer))
        {
            //'-- Send error e-mail to ErrorEmailRecipient:
            Mailer mailer = new Mailer();
            mailer.SendMail(senderEmail, senderName, recipientEmail, recipientName, "ERROR [" + applicationName + "] occured at " + DateTime.Now, FullErrorText, true, smtpServer);
        }

        if (showCustomErrorMessageOnPage)
        {
            Literal error = (Literal)page.FindControl("litError");
            if (error != null)
            {
                error.Text += "<br /><div style='border:1px solid red; width:90%'>" + FullErrorText + "</div>";
                error.Visible = true;
            }
        }
    }

    /// <summary>
    /// Writes the error to a log file, "LogFile.txt", which is created if it doesn't exist.
    /// </summary>
    /// <param name="FullErrorText">The full error text</param>
    public void WriteErrorToLogFile(string FullErrorText)
    {
        System.IO.StreamWriter sw = null;
        try
        {
            sw = System.IO.File.AppendText(HttpContext.Current.Server.MapPath("LogFile.txt"));
            string strHeader = Environment.NewLine + Environment.NewLine + Environment.NewLine +
                               "#################################" + Environment.NewLine +
                               "#####  " + DateTime.Now + "  #####" + Environment.NewLine +
                               Environment.NewLine;
            sw.Write(ReplaceHtmlInErrorText(strHeader + FullErrorText));
        }
        finally
        {
            sw.Close();
        }
    }

    #region Logger Helper Methods

    /// <summary>
    /// Removes or replaces html tags line breaks or text "equivalents"
    /// </summary>
    /// <param name="ReplaceString">The string to be replaced</param>
    /// <returns>The corrected string</returns>
    public string ReplaceHtmlInErrorText(string ReplaceString)
    {
        ReplaceString = ReplaceString.Replace("<b>", "## ");
        ReplaceString = ReplaceString.Replace(":</b>", String.Empty);
        ReplaceString = ReplaceString.Replace("<i>", String.Empty);
        ReplaceString = ReplaceString.Replace("</i>", String.Empty);
        ReplaceString = ReplaceString.Replace("<br />", System.Environment.NewLine);
        return ReplaceString;
    }

    /// <summary>
    /// Writes an entry to the log consisting of an object and its value.
    /// Fx. the Message or InnerException of an exception, the HTTP_REFERER or the Session values.
    /// The method calls the two helper methods WriteHeading() and WriteValue().
    /// </summary>
    /// <param name="_object">The name of the object</param>
    /// <param name="_value">The value of the object</param>
    /// <returns>A html-formatted string suitable for display in an e-mail or on the page.</returns>
    public string WriteEntry(string _object, string _value)
    {
        string strEntry = WriteHeading(_object);
        strEntry += WriteValue(_value);
        return strEntry;
    }

    /// <summary>
    /// Writes the header part of the log entry, displaying the name of the object in bold.
    /// </summary>
    /// <param name="_object">The name of the object</param>
    /// <returns>The object name as an html-formatted suitable for display in an e-mail or directly on the page.</returns>
    public string WriteHeading(string _object)
        { return "<br /><br /><b>" + _object + ":</b>"; }

    /// <summary>
    /// Writes the value part of the log entry, displaying the value of the object.
    /// </summary>
    /// <param name="_value">The value of the object, fx. the value of the HTTP_REFERER object or the exception's Message property</param>
    /// <returns>The value as an html-formatted string suitable for display in an e-mail or directly on the page.</returns>
    public string WriteValue(string _value)
        { return "<br />" + _value; }

    #endregion

}