<%@ Page Title="" Language="C#" MasterPageFile="~/SrtsMaster.master" AutoEventWireup="true" MaintainScrollPositionOnPostback="true"
    CodeBehind="IndividualDetails.aspx.cs" Inherits="SrtsWebClinic.Individuals.IndividualDetails" EnableEventValidation="false" %>

<%@ MasterType VirtualPath="~/srtsMaster.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="../../Styles/PassFailConfirm.css" rel="stylesheet" />
    <script src="../../Scripts/patientdetail.js" type="text/javascript"></script>
    <script src="../../Scripts/Global/SrtsCustomValidators.js"></script>
    <%-- <script src="../../JavaScript/jsValidators.js"></script>--%>
    <script src="../../Scripts/Global/PassFailConfirm.js"></script>
</asp:Content>
<%--<asp:Content ID="PatientInformation" ContentPlaceHolderID="ContentTop_Title_Right" runat="server">
    <asp:Literal ID="litContentTop_Title_Right" runat="server"></asp:Literal>
</asp:Content>--%>
<%--<asp:Content ID="SubMenuContent" runat="server" ContentPlaceHolderID="contentSubMenu">
    <div class="button">
        <ul>
            <li>
                <asp:LinkButton ID="lnkSearchIndividual" runat="server" CommandArgument="A" OnCommand="rbIndividualSearch_Click"
                    ToolTip="Select to perform a new individual search." Text="Individual Search" />
            </li>
            <li>
                <asp:LinkButton ID="lnkAddIndividual" runat="server" OnCommand="rbNewIndividual_Click"
                    ToolTip="Select to Add a New Individual." Text="Add New Individual" />
            </li>
        </ul>
    </div>
