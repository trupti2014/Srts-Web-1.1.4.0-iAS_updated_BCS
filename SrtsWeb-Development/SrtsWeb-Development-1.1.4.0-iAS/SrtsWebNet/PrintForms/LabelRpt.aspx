<%@ Page Title="" Language="C#" MasterPageFile="~/srtsMaster.Master" AutoEventWireup="true" CodeBehind="LabelRpt.aspx.cs" Inherits="SrtsWeb.PrintForms" %>
<%@ MasterType VirtualPath="~/srtsMaster.Master" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:Content ID="Content7" ContentPlaceHolderID="MainContent" runat="server">
<div class="contentTitleleft" style="text-align:left;margin:0px 0px 0px -2px">
<h2 style="text-align:center"> <asp:Literal ID="litModuleTitle" runat="server"  /></h2>
<div style="margin:0px 0px 0px -10px;padding:10px;border-bottom:1px solid #E7CFAD;">
</div>
 </div>


<div  style="margin:0px 0px 0px -12px;padding:20px 0px 40px 0px">
        <rsweb:ReportViewer ID="rptViewerLocal" runat="server" AsyncRendering="False" BorderStyle="Ridge" Font-Names="Verdana" Font-Size="8pt" Height="100%" ProcessingMode="Local" WaitMessageFont-Names="Verdana" WaitMessageFont-Size="14pt" Width="1055px" BackColor="#E2F2FE" >
            <ServerReport ReportServerUrl="http://AMEDSATXA40007/reportserver" />
             <LocalReport ReportEmbeddedResource="SrtsWeb.PrintForms.Avery5160_5260.rdlc">
                <DataSources>
                    <rsweb:ReportDataSource DataSourceId="ObjectDataSource1" Name="DataSet1" />
                </DataSources>
            </LocalReport>
        </rsweb:ReportViewer>
</div>
</asp:Content>