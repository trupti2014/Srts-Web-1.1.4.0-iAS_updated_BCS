<%@ Page Title="" Language="C#" MasterPageFile="~/srtsMaster.Master" AutoEventWireup="True" EnableViewState="true" CodeBehind="ManageOrdersLab.aspx.cs" Inherits="SrtsWebLab.ManageOrdersLab" %>

<%@ MasterType VirtualPath="~/srtsMaster.Master" %>

<asp:Content ContentPlaceHolderID="HeadContent" ID="headContent" runat="server">
    <link rel="stylesheet" type="text/css" href="../../../Styles/w3.css" />
    <link rel="stylesheet" type="text/css" href="../../../Styles/ui.jqgrid.css" />
    <%--<link rel="stylesheet" type="text/css" href="../../../Styles/jquery-ui.css" />--%>
    <link rel="stylesheet" type="text/css" href="../../Styles/PassFailConfirm.css" />
    <style type="text/css">
        .OrderHist {
            color: #585848;
        }

            .OrderHist th {
                color: #052078;
                text-decoration: underline;
            }

            .OrderHist td {
                padding: 5px;
            }

        .OrderHistAlt {
            color: #3d5f80;
        }

            .OrderHistAlt td {
                padding: 5px;
            }

        #divSubMenu ul {
            width: 185px;
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

        #divSubMenu ul li .active {
            color: #fff;
            background-color: #004994;
        }

        #divGridWrapper {
            width: 95%;
            margin: 0 auto;
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
            z-index: 501;
            border: none;
            background: url("/../../Styles/images/img_loading.gif");
            background-position-x: 50%;
            background-position-y: 50%;
            background-repeat: no-repeat;
            height: 63px;
            width: 63px;
        }

        
        .ui-jqgrid-labels .ui-th-column {
            border-right-color: transparent;
            color:#004994;
        }
       
        .ui-jqgrid-view>.ui-jqgrid-titlebar {
		padding-top: 0px;
		}

        .ui-jqgrid .ui-jqgrid-title {
		float: left;
		margin: 5px;
		}

        .ui-jqgrid .ui-widget-header {
        border: solid 1px #C6E7FF;
        padding-top: -15px;
        color:green;
        font-size: 15px;
        font-weight:normal;
        height: 30px;
		line-height: 18px;
		padding-bottom: 0px!important;
        background-image: url('../../Styles/images/mgrid_hder_bk.png');
        background-repeat: repeat-x;
        background-position: 0px 0px;
        }

        .ui-jqgrid .ui-jqgrid-view, .ui-jqgrid .ui-paging-info, .ui-jqgrid .ui-pg-selbox, .ui-jqgrid .ui-pg-table {
		font-size: 13px;
		}


        .ui-jqgrid .ui-jqgrid-pager, .ui-jqgrid .ui-jqgrid-toppager {
		line-height: 15px;
		padding-top: -18px!important;
		padding-bottom: 0px!important;
        background-image: url('../../Styles/images/mgrid_hder_bk.png');
        background-repeat: repeat-x;
        background-position: 0px 0px;
        display: inline-block;
        height: 20px;
        color: #004994;
        border: none;
		}
        .ui-jqgrid .ui-jqgrid-toppager {
		border-top: 1px solid #E1E1E1!important;
        height: 20px;
		}

		.ui-jqgrid .ui-pg-input {
		font-size: inherit;
		width: 24px;
		height: 20px;
		line-height: 16px;
		-webkit-box-sizing: content-box;
		-moz-box-sizing: content-box;
		box-sizing: content-box;
		text-align: center;
		padding-top: 1px;
		padding-bottom: 1px;
        background-color:transparent;
        color:#004994;
		}
		.ui-jqgrid .ui-pg-selbox {
		display: block;
		height: 16px;
		width: 60px;
		margin: 0;
		padding: 1px;
		line-height: normal;
        background-color:transparent;
		}

		.ui-jqgrid .ui-pager-control {
		position: relative;
		padding-left: 9px;
		padding-right: 9px;
        padding-top:-5px;
		}

        /*     */
        .ui-jqgrid .ui-jqgrid-labels {
		border-bottom: none;
		padding: 0!important;
		border-left: 1px solid #E1E1E1!important;
        color:#004994;
		}
		.ui-jqgrid .ui-jqgrid-labels th {
		border-top: 1px solid #E1E1E1!important;
		text-align: left!important;
		}
		.ui-jqgrid-labels th[id*="_cb"]:first-child>div {
		padding-top: 0;
		text-align: center!important;
		}
