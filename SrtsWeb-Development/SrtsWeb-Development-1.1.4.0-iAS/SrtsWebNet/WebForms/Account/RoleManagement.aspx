<%@ Page Title="Register" Language="C#" MasterPageFile="~/SrtsMaster.master" AutoEventWireup="true"
    CodeBehind="RoleManagement.aspx.cs" Inherits="SrtsWeb.Account.RoleManagement" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <h2>Manage Roles and Profiles
    </h2>
    <hr />
    <div style="margin-left: 5em;">
        <h3>Select User</h3>
        <table cellpadding="3" border="0">
            <tr>
                <td valign="top">Select User:
                </td>
                <td valign="top">
                    <asp:ListBox ID="UsersListBox" DataTextField="Username" Rows="8" SelectionMode="Single"
                        BackColor="Beige" runat="server" Width="200px" />
                </td>
                <td valign="top">Find User by Username:
                </td>
                <td valign="top">
                    <asp:TextBox ID="FindUser" DataTextField="Username" Rows="8" BackColor="Beige" runat="server"
                        Width="200px" /><br />
                    <asp:Button ID="btnFindUser" Text="Submit" runat="server" CssClass="srtsButton" OnClick="btnFindUser_Click" />
                </td>
            </tr>
        </table>
        <h3>Role Membership</h3>
        <asp:Label ID="MsgRole" runat="server"></asp:Label>
        <table cellpadding="3" border="0">
            <tr>
                <td valign="top">Select Role:
                </td>
                <td valign="top">
                    <asp:ListBox ID="RolesListBox" runat="server" Rows="8" AutoPostBack="true" BackColor="Beige"
                        SelectionMode="Single" Width="200px" />
                </td>
                <td valign="top"></td>
                <td valign="top"></td>
                <td valign="top">Users In Role:
                </td>
                <td valign="top">
                    <br />
                    <asp:GridView runat="server" CellPadding="4" ID="UsersInRoleGrid" AutoGenerateColumns="false"
                        GridLines="None" CellSpacing="0" OnRowCommand="UsersInRoleGrid_RemoveFromRole">
                        <HeaderStyle BackColor="navy" ForeColor="white" />

                        <PagerSettings Mode="NextPreviousFirstLast" Position="TopAndBottom" FirstPageText="<< First"
                            NextPageText="Next >" PreviousPageText="< Previous" LastPageText="Last >>" />
                        <Columns>
                            <asp:TemplateField HeaderText="User Name">
                                <ItemTemplate>
                                    <%# Container.DataItem.ToString() %>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:ButtonField Text="Remove From Role" ControlStyle-CssClass="srtsButton" ButtonType="Link" />
                        </Columns>
                    </asp:GridView>
                </td>
            </tr>
        </table>
        <asp:Label ID="MsgAdd" runat="server"></asp:Label>
        <asp:Button ID="AddUsers" runat="server" CssClass="srtsButton" Text="Add Role To User" OnClick="AddUsers_OnClick" />
    </div>
</asp:Content>