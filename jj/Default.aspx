<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="jj_Default" MasterPageFile="~/MasterPage.master" %>

<%@ Register Src="../ShowQuotes.ascx" TagName="ShowQuotes" TagPrefix="uc1" %>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="ContentPlaceHolder1">
    <uc1:ShowQuotes ID="ShowQuotes1" runat="server" />
        
</asp:Content>
    
