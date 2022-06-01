<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ucBoxRight.ascx.cs" Inherits="SrtsWeb.UserControls.ucBoxRight" %>

<asp:PlaceHolder ID="phContent" runat="server">
    <div class="boxheader">
        <h1><span class="box_title"><%= Title %></span></h1>
        <br />
        <asp:Image ID="imgHeader" runat="server" ImageUrl="../Styles/images/img_noteyellow.png" AlternateText="Header Image" ImageAlign="Top" CssClass="box_image" />
    </div>
    <div class="box_right_top"></div>
    <div class="box_right_content">
        <%= Text %>
        <p>
            <asp:PlaceHolder ID="plhBody" runat="server"></asp:PlaceHolder>
        </p>
    </div>
    <div class="box_right_bottom"></div>
</asp:PlaceHolder>