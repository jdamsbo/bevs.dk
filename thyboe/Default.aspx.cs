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

public partial class thyboe_Default : BaseClass
{
    public QuoteType qt;

    protected void Page_Load(object sender, EventArgs e)
    {

        qt = new QuoteType();
        qt.QuoteTypeText = QuoteTypes.thyboe.ToString();
        qt.Heading = "Kurt Thyboe Generator";
        //qt.SubHeading = "Hvad så, champ?";
        qt.LeadText =  "Den ultimative Kurt Thyboe tribute! Her kan du fornøje dig med sjove citater og de herligste replikker fra boksekommentatorernes absolutte stormester Kurt Thyboe.";
            qt.LeadText += "<br />Det er showtime, fighttime, action...";
        
        /// Max 500 karakterer i keywords og max 15-20 ord!
        qt.MetaTagKeywords = "Kurt Thyboe, Kurt Tybo, Kurt Thybo, generator, sjove citater, boksning, kommentator, floskel, anekdote, ringside, ringens mestre, storyteller, reporter, showtime, champ, fight";
        qt.MetaTagDescription = "Ægte sjove citater fra reporter Kurt Thyboe, boksekommentatorernes absolutte stormester, svulstighedens mesterfighter og flosklernes superchamp!";
        qt.PageTitle = "Kurt Thyboe Generator - Sjove citater fra Kurt Thyboe fra flosklernes overdrev";

        qt.PageViews = UpdatePageViews(Request.Path);

        ShowQuotes1.QT = qt;

    }
}
