<%@ Page Title="" Language="C#" MasterPageFile="~/srtsMaster.Master" AutoEventWireup="True"
    EnableViewState="true" CodeBehind="ManageOrders.aspx.cs" Inherits="SrtsWebClinic.Orders.ManageOrders"
    MaintainScrollPositionOnPostback="true" %>

<%@ MasterType VirtualPath="~/srtsMaster.Master" %>
<asp:Content ID="contentHead" ContentPlaceHolderID="HeadContent" runat="server">
    <link rel="stylesheet" type="text/css" href="../../../Styles/w3.css" />
    <link rel="stylesheet" type="text/css" href="../../../Styles/jquery-ui.css" />
    <link rel="stylesheet" type="text/css" href="../../../Styles/ui.jqgrid.css" />
    <style>
        #divSubMenu ul {
            width: 449px;
            list-style-type: none;
            margin: 0px auto;
            padding: 0;
            overflow: hidden;
            background-color: #fff;
        }

        #divSubMenu li {
            float: left;
        }

            #divSubMenu li a {
                display: block;
                color: #004994;
                text-align: center;
                padding: 5px 16px;
                text-decoration: none;
                border: 1px solid #782e1e;
            }

                #divSubMenu li a:hover {
                }

        #divSubMenu ul li .active {
            color: #fff;
            background-color: #004994;
        }

        #divManageCheckIn h1 {
            width: 320px;
            margin: 0px auto;
            padding: 0;
            overflow: hidden;
        }

        #divGridtotals {
            height: 30px;
            width: 95%;
            margin: 0px 10px 10px 25px;
            text-align: left;
        }

        #divGridWrapper {
            float: left;
            clear: both;
            width: 95%;
            margin: 0px 10px 0px 25px;
        }

        #divGridHolder {
            margin: 0 auto;
            text-align: center;
        }

        .ui-jqgrid-bdiv {
            overflow-x: hidden !important;
        }

        .ui-grid-ico-sort {
            margin: 2px 0px 0px 0px !important;
        }
         .ui-jqgrid .loading {
            position: absolute; 
            top: 45%;
            left: 45%;
            z-index:501;
            border:none;
            background: url("/../../Styles/images/img_loading.gif");
            background-position-x: 50%;
            background-position-y: 50%;
            background-repeat: no-repeat;
            height: 63px;
            width: 63px;
          
        }
        .hide {
            display: none;
        }
    </style>
</asp:Content>

<asp:Content ID="contentClinicName" ContentPlaceHolderID="ContentTop_Title_Right" runat="server">
    <div style="position: relative; top: -60px; left:50px; right:0px;float: right">
        <span id="spanWrapper">
            <asp:Literal ID="litContentTop_Title_Right" runat="server"></asp:Literal>
        </span>
    </div>
</asp:Content>

