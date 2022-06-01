<%@ Page Title="Log In" Language="C#" AutoEventWireup="True" MasterPageFile="~/SrtsMaster.Master" CodeBehind="Login.aspx.cs" Inherits="SrtsWeb.Account.Login" %>

<%@ MasterType VirtualPath="~/srtsMaster.Master" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <link rel="stylesheet" type="text/css" href="../../Styles/w3.css" />
    <script type="text/javascript">
        var LoginTabs = '#<%=tbcLogin.ClientID%>';
    </script>

    <style>
        .modalAlert {
            position: absolute;
            top: 200px;
            left: 238px;
            height: 200px;
            width: 400px;
            padding: 0px;
            background: #fff;
            border: 1px solid #004994;
            border-radius: 4px;
            -webkit-box-shadow: 10px 10px 5px 0px rgba(0,0,0,0.12);
            -moz-box-shadow: 10px 10px 5px 0px rgba(0,0,0,0.12);
            box-shadow: 10px 10px 5px 0px rgba(0,0,0,0.12);
        }

            .modalAlert .header {
                font-size: 15px;
                color: #fff;
                padding: 5px 10px;
                background-color: #004994;
                border-bottom: 1px solid #006699;
            }

            .modalAlert .content {
                background-color: #fff;
                padding: 5px 10px 5px 0px;
                width: 350px;
                height: 145px;
                text-align: center;
            }

                .modalAlert .content p {
                    font-size: 17px!important;
                    color: #004994;
                    text-align: center;
                }

            .modalAlert .w3-closebtn {
                margin-top: -3px;
            }
    </style>
</asp:Content>

