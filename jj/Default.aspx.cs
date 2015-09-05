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
        qt.Heading = "J�rn & J�rgen Generator";
        //qt.SubHeading = "Riis er sat! ... eller er han k�rt ... ja, Riis er k�rt!";
        qt.LeadText = "Den definitive samling af Mader og Leth citater fra deres udgydelser under den traditionsrige kommentering af Tour de France.";
            qt.LeadText += "<br />J�rn &amp; J�rgen er efter min bedste overbevisning de eneste personager, som kan g�re selv <a href='../thyboe/'>boksekommentatorovermesteren Kurt Thyboe</a> rangen stridig som Danmarks mest pr�tenti�se og floskelfloromvundne s�tningsfabrikanter:";

        /// Max 500 karakterer i keywords og max 15-20 ord!
        qt.MetaTagKeywords = "J�rn Mader, J�rgen Leth, J�rn Let, sjove citater, generator, Tour de France, cykling, klassiker, etape, floskel, anekdote, s�tningsfabrikant, kommentator";
        qt.MetaTagDescription = "Autentiske sjove citater fra J�rn Mader &amp; J�rgen Leth i deres velmagtsdage som pr�tenti�se og floskelfloromvundne Tour de France kommentatorer.";
        qt.PageTitle = "J�rn &amp; J�rgen Generator - Sjove citater af J�rn Mader og J�rgen Leth fra cykelflosklernes overdrev";

        qt.PageViews = UpdatePageViews(Request.Path);
        qt.UrlFB = "https://www.facebook.com/pages/J%C3%B8rn-J%C3%B8rgen-Generator/140153299328076";

        ShowQuotes1.QT = qt;
    }
}
