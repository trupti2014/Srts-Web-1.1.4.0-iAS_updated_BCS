<%@ Page Title="" Language="C#" MasterPageFile="~/srtsMaster.Master" AutoEventWireup="true" CodeBehind="PublicPageTemplate.aspx.cs" Inherits="SrtsWeb.Public.PublicPageTemplate" %>

<%@ MasterType VirtualPath="~/srtsMaster.Master" %>

<asp:Content ID="Content6" ContentPlaceHolderID="MainContent" runat="server">
    <div class="contentTitleleft" style="text-align: center">
        <div class="SRTSwebInfo">
            <h1>&nbsp;</h1>
        </div>
        <h2 style="text-align: center">
            <asp:Literal ID="litModuleTitle" runat="server" /></h2>
        <div style="margin: -10px 0px 20px 20px; padding-left: 10px; border-bottom: 1px solid #E7CFAD; width: 90%">
        </div>
        <h1 class="headerBlue">Page Title</h1>
    </div>

    <div class="padding" style="margin: 0px 30px">
        <p>content goes here....</p>
    </div>
</asp:Content>