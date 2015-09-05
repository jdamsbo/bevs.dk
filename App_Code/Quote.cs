using System;
using System.Data;
using System.Collections;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using MySql.Data.MySqlClient;

/// <summary>
/// Summary description for Quote
/// </summary>
public class Quote : BaseClass
{
    private int _quoteId;
    private string _quoteGuid;
    private string _quoteText;
    private string _comment;
    private string _type;
    private int _approved;
    private double _rating;

	public Quote()
	{
        ; //Constructor
    }

    public int QuoteId
    {
        get { return _quoteId; }
        set { _quoteId = value; }
    }
    public string QuoteGuid
    {
        get { return _quoteGuid; }
        set { _quoteGuid = value; }
    }
    public string QuoteText
    {
        get { return _quoteText; }
        set { _quoteText = value; }
    }
    public string Comment
    {
        get { return _comment; }
        set { _comment = value; }
    }
    public string Type
    {
        get { return _type; }
        set { _type = value; }
    }
    public int Approved
    {
        get { return _approved; }
        set { _approved = value; }
    }
    public double Rating
    {
        get { return _rating; }
        set { _rating = value; }
    }

    public Quote GetRandomQuote(string _type)
    {
        MySqlCommand mysql = null;
        MySqlDataReader reader = null;
        Quote quote = new Quote();
        try
        {
            strSQL  = "   SELECT * ";
            strSQL += "     FROM quotes ";
            strSQL += "    WHERE Type = ( SELECT TypeId FROM types WHERE Type = ?Type ) ";
            strSQL += "      AND Approved = 1 ";
            strSQL += " ORDER BY rand() limit 1 ";

            using (conn = new MySqlConnection(_dsn))
            {
                using (mysql = new MySqlCommand(strSQL, conn))
                {
                    mysql.Parameters.Add("?Type", MySqlDbType.VarChar, 20).Value = _type;
                    conn.Open();
                    using (reader = mysql.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            quote.QuoteId = Convert.ToInt16(reader["QuoteId"]);
                            quote.QuoteGuid = reader["guid"].ToString();
                            quote.QuoteText = reader["QuoteText"].ToString();
                            quote.Comment = reader["Comment"].ToString();
                            quote.Approved = Convert.ToInt16(reader["Approved"]);
                            _getRating(quote);
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            LogError(ex);
        }
        return quote;
    }

    public Quote GetQuote(string _quoteGuid)
    {
        MySqlCommand mysql = null;
        MySqlDataReader reader = null;
        Quote quote = new Quote();
        try
        {
            strSQL = "   SELECT * ";
            strSQL += "     FROM quotes ";
            strSQL += "    WHERE guid = ?Guid ";
            strSQL += "      AND Approved = 1 ";

            using (conn = new MySqlConnection(_dsn))
            {
                using (mysql = new MySqlCommand(strSQL, conn))
                {
                    mysql.Parameters.Add("?Guid", MySqlDbType.VarChar, 36).Value = _quoteGuid;
                    conn.Open();
                    using (reader = mysql.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            quote.QuoteGuid = reader["guid"].ToString();
                            quote.QuoteText = reader["QuoteText"].ToString();
                            quote.Comment = reader["Comment"].ToString();
                            quote.Approved = Convert.ToInt16(reader["Approved"]);
                            _getRating(quote);
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            LogError(ex);
        }
        return quote;
    }

    public void Save()
    {
        MySqlCommand mysql = null;
        try
        {
            /// Insert into quotes
            strSQL = " INSERT INTO quotes ";
            strSQL += "            ( Guid, ";
            strSQL += "              QuoteText, ";
            strSQL += "              Comment, ";
            strSQL += "              Type, ";
            strSQL += "              Approved, ";
            strSQL += "              CreatedDate ) ";
            strSQL += "     VALUES ( ?Guid, ";
            strSQL += "              ?QuoteText, ";
            strSQL += "              ?Comment, ";
            strSQL += "              ?Type, ";
            strSQL += "              ?Approved, ";
            strSQL += "              ?CreatedDate ) ";

            using (conn = new MySqlConnection(_dsn))
            {
                using (mysql = new MySqlCommand(strSQL, conn))
                {
                    //MySql.Data.Types.MySqlDateTime mySqlDate = (MySql.Data.Types.MySqlDateTime)reader["DateOfBirth"];
                    //_dateOfBirth = mySqlYear.GetDateTime();

                    mysql.Parameters.Add("?Guid", MySqlDbType.VarChar, 36).Value = _quoteGuid;
                    mysql.Parameters.Add("?QuoteText", MySqlDbType.VarChar, 250).Value = _quoteText;
                    mysql.Parameters.Add("?Comment", MySqlDbType.VarChar, 500).Value = _comment;
                    mysql.Parameters.Add("?Type", MySqlDbType.Int16, 11).Value = Convert.ToInt16(_type);
                    mysql.Parameters.Add("?Approved", MySqlDbType.Int16, 11).Value = _approved;
                    mysql.Parameters.Add("?CreatedDate", MySqlDbType.DateTime).Value = DateTime.Now;

                    conn.Open();
                    mysql.ExecuteNonQuery();
                }
            }
        }
        catch (Exception ex)
        {
            LogError(ex);
        }
    }

    public void SaveRating(int _newRating, int _quoteId)
    {
        MySqlCommand mysql = null;
        MySqlDataReader reader = null;
        try
        {
            strSQL = "  INSERT INTO ratings ";
            strSQL += "           ( Rating, Quote, CreatedDate ) ";
            strSQL += "    VALUES ( ?Rating, ?QuoteId, ?CreatedDate) ";

            using (conn = new MySqlConnection(_dsn))
            {
                using (mysql = new MySqlCommand(strSQL, conn))
                {
                    mysql.Parameters.Add("?Rating", MySqlDbType.Int16, 11).Value = _newRating;
                    mysql.Parameters.Add("?QuoteId", MySqlDbType.Int16, 11).Value = _quoteId;
                    mysql.Parameters.Add("?CreatedDate", MySqlDbType.DateTime).Value = DateTime.Now;
                    conn.Open();
                    using (reader = mysql.ExecuteReader()) { }
                }
            }
        }
        catch (Exception ex)
        {
            LogError(ex);
        }
    }

    public void _getRating(Quote quote)
    {
        MySqlCommand mysql = null;
        MySqlDataReader reader = null;
        try
        {
            strSQL =  " SELECT round(sum(rating) / count(*), 2) AS RatingWithDecimals, ";
            strSQL += "        round(sum(rating) / count(*)) AS RatingRounded ";
            strSQL += "   FROM ratings ";
            strSQL += "  WHERE QuoteId = ?QuoteId ";

            using (conn = new MySqlConnection(_dsn))
            {
                using (mysql = new MySqlCommand(strSQL, conn))
                {
                    mysql.Parameters.Add("?QuoteId", MySqlDbType.Int16, 11).Value = quote.QuoteId;
                    conn.Open();
                    using (reader = mysql.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (!Convert.IsDBNull(reader["RatingWithDecimals"]))
                                quote.Rating = Convert.ToDouble(reader["RatingWithDecimals"]);
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
