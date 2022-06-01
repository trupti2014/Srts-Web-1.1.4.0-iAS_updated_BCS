<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="BMTUpload.aspx.cs" Inherits="SrtsWebClinic.Patients.BMTUpload" MasterPageFile="~/srtsMaster.Master" EnableViewState="true" %>

<%@ MasterType VirtualPath="~/srtsMaster.Master" %>
<asp:Content ID="cphHeader" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="../../Styles/PassFailConfirm.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="cphMain" ContentPlaceHolderID="MainContent" runat="server">
    <asp:ScriptManagerProxy ID="smpBmt" runat="server">
        <Scripts>
            <asp:ScriptReference Path="~/Scripts/BMT/BMT.js" />
            <asp:ScriptReference Path="~/Scripts/Global/PassFailConfirm.js" />
        </Scripts>
    </asp:ScriptManagerProxy>
    <p style="text-align: center" class="colorBlue">
        Select a file to upload your data then DOUBLE CLICK the <strong>Process File</strong> Button.
                                                <br />
        When complete, you will be given the number of records entered
                                                <br />
        or you will receive a message indicating the individuals who were not entered into the database.
    </p>
    <br />
    <br />
    <div id="divError" style="margin: 10px; text-align: center;">
        <div id="divBmtMessage" style="width: 350px;"></div>
        <asp:ValidationSummary ID="uploadSummary" DisplayMode="BulletList" ShowSummary="true" runat="server" />
    </div>

    <div style="text-align: center;">
        <asp:LinkButton ID="lbGetFile" runat="server" Text="Download BMT Template File" OnClick="lbGetFile_Click"></asp:LinkButton>

        <div id="divFileUpload" style="margin-top: 20px;">
            <hr style="width: 40%; margin-bottom: 10px;" />
            <asp:Label ID="Label11" runat="server" Text="Click the ''Browse'' button and select a file to upload" CssClass="srtsLabel_medium" AssociatedControlID="fUpload" /><br />
            <div style="border: 1px solid silver; margin: 12px auto; padding: 12px; width: 460px;">
                <asp:FileUpload ID="fUpload" CssClass="fUpload" runat="server" Width="450px" />
            </div>
        </div>

        <div style="margin-top: 10px;">
            <asp:Button ID="btnProcess" runat="server" CssClass="srtsButton" Text="Process File" OnClick="btnProcess_Click" />
        </div>
    </div>
    <div style="clear: both;"></div>
    <div style="text-align: center; margin-top: 10px; overflow: auto; display: normal;">
        <asp:Label ID="lblLoadError" runat="server" CssClass="colorRed" /><div style="margin-top: 10px;"></div>
        <asp:GridView ID="gvBmtOutput" runat="server" GridLines="Both" CssClass="mGrid" AlternatingRowStyle-CssClass="alt" CellPadding="5" CellSpacing="3" AutoGenerateColumns="true" Width="90%"></asp:GridView>
    </div>
</asp:Content>