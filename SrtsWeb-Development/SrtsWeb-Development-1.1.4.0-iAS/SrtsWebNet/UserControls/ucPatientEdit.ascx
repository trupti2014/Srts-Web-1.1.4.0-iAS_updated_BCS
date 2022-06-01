<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ucPatientEdit.ascx.cs" Inherits="SrtsWeb.UserControls.ucPatientEdit" EnableViewState="true" %>

<div id="divPersonalInfoEdit" runat="server" class="padding" style="margin-top: -20px" clientidmode="Static">
    <div class="patientnameheader">
        <asp:Literal ID="litPatientNameHeader" runat="server"></asp:Literal>
    </div>
    <div id="divEditPerson" style="padding: 10px 0px; margin-top: 10px; width: 900px; border-top: 1px solid #E7CFAD; border-bottom: 1px solid #E7CFAD">
        <div class="tabContent">
            <h1 style="font-size: 20px; line-height: 30px; color: #7B7984; padding-bottom: 10px; text-align: left; border-bottom: solid 1px #E7CFAD">Edit Information</h1>

            <div id="divEditMsg" style="color: red; width: 90%; display: none;"></div>

            <div style="width: 100%; margin: 15px 0px">
                <asp:ValidationSummary ID="vsErrors" CssClass="validatorSummary" runat="server" />
                <br />
                <div>
                    <asp:Label ID="lblSiteCode" runat="server" Text="*Facility (site)" CssClass="srtsLabel_medium" />
                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                    <asp:DropDownList ID="ddlSite" runat="server" TabIndex="1" AppendDataBoundItems="true" CssClass="srtsDropDown_medium" Width="305px">
                        <Items>
                            <asp:ListItem Text="Select Site Code..." Value="X" />
                        </Items>
                    </asp:DropDownList>
                    <ajaxToolkit:ListSearchExtender runat="server" TargetControlID="ddlSite" ID="LSE_ddlSite" Enabled="True" PromptText="Type to search" QueryTimeout="2" PromptCssClass="listSearchPromptOverlay" QueryPattern="Contains"></ajaxToolkit:ListSearchExtender>
                </div>
            </div>
            <div style="float: right; width: 45%; position: relative; top: -65px;">
                <br />
                <br />
                <asp:UpdatePanel ID="upActive" runat="server" ChildrenAsTriggers="false" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:Label ID="lblBranch" runat="server" Text="*Branch" Width="100px" CssClass="srtsLabel_medium" />
                        <asp:DropDownList ID="ddlBOS" runat="server" AppendDataBoundItems="true" ToolTip="Select the individuals' branch."
                            DataTextField="bosText" DataValueField="bosValue" TabIndex="8" CssClass="srtsDropDown_medium" AutoPostBack="true" OnSelectedIndexChanged="ddlBOS_SelectedIndexChanged">
                        </asp:DropDownList>
                        <asp:RequiredFieldValidator ID="rfvBOS" runat="server" ControlToValidate="ddlBOS"
                            ErrorMessage="Branch Of Service is a required selection" Display="None"></asp:RequiredFieldValidator>
                        <br />
                        <br />
                        <asp:Label ID="lblStatus" runat="server" Text="*Status" Width="100px" CssClass="srtsLabel_medium" />
                        <asp:DropDownList ID="ddlStatusType" runat="server" DataTextField="statusText"
                            DataValueField="statusValue" AutoPostBack="true" TabIndex="9" OnSelectedIndexChanged="ddlStatusType_SelectedIndexChanged"
                            CssClass="srtsDropDown_medium">
                        </asp:DropDownList>
                        <asp:RequiredFieldValidator ID="rfvStatus" runat="server" ControlToValidate="ddlStatusType"
                            ErrorMessage="Status is a required selection" Display="None"></asp:RequiredFieldValidator>
                        <br />
                        <br />
                        <asp:Label ID="lblRank" runat="server" Text="*Grade" Width="100px" CssClass="srtsLabel_medium" />
                        <asp:DropDownList ID="ddlRank" runat="server" AppendDataBoundItems="true" ToolTip="Select the individuals' grade."
                            DataTextField="rankText" DataValueField="rankValue" TabIndex="10" CssClass="srtsDropDown_medium">
                        </asp:DropDownList>
                        <asp:RequiredFieldValidator ID="rfvRank" runat="server" ControlToValidate="ddlRank"
                            InitialValue="0" ErrorMessage="Grade is a required selection." Display="None"></asp:RequiredFieldValidator>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ddlBOS" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="ddlStatusType" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
                <div style="margin-top: 15px;">
                    <asp:Label ID="lblTheater" runat="server" Text="Theater Zip Code" Width="107px" CssClass="srtsLabel_medium" />
                    <asp:DropDownList ID="ddlTheaterLocationCodes" runat="server" DataTextField="TheaterCode"
                        DataValueField="TheaterCode" AppendDataBoundItems="true" TabIndex="11" CssClass="srtsDropDown_medium"
                        Width="255px">
                        <asp:ListItem Text="Select If Needed" Value=""></asp:ListItem>
                    </asp:DropDownList>
                </div>
                <div style="margin-top: 15px">
                    <asp:Label ID="lblActiveDutyExtend" runat="server" Text="Extended Active Duty Expiration Date" Width="110px" CssClass="srtsLabel_medium" />
                    <asp:TextBox ID="tbEADExpires" runat="server" Width="100px" TabIndex="12"></asp:TextBox>
                    <asp:Image runat="server" ID="calImage" AlternateText="Calendar" ImageUrl="~/Styles/images/Calendar_scheduleHS.png" />
                    <ajaxToolkit:CalendarExtender ID="ceEAD" runat="server" TargetControlID="tbEADExpires" Format="MM/dd/yyyy" PopupButtonID="calImage"></ajaxToolkit:CalendarExtender>
                    <asp:CustomValidator ID="cvEad" runat="server" ClientValidationFunction="ValidateDate" ControlToValidate="tbEADExpires" ValidateEmptyText="True">
                    </asp:CustomValidator>
                </div>
            </div>
            <div style="float: left; width: 45%;">
                <div style="margin-top: 10px;">
                    <asp:Label ID="lblLastName" runat="server" Text="*Last Name" Width="100px" CssClass="srtsLabel_medium" />
                    <asp:TextBox ID="txtLastName" runat="server" MaxLength="75" ToolTip="Enter patient last name." CssClass="srtsTextBox_medium" TabIndex="2" ClientIDMode="Static" />
                    <asp:CustomValidator ID="cvLastName" runat="server" ClientValidationFunction="ValidateName" ControlToValidate="txtLastName" ValidateEmptyText="True" />
                </div>

                <div style="margin-top: 10px;">
                    <asp:Label ID="lblFirstName" runat="server" Text="*First Name" Width="100px" CssClass="srtsLabel_medium" />
                    <asp:TextBox ID="tbFirstName" ClientIDMode="Static" runat="server" MaxLength="75" ToolTip="Enter patient first name." CssClass="srtsTextBox_medium" TabIndex="3" />
                    <asp:CustomValidator ID="cvFirstName" runat="server" ClientValidationFunction="ValidateName" ControlToValidate="tbFirstName" ValidateEmptyText="True" />
                </div>

                <div style="margin-top: 10px;">
                    <asp:Label ID="lblMiddleName" runat="server" Text="Middle Name" Width="100px" CssClass="srtsLabel_medium" />
                    <asp:TextBox ID="tbMiddleName" runat="server" MaxLength="75" ToolTip="Enter patient middle name or initial." CssClass="srtsTextBox_medium" TabIndex="4" ClientIDMode="Static" />
                    <asp:CustomValidator ID="cvMiddleName" runat="server" ClientValidationFunction="ValidateName" ControlToValidate="tbMiddleName" ValidateEmptyText="True" />
                </div>

                <div style="margin-top: 10px;">
                    <asp:Label ID="lblDOB" runat="server" Text="Date of Birth(MM/DD/YYYY)" Width="100px" CssClass="srtsLabel_medium" />
                    <asp:TextBox ID="tbDOB" runat="server" TabIndex="5" Width="100px" />
                    <asp:Image runat="server" ID="calImage1" AlternateText="Calendar" ImageUrl="~/Styles/images/Calendar_scheduleHS.png" />
                    <ajaxToolkit:CalendarExtender ID="ceDOB" runat="server" CssClass="calendar_addpatient"
                        PopupPosition="BottomLeft" TargetControlID="tbDOB" Format="MM/dd/yyyy" PopupButtonID="calImage1">
                    </ajaxToolkit:CalendarExtender>
                    <asp:CustomValidator ID="cvDOB" runat="server" ClientValidationFunction="ValidateDate" ControlToValidate="tbDOB" ValidateEmptyText="True"></asp:CustomValidator>
                </div>

                <div style="margin: 10px 0px 10px 0px;">
                    <div style="float: left; width: 100px;">
                        <asp:Label ID="lblSex" runat="server" Text="Gender" Width="100px" CssClass="srtsLabel_medium" AssociatedControlID="rblGender" />
                    </div>
                    <div style="float: left; margin-top: -3px;">
                        <asp:RadioButtonList ID="rblGender" runat="server" TabIndex="6" RepeatDirection="Horizontal" ToolTip="Select patient gender.">
                            <asp:ListItem Text="Male" Value="M" />
                            <asp:ListItem Text="Female" Value="F" />
                        </asp:RadioButtonList>
                    </div>
                </div>

                <div style="clear: both;">
                    <asp:Label ID="lblIDType" runat="server" CssClass="srtsLabel_medium" Text="Individual Type" Width="100px" />
                    <asp:TextBox ID="tbIndividualType" runat="server" Text="PATIENT" TabIndex="7" CssClass="srtsTextBox_medium" Enabled="false"></asp:TextBox>
                </div>
            </div>

            <div style="clear: both; width: 100%; margin-top: 8px; height: 100px">
                <asp:Label ID="lblComments" runat="server" Text="Comments" Width="100px" CssClass="srtsLabel_medium" />

                <asp:TextBox ID="tbComments" runat="server" onKeyDown="return textboxMaxCommentSize(this, 256, event, getlblRemainingID(), gettbCommentID())"
                    ToolTip="Enter any comments, valid characters include + - . , ! ? # ' / ( )"
                    Rows="5" TextMode="MultiLine" Width="700px" TabIndex="13" CssClass="srtsTextBox_medium"
                    Height="70px">
                </asp:TextBox>
                <asp:CustomValidator ID="cvComment"
                    runat="server"
                    ErrorMessage="Invalid character(s) in Comment"
                    ClientValidationFunction="textboxCommentValidation"
                    Text="*"
                    ControlToValidate="tbComments"
                    ValidateEmptyText="True"
                    CssClass="requestValidator">
                </asp:CustomValidator>
                <div style="text-align: center;">
                    <asp:Label ID="lblRemaining" runat="server" CssClass="srtsLabel_medium"></asp:Label>
                </div>
            </div>
        </div>
        <div id="divButtonsBottom" style="margin: 13px 70px 0px 0px; text-align: right">
            <asp:Button ID="btnUpdateRecord" runat="server" CssClass="srtsButton" TabIndex="14" Text="Save" ToolTip="Update patient information" OnClick="btnUpdate_Click" CausesValidation="false" />
            <asp:Button ID="btnCancel" runat="server" CssClass="srtsButton" TabIndex="15" ToolTip="Cancel current changes and return to patient information" CausesValidation="false" OnClick="btnCancel_Click" Text="Cancel" />
        </div>
    </div>
</div>
