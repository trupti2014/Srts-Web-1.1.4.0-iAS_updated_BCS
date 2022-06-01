<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/SrtsMaster.Master" CodeBehind="Logout.aspx.cs" Inherits="SrtsWeb.Account.Logout" %>

<%@ MasterType VirtualPath="~/srtsMaster.Master" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <style>
        .logoutMessage
        {
            text-align: center;
            width: 70%;
            margin: 90px auto;
            height: auto;
            background-color:#f7f7f7;
            border: 1px solid #888888;
            -webkit-box-shadow: 10px 10px 10px 0px #888888;
            -moz-box-shadow: 10px 10px 10px 0px #888888;
            box-shadow: 10px 10px 10px 0px #888888;
        }
    </style>
</asp:Content>

<asp:Content ID="contentMainContent" ContentPlaceHolderID="MainContent_Public" runat="server">
  <div class="logoutMessage">
        <div style="font-size: 1.5em; padding: 20px 10px;">
            <asp:Literal ID="litMessage" runat="server" Text="User was automatically logged out due to session timeout." />            
        </div>
        <div style="font-size: 1.0em; padding-bottom: 20px">
            <asp:LinkButton ID="lbtn" runat="server" PostBackUrl="~/WebForms/Account/Login.aspx">Go to login page</asp:LinkButton>
        </div>
    </div>
    <asp:ScriptManagerProxy ID="smpLogout" runat="server">
        <Scripts>
            <asp:ScriptReference Path="~/Scripts/Logout/Logout.js" />
        </Scripts>
    </asp:ScriptManagerProxy>
</asp:Content>
