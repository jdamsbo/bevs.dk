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
        qt.Heading = "Odi�se Navne";
        //qt.SubHeading = "Latterlige og sp�jse navne til overflod";
        qt.LeadText = "Links til folk med lettere underl�dige navne, som ikke aner, at deres navne er for sjove p� dansk. Sp�js og lettere underl�dig humor i topklasse:";

        /*qt.SubText = "En sommerdag i '97 faldt jeg over Karl Smart p� nettet. Efter at have sundet mig, begyndte jeg at s�ge efter lignende odi�se navne. Resten er historie.";
            qt.SubText += "<br />Andre neoklassiske odi�se navne som advokaten Brent Fisse, Bedford-borgmesteren Elizabeth Luder, gyn�kologen Roger Klam og guitarspilleren Peter Pik er kommet og g�et, men minderne har vi stadig...";*/

        /// Max 500 karakterer i keywords og max 15-20 ord!
        qt.MetaTagKeywords = "Odi�se navne, drenger�vshumor, sp�js humor, Karl Smart, Roger Klam, Elizabeth Luder, Rolv Br�k, Taber Industries, Niels R�v, Fims Quality Control, Chew Shit Fun, Bruce Tis";
        qt.MetaTagDescription = "Sp�js, underl�dig drenger�vshumor for dig, der kan grine af rigtige navne som Karl Smart, Roger Klam og Niels R�v eller hvad med firmaet Taber Industries :o)";
        qt.PageTitle = "Odi�se Navne - Rigtige folk med �gte, odi�se og virkeligt sjove navne";

        qt.PageViews = UpdatePageViews(Request.Path);

        ShowQuotes1.QT = qt;
        //LinkArea1.QT = qt;

    }
}
