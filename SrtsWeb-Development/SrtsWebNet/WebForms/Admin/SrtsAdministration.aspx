<%@ Page Title="" Language="C#" MasterPageFile="~/SrtsMaster.Master" AutoEventWireup="true" CodeBehind="SrtsAdministration.aspx.cs" Inherits="SrtsWeb.Admin.SrtsAdministration" %>

<%@ MasterType VirtualPath="~/srtsMaster.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="contentClinicName" ContentPlaceHolderID="ContentTop_Title_Right" runat="server">
    <asp:UpdatePanel ID="UpdatePanel2" runat="server">
        <ContentTemplate>
            <asp:Literal ID="litContentTop_Title_Right" runat="server"></asp:Literal>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

<asp:Content ID="contentMenuClinic" runat="server" ContentPlaceHolderID="contentSubMenu">
    <div class="button">
        <ul>
        </ul>
    </div>
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="MainContent" runat="server">
    <div id="divSingleColumns" style="margin: 45px 0px 0px 10px;">
        <div class="box_fullinner_top"></div>
        <div class="box_fullinner_content">
            <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <ajaxToolkit:TabContainer ID="tbcAdministration" runat="server" CssClass="tabHeader" AutoPostBack="true" OnActiveTabChanged="ActiveTabChanged">
                        <ajaxToolkit:TabPanel ID="tbpAdministration" runat="server" ClientIDMode="Static">
                            <HeaderTemplate>
                                <div>Administration</div>
                            </HeaderTemplate>
                            <ContentTemplate>
                                <div class="tabContent">
                                    <h1 style="border-bottom: solid 1px #E7CFAD"><span class="colorBurgandy">SRTSweb Administration</span></h1>
                                    <table>
                                        <tr>
                                            <td>
                                                <srts:AdminUnlockUser ID="AdminUnlockUser1" runat="server" />
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                            </ContentTemplate>
                        </ajaxToolkit:TabPanel>

                        <ajaxToolkit:TabPanel ID="tbpUserManager" runat="server" ClientIDMode="Static">
                            <HeaderTemplate>
                                <div>&nbsp;&nbsp;User Manager</div>
                            </HeaderTemplate>
                            <ContentTemplate>
                                <div class="tabContent">
                                </div>
                            </ContentTemplate>
                        </ajaxToolkit:TabPanel>

                        <ajaxToolkit:TabPanel ID="tbpManageUserRoles" runat="server" ClientIDMode="Static">
                            <HeaderTemplate>
                                <div>&nbsp;&nbsp;Manage User Roles</div>
                            </HeaderTemplate>
                            <ContentTemplate>
                                <div class="tabContent">
                                </div>
                            </ContentTemplate>
                        </ajaxToolkit:TabPanel>

                        <ajaxToolkit:TabPanel ID="tbpCreateUsers" runat="server" ClientIDMode="Static">
                            <HeaderTemplate>
                                <div>&nbsp;&nbsp;Create New User</div>
                            </HeaderTemplate>
                            <ContentTemplate>
                                <div class="tabContent">
                                </div>
                            </ContentTemplate>
                        </ajaxToolkit:TabPanel>
                    </ajaxToolkit:TabContainer>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
        <div class="box_fullinner_bottom"></div>
    </div>
</asp:Content>