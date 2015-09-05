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

public partial class chuck_Default : BaseClass
{
    public QuoteType qt;

    protected void Page_Load(object sender, EventArgs e)
    {
        qt = new QuoteType();
        qt.QuoteTypeText = QuoteTypes.chuck.ToString();
        qt.Heading = "Chuck Norris Generator";
        //qt.SubHeading = "Chuck Norris, verdens definitivt sejeste karl!";
        qt.LeadText = "Tag Arnold, bland ham op med van Damme, tilsæt et skvæt Mr. T, krydr det med lidt Robert de Niro, mix det op med Steven Seagal og server det hele med et bæger Sylvester Stallone. Så har du ikke bare verdens bedste method actor, stuntman, voldstosse eller karatechamp, nej det er galaksens skrappeste gut.";

        /// Max 500 karakterer i keywords og max 15-20 ord!
        qt.MetaTagKeywords = "Chuck Norris, generator, sjove fakta";
        qt.MetaTagDescription = "Sjove fakta om Chuck Norris, manden som er sejere end læder og hårdere end stål.";
        qt.PageTitle = "Chuck Norris Generator - Sjove fakta om Chuck Norris, verdens definitivt sejeste karl!";

        qt.PageViews = UpdatePageViews(Request.Path);

        ShowQuotes1.QT = qt;
    }
}
