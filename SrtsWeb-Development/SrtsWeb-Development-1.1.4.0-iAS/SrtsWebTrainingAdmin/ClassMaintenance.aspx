<%@ Page Title="" Language="C#" MasterPageFile="~/srtsMaster.Master" AutoEventWireup="true" CodeBehind="ClassMaintenance.aspx.cs" Inherits="SrtsWebTrainingAdmin.ClassMaintenance" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="MainContent" runat="server">
    <div>
        <asp:Label ID="lblFeedback" runat="server" ForeColor="Red"></asp:Label>
        <br />
        <br />
        <h1>Create Class </h1>
        <asp:Label ID="lblClassName" runat="server" CssClass="srtsLabel_medium" Text="Please enter the 5 digit class ID"></asp:Label>
        <asp:TextBox ID="tbClassName" runat="server" CssClass="srtsTextBox_medium"></asp:TextBox>
        <br />
        <br />
        <asp:Label ID="lblInfo" runat="server" Text="All new accounts will have the following default password that must be changed on initial login"></asp:Label>
        <br />
        <asp:Literal ID="litPass" runat="server" Text="1234!@#$abcdABCD"></asp:Literal>
        <br />
        <br />
        <asp:Button ID="btnCreateClass" runat="server" CssClass="srtsButton" Text="Create Class" OnClick="btnCreateClass_Click" />
    </div>
    <%--<br />
    <br />
    <br />
    <div>
        <h1>Delete Class </h1>
        <asp:Label ID="lblDeleteClass" runat="server" CssClass="srtsLabel_medium" Text="Please enter the 5 digit class ID to delete"></asp:Label>
        <asp:TextBox ID="tbDeleteClassID" runat="server" CssClass="srtsTextBox_medium"></asp:TextBox>
        <br />
        <asp:Button ID="btnDeleteClass" runat="server" CssClass="srtsButton" Text="Delete Class" OnClick="btnDeleteClass_Click" />
    </div>--%>
</asp:Content>