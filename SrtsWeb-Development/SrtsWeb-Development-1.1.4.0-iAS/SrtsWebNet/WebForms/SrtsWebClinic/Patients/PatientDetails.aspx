<%@ Page Title="" Language="C#" MasterPageFile="~/SrtsMaster.master" AutoEventWireup="true" CodeBehind="PatientDetails.aspx.cs" Inherits="SrtsWebClinic.Patients.PatientDetails" EnableEventValidation="false" %>

<%@ MasterType VirtualPath="~/srtsMaster.Master" %>
<asp:Content ID="cHeadContent" runat="server" ContentPlaceHolderID="HeadContent">
    <link href="../../Styles/PassFailConfirm.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="PatientInformation" ContentPlaceHolderID="ContentTop_Title_Right" runat="server">
    <asp:Literal ID="litContentTop_Title_Right" runat="server"></asp:Literal>
</asp:Content>
<%--<asp:Content ID="SubMenuContent" runat="server" ContentPlaceHolderID="contentSubMenu">
    <div class="button">
        <ul>
            <li>
                <asp:LinkButton ID="lnkSearchPatient" runat="server" CommandArgument="A" OnCommand="rbPatientSearch_Click"
                    ToolTip="Select to perform a new patient search." Text="Patient Search" />
            </li>
            <li>
                <asp:LinkButton ID="lnkAddPatient" runat="server" OnCommand="rbNewPatient_Click"
                    ToolTip="Select to Add a New Patient." Text="Add New Patient" />
            </li>
            <li>
                <asp:LinkButton ID="lnkOrderCheckin" runat="server" PostBackUrl="~/SrtsWebClinic/Orders/ManageOrders.aspx?id=checkin"
                    ToolTip="" Text="Order Check-In" />
            </li>
            <li>
                <asp:LinkButton ID="lnkDispenseOrder" runat="server" PostBackUrl="~/SrtsWebClinic/Orders/ManageOrders.aspx?id=dispense"
                    ToolTip="" Text="Dispense Order" />
            </li>
        </ul>
    </div>
