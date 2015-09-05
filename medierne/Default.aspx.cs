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

public partial class medierne_Default : BaseClass
{
    public QuoteType qt;

    protected void Page_Load(object sender, EventArgs e)
    {

        qt = new QuoteType();
        qt.QuoteTypeText = QuoteTypes.medierne.ToString();
        qt.Heading = "I medierne";
        qt.SubHeading = "I medierne";
        qt.LeadText = "5 minutes - nogle af de mere og mindre prominente eksponeringer i medierne gennem tiderne";

        /// Max 500 karakterer i keywords og max 15-20 ord!
        qt.MetaTagKeywords = "Odiøse navne, drengerøvshumor, spøjs humor, Karl Smart, Roger Klam, Elizabeth Luder, Rolv Bræk, Taber Industries, Niels Røv, Fims Quality Control, Chew Shit Fun, Bruce Tis";
        qt.MetaTagDescription = "Spøjs, underlødig drengerøvshumor for dig, der kan grine af rigtige navne som Karl Smart, Roger Klam og Niels Røv eller hvad med firmaet Taber Industries :o)";
        qt.PageTitle = "I medierne";

        qt.PageViews = UpdatePageViews(Request.Path);

        ShowQuotes1.QT = qt;
        //LinkArea1.QT = qt;

    }
}