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
        qt.LeadText = "Kender du fornemmelsen, når du lige retter ryggen, lægger hovedet en kende på skrå og retter blikket ind på uendeligt, mens du tænker: \"hmmm...\"?";
            qt.LeadText += "<br />I så fald er det øjeblik lige nu, så læn dig tilbage i stolen, sæt sixpencen på sned og nyd en hel stribe mærkværdigheder, spøjse udsagn og løjerlige sætninger.";

        /// Max 500 karakterer i keywords og max 15-20 ord!
        qt.MetaTagKeywords = "hmmm, spøjse spørgsmål, palindrom, endeskive, forkortelse, ordbog, mørkets hastighed";
        qt.MetaTagDescription = "Spøjse spørgsmål. Sveder fisk? Hvorfor staves palindrom ikke ens forfra og bagfra? Er der et andet ord for synonym?";
        qt.PageTitle = "Hmmm - spøjse spørgsmål.";

        qt.PageViews = UpdatePageViews(Request.Path);

        ShowQuotes1.QT = qt;
    }
}
