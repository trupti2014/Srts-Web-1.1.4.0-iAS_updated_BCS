<%@ Page Title="" Language="C#" MasterPageFile="~/srtsMaster.Master" AutoEventWireup="true" CodeBehind="SrtsAccountManager.aspx.cs" Inherits="SrtsWeb.Admin.SrtsAccountManager" %>

<%@ MasterType VirtualPath="~/srtsMaster.Master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link rel="stylesheet" type="text/css" href="../../Styles/w3.css" />
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="MainContent" runat="server">
    <p style="margin:20px 25%">Select the appropriate option below to enable or disable a user account.  Choose the 'Unlock User Account' option to unlock an account that is blocked due to a password lockout.</p>
    <div class="w3-row" style="margin:20px 20px">
        <!-- Disable User Account -->
        <div class="w3-third">
            <div class="BeigeBoxContainer" style="margin: 0px 10px">
                        <div class="BeigeBoxHeader" style="padding: 12px 10px 3px 15px">
                            <span class="label">Disable User Account</span>
                        </div>
                        <div class="BeigeBoxContent padding" style="min-height: 200px">
                    <asp:UpdatePanel ID="upDisableUser" runat="server" UpdateMode="Always">
                        <ContentTemplate>
                            <srts:AdminDisableUser ID="AdminDisableUser1" runat="server" />
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </div>
        </div>

        <!-- Enable User Account -->
        <div class="w3-third">
            <div class="BeigeBoxContainer" style="margin: 0px 10px">
                <div class="BeigeBoxHeader" style="padding: 12px 10px 3px 15px">
                    <span class="label">Enable User Account</span>
                </div>
                <div class="BeigeBoxContent padding" style="min-height: 200px">
                    <asp:UpdatePanel ID="upEnableUser" runat="server" UpdateMode="Always">
                        <ContentTemplate>
                            <srts:AdminEnableUser ID="AdminEnableUser1" runat="server" />
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </div>
        </div>

        <!-- Unlock User Account -->
        <div class="w3-third">
            <div class="BeigeBoxContainer" style="margin: 0px 10px">
                <div class="BeigeBoxHeader" style="padding: 12px 10px 3px 15px">
                    <span class="label">Unlock User Account</span>
                </div>
                <div class="BeigeBoxContent padding" style="min-height: 200px">
                    <asp:UpdatePanel ID="upUnlockUser" runat="server" UpdateMode="Always">
                        <ContentTemplate>
                            <srts:AdminUnlockUser ID="AdminUnlockUser1" runat="server" />
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
