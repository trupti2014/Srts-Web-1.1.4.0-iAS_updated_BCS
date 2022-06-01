<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ucSiteAdministratorsLookup.ascx.cs" Inherits="SrtsWeb.UserControls.ucSiteAdministratorsLookup" %>

<style type="text/css">

        .errorStyle {
            color: red;
        }

        .cellCaption {
            width: 15%;
            text-align: left;
        }

        .cellContent {
            width: 35%;
            text-align: left;
        }

        .srtsTextBox_medium {
            position: relative !important;
        }

        .srtsDropDown_medium {
            position: relative !important;
        }

</style>

<asp:UpdatePanel ID="upSiteAdministratorsLookup" runat="server" UpdateMode="conditional" ChildrenAsTriggers="true">
    <ContentTemplate>

        <%--  Search--%>
        <div class="w3-row" style="margin-top: 5px">
            <div class="BeigeBoxContainer" style="margin: 10px 0px 0px 0px">
                <div class="BeigeBoxHeader" style="text-align: left; position: relative; top: 0px; padding: 12px 10px 3px 15px">
                    <span class="label">Search</span>
                    <h1 style="float: right; padding-right: 20px">Select a site or enter site code to view site admin information.</h1>
                </div>
                <div class="BeigeBoxContent padding" style="padding-top: 10px; margin-top: 0px; margin-left: 10px; height: auto">
                    <asp:Panel ID="pnlSearch" runat="server" style="margin-bottom: 55px" DefaultButton="btnSearch">
                        <div>
                            <div style="float: left;" >
                                <span class="srtsLabel_medium" style="margin-left: 0px;">Select a site:</span><br />
                                <asp:DropDownList ID="ddlSiteCode" runat="server" CssClass="srtsDropDown_medium"
                                              Width="475px" OnSelectedIndexChanged="ddlSiteCode_SelectedIndexChanged" AutoPostBack="True">
                                </asp:DropDownList>
                                <ajaxToolkit:ListSearchExtender runat="server" TargetControlID="ddlSiteCode" ID="lseddlSiteCode" Enabled="True" PromptText="Type to search" QueryTimeout="2" PromptCssClass="listSearchPromptOverlay" QueryPattern="Contains"></ajaxToolkit:ListSearchExtender> <%----%>
                            </div>
                            <div style="float: left; margin-top: 15px; margin-left: 20px;">
                                <b><span class="srtsLabel_medium" style="margin-left: 0px;">OR</span></b>
                            </div>
                            <div style="float: left; margin-left: 20px;">
                                <span class="srtsLabel_medium">Enter Site Code:</span><br />
                                <asp:TextBox ID="tbSiteCode" runat="server" MaxLength="6" CssClass="srtsTextBox_small" ></asp:TextBox><br /><br />
