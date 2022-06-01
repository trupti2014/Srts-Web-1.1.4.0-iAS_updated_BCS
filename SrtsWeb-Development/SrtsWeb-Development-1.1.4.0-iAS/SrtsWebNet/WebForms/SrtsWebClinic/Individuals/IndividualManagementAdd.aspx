<%@ Page Title="" Language="C#" MasterPageFile="~/SrtsMaster.Master" AutoEventWireup="true"
    MaintainScrollPositionOnPostback="true" CodeBehind="IndividualManagementAdd.aspx.cs"
    EnableViewState="true" Inherits="SrtsWebClinic.Individuals.IndividualManagementAdd" %>

<%@ MasterType VirtualPath="~/srtsMaster.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script src="<%=ResolveUrl("~/Scripts/Global/SrtsCustomValidators.js") %>"></script>
    <script type="text/javascript">
        function getlblRemainingID() {
            var lblID = '<%=lblRemaining.ClientID%>';
            return lblID;
        }
        function gettbCommentID() {
            var tbID = '<%=tbComments.ClientID%>';
            return tbID;
        }
    </script>
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="MainContent" runat="server">
    <div class="tabContent">
        <div style="width: 100%; margin: 20px 0px">
            <span class="colorRed">
                <asp:Literal ID="litMessage" runat="server" Visible="false"></asp:Literal></span>
            <asp:ValidationSummary ID="vsErrors" runat="server" CssClass="validatorSummary" ValidationGroup="allValidators" DisplayMode="BulletList" />
            <br />
            <div>
                <asp:Label ID="lblSiteCode" runat="server" Text="*Facility (site)" Width="100px" CssClass="srtsLabel_medium" />
                <asp:DropDownList ID="ddlSite" runat="server" AppendDataBoundItems="true"
                    CssClass="srtsDropDown_medium" Width="400px" DataTextField="SiteCombination" DataValueField="SiteCode" TabIndex="1">
                    <Items>
                        <asp:ListItem Text="-Select-" Value="X" />
                    </Items>
                </asp:DropDownList>
                <ajaxToolkit:ListSearchExtender runat="server" TargetControlID="ddlSite" ID="LSE_ddlSite" Enabled="True" PromptText="Type to search" QueryTimeout="2" PromptCssClass="listSearchPromptOverlay" QueryPattern="Contains"></ajaxToolkit:ListSearchExtender>
            </div>
        </div>

        <div>
            <asp:Label ID="lblIdNumber" runat="server" CssClass="srtsLabel_medium" Text="*ID Type" Width="100px" />
            <asp:DropDownList ID="ddlIDNumberType" runat="server" ToolTip="Select ID Type for an Individual."
                AppendDataBoundItems="true" DataTextField="Value" DataValueField="Key" TabIndex="3" CssClass="srtsDropDown_medium" Width="265px">
                <asp:ListItem Text="-Select-" Value="X"></asp:ListItem>
            </asp:DropDownList>
            <asp:RequiredFieldValidator ID="rfvIDNumberType" runat="server" ControlToValidate="ddlIDNumberType" ValidationGroup="allValidators"
                InitialValue="X" ErrorMessage="ID Type is a required selection" Display="None"></asp:RequiredFieldValidator>
            <br />
            <br />

            <asp:Label ID="blIdNumber" runat="server" CssClass="srtsLabel_medium" Text="*ID Number" Width="100px" />
            <asp:TextBox ID="tbIDNumber" runat="server" ToolTip="Enter individual ID number." MaxLength="11" TabIndex="4" CssClass="srtsTextBox_medium" AutoPostBack="true" OnTextChanged="tbIDNumber_TextChanged" />
            <br />
            <br />
            <asp:Label ID="taboff" Text="Type an ID Number and hit Enter." runat="server" Visible="false" ForeColor="#FF3300"></asp:Label>
            <asp:RequiredFieldValidator ID="rfvtbIDNumber" runat="server" ControlToValidate="tbIDNumber" ValidationGroup="allValidators"
                InitialValue="X" ErrorMessage="ID Number is required" Display="None"></asp:RequiredFieldValidator>
            <asp:CustomValidator ID="cvIDNumber"
                runat="server"
                ErrorMessage="TBD"
                Text="cvIDNumber"
                ValidateEmptyText="True"
                OnServerValidation="ValidateIDNumber"
                CssClass="requestValidator" Display="none" EnableClientScript="False"></asp:CustomValidator>
            <br />
            <br />
            <br />

            <div style="margin-bottom: 15px;">
                <asp:GridView ID="gvSearch" runat="server" ClientIDMode="Static" AutoGenerateColumns="False" GridLines="None"
                    DataKeyNames="ID" Width="930px" CssClass="mGrid" ViewStateMode="Enabled" PageSize="1"
                    OnRowDataBound="gvSearch_RowDataBound" Visible="false"
                    EmptyDataText="No Data Found">
                    <AlternatingRowStyle CssClass="alt" />
                    <Columns>
                        <asp:BoundField DataField="LastName" HeaderText="Last Name" SortExpression="LastName">
                            <HeaderStyle HorizontalAlign="Left" />
                            <ItemStyle Width="70px" />
                        </asp:BoundField>
                        <asp:BoundField DataField="FirstName" HeaderText="First Name">
                            <HeaderStyle HorizontalAlign="Left" />
                            <ItemStyle Width="70px" />
                        </asp:BoundField>
                        <asp:BoundField DataField="IDNumberDisplay" HeaderText="Individual ID" SortExpression="IDNumber">
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
                        <asp:BoundField DataField="PersonalType" HeaderText="Individual Type">
                            <ItemStyle HorizontalAlign="Center" Width="75px" />
                        </asp:BoundField>
                    </Columns>
                </asp:GridView>
            </div>

            <asp:RegularExpressionValidator ID="revIDNumber" runat="server" ControlToValidate="tbIDNumber" ValidationGroup="allValidators"
                ErrorMessage="Invalid characters in ID" ValidationExpression="^[0-9a-zA-Z '-]+$"
                Display="None"></asp:RegularExpressionValidator>
            <asp:RequiredFieldValidator ID="rfvIDNumber" runat="server" ControlToValidate="tbIDNumber" ValidationGroup="allValidators"
                ErrorMessage="ID Number is a required field." Display="None"></asp:RequiredFieldValidator>
        </div>

        <asp:Panel ID="pnlCompleteForm" runat="server" Visible="False">
            <div>
                <%-- <asp:Label ID="lblIDType" runat="server" CssClass="srtsLabel_medium" Text="*Individual Type"
                    Width="100px" />
                <asp:DropDownList ID="ddlIndividualType" runat="server" ToolTip="Select the type of the individual."
                    DataTextField="Value" DataValueField="Key"
                    TabIndex="2" CssClass="srtsDropDown_medium" Width="265px" ValidationGroup="allValidators">
                </asp:DropDownList>
                <asp:RequiredFieldValidator ID="rfvIndividualType" runat="server" ControlToValidate="ddlIndividualType"
                    InitialValue="X" ErrorMessage="Individual Type is a required selection" ValidationGroup="allValidators"
                    Display="None"></asp:RequiredFieldValidator>
                <br />--%>

                <div id="IndTypesCBs" style="text-align: left">
                    <asp:Label ID="lblIndTypes" runat="server" Text="*Select individual type(s):" Width="175px" CssClass="srtsLabel_medium" ControlToValidate="lblIndTypes" />
                    <asp:CheckBox ID="cbProvider" runat="server" ClientIDMode="Static" /><asp:Label ID="lblProvider" runat="server" Text="Provider" Style="padding-left: 5px" />
                    <asp:CheckBox ID="cbTechnician" runat="server" ClientIDMode="Static" Style="padding-left: 20px" /><asp:Label ID="lblTechnician" runat="server" Text="Technician" Style="padding-left: 5px" />
                    <asp:CheckBox ID="cbAdministrator" runat="server" ClientIDMode="Static" Style="padding-left: 20px" /><asp:Label ID="lblOther" runat="server" Text="Other (e.g., Admin, Clerk)" Style="padding-left: 5px" /><br />
                    <asp:CustomValidator ID="cvIndTypes" runat="server" EnableClientScript="True" ClientValidationFunction="ClientValidateIndTypeCBs" OnServerValidate="ValidateIndTypeCBs" ValidationGroup="allValidators" ErrorMessage="Select at least one individual type" Display="none"></asp:CustomValidator>
                </div>
                <br />
                <asp:Label ID="lblLastName" runat="server" Text="*Last Name" Width="100px" CssClass="srtsLabel_medium" />
                <asp:TextBox ID="txtLastName" runat="server" MaxLength="75" ToolTip="Enter individual last name."
                    CssClass="srtsTextBox_medium" TabIndex="5" />
                <asp:RegularExpressionValidator ID="revLastName" runat="server" ControlToValidate="txtLastName" ValidationGroup="allValidators"
                    ErrorMessage="Invalid characters in Last Name" ValidationExpression="^[a-zA-Z '-]+$"
                    Display="Dynamic"></asp:RegularExpressionValidator>
                <asp:RequiredFieldValidator ID="rfvLastName" runat="server" ControlToValidate="txtLastName" ValidationGroup="allValidators"
                    ErrorMessage="Individual last name is required." Display="None"></asp:RequiredFieldValidator>
                <br />
                <br />
                <asp:Label ID="lblFirstName" runat="server" Text="*First Name" Width="100px" CssClass="srtsLabel_medium" />
                <asp:TextBox ID="tbFirstName" ClientIDMode="Static" runat="server" MaxLength="75"
                    ToolTip="Enter individual first name." CssClass="srtsTextBox_medium" TabIndex="6" />
                <asp:RegularExpressionValidator ID="revFirstName" runat="server" ControlToValidate="tbFirstName" ValidationGroup="allValidators"
                    ErrorMessage="Invalid characters in First Name" ValidationExpression="^[a-zA-Z '-]+$"
                    Display="Dynamic"></asp:RegularExpressionValidator>
                <asp:RequiredFieldValidator ID="rfvFirstName" runat="server" ControlToValidate="tbFirstName" ValidationGroup="allValidators"
                    ErrorMessage="Individual first name is required" Display="None"></asp:RequiredFieldValidator>
                <br />
                <br />
                <asp:Label ID="lblMiddleName" runat="server" Text="Middle Name" Width="100px" CssClass="srtsLabel_medium" />
                <asp:TextBox ID="tbMiddleName" runat="server" MaxLength="75" ToolTip="Enter individual middle name or initial."
                    CssClass="srtsTextBox_medium" TabIndex="7" />
                <asp:RegularExpressionValidator ID="revMiddleName" runat="server" ControlToValidate="tbMiddleName" ValidationGroup="allValidators"
                    ErrorMessage="Invalid characters in Middle Name" ValidationExpression="^[a-zA-Z '-]+$"
                    Display="Dynamic"></asp:RegularExpressionValidator>
                <br />
                <br />
                <asp:Label ID="lblDOB" runat="server" Text="Date of Birth" Width="100px" CssClass="srtsLabel_medium" />
                <asp:TextBox ID="tbDOB" runat="server" TabIndex="8" Width="100px" />
                <%--<asp:RequiredFieldValidator ID="rfvDBO" runat="server" ControlToValidate="tbDOB" ValidationGroup="allValidators"
                    ErrorMessage="DOB is required." Display="None"></asp:RequiredFieldValidator>--%>
                <asp:Image runat="server" ID="calImage1" AlternateText="Calendar" ImageUrl="~/Styles/images/Calendar_scheduleHS.png" />
                <ajaxToolkit:CalendarExtender ID="CalendarExtender1" runat="server"
                    PopupPosition="BottomLeft" TargetControlID="tbDOB" Format="MM/dd/yyyy" PopupButtonID="calImage1">
                </ajaxToolkit:CalendarExtender>
                <asp:CustomValidator ID="cvDOB" runat="server" ErrorMessage="CustomValidator" EnableClientScript="False" OnServerValidate="ValidateDOB" ValidationGroup="allValidators" Text="*" ControlToValidate="tbDOB" ValidateEmptyText="True"></asp:CustomValidator>
                <br />
                <br />
                <asp:Label ID="lblSex" runat="server" Text="Gender" Width="25px" CssClass="srtsLabel_medium" />
                <div style="margin: -28px 0px 0px 100px">
                    <asp:RadioButtonList ID="rblGender" runat="server" TabIndex="9" RepeatDirection="Horizontal"
                        ToolTip="Select individual gender.">
                        <asp:ListItem Text="Male" Value="M" Selected="True" />
                        <asp:ListItem Text="Female" Value="F" />
                    </asp:RadioButtonList>
                </div>
                <br />
                <asp:Label ID="lblIsPOC" runat="server" Text="Site POC?" CssClass="srtsLabel_medium" Width="45px"></asp:Label>
                <div style="margin: -28px 0px 0px 100px;">
                    <asp:RadioButtonList runat="server" ID="rblIsPOC" RepeatDirection="Horizontal"
                        AutoPostBack="true" TextAlign="Left" TabIndex="10">
                        <asp:ListItem Text="True" Value="True"></asp:ListItem>
                        <asp:ListItem Text="False" Value="False" Selected="True"></asp:ListItem>
                    </asp:RadioButtonList>
                </div>
                <br />
                <asp:UpdatePanel ID="upDemographic" runat="server" UpdateMode="Always">
                    <ContentTemplate>
                        <asp:Label ID="lblBranch" runat="server" Text="*Branch" Width="100px" CssClass="srtsLabel_medium" />
                        <asp:DropDownList ID="ddlBOS" runat="server" AppendDataBoundItems="true" ToolTip="Select the individuals' branch." AutoPostBack="true"
                            DataTextField="bosText" DataValueField="bosValue" TabIndex="11" CssClass="srtsDropDown_medium" OnSelectedIndexChanged="ddlBOS_SelectedIndexChanged">
                            <Items>
                                <asp:ListItem Text="-Select-" Value="X" />
                            </Items>
                        </asp:DropDownList>
                        <asp:RequiredFieldValidator ID="rfvBOS" runat="server" ControlToValidate="ddlBOS" ValidationGroup="allValidators"
                            InitialValue="X" ErrorMessage="Branch Of Service is a required selection" Display="None"></asp:RequiredFieldValidator>
                        <br />
                        <br />
                        <asp:Label ID="lblStatus" runat="server" Text="*Status" Width="100px" CssClass="srtsLabel_medium" />
                        <asp:DropDownList ID="ddlStatusType" runat="server" AppendDataBoundItems="true" DataTextField="statusText"
                            DataValueField="statusValue" AutoPostBack="true" TabIndex="12" CssClass="srtsDropDown_medium" OnSelectedIndexChanged="ddlStatusType_SelectedIndexChanged">
                            <Items>
                                <asp:ListItem Text="-Select-" Value="X" />
                            </Items>
                        </asp:DropDownList>
                        <asp:RequiredFieldValidator ID="rfvStatus" runat="server" ControlToValidate="ddlStatusType" ValidationGroup="allValidators"
                            InitialValue="X" ErrorMessage="Status is a required selection" Display="None"></asp:RequiredFieldValidator>
                        <br />
                        <br />
                        <asp:Label ID="lblRank" runat="server" Text="*Grade" Width="100px" CssClass="srtsLabel_medium" />
                        <asp:DropDownList ID="ddlRank" runat="server" AppendDataBoundItems="true" ToolTip="Select the individuals' grade." AutoPostBack="true"
                            DataTextField="rankText" DataValueField="rankValue" TabIndex="13" CssClass="srtsDropDown_medium" OnSelectedIndexChanged="ddlRank_SelectedIndexChanged">
                            <Items>
                                <asp:ListItem Text="-Select-" Value="X" />
                            </Items>
                        </asp:DropDownList>
                        <asp:RequiredFieldValidator ID="rfvRank" runat="server" ControlToValidate="ddlRank" ValidationGroup="allValidators"
                            InitialValue="X" ErrorMessage="Grade is a required selection." Display="None"></asp:RequiredFieldValidator>

                        <%--<asp:Label ID="lblPriority" runat="server" Text="*Order Priority" Width="100px" CssClass="srtsLabel_medium" Visible="false" />
                        <asp:DropDownList ID="ddlOrderPriority" runat="server" AppendDataBoundItems="true"
                            CssClass="srtsDropDown_medium" DataTextField="Value" DataValueField="Key" TabIndex="14"
                            Width="265px" Visible="false">
                            <Items>
                                <asp:ListItem Text="-Select-" Value="X" />
                                <asp:ListItem Text="NONE" Value="N" Selected="True" />
                            </Items>
                        </asp:DropDownList>--%>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
            <br />

            <div style="width: 100%; padding-top: 15px; height: 75px">
                <asp:Label ID="lblComments" runat="server" Text="Comments" Width="100px" CssClass="srtsLabel_medium" />
                <asp:TextBox ID="tbComments" runat="server" onKeyDown="return textboxMaxCommentSize(this, 256, event, getlblRemainingID(), gettbCommentID())" ToolTip="Enter any comments, valid characters include + - . , ! ? # ' / ( )"
                    Rows="5" TextMode="MultiLine" Width="800px" TabIndex="15" CssClass="srtsTextBox_medium"
                    Height="70px"> </asp:TextBox>
                <asp:CustomValidator ID="cvComment" runat="server" ErrorMessage="Invalid character(s) in Comment" ClientValidationFunction="textboxCommentValidation" OnServerValidate="ValidateCommentFormat" ValidationGroup="allValidators" Text="*" ControlToValidate="tbComments" ValidateEmptyText="True" CssClass="requestValidator"></asp:CustomValidator><br />
            </div>
            <div style="text-align: center">
                <asp:Label ID="lblRemaining" runat="server" CssClass="srtsLabel_medium"></asp:Label>
            </div>
            <div class="gvButtonsBottom">
                <asp:Button ID="btnSubmit" runat="server" CssClass="srtsButton" Text="Submit" OnClick="btnAdd_Click" ToolTip="Add new individual information" ValidationGroup="allValidators" />
                <asp:Button ID="btnCancel" runat="server" CssClass="srtsButton" Text="Cancel" OnClick="btnCancel_Click"
                    CausesValidation="false" ToolTip="Cancel current changes and return to individual information" />
            </div>
        </asp:Panel>
    </div>
</asp:Content>