/*    */

         #GridData {
             border: solid 1px #C6E7FF;
             border-top:0px;
             border-bottom:0px;
        }

         #GridData tr {
             border-right: solid 1px #C6E7FF;
             border-left: solid 1px #C6E7FF;
        }

        #GridData td {
             border-right: solid 1px #C6E7FF;
             border-bottom: solid 1px #C6E7FF;
        }



        .srtsTextBox_medium {
            position: relative !important;
        }

        .hide {
            display: none;
        }

        .OrdersOnHold {  
            position:relative;
            right:0px;
            left:-25px;
            top:-45px;   
            width:100%;
            text-align:right;
            margin:20px 0px;    
        }
        .OrdersOnHold a {
            color:#fff;
            text-decoration:none;
        }

        .grdHeader {
            background-color:#E2F2FE;	        
            text-align:left;
            border:1px solid #782e1e;
            border-top-left-radius:4px;
            border-top-right-radius:4px;
        }


            .grdHeader tr td {
            color:#0f2a44;
            font-size:18px;        
            }

        .grdFooter {
            background-color:#E2F2FE;	        
            text-align:left;
            border:1px solid #782e1e;
            border-bottom-left-radius:4px;
            border-bottom-right-radius:4px;
        }
        .radio {
        padding-right:20px;
        }

        .hightlight {
        background-color:gray;
        
        }
        .hold {

        }
        .checkbox {
            cursor:pointer;
        }
             .EditDialogMessage {
            position: absolute;
            top: 20px;
            left: 250px;
            height: auto;
            min-height: 120px;
            min-width: 400px;
            padding: 0px;
            background: transparent;
            border-radius: 4px;
        }

        .shadow {
            -webkit-box-shadow: 10px 10px 5px 0px rgba(0,0,0,0.12);
            -moz-box-shadow: 10px 10px 5px 0px rgba(0,0,0,0.12);
            box-shadow: 10px 10px 5px 0px rgba(0,0,0,0.12);
        }

        .EditDialogMessage .header_info {
            font-size: 15px;
            color: #004994;
            padding: 5px 10px;
            background-color: transparent;
        }

        .EditDialogMessage .content {
            background-color: #fff;
            padding: 10px 10px;
            text-align: left;
        }

        .EditDialogMessage .title {
            width: 95%;
            padding: 10px 10px;
            text-align: center;
            font-size: 17px!important;
            color: #006600;
        }

        .EditDialogMessage .message {
            margin: 5px;
            padding: 5px 10px;
            text-align: center;
            font-size: 13px!important;
            color: #000;
        }

        .EditDialogMessage .w3-closebtn {
            margin-top: -3px;
        }

        .lblMessage {
        color: #004994;
        }
        .hfsDate {
        display: none;
        }
        .lblTotalOrdersOnHold {
        margin-top:-50px;        
        }
        .lbHoldStock {
        }
        .alignleft {
        text-align:left;
        }

    </style>
</asp:Content>
<asp:Content ID="contentClinicName" ContentPlaceHolderID="ContentTop_Title_Right" runat="server">
    <div style="position: relative; top: -60px; float: right;">
        <span id="spanWrapper">
            <asp:Literal ID="litContentTop_Title_Right" runat="server"></asp:Literal>
        </span>
    </div>
</asp:Content>
<asp:Content ID="contentManageOrders" ContentPlaceHolderID="MainContent" runat="server">
    <div id="divSingleColumns" style="margin: 0px 10px;">
        <div class="BeigeBoxContent">
            <asp:UpdatePanel ID="udpManageOrders" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <div id="divSubMenu" style="height: 150px; float: left; clear: left; position: relative; top: -40px; left: 425px; margin-bottom: -140px;">
                        <ul style="width: auto;">
                            <li>
                                <asp:LinkButton ID="lbCheckin" runat="server" Text="Check In" CssClass="PageSubMenu"></asp:LinkButton></li>
                            <li>
                                <asp:LinkButton ID="lbRedirectReject" runat="server" Text="Redirect / Reject" CssClass="PageSubMenu"></asp:LinkButton></li>
                            <li>
                                <asp:LinkButton ID="lbDispense" runat="server" Text="Dispense" CssClass="PageSubMenu"></asp:LinkButton></li>
                            <li>
                                <asp:LinkButton ID="lbHoldStock" runat="server" Text="Hold For Stock" CssClass="lbHoldStock PageSubMenu"></asp:LinkButton></li>
                        </ul>
                    </div>
                    <div style="min-height: 30px; max-height: 70px; clear: left; top: 0px;" >
                        <div id="divSubmitError">
                        </div>
                    </div>
                    <div id="divManageCheckIn" style="float: left; position: relative; top: 0px; left: 20px;">
                        <h1 style="">
                            <span id="spnPageSubModule" class="colorBurgandy"></span>
                            <span id="spnSubModuleOrderCount" class="colorBlue"></span>
                        </h1>
                    </div>
                    <div style="float: left; width: 100%; color: #004994; font-size: 1em; margin-top: -35px;margin-bottom:20px" class="dispense">
                        <asp:ValidationSummary ID="vsManageOrders" runat="server" ForeColor="Red" />
                        <p style="text-align: left; text-indent: 30px;font-size:15px">To select orders, scan a single barcode, check the box next to the order, or use Bulk Input.</p>
                    </div>

                    <!-- Buttons Top -->
                    <div id="divButtonsTop" style="float: right; margin-top: -5px;">
                        <asp:Button ID="btnSubmitTop" runat="server" CssClass="srtsButton submit" ToolTip="Confirm and Submit Your Order" Text="Submit" OnClientClick="return DoSubmit();" OnClick="btnSubmit_Click" />
                        <asp:Button ID="btnClearTop" runat="server" CssClass="srtsButton clear" ToolTip="Clear Selections" Text="Clear" OnClientClick="ClearSelectedOrders(); return false;" />
                        <asp:Button ID="btnCancelTop" runat="server" CssClass="srtsButton" ToolTip="Cancel out of page" Text="Cancel" OnClick="btnCancel_Click" />
                    </div>

                    <!-- Check-In / Dispense Header -->
                    <div id="divCheckInDispenseHeader" class="countHeader" style="float: left; margin-top: -20px;">
                        <div style="float: left; margin: 10px 0px 0px 25px; min-width: 260px; max-width: 260px; text-align: left" class="checkin">
                            <p class="colorBurgandy" style="font-size: 1em; text-indent: 0px;">Check-In which orders?  </p>
                        </div>
                        <div id="divTest" style="float: left; margin: 7px 0px 0px -80px;" class="checkin">
                            <asp:CheckBox ID="cbCheckInAll" runat="server" Text="  All Orders" CssClass="colorBlue" />
                            &nbsp&nbsp&nbsp&nbsp
                            <asp:Label ID="lblFindSite" runat="server" Text="By Clinic" AssociatedControlID="ddlOrderClinicCodes" CssClass="colorBlue"></asp:Label>
                            &nbsp&nbsp
                            <select id="ddlOrderClinicCodes" runat="server" class="srtsDropDown_medium" />
                        </div>
                        <div style="float: left; margin: 10px 0px 0px 25px; text-align:left" class="redirectreject" >
                            <asp:RadioButtonList ID="rblRejectRedirect" runat="server" RepeatDirection="Vertical" ClientIDMode="Static" >
                                <asp:ListItem Text="Redirect" Value="redirect" Selected="True"></asp:ListItem>
                                <asp:ListItem Text="Reject" Value="reject"></asp:ListItem>
                            </asp:RadioButtonList>
                        </div>

                        <div id="divRedirectLab" style="float: left; margin: 10px 0px 0px 0px; width: 270px;" class="redirectreject">
                            <asp:Label ID="lblLabRedirect" Text='Select lab to redirect orders to: ' runat="server" CssClass="srtsLabel_medium_text"></asp:Label>
                            <br />
                            <p><asp:DropDownList ID="ddlRedirectLab" Width="200px" runat="server" CssClass="srtsDropDown_medium" onchange="DoRejectRedirectVal()"></asp:DropDownList></p>
