<%@ Page Title="" MaintainScrollPositionOnPostback="true" Language="C#" MasterPageFile="~/srtsMaster.Master" AutoEventWireup="true" EnableEventValidation="false"
    CodeBehind="SrtsUserManager.aspx.cs" Inherits="SrtsWeb.Admin.SrtsUserManager" EnableViewState="true" %>

<%@ MasterType VirtualPath="~/srtsMaster.Master" %>
<asp:Content ID="HeadContent1" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="../../Styles/PassFailConfirm.css" rel="stylesheet" />
    <link rel="stylesheet" type="text/css" href="../../Styles/w3.css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:ScriptManagerProxy ID="smpUserManagement" runat="server">
        <Scripts>
            <asp:ScriptReference Path="~/Scripts/Global/PassFailConfirm.js" />
            <asp:ScriptReference Path="~/Scripts/UserManagement/UserManagement.js" />
        </Scripts>
    </asp:ScriptManagerProxy>
    <asp:UpdatePanel ID="upUserMgmt" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
        <ContentTemplate>
            <div id="divSearchUser" runat="server" class="padding">
                <div style="min-height: 30px; max-height: 200px; width: 80%; text-align: center;">
                    <div id="divSearchUserStatus" style="width: 100%; text-align: center;">
                    </div>
                </div>

                <div style="margin-bottom: 15px;">
                    <asp:Panel ID="pnlSearch" runat="server" DefaultButton="bSearch">
                        <asp:Label ID="Label1" runat="server" CssClass="srtsLabel_medium" Text="Enter a username, or a partial username to get a list of matching user accounts."></asp:Label><br />
                        <div style="float: left; margin-top: 10px; display: inline-block;">
                            <asp:TextBox ID="tbName" runat="server" Width="250px"></asp:TextBox>
                        </div>
                        <div style="float: left; display: inline-block;">
                            <asp:Button ID="bSearch" runat="server" Text="Search" OnClick="bSearch_Click" CssClass="srtsButton" />
                            <asp:Button ID="bShowAll" runat="server" Text="Show All" OnClick="bShowAll_Click" CssClass="srtsButton" />
                        </div>
                    </asp:Panel>
                </div>

                <asp:UpdatePanel ID="upUserGrid" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
                    <ContentTemplate>
                        <asp:GridView ID="gvUsers" runat="server" AllowPaging="true" AutoGenerateColumns="false"
                            CssClass="mGrid" AllowSorting="true" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                            HeaderStyle-HorizontalAlign="Left" RowStyle-HorizontalAlign="Left"
                            OnPageIndexChanging="gvUsers_PageIndexChanging" DataKeyNames="UserName" PageSize="20"
                            OnRowCommand="gvUsers_RowCommand" EnableViewState="true" OnRowDataBound="gvUsers_RowDataBound">
                            <PagerSettings Mode="NextPreviousFirstLast" Position="TopAndBottom" FirstPageText="<< First"
                                NextPageText="Next >" PreviousPageText="< Previous" LastPageText="Last >>" />
                            <Columns>
                                <asp:TemplateField>
                                    <ItemTemplate>
                                        <asp:LinkButton ID="lnbUserName" CommandArgument='<%#Eval("UserName") %>' Text="Select" runat="server" CommandName="Select"></asp:LinkButton>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <ItemTemplate>
                                        <asp:Label ID="lblUserName" runat="server"></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="Email" HeaderText="Email" />
                                <asp:BoundField DataField="LastLoginDate" HeaderText="Last Login Date" />
                            </Columns>
                        </asp:GridView>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="gvUsers" EventName="PageIndexChanging" />
                        <asp:AsyncPostBackTrigger ControlID="gvUsers" EventName="RowCommand" />
                    </Triggers>
                </asp:UpdatePanel>
            </div>

            <div id="divUser" runat="server">
                <div style="min-height: 30px; max-height: 200px; width: 80%; text-align: center;">
                    <div id="divUserManagerStatus" style="width: 100%; text-align: center;">
                    </div>
                </div>

                <asp:ValidationSummary ID="Summary" runat="server" ValidationGroup="enter" />

                <div class="padding">
                    <div style="float: left;">
                        <h1>User Profile Information</h1>
                    </div>
                    <div style="float: right; margin-right: 10%;">
                        <asp:LinkButton ID="lnbBackToSearch" runat="server" Text="Back To Search" OnClick="btnCancel_Click"></asp:LinkButton>
                    </div>
                </div>
                <div style="clear: both;" class="padding">

                    <div id="divApprove" runat="server">
                        <h2><span style="color: red;">Approve User</span></h2>
                        <div style="height: 25px; margin: 10px 100px 10px 20px; padding: 10px 0px; border-top: 2px solid #E7CFAD; border-bottom: 1px solid #E7CFAD;">
                            <asp:UpdatePanel ID="upApprove" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <asp:CheckBox ID="cbApprove" runat="server" TextAlign="Right" Text="Check to approve user for this site." AutoPostBack="true" OnCheckedChanged="cbApprove_CheckedChanged" />
                                </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="cbApprove" EventName="CheckedChanged" />
                                </Triggers>
                            </asp:UpdatePanel>
                        </div>
                    </div>

                    <h2 style="clear: both; margin-top: 30px;">User Data</h2>
                    <div style="height: 70px; margin: 10px 100px 10px 20px; padding: 10px 0px; border-top: 2px solid #E7CFAD; border-bottom: 1px solid #E7CFAD;">
                        <div style="margin: 0px 10px 10px 0px; float: left; display: inline-block;">
                            <asp:Label ID="lblLastName" runat="server" CssClass="srtsLabel_medium" AssociatedControlID="tbLastName">Last Name: </asp:Label>
                            <asp:TextBox ID="tbLastName" runat="server" ReadOnly="True" />
                        </div>
                        <div style="margin: 0px 10px 10px 0px; float: left; display: inline-block;">
                            <asp:Label ID="lblFirstName" runat="server" CssClass="srtsLabel_medium" AssociatedControlID="tbFirstName">First Name: </asp:Label>
                            <asp:TextBox ID="tbFirstName" runat="server" ReadOnly="True" />
                        </div>

                        <div style="margin: 0px 10px 10px 0px; float: left; display: inline-block;">
                            <asp:Label ID="lblMiddleName" runat="server" CssClass="srtsLabel_medium" AssociatedControlID="tbMiddleName">Middle Name: </asp:Label>
                            <asp:TextBox ID="tbMiddleName" runat="server" ReadOnly="True" />
                        </div>

                        <div style="clear: both;"></div>

                        <div style="margin: 0px 10px 10px 0px; float: left; display: inline-block;">
                            <asp:Label ID="lbProfileEmail" runat="server" CssClass="srtsLabel_medium">Email Address: </asp:Label>
                            <asp:TextBox ID="tbProfileEmail" runat="server" TabIndex="5"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvProfileEmailRequired" runat="server" ControlToValidate="tbProfileEmail" Display="None"
                                CssClass="failureNotification" ErrorMessage="E-mail is required." ToolTip="E-mail is required."
                                ValidationGroup="enter">*</asp:RequiredFieldValidator>
                            <asp:RegularExpressionValidator ID="revProfileEmailValid" runat="server" Display="None"
                                ValidationExpression="^([a-zA-Z][\w\.-]*[a-zA-Z0-9]@[a-zA-Z0-9][\w\.-]*[a-zA-Z0-9]\.[a-zA-Z][a-zA-Z\.]*[a-zA-Z]){1,70}$"
                                CssClass="failureNotificationSummary" ErrorMessage="Invalid email format entered" ControlToValidate="tbProfileEmail"
                                ValidationGroup="enter"></asp:RegularExpressionValidator>
                        </div>
                        <div style="margin: 0px 10px 10px 0px; float: left; display: inline-block;">
                            <asp:Label ID="Label99" runat="server" CssClass="srtsLabel_medium">Is User CAC Enabled: </asp:Label>
                            <asp:Label ID="lblCacEnabled" runat="server" />
                        </div>
                    </div>

                    <div id="divProfileData" runat="server">
                        <h2 style="clear: both; margin-top: 30px;">User's Primary Site</h2>
                        <div style="height: 95px; margin: 10px 100px 10px 20px; padding: 10px 0px; border-top: 2px solid #E7CFAD; border-bottom: 1px solid #E7CFAD;">
                            <asp:Label ID="lblCurrentlyAssigned" runat="server" CssClass="srtsLabel_medium"></asp:Label>
                            <div style="margin-top: 8px;">
                                <asp:Label ID="Label111" runat="server" Text="Use the drop down list to change the users primary site.<BR>NOTE: Once changed only the admin and the new site can modify this users record." />
                                <div style="margin: 8px 0px 0px 15px;">
                                    <asp:DropDownList ID="ddlPrimarySiteSelection" runat="server"></asp:DropDownList>
                                </div>
                            </div>
                        </div>

                        <h2 style="clear: both; margin-top: 30px;">User Location(s)</h2>
                        <div style="height: 220px; margin: 10px 100px 10px 20px; padding: 10px 0px; border-top: 2px solid #E7CFAD; border-bottom: 1px solid #E7CFAD;">
                            <asp:UpdatePanel ID="upSites" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <table style="width: 100%; text-align: center; margin: 15px 0px 30px 0px;">
                                        <tr>
                                            <td>
                                                <fieldset style="float: right;">
                                                    <legend><span class="srtsLabel_medium">Available Sites</span></legend>
                                                    <ajaxToolkit:ListSearchExtender runat="server" TargetControlID="lbAvailSiteCode" ID="LSE_lbAvailSiteCode" Enabled="True" PromptText="Type to search"
                                                        QueryTimeout="2" PromptCssClass="listSearchPromptOverlay" QueryPattern="Contains" PromptPosition="Top">
                                                    </ajaxToolkit:ListSearchExtender>
                                                    <asp:ListBox ID="lbAvailSiteCode" runat="server" SelectionMode="Single" Width="80%" Height="180px" />
                                                </fieldset>
                                            </td>
                                            <td style="vertical-align: middle;">
                                                <asp:ImageButton ID="bAddSite" runat="server" ImageUrl="~/Styles/images/img_arrow_right.png" OnClick="bAddSite_Click" /><br />
                                                <asp:ImageButton ID="bRemSite" runat="server" ImageUrl="~/Styles/images/img_arrow_left.png" OnClick="bRemSite_Click" OnClientClick="return ConfirmLastSite();" />
                                            </td>
                                            <td>
                                                <asp:UpdatePanel ID="upAssgSiteApproval" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
                                                    <ContentTemplate>
                                                        <fieldset style="float: left;">
                                                            <legend><span class="srtsLabel_medium">Assigned Sites</span></legend>
                                                            <asp:ListBox ID="lbAssgSiteCode" runat="server" SelectionMode="Single" Width="80%" Height="180px"
                                                                AutoPostBack="true" OnSelectedIndexChanged="lbAssgSiteCode_SelectedIndexChanged" /><br />
                                                        </fieldset>
                                                    </ContentTemplate>
                                                    <Triggers>
                                                        <asp:PostBackTrigger ControlID="lbAssgSiteCode" />
                                                    </Triggers>
                                                </asp:UpdatePanel>
                                            </td>
                                        </tr>
                                    </table>
                                </ContentTemplate>
                                <Triggers>
                                    <asp:PostBackTrigger ControlID="bAddSite" />
                                    <asp:PostBackTrigger ControlID="bRemSite" />
                                </Triggers>
                            </asp:UpdatePanel>
                        </div>

                        <asp:UpdatePanel ID="upRoles" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
                            <ContentTemplate>
                                <h2 style="clear: both; margin-top: 30px;">Role Assignments</h2>
                                <div style="height: 180px; margin: 10px 100px 10px 20px; padding: 10px 0px; border-top: 2px solid #E7CFAD; border-bottom: 1px solid #E7CFAD;">
                                    <table style="width: 100%; text-align: center; margin: 15px 0px 30px 0px;">
                                        <tr>
                                            <td>
                                                <fieldset style="float: right;">
                                                    <legend><span class="srtsLabel_medium">Available Roles</span></legend>
                                                    <asp:ListBox ID="lbAvailable" runat="server" SelectionMode="Single" Height="140px"></asp:ListBox>
                                                </fieldset>
                                            </td>
                                            <td style="vertical-align: middle;">
                                                <asp:ImageButton ID="bAddRole" runat="server" ImageUrl="~/Styles/images/img_arrow_right.png" OnClick="bAddRole_Click" /><br />
                                                <asp:ImageButton ID="bRemRole" runat="server" ImageUrl="~/Styles/images/img_arrow_left.png" OnClick="bRemRole_Click" OnClientClick="return ConfirmLastRole();" />
                                            </td>
                                            <td>
                                                <fieldset style="float: left;">
                                                    <legend><span class="srtsLabel_medium">Assigned Roles</span></legend>
                                                    <asp:ListBox ID="lbAssigned" runat="server" SelectionMode="Single" Height="140px"></asp:ListBox>
                                                </fieldset>
                                            </td>
                                        </tr>
                                    </table>
                                </div>

                                <div id="divCms" runat="server">
                                    <h2 style="margin-top: 30px;">CMS</h2>
                                    <div style="height: 30px; margin: 10px 100px 10px 20px; padding: 10px 0px; border-top: 2px solid #E7CFAD; border-bottom: 1px solid #E7CFAD;">
                                        <asp:CheckBox ID="cbCms" runat="server" Text="Check if user will manage CMS content: " Checked="false" TextAlign="Left" CssClass="srtsLabel_medium" />
                                    </div>
                                </div>
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="bAddRole" EventName="Click" />
                                <asp:AsyncPostBackTrigger ControlID="bRemRole" EventName="Click" />
                            </Triggers>
                        </asp:UpdatePanel>
                    </div>

                    <asp:Button ID="btnSubmit" runat="server" CssClass="srtsButton" Text="Submit" OnClick="btnSubmit_Click"
                        TabIndex="7" CausesValidation="true" />
                    <asp:Button ID="btnCancel" runat="server" CssClass="srtsButton" Text="Cancel" OnClick="btnCancel_Click"
                        TabIndex="8" />
                </div>
            </div>

            <div id="divIndividualLinkDialog" style="display: none;">
                <asp:UpdatePanel ID="upLinkIndividual" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
                    <ContentTemplate>
                        <div style="text-align: center; margin: 8px;">
                            <asp:Label ID="Label11" runat="server" Text="Select location where individual is located."></asp:Label><br />
                            <asp:DropDownList ID="ddlIndividualLoc" runat="server" OnSelectedIndexChanged="ddlIndividualLoc_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                        </div>
                        <div style="text-align: center; margin: 0px 8px 8px 8px;">
                            <asp:Label ID="Label12" runat="server" Text="Select individual to link to profile."></asp:Label><br />
                            <asp:DropDownList ID="ddlLinkIndividual" runat="server"></asp:DropDownList>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
                <div style="margin: 0px 8px 8px 30px; display: inline;">
                    <asp:Button ID="bLinkIndividual" runat="server" Text="Link Individual" OnClick="bLinkIndividual_Click" />
                </div>
            </div>

            <div id="divSetSiteCode" style="display: none;">
                <asp:UpdatePanel ID="upSetSiteCode" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
                    <ContentTemplate>
                        <div style="text-align: center; margin: 8px;">
                            <asp:Label ID="Label13" runat="server" Text="Select location where individual is located."></asp:Label><br />
                            <asp:DropDownList ID="ddlProfileSite" runat="server"></asp:DropDownList>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
                <div style="margin: 0px 8px 8px 30px; display: inline;">
                    <asp:Button ID="bSetSite" runat="server" Text="Submit" OnClick="bSetSite_Click" />
                </div>
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="bSearch" />
            <asp:PostBackTrigger ControlID="bShowAll" />
            <asp:PostBackTrigger ControlID="btnSubmit" />
            <asp:PostBackTrigger ControlID="btnCancel" />
            <asp:PostBackTrigger ControlID="lnbBackToSearch" />
        </Triggers>
    </asp:UpdatePanel>

</asp:Content>