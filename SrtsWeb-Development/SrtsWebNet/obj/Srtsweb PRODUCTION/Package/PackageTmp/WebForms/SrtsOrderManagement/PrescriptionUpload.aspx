<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PrescriptionUpload.aspx.cs" Inherits="SrtsWeb.WebForms.SrtsOrderManagement.PrescriptionUpload" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
        <link rel="stylesheet" type="text/css" href="../../Styles/w3.css" />
</head>
<body>
   <form id="frmPrescriptionFileUpload" runat="server"> 
        <div style="height:60px">      
        <div id="divPrescriptionDoc" runat="server" class="w3-row" style="margin:0px;padding:0px;width:100%;">
            <div class="w3-col" style="width:30px">
                <img src="../../Styles/images/img_headline.png" style="margin: 0px 0px 0px -10px; float: left" alt="Upload a copy of the prescription." />
            </div>
            <div class="w3-col" style="width:300px">
                <asp:FileUpload runat="server" ID="fileUpload" Width="280px" onchange="this.form.submit()"/>                 
            </div>
        </div>      
    <div style="color:#900d0d;margin-left:35px"> 
        <asp:Label ID="lblFileUpload" runat="server" Width="280px" Visible="true" style="color:#004994" /><br />
        <asp:Label runat="server" ID="lblInfo"/>
         <div style="position:relative;top:0px;left:330px"><asp:Button runat="server" ID="btnCancelUpload" Text="Cancel" OnClick="btnCancelUpload_Click" Visible="false"/></div>
    </div> 
            </div>
    </form>
</body>
</html>