<asp:Content ID="contentManageOrders" ContentPlaceHolderID="MainContent" runat="server">
    <div id="divSingleColumns" style="margin: 0px 10px; text-align: center">
        <div class="BeigeBoxContent">
            <asp:UpdatePanel ID="udpManageOrders" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <div id="divManageCheckIn" style="height: 20px; float: left; position: relative; top: -25px; margin-top: -40px;">
                        <h1 style="height: 20px;">
                            <span id="spnPageSubModule" class="colorBurgandy"></span>
                            <span id="spnSubModuleOrderCount" class="colorBlue"></span>
                        </h1>
                    </div>

                    <!-- Pending, CheckIn, Dispense, Problem, Overdue Buttons -->
                    <div id="divSubMenu" style="height: 150px; float: left; clear: left; position: relative; top: -43px; left: 330px; margin-bottom: -140px;">
                        <ul>
                            <li>
                                <asp:LinkButton ID="lbPending" runat="server" Text="Pending" CssClass="PageSubMenu"></asp:LinkButton></li>
                            <li>
                                <asp:LinkButton ID="lbCheckin" runat="server" Text="Check In" CssClass="PageSubMenu"></asp:LinkButton></li>
                            <li>
                                <asp:LinkButton ID="lbDispense" runat="server" Text="Dispense" CssClass="PageSubMenu"></asp:LinkButton></li>
                            <li>
                                <asp:LinkButton ID="lbProblem" runat="server" Text="Problem" CssClass="PageSubMenu"></asp:LinkButton></li>
                            <li>
                                <asp:LinkButton ID="lbOverdue" runat="server" Text="Overdue" CssClass="PageSubMenu"></asp:LinkButton></li>
                        </ul>
                    </div>

                    <asp:ValidationSummary ID="vsManageOrders" runat="server" ForeColor="Red" />
                    <div id="divInstruction" style="margin-left: 25px; width: 100%; color: #004994; font-size: 1em; text-align: left; float: left; padding-bottom: 10px;">
                        <%--This text is set in the page load of order.js--%>
                    </div>
                    <br />

                    <!-- Submit, Clear, Cancel Buttons - Top -->
                    <div style="float: right; margin-top: 1px;">
                        <asp:Button ID="btnSubmitTop" runat="server" CssClass="srtsButton submit" ToolTip="Confirm and Submit Your Order" Text="Submit" OnClientClick="DumpOrderArrays();" OnClick="btnSubmit_Click" />
                        <asp:Button ID="btnClearTop" runat="server" CssClass="srtsButton clear" ToolTip="Clear Selections" Text="Clear" OnClientClick="ClearSelectedOrders(); return false;" />
                        <asp:Button ID="btnCancelTop" runat="server" CssClass="srtsButton" ToolTip="Cancel out of page" Text="Cancel" OnClick="btnCancel_Click" />
                    </div>

                    <div class="countHeader" style="float: left; margin-left: 10px">
                        <!-- Single Order, Bulk Input -->
                        <div style="float: left; margin: 10px 0px 0px 15px; min-width: 260px; max-width: 260px; text-align: left">
                            <asp:Label ID="lblSingleOrder" Text='Single Order: ' runat="server" CssClass="srtsLabel_medium_text"></asp:Label>
                            <asp:TextBox ID="tbSingleReadScan" runat="server" CssClass="srtsTextBox_medium" Width="150px"></asp:TextBox>
                        </div>
                        <div style="float: left; margin-top: 1px;">
                            <asp:Button ID="btnBulkInput" runat="server" CssClass="srtsButton bulk" Text="Bulk Input" OnClientClick="DoBulkInput()" />
                        </div>

                        <!-- Label Format Selection -->
                        <div style="float: left; margin: 12px 0px 0px 15px;" class="print">
                            <asp:Label ID="lblPrintFormat" Text='Label Format:  ' runat="server" CssClass="srtsLabel_medium_text"></asp:Label>
                            <asp:DropDownList ID="ddlLabelFormat" runat="server" ClientIDMode="Static">
                                <asp:ListItem Text="Print To Label Avery 5160" Value="Avery5160.rpt"></asp:ListItem>
                                <asp:ListItem Text="Print To Single Label" Value="SingleLabel.rpt"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>

                    <div id="divOrdersNotInLabDispense" style="clear: both; margin-left: 25px;">
                        <asp:Label ID="lblOrdersNotInLabDispense" runat="server" ClientIDMode="Static" CssClass="colorBlue" />
                    </div>

                    <!-- Data Grid Totals -->
                    <div id="divGridtotals" class="w3-row">

                        <div id="divTotalOrders" class="w3-col" style="width: 225px">
                            <asp:Label ID="lblTotalOrders" runat="server" ClientIDMode="Static" CssClass="colorBlue" Text="Total Available Orders:"></asp:Label>
                            <span id="spnTotalOrders" class="w3-badge w3-grey" style="padding-top: 10px; width: 25px; height: 25px; text-align: center"></span>
                        </div>
                        <div id="divTotalSelected" class="w3-rest">
                            <asp:Label ID="lblTotalSelectedOrders" runat="server" ClientIDMode="Static" CssClass="colorBlue" Text="Total Selected Orders:"></asp:Label>
                            <span id="spnTotalSelectedOrders" class="w3-badge w3-green" style="padding-top: 10px; width: 25px; height: 25px; text-align: center"></span>
                        </div>
                    </div>

                    <!-- Data Grid -->
                    <div id="divGridWrapper">
                        <div id="divGridHolder">
                            <table id="GridData"></table>
                            <div id="GridPager"></div>
                        </div>
                    </div>

                    <!-- Submit, Clear, Cancel Buttons - Bottom -->
                    <div style="float: right; text-align: right; margin: 5px 0px -5px 0px;">
                        <asp:Button ID="btnSubmitBottom" runat="server" CssClass="srtsButton submit" ToolTip="Confirm and Submit Your Order" Text="Submit" OnClientClick="DumpOrderArrays();" OnClick="btnSubmit_Click" />
                        <asp:Button ID="btnClearBottom" runat="server" CssClass="srtsButton clear" ToolTip="Clear Selections" Text="Clear" OnClientClick="ClearSelectedOrders(); return false;" />
                        <asp:Button ID="btnCancelBottom" runat="server" CssClass="srtsButton" ToolTip="Cancel out of page" Text="Cancel" OnClick="btnCancel_Click" />
                    </div>

                    <iframe id="hiddenDownload" runat="server" width="1" height="1"></iframe>
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="lbCheckin" EventName="Click" />
                    <asp:AsyncPostBackTrigger ControlID="lbDispense" EventName="Click" />
                    <asp:AsyncPostBackTrigger ControlID="lbProblem" EventName="Click" />
                    <asp:AsyncPostBackTrigger ControlID="lbOverdue" EventName="Click" />
                </Triggers>
            </asp:UpdatePanel>
        </div>
    </div>

    <asp:HiddenField ID="hfSkipOverdueProblem" ClientIDMode="Static" runat="server" />
    <asp:HiddenField ID="hfOrdersInGrid" runat="server" />
    <asp:HiddenField ID="hfOrdersNotInGrid" runat="server" />
    <asp:HiddenField ID="hfPageModule" runat="server" />
    <asp:HiddenField ID="hfOrderNumber" runat="server" />
    <asp:HiddenField ID="hfSiteCode" runat="server" />

    <div id="bulkInput" class="centered" style="display: none;">
        <p>Scan all orders you want to take action on, then click &quot;Done&quot;</p>
        <div id="bulkInputContainer" class="fullwidth centered" style="max-width: 400px; margin-top: 20px;">
            <div id="bulkInputLeft" class="halfwidthleft">
                <asp:TextBox ID="tbOrderNumbers" runat="server" TextMode="MultiLine" Width="150px" Height="250px"></asp:TextBox>
            </div>
            <div id="bulkInputRight" class="halfwidthleft centered">
                <div id="bulkInputRightTop" class="halfheight fullwidth left">
                    <asp:Label ID="lblOrderScanCount" runat="server"></asp:Label>
                </div>
                <div id="bulkInputRightBottom" class="halfheight fullwidth left">
                    <asp:Button ID="btnBulkDone" runat="server" CssClass="srtsButton" Text="Done" OnClientClick="BtnBulkDone(); return false;" />
                </div>
            </div>
        </div>
    </div>

    <asp:ScriptManagerProxy ID="smpProxy" runat="server">
        <Scripts>
            <asp:ScriptReference Path="~/Scripts/jqGrid/grid.locale-en.min.js" />
            <asp:ScriptReference Path="~/Scripts/jqGrid/jquery.jqgrid.min.js" />
            <asp:ScriptReference Path="~/Scripts/Orders/ManageOrders.js" />
            <asp:ScriptReference Path="~/Scripts/Global/SrtsCustomValidators.js" />
        </Scripts>
    </asp:ScriptManagerProxy>
    <script type="text/javascript" >
            $.jgrid.defaults.loadtext = '';
    </script>
</asp:Content>