<%@ Page Title="" Language="C#" MasterPageFile="~/srtsMaster.Master" AutoEventWireup="true" CodeBehind="UserManagement.aspx.cs" Inherits="SrtsWeb.Admin.UserManagement" %>

<%@ MasterType VirtualPath="~/srtsMaster.Master" %>
<asp:Content ID="ContentHead" ContentPlaceHolderID="HeadContent" runat="server">
    <link rel="stylesheet" type="text/css" href="../../Styles/w3.css" />
    <link href="../../Styles/PassFailConfirm.css" rel="stylesheet" />
    <style type="text/css">
        .stackLeft {
            display: inline-block;
            float: left;
            margin: 10px 0px 10px 10px;
            /*margin-left: 10px;*/
        }

        .marginOnly {
            display: inline-block;
            margin: 10px 0px 10px 10px;
            /*margin-left: 10px;*/
        }

        .headerStyle {
            height: 80px;
            vertical-align: top;
        }

        .NewUserLabel {
            color: #143289;
            font-size: 17px;
        }

        .lblName {
            padding-left: 40px;
            color: #143289;
            font-size: 19px;
            font-weight: bold;
        }

        .idError {
            color: #f00;
        }

        .leftTableCell {
            text-align: right;
            margin: 0px 5px 2px 0px;
        }

        .rightTableCell {
            text-align: left;
            margin: 0px 0px 2px 5px;
        }

        .stepHeader {
            text-align: left;
            padding: 0px;
            margin: 8px 0px 0px 15px;
            color: #004994;
            font-size: 1.3em;
            letter-spacing: .5px;
        }

        .searchError {
            text-align: center;
            color: red;
            font-size: 1.0em;
            padding: 10px 0px;
        }
    </style>
