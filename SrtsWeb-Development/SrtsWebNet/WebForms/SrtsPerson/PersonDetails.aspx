<%@ Page Title="" Language="C#" MasterPageFile="~/srtsMaster.Master" AutoEventWireup="true" CodeBehind="PersonDetails.aspx.cs" Inherits="SrtsWeb.SrtsPerson.PersonDetails" EnableEventValidation="false" EnableViewState="true" %>

<%@ MasterType VirtualPath="~/srtsMaster.Master" %>
<asp:Content ID="cHeadContent" runat="server" ContentPlaceHolderID="HeadContent">
    <link href="../../Styles/PassFailConfirm.css" rel="stylesheet" />
    <link rel="stylesheet" type="text/css" href="../../Styles/w3.css" />
    <style>
        .messagePersonal_Service {
            position: relative;
            top: -345px;
            left: -195px;
            z-index: 2000;
            width: 400px;
        }

        .valEAD {
            position: relative;
            top: 15px;
            left: 10px;
            right: 0px;
        }

        td {
            text-align: center;
            height: 20px;
            padding-left: 10px;
        }

        .AddressVerificationDialog {
            position: absolute;
            top: 10px;
            left: 95px;
            height: auto;
            min-height: 120px;
            min-width: 650px;
            padding: 0px;
            background: transparent;
            border-radius: 4px;
        }

        .shadow {
            -webkit-box-shadow: 10px 10px 5px 0px rgba(0,0,0,0.12);
            -moz-box-shadow: 10px 10px 5px 0px rgba(0,0,0,0.12);
            box-shadow: 10px 10px 5px 0px rgba(0,0,0,0.12);
        }


        .AddressVerificationDialog .header_info {
            font-size: 15px;
            color: #004994;
            padding: 5px 10px;
            background-color: transparent;
        }

        .AddressVerificationDialog .content {
            background-color: #fff;
            padding: 10px 10px;
            text-align: left;
        }

        .AddressVerificationDialog .title {
            width: 95%;
            padding: 10px 10px;
            text-align: center;
            font-size: 17px !important;
            color: #006600;
        }

        .AddressVerificationDialog .message {
            margin: 5px;
            padding: 5px 10px;
            text-align: center;
            font-size: 13px !important;
            color: #000;
        }

        .AddressVerificationDialog .w3-closebtn {
            margin-top: -3px;
        }

        .rightArrow {
            float: left;
            margin: -3px 0px 0px 0px;
        }

        .btnHide {
            display: none;
        }

        .btnShow {
            display: inline;
        }

        .invalidField {
            background-color: #f3e28f;
        }
    </style>
</asp:Content>

<asp:Content ID="PatientInformation" ContentPlaceHolderID="ContentTop_Title_Right" runat="server">
    <asp:Literal ID="litContentTop_Title_Right" runat="server"></asp:Literal>
</asp:Content>