<asp:Content ID="contentMainContent" ContentPlaceHolderID="MainContent_Public" runat="server">
    <asp:ScriptManagerProxy ID="smpLogin" runat="server">
        <Scripts>
            <asp:ScriptReference Path="~/Scripts/Login/Login.js" />
            <asp:ScriptReference Path="~/Scripts/Login/LoginVal.js" />
            <asp:ScriptReference Path="~/Scripts/Global/SrtsCustomValidators.js" />
        </Scripts>
    </asp:ScriptManagerProxy>

     <%--Security Message--%>
    <div id="divSecurityMessage" class="pnlSecurityMessage" style="display: none; width: 865px; height: 620px;">
        <div class="align_center" style="width: 100%;">
            <div class="contentTitleleft">
                <div style="margin: 20px auto 20px auto; border-bottom: 1px solid #E7CFAD; width: 85%;">
                    <p style="color: #782E1E;">
                        You are accessing a U.S. Government (USG) Information System (IS)<br />
                        that is provided for USG-authorized use only.
                    </p>
                </div>
                <p style="text-align: left; font-size: 14px;">By using this IS (which includes any device attached to this IS), you consent to the following conditions:</p>
            </div>

            <div id="SecurityMessage_MainContent" style="margin-left: 55px; padding: 0px; padding-right: 55px; text-align: left;">

                <br />
                <ul style="list-style-type: disc !important;">
                    <li style="list-style-type: disc !important;">The USG routinely intercepts and monitors communications on this IS for purposes including,<br />
                        but not limited to, penetration testing, COMSEC monitoring, network operations and defense,<br />
                        personnel misconduct (PM), law enforcement (LE), and counterintelligence (CI) investigations.</li>
                    <li style="list-style-type: disc !important;">At any time, the USG may inspect and seize data stored on this IS.</li>
                    <li style="list-style-type: disc !important;">Communications using, or data stored on, this IS are not private, are subject to routine monitoring, interception, and search, and may be disclosed or used for any USG-authorized purpose.</li>
                    <li style="list-style-type: disc !important;">This IS includes security measures (e.g., authentication and access controls) to protect USG interests,<br />
                        not for your personal benefit or privacy.</li>
                    <li style="list-style-type: disc !important;">Notwithstanding the above, using this IS does not constitute consent to PM, LE or CI investigative searching or monitoring of the content of privileged communications, or work product, related to<br />
                        personal representation or services by attorneys, psychotherapists, or clergy, and their assistants.<br />
                        Such communications and work product are private and confidential. See User Agreement for details.</li>
                </ul>
            </div>
            <div style="text-align: center; padding-top: 60px;">
                <input type="submit" value="I Accept" class="srtsButton" onclick="$('[id$=divSecurityMessage]').dialog('close');" />
            </div>
        </div>
    </div>

    <div id="divMultipleColumns" runat="server" style="overflow: hidden; margin-bottom: 20px; max-height:870px; background: #ffffff" ClientIDMode ="Static">
        <asp:Panel ID="pnlMainContent" runat="server" Visible="false">
             <%-- Welcome to the U.S. Department......--%>
            <div style="width: 100%; text-align: center;">
                <div class="contentTitleleft" style="margin: 10px auto 25px auto; width: 575px;">
                    <h2 style="text-align: center; border-bottom: 0px solid #E7CFAD;"><span style="font-size: smaller">Welcome to the U.S. Department of Defense</span><br />
                        Spectacle Request Transmission System (SRTSweb)</h2>
                </div>
            </div>
            <div class="divMultipleColumnsleft">
                 <%--Left Column - User Login--%>
                 <div class="right">
                    <asp:UpdatePanel ID="upLogin" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
                        <ContentTemplate>
                            <asp:HiddenField ID="hdfCacRegistrationCode" runat="server" />
                            <ajaxToolkit:TabContainer ID="tbcLogin" runat="server" CssClass="tabLogin" ActiveTabIndex="0"
                                Visible="True" TabIndex="1" OnClientActiveTabChanged="SetTabFocus">
                                <ajaxToolkit:TabPanel ID="tbpCaCCard" runat="server" HeaderText="CAC/PIV Login" TabIndex="0">
                                    <ContentTemplate>
                                        <div style="margin-top: 0px">
                                            <div class="box_center_top"></div>
                                            <div class="box_center_content">
                                                <h1>Login Using My CaC or PIV</h1>

                                                <div class="tabLogin_cac">
                                                    <br />
                                                    <asp:Label ID="lblCacError" runat="server" Text="" Visible="false" ForeColor="Red"></asp:Label>
                                                    <asp:Label ID="lblCacInsert" runat="server">Make sure your card is<br />inserted into the card reader.</asp:Label>
                                                    <div id="cacLogin_lnk">
                                                        <br />
                                                        <br />
                                                        <br />
                                                        <div style="float: left; width: 120px;"></div>
                                                        <div class="cacLogin_lnk">
                                                            <asp:LinkButton ID="lnkCacLogin" runat="server" class="lnk"
                                                                OnClick="btnCacYes_Click"><b>Log In</b></asp:LinkButton>
                                                            <!-- OnClientClick="document.execCommand('ClearAuthenticationCache')" -->
                                                            <br /><br />
                                                            <div>
                                                                <asp:DropDownList ID="ddlChooseCert" runat="server" Visible="true" OnSelectedIndexChanged="ddlChooseCert_SelectedIndexChanged" CssClass="srtsTextBox" AutoPostBack="true">
                                                                    <asp:ListItem>-- Select Test Cert --</asp:ListItem>
                                                                    <asp:ListItem>Entrust Cert</asp:ListItem>
                                                                    <asp:ListItem>Topaz Cert</asp:ListItem>
                                                                    <asp:ListItem>VA Cert</asp:ListItem>
                                                                </asp:DropDownList>
                                                            </div>
                                                        </div>
                                                        <br />
                                                        <br />
                                                        <asp:LinkButton ID="lnkbRegisterCAC" runat="server" class="lnk" OnClick="lnkbRegisterCAC_Click" Visible="False">Register new CAC?</asp:LinkButton>
                                                        <!-- OnClientClick="document.execCommand('ClearAuthenticationCache')" -->
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="box_center_bottom">
                                                <a id="site" href="~/WebForms/Public/FacilityAccountRequest.aspx?t=site" target="_self" runat="server">Request New Site/Facility</a>
                                                - OR -
                                            <a id="access" href="~/WebForms/Public/FacilityAccountRequest.aspx?t=access" target="_self" runat="server">Request System Access</a>
                                            </div>
                                        </div>
                                    </ContentTemplate>
                                </ajaxToolkit:TabPanel>

                                <ajaxToolkit:TabPanel ID="tbpUserName" runat="server" HeaderText="&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;User ID and Password" TabIndex="1" Visible="False">
                                    <ContentTemplate>
                                        <div style="margin-top: 0px">
                                            <div class="box_center_top"></div>

                                            <div id="divLoginPanel" class="box_center_content" runat="server" visible="true">
                                                <h1>Login with my Username and Password</h1>
                                                <div class="tabLogin_user">
                                                    <asp:Label ID="lblRegisterCAC" runat="server" Visible="False" ForeColor="Red"></asp:Label>
                                                    <div class="padding">
                                                        <asp:Panel ID="LoginPanel" runat="server" DefaultButton="LoginUser$LoginButton">
                                                            <asp:Login ID="LoginUser" runat="server"
                                                                EnableViewState="False"
                                                                RenderOuterTable="False"
                                                                VisibleWhenLoggedIn="False"
                                                                EnableTheming="True"
                                                                OnAuthenticate="LoginUser_Authenticate"
                                                                OnLoggingIn="LoginUser_LoggingIn">
                                                                <LayoutTemplate>

                                                                    <div class="accountInfo">
                                                                        <div id="divUserName">
                                                                            <asp:Label ID="UserNameLabel" runat="server" Text="Username"
                                                                                CssClass="srtsLabel_medium" AssociatedControlID="UserName"></asp:Label>
                                                                            <br />
                                                                            <asp:TextBox ID="UserName" runat="server" CssClass="srtsTextBox_small LoginUserName"></asp:TextBox>
                                                                            <asp:RequiredFieldValidator ID="UserNameRequired" runat="server" ControlToValidate="UserName"
                                                                                ToolTip="User Name is required." ValidationGroup="LoginUserValidationGroup">
                                                <p class="failureNotification_entry">User name is required.</p>
                                                                            </asp:RequiredFieldValidator>
                                                                            <asp:CustomValidator ID="CustUserName" runat="server"
                                                                                ControlToValidate="UserName" SetFocusOnError="false" ToolTip="Invalid characters in user name"
                                                                                ValidationGroup="LoginUserValidationGroup" ClientValidationFunction="validateChars"
                                                                                ErrorMessage="Invalid characters in user name">
                                                <p class="failureNotification_inValidChars">Invalid characters in user name</p>
                                                                            </asp:CustomValidator>
                                                                        </div>
                                                                        <br />
                                                                        <br />

                                                                        <div id="divPasword">
                                                                            <asp:Label ID="lblFact2" runat="server" Text="Password"
                                                                                CssClass="srtsLabel_medium" AssociatedControlID="Password"></asp:Label><br />
                                                                            <asp:TextBox ID="Password" runat="server" TextMode="Password" CssClass="srtsTextBox_small"></asp:TextBox>
                                                                            <asp:RequiredFieldValidator ID="PasswordRequired" runat="server" ControlToValidate="Password"
                                                                                SetFocusOnError="false" ToolTip="Password is required."
                                                                                ValidationGroup="LoginUserValidationGroup">
                                            <p class="failureNotification_entry">Password is required</p>
                                                                            </asp:RequiredFieldValidator>
                                                                            <asp:CustomValidator ID="CustPassword" runat="server" ControlToValidate="Password"
                                                                                SetFocusOnError="false" ToolTip="Invalid characters in password"
                                                                                ValidationGroup="LoginUserValidationGroup" ClientValidationFunction="validateChars">
                                                <p class="failureNotification_inValidChars">Invalid characters in password</p>
                                                                            </asp:CustomValidator>
                                                                        </div>
                                                                    </div>
                                                                    <div>
                                                                        <table>
                                                                            <tr>
                                                                                <td>
                                                                                    <div class="submitButton" style="float: left;">
                                                                                        <asp:Button ID="LoginButton" CssClass="srtsButton" runat="server" CommandName="Login" Text="Log In"
                                                                                            ValidationGroup="LoginUserValidationGroup" OnClientClick="DisableAfterClick(this);"
                                                                                            UseSubmitBehavior="false" />

                                                                                        <asp:HyperLink ID="lnkResetFact2" NavigateUrl="~/WebForms/Account/RecoverPassword.aspx"
                                                                                            class="lnk" runat="server">Forgot password?</asp:HyperLink>

                                                                                        <div class="failureNotificationSummary" style="margin-top: -5px; height: 20px;">
                                                                                            <asp:Literal ID="FailureText" runat="server"></asp:Literal>
                                                                                        </div>
                                                                                        <br />
                                                                                    </div>
                                                                                </td>
                                                                            </tr>
                                                                        </table>
                                                                    </div>
                                                                </LayoutTemplate>
                                                            </asp:Login>
                                                        </asp:Panel>
                                                    </div>
                                                </div>
                                            </div>

                                            <div id="divCacQuestion" class="box_center_content" runat="server" visible="False">
                                                <h1>REGISTER A CAC</h1>
                                                <div>
                                                    <div class="padding">
                                                        <asp:Panel ID="pnlCacQuestion" runat="server">
                                                            <div class="tabLogin_cac">
                                                                <br />
                                                                <br />
                                                                <br />
                                                                <asp:Label ID="lblCacQuestion" runat="server"></asp:Label>
                                                                <br />
                                                                <asp:Button ID="Register" runat="server" CssClass="srtsButton" CommandName="Register" Text="Register" OnClick="btnCacYes_Click" OnClientClick="DisableAfterClick(this);" UseSubmitBehavior="false" Visible="false" />
                                                                <asp:Button ID="btnCacYes" runat="server" CssClass="srtsButton" CommandName="Yes" Text="Yes" OnClick="btnCacYes_Click" OnClientClick="DisableAfterClick(this);" UseSubmitBehavior="false" />
                                                                <asp:Button ID="btnCacNo" runat="server" CssClass="srtsButton" CommandName="No" Text="No" OnClick="btnCacNo_Click" OnClientClick="DisableAfterClick(this);" UseSubmitBehavior="false" CausesValidation="False" />
                                                            </div>
                                                        </asp:Panel>
                                                    </div>
                                                </div>
                                            </div>

                                            <div id="divUserNameToAccount" class="box_center_content" runat="server" visible="False">
                                                <h1>LINK ACCOUNTS</h1>
                                                <div style="height: 190px">
                                                    <div class="padding">
                                                        <asp:Panel ID="pnlUTAQuestion" runat="server">
                                                            <div class="tabLogin_user">
                                                                <asp:Label ID="lblUTAQuestion" runat="server"></asp:Label>
                                                                <br />
                                                                <asp:Button ID="btnUTAYes" runat="server" CssClass="srtsButton" Text="Yes" OnClick="btnUTAYes_Click" OnClientClick="DisableAfterClick(this);" UseSubmitBehavior="false" Width="50px" />
                                                                <asp:Button ID="btnUTANo" runat="server" CssClass="srtsButton" Text="No" OnClick="btnUTANo_Click" OnClientClick="DisableAfterClick(this);" UseSubmitBehavior="false" Width="50px" />
                                                            </div>
                                                        </asp:Panel>

                                                        <asp:Panel ID="pnlUTA" runat="server" Visible="False" DefaultButton="btnValidateButtonUTA">
                                                            <div class="accountInfo">
                                                                <asp:Label ID="lblUTA" runat="server"></asp:Label>
                                                                <div id="divUserName">
                                                                    <asp:Label ID="UserNameLabelUTA" runat="server" Text="Username" CssClass="srtsLabel_medium" AssociatedControlID="txbUserNameUTA"></asp:Label>
                                                                    <br />
                                                                    <asp:TextBox ID="txbUserNameUTA" runat="server" CssClass="srtsTextBox_small"></asp:TextBox>
                                                                    <asp:RequiredFieldValidator ID="UserNameRequired" runat="server" ControlToValidate="txbUserNameUTA"
                                                                        ToolTip="User Name is required." ValidationGroup="LoginUserValidationGroup">
                                                                        <p class="failureNotification_entry">User name is required.</p>
                                                                    </asp:RequiredFieldValidator>
                                                                    <asp:CustomValidator ID="CustUserNameUTA" runat="server" ControlToValidate="txbUserNameUTA" SetFocusOnError="false" ToolTip="Invalid characters in user name"
                                                                        ValidationGroup="LoginUserValidationGroup" ClientValidationFunction="validateChars" ErrorMessage="Invalid characters in user name">
                                                                <p class="failureNotification_inValidChars">Invalid characters in user name</p>
                                                                    </asp:CustomValidator>
                                                                </div>
                                                                <br />
                                                                <br />

                                                                <div>
                                                                    <div id="divPasword">
                                                                        <asp:Label ID="txbFact2LabelUTA" runat="server" Text="Password" CssClass="srtsLabel_medium" AssociatedControlID="txbFact2UTA"></asp:Label>
                                                                        <br />
                                                                        <asp:TextBox ID="txbFact2UTA" runat="server" TextMode="Password" CssClass="srtsTextBox_small"></asp:TextBox>
                                                                        <asp:RequiredFieldValidator ID="PasswordRequired" runat="server" ControlToValidate="txbFact2UTA" ToolTip="Password is required."
                                                                            ValidationGroup="LoginUserValidationGroup">
                                                                    <p class="failureNotification_entry">Password is required</p>
                                                                        </asp:RequiredFieldValidator>
                                                                        <asp:CustomValidator ID="CustPasswordUTA" runat="server" ControlToValidate="txbFact2UTA" SetFocusOnError="false" ToolTip="Invalid characters in password"
                                                                            ValidationGroup="LoginUserValidationGroup" ClientValidationFunction="validateChars">
                                                                    <p class="failureNotification_inValidChars">Invalid characters in password</p>
                                                                        </asp:CustomValidator>
                                                                    </div>
                                                                </div>

                                                                <div class="submitButton">
                                                                    <asp:Button ID="btnValidateButtonUTA" runat="server" CssClass="srtsButton" CommandName="Login" Text="Associate" ValidationGroup="LoginUserValidationGroup" OnClick="btnValidateButtonUTA_Click" OnClientClick="DisableAfterClick(this);" UseSubmitBehavior="false" Width="70px" />
                                                                    <asp:Button ID="btnSkipUTA" runat="server" CssClass="srtsButton" Text="Cancel" CausesValidation="false" OnClick="btnSkipUTA_Click" />
                                                                </div>

                                                                <div class="failureNotificationSummary">
                                                                    <asp:Literal ID="FailureText" runat="server"></asp:Literal>
                                                                </div>
                                                            </div>
                                                        </asp:Panel>
                                                    </div>
                                                </div>
                                            </div>

                                            <div id="divSiteRole" class="box_center_content" runat="server" visible="False">
                                                <h1>ACCOUNT</h1>
                                                <div>
                                                    <div style="">
                                                        <asp:Panel ID="pnlSitesRoles" runat="server">
                                                            <div style="display: block; margin-left: auto; margin-right: auto;">
                                                                <asp:Label ID="lblSiteRoles" runat="server"></asp:Label>
                                                                <asp:GridView ID="gvSitesRoles" runat="server"
                                                                    CellPadding="4" ForeColor="#333333" GridLines="None" DataKeyNames="UserName"
                                                                    CssClass="mGrid" AutoGenerateColumns="False"
                                                                    OnSelectedIndexChanged="gvSitesRoles_SelectedIndexChanged">
                                                                    <AlternatingRowStyle BackColor="White" ForeColor="#284775" CssClass="alt" />
                                                                    <Columns>
                                                                        <asp:CommandField ButtonType="Button" ShowSelectButton="True" />
                                                                        <asp:BoundField DataField="SiteCode" HeaderText="SiteCode" SortExpression="SiteCode"></asp:BoundField>
                                                                        <asp:BoundField DataField="RoleName" HeaderText="Role" SortExpression="RoleName"></asp:BoundField>
                                                                        <asp:BoundField DataField="UserName" HeaderText="UserName" SortExpression="UserName"></asp:BoundField>
                                                                    </Columns>
                                                                    <PagerSettings Mode="NextPreviousFirstLast" Position="TopAndBottom"
                                                                        FirstPageText="<< First" NextPageText="Next >"
                                                                        PreviousPageText="< Previous" LastPageText="Last >>" />
                                                                    <EditRowStyle BackColor="#999999" />
                                                                    <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                                                                    <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" HorizontalAlign="Left" />
                                                                    <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" CssClass="pgr" />
                                                                    <RowStyle BackColor="#F7F6F3" ForeColor="#333333" HorizontalAlign="Left" Wrap="False" />
                                                                    <SelectedRowStyle BackColor="#FFFF99" Font-Bold="True" ForeColor="#333333" />
                                                                    <SortedAscendingCellStyle BackColor="#E9E7E2" />
                                                                    <SortedAscendingHeaderStyle BackColor="#506C8C" />
                                                                    <SortedDescendingCellStyle BackColor="#FFFDF8" />
                                                                    <SortedDescendingHeaderStyle BackColor="#6F8DAE" />
                                                                </asp:GridView>
                                                            </div>
                                                        </asp:Panel>
                                                    </div>
                                                </div>
                                            </div>

                                            <div id="divChangePassword" class="box_center_content" runat="server" visible="false">
                                                <h1>CHANGE PASSWORD</h1>
                                                <div class="tabLogin">
                                                    <div class="padding">
                                                        <div class="colorRed" style="margin-bottom: 15px;">
                                                            <asp:Label runat="server" ID="lblChangeError" ClientIDMode="Static"></asp:Label>
                                                        </div>
                                                        <asp:ChangePassword ID="cpNewFact2" runat="server" DisplayUserName="true"
                                                            EnableViewState="False"
                                                            RenderOuterTable="False"
                                                            VisibleWhenLoggedIn="False"
                                                            EnableTheming="True"
                                                            OnChangingPassword="cpNewFact2_ChangingPassword"
                                                            OnChangePasswordError="cpNewFact2_ChangePasswordError"
                                                            OnContinueButtonClick="cpNewFact2_ContinueButtonClick">
                                                            <ChangePasswordTemplate>
                                                                <div class="accountInfo">
                                                                    <asp:ValidationSummary ID="vsChangePassword" runat="server" DisplayMode="List" ShowSummary="true" ValidationGroup="changePassword" />
                                                                    <div style="margin-bottom: 15px;">
                                                                        <asp:Label ID="UserNameLabel" runat="server" CssClass="srtsLabel_medium" AssociatedControlID="UserName">Username</asp:Label>
                                                                        <br />
                                                                        <asp:TextBox ID="UserName" runat="server" CssClass="srtsTextBox_small"></asp:TextBox>
                                                                        <asp:RequiredFieldValidator ID="UserNameRequired" runat="server" ControlToValidate="UserName"
                                                                            CssClass="failureNotification" ErrorMessage="User name is required." ToolTip="User name is required."
                                                                            ValidationGroup="changePassword"></asp:RequiredFieldValidator>
                                                                    </div>
                                                                    <hr style="width: 50%;" />
                                                                    <div style="margin-top: 8px;">
                                                                        <asp:Label ID="CurrentPasswordLabel" runat="server" CssClass="srtsLabel_medium" AssociatedControlID="CurrentPassword">Old Password</asp:Label>
                                                                        <br />
                                                                        <asp:TextBox ID="CurrentPassword" runat="server" CssClass="srtsTextBox_small" TextMode="Password"></asp:TextBox>
                                                                        <asp:RequiredFieldValidator ID="CurrentPasswordRequired" runat="server" ControlToValidate="CurrentPassword"
                                                                            CssClass="failureNotification" ErrorMessage="Password is required." ToolTip="Old Password is required."
                                                                            ValidationGroup="changePassword">*</asp:RequiredFieldValidator>
                                                                        <asp:CustomValidator ID="CustCurrPassword" runat="server" ControlToValidate="CurrentPassword" SetFocusOnError="false"
                                                                            ToolTip="Invalid characters in password" EnableClientScript="true" ValidateEmptyText="true"
                                                                            ValidationGroup="changePassword" ClientValidationFunction="validatePasswordChars" />
                                                                    </div>
                                                                    <div style="margin-top: 8px;">
                                                                        <asp:Label ID="NewPasswordLabel" runat="server" CssClass="srtsLabel_medium" AssociatedControlID="NewPassword">New Password</asp:Label>
                                                                        <br />
                                                                        <asp:TextBox ID="NewPassword" runat="server" CssClass="srtsTextBox_small" TextMode="Password"></asp:TextBox>
                                                                        <asp:RequiredFieldValidator ID="NewPasswordRequired" runat="server" ControlToValidate="NewPassword"
                                                                            CssClass="failureNotification" ErrorMessage="New Password is required." ToolTip="New Password is required."
                                                                            ValidationGroup="changePassword">*</asp:RequiredFieldValidator>
                                                                        <asp:CustomValidator ID="CustNewPassword" runat="server" ControlToValidate="NewPassword"
                                                                            EnableClientScript="true" ValidateEmptyText="true"
                                                                            SetFocusOnError="false" ToolTip="Invalid characters in password"
                                                                            Display="Dynamic" ValidationGroup="changePassword" ClientValidationFunction="validatePasswordChars" />
                                                                    </div>
                                                                    <div style="margin-top: 8px;">
                                                                        <asp:Label ID="ConfirmNewPasswordLabel" runat="server" CssClass="srtsLabel_medium" AssociatedControlID="ConfirmNewPassword">Confirm New Password</asp:Label>
                                                                        <br />
                                                                        <asp:TextBox ID="ConfirmNewPassword" runat="server" CssClass="srtsTextBox_small" TextMode="Password"></asp:TextBox>
                                                                        <asp:RequiredFieldValidator ID="ConfirmNewPasswordRequired" runat="server" ControlToValidate="ConfirmNewPassword"
                                                                            CssClass="failureNotification" Display="Dynamic" ErrorMessage="Confirm New Password is required."
                                                                            ToolTip="Confirm New Password is required." ValidationGroup="LoginUserValidationGroup">*</asp:RequiredFieldValidator>
                                                                        <asp:CompareValidator ID="NewPasswordCompare" runat="server" ControlToCompare="NewPassword" ControlToValidate="ConfirmNewPassword"
                                                                            CssClass="failureNotification" Display="Dynamic" ErrorMessage="The Confirm New Password must match the New Password entry."
                                                                            ValidationGroup="changePassword">*</asp:CompareValidator>
                                                                        <asp:CustomValidator ID="CustConfirmPassword" runat="server" ControlToValidate="ConfirmNewPassword"
                                                                            EnableClientScript="true" ValidateEmptyText="true"
                                                                            SetFocusOnError="false" ToolTip="Invalid characters in password"
                                                                            ValidationGroup="changePassword" ClientValidationFunction="validatePasswordChars" />
                                                                        <asp:CustomValidator ID="CustPasswordDiff" runat="server" ControlToValidate="ConfirmNewPassword" 
                                                                            SetFocusOnError="false" ToolTip="At least 8 must change." ValidationGroup="changePassword" 
                                                                            OnServerValidate="CustPasswordDiff_ServerValidate" />
                                                                    </div>
                                                                </div>
                                                                <p class="submitButton">
                                                                    <asp:Button ID="ChangePasswordPushButton" runat="server" CssClass="srtsButton" CommandName="ChangePassword" Text="Change Password" ValidationGroup="changePassword"
                                                                        OnClientClick="DisableAfterClick(this);" UseSubmitBehavior="false" />
                                                                    <asp:Button ID="CancelPushButton" runat="server" CssClass="srtsButton" CausesValidation="False" CommandName="Cancel" Text="Cancel" OnClick="CancelPushButton_Click" UseSubmitBehavior="false" />
                                                                </p>
                                                            </ChangePasswordTemplate>
                                                            <SuccessTemplate>
                                                                <h2>Password successfully changed.</h2>
                                                                <br />
                                                                <br />
                                                                <asp:Button ID="ContinuePushButton" runat="server" CssClass="srtsButton" CommandName="Continue" Text="Continue" />
                                                            </SuccessTemplate>
                                                        </asp:ChangePassword>
                                                        <div style="margin-top: 8px;">
                                                            <asp:Label ID="lblMessage" runat="server" CssClass="colorRed" Visible="false"></asp:Label>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="box_center_bottom">
                                                <p class="footer" style="margin-top: 2px; text-align: center; color: #FF0029">
                                                    <div class="footer">
                                                        <a id="A3" href="~/WebForms/Public/FacilityAccountRequest.aspx?t=site" target="_self" runat="server">Request New Site/Facility</a>
                                                        - OR -
                                                <a id="a4" href="~/WebForms/Public/FacilityAccountRequest.aspx?t=access" target="_self" runat="server">Request System Access</a>
                                                    </div>
                                                    <asp:Literal ID="litLoginMessage" runat="server"></asp:Literal>
                                                </p>
                                                <asp:ValidationSummary ID="LoginUserValidationSummary" runat="server" ValidationGroup="LoginUserValidationGroup" />
                                            </div>
                                        </div>
                                    </ContentTemplate>
                                </ajaxToolkit:TabPanel>
                            </ajaxToolkit:TabContainer>
                        </ContentTemplate>
                    </asp:UpdatePanel>

                     <%--Left Column - No CAC? No Problem!--%>
                    <div style="padding: 0px 15px; color: #004994; margin-top: -15px;">
                        <h2 style="font-size: 19px; text-align: center; border-bottom: 1px solid #E7CFAD; margin-bottom: 15px;">Got Questions?</h2>
                        <p style="text-align: justify; text-indent: 10px;">
                            If you need assistance with SRTSweb, please contact the Global Service Center Desk to submit a trouble ticket.
                        </p>
                        <br />
                        <div style="display: table; margin: 0px auto">
                            <p style="text-indent: 0px;">1-800-600-9332 or by <a href="mailto:dhagsc@mail.mil;usarmy.jbsa.medcom-usamitc.list.srts-team@mail.mil?subject=SRTSWeb Support Request&body=When you submit a SRTSWeb support request, please include the following:%0A%0AName:%0A%0APhone Number (including area code):%0A%0AE-mail Address:%0A%0ASite:%0A%0ADepartment:%0A%0ABuilding Number:%0A%0ADetailed description of the request:%0A%0ANote: Do not include Personally Identifiable Information (PII).%0A%0AGlobal Service Center: Please enter ticket and assign to Defense Heath Agency (DHA) | DHA-DHCS | DHCS SRTS">email</a></p>
                        </div>
                       <%-- <p style="font-size: 14px; line-height: 20px; text-align: justify;">
                            The preferred authentication method for SRTSweb is a DoD issued Common Access Card (CAC). 
                            If you do not  have  a card, you can log in with an assigned temporary username and password. 
                            Please contact your facility administrator for further details.
                        </p>--%>
                    </div>
                </div>

                 <%--Left Column - What is SRTSWeb?--%>
                <div class="left">
                    <div class="" style="text-align: justify; margin: -23px 0px 0px 0px; color: #004994;">
                        <h2 style="font-size: 19px; text-align: center; border-bottom: 1px solid #E7CFAD; margin-bottom: 15px;">What is SRTSWeb?</h2>
                        <div class="" style="margin: 0px 0px 0px 20px;">
                            <p style="font-size: 14px; line-height: 22px; text-align: justify;">
                                The Spectacle Request Transmission System (SRTS) is a web-based application that provides the United States Department of Defense (DoD) 
                                    military ordering facilities(clinics) and fabrication facilities (labs)
                                 <img style="float: left; padding: 10px 10px 5px 0px" src="../../Styles/images/geyes7.png" title="U.S. Navy Cmdr. Amy Burin receives an exam for prescription eyeglasses at the Naval Branch Health Clinic at Naval Support Activity Bahrain, Manama, Bahrain, Sept. 22, 2010. The optometry department's one optometrist and one optometry technician provide services to 4,000 personnel. Burin is assigned to U.S. Naval Forces Central Command." />
                                with an automated mechanism to order and track military eyewear.
                            </p>

                            <p style="font-size: 14px; line-height: 22px; text-align: justify;">
                                In addition to servicing our eyewear facilities, we also provide our military personnel who are in theatre access to our <a href="../GEyes/Forms/GEyesHomePage.aspx">G-Eyes application</a>
                                allowing our soldiers to reorder eyewear as the need arises.
                            </p>
                            <p style="font-size: 14px; line-height: 22px; text-align: justify;">
                                SRTSweb is a secure system for authorized personnel use only.  A clinic or lab wishing to use SRTSweb to manage military eyewear processing, must obtain a<a id="lnkAuthAccount" href="~/WebForms/Public/FacilityAccountRequest.aspx?t=access" target="_self" runat="server"> NOSTRA authorized account</a>.
                            </p>
                        </div>
                    </div>
                </div>


            </div>

             <%--Left Column Bottom - News and Announcements--%>
            <div class="divMultipleColumnsleft" style="clear: both">
                <h2 style="font-size: 19px; text-align: center; border-bottom: 1px solid #E7CFAD">News and Announcements from the SRTSweb Team&nbsp;&nbsp;</h2>
                <div style="display: table; margin: 0px auto; padding: 20px 0px;">
                    <h2 style="font-size: 16px; text-align: left; color: #004994; font-weight: bold;">Remember to check this page often for  important information and updates!
                    </h2>
                </div>
                <div style="float: left; width: 99%; text-align: left">
                    <asp:Repeater ID="rpAnnouncements" runat="server" OnItemDataBound="rpAnnouncements_ItemDataBound">
                        <HeaderTemplate>
                            <ul>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <li class="newsheadline">
                                <asp:Literal ID="litHeadline" runat="server"></asp:Literal></li>
                            <li>
                                <div style="float:left;clear:both"><asp:Literal ID="litSummary" runat="server"></asp:Literal></div></li>
                        </ItemTemplate>
                        <FooterTemplate>
                            </ul>
                        </FooterTemplate>
                    </asp:Repeater>
                </div>
            </div>

             <%--Right Column - G-Eyes Orders, Got Questions--%>
            <div class="divMultipleColumnsright" style="position: relative; top: -514px;">
                <div class="" style="margin: 15px 0px 0px 0px; color: #004994;">
                    <h2 style="font-size: 19px; text-align: center; border-bottom: 1px solid #E7CFAD; margin-bottom: 5px">G-Eyes Orders&nbsp;&nbsp;</h2>
                    <div id="divGeyes" class="" style="margin-top: -30px">
                        <div class="gotogeyes"><a href="../GEyes/Forms/GEyesHomePage.aspx">Click here to go to G-Eyes</a></div>
                        <div class="gotogeyes"><a href="../JSpecs/Forms/JSpecsHomePage.aspx">Click here to go to JSpecs</a></div>
                        <p style="font-size: 14px; line-height: 22px; text-align: justify;">
                            In order to use G-Eyes, you must be deployed to an APO, FPO, or DPO and have an optical prescription on file.                          
                        </p>
                        <div style="margin: 10px 0px 10px 10px">
                            <img src="../../Styles/images/geyes6.png" style="width: 100%" title="Airman 1st Class Nathan Kosters, a crew chief with the 34th Aircraft Maintenance Unit, prepares to launch an F-35A Lightning II aircraft during Red Flag 17-1 at Nellis Air Force Base, Nevada, Feb. 7, 2017. (U.S. Air Force photo/R. Nial Bradshaw)" />
                        </div>

                        <p style="font-size: 14px; line-height: 22px; text-align: justify; margin-top: 0px">
                            Military Combat Eye Protection is a unit issue item. Please see your unit supply representative if you are in need of those items.
                        </p>
                    </div>
                    
                    <%--Right Column - Check My Order Status--%>
                    <div>
                         <h2 style="font-size: 19px; text-align: center; border-bottom: 1px solid #E7CFAD; margin-top:30px;margin-bottom: 15px">Check My Order Status&nbsp;&nbsp;</h2>
                       <p style="font-size:14px;text-align:center">Your most recent order status(es) are available.<br />
                       &nbsp;&nbsp;&nbsp;<a id="A11" href="~/WebForms/Public/CheckOrderStatus.aspx" runat="server">Click here</a> to get an update.
                       </p>
                    </div>
                    <br /><br />


                </div>
                <div class="" style="text-align: left; margin: 3px 0px 0px 0px; color: #004994;">
                    <h1 style="font-size: 19px; text-align: center; border-bottom: 1px solid #E7CFAD; margin-bottom: 15px">System Requirements&nbsp;&nbsp;</h1>
                    <%-- <div style="background-color: #FFFFFF; border-radius: 20px; border: 2px solid #DEC99A">--%>
                    <div class="padding;">
                        <%--<p style="text-align: justify; text-indent: 10px;">
                            If you need assistance with SRTSweb, please contact the Global Service Center Desk to submit a trouble ticket.
                        </p>
                        <br />
                        <div style="display: table; margin: 0px auto">
                            <p style="text-indent: 0px;">1-800-600-9332 or by <a href="mailto:dhagsc@mail.mil;usarmy.jbsa.medcom-usamitc.list.srts-team@mail.mil?subject=SRTSWeb Support Request&body=When you submit a SRTSWeb support request, please include the following:%0A%0AName:%0A%0APhone Number (including area code):%0A%0AE-mail Address:%0A%0ASite:%0A%0ADepartment:%0A%0ABuilding Number:%0A%0ADetailed description of the request:%0A%0ANote: Do not include Personally Identifiable Information (PII).%0A%0AGlobal Service Center: Please enter ticket and assign to Defense Heath Agency (DHA) | DHA-DHCS | DHCS SRTS">email</a></p>
                        </div>--%>
                   <%--     <br />--%>
                        <%-- <hr style="width: 95%; text-align: center;" />--%>

                     <%--   <h1 style="font-weight: bold;">System Requirements</h1>--%>
                        <p style="text-indent: 10px;">SRTSweb is optimized for Internet Explorer version 9 or higher with Javascript enabled and may not operate properly in other browsers.</p>
                    </div>
                    <%-- </div>--%>
                    <%--<srts:BoxRight ID="modTechSupport" runat="server" Title="We have answers">
                        <img class="" id="imgTechSupport" style="position: relative; float: left; top: -15px;" alt="Header Image" src="../Styles/images/img_help_1.png">
                        <div class="padding">
                            <p style="text-align: justify; text-indent: 10px;">
                                If you need assistance with SRTSweb, please contact the Global Service Center Desk to submit a trouble ticket.
                            </p>
                            <br />
                            <div style="display: table; margin: 0px auto;">
                                <p style="text-indent: 0px;">1-800-872-6482 or by <a href="mailto:dhagsc@mail.mil;usarmy.jbsa.medcom-usamitc.list.srts-team@mail.mil?subject=SRTSWeb Support Request&body=When you submit a SRTSWeb support request, please include the following:%0A%0AName:%0A%0APhone Number (including area code):%0A%0AE-mail Address:%0A%0ASite:%0A%0ADepartment:%0A%0ABuilding Number:%0A%0ADetailed description of the request:%0A%0ANote: Do not include Personally Identifiable Information (PII).%0A%0AGlobal Service Center: Please enter ticket and assign to Defense Heath Agency (DHA) | DHA-DHCS | DHCS SRTS">email</a></p>
                            </div>
                            <br />
                            <hr style="width: 95%; text-align:center" />
                            <br />
                            <h1 style="font-weight: bold;">System Requirements</h1>
                            <p style="text-indent: 10px;">SRTS is optimized for Internet Explorer version 8 with Javascript enabled.</p>
                            <h1 style="float: left; position: relative; top: -248px; left: 50px; font-weight: bold;">We have answers....</h1>
                    </div>
                    </srts:BoxRight>--%>
                </div>
            </div>
        </asp:Panel>
        
        <%--Alert Message to display CAC Login Requirement--%>
        <div id="modalAlert" class="w3-modal">
            <div class="w3-modal-content">
                <div class="w3-container">
                    <div class="modalAlert">
                        <div class="header">
                            <span onclick="document.getElementById('modalAlert').style.display='none'"
                                class="w3-closebtn">&times;</span>
                            SRTSweb Message
                        </div>
                        <div class="content">
                            <p>
                                <img src="../../Styles/images/img_user_cac.png" alt="CAC" class="" style="float: left; margin-top: -5px" />
                                <br />
                                <br />
                                Please use your CAC to log in!
                            </p>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

</asp:Content>
