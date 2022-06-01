<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ucMembershipUserRoles.ascx.cs"
    Inherits="SrtsWeb.UserControls.ucMembershipUserRoles" EnableViewState="true" %>
<script type="text/javascript">

    function filterList() {
        var tb = $('#<%=FindUser.ClientID%>').val();
        $('#<%=UsersListBox.ClientID%> option').filter(function () {
            return $(this).text().toLowerCase() == tb.toLowerCase();
        }).prop('selected', true).blur();
    };

    $(function () {
        var prm = Sys.WebForms.PageRequestManager.getInstance();
        prm.add_endRequest(
            function (s, e) {
                filterList();
            });
    }).on('change', '#<%=UsersListBox.ClientID%>', function () {
        var selVal = $(this).find('option:selected').text();
        $('#<%=FindUser.ClientID%>').val(selVal);
    });
</script>

<asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>
        <asp:Panel ID="rolesPanel" runat="server">
            <div id="UsrMgmtRoleMgmt" style="margin: 0px 0px 15px 0px;">
                <div id="RoleMgmtHead" style="width: 185px; margin: auto;">
                    <h1 style="font-weight: bold;">Add & Remove User Roles</h1>
                </div>
                <div id="RoleMgmtControls" style="width: 100%; height: 150px; margin: auto;">
                    <div id="RoleMgmtSearchContainer" style="width: 48%; float: left;">
                        <div id="RoleMgmtSearch" style="width: 200px; float: right;">
                            <p class="srtsLabel_medium" style="font-weight: bold;">Search By Username</p>
                            <asp:TextBox ID="FindUser" runat="server" CssClass="srtsTextBox_small" />
                            <br />
                            <br />
                            <div id="RoleMgmtSearchBtn" style="margin: 0px 0px 0px -8px">
                                <asp:Button ID="bFindUser" runat="server" CssClass="srtsButton" OnClientClick="filterList();" Text="Search Local" />
                            </div>
                        </div>
                    </div>
                    <div id="RoleMgmtDivider" style="margin: 0px 15px 0px 15px; width: 0px; height: 165px; border-left: 1px solid grey; float: left"></div>
                    <div id="RoleMgmtSelectContainer" style="width: 48%; float: left;">
                        <div id="RoleMgmtSelect" style="width: 200px; float: left;">
                            <p class="srtsLabel_medium" style="font-weight: bold;">&nbsp&nbsp&nbsp&nbsp&nbsp Select A User</p>
                            <asp:ListBox ID="UsersListBox" Rows="8" SelectionMode="Single" runat="server" Width="200px"
                                AutoPostBack="true" OnSelectedIndexChanged="UsersListBox_SelectedIndexChanged" />
                        </div>
                    </div>
                </div>
            </div>
            <br />
            <hr style="width: 50%; text-align: center;" />

            <div id="RoleMgmtConfirmRemoval" runat="server" style="width: 370px; margin: auto; text-align: center;" visible="false">
                <p class="srtsLabel_medium" style="font-weight: bold;">Are you sure you want to remove this users' last role?</p>
                <p class="srtsLabel_medium" style="font-weight: bold;">They will no longer have access to SRTSWeb!</p>
                <div id="RoleMgmtConfirmRemovalYes" style="width: 47%; float: left; text-align: right;">
                    <asp:Button ID="btnRoleMgmtYes" runat="server" CssClass="srtsButton" OnClick="btnRoleMgmtYes_Click" Text="Yes" />
                </div>
                <div id="RoleMgmtConfirmRemovalNo" style="width: 49%; float: left;">
                    <asp:Button ID="btnRoleMgmtNo" runat="server" CssClass="srtsButton" OnClick="btnRoleMgmtNo_Click" Text="No" />
                </div>
            </div>

            <table style="width: 100%; text-align: center; margin: 15px 0px 30px 0px;">
                <tr>
                    <td>
                        <fieldset style="float: right;">
                            <legend>Available Roles</legend>
                            <asp:ListBox ID="lbAvailable" runat="server" SelectionMode="Multiple" Width="50%" Height="140px"></asp:ListBox>
                        </fieldset>
                    </td>
                    <td style="vertical-align: middle;">
                        <asp:ImageButton ID="bAddRole" runat="server" ImageUrl="~/Styles/images/img_arrow_right.png" OnClick="bAddRole_Click" /><br />
                        <asp:ImageButton ID="bRemRole" runat="server" ImageUrl="~/Styles/images/img_arrow_left.png" OnClick="bRemRole_Click_GetUserRoles" />
                    </td>
                    <td>
                        <fieldset style="float: left;">
                            <legend>Assigned Roles</legend>
                            <asp:ListBox ID="lbAssigned" runat="server" SelectionMode="Multiple" Width="50%" Height="140px"></asp:ListBox>
                        </fieldset>
                    </td>
                </tr>
                <tr>
                    <td colspan="3">
                        <div style="margin-top: 5px;">
                            <asp:Label ID="lblRoleMsg" runat="server" ForeColor="Red" Visible="false" />
                        </div>
                    </td>
                </tr>
            </table>
            <hr style="margin-bottom: 30px; color: #E7CFAD;" />
            <fieldset>
                <div id="UsersInRoleHead" style="width: 230px; margin: auto;">
                </div>
                <legend>
                    <h1 style="font-weight: bold;">View &amp; Remove Users By Role</h1>
                </legend>
                </div>
                <div style="float: left;">
                    <p class="srtsLabel_medium" style="font-weight: bold;">&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp Select Role</p>
                    <asp:ListBox ID="RolesListBox" runat="server" Rows="8" AutoPostBack="true" SelectionMode="Single" Width="200px"
                        OnSelectedIndexChanged="RolesListBox_SelectedIndexChanged" Enabled="true" />
                </div>
                <div style="margin: 23px 0px 0px 30px; float: left;">
                    <asp:GridView ID="UsersInRoleGrid" runat="server" AutoGenerateColumns="false" CellPadding="4" CellSpacing="0" GridLines="None"
                        OnRowCommand="UsersInRoleGrid_RemoveFromRole" OnPageIndexChanging="UsersInRoleGrid_PageIndexChanging"
                        AllowPaging="true" PagerStyle-CssClass="pgr">
                        <HeaderStyle BackColor="navy" ForeColor="white" />
                        <PagerSettings FirstPageText="&lt;&lt; First" LastPageText="Last &gt;&gt;" Mode="NextPreviousFirstLast" NextPageText="Next &gt;" Position="TopAndBottom" PreviousPageText="&lt; Previous" />
                        <Columns>
                            <asp:TemplateField HeaderText="User Name">
                                <ItemTemplate>
                                    <%# Container.DataItem.ToString() %>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:ButtonField ButtonType="Link" ControlStyle-CssClass="srtsButton" Text="Remove From Role" />
                        </Columns>
                    </asp:GridView>
                    <br />
                    <span class="failureNotification">
                        <asp:Label ID="MsgAdd" runat="server"></asp:Label>
                    </span>
                    <br />
                    <span class="failureNotification">
                        <asp:Label ID="MsgRole" runat="server"></asp:Label>
                    </span>
                </div>
            </fieldset>
        </asp:Panel>
    </ContentTemplate>
</asp:UpdatePanel>