<%@ Page Title="" Language="C#" MasterPageFile="~/SrtsMaster.Master" AutoEventWireup="True"
    CodeBehind="ManagePatients.aspx.cs" Inherits="SrtsWebClinic.Patients.ManagePatients" EnableEventValidation="false"
    EnableViewState="true" %>

<%@ MasterType VirtualPath="~/srtsMaster.Master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style>
        .hdr_searchResults {
        float:left;
        position:relative;
        top: 20px;
        left: 40px;
        color:#0f621e;
        margin:0px;
        padding:0px;
        font-size:12px;
        }

    </style>
</asp:Content>
<asp:Content ID="contentClinicName" ContentPlaceHolderID="ContentTop_Title_Right"
    runat="server" ClientIDMode="Static">
    <asp:Literal ID="litContentTop_Title_Right" runat="server" ClientIDMode="Static"></asp:Literal>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="xBeigeBoxContainer" style="min-width: 1096px;margin-top:-15px">
        <div class="xBeigeBoxHeader" style="min-height: 25px; max-height: 25px;"></div>
        <div class="xBeigeBoxContent">
            <asp:Panel ID="pnlSearch" runat="server" DefaultButton="btnSearchLocal">
                <div class="headerBlue" style="font-size:14px;height:45px;padding:0px;text-align:left;margin:0px 0px 0px 40px">Enter search criteria below:</div>                       
                <div style="margin: -70px 0px 0px 0px">
                    <div class="gvButtonsTop" style="position:relative;left:-50px;top:5px;margin: 0px 0px 0px 0px; border: none; float: right;">
                        <asp:Button ID="btnSearchLocal" runat="server" CssClass="srtsButton" ClientIDMode="Static" OnCommand="rbSearch_Click" CommandArgument="S" CausesValidation='False' Text="Search Local" />
                        <asp:Button ID="btnSearchGlobal" runat="server" CssClass="srtsButton" OnCommand="rbSearch_Click" CommandArgument="G" CausesValidation="False" Text="Search Global" />
                    </div>
                    <div style="float: left; width: 23%; margin-left: 40px;">
                        <span class="srtsLabel_medium" style="margin-left: -130px;">Enter Last Name:</span><br />
                        <asp:TextBox ID="tbLastName" runat="server" MaxLength="25" Width="160px" CssClass="srtsTextBox_small"
                            ToolTip="Enter as much of the patients last name as you know." TabIndex="1"></asp:TextBox><br />
                        <ajaxToolkit:FilteredTextBoxExtender ID="ftb_tbLastName" runat="server" FilterType="Custom, UppercaseLetters, LowercaseLetters" ValidChars="'- " TargetControlID="tbLastName" Enabled="True" />
                    </div>
                    <div style="float: left; width: 23%; margin-left: -15px">
                        <span class="srtsLabel_medium" style="margin-left: -130px;">Enter First Name:</span><br />
                        <asp:TextBox ID="tbFirstName" runat="server" MaxLength="25" Width="160px" CssClass="srtsTextBox_small"
                            ToolTip="Optional Enter as much of the patients first name as you know." TabIndex="2"></asp:TextBox><br />
                        <ajaxToolkit:FilteredTextBoxExtender ID="ftb_tbFirstName" runat="server" FilterType="Custom, UppercaseLetters, LowercaseLetters" ValidChars="'- " TargetControlID="tbFirstName" Enabled="True" />
                    </div>
                    <div style="float: left; margin-left: -25px; margin-top: 15px;">
                        <b><span class="srtsLabel_medium" style="margin-left: -20px;">OR</span></b><br />
                    </div>
                    <div style="float: left; width: 23%; margin-left: 15px;">
                        <span class="srtsLabel_medium" style="margin-left: -80px;">Enter ID Number or SSN:</span><br />
                        <asp:TextBox ID="tbID" runat="server" MaxLength="11" ToolTip="Enter the patients full ID, SSN or the last four digits of the users SSN."
                            TabIndex="3" CssClass="srtsTextBox_small"></asp:TextBox><br />
                        <ajaxToolkit:FilteredTextBoxExtender ID="ftb_tbID" runat="server" FilterType="Custom, Numbers" ValidChars="-" TargetControlID="tbID" Enabled="True" />
                    </div>
                    <div style="margin: 65px 0px 20px 0px">
                        <div style="margin-top: 65px; display: none;">
                            <span class="srtsLabel_medium" style="float: left">Search Active Records Only:</span><br />
                            <asp:RadioButtonList ID="rblActiveOnly" runat="server" RepeatDirection="Horizontal" Style="clear: left">
                                <asp:ListItem Text="Yes" Value="true" Selected="True"></asp:ListItem>
                                <asp:ListItem Text="No" Value="false"></asp:ListItem>
                            </asp:RadioButtonList>
                        </div>
                        <asp:ValidationSummary ID="vsmPatientSearch" runat="server" Style="clear: both" />
                    </div>
                </div>
                <h1 id="hdrSearchResults" runat="server" visible="false" class="hdr_searchResults">Search results are displayed below. Click on  a row to view patient detail.</h1>
            </asp:Panel>
        </div>
        <div id="divSearchResults" runat="server" style="clear: both; width: 100%; margin: -20px auto 0px auto; text-align: center;" visible="False">
          <%--  <h1 class="colorBlue">Search results are displayed below. Click on  a row to view patient detail.</h1>--%>
           <div class="padding" style="margin:0px 0px 0px 15px">
                <asp:Literal ID="litPageMessage" runat="server" Visible="False"></asp:Literal>
                <asp:GridView ID="gvSearch" runat="server" ClientIDMode="Static" AllowSorting="True" AllowPaging="True"
                    AutoGenerateColumns="False" GridLines="None" DataKeyNames="ID" OnSorting="gvSearch_Sorting"
                    EmptyDataText="No Data Found" Width="95%" ViewStateMode="Enabled" PageSize="25"
                    CssClass="mGrid" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt" ShowHeaderWhenEmpty="true"
                    OnRowDataBound="gvSearch_RowDataBound"
                    OnPageIndexChanging="gvSearch_PageIndexChanging"
                    OnRowCreated="gvSearch_RowCreated"
                    OnRowCommand="gvSearch_RowCommand">
                    <PagerSettings Mode="NextPreviousFirstLast" FirstPageText="<< First"
                        NextPageText="Next >" PreviousPageText="< Previous" LastPageText="Last >>" />
                    <Columns>
                        <asp:ButtonField CommandName="Orders" ButtonType="Button" Text="Orders">
                            <ItemStyle Width="50px" />
                        </asp:ButtonField>
                        <asp:BoundField DataField="LastName" HeaderText="Last Name" SortExpression="LastName">
                            <HeaderStyle HorizontalAlign="Left" />
                            <ItemStyle Width="70px" />
                        </asp:BoundField>
                        <asp:BoundField DataField="FirstName" HeaderText="First Name" SortExpression="FirstName">
                            <HeaderStyle HorizontalAlign="Left" />
                            <ItemStyle Width="70px" />
                        </asp:BoundField>
                        <asp:BoundField DataField="IDNumber" HeaderText="ID Number(s) *Last Four" SortExpression="IDNumber">
                            <HeaderStyle HorizontalAlign="Left" />
                            <ItemStyle Width="140px" HorizontalAlign="Center" />
                        </asp:BoundField>
                        <asp:BoundField DataField="IDNumberTypeDescription" HeaderText="ID Type(s)" SortExpression="IDNumberTypeDescription">
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle Width="140px" />
                        </asp:BoundField>
                        <asp:BoundField DataField="StatusDescription" HeaderText="Status" SortExpression="StatusDescription">
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Left" Width="75px" />
                        </asp:BoundField>
                        <asp:BoundField DataField="BOSDescription" HeaderText="Branch" SortExpression="BOSDescription">
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Left" Width="70px" />
                        </asp:BoundField>
                        <asp:BoundField DataField="Rank" HeaderText="Grade" SortExpression="Rank">
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" Width="40px" />
                        </asp:BoundField>
                    </Columns>
                    <PagerStyle CssClass="pgr" />
                </asp:GridView>
           </div>
        </div>
    </div>
    <asp:ScriptManagerProxy ID="smpProxy" runat="server">
        <Scripts>
            <asp:ScriptReference Path="~/Scripts/Patient/ManagePatients.js" />
            <asp:ScriptReference Path="~/Scripts/Global/SrtsCustomValidators.js" />
        </Scripts>
    </asp:ScriptManagerProxy>
</asp:Content>