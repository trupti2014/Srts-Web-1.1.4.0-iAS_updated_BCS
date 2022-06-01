<%@ Page Language="C#" MasterPageFile="~/SrtsMaster.Master" AutoEventWireup="true" CodeBehind="SitePreferences.aspx.cs" Inherits="SrtsWeb.WebForms.Admin.SitePreferences" %>


<%@ MasterType VirtualPath="~/srtsMaster.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="../../Styles/PassFailConfirm.css" rel="stylesheet" />
    <style>
        #selectPreference li {
            width: 9.1em;
            padding: .5em 0 .5em 1.5em;
            margin-bottom: 1em;
            margin-left: 1em;
            font-family: 'Trebuchet MS', 'Lucida Grande', Verdana, Lucida, Geneva, Helvetica, Arial, sans-serif;
            font-size: .9em;
            color: #333;
            text-align: left;
            background-color: white;
            line-height: 25px;
        }

            #selectPreference li.active {
                border-top: 1px solid #ebd9c7;
                border-bottom: 1px solid #ebd9c7;
                border-left: 1px solid #ebd9c7;
                border-top-left-radius: 6px;
                border-bottom-left-radius: 6px;
                color: red!important;
                background-image: url("/../Styles/images/img_note22.png");
                background-position: 5px 5px;
                background-repeat: no-repeat;
                outline-width: 0px;
            }

            #selectPreference li a {
                text-decoration: none;
                padding-left: 5px;
            }

        .defaultPanels {
            margin: 10px 20px;
            padding: 20px;
        }

        .alignLeft {
            text-align: left;
        }

        .customValidators {
            margin-top:10px;
            max-height: 300px;
            padding: 20px;
            text-align: left;
            font-size: 13px;
            color: red;
        }

        .noShow {
            display:none;
        }
    </style>
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="MainContent" runat="server">
    <asp:ScriptManagerProxy ID="smpProxy" runat="server">
        <Scripts>
            <asp:ScriptReference Path="~/Scripts/Preferences/SitePreferences.js" />
            <asp:ScriptReference Path="~/Scripts/Global/PassFailConfirm.js" />
            <asp:ScriptReference Path="~/Scripts/Global/GlobalVal.js" />
            <asp:ScriptReference Path="~/Scripts/Preferences/PrescriptionPreferences.js" />
        </Scripts>
    </asp:ScriptManagerProxy>
    <asp:HiddenField id="hdfUserRole" runat="server" ClientIDMode="Static" />
    <asp:HiddenField id="hdfSiteType" runat="server" ClientIDMode="Static" />
    <asp:HiddenField ID="hdfSelectedPreference" runat="server" ClientIDMode="static" />
    <div id="divSingleColumns" style="margin: 0px 0px 20px 0px; padding-bottom: 0px; padding-top: 0px; border-bottom: 1px solid #ebd9c7">
        <div class="w3-row" style="margin: 0px; margin-top: -3px">

            <div class="w3-col" style="width: 150px; height: 480px; max-height: 480px">
                <div style="font-size: 11px; padding: 24px; text-align: left">Select an option below to manage site defaults.</div>
                <%--Menu Options--%>
                <div id="selectPreference">
                    <asp:UpdatePanel ID="uplMenu" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                    <ul>
                        <li id="lstLabParameters"><a href="#" id="lnkLabParameters">Lab Parameters</a></li>
                        <li id="lstLabJustifications"><a href="#" id="lnkLabJustifications">Lab Justifications</a></li>
   <%--                     <li id="lstLabMailToPatient"  runat="server" ClientIdMode="Static"><a href="#" id="lnkLabMailToPatient">Mail-to-Patient</a></li>--%>
                        <li id="lstGeneral"><a href="#" id="lnkGeneral">General</a></li>
                      <%--  <li id="lstShipping"><a href="#" id="lnkShipping">Shipping</a></li>--%>
                        <li id="lstRoutingOrders"><a href="#" id="lnkRoutingOrders">Routing Orders</a></li>
                        <li id="lstOrders" class="active"><a href="#" id="lnkOrders">Orders</a></li>
                        <li id="lstFrames"><a href="#" id="lnkFrames">Frames</a></li>
                        <li id="lstPrescriptions"><a href="#" id="lnkPrescriptions">Prescriptions</a></li>
                    </ul>
                    </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </div>

            <div class="w3-rest" style="border-left: 1px solid #ebd9c7">
                <%--Site Name / Site Selection --%>
                <div style="height: 30px; border-bottom: solid 1px #ead9c8; padding-top: 10px;">
                    <div class="w3-col" style="width: 500px; padding-top: 0px">
                        <h1 style="text-align: left; padding-left: 10px; padding-top: 3px" class="w3-medium">Site Name/Code:  <span style="color: #000">
                            <asp:Literal ID="litSiteName" runat="server" /></span></h1>
                    </div>
                    <div id="divSelectSite" runat="server" class="col-rest" style="text-align: left">
                        <h1 class="w3-medium" style="margin-top: 3px">Select a Site to Manage:
                            <asp:DropDownList ID="ddlSiteCode" runat="server" CssClass="srtsDropDown_medium" AutoPostBack="true" OnSelectedIndexChanged="ddlSiteCode_SelectedIndexChanged"></asp:DropDownList>
                        </h1>
                    <%--    <ajaxToolkit:ListSearchExtender runat="server" TargetControlID="ddlSiteCode" ID="LSE_ddlSiteCode" Enabled="True" PromptText="Type to search" QueryTimeout="2" PromptCssClass="listSearchPromptOverlay" QueryPattern="Contains"></ajaxToolkit:ListSearchExtender>
         --%>           </div>
                </div>
                <br />
                <div style="padding-bottom:20px">
                <%--Manage Orders --%>
                <div id="pnlOrders" style="height: 450px; max-height: 450px; display: block">
                    <h1 style="padding-top: 0px; text-align: center" class="w3-large">Manage Orders Preferences</h1>
                    <div class="defaultPanels">
                        <srts:SitePreferencesOrders ID="srtsOrders" runat="server" />
                    </div>
                </div>

                <%--Manage Frames --%>
                <div id="pnlFrames" style="height: 550px; max-height: 550px; display: none">
                    <h1 style="padding-top: 0px; text-align: center" class="w3-large">Manage Frames Preferences</h1>
                    <div class="defaultPanels">
                        <srts:SitePreferencesFrames ID="SitePreferencesFrames" runat="server" />
                    </div>
                </div>

                <%--Manage Orders --%>
                <div id="pnlPrescriptions" style="height: 450px; max-height: 450px; display: none">
                    <h1 style="padding-top: 0px; text-align: center" class="w3-large">Manage Prescriptions Preferences</h1>
                    <div class="defaultPanels">
                        <asp:UpdatePanel ID="uplPrescriptions" runat="server" UpdateMode="Always">
                            <ContentTemplate>
                                <srts:SitePreferencesPrescriptions ID="SitePreferencesPrescriptions" runat="server" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                </div>

                <%--Manage General Preferences --%>
                <div id="pnlGeneral" style="height: 450px; max-height: 450px; display: none">
                    <h1 style="padding-top: 0px; text-align: center" class="w3-large">Manage General Site Preferences</h1>
                    <div class="defaultPanels">
                        <srts:SitePreferencesGeneral ID="SitePreferencesGeneral" runat="server" />
                    </div>
                </div>

                <%--Manage Lab Parameters --%>
                <div id="pnlLabParameters" style="height: 650px; max-height: 650px; display: none">
                    <h1 style="padding-top: 0px; text-align: center" class="w3-large">Manage Lab Parameters</h1>
                    <div class="defaultPanels">
                        <srts:SitePreferencesLabParameters ID="SitePreferencesLabParameters" runat="server" />
                    </div>
                </div>

                <%--Manage Lab Justifications --%>
                <div id="pnlLabJustifications" style="height: 450px; max-height: 450px; display: none">
                    <h1 style="padding-top: 0px; text-align: center" class="w3-large">Manage Lab Justifications</h1>
                    <div class="defaultPanels">
                        <srts:SitePreferencesLabJustifications ID="SitePreferencesLabJustifications" runat="server" />
                    </div>
                </div>

                <%--Manage Lab Mail To Patient --%>
                <div id="pnlLabMailToPatient" style="height: 450px; max-height: 450px; display: none">
                    <h1 style="padding-top: 0px; text-align: center" class="w3-large">Manage Lab Mail-to-Patient Status</h1>
                    <div class="defaultPanels">
                         <srts:SitePreferencesLabMailToPatient ID="SitePreferencesLabMailToPatient" runat="server" /> 
                    </div>
                </div>

                 <%--Manage Shipping --%>
               <%-- <div id="pnlShipping" style="height: 450px; max-height: 450px; display: none">
                    <h1 style="padding-top: 0px; text-align: center" class="w3-large">Manage Shipping</h1>
                    <div class="defaultPanels">
                         <srts:SitePreferencesShipping ID="SitePreferencesShipping1" runat="server" />
                    </div>
                </div>--%>

                 <%--Manage Routing Orders --%>
                <div id="pnlRoutingOrders" style="height: 500px; max-height: 500px; display: none">
                    <h1 style="padding-top: 0px; text-align: center" class="w3-large">Manage Lab Routing Orders</h1>
                    <div class="defaultPanels">
                         <srts:SitePreferencesRoutingOrders ID="SitePreferencesRoutingOrders" runat="server" />
                    </div>
                </div>
           
                </div>

            </div>

        </div>
    </div>
</asp:Content>
