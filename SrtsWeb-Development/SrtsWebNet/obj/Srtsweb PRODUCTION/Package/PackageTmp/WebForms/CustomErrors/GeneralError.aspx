<%@ Page Language="c#" MasterPageFile="~/srtsMaster.Master" CodeBehind="GeneralError.aspx.cs" AutoEventWireup="True" Inherits="SrtsWeb.CustomErrors.GeneralError" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="MainContent" runat="server">
    <div class="padding" style="text-align: center; width: 500px; margin: 0px auto">
        <p style="color: #782E1E; font-size: 1.2em">
            The server is experiencing a problem with the page you requested. We apologize for
   the inconvenience. We will resolve this issue shortly.
        </p>
    </div>
</asp:Content>