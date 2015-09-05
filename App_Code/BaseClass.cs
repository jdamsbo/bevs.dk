using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

using MySql.Data.MySqlClient;

/// <summary>
/// Summary description for BaseClass
/// </summary>
public class BaseClass : System.Web.UI.Page
{
    public BaseClass()
    {
        _init();
    }

    protected string _dsn;
    public MySqlConnection conn = null;
    public string strSQL;

    private void _init()
    {
        try
        {
            _dsn = ConfigurationManager.ConnectionStrings["mysqlConnString"].ConnectionString;
            //conn = new MySqlConnection(_dsn);
        }
        catch (Exception ex)
        {
            LogError(ex);
        }
    }

    public void LogError(Exception ex)
    {
        Logger logger = new Logger();
        logger.Storage = Logger.StorageType.File;
        logger.LogError(ex, _dsn, Page, false);
    }

    public enum QuoteTypes
    {
        thyboe,
        jj,
        chuck,
        hmmm,
        inspiration,
        navne,
        medierne
    }
    protected int UpdatePageViews(string _page)
    {
        int pageViews = 0;
        MySqlCommand mysql = null;
        MySqlDataReader reader = null;
        bool isRecords;
        try
        {
            /// Update PageViews
            strSQL = "   UPDATE pageviews ";
            strSQL += "     SET PageViews = PageViews + 1 ";
            strSQL += "    WHERE Page = ?Page ";

            using (conn = new MySqlConnection(_dsn))
            {
                using (mysql = new MySqlCommand(strSQL, conn))
                {
                    mysql.Parameters.Add("?Page", MySqlDbType.VarChar, 1000).Value = _page;
                    conn.Open();
                    using (reader = mysql.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        isRecords = Convert.ToBoolean(reader.RecordsAffected);
                    }
                }
            }

            if (!isRecords)
            {   /// Page not viewed/in database yet
                strSQL = "  INSERT INTO pageviews ";
                strSQL += "           ( PageViews, Page ) ";
                strSQL += "    VALUES ( 1, ?Page) ";

                using (conn = new MySqlConnection(_dsn))
                {
                    using (mysql = new MySqlCommand(strSQL, conn))
                    {
                        mysql.Parameters.Add("?page", MySqlDbType.VarChar, 1000).Value = _page;
                        conn.Open();
                        using (reader = mysql.ExecuteReader(CommandBehavior.CloseConnection)) { }
                    }
                }
            }

            /// Get PageViews
            strSQL = "   SELECT PageViews ";
            strSQL += "     FROM pageviews ";
            strSQL += "    WHERE Page = ?Page ";

            using (conn = new MySqlConnection(_dsn))
            {
                using (mysql = new MySqlCommand(strSQL, conn))
                {
                    mysql.Parameters.Add("?page", MySqlDbType.VarChar, 1000).Value = _page;
                    conn.Open();
                    using (reader = mysql.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            pageViews = Convert.ToInt32(reader["PageViews"]);
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            LogError(ex);
        }
        return pageViews;
    }
}