<%--                            <asp:CustomValidator ID="CustomValidator1" runat="server" ControlToValidate="ddlRedirectLab" ErrorMessage="Multivision orders cannot be sent to single vision labs." OnServerValidate="ddlRedirectLab_ServerValidate" Display="Dynamic">*</asp:CustomValidator>                  --%>
                        </div>
                        <div style="float: left; margin: 10px 0px 0px 25px; width: 260px;" class="dispense holdstock">
                            <asp:Label ID="lblSingleOrder" Text='Single order: ' runat="server" CssClass="srtsLabel_medium_text"></asp:Label>
                            <asp:TextBox ID="tbSingleReadScan" runat="server" CssClass="srtsTextBox_medium" Width="150px"></asp:TextBox>
                        </div>
                           <div style="float: left; margin-top: 20px;" class="dispense holdstock redirectreject">
                            <asp:Button ID="btnBulkInput" runat="server" CssClass="srtsButton bulk" Text="Bulk Input" OnClientClick="DoBulkInput()" />
                        </div>
                        <div style="float: left; margin: 10px 0px 0px 0px; width: 300px;" class="redirectreject">
                            <asp:Label ID="lblJustification" Text='Enter justification for redirecting orders: ' runat="server" CssClass="srtsLabel_medium_text"></asp:Label>
                            <br/>  
                            &nbsp;&nbsp;<asp:TextBox ID="tbLabJust" runat="server" TextMode="MultiLine" Width="300px" CssClass="srtsTextBox_multi" ClientIDMode="Static" onchange="DoRejectRedirectVal()"></asp:TextBox>
                        </div>
                        <div style="float: left; margin: 20px 0px 0px 15px;display:none" class="print">
                            <asp:Label ID="lblPrintFormat" Text='Label Format:  ' runat="server" CssClass="srtsLabel_medium_text"></asp:Label>&nbsp;&nbsp;
                            <asp:DropDownList ID="ddlLabelFormat" runat="server" ClientIDMode="Static" class="srtsDropDown_medium">
                                <asp:ListItem Text="Print To Label Avery 5160" Value="Avery5160.rpt"></asp:ListItem>
                                <asp:ListItem Text="Print To Single Label" Value="SingleLabel.rpt"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>

                    <!-- Data Grid Totals -->
                    <div id="divGridtotals" class="w3-row" style="clear: both; width: 80%; margin-bottom: 5px">
                        <div id="divInstructionsOrdersPending" style="position:relative;top:-40px;left:25px;text-align:left;width: 100%; color: #004994; font-size: 1.1em; margin-top:0px;margin-bottom:-25px">
                            <p style="text-align: left; text-indent: 0px;">To place an order on hold for stock for a frame or a lens material, select the checkbox next to the item type.</p>
                        </div>
                        <div id="divTotalOrders" class="w3-col" style="width: 225px; padding-left: 10px">
                            <asp:Label ID="lblTotalOrders" runat="server" ClientIDMode="Static" CssClass="colorBlue" Text="Total Available Orders:"></asp:Label>
                            <span id="spnTotalOrders" class="w3-badge w3-grey" style="padding-top: 10px; width: 25px; height: 25px; text-align: center"></span>
                        </div>
                        <div id="divTotalSelected" class="w3-rest" style="text-align: left; margin-left: 15px;">
                            <asp:Label ID="lblTotalSelectedOrders" runat="server" ClientIDMode="Static"
                                CssClass="colorBlue" Text="Total Selected Orders:"></asp:Label>
                            <span id="spnTotalSelectedOrders" class="w3-badge w3-green" style="padding-top: 10px; width: 25px; height: 25px; text-align: center"></span>
                        </div>
                        <div id="divHoldForStockTotals" class="w3-rest" style="text-align: left; margin-left: 15px;padding-top:-30px">
                            <asp:Label ID="lblTotalOrdersOnHold" runat="server" ClientIDMode="Static"
                                CssClass="colorBlue lblTotalOrdersOnHold" Text="Total Orders on Hold:"></asp:Label>
                            <span id="spnTotalOrdersOnHold" class="w3-badge w3-green" style="padding-top: 10px; width: 25px; height: 25px; text-align: center"></span>
                            &nbsp; &nbsp; &nbsp; 
                            <asp:Label ID="lblTotalOrdersPending" runat="server" ClientIDMode="Static"
                                CssClass="colorBlue lblTotalOrdersOnHold" Text="Total Orders Pending Processing:"></asp:Label>
                            <span id="spnTotalOrdersPending" class="w3-badge w3-blue" style="padding-top: 10px; width: 25px; height: 25px; text-align: center"></span>
                        </div>
                    </div>

                  <%--  ////--%>

                       <asp:UpdatePanel ID="uplHoldforStock" runat="server" ChildrenAsTriggers="true" UpdateMode="Conditional" ClientIDMode="Static">
                        <ContentTemplate>
                                      <%--///// Hold for Stock Grid--%>
                                 <div class="w3-row">
                                   <div id="divHoldOptions" runat="server" ClientIDMode="Static" class="OrdersOnHold" style="display:none">
                                    <a id="displayText" runat="server" class="srtsButton" href="javascript:toggle('View Orders on Hold','View Orders Pending Processing');"></hi>View Orders On Hold</a>
                                    </div>
                                        <div id="divOrdersOnHold" runat="server" style="display: none">
                                           <div style="width:95%;margin:-30px 0px 0px 30px">
                                        <div id="divInstructionsOrdersOnHold" style="position:relative;top:-100px;left:-5px;text-align:left;width: 100%; color: #004994; font-size: 1.1em; margin-top:5px">
                                        <p style="text-align: left; text-indent: 0px;">To release an order from hold for stock, select the checkbox next to the order number.</p>
                                        </div>
                                        <div style="height:auto;margin-top:-30px;max-height:483px;overflow-y:auto">
                                       <asp:GridView ID="gvStockonHold" runat="server" AutoGenerateColumns="false" CssClass="mGrid"  DataKeyNames="IndividualId, OrderNumber"
                                           AlternatingRowStyle-CssClass="alt" ShowHeaderWhenEmpty="true" OnRowCreated="gvStockonHold_RowCreated" 
                                           OnRowDataBound="gvStockonHold_RowDataBound" OnDataBound="gvStockonHold_DataBound" ClientIDMode="Static" OnSorting="gvStockonHold_Sorting"
                                           AllowSorting="true" AlternatingRowStyle-BackColor="WhiteSmoke"
                                           Width="100%">      
                                    <Columns>
                                         <asp:TemplateField HeaderText="Release">
                                                <ItemTemplate>
                                                    <asp:CheckBox ID="chkCheckinHold" runat="server" ToolTip="Select to release this order from hold."
                                                        CausesValidation="false" ClientIDMode="Static" CssClass="checkbox">                
                                                    </asp:CheckBox>                                               
                                                </ItemTemplate>
                                             <ItemStyle Width="50px" HorizontalAlign="Center" />
                                             </asp:TemplateField>                                          
                                        <asp:TemplateField HeaderText="Order Number" SortExpression="OrderNumber">
                                                <ItemTemplate>
                                                    <asp:Label ID="OrderNumber" runat="server" Text='<%# Eval("OrderNumber") %>'></asp:Label><br />
                                                    </ItemTemplate>
                                            </asp:TemplateField>                                
                                            <asp:TemplateField HeaderText="Frame" SortExpression="FrameCode">
                                                <ItemTemplate>
                                                    <asp:Label ID="Frame" runat="server" Text='<%# Eval("FrameCode") %>'></asp:Label><br />
                                                    </ItemTemplate>
                                                 <ItemStyle HorizontalAlign="Left" />
                                             </asp:TemplateField>
                                            <asp:TemplateField meta:resourcekey="LensMaterialHeader" HeaderText="Lens Material" SortExpression="LensMaterial">
                                                <ItemTemplate>
                                                      <asp:Label ID="Lens" runat="server" Text='<%# Eval("LensMaterial") %>'></asp:Label><br />
                                                </ItemTemplate>
                                                <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:TemplateField>                     
                                            <asp:TemplateField HeaderText="Status Comment" SortExpression="StatusComment">
                                                <ItemTemplate>
                                                    <asp:Label ID="Status" runat="server" Text='<%# Eval("StatusComment") %>'></asp:Label><br />                                              
                                                </ItemTemplate>
                                                <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Hold End Date" SortExpression="DateHoldStockEnd">
                                                <ItemTemplate>
                                                   <asp:Label ID="EndDate" runat="server" Text='<%# Eval("DateHoldStockEnd", "{0:MM/dd/yyyy}")%>'></asp:Label><br />                                       
                                                </ItemTemplate>
                                                <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Order Date" SortExpression="DateLastModified">
                                                <ItemTemplate>
                                                    <asp:Label ID="OrderDate" runat="server" Text='<%# Eval("DateLastModified", "{0:MM/dd/yyyy}") %>'></asp:Label><br />                                              
                                                </ItemTemplate>
                                                <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Clinic" SortExpression="ClinicSiteCode">
                                                <ItemTemplate>
                                                    <asp:Label ID="Clinic" runat="server" Text='<%# Eval("ClinicSiteCode") %>'></asp:Label><br />                                              
                                                </ItemTemplate>
                                                <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:TemplateField>
                                    </Columns>
                                <FooterStyle CssClass="grdFooter" />
                                </asp:GridView>
                                            </div>
                                        </div>
                                        </div>                        
                                 </div>
                                 <%--///// End Hold for Stock Grid--%>
                                                    
                                <%--///// Orders Pending Processing--%>
                                <div class="w3-row" style="margin-bottom:20px">
                                    <div id="divOrdersNotOnHold" runat="server" style="display: block">
                                    <div style="width:95%;margin:-30px 0px 0px 30px">
                                   <%--     <div id="divInstructionsOrdersPending" style="position:relative;top:-105px;left:-5px;text-align:left;width: 100%; color: #004994; font-size: 1.1em; margin-top:5px">
                                        <p style="text-align: left; text-indent: 0px;">To place an order on hold for stock for a frame or a lens material, select the checkbox next to the item type.</p>
                                        </div>--%>
                                        <div style="margin-top:-30px;height:auto;max-height:483px;overflow-y:auto"> 
                                             <asp:GridView ID="gvPending" runat="server" AutoGenerateColumns="false" CssClass="mGrid hold"  DataKeyNames="IndividualId, OrderNumber"
                                           ShowFooter="true" ShowHeaderWhenEmpty="true" OnRowCreated="gvPending_RowCreated" HeaderStyle-CssClass="grdHeaderSub"
                                                 OnRowDataBound="gvPending_RowDataBound" OnDataBound="gvPending_DataBound" ClientIDMode="Static" AllowPaging="false" OnSorting="gvPending_Sorting"
                                           AllowSorting="false" AlternatingRowStyle-BackColor="WhiteSmoke"
                                           Width="100%">
                                    <Columns>                                
                                        <asp:TemplateField HeaderText="Order Number" SortExpression="OrderNumber">
                                                <ItemTemplate>
                                                    <asp:Label ID="OrderNumber" runat="server" Text='<%# Eval("OrderNumber") %>'></asp:Label><br />
                                                    </ItemTemplate>
                                            </asp:TemplateField>                                
                                            <asp:TemplateField HeaderText="Frame" SortExpression="FrameCode">
                                                <ItemTemplate>                                                                          
                                                    <div class="w3-row">
                                                    <div class="w3-col" style="width:30%">
                                                   <asp:CheckBox ID="chkHoldFrame" runat="server" ToolTip="Select to place this frame on hold for stock."
                                                        CausesValidation="false" ClientIDMode="Static" CssClass="checkbox">                
                                                    </asp:CheckBox> 
                                                        </div>  
                                                    <div class="w3-rest" style="">
                                                    <asp:Label ID="Frame" runat="server" Text='<%# Eval("FrameCode") %>'></asp:Label><br />
                                                    </div>
                                                    </div>
                                                </ItemTemplate>
                                                 <ItemStyle HorizontalAlign="Left" />
                                             </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Lens Material" SortExpression="LensMaterial">
                                                <ItemTemplate>
                                                     <div class="w3-row">
                                                    <div class="w3-col" style="width:30%">
                                                   <asp:CheckBox ID="chkHoldLens" runat="server" ToolTip="Select to place this lens material on hold for stock."
                                                        CausesValidation="false" ClientIDMode="Static" CssClass="checkbox">                 
                                                    </asp:CheckBox> 
                                                        </div>  
                                                    <div class="w3-rest" style="">
                                                    <asp:Label ID="Lens" runat="server" Text='<%# Eval("LensMaterial") %>'></asp:Label><br />
                                                    </div>
                                                    </div>                                         
                                                </ItemTemplate>
                                                <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:TemplateField>                     
                                            <asp:TemplateField HeaderText="Status Comment" SortExpression="StatusComment">
                                                <ItemTemplate>
                                                    <asp:Label ID="Status" runat="server" Text='<%# Eval("StatusComment") %>'></asp:Label><br />                                              
                                                </ItemTemplate>
                                                <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Order Date" SortExpression="DateLastModified">
                                                <ItemTemplate>
                                                    <asp:Label ID="OrderDate" runat="server" Text='<%# Eval("DateLastModified", "{0:MM/dd/yyyy}") %>'></asp:Label><br />                                              
                                                </ItemTemplate>
                                                <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Clinic" SortExpression="ClinicSiteCode">
                                                <ItemTemplate>
                                                    <asp:Label ID="Clinic" runat="server" Text='<%# Eval("ClinicSiteCode") %>'></asp:Label><br />                                              
                                                </ItemTemplate>
                                                <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:TemplateField>

                                    </Columns>
                                </asp:GridView>    
                                            </div> 
                                    </div>                           
                                    </div>
                                </div>
                                 <%--///// Orders Pending Processing--%>

                            </ContentTemplate>
                                      <Triggers>
            <%--            <asp:AsyncPostBackTrigger ControlID="btnYesSelect" EventName="click" />
                        <asp:AsyncPostBackTrigger ControlID="btnYesHold" EventName="click" />
                        <asp:AsyncPostBackTrigger ControlID="gvPending" EventName="RowCommand" />  --%>  
                    </Triggers>
                    </asp:UpdatePanel>
                
                <%--Message Hold for Stock Dialog --%>                            
                <div id="msgHoldforStock" class="w3-modal" style="z-index: 30000">
                <div class="w3-modal-content">
                    <div class="w3-container">
                        <div class="EditDialogMessage">
                            <div class="BeigeBoxContainer shadow" style="width:400px">
                                <div style="background-color: #fff">
                                    <div class="BeigeBoxHeader" style="text-align:left;padding: 12px 10px 3px 15px">
                                        <div id="Div2" class="header_info">
                                            <span onclick="document.getElementById('msgHoldforStock').style.display='none'"
                                                class="w3-closebtn">&times;</span>
                                            Hold for Stock
                                        </div>
                                    </div>
                                    <div class="BeigeBoxContent" style="padding: 0px 20px 20px 25px; min-height: 150px">
                                        <div class="w3-row" style="text-align:left">
                                            <div class="w3-col" style="margin-left:0px;margin-top:10px;width:320px;text-align:left;padding:0px 10px 10px 10px">
                                                
                                                <asp:Label ID="lblMessage" runat="server" ClientIDMode="Static" CssClass="lblMessage" Text=""></asp:Label>
                                               
                                                <div id="divHoldForStockInfo" runat="server" ClientIDMode="Static" style="" class="hfsDate">
                                                <div id="divHoldForStock">
                                                    <div style="margin-top: 10px;">                                                  
                                                        <div class="left">
                                                            <asp:Label ID="Label111" runat="server" CssClass="srtsLabel_medium" Text="Select Estimated Stock Date:"></asp:Label>
                                                            <asp:TextBox ID="tbHfsDate" runat="server" TabIndex="5" Width="85px" CssClass="srtsTextBox_medium" ClientIDMode="Static" ReadOnly="true" onchange="DoStatusDateVal();" />
                                                        </div>
                                                        <div class="left" style="margin-left: 8px; margin-top: 2px;">
                                                            <asp:Image runat="server" ID="imgHoldForStock" AlternateText="Calendar" ImageUrl="~/Styles/images/Calendar_scheduleHS.png" ClientIDMode="Static" />
                                                            <ajaxToolkit:CalendarExtender ID="ceHfsDate" runat="server" TargetControlID="tbHfsDate" Format="MM/dd/yyyy" PopupButtonID="imgHoldForStock" ClientIDMode="Static">
                                                            </ajaxToolkit:CalendarExtender>
                                                        </div>
                                                    </div>
                                                </div>
                                                </div>
                                             
                                                
                                                <div style="text-align:center;margin-top:30px">
                                                    <br />
                                                 
                                                    <asp:Button ID="btnYes" runat="server" CssClass="srtsButton hide" Text="Yes, Select All" ClientIDMode="Static"
                                                        OnClientClick="return CloseHoldStockDialog('btnYes_SelectAll');" OnClick="BindGridviews"/>
                                                    <asp:Button ID="btnNo" runat="server" CssClass="srtsButton hide" Text="No" ClientIDMode="Static"
                                                        OnClientClick="CloseHoldStockDialog('btnNo'); return false;"/>
                                                 
                                                    
                                            
                                                    <asp:Button ID="btnSubmitHold" runat="server" CssClass="srtsButton" Text="Submit" ClientIDMode="Static"
                                                        OnClientClick="return DoHoldForStockSubmit();" OnClick="BindGridviews" CommandName="SubmitHoldItems" CausesValidation="false"/>

                                                      <asp:Button ID="btnSubmitRelease" runat="server" CssClass="srtsButton hide" Text="Submit Release" ClientIDMode="Static"
                                                        OnClick="ReleaseHoldItems" CommandName="SubmitReleaseHoldItems" CausesValidation="false"/>
                                                    
                                                    <asp:Button ID="btnCancelHold" runat="server" CssClass="srtsButton" Text="Cancel" ClientIDMode="Static"
                                                        OnClientClick="return CloseHoldStockDialog('btnCancelHold');" OnClick="BindGridviews" CommandName="CancelHold"/>
                                                    
                                                     <asp:Button ID="btnCancelRelease" runat="server" CssClass="srtsButton hide" Text="Cancel" ClientIDMode="Static"
                                                        OnClientClick="return CloseHoldStockDialog('btnCancelHold');" OnClick="BindGridviews" CommandName="CancelRelease"/>

                                                    <asp:Button ID="btnOK" runat="server" CssClass="srtsButton hide" Text="OK" Visible="true" ClientIDMode="Static"
                                                        OnClientClick="return CloseHoldStockDialog('btnOK');"/>

                                                    
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    </div>                                             
                                </div>
                            </div>
                        </div>
                    </div>
                </div>





                  <%--  ////--%>

                    <div id="divGridWrapper">
                       <!--Order Print Priority -->
                        <div class="w3-col checkin" style="width:28%;text-align:left;padding:0px">
                            <asp:GridView ID="gvOrderPrintPriority" runat="server" AutoGenerateColumns="false" CssClass="mGrid"  DataKeyNames="PriorityId"
                                AlternatingRowStyle-CssClass="alt" ShowHeaderWhenEmpty="true" ClientIDMode="Static" AllowSorting="true" AlternatingRowStyle-BackColor="WhiteSmoke"
                                Width="100%" OnRowCreated="gvOrderPrintPriority_RowCreated">      
                        <Columns>
                          <asp:TemplateField HeaderText="">
                         <%--           <HeaderTemplate>
                                          <asp:CheckBox ID="checkboxSelectAll" onClick="checkAllPriorities()" runat="server" />
                                        </HeaderTemplate>--%>
                                    <ItemTemplate>
                                        <asp:CheckBox ID="chkPriority" onClick="checkPriorityItem(this)" runat="server" ToolTip="Select this priority to print."
                                            CausesValidation="false" ClientIDMode="Static" CssClass="checkbox">                
                                        </asp:CheckBox>
                                       <ItemStyle HorizontalAlign="Left" />
                                    </ItemTemplate>
                                    </asp:TemplateField> 
                             <asp:TemplateField HeaderText="c" HeaderStyle-HorizontalAlign="Left" Visible="false">
                                    <ItemTemplate>
                                        <asp:Label ID="PriorityCode" runat="server" Text='<%# Eval("PriorityId") %>' />
                              <ItemStyle HorizontalAlign="Left" />
                                        </ItemTemplate>
                           </asp:TemplateField>   
                          <asp:TemplateField HeaderText="Priority" HeaderStyle-HorizontalAlign="Left">
                                    <ItemTemplate>
                                        <asp:Label ID="Priority" runat="server" Text='<%# Eval("Priority") %>'></asp:Label><br />
                                        </ItemTemplate>
                              <ItemStyle HorizontalAlign="Left" />
                           </asp:TemplateField>                                
                           <asp:TemplateField HeaderText="Count">
                                    <ItemTemplate>
                                        <asp:Label ID="Count" runat="server" Text='<%# Eval("Count") %>'></asp:Label><br />
                                        </ItemTemplate>
                                        <ItemStyle HorizontalAlign="Left" />
                                    </asp:TemplateField>                    
                            <asp:TemplateField HeaderText="Oldest">
                                    <ItemTemplate>
                                        <asp:Label ID="Oldest" runat="server" Text='<%# Eval("Date") %>' Visible='<%# Eval("Date").ToString() == "01/01/0001" ? false : true %>'></asp:Label><br />                                              
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                            </asp:TemplateField>
                        </Columns>
                    <FooterStyle CssClass="grdFooter" />
                    </asp:GridView>
                        </div>
                        <!--Order Grid -->
                        <div class="w3-rest" style="text-align:right;margin-top:20px">
                            <div id="divGridHolder" style="float:right">
                                <table id="GridData"></table>
                                <div id="GridPager"></div>
                            </div>
                        </div>
                    </div>
                    
                    <!-- Buttons Bottom -->
                    <div id="divButtonsBottom" style="float: right;">
                        <asp:Button ID="btnSubmitBottom" runat="server" CssClass="srtsButton submit" ToolTip="Confirm and Submit Your Order" Text="Submit" OnClientClick="return DoSubmit();" OnClick="btnSubmit_Click" />
                        <asp:Button ID="btnClearBottom" runat="server" CssClass="srtsButton clear" ToolTip="Clear Selections" Text="Clear" OnClientClick="ClearSelectedOrders(); return false;" />
                        <asp:Button ID="btnCancelBottom" runat="server" CssClass="srtsButton" ToolTip="Cancel out of page" Text="Cancel" OnClientClick="DumpClinicDDL();" OnClick="btnCancel_Click" />
                    </div>
                    <iframe id="hiddenDownload" runat="server" width="1" height="1"></iframe>

                    <asp:HiddenField ID="hdfHoldForStockReason" runat="server" ClientIDMode="Static" Value="Add Reason Here" />
                    <asp:HiddenField ID="hdfHoldStockItemType" runat="server" ClientIDMode="Static" />
                    <asp:HiddenField ID="hdfHoldStockItem" runat="server" ClientIDMode="Static" />
                    <asp:HiddenField ID="hfStockDate" runat="server" Value="" ClientIDMode="Static" />
                    <asp:HiddenField ID="hdfCommandName" runat="server" Value="" ClientIDMode="Static" />
                    <asp:HiddenField ID="hdfStatusType" runat="server" Value="" ClientIDMode="Static" />
                    <asp:HiddenField ID="hdfCurrentModule" runat="server" Value="" ClientIDMode="Static" />
                    <asp:HiddenField ID="hdfOrdersSelected" runat="server" Value="" ClientIDMode="Static" />
                    <asp:HiddenField ID="hdftotalOrdersChecked" runat="server" Value="" ClientIDMode="Static" />
                    <asp:HiddenField ID="hdftotalFrameItemsChecked" runat="server" Value="" ClientIDMode="Static" />
                    <asp:HiddenField ID="hdftotalLensItemsChecked" runat="server" Value="" ClientIDMode="Static" />
                    <asp:HiddenField ID="hdfTotalOrdersOnHold" runat="server" Value="" ClientIDMode="Static" />
                    <asp:HiddenField ID="hdfTotalOrdersPending" runat="server" Value="" ClientIDMode="Static" />
                    <asp:HiddenField ID="hdfCheckInPriorities" runat="server" Value="" ClientIDMode="Static" />
                </ContentTemplate>
                <Triggers>
                    <asp:PostBackTrigger ControlID="btnSubmitTop" />
                    <asp:PostBackTrigger ControlID="btnSubmitBottom" />
                    <asp:PostBackTrigger ControlID="btnSubmitHold" />
                </Triggers>
            </asp:UpdatePanel>
        </div>
    </div>

    <asp:HiddenField ID="hfOrdersInGrid" runat="server" ClientIDMode="Static" />
    <asp:HiddenField ID="hfPageModule" runat="server" ClientIDMode="Static" />
    <asp:HiddenField ID="hfOrderNumber" runat="server" ClientIDMode="Static" />
    <asp:HiddenField ID="hdfSiteHasLMS" runat="server" ClientIDMode="Static" />

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