<asp:Content ID="Content6" ContentPlaceHolderID="MainContent" runat="server">
    <asp:ScriptManagerProxy ID="smpProxy" runat="server">
        <Scripts>
            <asp:ScriptReference Path="~/Scripts/Person/PersonDetails.js" />
            <asp:ScriptReference Path="~/Scripts/Person/PersonDetailVal.js" />
            <asp:ScriptReference Path="~/Scripts/Global/SrtsCustomValidators.js" />
            <asp:ScriptReference Path="~/Scripts/Global/PassFailConfirm.js" />
            <asp:ScriptReference Path="~/Scripts/Global/GlobalVal.js" />
            <asp:ScriptReference Path="~/Scripts/Global/SharedAddress.js" />
            <asp:ScriptReference Path="~/Scripts/Global/Ascii.js" />
        </Scripts>
    </asp:ScriptManagerProxy>

    <div class="divSingleColumn">
        <div id="divOrderMgmt" runat="server" style="position: relative; top: -25px;">
            <asp:LinkButton ID="lnbOrderManagement" runat="server" Text="Order Management" OnClick="lnbOrderManagement_Click" CausesValidation="false" />
        </div>

        <div style="clear: both;"></div>
        <asp:UpdatePanel ID="upDeersRefresh" runat="server" ChildrenAsTriggers="true" UpdateMode="Conditional">
            <ContentTemplate>
                <!-- Patient Name Header -->
                <div id="divPatientNameHeader" class="patientnameheader" runat="server" visible="true">
                    <asp:LinkButton ID="lnbPatientNameHeader" runat="server" Text="Patient Name" ClientIDMode="Static" OnClick="lnbPatientNameHeader_Click" CausesValidation="false" ToolTip="Click the name to refresh data from DEERS/DMDC."></asp:LinkButton>
                </div>
                <div id="divPersonError" runat="server" style="width: 75%;"></div>
                <!-- Personal / Service Information Container -->
                <div class="w3-row padding" style="margin-top: -40px">
                    <asp:UpdatePanel ID="uplPersonDemographics" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="False">
                        <ContentTemplate>
                            <div class="w3-col s8">
                                <div class="BeigeBoxContainer" style="margin: 0px 10px">
                                    <!-- Personal Information Container -->
                                    <div class="w3-half">
                                        <div class="BeigeBoxHeader" style="padding: 12px 10px 3px 15px">
                                            <span class="label" style="margin-left: 10px">Personal Information</span>
                                        </div>
                                        <div class="BeigeBoxContent" style="height: 330px; margin-top: 5px; border-right: 1px solid #E7CFAD">
                                            <div id="divPersonalData" style="clear: both;">
                                                <div id="divPersonalDataMsg" style="color: red; width: 90%;"></div>
                                                <div style="height: auto">
                                                    <div id="personalDataMessage"></div>
                                                </div>

                                                <div class="padding" style="padding-top: 0px; margin-top: 0px">
                                                    <!-- Last Name -->
                                                    <div class="padding">
                                                        <asp:Label ID="lblLastName" runat="server" Text="*Last Name" CssClass="srtsLabel_medium" /><br />
                                                        <asp:TextBox ID="txtLastName" runat="server" MaxLength="75" ToolTip="Enter patient last name." CssClass="srtsTextBox_medium" TabIndex="2" ClientIDMode="Static" />
                                                        <asp:CustomValidator ID="cvLastName" runat="server" ClientValidationFunction="ValidateName" ControlToValidate="txtLastName" ValidateEmptyText="True" />
                                                    </div>
                                                    <!-- First Name -->
                                                    <div class="padding">
                                                        <asp:Label ID="lblFirstName" runat="server" Text="*First Name" CssClass="srtsLabel_medium" /><br />
                                                        <asp:TextBox ID="tbFirstName" ClientIDMode="Static" runat="server" MaxLength="75" ToolTip="Enter patient first name." CssClass="srtsTextBox_medium" TabIndex="3" />
                                                        <asp:CustomValidator ID="cvFirstName" runat="server" ClientValidationFunction="ValidateName" ControlToValidate="tbFirstName" ValidateEmptyText="True" />
                                                    </div>
                                                    <!-- Middle Name -->
                                                    <div class="padding">
                                                        <asp:Label ID="lblMiddleName" runat="server" Text="Middle Name" CssClass="srtsLabel_medium" /><br />
                                                        <asp:TextBox ID="tbMiddleName" runat="server" MaxLength="75" ToolTip="Enter patient middle name or initial." CssClass="srtsTextBox_medium" TabIndex="4" ClientIDMode="Static" />
                                                        <asp:CustomValidator ID="cvMiddleName" runat="server" ClientValidationFunction="ValidateName" ControlToValidate="tbMiddleName" ValidateEmptyText="True" />
                                                    </div>
                                                    <!-- Gender / DOB -->
                                                    <div class="w3-row padding" style="padding-bottom: 0px; margin-bottom: 0px">
                                                        <!-- Sex -->
                                                        <div class="w3-half">
                                                            <div>
                                                                <asp:Label ID="lblSex" runat="server" Text="Gender" CssClass="srtsLabel_medium" AssociatedControlID="rblGender" /><br />
                                                                <div style="float: left;">
                                                                    <asp:RadioButtonList ID="rblGender" runat="server" TabIndex="6" RepeatDirection="Horizontal" ToolTip="Select patient gender.">
                                                                        <asp:ListItem Text="Male" Value="M" />
                                                                        <asp:ListItem Text="Female" Value="F" />
                                                                    </asp:RadioButtonList>
                                                                </div>
                                                            </div>
                                                        </div>

                                                        <!-- DOB -->
                                                        <div class="w3-half">
                                                            <div style="padding-left: 20px; padding-bottom: 0px; margin-bottom: 0px">
                                                                <asp:Label ID="lblDOB" runat="server" Text="Date of Birth(MM/DD/YYYY)" CssClass="srtsLabel_medium" /><br />
                                                                <asp:TextBox ID="tbDOB" runat="server" TabIndex="5" Width="85px" CssClass="srtsTextBox_medium" ClientIDMode="Static" />
                                                                <div class="calRight">
                                                                    <asp:Image runat="server" ID="calImage1" AlternateText="Calendar" ImageUrl="~/Styles/images/Calendar_scheduleHS.png" ClientIDMode="Static" />
                                                                </div>

                                                                <ajaxToolkit:CalendarExtender ID="ceDOB" runat="server" TargetControlID="tbDOB" Format="MM/dd/yyyy" PopupButtonID="calImage1">
                                                                </ajaxToolkit:CalendarExtender>
                                                                <asp:CustomValidator ID="cvDOB" runat="server" ClientValidationFunction="ValidateDate" ControlToValidate="tbDOB" ValidateEmptyText="True"></asp:CustomValidator>
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <!-- Comments -->
                                                    <div class="padding" style="padding-bottom: 0px; padding-top: 3px">
                                                        <asp:Label ID="lblComments" runat="server" Text="Comments" Width="100px" CssClass="srtsLabel_medium" /><br />
                                                        <asp:TextBox ID="tbComments" runat="server" onKeyDown="return textboxMaxCommentSize(this, 256, event, getlblRemainingID(), gettbCommentID())"
                                                            ToolTip="Enter any comments, valid characters include + - . , ! ? # ' / ( )"
                                                            Rows="3" TextMode="MultiLine" TabIndex="13" CssClass="srtsTextBox_medium" Height="50px">
                                                        </asp:TextBox>
                                                        <asp:CustomValidator ID="cvComment" runat="server" ErrorMessage="Invalid character(s) in Comment" ClientValidationFunction="textboxCommentValidation"
                                                            Text="*" ControlToValidate="tbComments" ValidateEmptyText="True" CssClass="requestValidator">
                                                        </asp:CustomValidator>
                                                        <div style="text-align: center;">
                                                            <asp:Label ID="lblRemaining" runat="server" CssClass="srtsLabel_medium"></asp:Label>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="BeigeBoxFooter">
                                        </div>
                                    </div>

                                    <!-- Service Information Container -->
                                    <div class="w3-half">
                                        <div class="BeigeBoxHeader" style="padding: 12px 10px 3px 15px">
                                            <span class="label" style="margin-left: 10px">Service Information</span>
                                        </div>
                                        <div class="BeigeBoxContent" style="height: 330px; margin-top: 5px; margin-left: 10px">
                                            <div id="divServiceData" style="clear: both;">
                                                <div id="divServiceDataMsg" style="color: red; width: 90%;"></div>
                                                <div class="w3-row padding" style="padding-top: 10px; padding-bottom: 0px; margin-bottom: 0px">
                                                    <asp:UpdatePanel ID="upDemo" runat="server" ChildrenAsTriggers="false" UpdateMode="Conditional">
                                                        <ContentTemplate>
                                                            <!-- Branch of Service -->
                                                            <div style="padding-top: 3px; margin-top: 0px; margin-bottom: 10px">
                                                                <asp:Label ID="lblBranch" runat="server" Text="*Branch" Width="100px" CssClass="srtsLabel_medium" /><br />
                                                                <asp:DropDownList ID="ddlBOS" runat="server" AppendDataBoundItems="true" ToolTip="Select the individuals' branch."
                                                                    DataTextField="bosText" DataValueField="bosValue" TabIndex="8" AutoPostBack="true"
                                                                    OnSelectedIndexChanged="ddlBOS_SelectedIndexChanged" Width="275px">
                                                                    <asp:ListItem Text="Select Branch" Value=""></asp:ListItem>
                                                                </asp:DropDownList>
                                                                <asp:RequiredFieldValidator ID="rfvBOS" runat="server" ControlToValidate="ddlBOS"
                                                                    ErrorMessage="Branch Of Service is a required selection" Display="None"></asp:RequiredFieldValidator>
                                                            </div>

                                                            <!-- Status -->
                                                            <div style="padding-top: 3px; margin-top: 0px; margin-bottom: 10px">
                                                                <asp:Label ID="lblStatus" runat="server" Text="*Status" Width="100px" CssClass="srtsLabel_medium" /><br />
                                                                <asp:DropDownList ID="ddlStatusType" runat="server" DataTextField="statusText"
                                                                    DataValueField="statusValue" AutoPostBack="true" TabIndex="9" OnSelectedIndexChanged="ddlStatusType_SelectedIndexChanged" Width="275px">
                                                                    <asp:ListItem Text="Select Status" Value=""></asp:ListItem>
                                                                </asp:DropDownList>
                                                                <asp:RequiredFieldValidator ID="rfvStatus" runat="server" ControlToValidate="ddlStatusType"
                                                                    ErrorMessage="Status is a required selection" Display="None"></asp:RequiredFieldValidator>
                                                            </div>

                                                            <!-- Grade -->
                                                            <div style="padding-top: 3px; margin-top: 0px; margin-bottom: 10px">
                                                                <asp:Label ID="lblRank" runat="server" Text="*Grade" Width="100px" CssClass="srtsLabel_medium" /><br />
                                                                <asp:DropDownList ID="ddlRank" runat="server" AppendDataBoundItems="true" ToolTip="Select the individuals' grade."
                                                                    DataTextField="rankText" DataValueField="rankValue" TabIndex="10" Width="275px">
                                                                    <asp:ListItem Text="Select Grade" Value=""></asp:ListItem>
                                                                </asp:DropDownList>
                                                                <asp:RequiredFieldValidator ID="rfvRank" runat="server" ControlToValidate="ddlRank"
                                                                    InitialValue="0" ErrorMessage="Grade is a required selection." Display="None"></asp:RequiredFieldValidator>
                                                            </div>
                                                        </ContentTemplate>
                                                        <Triggers>
                                                            <asp:AsyncPostBackTrigger ControlID="ddlBOS" EventName="SelectedIndexChanged" />
                                                            <asp:AsyncPostBackTrigger ControlID="ddlStatusType" EventName="SelectedIndexChanged" />
                                                        </Triggers>
                                                    </asp:UpdatePanel>

                                                    <!-- Extended Active Duty Expiration Date -->
                                                    <div style="padding-top: 3px; margin-top: 0px; margin-bottom: 10px">
                                                        <asp:Label ID="lblActiveDutyExtend" runat="server" Text="Extended Active Duty Expiration Date" CssClass="srtsLabel_medium" /><br />
                                                        <asp:TextBox ID="tbEADExpires" runat="server" TabIndex="12" Width="250px" CssClass="srtsTextBox_medium" ClientIDMode="Static"></asp:TextBox>
                                                        <div class="calRight" style="left: 256px">
                                                            <asp:Image runat="server" ID="calImage" AlternateText="Calendar" ImageUrl="~/Styles/images/Calendar_scheduleHS.png" />
                                                        </div>

                                                        <ajaxToolkit:CalendarExtender ID="ceEAD" runat="server" TargetControlID="tbEADExpires" Format="MM/dd/yyyy" PopupButtonID="calImage"></ajaxToolkit:CalendarExtender>
                                                        <asp:CustomValidator ID="cvEad" runat="server" ClientValidationFunction="ValidateDate" ControlToValidate="tbEADExpires" ValidateEmptyText="True">
                                                        </asp:CustomValidator>
                                                    </div>
                                                    <!-- Theater -->
                                                    <div style="padding-top: 3px; margin-top: 0px; margin-bottom: 10px">
