using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Net.Mail;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

public partial class LinkArea : UsercontrolBaseClass
{
    private Quote _quote = new Quote();
    private QuoteType _qt;
    private int _count;

    protected void Page_Load(object sender, EventArgs e)
    {
        if(!Page.IsPostBack)
            BindData();
        revRecipientEmail.ValidationExpression = ConfigurationManager.AppSettings["EmailValidationRegularExpression"];

        #region Bookmarking Buttons
        delicious.NavigateUrl = "http://del.icio.us/post?url=bevs.dk/thyboe&title=Kurt+Thyboe+Generatoren";
        delicious.ImageUrl = "~/images/delicious.gif";
        delicious.ToolTip = "Tilføj til del.icio.us";
        magnolia.NavigateUrl = "http://ma.gnolia.com/bookmarklet/add?url=http://bevs.dk/thyboe&title=Kurt+Thyboe+Generatoren";
        magnolia.ImageUrl = "~/images/magnolia.gif";
        magnolia.ToolTip = "Tilføj til ma.gno.lia";
        google.NavigateUrl = "http://www.google.com/bookmarks/mark?op=edit&bkmk=http://bevs.dk/thyboe&title=Kurt+Thyboe+Generatoren";
        google.ImageUrl = "~/images/google.gif";
        google.ToolTip = "Tilføj til Google Bookmarks";
        digg.NavigateUrl = "http://digg.com/submit?phase=2&title=Kurt+Thyboe+Generatoren&url=http://bevs.dk/thyboe";
        digg.ImageUrl = "~/images/digg.gif";
        digg.ToolTip = "Tilføj til Digg";
        //stumple.NavigateUrl = "http://www.stumbleupon.com/submit?title=Kurt+Thyboe+Generatoren&url=http://bevs.dk/thyboe";
        //stumple.ImageUrl = "~/images/stumple.gif";
        //stumple.ToolTip = "Tilføj til StumpleUpon";
        yahoo.NavigateUrl = "http://bookmarks.yahoo.com/toolbar/savebm?opener=tb&u=http://bevs.dk/thyboe&t=Kurt+Thyboe+Generatoren";
        yahoo.ImageUrl = "~/images/yahoo.png";
        yahoo.ToolTip = "Tilføj til Yahoo Bookmarks";
        furl.NavigateUrl = "http://furl.net/storeIt.jsp?u=http://bevs.dk/thyboe&t=Kurt+Thyboe+Generatoren";
        furl.ImageUrl = "~/images/furl.png";
        furl.ToolTip = "Tilføj til Furl";
        linkedin.NavigateUrl = "http://www.linkedin.com/shareArticle?mini=true&url=http://bevs.dk/thyboe&title=Kurt+Thyboe+Generatoren";
        linkedin.ImageUrl = "~/images/LinkedIn.png";
        linkedin.ToolTip = "Tilføj til LinkedIn";

        /* URL: bevs.dk | Page title: Hvis du er til sjove citater og spøjs humor
         * Yahoo! Buzz:     http://buzz.yahoo.com/submit?submitUrl=http%3A%2F%2Fbevs.dk%2F&submitHeadline=bevs.dk%20-%20Hvis%20du%20er%20til%20sjove%20citater%20og%20sp%C3%B8js%20humor&submitSummary=&sref=shareaholic
         * mixx             http://www.mixx.com/submit?page_url=http%3A%2F%2Fbevs.dk%2F&partner=SHARE
         * myspace          http://www.myspace.com/Modules/PostTo/Pages/?l=3&u=http%3A%2F%2Fbevs.dk%2F&t=bevs.dk%20-%20Hvis%20du%20er%20til%20sjove%20citater%20og%20sp%C3%B8js%20humor&c=%3Cp%3EPosted%20via%20%3Ca%20href=%22http://shareaholic.com%22%3EShareaholic%3C/a%3E%3C/p%3E
         * twine            http://www.twine.com/bookmark/basic?u=http%3A%2F%2Fbevs.dk%2F&t=bevs.dk%20-%20Hvis%20du%20er%20til%20sjove%20citater%20og%20sp%C3%B8js%20humor&s=&v=1&k=&sref=shareaholic
         * facebook         http://www.facebook.com/sharer.php?u=http%3A%2F%2Fwww.compassdesigns.net%2Fjoomla-blog%2FWhy-50-Social-Bookmarking-Icons-is-50-Too-Many.html&t=Why+50+Social+Bookmarking+Icons+is+50+Too+Many
         * reddit           http://reddit.com/submit?url=http%3A%2F%2Fwww.compassdesigns.net%2Fjoomla-blog%2FWhy-50-Social-Bookmarking-Icons-is-50-Too-Many.html&title=Why+50+Social+Bookmarking+Icons+is+50+Too+Many
         * technorati       http://www.technorati.com/faves?add=http%3A%2F%2Fwww.compassdesigns.net%2Fjoomla-blog%2FWhy-50-Social-Bookmarking-Icons-is-50-Too-Many.html
         * friendfeed       http://friendfeed.com/share?url=http://mashable.com/2009/02/03/flickr-firefox-extensions/&title=15%20Great%20Flickr%20Extensions%20for%20Firefox
         * Newsvine         http://www.newsvine.com/_tools/seed&save?popoff=0&u=http://mashable.com/2009/02/03/flickr-firefox-extensions/&h=15%20Great%20Flickr%20Extensions%20for%20Firefox
         * 
         * 
         * Twitter:         check twitthat.com ud
         * 
         * */
        #endregion
    }

    private void BindData()
    {
        ddlQuotetype.DataSource = _qt.getQuoteTypes(1);
        ddlQuotetype.DataTextField = "LongName";
        ddlQuotetype.DataValueField = "TypeId";
        ddlQuotetype.DataBind();
    }

    protected void SendMail_Click(object sender, EventArgs e)
    {
        _quote.QuoteText = ValidateInput(tbxQuote.Text);
        _quote.Comment = "";
        _quote.Type = ValidateInput(ddlQuotetype.SelectedValue.ToString());
        _quote.Approved = 0;
        _quote.Save();

        Response.Redirect(Request.Url.ToString());
    }
    protected void btnTipFriend_Click(object sender, EventArgs e)
    {
        /*
        Mailer mailer = new Mailer();
        string senderEmail = "";
        string senderName = "";
        string recipientEmail = "";
        string recipientName = "";
        string subject = "";
        string body = "";
        bool isBodyHtml = true;
        string smtpServer = "";
        mailer.SendMail(senderEmail, senderName, recipientEmail, recipientName, subject, body, isBodyHtml, smtpServer);
        */
        string recipient = ValidateInput(tbxRecipientEmail.Text);
        string subject = QT.Heading;
        string body = "<a href='" + Request.Url.ToString() + "' title='" + QT.Heading + " på " + Request.Url.ToString() + "'>" + _qt.PageTitle + "</a><br /><br />" + _qt.LeadText;
        if (ValidateInput(tbxNote.Text) != string.Empty)
            body += ValidateInput(tbxNote.Text);
        SendMail(ConfigurationManager.AppSettings["MailFrom"], "En ven",
                                     recipient, recipient, subject, body, false, 
                                     ConfigurationManager.AppSettings["Smtp"]);
    }

    public QuoteType QT
    {
        get { return _qt; }
        set { _qt = value; }
    }

}