<%--    <div id="divHoldForStockCheckIn" style="display: none; overflow: hidden;">
        <div id="divHfsCiErrorMessage" style="margin-top: 10px;"></div>
        <div id="divCheckInHoldStock" style="margin-top: 10px;">
            <asp:Label ID="Label122" runat="server" CssClass="srtsLabel_medium" Text="Check-in Orders:"></asp:Label>
            <asp:CheckBox ID="cbCheckInHoldStock" runat="server" onchange="$('#divHoldForStock').toggle();" ClientIDMode="Static" />
        </div>
        <div id="divHoldForStock">
            <div style="margin-top: 10px;">
                <asp:HiddenField ID="hfStockDate" runat="server" Value="" ClientIDMode="Static" />
                <div class="left">
                    <asp:Label ID="Label111" runat="server" CssClass="srtsLabel_medium" Text="Select Estimated Stock Date:"></asp:Label>
                    <asp:TextBox ID="tbHfsDate" runat="server" TabIndex="5" Width="85px" CssClass="srtsTextBox_medium" ClientIDMode="Static" ReadOnly="true" onchange="DoStatusDateVal();" />
                </div>
                <div class="left" style="margin-left: 8px; margin-top: 2px;">
                    <asp:Image runat="server" ID="imgHoldForStock" AlternateText="Calendar" ImageUrl="~/Styles/images/Calendar_scheduleHS.png" ClientIDMode="Static" />
                    <ajaxToolkit:CalendarExtender ID="ceHfsDate" runat="server" TargetControlID="tbHfsDate" Format="MM/dd/yyyy" PopupButtonID="imgHoldForStock" ClientIDMode="Static">
                    </ajaxToolkit:CalendarExtender>
                </div>
            </div>
            <div style="clear: both;"></div>
            <div style="margin-top: 10px;">
                <asp:Label ID="Label112" runat="server" CssClass="srtsLabel_medium" Text="Select Out of Stock Reason:"></asp:Label>
                <asp:DropDownList ID="ddlStockReason" runat="server" ClientIDMode="Static" onchange="DoStockReason();">
                    <asp:ListItem Text="-Select-" Value="X" Selected="True"></asp:ListItem>
                    <asp:ListItem Text="Frame Unavailable" Value="f"></asp:ListItem>
                    <asp:ListItem Text="Lens Unavailable" Value="l"></asp:ListItem>
                    <asp:ListItem Text="Other" Value="o"></asp:ListItem>
                </asp:DropDownList>
            </div>
            <div id="divHoldForStockReason" style="margin-top: 10px;">
                <asp:TextBox ID="tbHoldForStockReason" runat="server" Width="90%" Height="100px" TextMode="MultiLine" ClientIDMode="Static" onchange="return DoHoldForStockVal();"></asp:TextBox>
            </div>
        </div>

        <div id="hfsRightBottom" style="margin-top: 10px;">
            <asp:Button ID="bHoldStockSubmit" runat="server" CssClass="srtsButton" Text="Submit" OnClientClick="return DoHoldForStockSubmit();" OnClick="bHoldStockSubmit_Click" CausesValidation="false" />
        </div>
    </div>--%>

    <asp:ScriptManagerProxy ID="smpProxy" runat="server">
        <Scripts>
            <asp:ScriptReference Path="~/Scripts/jqGrid/grid.locale-en.min.js" />
            <asp:ScriptReference Path="~/Scripts/jqGrid/jquery.jqgrid.min.js" />
            <asp:ScriptReference Path="~/Scripts/Orders/ManageOrdersLab.js" />
            <asp:ScriptReference Path="~/Scripts/Global/SrtsCustomValidators.js" />
            <asp:ScriptReference Path="~/Scripts/Global/GlobalShared.js" />
            <asp:ScriptReference Path="~/Scripts/Global/PassFailConfirm.js" />
        </Scripts>
    </asp:ScriptManagerProxy>
    <script type="text/javascript">
        $.jgrid.defaults.loadtext = '';


        function toggle(displayText, HideText) {
            //var eleHold = document.getElementById("divOrdersOnHold"); //Aldela: Commented this out
            //var eleNoHold = document.getElementById("divOrdersNotOnHold"); //Aldela: Commented this out
            //var text = document.getElementById("displayText");
            var eleHold = document.getElementById("<%=divOrdersOnHold.ClientID%>"); //Aldela: Commented this out
            var eleNoHold = document.getElementById("<%=divOrdersNotOnHold.ClientID%>"); //Aldela: Commented this out
            var text = document.getElementById("<%=displayText.ClientID%>");
            if (eleHold.style.display == "block") {
                eleHold.style.display = "none";
                eleNoHold.style.display = "block";
                if (text != null && text != "undefined") {
                    text.innerHTML = displayText;
                }
                $("#divInstructionsOrdersPending").show();
            }
            else {
                eleHold.style.display = "block";
                eleNoHold.style.display = "none";
                if (text != null && text != "undefined") {
                    text.innerHTML = HideText;
                }
                $("#divInstructionsOrdersPending").hide();
            }
        }
    </script>
</asp:Content>
