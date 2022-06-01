<%@ Page Title="" Language="C#" MasterPageFile="~/srtsMaster.Master" AutoEventWireup="true" CodeBehind="AddPerson.aspx.cs" Inherits="SrtsWeb.SrtsPerson.AddPerson" %>

<%@ MasterType VirtualPath="~/srtsMaster.Master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link rel="stylesheet" type="text/css" href="../../Styles/w3.css" />
    <link rel="stylesheet" type="text/css" href="../../Styles/PassFailConfirm.css" />
    <style type="text/css">
        .PersonAddTextBox {
            width: 240px;
            z-index: 210 !important;
            border: 1px solid #E4CFAC;
            margin: 0px 2px 0px 2px;
            padding: 0px;
            height: 20px;
            color: #000000;
        }

        .redBorder {
            border: 1px solid red;
        }
    </style>
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="MainContent" runat="server">
    <asp:ScriptManagerProxy ID="smpProxy" runat="server">
        <Scripts>
            <asp:ScriptReference Path="~/Scripts/Global/PassFailConfirm.js" />
            <asp:ScriptReference Path="~/Scripts/Global/SrtsCustomValidators.js" />
            <asp:ScriptReference Path="~/Scripts/Person/AddPerson.js" />
            <asp:ScriptReference Path="~/Scripts/Person/AddPersonVal.js" />
            <asp:ScriptReference Path="~/Scripts/Global/GlobalVal.js" />
        </Scripts>
    </asp:ScriptManagerProxy>

    <div id="divAddPerson">
        <div id="divAddPersonMsg" class="colorRed"></div>
        <!-- Add Patient Ids -->
        <div class="w3-row">
            <div style="text-align: left; width: 100%; margin: 5px 0px;">

                <div style="margin-bottom: 10px;">
                    <asp:ValidationSummary ID="vsErrors" runat="server" CssClass="validatorSummary" ValidationGroup="allValidators" DisplayMode="BulletList" />
                </div>

                <!-- Select Facility Site -->
                <div id="divSiteCodes" runat="server" style="float: left; margin: 0px 0px 20px 30px;" visible="false">
                    <asp:Label ID="lblSiteCode" runat="server" Text="*Facility (site)" Width="120px" CssClass="srtsLabel_medium" />
                    <asp:DropDownList ID="ddlSite" runat="server" AppendDataBoundItems="true" Width="400px" DataTextField="SiteCombination" DataValueField="SiteCode" TabIndex="1" CssClass="srtsDropDown_medium">
                        <Items>
                            <asp:ListItem Text="-select-" Value="X" />
                        </Items>
                    </asp:DropDownList>
                    <ajaxToolkit:ListSearchExtender runat="server" TargetControlID="ddlSite" ID="LSE_ddlSite" Enabled="True" PromptText="Type to search" QueryTimeout="2" PromptCssClass="listSearchPromptOverlay" QueryPattern="Contains"></ajaxToolkit:ListSearchExtender>
                </div>
                <br />
                <br />

                <!-- Select ID Type -->
                <div style="clear: both; margin: 0px 0px 20px 30px;">
                    <asp:Label ID="lblIdNumber" runat="server" CssClass="srtsLabel_medium" Text="*ID Type" Width="120px" />
                    <asp:DropDownList ID="ddlIDNumberType" runat="server" ToolTip="Select the type of ID patient is using to identify self." CssClass="srtsDropDown_medium"
                        AppendDataBoundItems="true" DataTextField="Value" DataValueField="Key" TabIndex="2" Width="245px">
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator ID="rfvIDNumberType"
                        runat="server"
                        ControlToValidate="ddlIDNumberType"
                        InitialValue="X"
                        ErrorMessage="ID Number Type is a required field"
                        Display="None"
                        ValidationGroup="allValidators"></asp:RequiredFieldValidator>
                </div>

                <!-- Enter ID Number -->
                <div style="margin: 0px 0px 20px 30px;">
                    <asp:Label ID="blIdNumber" runat="server" CssClass="srtsLabel_medium" Text="*ID Number" Width="120px" />
                    <asp:TextBox ID="tbIDNumber" runat="server" CssClass="PersonAddTextBox" ToolTip="Enter patient ID number" ClientIDMode="Static" 
                        AutoPostBack="true" OnTextChanged="tbIDNumber_TextChanged" TabIndex="3" MaxLength="11" />
                    <asp:CustomValidator ID="cvIDNumber"
                        runat="server"
                        ErrorMessage="TBD"
                        Text="cvIDNumber"
                        ValidateEmptyText="True"
                        OnServerValidation="ValidateIDNumber"
                        CssClass="requestValidator" Display="none" EnableClientScript="False"></asp:CustomValidator>
                    <asp:Label ID="lblTaboff" Text="Type an ID Number and hit Enter." runat="server" Visible="false" ForeColor="#FF3300"></asp:Label>
                </div>

                <asp:GridView ID="gvSearch" runat="server" ClientIDMode="Static" AutoGenerateColumns="False"
                    GridLines="None" DataKeyNames="ID" Width="930px" CssClass="mGrid" AlternatingRowStyle-CssClass="alt"
                    ViewStateMode="Enabled" PageSize="7" EmptyDataText="No Data Found" Visible="False"
                    OnRowDataBound="gvSearch_RowDataBound">
                    <Columns>
                        <asp:TemplateField HeaderStyle-HorizontalAlign="Left" ItemStyle-Width="130px">
                            <ItemTemplate>
                                <asp:Button ID="btnAddIndividualType" runat="server" CssClass="srtsButton" Text="Add" OnCommand="btnAddIndividualType_Command" />
                                <asp:Button ID="btnGoToPerson" runat="server" CssClass="srtsButton" Text="View" />
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
        </div>

        <asp:Panel ID="pnlCompleteForm" runat="server" Visible="false">
            <asp:UpdatePanel ID="upCompleteForm" runat="server" ChildrenAsTriggers="false" UpdateMode="Conditional">
                <ContentTemplate>
                    <!-- Select Individual Type -->
                    <div id="divIndTypes" runat="server" style="text-align: left; margin: -3px 0px 5px 30px;">
                        <asp:Label ID="lblIndTypes" runat="server" Text="*Select individual type(s):" Width="175px" CssClass="srtsLabel_medium" ControlToValidate="lblIndTypes" />
                        <div id="divIndTypeHighlight" runat="server">
                            <asp:CheckBox ID="cbProvider" runat="server" ClientIDMode="Static" /><asp:Label ID="lblProvider" runat="server" Text="Provider" Style="padding-left: 5px" />
                            <asp:CheckBox ID="cbTechnician" runat="server" ClientIDMode="Static" Style="padding-left: 20px" /><asp:Label ID="lblTechnician" runat="server" Text="Technician" Style="padding-left: 5px" />
                            <asp:CheckBox ID="cbAdministrator" runat="server" ClientIDMode="Static" Style="padding-left: 20px" /><asp:Label ID="lblOther" runat="server" Text="Other (e.g., Admin, Clerk)" Style="padding-left: 5px" /><br />
                            <asp:CustomValidator ID="cvIndTypes" runat="server" OnServerValidate="ValidateIndTypeCBs" ValidationGroup="allValidators" ErrorMessage="At least one individual type is required."></asp:CustomValidator>
                        </div>
                    </div>

                    <!-- Add Personal Information, Service Information -->
                    <div class="w3-row">
                        <!-- Personal Information -->
                        <div class="w3-half" style="float: left; width: 50%">
                            <div class="BeigeBoxContainer" style="margin: 0px 10px">
                                <div class="BeigeBoxHeader" style="padding: 12px 10px 3px 15px">
                                    <span class="label">Add Personal Information</span>
                                </div>
                                <div class="BeigeBoxContent padding" style="height: 200px">
                                    <div id="divPersonalData">
                                        <div id="divPersonalDataMsg" style="color: red; width: 90%; margin-bottom: 10px;"></div>

                                        <!-- Last Name -->
                                        <div style="margin: 0px 0px 10px 0px;">
                                            <asp:Label ID="lblLastName" runat="server" Text="*Last Name" Width="120px" CssClass="srtsLabel_medium" />
                                            <asp:TextBox ID="tbLastName" runat="server" MaxLength="75" ToolTip="Enter last name." CssClass="srtsTextBox_medium" TabIndex="4" Width="300px" ClientIDMode="Static" />
                                            <asp:CustomValidator ID="cvLastName" runat="server"
                                                ClientValidationFunction="ValidateName"
                                                ControlToValidate="tbLastName" ValidateEmptyText="true"></asp:CustomValidator>
                                        </div>
                                        <!-- First Name -->
                                        <div style="margin: 0px 0px 10px 0px;">
                                            <asp:Label ID="lblFirstName" runat="server" Text="*First Name" Width="120px" CssClass="srtsLabel_medium" />
                                            <asp:TextBox ID="tbFirstName" ClientIDMode="Static" runat="server" MaxLength="75" ToolTip="Enter first name." CssClass="srtsTextBox_medium" TabIndex="5" Width="300px" />
                                            <asp:CustomValidator ID="cvFirstName"
                                                runat="server"
                                                ClientValidationFunction="ValidateName"
                                                ControlToValidate="tbFirstName" ValidateEmptyText="true" />
                                        </div>
                                        <!-- Middle Name -->
                                        <div style="margin: 0px 0px 10px 0px;">
                                            <asp:Label ID="lblMiddleName" runat="server" Text="Middle Name" Width="120px" CssClass="srtsLabel_medium" />
                                            <asp:TextBox ID="tbMiddleName" runat="server" MaxLength="75" ToolTip="Enter middle name or initial."
                                                CssClass="srtsTextBox_medium" TabIndex="6" Width="300px" ClientIDMode="Static" />
                                            <asp:CustomValidator ID="cvMiddleName"
                                                runat="server"
                                                ClientValidationFunction="ValidateName"
                                                ControlToValidate="tbMiddleName"
                                                ValidateEmptyText="True" />
                                        </div>
                                        <!-- DOB -->
                                        <div style="margin: 0px 0px 0px 0px; width: 100%;">
                                            <div class="left">
                                                <asp:Label ID="lblDOB" runat="server" Text="Date of Birth(mm/dd/yyyy)" Width="120px" CssClass="srtsLabel_medium" />
                                            </div>
                                            <div class="left">
                                                &nbsp;<asp:TextBox ID="tbDOB" runat="server" TabIndex="7" CssClass="srtsDateTextBox_medium" ToolTip="Enter DOB (mm/dd/yyyy)" Width="300px" />
                                                <asp:Image runat="server" ID="calImage1" AlternateText="Calendar" ImageUrl="~/Styles/images/Calendar_scheduleHS.png" />
                                                <ajaxToolkit:CalendarExtender ID="ceDOB" runat="server"
                                                    TargetControlID="tbDOB" Format="MM/dd/yyyy" PopupButtonID="calImage1" DefaultView="Years">
                                                </ajaxToolkit:CalendarExtender>
                                                <asp:CustomValidator ID="cvDOB" runat="server"
                                                    ClientValidationFunction="ValidateDate"
                                                    ControlToValidate="tbDOB"
                                                    ValidateEmptyText="True"></asp:CustomValidator>
                                            </div>
                                        </div>

                                        <div class="w3-row" style="clear: both">
                                            <!-- Gender -->
                                            <div class="w3-third">
                                                <div style="margin: 0px 0px 10px 0px;">
                                                    <div>
                                                        <asp:Label ID="lblSex" runat="server" Text="&nbsp;&nbsp;Gender" Width="120px" CssClass="srtsLabel_medium" />
                                                    </div>
                                                    <div class="left">
                                                        <asp:RadioButtonList ID="rblGender" runat="server" TabIndex="8" RepeatDirection="Horizontal" ToolTip="Select gender.">
                                                            <asp:ListItem Text="Male" Value="M" />
                                                            <asp:ListItem Text="Female" Value="F" />
                                                        </asp:RadioButtonList>
                                                    </div>
                                                </div>
                                            </div>

                                            <!-- Site POC -->
                                            <div class="w3-third">
                                                <div class="clearleft left" style="margin: 0px 0px 10px 0px;">
                                                    <div class="left">
                                                        <asp:Label ID="lblIsPOC" runat="server" Text="&nbsp;&nbsp;Site POC?" CssClass="srtsLabel_medium" Width="120px"></asp:Label>
                                                    </div>
                                                    <div class="left">
                                                        <asp:RadioButtonList runat="server" ID="rblIsPOC" RepeatDirection="Horizontal" TabIndex="10">
                                                            <asp:ListItem Text="True" Value="True"></asp:ListItem>
                                                            <asp:ListItem Text="False" Value="False" Selected="True"></asp:ListItem>
                                                        </asp:RadioButtonList>
                                                    </div>
                                                </div>
                                            </div>

                                            <!-- EAD Expiration Date -->
                                            <div class="w3-third">
                                                <div class="clearleft left" style="margin: 0px 0px 10px 0px;">
                                                    <asp:Label ID="lblActiveDutyExtend" runat="server" Text="EAD Expiration Date" Width="140px" CssClass="srtsLabel_medium" />
                                                    <asp:TextBox ID="tbEADExpires" runat="server" CssClass="srtsDateTextBox_medium" TabIndex="9"></asp:TextBox>
                                                    <asp:Image runat="server" ID="calImage" AlternateText="Calendar" ImageUrl="~/Styles/images/Calendar_scheduleHS.png" />
                                                    <ajaxToolkit:CalendarExtender ID="ceEAD" runat="server" TargetControlID="tbEADExpires"
                                                        Format="MM/dd/yyyy" PopupButtonID="calImage">
                                                    </ajaxToolkit:CalendarExtender>
                                                    <asp:CustomValidator ID="cvEad" runat="server"
                                                        ClientValidationFunction="ValidateDate"
                                                        ControlToValidate="tbEADExpires"
                                                        ValidateEmptyText="True"></asp:CustomValidator>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <!-- Add Service Information -->
                        <div class="w3-half" style="float: right; width: 50%">
                            <div class="BeigeBoxContainer" style="margin: 0px 10px">
                                <div class="BeigeBoxHeader" style="padding: 12px 10px 3px 15px">
                                    <span class="label">Add Service Information</span>
                                </div>
                                <div class="BeigeBoxContent padding" style="height: 200px; margin: 0px">
                                    <div id="divServiceData">
                                        <div id="divServiceDataMsg"></div>
                                        <asp:UpdatePanel ID="upEligibility" runat="server" ChildrenAsTriggers="false" UpdateMode="Conditional">
                                            <ContentTemplate>
                                                <div style="margin: 20px 0px 20px 0px;">
                                                    <asp:Label ID="lblBranch" runat="server" Text="*Branch" Width="120px" CssClass="srtsLabel_medium" />
                                                    <asp:DropDownList ID="ddlBOS" runat="server" AppendDataBoundItems="true" ToolTip="Select the individuals' branch." ClientIDMode="Static"
                                                        DataTextField="bosText" DataValueField="bosValue" TabIndex="10" CssClass="srtsDropDown_medium" OnSelectedIndexChanged="ddlBOS_SelectedIndexChanged" AutoPostBack="true" Width="300px">
                                                    </asp:DropDownList>
                                                    <asp:CustomValidator ID="cvBos" runat="server" ControlToValidate="ddlBOS" EnableClientScript="true" ClientValidationFunction="ValidateBos"></asp:CustomValidator>
                                                </div>

                                                <div style="margin: 0px 0px 20px 0px;">
                                                    <asp:Label ID="lblStatus" runat="server" Text="*Status" Width="120px" CssClass="srtsLabel_medium" />
                                                    <asp:DropDownList ID="ddlStatusType" runat="server" AppendDataBoundItems="true" DataTextField="statusText"
                                                        DataValueField="statusValue" TabIndex="11" OnSelectedIndexChanged="ddlStatusType_SelectedIndexChanged1" AutoPostBack="true"
                                                        CssClass="srtsDropDown_medium" Width="300px">
                                                    </asp:DropDownList>
                                                    <asp:CustomValidator ID="cvStatus" runat="server" ControlToValidate="ddlStatusType" EnableClientScript="true" ClientValidationFunction="ValidateStatus"></asp:CustomValidator>
                                                </div>

                                                <div style="margin: 0px 0px 20px 0px;">
                                                    <asp:Label ID="lblRank" runat="server" Text="*Grade" Width="120px" CssClass="srtsLabel_medium" />
                                                    <asp:DropDownList ID="ddlRank" runat="server" AppendDataBoundItems="true" ToolTip="Select the individuals' grade."
                                                        DataTextField="rankText" DataValueField="rankValue" TabIndex="12" CssClass="srtsDropDown_medium" Width="300px">
                                                    </asp:DropDownList>
                                                    <asp:CustomValidator ID="cvGrade" runat="server" ControlToValidate="ddlRank" EnableClientScript="true" ClientValidationFunction="ValidateGrade"></asp:CustomValidator>
                                                </div>
                                            </ContentTemplate>
                                            <Triggers>
                                                <asp:AsyncPostBackTrigger ControlID="ddlBOS" EventName="SelectedIndexChanged" />
                                                <asp:AsyncPostBackTrigger ControlID="ddlStatusType" EventName="SelectedIndexChanged" />
                                            </Triggers>
                                        </asp:UpdatePanel>

                                        <div class="clearleft left" style="margin: 0px 0px 20px 0px;">
                                            <%--<asp:Label ID="lblTheater" runat="server" Text="Theater Zip Code" Width="120px" CssClass="srtsLabel_medium" />--%>
                                            <asp:DropDownList ID="ddlTheaterLocationCodes" runat="server" DataTextField="TheaterCode" Visible ="false"
                                                DataValueField="TheaterCode" AppendDataBoundItems="true" TabIndex="14" CssClass="srtsDropDown_medium" Width="300px">
                                                <asp:ListItem Text="-select-" Value=""></asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <br />
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="btnAdd" EventName="Click" />
                </Triggers>
            </asp:UpdatePanel>
        </asp:Panel>

        <div id="divButtons" runat="server" style="clear: both; margin-top: 20px;" visible="false">
            <asp:Button ID="btnAdd" runat="server" CssClass="srtsButton" Text="Submit" ToolTip="Add new information" OnClick="btnAdd_Click" TabIndex="29" CausesValidation="true" />
            <asp:Button ID="btnCancel" runat="server" CssClass="srtsButton" Text="Cancel" ToolTip="Cancel current changes and return to previous page" OnClick="btnCancel_Click" CausesValidation="False" TabIndex="30" />
        </div>
    </div>
    <asp:HiddenField ID="hdfState" runat="server" />
    <asp:HiddenField ID="hdfCity" runat="server" />
</asp:Content>
