<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ucIndividualEdit.ascx.cs" Inherits="SrtsWeb.UserControls.ucIndividualEdit" %>

<div id="divPersonalInfoEdit" runat="server" class="padding" style="margin-top: -20px" clientidmode="Static">
    <div class="patientnameheader">
        <asp:Literal ID="litPatientNameHeader" runat="server"></asp:Literal>
    </div>
    <div style="padding: 10px 0px; margin-top: 10px; width: 900px; border-top: 1px solid #E7CFAD; border-bottom: 1px solid #E7CFAD">
        <div id="divButtonsTop" style="margin: 0px 35px -80px 0px; text-align: right">
            <asp:Button ID="btnSave" runat="server" CssClass="srtsButton" ToolTip="Update individual information"
                OnClick="btnUpdate_Click" Text="Save" />
            <asp:Button ID="btnCancelEdit" runat="server" CssClass="srtsButton" ToolTip="Cancel changes and return to individual information"
                CausesValidation="false" OnClick="btnCancel_Click" Text="Cancel" />
        </div>
        <div class="tabContent">
            <h1 style="font-size: 20px; line-height: 30px; color: #7B7984; padding-bottom: 10px; text-align: left; border-bottom: solid 1px #E7CFAD">Edit Individual Information</h1>
            <span class="colorRed">
                <asp:Literal ID="litMessage" runat="server" Visible="false"></asp:Literal></span>
            <div style="width: 100%; margin: 15px 0px">
                <asp:ValidationSummary ID="vsErrors" runat="server" CssClass="validatorSummary" />
                <br />
                <div>
                    <asp:Label ID="lblSiteCode" runat="server" Text="*Facility (site)" CssClass="srtsLabel_medium" />
                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<asp:DropDownList ID="ddlSite" runat="server" AppendDataBoundItems="true" CssClass="srtsDropDown_medium"
                        Width="715px">
                        <Items>
                            <asp:ListItem Text="Select Site Code..." Value="X" />
                        </Items>
                    </asp:DropDownList>
                </div>
            </div>
            <div style="float: right; width: 45%">
                <asp:UpdatePanel ID="upDemographic" runat="server" UpdateMode="Always">
                    <ContentTemplate>
                        <asp:Label ID="lblBranch" runat="server" Text="*Branch" Width="100px" CssClass="srtsLabel_medium" />
                        <asp:DropDownList ID="ddlBOS" runat="server" AppendDataBoundItems="true" ToolTip="Select the individuals' branch." AutoPostBack="true"
                            DataTextField="bosText" DataValueField="bosValue" TabIndex="6" CssClass="srtsDropDown_medium" OnSelectedIndexChanged="ddlBOS_SelectedIndexChanged">
                            <Items>
                                <asp:ListItem Text="Select Branch Of Service..." Value="X" />
                            </Items>
                        </asp:DropDownList>
                        <asp:RequiredFieldValidator ID="rfvBOS" runat="server" ControlToValidate="ddlBOS"
                            InitialValue="X" ErrorMessage="Branch Of Service is a required selection" Display="None"></asp:RequiredFieldValidator>
                        <br />
                        <br />
                        <asp:Label ID="lblStatus" runat="server" Text="*Status" Width="100px" CssClass="srtsLabel_medium" />
                        <asp:DropDownList ID="ddlStatusType" runat="server" AppendDataBoundItems="true" DataTextField="statusText"
                            DataValueField="statusValue" AutoPostBack="true" TabIndex="7" OnSelectedIndexChanged="ddlStatusType_SelectedIndexChanged"
                            CssClass="srtsDropDown_medium">
                            <Items>
                                <asp:ListItem Text="Select Status..." Value="X" />
                            </Items>
                        </asp:DropDownList>
                        <asp:RequiredFieldValidator ID="rfvStatus" runat="server" ControlToValidate="ddlStatusType"
                            InitialValue="X" ErrorMessage="Status is a required selection" Display="None"></asp:RequiredFieldValidator>
                        <br />
                        <br />
                        <asp:Label ID="lblRank" runat="server" Text="*Grade" Width="100px" CssClass="srtsLabel_medium" />
                        <asp:DropDownList ID="ddlRank" runat="server" AppendDataBoundItems="true" ToolTip="Select the individuals' grade." AutoPostBack="true"
                            DataTextField="rankText" DataValueField="rankValue" TabIndex="8" CssClass="srtsDropDown_medium" OnSelectedIndexChanged="ddlRank_SelectedIndexChanged">
                            <Items>
                                <asp:ListItem Text="Select Rank..." Value="X" />
                            </Items>
                        </asp:DropDownList>
                        <asp:RequiredFieldValidator ID="rfvRank" runat="server" ControlToValidate="ddlRank"
                            InitialValue="X" ErrorMessage="Grade is a required selection." Display="None"></asp:RequiredFieldValidator>

                        <%--<asp:Label ID="lblPriority" runat="server" Text="*Order Priority" Width="100px" CssClass="srtsLabel_medium" Visible="false" />
                        <asp:DropDownList ID="ddlOrderPriority" runat="server" AppendDataBoundItems="true"
                            CssClass="srtsDropDown_medium" DataTextField="Value" DataValueField="Key" TabIndex="12" Visible="false">
                            <Items>
                                <asp:ListItem Text="Select Order Priority..." Value="X" />
                                <asp:ListItem Text="NONE" Value="N" Selected="True" />
                            </Items>
                        </asp:DropDownList>--%>
                    </ContentTemplate>
                </asp:UpdatePanel>
                <br />
                <br />
                <asp:Label ID="lblTheater" runat="server" Text="Theater Zip Code" Width="107px"
                    CssClass="srtsLabel_medium" />
                <asp:DropDownList ID="ddlTheaterLocationCodes" runat="server" DataTextField="TheaterCode"
                    DataValueField="TheaterCode" AppendDataBoundItems="true" TabIndex="13" CssClass="srtsDropDown_medium"
                    Width="255px">
                    <asp:ListItem Text="Select If Needed" Value=""></asp:ListItem>
                </asp:DropDownList>
                <br />
                <br />
                <div id="divIsActive" runat="server" visible="false">
                    <p class="label" style="text-align: right; margin-right: 30px">
                        <asp:Label ID="lblIsActive" runat="server" Text="Is Active" CssClass="srtsLabel_medium"
                            Width="150px"></asp:Label>
                        <asp:RadioButtonList runat="server" ID="rblIsActive" RepeatDirection="Horizontal"
                            AutoPostBack="true" TextAlign="Left" TabIndex="10">
                            <asp:ListItem Text="True" Value="True"></asp:ListItem>
                            <asp:ListItem Text="False" Value="False"></asp:ListItem>
                        </asp:RadioButtonList>
                    </p>
                </div>

                <div style="margin-top: 20px">
                    <asp:Panel ID="pnlExtra" runat="server" Visible="false">
                        <asp:Label ID="lblActiveDutyExtend" runat="server" Text="Extended Active Duty Expiration Date"
                            Width="240px" CssClass="srtsLabel_medium" />
                        <asp:TextBox ID="tbEADExpires" runat="server" Enabled="false" Width="100px">
                        </asp:TextBox>
                        <asp:Image runat="server" ID="calImage" AlternateText="Calendar" ImageUrl="~/Styles/images/Calendar_scheduleHS.png" />
                        <ajaxToolkit:CalendarExtender ID="ceEAD" runat="server" TargetControlID="tbEADExpires"
                            Format="MMMM d, yyyy" PopupButtonID="calImage">
                        </ajaxToolkit:CalendarExtender>
                    </asp:Panel>
                </div>
            </div>
            <div style="float: left; width: 45%;">

                <asp:Label ID="lblLastName" runat="server" Text="*Last Name" Width="100px" CssClass="srtsLabel_medium" />
                <asp:TextBox ID="txtLastName" runat="server" MaxLength="75" ToolTip="Enter individuals last name."
                    CssClass="srtsTextBox_medium" TabIndex="3" />
                <asp:RegularExpressionValidator ID="revLastName" runat="server" ControlToValidate="txtLastName"
                    ErrorMessage="Invalid characters in Last Name" ValidationExpression="^[a-zA-Z '-]+$"
                    Display="Dynamic"></asp:RegularExpressionValidator>
                <asp:RequiredFieldValidator ID="rfvLastName" runat="server" ControlToValidate="txtLastName"
                    ErrorMessage="Individual last name is required." Display="None"></asp:RequiredFieldValidator>
                <br />
                <br />
                <asp:Label ID="lblFirstName" runat="server" Text="*First Name" Width="100px" CssClass="srtsLabel_medium" />
                <asp:TextBox ID="tbFirstName" ClientIDMode="Static" runat="server" MaxLength="75"
                    ToolTip="Enter individuals first name." CssClass="srtsTextBox_medium" TabIndex="1" />
                <asp:RegularExpressionValidator ID="revFirstName" runat="server" ControlToValidate="tbFirstName"
                    ErrorMessage="Invalid characters in First Name" ValidationExpression="^[a-zA-Z '-]+$"
                    Display="Dynamic"></asp:RegularExpressionValidator>
                <asp:RequiredFieldValidator ID="rfvFirstName" runat="server" ControlToValidate="tbFirstName"
                    ErrorMessage="Individual first name is required" Display="None"></asp:RequiredFieldValidator>
                <br />
                <br />
                <asp:Label ID="lblMiddleName" runat="server" Text="Middle Name" Width="100px" CssClass="srtsLabel_medium" />
                <asp:TextBox ID="tbMiddleName" runat="server" MaxLength="75" ToolTip="Enter individuals middle name or initial."
                    CssClass="srtsTextBox_medium" TabIndex="2" />
                <asp:RegularExpressionValidator ID="revMiddleName" runat="server" ControlToValidate="tbMiddleName"
                    ErrorMessage="Invalid characters in Middle Name" ValidationExpression="^[a-zA-Z '-]+$"
                    Display="Dynamic"></asp:RegularExpressionValidator>
                <br />
                <br />
                <asp:Label ID="lblDOB" runat="server" Text="Date of Birth(MM/DD/YYYY)" Width="100px" CssClass="srtsLabel_medium" />
                <asp:TextBox ID="tbDOB" runat="server" TabIndex="4" Width="100px" />
                <asp:Image runat="server" ID="calImage1" AlternateText="Calendar" ImageUrl="~/Styles/images/Calendar_scheduleHS.png" />
                <ajaxToolkit:CalendarExtender ID="ceDOB" runat="server" CssClass="calendar_addpatient"
                    PopupPosition="BottomLeft" TargetControlID="tbDOB" Format="MM/dd/yyyy" PopupButtonID="calImage1">
                </ajaxToolkit:CalendarExtender>
                <asp:CustomValidator ID="cvDOB" runat="server" ErrorMessage="CustomValidator" EnableClientScript="False" OnServerValidate="ValidateDOB" ValidationGroup="allValidators" Text="*" ControlToValidate="tbDOB" ValidateEmptyText="True"></asp:CustomValidator>
                <%--<asp:RequiredFieldValidator ID="rfvDOB" runat="server" ControlToValidate="tbDOB" Display="None" ErrorMessage="DOB is a required field"></asp:RequiredFieldValidator>--%>
                <%--<asp:RangeValidator
                    runat="server"
                    ID="rvDOB"
                    Type="Date"
                    ControlToValidate="tbDOB"
                    ErrorMessage="enter valid date"
                    Display="None" />--%>
                <br />
                <br />
                <asp:Label ID="lblSex" runat="server" TabIndex="5" Text="Gender" Width="25px" CssClass="srtsLabel_medium" />
                <div style="margin: -28px 0px 0px 100px">
                    <asp:RadioButtonList ID="rblGender" runat="server" TabIndex="11" RepeatDirection="Horizontal"
                        ToolTip="Select individuals gender.">
                        <asp:ListItem Text="Male" Value="M" />
                        <asp:ListItem Text="Female" Value="F" />
                    </asp:RadioButtonList>
                </div>
                <br />
                <%--<asp:Label ID="lblIDType" runat="server" CssClass="srtsLabel_medium" Text="Individual Type"
                    Width="100px" />
                <asp:DropDownList ID="ddlIndividualType" runat="server" ToolTip="Select the type of the individual."
                    AppendDataBoundItems="true" DataTextField="Value" DataValueField="Value" TabIndex="11"
                    CssClass="srtsDropDown_medium">
                    <asp:ListItem Text="Select Individual Type..." Value="X"></asp:ListItem>
                </asp:DropDownList>
                <asp:RequiredFieldValidator ID="rfvIndividualType" runat="server" ControlToValidate="ddlIndividualType"
                    InitialValue="X" ErrorMessage="(Add)Individual Type is a required selection"
                    Display="None"></asp:RequiredFieldValidator>--%>
            </div>
            <div style="width: 100%; margin-top: 8px; height: 100px">
                <asp:Label ID="lblComments" runat="server" Text="Comments" Width="100px" CssClass="srtsLabel_medium" />
                <asp:TextBox ID="tbComments" runat="server" onKeyDown="return textboxMaxCommentSize(this, 256, event, getlblRemainingID(), gettbCommentID())" ToolTip="Enter any comments, valid characters include + - . , ! ? # ' / ( )"
                    Rows="5" TextMode="MultiLine" Width="700px" TabIndex="15" CssClass="srtsTextBox_medium"
                    Height="70px">
                </asp:TextBox>
                <asp:CustomValidator ID="cvComment" runat="server" ErrorMessage="Invalid character(s) in Comment" ClientValidationFunction="textboxCommentValidation" OnServerValidate="ValidateCommentFormat" Text="*" ControlToValidate="tbComments" ValidateEmptyText="True" CssClass="requestValidator"></asp:CustomValidator>
                <div style="margin-top: 60px; text-align: center;">
                    <asp:Label ID="lblRemaining" runat="server" CssClass="srtsLabel_medium"></asp:Label>
                </div>
            </div>
        </div>
    </div>
    <%--<div id="divButtonsBottom" style="margin: 13px 70px 0px 0px; text-align: right">
        <asp:Button ID="btnUpdateRecord" runat="server" CssClass="srtsButton" Text="Save" ToolTip="Update individual information"
            OnClick="btnUpdate_Click" />
        <asp:Button ID="btnCancel" runat="server" CssClass="srtsButton" ToolTip="Cancel current changes and return to individuals information"
            CausesValidation="false" OnClick="btnCancel_Click" Text="Cancel" />
    </div>--%>
</div>