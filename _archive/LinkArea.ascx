<%@ Control Language="C#" AutoEventWireup="true" CodeFile="LinkArea.ascx.cs" Inherits="LinkArea" %>

<div style="width:100%;border:3px solid red"></div>

            <div id="linkarea2" style="text-align:center; padding-top:20px;">
                <asp:HyperLink runat="server" ID="delicious"></asp:HyperLink>&nbsp;
                <asp:HyperLink runat="server" ID="magnolia"></asp:HyperLink>&nbsp;
                <asp:HyperLink runat="server" ID="google"></asp:HyperLink>&nbsp;
                <asp:HyperLink runat="server" ID="digg"></asp:HyperLink>&nbsp;
                <!--<asp:HyperLink runat="server" ID="stumple"></asp:HyperLink>&nbsp;-->
                <asp:HyperLink runat="server" ID="yahoo"></asp:HyperLink>&nbsp;
                <asp:HyperLink runat="server" ID="furl"></asp:HyperLink>&nbsp;
                <asp:HyperLink runat="server" ID="linkedin"></asp:HyperLink>
                | <a href='#' onclick='document.getElementById("mailafriend").className="maildisplay"; docment.getElementById("mailarea").className="mailhide"' title='Send linket til en ven'><span style='text-decoration:none; vertical-align:bottom'><asp:Image ID='email2' runat='server' ImageUrl='~/images/email.gif' AlternateText='Send linket til en ven' ImageAlign="AbsBottom" /></span></a> <a href='#' onclick='document.getElementById("mailafriend").className="maildisplay"; docment.getElementById("mailarea").className="mailhide"' title='Send linket til en ven'>Tip en ven</a>
                | <a href='#' onclick='document.getElementById("mailarea").className="maildisplay"; document.getElementById("mailafriend").className="mailhide"' title='Tilføj citat'><span style='text-decoration:none; vertical-align:bottom'><asp:Image ID='email' runat='server' ImageUrl='~/images/email.gif' AlternateText='Tilføj citat' ImageAlign="AbsBottom" /></span></a> <a href='#' onclick='document.getElementById("mailarea").className="maildisplay"; document.getElementById("mailafriend").className="mailhide"' title='Tilføj citat'>Tilføj citat</a>
            </div>



            <!-- Tilføj citat -->
            <div id="mailarea" class="mailhide">
                <div class="mailinner">
                    <strong>Tilføj citat</strong>
                    <div>
                        <asp:Label ID="lblName" runat="server" CssClass="maillabels" ToolTip="Hvis du vil krediteres">Dit navn:</asp:Label>
                        <asp:TextBox ID="tbxName" runat="server" CssClass="mailvalue" ToolTip="Hvis du vil krediteres"></asp:TextBox>
                    </div>
                    <div>
                        <asp:Label ID="lblQuoteType" runat="server" CssClass="maillabels">Hvem:</asp:Label>
                        <asp:DropDownList ID="ddlQuotetype" runat="server" CssClass="mailvalue">
                        </asp:DropDownList>
                    </div>
                    <div>
                        <asp:Label ID="lblquote" runat="server" CssClass="maillabels">Citat:</asp:Label>
                        <asp:TextBox ID="tbxQuote" runat="server" CssClass="mailvalue" TextMode="MultiLine" Rows="5"></asp:TextBox>
                    </div>
                    <div style="text-align: right">
                        <asp:Button ID="btnSendMail" runat="server" Text=" Send " OnClick="SendMail_Click" />
                        <asp:Button ID="btnReset" runat="server" Text=" Fortryd " OnClientClick="javascript:document.getElementById('mailarea').className='mailhide';return false;" />
                    </div>
                    <div style="width:350px">
                        <span>Vi kan ikke garantere, at citatet overlever vores utroligt strikse kvalitetskontrol, men hvis det slipper igennem nåleøjet, bliver du krediteret med dit navn :o)</span>
                    </div>
                </div>
            </div>
            <!-- Send til en ven -->
            <div id="mailafriend" class="mailhide">
                <div class="mailinner">
                    <strong>Tip en ven</strong>
                    <div>
                        <asp:Label ID="lblRecipientEmail" runat="server" CssClass="maillabels">Din vens e-mail:</asp:Label>
                        <asp:TextBox ID="tbxRecipientEmail" runat="server" CssClass="mailvalue"></asp:TextBox>
                    </div>
                    <div>
                        <asp:Label ID="lblNote" runat="server" CssClass="maillabels">Besked:</asp:Label>
                        <asp:TextBox ID="tbxNote" runat="server" CssClass="mailvalue" TextMode="MultiLine" Rows="5"></asp:TextBox>
                    </div>
                    <div style="text-align: right">
                        <asp:RequiredFieldValidator ID="rfvRecipientEmail" runat="server" ErrorMessage="Du skal skrive en e-mailadresse" ControlToValidate="tbxRecipientEmail" SetFocusOnError="true" Display="Dynamic"></asp:RequiredFieldValidator>
                        <asp:RegularExpressionValidator ID="revRecipientEmail" runat="server" ErrorMessage="Ikke en valid e-mailadresse" ControlToValidate="tbxRecipientEmail" SetFocusOnError="true" Display="Dynamic"></asp:RegularExpressionValidator>
                        <asp:Button ID="btnTipFriend" runat="server" Text=" Send " OnClick="btnTipFriend_Click" />
                        <asp:Button ID="btnCancel" runat="server" Text=" Fortryd " OnClientClick="javascript:document.getElementById('mailafriend').className='mailhide';return false;" />
                    </div>
                </div>
            </div>
