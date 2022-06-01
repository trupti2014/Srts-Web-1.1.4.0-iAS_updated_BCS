<%@ Page Title="" Language="C#" MasterPageFile="~/srtsMaster.Master" AutoEventWireup="true" CodeBehind="SrtsCMSManager.aspx.cs" Inherits="SrtsWeb.Admin.SrtsCMSManager" %>

<%@ MasterType VirtualPath="~/srtsMaster.Master" %>

<asp:Content ID="ContentHead" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="padding">
    <table>
        <tr>
            <td>
                <p class="srtsLabel_medium noindent">Select Message Title</p>
                <asp:DropDownList ID="ddlTitles" runat="server" Width="180px" ClientIDMode="Static" AutoPostBack="true"
                    OnSelectedIndexChanged="ddlTitles_SelectedIndexChanged">
                </asp:DropDownList>
            </td>
            <td>
                <asp:Button ID="btnDeleteContent" runat="server" CssClass="srtsButton" Text="Delete" OnClick="btnDeleteContent_Click" />
            </td>
        </tr>
    </table>
    <br />
    <div id="cmsUpdateContainer" style="padding-top: 10px;">
        <srts:CmsManager ID="cmsUpdate" runat="server" />
    </div>
    </div>
</asp:Content>