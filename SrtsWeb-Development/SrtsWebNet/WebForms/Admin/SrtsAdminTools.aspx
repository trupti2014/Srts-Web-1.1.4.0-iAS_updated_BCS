<%@ Page Title="" Language="C#" MasterPageFile="~/srtsMaster.Master" AutoEventWireup="true" CodeBehind="SrtsAdminTools.aspx.cs" Inherits="SrtsWeb.Admin.SrtsAdminTools" %>

<%@ MasterType VirtualPath="~/srtsMaster.Master" %>

<asp:Content ID="SrtsAdminToolsHead" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:Content ID="SrtsAdminToolsBody" ContentPlaceHolderID="MainContent" runat="server">
    <asp:ScriptManagerProxy ID="smpProxy" runat="server">
        <Scripts>
            <asp:ScriptReference Path="~/Scripts/jqGrid/jquery.jqgrid.min.js" />
            <asp:ScriptReference Path="~/Scripts/jqGrid/grid.locale-en.min.js" />
            <asp:ScriptReference Path="~/Scripts/AdminTools/admintools.js" />
        </Scripts>
    </asp:ScriptManagerProxy>
    <div id="divAdminTools" runat="server" style="margin-top: -20px;">
        <div id="divAdminButtons" runat="server">
            <asp:Button ID="btnSOT" CssClass="AdminToolsSelect" Text="Site Orders" runat="server" OnClientClick="return false;"></asp:Button>
            <asp:Button ID="btnGZT" CssClass="AdminToolsSelect" Text="GEYES Zipcodes" runat="server" OnClientClick="return false;"></asp:Button>
            <asp:Label ID="lblUDPChange" runat="server" />
        </div>
        <asp:UpdatePanel ID="udpSOT" runat="server" UpdateMode="Conditional">
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="btnSOT" EventName="Click" />
                <asp:AsyncPostBackTrigger ControlID="btnGZT" EventName="Click" />
            </Triggers>
            <ContentTemplate>
                Select a site:
                    <asp:DropDownList ID="ddlSiteCode" runat="server" Width="350px"></asp:DropDownList>
                &nbsp&nbsp&nbspStart Date:
                    <input type="text" id="dpkStartDate" />
                &nbsp&nbsp&nbspEnd Date:
                    <input type="text" id="dpkEndDate" />
                &nbsp&nbsp&nbsp<input type="button" onclick="javascript: LoadSiteOrdersGrid();" value="Get Orders" />

                <br />
                <div id="divGridHolder" style="text-align: center;">
                    <table id="GridData" style="border-collapse: separate;"></table>
                    <div id="GridPager"></div>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
        <asp:UpdatePanel ID="udpGZT" runat="server" UpdateMode="Conditional">
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="btnSOT" EventName="Click" />
                <asp:AsyncPostBackTrigger ControlID="btnGZT" EventName="Click" />
            </Triggers>
            <ContentTemplate>
                <script type="text/javascript">
                    Sys.Application.add_load(BindGZTEvents);
                </script>
                <asp:DropDownList ID="ddlGeyesZips" runat="server" AutoPostBack="true" Width="150px"></asp:DropDownList>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</asp:Content>