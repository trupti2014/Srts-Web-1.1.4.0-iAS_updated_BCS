<%@ Page Title="Individual Search" Language="C#" MasterPageFile="~/SrtsMaster.master"
    AutoEventWireup="true" CodeBehind="IndividualSearch.aspx.cs" Inherits="SrtsWebClinic.Individuals.IndividualSearch"
    EnableViewState="true" MaintainScrollPositionOnPostback="true" %>

<%@ MasterType VirtualPath="~/srtsMaster.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="" style="min-width: 1096px; max-width: 1096px;">
        <asp:UpdatePanel ID="upIndSearch" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
            <ContentTemplate>
                <div class="gvButtonsTop" style="margin: 50px 120px -90px 0px; border: none">
                    <asp:Button ID="btnSearchLocal" runat="server" CssClass="srtsButton" ClientIDMode="Static"
                        OnCommand="rbSearch_Click" CommandArgument="S" CausesValidation='False'
                        Text="Search Local" />
                    <asp:Button ID="btnSearchGlobal" runat="server" CssClass="srtsButton" OnCommand="rbSearch_Click"
                        CommandArgument="G" CausesValidation="False"
                        Text="Search Global" />
                </div>
                <h1 style="font-size: 20px; color: #004994; text-align: left; margin-left: 25px; border-bottom: solid 1px #E7CFAD">Enter Search Criteria</h1>
                <div style="margin: 10px 0px 60px 20px">
                    <div style="float: left; width: 200px;">
                        <span class="srtsLabel_medium" style="margin-left: -45px;">Enter Last Name:</span><br />
                        <asp:TextBox ID="tbLastname" runat="server" MaxLength="25" CssClass="srtsTextBox_small"
                            ToolTip="Enter as much of the individuals last name as you know." TabIndex="1"></asp:TextBox><br />
                    </div>
                    <div style="float: left; width: 225px;">
                        <span class="srtsLabel_medium" style="margin-left: -60px">Enter First Name:</span><br />
                        <asp:TextBox ID="tbFirstName" runat="server" MaxLength="25" CssClass="srtsTextBox_small"
                            ToolTip="Optional Enter as much of the individuals first name as you know." TabIndex="2"></asp:TextBox><br />
                    </div>
                    <div style="float: left; margin-left: -30px; margin-top: 20px;">
                        <b><span class="srtsLabel_medium">OR</span></b><br />
                    </div>
                    <div style="float: left; width: 200px; margin-left: 15px;">
                        <span class="srtsLabel_medium" style="margin-left: -30px">Enter ID Number or SSN:</span><br />

                        <asp:TextBox ID="tbID" runat="server" MaxLength="11" ToolTip="Enter the individuals full ID, SSN or the last four digits of the users SSN."
                            TabIndex="2" CssClass="srtsTextBox_small"></asp:TextBox><br />
                    </div>
                    <div style="clear: left; height: 30px; width: 800px; margin-top: 60px; margin-bottom: 20px;">
                        <div style="float: left; width: 350px;">
                            <span class="srtsLabel_medium">Select Site for Local Search:</span><br />
                            <div class="styled-select">
                                <asp:DropDownList ID="ddlSiteCode" runat="server" DataTextField="SiteCombination" DataValueField="SiteCode" CssClass="styled-select" Width="350px">
                                </asp:DropDownList>
                                <ajaxToolkit:ListSearchExtender runat="server" TargetControlID="ddlSiteCode" ID="LSE_ddlSiteCode" Enabled="True" PromptText="Type to search" QueryTimeout="2" PromptCssClass="listSearchPromptOverlay" QueryPattern="Contains"></ajaxToolkit:ListSearchExtender>
                            </div>
                        </div>
                        <div style="float: left; margin-left: 50px;">
                            <span class="srtsLabel_medium">Select Type:</span><br />
                            <asp:DropDownList ID="ddlIndividualType" runat="server" DataTextField="Text" DataValueField="Value" CssClass="styled-select" Width="150px" TabIndex="3">
                            </asp:DropDownList><br />
                            <br />
                        </div>
                    </div>
                    <asp:ValidationSummary ID="vsmPatientSearch" runat="server" CssClass="validatorSummary" />
                </div>

                <div id="divSearchResults" runat="server" style="width: 100%; margin: 0px 5px; text-align: center;" visible="False">
                    <h1 class="colorBlue">Search results are displayed below. Click on  a row to view Individual detail.</h1>
                    <div class="xtabContent_full" style="width: 90%; margin: auto;">
                        <asp:Literal ID="litPageMessage" runat="server" Visible="False"></asp:Literal>
                        <asp:GridView ID="gvSearch" runat="server" ClientIDMode="Static" AllowSorting="True" AllowPaging="True"
                            AutoGenerateColumns="False" GridLines="None" DataKeyNames="ID" OnSorting="gvSearch_Sorting"
                            Width="100%" CssClass="mGrid" ViewStateMode="Enabled" PageSize="25" OnRowDataBound="gvSearch_RowDataBound"
                            OnPageIndexChanging="gvSearch_PageIndexChanging" EmptyDataText="No Data Found">
                            <PagerSettings Mode="NextPreviousFirstLast" FirstPageText="<< First"
                                NextPageText="Next >" PreviousPageText="< Previous" LastPageText="Last >>" />
                            <AlternatingRowStyle CssClass="alt" />
                            <Columns>
                                <asp:BoundField DataField="LastName" HeaderText="Last Name" SortExpression="LastName">
                                    <HeaderStyle HorizontalAlign="Left" />
                                    <ItemStyle Width="70px" />
                                </asp:BoundField>
                                <asp:BoundField DataField="FirstName" HeaderText="First Name">
                                    <HeaderStyle HorizontalAlign="Left" />
                                    <ItemStyle Width="70px" />
                                </asp:BoundField>
                                <asp:BoundField DataField="IDNumber" HeaderText="ID Number(s) *Last Four" SortExpression="IDNumber">
                                    <HeaderStyle HorizontalAlign="Center" />
                                    <ItemStyle HorizontalAlign="Center" Width="140px" />
                                </asp:BoundField>
                                <asp:BoundField DataField="IDNumberTypeDescription" HeaderText="ID Type(s)">
                                    <HeaderStyle HorizontalAlign="Center" />
                                    <ItemStyle HorizontalAlign="Center" Width="140px" />
                                </asp:BoundField>
                                <asp:BoundField DataField="StatusDescription" HeaderText="Status">
                                    <HeaderStyle HorizontalAlign="Center" />
                                    <ItemStyle HorizontalAlign="Left" Width="75px" />
                                </asp:BoundField>
                                <asp:BoundField DataField="BOSDescription" HeaderText="Branch">
                                    <HeaderStyle HorizontalAlign="Center" />
                                    <ItemStyle HorizontalAlign="Left" Width="70px" />
                                </asp:BoundField>
                                <asp:BoundField DataField="Rank" HeaderText="Grade">
                                    <HeaderStyle HorizontalAlign="Center" />
                                    <ItemStyle HorizontalAlign="Center" Width="25px" />
                                </asp:BoundField>
                                <asp:BoundField DataField="IsPOC" HeaderText="IsPOC">
                                    <ItemStyle HorizontalAlign="Center" Width="25px" />
                                </asp:BoundField>
                            </Columns>
                            <PagerStyle CssClass="pgr" />
                        </asp:GridView>
                    </div>
                </div>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="btnSearchLocal" EventName="Command" />
                <asp:AsyncPostBackTrigger ControlID="btnSearchGlobal" EventName="Command" />
                <asp:AsyncPostBackTrigger ControlID="gvSearch" EventName="Sorting" />
                <asp:AsyncPostBackTrigger ControlID="gvSearch" EventName="RowDataBound" />
                <asp:AsyncPostBackTrigger ControlID="gvSearch" EventName="PageIndexChanging" />
            </Triggers>
        </asp:UpdatePanel>
    </div>
</asp:Content>