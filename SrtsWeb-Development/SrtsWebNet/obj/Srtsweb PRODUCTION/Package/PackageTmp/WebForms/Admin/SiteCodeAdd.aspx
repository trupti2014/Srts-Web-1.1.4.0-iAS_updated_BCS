<%@ Page Title="" Language="C#" MasterPageFile="~/srtsMaster.Master" AutoEventWireup="True"
    CodeBehind="SiteCodeAdd.aspx.cs" Inherits="SrtsWeb.Admin.SiteCodeAdd"
    EnableViewState="true" MaintainScrollPositionOnPostback="true" %>

<%@ MasterType VirtualPath="~/srtsMaster.Master" %>

<asp:Content ID="Content6" ContentPlaceHolderID="MainContent" runat="server">
    <asp:Panel ID="pnlSite" runat="server" Visible="true">
        <div id="wrapperSiteAdd" runat="server" style="width: 100%;">
            <asp:ValidationSummary ID="validSumm" runat="server" />
            <asp:LinkButton ID="lbtnReturn" CausesValidation="false" runat="server" PostBackUrl="~/WebForms/Admin/SiteCodeManagement.aspx" Text="Return to Facilities Manager Page"></asp:LinkButton>
            <br />
            <br />
            <div id="leftSiteAdd" runat="server" style="width: 30%; float: left; padding-right: 5px; border-right: 2px solid #782E1E;">
                <h3>Site Information</h3>
                <div id="leftSiteAddLabels" runat="server" style="width: 45%; float: left">
                    <br />
                    <asp:Label ID="lblSiteCode" runat="server" Text="Site Code" CssClass="srtsLabel_medium"></asp:Label>
                    <br />
                    <br />
                    <asp:Label ID="lblSiteType" runat="server" CssClass="srtsLabel_medium" Text="Site Type"></asp:Label>
                    <br />
                    <br />
                    <br />
                    <asp:Label ID="lblSiteName" runat="server" CssClass="srtsLabel_medium" Text="Site Name"></asp:Label>
                    <br />
                    <br />
                    <asp:Label ID="lblSiteDescription" runat="server" CssClass="srtsLabel_medium" Text="Site Description"></asp:Label>
                    <br />
                    <br />
                    <asp:Label ID="lblIsReimbursable" runat="server" CssClass="srtsLabel_medium" Text="Is Reimbursable"></asp:Label>
                    <br />
                    <br />
                    <asp:Label ID="lblBOS" runat="server" CssClass="srtsLabel_medium" Text="BOS"></asp:Label>
                    <br />
                    <br />
                    <asp:Label ID="lblEmail" runat="server" CssClass="srtsLabel_medium" Text="Email Address"></asp:Label>
                    <br />
                    <br />
                    <asp:Label ID="lblDSNPhoneNumber" runat="server" CssClass="srtsLabel_medium" Text="DSN Phone Number"></asp:Label>
                    <br />
                    <br />
                    <asp:Label ID="lblRegPhoneNumber" runat="server" CssClass="srtsLabel_medium" Text="Local Phone Number"></asp:Label>
                    <br />
                    <br />
                    <asp:Panel ID="pnlLabsLabels" runat="server">
                        <asp:Label ID="lblMPrimary" runat="server" Text="Primary Multivision" CssClass="srtsLabel_medium"></asp:Label>
                        <br />
                        <br />
                        <asp:Label ID="lblSPrimary" runat="server" Text="Primary Singlevision" CssClass="srtsLabel_medium"></asp:Label>
                        <br />
                        <br />
                    </asp:Panel>
                    <asp:Panel ID="pnlLabsOnlyLabels" runat="server">
                        <asp:Label ID="lblIsMultivision" runat="server" CssClass="srtsLabel_medium" Text="Is Multivision"></asp:Label>
                        <br />
                        <br />
                        <asp:Label ID="lblMaxEyeSize" runat="server" CssClass="srtsLabel_medium" Text="Max Eye Size"></asp:Label>
                        <br />
                        <br />
                        <asp:Label ID="lblMaxFrames" runat="server" CssClass="srtsLabel_medium" Text="Max Frames Per Month"></asp:Label>
                        <br />
                        <br />
                        <asp:Label ID="lblMaxPower" runat="server" CssClass="srtsLabel_medium" Text="Max Power"></asp:Label>
                        <br />
                        <br />
                        <asp:Label ID="lblHasLMS" runat="server" CssClass="srtsLabel_medium" Text="Has LMS"></asp:Label>
                        <br />
                        <br />
                        <asp:Label ID="lblShipToPatientLab" runat="server" CssClass="srtsLabel_medium">Lab ships directly to patients: </asp:Label>
                        <br />
                        <br />
                    </asp:Panel>
                    <br />
                    <asp:Label ID="lblIsActive" runat="server" CssClass="srtsLabel_medium" Text="Is Active"></asp:Label>
                </div>
                <div id="leftSiteAddInput" runat="server" style="width: 55%; float: left">

                    <br />
                    <asp:TextBox ID="tbSiteCode" runat="server" MinLength="6" MaxLength="6" CssClass="srtsTextBox_small"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvSiteCode" runat="server" ControlToValidate="tbSiteCode" Display="None" ValidationGroup="site" ErrorMessage="A Six Character Site Code is required"></asp:RequiredFieldValidator>\
                    <asp:RegularExpressionValidator ID="SiteCodeValid"
                        runat="server"
                        Display="None"
                        ValidationExpression="^[a-zA-Z0-9\s]{6,6}$"
                        CssClass="failureNotificationSummary"
                        ErrorMessage="Site Code Must be 6 Alphanumeric Characters"
                        ControlToValidate="tbSiteCode"
                        ValidationGroup="site">
                    </asp:RegularExpressionValidator>
                    <br />
                    <br />
                    <asp:DropDownList ID="ddlSiteType" runat="server" DataTextField="Text" DataValueField="Value" CssClass="srtsDropDown_small" OnSelectedIndexChanged="ddlSiteType_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                    <br />
                    <br />
                    <asp:TextBox ID="tbSiteName" runat="server" CssClass="srtsTextBox_small"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvSiteName" runat="server" ControlToValidate="tbSiteName" Display="None" ValidationGroup="site" ErrorMessage="Site Name is required"></asp:RequiredFieldValidator>
                    <br />
                    <br />
                    <asp:TextBox ID="tbSiteDescription" runat="server" CssClass="srtsTextBox_small"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvSiteDescription" runat="server" Display="None" ErrorMessage="Please enter a site description" ValidationGroup="site" ControlToValidate="tbSiteDescription"></asp:RequiredFieldValidator>
                    <br />
                    <br />
                    <asp:RadioButtonList ID="rblIsReimbursable" RepeatDirection="Horizontal" runat="server">
                        <asp:ListItem Text="Yes" Value="True"></asp:ListItem>
                        <asp:ListItem Text="No" Value="False" Selected="True"></asp:ListItem>
                    </asp:RadioButtonList>
                    <br />
                    <asp:DropDownList ID="ddlBOS" runat="server" DataTextField="Value" DataValueField="Key" CssClass="srtsDropDown_small">
                    </asp:DropDownList>
                    <br />
                    <div style="margin-bottom: 10px;"></div>
                    <asp:TextBox ID="tbEmail" runat="server" CssClass="srtsTextBox_small"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvEmail" runat="server" Display="None" ControlToValidate="tbEmail"
                        ValidationGroup="site" ErrorMessage="Please enter an email address"></asp:RequiredFieldValidator>
                    <asp:RegularExpressionValidator ID="EmailValid"
                        runat="server"
                        Display="None"
                        ValidationExpression="^([a-zA-Z][\w\.-]*[a-zA-Z0-9]@[a-zA-Z0-9][\w\.-]*[a-zA-Z0-9]\.[a-zA-Z][a-zA-Z\.]*[a-zA-Z]){1,70}$"
                        CssClass="failureNotificationSummary"
                        ErrorMessage="Invalid email format entered"
                        ControlToValidate="tbEmail"
                        ValidationGroup="site"></asp:RegularExpressionValidator>
                    <br />
                    <br />
                    <asp:TextBox ID="tbDSNPhoneNumber" runat="server" CssClass="srtsTextBox_small"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvDSN" ControlToValidate="tbDSNPhoneNumber" runat="server" Display="None" ValidationGroup="site" ErrorMessage="Please enter a DSN Phone Number"></asp:RequiredFieldValidator>
                    <br />
                    <br />
                    <asp:TextBox ID="tbRegPhoneNumber" runat="server" CssClass="srtsTextBox_small"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvReg" ControlToValidate="tbRegPhoneNumber" Display="None" runat="server" ValidationGroup="site" ErrorMessage="Please enter a regular phone number"></asp:RequiredFieldValidator>
                    <asp:Panel ID="pnlLabsInput" runat="server">
                        <br />
                        <br />
                        <asp:DropDownList ID="ddlMPrimary" runat="server" AutoPostBack="true" CssClass="srtsDropDown_small">
                        </asp:DropDownList>
                        <br />
                        <br />
                        <asp:DropDownList ID="ddlSPrimary" runat="server" AutoPostBack="true" CssClass="srtsDropDown_small">
                        </asp:DropDownList>
                    </asp:Panel>
                    <asp:Panel ID="pnlLabsOnlyInput" runat="server">

                        <br />
                        <br />
                        <asp:RadioButtonList ID="rblIsMultivision" RepeatDirection="Horizontal" runat="server" CssClass="srtsDropDown_small">
                            <asp:ListItem Text="Yes" Value="True" Selected="True"></asp:ListItem>
                            <asp:ListItem Text="No" Value="False"></asp:ListItem>
                        </asp:RadioButtonList>
                        <br />
                        <asp:TextBox ID="tbMaxEyeSize" runat="server" CssClass="srtsTextBox_small"></asp:TextBox>
                        <br />
                        <br />
                        <asp:TextBox ID="tbMaxFrames" runat="server" CssClass="srtsTextBox_small"></asp:TextBox>
                        <br />
                        <br />
                        <br />
                        <asp:TextBox ID="tbMaxPower" runat="server" CssClass="srtsTextBox_small"></asp:TextBox>
                        <br />
                        <br />
                        <asp:RadioButtonList ID="rblHasLMS" RepeatDirection="Horizontal" runat="server" CssClass="srtsDropDown_small">
                            <asp:ListItem Text="Yes" Value="True"></asp:ListItem>
                            <asp:ListItem Text="No" Value="False" Selected="True"></asp:ListItem>
                        </asp:RadioButtonList>
                        <br />
                        <asp:RadioButtonList ID="rblShipToPatientLab" runat="server" RepeatDirection="Horizontal" CssClass="srtsDropDown_small">
                            <asp:ListItem Selected="True" Text="Yes" Value="True"></asp:ListItem>
                            <asp:ListItem Text="No" Value="False"></asp:ListItem>
                        </asp:RadioButtonList>
                    </asp:Panel>
                    &nbsp
                    <br />
                    <asp:RadioButtonList ID="rblIsActive" RepeatDirection="Horizontal" runat="server" CssClass="srtsDropDown_small">
                        <asp:ListItem Text="Yes" Value="True" Selected="True"></asp:ListItem>
                        <asp:ListItem Text="No" Value="False"></asp:ListItem>
                    </asp:RadioButtonList>

                    <br />
                </div>
            </div>
            <div id="middleSiteAdd" runat="server" style="width: 68%; float: left; margin-left: 10px;">
                <asp:Panel ID="pnlAddress" runat="server" Visible="true">
                    <div id="middleSiteAddPhysAddress" runat="server" style="width: 50%; float: left">
                        <h3>Site Address</h3>
                        <div id="middleSiteAddPhysAddressLabels" runat="server" style="width: 45%; float: left;">
                            <br />
                            <asp:Label ID="lblAddress1" runat="server" CssClass="srtsLabel_medium">Address1: </asp:Label>
                            <br />
                            <br />
                            <asp:Label ID="lblAddress2" runat="server" CssClass="srtsLabel_medium">Address2: </asp:Label>
                            <br />
                            <br />
                            <asp:Label ID="lblAddress3" runat="server" CssClass="srtsLabel_medium">Address3: </asp:Label>
                            <br />
                            <br />
                            <asp:Label ID="lblCity" runat="server" CssClass="srtsLabel_medium">City: </asp:Label>
                            <br />
                            <br />
                            <asp:Label ID="lblState" runat="server" CssClass="srtsLabel_medium">State: </asp:Label>
                            <br />
                            <br />
                            <asp:Label ID="lblCountry" runat="server" CssClass="srtsLabel_medium">Country: </asp:Label>
                            <br />
                            <br />
                            <br />
                            <asp:Label ID="lblZipCode" runat="server" CssClass="srtsLabel_medium">ZipCode: </asp:Label>
                            <br />
                            <br />
                            <asp:Label ID="lblIsConus" runat="server" CssClass="srtsLabel_medium">Is CONUS: </asp:Label>
                            <br />
                            <br />
                            <asp:Label ID="lblUse" runat="server" CssClass="srtsLabel_medium">Same Mailing Address? </asp:Label>
                            <br />
                        </div>
                        <div id="middleSiteAddPhysAddressInput" runat="server" style="width: 50%; float: left;">
                            <br />
                            <asp:TextBox ID="tbAddress1" runat="server" CssClass="srtsTextBox_small"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvAddress1" runat="server" ControlToValidate="tbAddress1" Display="None" ValidationGroup="site" ErrorMessage="Address1 is a required field"></asp:RequiredFieldValidator>
                            <br />
                            <br />
                            <asp:TextBox ID="tbAddress2" runat="server" CssClass="srtsTextBox_small"></asp:TextBox>
                            <br />
                            <br />
                            <asp:TextBox ID="tbAddress3" runat="server" CssClass="srtsTextBox_small"></asp:TextBox>
                            <br />
                            <br />
                            <asp:TextBox ID="tbCity" runat="server" CssClass="srtsTextBox_small"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvCity" runat="server" ControlToValidate="tbCity" Display="None" ValidationGroup="site" ErrorMessage="City is a required field"></asp:RequiredFieldValidator>
                            <br />
                            <br />
                            <asp:DropDownList ID="ddlState" runat="server" Width="185px" CssClass="srtsDropDown_small" DataTextField="ValueTextCombo" DataValueField="Value" ClientIDMode="Static"></asp:DropDownList>
                            &nbsp
                            <asp:DropDownList ID="ddlCountry" runat="server" Width="185px" CssClass="srtsDropDown_small" DataTextField="Text" DataValueField="Value" ClientIDMode="Static"></asp:DropDownList>
                            <br />
                            <asp:TextBox ID="tbZipCode" runat="server" CssClass="srtsTextBox_small"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvZipCode" runat="server" ControlToValidate="tbZipCode" ValidationGroup="site" ErrorMessage="Zip Code is a required field"></asp:RequiredFieldValidator>
                            <asp:RegularExpressionValidator ID="revZipCode" runat="server" ControlToValidate="tbZipCode"
                                Display="None" ValidationGroup="site" ErrorMessage="ZipCode Is Not Formatted Correctly"
                                ValidationExpression="^\d{5}(\-\d{4})?$"></asp:RegularExpressionValidator>
                            <br />
                            &nbsp
                            <asp:RadioButtonList ID="rblIsConus" runat="server" RepeatDirection="Horizontal" CssClass="srtsDropDown_small">
                                <asp:ListItem Selected="True" Text="Yes" Value="True"></asp:ListItem>
                                <asp:ListItem Text="No" Value="False"></asp:ListItem>
                            </asp:RadioButtonList>
                            &nbsp
                            <asp:RadioButtonList ID="rblUseAddress" runat="server" RepeatDirection="Horizontal" CssClass="srtsDropDown_small" AutoPostBack="true" OnSelectedIndexChanged="rblUseAddress_SelectedIndexChanged">
                                <asp:ListItem Text="Yes" Value="true" Selected="True"></asp:ListItem>
                                <asp:ListItem Text="No" Value="false"></asp:ListItem>
                            </asp:RadioButtonList>
                        </div>
                    </div>
                    <div id="middleSiteAddMailAddress" runat="server" visible="false" style="width: 45%; margin-left: 5px; padding: 5px; float: left; border-left: 2px solid #782E1E;">
                        <h3>Mail Address</h3>
                        <div id="middleSiteAddMailAddressLables" runat="server" style="width: 45%; float: left;">
                            <br />
                            <asp:Label ID="lblMailAddress1" runat="server" CssClass="srtsLabel_medium">Address1: </asp:Label>
                            <br />
                            <br />
                            <asp:Label ID="lblMailAddress2" runat="server" CssClass="srtsLabel_medium">Address2: </asp:Label>
                            <br />
                            <br />
                            <asp:Label ID="lblMailAddress3" runat="server" CssClass="srtsLabel_medium">Address3: </asp:Label>
                            <br />
                            <br />
                            <asp:Label ID="lblMailCity" runat="server" CssClass="srtsLabel_medium">City: </asp:Label>
                            <br />
                            <br />
                            <asp:Label ID="lblMailState" runat="server" CssClass="srtsLabel_medium">State: </asp:Label>
                            <br />
                            <br />
                            <asp:Label ID="lblMailCountry" runat="server" CssClass="srtsLabel_medium">Country: </asp:Label>
                            <br />
                            <br />
                            <br />
                            <asp:Label ID="lblMailZipCode" runat="server" CssClass="srtsLabel_medium">ZipCode: </asp:Label>
                            <br />
                            <br />
                            <asp:Label ID="lblMailIsConus" runat="server" CssClass="srtsLabel_medium">Is CONUS: </asp:Label>
                        </div>
                        <div id="middleSiteAddMailAddressInput" runat="server" style="width: 50%; float: left;">
                            <br />
                            <asp:TextBox ID="tbMailAddress1" runat="server" CssClass="srtsTextBox_small"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvMailAddress1" runat="server" ControlToValidate="tbMailAddress1" Display="None" ValidationGroup="site" ErrorMessage="Mail Address1 is a required field"></asp:RequiredFieldValidator>
                            <br />
                            <br />
                            <asp:TextBox ID="tbMailAddress2" runat="server" CssClass="srtsTextBox_small"></asp:TextBox>
                            <br />
                            <br />
                            <asp:TextBox ID="tbMailAddress3" runat="server" CssClass="srtsTextBox_small"></asp:TextBox>
                            <br />
                            <br />
                            <asp:TextBox ID="tbMailCity" runat="server" CssClass="srtsTextBox_small"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvMailCity" runat="server" ControlToValidate="tbMailCity" Display="None" ValidationGroup="site" ErrorMessage="Mail City is a required field"></asp:RequiredFieldValidator>
                            <br />
                            <br />
                            <asp:DropDownList ID="ddlMailState" runat="server" Width="185px" CssClass="srtsDropDown_small" DataTextField="Text" DataValueField="Value" ClientIDMode="Static"></asp:DropDownList>
                            <br />
                            <asp:DropDownList ID="ddlMailCountry" runat="server" Width="185px" CssClass="srtsDropDown_small" DataTextField="Text" DataValueField="Value" ClientIDMode="Static"></asp:DropDownList>
                            <br />
                            <asp:TextBox ID="tbMailZipCode" runat="server" CssClass="srtsTextBox_small"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvMailZip" runat="server" ControlToValidate="tbMailZipCode" Display="None" ValidationGroup="site" ErrorMessage="Mail Zip Code is a required field"></asp:RequiredFieldValidator>
                            <br />
                            <br />
                            <asp:RadioButtonList ID="rblMailIsConus" runat="server" CssClass="srtsDropDown_small" RepeatDirection="Horizontal">
                                <asp:ListItem Selected="True" Text="Yes" Value="True"></asp:ListItem>
                                <asp:ListItem Text="No" Value="False"></asp:ListItem>
                            </asp:RadioButtonList>
                        </div>
                        <br />
                        <br />
                    </div>
                </asp:Panel>
            </div>
        </div>
        <div id="divValidationSumm">
            <asp:ValidationSummary ID="vsErrors" runat="server" DisplayMode="BulletList" ForeColor="Red" ValidationGroup="site" />
            <br />
        </div>
        <div id="divBtnControls" runat="server" style="width: 100%;">
            <div id="divBtnControlsMid" runat="server" style="width: 20%; margin: auto;">
                <div id="divBtnConrolsMidSubmit" runat="server" style="width: 100px; float: left;">
                    <asp:Button ID="btnAdd" runat="server" CssClass="srtsButton" Text="Submit" OnClick="btnAdd_Click" CausesValidation="true" ValidationGroup="site" />
                </div>
                <div id="divBtnConrolsMidCancel" runat="server" style="width: 100px; float: right;">
                    <asp:Button ID="btnCancel" runat="server" CssClass="srtsButton" Text="Cancel" OnClick="btnCancel_Click" />
                </div>
            </div>
        </div>
    </asp:Panel>
    <asp:ScriptManagerProxy ID="smpSiteCodeAdd" runat="server">
        <Scripts>
            <asp:ScriptReference Path="~/Scripts/Sites/SiteShared.js" />
        </Scripts>
    </asp:ScriptManagerProxy>
</asp:Content>