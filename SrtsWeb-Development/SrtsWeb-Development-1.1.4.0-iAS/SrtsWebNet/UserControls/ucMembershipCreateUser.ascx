<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ucMembershipCreateUser.ascx.cs" Inherits="SrtsWeb.UserControls.ucMembershipCreateUser" %>

<div style="margin-top: 0px; margin-left: 0px; margin-bottom: 5px; float: right; padding-top: 50px; width: 49%;">

    <asp:Label ID="ErrorMessage" runat="server" CssClass="failureNotificationSummary"></asp:Label>
</div>
<div style="margin-top: 0px; margin-left: 0px; clear: both; float: left; width: 100%;">
    <asp:CreateUserWizard ID="RegisterUser" runat="server" EnableViewState="False" OnCreatedUser="RegisterUser_CreatedUser"
        LoginCreatedUser="False" OnCreatingUser="RegisterUser_CreatingUser">
        <LayoutTemplate>
            <asp:PlaceHolder ID="wizardStepPlaceholder" runat="server"></asp:PlaceHolder>
            <asp:PlaceHolder ID="navigationPlaceholder" runat="server"></asp:PlaceHolder>
        </LayoutTemplate>
        <WizardSteps>
            <asp:CreateUserWizardStep ID="RegisterUserWizardStep" runat="server">
                <ContentTemplate>
                    <fieldset style="border: 1px solid; padding: 5px; width: 375px;">
                        <legend>Use the form below to create a new account.</legend>
                        <br />
                        <ul>
                            <li>- Passwords are required to have a minimum of <%= Membership.MinRequiredPasswordLength %> characters.</li>
                            <li>- Passwords must be made up of at least:</li>
                            <ul style="margin-left: 15px;">
                                <li>- 2 Upper case letters</li>
                                <li>- 2 Lower letter</li>
                                <li>- 2 Numbers</li>
                                <li>- 2 Special characters & ! @ # $ % ^ * ( )</li>
                            </ul>
                        </ul>
                    </fieldset>
                    <br />
                    <br />
                    <div class="accountInfo">
                        <br />
                        <br />
                        <fieldset class="register">
                            <table>
                                <tr>
                                    <td>
                                        <asp:Label ID="UserNameLabel" runat="server" AssociatedControlID="UserName" CssClass="srtsLabel_medium_MCU">User Name:</asp:Label>
                                        <asp:TextBox ID="UserName" runat="server" CssClass="srtsTextBox_small_MCU" ValidationGroup="RegisterUserValidationGroup" ToolTip="Example: j.doe.ct"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="UserNameRequired" runat="server" ControlToValidate="UserName" Display="Dynamic"
                                            CssClass="validation_error_MCU" ErrorMessage="User Name is required." ToolTip="User Name is required."
                                            ValidationGroup="RegisterUserValidationGroup">* User Name is required.</asp:RequiredFieldValidator>
                                        <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" Display="Dynamic"
                                            ValidationExpression="^[a-zA-Z0-9]+(\.[a-zA-Z0-9]+)+$"
                                            CssClass="validation_error_MCU" ErrorMessage="Invalid input. Valid input example: j.doe.ct (John Doe the clinic tech)" ControlToValidate="UserName"
                                            ValidationGroup="RegisterUserValidationGroup">* Invalid input. Valid input example: j.doe.ct (John Doe the clinic tech)</asp:RegularExpressionValidator>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Label ID="EmailLabel" runat="server" AssociatedControlID="Email" CssClass="srtsLabel_medium_MCU">E-mail:</asp:Label>
                                        <asp:TextBox ID="Email" runat="server" CssClass="srtsTextBox_small_MCU"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="EmailRequired" runat="server" ControlToValidate="Email" Display="Dynamic"
                                            CssClass="validation_error_MCU" ErrorMessage="E-mail is required." ToolTip="E-mail is required."
                                            ValidationGroup="RegisterUserValidationGroup">* E-mail is required.</asp:RequiredFieldValidator>
                                        <asp:RegularExpressionValidator ID="EmailValid" runat="server" Display="Dynamic"
                                            ValidationExpression="^([a-zA-Z][\w\.-]*[a-zA-Z0-9]@[a-zA-Z0-9][\w\.-]*[a-zA-Z0-9]\.[a-zA-Z][a-zA-Z\.]*[a-zA-Z]){1,70}$"
                                            CssClass="validation_error_MCU" ErrorMessage="Invalid email format entered" ControlToValidate="Email"
                                            ValidationGroup="RegisterUserValidationGroup">* Invalid email format entered.</asp:RegularExpressionValidator>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Label ID="PasswordLabel" runat="server" AssociatedControlID="Password" CssClass="srtsLabel_medium_MCU">Password:</asp:Label>
                                        <asp:TextBox ID="Password" runat="server" CssClass="srtsTextBox_small_MCU" TextMode="Password"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="PasswordRequired" runat="server" ControlToValidate="Password" Display="Dynamic"
                                            CssClass="validation_error_MCU" ErrorMessage="Password is required." ToolTip="Password is required."
                                            ValidationGroup="RegisterUserValidationGroup">* Password is required.</asp:RequiredFieldValidator></td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Label ID="ConfirmPasswordLabel" runat="server" AssociatedControlID="ConfirmPassword" CssClass="srtsLabel_medium_MCU">Confirm Password:</asp:Label>
                                        <asp:TextBox ID="ConfirmPassword" runat="server" CssClass="srtsTextBox_small_MCU" TextMode="Password"></asp:TextBox>
                                        <asp:RequiredFieldValidator ControlToValidate="ConfirmPassword" CssClass="validation_error_MCU" Display="Dynamic"
                                            ErrorMessage="Confirm Password is required." ID="ConfirmPasswordRequired" runat="server"
                                            ToolTip="Confirm Password is required." ValidationGroup="RegisterUserValidationGroup">* Confirm Password is required.</asp:RequiredFieldValidator>
                                        <asp:CompareValidator ID="PasswordCompare" runat="server" ControlToCompare="Password" ControlToValidate="ConfirmPassword"
                                            CssClass="validation_error_MCU" Display="Dynamic" ErrorMessage="The Password and Confirmation Password must match."
                                            ValidationGroup="RegisterUserValidationGroup">* The Password and Confirmation Password must match.</asp:CompareValidator></td>
                                </tr>
                            </table>
                        </fieldset>
                        <p class="submitButton">
                            <asp:Button ID="CreateUserButton" runat="server" CssClass="srtsButton" CommandName="MoveNext" Text="Submit" ValidationGroup="RegisterUserValidationGroup" />
                            <asp:Button ID="btnCancel" runat="server" CssClass="srtsButton" Text="Cancel" OnClick="btnCancel_Click" />
                        </p>
                    </div>
                </ContentTemplate>
                <CustomNavigationTemplate>
                </CustomNavigationTemplate>
            </asp:CreateUserWizardStep>
            <asp:CompleteWizardStep runat="server">
                <ContentTemplate>
                    <table>
                        <tr>
                            <td style="text-align: center" colspan="2">Complete</td>
                        </tr>
                        <tr>
                            <td>Your account has been successfully created.</td>
                        </tr>
                        <tr>
                            <td style="text-align: right" colspan="2">
                                <asp:Button ID="ContinueButton" runat="server" CssClass="srtsButton" CausesValidation="False"
                                    CommandName="Continue" Text="Continue" ValidationGroup="RegisterUser" OnClick="ContinueButton_Click" />
                            </td>
                        </tr>
                    </table>
                </ContentTemplate>
            </asp:CompleteWizardStep>
        </WizardSteps>
    </asp:CreateUserWizard>
</div>
