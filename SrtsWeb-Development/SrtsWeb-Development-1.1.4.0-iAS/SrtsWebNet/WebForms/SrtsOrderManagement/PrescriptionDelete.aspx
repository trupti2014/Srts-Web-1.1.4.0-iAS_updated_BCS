<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PrescriptionDelete.aspx.cs" Inherits="SrtsWeb.WebForms.SrtsOrderManagement.PrescriptionDelete" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
        <link rel="stylesheet" type="text/css" href="../../Styles/w3.css" />
    <script type="text/javascript">
        function IsDeletePrescritionScan() {
            document.getElementById('id_confrmdiv').style.display = "block"; //this is the replace of this line


            document.getElementById('id_truebtn').onclick = function () {
                //do your delete operation
                alert('true');
            };

            document.getElementById('id_falsebtn').onclick = function () {
                alert('false');
                return false;
            };
        }
    </script>
</head>
<body>
    <form id="frmDelte" runat="server">       
        <div class="w3-row" style="margin:0px;padding:0px;width:100%">
        <div class="w3-col" style="width:30px">
            <img src="../../Styles/images/img_delete_x.png" style="margin: 0px 0px 0px -10px; float: left" alt="Delete this prescription's attached file." />
        </div>

        <div class="w3-col" style="width:300px;">
            <asp:LinkButton runat="server" ID="lnkDelete" Text="Delete Attached Prescription File" OnClick="btnDelete_Click" OnClientClick="IsDeletePrescritionScan()" />
            <div id="id_confrmdiv" style="display:none">confirmation
            <button id="id_truebtn">Yes</button>
            <button id="id_falsebtn">No</button>
            </div>
            <div style="display: none;margin-top:2px" id="deleting....">
                <asp:Image runat="server" ID="imgDeleting" ImageUrl="~/Styles/images/img_loading_line.gif" />
            </div>
        </div> <br />     
    </div>      
    <div style="color:#900d0d;margin-left:35px"> <asp:Label runat="server" ID="lblDeleteInfo" /></div> 
    </form>
</body>
</html>
