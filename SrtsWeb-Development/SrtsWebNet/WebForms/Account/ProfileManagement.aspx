<%@ Page Title="Profile" Language="C#" MasterPageFile="~/SrtsMaster.master" AutoEventWireup="true"
    CodeBehind="ProfileManagement.aspx.cs" Inherits="SrtsWeb.Account.ProfileManagement" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <h2>User Profile Information
    </h2>
    <asp:Label ID="lblUserName" runat="server" Text="Enter UserName:"></asp:Label>
    <asp:TextBox ID="tbUserName" runat="server" Width="200px"></asp:TextBox>
    <hr />
    <div style="margin-left: 5em;">
        <h3>Enter User Information</h3>
        <asp:ValidationSummary ID="Summary" runat="server" ValidationGroup="enter" />
        <asp:Label ID="lblMsg" runat="server"></asp:Label>
        <table cellpadding="3" border="0">
            <tr>
                <td valign="top">Last name:
                </td>
                <td valign="top">
                    <asp:TextBox ID="tbLastName" runat="server" Width="200px"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvLastName" runat="server" ControlToValidate="tbLastName"
                        Display="None" ValidationGroup="enter" ErrorMessage="Last Name Is Required"></asp:RequiredFieldValidator>
                </td>
                <td valign="top">First name:
                </td>
                <td valign="top">
                    <asp:TextBox ID="tbFirstName" runat="server" Width="200px"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvFirstName" runat="server" ControlToValidate="tbFirstName"
                        Display="None" ValidationGroup="enter" ErrorMessage="First Name Is Required"></asp:RequiredFieldValidator>
                </td>
                <td valign="top">Middle Name:
                </td>
                <td valign="top">
                    <asp:TextBox ID="tbMiddleName" runat="server" Width="200px"></asp:TextBox>
                </td>
            </tr>
        </table>
        <h3>Enter Address Information</h3>
        <table cellpadding="3" border="0">
            <tr>
                <td valign="top">Address1:
                </td>
                <td valign="top">
                    <asp:TextBox ID="tbAddress1" runat="server" Width="200px"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvAddress1" runat="server" ControlToValidate="tbAddress1"
                        Display="None" ValidationGroup="enter" ErrorMessage="Address1 Is Required"></asp:RequiredFieldValidator>
                </td>
                <td valign="top">Address2:
                </td>
                <td valign="top">
                    <asp:TextBox ID="tbAddress2" runat="server" Width="200px"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td valign="top">City:
                </td>
                <td valign="top">
                    <asp:TextBox ID="tbCity" runat="server" Width="200px"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvCity" runat="server" ControlToValidate="tbCity"
                        Display="None" ValidationGroup="enter" ErrorMessage="City Is Required"></asp:RequiredFieldValidator>
                </td>
                <td valign="top">State:
                </td>
                <td valign="top">
                    <asp:TextBox ID="tbState" runat="server" Width="200px"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvState" runat="server" ControlToValidate="tbState"
                        Display="None" ValidationGroup="enter" ErrorMessage="State Is Required"></asp:RequiredFieldValidator>
                </td>
                <td valign="top">Zip Code:
                </td>
                <td valign="top">
                    <asp:TextBox ID="tbZipCode" runat="server" Width="200px"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvZipCode" runat="server" ControlToValidate="tbZipCode"
                        Display="None" ValidationGroup="enter" ErrorMessage="ZipCode Is Required"></asp:RequiredFieldValidator>
                    <asp:RegularExpressionValidator ID="revZipCode" runat="server" ControlToValidate="tbZipCode"
                        Display="None" ValidationGroup="enter" ErrorMessage="ZipCode Is Not Formatted Correctly"
                        ValidationExpression="^\d{5}(\-\d{4})?$"></asp:RegularExpressionValidator>
                </td>
            </tr>
        </table>
        <h3>Enter Misc Information</h3>
        <table cellpadding="3" border="0">
            <tr>
                <td valign="top">Work Phone:
                </td>
                <td valign="top">
                    <asp:TextBox ID="tbWorkPhone" runat="server" Width="200px"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvWorkPhone" runat="server" ControlToValidate="tbWorkPhone"
                        Display="None" ValidationGroup="enter" ErrorMessage="WorkPhone Is Required"></asp:RequiredFieldValidator>
                </td>
                <td valign="top">Work Phone(DSN):
                </td>
                <td valign="top">
                    <asp:TextBox ID="tbDSN" runat="server" Width="200px"></asp:TextBox>
                </td>
                <td valign="top">Site Code:
                </td>
                <td valign="top">
                    <asp:TextBox ID="tbSiteCode" runat="server" Width="200px"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvSiteCode" runat="server" ControlToValidate="tbSiteCode"
                        Display="None" ValidationGroup="enter" ErrorMessage="Site Code Is Required"></asp:RequiredFieldValidator>
                </td>
            </tr>
        </table>
        <h3>Enter Certification Information</h3>
        <table cellpadding="3" border="0">
            <tr>
                <td valign="top">HIPPA Certification Date:
                </td>
                <td valign="top">
                    <asp:TextBox ID="tbHIPPAStart" runat="server"></asp:TextBox>
                    <asp:Image ImageUrl="~/Styles/images/Calendar_scheduleHS.png" runat="server" ID="image1" />
                    <ajaxToolkit:CalendarExtender ID="cHStart" runat="server" TargetControlID="tbHIPPAStart"
                        Format="MMMM d, yyyy" PopupButtonID="image1">
                    </ajaxToolkit:CalendarExtender>
                    <asp:RequiredFieldValidator ID="rfvHippaStart" runat="server" ControlToValidate="tbHIPPAStart"
                        Display="None" ValidationGroup="enter" ErrorMessage="HIPPA Certification date is required"></asp:RequiredFieldValidator>
                </td>
                <td valign="top">HIPPA Expiration Date:
                </td>
                <td valign="top">
                    <asp:TextBox ID="tbHIPPAStop" runat="server" Width="200px"></asp:TextBox>
                    <asp:Image ImageUrl="~/Styles/images/Calendar_scheduleHS.png" runat="server" ID="image2" />
                    <ajaxToolkit:CalendarExtender ID="cHIPPAStop" runat="server" TargetControlID="tbHIPPAStop"
                        Format="MMMM d, yyyy" PopupButtonID="image2">
                    </ajaxToolkit:CalendarExtender>
                    <asp:RequiredFieldValidator ID="rfvHippaStop" runat="server" ControlToValidate="tbHIPPAStop"
                        Display="None" ValidationGroup="enter" ErrorMessage="HIPPA end date is required"></asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <td valign="top">IASO Certification Date:
                </td>
                <td valign="top">
                    <asp:TextBox ID="tbIASOStart" runat="server"></asp:TextBox>
                    <asp:Image ImageUrl="~/Styles/images/Calendar_scheduleHS.png" runat="server" ID="image3" />
                    <ajaxToolkit:CalendarExtender ID="cIASOStart" runat="server" TargetControlID="tbIASOStart"
                        Format="MMMM d, yyyy" PopupButtonID="image3">
                    </ajaxToolkit:CalendarExtender>
                    <asp:RequiredFieldValidator ID="rfvIasoStart" runat="server" ControlToValidate="tbIASOStart"
                        Display="None" ValidationGroup="enter" ErrorMessage="IASO Certification date is required"></asp:RequiredFieldValidator>
                </td>
                <td valign="top">IASO Expiration Date:
                </td>
                <td valign="top">
                    <asp:TextBox ID="tbIASOStop" runat="server" Width="200px"></asp:TextBox>
                    <asp:Image ImageUrl="~/Styles/images/Calendar_scheduleHS.png" runat="server" ID="image4" />
                    <ajaxToolkit:CalendarExtender ID="cIASOStop" runat="server" TargetControlID="tbIASOStop"
                        Format="MMMM d, yyyy" PopupButtonID="image4">
                    </ajaxToolkit:CalendarExtender>
                    <asp:RequiredFieldValidator ID="rfvIasoStop" runat="server" ControlToValidate="tbIASOStop"
                        Display="None" ValidationGroup="enter" ErrorMessage="IASO end date is required"></asp:RequiredFieldValidator>
                </td>
            </tr>
        </table>
        <asp:Button ID="btnSubmit" runat="server" CssClass="srtsButton" Text="Submit Profile" OnClick="btnSubmit_Click"
            CausesValidation="true" ValidationGroup="enter" />
    </div>
</asp:Content>