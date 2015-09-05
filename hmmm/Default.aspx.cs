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

public partial class hmmm_Default : BaseClass
{
    public QuoteType qt;

    protected void Page_Load(object sender, EventArgs e)
    {
        qt = new QuoteType();
        qt.QuoteTypeText = QuoteTypes.hmmm.ToString();
        qt.Heading = "Hmmm";
        //qt.SubHeading = "Sveder fisk?";
        qt.LeadText = "Kender du fornemmelsen, n�r du lige retter ryggen, l�gger hovedet en kende p� skr� og retter blikket ind p� uendeligt, mens du t�nker: \"hmmm...\"?";
            qt.LeadText += "<br />I s� fald er det �jeblik lige nu, s� l�n dig tilbage i stolen, s�t sixpencen p� sned og nyd en hel stribe m�rkv�rdigheder, sp�jse udsagn og l�jerlige s�tninger.";

        /// Max 500 karakterer i keywords og max 15-20 ord!
        qt.MetaTagKeywords = "hmmm, sp�jse sp�rgsm�l, palindrom, endeskive, forkortelse, ordbog, m�rkets hastighed";
        qt.MetaTagDescription = "Sp�jse sp�rgsm�l. Sveder fisk? Hvorfor staves palindrom ikke ens forfra og bagfra? Er der et andet ord for synonym?";
        qt.PageTitle = "Hmmm - sp�jse sp�rgsm�l.";

        qt.PageViews = UpdatePageViews(Request.Path);

        ShowQuotes1.QT = qt;
    }
}
