<%@ Page Title="" Language="C#" MasterPageFile="~/srtsMaster.Master" AutoEventWireup="True"
    CodeBehind="SiteCodeEdit.aspx.cs" Inherits="SrtsWeb.Admin.SiteCodeEdit"
    EnableViewState="true" MaintainScrollPositionOnPostback="true" %>

<%@ MasterType VirtualPath="~/srtsMaster.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="../../Styles/PassFailConfirm.css" rel="stylesheet" />
    <script src="../../Scripts/Global/PassFailConfirm.js"></script>
    <style type="text/css">
        .srtsTextBox_medium {
            position: relative !important;
        }

        .captionLeft {
            clear: both;
            float: left;
            margin-left: 15px;
            margin-top: 10px;
        }

        .rblCaptionLeft {
            float: left;
            margin-left: 15px;
            margin-top: 5px;
        }

        .srtsDropDown_medium {
            position: relative !important;
        }
    </style>

</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="MainContent" runat="server">
    <asp:ValidationSummary ID="vsErrors" runat="server" DisplayMode="BulletList" ForeColor="Red" ShowMessageBox="false" ShowSummary="true" EnableClientScript="true" ValidationGroup="site" />
    <asp:ValidationSummary ID="vsErrors2" runat="server" ValidationGroup="addr" DisplayMode="BulletList" EnableClientScript="true" ShowMessageBox="false" ShowSummary="true" />

    <asp:Panel ID="pnlSite" runat="server" Visible="true">
        <div>
            <asp:LinkButton ID="lbtnReturn" runat="server" CausesValidation="false" PostBackUrl="~/WebForms/Admin/SiteCodeManagement.aspx" Text="Return to Site Management Page"></asp:LinkButton>
        </div>

        <div style="width: 100%; float: left;">
            <div class="BeigeBoxContainer" style="margin: 10px 20px 10px 20px;">
                <div class="BeigeBoxHeader" style="padding: 12px 10px 3px 15px; text-align: left;">
                    <span class="label">Site Information</span>
                </div>
                <div class="BeigeBoxContent padding">
                    <div class="w3-row">
                        <div class="w3-half">
                          <%--  Site Code--%>
                            <div class="captionLeft">
                                <asp:Label ID="lblSiteCode" runat="server" Text="Site Code" CssClass="srtsLabel_medium"></asp:Label>
                                <asp:TextBox ID="tbSiteCode" runat="server" CssClass="srtsTextBox_medium" Width="340px" Enabled="false"></asp:TextBox>
                            </div>
                            <%--Site Name--%>
                            <div class="captionLeft">
                                <asp:Label ID="lblSiteName" runat="server" CssClass="srtsLabel_medium" Text="Site Name"></asp:Label>
                                <asp:TextBox ID="tbSiteName" runat="server" CssClass="srtsTextBox_medium" Width="335px"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvSiteName" runat="server" ControlToValidate="tbSiteName" ErrorMessage="Site Name is required" ValidationGroup="site" Display="None"></asp:RequiredFieldValidator>
                            </div>
                            <%--Site Type--%>
                            <div class="captionLeft">
                                <asp:Label ID="lblSiteType" runat="server" CssClass="srtsLabel_medium" Text="Site Type"></asp:Label>
                                <asp:DropDownList ID="ddlSiteType" runat="server" DataTextField="Text" DataValueField="Value" CssClass="srtsDropDown_medium"
                                  width="350px"  OnSelectedIndexChanged="ddlSiteType_SelectedIndexChanged">
                                </asp:DropDownList>
                            </div>
                            <%--Site Description--%>
                            <div class="captionLeft">
                                <asp:Label ID="lblSiteDescription" runat="server" CssClass="srtsLabel_medium" Text="Site Description"></asp:Label>
                                <asp:TextBox ID="tbSiteDescription" runat="server" CssClass="srtsTextBox_medium" Width="300px"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvSiteDescription" runat="server" ErrorMessage="Please enter a site description" ControlToValidate="tbSiteDescription" ValidationGroup="site" Display="None"></asp:RequiredFieldValidator>
                            </div>
                                <%--BOS--%>
                            <div class="captionLeft">
                                <asp:Label ID="lblBOS" runat="server" CssClass="srtsLabel_medium" Text="BOS"></asp:Label>
                                <asp:DropDownList ID="ddlBOS" runat="server" DataTextField="Value" DataValueField="Key" CssClass="srtsDropDown_medium" Width="380px">
                                </asp:DropDownList>
                            </div>
                              <%--Email Address--%>
                            <div class="captionLeft">
                                <asp:Label ID="lblEmail" runat="server" CssClass="srtsLabel_medium" Text="Email Address"></asp:Label>
                                <asp:TextBox ID="tbEmail" runat="server" CssClass="srtsTextBox_medium" Width="305px"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvEmail"
                                    runat="server"
                                    ControlToValidate="tbEmail"
                                    ErrorMessage="Please enter an email address"
                                    ValidationGroup="site"
                                    Display="None"></asp:RequiredFieldValidator>
                                <asp:RegularExpressionValidator ID="EmailValid"
                                    runat="server"
                                    Display="None"
                                    ValidationExpression="^([a-zA-Z][\w\.-]*[a-zA-Z0-9]@[a-zA-Z0-9][\w\.-]*[a-zA-Z0-9]\.[a-zA-Z][a-zA-Z\.]*[a-zA-Z]){1,70}$"
                                    CssClass="failureNotificationSummary"
                                    ErrorMessage="Invalid email format entered"
                                    ControlToValidate="tbEmail"
                                    ValidationGroup="site"></asp:RegularExpressionValidator>
                            </div>
                                <%--DSN Phone Number--%>
                            <div class="captionLeft">
                                <asp:Label ID="lblDSNPhoneNumber" runat="server" CssClass="srtsLabel_medium" Text="DSN Phone Number"></asp:Label>
                                <asp:TextBox ID="tbDSNPhoneNumber" runat="server" CssClass="srtsTextBox_medium" Width="268px"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvDSN" ControlToValidate="tbDSNPhoneNumber" runat="server" ErrorMessage="Please enter a DSN Phone Number" ValidationGroup="site" Display="None"></asp:RequiredFieldValidator>
                            </div>
                               <%--Local Phone Number--%>
                            <div class="captionLeft">
                                <asp:Label ID="lblRegPhoneNumber" runat="server" CssClass="srtsLabel_medium" Text="Local Phone Number"></asp:Label>
                                <asp:TextBox ID="tbRegPhoneNumber" runat="server" CssClass="srtsTextBox_medium" Width="264px"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvReg" ControlToValidate="tbRegPhoneNumber" runat="server" ErrorMessage="Please enter a regular phone number" ValidationGroup="site" Display="None"></asp:RequiredFieldValidator>
                            </div>
                        </div>
                        <div class="w3-half">
                            <%--Is Reimbursable--%>
                            <div class="captionLeft">
                            <asp:Label ID="lblIsReimbursable" runat="server" CssClass="srtsLabel_medium" Text="Is Reimbursable"></asp:Label>
                            </div>
                            <div class="rblCaptionLeft">
                            <asp:RadioButtonList ID="rblIsReimbursable" RepeatDirection="Horizontal" runat="server">
                                <asp:ListItem Text="Yes" Value="True"></asp:ListItem>
                                <asp:ListItem Text="No" Value="False" Selected="True"></asp:ListItem>
                            </asp:RadioButtonList>
                            </div>
                            <asp:UpdatePanel ID="upRegion" runat="server" UpdateMode="Always">
                                <ContentTemplate>
                                    <asp:Panel ID="pnlLabs" runat="server">
                                        <div class="captionLeft">
                                            <asp:Label ID="lblMPrimary" runat="server" Text="Primary Multivision" CssClass="srtsLabel_medium"></asp:Label>
                                            <asp:DropDownList ID="ddlMPrimary" runat="server" AutoPostBack="true" CssClass="srtsDropDown_medium" Width="325px">
                                            </asp:DropDownList>
                                        </div>
                                        <div class="captionLeft">
                                            <asp:Label ID="lblSPrimary" runat="server" Text="Primary Singlevision" CssClass="srtsLabel_medium"></asp:Label>
                                            <asp:DropDownList ID="ddlSPrimary" runat="server" AutoPostBack="true" CssClass="srtsDropDown_medium" Width="315px">
                                            </asp:DropDownList>
                                        </div>
                                    </asp:Panel>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                            <div class="captionLeft">
                                <asp:Label ID="lblIsActive" runat="server" CssClass="srtsLabel_medium" Text="Is Active"></asp:Label>
                            </div>
                            <div class="rblCaptionLeft">
                                <asp:RadioButtonList ID="rblIsActive" RepeatDirection="Horizontal" runat="server">
                                    <asp:ListItem Text="Yes" Value="True"></asp:ListItem>
                                    <asp:ListItem Text="No" Value="False"></asp:ListItem>
                                </asp:RadioButtonList>
                            </div>

                            <%--Labs Only--%>
                            <asp:Panel ID="pnlLabsOnly" runat="server">
                        <div class="captionLeft">
                            <asp:Label ID="lblIsMultivision" runat="server" CssClass="srtsLabel_medium" Text="Is Multivision"></asp:Label>
                        </div>
                        <div class="rblCaptionLeft">
                            <asp:RadioButtonList ID="rblIsMultivision" RepeatDirection="Horizontal" runat="server">
                                <asp:ListItem Text="Yes" Value="True"></asp:ListItem>
                                <asp:ListItem Text="No" Value="False"></asp:ListItem>
                            </asp:RadioButtonList>
                        </div>
                        <div class="captionLeft">
                            <asp:Label ID="lblMaxEyeSize" runat="server" CssClass="srtsLabel_medium" Text="Max Eye Size"></asp:Label>
                            <asp:TextBox ID="tbMaxEyeSize" runat="server" CssClass="srtsTextBox_medium" Width="300px"></asp:TextBox>
                        </div>
                        <div class="captionLeft">
                            <asp:Label ID="lblMaxFrames" runat="server" CssClass="srtsLabel_medium" Text="Max Frames Per Month"></asp:Label>
                            <asp:TextBox ID="tbMaxFrames" runat="server" CssClass="srtsTextBox_medium" Width="237px"></asp:TextBox>
                        </div>
                        <div class="captionLeft">
                            <asp:Label ID="lblMaxPower" runat="server" CssClass="srtsLabel_medium" Text="Max Power"></asp:Label>
                            <asp:TextBox ID="tbMaxPower" runat="server" CssClass="srtsTextBox_medium" Width="315px"></asp:TextBox>
                        </div>
                        <div class="captionLeft">
                            <asp:Label ID="lblHasLMS" runat="server" CssClass="srtsLabel_medium" Text="Has LMS"></asp:Label>
                        </div>
                        <div class="rblCaptionLeft">
                            <asp:RadioButtonList ID="rblHasLMS" RepeatDirection="Horizontal" runat="server">
                                <asp:ListItem Text="Yes" Value="True"></asp:ListItem>
                                <asp:ListItem Text="No" Value="False" Selected="True"></asp:ListItem>
                            </asp:RadioButtonList>
                        </div>
                        <div class="captionLeft">
                            <asp:Label ID="lblShipToPatientLab" runat="server" CssClass="srtsLabel_medium">Lab ships directly to patients: </asp:Label>
                        </div>
                        <div class="rblCaptionLeft">
                            <asp:RadioButtonList ID="rblShipToPatientLab" runat="server" RepeatDirection="Horizontal">
                                <asp:ListItem Selected="True" Text="Yes" Value="True"></asp:ListItem>
                                <asp:ListItem Text="No" Value="False"></asp:ListItem>
                            </asp:RadioButtonList>
                        </div>
                    </asp:Panel>
                        </div>
                    </div>
             </div>
                <div class="BeigeBoxFooter" style="clear: both;"></div>
            </div>
        </div>

    </asp:Panel>
    <asp:Panel ID="pnlAddress" runat="server" Visible="true">
        <table style="width: 100%;">
            <tr>
                <td style="width: 50%;">
                    <div class="padding">
                        <div class="BeigeBoxContainer">
                                <div class="BeigeBoxHeader" style="padding: 12px 10px 3px 15px; text-align: left;">
                                    <span class="label">Site Address</span>
                                </div>                     
                            <div class="BeigeBoxContent padding" style="height:275px">
                                <div class="captionLeft">
                                    <asp:Label ID="lblAddress1" runat="server" CssClass="srtsLabel_medium">Address1: </asp:Label>
                                    <asp:TextBox ID="tbAddress1" runat="server" CssClass="srtsTextBox_medium"  Width="325px"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvAddress1" runat="server" ControlToValidate="tbAddress1" Display="None" ErrorMessage="Address1 is a required field" ValidationGroup="addr"></asp:RequiredFieldValidator>
                                </div>
                                <div class="captionLeft">
                                    <asp:Label ID="lblAddress2" runat="server" CssClass="srtsLabel_medium">Address2: </asp:Label>
                                    <asp:TextBox ID="tbAddress2" runat="server" CssClass="srtsTextBox_medium" Width="325px"></asp:TextBox>
                                </div>
                                <div class="captionLeft">
                                    <asp:Label ID="lblAddress3" runat="server" CssClass="srtsLabel_medium">Address3: </asp:Label>
                                    <asp:TextBox ID="tbAddress3" runat="server" CssClass="srtsTextBox_medium" Width="325px"></asp:TextBox>
                                </div>
                                <div class="captionLeft">
                                    <asp:Label ID="lblCity" runat="server" CssClass="srtsLabel_medium">City: </asp:Label>
                                    <asp:TextBox ID="tbCity" runat="server" CssClass="srtsTextBox_medium" Width="360px"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvCity" runat="server" ControlToValidate="tbCity" Display="None" ErrorMessage="City is a required field" ValidationGroup="addr"></asp:RequiredFieldValidator>
                                </div>
                                <div class="captionLeft">
                                    <asp:Label ID="lblState" runat="server" CssClass="srtsLabel_medium">State: </asp:Label>
                                    <asp:DropDownList ID="ddlState" runat="server" CssClass="srtsDropDown_medium"  Width="360px" DataTextField="ValueTextCombo" DataValueField="Value"></asp:DropDownList>
                                </div>
                                <div class="captionLeft">
                                    <asp:Label ID="lblCountry" runat="server" CssClass="srtsLabel_medium">Country: </asp:Label>
                                    <asp:DropDownList ID="ddlCountry" runat="server" CssClass="srtsDropDown_medium"  Width="345px" DataTextField="Text" DataValueField="Value"></asp:DropDownList>
                                </div>
                                <div class="captionLeft">
                                    <asp:Label ID="lblZipCode" runat="server" CssClass="srtsLabel_medium">ZipCode: </asp:Label>
                                    <asp:TextBox ID="tbZipCode" runat="server" CssClass="srtsTextBox_medium" Width="330px"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvZip" runat="server" ControlToValidate="tbZipCode" Display="None" ErrorMessage="Zip Code is a required field" ValidationGroup="addr"></asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="revZipCode" runat="server" ControlToValidate="tbZipCode"
                                        Display="None" ValidationGroup="addr" ErrorMessage="ZipCode Is Not Formatted Correctly"
                                        ValidationExpression="^\d{5}(\-\d{4})?$"></asp:RegularExpressionValidator>
                                </div>
                                <div class="captionLeft">
                                    <asp:Label ID="lblIsConus" runat="server" CssClass="srtsLabel_medium">Is CONUS: </asp:Label>
                                </div>
                                <div class="rblCaptionLeft">
                                    <asp:RadioButtonList ID="rblIsConus" runat="server" RepeatDirection="Horizontal">
                                        <asp:ListItem Selected="True" Text="Yes" Value="True"></asp:ListItem>
                                        <asp:ListItem Text="No" Value="False"></asp:ListItem>
                                    </asp:RadioButtonList>
                                </div>
                                <div class="captionLeft">
                                    <asp:Label ID="lblUse" runat="server" CssClass="srtsLabel_medium">Use Same Address for Mailing Address: </asp:Label>
                                </div>
                                <div class="rblCaptionLeft">
                                    <asp:RadioButtonList ID="rblUseAddress" runat="server" RepeatDirection="Horizontal" AutoPostBack="true" OnSelectedIndexChanged="rblUseAddress_SelectedIndexChanged" on>
                                        <asp:ListItem Text="Yes" Value="True" Selected="True"></asp:ListItem>
                                        <asp:ListItem Text="No" Value="False"></asp:ListItem>
                                    </asp:RadioButtonList>
                                    <asp:RequiredFieldValidator ID="rfvUseAddress" runat="server" ControlToValidate="rblUseAddress" Display="None"
                                        ErrorMessage="Use Same Address for Mailing Address is required." ValidationGroup="addr"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="BeigeBoxFooter" style="clear: both;"></div>
                        </div>
                    </div>
                </td>
                <td style="width: 50%;">
                    <div class="padding" style="clear: both; margin-top: 15px;">
                        <div class="BeigeBoxContainer">
                             <div class="BeigeBoxHeader" style="padding: 12px 10px 3px 15px; text-align: left;">
                                    <span class="label">Mail Address</span>
                                </div> 
                            <div class="BeigeBoxContent padding" style="height:275px">
                                <div class="captionLeft">
                                    <asp:Label ID="lblMailAddress1" runat="server" CssClass="srtsLabel_medium">Address1: </asp:Label>
                                    <asp:TextBox ID="tbMailAddress1" runat="server" CssClass="srtsTextBox_medium" Width="325px"></asp:TextBox>
                                </div>
                                <div class="captionLeft">
                                    <asp:Label ID="lblMailAddress2" runat="server" CssClass="srtsLabel_medium">Address2: </asp:Label>
                                    <asp:TextBox ID="tbMailAddress2" runat="server" CssClass="srtsTextBox_medium" Width="325px"></asp:TextBox>
                                </div>
                                <div class="captionLeft">
                                    <asp:Label ID="lblMailAddress3" runat="server" CssClass="srtsLabel_medium">Address3: </asp:Label>
                                    <asp:TextBox ID="tbMailAddress3" runat="server" CssClass="srtsTextBox_medium" Width="325px"></asp:TextBox>
                                </div>
                                <div class="captionLeft">
                                    <asp:Label ID="lblMailCity" runat="server" CssClass="srtsLabel_medium">City: </asp:Label>
                                    <asp:TextBox ID="tbMailCity" runat="server" CssClass="srtsTextBox_medium" Width="360px"></asp:TextBox>
                                </div>
                                <div class="captionLeft">
                                    <asp:Label ID="lblMailState" runat="server" CssClass="srtsLabel_medium">State: </asp:Label>
                                    <asp:DropDownList ID="ddlMailState" runat="server" CssClass="srtsDropDown_medium"  Width="360px" DataTextField="Text" DataValueField="Value"></asp:DropDownList>
                                </div>
                                <div class="captionLeft">
                                    <asp:Label ID="lblMailCountry" runat="server" CssClass="srtsLabel_medium">Country: </asp:Label>
                                    <asp:DropDownList ID="ddlMailCountry" runat="server" CssClass="srtsDropDown_medium" Width="345px" DataTextField="Text" DataValueField="Value"></asp:DropDownList>
                                </div>
                                <div class="captionLeft">
                                    <asp:Label ID="lblMailZipCode" runat="server" CssClass="srtsLabel_medium">ZipCode: </asp:Label>
                                    <asp:TextBox ID="tbMailZipCode" runat="server" CssClass="srtsTextBox_medium" Width="330px"></asp:TextBox>
                                </div>
                                <div class="captionLeft">
                                    <asp:Label ID="lblMailIsConus" runat="server" CssClass="srtsLabel_medium">Is CONUS: </asp:Label>
                                </div>
                                <div class="rblCaptionLeft">
                                    <asp:RadioButtonList ID="rblMailIsConus" runat="server" RepeatDirection="Horizontal">
                                        <asp:ListItem Selected="True" Text="Yes" Value="True"></asp:ListItem>
                                        <asp:ListItem Text="No" Value="False"></asp:ListItem>
                                    </asp:RadioButtonList>
                                </div>
                            </div>
                            <div class="BeigeBoxFooter" style="clear: both;"></div>
                        </div>
                    </div>
                </td>
            </tr>
        </table>

        <div class="captionLeft">
            <asp:Button ID="btnUpdate" runat="server" CssClass="srtsButton" Text="Update" OnClick="btnUpdate_Click" CausesValidation="true" ValidationGroup="addr" />
            <asp:Button ID="btnCancel" runat="server" CssClass="srtsButton" Text="Cancel" OnClick="btnCancel_Click" />
        </div>
    </asp:Panel>

    <asp:ScriptManagerProxy ID="smpSiteCodeEdit" runat="server">
        <Scripts>
            <asp:ScriptReference Path="~/Scripts/Sites/SiteShared.js" />
 <%--           <asp:ScriptReference Path="~/Scripts/Sites/SiteCodeEdit.js" />--%>
        </Scripts>
    </asp:ScriptManagerProxy>
</asp:Content>