</asp:Content>--%>
<asp:Content ID="Content6" ContentPlaceHolderID="MainContent" runat="server">
    <asp:ScriptManagerProxy ID="smpProxy" runat="server">
        <Scripts>
            <asp:ScriptReference Path="~/Scripts/PatientDetails/patientdetail.js" />
            <asp:ScriptReference Path="~/Scripts/PatientDetails/patientdetailval.js" />
            <asp:ScriptReference Path="~/Scripts/Global/SrtsCustomValidators.js" />
            <asp:ScriptReference Path="~/Scripts/Global/PassFailConfirm.js" />
            <asp:ScriptReference Path="~/Scripts/Global/GlobalVal.js" />
        </Scripts>
    </asp:ScriptManagerProxy>

    <div class="divSingleColumn" style="margin-top: 20px;">
        <div class="box_fullinner_top"></div>
        <div class="box_fullinner_content">
            <div style="margin: 20px 0px 0px 20px;">
                <asp:LinkButton ID="lnbOrderManagement" runat="server" Text="Order Management" OnClick="lnbOrderManagement_Click" />
            </div>
            <div id="divPersonalInformation" runat="server" style="margin-top: 50px" clientidmode="Static">
                <asp:ValidationSummary ID="detailSummary" runat="server" CssClass="validatorSummary" ValidationGroup="load" />
                <div style="margin: 30px 120px -25px 0px; text-align: right">
                    <asp:Button ID="btnEditPersonalInfo" runat="server" CssClass="srtsButton"
                        Text="Edit" ToolTip="Edit Patient Personal Information" OnClick="btnEditPatientInfo_Click" />
                    <asp:Button ID="btnReturnToSearch" runat="server" CssClass="srtsButton"
                        Text="Cancel" ToolTip="Return to Patient Search" OnClick="btnReturnToSearch_Click" />
                </div>
                <srts:PatientDemographics ID="modPatientInfo" runat="server" />
            </div>
            <div id="divPersonalInfo_edit" class="padding" runat="server" style="margin-top: 30px">
                <br />
                <srts:PatientEdit ID="modPatientEdit" runat="server" />
                <br />
            </div>

            <div id="divContactMessage" style="display: none;"></div>
            <div id="divContactInformation" runat="server" class="padding" clientidmode="Static">
                <div style="padding: 0px 0px 20px 0px">
                    <div id="divContainerBeige" style="margin: 0px 0px 0px 0px; width: 100%; border-top: none;">
                        <div class="padding" style="margin-top: -20px;">
                            <div class="containertop" style="height: auto; padding-bottom: 0px">
                                <div class="containertop_right" style="height: 40px; padding-bottom: 10px"></div>
                                <div class="containertop_left" style="height: 40px; padding-bottom: 10px"></div>
                                <div class="subheader" style="margin-top: 20px">
                                    Contact Information
                                </div>
                            </div>

                            <div id="divIDNumber" runat="server" clientidmode="Static">
                                <div style="border: 1px solid #C6A252; width: auto; height: 45px;">
                                    <div style="float: left">
                                        <b>
                                            <p class="srtsLabel_medium" style="margin-top: 15px;">Identification Numbers</p>
                                        </b>
                                    </div>
                                    <div style="float: right">
                                        <asp:Button ID="btnAddIDNumber" runat="server" CssClass="srtsButton" Text="Add" CommandArgument="id" OnCommand="OpenDialog_Command" CausesValidation="false" />
                                    </div>
                                </div>
                                <asp:GridView ID="gvIDNumbers" runat="server" AutoGenerateColumns="false" GridLines="None"
                                    AllowPaging="false" CssClass="mGrid" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                                    EmptyDataText="<span class='colorGrey'>No identification numbers on file.<br /><br />Click the 'Add' button<br />to add a new identification number.<br /><br /></span>"
                                    CellSpacing="0" Width="100%" EmptyDataRowStyle-CssClass="emptyrow" DataKeyNames="ID"
                                    OnRowCommand="gvIDNumbers_RowCommand"
                                    OnRowDataBound="gvIDNumbers_RowDataBound">
                                    <AlternatingRowStyle CssClass="alt" />
                                    <SortedAscendingHeaderStyle CssClass="sortDesc" />
                                    <PagerSettings Mode="NextPreviousFirstLast" Position="TopAndBottom" FirstPageText="<< First"
                                        NextPageText="Next >" PreviousPageText="< Previous" LastPageText="Last >>" />
                                    <Columns>
                                        <asp:TemplateField HeaderText="Identification Type">
                                            <ItemTemplate>
                                                <asp:Label ID="lblPNTypeDescription" runat="server" Text='<%# Eval("IDNumberTypeDescription") %>'></asp:Label>
                                            </ItemTemplate>
                                            <ItemStyle HorizontalAlign="left" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Identification Number">
                                            <ItemTemplate>
                                                <asp:Label ID="lblIDNumber" runat="server" Text='<%# Eval("IDNumberFilter") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Record Status">
                                            <ItemTemplate>
                                                <asp:Label ID="lblIDNumberStatus" runat="server" Text='<%# Status((Boolean)Eval("IsActive")) %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Last Updated">
                                            <ItemTemplate>
                                                <asp:Label ID="lblLastUpdated" runat="server" Text='<%# Eval("DateLastModified") %>' />
                                            </ItemTemplate>
                                            <ItemStyle Width="150px" />
                                        </asp:TemplateField>
                                    </Columns>
                                    <HeaderStyle HorizontalAlign="Left" />
                                    <PagerStyle CssClass="pgr" />
                                </asp:GridView>
                            </div>

                            <hr style="text-align: center; width: 75%; margin: 5px 0 5px 0;" />

                            <div id="divAddress" runat="server" clientidmode="Static">
                                <div style="text-align: right; border: 1px solid #C6A252; width: auto; height: 45px;">
                                    <div style="float: left">
                                        <b>
                                            <p class="srtsLabel_medium" style="margin-top: 15px;">Addresses</p>
                                        </b>
                                    </div>
                                    <div style="float: right">
                                        <asp:Button ID="btnAddMailAddress" runat="server" CssClass="srtsButton" Text="Add" CommandArgument="address" OnCommand="OpenDialog_Command" CausesValidation="false" />
                                    </div>
                                </div>
                                <asp:GridView ID="gvAddresses" runat="server" AutoGenerateColumns="False" DataKeyNames="ID"
                                    GridLines="None" AllowPaging="false" CssClass="mGrid" PagerStyle-CssClass="pgr"
                                    AlternatingRowStyle-CssClass="alt" EmptyDataText="<span class='colorGrey'>No address on file.<br /><br />Click the 'Add' button<br />to add a new address.<br /><br /></span>"
                                    CellSpacing="0" Width="100%" EmptyDataRowStyle-CssClass="emptyrow"
                                    OnRowCommand="gvAddresses_RowCommand"
                                    OnRowDataBound="gvAddresses_RowDataBound">
                                    <PagerSettings Mode="NextPreviousFirstLast" Position="TopAndBottom" FirstPageText="<< First"
                                        NextPageText="Next >" PreviousPageText="< Previous" LastPageText="Last >>" />
                                    <Columns>
                                        <asp:TemplateField HeaderText="Type">
                                            <ItemTemplate>
                                                <asp:Label ID="lblAddressType" runat="server" Text='<%# Eval("AddressType") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Address">
                                            <ItemTemplate>
                                                <asp:Label ID="lblAddress1" runat="server" Text='<%# Eval("Address1") %>'></asp:Label>
                                                <asp:Label ID="lblAddress2" runat="server" Text='<%# Eval("Address2") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="City,State(Country)">
                                            <ItemTemplate>
                                                <asp:Label ID="lblCity" runat="server" Text='<%# Eval("City") %>'></asp:Label>,&nbsp;<asp:Label ID="lblState" runat="server" Text='<%# Eval("State") %>'></asp:Label>&nbsp;(<asp:Label ID="lblCountry" runat="server" Text='<%# Eval("Country") %>'></asp:Label>)
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Zip Code">
                                            <ItemTemplate>
                                                <asp:Label ID="lblZipCode" runat="server" Text='<%# Eval("ZipCode") %>' Width="100px"></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="UIC">
                                            <ItemTemplate>
                                                <asp:Label ID="lblUIC" runat="server" Text='<%# Eval("UIC") %>' Width="100px"></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Record Status">
                                            <ItemTemplate>
                                                <asp:Label ID="lblAddressStatus" runat="server" Text='<%# Status((Boolean)Eval("IsActive")) %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Last Updated">
                                            <ItemTemplate>
                                                <asp:Label ID="lblLastUpdated" runat="server" Text='<%# Eval("DateLastModified") %>' />
                                            </ItemTemplate>
                                            <ItemStyle Width="150px" />
                                        </asp:TemplateField>
                                    </Columns>
                                    <HeaderStyle HorizontalAlign="Left"></HeaderStyle>
                                </asp:GridView>
                            </div>

                            <hr style="text-align: center; width: 75%; margin: 5px 0 5px 0;" />

                            <div id="divPhone" runat="server" clientidmode="Static">
                                <div style="text-align: right; border: 1px solid #C6A252; width: auto; height: 45px;">
                                    <div style="float: left">
                                        <b>
                                            <p class="srtsLabel_medium" style="margin-top: 15px;">Phone Numbers</p>
                                        </b>
                                    </div>
                                    <div style="float: right">
                                        <asp:Button ID="btnAddPhoneNumber" runat="server" CssClass="srtsButton" Text="Add" CommandArgument="phone" OnCommand="OpenDialog_Command" CausesValidation="false" />
                                    </div>
                                </div>
                                <asp:GridView ID="gvPhones" runat="server" DataKeyNames="ID" AutoGenerateColumns="False"
                                    GridLines="None" AllowPaging="false" CssClass="mGrid" PagerStyle-CssClass="pgr"
                                    AlternatingRowStyle-CssClass="alt" EmptyDataText="<span class='colorGrey'>No phone numbers on file.<br /><br />Click the 'Add' button<br />to add a new phone number.<br /><br /></span>"
                                    CellSpacing="0" Width="100%" EmptyDataRowStyle-CssClass="emptyrow"
                                    OnRowCommand="gvPhones_RowCommand"
                                    OnRowDataBound="gvPhones_RowDataBound">
                                    <PagerSettings Mode="NextPreviousFirstLast" Position="TopAndBottom" FirstPageText="<< First"
                                        NextPageText="Next >" PreviousPageText="< Previous" LastPageText="Last >>" />
                                    <Columns>
                                        <asp:TemplateField HeaderText="Phone Type">

                                            <ItemTemplate>
                                                <asp:Label ID="lblPhoneType" runat="server" Text='<%# Eval("PhoneNumberType") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Phone Number">
                                            <ItemTemplate>
                                                <asp:Label ID="lblPhoneNumber" runat="server" Text='<%# Eval("PhoneNumber") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Extension">
                                            <ItemTemplate>
                                                <asp:Label ID="lblExtension" runat="server" Text='<%# Eval("Extension") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Record Status">
                                            <ItemTemplate>
                                                <asp:Label ID="lblIsActivePN" runat="server" Text='<%# Status((Boolean)Eval("IsActive")) %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Last Updated">
                                            <ItemTemplate>
                                                <asp:Label ID="lblLastUpdated" runat="server" Text='<%# Eval("DateLastModified") %>' />
                                            </ItemTemplate>
                                            <ItemStyle Width="150px" />
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>
                            </div>

                            <hr style="text-align: center; width: 75%; margin: 5px 0 5px 0;" />

                            <div id="divEmail" runat="server" clientidmode="Static">
                                <div style="text-align: right; border: 1px solid #C6A252; width: auto; height: 45px;">
                                    <div style="float: left">
                                        <b>
                                            <p class="srtsLabel_medium" style="margin-top: 15px;">Email Addresses</p>
                                        </b>
                                    </div>
                                    <div style="float: right">
                                        <asp:Button ID="btnAddEmailAddress" runat="server" CssClass="srtsButton" Text="Add" CommandArgument="email" OnCommand="OpenDialog_Command" CausesValidation="false" />
                                    </div>
                                </div>
                                <asp:GridView ID="gvEMail" runat="server" AutoGenerateColumns="False" GridLines="None"
                                    AllowPaging="false" CssClass="mGrid" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                                    EmptyDataText="<span class='colorGrey'>No email address on file.<br /><br />Click the 'Add' button<br />to add a new email address.<br /><br /></span>"
                                    CellSpacing="0" Width="100%" EmptyDataRowStyle-CssClass="emptyrow" DataKeyNames="ID"
                                    OnRowCommand="gvEMail_RowCommand"
                                    OnRowDataBound="gvEMail_RowDataBound">
                                    <PagerSettings Mode="NextPreviousFirstLast" Position="TopAndBottom" FirstPageText="<< First"
                                        NextPageText="Next >" PreviousPageText="< Previous" LastPageText="Last >>" />
                                    <Columns>
                                        <asp:TemplateField HeaderText="Email Type">
                                            <EditItemTemplate>
                                                <asp:Label ID="lblEmailTypeEdit" runat="server" Text='<%# Eval("EmailType") %>'></asp:Label>
                                            </EditItemTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="lblEmailType" runat="server" Text='<%# Eval("EmailType") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Email Address">
                                            <ItemTemplate>
                                                <asp:Label ID="lblEmailAddress" runat="server" Text='<%# Eval("EmailAddress") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Record Status">
                                            <ItemTemplate>
                                                <asp:Label ID="lblIsActiveEmail" runat="server" Text='<%# Status((Boolean)Eval("IsActive")) %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Last Updated">
                                            <ItemTemplate>
                                                <asp:Label ID="lblLastUpdated" runat="server" Text='<%# Eval("DateLastModified") %>' />
                                            </ItemTemplate>
                                            <ItemStyle Width="150px" />
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>
                            </div>


                            <div id="divAddPatientIdNumber" style="display: none;">
                                <div id="divIdNum" style="color: red; width: 90%;"></div>

                                <div style="margin: 10px 10px 30px 10px; display: inline-block;">
                                    <asp:Label ID="lblIDNumber" runat="server" CssClass="srtsLabel_medium" Text="Identification Number:" /><br />
                                    <asp:TextBox ID="tbIDNumber" runat="server" TabIndex="1" CssClass="srtsTextBox_medium" onchange="ValidateIdNum('tbIDNumber')" />
                                </div>

                                <div style="margin: 10px; display: inline-block;">
                                    <asp:Label ID="lblIDNumberType" runat="server" Text="Select an Identification Type:" CssClass="srtsLabel_medium" /><br />
                                    <asp:DropDownList ID="ddlIDNumberType" runat="server" TabIndex="2" DataTextField="Value" DataValueField="Key" onchange="ValidateIdNum('ddlIDNumberType')" />
                                </div>

                                <div style="margin: 10px; display: inline-block;">
                                    <asp:Label ID="lblIDNumberStatus" runat="server" Text="Make this ID default:  " TabIndex="3" CssClass="srtsLabel_medium" />
                                    <asp:CheckBox ID="chkMakeDefaultIDNumber" runat="server" Checked="false" />
                                </div>

                                <div style="float: right;">
                                    <asp:Button ID="btnSaveIDNumber" runat="server" CssClass="srtsButton" CausesValidation="false" Text="Save" OnClientClick="return ValidateIdNum('all')" 
                                        OnCommand="btnSave" CommandArgument="idnumber" TabIndex="4" />
                                </div>
                                <%--<asp:HiddenField ID="hdf" runat="server" />--%>
                            </div>

                            <div id="divAddMailingAddress" style="display: none;">
                                <div id="divMailAddress" style="color: red; width: 90%;"></div>

                                <div style="margin: 10px 10px 30px 10px; display: block;">
                                    <asp:Label ID="lblAddAddress1" runat="server" Text="Address 1:" CssClass="srtsLabel_medium" /><br />
                                    <asp:TextBox ID="tbAddress1" runat="server" MaxLength="100" CssClass="srtsTextBox_medium" onchange="ValidateAddress('tbAddress1')" ToolTip="Enter the patient house and street address." />
                                </div>

                                <div style="margin: 10px 10px 30px 10px; display: block;">
                                    <asp:Label ID="lblAddAddress2" runat="server" Text="Address 2:" CssClass="srtsLabel_medium" /><br />
                                    <asp:TextBox ID="tbAddress2" runat="server" MaxLength="100" CssClass="srtsTextBox_medium" onchange="ValidateAddress('tbAddress2')" ToolTip="Continuation of patient address."></asp:TextBox>
                                </div>

                                <div style="margin: 10px 10px 30px 10px; display: inline-block;">
                                    <asp:Label ID="lblCity" runat="server" Text="City:" CssClass="srtsLabel_medium" /><br />
                                    <asp:TextBox ID="tbCity" runat="server" MaxLength="100" CssClass="srtsTextBox_medium" onchange="ValidateAddress('tbCity')" ToolTip="Enter city name from patient address" />
                                </div>

                                <div style="margin: 10px; display: inline-block;">
                                    <asp:Label ID="lblState" runat="server" Text="State:" CssClass="srtsLabel_medium" /><br />
                                    <asp:DropDownList ID="ddlState" runat="server" ToolTip="Select patient residence state." onchange="ValidateAddress('ddlState')"
                                        DataTextField="Value" DataValueField="Key" Width="265px"></asp:DropDownList>
                                </div>

                                <div style="margin: 10px 10px 30px 10px; display: inline-block;">
                                    <asp:Label ID="lblZip" runat="server" Text="Zip:" CssClass="srtsLabel_medium" /><br />
                                    <asp:TextBox ID="tbZipCode" runat="server" CssClass="srtsTextBox_medium" onchange="ValidateAddress('tbZipCode')" ToolTip="Enter patient residence zip code" />
                                </div>

                                <div style="margin: 10px; display: inline-block;">
                                    <asp:Label ID="lblCountry" runat="server" Text="Country:" CssClass="srtsLabel_medium" /><br />
                                    <asp:DropDownList ID="ddlCountry" runat="server" ToolTip="Select patient residence country." onchange="ValidateAddress('ddlCountry')"
                                         DataTextField="Text" DataValueField="Value" Width="265px"></asp:DropDownList>
                                </div>

                                <div style="margin: 10px 10px 30px 10px; display: inline-block;">
                                    <asp:Label ID="lblUIC" runat="server" Text="UIC:" CssClass="srtsLabel_medium" /><br />
                                    <asp:TextBox ID="tb2UIC" runat="server" CssClass="srtsTextBox_medium"></asp:TextBox>
                                </div>

                                <div style="margin: 10px; display: inline-block;">
                                    <asp:Label ID="lblAddressType" runat="server" Text="Address Type:" CssClass="srtsLabel_medium" /><br />
                                    <asp:DropDownList ID="ddlAddressType" runat="server" ToolTip="Select patient address type." onchange="ValidateAddress('ddlAddressType')"
                                        DataTextField="Value" DataValueField="Key" Width="265px"></asp:DropDownList>
                                </div>

                                <div style="margin: 10px; display: inline-block;">
                                    <asp:Label ID="lblMakeDefaultAddress" runat="server" Text="Make this address default:  " CssClass="srtsLabel_medium" />
                                    <asp:CheckBox ID="chkMakeDefaultAddress" runat="server" Checked="false" />
                                </div>

                                <div style="float: right;">
                                    <asp:Button ID="btnSaveAddress" runat="server" CssClass="srtsButton" CausesValidation="false" Text="Save"
                                        OnClientClick="ValidateAddress('all')" OnCommand="btnSave" CommandArgument="address" />
                                </div>

                                <%--<asp:HiddenField ID="hdfaddress" runat="server" />--%>
                            </div>

                            <div id="divAddPhoneNumber" style="display: none;">
                                <div id="divPhoneNum" style="color: red; width: 90%;"></div>

                                <div style="margin: 10px 10px 30px 10px; display: block;">
                                    <asp:Label ID="lblPhoneNumber" runat="server" CssClass="srtsLabel_medium" Text="Phone Number" /><br />
                                    <asp:TextBox ID="tbPhoneNumber" runat="server" MaxLength="100" ValidationGroup="phone"  onchange="ValidatePhone('')"
                                        ToolTip="Continuation of patient Phone Number." CssClass="srtsTextBox_medium" />
