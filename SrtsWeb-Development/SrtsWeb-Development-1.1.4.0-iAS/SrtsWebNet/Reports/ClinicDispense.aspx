﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ClinicDispense.aspx.cs" Inherits="SrtsWeb.Reports.ClinicDispense" %>

<%@ Register assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" namespace="Microsoft.Reporting.WebForms" tagprefix="rsweb" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
    </div>
        <asp:ObjectDataSource ID="ObjectDataSource1" runat="server" SelectMethod="GetData" TypeName="ClinicDispenseDataSetTableAdapters.GetClinicDispenseDataTableAdapter" OnSelecting="ObjectDataSource1_Selecting"></asp:ObjectDataSource>
        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
        <rsweb:ReportViewer ID="rptClinicDispense" runat="server" ExportContentDisposition="AlwaysInline" Font-Names="Verdana" Font-Size="8pt" ProcessingMode="Remote" WaitMessageFont-Names="Verdana" WaitMessageFont-Size="14pt" Width="967px">
            <ServerReport ReportPath="/SRTSWeb/Reports/ClinicDispense.rdlc" />
            <LocalReport ReportEmbeddedResource="SrtsWeb.Reports.ClinicDispense.rdlc">
                <DataSources>
                    <rsweb:ReportDataSource DataSourceId="ObjectDataSource1" Name="dsClinicDispense" />
                </DataSources>
            </LocalReport>
        </rsweb:ReportViewer>
    </form>
</body>
</html>
