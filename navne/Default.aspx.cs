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

public partial class navne_Default : BaseClass
{
    public QuoteType qt;

    protected void Page_Load(object sender, EventArgs e)
    {

        qt = new QuoteType();
        qt.QuoteTypeText = QuoteTypes.navne.ToString();
        qt.Heading = "Odiøse Navne";
        //qt.SubHeading = "Latterlige og spøjse navne til overflod";
        qt.LeadText = "Links til folk med lettere underlødige navne, som ikke aner, at deres navne er for sjove på dansk. Spøjs og lettere underlødig humor i topklasse:";

        /*qt.SubText = "En sommerdag i '97 faldt jeg over Karl Smart på nettet. Efter at have sundet mig, begyndte jeg at søge efter lignende odiøse navne. Resten er historie.";
            qt.SubText += "<br />Andre neoklassiske odiøse navne som advokaten Brent Fisse, Bedford-borgmesteren Elizabeth Luder, gynækologen Roger Klam og guitarspilleren Peter Pik er kommet og gået, men minderne har vi stadig...";*/

        /// Max 500 karakterer i keywords og max 15-20 ord!
        qt.MetaTagKeywords = "Odiøse navne, drengerøvshumor, spøjs humor, Karl Smart, Roger Klam, Elizabeth Luder, Rolv Bræk, Taber Industries, Niels Røv, Fims Quality Control, Chew Shit Fun, Bruce Tis";
        qt.MetaTagDescription = "Spøjs, underlødig drengerøvshumor for dig, der kan grine af rigtige navne som Karl Smart, Roger Klam og Niels Røv eller hvad med firmaet Taber Industries :o)";
        qt.PageTitle = "Odiøse Navne - Rigtige folk med ægte, odiøse og virkeligt sjove navne";

        qt.PageViews = UpdatePageViews(Request.Path);

        ShowQuotes1.QT = qt;
        //LinkArea1.QT = qt;

    }
}
