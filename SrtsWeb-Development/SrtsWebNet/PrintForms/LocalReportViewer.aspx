﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LocalReportViewer.aspx.cs" Inherits="SrtsWeb.PrintForms.LocalReportViewer" %>

<%@ Register assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" namespace="Microsoft.Reporting.WebForms" tagprefix="rsweb" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
        <rsweb:ReportViewer ID="RptViewerLocal" runat="server" SizeToReportContent ="True">
        </rsweb:ReportViewer>
    
    </div>
    </form>
</body>
</html>
