<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ucBoxBeigeHeader.ascx.cs" Inherits="SrtsWeb.UserControls.ucBoxBeigeHeader" %>

<asp:PlaceHolder ID="phContent" runat="server">

    <div id="divContainerBeige" style="margin-top: 30px">
        <div class="containertop">
            <div class="containertop_right"></div>
            <div class="containertop_left"></div>
        </div>

        <div class="container_content">
            <div class="container_inner">

                <h1 class="headerImage" style="margin: -55px 0px 15px 50px">
                    <span class="container_title_custom"><%= Title %></span>
                </h1>
                <div class="container_image_custom">
                    <asp:Image ID="imgHeader" runat="server" ImageUrl="../Styles/images/img_noteyellow.png" AlternateText="Header Image" ImageAlign="TextTop" CssClass="container_image" />
                </div>
</asp:PlaceHolder>