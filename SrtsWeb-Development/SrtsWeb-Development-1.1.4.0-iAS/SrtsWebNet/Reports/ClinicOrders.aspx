<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ClinicOrders.aspx.cs" Inherits="SrtsWeb.Reports.ClinicOrders" %>

<%@ Register assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" namespace="Microsoft.Reporting.WebForms" tagprefix="rsweb" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div style="height: 567px">
    
        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
        <asp:ObjectDataSource ID="ObjectDataSource1" runat="server" SelectMethod="GetData" TypeName="srtsProdTestDBDataSetTableAdapters.GetClinicOrdersReportDataTableAdapter"></asp:ObjectDataSource>
    
        <rsweb:ReportViewer ID="rptvwrClinicOrders" runat="server" ExportContentDisposition="AlwaysInline" Font-Names="Verdana" Font-Size="8pt" WaitMessageFont-Names="Verdana" WaitMessageFont-Size="14pt" Width="889px" ProcessingMode="Remote">
            <ServerReport ReportPath="/SRTSWeb/Reports/ClinicOrders.rdlc" />
            <LocalReport ReportEmbeddedResource="SrtsWeb.Reports.ClinicOrders.rdlc">
                <DataSources>
                    <rsweb:ReportDataSource DataSourceId="ObjectDataSource1" Name="ClinicOrdersRpt" />
                </DataSources>
            </LocalReport>
        </rsweb:ReportViewer>
    
    </div>
    </form>
</body>
</html>
