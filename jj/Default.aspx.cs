using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

public partial class jj_Default : BaseClass
{
    public QuoteType qt;

    protected void Page_Load(object sender, EventArgs e)
    {
        qt = new QuoteType();
        qt.QuoteTypeText = QuoteTypes.jj.ToString();
        qt.Heading = "Jørn & Jørgen Generator";
        //qt.SubHeading = "Riis er sat! ... eller er han kørt ... ja, Riis er kørt!";
        qt.LeadText = "Den definitive samling af Mader og Leth citater fra deres udgydelser under den traditionsrige kommentering af Tour de France.";
            qt.LeadText += "<br />Jørn &amp; Jørgen er efter min bedste overbevisning de eneste personager, som kan gøre selv <a href='../thyboe/'>boksekommentatorovermesteren Kurt Thyboe</a> rangen stridig som Danmarks mest prætentiøse og floskelfloromvundne sætningsfabrikanter:";

        /// Max 500 karakterer i keywords og max 15-20 ord!
        qt.MetaTagKeywords = "Jørn Mader, Jørgen Leth, Jørn Let, sjove citater, generator, Tour de France, cykling, klassiker, etape, floskel, anekdote, sætningsfabrikant, kommentator";
        qt.MetaTagDescription = "Autentiske sjove citater fra Jørn Mader &amp; Jørgen Leth i deres velmagtsdage som prætentiøse og floskelfloromvundne Tour de France kommentatorer.";
        qt.PageTitle = "Jørn &amp; Jørgen Generator - Sjove citater af Jørn Mader og Jørgen Leth fra cykelflosklernes overdrev";

        qt.PageViews = UpdatePageViews(Request.Path);
        qt.UrlFB = "https://www.facebook.com/pages/J%C3%B8rn-J%C3%B8rgen-Generator/140153299328076";

        ShowQuotes1.QT = qt;
    }
}
