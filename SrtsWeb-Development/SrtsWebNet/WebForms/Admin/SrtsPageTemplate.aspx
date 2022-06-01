<%@ Page Title="" Language="C#" MasterPageFile="~/srtsMaster.Master" AutoEventWireup="true" CodeBehind="SrtsPageTemplate.aspx.cs" Inherits="SrtsWeb.Admin.SrtsPageTemplate" %>

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

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div id="divSingleColumns" style="margin: 45px 0px 0px 0px; padding: 0px 10px 0px 35px">
        <div class="box_fullinner_top"></div>
        <div class="box_fullinner_content">
            <asp:UpdatePanel ID="UpdatePanel1Template" runat="server" UpdateMode="Always">
                <ContentTemplate>

                    <ajaxToolkit:TabContainer ID="tbcTemplate" runat="server" CssClass="tabHeader" AutoPostBack="true" OnActiveTabChanged="ActiveTabChanged">

<%--                        <ajaxToolkit:TabPanel ID="tbpTemplate" runat="server" ClientIDMode="Static" Visible="false">
                            <HeaderTemplate>
                                <div>Administration</div>
                            </HeaderTemplate>
                            <ContentTemplate>
                                <div class="tabContent">
                                    <h1 style="border-bottom: solid 1px #E7CFAD"><span class="colorBurgandy">SRTSweb Administration</span></h1>
                                    <div class="padding">
                                        <div>
                                        </div>
                                    </div>
                                </div>
                            </ContentTemplate>
                        </ajaxToolkit:TabPanel>--%>
                        <ajaxToolkit:TabPanel ID="tbpanelTemplate" runat="server" ClientIDMode="Static">
                            <HeaderTemplate>
                                <div>&nbsp;&nbsp;View Reports</div>
                            </HeaderTemplate>
                            <ContentTemplate>
                                <asp:UpdatePanel ID="uplTemplate" runat="server">
                                    <ContentTemplate>
                                        <div class="tabContent">
                                            <h1 style="border-bottom: solid 1px #E7CFAD"><span class="colorBurgandy">SRTSweb Report Manager - View Reports</span></h1>
                                            <div class="padding">
                                                <div class="countHeader">
                                                    <div style="float: left; width: 375px">
                                                        Select Report to View:&nbsp;&nbsp;
                                                        <asp:DropDownList ID="ddlTemplate" runat="server" AutoPostBack="true" OnSelectedIndexChanged="GetSelectedReport">
                                                            <asp:ListItem>-- Select a Report to View --</asp:ListItem>
                                                        </asp:DropDownList>&nbsp;&nbsp;
                                                    </div>
                                                </div>
                                            </div>
                                            <asp:Label ID="Label2Template" runat="server"></asp:Label><br />
                                        </div>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </ContentTemplate>
                        </ajaxToolkit:TabPanel>
                    </ajaxToolkit:TabContainer>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
        <div class="box_fullinner_bottom"></div>
    </div>
</asp:Content>