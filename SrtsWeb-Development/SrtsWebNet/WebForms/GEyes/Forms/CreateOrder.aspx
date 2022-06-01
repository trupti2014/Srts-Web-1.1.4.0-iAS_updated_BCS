<%@ Page Title="" Language="C#" MasterPageFile="~/GEyes/GeyesMaster.Master" AutoEventWireup="true"
    CodeBehind="CreateOrder.aspx.cs" Inherits="GEyes.Forms.CreateOrder" %>

<%@ MasterType VirtualPath="~/GEyes/GeyesMaster.Master" %>

<asp:Content ID="Content6" ContentPlaceHolderID="MainContent_Public" runat="server">

    <h4>Add New Order</h4>
    <br />
    <asp:ValidationSummary ID="vsErrors" runat="server" DisplayMode="BulletList" ForeColor="Red"
        ShowSummary="true" />
    <p>
        <asp:Label ID="lblOrderPriority" runat="server" Text="Order Priority:" Width="175px"
            CssClass="srtsLabel_medium"></asp:Label>
        <asp:DropDownList ID="ddlOrderPriority" runat="server" TabIndex="1" AutoPostBack="True"
            OnSelectedIndexChanged="ddlPriority_SelectedIndexChanged" DataTextField="Value"
            DataValueField="Key" CssClass="srtsDropDown_medium">
        </asp:DropDownList>
    </p>
    <hr />
    <h3>Deployment</h3>
    <p>
        <asp:Label ID="lblLocation" runat="server" Text="Location Zip Code:" Width="175px" CssClass="srtsLabel_medium"></asp:Label>
        <asp:DropDownList ID="ddlDeployLocation" runat="server" TabIndex="3" OnSelectedIndexChanged="ddlDeployLocation_SelectedIndexChanged"
            DataTextField="TheaterCode" DataValueField="TheaterCode" CssClass="srtsDropDown_medium">
        </asp:DropDownList>
    </p>
    <p>
        <asp:Label ID="lblStartDate" runat="server" Text="Deployment Start Date:" Width="175px" CssClass="srtsLabel_medium"></asp:Label>
        <asp:TextBox ID="tbDeployStartDate" runat="server" TabIndex="4" CssClass="clear"></asp:TextBox>
        <asp:Image runat="server" ID="calImage1" AlternateText="Calendar" ImageUrl="~/Styles/images/Calendar_scheduleHS.png" />
        <ajaxToolkit:CalendarExtender ID="ceExamDate" runat="server" TargetControlID="tbDeployStartDate"
            Format="MMMM d, yyyy" PopupButtonID="calImage1">
        </ajaxToolkit:CalendarExtender>
    </p>
    <hr />
    <h3>Frame</h3>
    <p>
        <asp:Label ID="lblFrame" runat="server" Text="Frame:" Width="175px" CssClass="srtsLabel_medium"></asp:Label>
        <asp:DropDownList ID="ddlFrame" runat="server" TabIndex="5" AutoPostBack="True" OnSelectedIndexChanged="ddlFrame_SelectedIndexChanged"
            DataTextField="FrameDescription" DataValueField="FrameCode" CssClass="srtsDropDown_medium">
        </asp:DropDownList>
    </p>
    <p>
        <asp:Label ID="lblColor" runat="server" Text="Color:" Width="175px" CssClass="srtsLabel_medium"></asp:Label>
        <asp:DropDownList ID="ddlColor" runat="server" DataTextField="Text" DataValueField="Value"
            TabIndex="6" OnSelectedIndexChanged="ddlColor_SelectedIndexChanged" CssClass="srtsDropDown_medium">
        </asp:DropDownList>
    </p>
    <p>
        <asp:Label ID="lblTint" runat="server" Text="Tint:" Width="175px" CssClass="srtsLabel_medium"></asp:Label>
        <asp:DropDownList ID="ddlTint" runat="server" DataTextField="Text" DataValueField="Value"
            TabIndex="11" AutoPostBack="True" OnSelectedIndexChanged="ddlTint_SelectedIndexChanged" CssClass="srtsDropDown_medium">
        </asp:DropDownList>
    </p>
    <p>
        <asp:Label ID="lblPair" runat="server" Text="Pair:" Width="175px" CssClass="srtsLabel_medium"></asp:Label>
        <asp:TextBox ID="tbPair" runat="server" Width="100px" TabIndex="13" CssClass="srtsTextBox_medium"></asp:TextBox>
    </p>
    <p>
        <asp:Label ID="lblCases" runat="server" Text="Cases:" Width="175px" CssClass="srtsLabel_medium"></asp:Label>
        <asp:TextBox ID="tbCases" runat="server" Width="100px" TabIndex="14" CssClass="srtsTextBox_medium"></asp:TextBox>
    </p>
    <hr />
    <p>
        <asp:Label ID="lblComment" runat="server" Text="Comment:" Width="175px" CssClass="srtsLabel_medium"></asp:Label>
        <asp:TextBox ID="tbComment" runat="server" TextMode="MultiLine" MaxLength="256" Width="500px"
            TabIndex="50"></asp:TextBox>
    </p>
    <hr />
    <br />
    <div align="center">
        <asp:Button ID="btnSubmit" runat="server" CssClass="srtsButton" Text="Submit" ClientIDMode="Static" OnClick="btnAdd_Click" />&nbsp
         <asp:Button ID="btnCancel" runat="server" CssClass="srtsButton" Text="Cancel" CausesValidation="False" ClientIDMode="Static" OnClick="btnCancel_Click" />
    </div>
</asp:Content>