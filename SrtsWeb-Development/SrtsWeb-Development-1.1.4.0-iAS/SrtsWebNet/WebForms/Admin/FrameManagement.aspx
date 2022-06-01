<%@ Page Title="" Language="C#" MasterPageFile="~/srtsMaster.Master" AutoEventWireup="true" MaintainScrollPositionOnPostback="true"
    CodeBehind="FrameManagement.aspx.cs" Inherits="SrtsWeb.Admin.FrameManagement" EnableViewState="true" ViewStateMode="Enabled" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div style="margin-bottom: 15px;">
        <asp:ValidationSummary ID="vsSummary" runat="server" DisplayMode="BulletList" ForeColor="Red" ShowSummary="true" />
        <asp:Label ID="lblContactSrts" runat="server" CssClass="colorRed" Text="Contact the SRTS Web team to edit an existing frame" Visible="false"></asp:Label>
    </div>
    <h2>Manage Frames</h2>
    <hr />

    <asp:UpdatePanel ID="upFrameInfo" runat="server" UpdateMode="Always">
        <ContentTemplate>
            <div style="margin-top: 10px; margin-left: 15px;">
                <asp:Label ID="Label17" runat="server" Text="Frames:" CssClass="srtsLabel_medium" AssociatedControlID="ddlFrames"></asp:Label>
                <asp:DropDownList ID="ddlFrames" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlFrames_SelectedIndexChanged"></asp:DropDownList>
            </div>
            <hr />
            <fieldset>
                <legend>
                    <h1>Frame Data:</h1>
                </legend>
                <div style="margin-top: 10px; margin-left: 15px; float: left; display: block;">
                    <asp:Label ID="Label1" runat="server" Text="Frame Code:" CssClass="srtsLabel_medium" AssociatedControlID="tbFrameCode"></asp:Label>
                    <asp:TextBox ID="tbFrameCode" runat="server" CssClass="srtsTextBox_medium_FM"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvFrameCode" runat="server" Display="None" Text="*" ControlToValidate="tbFrameCode"
                        ErrorMessage="Frame code is required"></asp:RequiredFieldValidator>
                </div>
                <div style="margin-top: 10px; margin-left: 15px; float: left; display: block;">
                    <asp:Label ID="Label2" runat="server" Text="Description:" CssClass="srtsLabel_medium" AssociatedControlID="tbDescription"></asp:Label>
                    <asp:TextBox ID="tbDescription" runat="server" CssClass="srtsTextBox_medium_FM"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvDescription" runat="server" Display="None" Text="*" ControlToValidate="tbDescription"
                        ErrorMessage="Description is required"></asp:RequiredFieldValidator>
                </div>
                <div style="margin-top: 10px; margin-left: 15px; float: left; display: block;">
                    <asp:Label ID="Label3" runat="server" Text="Notes:" CssClass="srtsLabel_medium" AssociatedControlID="tbNotes"></asp:Label>
                    <asp:TextBox ID="tbNotes" runat="server" CssClass="srtsTextBox_medium_FM"></asp:TextBox>
                </div>
                <div style="margin-top: 10px; margin-left: 15px; float: left; display: block;">
                    <asp:Label ID="Label5" runat="server" Text="Max Pairs:" CssClass="srtsLabel_medium" AssociatedControlID="tbMaxPair"></asp:Label>
                    <asp:TextBox ID="tbMaxPair" runat="server" CssClass="srtsTextBox_medium_FM"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvMaxPair" runat="server" Display="None" Text="*" ControlToValidate="tbMaxPair"
                        ErrorMessage="Max pair is required"></asp:RequiredFieldValidator>
                </div>
                <div style="margin-top: 10px; margin-left: 15px; float: left; display: block;">
                    <asp:Label ID="Label6" runat="server" Text="Image URL:" CssClass="srtsLabel_medium" AssociatedControlID="tbImageUrl"></asp:Label>
                    <asp:TextBox ID="tbImageUrl" runat="server" CssClass="srtsTextBox_medium_FM"></asp:TextBox>
                </div>
                <div style="margin-top: 10px; margin-left: 15px; float: left; display: block;">
                    <asp:Label ID="Label4" runat="server" Text="Is Insert:" CssClass="srtsLabel_medium" AssociatedControlID="cbIsInsert"></asp:Label>
                    <asp:CheckBox ID="cbIsInsert" runat="server" ClientIDMode="Static"></asp:CheckBox>
                </div>
                <div style="margin-top: 10px; margin-left: 15px; float: left; display: block;">
                    <asp:Label ID="Label7" runat="server" Text="Is Active:" CssClass="srtsLabel_medium" AssociatedControlID="cbIsActive"></asp:Label>
                    <asp:CheckBox ID="cbIsActive" runat="server"></asp:CheckBox>
                </div>
                <div style="margin-top: 10px; margin-left: 15px; float: left; display: block;">
                    <asp:Label ID="Label8" runat="server" Text="Is FOC:" CssClass="srtsLabel_medium" AssociatedControlID="cbIsFoc"></asp:Label>
                    <asp:CheckBox ID="cbIsFoc" runat="server" ClientIDMode="Static"></asp:CheckBox>
                </div>
                <div style="margin-top: 10px; margin-left: 15px;">
                    <asp:Label ID="Label19" runat="server" Text="Gender:" CssClass="srtsLabel_medium" AssociatedControlID="rblGender"></asp:Label>
                    <asp:RadioButtonList ID="rblGender" runat="server" RepeatDirection="Horizontal">
                        <asp:ListItem Text="Both" Value="B" Selected="True"></asp:ListItem>
                        <asp:ListItem Text="Male" Value="M"></asp:ListItem>
                        <asp:ListItem Text="Female" Value="F"></asp:ListItem>
                    </asp:RadioButtonList>
                </div>
            </fieldset>
            <hr />
            <fieldset>
                <legend>
                    <h1>Frame Item Data:</h1>
                </legend>
                <div style="margin-top: 10px; margin-left: 15px; float: left; display: block;">
                    <asp:Label ID="Label9" runat="server" Text="Temple:" CssClass="srtsLabel_medium" AssociatedControlID="lbTemple"></asp:Label><br />
                    <asp:ListBox ID="lbTemple" runat="server" SelectionMode="Multiple" Height="130px"></asp:ListBox>
                </div>
                <div style="margin-top: 10px; margin-left: 15px; float: left; display: block;">
                    <asp:Label ID="Label10" runat="server" Text="Bridge:" CssClass="srtsLabel_medium" AssociatedControlID="lbBridge"></asp:Label><br />
                    <asp:ListBox ID="lbBridge" runat="server" SelectionMode="Multiple" Height="130px"></asp:ListBox>
                </div>
                <div style="margin-top: 10px; margin-left: 15px; float: left; display: block;">
                    <asp:Label ID="Label11" runat="server" Text="Color" CssClass="srtsLabel_medium" AssociatedControlID="lbColor"></asp:Label><br />
                    <asp:ListBox ID="lbColor" runat="server" SelectionMode="Multiple" Height="130px"></asp:ListBox>
                </div>
                <div style="margin-top: 10px; margin-left: 15px; float: left; display: block;">
                    <asp:Label ID="Label12" runat="server" Text="Eye Size:" CssClass="srtsLabel_medium" AssociatedControlID="lbEyeSize"></asp:Label><br />
                    <asp:ListBox ID="lbEyeSize" runat="server" SelectionMode="Multiple" Height="130px"></asp:ListBox>
                </div>
                <div style="margin-top: 10px; margin-left: 15px; float: left; display: block;">
                    <asp:Label ID="Label13" runat="server" Text="Lens Type:" CssClass="srtsLabel_medium" AssociatedControlID="lbLensType"></asp:Label><br />
                    <asp:ListBox ID="lbLensType" runat="server" SelectionMode="Multiple" Height="130px"></asp:ListBox>
                </div>
                <div style="margin-top: 10px; margin-left: 15px; float: left; display: block;">
                    <asp:Label ID="Label14" runat="server" Text="Material:" CssClass="srtsLabel_medium" AssociatedControlID="lbMaterial"></asp:Label><br />
                    <asp:ListBox ID="lbMaterial" runat="server" SelectionMode="Multiple" Height="130px"></asp:ListBox>
                </div>
                <div style="margin-top: 10px; margin-left: 15px; float: left; display: block;">
                    <asp:Label ID="Label15" runat="server" Text="Tint:" CssClass="srtsLabel_medium" AssociatedControlID="lbTint"></asp:Label><br />
                    <asp:ListBox ID="lbTint" runat="server" SelectionMode="Multiple" Height="130px"></asp:ListBox>
                </div>
            </fieldset>
            <hr />
            <fieldset>
                <legend>
                    <h1>Priority:</h1>
                </legend>
                <div style="margin-top: 10px; margin-left: 15px;">
                    <asp:Label ID="Label16" runat="server" Text="Priority:" CssClass="srtsLabel_medium" AssociatedControlID="lbPriority"></asp:Label><br />
                    <asp:ListBox ID="lbPriority" runat="server" SelectionMode="Multiple" Height="130px"></asp:ListBox>
                </div>
            </fieldset>
            <hr />
            <fieldset>
                <legend>
                    <h1>Eligibility:</h1>
                </legend>
                <asp:UpdatePanel ID="upEligibility" runat="server" UpdateMode="Always">
                    <ContentTemplate>
                        <asp:Panel ID="pnlEligibility" runat="server"></asp:Panel>
                    </ContentTemplate>
                    <Triggers>
                        <asp:PostBackTrigger ControlID="ddlFrames" />
                    </Triggers>
                </asp:UpdatePanel>
            </fieldset>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="bCancel" />
            <asp:PostBackTrigger ControlID="bSubmit" />
        </Triggers>
    </asp:UpdatePanel>
    <div style="float: right;">
        <asp:Button ID="bSubmit" runat="server" Text="Submit" CssClass="srtsButton" OnClick="bSubmit_Click" />
        <asp:Button ID="bCancel" runat="server" Text="Cancel" CssClass="srtsButton" CausesValidation="false" OnClick="bCancel_Click" />
    </div>

    <asp:ScriptManagerProxy ID="smpFrameManagement" runat="server">
        <Scripts>
            <asp:ScriptReference Path="~/Scripts/Frames/FrameManagement.js" />
        </Scripts>
    </asp:ScriptManagerProxy>
</asp:Content>