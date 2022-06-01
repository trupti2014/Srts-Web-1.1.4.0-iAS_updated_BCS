<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/SrtsMaster.Master" CodeBehind="getCertificateInfo.aspx.cs" Inherits="SrtsWeb.Account.CACcert.getCertificateInfo" %>

<%@ MasterType VirtualPath="~/srtsMaster.Master" %>

<asp:Content ID="contentMainContent_Public" ContentPlaceHolderID="MainContent_Public" runat="server">
    <div>
        <div id="divMessages" class="align_center">
            <asp:Label ID="lblsuccess" runat="server" Text="" Visible="false"></asp:Label>
            <asp:Label ID="lblfail" runat="server" Text="" Visible="false"></asp:Label>
            <asp:LinkButton ID="NonProxyServer" runat="server" OnClick="NonProxyServer_Click" PostBackUrl="#" Visible="false">OK</asp:LinkButton>
        </div>

        <div id="divSiteRoles">

            <asp:GridView ID="gvSitesRoles" runat="server"
                CellPadding="4" ForeColor="#333333" GridLines="None" DataKeyNames="UserName"
                HeaderStyle-HorizontalAlign="Left" RowStyle-HorizontalAlign="Left"
                CssClass="mGrid" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                RowStyle-Wrap="false" AutoGenerateColumns="False"
                OnSelectedIndexChanged="gvSitesRoles_SelectedIndexChanged"
                OnRowDataBound="gvSitesRoles_RowDataBound"
                Width="40%" HorizontalAlign="Center">
                <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                <Columns>
                    <asp:CommandField ButtonType="Button" ShowSelectButton="True" />
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:Label ID="lblUserName" runat="server" Text="" Font-Bold="true" Font-Underline="true"></asp:Label>
                            <div style="margin-left: 6px;">
                                <div>
                                    <asp:Label ID="lblRole" runat="server" Text=""></asp:Label>
                                </div>
                                <div style="margin-left: 6px;">
                                    <asp:Label ID="Label111" runat="server" Text="Available Sites:">
                                    </asp:Label>
                                    <div style="margin: 3px 0px 0px 6px">
                                        <asp:Label ID="lblSites" runat="server" Text=""></asp:Label>
                                    </div>
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <EditRowStyle BackColor="#999999" />
                <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
                <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
                <SelectedRowStyle BackColor="#FFFF99" Font-Bold="True" ForeColor="#333333" />
                <SortedAscendingCellStyle BackColor="#E9E7E2" />
                <SortedAscendingHeaderStyle BackColor="#506C8C" />
                <SortedDescendingCellStyle BackColor="#FFFDF8" />
                <SortedDescendingHeaderStyle BackColor="#6F8DAE" />
            </asp:GridView>
            <br />
            <br />
        </div>
    </div>
</asp:Content>