<%@ Page Language="C#" MasterPageFile="~/SrtsMaster.Master" AutoEventWireup="true" CodeBehind="ManageFrames.aspx.cs" Inherits="SrtsWeb.WebForms.Admin.ManageFrames" %>

<%@ Register Assembly="SrtsWeb" Namespace="SrtsWeb.UserControls" TagPrefix="srts" %>

<%@ MasterType VirtualPath="~/srtsMaster.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="../../Styles/PassFailConfirm.css" rel="stylesheet" />
    <style>
        #selectDatabaseTool li {
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

            #selectDatabaseTool li.active {
                border-top: 1px solid #ebd9c7;
                border-bottom: 1px solid #ebd9c7;
                border-left: 1px solid #ebd9c7;
                border-top-left-radius: 6px;
                border-bottom-left-radius: 6px;
                color: red !important;
                background-image: url("/../Styles/images/img_note22.png");
                background-position: 5px 5px;
                background-repeat: no-repeat;
                outline-width: 0px;
            }

            #selectDatabaseTool li a {
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
            margin-top: 10px;
            max-height: 300px;
            padding: 20px;
            text-align: left;
            font-size: 13px;
            color: red;
        }

        .noShow {
            display: none;
        }
    </style>
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="MainContent" runat="server">
    <asp:ScriptManagerProxy ID="smpDatabaseToolsProxy" runat="server">
        <Scripts>
            <asp:ScriptReference Path="~/Scripts/Global/PassFailConfirm.js" />
            <asp:ScriptReference Path="~/Scripts/Global/GlobalVal.js" />
            <asp:ScriptReference Path="~/Scripts/ManagementEnterprise/FrameImages.js" />
        </Scripts>
    </asp:ScriptManagerProxy>
    <asp:HiddenField ID="hfActivePanel" runat="server" Value="" ClientIDMode="Static" />
    <asp:HiddenField ID="hdfSelectedTool" runat="server" ClientIDMode="static" />
    <div id="divSingleColumns" style="margin: 0px 0px 20px 0px; padding-bottom: 0px; padding-top: 0px; border-bottom: 1px solid #ebd9c7">
        <div class="w3-row" style="margin: 0px; margin-top: -3px">

            <div class="w3-col" style="width: 150px; height: auto">
                <div style="font-size: 11px; padding: 24px; text-align: left">Select an option below to manage Frame Images.</div>
                <%--Menu Options--%>
                <div id="selectDatabaseTool">
                    <asp:UpdatePanel ID="uplMenu" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <ul>
                                <li id="lstFrameImages"><a href="#" id="lnkFrameImages">Add Frame Images</a></li>
                                <li id="lstFrameImagesEdit"><a href="#" id="lnkFrameImagesEdit">Edit Frame Images</a></li>
                            </ul>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </div>

            <div class="w3-rest" style="border-left: 1px solid #ebd9c7">
                <%--Panel Description --%>
                <div style="height: 30px; border-bottom: solid 1px #ead9c8; padding-top: 10px;">
                    <div class="w3-col" style="width: 100%; padding-top: 0px">
                        <h1 style="text-align: left; padding-left: 10px; padding-top: 3px" class="w3-medium">The 'Manage Frame Images' tool allows 
                            inserting and updating of the images associated with the frames available to order.  <span style="color: #000"></span></h1>
                    </div>
                </div>
                <br />

                <%--Frame Image Tools --%>
                <div id="pnlFrameImages" style="height: auto; display: block">
                    <h1 style="padding-top: 0px; text-align: center" class="w3-large">Manage Frame Images</h1>
                    <div style="margin-top: 30px; padding: 10px 20px">
                        <srts:ManageFrame ID="ucManageFrames" runat="server" />
                    </div>
                </div>
                <div id="pnlFrameImagesEdit" style="height: auto; display: block">
                    <h1 style="padding-top: 0px; text-align: center" class="w3-large">Manage Frame Images Edit</h1>
                    <div style="margin-top: 30px; padding: 10px 20px">
                        <srts:ManageFrameEdit ID="ucManageFramesEdit" runat="server" />
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