</asp:Content>--%>
<asp:Content ID="Content6" ContentPlaceHolderID="MainContent" runat="server">
    <asp:UpdatePanel ID="UpdatePanelMain" runat="server">
        <ContentTemplate>
            <div id="divPersonalInformation" runat="server" style="margin-top: 10px" clientidmode="Static">
                <div style="margin: 30px 120px -25px 0px; text-align: right">
                    <asp:Button ID="btnEditPersonalInfo" runat="server" CssClass="srtsButton" Text="Edit" ToolTip="Edit Individual Personal Information" OnClick="btnEditPatientInfo_Click" />
                    <asp:Button ID="btnReturnToSearch" runat="server" CssClass="srtsButton" Text="Cancel" ToolTip="Return to Individual Search" OnClick="btnReturnToSearch_Click" />
                </div>
                <srts:PatientDemographics ID="modPatientInfo" runat="server" />
            </div>
            <div id="divPersonalInfo_edit" class="padding" runat="server" style="margin-top: 30px" visible="false">
                <br />
                <srts:IndividualEdit ID="modPatientEdit" runat="server" />
                <br />
            </div>

            <div id="statusMessage"></div>

            <div id="divIndType" runat="server" clientidmode="Static" style="width: 720px; margin-left: auto; margin-right: auto; border: 1px solid #EFD3A5; text-align: center; height: 100px">
                <asp:UpdatePanel ID="upnlIndividualType" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <div id="IndTypesCBs" style="text-align: center">
                            <asp:CustomValidator ID="cvIndTypes" runat="server" EnableClientScript="True" ClientValidationFunction="ClientValidateIndTypeCBs" OnServerValidate="ValidateIndTypeCBs" ErrorMessage="Select at least one individual type" Display="Static"></asp:CustomValidator><br />
                            <asp:Label ID="lblIndTypes" runat="server" Text="*Select individual type(s):" Width="175px" CssClass="srtsLabel_medium" ControlToValidate="lblIndTypes" />
                            <asp:CheckBox ID="cbPatient" runat="server" ClientIDMode="Static" Enabled="False" /><asp:Label ID="lblPatient" runat="server" Text="Patient" Style="padding-left: 5px"/>
                            <asp:CheckBox ID="cbProvider" runat="server" ClientIDMode="Static" Style="padding-left: 20px"/><asp:Label ID="lblProvider" runat="server" Text="Provider" Style="padding-left: 5px" />
                            <asp:CheckBox ID="cbTechnician" runat="server" ClientIDMode="Static" Style="padding-left: 20px" /><asp:Label ID="lblTechnician" runat="server" Text="Technician" Style="padding-left: 5px" />
                            <asp:CheckBox ID="cbAdministrator" runat="server" ClientIDMode="Static" Style="padding-left: 20px" /><asp:Label ID="lblOther" runat="server" Text="Other (e.g., Admin, Clerk)" Style="padding-left: 5px" /><br />
                        </div>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="btnUpdateIndTypes" EventName="Click" />
                    </Triggers>
                </asp:UpdatePanel>
                <br />
                <asp:Button ID="btnUpdateIndTypes" runat="server" CssClass="srtsButton" Text="Update" OnClick="btnUpdateIndTypes_Click" />
            </div>

            <div id="divContactInformation" runat="server" class="padding" style="margin-top: -80px" clientidmode="Static">
                <div style="padding: 0px 0px 20px 0px">
                    <div id="divContainerBeige" style="margin: 40px 0px 0px 0px; width: 100%; border-top: none;">
                        <div class="padding" style="margin-top: 0px">
                            <div class="containertop" style="height: auto; padding-bottom: 0px">
                                <div class="containertop_right" style="height: 40px; padding-bottom: 10px"></div>
                                <div class="containertop_left" style="height: 40px; padding-bottom: 10px"></div>
                                <div class="subheader" style="margin-top: 20px">
                                    Contact Information
                                </div>
                            </div>
                            <div id="divIDNumber" runat="server" clientidmode="Static">
                                <%-- <div style="text-align: right; border-left: 1px solid #C6A252; border-right: 1px solid #C6A252; width: auto; height: 40px; margin-top: -35px">
                                                        <asp:Button ID="btnAddIDNumber" runat="server" CssClass="srtsButton" Text="Add" OnCommand="AddPatientInfo_Click" CommandArgument="idnumber" CausesValidation="false" />
                                </div>--%>
                                <div style="border: 1px solid #EFD3A5; width: auto; height: 40px;">
                                    <div style="float: left">
                                        <b>
                                            <p class="srtsLabel_medium" style="margin-top: 10px;">Identification Numbers</p>
                                        </b>
                                    </div>
                                    <div style="float: right">
                                        <asp:Button ID="btnAddIDNumber" runat="server" CssClass="srtsButton" Text="Add" OnCommand="AddPatientInfo_Click" CommandArgument="idnumber" CausesValidation="false" />
                                    </div>
                                </div>
                                <asp:GridView ID="gvIDNumbers" runat="server" AutoGenerateColumns="false" GridLines="None"
                                    AllowPaging="false" CssClass="mGrid" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                                    EmptyDataText="<span class='colorGrey'>Click the 'Add' button to add a new identification number.<br /></span>"
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

                            <%-- <hr style="text-align: center; width: 75%; margin: 5px 0 5px 0;" />--%>

                            <div id="divAddress" runat="server" clientidmode="Static" style="margin-top: 20px;">
                                <%--<div style="border-left: 1px solid #C6A252; border-right: 1px solid #C6A252; width: auto; height: 40px; margin-top: 20px">
                                    <div class="srtsLabel_medium" style="font-weight: bold; float: left; padding: 10px 0px 0px 10px;">Addresses</div>
                                    <div style="float: right">
                                                        <asp:Button ID="btnAddMailAddress" runat="server" CssClass="srtsButton" Text="Add" OnCommand="AddPatientInfo_Click" CommandArgument="address" CausesValidation="false" />
                                                    </div>
                                </div>--%>
                                <div style="text-align: right; border: 1px solid #EFD3A5; width: auto; height: 40px;">
                                    <div style="float: left">
                                        <b>
                                            <p class="srtsLabel_medium" style="margin-top: 10px;">Addresses</p>
                                        </b>
                                    </div>
                                    <div style="float: right">
                                        <asp:Button ID="btnAddMailAddress" runat="server" CssClass="srtsButton" Text="Add"
                                            OnCommand="AddPatientInfo_Click" CommandArgument="address" CausesValidation="false" />
                                    </div>
                                </div>
                                <asp:GridView ID="gvAddresses" runat="server" AutoGenerateColumns="False" DataKeyNames="ID"
                                    GridLines="None" AllowPaging="false" CssClass="mGrid" PagerStyle-CssClass="pgr"
                                    AlternatingRowStyle-CssClass="alt" EmptyDataText="<span class='colorGrey'>Click the 'Add' button to add a new address.<br /></span>"
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

                            <%-- <hr style="text-align: center; width: 75%; margin: 5px 0 5px 0;" />--%>

                            <div id="divPhone" runat="server" clientidmode="Static" style="margin-top: 20px">
                                <%-- <div style="text-align: right; border-left: 1px solid #C6A252; border-right: 1px solid #C6A252; width: auto; height: 40px; margin-top: -35px">
                                                        <asp:Button ID="btnAddPhoneNumber" runat="server" CssClass="srtsButton" Text="Add" OnCommand="AddPatientInfo_Click" CommandArgument="phonenumber" CausesValidation="false" />
                                </div>--%>

                                <div style="text-align: right; border: 1px solid #EFD3A5; width: auto; height: 40px;">
                                    <div style="float: left">
                                        <b>
                                            <p class="srtsLabel_medium" style="margin-top: 10px;">Phone Numbers</p>
                                        </b>
                                    </div>
                                    <div style="float: right">
                                        <asp:Button ID="btnAddPhoneNumber" runat="server" CssClass="srtsButton" Text="Add"
                                            OnCommand="AddPatientInfo_Click" CommandArgument="phonenumber" CausesValidation="false" />
                                    </div>
                                </div>
                                <asp:GridView ID="gvPhones" runat="server" DataKeyNames="ID" AutoGenerateColumns="False"
                                    GridLines="None" AllowPaging="false" CssClass="mGrid" PagerStyle-CssClass="pgr"
                                    AlternatingRowStyle-CssClass="alt" EmptyDataText="<span class='colorGrey'>Click the 'Add' button to add a new phone number.<br /></span>"
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

                            <%--  <hr style="text-align: center; width: 75%; margin: 5px 0 5px 0;" />--%>

                            <div id="divEmail" runat="server" clientidmode="Static" style="margin-top: 20px">
                                <%--<div style="text-align: right; border-left: 1px solid #C6A252; border-right: 1px solid #C6A252; width: auto; height: 40px; margin-top: -35px">
                                                        <asp:Button ID="btnAddEmailAddress" runat="server" CssClass="srtsButton" Text="Add" OnCommand="AddPatientInfo_Click" CommandArgument="email" CausesValidation="false" />
                                </div>--%>
                                <div style="text-align: right; border: 1px solid #EFD3A5; width: auto; height: 40px;">
                                    <div style="float: left">
                                        <b>
                                            <p class="srtsLabel_medium" style="margin-top: 10px;">Email Addresses</p>
                                        </b>
                                    </div>
                                    <div style="float: right">
                                        <asp:Button ID="btnAddEmailAddress" runat="server" CssClass="srtsButton" Text="Add"
                                            OnCommand="AddPatientInfo_Click" CommandArgument="email" CausesValidation="false" />
                                    </div>
                                </div>
                                <asp:GridView ID="gvEMail" runat="server" AutoGenerateColumns="False" GridLines="None"
                                    AllowPaging="false" CssClass="mGrid" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                                    EmptyDataText="<span class='colorGrey'>Click the 'Add' button to add a new email address.<br /></span>"
                                    CellSpacing="0" Width="100%" EmptyDataRowStyle-CssClass="emptyrow" DataKeyNames="ID"
                                    OnRowCommand="gvEMail_RowCommand"
                                    OnRowDataBound="gvEMail_RowDataBound">
                                    <PagerSettings Mode="NextPreviousFirstLast" Position="TopAndBottom" FirstPageText="<< First"
                                        NextPageText="Next >" PreviousPageText="< Previous" LastPageText="Last >>" />
                                    <Columns>
                                        <asp:TemplateField HeaderText="Email Type">
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
                            <%-- <div id="divIndType" runat="server" clientidmode="Static">
                                <div style="text-align: center; border-left: 1px solid #C6A252; border-right: 1px solid #C6A252; width: auto; height: 100px">
                                    <asp:UpdatePanel ID="upnlIndividualType" runat="server" UpdateMode="Conditional">
                                        <ContentTemplate>
                                            <div id="IndTypesCBs" style="text-align: center">
                                                <asp:CustomValidator ID="cvIndTypes" runat="server" EnableClientScript="True" ClientValidationFunction="ClientValidateIndTypeCBs" OnServerValidate="ValidateIndTypeCBs" ErrorMessage="Select at least one individual type" Display="Static"></asp:CustomValidator><br />--%>
                            <%-- <asp:CheckBox ID="cbPatient" runat="server" ClientIDMode="Static" Enabled="False" /><asp:Label ID="lblPatient" runat="server" Text="Patient" /><br />--%>
                            <%-- <asp:Label ID="lblIndTypes" runat="server" Text="*Select individual type(s):" Width="175px" CssClass="srtsLabel_medium" ControlToValidate="lblIndTypes" />
                                                <asp:CheckBox ID="cbProvider" runat="server" ClientIDMode="Static" /><asp:Label ID="lblProvider" runat="server" Text="Provider" Style="padding-left: 5px" />
                                                <asp:CheckBox ID="cbTechnician" runat="server" ClientIDMode="Static" Style="padding-left: 20px" /><asp:Label ID="lblTechnician" runat="server" Text="Technician" Style="padding-left: 5px" />
                                                <asp:CheckBox ID="cbAdministrator" runat="server" ClientIDMode="Static" Style="padding-left: 20px" /><asp:Label ID="lblOther" runat="server" Text="Other (e.g., Admin, Clerk)" Style="padding-left: 5px" /><br />
                                                    </div>
                                        </ContentTemplate>
                                        <Triggers>
                                            <asp:AsyncPostBackTrigger ControlID="btnUpdateIndTypes" EventName="Click" />
                                        </Triggers>
                                    </asp:UpdatePanel>
                                    <br />
                                    <asp:Button ID="btnUpdateIndTypes" runat="server" Text="Update" OnClick="btnUpdateIndTypes_Click" />
                                                </div>
                            </div>--%>
                            <asp:Panel ID="divAddContactInfo" runat="server" CssClass="divdragme" Style="display: none">
                                <div class="box_leftandcenter_top"></div>
                                <div class="box_leftandcenter_content" style="border: none; padding-top: 9px">
                                    <asp:Image ID="imgDrag" runat="server" AlternateText="Click and Drag to Reposition Me!" CssClass="dragme" ImageUrl="~/Styles/images/img_position.png" />
                                    <asp:UpdatePanel ID="uplAddContactInfo" runat="server" ChildrenAsTriggers="true">
                                        <ContentTemplate>
                                            <div class="padding">
                                                <div id="divUpdatePatientInformation" runat="server" clientidmode="Static" visible="false">
                                                    <div id="hdrUpdatePatientInfo" runat="server" clientidmode="static" class="modalHeader">Manage Individuals - Update Individual Information</div>
                                                    <div style="border-top: 1px solid #6BB600; border-bottom: 1px solid #6BB600; width: 668px; padding: 10px 0px; margin-bottom: 30px">
                                                        <div class="tabContent">
                                                            <div class="modalHeader_sub">
                                                                <asp:Literal ID="litActionHeader" runat="server" Text="Add Individual Information"></asp:Literal>
                                                            </div>
                                                            <div id="divAddFunction" runat="server" visible="false">
                                                                <div>
                                                                    <asp:ValidationSummary ID="vsErrors" runat="server" ShowMessageBox="true" ShowSummary="true"
                                                                        DisplayMode="BulletList" />
                                                                </div>
                                                                <div class="padding modalContent_right">
                                                                    <div id="divAddPatientIdNumber" runat="server" visible="false">
                                                                        <asp:Label ID="lblIDNumber" runat="server" CssClass="srtsLabel_medium" Text="Identification Number:" Width="200px" /><br />
                                                                        <asp:TextBox ID="tbIDNumber" runat="server" CssClass="srtsTextBox_medium" MaxLength="100" Width="250px"
                                                                            ToolTip="Enter the individual's Identification Number">
                                                                        </asp:TextBox>
                                                                        <asp:RequiredFieldValidator ID="rfvIdentificationNumbers" ControlToValidate="tbIDNumber"
                                                                            ValidationGroup="idnumb" runat="server" Display="None" ErrorMessage="Identification Number is a required field"></asp:RequiredFieldValidator>
                                                                        <asp:RegularExpressionValidator ID="revIdentificationNumbers" Text="*" ControlToValidate="tbIDNumber"
                                                                            runat="server" ErrorMessage="Invalid ID Number.  ID Numbers can only contain numbers."
                                                                            Display="None" ValidationExpression="^\d+$" ValidationGroup="idnumb"></asp:RegularExpressionValidator>
                                                                        <ajaxToolkit:ValidatorCalloutExtender ID="vceIDNumber" runat="server" TargetControlID="rfvIdentificationNumbers" BehaviorID="idValidator" Enabled="true"></ajaxToolkit:ValidatorCalloutExtender>
                                                                        <ajaxToolkit:ValidatorCalloutExtender ID="vceIDNumberRegEx" runat="server" TargetControlID="revIdentificationNumbers" BehaviorID="idValidator1" Enabled="true"></ajaxToolkit:ValidatorCalloutExtender>
                                                                        <br />
                                                                        <br />
                                                                        <asp:Label ID="lblIDNumberType" runat="server" Text="Select an Identification Type:" CssClass="srtsLabel_medium" Width="205px" /><br />
                                                                        <asp:DropDownList ID="ddlIDNumberType" runat="server" TabIndex="2" DataTextField="Value"
                                                                            DataValueField="Key" ToolTip="Select individual identification type." Width="260px">
                                                                        </asp:DropDownList>
                                                                        <asp:RequiredFieldValidator ID="rfvIDNumberType" runat="server" Display="None" ValidationGroup="idnumb"
                                                                            ControlToValidate="ddlIDNumberType" ErrorMessage="Type is a required field" InitialValue="--please make a selection--"></asp:RequiredFieldValidator>
                                                                        <ajaxToolkit:ValidatorCalloutExtender ID="vceIDNumberType" runat="server" TargetControlID="rfvIDNumberType" Enabled="true" BehaviorID="typeValidator"></ajaxToolkit:ValidatorCalloutExtender>
                                                                        <br />
                                                                        <br />
                                                                        <asp:Label ID="lblIDNumberStatus" runat="server" Text="Make this ID default:  " CssClass="srtsLabel_medium" />
                                                                        <asp:CheckBox ID="chkMakeDefaultIDNumber" runat="server" Checked="false" />
                                                                        <div class="modalButtons">
                                                                            <asp:Button ID="btnSaveIDNumber" runat="server" CssClass="srtsButton" CausesValidation="true" Text="Save"
                                                                                ValidationGroup="idnumb" OnCommand="btnSave" CommandArgument="idnumber" />
                                                                            <asp:Button ID="btnCancelAddIDNumber" runat="server" CssClass="srtsButton" CausesValidation="false" Text="Cancel"
                                                                                ValidationGroup="idnumb" OnCommand="btnCancel" CommandArgument="idnumber" />
                                                                        </div>
                                                                        <asp:HiddenField ID="hdf" runat="server" />
                                                                        <ajaxToolkit:ModalPopupExtender ID="mdlAddContactInfo_ID"
                                                                            runat="server"
                                                                            OkControlID="hdf"
                                                                            TargetControlID="hdf"
                                                                            PopupControlID="divAddContactInfo"
                                                                            CancelControlID="hdf"
                                                                            PopupDragHandleControlID="imgDrag"
                                                                            Drag="true"
                                                                            BackgroundCssClass="modalBackground"
                                                                            BehaviorID="modalwithinputid">
                                                                        </ajaxToolkit:ModalPopupExtender>
                                                                    </div>
                                                                    <div id="divAddMailingAddress" runat="server" visible="false">
                                                                        <asp:Label ID="lblAddAddress1" runat="server" Text="Address 1" CssClass="srtsLabel_medium" /><br />
                                                                        <asp:TextBox ID="tbAddress1" runat="server" MaxLength="100" CssClass="srtsTextBox_medium"
                                                                            ToolTip="Enter the individual house and street address.">
                                                                        </asp:TextBox>
                                                                        <asp:RequiredFieldValidator ID="rfvAddress1" ControlToValidate="tbAddress1" ErrorMessage="Address1 is a required field"
                                                                            Display="None" runat="server" ValidationGroup="addr"></asp:RequiredFieldValidator>
                                                                        <asp:RegularExpressionValidator ID="revAddress1" runat="server" ControlToValidate="tbAddress1"
                                                                            ErrorMessage="Invalid characters in Address 1" ValidationExpression="^[a-zA-Z0-9'.\s-\/]{1,40}$"
                                                                            Display="None" ValidationGroup="addr"></asp:RegularExpressionValidator>
                                                                        <ajaxToolkit:ValidatorCalloutExtender ID="vceAddress1" runat="server" TargetControlID="rfvAddress1" Enabled="true" BehaviorID="addr1Validator"></ajaxToolkit:ValidatorCalloutExtender>
                                                                        <ajaxToolkit:ValidatorCalloutExtender ID="vceImpAddr1" runat="server" TargetControlID="revAddress1" Enabled="true" BehaviorID="addr1ImpValidator"></ajaxToolkit:ValidatorCalloutExtender>
                                                                        <br />
                                                                        <br />
                                                                        <asp:Label ID="lblAddAddress2" runat="server" Text="Address 2" CssClass="srtsLabel_medium" /><br />
                                                                        <asp:TextBox ID="tbAddress2" runat="server" MaxLength="100" CssClass="srtsTextBox_medium"
                                                                            ToolTip="Continuation of individual address.">
                                                                        </asp:TextBox>
                                                                        <asp:RegularExpressionValidator ID="revAddress2" runat="server" ControlToValidate="tbAddress2"
                                                                            ErrorMessage="Invalid characters in Address 2" ValidationExpression="^[a-zA-Z0-9'.\s-\/]{1,40}$"
                                                                            Display="None" ValidationGroup="addr"></asp:RegularExpressionValidator>
                                                                        <ajaxToolkit:ValidatorCalloutExtender ID="vceImpAddr2" runat="server" TargetControlID="revAddress2" Enabled="true" BehaviorID="addr2ImpValidator"></ajaxToolkit:ValidatorCalloutExtender>
                                                                        <br />
                                                                        <br />
                                                                        <asp:Label ID="lblCity" runat="server" Text="City" CssClass="srtsLabel_medium" /><br />
                                                                        <asp:TextBox ID="tbCity" runat="server" MaxLength="100" CssClass="srtsTextBox_medium" ToolTip="Enter city name from individual address" />
                                                                        <asp:RequiredFieldValidator ID="rfvCity" runat="server" ErrorMessage="City is a required field"
                                                                            ControlToValidate="tbCity" Display="None" ValidationGroup="addr"></asp:RequiredFieldValidator>
                                                                        <ajaxToolkit:ValidatorCalloutExtender ID="vceCity" runat="server" TargetControlID="rfvCity" Enabled="true" BehaviorID="cityValidator"></ajaxToolkit:ValidatorCalloutExtender>
                                                                        <br />
                                                                        <br />
                                                                        <asp:Label ID="lblState" runat="server" Text="State" CssClass="srtsLabel_medium" /><br />
                                                                        <asp:DropDownList ID="ddlState" runat="server" ToolTip="Select individual residence state."
                                                                            DataTextField="Value" DataValueField="Key" Width="265px">
                                                                        </asp:DropDownList>
                                                                        <asp:RequiredFieldValidator ID="rfvState" runat="server" ErrorMessage="Please select a state" ControlToValidate="ddlState"
                                                                            Display="None" ValidationGroup="addr" InitialValue="--please make a selection--"></asp:RequiredFieldValidator>
                                                                        <ajaxToolkit:ValidatorCalloutExtender ID="vceState" runat="server" TargetControlID="rfvState" Enabled="true" BehaviorID="stateValidator"></ajaxToolkit:ValidatorCalloutExtender>
                                                                        <br />
                                                                        <br />
                                                                        <asp:Label ID="lblZip" runat="server" Text="Zip" CssClass="srtsLabel_medium" /><br />
                                                                        <asp:TextBox ID="tbZipCode" runat="server" CssClass="srtsTextBox_medium" ToolTip="Enter individual residence zip code">
                                                                        </asp:TextBox>
                                                                        <asp:RequiredFieldValidator ID="rfvZipCode" runat="server" Display="None" ErrorMessage="ZipCode is a required field"
                                                                            ControlToValidate="tbZipCode" ValidationGroup="addr"></asp:RequiredFieldValidator>
                                                                        <asp:RegularExpressionValidator ID="revZipCode" runat="server" ControlToValidate="tbZipCode"
                                                                            Display="None" ErrorMessage="ZipCode Is Not Formatted Correctly" ValidationGroup="addr"
                                                                            ValidationExpression="^\d{5}(\-\d{4})?$"></asp:RegularExpressionValidator>
                                                                        <ajaxToolkit:ValidatorCalloutExtender ID="vceZip" runat="server" TargetControlID="rfvZipCode" Enabled="true" BehaviorID="zipValidator"></ajaxToolkit:ValidatorCalloutExtender>
                                                                        <ajaxToolkit:ValidatorCalloutExtender ID="vceZipRegEx" runat="server" TargetControlID="revZipCode" Enabled="true" BehaviorID="zipValidator1"></ajaxToolkit:ValidatorCalloutExtender>

                                                                        <br />
                                                                        <br />
                                                                        <asp:Label ID="lblCountry" runat="server" Text="Country" CssClass="srtsLabel_medium" /><br />
                                                                        <asp:DropDownList ID="ddlCountry" runat="server" ToolTip="Select individual residence country."
                                                                            DataTextField="Value" DataValueField="Key" Width="265px">
                                                                        </asp:DropDownList>
                                                                        <asp:RequiredFieldValidator ID="rfvCountry" runat="server" ErrorMessage="Please select a country" ControlToValidate="ddlCountry"
                                                                            Display="None" ValidationGroup="addr" InitialValue="--please make a selection--"></asp:RequiredFieldValidator>
                                                                        <ajaxToolkit:ValidatorCalloutExtender ID="vcsCountry" runat="server" TargetControlID="rfvCountry" Enabled="true" BehaviorID="countryValidator"></ajaxToolkit:ValidatorCalloutExtender>
                                                                        <br />
                                                                        <br />
                                                                        <asp:Label ID="lblUIC" runat="server" Text="UIC" CssClass="srtsLabel_medium" /><br />
                                                                        <asp:TextBox ID="tb2UIC" runat="server"></asp:TextBox>
                                                                        <br />
                                                                        <br />
                                                                        <asp:Label ID="lblAddressType" runat="server" Text="Address Type" CssClass="srtsLabel_medium" /><br />
                                                                        <asp:DropDownList ID="ddlAddressType" runat="server" ToolTip="Select individual address type."
                                                                            DataTextField="Value" DataValueField="Key" Width="265px">
                                                                        </asp:DropDownList>
                                                                        <asp:RequiredFieldValidator ID="rfvAddrType" runat="server" ErrorMessage="Please select a type" ControlToValidate="ddlAddressType"
                                                                            Display="None" ValidationGroup="addr" InitialValue="--please make a selection--"></asp:RequiredFieldValidator>
                                                                        <ajaxToolkit:ValidatorCalloutExtender ID="vceAddrType" runat="server" TargetControlID="rfvAddrType" Enabled="true" BehaviorID="addrTypeValidator"></ajaxToolkit:ValidatorCalloutExtender>
                                                                        <br />
                                                                        <br />
                                                                        <asp:Label ID="lblMakeDefaultAddress" runat="server" Text="Make this address default:  " CssClass="srtsLabel_medium" />
                                                                        <asp:CheckBox ID="chkMakeDefaultAddress" runat="server" Checked="false" />
                                                                        <div class="modalButtons">
                                                                            <asp:Button ID="btnSaveAddress" runat="server" CssClass="srtsButton" CausesValidation="true" Text="Save"
                                                                                ValidationGroup="addr" OnCommand="btnSave" CommandArgument="address" />
                                                                            <asp:Button ID="btnCancelAddress" runat="server" CssClass="srtsButton" CausesValidation="false" Text="Cancel"
                                                                                ValidationGroup="addr" OnCommand="btnCancel" CommandArgument="address" />
                                                                        </div>
                                                                        <asp:HiddenField ID="hdfaddress" runat="server" />
                                                                        <ajaxToolkit:ModalPopupExtender
                                                                            ID="mdlAddMailingAddress"
                                                                            runat="server"
                                                                            OkControlID="hdfaddress"
                                                                            TargetControlID="hdfaddress"
                                                                            PopupControlID="divAddContactInfo"
                                                                            CancelControlID="hdfaddress"
                                                                            PopupDragHandleControlID="imgDrag"
                                                                            Drag="true"
                                                                            BackgroundCssClass="modalBackground"
                                                                            OnCancelScript="ClearUIAddr()"
                                                                            BehaviorID="modalwithinput1">
                                                                        </ajaxToolkit:ModalPopupExtender>
                                                                    </div>
                                                                    <div id="divAddPhoneNumber" runat="server" clientidmode="Static" visible="false">
                                                                        <asp:Label ID="lblPhoneNumber" runat="server" CssClass="srtsLabel_medium" Text="Phone Number" /><br />
                                                                        <asp:TextBox ID="tbPhoneNumber" runat="server" MaxLength="100" Width="250px"
                                                                            ValidationGroup="phone" ToolTip="Continuation of individual Phone Number." CssClass="srtsTextBox_medium">
                                                                        </asp:TextBox>
                                                                        <asp:RequiredFieldValidator ID="rfvPhoneNumber" ControlToValidate="tbPhoneNumber"
                                                                            ValidationGroup="phone" ErrorMessage="Phone Number is a required field" Display="None"
                                                                            runat="server"></asp:RequiredFieldValidator>
                                                                        <asp:RegularExpressionValidator ID="revPhoneNumber" Text="*" ControlToValidate="tbPhoneNumber"
                                                                            runat="server" ErrorMessage="Invalid Phone Number format, please try again, must be between 7 and 15 numbers long with an optional dash."
                                                                            Display="None" ValidationExpression="^[0-9-\-]{7,15}$" ValidationGroup="phone"></asp:RegularExpressionValidator>
                                                                        <ajaxToolkit:ValidatorCalloutExtender ID="vcePhone" runat="server" TargetControlID="rfvPhoneNumber" Enabled="true" BehaviorID="phoneValidator"></ajaxToolkit:ValidatorCalloutExtender>
                                                                        <ajaxToolkit:ValidatorCalloutExtender ID="vcePhoneRegEx" runat="server" TargetControlID="revPhoneNumber" Enabled="true" BehaviorID="phoneValidator1"></ajaxToolkit:ValidatorCalloutExtender>
                                                                        <br />
                                                                        <br />
                                                                        <asp:Label ID="lblExtension" runat="server" CssClass="srtsLabel_medium" Text="Extension" /><br />
                                                                        <asp:TextBox ID="tbExtension" runat="server" MaxLength="100" ToolTip="Enter individual extension" CssClass="srtsTextBox_medium">
                                                                        </asp:TextBox>
                                                                        <br />
                                                                        <br />
                                                                        <asp:Label ID="lblPhoneType" runat="server" CssClass="srtsLabel_medium" Text="Phone Type" /><br />
                                                                        <asp:DropDownList ID="ddlPhoneType" runat="server" ToolTip="Select individual phone type."
                                                                            DataTextField="Key" DataValueField="Value" Width="265px">
                                                                        </asp:DropDownList>
                                                                        <asp:RequiredFieldValidator ID="rfvPhoneType" runat="server" ErrorMessage="Please select a type" ControlToValidate="ddlPhoneType"
                                                                            Display="None" ValidationGroup="phone" InitialValue="--please make a selection--"></asp:RequiredFieldValidator>
                                                                        <ajaxToolkit:ValidatorCalloutExtender ID="vcePhoneType" runat="server" TargetControlID="rfvPhoneType" Enabled="true" BehaviorID="phoneTypeValidator"></ajaxToolkit:ValidatorCalloutExtender>
                                                                        <br />
                                                                        <br />
                                                                        <asp:Label ID="lblMakeDefaultPhone" runat="server" Text="Make this phone number default:  " CssClass="srtsLabel_medium" />
                                                                        <asp:CheckBox ID="chkMakeDefaultPhone" runat="server" Checked="false" />
                                                                        <div class="modalButtons">
                                                                            <asp:Button ID="btnSavePhoneNumber" runat="server" CssClass="srtsButton" Text="Save"
                                                                                CausesValidation="true" ValidationGroup="phone" OnCommand="btnSave" CommandArgument="phonenumber" />
                                                                            <asp:Button ID="btnCancelAddPhone" runat="server" CssClass="srtsButton" Text="Cancel"
                                                                                CausesValidation="false" ValidationGroup="phone" OnCommand="btnCancel" CommandArgument="phonenumber" />
                                                                        </div>
                                                                        <asp:HiddenField ID="hdfPhone" runat="server" />
                                                                        <ajaxToolkit:ModalPopupExtender ID="mdlAddPhoneNumber" runat="server"
                                                                            OkControlID="hdfPhone" TargetControlID="hdfPhone"
                                                                            PopupControlID="divAddContactInfo" CancelControlID="hdfPhone"
                                                                            PopupDragHandleControlID="imgDrag" Drag="true"
                                                                            BackgroundCssClass="modalBackground" OnCancelScript="ClearUIPhone()" BehaviorID="modalwithinput2">
                                                                        </ajaxToolkit:ModalPopupExtender>
                                                                    </div>
                                                                    <div id="divAddEmailAddress" clientidmode="Static" runat="server" visible="false">
                                                                        <asp:Label ID="lblEmailAddress" runat="server" Text="Email Address" CssClass="srtsLabel_medium" /><br />
                                                                        <asp:TextBox ID="tbEMailAddress" runat="server" CssClass="srtsTextBox_medium" ToolTip="Enter the individual eMail address">
                                                                        </asp:TextBox>
                                                                        <asp:RequiredFieldValidator ID="rfvEmailAddress"
                                                                            runat="server"
                                                                            ControlToValidate="tbEMailAddress"
                                                                            ValidationGroup="email"
                                                                            Display="None"
                                                                            ErrorMessage="Email Address is a required field"></asp:RequiredFieldValidator>
                                                                        <asp:RegularExpressionValidator ID="revEmailAddress"
                                                                            runat="server"
                                                                            ErrorMessage="Email Address format is not correct"
                                                                            ValidationGroup="email"
                                                                            Display="None"
                                                                            ControlToValidate="tbEmailAddress"
                                                                            ValidationExpression="^([a-zA-Z][\w\.-]*[a-zA-Z0-9]@[a-zA-Z0-9][\w\.-]*[a-zA-Z0-9]\.[a-zA-Z][a-zA-Z\.]*[a-zA-Z]){1,70}$">
                                                                        </asp:RegularExpressionValidator>
                                                                        <ajaxToolkit:ValidatorCalloutExtender ID="vceEmail" runat="server" TargetControlID="rfvEmailAddress" Enabled="true" BehaviorID="emailValidator"></ajaxToolkit:ValidatorCalloutExtender>
                                                                        <br />
                                                                        <br />
                                                                        <asp:Label ID="lblEmailtype" runat="server" Text="Email type" CssClass="srtsLabel_medium" /><br />
                                                                        <asp:DropDownList ID="ddlEMailType" runat="server" Width="265px" ToolTip="Select individual email type."
                                                                            DataTextField="Key" DataValueField="Value">
                                                                        </asp:DropDownList>
                                                                        <asp:RequiredFieldValidator ID="rfvEmailType" runat="server" ControlToValidate="ddlEmailType" InitialValue="--please make a selection--"
                                                                            ValidationGroup="email" Display="None" ErrorMessage="Email Address Type is a required field"></asp:RequiredFieldValidator>
                                                                        <ajaxToolkit:ValidatorCalloutExtender ID="vceEmailType" runat="server" TargetControlID="rfvEmailType" Enabled="true" BehaviorID="emailTypeValidator"></ajaxToolkit:ValidatorCalloutExtender>
                                                                        <br />
                                                                        <br />
                                                                        <asp:Label ID="lblMakeDefaultEmail" runat="server" Text="Make this email default:  " CssClass="srtsLabel_medium" />
                                                                        <asp:CheckBox ID="chkMakeDefaultEmail" runat="server" Checked="false" />
                                                                        <div class="modalButtons">
                                                                            <asp:Button ID="btnSaveEmailAddress" runat="server" CssClass="srtsButton" Text="Save"
                                                                                CausesValidation="true" ValidationGroup="email" OnCommand="btnSave" CommandArgument="email" />
                                                                            <asp:Button ID="btnCancelAddEmail" runat="server" CssClass="srtsButton" Text="Cancel"
                                                                                CausesValidation="false" ValidationGroup="email" OnCommand="btnCancel" CommandArgument="email" />
                                                                        </div>
                                                                        <asp:HiddenField ID="hdfEmail" runat="server" />
                                                                        <ajaxToolkit:ModalPopupExtender
                                                                            ID="mdlAddEmailAddress"
                                                                            runat="server"
                                                                            OkControlID="hdfEmail"
                                                                            TargetControlID="hdfEmail"
                                                                            PopupControlID="divAddContactInfo"
                                                                            CancelControlID="hdfEmail"
                                                                            PopupDragHandleControlID="imgDrag"
                                                                            Drag="true"
                                                                            BackgroundCssClass="modalBackground"
                                                                            OnCancelScript="ClearUIEmail()"
                                                                            BehaviorID="modalwithinput3">
                                                                        </ajaxToolkit:ModalPopupExtender>
                                                                    </div>
                                                                    <div id="divAddIndType" clientidmode="Static" runat="server" visible="false">
                                                                        <asp:Label ID="lblIndType" runat="server" Text="Individual type" CssClass="srtsLabel_medium" /><br />
                                                                        <asp:DropDownList ID="ddlIndType" runat="server" Width="265px" ToolTip="Select individual type."
                                                                            DataTextField="Key" DataValueField="Value">
                                                                        </asp:DropDownList>
                                                                        <asp:RequiredFieldValidator ID="rfvIndType" runat="server" ControlToValidate="ddlIndType" InitialValue="-Select-"
                                                                            ValidationGroup="indType" Display="None" ErrorMessage="Individual Type is a required field"></asp:RequiredFieldValidator>
                                                                        <ajaxToolkit:ValidatorCalloutExtender ID="vceIndType" runat="server" TargetControlID="rfvIndType" Enabled="true" BehaviorID="indTypeValidator"></ajaxToolkit:ValidatorCalloutExtender>
                                                                        <br />
                                                                        <br />
                                                                        <asp:Label ID="lblMakeDefaultIndType" runat="server" Text="Individual type is active:  " CssClass="srtsLabel_medium" />
                                                                        <asp:CheckBox ID="chkMakeDefaultIndType" runat="server" Checked="false" />
                                                                        <div class="modalButtons">
                                                                            <asp:Button ID="btnSaveIndType" runat="server" CssClass="srtsButton" Text="Save"
                                                                                CausesValidation="true" ValidationGroup="indType" OnCommand="btnSave" CommandArgument="individualtype" />
                                                                            <asp:Button ID="btnCancelAddIndType" runat="server" CssClass="srtsButton" Text="Cancel"
                                                                                CausesValidation="false" ValidationGroup="indType" OnCommand="btnCancel" CommandArgument="individualtype" />
                                                                        </div>
                                                                        <asp:HiddenField ID="hdfIndType" runat="server" />
                                                                        <ajaxToolkit:ModalPopupExtender
                                                                            ID="mdlAddIndType"
                                                                            runat="server"
                                                                            OkControlID="hdfIndType"
                                                                            TargetControlID="hdfIndType"
                                                                            PopupControlID="divAddContactInfo"
                                                                            CancelControlID="hdfIndType"
                                                                            PopupDragHandleControlID="imgDrag"
                                                                            Drag="true"
                                                                            BackgroundCssClass="modalBackground"
                                                                            OnCancelScript="ClearUIIndType()"
                                                                            BehaviorID="modalwithinput4">
                                                                        </ajaxToolkit:ModalPopupExtender>
                                                                    </div>
                                                                </div>
                                                                <div class="padding modalContent_left">
                                                                    <div class="colorBlue" style="text-align: left; font-size: .8em;">
                                                                        <asp:Literal ID="litActionInstruction" runat="server"></asp:Literal>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                            <div id="divConfirmation" runat="server" visible="false">
                                                                <div class="confirmtext">
                                                                    <asp:Literal ID="litConfirmationText" runat="server" />
                                                                </div>
                                                                <asp:HiddenField ID="hdfConfirm" runat="server" />
                                                                <ajaxToolkit:ModalPopupExtender
                                                                    ID="mdlAddContactConfirm"
                                                                    runat="server"
                                                                    OkControlID="hdfConfirm"
                                                                    TargetControlID="hdfConfirm"
                                                                    PopupControlID="divAddContactInfo"
                                                                    CancelControlID="hdfConfirm"
                                                                    PopupDragHandleControlID="imgDrag"
                                                                    Drag="true"
                                                                    BackgroundCssClass="modalBackground">
                                                                </ajaxToolkit:ModalPopupExtender>
                                                                <div id="divConfirmButtons" runat="server" class="modalButtons">
                                                                    <asp:Button ID="btnOK" runat="server" CssClass="srtsButton" OnCommand="Confirmation_Click" CommandArgument="OK" Text="Ok" Visible="false" />
                                                                    <asp:Button ID="btnYes" runat="server" CssClass="srtsButton" OnCommand="Confirmation_Click" CommandArgument="Yes" Text="Yes" />
                                                                    <asp:Button ID="btnNo" runat="server" CssClass="srtsButton" OnCommand="btnCancel" Text="No" />
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </div>
                                <div class="box_leftandcenter_bottom"></div>
                            </asp:Panel>
                            <asp:Button ID="btnShowPopup" runat="server" CssClass="srtsButton" Style="display: none" />
                            <asp:Button ID="btnCancel2" runat="server" CssClass="srtsButton" Text="Cancel" Style="display: none" />
                            <ajaxToolkit:ModalPopupExtender
                                ID="mdlEditWindow"
                                runat="server"
                                TargetControlID="btnShowPopup"
                                PopupControlID="divAddContactInfo"
                                CancelControlID="btnCancel2"
                                BackgroundCssClass="modalBackground"
                                PopupDragHandleControlID="imgDrag"
                                Drag="true">
                            </ajaxToolkit:ModalPopupExtender>
                            <%--<div style="margin: -5px 0px 20px 0px; width: auto; height: 30px; border: 1px solid #C6A252; border-top: none; padding-bottom: 10px"></div>--%>
                        </div>
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
    <script type="text/javascript">
        $(function () { }).on('change', $('#<%=this.ddlState.ClientID%>'), function () {
            var s = $('#<%=this.ddlState.ClientID%> option:selected').val();
            if (s == 'UN' || s == '0' || s == 'AA' || s == 'AE' || s == 'AP') return;
            $('#<%=this.ddlCountry.ClientID%>').val('US');
        });
    </script>
</asp:Content>
