<%@ Page Title="" Language="C#" MasterPageFile="~/srtsMaster.Master" AutoEventWireup="true" CodeBehind="rptViewerTemplate.aspx.cs" Inherits="SrtsWeb.Reports.rptViewerTemplate" %>

<%@ MasterType VirtualPath="~/srtsMaster.Master" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<%@ Register Assembly="PdfViewerAspNet" Namespace="PdfViewer4AspNet" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:Content ID="Content7" ContentPlaceHolderID="MainContent" runat="server">

    <style type="text/css">
        .trClass {
	        vertical-align: middle;
        }
        .tdClass {
	        vertical-align: middle;
            text-align:left;
        }
        .labelClass {
	        font-family: Verdana;
            font-size: 8pt;
        }
        .dateClass {
            font-family: Verdana;
            font-size: 8pt;
            vertical-align:central;
        }
        .calanderClass {
            background-color: white;
        }
        .textBoxClass {
            font-family: Verdana;
            font-size: 8pt;
        }
        .checkBoxClass {
            font-family: Verdana;
            font-size: 8pt;
            white-space: nowrap;
        }
        .listBoxClass {
            font-family: Verdana;
            font-size: 8pt;
        }
    </style>

    <script type="text/javascript">
        function enableTextbox(checkBox, textBoxName) {
            var inputMap = {
                "txtLabSiteCode": "<%=txtLabSiteCode.ClientID %>",
                "WWClinicSiteTextBox": "<%= WWClinicSiteTextBox.ClientID %>",
                "WWLabSiteTextBox": "<%= WWLabSiteTextBox.ClientID %>",
                "ClinicDispenseStatusTextBox": "<%= ClinicDispenseStatusTextBox.ClientID %>",
                "ClinicDispensePriorityTextBox": "<%= ClinicDispensePriorityTextBox.ClientID %>",
                "ClinicOrdersStatusTextBox": "<%= ClinicOrdersStatusTextBox.ClientID %>",
                "ClinicOrdersPriorityTextBox": "<%= ClinicOrdersPriorityTextBox.ClientID %>",
                "ClinicOverdueStatusTextBox": "<%= ClinicOverdueStatusTextBox.ClientID %>",
                "ClinicOverduePriorityTextBox": "<%= ClinicOverduePriorityTextBox.ClientID %>",
                "ClinicProductionStatusTextBox": "<%= ClinicProductionStatusTextBox.ClientID %>",
                "ClinicProductionPriorityTextBox": "<%= ClinicProductionPriorityTextBox.ClientID %>",
                "ClinicSummaryStatusTextBox": "<%= ClinicSummaryStatusTextBox.ClientID %>",
                "ClinicSummaryPriorityTextBox": "<%= ClinicSummaryPriorityTextBox.ClientID %>",
                "OrderDetailStatusTextBox": "<%= OrderDetailStatusTextBox.ClientID %>",
                "OrderDetailPriorityTextBox": "<%= OrderDetailPriorityTextBox.ClientID %>",
                "TurnAroundStatusTextBox": "<%= TurnAroundStatusTextBox.ClientID %>",
                "TurnAroundPriorityTextBox": "<%= TurnAroundPriorityTextBox.ClientID %>"
            }
            var textBox = document.getElementById(inputMap[textBoxName]);
            textBox.disabled = document.getElementById(checkBox).checked;
        }
    </script>

    <asp:Label ID="lblDebug" runat="server" Text="-- DEVELOPMENT --" Font-Bold="true" Font-Underline="true" Visible="false"></asp:Label>
    <div class="contentTitleleft" style="text-align: left; margin: 0px 0px 0px -2px">
        <h2 style="text-align: center">
            <asp:Literal ID="litModuleTitle" runat="server" /></h2>
        <div style="margin: 0px 0px 0px -10px; padding: 10px; border-bottom: 1px solid #E7CFAD;">
        </div>
    </div>

    <asp:Panel ID="ReportControlPanel" runat="server" Visible="false">
        <div style="margin: 0px 0px 0px -12px; padding: 20px 0px 40px 0px;">
            <table width="100%" style="padding: 5px 5px 10px; border-bottom-color: rgb(204, 204, 204); border-bottom-width: 1px; border-bottom-style: solid; background-color: rgb(226, 242, 254);" cellSpacing="0" cellPadding="0">
			    <tbody>
                    <tr>
				        <td width="100%" height="100%">
                            <table>
							    <tbody>
                                    <asp:Panel ID="LabRoutingFormPanel" runat="server" Visible="false">
                                        <tr class="trClass">
                                            <td class="tdClass">
                                                <asp:Label ID="LabRoutingSiteLabel" runat="server" Text="Site Code" CssClass="labelClass"></asp:Label>
                                                &nbsp;
                                                <asp:TextBox ID="LabRoutingSiteTextBox" runat="server" CssClass="textBoxClass"></asp:TextBox>
                                                &nbsp;
				                            </td>
                                            <td class="tdClass">
                                                <asp:Label ID="LabRoutingOrderNumbersLabel" runat="server" Text="Order Numbers" CssClass="labelClass"></asp:Label>
                                                &nbsp;
                                                <asp:TextBox ID="LabRoutingOrderNumbersTextBox" runat="server" CssClass="textBoxClass"></asp:TextBox>
                                                &nbsp;
				                            </td>
                                        </tr>
                                    </asp:Panel>
                                    <asp:Panel ID="Report54Panel" runat="server" Visible="false">
                                        <tr class="trClass">
									        <td class="tdClass">
                                                <asp:Label ID="Report54FromDateLabel" runat="server" Text="From Date" CssClass="labelClass"></asp:Label>
                                                &nbsp;
                                                <asp:TextBox ID="Report54FromDateText" runat="server" CssClass="dateClass"></asp:TextBox>
                                                <asp:ImageButton ID="ibtnReport54FromDateCalendar" ImageUrl="~/Styles/images/Calendar_scheduleHS.png" Width="20px" runat="server" />
                                                <ajaxToolkit:CalendarExtender runat="server" TargetControlID="Report54FromDateText" CssClass="calanderClass" Format="MM/dd/yyyy" PopupButtonID="ibtnReport54FromDateCalendar" />
				                                &nbsp;
                                            </td>
                                            <td class="tdClass">
                                                <asp:Label ID="Report54ToDateLabel" runat="server" Text="To Date" CssClass="labelClass"></asp:Label>
                                                &nbsp;
                                                <asp:TextBox ID="Report54ToDateText" runat="server" CssClass="dateClass"></asp:TextBox>
                                                <asp:ImageButton ID="ibtnReport54ToDateCalendar" ImageUrl="~/Styles/images/Calendar_scheduleHS.png" Width="20px" runat="server" CssClass="dateClass" />
                                                <ajaxToolkit:CalendarExtender runat="server" TargetControlID="Report54ToDateText" CssClass="calanderClass" Format="MM/dd/yyyy" PopupButtonID="ibtnReport54ToDateCalendar" />
                                                &nbsp;
                                            </td>
                                        </tr>
                                    </asp:Panel>
                                    <asp:Panel ID="WWPanel" runat="server" Visible="false">
                                        <tr class="trClass">
									        <td class="tdClass">
                                                <asp:Label ID="WWFromDateLabel" runat="server" Text="From Date" CssClass="labelClass"></asp:Label>
                                                &nbsp;
                                                <asp:TextBox ID="WWFromDateText" runat="server" CssClass="dateClass"></asp:TextBox>
                                                <asp:ImageButton ID="ibtnWWFromDateCalendar" ImageUrl="~/Styles/images/Calendar_scheduleHS.png" Width="20px" runat="server" />
                                                <ajaxToolkit:CalendarExtender runat="server" TargetControlID="WWFromDateText" CssClass="calanderClass" Format="MM/dd/yyyy" PopupButtonID="ibtnWWFromDateCalendar" />
				                                &nbsp;
                                            </td>
                                            <td class="tdClass">
                                                <asp:Label ID="WWToDateLabel" runat="server" Text="To Date" CssClass="labelClass"></asp:Label>
                                                &nbsp;
                                                <asp:TextBox ID="WWToDateText" runat="server" CssClass="dateClass"></asp:TextBox>
                                                <asp:ImageButton ID="ibtnWWToDateCalendar" ImageUrl="~/Styles/images/Calendar_scheduleHS.png" Width="20px" runat="server" CssClass="dateClass" />
                                                <ajaxToolkit:CalendarExtender runat="server" TargetControlID="WWToDateText" CssClass="calanderClass" Format="MM/dd/yyyy" PopupButtonID="ibtnWWToDateCalendar" />
                                                &nbsp;
                                            </td>
                                        </tr>
                                        <tr class="trClass">
									        <td class="tdClass">
                                                <asp:Label ID="WWClinicSiteLabel" runat="server" Text="Clinic Site Code" CssClass="labelClass"></asp:Label>
                                                &nbsp;
                                                <asp:TextBox ID="WWClinicSiteTextBox" runat="server" CssClass="textBoxClass" Enabled="false"></asp:TextBox>
                                                &nbsp;
                                                <asp:CheckBox ID="WWClinicSiteNULLCheckBox" runat="server" OnClick="enableTextbox(this.id, 'WWClinicSiteTextBox')" Checked="true" Text="NULL" CssClass="checkBoxClass" />
				                                &nbsp;
				                            </td>
                                            <td class="tdClass">
                                                <asp:Label ID="WWLabSiteLabel" runat="server" Text="Lab Site Code" CssClass="labelClass"></asp:Label>
                                                &nbsp;
                                                <asp:TextBox ID="WWLabSiteTextBox" runat="server" CssClass="textBoxClass" Enabled="false"></asp:TextBox>
                                                &nbsp;
                                                <asp:CheckBox ID="WWLabSiteNULLCheckBox" runat="server" OnClick="enableTextbox(this.id, 'WWLabSiteTextBox')" Checked="true" Text="NULL" CssClass="checkBoxClass" />
				                                &nbsp;
				                            </td>
                                        </tr>
                                    </asp:Panel>
                                    <asp:Panel ID="ClinicDispensePanel" runat="server" Visible="false">
                                        <tr class="trClass">
									        <td class="tdClass">
                                                <asp:Label ID="ClinicDispenseFromDateLabel" runat="server" Text="From Date" CssClass="labelClass"></asp:Label>
                                                &nbsp;
                                                <asp:TextBox ID="ClinicDispenseFromDateText" runat="server" CssClass="dateClass"></asp:TextBox>
                                                <asp:ImageButton ID="ibtnClinicDispenseFromDateCalendar" ImageUrl="~/Styles/images/Calendar_scheduleHS.png" Width="20px" runat="server" />
                                                <ajaxToolkit:CalendarExtender runat="server" TargetControlID="ClinicDispenseFromDateText" CssClass="calanderClass" Format="MM/dd/yyyy" PopupButtonID="ibtnClinicDispenseFromDateCalendar" />
				                                &nbsp;
                                            </td>
                                            <td class="tdClass">
                                                <asp:Label ID="ClinicDispenseToDateLabel" runat="server" Text="To Date" CssClass="labelClass"></asp:Label>
                                                &nbsp;
                                                <asp:TextBox ID="ClinicDispenseToDateText" runat="server" CssClass="dateClass"></asp:TextBox>
                                                <asp:ImageButton ID="ibtnClinicDispenseToDateCalendar" ImageUrl="~/Styles/images/Calendar_scheduleHS.png" Width="20px" runat="server" CssClass="dateClass" />
                                                <ajaxToolkit:CalendarExtender runat="server" TargetControlID="ClinicDispenseToDateText" CssClass="calanderClass" Format="MM/dd/yyyy" PopupButtonID="ibtnClinicDispenseToDateCalendar" />
                                                &nbsp;
                                            </td>
                                        </tr>
                                        <tr class="trClass">
									        <td class="tdClass">
                                                <asp:Label ID="ClinicDispenseStatusLabel" runat="server" Text="Status" CssClass="labelClass"></asp:Label>
                                                &nbsp;
                                                <asp:TextBox ID="ClinicDispenseStatusTextBox" runat="server" CssClass="textBoxClass" Enabled="false"></asp:TextBox>
                                                &nbsp;
                                                <asp:CheckBox ID="ClinicDispenseStatusNULLCheckBox" runat="server" OnClick="enableTextbox(this.id, 'ClinicDispenseStatusTextBox')" Checked="true" Text="NULL" CssClass="checkBoxClass" />
				                                &nbsp;
                                            </td>
                                            <td class="tdClass">
                                                <asp:Label ID="ClinicDispensePriorityLabel" runat="server" Text="Priority" CssClass="labelClass"></asp:Label>
                                                &nbsp;
                                                <asp:TextBox ID="ClinicDispensePriorityTextBox" runat="server" CssClass="textBoxClass" Enabled="false"></asp:TextBox>
                                                &nbsp;
                                                <asp:CheckBox ID="ClinicDispensePriorityNULLCheckBox" runat="server" OnClick="enableTextbox(this.id, 'ClinicDispensePriorityTextBox')" Checked="true" Text="NULL" CssClass="checkBoxClass" />
                                                &nbsp;
                                            </td>
                                        </tr>
                                    </asp:Panel>
                                    <asp:Panel ID="ClinicOrdersPanel" runat="server" Visible="false">
                                        <tr class="trClass">
									        <td class="tdClass">
                                                <asp:Label ID="ClinicOrdersFromDateLabel" runat="server" Text="From Date" CssClass="labelClass"></asp:Label>
                                                &nbsp;
                                                <asp:TextBox ID="ClinicOrdersFromDateText" runat="server" CssClass="dateClass"></asp:TextBox>
                                                <asp:ImageButton ID="ibtnClinicOrdersFromDateCalendar" ImageUrl="~/Styles/images/Calendar_scheduleHS.png" Width="20px" runat="server" />
                                                <ajaxToolkit:CalendarExtender runat="server" TargetControlID="ClinicOrdersFromDateText" CssClass="calanderClass" Format="MM/dd/yyyy" PopupButtonID="ibtnClinicOrdersFromDateCalendar" />
				                                &nbsp;
                                            </td>
                                            <td class="tdClass">
                                                <asp:Label ID="ClinicOrdersToDateLabel" runat="server" Text="To Date" CssClass="labelClass"></asp:Label>
                                                &nbsp;
                                                <asp:TextBox ID="ClinicOrdersToDateText" runat="server" CssClass="dateClass"></asp:TextBox>
                                                <asp:ImageButton ID="ibtnClinicOrdersToDateCalendar" ImageUrl="~/Styles/images/Calendar_scheduleHS.png" Width="20px" runat="server" CssClass="dateClass" />
                                                <ajaxToolkit:CalendarExtender runat="server" TargetControlID="ClinicOrdersToDateText" CssClass="calanderClass" Format="MM/dd/yyyy" PopupButtonID="ibtnClinicOrdersToDateCalendar" />
                                                &nbsp;
                                            </td>
                                        </tr>
                                        <tr class="trClass">
									        <td class="tdClass">
                                                <asp:Label ID="ClinicOrdersStatusLabel" runat="server" Text="Status" CssClass="labelClass"></asp:Label>
                                                &nbsp;
                                                <asp:TextBox ID="ClinicOrdersStatusTextBox" runat="server" CssClass="textBoxClass" Enabled="false"></asp:TextBox>
                                                &nbsp;
                                                <asp:CheckBox ID="ClinicOrdersStatusNULLCheckBox" runat="server" OnClick="enableTextbox(this.id, 'ClinicOrdersStatusTextBox')" Checked="true" Text="NULL" CssClass="checkBoxClass" />
				                                &nbsp;
                                            </td>
                                            <td class="tdClass">
                                                <asp:Label ID="ClinicOrdersPriorityLabel" runat="server" Text="Priority" CssClass="labelClass"></asp:Label>
                                                &nbsp;
                                                <asp:TextBox ID="ClinicOrdersPriorityTextBox" runat="server" CssClass="textBoxClass" Enabled="false"></asp:TextBox>
                                                &nbsp;
                                                <asp:CheckBox ID="ClinicOrdersPriorityNULLCheckBox" runat="server" OnClick="enableTextbox(this.id, 'ClinicOrdersPriorityTextBox')" Checked="true" Text="NULL" CssClass="checkBoxClass" />
                                                &nbsp;
                                            </td>
                                        </tr>
                                        <tr class="trClass">
									        <td colspan="2" class="tdClass">
                                                <asp:Label ID="ClinicOrdersTypeLabel" runat="server" Text="Type" CssClass="labelClass"></asp:Label>
                                                &nbsp;
                                                <asp:DropDownList ID="ClinicOrdersTypeList" runat="server">
                                                    <asp:ListItem Text="All" Value="All" Selected="True" />
                                                    <asp:ListItem Text="Order Date" Value="Ordered" />
                                                    <asp:ListItem Text="Lab Received Date" Value="Lab Received" />
                                                    <asp:ListItem Text="Lab Dispensed Date" Value="Lab Dispensed" />
                                                    <asp:ListItem Text="Clinic Received" Value="Received" />
                                                    <asp:ListItem Text="Clinic Dispensed" Value="ClinicDispense" />
                                                    <asp:ListItem Text="Lab Shipped to Patient" Value="ShipToPatient" />
                                                </asp:DropDownList>
				                                &nbsp;
                                            </td>
                                        </tr>
                                    </asp:Panel>
                                    <asp:Panel ID="ClinicOverduePanel" runat="server" Visible="false">
                                        <tr class="trClass">
									        <td class="tdClass">
                                                <asp:Label ID="ClinicOverdueFromDateLabel" runat="server" Text="From Date" CssClass="labelClass"></asp:Label>
                                                &nbsp;
                                                <asp:TextBox ID="ClinicOverdueFromDateText" runat="server" CssClass="dateClass"></asp:TextBox>
                                                <asp:ImageButton ID="ibtnClinicOverdueFromDateCalendar" ImageUrl="~/Styles/images/Calendar_scheduleHS.png" Width="20px" runat="server" />
                                                <ajaxToolkit:CalendarExtender runat="server" TargetControlID="ClinicOverdueFromDateText" CssClass="calanderClass" Format="MM/dd/yyyy" PopupButtonID="ibtnClinicOverdueFromDateCalendar" />
				                                &nbsp;
                                            </td>
                                            <td class="tdClass">
                                                <asp:Label ID="ClinicOverdueToDateLabel" runat="server" Text="To Date" CssClass="labelClass"></asp:Label>
                                                &nbsp;
                                                <asp:TextBox ID="ClinicOverdueToDateText" runat="server" CssClass="dateClass"></asp:TextBox>
                                                <asp:ImageButton ID="ibtnClinicOverdueToDateCalendar" ImageUrl="~/Styles/images/Calendar_scheduleHS.png" Width="20px" runat="server" CssClass="dateClass" />
                                                <ajaxToolkit:CalendarExtender runat="server" TargetControlID="ClinicOverdueToDateText" CssClass="calanderClass" Format="MM/dd/yyyy" PopupButtonID="ibtnClinicOverdueToDateCalendar" />
                                                &nbsp;
                                            </td>
                                        </tr>
                                        <tr class="trClass">
									        <td class="tdClass">
                                                <asp:Label ID="ClinicOverdueStatusLabel" runat="server" Text="Status" CssClass="labelClass"></asp:Label>
                                                &nbsp;
                                                <asp:TextBox ID="ClinicOverdueStatusTextBox" runat="server" CssClass="textBoxClass" Enabled="false"></asp:TextBox>
                                                &nbsp;
                                                <asp:CheckBox ID="ClinicOverdueStatusNULLCheckBox" runat="server" OnClick="enableTextbox(this.id, 'ClinicOverdueStatusTextBox')" Checked="true" Text="NULL" CssClass="checkBoxClass" />
				                                &nbsp;
                                            </td>
                                            <td class="tdClass">
                                                <asp:Label ID="ClinicOverduePriorityLabel" runat="server" Text="Priority" CssClass="labelClass"></asp:Label>
                                                &nbsp;
                                                <asp:TextBox ID="ClinicOverduePriorityTextBox" runat="server" CssClass="textBoxClass" Enabled="false"></asp:TextBox>
                                                &nbsp;
                                                <asp:CheckBox ID="ClinicOverduePriorityNULLCheckBox" runat="server" OnClick="enableTextbox(this.id, 'ClinicOverduePriorityTextBox')" Checked="true" Text="NULL" CssClass="checkBoxClass" />
                                                &nbsp;
                                            </td>
                                        </tr>
                                    </asp:Panel>
                                    <asp:Panel ID="ClinicProductionPanel" runat="server" Visible="false">
                                        <tr class="trClass">
									        <td class="tdClass">
                                                <asp:Label ID="ClinicProductionFromDateLabel" runat="server" Text="From Date" CssClass="labelClass"></asp:Label>
                                                &nbsp;
                                                <asp:TextBox ID="ClinicProductionFromDateText" runat="server" CssClass="dateClass"></asp:TextBox>
                                                <asp:ImageButton ID="ibtnClinicProductionFromDateCalendar" ImageUrl="~/Styles/images/Calendar_scheduleHS.png" Width="20px" runat="server" />
                                                <ajaxToolkit:CalendarExtender runat="server" TargetControlID="ClinicProductionFromDateText" CssClass="calanderClass" Format="MM/dd/yyyy" PopupButtonID="ibtnClinicProductionFromDateCalendar" />
				                                &nbsp;
                                            </td>
                                            <td class="tdClass">
                                                <asp:Label ID="ClinicProductionToDateLabel" runat="server" Text="To Date" CssClass="labelClass"></asp:Label>
                                                &nbsp;
                                                <asp:TextBox ID="ClinicProductionToDateText" runat="server" CssClass="dateClass"></asp:TextBox>
                                                <asp:ImageButton ID="ibtnClinicProductionToDateCalendar" ImageUrl="~/Styles/images/Calendar_scheduleHS.png" Width="20px" runat="server" CssClass="dateClass" />
                                                <ajaxToolkit:CalendarExtender runat="server" TargetControlID="ClinicProductionToDateText" CssClass="calanderClass" Format="MM/dd/yyyy" PopupButtonID="ibtnClinicProductionToDateCalendar" />
                                                &nbsp;
                                            </td>
                                        </tr>
                                        <tr class="trClass">
									        <td class="tdClass">
                                                <asp:Label ID="ClinicProductionStatusLabel" runat="server" Text="Status" CssClass="labelClass"></asp:Label>
                                                &nbsp;
                                                <asp:TextBox ID="ClinicProductionStatusTextBox" runat="server" CssClass="textBoxClass" Enabled="false"></asp:TextBox>
                                                &nbsp;
                                                <asp:CheckBox ID="ClinicProductionStatusNULLCheckBox" runat="server" OnClick="enableTextbox(this.id, 'ClinicProductionStatusTextBox')" Checked="true" Text="NULL" CssClass="checkBoxClass" />
				                                &nbsp;
                                            </td>
                                            <td class="tdClass">
                                                <asp:Label ID="ClinicProductionPriorityLabel" runat="server" Text="Priority" CssClass="labelClass"></asp:Label>
                                                &nbsp;
                                                <asp:TextBox ID="ClinicProductionPriorityTextBox" runat="server" CssClass="textBoxClass" Enabled="false"></asp:TextBox>
                                                &nbsp;
                                                <asp:CheckBox ID="ClinicProductionPriorityNULLCheckBox" runat="server" OnClick="enableTextbox(this.id, 'ClinicProductionPriorityTextBox')" Checked="true" Text="NULL" CssClass="checkBoxClass" />
                                                &nbsp;
                                            </td>
                                        </tr>
                                    </asp:Panel>
                                    <asp:Panel ID="ClinicSummaryPanel" runat="server" Visible="false">
                                        <tr class="trClass">
									        <td class="tdClass">
                                                <asp:Label ID="ClinicSummaryFromDateLabel" runat="server" Text="From Date" CssClass="labelClass"></asp:Label>
                                                &nbsp;
                                                <asp:TextBox ID="ClinicSummaryFromDateText" runat="server" CssClass="dateClass"></asp:TextBox>
                                                <asp:ImageButton ID="ibtnClinicSummaryFromDateCalendar" ImageUrl="~/Styles/images/Calendar_scheduleHS.png" Width="20px" runat="server" />
                                                <ajaxToolkit:CalendarExtender runat="server" TargetControlID="ClinicSummaryFromDateText" CssClass="calanderClass" Format="MM/dd/yyyy" PopupButtonID="ibtnClinicSummaryFromDateCalendar" />
				                                &nbsp;
                                            </td>
                                            <td class="tdClass">
                                                <asp:Label ID="ClinicSummaryToDateLabel" runat="server" Text="To Date" CssClass="labelClass"></asp:Label>
                                                &nbsp;
                                                <asp:TextBox ID="ClinicSummaryToDateText" runat="server" CssClass="dateClass"></asp:TextBox>
                                                <asp:ImageButton ID="ibtnClinicSummaryToDateCalendar" ImageUrl="~/Styles/images/Calendar_scheduleHS.png" Width="20px" runat="server" CssClass="dateClass" />
                                                <ajaxToolkit:CalendarExtender runat="server" TargetControlID="ClinicSummaryToDateText" CssClass="calanderClass" Format="MM/dd/yyyy" PopupButtonID="ibtnClinicSummaryToDateCalendar" />
                                                &nbsp;
                                            </td>
                                        </tr>
                                        <tr class="trClass">
									        <td class="tdClass">
                                                <asp:Label ID="ClinicSummaryStatusLabel" runat="server" Text="Status" CssClass="labelClass"></asp:Label>
                                                &nbsp;
                                                <asp:TextBox ID="ClinicSummaryStatusTextBox" runat="server" CssClass="textBoxClass" Enabled="false"></asp:TextBox>
                                                &nbsp;
                                                <asp:CheckBox ID="ClinicSummaryStatusNULLCheckBox" runat="server" OnClick="enableTextbox(this.id, 'ClinicSummaryStatusTextBox')" Checked="true" Text="NULL" CssClass="checkBoxClass" />
				                                &nbsp;
                                            </td>
                                            <td class="tdClass">
                                                <asp:Label ID="ClinicSummaryPriorityLabel" runat="server" Text="Priority" CssClass="labelClass"></asp:Label>
                                                &nbsp;
                                                <asp:TextBox ID="ClinicSummaryPriorityTextBox" runat="server" CssClass="textBoxClass" Enabled="false"></asp:TextBox>
                                                &nbsp;
                                                <asp:CheckBox ID="ClinicSummaryPriorityNULLCheckBox" runat="server" OnClick="enableTextbox(this.id, 'ClinicSummaryPriorityTextBox')" Checked="true" Text="NULL" CssClass="checkBoxClass" />
                                                &nbsp;
                                            </td>
                                        </tr>
                                    </asp:Panel>
                                    <asp:Panel ID="OrderDetailPanel" runat="server" Visible="false">
                                        <tr class="trClass">
									        <td class="tdClass">
                                                <asp:Label ID="OrderDetailFromDateLabel" runat="server" Text="From Date" CssClass="labelClass"></asp:Label>
                                                &nbsp;
                                                <asp:TextBox ID="OrderDetailFromDateText" runat="server" CssClass="dateClass"></asp:TextBox>
                                                <asp:ImageButton ID="ibtnOrderDetailFromDateCalendar" ImageUrl="~/Styles/images/Calendar_scheduleHS.png" Width="20px" runat="server" />
                                                <ajaxToolkit:CalendarExtender runat="server" TargetControlID="OrderDetailFromDateText" CssClass="calanderClass" Format="MM/dd/yyyy" PopupButtonID="ibtnOrderDetailFromDateCalendar" />
				                                &nbsp;
                                            </td>
                                            <td class="tdClass">
                                                <asp:Label ID="OrderDetailToDateLabel" runat="server" Text="To Date" CssClass="labelClass"></asp:Label>
                                                &nbsp;
                                                <asp:TextBox ID="OrderDetailToDateText" runat="server" CssClass="dateClass"></asp:TextBox>
                                                <asp:ImageButton ID="ibtnOrderDetailToDateCalendar" ImageUrl="~/Styles/images/Calendar_scheduleHS.png" Width="20px" runat="server" CssClass="dateClass" />
                                                <ajaxToolkit:CalendarExtender runat="server" TargetControlID="OrderDetailToDateText" CssClass="calanderClass" Format="MM/dd/yyyy" PopupButtonID="ibtnOrderDetailToDateCalendar" />
                                                &nbsp;
                                            </td>
                                        </tr>
                                        <tr class="trClass">
									        <td class="tdClass">
                                                <asp:Label ID="OrderDetailStatusLabel" runat="server" Text="Status" CssClass="labelClass"></asp:Label>
                                                &nbsp;
                                                <asp:TextBox ID="OrderDetailStatusTextBox" runat="server" CssClass="textBoxClass" Enabled="false"></asp:TextBox>
                                                &nbsp;
                                                <asp:CheckBox ID="OrderDetailStatusNULLCheckBox" runat="server" OnClick="enableTextbox(this.id, 'OrderDetailStatusTextBox')" Checked="true" Text="NULL" CssClass="checkBoxClass" />
				                                &nbsp;
                                            </td>
                                            <td class="tdClass">
                                                <asp:Label ID="OrderDetailPriorityLabel" runat="server" Text="Priority" CssClass="labelClass"></asp:Label>
                                                &nbsp;
                                                <asp:TextBox ID="OrderDetailPriorityTextBox" runat="server" CssClass="textBoxClass" Enabled="false"></asp:TextBox>
                                                &nbsp;
                                                <asp:CheckBox ID="OrderDetailPriorityNULLCheckBox" runat="server" OnClick="enableTextbox(this.id, 'OrderDetailPriorityTextBox')" Checked="true" Text="NULL" CssClass="checkBoxClass" />
                                                &nbsp;
                                            </td>
                                        </tr>
                                    </asp:Panel>
                                    <asp:Panel ID="TurnAroundPanel" runat="server" Visible="false">
                                        <tr class="trClass">
									        <td class="tdClass">
                                                <asp:Label ID="TurnAroundFromDateLabel" runat="server" Text="From Date" CssClass="labelClass"></asp:Label>
                                                &nbsp;
                                                <asp:TextBox ID="TurnAroundFromDateText" runat="server" CssClass="dateClass"></asp:TextBox>
                                                <asp:ImageButton ID="ibtnTurnAroundFromDateCalendar" ImageUrl="~/Styles/images/Calendar_scheduleHS.png" Width="20px" runat="server" />
                                                <ajaxToolkit:CalendarExtender runat="server" TargetControlID="TurnAroundFromDateText" CssClass="calanderClass" Format="MM/dd/yyyy" PopupButtonID="ibtnTurnAroundFromDateCalendar" />
				                                &nbsp;
                                            </td>
                                            <td class="tdClass">
                                                <asp:Label ID="TurnAroundToDateLabel" runat="server" Text="To Date" CssClass="labelClass"></asp:Label>
                                                &nbsp;
                                                <asp:TextBox ID="TurnAroundToDateText" runat="server" CssClass="dateClass"></asp:TextBox>
                                                <asp:ImageButton ID="ibtnTurnAroundToDateCalendar" ImageUrl="~/Styles/images/Calendar_scheduleHS.png" Width="20px" runat="server" CssClass="dateClass" />
                                                <ajaxToolkit:CalendarExtender runat="server" TargetControlID="TurnAroundToDateText" CssClass="calanderClass" Format="MM/dd/yyyy" PopupButtonID="ibtnTurnAroundToDateCalendar" />
                                                &nbsp;
                                            </td>
                                        </tr>
                                        <tr class="trClass">
									        <td class="tdClass">
                                                <asp:Label ID="TurnAroundStatusLabel" runat="server" Text="Status" CssClass="labelClass"></asp:Label>
                                                &nbsp;
                                                <asp:TextBox ID="TurnAroundStatusTextBox" runat="server" CssClass="textBoxClass" Enabled="false"></asp:TextBox>
                                                &nbsp;
                                                <asp:CheckBox ID="TurnAroundStatusNULLCheckBox" runat="server" OnClick="enableTextbox(this.id, 'TurnAroundStatusTextBox')" Checked="true" Text="NULL" CssClass="checkBoxClass" />
				                                &nbsp;
                                            </td>
                                            <td class="tdClass">
                                                <asp:Label ID="TurnAroundPriorityLabel" runat="server" Text="Priority" CssClass="labelClass"></asp:Label>
                                                &nbsp;
                                                <asp:TextBox ID="TurnAroundPriorityTextBox" runat="server" CssClass="textBoxClass" Enabled="false"></asp:TextBox>
                                                &nbsp;
                                                <asp:CheckBox ID="TurnAroundPriorityNULLCheckBox" runat="server" OnClick="enableTextbox(this.id, 'TurnAroundPriorityTextBox')" Checked="true" Text="NULL" CssClass="checkBoxClass" />
                                                &nbsp;
                                            </td>
                                        </tr>
                                    </asp:Panel>
                                    <asp:Panel ID="AccountInfoPanel" runat="server" Visible="false">
                                        <tr class="trClass">
                                            <td class="tdClass">
                                                <asp:Label ID="AccountInfoSiteLabel" runat="server" Text="Select Site Type" CssClass="labelClass"></asp:Label>
                                                &nbsp;
                                                <asp:ListBox ID="AccountInfoListBox" runat="server" SelectionMode="Multiple" CssClass="listBoxClass">
                                                    <asp:ListItem Text="Clinic" Value="CLINIC" Selected="True"></asp:ListItem>
                                                    <asp:ListItem Text="Lab" Value="LAB"></asp:ListItem>
                                                </asp:ListBox>
                                                &nbsp;
				                            </td>
                                        </tr>
                                    </asp:Panel>
                                    <asp:Panel ID="GEyesCountPanel" runat="server" Visible="false">
                                        <tr class="trClass">
                                            <td class="tdClass">
                                                <asp:Label ID="YearLabel" runat="server" Text="Select Year Created" CssClass="labelClass"></asp:Label>
                                                &nbsp;
                                                <asp:ListBox ID="listboxYear" runat="server" SelectionMode="Multiple" CssClass="listBoxClass">
                                                    <asp:ListItem Text="2022" Value="2022" Selected="True"></asp:ListItem>
                                                    <asp:ListItem Text="2021" Value="2021"></asp:ListItem>
                                                    <asp:ListItem Text="2020" Value="2020"></asp:ListItem>
                                                    <asp:ListItem Text="2019" Value="2019"></asp:ListItem>
                                                    <asp:ListItem Text="2018" Value="2018"></asp:ListItem>
                                                    <asp:ListItem Text="2017" Value="2017"></asp:ListItem>
                                                    <asp:ListItem Text="2016" Value="2016"></asp:ListItem>
                                                </asp:ListBox>
                                                &nbsp;
				                            </td>
                                            <td class="tdClass">
                                                <asp:Label ID="MonthLabel" runat="server" Text="Select MOnth Created" CssClass="labelClass"></asp:Label>
                                                &nbsp;
                                                <asp:ListBox ID="listboxMonth" runat="server" SelectionMode="Multiple" CssClass="listBoxClass">
                                                    <asp:ListItem Text="01" Value="01" Selected="True"></asp:ListItem>
                                                    <asp:ListItem Text="02" Value="01"></asp:ListItem>
                                                    <asp:ListItem Text="03" Value="03"></asp:ListItem>
                                                    <asp:ListItem Text="04" Value="04"></asp:ListItem>
                                                    <asp:ListItem Text="05" Value="05"></asp:ListItem>
                                                    <asp:ListItem Text="06" Value="06"></asp:ListItem>
                                                    <asp:ListItem Text="07" Value="07"></asp:ListItem>
                                                    <asp:ListItem Text="08" Value="08"></asp:ListItem>
                                                    <asp:ListItem Text="09" Value="09"></asp:ListItem>
                                                    <asp:ListItem Text="10" Value="10"></asp:ListItem>
                                                    <asp:ListItem Text="11" Value="11"></asp:ListItem>
                                                    <asp:ListItem Text="12" Value="12"></asp:ListItem>
                                                </asp:ListBox>
                                                &nbsp;
				                            </td>
                                        </tr>
                                    </asp:Panel>
                                    <asp:Panel ID="ReprintFormPanel" runat="server" Visible="false">
                                        <tr class="trClass">
                                            <td class="tdClass">
                                                <asp:Label ID="OrderNumberLabel" runat="server" Text="Order Number" CssClass="labelClass"></asp:Label>
                                                &nbsp;
                                                <asp:TextBox ID="txtOrderNumber" runat="server" CssClass="textBoxClass"></asp:TextBox>
                                                &nbsp;
				                            </td>
                                            <td class="tdClass">
                                                <asp:Label ID="LabSiteCodeLabel" runat="server" Text="Lab Site Code" CssClass="labelClass"></asp:Label>
                                                &nbsp;
                                                <asp:TextBox ID="txtLabSiteCode" runat="server" CssClass="textBoxClass" Enabled="false"></asp:TextBox>
                                                &nbsp;
                                                <asp:CheckBox ID="chkLabSiteCode" runat="server" OnClick="enableTextbox(this.id, 'txtLabSiteCode')" Checked="true" Text="NULL" CssClass="checkBoxClass" />
                                                &nbsp;
				                            </td>
                                        </tr>
                                    </asp:Panel>
                                </tbody>
                            </table>
                        </td>
                        <td width="6"></td>
                        <td align="center" vAlign="top" style="padding: 10px; border-left-color: rgb(204, 204, 204); border-left-width: 1px; border-left-style: solid;">
                            <table>
						        <tbody>
                                    <tr>
							            <td>
                                            <asp:Button ID="viewReport" runat="server" Text="View Report" OnClick="viewReport_Click"/>
							            </td>
						            </tr>
					            </tbody>
                            </table>
                        </td>
				    </tr>
                </tbody>
            </table>
        </div>
    </asp:Panel>
    <div style="width: 100%; height: 100%; overflow: auto; position: relative;">
	    <iframe id="BCSFrame" runat="server" width="100%" align="top" height="1000" visible="false"></iframe>
    </div>
      
   <%-- <script type="text/javascript">
        function navigate(filename) {
            document.getElementById
                ("BCSFrame").src = "https://srts-demo.csd.disa.mil/TMP/" + filename;
            alert(document.getElementById
                ("BCSFrame").src);
        }
    </script>--%>
</asp:Content>