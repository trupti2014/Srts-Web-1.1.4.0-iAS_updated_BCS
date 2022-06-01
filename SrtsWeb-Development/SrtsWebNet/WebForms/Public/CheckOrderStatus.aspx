<%@ Page Title="" Language="C#" MasterPageFile="~/srtsMaster.Master" AutoEventWireup="true" CodeBehind="CheckOrderStatus.aspx.cs" Inherits="SrtsWeb.WebForms.Public.CheckOrderStatus" %>

<%@ MasterType VirtualPath="~/srtsMaster.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link rel="stylesheet" type="text/css" href="../../Styles/w3.css" />
    <style>
        .pointer_header {
            background-image: url("../../Styles/images/img_hint_white.png");
            background-repeat: no-repeat;
            background-position: 80px -3px;
            cursor: pointer;
            width: 150px;
            margin-top: 0px 0px 10px 0px;
            padding-bottom: 10px;
        }

        .pointer_header2 {
            background-image: url("../../Styles/images/img_hint_white.png");
            background-repeat: no-repeat;
            background-position: 110px -3px;
            cursor: pointer;
            width: 180px;
            margin: 10px 0px 3px 5px;
            padding-bottom: 10px;
        }

        .pointer {
            cursor: pointer;
            width: 180px;
            margin-top: -20px;
            margin-left: -30px;
            font-weight: bold;
        }

        .pointer_small {
            cursor: pointer;
            width: 130px;
            margin-top: -20px;
            margin-left: -30px;
            font-weight: bold;
        }

        .headerClass > th {
            text-align: left;
            padding-left: 10px;
        }

        #divOrderDetailToolTip {
            position: absolute;
            top: -210px;
            left: 120px;
            bottom: 150px;
            z-index: 4000;
            height: 50px;
            width: 350px!important;
        }

        #divOrdersListToolTip {
            position: absolute;
            top: -135px;
            left: 140px;
            bottom: 150px;
            z-index: 4000;
            height: 50px;
            width: 350px!important;
        }

        #divFrameToolTip {
            position: absolute;
            top: -180px;
            left: 120px;
            bottom: 150px;
            z-index: 4000;
            height: 175px;
            width: 350px!important;
        }

        #divLensToolTip {
            position: absolute;
            top: -155px;
            left: 260px;
            bottom: 150px;
            z-index: 4000;
            height: 150px;
            width: 400px!important;
        }

        #divTintToolTip {
            position: absolute;
            top: -120px;
            left: 370px;
            bottom: 150px;
            z-index: 4000;
            height: 130px;
            width: 360px!important;
        }

        #divLabToolTip {
            position: absolute;
            top: -155px;
            left: 450px;
            bottom: 150px;
            z-index: 4000;
            height: 130px;
            width: 360px!important;
        }

        #divCurrentOrderStatusToolTip {
            position: absolute;
            top: -185px;
            left: 40px;
            bottom: 150px;
            z-index: 3000;
            width: 550px!important;
        }

        .content {
            padding: 10px;
        }

            .content div {
                text-align: left!important;
                font-size: 12px;
                line-height: 1.5;
            }

        .descriptionItem {
            margin-bottom: 10px;
        }

        .colorBlue {
            color: #1252a0;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent_Public" runat="server">
    <asp:UpdatePanel ID="upOrderStatus" runat="server" ChildrenAsTriggers="false" UpdateMode="Conditional">
        <ContentTemplate>
            <div id="divIdEntry" runat="server">
                <div class="box_full_top" style="margin-top: 30px; padding-top: 10px; text-align: center; margin-bottom: -10px">
                    <h1 style="font-size: 1.4em; margin-bottom: 0px">Check My Order Status</h1>
                </div>
                <div class="box_full_content" style="min-height: 250px">
                    <div id="divError"></div>
                    <div class="w3-row" style="margin-bottom: 10px">
                        <div class="w3-col" style="width: 500px">
                            <div class="padding">
                                <div style="float: left; margin: 0px 0px 0px 15px;">
                                    <asp:Label ID="lbl1" runat="server" CssClass="srtsLabel_medium" Text="Enter SSN or DODID to check order statuses for last 2 years:"></asp:Label>
                                </div>
                                <div style="clear: left; float: left; margin: 10px 0px 10px 15px;">
                                    <asp:TextBox ID="tbIdNum" runat="server" CssClass="srtsTextBox_medium" ClientIDMode="Static"></asp:TextBox>
                                </div>
                                <div style="clear: left; float: left; margin: 20px 20px 20px 15px;">
                                    <asp:Label ID="CaptchaLabel" runat="server" style="margin: 5px 0px 0px 0px;" AssociatedControlID="CaptchaCode">Retype the characters from the picture:</asp:Label>
                                    <BotDetect:WebFormsCaptcha ID="CheckOrderStatusCaptcha" runat="server" style="margin: 5px 0px 0px 0px;" UserInputID="CaptchaCode" />
                                    <asp:TextBox ID="CaptchaCode" runat="server" Width="247px" style="margin: 5px 0px 0px 0px;"/>
                                    <br />
                                    <asp:RequiredFieldValidator ID="rfvCaptchaCode" runat="server" ControlToValidate="CaptchaCode" ErrorMessage="Retyping the code from the picture is required" Display="Dynamic" />
                                    <asp:CustomValidator runat="server" ID="CaptchaValidator" ControlToValidate="CaptchaCode" ErrorMessage="Incorrect CAPTCHA code. Please retype the code from the picture." OnServerValidate="CaptchaValidator_ServerValidate" />
                                </div>
                                <div style="clear: left; float: left; margin: 25px 0px 0px 10px;">
                                    <asp:Button ID="bSubmit" runat="server" CssClass="srtsButton" Text="Submit" OnClick="bSubmit_Click" OnClientClick="return IdNumGood();" />
                                    <asp:Button ID="bExitSearch" runat="server" CssClass="srtsButton" Text="Exit" OnCommand="Exit_Command" />
                                </div>
                            </div>

                        </div>
                        <div class="w3-rest" style="margin-top: 40px">
                            <div id="divMessage" runat="server" class="w3-panel w3-pale-red w3-border w3-border-red" style="padding: 20px; height: auto; width: 400px; visibility: hidden">
                                <asp:Literal ID="litMessage" runat="server"></asp:Literal>
                            </div>
                        </div>
                    </div>
                    <div style="clear: both;"></div>
                </div>
                <div class="box_full_bottom"></div>
            </div>


            <div id="divOrderStatus" runat="server">
                <!-- Patient Name Header -->
                <div id="divPatientNameHeader" runat="server" visible="true">
                    <asp:Literal ID="litPatientNameHeader" runat="server"></asp:Literal>
                </div>

                <!-- Order Detail -->
                <div id="divOrderDetail" runat="server" class="padding" visible="false">
                    <div class="pointer_header colorBlue"><span onmouseover="GetOrderDetailToolTip(); return false" onmouseout="Hide('divOrderDetailToolTip'); return false">Order Detail&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</span></div>
                    <asp:GridView ID="gvOrderStatus" runat="server" AutoGenerateColumns="false" CssClass="mGrid" AlternatingRowStyle-CssClass="alt">
                        <Columns>
                            <asp:BoundField DataField="ClinicSiteCode" HeaderText="Ordering Clinic" />
                            <asp:BoundField DataField="OrderNumber" HeaderText="Order Number" />
                            <asp:BoundField DataField="FrameCombo" HeaderText="Frame" />
                        </Columns>
                    </asp:GridView>
                </div>

                <!-- Available Orders List -->
                <div id="divOrderList" runat="server" class="padding">

                    <!-- Tooltip Descriptions -->
                    <div style="position: absolute; width: 1085px">
                        <div id="divOrderDetailToolTip" style="visibility: hidden">
                            <div class="BeigeBoxContainer shadow" style="">
                                <div style="background-color: #fff">
                                    <div class="BeigeBoxHeader" style="padding: 12px 10px 5px 15px"><span class='colorBlue'>Order Detail Information</span></div>
                                    <div class="BeigeBoxContent" style="min-height: 50px;">
                                        <div id="divOrderDetailContent" class="content" style="padding: 5px 20px"></div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div id="divOrdersListToolTip" style="visibility: hidden">
                            <div class="BeigeBoxContainer shadow" style="">
                                <div style="background-color: #fff">
                                    <div class="BeigeBoxHeader" style="padding: 12px 10px 5px 15px"><span class='colorBlue'>Available Orders Information</span></div>
                                    <div class="BeigeBoxContent" style="min-height: 50px;">
                                        <div id="divOrdersListContent" class="content" style="padding: 5px 20px"></div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div id="divCurrentOrderStatusToolTip" style="visibility: hidden">
                            <div class="BeigeBoxContainer shadow" style="">
                                <div style="background-color: #fff">
                                    <div class="BeigeBoxHeader" style="padding: 12px 10px 5px 15px">
                                        <span class='colorBlue'>Current Order Status Information</span>
                                    </div>
                                    <div class="BeigeBoxContent" style="min-height: 75px; width: 500px">
                                        <div id="divCurrentOrderStatusContent" class="content" style="padding-left: 20px"></div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div id="divTintToolTip" style="visibility: hidden">
                            <div class="BeigeBoxContainer shadow" style="">
                                <div style="background-color: #fff">
                                    <div class="BeigeBoxHeader" style="padding: 12px 10px 5px 15px"><span class='colorBlue'>Tint Information</span></div>
                                    <div class="BeigeBoxContent" style="min-height: 50px;">
                                        <div id="divTintContent" class="content" style="padding: 5px 20px"></div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div id="divLabToolTip" style="visibility: hidden">
                            <div class="BeigeBoxContainer shadow" style="">
                                <div style="background-color: #fff">
                                    <div class="BeigeBoxHeader" style="padding: 12px 10px 5px 15px"><span class='colorBlue'>Lab Information</span></div>
                                    <div class="BeigeBoxContent" style="min-height: 50px;">
                                        <div id="divLabContent" class="content" style="padding: 5px 20px"></div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="pointer_header2 colorBlue">
                        <span onmouseover="GetOrdersListToolTip(); return false" onmouseout="Hide('divOrdersListToolTip'); return false">Available Orders&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</span>
                    </div>
                    <asp:GridView ID="gvOrders" runat="server" AutoGenerateColumns="false" ClientIDMode="Static" DataKeyNames="OrderNumber"
                        CssClass="mGrid" AlternatingRowStyle-CssClass="alt"
                        OnRowCommand="gvOrders_RowCommand"
                        OnRowCreated="gvOrders_RowCreated"
                        OnRowDataBound="gvOrders_RowDataBound"
                        OnSelectedIndexChanged="gvOrders_SelectedIndexChanged">
                        <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" HorizontalAlign="Left" CssClass="headerClass" />
                        <Columns>
                            <asp:BoundField DataField="OrderDate" HeaderText="Order Date" DataFormatString="{0:d}" />
                            <asp:BoundField DataField="Frame" HeaderText="Frame" />
                            <asp:BoundField DataField="Color" HeaderText="Color" />
                            <asp:BoundField DataField="Lens" HeaderText="Lens" />
                            <asp:BoundField DataField="Tint" HeaderText="Tint" />
                            <asp:BoundField DataField="LabSiteCode" HeaderText="Lab" />
                            <asp:BoundField DataField="CurrentStatus" HeaderText="Current Order Status" />
                            <%--<asp:ButtonField ButtonType="Link" DataTextField="CurrentStatus" HeaderText="Current Order Status" CommandName="GetStatus"  />--%>
                            <asp:BoundField DataField="CurrentStatusDate" HeaderText="Order Status Date" />
                        </Columns>
                    </asp:GridView>
                </div>
                <div class="padding">
                    <asp:Button ID="bExit" runat="server" CssClass="srtsButton" Text="Exit" OnCommand="Exit_Command"></asp:Button>
                </div>
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="bSubmit" EventName="click" />
            <asp:AsyncPostBackTrigger ControlID="gvOrders" EventName="RowCommand" />
        </Triggers>
    </asp:UpdatePanel>
    <asp:ScriptManagerProxy ID="smpScripts" runat="server">
        <Scripts>
            <asp:ScriptReference Path="~/Scripts/Public/CheckOrderStatus.js" />
        </Scripts>
    </asp:ScriptManagerProxy>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="MainContent" runat="server">
</asp:Content>
