<%@ Page Title="" Language="C#" MasterPageFile="~/srtsMaster.Master" AutoEventWireup="true" CodeBehind="ReportPageTemplate.aspx.cs" Inherits="SrtsWeb.Reports.ReportPageTemplate" %>
<%@ MasterType VirtualPath="~/srtsMaster.Master" %>


<%@ Register assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" namespace="Microsoft.Reporting.WebForms" tagprefix="rsweb" %>


<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:Content ID="Content7" ContentPlaceHolderID="MainContent" runat="server">
    <div class="contentTitleleft" style="text-align:center">
<div class="SRTSwebInfo"> <h1>&nbsp;</h1></div>
<h2 style="text-align:center"> <asp:Literal ID="litModuleTitle" runat="server"  /></h2>
<div style="margin:-10px 0px 20px 20px;padding-left:10px;border-bottom:1px solid #E7CFAD;width:90%">

</div>
<h1 class="headerBlue">Report Title</h1>
 </div>

 
<div class="padding" style="margin:0px 30px">
    <p>
      <asp:ScriptManagerProxy ID="ScriptManagerProxy" runat="server">
    </asp:ScriptManagerProxy>
    <rsweb:ReportViewer ID="ReportViewer2" runat="server" ProcessingMode="Remote" Font-Names="Verdana" Font-Size="8pt" WaitMessageFont-Names="Verdana" WaitMessageFont-Size="14pt">
        <ServerReport ReportServerUrl="http://amedsatxa40007/reportserver" />
    </rsweb:ReportViewer>
     </p>
</div>
  
</asp:Content>
