<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ShowQuotes.ascx.cs" Inherits="ShowQuotes" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<h1 id="h1" runat="server" style="border-bottom:1px solid black"></h1>
<div class="hittext">
    <asp:Label Visible="true" ID="lblPageViews" runat="server"></asp:Label>
    <asp:Label ID="lblNoQuotes" runat="server"></asp:Label>
</div>

<p id="leadtext"><div id="leadtext" runat="server"></div></p>

<br />
<div class="hero-unit">
    <blockquote runat="server" id="quoteID"><asp:Label ID="lblQuote" runat="server"></asp:Label></blockquote>
    <p class="quotetext_author"><asp:Label ID="lblComment" runat="server"></asp:Label></p>
</div>

<hr />

<!-- Row of columns -->

<div class="row">
    <div class="sub-hero-unit" id="buttonrowReload" runat="server">
        <h2></h2>
        <p><a href='javascript:window.location.reload();' class="btn btn-primary btn-large" title='Du kan opdatere ved at klikke her eller trykke på F5'><i class="icon-refresh icon-white"></i> F5 for nye replikker</a></p>
        <br /><p><a href="mailto:nytcitat@bevs.dk" class="btn btn-primary btn-large" title="Send en email til nytcitat@bevs.dk - du bliver krediteret"><i class="icon-envelope icon-white"></i> Har du et godt citat?</a></p>
    </div>
    <div class="sub-hero-unit" id="buttonrowShare" runat="server">
        <span class='st_facebook_large' displayText='Facebook'></span>
        <span class='st_twitter_large' displayText='Tweet'></span>
        <span class='st_googleplus_large' displayText='Google +'></span>
        <span class='st_email_large' displayText='Email'></span>
        <span class='st_sharethis_large' displayText='ShareThis'></span>
        <hr />
        <a href="https://twitter.com/share" class="twitter-share-button" data-via="jdamsbo" 
            data-url="http://bevs.dk/navne/hdjejejddkdkdkfjfj/default.aspx?id=98763D8D-FB71-42AF-8EB7-2795BA6AFA35" 
            data-text="Dele-teksten... bla bla bla bla bla..." 
            data-size="large">Tweet</a>
        <script>
            !function (d, s, id)
            {
                var js, fjs = d.getElementsByTagName(s)[0], p = /^http:/.test(d.location) ? 'http' : 'https';
                if (!d.getElementById(id))
                {
                    js = d.createElement(s); js.id = id; js.src = p + '://platform.twitter.com/widgets.js'; fjs.parentNode.insertBefore(js, fjs);
                }
            }
            (document, 'script', 'twitter-wjs');
        </script>
    </div>
    <div class="sub-hero-unit" id="buttonrowFB" runat="server"></div>
</div><!-- /row -->
