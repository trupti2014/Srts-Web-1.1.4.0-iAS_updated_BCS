<%@ Page Title="" Language="C#" MasterPageFile="~/srtsMaster.Master" AutoEventWireup="true" CodeBehind="PrivacyAndSecurity.aspx.cs" Inherits="SrtsWeb.Public.PrivacyAndSecurity" %>

<%@ MasterType VirtualPath="~/srtsMaster.Master" %>

<asp:Content ID="Content6" ContentPlaceHolderID="MainContent_Public" runat="server">
     <div class="box_full_top" style="margin-top:30px;padding-top:10px;text-align:center;margin-bottom:-10px">
        <h1 style="font-size: 1.4em;margin-bottom:0px">Privacy and Security Notice</h1>
    </div>
    <div class="box_full_content" style="min-height: 300px">
        <div class="padding">
            <h2 style="text-align: center">
                <asp:Literal ID="litModuleTitle" runat="server" /></h2>
            <!--Privacy and security information  -->
            <div class="padding">
                <ol class="general">
                    <li>The web-based Spectacle Request Transmission System (SRTSweb) website and application is provided as a service by the <a href="https://info.health.mil" target="_blank">Defense Health Agency (DHA)</a>.
                    </li>
                    <li>Information presented on the SRTS Homepage is considered public information and may be distributed or copied. Use of appropriate byline/photo/image credits is requested.
                    </li>
                    <li>For site management, <a href="InformationCollected.aspx">information is collected</a> for statistical purposes. This government computer system uses software programs to create summary statistics, which are used for such purposes as assessing what information is of most and least interest, determining technical design specifications, and identifying system performance or problem areas.
                    </li>
                    <li>For site security purposes and to ensure that this service remains available to all users, this government computer system employs software programs to monitor network traffic to identify unauthorized attempts to upload or change information, or otherwise cause damage.
                    </li>
                    <li>Cookie Disclaimer - This website does not use persistent cookies (persistent tokens that pass information back and forth from the client machine to the server). SRTS uses session cookies (tokens that remain active only until you close your browser) in order to make the site easier to use. The Department of Defense DOES NOT keep a database of information obtained from these cookies. You can choose not to accept these cookies and still use the site.
                    </li>
                    <li>Except for authorized law enforcement investigations, no other attempts are made to identify individual users or their usage habits. Raw data logs are used for no other purposes and are scheduled for regular destruction in accordance with National Archives and Records Administration Guidelines.
                    <br />
                        All data collection activities are in strict accordance with DoD Directive 5240.1 (reference (p)).
                    </li>
                    <li>Unauthorized attempts to upload information or change information on this service are strictly prohibited and may be punishable under the Computer Fraud and Abuse Act of 1987 and the National Information Infrastructure Protection Act.<br />
                        <br />
                    </li>
                </ol>
            </div>
        </div>
    </div>
    <div class="box_full_bottom"></div>
</asp:Content>