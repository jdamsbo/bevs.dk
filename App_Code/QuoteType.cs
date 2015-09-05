using System;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using MySql.Data.MySqlClient;

/// <summary>
/// Summary description for QuoteType
/// </summary>
public class QuoteType : BaseClass
{
    private string _quoteTypeText;
    private string _heading;
    private string _subHeading;
    private string _leadText;
    private string _metaTagKeywords;
    private string _metaTagDescription;
    private string _pageTitle;
    private int _pageViews;
    private int _count;
    private string _urlFB = String.Empty;

	public QuoteType()
	{
        ; //Constructor
	}

    public string QuoteTypeText
    {
        get { return _quoteTypeText; }
        set { _quoteTypeText = value; }
    }
    public string Heading
    {
        get { return _heading; }
        set { _heading = value; }
    }
    public string SubHeading
    {
        get { return _subHeading; }
        set { _subHeading = value; }
    }
    public string LeadText
    {
        get { return _leadText; }
        set { _leadText = value; }
    }
    public string MetaTagKeywords
    {
        get { return _metaTagKeywords; }
        set { _metaTagKeywords = value; }
    }
    public string MetaTagDescription
    {
        get { return _metaTagDescription; }
        set { _metaTagDescription = value; }
    }
    public string PageTitle
    {
        get { return _pageTitle; }
        set { _pageTitle = value; }
    }
    public int PageViews
    {
        get { return _pageViews; }
        set { _pageViews = value; }
    }
    public int Count
    {
        get { _getCount(); return _count; }
    }
    public string UrlFB
    {
        get { return _urlFB; }
        set { _urlFB = value; }
    }

    /// <summary>
    /// Retrieve a list of quotetypes. The dataset can be sorted by 
    /// </summary>
    /// <param name="inact0_act1_all2">0, 1 or 2. 0 = inactive, 1 = active, 2 = all</param>
    /// <returns>A DataSet with LongName and TypeId for the quotetypes</returns>
    public DataSet getQuoteTypes(int inact0_act1_all2)
    {
        MySqlDataAdapter adapter = null;
        DataSet dsTypes = new DataSet();
        try
        {
            strSQL = "   SELECT LongName, TypeId ";
            strSQL += "    FROM types ";
            if (inact0_act1_all2 < 2)
                strSQL += "   WHERE Active = ?Active ";

            using (conn = new MySqlConnection(_dsn))
            {
                using (adapter = new MySqlDataAdapter(strSQL, conn))
                {
                    if (inact0_act1_all2 < 2)
                        adapter.SelectCommand.Parameters.Add("?Active", MySqlDbType.Int16, 11).Value = inact0_act1_all2;
                    conn.Open();
                    adapter.Fill(dsTypes);
                }
            }
        }
        catch (Exception ex)
        {
            LogError(ex);
        }
        return dsTypes;
    }

    private void _getCount()
    {
        MySqlCommand mysql = null;
        MySqlDataReader reader = null;
        try
        {
            strSQL = "   SELECT count(*) AS Total ";
            strSQL += "     FROM quotes ";
            strSQL += "    WHERE Type = ( SELECT TypeId FROM types WHERE Type = ?Type ) ";
            strSQL += "      AND Approved = 1 ";

            using (conn = new MySqlConnection(_dsn))
            {
                using (mysql = new MySqlCommand(strSQL, conn))
                {
                    mysql.Parameters.Add("?Type", MySqlDbType.VarChar, 20).Value = QuoteTypeText;
                    conn.Open();
                    using (reader = mysql.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            _count = Convert.ToInt16(reader["Total"]);
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            LogError(ex);
        }
    }


}
