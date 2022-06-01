<%@ Page Title="Change Password" Language="C#" MasterPageFile="~/SrtsMaster.master" AutoEventWireup="true"
    CodeBehind="ChangePassword.aspx.cs" Inherits="SrtsWeb.Account.ChangePassword" %>

<asp:Content ID="MainContent" runat="server" ContentPlaceHolderID="MainContent">
    <asp:ScriptManagerProxy ID="smpChangePassword" runat="server">
        <Scripts>
            <asp:ScriptReference Path="~/Scripts/Global/SrtsCustomValidators.js" />
            <asp:ScriptReference Path="~/Scripts/Password/ChangePasswordVal.js" />
        </Scripts>
    </asp:ScriptManagerProxy>
    <div class="full" style="width: 730px">
        <h2>Change Password
        </h2>
        <p>
            Use the form below to change your password.
        </p>
        <br />
        <div style="text-align: left; margin-left: 15px;">
            <ul>
                <li>- Passwords can only be changed after <%= ConfigurationManager.AppSettings["minPasswordLife_Hours"] %> hours of previous change.</li>
                <li>- Passwords are required to have a minimum of <%= Membership.MinRequiredPasswordLength %> characters.</li>
                <li>- Passwords must be made up of at least:</li>
                <li style="margin-left: 15px;">- 2 Upper case letters</li>
                <li style="margin-left: 15px;">- 2 Lower letter</li>
                <li style="margin-left: 15px;">- 2 Numbers</li>
                <li style="margin-left: 15px;">- 2 Special characters & ! @ # $ % ^ * ( )</li>
            </ul>
        </div>
    </div>
    <div style="margin-left: 15px;">
        <asp:ChangePassword ID="ChangeUserFact2" runat="server" CancelDestinationPageUrl="~/WebForms/Default.aspx" EnableViewState="false" RenderOuterTable="false"
            SuccessPageUrl="~/WebForms/Account/ChangePasswordSuccess.aspx" ChangePasswordFailureText="Password incorrect or New Password invalid. New Password length minimum: {0}."
            OnChangedPassword="ChangeUserFact2_ChangedPassword" OnChangePasswordError="ChangeUserFact2_ChangePasswordError" >
            <ChangePasswordTemplate>
                <span class="failureNotification" style="color: red;">
                    <asp:Literal ID="FailureText" runat="server"></asp:Literal>
                </span>
                <br />
                <asp:ValidationSummary ID="ChangeUserFact2ValidationSummary" runat="server" CssClass="failureNotification"
                    ValidationGroup="ChangeUserFact2ValidationGroup" ForeColor="Red" />
                <div id="lblChangeError" class="colorRed" style="margin-bottom: 15px;"></div>
                <br />
                <br />
                <div class="w3-col s4">
                    <div class="BeigeBoxContainer" style="margin: 0px 10px">
                        <div class="BeigeBoxHeader" style="padding: 12px 10px 3px 15px">
                            <span class="label">Change Password:</span>
                        </div>
                        <div class="BeigeBoxContent" style="width: 500px; margin-left: 10px">
                            <div style="text-align: left; margin: 10px 15px;">
                                <asp:Label ID="CurrentPasswordLabel" runat="server" CssClass="srtsLabel_medium" AssociatedControlID="CurrentPassword">Old Password:</asp:Label>
                                <asp:TextBox ID="CurrentPassword" runat="server" CssClass="passwordEntry" TextMode="Password"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="CurrentPasswordRequired" runat="server" ControlToValidate="CurrentPassword"
                                    CssClass="failureNotification" ErrorMessage="Password is required." ToolTip="Old Password is required."
                                    ValidationGroup="ChangeUserFact2ValidationGroup">*</asp:RequiredFieldValidator>
                                <asp:CustomValidator ID="CustCurrPassword" runat="server" ControlToValidate="CurrentPassword" SetFocusOnError="false" ToolTip="Invalid characters in password"
                                    ValidationGroup="ChangeUserFact2ValidationGroup" ClientValidationFunction="validateChars" />
                            </div>
                            <div style="text-align: left; margin: 10px 15px;">
                                <asp:Label ID="NewPasswordLabel" runat="server" CssClass="srtsLabel_medium" AssociatedControlID="NewPassword">New Password:</asp:Label>
                                <asp:TextBox ID="NewPassword" runat="server" CssClass="passwordEntry" TextMode="Password"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="NewPasswordRequired" runat="server" ControlToValidate="NewPassword"
                                    CssClass="failureNotification" ErrorMessage="New Password is required." ToolTip="New Password is required."
                                    ValidationGroup="ChangeUserFact2ValidationGroup">*</asp:RequiredFieldValidator>
                                <asp:CustomValidator ID="CustNewPassword" runat="server" ControlToValidate="NewPassword" SetFocusOnError="false" ToolTip="Invalid characters in password"
                                    ValidationGroup="ChangeUserFact2ValidationGroup" ClientValidationFunction="validateChars" />
                            </div>
                            <div style="text-align: left; margin: 10px 15px;">
                                <asp:Label ID="ConfirmNewPasswordLabel" runat="server" CssClass="srtsLabel_medium" AssociatedControlID="ConfirmNewPassword">Confirm New Password:</asp:Label>
                                <asp:TextBox ID="ConfirmNewPassword" runat="server" CssClass="passwordEntry" TextMode="Password"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="ConfirmNewPasswordRequired" runat="server" ControlToValidate="ConfirmNewPassword"
                                    CssClass="failureNotification" Display="Dynamic" ErrorMessage="Confirm New Password is required."
                                    ToolTip="Confirm New Password is required." ValidationGroup="ChangeUserFact2ValidationGroup">*</asp:RequiredFieldValidator>
                                <asp:CompareValidator ID="NewPasswordCompare" runat="server" ControlToCompare="NewPassword" ControlToValidate="ConfirmNewPassword"
                                    CssClass="failureNotification" Display="Dynamic" ErrorMessage="The Confirm New Password must match the New Password entry."
                                    ValidationGroup="ChangeUserFact2ValidationGroup">*</asp:CompareValidator>
                                <asp:CustomValidator ID="CustConfirmPassword" runat="server" ControlToValidate="ConfirmNewPassword" SetFocusOnError="false" ToolTip="Invalid characters in password"
                                    ValidationGroup="ChangeUserFact2ValidationGroup" ClientValidationFunction="validateChars" />
                                <asp:CustomValidator ID="CustPasswordDiff" runat="server" ControlToValidate="ConfirmNewPassword" SetFocusOnError="false"
                                    ValidationGroup="ChangeUserFact2ValidationGroup" OnServerValidate="CustPasswordDiff_ServerValidate" />
                            </div>

                            <div class="submitButton">
                                <asp:Button ID="CancelPushButton" CssClass="srtsButton" runat="server" CausesValidation="False" CommandName="Cancel" Text="Cancel" />
                                <asp:Button ID="ChangePasswordPushButton" CssClass="srtsButton" runat="server" CommandName="ChangePassword" Text="Change Password"
                                    ValidationGroup="ChangeUserFact2ValidationGroup" />
                            </div>
                        </div>
                        <div class="BeigeBoxFooter"></div>
                    </div>
                </div>

            </ChangePasswordTemplate>
        </asp:ChangePassword>
    </div>
</asp:Content>
