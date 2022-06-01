<%@ Page Title="" Language="C#" MasterPageFile="~/srtsMaster.Master" AutoEventWireup="true" CodeBehind="Accessability.aspx.cs" Inherits="SrtsWeb.Public.Accessability" %>

<%@ MasterType VirtualPath="~/srtsMaster.Master" %>

<asp:Content ID="Content6" ContentPlaceHolderID="MainContent_Public" runat="server">
      <div class="box_full_top" style="margin-top:30px;padding-top:10px;text-align:center;margin-bottom:-10px">
        <h1 style="font-size: 1.4em;margin-bottom:0px">Web Site Accessibility</h1>
    </div>
    <div class="box_full_content" style="min-height: 300px">
        <div class="padding">

            <h2 style="text-align: center">
                <asp:Literal ID="litModuleTitle" runat="server" /></h2>
            <div class="padding" style="margin: 0px 30px">
                <p>
                    The web-based Spectacle Request Transmission System (SRTSweb) website and application is provided as a service by the <a href="https://info.health.mil" target="_blank">Defense Health Agency (DHA)</a>.
                </p>
                <br />
                <p>
                    We are committed to making our web sites accessible to the widest possible audience, including to individuals with disabilities. If you are unable to access a piece of information that is contained within this web site, please <a href="Support.aspx">contact us</a>.
                </p>
                <br />
                <p>
                    When contacting us, let us know the nature of your accessibility problem, the web address of the requested information, and your contact information.
                </p>
                <br />
                <h1 class="headerBlue" style="text-align: center; margin: 30px 0px 0px 0px; line-height: 25px">It is our commitment<br />
                    to ensure that you are able to access<br />
                    the information that you need.
                </h1>
                <br />
                <br />
            </div>
        </div>
    </div>
    <div class="box_full_bottom"></div>
</asp:Content>