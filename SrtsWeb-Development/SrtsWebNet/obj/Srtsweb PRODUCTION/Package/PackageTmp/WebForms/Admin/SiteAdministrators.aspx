<%@ Page Language="C#" MasterPageFile="~/SrtsMaster.Master" AutoEventWireup="true" CodeBehind="SiteAdministrators.aspx.cs" Inherits="SrtsWeb.WebForms.Admin.SiteAdministrators" %>


<%@ MasterType VirtualPath="~/srtsMaster.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="../../Styles/PassFailConfirm.css" rel="stylesheet" />
    <style>
        #selectSiteType li {
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

            #selectSiteType li.active {
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

            #selectSiteType li a {
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
            <asp:ScriptReference Path="~/Scripts/Admin/SiteAdministrators.js" />
            <asp:ScriptReference Path="~/Scripts/Global/GlobalVal.js" />
        </Scripts>
    </asp:ScriptManagerProxy>
    <asp:HiddenField id="hdfUserRole" runat="server" ClientIDMode="Static" />
    <asp:HiddenField id="hdfSiteType" runat="server" ClientIDMode="Static" />
    <asp:HiddenField ID="hdfSelectedPreference" runat="server" ClientIDMode="static" />
    <div id="divSingleColumns" style="margin: 0px 0px 20px 0px; padding-bottom: 0px; padding-top: 0px; border-bottom: 1px solid #ebd9c7">
        <div class="w3-row" style="margin: 0px; margin-top: -3px">

            <div class="w3-col" style="width: 150px; height: 480px; max-height: 480px">
                <div style="font-size: 11px; padding: 24px; text-align: left">Select an option below to look up site admin information.</div>
                <%--Menu Options--%>
                <div id="selectSiteType">
                    <asp:UpdatePanel ID="uplMenu" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                    <ul>
                        <li id="lstClinicAdmins" class="active"><a href="#" id="lnkClinicAdmins">Clinic Admins</a></li>
                        <li id="lstLabAdmins"><a href="#" id="lnkLabAdmins">Lab Admins</a></li>
                    </ul>
                    </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </div>

            <div class="w3-rest" style="border-left: 1px solid #ebd9c7">
                <br />
                <div style="padding-bottom:20px">
                <%--Clinic Admins--%>
                <div id="pnlClinicAdmins" style="height: 450px; max-height: 450px; display: block">
                    <h1 style="padding-top: 0px; text-align: center" class="w3-large">Lookup Site Administrators</h1>
                    <div class="defaultPanels">
                        <srts:SiteAdministratorsLookup ID="clinicAdmins" runat="server" AdminSiteType="CLINIC"  />
                    </div>
                </div>
                <%--Lab Admins--%>
                <div id="pnlLabAdmins" style="height: 450px; max-height: 450px; display: none">
                    <h1 style="padding-top: 0px; text-align: center" class="w3-large">Lookup Site Administrators</h1>
                    <div class="defaultPanels">
                        <srts:SiteAdministratorsLookup ID="labAdmins" runat="server" AdminSiteType="LABORATORY"  />
                    </div>
                </div>

                </div>
            </div>
        </div>
    </div>
</asp:Content>
