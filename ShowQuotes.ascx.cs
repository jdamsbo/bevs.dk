using System;
using System.Configuration;
using System.Collections;
using System.Data;
using System.Threading;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using AjaxControlToolkit;
using MySql.Data.MySqlClient;


public partial class ShowQuotes : UsercontrolBaseClass
{
    private QuoteType _qt;
    private int _count;
    public Quote quote;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!_qt.QuoteTypeText.Equals(QuoteType.QuoteTypes.medierne.ToString()))
        {
            quote = new Quote();
            bool hasGuid = false;
            if (Request.Params["id"] != null) hasGuid = true;

            if (hasGuid)
                quote = quote.GetQuote(Request.Params["id"]);
            else
                quote = quote.GetRandomQuote(QT.QuoteTypeText);
        }
        /// "I medierne":
        else { }

        if (!Page.IsPostBack)
        {
            _init();
        }
    }

    /// <summary>
    /// Set the alignment of the stars in the rating control
    /// </summary>
    /// <param name="e">argument</param>
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
    }


    protected void _init()
    {
        try
        {
            /// Head section
            HtmlMeta metaKeywords = new HtmlMeta();
            metaKeywords.Name = "keywords";
            metaKeywords.Content = QT.MetaTagKeywords;
            Page.Master.Page.Header.Controls.Add(metaKeywords);
            HtmlMeta metaContent = new HtmlMeta();
            metaContent.Name = "description";
            metaContent.Content = QT.MetaTagDescription;
            Page.Master.Page.Header.Controls.Add(metaContent);
            HtmlMeta metaCopyright = new HtmlMeta();
            metaCopyright.Name = "copyright";
            metaCopyright.Content = "Copyright 1997-" + DateTime.Now.Year.ToString() + ": bevs.dk";
            Page.Master.Page.Header.Controls.Add(metaCopyright);
            Page.Title = QT.PageTitle;

            /// Top-of-page texts
            h1.InnerHtml = QT.Heading;
            //subheading.InnerHtml = QT.SubHeading;
            leadtext.InnerHtml = QT.LeadText;
            lblPageViews.Text = QT.PageViews.ToString("#,#") + " visninger";

            /// Quote
            if (_qt.QuoteTypeText.Equals(QuoteType.QuoteTypes.medierne.ToString()))
            {   /// "I medierne" is only a "cheat"-quote - only uses the Quote class to benefit from functions such as PageViews and such.
                /// lblQuote og lblComment
                string txtQuote = "<a href='http://archive.hitmanforum.com/index.php/topic/32040-wierd-website-on-one-of-the-newspaper/' target='blank'>Hitman: Blood Money</a>";
                txtQuote += "<br /><a href='../politiken.pdf' target='blank'>Politiken: Dr. Rumpe, formoder jeg</a>";
                txtQuote += "<br /><a href='http://www.bt.dk/sport/poesi-og-pladder-fra-leth-og-mader' target='_blank'>B.T.: Poesi og pladder fra Leth og Mader</a>";
                lblQuote.Text = txtQuote;
                lblNoQuotes.Visible = false;
            }
            else
            {   /// Real quote (JJ, Thyboe, Chuck, Navne, Hmmm, ...
                quote.Type = QT.QuoteTypeText;
                _count = QT.Count;
                lblQuote.Text = quote.QuoteText.Trim();
                bool addCitationMarks = false;
                if (quote.Type.Equals("thyboe") || quote.Type.Equals("jj"))
                    addCitationMarks = true;
                if (addCitationMarks)
                {
                    HtmlControl quoteID = (HtmlControl)this.FindControl("quoteID");
                    quoteID.Attributes["class"] = "realquote";
                }
                lblComment.Text = quote.Comment.Trim();
                if (lblComment.Text != string.Empty)
                    lblComment.Text = "- " + lblComment.Text;

                lblNoQuotes.Text = " | "+ Count.ToString() + " citater";
            }

            /// 2 or 3 bottom boxes: 1) Update/send quote 2) Share 3) FB
            buttonrowFB.InnerHtml = String.Empty;
            bool showFB = false;
            if (QT.UrlFB != String.Empty) showFB = true;
            if(showFB)
            {
                buttonrowFB.InnerHtml = "<div class='fb-like' data-href='https://www.facebook.com/pages/J%C3%B8rn-J%C3%B8rgen-Generator/140153299328076' data-send='true' data-width='200' data-show-faces='true'></div>";
                buttonrowReload.Attributes["class"] = "sub-hero-unit shu-span4";
                buttonrowShare.Attributes["class"] = "sub-hero-unit shu-span4";
                buttonrowFB.Attributes["class"] = "sub-hero-unit shu-span4";
            }
            else
            {
                buttonrowReload.Attributes["class"] = "sub-hero-unit shu-span6";
                buttonrowShare.Attributes["class"] = "sub-hero-unit shu-span6";
                buttonrowFB.Visible = false;
            }
        }
        catch (Exception ex)
        {
            LogError(ex);
        }
        finally
        {
            ;
        }

    }

    public QuoteType QT
    {
        get { return _qt; }
        set { _qt = value; }
    }
    public int Count
    {
        get { return _count; }
    }



}