<%--                                    <asp:RequiredFieldValidator ID="rfvPhoneNumber" ControlToValidate="tbPhoneNumber" ValidationGroup="phone" ErrorMessage="Phone Number is a required field" Display="None" runat="server"></asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="revPhoneNumber" ControlToValidate="tbPhoneNumber" runat="server"
                                        ErrorMessage="Invalid Phone Number format, please try again, must be between 7 and 15 numbers long with an optional dash.."
                                        Display="None" ValidationExpression="^[0-9-\-]{7,15}$" ValidationGroup="phone"></asp:RegularExpressionValidator>--%>
                                </div>

                                <div style="margin: 10px 10px 30px 10px; display: block;">
                                    <asp:Label ID="lblExtension" runat="server" CssClass="srtsLabel_medium" Text="Extension" /><br />
                                    <asp:TextBox ID="tbExtension" runat="server" MaxLength="100" ToolTip="Enter patient extension" onchange="ValidatePhone('')" CssClass="srtsTextBox_medium" />
<%--                                    <asp:RegularExpressionValidator ID="revExtension" ControlToValidate="tbExtension" runat="server" 
                                        ErrorMessage="Invalid Extension format, please try again (must be 1 to 7 digits)." Display="None" ValidationExpression="^\d{1,7}$" ValidationGroup="phone"></asp:RegularExpressionValidator>--%>
                                </div>

                                <div style="margin: 10px; display: block;">
                                    <asp:Label ID="lblPhoneType" runat="server" CssClass="srtsLabel_medium" Text="Phone Type" /><br />
                                    <asp:DropDownList ID="ddlPhoneType" runat="server" ToolTip="Select patient phone type." onchange="ValidatePhone('')"
                                        DataTextField="Key" DataValueField="Value" Width="265px"></asp:DropDownList>