<%--                                <asp:CustomValidator id="cvtbSiteCode"  runat="server" ControlToValidate="tbSiteCode" ValidationGroup="sc" Display="Static" ValidateEmptyText="true" ErrorMessage="Please enter a valid site code."
                                    ForeColor="Red" EnableClientScript="false" OnServerValidate="cvtbSiteCode_ServerValidate" />--%>
                                <asp:Label ID="lblError" runat="server" CssClass="srtsLabel_medium" Text="Please enter a valid site code." Visible="False" ForeColor="Red"></asp:Label><br />
                            </div>
                            <div style="margin: 0px 0px 0px 0px; border: none; float: right;">
                                <asp:Button ID="btnSearch" runat="server" CssClass="srtsButton" ClientIDMode="Static" ValidationGroup="sc" Text="Search" CausesValidation="true" OnCommand="btnSearch_Click" /> 
                            </div>
                        </div>
                    </asp:Panel>
                </div>
                <div class="BeigeBoxFooter"></div>
            </div>
        </div>

        <%--  Search Results--%>
        <div class="w3-row" style="margin-top: 5px">
            <div class="BeigeBoxContainer" style="margin: 10px 0px 0px 0px">
                <div class="BeigeBoxHeader" style="text-align: left; position: relative; top: 0px; padding: 12px 10px 3px 15px">
                    <span class="label">Search Results</span>
                </div>
             <div class="BeigeBoxContent padding" style="padding-top: 10px; margin-top: 0px; margin-left: 10px; max-height: 150px; overflow-y: scroll; height: auto">
                 <asp:ListView ID="lvSiteAdminSearchResults" runat="server">
                    <ItemTemplate>
                            <table style="width: 90%; padding-bottom: 10px; padding-top: 10px">
                            <tr style="width: 100%;">
                                <td class="cellCaption srtsLabel_medium">Site Name:
                                </td>
                                <td class="cellContent">
                                    <asp:TextBox ID="tbSiteName" ReadOnly="true" runat="server" CssClass="srtsTextBox_medium" Text='<%# Eval("SiteCode")%>' ></asp:TextBox>
                                </td>
                                <td class="cellCaption srtsLabel_medium">Address:
                                </td>
                                <td class="cellContent">
                                    <asp:TextBox ID="tbAddress1" ReadOnly="true" runat="server" CssClass="srtsTextBox_medium" Text='<%# Eval("Address1")%>' ></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td class="cellCaption srtsLabel_medium">Admin Name:
                                </td>
                                <td class="cellContent">
                                    <asp:TextBox ID="tbAdminName" ReadOnly="true" runat="server" CssClass="srtsTextBox_medium" Text='<%# Eval("FullName")%>'></asp:TextBox>
                                </td>
                                <td class="cellCaption srtsLabel_medium">
                                </td>
                                <td class="cellContent">
                                    <asp:TextBox ID="tbAddress2" ReadOnly="true" runat="server" CssClass="srtsTextBox_medium" Text='<%# Eval("Address2")%>'></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td class="cellCaption srtsLabel_medium">Phone:
                                </td>
                                <td class="cellContent">
                                    <asp:TextBox ID="tbRegPhoneNumber" ReadOnly="true" runat="server" CssClass="srtsTextBox_medium" Text='<%# Eval("RegPhoneNumber")%>'></asp:TextBox>
                                </td>
                                <td class="cellCaption srtsLabel_medium">
                                </td>
                                <td class="cellContent">
                                    <asp:TextBox ID="tbAddress3" ReadOnly="true" runat="server" CssClass="srtsTextBox_medium" Text='<%# Eval("Address3")%>'></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td class="cellCaption srtsLabel_medium">DSN Phone:
                                </td>
                                <td class="cellContent">
                                    <asp:TextBox ID="tbDSNPhoneNumber" ReadOnly="true" runat="server" CssClass="srtsTextBox_medium" Text='<%# Eval("DSNPhoneNumber")%>'></asp:TextBox>
                                </td>
                                <td class="cellCaption srtsLabel_medium">
                                </td>
                                <td class="cellContent">
                                    <asp:TextBox ID="tbCityStateZip" ReadOnly="true" runat="server" CssClass="srtsTextBox_medium" Text='<%# Eval("CityStateZipCodeCombination")%>'></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td class="cellCaption srtsLabel_medium">Email:
                                </td>
                                <td class="cellContent">
                                    <asp:TextBox ID="tbEmailAddress" ReadOnly="true" runat="server" CssClass="srtsTextBox_medium" Text='<%# Eval("EmailAdddress")%>'></asp:TextBox>
                                </td>
                                <td class="cellCaption srtsLabel_medium">
                                </td>
                                <td class="cellContent">
                                    <asp:TextBox ID="tbCountry" ReadOnly="true" runat="server" CssClass="srtsTextBox_medium" Text='<%# Eval("Country")%>'></asp:TextBox>
                                </td>
                            </tr>
                        </table>
                </ItemTemplate>
                <EmptyDataTemplate>
                    <p>No search results returned.</p>
                </EmptyDataTemplate>
            </asp:ListView>
                </div>
                <div class="BeigeBoxFooter"></div>
            </div>
        </div>
        <asp:ScriptManagerProxy ID="smpSiteAdministratorsLookup" runat="server">
            <Scripts>
  <%--              <asp:ScriptReference Path="~/Scripts/Admin/SiteAdministratorsLookup.js" />--%>
            </Scripts>
        </asp:ScriptManagerProxy>

    </ContentTemplate>
    <Triggers>
        <asp:AsyncPostBackTrigger ControlID="btnSearch" EventName="Click" />
        <asp:AsyncPostBackTrigger ControlID="ddlSiteCode" EventName="SelectedIndexChanged" />
    </Triggers>
</asp:UpdatePanel>

