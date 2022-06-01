<%@ Page Title="" Language="C#" MasterPageFile="~/srtsMaster.Master" AutoEventWireup="True" CodeBehind="Sitemap.aspx.cs" Inherits="SrtsWeb.Public.Sitemap" %>

<%@ MasterType VirtualPath="~/srtsMaster.Master" %>

<asp:Content ID="Content6" ContentPlaceHolderID="MainContent_Public" runat="server">
    <div class="box_full_top"></div>
    <div class="box_full_content" style="min-height: 300px">
        <div class="padding">
            <asp:SiteMapDataSource ID="SrtsSitemapData" runat="server" />
            <asp:TreeView ID="SrtsSitemapTree" runat="server" DataSourceID="SrtsSitemapData"></asp:TreeView>
        </div>
    </div>
    <div class="box_full_bottom"></div>
</asp:Content>