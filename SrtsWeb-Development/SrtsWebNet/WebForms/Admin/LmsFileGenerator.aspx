<%@ Page Title="" Language="C#" MasterPageFile="~/srtsMaster.Master" AutoEventWireup="true" CodeBehind="LmsFileGenerator.aspx.cs" Inherits="SrtsWeb.Admin.LmsFileGenerator" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="MainContent" runat="server">
    <div id="divError" runat="server" class="colorRed">
        <asp:Literal ID="litError" runat="server"></asp:Literal>
    </div>
    <div class="padding">
        <div style="margin-left: 10px; float: left; display: block;">
            <asp:Label ID="Label1" runat="server" CssClass="srtsLabel_medium" Text="Select A Lab: "></asp:Label>
            <asp:DropDownList ID="ddlLabList" runat="server"></asp:DropDownList>
        </div>
        <div style="clear: both; margin-left: 10px; margin-top: 15px; float: left; display: block;">
            <asp:Label ID="Label2" runat="server" CssClass="srtsLabel_medium" Text="Mark Orders As Complete: "></asp:Label>
            <asp:RadioButtonList ID="rblMarkComplete" runat="server" RepeatDirection="Horizontal">
                <asp:ListItem Text="Yes" Value="Y"></asp:ListItem>
                <asp:ListItem Text="No" Value="N" Selected="True"></asp:ListItem>
            </asp:RadioButtonList>
        </div>
        <div style="clear: both; margin-left: 10px; margin-top: 15px; float: left; display: block;">
            <asp:Button ID="bGetData" runat="server" Text="Submit" CssClass="srtsButton" OnClick="bGetData_Click" />
        </div>
        <hr style="clear: both; width: 80px;" />
        <div style="margin-left: 10px; margin-top: 15px;">
            <asp:Label ID="Label3" runat="server" CssClass="srtsLabel_medium" Text="Good Orders"></asp:Label>
            <asp:GridView ID="gvProcessedOrders" runat="server" Width="100%"></asp:GridView>
        </div>
        <hr style="width: 80px;" />
        <div style="margin-left: 10px; margin-top: 15px;">
            <asp:Label ID="Label4" runat="server" CssClass="srtsLabel_medium" Text="Unprocessed Orders"></asp:Label>
            <asp:GridView ID="gvUnprocessedOrders" runat="server" Width="100%"></asp:GridView>
        </div>
        <hr style="width: 80px;" />
        <div style="margin-left: 10px; margin-top: 15px;">
            <asp:Label ID="Label5" runat="server" CssClass="srtsLabel_medium" Text="Unprocessed Orders"></asp:Label>
            <asp:GridView ID="gvErrorOrders" runat="server" Width="100%"></asp:GridView>
        </div>
    </div>
</asp:Content>