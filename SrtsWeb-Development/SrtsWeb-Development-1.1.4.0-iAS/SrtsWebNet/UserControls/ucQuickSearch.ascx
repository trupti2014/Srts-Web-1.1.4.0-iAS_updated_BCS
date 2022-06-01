<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ucQuickSearch.ascx.cs" Inherits="SrtsWeb.UserControls.ucQuickSearch" %>
<asp:Panel ID="pnlQs" runat="server" DefaultButton="bQuickSearch">
    <asp:Label ID="lblQs" runat="server" Text="Quick Search:" AssociatedControlID="tbQuickSearch" ForeColor="#004994" Font-Bold="true"></asp:Label>

    <asp:TextBox ID="tbQuickSearch" runat="server" ClientIDMode="Static" ForeColor="Black" Height="20px" Width="250px" BorderStyle="Solid" BorderWidth="1px" BorderColor="#E4CFAC"></asp:TextBox>

    <asp:Button ID="bQuickSearch" runat="server" Text="Go" CssClass="qsButton" ClientIDMode="Static" OnClientClick="return QuickSearchHasText($('#tbQuickSearch').val());" OnClick="bQuickSearch_Click" />
</asp:Panel>
<div id="dialogStatusChange" style="display: none;">
    <div style="margin-top: 25px; vertical-align: middle; text-align: center;">
        <asp:Button ID="bView" runat="server" CssClass="srtsButton" Text="View Order" OnClick="bView_Click" CausesValidation="false" />
        <asp:Button ID="bChangeStatus" runat="server" CssClass="srtsButton" Text="ChangeStatus" OnClick="bChangeStatus_Click" CausesValidation="false" />
    </div>
</div>
<div id="dialogPatientSearch" style="display: none;">
    <div style="margin-top: 10px; vertical-align: middle; text-align: center;">
        <asp:Button ID="bDetails" runat="server" CssClass="srtsButton" Text="View Details" OnClick="bDetails_Click" CausesValidation="false" /><br />
        <asp:Button ID="bOrders" runat="server" CssClass="srtsButton" Text="View Orders" OnClick="bOrders_Click" CausesValidation="false" />
    </div>
</div>
<script type="text/javascript">
    function QuickSearchHasText(t) {
        return t.length > 0;
    }

    function DoStatusDialog() {
        var dialogOpts = {
            autoOpen: false,
            modal: true,
            width: '350',
            height: '150',
            title: 'Order View/Status Change',
            dialogClass: 'generic'
        };

        var d = $("#dialogStatusChange").dialog(dialogOpts);
        d.parent().appendTo($('form:first'));
        d.dialog('open');
    }

    function DoPatientDialog() {
        var dialogOpts = {
            autoOpen: false,
            modal: true,
            width: '300',
            height: '180',
            title: 'Order View/Status Change',
            dialogClass: 'generic'
        };

        var d = $("#dialogPatientSearch").dialog(dialogOpts);
        d.parent().appendTo($('form:first'));
        d.dialog('open');
    }
</script>