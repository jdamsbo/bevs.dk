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

public partial class MasterPage : System.Web.UI.MasterPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        bool displayGoogleAds = true;
        if (Request.Url.AbsoluteUri.Contains("navne")) { displayGoogleAds = false; }
        //GoogleAds.Text += "<br>"+ Request.Url.AbsoluteUri +"<br>";
        GoogleAds.Visible = displayGoogleAds;
    }

}
