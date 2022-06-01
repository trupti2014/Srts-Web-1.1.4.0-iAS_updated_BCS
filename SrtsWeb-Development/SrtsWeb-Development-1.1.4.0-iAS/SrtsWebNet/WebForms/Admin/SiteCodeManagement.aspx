<%@ Page Title="" Language="C#" MasterPageFile="~/srtsMaster.Master" AutoEventWireup="True"
    CodeBehind="SiteCodeManagement.aspx.cs" Inherits="SrtsWeb.Admin.SiteCodeManagement"
    EnableViewState="true" MaintainScrollPositionOnPostback="true" %>

<%@ MasterType VirtualPath="~/srtsMaster.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        .cellCaption {
            width: 10%;
            text-align: left;
        }

        .cellContent {
            width: 29%;
            text-align: left;
        }

        .srtsTextBox_medium {
            position: relative !important;
        }

        .srtsDropDown_medium {
            position: relative !important;
        }
    </style>
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="MainContent" runat="server">
    <asp:ValidationSummary ID="vsError" runat="server" ForeColor="Red" DisplayMode="BulletList" />
    <asp:Panel ID="pnlSite" runat="server" Visible="true" CssClass="padding">
        <div class="BeigeBoxContainer">
             <div class="BeigeBoxHeader" style="padding: 12px 10px 3px 15px">
              <span class="label" style="margin-left: 10px">Site Information</span>
             </div>
            <div class="BeigeBoxContent padding">

                <table style="width: 90%;">
                    <tr style="width: 100%;">
                        <td class="cellCaption srtsLabel_medium">Site Code
                        </td>
                        <td class="cellContent">
                            <asp:DropDownList ID="ddlSiteCode" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlSiteCode_SelectedIndexChanged" CssClass="srtsDropDown_medium">
                            </asp:DropDownList>
                            <ajaxToolkit:ListSearchExtender runat="server" TargetControlID="ddlSiteCode" ID="LSE_ddlSiteCode" Enabled="True" PromptText="Type to search" QueryTimeout="2" PromptCssClass="listSearchPromptOverlay" QueryPattern="Contains"></ajaxToolkit:ListSearchExtender>
                        </td>
                        <td class="cellCaption srtsLabel_medium">Email Address
                        </td>
                        <td class="cellContent">
                            <asp:TextBox ID="tbEmail" ReadOnly="true" runat="server" CssClass="srtsTextBox_medium"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="cellCaption srtsLabel_medium">Site Name
                        </td>
                        <td class="cellContent">
                            <asp:TextBox ID="tbSiteName" ReadOnly="true" runat="server" CssClass="srtsTextBox_medium"></asp:TextBox>
                        </td>
                        <td class="cellCaption srtsLabel_medium">DSN Phone #
                        </td>
                        <td class="cellContent">
                            <asp:TextBox ID="tbDSNPhoneNumber" ReadOnly="true" runat="server" CssClass="srtsTextBox_medium"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="cellCaption srtsLabel_medium">Site Type
                        </td>
                        <td class="cellContent">
                            <asp:TextBox ID="tbSiteType" ReadOnly="true" runat="server" CssClass="srtsTextBox_medium"></asp:TextBox>
                        </td>
                        <td class="cellCaption srtsLabel_medium">Local Phone #
                        </td>
                        <td class="cellContent">
                            <asp:TextBox ID="tbRegPhoneNumber" ReadOnly="true" runat="server" CssClass="srtsTextBox_medium"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="cellCaption srtsLabel_medium">Site Description
                        </td>
                        <td class="cellContent">
                            <asp:TextBox ID="tbSiteDescription" ReadOnly="true" runat="server" CssClass="srtsTextBox_medium"></asp:TextBox>
                        </td>
                        <td class="cellCaption srtsLabel_medium">BOS
                        </td>
                        <td class="cellContent">
                            <asp:TextBox ID="tbBOS" ReadOnly="true" runat="server" CssClass="srtsTextBox_medium"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td style="font-weight: bold;"></td>
                        <td></td>
                        <td class="cellCaption srtsLabel_medium">Is Active
                        </td>
                        <td class="cellContent">
                            <asp:TextBox ID="tbIsActive" ReadOnly="true" runat="server" CssClass="srtsTextBox_medium"></asp:TextBox>
                        </td>
                    </tr>
                </table>
                <asp:Panel ID="pnlLabOnly" runat="server">
                    <table style="width: 90%;">
                        <tr style="width: 100%">
                            <td class="cellCaption srtsLabel_medium">Max Eye Size
                            </td>
                            <td class="cellContent">
                                <asp:TextBox ID="tbMaxEyeSize" ReadOnly="true" runat="server" CssClass="srtsTextBox_medium"></asp:TextBox>
                            </td>
                            <td class="cellCaption srtsLabel_medium">Frames Per Month
                            </td>
                            <td class="cellContent">
                                <asp:TextBox ID="tbMaxFrames" ReadOnly="true" runat="server" CssClass="srtsTextBox_medium"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="cellCaption srtsLabel_medium">Is Multivision
                            </td>
                            <td class="cellContent">
                                <asp:TextBox ID="tbMultivision" ReadOnly="true" runat="server" CssClass="srtsTextBox_medium"></asp:TextBox>
                            </td>
                            <td class="cellCaption srtsLabel_medium">Max Power
                            </td>
                            <td class="cellContent">
                                <asp:TextBox ID="tbMaxPower" ReadOnly="true" runat="server" CssClass="srtsTextBox_medium"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="cellCaption srtsLabel_medium">Has LMS
                            </td>
                            <td class="cellContent">
                                <asp:TextBox ID="tbHasLMS" ReadOnly="true" runat="server" CssClass="srtsTextBox_medium"></asp:TextBox>
                            </td>
                            <td class="cellCaption srtsLabel_medium">Ships to patient
                            </td>
                            <td class="cellContent">
                                <asp:RadioButtonList ID="rblShipToPatientLab" runat="server" RepeatDirection="Horizontal">
                                    <asp:ListItem Selected="True" Text="Yes" Value="True"></asp:ListItem>
                                    <asp:ListItem Text="No" Value="False"></asp:ListItem>
                                </asp:RadioButtonList>
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
                <asp:Panel ID="pnlLabs" runat="server">
                    <br />
                    <table style="width: 50%;">
                        <tr style="width: 100%">
                            <td></td>
                            <td class="cellCaption">
                                <h3>Primary</h3>
                            </td>
                        </tr>
                        <tr>
                            <td class="cellCaption srtsLabel_medium">Multi Vision Lab</td>
                            <td class="cellContent">
                                <asp:TextBox ID="tbMPrimary" ReadOnly="true" runat="server" CssClass="srtsTextBox_medium"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="cellCaption srtsLabel_medium">Single Vision Lab</td>
                            <td class="cellContent">
                                <asp:TextBox ID="tbSPrimary" ReadOnly="true" runat="server" CssClass="srtsTextBox_medium"></asp:TextBox>
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
            </div>
            <div class="BeigeBoxFooter"></div>
        </div>
    </asp:Panel>

    <asp:Panel ID="pnlAddress" runat="server" Visible="true" CssClass="padding">
        <div class="BeigeBoxContainer">
             <div class="BeigeBoxHeader" style="padding: 12px 10px 3px 15px">
              <span class="label" style="margin-left: 10px">Addresses</span>
             </div>
            <div class="BeigeBoxContent padding">

                <table style="width: 90%;">
                    <tr style="width: 100%;">
                        <td></td>
                        <td class="cellCaption">
                            <h3>Street Address</h3>
                        </td>
                        <td class="cellCaption">
                            <h3>Mailing Address</h3>
                        </td>
                    </tr>
                    <tr>
                        <td class="cellCaption srtsLabel_medium">Address1
                        </td>
                        <td class="cellContent">
                            <asp:TextBox ID="tbAddress1" ReadOnly="true" runat="server" CssClass="srtsTextBox_medium"></asp:TextBox>
                        </td>
                        <td class="cellContent">
                            <asp:TextBox ID="tbMailAddress1" ReadOnly="true" runat="server" CssClass="srtsTextBox_medium"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="cellCaption srtsLabel_medium">Address2
                        </td>
                        <td class="cellContent">
                            <asp:TextBox ID="tbAddress2" ReadOnly="true" runat="server" CssClass="srtsTextBox_medium"></asp:TextBox>
                        </td>
                        <td class="cellContent">
                            <asp:TextBox ID="tbMailAddress2" ReadOnly="true" runat="server" CssClass="srtsTextBox_medium"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="cellCaption srtsLabel_medium">Address3
                        </td>
                        <td class="cellContent">
                            <asp:TextBox ID="tbAddress3" ReadOnly="true" runat="server" CssClass="srtsTextBox_medium"></asp:TextBox>
                        </td>
                        <td class="cellContent">
                            <asp:TextBox ID="tbMailAddress3" ReadOnly="true" runat="server" CssClass="srtsTextBox_medium"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="cellCaption srtsLabel_medium">City
                        </td>
                        <td class="cellContent">
                            <asp:TextBox ID="tbCity" ReadOnly="true" runat="server" CssClass="srtsTextBox_medium"></asp:TextBox>
                        </td>
                        <td class="cellContent">
                            <asp:TextBox ID="tbMailCity" ReadOnly="true" runat="server" CssClass="srtsTextBox_medium"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="cellCaption srtsLabel_medium">State
                        </td>
                        <td class="cellContent">
                            <asp:TextBox ID="tbState" ReadOnly="true" runat="server" CssClass="srtsTextBox_medium"></asp:TextBox>
                        </td>
                        <td class="cellContent">
                            <asp:TextBox ID="tbMailState" ReadOnly="true" runat="server" CssClass="srtsTextBox_medium"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="cellCaption srtsLabel_medium">Country
                        </td>
                        <td class="cellContent">
                            <asp:TextBox ID="tbCountry" ReadOnly="true" runat="server" CssClass="srtsTextBox_medium"></asp:TextBox>
                        </td>
                        <td class="cellContent">
                            <asp:TextBox ID="tbMailCountry" ReadOnly="true" runat="server" CssClass="srtsTextBox_medium"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="cellCaption srtsLabel_medium">Zip Code
                        </td>
                        <td class="cellContent">
                            <asp:TextBox ID="tbZipCode" ReadOnly="true" runat="server" CssClass="srtsTextBox_medium"></asp:TextBox>
                        </td>
                        <td class="cellContent">
                            <asp:TextBox ID="tbMailZipCode" ReadOnly="true" runat="server" CssClass="srtsTextBox_medium"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="cellCaption srtsLabel_medium">Is CONUS
                        </td>
                        <td class="cellContent">
                            <asp:TextBox ID="tbIsConus" ReadOnly="true" runat="server" CssClass="srtsTextBox_medium"></asp:TextBox>
                        </td>
                        <td class="cellContent">
                            <asp:TextBox ID="tbMailIsConus" ReadOnly="true" runat="server" CssClass="srtsTextBox_medium"></asp:TextBox>
                        </td>
                    </tr>
                </table>
            </div>
            <div class="BeigeBoxFooter"></div>
        </div>
    </asp:Panel>

    <div style="text-align: center">
        <asp:Button ID="btnUpdate" runat="server" CssClass="srtsButton" OnClick="btnUpdate_Click" Text="Edit Site" />
        <asp:Button ID="btnAdd" runat="server" CssClass="srtsButton" OnClick="btnAdd_Click" Text="Add New Site" />
        <asp:Button ID="btnCancel" runat="server" CssClass="srtsButton" Text="Cancel" OnClick="btnCancel_Click" />
    </div>
</asp:Content>