</asp:Content>
<asp:Content ID="ContentMain" ContentPlaceHolderID="MainContent" runat="server">
    <asp:ScriptManagerProxy ID="smpScripts" runat="server">
        <Scripts>
            <asp:ScriptReference Path="~/Scripts/Global/PassFailConfirm.js" />
            <asp:ScriptReference Path="~/Scripts/Global/GlobalVal.js" />
            <asp:ScriptReference Path="~/Scripts/NewUser/NewUser.js" />
        </Scripts>
    </asp:ScriptManagerProxy>
    <asp:UpdatePanel ID="upNewUser" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
        <ContentTemplate>
            <div id="divError" style="width: 80%;"></div>
            <!-- Step 1 -->
            <div style="text-align: left!important; margin: 20px 20px 25px 20px">
                <div class="w3-pale-blue w3-leftbar w3-border-green w3-hover-border-green">
                    <div class="w3-col" style="width: 250px">
                        <div class="stepHeader">Step 1: Search Individual</div>
                    </div>
                    <div class="w3-rest">
                        <p style="min-width: 500px; min-height: 40px">&nbsp;&nbsp;</p>
                        <p></p>
                    </div>
                </div>
            </div>

            <div id="searchError" class="searchError" runat="server" visible="false">
                No record found, you must 
                <span>[
                    <asp:HyperLink ID="HyperLink1" runat="server" Text="Add Individual" NavigateUrl="~/WebForms/SrtsPerson/AddPerson.aspx/add">
                    </asp:HyperLink>
                    &nbsp;]
                </span>
                before continuing with user account creation.
            </div>

            <div style="margin: 0px 0px 10px 0px;">
                <asp:Label ID="lblIDNumber" runat="server" Text="Enter DoD ID or SSN:" CssClass="NewUserLabel"></asp:Label>
                <asp:TextBox ID="tbIDNumber" runat="server" ClientIDMode="Static" CssClass="" onblur="validateID()" TextMode="Number" MaxLength="10"></asp:TextBox>
                <asp:Label ID="lblIDNumError" ClientIDMode="Static" CssClass="idError" runat="server"></asp:Label>

                <asp:Button ID="btnSearch" runat="server" ClientIDMode="Static" OnClientClick="return SearchClick();" CssClass="srtsButton" Text="Search" OnClick="btnSearch_Click" />
                <asp:Label ID="lblName" CssClass="lblName" ClientIDMode="Static" runat="server" Text=""></asp:Label>
            </div>
            <asp:Panel ID="pnlComplete" runat="server" Visible="false">

                <!-- Step 2 -->
                <div style="text-align: left!important; margin: 20px 20px 25px 20px;">
                    <div class="w3-pale-blue w3-leftbar w3-border-green w3-hover-border-green">
                        <div class="w3-col" style="width: 250px">
                            <div class="stepHeader">Step 2: Setup Profile</div>
                        </div>
                        <div class="w3-rest">
                            <p style="min-width: 500px; min-height: 40px">&nbsp;&nbsp;</p>
                            <p></p>
                        </div>
                    </div>
                </div>
                <!-- Site Location, Email Address, CMS Access, CAC Enabled -->
                <div style="margin-left: 30px;">
                    <table style="text-align: left; width: 90%;">
                        <tr>
                            <td>
                                <asp:Label ID="label6" runat="server" Text="Site Location" CssClass="NewUserLabel"></asp:Label>
                                <span style="font-size: 80%">&nbsp;&nbsp;(Individual's site location)</span>
                            </td>
                            <td>
                                <asp:Label ID="label16" runat="server" Text="Enter Email Address" CssClass="NewUserLabel"></asp:Label>
                                <span style="font-size: 80%">&nbsp;&nbsp;(An email address is required for password resets)</span></td>
                        </tr>
                        <tr>
                            <td>
                                <asp:DropDownList ID="ddlDestinationSiteCodes" runat="server" ClientIDMode="Static" OnSelectedIndexChanged="ddlDestinationSiteCodes_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                                <ajaxToolkit:ListSearchExtender ID="lseDestSite" runat="server" TargetControlID="ddlDestinationSiteCodes"
                                    PromptText="Type to search" QueryTimeout="2" PromptCssClass="listSearchPromptOverlay" QueryPattern="Contains">
                                </ajaxToolkit:ListSearchExtender>
                            </td>
                            <td>
                                <asp:TextBox ID="tbEmail" runat="server" CssClass="srtsTextBox" ClientIDMode="Static"></asp:TextBox></td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label ID="lblDestSiteCodeError" runat="server" Text="* Destination site code is a required field." ForeColor="Red" Visible="false"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="lblEmailError" runat="server" Text="* Email address is a required field." ForeColor="Red" Visible="false"></asp:Label>
                                <asp:RegularExpressionValidator ID="EmailValid" runat="server" Display="Dynamic"
                                    ValidationExpression="^([a-zA-Z][\w\.-]*[a-zA-Z0-9]@[a-zA-Z0-9][\w\.-]*[a-zA-Z0-9]\.[a-zA-Z][a-zA-Z\.]*[a-zA-Z]){1,70}$"
                                    CssClass="colorRed" ErrorMessage="* Invalid email format entered" ControlToValidate="tbEmail"></asp:RegularExpressionValidator></td>
                        </tr>
                        <tr>
                            <td colspan="2">&nbsp;</td>
                        </tr>
                        <tr>
                            <td>
                                <asp:CheckBox ID="cbIsCms" runat="server" Text="CMS access Required?&nbsp;&nbsp;" TextAlign="Left" CssClass="NewUserLabel" />
                                <span style="font-size: 80%">&nbsp;&nbsp;(Check if this user will require CMS access?)</span>
                            </td>
                            <td>
                                <asp:CheckBox ID="cbCacEnable" runat="server" Text="CAC Enable This User&nbsp;&nbsp;" TextAlign="Left" CssClass="NewUserLabel" />
                            </td>
                        </tr>
                    </table>
                </div>
                <div style="clear: both;"></div>

                <!-- Step 3 -->
                <div style="text-align: left!important; margin: 20px 20px 25px 20px;">
                    <div class="w3-pale-blue w3-leftbar w3-border-green w3-hover-border-green">
                        <div class="w3-col" style="width: 250px">
                            <div class="stepHeader">Step 3: Select User Role</div>
                        </div>
                        <div class="w3-rest">
                            <p style="min-width: 500px; min-height: 40px">&nbsp;&nbsp;</p>
                            <p></p>
                        </div>
                    </div>
                </div>
                <asp:UpdatePanel ID="upRoles" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
                    <ContentTemplate>
                        <%-- <div style="width: 60%; margin: 0px 25%; text-align: left">--%>
                        <div>
                            <%--<table style="text-align: center; float: left; width: 50%;">--%>
                            <table style="width: 60%; margin-left: auto; margin-right: auto;">
                                <tr>
                                    <td>
                                        <fieldset style="float: right;">
                                            <legend class="NewUserLabel">Available Roles</legend>
                                            <asp:ListBox ID="lbAvailable" runat="server" SelectionMode="Multiple" Height="100px" Width="120px"></asp:ListBox>
                                        </fieldset>
                                    </td>
                                    <td style="vertical-align: middle;">
                                        <asp:ImageButton ID="bAddRole" runat="server" ImageUrl="~/Styles/images/img_arrow_right.png" OnClick="bAddRole_Click" /><br />
                                        <asp:ImageButton ID="bRemRole" runat="server" ImageUrl="~/Styles/images/img_arrow_left.png" OnClick="bRemRole_Click" />
                                    </td>
                                    <td>
                                        <fieldset style="float: left;">
                                            <legend class="NewUserLabel">Assigned Role</legend>
                                            <asp:ListBox ID="lbAssigned" runat="server" SelectionMode="Multiple" Height="100px" Width="120px"></asp:ListBox>
                                        </fieldset>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="3">
                                        <div style="margin-top: 5px;">
                                            <asp:Label ID="lblRoleMsg" runat="server" Text="* The new user must have at least one assigned role." ForeColor="Red" Visible="false" />
                                        </div>
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="bAddRole" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="bRemRole" EventName="Click" />
                    </Triggers>
                </asp:UpdatePanel>
                <%--<div style="clear: both;"></div>--%>
            </asp:Panel>

            <!-- Step 4 -->
            <div id="divSuccessDialog" style="display: none;">
                <div style="margin: 20px; text-align: left;">
                    <table style="width: 90%;">
                        <tr>
                            <td class="leftTableCell">
                                <asp:Label ID="Label7" runat="server" Text="User Name:" CssClass="NewUserLabel" /></td>
                            <td>&nbsp;&nbsp;&nbsp;</td>
                            <td class="rightTableCell">
                                <asp:Label ID="lblUserName" runat="server"></asp:Label></td>
                        </tr>
                        <tr>
                            <td class="leftTableCell">
                                <asp:Label ID="Label15" runat="server" Text="Password:" CssClass="NewUserLabel" /></td>
                            <td>&nbsp;&nbsp;&nbsp;</td>
                            <td class="rightTableCell">
                                <asp:Label ID="lblPassword" runat="server"></asp:Label></td>
                        </tr>
                        <tr>
                            <td class="leftTableCell">
                                <asp:Label ID="Label8" runat="server" Text="First Name:" CssClass="NewUserLabel" /></td>
                            <td>&nbsp;&nbsp;&nbsp;</td>
                            <td class="rightTableCell">
                                <asp:Label ID="lblFirstName" runat="server" /></td>
                        </tr>
                        <tr>
                            <td class="leftTableCell">
                                <asp:Label ID="Label9" runat="server" Text="Last Name:" CssClass="NewUserLabel" /></td>
                            <td>&nbsp;&nbsp;&nbsp;</td>
                            <td class="rightTableCell">
                                <asp:Label ID="lblLastName" runat="server" /></td>
                        </tr>
                        <tr>
                            <td class="leftTableCell">
                                <asp:Label ID="Label10" runat="server" Text="Roles:" CssClass="NewUserLabel" /></td>
                            <td>&nbsp;&nbsp;&nbsp;</td>
                            <td class="rightTableCell">
                                <asp:Label ID="lblRoles" runat="server" /></td>
                        </tr>
                        <tr>
                            <td class="leftTableCell">
                                <asp:Label ID="Label11" runat="server" Text="Location:" CssClass="NewUserLabel" /></td>
                            <td>&nbsp;&nbsp;&nbsp;</td>
                            <td class="rightTableCell">
                                <asp:Label ID="lblLocation" runat="server" /></td>
                        </tr>
                        <tr>
                            <td class="leftTableCell">
                                <asp:Label ID="Label13" runat="server" Text="Email Address:" CssClass="NewUserLabel" />
                            </td>
                            <td>&nbsp;&nbsp;&nbsp;</td>
                            <td class="rightTableCell">
                                <asp:Label ID="lblEmail" runat="server" />
                            </td>
                        </tr>
                        <tr>
                            <td class="leftTableCell">
                                <asp:Label ID="Label12" runat="server" Text="CMS User:" CssClass="NewUserLabel" /></td>
                            <td>&nbsp;&nbsp;&nbsp;</td>
                            <td class="rightTableCell">
                                <asp:Label ID="lblIsCms" runat="server" /></td>
                        </tr>
                        <tr>
                            <td class="leftTableCell">
                                <asp:Label ID="Label14" runat="server" Text="CAC Enabled:" CssClass="NewUserLabel" /></td>
                            <td>&nbsp;&nbsp;&nbsp;</td>
                            <td class="rightTableCell">
                                <asp:Label ID="lblCacEnable" runat="server" /></td>
                        </tr>
                    </table>
                </div>
            </div>

            <div style="text-align: center; margin-top: 30px;">
                <asp:Button ID="bSubmit" runat="server" CssClass="srtsButton" Text="Submit" OnClick="bSubmit_Click" />
                <asp:Button ID="bExit" runat="server" CssClass="srtsButton" Text="Exit" OnClick="bExit_Click" />
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="bSubmit" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="btnSearch" EventName="Click" />
            <%--<asp:AsyncPostBackTrigger ControlID="ddlIndividuals" EventName="SelectedIndexChanged" />--%>
            <%--<asp:AsyncPostBackTrigger ControlID="ddlIndSourceSiteCode" EventName="SelectedIndexChanged" />--%>
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>
