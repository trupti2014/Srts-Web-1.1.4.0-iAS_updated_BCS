<%@ Page Title="" Language="C#" MasterPageFile="~/srtsMaster.Master" AutoEventWireup="true" CodeBehind="MessageCenter.aspx.cs" Inherits="SrtsWeb.SrtsMessageCenter.MessageCenter" %>

<%@ MasterType VirtualPath="~/srtsMaster.Master" %>

<asp:Content ID="Content5" ContentPlaceHolderID="MainContent" runat="server">

    <div class="padding" style="min-height:100px">
    <div class="padding">
    <ajaxToolkit:Accordion ID="accMaster1" runat="server" ContentCssClass="accordianContent" OnItemDataBound="accMaster1_ItemDataBound">
        <HeaderTemplate>
            <asp:HiddenField ID="hfId" runat="server" />
            <asp:Literal ID="litHeader" runat="server" Mode="PassThrough" />
        </HeaderTemplate>
        <ContentTemplate>
            <asp:Literal ID="litContent" runat="server" Mode="PassThrough" />
        </ContentTemplate>
    </ajaxToolkit:Accordion>
    </div>
    </div>

</asp:Content>