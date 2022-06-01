<%@ Page Title="" Language="C#" MasterPageFile="~/srtsMaster.Master" AutoEventWireup="true" CodeBehind="SrtsMgmtTools.aspx.cs" Inherits="SrtsWeb.Admin.SrtsMgmtTools" %>
<%@ MasterType VirtualPath="~/srtsMaster.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server"></asp:Content>

<asp:Content ID="contentClinicName" ContentPlaceHolderID="ContentTop_Title_Right" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div>
        <asp:Label ID="CurrentTime" runat="server" />
        <asp:UpdatePanel ID="UpdatePanel2" runat="server">
            <ContentTemplate>
                <asp:Label ID="Label1" runat="server" Text="This is a label!"></asp:Label>
                <asp:Button ID="Button1" runat="server" Text="Click Me" OnClick="Button1_Click" />
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</asp:Content>