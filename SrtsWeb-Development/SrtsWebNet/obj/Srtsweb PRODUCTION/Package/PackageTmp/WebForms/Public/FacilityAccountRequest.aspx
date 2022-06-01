<%@ Page Title="" Language="C#" MasterPageFile="~/srtsMaster.Master" AutoEventWireup="true" CodeBehind="FacilityAccountRequest.aspx.cs" Inherits="SrtsWeb.Public.FacilityAccountRequest" MaintainScrollPositionOnPostback="true" %>

<%@ MasterType VirtualPath="~/srtsMaster.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script src="../../Scripts/Global/SrtsCustomValidators.js"></script>
    <style>
    .failMessage
    {
        background-image: url('../Styles/images/img_Xround.png');
        background-repeat: no-repeat;
        background-position: 10px 2px;
        font: bold 1em arial, verdana;
        text-align: left;
        padding: 5px 0px 0px 40px;
        min-height: 22px;
        overflow-y: visible;
        width: 300px;
        margin-left: auto;
        margin-right: auto;
        padding: 5px 0px 0px 40px;
        background-color: #f72e21;
        color: #fff;
        border: 1px solid #f72e21;
        border-radius: 5px;
    }
    </style>
</asp:Content>

<asp:Content ID="Content6" ContentPlaceHolderID="MainContent_Public" runat="server">
    <div class="box_full_top" style="margin-top: 30px; padding-top: 10px; text-align: center; margin-bottom: -10px">
        <h1 style="font-size: 1.4em; margin-bottom: 0px">New Facility / System Access Request</h1>
    </div>
    <div class="box_full_content" style="min-height: 300px">
        <div class="padding">
            <div id="divNewSiteRequest" runat="server">
                <div class="padding" style="margin: -20px 30px 0px 30px; color: #004994">
                    <p>
                        SRTSweb is a secure system for authorized personnel use only. A clinic or lab wishing to use SRTSweb to manage military eyewear processing, must obtain a NOSTRA authorized account.

            To register a <b>new</b> lab or clinic, please complete the registration form below.  You will be notified via email once NOSTRA has created your account.
                    </p>
                    <br />
                    <p>Note: If you are an existing lab or clinic already using SRTS and you require user access, please complete the <a id="access" href="~/WebForms/Public/FacilityAccountRequest.aspx?t=access" target="_self" runat="server">Request System Access</a> form.</p>
                </div>

                <h1 class="headerBlue" style="text-align: center; border-bottom: 1px solid #E7CFAD">Facility Account Request Form</h1>
                <br />
                <div style="margin-bottom: -25px">
                    <asp:Panel ID="pnlNewSiteRequestSuccess" runat="server" CssClass="successMessage" Visible="false">Success!</asp:Panel>
                </div>
                <asp:ValidationSummary ID="ValidationSummary" runat="server" ValidationGroup="allValidators" CssClass="validatorSummary" />

                <asp:Panel ID="pnlMain" runat="server">
                    <div style="float: left; width: 500px; height: 350px; margin: 0px auto 30px auto; border-bottom: 1px solid #E7CFAD; padding: 20px 0px">
                        <div style="float: right; width: 300px">
                            <asp:TextBox ID="txtRequesterName" runat="server" CssClass="srtsTextBox_medium" MaxLength="40" TabIndex="1" ToolTip="Enter your name." />
                            <asp:CustomValidator ID="cvRequesterName" runat="server" ErrorMessage="CustomValidator" EnableClientScript="False" OnServerValidate="ValidateRequestorName" ValidationGroup="allValidators" Text="*" CssClass="requestValidator" ControlToValidate="txtRequesterName" ValidateEmptyText="True"></asp:CustomValidator>
                            <br />
                            <br />
                            <asp:TextBox ID="txtRequesterTitle" runat="server" CssClass="srtsTextBox_medium" MaxLength="40" TabIndex="2" ToolTip="Enter your title." />
                            <asp:CustomValidator ID="cvRequesterTitle" runat="server" ErrorMessage="CustomValidator" EnableClientScript="False" OnServerValidate="ValidateRequestorTitle" ValidationGroup="allValidators" Text="*" CssClass="requestValidator" ControlToValidate="txtRequesterTitle" ValidateEmptyText="True"></asp:CustomValidator>
                            <br />
                            <br />
                            <asp:TextBox ID="txtWorkPhone" runat="server" CssClass="srtsTextBox_medium" TabIndex="3" ToolTip="Enter your work phone number (e.g., (210)555-1234 ext 123)." />
                            <asp:CustomValidator ID="cvWorkPhone" runat="server" ErrorMessage="CustomValidator" EnableClientScript="False" OnServerValidate="ValidateWorkPhone" ValidationGroup="allValidators" Text="*" CssClass="requestValidator" ControlToValidate="txtWorkPhone" ValidateEmptyText="True"></asp:CustomValidator>
                            <br />
                            <br />
                            <asp:TextBox ID="txtDSNPhone" runat="server" CssClass="srtsTextBox_medium" TabIndex="4" ToolTip="Enter your DSN phone number (e.g., 555-1234)." />
                            <asp:CustomValidator ID="cvDSNPhone" runat="server" ErrorMessage="CustomValidator" EnableClientScript="False" OnServerValidate="ValidateDSNPhone" ValidationGroup="allValidators" Text="*" CssClass="requestValidator" ControlToValidate="txtDSNPhone" ValidateEmptyText="True"></asp:CustomValidator>
                            <br />
                            <br />
                            <asp:TextBox ID="txtFax" runat="server" CssClass="srtsTextBox_medium" ToolTip="Enter your FAX number." numberTabIndex="5" />
                            <asp:CustomValidator ID="cvFax" runat="server" ErrorMessage="CustomValidator" EnableClientScript="False" OnServerValidate="ValidateFax" ValidationGroup="allValidators" Text="*" CssClass="requestValidator" ControlToValidate="txtFax" ValidateEmptyText="True"></asp:CustomValidator>
                            <br />
                            <br />
                            <asp:TextBox ID="txtEmail" runat="server" CssClass="srtsTextBox_medium" ToolTip="Enter your Email Address" TabIndex="6" />
                            <asp:RequiredFieldValidator ID="rfvEmail"
                                runat="server"
                                ControlToValidate="txtEmail"
                                ValidationGroup="allValidators"
                                Display="None"
                                ErrorMessage="Your Email is required">
                            </asp:RequiredFieldValidator>
                            <asp:RegularExpressionValidator ID="revEmailAddress"
                                runat="server"
                                ErrorMessage="Email Address format is not correct"
                                ValidationGroup="allValidators"
                                Display="None"
                                ControlToValidate="txtEmail"
                                ValidationExpression="^([a-zA-Z][\w\.-]*[a-zA-Z0-9]@[a-zA-Z0-9][\w\.-]*[a-zA-Z0-9]\.[a-zA-Z][a-zA-Z\.]*[a-zA-Z]){1,70}$">
                            </asp:RegularExpressionValidator>
                            <br />
                            <br />
                            <br />
                            <br />
                            <br />
                            <asp:DropDownList ID="ddlFacilityType" runat="server" CssClass="srtsDropDown_medium" TabIndex="7"></asp:DropDownList>
                            <asp:RequiredFieldValidator ID="rfvFacilityType" runat="server" ErrorMessage="Select a facility type" Text="*" CssClass="requestValidator" ValidationGroup="allValidators" ControlToValidate="ddlFacilityType" InitialValue="-- Select --"></asp:RequiredFieldValidator>
                            <br />
                            <br />
                            <asp:DropDownList ID="ddlFaclityBOS" runat="server" CssClass="srtsDropDown_medium" TabIndex="8"></asp:DropDownList>
                            <asp:RequiredFieldValidator ID="rfvBOS" runat="server" ControlToValidate="ddlFaclityBOS" CssClass="requestValidator" ValidationGroup="allValidators" Text="*" ErrorMessage="Select facility BOS" InitialValue="-- Select --"></asp:RequiredFieldValidator>
                            <br />
                            <br />
                            <asp:DropDownList ID="ddlFacilityComponent" runat="server" CssClass="srtsDropDown_medium" TabIndex="9">
                                <asp:ListItem Selected="True">-- Select --</asp:ListItem>
                                <asp:ListItem>ACTIVE</asp:ListItem>
                                <asp:ListItem>RESERVE</asp:ListItem>
                            </asp:DropDownList>
                            <asp:RequiredFieldValidator ID="rfvFavilityComponent" runat="server" ControlToValidate="ddlFacilityComponent" CssClass="requestValidator" ValidationGroup="allValidators" Text="*" ErrorMessage="Select facility component" InitialValue="-- Select --"></asp:RequiredFieldValidator>
                            <br />
                            <br />
                        </div>

                        <div style="float: left; width: 190px; text-align: right">
                            <asp:Label ID="lblRequesterName" runat="server" CssClass="srtsLabel_medium" Text="Requester Name:*" />
                            <br />
                            <br />
                            <asp:Label ID="lblRequesterTitle" runat="server" CssClass="srtsLabel_medium" Text="Requester Title:*" />
                            <br />
                            <br />
                            <asp:Label ID="lblWorkPhone" runat="server" CssClass="srtsLabel_medium" Text="Requester Work Phone:*" />
                            <br />
                            <br />
                            <asp:Label ID="lblDSNPhone" runat="server" CssClass="srtsLabel_medium" Text="Requester DSN Phone:" />
                            <br />
                            <br />
                            <asp:Label ID="lblFax" runat="server" CssClass="srtsLabel_medium" Text="Requester FAX:" />
                            <br />
                            <br />
                            <asp:Label ID="lblEmail" runat="server" CssClass="srtsLabel_medium" Text="Requester Email:*" />
                            <br />
                            <br />
                            <br />
                            <br />
                            <br />
                            <asp:Label ID="lblSiteType" runat="server" CssClass="srtsLabel_medium" Text="Facility Type:*" />
                            <br />
                            <br />
                            <asp:Label ID="lblFacilityBranchofService" runat="server" CssClass="srtsLabel_medium" Text="Facility Branch of Service:*" />
                            <br />
                            <br />
                            <asp:Label ID="lblFacilityComponent" runat="server" CssClass="srtsLabel_medium" Text="Facility Component:*" />
                            <br />
                            <br />
                        </div>
                    </div>
                    <div style="float: right; width: 500px; height: 350px; margin: 0px auto 30px auto; border-bottom: 1px solid #E7CFAD; padding: 20px 0px">
                        <div style="float: right; width: 300px">
                            <asp:TextBox ID="txtUnitName" runat="server" CssClass="srtsTextBox_medium" TabIndex="10" />
                            <asp:CustomValidator ID="cvUnitName" runat="server" ErrorMessage="CustomValidator" EnableClientScript="False" OnServerValidate="ValidateUnitName" ValidationGroup="allValidators" Text="*" CssClass="requestValidator" ControlToValidate="txtUnitName" ValidateEmptyText="True"></asp:CustomValidator>
                            <br />
                            <br />
                            <asp:TextBox ID="txtUnitAddress1" runat="server" CssClass="srtsTextBox_medium" TabIndex="11" />
                            <asp:CustomValidator ID="cvAddress" runat="server" ErrorMessage="CustomValidator" EnableClientScript="False" OnServerValidate="ValidateAddress" ValidationGroup="allValidators" Text="*" CssClass="requestValidator" ControlToValidate="txtUnitAddress1" ValidateEmptyText="True"></asp:CustomValidator>
                            <br />
                            <br />
                            <asp:TextBox ID="txtUnitAddress2" runat="server" CssClass="srtsTextBox_medium" TabIndex="12" />
                            <asp:CustomValidator ID="cvAddress2" runat="server" ErrorMessage="CustomValidator" EnableClientScript="False" OnServerValidate="ValidateAddress2" ValidationGroup="allValidators" Text="*" CssClass="requestValidator" ControlToValidate="txtUnitAddress2" ValidateEmptyText="True"></asp:CustomValidator>
                            <br />
                            <br />
                            <asp:TextBox ID="txtUnitAddress3" runat="server" CssClass="srtsTextBox_medium" TabIndex="13" />
                            <asp:CustomValidator ID="cvAddress3" runat="server" ErrorMessage="CustomValidator" EnableClientScript="False" OnServerValidate="ValidateAddress2" ValidationGroup="allValidators" Text="*" CssClass="requestValidator" ControlToValidate="txtUnitAddress3" ValidateEmptyText="True"></asp:CustomValidator>
                            <br />
                            <br />
                            <asp:TextBox ID="txtUnitCity" runat="server" CssClass="srtsTextBox_medium" TabIndex="14" />
                            <asp:CustomValidator ID="cvCity" runat="server" ErrorMessage="CustomValidator" EnableClientScript="False" OnServerValidate="ValidateCity" ValidationGroup="allValidators" Text="*" CssClass="requestValidator" ControlToValidate="txtUnitCity" ValidateEmptyText="True"></asp:CustomValidator>
                            <br />
                            <br />
                            <asp:DropDownList ID="ddlState" runat="server" CssClass="srtsTextBox_medium" TabIndex="15" AutoPostBack="True" OnSelectedIndexChanged="ddlState_SelectedIndexChanged"></asp:DropDownList>
                            <asp:RequiredFieldValidator ID="rfvState" runat="server" ControlToValidate="ddlState" CssClass="requestValidator" ValidationGroup="allValidators" Text="*" ErrorMessage="Select State" InitialValue="-- Select --"></asp:RequiredFieldValidator>
                            <br />
                            <br />
                            <asp:DropDownList ID="ddlCountry" runat="server" CssClass="srtsTextBox_medium" TabIndex="16" AutoPostBack="True" OnSelectedIndexChanged="ddlCountry_SelectedIndexChanged"></asp:DropDownList>
                            <asp:RequiredFieldValidator ID="rfvCountry" runat="server" ControlToValidate="ddlCountry" CssClass="requestValidator" ValidationGroup="allValidators" Text="*" ErrorMessage="Select Country" InitialValue="-- Select --"></asp:RequiredFieldValidator>
                            <br />
                            <br />
                            <asp:TextBox ID="txtUnitZipCode" runat="server" CssClass="srtsTextBox_medium" ToolTip="Enter five numbers or nine numbers with a hyphen (e.g, 55555 or 55555-4444)." TabIndex="17" />
                            <asp:CustomValidator ID="cvZipCode" runat="server" ErrorMessage="CustomValidator" EnableClientScript="False" OnServerValidate="ValidateZipCode" ValidationGroup="allValidators" Text="*" CssClass="requestValidator" ControlToValidate="txtUnitZipCode" ValidateEmptyText="True"></asp:CustomValidator>
                            <br />
                            <br />
                            <asp:TextBox ID="txtComments" runat="server" TextMode="MultiLine" Rows="5" CssClass="srtsTextBox_medium_multi" onKeyDown="return textboxMaxCommentSize(this, 256, event, getlblRemainingID(), gettbCommentID())" ToolTip="Enter any comments, valid characters include + - . , ! ? # ' / ( )" TabIndex="18" />
                            <asp:CustomValidator ID="cvComment" runat="server" ErrorMessage="Invalid character(s) in Comment" ValidationGroup="allValidators" Text="*" CssClass="requestValidator" ControlToValidate="txtComments" ValidateEmptyText="True" ClientValidationFunction="textboxCommentValidation" OnServerValidate="ValidateCommentFormat"></asp:CustomValidator>
                            <br />
                            <br />
                        </div>
                        <div style="float: left; width: 190px; text-align: right">
                            <asp:Label ID="lblUnitName" runat="server" CssClass="srtsLabel_medium" Text="Unit Name:*" />
                            <br />
                            <br />
                            <asp:Label ID="lblUnitAddress1" runat="server" CssClass="srtsLabel_medium" Text="Unit Address 1:*" />
                            <br />
                            <br />
                            <asp:Label ID="lblUnitAddress2" runat="server" CssClass="srtsLabel_medium" Text="Unit Address 2:" />
                            <br />
                            <br />
                            <asp:Label ID="lblUnitAddress3" runat="server" CssClass="srtsLabel_medium" Text="Unit Address 3:" />
                            <br />
                            <br />
                            <asp:Label ID="lblUnitCity" runat="server" CssClass="srtsLabel_medium" Text="Unit City:*" />
                            <br />
                            <br />
                            <asp:Label ID="lblUnitState" runat="server" CssClass="srtsLabel_medium" Text="Unit State:*" />
                            <br />
                            <br />
                            <asp:Label ID="lblCountry" runat="server" CssClass="srtsLabel_medium" Text="Unit Country:*" />
                            <br />
                            <br />
                            <asp:Label ID="lblUnitZipCode" runat="server" CssClass="srtsLabel_medium" Text="Unit Zip Code:*" />
                            <br />
                            <br />
                            <br />
                            <br />
                            <asp:Label ID="lblComments" runat="server" CssClass="srtsLabel_medium" Text="Comments:" ClientIDMode="Static" /><br />
                            <asp:Label ID="lblRemaining" runat="server" CssClass="srtsLabel_medium" ClientIDMode="Static"></asp:Label>

                            <br />
                            <br />
                        </div>
                    </div>
                </asp:Panel>
            </div>

            <div id="divSrtsAccessRequest" runat="server">
                <div class="padding" style="margin: 0px 30px 15px 30px; color: #004994">
                    <p>SRTSweb is a secure system for authorized personnel use only.  To request access to the system, you must know your SRTS site code.</p>
                    <br />
                    <p>Please fill out the below form.  The SRTS team will notify you via email with further instructions.</p>
                    <br />
                    <p>Note: If you are a NEW site/facility and do not yet have a SRTS site code, please complete the <a id="site" href="~/WebForms/Public/FacilityAccountRequest.aspx?t=site" target="_self" runat="server">Request New Site/Facility</a> form.</p>
                </div>

                <h1 class="headerBlue" style="text-align: center; border-bottom: 1px solid #E7CFAD">SRTS Web System Access Request Form</h1>
                <br />
                <div style="margin-bottom: -25px">
                    <asp:Panel ID="pnlAccessRequestSuccess" runat="server" CssClass="successMessage" Visible="false">Success!</asp:Panel>
                </div>
                <asp:ValidationSummary ID="vsAccessReq" runat="server" ValidationGroup="AccessReq" CssClass="validatorSummary" DisplayMode="BulletList" ClientIDMode="Static" ShowSummary="true" />

                <div id="divSrtsAccessRequestForm" runat="server" style="clear: both; width: 500px; margin: 0px auto 30px auto; padding: 20px 0px">
                    <div style="float: right; width: 300px">

                        <asp:TextBox ID="tbName" runat="server" CssClass="srtsTextBox_medium" MaxLength="80" TabIndex="1" ToolTip="Enter your name." />
                        <asp:RequiredFieldValidator ID="rfvName" runat="server" ControlToValidate="tbName" CssClass="requestValidator" Text="*" ValidationGroup="AccessReq"
                            ErrorMessage="A name is required." />
                        <asp:RegularExpressionValidator ID="revName" runat="server" ControlToValidate="tbName" CssClass="requestValidator" Text="*" ValidationGroup="AccessReq"
                            ErrorMessage="Verify the entered name is correct." ValidationExpression="^[a-zA-Z'\s-]{1,40}$" />
                        <br />
                        <br />
                        <asp:TextBox ID="tbTitle" runat="server" CssClass="srtsTextBox_medium" MaxLength="40" TabIndex="2" ToolTip="Enter your title." />
                        <br />
                        <br />
                        <asp:TextBox ID="tbPhone" runat="server" CssClass="srtsTextBox_medium" TabIndex="3" ToolTip="Enter your phone number." />
                        <asp:RequiredFieldValidator ID="rfvPhone" runat="server" ControlToValidate="tbPhone" CssClass="requestValidator" Text="*" ValidationGroup="AccessReq"
                            ErrorMessage="A phone number is required." />
                        <asp:RegularExpressionValidator ID="revPhone" runat="server" ControlToValidate="tbPhone" CssClass="requestValidator" Text="*" ValidationGroup="AccessReq"
                            ErrorMessage="Verify the entered phone number is correct." ValidationExpression="^[0-9-\-]{7,15}$" />
                        <br />
                        <br />
                        <asp:TextBox ID="tbEmail" runat="server" CssClass="srtsTextBox_medium" TabIndex="4" ToolTip="Enter your email address." />
                        <asp:RequiredFieldValidator ID="rfvEmail1" runat="server" ControlToValidate="tbEmail" CssClass="requestValidator" Text="*" ValidationGroup="AccessReq"
                            ErrorMessage="An email address is required." />
                        <asp:RegularExpressionValidator ID="revEmail" runat="server" ControlToValidate="tbEmail" CssClass="requestValidator" Text="*" ValidationGroup="AccessReq"
                            ErrorMessage="Verify the entered email address is correct." ValidationExpression="^([a-zA-Z][\w\.-]*[a-zA-Z0-9]@[a-zA-Z0-9][\w\.-]*[a-zA-Z0-9]\.[a-zA-Z][a-zA-Z\.]*[a-zA-Z]){1,70}$" />
                        <br />
                        <br />
                        <ajaxToolkit:ListSearchExtender ID="lseSiteCode" runat="server" QueryPattern="Contains" TargetControlID="ddlSiteCode" />
                        <asp:DropDownList ID="ddlSiteCode" runat="server" TabIndex="5" ToolTip="Select your site code." CssClass="srtsDropDown_medium" />
                        <asp:RequiredFieldValidator ID="rfvSiteCode" runat="server" ControlToValidate="ddlSiteCode" CssClass="requestValidator" Text="*" ValidationGroup="AccessReq"
                            ErrorMessage="A site code is required." InitialValue="X" />
                    </div>

                    <div style="float: left; width: 190px;">
                        <div style="margin-bottom: 15px; text-align: right">
                            <asp:Label ID="Label1" runat="server" CssClass="srtsLabel_medium" Text="Requester Name:*" />
                        </div>
                        <div style="margin-bottom: 15px; text-align: right">
                            <asp:Label ID="Label2" runat="server" CssClass="srtsLabel_medium" Text="Requester Title:" />
                        </div>
                        <div style="margin-bottom: 15px; text-align: right">
                            <asp:Label ID="Label3" runat="server" CssClass="srtsLabel_medium" Text="Phone Number:*" />
                        </div>
                        <div style="margin-bottom: 15px; text-align: right">
                            <asp:Label ID="Label4" runat="server" CssClass="srtsLabel_medium" Text="Email Address:*" />
                        </div>
                        <div style="margin-bottom: 15px; text-align: right">
                            <asp:Label ID="Label5" runat="server" CssClass="srtsLabel_medium" Text="Site Code:*" />
                        </div>
                    </div>
                </div>
            </div>
            
            <div id="divcaptcha" runat="server" style="margin-top: 30px !important; margin-bottom: 12px !important;">
                 <table style="width: 100%; margin-top: 30px;">
                    <tr>
                    <td style="width: 370px"></td>
                    <td style="text-align: left;" >
                        <div>
                            <asp:Label ID="CaptchaLabel" runat="server" AssociatedControlID="FacilityAccountRequestCaptchaCode">Retype the characters from the picture:</asp:Label>
                        </div>
                        </td>
                    <td></td>
                    </tr>
                    <tr>
                    <td></td>
                    <td style="align-content: center" >
                        <div>
                            <BotDetect:WebFormsCaptcha ID="FacilityAccountRequestCaptcha" runat="server" UserInputID="FacilityAccountRequestCaptchaCode"/>
                        </div>
                    </td>
                    <td></td>
                    </tr>
                    <tr>
                    <td></td>
                    <td style="text-align: left;">
                        <div>
                         <asp:TextBox ID="FacilityAccountRequestCaptchaCode" runat="server" Width="247px"/>
                         <asp:RequiredFieldValidator ID="rfvCaptchaCode" runat="server" ControlToValidate="FacilityAccountRequestCaptchaCode" ErrorMessage="Retyping the code from the picture is required" Text="*" />
                         <asp:CustomValidator ID="cvCaptcha" runat="server" EnableClientScript="False" ControlToValidate="FacilityAccountRequestCaptchaCode" ErrorMessage="Incorrect CAPTCHA code. Please retype the code from the picture." Text="*" OnServerValidate="FacilityAccountRequestCaptchaValidator_ServerValidate" />
                        </div>
                    </td>
                    <td></td>
                    </tr>
                    </table>
            </div>
           
            <div id="buttons" style="clear: both; text-align: center; margin-bottom: 12px;">
                <asp:Button ID="btnSubmit" runat="server" CssClass="srtsButton" Text="Submit" TabIndex="50"
                    ToolTip="Submit information to NOSTRA" OnClick="btnSubmit_Click" CausesValidation="true" /><br />
                <asp:LinkButton ID="bBackToLogin" runat="server" Text="Return to Login Page" TabIndex="51"
                    ToolTip="Return to Login Page" PostBackUrl="~/WebForms/Account/Login.aspx" CausesValidation="false" />
            </div>
        </div>
    </div>
    <div class="box_full_bottom"></div>
    <div class="contentTitleleft" style="text-align: center; visibility: hidden">
        <h2 style="text-align: center">
            <asp:Literal ID="litModuleTitle" runat="server" /></h2>
        <div style="margin: 0px 0px 15px 20px; padding-left: 10px; border-bottom: 1px solid #E7CFAD; width: 90%"></div>
    </div>

    <asp:ScriptManagerProxy ID="smpFacilityAccountRequest" runat="server">
        <Scripts>
            <asp:ScriptReference Path="~/Scripts/Public/FacilityAccountRequest.js" />
        </Scripts>
    </asp:ScriptManagerProxy>

    <%--    <script type="text/javascript">
        function getlblRemainingID() {
            var lblID = '<%=lblRemaining.ClientID%>';
            return lblID;
        }
        function gettbCommentID() {
            var tbID = '<%=txtComments.ClientID%>';
            return tbID;
        }

        $(function () { }).on('change', $('#<%=this.ddlState.ClientID%>'), function () {
            var s = $('#<%=this.ddlState.ClientID%> option:selected').val();
            if (s == 'UN' || s == '0' || s == 'AA' || s == 'AE' || s == 'AP') return;
            $('#<%=this.ddlCountry.ClientID%>').val('US');
        });
    </script>--%>
</asp:Content>
