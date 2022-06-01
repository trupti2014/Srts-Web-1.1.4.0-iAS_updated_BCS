<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ucPatientAdd.ascx.cs"
    Inherits="SrtsWeb.UserControls.ucPatientAdd" %>

<style type="text/css">
    .PatientAddTextBox {
        width: 240px;
        z-index: 210 !important;
        border: 1px solid #E4CFAC;
        margin: 0px 2px 0px 2px;
        padding: 0px;
        height: 20px;
        color: #000000;
    }
</style>

<div class="tabContent">
    <h1 style="font-size: 20px; line-height: 30px; color: #004994; padding-bottom: 10px; text-align: left; border-bottom: solid 1px #E7CFAD">Enter New Patient Information</h1>
    <div style="width: 100%; margin: 5px 0px">

        <span class="colorRed">
            <asp:Literal ID="litMessage" runat="server" Visible="false"></asp:Literal>
        </span>
        <asp:ValidationSummary ID="vsErrors" runat="server" CssClass="validatorSummary" ValidationGroup="allValidators" DisplayMode="BulletList" />
        <asp:ValidationSummary ID="vsAddrErrors" runat="server" CssClass="validatorSummary" ValidationGroup="addrValidators" DisplayMode="BulletList" />
        <br />

        <%--        <div style="margin-bottom: 20px;">
            <asp:Label ID="lblSiteCode" runat="server" Text="*Facility (site)" CssClass="srtsLabel_medium" Width="120px" />
            <asp:DropDownList ID="ddlSite" runat="server" AppendDataBoundItems="true" CssClass="srtsDropDown_medium" Width="400px" DataTextField="SiteCombination" DataValueField="SiteCode" TabIndex="1">
            </asp:DropDownList>
            <ajaxToolkit:ListSearchExtender runat="server" TargetControlID="ddlSite" ID="LSE_ddlSite" Enabled="True" PromptText="Type to search" QueryTimeout="2" PromptCssClass="listSearchPrompt" QueryPattern="Contains"></ajaxToolkit:ListSearchExtender>
            <asp:RequiredFieldValidator ID="rfvSiteCode"
                runat="server"
                ControlToValidate="ddlSite"
                InitialValue="X"
                ErrorMessage="Site Code is a required field"
                Display="None"
                ValidationGroup="allValidators">*</asp:RequiredFieldValidator>
        </div>--%>

        <div style="margin-bottom: 20px;">
            <asp:Label ID="lblIdNumber" runat="server" CssClass="srtsLabel_medium" Text="*ID Type" Width="120px" />
            <asp:DropDownList ID="ddlIDNumberType" runat="server" ToolTip="Select the type of ID patient is using to identify self."
                AppendDataBoundItems="true" DataTextField="Value" DataValueField="Key" TabIndex="2" CssClass="srtsDropDown_medium" Width="245px">
            </asp:DropDownList>
            <asp:RequiredFieldValidator ID="rfvIDNumberType"
                runat="server"
                ControlToValidate="ddlIDNumberType"
                InitialValue="X"
                ErrorMessage="ID Number Type is a required field"
                Display="None"
                ValidationGroup="allValidators">*</asp:RequiredFieldValidator>
        </div>

        <div style="margin-bottom: 20px;">
            <asp:Label ID="blIdNumber" runat="server" CssClass="srtsLabel_medium" Text="*ID Number" Width="120px" />
            <asp:TextBox ID="tbIDNumber" runat="server" CssClass="PatientAddTextBox" ToolTip="Enter patient ID number" AutoPostBack="true" OnTextChanged="tbIDNumber_TextChanged" TabIndex="3" MaxLength="11" />
            <asp:CustomValidator ID="cvIDNumber"
                runat="server"
                ErrorMessage="TBD"
                Text="cvIDNumber"
                ValidateEmptyText="True"
                OnServerValidation="ValidateIDNumber"
                CssClass="requestValidator" Display="none" EnableClientScript="False"></asp:CustomValidator>
            <br />
            <br />
            <asp:Label ID="taboff" Text="Type an ID Number and hit Enter." runat="server" Visible="false" ForeColor="#FF3300"></asp:Label>
        </div>

        <asp:GridView ID="gvSearch" runat="server" ClientIDMode="Static" AutoGenerateColumns="False" GridLines="None" DataKeyNames="ID" Width="930px" CssClass="mGrid" ViewStateMode="Enabled" PageSize="7"
            OnRowDataBound="gvSearch_RowDataBound" EmptyDataText="No Data Found" Visible="False">
            <AlternatingRowStyle CssClass="alt" />
            <Columns>
                <asp:TemplateField HeaderStyle-HorizontalAlign="Left" ItemStyle-Width="130px">
                    <ItemTemplate>
                        <asp:Button ID="btnAddPatientType" runat="server" CssClass="srtsButton" Text="Add" OnCommand="btnAddPatientType_Command" />
                        <asp:Button ID="btnGoToPatient" runat="server" CssClass="srtsButton" Text="View" />
                    </ItemTemplate>

                    <HeaderStyle HorizontalAlign="Left"></HeaderStyle>

                    <ItemStyle Width="70px" HorizontalAlign="Center"></ItemStyle>
                </asp:TemplateField>
                <asp:BoundField DataField="LastName" HeaderText="Last Name" SortExpression="LastName">
                    <HeaderStyle HorizontalAlign="Left" />
                    <ItemStyle Width="70px" />
                </asp:BoundField>
                <asp:BoundField DataField="FirstName" HeaderText="First Name">
                    <HeaderStyle HorizontalAlign="Left" />
                    <ItemStyle Width="70px" />
                </asp:BoundField>
                <asp:BoundField DataField="IDNumberDisplay" HeaderText="Patient ID" SortExpression="IDNumber">
                    <HeaderStyle HorizontalAlign="Left" />
                    <ItemStyle Width="30px" HorizontalAlign="Right" />
                </asp:BoundField>
                <asp:BoundField DataField="IDNumberTypeDescription" HeaderText="ID Type">
                    <HeaderStyle HorizontalAlign="Left" />
                    <ItemStyle Width="130px" />
                </asp:BoundField>
                <asp:BoundField DataField="StatusDescription" HeaderText="Status">
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Left" Width="75px" />
                </asp:BoundField>
                <asp:BoundField DataField="BOSDescription" HeaderText="Branch">
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Left" Width="70px" />
                </asp:BoundField>
                <asp:BoundField DataField="Rank" HeaderText="Grade">
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" Width="25px" />
                </asp:BoundField>
                <asp:BoundField DataField="Gender" HeaderText="Gender">
                    <ItemStyle HorizontalAlign="Center" Width="25px" />
                </asp:BoundField>
                <asp:BoundField DataField="IsNewPatient" HeaderText="Is new patient" Visible="false" />
            </Columns>
        </asp:GridView>

        <asp:RequiredFieldValidator ID="rfvIDNumber" runat="server" ControlToValidate="tbIDNumber" ValidationGroup="allValidators"
            ErrorMessage="ID Number is a required field." Display="None"></asp:RequiredFieldValidator>
    </div>

    <asp:Panel ID="pnlCompleteForm" runat="server" Visible="false">
        <div style="float: right; width: 40%; margin-right: 120px">
            <asp:UpdatePanel ID="upEligibility" runat="server">
                <ContentTemplate>
                    <asp:Label ID="lblBranch" runat="server" Text="*Branch" Width="120px" CssClass="srtsLabel_medium" />
                    <asp:DropDownList ID="ddlBOS" runat="server" AppendDataBoundItems="true" ToolTip="Select the individuals' branch." ClientIDMode="Static"
                        DataTextField="bosText" DataValueField="bosValue" TabIndex="10" CssClass="srtsDropDown_medium" OnSelectedIndexChanged="ddlBOS_SelectedIndexChanged" AutoPostBack="true">
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator ID="rfvBOS"
                        runat="server"
                        ControlToValidate="ddlBOS"
                        InitialValue="X"
                        ErrorMessage="Branch of Service is Required"
                        Display="None"
                        ValidationGroup="allValidators">*</asp:RequiredFieldValidator>
                    <br />
                    <br />
                    <asp:Label ID="lblStatus" runat="server" Text="*Status" Width="120px" CssClass="srtsLabel_medium" />
                    <asp:DropDownList ID="ddlStatusType" runat="server" AppendDataBoundItems="true" DataTextField="statusText"
                        DataValueField="statusValue" TabIndex="11" OnSelectedIndexChanged="ddlStatusType_SelectedIndexChanged1" AutoPostBack="true"
                        CssClass="srtsDropDown_medium">
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator ID="rfvStatusType"
                        runat="server"
                        ControlToValidate="ddlStatusType"
                        InitialValue="X"
                        ErrorMessage="Status Type is a required field"
                        Display="None"
                        ValidationGroup="allValidators">*</asp:RequiredFieldValidator>
                    <br />
                    <br />
                    <asp:Label ID="lblRank" runat="server" Text="*Grade" Width="120px" CssClass="srtsLabel_medium" />
                    <asp:DropDownList ID="ddlRank" runat="server" AppendDataBoundItems="true" ToolTip="Select the individuals' grade."
                        DataTextField="rankText" DataValueField="rankValue" TabIndex="12" CssClass="srtsDropDown_medium">
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator ID="rfvRank"
                        runat="server"
                        ControlToValidate="ddlRank"
                        InitialValue="X"
                        ErrorMessage="Grade is a required field"
                        Display="None"
                        ValidationGroup="allValidators">*</asp:RequiredFieldValidator>
                    <br />
                    <br />
                    <%--<asp:Label ID="lblPriority" runat="server" Text="*Order Priority" Width="120px" CssClass="srtsLabel_medium" Visible="false" />
                    <asp:DropDownList ID="ddlOrderPriority" runat="server" AppendDataBoundItems="true" TabIndex="13" CssClass="srtsDropDown_medium" DataTextField="Value" DataValueField="Key" Width="265px" Visible="false">
                        <asp:ListItem Text="-Select-" Value="X" />
                        <asp:ListItem Text="NONE" Value="N" Selected="True" />
                    </asp:DropDownList>--%>
                </ContentTemplate>
            </asp:UpdatePanel>
            <div style="width: 100%; margin: 20px 0px 20px 0px">
                <asp:Label ID="lblTheater" runat="server" Text="Theater Zip Code" Width="120px"
                    CssClass="srtsLabel_medium" />
                <asp:DropDownList ID="ddlTheaterLocationCodes" runat="server" DataTextField="TheaterCode"
                    DataValueField="TheaterCode" AppendDataBoundItems="true" TabIndex="14" CssClass="srtsDropDown_medium"
                    Width="265px">
                    <asp:ListItem Text="-Select-" Value=""></asp:ListItem>
                </asp:DropDownList>
            </div>
            <asp:Label ID="lblIsActive" runat="server" Text="Is Active" CssClass="srtsLabel_medium" Width="80px" Visible="False" />
            <div style="margin: -23px 0px 0px 100px;">
                <asp:RadioButtonList runat="server" ID="rblIsActive" RepeatDirection="Horizontal" AutoPostBack="true" Visible="False">
                    <asp:ListItem Text="True" Value="True"></asp:ListItem>
                    <asp:ListItem Text="False" Value="False"></asp:ListItem>
                </asp:RadioButtonList>
            </div>
        </div>
        <div style="float: left; width: 40%;">
            <asp:Label ID="lblLastName" runat="server" Text="*Last Name" Width="120px" CssClass="srtsLabel_medium" />
            <asp:TextBox ID="txtLastName" runat="server" MaxLength="75" ToolTip="Enter patient last name." CssClass="PatientAddTextBox" TabIndex="4" />
            <asp:CustomValidator ID="cvLastName" runat="server" ErrorMessage="CustomValidator" EnableClientScript="False" OnServerValidate="ValidateLastName" ValidationGroup="allValidators" Text="*" CssClass="requestValidator" ControlToValidate="txtLastName" ValidateEmptyText="True"></asp:CustomValidator>
            <br />
            <br />
            <asp:Label ID="lblFirstName" runat="server" Text="*First Name" Width="120px" CssClass="srtsLabel_medium" />
            <asp:TextBox ID="tbFirstName" ClientIDMode="Static" runat="server" MaxLength="75"
                ToolTip="Enter patient first name." CssClass="PatientAddTextBox" TabIndex="5" />
            <asp:CustomValidator ID="cvFirstName"
                runat="server"
                ErrorMessage="CustomValidator"
                OnServerValidate="ValidateFirstName"
                ValidationGroup="allValidators"
                Text="*"
                CssClass="requestValidator"
                ControlToValidate="tbFirstName"
                ValidateEmptyText="True" />
            <br />
            <br />
            <asp:Label ID="lblMiddleName" runat="server" Text="Middle Name" Width="120px" CssClass="srtsLabel_medium" />
            <asp:TextBox ID="tbMiddleName" runat="server" MaxLength="75" ToolTip="Enter patient middle name or initial."
                CssClass="PatientAddTextBox" TabIndex="6" />
            <asp:CustomValidator ID="cvMiddleName"
                runat="server"
                ErrorMessage="CustomValidator"
                OnServerValidate="ValidateMiddleName"
                ValidationGroup="allValidators"
                Text="*"
                CssClass="requestValidator"
                ControlToValidate="tbMiddleName"
                ValidateEmptyText="True" />
            <br />
            <br />
            <asp:Label ID="lblDOB" runat="server" Text="Date of Birth(mm/dd/yyyy)" Width="120px" CssClass="srtsLabel_medium" />
            <asp:TextBox ID="tbDOB" runat="server" TabIndex="7" CssClass="srtsDateTextBox_medium" ToolTip="Enter patient DOB (mm/dd/yyyy)" />
            <asp:Image runat="server" ID="calImage1" AlternateText="Calendar" ImageUrl="~/Styles/images/Calendar_scheduleHS.png" />
            <ajaxToolkit:CalendarExtender ID="ceDOB" runat="server"
                TargetControlID="tbDOB" Format="MM/dd/yyyy" PopupButtonID="calImage1">
            </ajaxToolkit:CalendarExtender>
            <asp:CustomValidator ID="cvDOB" runat="server" ErrorMessage="CustomValidator" EnableClientScript="False" OnServerValidate="ValidateDOB" ValidationGroup="allValidators" Text="*" ControlToValidate="tbDOB" ValidateEmptyText="True"></asp:CustomValidator>
            <br />
            <br />
            <asp:Label ID="lblSex" runat="server" Text="Gender" Width="30px" CssClass="srtsLabel_medium" />
            <div style="margin: -23px 0px 0px 120px; width: 200px; height: 25px;">
                <asp:RadioButtonList ID="rblGender" runat="server" TabIndex="8" RepeatDirection="Horizontal" ToolTip="Select patient gender.">
                    <asp:ListItem Text="Male" Value="M" />
                    <asp:ListItem Text="Female" Value="F" />
                </asp:RadioButtonList>
            </div>
            <br />
            <br />
            <asp:Label ID="lblActiveDutyExtend" runat="server" Text="EAD Expiration Date" Width="120px"
                CssClass="srtsLabel_medium" />
            <asp:TextBox ID="tbEADExpires" runat="server" CssClass="srtsDateTextBox_medium" TabIndex="9">
            </asp:TextBox>
            <asp:Image runat="server" ID="calImage" AlternateText="Calendar" ImageUrl="~/Styles/images/Calendar_scheduleHS.png" />
            <ajaxToolkit:CalendarExtender ID="ceEAD" runat="server" TargetControlID="tbEADExpires"
                Format="MM/dd/yyyy" PopupButtonID="calImage">
            </ajaxToolkit:CalendarExtender>
            <asp:CustomValidator ID="cvEad" runat="server" ErrorMessage="CustomValidator" EnableClientScript="False"
                OnServerValidate="ValidateEAD" Text="*" ControlToValidate="tbEADExpires" ValidateEmptyText="True"></asp:CustomValidator>
        </div>
        <%--<div style="width: 100%; height: 120px; padding-top: 10px;">
            <asp:Label ID="lblComments" runat="server" Text="Comments" Width="120px" CssClass="srtsLabel_medium" />
            <asp:TextBox ID="tbComments" runat="server" onKeyDown="return textboxMaxCommentSize(this, 256, event, getlblRemainingID(), gettbCommentID())" ToolTip="Enter any comments, valid characters include + - . , ! ? # ' / ( )"
                Rows="5" TextMode="MultiLine" Width="800px" TabIndex="28" CssClass="PatientAddTextBox"
                Height="70px"></asp:TextBox>
            <asp:CustomValidator ID="cvComment" runat="server" ErrorMessage="Invalid character(s) in Comment" ClientValidationFunction="textboxCommentValidation" OnServerValidate="ValidateCommentFormat" ValidationGroup="allValidators" Text="*" ControlToValidate="tbComments" ValidateEmptyText="True" CssClass="requestValidator"></asp:CustomValidator><br />
            <div style="margin-top: 55px; text-align: center;">
                <asp:Label ID="lblRemaining" runat="server" CssClass="srtsLabel_medium"></asp:Label>
            </div>
        </div>--%>

        <hr style="clear: both; text-align: center; width: 100%; margin-top: 15px; color: #E7CFAD" />
        <div id="divAddMailingAddress" runat="server" style="float: left; width: 45%; padding-bottom: 20px;">
            <h1>Add Address</h1>
            <asp:Label ID="lblAddAddress1" runat="server" Text="Address 1" CssClass="srtsLabel_medium" Width="120px" />
            <asp:TextBox ID="tbAddress1" runat="server" MaxLength="100" CssClass="PatientAddTextBox" ToolTip="Enter the patient house and street address." TabIndex="15"></asp:TextBox>
            <asp:RegularExpressionValidator ID="revAddress1" runat="server" Text="*" ControlToValidate="tbAddress1" ErrorMessage="Invalid characters in Address 1" ValidationExpression="^[a-zA-Z0-9'.\s\-\\/#]{1,40}$" Display="None" ValidationGroup="allValidators"></asp:RegularExpressionValidator>
            <br />
            <br />
            <asp:Label ID="lblAddAddress2" runat="server" Text="Address 2" CssClass="srtsLabel_medium" Width="120px" />
            <asp:TextBox ID="tbAddress2" runat="server" MaxLength="100" CssClass="PatientAddTextBox" ToolTip="Continuation of patient address." TabIndex="16"></asp:TextBox>
            <asp:RegularExpressionValidator ID="revAddress2" Text="*" runat="server" ControlToValidate="tbAddress2" ErrorMessage="Invalid characters in Address 2" ValidationExpression="^[a-zA-Z0-9'.\s\-\\/#]{1,40}$" Display="None" ValidationGroup="allValidators"></asp:RegularExpressionValidator>
            <br />
            <br />
            <asp:Label ID="lblCity" runat="server" Text="City" CssClass="srtsLabel_medium" Width="120px" />
            <asp:TextBox ID="tbCity" runat="server" MaxLength="100" CssClass="PatientAddTextBox" ToolTip="Enter city name from patient address" TabIndex="17" />
            <asp:CustomValidator ID="cvCity" runat="server" ErrorMessage="City is required" OnServerValidate="ValidateCity" ValidationGroup="allValidators" Text="*" ControlToValidate="tbCity" ValidateEmptyText="True" CssClass="requestValidator"></asp:CustomValidator>
            <br />
            <br />
            <asp:Label ID="lblState" runat="server" Text="State" CssClass="srtsLabel_medium" Width="120px" />
            <asp:DropDownList ID="ddlState" runat="server" ToolTip="Select patient residence state." DataTextField="Value" DataValueField="Key" Width="245px"
                ClientIDMode="Predictable" TabIndex="18">
            </asp:DropDownList>
            <asp:CustomValidator ID="cvState" runat="server" ErrorMessage="State is required" OnServerValidate="ValidateState" ValidationGroup="allValidators" Text="*" ControlToValidate="ddlState" ValidateEmptyText="True"></asp:CustomValidator>
            <br />
            <br />
            <asp:Label ID="lblZip" runat="server" Text="Zip" CssClass="srtsLabel_medium" Width="120px" />
            <asp:TextBox ID="tbZipCode" runat="server" CssClass="PatientAddTextBox" ToolTip="Enter patient residence zip code" TabIndex="19" MaxLength="10"></asp:TextBox>
            <asp:CustomValidator ID="cvZipCode" runat="server" ErrorMessage="Zipcode is required" OnServerValidate="ValidateZip" ValidationGroup="allValidators" Text="*" ControlToValidate="tbZipCode" ValidateEmptyText="True" CssClass="requestValidator"></asp:CustomValidator>
            <asp:RegularExpressionValidator ID="revZipCode1" runat="server" Text="*" ControlToValidate="tbZipCode"
                Display="None" ErrorMessage="ZipCode Is Not Formatted Correctly" ValidationGroup="allValidators"
                ValidationExpression="^\d{5}(\-\d{4})?$"></asp:RegularExpressionValidator>
            <br />
            <br />
            <asp:Label ID="lblCountry" runat="server" Text="Country" CssClass="srtsLabel_medium" Width="120px" />
            <asp:DropDownList ID="ddlCountry" runat="server" ToolTip="Select patient residence country." DataTextField="Value" DataValueField="Key" Width="245px" ClientIDMode="Predictable" TabIndex="20">
            </asp:DropDownList>
            <asp:CustomValidator ID="cvCountry" runat="server" ErrorMessage="Country is required" OnServerValidate="ValidateCountry" ValidationGroup="allValidators" Text="*" ControlToValidate="ddlCountry" ValidateEmptyText="True"></asp:CustomValidator>
            <br />
            <br />
            <asp:Label ID="lblUIC" runat="server" Text="UIC" CssClass="srtsLabel_medium" Width="120px" />
            <asp:TextBox ID="tb2UIC" runat="server" CssClass="PatientAddTextBox" TabIndex="21"></asp:TextBox>
            <br />
            <br />
            <asp:Label ID="lblAddressType" runat="server" Text="Address Type" CssClass="srtsLabel_medium" Width="120px" />
            <asp:DropDownList ID="ddlAddressType" runat="server" ToolTip="Select patient address type." DataTextField="Value" DataValueField="Key" Width="245px" TabIndex="22"></asp:DropDownList>
            <asp:CustomValidator ID="cvAddressType" runat="server" ErrorMessage="Address Type is required" OnServerValidate="ValidateAddressType" ValidationGroup="allValidators" Text="*" ControlToValidate="ddlAddressType" ValidateEmptyText="True"></asp:CustomValidator>
        </div>
        <div id="divAddPhoneNumber" runat="server" style="float: right; width: 45%;">
            <h1>Add Phone Number</h1>
            <asp:Label ID="lblPhoneNumber" runat="server" CssClass="srtsLabel_medium" Text="Phone Number" Width="120px" />
            <asp:TextBox ID="tbPhoneNumber" runat="server" MaxLength="100" ValidationGroup="allValidators" ToolTip="Continuation of patient Phone Number." CssClass="PatientAddTextBox" TabIndex="23"></asp:TextBox>
            <asp:RegularExpressionValidator ID="revPhoneNumber" Text="*" ControlToValidate="tbPhoneNumber" runat="server" ErrorMessage="Invalid Phone Number format, please try again, must be between 7 and 15 numbers long with an optional dash." Display="None" ValidationExpression="^[0-9-\-]{7,15}$" ValidationGroup="allValidators"></asp:RegularExpressionValidator>
            <br />
            <br />
            <asp:Label ID="lblExtension" runat="server" CssClass="srtsLabel_medium" Text="Extension" Width="120px" />
            <asp:TextBox ID="tbExtension" runat="server" MaxLength="100" ToolTip="Enter patient extension" CssClass="PatientAddTextBox" TabIndex="24"></asp:TextBox>
            <asp:RegularExpressionValidator ID="revExtension" Text="*" ControlToValidate="tbExtension" runat="server" ErrorMessage="Invalid Extension format, please try again (must be 1 to 7 digits)." Display="None" ValidationExpression="^\d{1,7}$" ValidationGroup="allValidators"></asp:RegularExpressionValidator>
            <br />
            <br />
            <asp:Label ID="lblPhoneType" runat="server" CssClass="srtsLabel_medium" Text="Phone Type" Width="120px" />
            <asp:DropDownList ID="ddlPhoneType" runat="server" ToolTip="Select patient phone type." DataTextField="Key" DataValueField="Value" Width="245px" TabIndex="25"></asp:DropDownList>
        </div>
        <br />
        <br />
        <div id="divAddEmailAddress" runat="server" style="float: right; width: 45%; margin-bottom: 20px;">
            <h1>Add Email Address</h1>
            <asp:Label ID="lblEmailAddress" runat="server" Text="Email Address" CssClass="srtsLabel_medium" Width="120px" />
            <asp:TextBox ID="tbEMailAddress" runat="server" CssClass="PatientAddTextBox" ToolTip="Enter the patient eMail address" TabIndex="26"></asp:TextBox>
            <asp:RegularExpressionValidator ID="revEmail" runat="server" Text="*"
                ErrorMessage="Invalid email format, please try again (example: john.doe@yourdomain.com)."
                Display="none"
                ControlToValidate="tbEMailAddress"
                ValidationExpression="^([a-zA-Z][\w\.-]*[a-zA-Z0-9]@[a-zA-Z0-9][\w\.-]*[a-zA-Z0-9]\.[a-zA-Z][a-zA-Z\.]*[a-zA-Z]){1,70}$"
                ValidationGroup="allValidators"></asp:RegularExpressionValidator>
            <br />
            <br />
            <asp:Label ID="lblEmailtype" runat="server" Text="Email type" CssClass="srtsLabel_medium" Width="120px" />
            <asp:DropDownList ID="ddlEMailType" runat="server" Width="245px" ToolTip="Select patient email type." DataTextField="Key" DataValueField="Value" TabIndex="27"></asp:DropDownList>
            <br />
            <div style="height: 120px; padding-top: 20px;">
                <asp:Label ID="lblComments" runat="server" Text="Comments" Width="120px" CssClass="srtsLabel_medium" /><br />
                <asp:TextBox ID="tbComments" runat="server" onKeyDown="return textboxMaxCommentSize(this, 256, event, getlblRemainingID(), gettbCommentID())" ToolTip="Enter any comments, valid characters include + - . , ! ? # ' / ( )"
                    Rows="5" TextMode="MultiLine" Width="400px" TabIndex="28" CssClass="PatientAddTextBox" Height="70px"></asp:TextBox>
                <asp:CustomValidator ID="cvComment" runat="server" ErrorMessage="Invalid character(s) in Comment" ClientValidationFunction="textboxCommentValidation" OnServerValidate="ValidateCommentFormat" ValidationGroup="allValidators" Text="*" ControlToValidate="tbComments" ValidateEmptyText="True" CssClass="requestValidator"></asp:CustomValidator><br />
                <asp:Label ID="lblRemaining" runat="server" CssClass="srtsLabel_medium"></asp:Label>
            </div>
            <br />
            <br />
            <asp:Button ID="btnAddPatient_btm" runat="server" CssClass="srtsButton" Text="Submit" ToolTip="Add new patient information" OnClick="btnAdd_Click" CausesValidation="False" TabIndex="29" ValidationGroup="allValidators" />
            <asp:Button ID="btnCancel_btm" runat="server" CssClass="srtsButton" Text="Cancel" ToolTip="Cancel current changes and return to patient information" OnClick="btnCancel_Click" CausesValidation="False" TabIndex="30" />
        </div>
    </asp:Panel>
</div>

<script type="text/javascript">
    function getlblRemainingID() {
        var lblID = '<%=lblRemaining.ClientID%>';
        return lblID;
    }
    function gettbCommentID() {
        var tbID = '<%=tbComments.ClientID%>';
        return tbID;
    }

    function idNumberFocus() {
        window.setTimeout(function () {
            $('[id*=tbIDNumber]').focus();
        }, 50);
    }

    function lastNameFocus() {
        window.setTimeout(function () {
            $('[id*=LastName]').focus();
        }, 50);
    }

    $(function () { }).on('change', $('#<%=this.ddlState.ClientID%>'), function () {
        var s = $('#<%=this.ddlState.ClientID%> option:selected').val();
        if (s == 'UN' || s == '0' || s == 'AA' || s == 'AE' || s == 'AP') return;
        $('#<%=this.ddlCountry.ClientID%>').val('US');
    });
</script>
