<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ucUserProfile.ascx.cs" Inherits="SrtsWeb.UserControls.ucUserProfile" %>

<div class="tabContent_full">
    <asp:Panel ID="pnlPersonalInformation" runat="server">
        <asp:GridView ID="gvUserPersonalInformation" runat="server" AutoGenerateColumns="false" GridLines="None" CssClass="mGrid" BorderColor="#c6e7ff">
            <PagerSettings Mode="NextPreviousFirstLast" Position="TopAndBottom" FirstPageText="<< First"
                NextPageText="Next >" PreviousPageText="< Previous" LastPageText="Last >>" />
            <Columns>
                <asp:TemplateField HeaderText="Full Name">
                    <ItemTemplate>
                        <asp:Literal ID="litFirstName" runat="server" Text='<%# UserFullName()%>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Email Address">
                    <ItemTemplate>
                        <asp:Literal ID="litAddress" runat="server" Text='<%# UserEmailAddress() %>' />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </asp:Panel>
    <br />
    <asp:Panel ID="pnlLoginInformation" runat="server">
        <asp:GridView ID="gvMembershipInformation" runat="server" AutoGenerateColumns="false" GridLines="None" CssClass="mGrid" BorderColor="#c6e7ff">
            <PagerSettings Mode="NextPreviousFirstLast" Position="TopAndBottom" FirstPageText="<< First"
                NextPageText="Next >" PreviousPageText="< Previous" LastPageText="Last >>" />
            <Columns>
                <asp:TemplateField HeaderText="Login Name">
                    <ItemTemplate>
                        <asp:Literal ID="litLoginName" runat="server" Text='<%# UserLoginName()%>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Manage Password">
                    <ItemTemplate>
                        <asp:LinkButton ID="lnkResetPassword" runat="server" PostBackUrl="~/WebForms/Account/ChangePassword.aspx" ToolTip="Change Password" Text="Change Password" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Roles">
                    <ItemTemplate>
                        <asp:Literal ID="litRoles" runat="server" Text='<%# UserRoles()%>' />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </asp:Panel>
    <br />
    <asp:Panel ID="pnlFacilityInformation" runat="server">
        <asp:GridView ID="gvFacilityInformation" runat="server" AutoGenerateColumns="false" GridLines="None" CssClass="mGrid" BorderColor="#c6e7ff">
            <PagerSettings Mode="NextPreviousFirstLast" Position="TopAndBottom" FirstPageText="<< First"
                NextPageText="Next >" PreviousPageText="< Previous" LastPageText="Last >>" />
            <Columns>
                <asp:TemplateField HeaderText="Facility Name and Address">
                    <ItemTemplate>
                        <div style="padding-top:10px;text-align:left">
                        
                        <asp:Literal ID="litSiteAddressType" runat="server" Text='<%# String.Format("<b>{0} ADDRESS:</b>", Eval("AddressType"))%>' /><br />
                        <div style="padding-top:5px;margin-left:10px">
                        <asp:Literal ID="litSiteName" runat="server" Text='<%# Eval("SiteName")%>' /><br />
                        <asp:Literal ID="litSiteAddress1" runat="server" Text='<%# Eval("Address1") %>' />
                        <asp:Literal ID="litSiteAddress2" runat="server" Text='<%# Eval("Address2") %>' /><br />
                        <asp:Literal ID="litSiteCityStateZip" runat="server" Text='<%# (String.Format("{0}, {1}    {2}",Eval("City"),Eval("State"),Eval("ZipCode")))%>' />      </div>
                       </div>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Facility Type">
                    <ItemTemplate>
                        <asp:Literal ID="litSiteType" runat="server" Text='<%# Eval("SiteType") %>' />
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Center" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Facility Code">
                    <ItemTemplate>
                        <asp:Literal ID="litSiteCode" runat="server" Text='<%# Eval("SiteCode") %>' />
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Center" />
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </asp:Panel>
</div>