<%--                                    <asp:RequiredFieldValidator ID="rfvPhoneType" runat="server" ErrorMessage="Phone Type is a required field" ControlToValidate="ddlPhoneType"
                                        Display="None" ValidationGroup="phone" InitialValue="X"></asp:RequiredFieldValidator>--%>
                                </div>

                                <div style="margin: 10px; display: block;">
                                    <asp:Label ID="lblMakeDefaultPhone" runat="server" Text="Make this phone number default:  " CssClass="srtsLabel_medium" />
                                    <asp:CheckBox ID="chkMakeDefaultPhone" runat="server" Checked="false" />
                                </div>

                                <div style="float: right;">
                                    <asp:Button ID="btnSavePhoneNumber" runat="server" CssClass="srtsButton" Text="Save" OnClientClick="ValidatePhone('all')"
                                        OnCommand="btnSave" CommandArgument="phonenumber" />
                                </div>

                                <%--<asp:HiddenField ID="hdfPhone" runat="server" />--%>
                            </div>

                            <div id="divAddEmailAddress" style="display: none;">
                                <div id="divEmailAddress" style="color: red; width: 90%;"></div>

                                <div style="margin: 10px 10px 30px 10px; display: inline-block;">
                                    <asp:Label ID="lblEmailAddress" runat="server" Text="Email Address" CssClass="srtsLabel_medium" /><br />
                                    <asp:TextBox ID="tbEMailAddress" runat="server" CssClass="srtsTextBox_medium" onchange="ValidateEmail('tbEMailAddress')" ToolTip="Enter the patient eMail address" />
                                </div>

                                <div style="margin: 10px; display: inline-block;">
                                    <asp:Label ID="lblEmailtype" runat="server" Text="Email type" CssClass="srtsLabel_medium" /><br />
                                    <asp:DropDownList ID="ddlEMailType" runat="server" Width="265px" ToolTip="Select patient email type." onchange="ValidateEmail('ddlEMailType')" 
                                        DataTextField="Key" DataValueField="Value"></asp:DropDownList>
                                </div>

                                <div style="margin: 10px; display: inline-block;">
                                    <asp:Label ID="lblMakeDefaultEmail" runat="server" Text="Make this email address default:  " CssClass="srtsLabel_medium" />
                                    <asp:CheckBox ID="chkMakeDefaultEmail" runat="server" Checked="false" />
                                </div>
                                <div style="float: right;">
                                    <asp:Button ID="btnSaveEmailAddress" runat="server" CssClass="srtsButton" CausesValidation="false" Text="Save" OnClientClick="ValidateEmail('all')"
                                        OnCommand="btnSave" CommandArgument="email" />
                                </div>
                                <%--<asp:HiddenField ID="hdfEmail" runat="server" />--%>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="box_fullinner_bottom"></div>
    </div>

    <script type="text/javascript">
        function ShowPersonalContent(e, title) {
            document.getElementById("hdrPatientContactInfo").innerHTML = "Patient Personal Information";
        }
        function ShowContactContent(d, e, title) {
            document.getElementById(d).style.display = "block";
        }
        function ShowOrdersContent(d, e, title) {
            document.getElementById(d).style.display = "block";
        }
        function ShowPrescriptionContent(d, e, title) {
            document.getElementById(d).style.display = "block";
        }
        function AddContactContent(d) {
            document.getElementById(d).style.display = "block";
        }
        function RefreshIDNumbers() {
            document.getElementByID("uplIDNumbers").Update();
        }
        function DoToggle(a, b) {
            var i = $('#' + b);
            var current = i.attr("src");
            var swap = i.attr("data-swap");
            i.attr('src', swap).attr('data-swap', current);
            $('#' + a).toggle();
        }

        $(function () { }).on('change', $('#<%=this.ddlState.ClientID%>'), function () {
            var s = $('#<%=this.ddlState.ClientID%> option:selected').val();
            if (s == 'UN' || s == '0' || s == 'AA' || s == 'AE' || s == 'AP') return;
            $('#<%=this.ddlCountry.ClientID%>').val('US');
        });

    </script>
</asp:Content>
