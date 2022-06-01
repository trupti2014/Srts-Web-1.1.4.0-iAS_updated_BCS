<%@ Page Language="C#" MasterPageFile="~/srtsMaster.Master" AutoEventWireup="true" CodeBehind="OrderTracking.aspx.cs" Inherits="SrtsWeb.WebForms.SrtsOrderManagement.OrderTracking" %>

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
            z-index: 501;
            border: none;
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


<asp:Content ID="contentTrackOrders" ContentPlaceHolderID="MainContent" runat="server">
    <asp:HiddenField ID="hfSuccessTrack" runat="server" Value="0" ClientIDMode="Static" />
    <asp:HiddenField ID="hfMsgTrack" runat="server" Value="" ClientIDMode="Static" />
    <div id="divSingleColumns" style="margin: 0px 10px; text-align: center">
        <div class="BeigeBoxContent">
            <asp:UpdatePanel ID="udpTrackOrders" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
                <ContentTemplate>
                    <div id="divInstruction1" style="margin-left: 0px; width: 100%; color: #004994; font-size: 1em; text-align: left; float: left; padding-bottom: 15px; padding-top: 10px">
                        <%--This text is set in the page load of the .js--%>
                    </div>
                    <div id="divInstruction2" style="margin-left: 0px; width: 100%; color: #004994; font-size: 1em; text-align: left; float: left; padding-bottom: 15px;">
                        <%--This text is set in the page load of the .js--%>
                    </div>

                    <br />

                    <%-- Provider--%>
                    <div id="divEnterProvider" class="w3-row padding" style="padding-left: 0px">
                        <div class="w3-half" style="text-align: left; width: auto">
                            <asp:Label ID="lblShippingProvider" Text='Shipping Provider: ' runat="server" CssClass="srtsLabel_medium_text"></asp:Label>
                            <asp:DropDownList ID="ddlShippingProvider" runat="server" ClientIDMode="Static">
                                <asp:ListItem Text="USPS" Value="USPS"></asp:ListItem>
                                <asp:ListItem Text="UPS" Value="UPS"></asp:ListItem>
                                <asp:ListItem Text="FEDEX" Value="FEDEX"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="w3-half" style="padding-left: 20px">
                            <asp:RadioButtonList ID="rblInputType" runat="server" TabIndex="0" RepeatDirection="Horizontal" ToolTip="Select Entry Type"
                                CausesValidation="false" ClientIDMode="Static" AutoPostBack="true" OnTextChanged="rblInputType_TextChanged" CssClass="srtsLabel_medium_text">
                                <asp:ListItem Text="Scan" Value="Scan" Selected="True" />
                                <asp:ListItem Text="Manual" Value="Manual" />
                            </asp:RadioButtonList>
                        </div>
                    </div>

                    <div id="divPanelManual">
                        <asp:Panel ID="pnlManual" runat="server" DefaultButton="btnSubmit" UpdateMode="Conditional">
                            <%-- Tracking--%>
                            <div id="divEnterTracking" class="w3-row padding" style="padding-left: 0px; padding-top: 0px">
                                <div class="w3-row" style="width: auto">
                                    <%-- Tracking--%>
                                    <div class="w3-half">
                                        <asp:Label ID="lblTrackingNumberM" runat="server" CssClass="srtsLabel_medium_text" Text="Tracking Number:"></asp:Label><br />
                                        <asp:TextBox ID="txtTrackingNumberM" runat="server" Width="600px" CssClass="srtsTextBox_medium" ReadOnly="False"></asp:TextBox>
                                        <ajaxToolkit:FilteredTextBoxExtender ID="ftbTNM" runat="server" FilterType="Custom, Numbers, UppercaseLetters" TargetControlID="txtTrackingNumberM" Enabled="True" />
                                    </div>
                                </div>
                            </div>

                            <%-- Order1, Order2, Order3, Order4--%>
                            <div id="divEnterOrders" class="w3-row padding" style="padding-left: 0px">
                                <div class="w3-row" style="width: auto">

                                    <%-- Order1--%>
                                    <div class="w3-quarter">
                                        <asp:Label ID="lblOrder1" runat="server" CssClass="srtsLabel_medium_text" Text="1st Order:"></asp:Label><br />
                                        <asp:TextBox ID="txtOrder1" runat="server" Width="250px" CssClass="srtsTextBox_medium" ReadOnly="False"></asp:TextBox>
                                        <ajaxToolkit:FilteredTextBoxExtender ID="ftbOrder1" runat="server" FilterType="Custom, Numbers" ValidChars="-" TargetControlID="txtOrder1" Enabled="True" />
                                    </div>
                                    <%--  Order2--%>
                                    <div class="w3-quarter">
                                        <asp:Label ID="lblOrder2" runat="server" CssClass="srtsLabel_medium_text" Text="2nd Order:"></asp:Label><br />
                                        <asp:TextBox ID="txtOrder2" runat="server" Width="250px" CssClass="srtsTextBox_medium" ReadOnly="False"></asp:TextBox>
                                        <ajaxToolkit:FilteredTextBoxExtender ID="ftbOrder2" runat="server" FilterType="Custom, Numbers" ValidChars="-" TargetControlID="txtOrder2" Enabled="True" />
                                    </div>
                                    <%--   Order3--%>
                                    <div class="w3-quarter">
                                        <asp:Label ID="lblOrder3" runat="server" CssClass="srtsLabel_medium_text" Text="3rd Order:"></asp:Label><br />
                                        <asp:TextBox ID="txtOrder3" runat="server" Width="250px" CssClass="srtsTextBox_medium" ReadOnly="False"></asp:TextBox>
                                        <ajaxToolkit:FilteredTextBoxExtender ID="ftbOrder3" runat="server" FilterType="Custom, Numbers" ValidChars="-" TargetControlID="txtOrder3" Enabled="True" />
                                    </div>
                                    <%--   Order4--%>
                                    <div class="w3-quarter">
                                        <asp:Label ID="lblOrder4" runat="server" CssClass="srtsLabel_medium_text" Text="4th Order:"></asp:Label><br />
                                        <asp:TextBox ID="txtOrder4" runat="server" Width="250px" CssClass="srtsTextBox_medium" ReadOnly="False"></asp:TextBox>
                                        <ajaxToolkit:FilteredTextBoxExtender ID="ftbOrder4" runat="server" FilterType="Custom, Numbers" ValidChars="-" TargetControlID="txtOrder4" Enabled="True" />
                                    </div>
                                </div>
                            </div>
                            <%-- Order1, Order2, Order3, Order4--%>
                            <div id="divSubmit" class="w3-row padding" style="padding-left: 0px">
                                <div class="w3-row" style="width: auto">
                                    <div class="w3-half">
                                        <asp:Button ID="btnSubmit" runat="server" CssClass="srtsButton" Text="Save" OnClick="btnSubmit_Click" CausesValidation="False" />
                                    </div>
                                </div>
                            </div>
                        </asp:Panel>
                    </div>

                    <div id="divPanelScan">
                        <asp:Panel ID="pnlScan" runat="server" DefaultButton="btnScan" UpdateMode="Conditional">

                            <%-- Tracking--%>
                            <div id="divScanTracking" class="w3-row padding" style="padding-left: 0px; padding-top: 0px">
                                <div class="w3-row" style="width: auto">
                                    <%-- Tracking--%>
                                    <div class="w3-half">
                                        <asp:Label ID="lblTrackingNumberS" runat="server" CssClass="srtsLabel_medium_text" Text="Scan Barcode:"></asp:Label><br />
                                        <asp:TextBox ID="txtTrackingNumberS" onkeypress="return GetKeyCode(event)" ClientIDMode="Static" runat="server" Width="600px" CssClass="srtsTextBox_medium" ReadOnly="False"></asp:TextBox>
                                        <ajaxToolkit:FilteredTextBoxExtender ID="ftb_tbTN" runat="server" FilterType="Custom, Numbers, UppercaseLetters" ValidChars="-" TargetControlID="txtTrackingNUmberS" Enabled="True" />
                                        <asp:Button ID="btnScan" runat="server" BackColor="Transparent" BorderColor="Transparent" ClientIDMode="Static" OnClick="btnScan_Click" />
                                    </div>
                                </div>
                            </div>
                        </asp:Panel>
                    </div>

                    <hr />
                    <br />
                    <%-- Patient Name, Tracking Number,  Order Count--%>
                    <asp:Panel ID="TrackingTablePanel" UpdateMode="Conditional" ChildrenAsTriggers="false" runat="server">
                        <asp:GridView ID="gvTracking" runat="server" AllowPaging="true" AutoGenerateColumns="false"
                            CssClass="mGrid" AllowSorting="true" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                            HeaderStyle-HorizontalAlign="Left" RowStyle-HorizontalAlign="Center"
                            DataKeyNames="TrackingNumber" PageSize="20" OnRowCommand="gvTracking_RowCommand" EnableViewState="true">
                            <PagerSettings Mode="NextPreviousFirstLast" Position="TopAndBottom" FirstPageText="<< First"
                                NextPageText="Next >" PreviousPageText="< Previous" LastPageText="Last >>" />
                            <Columns>
                                <asp:TemplateField HeaderText="Tracking Number">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="lnbTrackingNumber" CommandArgument='<%#Eval("TrackingNumber") %>'
                                            Text='<%#Eval("TrackingNumber") %>' runat="server" ToolTip="Edit this Patient Tracking."
                                            CommandName="viewrecords">
                                        </asp:LinkButton>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="Patient" HeaderText="Patient Name" />
                                <asp:BoundField DataField="OrderCount" HeaderText="Order Count" />
                            </Columns>
                        </asp:GridView>
                    </asp:Panel>

                    <asp:Panel ID="TrackingEditPanel" runat="server" Visible="false">
                        <asp:GridView ID="gvEditTracking" runat="server" AllowPaging="true" AutoGenerateColumns="false"
                            CssClass="mGrid" AllowSorting="true" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                            HeaderStyle-HorizontalAlign="Left" RowStyle-HorizontalAlign="Center"
                            DataKeyNames="TrackingNumber" PageSize="20" OnRowCommand="gvTracking_RowCommand" EnableViewState="true">
                            <PagerSettings Mode="NextPreviousFirstLast" Position="TopAndBottom" FirstPageText="<< First"
                                NextPageText="Next >" PreviousPageText="< Previous" LastPageText="Last >>" />
                            <Columns>
                                <asp:TemplateField HeaderText="Select">
                                    <ItemTemplate>
                                        <asp:ImageButton ID="btnEdit" runat="server" ClientIDMode="Static"
                                            ImageUrl="~/Styles/images/img_edit_pencil.png" ToolTip="add Order Number for this Tracking Label" Width="20px" Height="20px" CommandName="editrecord"
                                            CommandArgument='<%#Eval("TrackingNumber")%>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="TrackingNumber" HeaderText="Tracking Number" />
                                <asp:BoundField DataField="OrderNumber" HeaderText="Order Number" />
                                <asp:BoundField DataField="OrderDate" HeaderText="Order Date" />
                                <asp:BoundField DataField="FrameCode" HeaderText="Frame Code" />
                                <asp:BoundField DataField="LensType" HeaderText="Lens Type" />
                                <asp:TemplateField HeaderText="Delete">
                                    <ItemTemplate>
                                        <asp:ImageButton ID="btnDelete" runat="server" ClientIDMode="Static"
                                            ImageUrl="~/Styles/images/img_delete_x.png" ToolTip="Delete this tracking number entry." Width="20px" Height="20px" CommandName="deleterecord"
                                            CommandArgument='<%#Eval("OrderNumber")%>' CausesValidation="false" />
                                    </ItemTemplate>
                                    <ItemStyle Width="25px" />
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </asp:Panel>
                    <asp:HiddenField ID="hfOrderNumber" runat="server" />
                    <asp:HiddenField ID="hfSiteCode" runat="server" />

                    <asp:ScriptManagerProxy ID="smpTracking" runat="server">
                        <Scripts>
                            <asp:ScriptReference Path="~/Scripts/OrderManagement/OrderTracking.js" />
                            <asp:ScriptReference Path="~/Scripts/Global/PassFailConfirm.js" />
                            <asp:ScriptReference Path="~/Scripts/Global/SrtsCustomValidators.js" />
                        </Scripts>
                    </asp:ScriptManagerProxy>

                </ContentTemplate>


                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="btnScan" EventName="Click" />
                    <asp:AsyncPostBackTrigger ControlID="btnSubmit" EventName="Click" />
                    <asp:AsyncPostBackTrigger ControlID="rblInputType" EventName="TextChanged" />
                </Triggers>

            </asp:UpdatePanel>
        </div>
    </div>

</asp:Content>
