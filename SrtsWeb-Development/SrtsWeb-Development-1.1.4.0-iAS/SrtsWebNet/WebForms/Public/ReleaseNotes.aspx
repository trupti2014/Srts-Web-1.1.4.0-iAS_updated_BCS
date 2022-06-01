<%@ Page Title="" Language="C#" MasterPageFile="~/srtsMaster.Master" AutoEventWireup="true" CodeBehind="ReleaseNotes.aspx.cs" Inherits="SrtsWeb.Public.ReleaseNotes" %>

<%@ MasterType VirtualPath="~/srtsMaster.Master" %>
<asp:Content ID="Content5" ContentPlaceHolderID="MainContent_Public" runat="server">

    <div class="box_full_top" style="margin-top:30px;padding-top:5px;text-align:center;margin-bottom:-10px">
        <h1 style="font-size: 1.4em;margin-bottom:0px">Release Notes</h1>
    </div>
    <div class="box_full_content" style="min-height: 300px">
        <div class="padding">
          <%--  <h1 style="text-align: center; font-size: x-large;">Release Notes</h1>--%>
            <div class="padding">
                <ajaxToolkit:Accordion ID="accReleaseNotes" runat="server" ContentCssClass="accordianContent" OnItemDataBound="accReleaseNotes_ItemDataBound">
                    <HeaderTemplate>
                        <asp:Literal ID="litHeader" runat="server" Mode="PassThrough" />
                    </HeaderTemplate>
                    <ContentTemplate>
                        <asp:Literal ID="litContent" runat="server" Mode="PassThrough"/>
                    </ContentTemplate>
                </ajaxToolkit:Accordion>
            </div>
        </div>
    </div>
    <div class="box_full_bottom"></div>
</asp:Content>