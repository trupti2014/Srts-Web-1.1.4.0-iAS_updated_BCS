<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/srtsMaster.Master" CodeBehind="RecoverPassword.aspx.cs" Inherits="SrtsWeb.Account.RecoverPassword" %>

<asp:Content ContentPlaceHolderID="MainContent_Public" runat="server">
    <asp:PasswordRecovery ID="ForgotPassword" runat="server" OnSendingMail="ForgotPassword_SendingMail" OnVerifyingUser="ForgotPassword_VerifyingUser"
        SuccessText="<h2>Your password has been reset and emailed to you.<br /><a href='../Default.aspx'>Log on</a></h2>"
        EnableViewState="False"
        RenderOuterTable="False"
        VisibleWhenLoggedIn="False"
        EnableTheming="True"
        GeneralFailureText="An error was encountered accessing your account, contact your site administrator for assistance."
        UserNameFailureText="An error was encountered accessing your account, contact your site administrator for assistance.">
        <MailDefinition
            From="noreply@srtsweb.amed.army.mil"
            Subject="New SrtsWeb Password"
            Priority="High"
            BodyFileName="~/Account/RecoveryText.txt">
        </MailDefinition>
        <UserNameTemplate>
            <div>
                <div class="colorRed">
                    <asp:Literal ID="ErrorMessage" runat="server"></asp:Literal>
                </div>
                <div class="colorRed">
                    <asp:Literal ID="FailureText" runat="server"></asp:Literal>
                </div>
                <asp:ValidationSummary ID="ResetPasswordValidationSummary" runat="server" CssClass="failureNotification"
                    ValidationGroup="ResetPasswordValidationGroup" />
            </div>
            <br />
            <br />
            <div class="accountInfo">
                <fieldset class="register">
                    <legend class="srtsLabel_medium">Reset/Retrieve Password</legend>
                    <p class="srtsLabel_medium">Enter your User Name to start retrieval process.</p>
                    <p>
                        <asp:Label ID="UserNameLabel" runat="server" AssociatedControlID="UserName"
                            CssClass="srtsLabel_medium">User Name:</asp:Label>
                        <asp:TextBox ID="UserName" runat="server" CssClass="srtsTextBox_small"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="UserNameRequired" runat="server" CssClass="failureNotification"
                            ControlToValidate="UserName" ErrorMessage="User Name is required." ToolTip="User Name is required."
                            ValidationGroup="ResetPasswordValidationGroup">*</asp:RequiredFieldValidator>
                    </p>
                </fieldset>
                <p class="submitButton">
                    <asp:Button ID="SubmitButton" CssClass="srtsButton" runat="server" CommandName="Submit" Text="Next"
                        ValidationGroup="ResetPasswordValidationGroup" UseSubmitBehavior="false" />
                    <asp:Button ID="CancelButton" runat="server" CssClass="srtsButton" Text="Cancel" PostBackUrl="~/WebForms/Account/Login.aspx" UseSubmitBehavior="false" />
                </p>
        </UserNameTemplate>

        <SuccessTemplate>
            <div>
                <h2><asp:Label ID="lblMessage" runat="server" Text="Your password has been reset and emailed to you."></asp:Label></h2>
                <br />
                <asp:Button ID="ContinuePushButton" runat="server" CssClass="srtsButton" CommandName="Continue" Text="Continue" OnClick="ContinuePushButton_Click" UseSubmitBehavior="false" />
            </div>
        </SuccessTemplate>
    </asp:PasswordRecovery>
</asp:Content>