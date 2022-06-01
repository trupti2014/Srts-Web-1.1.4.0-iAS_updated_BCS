<%@ Page Title="" Language="C#" MasterPageFile="~/srtsMaster.Master" AutoEventWireup="true" CodeBehind="PageTemplate.aspx.cs" Inherits="SrtsWeb.PageTemplate" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="contentMenuClinic" runat="server" ContentPlaceHolderID="contentSubMenu">
    <div class="button">
        <ul>
            <li>
                <asp:LinkButton ID="lnkSearchPatient" runat="server" CommandArgument="A" OnCommand="rbPatientSearch_Click" ToolTip="Select to perform a new patient search." Text="Patient Search" />
            </li>

            <li>
                <asp:LinkButton ID="lnkAddPatient" runat="server" OnCommand="rbNewPatient_Click" ToolTip="Select to Add a New Patient." Text="Add New Patient" />
            </li>

            <li>
                <asp:LinkButton ID="lnkClinicOrderCheckin" runat="server" OnCommand="rbOrderCheckin_Click" ToolTip="Select to Check-In Orders" Text="Order Check-In" />
            </li>

            <li>
                <asp:LinkButton ID="lnkClinicDispenseOrder" runat="server" OnCommand="rbOrderDispense_Click" ToolTip="Select to Dispense Orders" Text="Order Dispense" />
            </li>
        </ul>
    </div>
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="MainContent" runat="server">
</asp:Content>