<%--                                                        <div style="margin-bottom: 4px">
                                                            <asp:Label ID="lblTheater" runat="server" Text="Theater Zip Code" CssClass="srtsLabel_medium" />
                                                        </div>--%>
                                                        <asp:DropDownList ID="ddlTheaterLocationCodes" runat="server" DataTextField="TheaterCode" Visible="False"
                                                            DataValueField="TheaterCode" AppendDataBoundItems="true" TabIndex="11" Width="275px">
                                                            <asp:ListItem Text="Select Theater" Value=""></asp:ListItem>
                                                        </asp:DropDownList>
                                                    </div>


                                                </div>
                                            <!-- Service Information - Save Button -->
                                            <div class="padding" style="position: relative; top: -5px; padding-bottom: 0px; text-align: right">
                                                <div style="display: inline-block; float: left; font-weight: bold;">
                                                    <asp:Label ID="lblSiteCode" runat="server" Text="" CssClass="srtsLabel_medium" />
                                                </div>
                                                <asp:Button ID="bSavePersonalServiceData" runat="server" CssClass="srtsButton" Text="Save" OnClientClick="return CanSubmit('bSavePersonalServiceData');"
                                                    OnClick="bSavePersonalServiceData_Click" CausesValidation="False" />
                                            </div>
                                        </div>

                                        <!-- Personal and Service Information - Message -->
                                        <div class="w3-row">
                                            <div style="height: auto">
                                                <div id="serviceDataMessage" class="messagePersonal_Service"></div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="BeigeBoxFooter"></div>
                                </div>
                            </div>
                            </div>
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="bSavePersonalServiceData" EventName="Click" />
                        </Triggers>
                    </asp:UpdatePanel>

                    <!-- Patient Id Numbers -->
                    <div class="w3-col s4">
                        <div class="BeigeBoxContainer" style="margin: 0px 10px">
                            <div class="BeigeBoxHeader" style="padding: 12px 10px 3px 15px">
                                <span class="label">ID Numbers</span>
                            </div>
                            <div class="BeigeBoxContent" style="height: 330px; margin-left: 10px">
                                <asp:UpdatePanel ID="upIdNumber" runat="server" ChildrenAsTriggers="false" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <div id="divIdNumber">
                                            <div id="divIdNumMsg" style="color: red; width: 90%; display: none;"></div>
                                            <div style="height: auto;">
                                                <div id="idMessage"></div>
                                            </div>
                                            <!-- Patient Id Numbers - SSN Number -->
                                            <div class="padding">
                                                <asp:Label ID="lblDss" runat="server" CssClass="srtsLabel_medium" Text="Social Security Number:" /><br />
                                                <asp:TextBox ID="tbDss" runat="server" TabIndex="1" CssClass="srtsTextBox_medium" ClientIDMode="Static" />
                                                <asp:CustomValidator ID="cvDss" runat="server" ControlToValidate="tbDss" ClientValidationFunction="ValidateDss" ValidateEmptyText="true" />
                                            </div>
                                            <br />
                                            <!-- Patient Id Numbers - DOD ID Number -->
                                            <div class="padding">
                                                <asp:Label ID="lblDin" runat="server" CssClass="srtsLabel_medium" Text="DOD ID Number:" /><br />
                                                <asp:TextBox ID="tbDin" runat="server" TabIndex="1" CssClass="srtsTextBox_medium" ClientIDMode="Static" />
                                                <asp:CustomValidator ID="cvDin" runat="server" ControlToValidate="tbDin" ClientValidationFunction="ValidateDin" ValidateEmptyText="true" />
                                            </div>
                                            <br />
                                            <!-- Patient Id Numbers - DOD Benefits Number-->
                                            <div class="padding">
                                                <asp:Label ID="lblDbn" runat="server" CssClass="srtsLabel_medium" Text="DOD Benefits Number:" /><br />
                                                <asp:TextBox ID="tbDbn" runat="server" TabIndex="1" CssClass="srtsTextBox_medium" ClientIDMode="Static" />
                                                <asp:CustomValidator ID="cvDbn" runat="server" ControlToValidate="tbDbn" ClientValidationFunction="ValidatePinDbn" ValidateEmptyText="true" />
                                            </div>
                                            <br />
                                            <!-- Patient Id Numbers - Provider ID Number -->
                                            <div class="padding">
                                                <asp:Label ID="lblPin" runat="server" CssClass="srtsLabel_medium" Text="Provider ID Number:" /><br />
                                                <asp:TextBox ID="tbPin" runat="server" TabIndex="1" CssClass="srtsTextBox_medium" ClientIDMode="Static" />
                                                <asp:CustomValidator ID="cvPin" runat="server" ControlToValidate="tbPin" ClientValidationFunction="ValidatePinDbn" ValidateEmptyText="true" />
                                            </div>
                                            <br />
                                            <!-- Patient Id Numbers - Save Button -->
                                            <div class="padding" style="position: relative; top: -15px; padding-bottom: 0px; text-align: right">
                                                <asp:Button ID="bSaveIdNumbers" runat="server" CssClass="srtsButton"
                                                    CausesValidation="false" Text="Save" TabIndex="4" OnClick="bSaveIdNumbers_Click" Enabled="true" />
                                            </div>
                                        </div>
                                    </ContentTemplate>
                                    <Triggers>
                                        <asp:AsyncPostBackTrigger ControlID="bSaveIdNumbers" EventName="click" />
                                    </Triggers>
                                </asp:UpdatePanel>
                            </div>
                            <div class="BeigeBoxFooter"></div>
                        </div>
                    </div>
                </div>

                <!-- Contact Information Container -->
                <div id="divContactMessage" style="display: none"></div>
                <div class="w3-row padding" style="margin-top: -20px">
                    <!-- Address -->
                    <div class="w3-col" style="width: 600px">
                        <div class="BeigeBoxContainer" style="margin: 0px 10px">
                            <asp:UpdatePanel ID="upAddresses" runat="server" ChildrenAsTriggers="true" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <div class="BeigeBoxHeader" style="padding: 12px 10px 3px 15px">

                                        <div class="w3-col" style="width: 150px">
                                            <span class="label">Address Information</span>
                                        </div>
                                        <div id="addressHeader" runat="server" class="w3-rest srtsLabel_LeftRed" style="float: right; font-size: .8em; margin-top: -2px" clientidmode="Static"></div>
                                    </div>
                                    <div class="BeigeBoxContent" style="margin-left: 10px; padding-top: 0px; min-height: 280px">


                                        <div id="divAddresses" style="clear: both;">

                                            <div class="w3-row" style="width: 535px">
                                                <div class="w3-col" style="width: 375px">
                                                    <asp:RadioButtonList ID="rblAddressType" runat="server" TabIndex="0" RepeatDirection="Horizontal" ToolTip="Select Address Type"
                                                        CausesValidation="false" ClientIDMode="Static" CssClass="srtsLabel_medium" onchange="DoRblAddressTypeChange();">
                                                        <asp:ListItem Text="US Address" Value="US" />
                                                        <asp:ListItem Text="Foreign Address" Value="FN" />
                                                    </asp:RadioButtonList>

                                                </div>

                                                <!-- UIC -->
                                                <div class="w3-rest">
                                                    <div id="divUIC" style="margin: 5px 0px 10px 0px; padding-right: 30px">
                                                        <asp:Label ID="lblPrimaryUIC" runat="server" Text="UIC" CssClass="srtsLabel_medium" />
                                                        <asp:TextBox ID="tbPrimaryUIC" runat="server" CssClass="srtsTextBox_medium" Width="120px" />
                                                    </div>
                                                </div>
                                            </div>

                                            <div style="height: 165px">
                                                <!-- Address 1, Address 2 -->
                                                <div class="w3-row">
                                                    <!-- Address 1, Address 2 -->
                                                    <div id="divAddress1" class="w3-half">
                                                        <!-- Address 1 -->
                                                        <div class="padding">
                                                            <asp:Label ID="lblPrimaryAddAddress1" runat="server" Text="Address 1" CssClass="srtsLabel_medium" /><br />
                                                            <asp:TextBox ID="tbPrimaryAddress1" runat="server" MaxLength="100" CssClass="srtsTextBox_medium ascii" ClientIDMode="Static"
                                                                ToolTip="Enter the patient house and street address." Width="220px" />
                                                            <asp:CustomValidator ID="cvPrimaryAddress1" runat="server" ClientIDMode="Static" ControlToValidate="tbPrimaryAddress1" ClientValidationFunction="ValidateAddress1" ValidateEmptyText="true" EnableClientScript="false" />
                                                        </div>
                                                    </div>
                                                    <div id="divAddress2" class="w3-half">
                                                        <!-- Address 2 -->
                                                        <div class="padding" style="margin-left: 0px">
                                                            <asp:Label ID="lblPrimaryAddAddress2" runat="server" Text="Address 2" CssClass="srtsLabel_medium" /><br />
                                                            <asp:TextBox ID="tbPrimaryAddress2" runat="server" MaxLength="100" CssClass="srtsTextBox_medium ascii" ClientIDMode="Static" ToolTip="Continuation of patient address." Width="220px" />
                                                            <asp:CustomValidator ID="cvPrimaryAddress2" runat="server" ControlToValidate="tbPrimaryAddress2" ClientValidationFunction="ValidateAddress2" ValidateEmptyText="true" />
                                                        </div>
                                                    </div>
                                                </div>

                                                <!-- City, State -->
                                                <div id="divUsAddress">
                                                    <div class="w3-row" style="margin-right: 35px">
                                                        <!-- Zip -->
                                                        <div class="w3-third">
                                                            <!-- Zip -->
                                                            <div style="margin: 0px 0px 10px 0px;">
                                                                <!-- Zip -->
                                                                <div class="padding">
                                                                    <asp:Label ID="lblPrimaryZip" runat="server" Text="Zip" CssClass="srtsLabel_medium" /><br />
                                                                    <asp:TextBox ID="tbPrimaryZipCode" runat="server" CssClass="srtsTextBox_medium" ClientIDMode="Static"
                                                                        ToolTip="Enter patient residence zip code" Width="150px" onkeydown="return (event.keyCode!=13);" onchange="javascript:DoZipLookup()" />
                                                                    <asp:CustomValidator ID="cvPrimaryZipCode" runat="server" ControlToValidate="tbPrimaryZipCode"
                                                                        ClientValidationFunction="ValidateZip" ValidateEmptyText="true" />

                                                                </div>
                                                            </div>
                                                        </div>

                                                        <!-- City -->
                                                        <div class="w3-third">
                                                            <div style="margin: 0px 0px 10px 0px;">
                                                                <!-- City -->
                                                                <div class="padding">
                                                                    <asp:Label ID="lblPrimaryCity" runat="server" Text="City" CssClass="srtsLabel_medium" /><br />
                                                                    <asp:TextBox ID="tbPrimaryCity" runat="server" MaxLength="100" CssClass="srtsTextBox_medium ascii" ClientIDMode="Static" CausesValidation="true"
                                                                        onclick="DoTbCityClick();" onblur="DoTbCityBlur();" ToolTip="Enter city name from patient address" Width="150px" />
                                                                    <asp:CustomValidator ID="cvPrimaryCity" runat="server" ControlToValidate="tbPrimaryCity" ClientValidationFunction="ValidateCity" ValidateEmptyText="true" />
                                                                </div>
                                                            </div>
                                                        </div>

                                                        <!-- APOCity -->
                                                        <div class="w3-third">
                                                            <div style="margin: 0px 0px 0px 20px">
                                                                &nbsp;&nbsp;<asp:RadioButtonList ID="rblCity" runat="server" TabIndex="8" RepeatDirection="Horizontal" ToolTip="Select Area"
                                                                    CausesValidation="true" ClientIDMode="Static" onchange="DoRblCityChange();">
                                                                    <asp:ListItem Text="APO" Value="APO" />
                                                                    <asp:ListItem Text="FPO" Value="FPO" />
                                                                    <asp:ListItem Text="DPO" Value="DPO" />
                                                                </asp:RadioButtonList>
                                                            </div>
                                                        </div>
                                                    </div>


                                                </div>
                                                <!-- State, Country -->
                                                <div class="w3-row" style="padding-top: 20px">
                                                    <!-- State -->
                                                    <div id="divState" class="w3-third">
                                                        <div style="margin-left: 20px">
                                                            <asp:Label ID="lblPrimaryState" runat="server" Text="State" CssClass="srtsLabel_medium" /><br />
                                                            <asp:DropDownList ID="ddlPrimaryState" runat="server" ToolTip="Select patient residence state." ClientIDMode="Static"
                                                                DataTextField="Value" DataValueField="Key" Width="160px">
                                                            </asp:DropDownList>
                                                            <asp:CustomValidator ID="cvPrimaryState" runat="server" ControlToValidate="ddlPrimaryState" ClientValidationFunction="ValidateState" ValidateEmptyText="true" />
                                                        </div>
                                                    </div>

                                                    <!-- Country -->
                                                    <div class="w3-third">
                                                        <div class="padding" style="padding-top: 0px">
                                                            <asp:Label ID="lblPrimaryCountry" runat="server" Text="Country" CssClass="srtsLabel_medium" /><br />
                                                            <asp:DropDownList ID="ddlPrimaryCountry" runat="server" ToolTip="Select patient residence country."
                                                                ClientIDMode="Static" onchange="DoDdlCountryChange();" DataTextField="Text" DataValueField="Value" Width="325px">
                                                            </asp:DropDownList>
                                                        </div>
                                                    </div>
                                                    <div class="w3-third"></div>
                                                </div>
                                            </div>
                                        </div>


                                        <div class="w3-row" style="text-align: left">
                                            <%-- Validation Messages--%>
                                            <div class="w3-col" style="width: 300px">
                                                <div id="divAddressMsg" style="color: red; width: 90%; text-align: left; padding-left: 20px"></div>
                                                <div style="height: auto">
                                                    <div id="addressMessage"></div>
                                                </div>
                                            </div>
                                            <!-- Address - Save Button -->
                                            <div class="w3-rest" style="float: left; width: 550px; margin-left: 15px">
                                                <input type="button" id="bSpecialChars" value="Special Characters" class="srtsButton" onclick="DoDialog();" style="display: none" />
                                                <asp:Button ID="btnAddressVerify" runat="server" CssClass="srtsButton" CausesValidation="false" Text="Validate"
                                                    OnClientClick="return IsValidAddress();" OnClick="bSaveAddress_Click" Enabled="true" ClientIDMode="Static" />
                                                <div class="padding" style="position: relative; top: -40px; left: 25px; padding-bottom: 0px; padding-top: 0px; text-align: right">
                                                    <asp:Button ID="bSaveAddress_Cancel" runat="server" CssClass="srtsButton btnShow" CausesValidation="false" Text="Cancel"
                                                        OnClientClick="Cancel_Save('address');" Enabled="true" ClientIDMode="Static" />
                                                </div>


                                            </div>
                                        </div>
                                        <!-- Special Character User Control -->
                                        <srts:SpecialCharacters id="uSpecialCharacters" runat="server"></srts:SpecialCharacters>


                                        <%--Address Verification Modal --%>
                                        <%-- ///////////////////////////////////////////////////////////////////--%>
                                        <div id="AddressVerificationDialog" class="w3-modal" style="z-index: 30000">
                                            <div class="w3-modal-content">
                                                <div class="w3-container">
                                                    <div class="AddressVerificationDialog">
                                                        <div class="BeigeBoxContainer shadow" style="width: 550px">
                                                            <div style="background-color: #fff">
                                                                <div class="BeigeBoxHeader" style="text-align: left; padding: 12px 10px 3px 15px">
                                                                    <div id="AddressVerificationDialogheader" class="header_info">
                                                                        <span onclick="document.getElementById('AddressVerificationDialog').style.display='none'"
                                                                            class="w3-closebtn">&times;</span>
                                                                        <span class="label">Address Information</span> - Address Validation
                                                                    </div>
                                                                </div>
                                                                <div class="BeigeBoxContent" style="margin-left: 10px; padding-top: 0px; height: 430px">
                                                                    <asp:UpdatePanel ID="UpdatePanel1" runat="server" ChildrenAsTriggers="false" UpdateMode="Conditional">
                                                                        <ContentTemplate>
                                                                            <div class="row padding">
                                                                                <div id="divAddressMessage" class="header_info"><span style="text-align: left; font-size: smaller">The United States Postal service has found and returned the below address. </span></div>
                                                                            </div>
                                                                            <div class="row padding" style="padding-left: 15px; padding-top: 0px; margin-left: 50px; margin-top: -25px">
                                                                                <%-- Address as Entered--%>
                                                                                <div class="w3-col" style="width: 50%">
                                                                                    <div id="divAddressEntered" style="height: 300px">
                                                                                        <div class="header_info">
                                                                                            <div class="rightArrow">
                                                                                                <asp:ImageButton ID="btnSaveEnteredAddress" CommandName="SaveEnteredAddress" runat="server" ImageUrl="~/Styles/images/Arrow_blue_right.gif" Width="25px"
                                                                                                    CausesValidation="false" OnClick="SaveAddress" ToolTip="I would like to use the Mailing Address as Entered." />
                                                                                            </div>
                                                                                            <span style="font-size: smaller">&nbsp;&nbsp;&nbsp;Mailing Address as Entered</span>
                                                                                        </div>
                                                                                        <div>
                                                                                            <!-- Address 1, Address 2 -->
                                                                                            <div class="w3-row">
                                                                                                <!-- Address 1, Address 2 -->

                                                                                                <!-- Address 1 -->
                                                                                                <div class="padding">
                                                                                                    <asp:Label ID="lblAddress1" runat="server" Text="Address 1" CssClass="srtsLabel_medium" /><br />
                                                                                                    <asp:TextBox ID="txtAddress1" runat="server" MaxLength="100" CssClass="srtsTextBox_medium"
                                                                                                        ClientIDMode="Static" Width="255px" ReadOnly="true" />
                                                                                                </div>

                                                                                                <!-- Address 2 -->
                                                                                                <div class="padding">
                                                                                                    <asp:Label ID="lblAddress2" runat="server" Text="Address 2" CssClass="srtsLabel_medium" /><br />
                                                                                                    <asp:TextBox ID="txtAddress2" runat="server" MaxLength="100" CssClass="srtsTextBox_medium"
                                                                                                        ClientIDMode="Static" Width="255px" ReadOnly="true" />
                                                                                                </div>

                                                                                            </div>

                                                                                            <!-- City, State -->
                                                                                            <div id="div6">
                                                                                                <div class="w3-row">
                                                                                                    <!-- City -->
                                                                                                    <div class="w3-half">
                                                                                                        <div style="margin: 0px 0px 10px 0px;">
                                                                                                            <!-- City -->
                                                                                                            <div class="padding">
                                                                                                                <asp:Label ID="lblCity" runat="server" Text="City" CssClass="srtsLabel_medium" /><br />
                                                                                                                <asp:TextBox ID="txtCity" runat="server" MaxLength="100" CssClass="srtsTextBox_medium"
                                                                                                                    ClientIDMode="Static" Width="140px" ReadOnly="true" />
                                                                                                            </div>
                                                                                                        </div>
                                                                                                    </div>
                                                                                                    <!-- Zip -->
                                                                                                    <div class="w3-half">
                                                                                                        <!-- Zip -->
                                                                                                        <div style="margin: 0px 0px 10px 30px;">
                                                                                                            <!-- Zip -->
                                                                                                            <div class="padding">
                                                                                                                <asp:Label ID="lblZipCode" runat="server" Text="Zip" CssClass="srtsLabel_medium" /><br />
                                                                                                                <asp:TextBox ID="txtZipCode" runat="server" CssClass="srtsTextBox_medium" ClientIDMode="Static" Width="75px" ReadOnly="true" />
                                                                                                            </div>
                                                                                                        </div>
                                                                                                    </div>


                                                                                                </div>


                                                                                            </div>
                                                                                            <!-- State, Country -->
                                                                                            <div class="w3-row" style="padding-top: 20px">
                                                                                                <!-- State -->
                                                                                                <div id="div7" class="w3-half">
                                                                                                    <div class="padding" style="padding-top: 0px">
                                                                                                        <asp:Label ID="lblState" runat="server" Text="State" CssClass="srtsLabel_medium" /><br />
                                                                                                        <asp:TextBox ID="txtState" runat="server" MaxLength="100" CssClass="srtsTextBox_medium"
                                                                                                            ClientIDMode="Static" Width="120px" ReadOnly="true" />

                                                                                                    </div>
                                                                                                </div>

                                                                                                <!-- Country -->
                                                                                                <div class="w3-half">
                                                                                                    <div class="padding" style="margin: 0px 0px 0px 10px; padding-top: 0px">
                                                                                                        <asp:Label ID="lblCountry" runat="server" Text="Country" CssClass="srtsLabel_medium" /><br />
                                                                                                        <asp:TextBox ID="txtCountry" runat="server" MaxLength="100" CssClass="srtsTextBox_medium"
                                                                                                            ClientIDMode="Static" Width="96px" ReadOnly="true" />
                                                                                                    </div>
                                                                                                </div>

                                                                                            </div>


                                                                                        </div>
                                                                                    </div>
                                                                                </div>



                                                                                <%--   Verified Address--%>
                                                                                <div class="w3-col" style="width: 50%">
                                                                                    <div id="divAddressVerified">
                                                                                        <div class="header_info">
                                                                                            <%--  <div class="rightArrow"><asp:ImageButton ID="btnSaveVerifiedAddress" CommandName="SaveVerifiedAddress" runat="server" 
                                                           ImageUrl="~/Styles/images/Arrow_blue_right.gif" Width="25px" CausesValidation="false" OnClick="SaveAddress" ToolTip="I would like to use the Verified Mailing Address."  /></div>--%>
                                                                                        </div>

                                                                                        <!-- Address 1, Address 2 -->
                                                                                        <div class="w3-row" style="width: 400px">
                                                                                            <!-- Address 1, Address 2 -->

                                                                                            <!-- Address 1 -->
                                                                                            <div class="padding">
                                                                                                <asp:Label ID="lblAddress1Verified" runat="server" Text="Address 1" CssClass="srtsLabel_medium" /><br />
                                                                                                <asp:TextBox ID="txtAddress1Verified" runat="server" MaxLength="100" CssClass="srtsTextBox_medium"
                                                                                                    ClientIDMode="Static" Width="302px" ReadOnly="true" />
                                                                                            </div>

                                                                                            <!-- Address 2 -->
                                                                                            <div class="padding">
                                                                                                <asp:Label ID="lblAddress2Verified" runat="server" Text="Address 2" CssClass="srtsLabel_medium" /><br />
                                                                                                <asp:TextBox ID="txtAddress2Verified" runat="server" MaxLength="100" CssClass="srtsTextBox_medium"
                                                                                                    ClientIDMode="Static" Width="302px" ReadOnly="true" />
                                                                                            </div>

                                                                                        </div>

                                                                                        <!-- City, State -->

                                                                                        <div class="w3-row" style="width: 400px">
                                                                                            <!-- City -->
                                                                                            <div class="w3-half">
                                                                                                <div style="margin: 0px 0px 10px 0px;">
                                                                                                    <!-- City -->
                                                                                                    <div class="padding">
                                                                                                        <asp:Label ID="lblCityVerified" runat="server" Text="City" CssClass="srtsLabel_medium" /><br />
                                                                                                        <asp:TextBox ID="txtCityVerified" runat="server" MaxLength="135" CssClass="srtsTextBox_medium"
                                                                                                            ClientIDMode="Static" Width="150px" ReadOnly="true" />
                                                                                                    </div>
                                                                                                </div>
                                                                                            </div>
                                                                                            <!-- Zip -->
                                                                                            <div class="w3-half" style="text-align: right">
                                                                                                <!-- Zip -->
                                                                                                <div style="margin: 0px 0px 10px 10px;">
                                                                                                    <!-- Zip -->
                                                                                                    <div class="padding">
                                                                                                        <asp:Label ID="lblZipCodeVerified" runat="server" Text="Zip" CssClass="srtsLabel_medium" /><br />
                                                                                                        <asp:TextBox ID="txtZipCodeVerified" runat="server" CssClass="srtsTextBox_medium" ClientIDMode="Static" Width="95px" ReadOnly="true" />
                                                                                                    </div>
                                                                                                </div>
                                                                                            </div>
                                                                                        </div>



                                                                                        <!-- State, Country -->
                                                                                        <div class="w3-row" style="padding-top: 20px; width: 400px">
                                                                                            <!-- State -->
                                                                                            <div id="div3" class="w3-half">
                                                                                                <div class="padding" style="padding-top: 0px">
                                                                                                    <asp:Label ID="lblStateVerified" runat="server" Text="State" CssClass="srtsLabel_medium" /><br />
                                                                                                    <asp:TextBox ID="txtStateVerified" runat="server" MaxLength="100" CssClass="srtsTextBox_medium"
                                                                                                        ClientIDMode="Static" Width="170px" ReadOnly="true" />
                                                                                                </div>
                                                                                            </div>

                                                                                            <!-- Country -->
                                                                                            <div class="w3-half">
                                                                                                <div class="padding" style="margin: 0px 0px 0px 10px; padding-top: 0px">
                                                                                                    <asp:Label ID="lblCountryVerified" runat="server" Text="Country" CssClass="srtsLabel_medium" /><br />
                                                                                                    <asp:TextBox ID="txtCountryVerified" runat="server" MaxLength="100" CssClass="srtsTextBox_medium"
                                                                                                        ClientIDMode="Static" Width="96px" ReadOnly="true" />
                                                                                                </div>
                                                                                            </div>

                                                                                        </div>



                                                                                        <br />
                                                                                        <br />
                                                                                        <br />
                                                                                        <div class="w3-row" style="width: 400px; text-align: right">
                                                                                            <asp:Button ID="btnSaveVerifiedAddress" runat="server" Text="Save" CssClass="srtsButton" CommandName="SaveVerifiedAddress" CausesValidation="false"
                                                                                                OnClick="SaveAddress" ToolTip="I would like to use the Verified Mailing Address." />
                                                                                            <asp:Button ID="btnSaveVerifiedAddress_Cancel" runat="server" Text="Cancel" CssClass="srtsButton" OnClick="btnCancelAddressSave_Click"
                                                                                                Enabled="true" ClientIDMode="Static" UseSubmitBehavior="false" CausesValidation="false" />
                                                                                        </div>

                                                                                    </div>
                                                                                </div>
                                                                            </div>
                                                                            <div id="divAddressSubmit">
                                                                                <div style="text-align: center">
                                                                                    <asp:Button ID="btnAddressSave" runat="server" Text="Save Address as Entered" CssClass="srtsButton" OnClick="SaveAddress" CausesValidation="false" CommandName="SaveEnteredAddress" />
                                                                                    <br />
                                                                                    (Please note:  The address will only remain valid<br />
                                                                                    for a period of 30 days.)
                                                                                </div>



                                                                                <br />
                                                                                <br />
                                                                                <div style="text-align: center">
                                                                                    <asp:Button ID="btnCancelAddressSave" runat="server" Text="Cancel" CssClass="srtsButton" OnClick="btnCancelAddressSave_Click"
                                                                                        Enabled="true" ClientIDMode="Static" UseSubmitBehavior="false" CausesValidation="false" /><br />
                                                                                    Edit the address and try the validation again.
                                                                                </div>

                                                                            </div>
                                                                        </ContentTemplate>
                                                                        <Triggers>
                                                                            <asp:AsyncPostBackTrigger ControlID="btnAddressVerify" EventName="click" />
                                                                        </Triggers>
                                                                    </asp:UpdatePanel>

                                                                </div>
                                                                <div class="BeigeBoxFooter" style="border-top: 1px solid #E7CFAD;"></div>
                                                            </div>
                                                        </div>

                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <%--////////////////////////////////////////////////////////////////--%>
                                    </div>

                                    <div class="BeigeBoxFooter" style="text-align: center">
                                        <p id="addressFooter" style="text-align: center; font-size: .8em"></p>
                                    </div>
                                </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="btnAddressVerify" EventName="click" />
                                </Triggers>
                            </asp:UpdatePanel>
                        </div>
                    </div>

                    <!-- Phone Number /Email Address -->
                    <div class="w3-rest">
                        <!-- Phone Number -->
                        <div class="BeigeBoxContainer" style="margin: 0px 10px">
                            <div class="BeigeBoxHeader" style="padding: 12px 10px 3px 15px">
                                <span class="label">Phone Information</span>
                            </div>
                            <div class="BeigeBoxContent" style="margin-left: 10px">
                                <asp:UpdatePanel ID="upPhoneNum" runat="server" ChildrenAsTriggers="false" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <div id="divPhoneNumbers" style="clear: both;">
                                            <div id="divPhoneNumMsg" style="color: red; width: 50%;"></div>
                                            <div style="height: auto">
                                                <div id="phoneMessage"></div>
                                            </div>
                                            <div class="w3-row" style="margin-top: 0px; padding-top: 0px">
                                                <!-- Phone Number -->
                                                <div class="w3-half">
                                                    <div class="padding">
                                                        <asp:Label ID="lblPrimaryPhoneNumber" runat="server" CssClass="srtsLabel_medium" Text="Phone Number:" /><br />
                                                        <asp:TextBox ID="tbPrimaryPhoneNumber" runat="server" MaxLength="100" ValidationGroup="phone"
                                                            ToolTip="Continuation of patient Phone Number." CssClass="srtsTextBox_medium" Width="175px" />
                                                        <asp:CustomValidator ID="cvPrimaryPhone" runat="server" ControlToValidate="tbPrimaryPhoneNumber" ClientValidationFunction="ValidatePhone" ValidateEmptyText="true" />
                                                    </div>
                                                </div>
                                                <!-- Extension Number -->
                                                <div class="w3-half">
                                                    <div class="padding">
                                                        <asp:Label ID="lblPrimaryExtension" runat="server" CssClass="srtsLabel_medium" Text="Extension:" /><br />
                                                        <asp:TextBox ID="tbPrimaryExtension" runat="server" MaxLength="100" ToolTip="Enter patient extension" CssClass="srtsTextBox_medium" Width="150px" />
                                                        <asp:CustomValidator ID="cvPrimaryExtension" runat="server" ControlToValidate="tbPrimaryExtension" ClientValidationFunction="ValidateExtension" ValidateEmptyText="true" />
                                                    </div>
                                                </div>
                                            </div>

                                            <!-- Phone Number - Save Button -->
                                            <div class="padding" style="position: relative; top: 15px; padding-top: 0px; padding-bottom: 0px; text-align: right">
                                                <asp:Button ID="bSavePhoneNumber" runat="server" CssClass="srtsButton" Text="Save" OnClick="bSavePhoneNumber_Click" CausesValidation="false" Enabled="true" />
                                            </div>
                                        </div>
                                    </ContentTemplate>
                                    <Triggers>
                                        <asp:AsyncPostBackTrigger ControlID="bSavePhoneNumber" EventName="click" />
                                    </Triggers>
                                </asp:UpdatePanel>
                            </div>
                            <div class="BeigeBoxFooter"></div>
                        </div>

                        <!-- Email Address -->
                        <div class="BeigeBoxContainer" style="margin: 10px 10px 0px 10px">
                            <div class="BeigeBoxHeader" style="padding: 12px 10px 3px 15px">
                                <span class="label">Email Address</span>
                            </div>
                            <div class="BeigeBoxContent" style="margin-left: 10px">
                                <asp:UpdatePanel ID="upEmailAddress" runat="server" ChildrenAsTriggers="false" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <div id="divEmailAddresses" style="clear: both;">
                                            <div id="divEmailMsg" style="color: red; width: 90%;"></div>
                                            <div style="height: auto">
                                                <div id="emailMessage"></div>
                                            </div>
                                            <div class="padding">
                                                <asp:Label ID="lblPrimaryEmail" runat="server" Text="Email Address:" CssClass="srtsLabel_medium" /><br />
                                                <asp:TextBox ID="tbPrimaryEmail" runat="server" CssClass="srtsTextBox_medium" ToolTip="Enter the primary e-mail address" Width="350px" />
                                                <asp:CustomValidator ID="cvEmail" runat="server" ControlToValidate="tbPrimaryEmail" ClientValidationFunction="ValidateEmail" ValidateEmptyText="true" />
                                            </div>
                                            <!-- Email - Save Button -->
                                            <div class="padding" style="position: relative; top: 20px; padding-top: 0px; padding-bottom: 0px; text-align: right">
                                                <asp:Button ID="bSaveEmailAddress" runat="server" CssClass="srtsButton" CausesValidation="false" Text="Save" OnClick="bSaveEmailAddress_Click" Enabled="true" />
                                            </div>
                                        </div>
                                    </ContentTemplate>
                                    <Triggers>
                                        <asp:AsyncPostBackTrigger ControlID="bSaveEmailAddress" EventName="click" />
                                    </Triggers>
                                </asp:UpdatePanel>
                            </div>
                            <div class="BeigeBoxFooter"></div>
                        </div>
                    </div>
                </div>

                <!-- Individual Type / Site Assignment Information -->
                <div class="w3-row padding" style="margin-top: -25px">
                    <asp:UpdatePanel ID="upnlIndTypesAndAssignSites" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="False">
                        <ContentTemplate>
                            <div id="divIndType" runat="server" clientidmode="Static">
                                <div class="BeigeBoxContainer" style="margin: 10px 10px 0px 10px">
                                    <div class="BeigeBoxHeader" style="padding: 12px 10px 5px 15px"></div>
                                    <!-- Individual Type Information -->
                                    <div class="w3-half">
                                        <div class="BeigeBoxHeader" style="position: relative; top: -44px; padding: 12px 10px 3px 15px">
                                            <span class="label">Individual Type Information</span>
                                        </div>
                                        <div class="BeigeBoxContent" style="margin-top: -40px; margin-left: 10px; height: 150px">
                                            <div id="Div1" runat="server" clientidmode="Static">
                                                <div id="IndTypeErrorMsg" style="height: 10px; color: red;"></div>
                                                <div style="height: 20px; margin-top: -5px;">
                                                    <div id="indTypesMessage"></div>
                                                </div>
                                                <div id="IndTypesCBs" style="margin-top: 10px; margin-left: 20px;">
                                                    <asp:Label ID="lblIndTypes" runat="server" Text="*Select individual type(s):"
                                                        Width="175px" CssClass="srtsLabel_medium" ControlToValidate="lblIndTypes" /><br />
                                                    <asp:CheckBox ID="cbPatient" runat="server" ClientIDMode="Static" Enabled="False" />
                                                    <asp:Label ID="lblPatient" runat="server" Text="Patient" Style="padding-left: 5px" />
                                                    <asp:CheckBox ID="cbProvider" runat="server" ClientIDMode="Static" Style="padding-left: 15px" />
                                                    <asp:Label ID="lblProvider" runat="server" Text="Provider" Style="padding-left: 5px" />
                                                    <asp:CheckBox ID="cbTechnician" runat="server" ClientIDMode="Static" Style="padding-left: 15px" />
                                                    <asp:Label ID="lblTechnician" runat="server" Text="Technician" Style="padding-left: 5px" />
                                                    <asp:CheckBox ID="cbAdministrator" runat="server" ClientIDMode="Static" Style="padding-left: 15px" />
                                                    <asp:Label ID="lblOther" runat="server" Text="Other (e.g., Admin)" Style="padding-left: 5px" /><br />
                                                </div>
                                                <!-- Individual Types - Save Button -->
                                                <div class="padding" style="position: relative; top: 30px; padding-bottom: 0px; text-align: right">
                                                    <asp:Button ID="btnUpdateIndTypes" runat="server" CssClass="srtsButton" Text="Save"
                                                        OnClick="btnUpdateIndTypes_Click" CausesValidation="False" />
                                                </div>
                                            </div>
                                        </div>
                                        <div class="BeigeBoxFooter"></div>
                                    </div>

                                    <!-- Site Assignment Information -->
                                    <div class="w3-half">
                                        <div class="BeigeBoxContent" style="margin-left: 10px; height: 154px">
                                            <div id="divAssignedSites" runat="server" clientidmode="Static">
                                                <div class="BeigeBoxHeader" style="position: relative; top: -44px; padding: 12px 10px 3px 15px">
                                                    <span class="label">Provider Assignments</span>
                                                    <p style="margin-left: 150px; margin-top: -28px; margin-right: 10px; font-size: 12px; color: #794545">
                                                        (Select where a provider will show up in the Exam/Rx
                                                <br />
                                                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;provider lists.)
                                                    </p>
                                                </div>
                                                <div class="BeigeBoxContent" style="margin-top: -40px; margin-left: 10px; height: 150px; border-left: 1px solid #E7CFAD">
                                                    <div id="AssignedSitesErrorMsg" style="height: 10px; color: red;"></div>
                                                    <div style="height: 20px; margin-top: -5px;">
                                                        <div id="individualSitesMessage"></div>
                                                    </div>
                                                    <div style="width: 47%; float: left; text-align: right;">
                                                        <div style="padding-right: 10px;">
                                                            <div style="display: block; margin: 10px 0px 3px 0px; text-align: left; padding-left: 12px;">
                                                                <span class="srtsLabel_medium_text">Available Sites:</span>
                                                            </div>
                                                            <asp:ListBox ID="lboxAvailSites" runat="server" Rows="4" Width="95%" Font-Size="0.9em"></asp:ListBox>
                                                        </div>
                                                    </div>
                                                    <div style="width: 6%; float: left; text-align: center; padding-top: 30px;">
                                                        <asp:ImageButton ID="btnAddSite" runat="server" ImageUrl="~/Styles/images/Arrow_blue_right.gif" Width="25px"
                                                            CausesValidation="false" OnClick="btnAddSite_Click" />
                                                        <asp:ImageButton ID="btnRemSite" runat="server" ImageUrl="~/Styles/images/Arrow_blue_left.gif" Width="25px"
                                                            CausesValidation="false" OnClick="btnRemSite_Click" />
                                                    </div>
                                                    <div style="width: 47%; float: right; text-align: left;">
                                                        <div style="padding-left: 10px;">
                                                            <div style="display: block; margin: 10px 0px 3px 0px;">
                                                                <span class="srtsLabel_medium_text">Provider Assigned Site(s):</span>
                                                            </div>
                                                            <asp:ListBox ID="lboxAssignedSites" runat="server" Rows="4" Width="95%" Font-Size="0.9em"></asp:ListBox>
                                                        </div>
                                                    </div>
                                                    <!-- Assigned Sites Save Button -->
                                                    <div class="padding" style="position: relative; top: 5px; padding-bottom: 0px; text-align: right">
                                                        <asp:Button ID="btnUpdateIndivSites" runat="server" CssClass="srtsButton" Text="Save" CausesValidation="False" OnClick="btnUpdateIndivSites_Click" />
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="BeigeBoxFooter"></div>
                                    </div>
                                </div>
                            </div>
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="btnUpdateIndTypes" EventName="Click" />
                            <asp:AsyncPostBackTrigger ControlID="btnUpdateIndivSites" EventName="Click" />
                            <asp:AsyncPostBackTrigger ControlID="btnAddSite" EventName="Click" />
                            <asp:AsyncPostBackTrigger ControlID="btnRemSite" EventName="Click" />
                        </Triggers>
                    </asp:UpdatePanel>
                </div>

                <asp:HiddenField ID="hdfState" runat="server" ClientIDMode="Static" />
                <asp:HiddenField ID="hdfCity" runat="server" ClientIDMode="Static" />
                <asp:HiddenField ID="hdfIsValid" runat="server" ClientIDMode="Static" />
                <asp:HiddenField ID="hdfDateVerified" runat="server" ClientIDMode="Static" />
                <asp:HiddenField ID="hdfVerifiedExpiry" runat="server" ClientIDMode="Static" />

                <div id="divRefreshDialog" style="display: none; width: 95%;">
                    <div id="divMessage" style="margin: 20px 10px; text-align: left;" class="headerBurgandy">
                        Data Refresh Message:<br />
                        If the data refreshed from DMDC is not correct for the person then please contact the SRTS Web help desk at 1-800-600-9332.
                    </div>
                    <div style="text-align: center;">
                        <hr style="width: 80%;" />
                    </div>
                    <div style="float: left; margin-left: 20px;">
                        <table id="tblRefresh" runat="server">
                            <tr style="font-weight: bold; height: 30px;">
                                <th>&nbsp;&nbsp;&nbsp;</th>
                                <th>
                                    <u>
                                        <asp:Label ID="lblOriginal" runat="server" Text="Original" CssClass="srtsLabel_medium_text"></asp:Label>
                                    </u>
                                </th>
                            </tr>
                            <tr>
                                <td style="text-align: right; font-weight: bold;">
                                    <asp:Label ID="Label1" runat="server" Text="Last Name: " CssClass="srtsLabel_medium_text"></asp:Label></td>
                                <td style="font-weight: normal;">
                                    <asp:Label ID="lblOrigLastName" runat="server" CssClass="srtsLabel_medium"></asp:Label></td>
                            </tr>
                            <tr>
                                <td style="text-align: right; font-weight: bold;">
                                    <asp:Label ID="Label2" runat="server" Text="First Name: " CssClass="srtsLabel_medium_text"></asp:Label></td>
                                <td style="font-weight: normal;">
                                    <asp:Label ID="lblOrigFirstName" runat="server" CssClass="srtsLabel_medium"></asp:Label></td>
                            </tr>
                            <tr>
                                <td style="text-align: right; font-weight: bold;">
                                    <asp:Label ID="Label3" runat="server" Text="Middle Name: " CssClass="srtsLabel_medium_text"></asp:Label></td>
                                <td style="font-weight: normal;">
                                    <asp:Label ID="lblOrigMiddleName" runat="server" CssClass="srtsLabel_medium"></asp:Label></td>
                            </tr>
                            <tr>
                                <td style="text-align: right; font-weight: bold;">
                                    <asp:Label ID="Label5" runat="server" Text="Date Of Birth: " CssClass="srtsLabel_medium_text"></asp:Label></td>
                                <td style="font-weight: normal;">
                                    <asp:Label ID="lblOrigDob" runat="server" CssClass="srtsLabel_medium"></asp:Label></td>
                            </tr>
                            <tr>
                                <td style="text-align: right; font-weight: bold;">
                                    <asp:Label ID="Label6" runat="server" Text="BOS: " CssClass="srtsLabel_medium_text"></asp:Label></td>
                                <td style="font-weight: normal;">
                                    <asp:Label ID="lblOrigBos" runat="server" CssClass="srtsLabel_medium"></asp:Label></td>
                            </tr>
                            <tr>
                                <td style="text-align: right; font-weight: bold;">
                                    <asp:Label ID="Label7" runat="server" Text="Status: " CssClass="srtsLabel_medium_text"></asp:Label></td>
                                <td style="font-weight: normal;">
                                    <asp:Label ID="lblOrigStatus" runat="server" CssClass="srtsLabel_medium"></asp:Label></td>
                            </tr>
                            <tr>
                                <td style="text-align: right; font-weight: bold;">
                                    <asp:Label ID="Label8" runat="server" Text="Grade: " CssClass="srtsLabel_medium_text"></asp:Label></td>
                                <td style="font-weight: normal;">
                                    <asp:Label ID="lblOrigGrade" runat="server" CssClass="srtsLabel_medium"></asp:Label></td>
                            </tr>
                            <tr>
                                <td style="text-align: right; font-weight: bold;">
                                    <asp:Label ID="Label9" runat="server" Text="SSN: " CssClass="srtsLabel_medium_text"></asp:Label></td>
                                <td style="font-weight: normal;">
                                    <asp:Label ID="lblOrigDss" runat="server" CssClass="srtsLabel_medium"></asp:Label></td>
                            </tr>
                            <tr>
                                <td style="text-align: right; font-weight: bold;">
                                    <asp:Label ID="Label10" runat="server" Text="DOD ID: " CssClass="srtsLabel_medium_text"></asp:Label></td>
                                <td style="font-weight: normal;">
                                    <asp:Label ID="lblOrigDin" runat="server" CssClass="srtsLabel_medium"></asp:Label></td>
                            </tr>
                            <tr>
                                <td style="text-align: right; font-weight: bold;">
                                    <asp:Label ID="Label11" runat="server" Text="Address 1: " CssClass="srtsLabel_medium_text"></asp:Label></td>
                                <td style="font-weight: normal;">
                                    <asp:Label ID="lblOrigAddress1" runat="server" CssClass="srtsLabel_medium"></asp:Label></td>
                            </tr>
                            <tr>
                                <td style="text-align: right; font-weight: bold;">
                                    <asp:Label ID="Label12" runat="server" Text="Address 2: " CssClass="srtsLabel_medium_text"></asp:Label></td>
                                <td style="font-weight: normal;">
                                    <asp:Label ID="lblOrigAddress2" runat="server" CssClass="srtsLabel_medium"></asp:Label></td>
                            </tr>
                            <tr>
                                <td style="text-align: right; font-weight: bold;">
                                    <asp:Label ID="Label13" runat="server" Text="City: " CssClass="srtsLabel_medium_text"></asp:Label></td>
                                <td style="font-weight: normal;">
                                    <asp:Label ID="lblOrigCity" runat="server" CssClass="srtsLabel_medium"></asp:Label></td>
                            </tr>
                            <tr>
                                <td style="text-align: right; font-weight: bold;">
                                    <asp:Label ID="Label14" runat="server" Text="State: " CssClass="srtsLabel_medium_text"></asp:Label></td>
                                <td style="font-weight: normal;">
                                    <asp:Label ID="lblOrigState" runat="server" CssClass="srtsLabel_medium"></asp:Label></td>
                            </tr>
                            <tr>
                                <td style="text-align: right; font-weight: bold;">
                                    <asp:Label ID="Label15" runat="server" Text="Country: " CssClass="srtsLabel_medium_text"></asp:Label></td>
                                <td style="font-weight: normal;">
                                    <asp:Label ID="lblOrigCountry" runat="server" CssClass="srtsLabel_medium"></asp:Label></td>
                            </tr>
                            <tr>
                                <td style="text-align: right; font-weight: bold;">
                                    <asp:Label ID="Label16" runat="server" Text="Zip: " CssClass="srtsLabel_medium_text"></asp:Label></td>
                                <td style="font-weight: normal;">
                                    <asp:Label ID="lblOrigZip" runat="server" CssClass="srtsLabel_medium"></asp:Label></td>
                            </tr>
                            <tr>
                                <td style="text-align: right; font-weight: bold;">
                                    <asp:Label ID="Label18" runat="server" Text="Phone: " CssClass="srtsLabel_medium_text"></asp:Label></td>
                                <td style="font-weight: normal;">
                                    <asp:Label ID="lblOrigPhone" runat="server" CssClass="srtsLabel_medium"></asp:Label></td>
                            </tr>
                            <tr>
                                <td style="text-align: right; font-weight: bold;">
                                    <asp:Label ID="Label19" runat="server" Text="Email Address: " CssClass="srtsLabel_medium_text"></asp:Label></td>
                                <td style="font-weight: normal;">
                                    <asp:Label ID="lblOrigEmail" runat="server" CssClass="srtsLabel_medium"></asp:Label></td>
                            </tr>
                            <tr>
                                <td colspan="2"></td>
                            </tr>
                        </table>
                    </div>
                    <div id="divDssRefresh" runat="server" style="float: left; margin-left: 5px;">
                        <table>
                            <tr style="font-weight: bold; height: 30px;">
                                <th><u>
                                    <asp:Label ID="lblRefresh1" runat="server" Text="SSN Refresh" CssClass="srtsLabel_medium_text"></asp:Label></u></th>
                            </tr>
                            <tr>
                                <td style="font-weight: normal;">
                                    <asp:Label ID="lblDssLastName" runat="server" CssClass="srtsLabel_medium"></asp:Label></td>
                            </tr>
                            <tr>
                                <td style="font-weight: normal;">
                                    <asp:Label ID="lblDssFirstName" runat="server" CssClass="srtsLabel_medium"></asp:Label></td>
                            </tr>
                            <tr>
                                <td style="font-weight: normal;">
                                    <asp:Label ID="lblDssMiddleName" runat="server" CssClass="srtsLabel_medium"></asp:Label></td>
                            </tr>
                            <tr>
                                <td style="font-weight: normal;">
                                    <asp:Label ID="lblDssDob" runat="server" CssClass="srtsLabel_medium"></asp:Label></td>
                            </tr>
                            <tr>
                                <td style="font-weight: normal;">
                                    <asp:Label ID="lblDssBos" runat="server" CssClass="srtsLabel_medium"></asp:Label></td>
                            </tr>
                            <tr>
                                <td style="font-weight: normal;">
                                    <asp:Label ID="lblDssStatus" runat="server" CssClass="srtsLabel_medium"></asp:Label></td>
                            </tr>
                            <tr>
                                <td style="font-weight: normal;">
                                    <asp:Label ID="lblDssGrade" runat="server" CssClass="srtsLabel_medium"></asp:Label></td>
                            </tr>
                            <tr>
                                <td style="font-weight: normal;">
                                    <asp:Label ID="lblDssDss" runat="server" CssClass="srtsLabel_medium"></asp:Label></td>
                            </tr>
                            <tr>
                                <td style="font-weight: normal;">
                                    <asp:Label ID="lblDssDin" runat="server" CssClass="srtsLabel_medium"></asp:Label></td>
                            </tr>
                            <tr>
                                <td style="font-weight: normal;">
                                    <asp:Label ID="lblDssAddress1" runat="server" CssClass="srtsLabel_medium"></asp:Label></td>
                            </tr>
                            <tr>
                                <td style="font-weight: normal;">
                                    <asp:Label ID="lblDssAddress2" runat="server" CssClass="srtsLabel_medium"></asp:Label></td>
                            </tr>
                            <tr>
                                <td style="font-weight: normal;">
                                    <asp:Label ID="lblDssCity" runat="server" CssClass="srtsLabel_medium"></asp:Label></td>
                            </tr>
                            <tr>
                                <td style="font-weight: normal;">
                                    <asp:Label ID="lblDssState" runat="server" CssClass="srtsLabel_medium"></asp:Label></td>
                            </tr>
                            <tr>
                                <td style="font-weight: normal;">
                                    <asp:Label ID="lblDssCountry" runat="server" CssClass="srtsLabel_medium"></asp:Label></td>
                            </tr>
                            <tr>
                                <td style="font-weight: normal;">
                                    <asp:Label ID="lblDssZip" runat="server" CssClass="srtsLabel_medium"></asp:Label></td>
                            </tr>
                            <tr>
                                <td style="font-weight: normal;">
                                    <asp:Label ID="lblDssPhone" runat="server" CssClass="srtsLabel_medium"></asp:Label></td>
                            </tr>
                            <tr>
                                <td style="font-weight: normal;">
                                    <asp:Label ID="lblDssEmail" runat="server" CssClass="srtsLabel_medium"></asp:Label></td>
                            </tr>
                            <tr>
                                <td style="font-weight: normal;">
                                    <asp:RadioButton ID="rbDss" runat="server" GroupName="refresh" ClientIDMode="Static" Checked="true" /></td>
                            </tr>
                        </table>
                    </div>
                    <div id="divDinRefresh" runat="server" style="float: left; margin-left: 5px;">
                        <table>
                            <tr style="font-weight: bold; height: 30px;">
                                <th>
                                    <u>
                                        <asp:Label ID="lblRefresh2" runat="server" Text="DOD ID Refresh" CssClass="srtsLabel_medium_text"></asp:Label></u>
                                </th>
                            </tr>
                            <tr>
                                <td style="font-weight: normal;">
                                    <asp:Label ID="lblDinLastName" runat="server" CssClass="srtsLabel_medium"></asp:Label></td>
                            </tr>
                            <tr>
                                <td style="font-weight: normal;">
                                    <asp:Label ID="lblDinFirstName" runat="server" CssClass="srtsLabel_medium"></asp:Label></td>
                            </tr>
                            <tr>
                                <td style="font-weight: normal;">
                                    <asp:Label ID="lblDinMiddleName" runat="server" CssClass="srtsLabel_medium"></asp:Label></td>
                            </tr>
                            <tr>
                                <td style="font-weight: normal;">
                                    <asp:Label ID="lblDinDob" runat="server" CssClass="srtsLabel_medium"></asp:Label></td>
                            </tr>
                            <tr>
                                <td style="font-weight: normal;">
                                    <asp:Label ID="lblDinBos" runat="server" CssClass="srtsLabel_medium"></asp:Label></td>
                            </tr>
                            <tr>
                                <td style="font-weight: normal;">
                                    <asp:Label ID="lblDinStatus" runat="server" CssClass="srtsLabel_medium"></asp:Label></td>
                            </tr>
                            <tr>
                                <td style="font-weight: normal;">
                                    <asp:Label ID="lblDinGrade" runat="server" CssClass="srtsLabel_medium"></asp:Label></td>
                            </tr>
                            <tr>
                                <td style="font-weight: normal;">
                                    <asp:Label ID="lblDinDss" runat="server" CssClass="srtsLabel_medium"></asp:Label></td>
                            </tr>
                            <tr>
                                <td style="font-weight: normal;">
                                    <asp:Label ID="lblDinDin" runat="server" CssClass="srtsLabel_medium"></asp:Label></td>
                            </tr>
                            <tr>
                                <td style="font-weight: normal;">
                                    <asp:Label ID="lblDinAddress1" runat="server" CssClass="srtsLabel_medium"></asp:Label></td>
                            </tr>
                            <tr>
                                <td style="font-weight: normal;">
                                    <asp:Label ID="lblDinAddress2" runat="server" CssClass="srtsLabel_medium"></asp:Label></td>
                            </tr>
                            <tr>
                                <td style="font-weight: normal;">
                                    <asp:Label ID="lblDinCity" runat="server" CssClass="srtsLabel_medium"></asp:Label></td>
                            </tr>
                            <tr>
                                <td style="font-weight: normal;">
                                    <asp:Label ID="lblDinState" runat="server" CssClass="srtsLabel_medium"></asp:Label></td>
                            </tr>
                            <tr>
                                <td style="font-weight: normal;">
                                    <asp:Label ID="lblDinCountry" runat="server" CssClass="srtsLabel_medium"></asp:Label></td>
                            </tr>
                            <tr>
                                <td style="font-weight: normal;">
                                    <asp:Label ID="lblDinZip" runat="server" CssClass="srtsLabel_medium"></asp:Label></td>
                            </tr>
                            <tr>
                                <td style="font-weight: normal;">
                                    <asp:Label ID="lblDinPhone" runat="server" CssClass="srtsLabel_medium"></asp:Label></td>
                            </tr>
                            <tr>
                                <td style="font-weight: normal;">
                                    <asp:Label ID="lblDinEmail" runat="server" CssClass="srtsLabel_medium"></asp:Label></td>
                            </tr>
                            <tr>
                                <td style="font-weight: normal;">
                                    <asp:RadioButton ID="rbDin" runat="server" GroupName="refresh" ClientIDMode="Static" /></td>
                            </tr>
                        </table>
                    </div>
                    <div style="text-align: center;">
                        <hr style="clear: both; width: 80%;" />
                    </div>
                    <div style="clear: both; margin: 15px 10px 0px 0px; float: right;">
                        <asp:Button ID="bSaveRefresh" runat="server" CssClass="srtsButton" OnClick="bSaveRefresh_Click" Text="Save" CausesValidation="false" />
                        <input type="button" id="bExitRefresh" class="srtsButton" onclick="$('#divRefreshDialog').dialog('close');" value="Cancel" />
                    </div>
                </div>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="lnbPatientNameHeader" EventName="Click" />
            </Triggers>
        </asp:UpdatePanel>
    </div>
</asp:Content>
