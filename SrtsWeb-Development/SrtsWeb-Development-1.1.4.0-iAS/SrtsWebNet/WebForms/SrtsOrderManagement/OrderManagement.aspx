<%@ Page Language="C#" AutoEventWireup="True" CodeBehind="OrderManagement.aspx.cs" Inherits="SrtsWeb.SrtsOrderManagement.OrderManagement"
    MasterPageFile="~/srtsMaster.Master" Async="true" EnableEventValidation="false" %>
<%@ MasterType VirtualPath="~/srtsMaster.Master" %>
<asp:Content ID="omHeader" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="../../Styles/PassFailConfirm.css" rel="stylesheet" />
    <link rel="stylesheet" type="text/css" href="../../Styles/w3.css" />
    <style type="text/css">
        .reorderNoClose .ui-dialog-titlebar-close {
            display: none;
        }

        .arrowPosition {
            float: left;
            margin-left: 5px;
        }

        .arrowDesc {
            margin: 35px 0px 0px 40px;
        }

        .orderStuff {
            display: block;
            float: left;
            margin: 10px 0px 10px 10px;
        }

        .marginOnly {
            margin: 10px 0px 10px 10px;
        }

        .mainContentDiv {
            margin-top: -20px;
        }

        .patientDemo {
            display: block;
            float: right;
            text-align: right;
        }

            .patientDemo a {
                margin-top: 10px;
            }

        .patientLabDemo {
            display: block;
            float: left;
            text-align: left;
        }

            .patientLabDemo a {
                margin-top: 10px;
            }

        span {
            font-weight: bold;
        }

        div.block {
            overflow: hidden;
        }

            div.block .label {
                width: 160px;
                display: block;
                float: left;
                text-align: left;
            }

            div.block .input {
                margin-left: 4px;
                float: left;
            }

        h1 {
            font-size: large;
            font-weight: bold;
            color: #782E1E;
        }

        .OrderHist {
            color: #585848;
            margin-right: 20px;
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

        a.AddScript {
            display: block;
            position: relative;
            color: #0000EE;
            padding-bottom: 10px;
        }

        .ScriptHist {
            margin-top: 10px;
            width: 100%;
            border: solid 1px #782E1E;
        }

            .ScriptHist th, .ScriptHist td {
                border: 1px solid #782E1E;
                vertical-align: middle !important;
                text-align: center !important;
            }

            .ScriptHist th {
                color: #FFFFFF;
                background-color: #782E1E;
            }

        .ScriptHistPri {
            color: #000000;
            background-color: #f0f0f0;
            height: 45px;
        }

        .ScriptHistAlt {
            color: #000000;
            background-color: #FFF;
            height: 45px;
        }

        .ScriptHistBtn {
            text-decoration: none;
            color: white;
            padding: 5px;
            background-color: #782E1E;
            text-transform: uppercase;
            font-weight: bold;
        }

            .ScriptHistBtn:hover {
                background-color: transparent;
                border: 1px #782E1E solid;
                color: #782E1E;
            }

        .widthfixed50px {
            width: 50px;
            min-width: 50px;
        }

        .width15 {
            width: 15%;
            min-width: 15%;
        }

        .widthquarter {
            width: 25%;
            min-width: 25%;
        }

        .widththird {
            width: 33%;
            min-width: 33%;
        }

        .widthhalf {
            width: 50%;
            min-width: 50%;
        }

        .width20 {
            width: 20%;
            min-width: 20%;
        }

        .width40 {
            width: 40%;
            min-width: 40%;
        }

        .width60 {
            width: 60%;
            min-width: 60%;
        }

        .width70 {
            width: 70%;
            min-width: 70%;
        }

        .width75 {
            width: 75%;
            min-width: 75%;
        }

        .width80 {
            width: 80%;
            min-width: 80%;
        }

        .widthfull {
            width: 100%;
            min-width: 100%;
        }

        .alignright {
            text-align: right;
        }

        .alignleft {
            text-align: left;
        }

        .aligncenter {
            text-align: center;
        }

        .borderless {
            border: none;
        }

        .transparent {
            background-color: transparent;
        }
        .imgViewDoc {
            margin-top:20px;
        }
                
  .PrescriptionDocumentDialog {
            position: absolute;
            top: 10px;
            left: 95px;
            height: auto;
            min-height: 120px;
            min-width: 650px;
            padding: 0px;
            background: transparent;
            border-radius: 4px;
        }

        .shadow {
            -webkit-box-shadow: 10px 10px 5px 0px rgba(0,0,0,0.12);
            -moz-box-shadow: 10px 10px 5px 0px rgba(0,0,0,0.12);
            box-shadow: 10px 10px 5px 0px rgba(0,0,0,0.12);
        }


        .PrescriptionDocumentDialog .header_info {
            font-size: 15px;
            color: #004994;
            padding: 5px 10px;
            background-color: transparent;
        }

        .PrescriptionDocumentDialog .content {
            background-color: #fff;
            padding: 10px 10px;
            text-align: left;
        }

        .PrescriptionDocumentDialog .title {
            width: 95%;
            padding: 10px 10px;
            text-align: center;
            font-size: 17px!important;
            color: #006600;
        }

        .PrescriptionDocumentDialog .message {
            margin: 5px;
            padding: 5px 10px;
            text-align: center;
            font-size: 13px!important;
            color: #000;
        }

        .PrescriptionDocumentDialog .w3-closebtn {
            margin-top: -3px;
        }
         #id_confrmdiv
        {
            display: none;
            position: relative;
            width: 450px;
            top:5px;
            left: 25px;
            padding: 6px 8px 8px;
            box-sizing: border-box;
            text-align: center;
        }

    </style>
    <script type="text/javascript">
        var OrderPriority = '<%=this.ddlOrderPriority.ClientID%>';
        var DispenseMethod = '<%=this.ddlShipTo.ClientID%>';
        var Frame = '<%=this.ddlFrame.ClientID%>';
        var Color = '<%=this.ddlColor.ClientID%>';
        var Eye = '<%=this.ddlEye.ClientID%>';
        var Bridge = '<%=this.ddlBridge.ClientID%>';
        var Temple = '<%=this.ddlTemple.ClientID%>';
        var Lens = '<%=this.ddlLens.ClientID%>';
        var Tint = '<%=this.ddlTint.ClientID%>';
        var Coating = '<%=this.ddlCoating.ClientID%>';
        var Material = '<%=this.ddlMaterial.ClientID%>';
        var Lab = '<%=this.ddlProdLab.ClientID%>';
        var IncompleteButton = '<%=this.bSaveIncompleteOrder.ClientID%>';
        var OrderSaveUpdateButton = '<%=this.bSaveOrder.ClientID%>';
        var CurrentDate = '<%=DateTime.Now.ToString("MM/dd/yyyy")%>';//dd MMM yyyy
        var IsIncomplete = '<%=this.Order.IsComplete%>';
        var NextCombo = '<%=this.hfNextCombo.ClientID%>';
        var OrderIsPrefill = '<%=this.hfOrderIsPrefill.ClientID%>';
    </script>
    <style>
        .ui-button {
            padding: 0em;
            margin: -1px 0px 0px -2px;
            height: 29px !important;
            width: 20px !important;
            }

        .ui-autocomplete-input {
            width: 80% !important;
            height: 25px !important;
            font-size: 12px !important;
        }

        .ui-menu {
            max-height: 500px;
            overflow: auto;
            overflow-x: hidden;
        }

        .ui-widget-content li {
            font-size: 12px !important;
            padding: 0.3em !important;
        }

        .ui-widget-content a {
            font-size: 12px !important;
            padding: 0.3em !important;
            text-decoration: none !important;
            border: none !important;
        }

        .ui-widget-content li:hover {
            padding: 0.3em !important;
            background-color: #007FFF !important;
            border: 1px solid blue !important;
        }

        .ui-widget-content a:hover {
            padding: 0.3em !important;
            border: none !important;
        }
    </style>
</asp:Content>
<asp:Content ID="omContent" ContentPlaceHolderID="MainContent" runat="server">
    <asp:ScriptManagerProxy ID="smpOrderManagement" runat="server">
        <Scripts>
            <asp:ScriptReference Path="~/Scripts/OrderManagement/OrderManagement.js" />
            <asp:ScriptReference Path="~/Scripts/OrderManagement/OrderManagementVal.js" />
            <asp:ScriptReference Path="~/Scripts/Global/GlobalVal.js" />
            <asp:ScriptReference Path="~/Scripts/Global/PassFailConfirm.js" />
            <asp:ScriptReference Path="~/Scripts/Global/Combobox.js" />
<%--            <asp:ScriptReference Path="~/Scripts/Global/SharedAddress.js" />--%>
        </Scripts>
    </asp:ScriptManagerProxy>
    <asp:HiddenField ID="hfPatientId" runat="server" ClientIDMode="Static" />
    <asp:HiddenField ID="hfUserId" runat="server" ClientIDMode="Static" />
    <asp:HiddenField ID="hfIsClinic" runat="server" ClientIDMode="Static" />
    <asp:HiddenField ID="hfRxName" runat="server" ClientIDMode="Static" />
    <asp:HiddenField ID="hfOdPdDistCombo" runat="server" ClientIDMode="Static" />
    <asp:HiddenField ID="hfOdPdNearCombo" runat="server" ClientIDMode="Static" />
    <asp:HiddenField ID="hfProviderId" runat="server" ClientIDMode="Static" />
    <asp:HiddenField ID="hdfOrderNumber" runat="server" ClientIDMode="Static" />
    <asp:HiddenField ID="hdfVerifiedDate" runat="server" ClientIDMode="Static" />
    <asp:HiddenField ID="hdfExpireDays" runat="server" ClientIDMode="Static" />
    <asp:HiddenField ID="hdfIsReprint" runat="server" ClientIDMode="Static" />
    <asp:HiddenField ID="hdfPatientEmailAddress" runat="server" ClientIDMode="Static" />


    <div id="divClinic" runat="server">
        <asp:HiddenField ID="hfButtonText" runat="server" />
         <div class="mainContentDiv">
            <div style="display: inline-block; width: 100%;">
                <div class="orderHist left" style="float: left; margin: -10px 0px 0px 200px; width: 50%;">
                    <a href="#" id="lbAjaxOrderHist" runat="server" style="float: left;" class="OrderHist" onclick="DoDialogAjax()">Order History</a>

                    <a href="#" id="lbIncompleteOrderHist" runat="server" style="float: left;" class="OrderHist" onclick="DoIncompleteDialog()">Incomplete Orders</a>
                </div>
                <div class="patientDemo" style="float: right; width: 50%;">
                    <div style="margin-right: 10px; margin-top: -15px">
                        <div>
                            <h1><a href="../SrtsPerson/PersonDetails.aspx?id=<%= this.PatientId %>&isP=true" style="text-decoration: none">
                                <asp:Label ID="lblDemo" runat="server" Style="font-weight: normal" /></a>
                            </h1>
                        </div>
                    
                    </div>
                </div>
            </div>
                        <div style="float:left;text-align:left;margin-top: 10px;padding-left:30px;padding-bottom:10px;width:100%">
                            <span>Next Eligible FOC Date: </span>
                            <asp:Label ID="lblNextEligFoc" runat="server"></asp:Label>
                               <div id="statusMessage"></div>
                            <div style="float:right;margin-top:-30px;padding-right:50px">
                                <asp:UpdatePanel ID="upMakeActive" runat="server" ChildrenAsTriggers="true">
                                <ContentTemplate>
                                    <div id="divConvertToActive" runat="server">
                                        <div style="float: right;">
                                            <asp:Button ID="bConvertToActive" runat="server" Text="Convert" CssClass="srtsButton" OnClick="bConvertToActive_Click" CausesValidation="false" />
                                        </div>
                                        <div style="margin-top: 12px; float: right;"><span>Convert patient to active duty: </span></div>
                                    </div>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                            </div>
                        </div>
          </div>

        <%--<div class="mainContentDiv">
            <div style="display: inline-block; width: 100%;">
                <div class="orderHist left" style="float: left; margin: -10px 0px 0px 10px; width: 33%;">
                    <a href="#" id="lbAjaxOrderHist" runat="server" style="float: left;" class="OrderHist" onclick="DoDialogAjax()">Order History</a>

                    <a href="#" id="lbIncompleteOrderHist" runat="server" style="float: left;" class="OrderHist" onclick="DoIncompleteDialog()">Incomplete Orders</a>
                </div>
                <div style="float: left; width: 33%; height: 35px;">
                    <div id="statusMessage"></div>
                </div>
                <div class="patientDemo" style="float: left; width: 33%;">
                    <div style="margin-right: 10px; margin-top: -13px">
                        <div>
                            <h1><a href="../SrtsPerson/PersonDetails.aspx?id=<%= this.PatientId %>&isP=true" style="text-decoration: none">
                                <asp:Label ID="lblDemo" runat="server" Style="font-weight: normal" /></a>
                            </h1>
                        </div>
                        <div style="margin-top: 10px;">
                            <span>Next Eligible FOC Date: </span>
                            <asp:Label ID="lblNextEligFoc" runat="server"></asp:Label>
                            <asp:UpdatePanel ID="upMakeActive" runat="server" ChildrenAsTriggers="true">
                                <ContentTemplate>
                                    <div id="divConvertToActive" runat="server">
                                        <div style="float: right;">
                                            <asp:Button ID="bConvertToActive" runat="server" Text="Convert" CssClass="srtsButton" OnClick="bConvertToActive_Click" CausesValidation="false" />
                                        </div>
                                        <div style="margin-top: 12px; float: right;"><span>Convert patient to active duty: </span></div>
                                    </div>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                    </div>
                </div>
            </div>
        </div>--%>

        <div class="padding">
            <div class="BeigeBoxContainer" style="width: 100%">
                <div class="BeigeBoxHeader" style="min-height: 25px; max-height: 25px; text-align: left;"></div>
                <div class="BeigeBoxContent" style="margin-left: 10px;">
                    <div style="float: left; width: 100%;">
                        <img src="../../Styles/images/CollapsePrescriptions.gif"
                            data-swap="../../Styles/images/ExpandPrescriptions.gif"
                            data-src="../../Styles/images/CollapsePrescriptions.gif"
                            id="imgPrescriptions"
                            onclick="DoToggle('divPrescriptionHist', 'imgPrescriptions')"
                            style="margin: 0px 0px 5px -5px; float: left;"
                            alt="Click to expand/collapse prescriptions" />
                        <div id="divPrescriptionHist" style="clear: left; margin: 5px;">
                            <a href="#" onclick="DoAddPrescription()" style="float: left; margin-bottom: 5px;" class="AddScript">+ Add Prescription</a>
                            <asp:GridView ID="gvPrescriptionHist" CssClass="ScriptHist" runat="server" AutoGenerateColumns="False" RowStyle-CssClass="ScriptHistPri"
                                AlternatingRowStyle-CssClass="ScriptHistAlt" OnRowCommand="gvPrescriptionHist_RowCommand" OnRowDataBound="gvPrescriptionHist_RowDataBound" OnSelectedIndexChanged="gvPrescriptionHist_SelectedIndexChanged">
                                <Columns>
                                    <asp:ButtonField Text="Add Order" ButtonType="Button" CommandName="Order" ControlStyle-CssClass="ScriptHistBtn" />
                                    <asp:TemplateField HeaderText="Rx Type">
                                        <ItemTemplate>
                                            <asp:Label ID="lblPrescriptionName" runat="server" Text='<%# Eval("PrescriptionName") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Prescription Date">
                                        <ItemTemplate>
                                            <asp:Label ID="lblPrescriptionDate" runat="server" Text='<%# Convert.ToDateTime(Eval("PrescriptionDate")).ToShortDateString() %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Rx">
                                        <ItemTemplate>
                                            <div>
                                                <asp:Label ID="lblRightRx" runat="server" Text='<%# String.Format("R {0} {1} x {2:000}",
                                Eval("OdSphereCalc").ToString().Equals("0.00") || Eval("OdSphereCalc").ToString().StartsWith("-") ? Eval("OdSphereCalc") : String.Format("+{0}", Eval("OdSphereCalc")),
                                Eval("OdCylinderCalc").ToString().Equals("0.00") || Eval("OdCylinderCalc").ToString().StartsWith("-") ? Eval("OdCylinderCalc") : String.Format("+{0}", Eval("OdCylinderCalc")),
                                Eval("OdAxisCalc")) %>'></asp:Label>
                                            </div>
                                            <div>
                                                <asp:Label ID="lblLeftRx" runat="server" Text='<%# String.Format("L {0} {1} x {2:000}",
                                Eval("OsSphereCalc").ToString().Equals("0.00") || Eval("OsSphereCalc").ToString().StartsWith("-") ? Eval("OsSphereCalc") : String.Format("+{0}", Eval("OsSphereCalc")),
                                Eval("OsCylinderCalc").ToString().Equals("0.00") || Eval("OsCylinderCalc").ToString().StartsWith("-") ? Eval("OsCylinderCalc") : String.Format("+{0}", Eval("OsCylinderCalc")),
                                Eval("OsAxisCalc")) %>'></asp:Label>
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Add">
                                        <ItemTemplate>
                                            <div>
                                                <asp:Label ID="lblOdAdd" runat="server" Text='<%# Eval("OdAdd") %>'></asp:Label>
                                            </div>
                                            <div>
                                                <asp:Label ID="lblOsAdd" runat="server" Text='<%# Eval("OsAdd") %>'></asp:Label>
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Prism">
                                        <ItemTemplate>
                                            <div>
                                                <asp:Label ID="lblOdPrism" runat="server" Text='<%# String.Format("H {0}, V {1}", Eval("OdHPrism"), Eval("OdVPrism")) %>'></asp:Label>
                                            </div>
                                            <div>
                                                <asp:Label ID="lblOsPrism" runat="server" Text='<%# String.Format("H {0}, V {1}", Eval("OsHPrism"), Eval("OsVPrism")) %>'></asp:Label>
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                       <asp:TemplateField HeaderText="Doc">
                                        <ItemTemplate>
                                               <asp:ImageButton ID="btnEdit" runat="server" ClientIDMode="Static"
                                             ImageUrl="~/Styles/images/img_headline.png" ToolTip="View this prescription document." Width="20px" Height="20px" CommandName="ViewDocument"
                                             CommandArgument='<%#Eval("PrescriptionScanId")+","+ Eval("PrescriptionId")%>' CausesValidation="false" Visible='<%# IsRxScan((Int32)Eval("PrescriptionScanId")) %>' />
                                        </ItemTemplate>
                                        <ItemStyle Width="25px"/>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="OrderedFrameHistory" HeaderText="Frames Ordered from RX" ItemStyle-Width="475px" />
                                </Columns>
                            </asp:GridView>
                        </div>
                    </div>
                </div>
                <div class="BeigeBoxFooter" style="clear: both;"></div>
            </div>
        </div>

        <div id="divPrescription" style="display: none; background-color: #f0f0f0;">
            <div style="min-height: 30px; max-height: 200px; padding-bottom: 10px;">
                <div id="errorMessage">
                </div>
            </div>
            <div style="margin-bottom: 10px;">
                <div style="float: right; margin-right: 5px; margin-top: -5px; margin-bottom: 5px;">
                    <asp:CheckBox ID="cbDeletePrescription" runat="server" Text="Delete" ClientIDMode="Static" onclick="return PrescriptionSaveUpdateButtonClick();" Visible="False" TabIndex="50"></asp:CheckBox>
                </div>
                <table class="widthfull">
                    <tr class="widthfull">
                        <td class="widththird">
                            <span>Date of Prescription:</span></td>
                            
                        <td class="alignleft">
                            <%--<asp:Label ID="lblCurrentDate" runat="server"></asp:Label>--%>
                            <asp:TextBox ID="tbPrescriptionDate" runat="server" ClientIDMode="Static" Width="95px"  onchange="DoIsDateMoreThan10Mos('tbPrescriptionDate')"></asp:TextBox>
                            <asp:Image runat="server" ID="calImagePrescriptionDate" AlternateText="Calendar" ImageUrl="~/Styles/images/Calendar_scheduleHS.png" />
                            <ajaxToolkit:CalendarExtender ID="CalendarExtender1" runat="server" TargetControlID="tbPrescriptionDate" Format="MM/dd/yyyy" PopupButtonID="calImagePrescriptionDate">
                            </ajaxToolkit:CalendarExtender>
                        </td>
                        <td class="alignright">
                            <asp:Label ID="lblPName1" runat="server" Text="Type:   " AssociatedControlID="ddlPrescriptionName" Font-Bold="true"></asp:Label></td>
                        <td class="widthquarter">
                            <asp:DropDownList ID="ddlPrescriptionName" runat="server" CssClass="" onchange="DdlHelper('ddlPrescriptionName', 'divPrescription', 'errorMessage', 'Prescription type'); " ClientIDMode="Static" TabIndex="1">
                                <asp:ListItem Text="-Select-" Value="X"></asp:ListItem>
                                <asp:ListItem Text="DVO" Value="DVO"></asp:ListItem>
                                <asp:ListItem Text="NVO" Value="NVO"></asp:ListItem>
                                <asp:ListItem Text="FTW" Value="FTW"></asp:ListItem>
                                <asp:ListItem Text="Computer/Other" Value="CO"></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                </table>
                <br />
                <br />
                <div style="float: right; margin-top: -20px; padding-right: 25px;">
                    <span>Mono  </span>
                    <asp:CheckBox ID="cbMonoOrComboPd" runat="server" ClientIDMode="Static" onclick="ToggleMonoComboPd('cbMonoOrComboPd')" />
                </div>
                <div class="left" style="position: relative; padding-left: 25px;">
                    <table class="">
                        <thead>
                            <tr class="aligncenter">
                                <td></td>
                                <td>Sph
                                </td>
                                <td>Cyl
                                </td>
                                <td>Axis
                                </td>
                                <td>Add
                                </td>
                            </tr>
                        </thead>
                        <tr class="">
                            <td class="alignright" style="padding-right: 10px">Right (OD)
                            </td>
                            <td>
                                <asp:TextBox ID="tbOdSphere" runat="server" Width="50px" onchange="SphInput('tbOdSphere', false)" ClientIDMode="Static" TabIndex="2" />
                            </td>
                            <td>
                                <asp:TextBox ID="tbOdCylinder" runat="server" Width="50px" onchange="CylInput('tbOdCylinder', 'tbOdAxis', false)" ClientIDMode="Static" TabIndex="3" />
                            </td>
                            <td>
                                <asp:TextBox ID="tbOdAxis" runat="server" Width="50px" onchange="AxisInput('tbOdAxis')" ClientIDMode="Static" TabIndex="4" />
                            </td>
                            <td>
                                <asp:TextBox ID="tbOdAdd" runat="server" Width="50px" onchange="AddHelper('tbOdAdd', 'tbOsAdd')" ClientIDMode="Static" TabIndex="5" />
                            </td>
                        </tr>
                        <tr class="">
                            <td class="alignright" style="padding-right: 10px">Left (OS)
                            </td>
                            <td>
                                <asp:TextBox ID="tbOsSphere" runat="server" Width="50px" onchange="SphInput('tbOsSphere', false)" ClientIDMode="Static" TabIndex="6" /></td>
                            <td>
                                <asp:TextBox ID="tbOsCylinder" runat="server" Width="50px" onchange="CylInput('tbOsCylinder', 'tbOsAxis', false)" ClientIDMode="Static" TabIndex="7" /></td>
                            <td>
                                <asp:TextBox ID="tbOsAxis" runat="server" Width="50px" onchange="AxisInput('tbOsAxis')" ClientIDMode="Static" TabIndex="8" /></td>
                            <td>
                                <asp:TextBox ID="tbOsAdd" runat="server" Width="50px" onchange="AddHelper('tbOsAdd', 'tbOdAdd')" ClientIDMode="Static" TabIndex="9" /></td>
                        </tr>
                    </table>
                </div>
                <div id="divComboPd" style="float: left;">
                    <table>
                        <thead>
                            <tr class="aligncenter">
                                <td>PD
                                </td>
                                <td>Near PD
                                </td>
                            </tr>
                        </thead>
                        <tr>
                            <td>
                                <asp:TextBox ID="tbOdPdDistCombo" runat="server" Width="50px" ClientIDMode="Static"
                                    onchange="CalcSetNearComboPdField('tbOdPdDistCombo', 'tbOdPdNearCombo')" TabIndex="10" /></td>
                            <td>
                                <asp:TextBox ID="tbOdPdNearCombo" runat="server" Width="50px" ClientIDMode="Static"
                                    onchange="DoComboPdVal('tbOdPdNearCombo')" TabIndex="11" />
                            </td>
                        </tr>
                    </table>
                </div>
                <div id="divMonoPd" style="display: none; float: left;">
                    <table>
                        <thead>
                            <tr class="aligncenter">
                                <td>PD
                                </td>
                                <td>Near PD
                                </td>
                            </tr>
                        </thead>
                        <tr>
                            <td>
                                <asp:TextBox ID="tbOdPdDistMono" runat="server" Width="50px" ClientIDMode="Static"
                                    onchange="CalcSetNearMonoPdField('tbOdPdDistMono', 'tbOdPdNearMono')" TabIndex="10" /></td>
                            <td>
                                <asp:TextBox ID="tbOdPdNearMono" runat="server" Width="50px" ClientIDMode="Static"
                                    onchange="DoMonoPdVal('tbOdPdNearMono')" TabIndex="11" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:TextBox ID="tbOsPdDistMono" runat="server" Width="50px" ClientIDMode="Static"
                                    onchange="CalcSetNearMonoPdField('tbOsPdDistMono', 'tbOsPdNearMono')" TabIndex="12" /></td>
                            <td>
                                <asp:TextBox ID="tbOsPdNearMono" runat="server" Width="50px" ClientIDMode="Static"
                                    onchange="DoMonoPdVal('tbOsPdNearMono')" TabIndex="13" /></td>
                        </tr>
                    </table>
                </div>
            </div>
            <br />
            <div class="left widthfull" style="clear: left;">
                <br />
                <img src="../../Styles/images/ExpandPrism.gif" data-swap="../../Styles/images/CollapsePrism.gif" data-src="../../Styles/images/ExpandPrism.gif" id="imgPrismBase" onclick="DoToggle('divPrismBase', 'imgPrismBase')" style="margin-top: 15px;" class="arrowPosition" alt="Click to expand/collapse prism" />
                <div id="divPrismBase" style="display: none; clear: left">
                    <hr />
                    <table id="tblPrismBase" style="margin-left: 25px;">
                        <thead>
                            <tr>
                                <td></td>
                                <td>H-Prism
                                </td>
                                <td>H-Base
                                </td>
                                <td>V-Prism
                                </td>
                                <td>V-Base
                                </td>
                            </tr>
                        </thead>
                        <tr>
                            <td>Right (OD)
                            </td>
                            <td>
                                <asp:TextBox ID="tbOdHPrism" runat="server" Width="60px" onchange="PrismInput('tbOdHPrism', 'tbOdHBase');" ClientIDMode="Static" TabIndex="14" /></td>
                            <td>
                                <asp:TextBox ID="tbOdHBase" runat="server" Width="60px" onblur="return PrismBase('tbOdHBase', 'tbOdHPrism');" ClientIDMode="Static" TabIndex="15" /></td>
                            <td>
                                <asp:TextBox ID="tbOdVPrism" runat="server" Width="60px" onchange="PrismInput('tbOdVPrism', 'tbOdVBase');" ClientIDMode="Static" TabIndex="16" /></td>
                            <td>
                                <asp:TextBox ID="tbOdVBase" runat="server" Width="60px" onblur="return PrismBase('tbOdVBase', 'tbOdVPrism');" ClientIDMode="Static" TabIndex="17" /></td>
                        </tr>
                        <tr>
                            <td>Left (OS)
                            </td>
                            <td>
                                <asp:TextBox ID="tbOsHPrism" runat="server" Width="60px" onchange="PrismInput('tbOsHPrism', 'tbOsHBase');" ClientIDMode="Static" TabIndex="18" /></td>
                            <td>
                                <asp:TextBox ID="tbOsHBase" runat="server" Width="60px" onblur="return PrismBase('tbOsHBase', 'tbOsHPrism');" ClientIDMode="Static" TabIndex="19" /></td>
                            <td>
                                <asp:TextBox ID="tbOsVPrism" runat="server" Width="60px" onchange="PrismInput('tbOsVPrism', 'tbOsVBase');" ClientIDMode="Static" TabIndex="20" /></td>
                            <td>
                                <asp:TextBox ID="tbOsVBase" runat="server" Width="60px" onblur="return PrismBase('tbOsVBase', 'tbOsVPrism');" ClientIDMode="Static" TabIndex="21" /></td>
                        </tr>
                    </table>
                </div>
                <%--Begin show calc values--%>
                <div class="left widthfull" style="clear: left;">
                    <br />
                    <img src="../../Styles/images/ExpandCalculated.gif" data-swap="../../Styles/images/CollapseCalculate.gif" data-src="../../Styles/images/ExpandCalculated.gif" id="imgCalcValues" onclick="DoToggle('divCalcValues', 'imgCalcValues')" class="arrowPosition" alt="Click to expand/collapse calculated values" />
                    <div id="divCalcValues" style="display: none; clear: left">
                        <hr />
                        <div class="prescriptionCalcValues">
                            <fieldset>
                                <legend>Calculated Values</legend>

                                <table style="width: 95%;">
                                    <tr>
                                        <td style="background: transparent;"></td>
                                        <td>Sph
                                        </td>
                                        <td>Cyl
                                        </td>
                                        <td>Axis
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="text-align: right;">Right (OD)&nbsp;&nbsp;
                                        </td>
                                        <td class="noColor">
                                            <asp:Label ID="lblODSphere_calc" runat="server" CssClass="prescriptionCalcValuesLabels" ClientIDMode="Static"></asp:Label>
                                            <asp:HiddenField ID="hfODSphereCalc" runat="server" ClientIDMode="Static" />
                                        </td>
                                        <td class="noColor">
                                            <asp:Label ID="lblODCylinder_calc" runat="server" CssClass="prescriptionCalcValuesLabels" ClientIDMode="Static"></asp:Label>
                                            <asp:HiddenField ID="hfODCylinderCalc" runat="server" ClientIDMode="Static" />
                                        </td>
                                        <td class="noColor">
                                            <asp:Label ID="lblODAxis_calc" runat="server" CssClass="prescriptionCalcValuesLabels" ClientIDMode="Static"></asp:Label>
                                            <asp:HiddenField ID="hfODAxisCalc" runat="server" ClientIDMode="Static" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="text-align: right;">Left (OS)&nbsp;&nbsp;
                                        </td>
                                        <td class="noColor">
                                            <asp:Label ID="lblOSSphere_calc" runat="server" CssClass="prescriptionCalcValuesLabels" ClientIDMode="Static"></asp:Label>
                                            <asp:HiddenField ID="hfOSSphereCalc" runat="server" ClientIDMode="Static" />
                                        </td>
                                        <td class="noColor">
                                            <asp:Label ID="lblOSCylinder_calc" runat="server" CssClass="prescriptionCalcValuesLabels" ClientIDMode="Static"></asp:Label>
                                            <asp:HiddenField ID="hfOSCylinderCalc" runat="server" ClientIDMode="Static" />
                                        </td>
                                        <td class="noColor">
                                            <asp:Label ID="lblOSAxis_calc" runat="server" CssClass="prescriptionCalcValuesLabels" ClientIDMode="Static"></asp:Label>
                                            <asp:HiddenField ID="hfOSAxisCalc" runat="server" ClientIDMode="Static" />
                                        </td>
                                    </tr>
                                </table>
                            </fieldset>
                        </div>
                    </div>
                </div>
                <%--End show calc values--%>
            </div>
            <div style="clear: both; padding: 20px 15px; text-align: left;">
                <asp:Label ID="lblScriptProvider" runat="server" AssociatedControlID="ddlPrescriptionProvider">Select Provider</asp:Label>
                <asp:DropDownList ID="ddlPrescriptionProvider" runat="server" ClientIDMode="Static" Width="320px" onchange="DdlHelper('ddlPrescriptionProvider', 'divPrescription', 'errorMessage', 'Provider name')" TabIndex="22"></asp:DropDownList>
            </div>

            <div id="divExtraRxs" runat="server" ClientIDMode="Static" class="left" style="clear: left; margin: 15px 0px 0px 15px">
                <asp:CheckBoxList ID="cblExtraRxTypes" runat="server" RepeatDirection="Horizontal">
                    <asp:ListItem Text="Add NVO" Value="N"></asp:ListItem>
                    <asp:ListItem Text="Add DVO" Value="D"></asp:ListItem>
                </asp:CheckBoxList>
            </div>

           <div id="divAttachPrescription" runat="server" style="float:left;margin-left:20px;margin-top:20px;height:110px;width:90%;overflow:hidden"> 
                <asp:Label id="lblUploadPrescription" runat="server" text="Attach Prescription"></asp:Label>                                   
                <iframe id="ifrUploadPrescription" runat="server" src="PrescriptionUpload.aspx" height="110" width="450" style="border: 0px none #FFF;overflow:hidden"></iframe>
            </div>
                      <div style="color:#900d0d;margin-left:35px"> <asp:Label runat="server" ID="lblDeleteInfo" /></div> 
            <div style="float:left;text-align: left;padding-left:0px;margin:20px 0px 0px -30px">
            <br />
            <div style="float:left;margin-left:50px">
                <asp:Button ID="bSaveUpdatePrescription" runat="server" CssClass="srtsButton" Text="Save" OnClientClick="return PrescriptionSaveUpdateButtonClick();" OnClick="bSaveUpdatePrescription_Click" TabIndex="55" />
                <asp:Button ID="bSaveRxOpenOrder" runat="server" CssClass="srtsButton" Text="Save and Order" OnClientClick="return PrescriptionSaveUpdateButtonClick();" OnClick="bSaveRxOpenOrder_Click" TabIndex="56" />
                <asp:Button ID="btnDeleteScan" runat="server" CssClass="srtsButton" Text="Delete Attached Document" OnClientClick="return IsDeletePrescritionScan();"/>

                <asp:Button ID="bSaveNewPrescription" runat="server" CssClass="srtsButton" Text="Save As New" OnClientClick="return PrescriptionSaveUpdateButtonClick();" OnClick="bSaveNewPrescription_Click" TabIndex="57" />
             </div>
             <div id="divDeletePrescription" runat="server" visible="false" style="float:left;margin-left:35px;margin-top:5px;width:90%"> 
                <div id="id_confrmdiv" style="display:none"><span style="color:#d30a0a">Are you sure you want to delete the attached document?</span><br />
                <asp:Button id="id_truebtn" runat="server" OnClick="btnDelete_Click" Text="Yes" CssClass="srtsButton" ClientIDMode="Static" ></asp:Button>
                <asp:Button id="id_falsebtn" runat ="server" Text="No" ClientIDMode="Static" CssClass="srtsButton"></asp:Button>
                </div> 
             </div>   

            </div>
        </div>

        <div class="padding">
            <div class="BeigeBoxContainer" style="width: 100%">
                <div class="BeigeBoxHeader" style="min-height: 25px; max-height: 25px; text-align: left;"></div>
                <div class="BeigeBoxContent" style="margin-left: 10px;">
                    <div style="clear: both; float: left; margin-top: 15px; width: 100%;">
                        <img src="../../Styles/images/ExpandExams.gif"
                            data-swap="../../Styles/images/CollapseExams.gif"
                            data-src="../../Styles/images/ExpandExams.gif"
                            id="imgExams"
                            onclick="DoToggle('divExam', 'imgExams')"
                            style="margin: 0px 0px 5px -5px; float: left;"
                            alt="Click to expand/collapse exams" />
                        <div id="divExam" style="float: left; display: none; clear: left;">
                            <a href="#" onclick="DoAddExam()" style="float: left;" class="AddScript">+ Add Exam</a>
                            <asp:GridView ID="gvExamHist" CssClass="ScriptHist" runat="server" AutoGenerateColumns="false" RowStyle-CssClass="ScriptHistPri"
                                AlternatingRowStyle-CssClass="ScriptHistAlt" OnSelectedIndexChanged="gvExamHist_SelectedIndexChanged" OnRowDataBound="gvExamHist_RowDataBound">
                                <Columns>
                                    <asp:BoundField DataField="ExamDate" HeaderText="Exam Date" DataFormatString="{0:d}" ReadOnly="true" />
                                    <asp:BoundField DataField="OdOsCorrected" HeaderText="Both Corrected" ReadOnly="true" />
                                    <asp:BoundField DataField="OdOsUncorrected" HeaderText="Both Uncorrected" ReadOnly="true" />
                                </Columns>
                            </asp:GridView>
                        </div>
                    </div>
                </div>
                <div class="BeigeBoxFooter" style="clear: both;"></div>
            </div>
        </div>

        <div id="divExamData" style="display: none;">
            <div style="margin-left: 10px;">
                <div style="min-height: 30px; max-height: 200px;">
                    <div id="divExamError">
                    </div>
                </div>
                Exam Date (mm/dd/yyyy):<br />
                <asp:TextBox ID="tbExamDate" runat="server" ClientIDMode="Static" onchange="DoIsNotFutureDate('tbExamDate')"></asp:TextBox>
                <asp:Image runat="server" ID="calImage1" AlternateText="Calendar" ImageUrl="~/Styles/images/Calendar_scheduleHS.png" />
                <ajaxToolkit:CalendarExtender ID="ceExamDate" runat="server" TargetControlID="tbExamDate" Format="MM/dd/yyyy" PopupButtonID="calImage1">
                </ajaxToolkit:CalendarExtender>
                <br />
                <br />
                <table style="width: 300px">
                    <tr class="srtsLabel_medium">
                        <td>&nbsp;
                        </td>
                        <td>Uncorrected
                        </td>
                        <td>Corrected
                        </td>
                    </tr>
                    <tr>
                        <td>Right(OD)
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlOdUncorrected" runat="server" ClientIDMode="Static">
                                <asp:ListItem Text="20/10" Value="20/10" Selected="True" />
                                <asp:ListItem Text="20/15" Value="20/15" />
                                <asp:ListItem Text="20/17" Value="20/17" />
                                <asp:ListItem Text="20/20" Value="20/20" />
                                <asp:ListItem Text="20/25" Value="20/25" />
                                <asp:ListItem Text="20/30" Value="20/30" />
                                <asp:ListItem Text="20/40" Value="20/40" />
                                <asp:ListItem Text="20/50" Value="20/50" />
                                <asp:ListItem Text="20/60" Value="20/60" />
                                <asp:ListItem Text="20/70" Value="20/70" />
                                <asp:ListItem Text="20/80" Value="20/80" />
                                <asp:ListItem Text="20/100" Value="20/100" />
                                <asp:ListItem Text="20/200" Value="20/200" />
                                <asp:ListItem Text="20/300" Value="20/300" />
                                <asp:ListItem Text="20/400" Value="20/400" />
                                <asp:ListItem Text="20/400+" Value="20/400+" />
                                <asp:ListItem Text="Unknown" Value="Unknown" />
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlOdCorrected" runat="server" ClientIDMode="Static">
                                <asp:ListItem Text="20/10" Value="20/10" Selected="True" />
                                <asp:ListItem Text="20/15" Value="20/15" />
                                <asp:ListItem Text="20/17" Value="20/17" />
                                <asp:ListItem Text="20/20" Value="20/20" />
                                <asp:ListItem Text="20/25" Value="20/25" />
                                <asp:ListItem Text="20/30" Value="20/30" />
                                <asp:ListItem Text="20/40" Value="20/40" />
                                <asp:ListItem Text="20/50" Value="20/50" />
                                <asp:ListItem Text="20/60" Value="20/60" />
                                <asp:ListItem Text="20/70" Value="20/70" />
                                <asp:ListItem Text="20/80" Value="20/80" />
                                <asp:ListItem Text="20/100" Value="20/100" />
                                <asp:ListItem Text="20/200" Value="20/200" />
                                <asp:ListItem Text="20/300" Value="20/300" />
                                <asp:ListItem Text="20/400" Value="20/400" />
                                <asp:ListItem Text="20/400+" Value="20/400+" />
                                <asp:ListItem Text="Unknown" Value="Unknown" />
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td>Left(OS)
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlOsUnCorrected" runat="server" ClientIDMode="Static">
                                <asp:ListItem Text="20/10" Value="20/10" Selected="True" />
                                <asp:ListItem Text="20/15" Value="20/15" />
                                <asp:ListItem Text="20/17" Value="20/17" />
                                <asp:ListItem Text="20/20" Value="20/20" />
                                <asp:ListItem Text="20/25" Value="20/25" />
                                <asp:ListItem Text="20/30" Value="20/30" />
                                <asp:ListItem Text="20/40" Value="20/40" />
                                <asp:ListItem Text="20/50" Value="20/50" />
                                <asp:ListItem Text="20/60" Value="20/60" />
                                <asp:ListItem Text="20/70" Value="20/70" />
                                <asp:ListItem Text="20/80" Value="20/80" />
                                <asp:ListItem Text="20/100" Value="20/100" />
                                <asp:ListItem Text="20/200" Value="20/200" />
                                <asp:ListItem Text="20/300" Value="20/300" />
                                <asp:ListItem Text="20/400" Value="20/400" />
                                <asp:ListItem Text="20/400+" Value="20/400+" />
                                <asp:ListItem Text="Unknown" Value="Unknown" />
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlOsCorrected" runat="server" ClientIDMode="Static">
                                <asp:ListItem Text="20/10" Value="20/10" Selected="True" />
                                <asp:ListItem Text="20/15" Value="20/15" />
                                <asp:ListItem Text="20/17" Value="20/17" />
                                <asp:ListItem Text="20/20" Value="20/20" />
                                <asp:ListItem Text="20/25" Value="20/25" />
                                <asp:ListItem Text="20/30" Value="20/30" />
                                <asp:ListItem Text="20/40" Value="20/40" />
                                <asp:ListItem Text="20/50" Value="20/50" />
                                <asp:ListItem Text="20/60" Value="20/60" />
                                <asp:ListItem Text="20/70" Value="20/70" />
                                <asp:ListItem Text="20/80" Value="20/80" />
                                <asp:ListItem Text="20/100" Value="20/100" />
                                <asp:ListItem Text="20/200" Value="20/200" />
                                <asp:ListItem Text="20/300" Value="20/300" />
                                <asp:ListItem Text="20/400" Value="20/400" />
                                <asp:ListItem Text="20/400+" Value="20/400+" />
                                <asp:ListItem Text="Unknown" Value="Unknown" />
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td>Both
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlOdOsUnCorrected" runat="server" ClientIDMode="Static">
                                <asp:ListItem Text="20/10" Value="20/10" Selected="True" />
                                <asp:ListItem Text="20/15" Value="20/15" />
                                <asp:ListItem Text="20/17" Value="20/17" />
                                <asp:ListItem Text="20/20" Value="20/20" />
                                <asp:ListItem Text="20/25" Value="20/25" />
                                <asp:ListItem Text="20/30" Value="20/30" />
                                <asp:ListItem Text="20/40" Value="20/40" />
                                <asp:ListItem Text="20/50" Value="20/50" />
                                <asp:ListItem Text="20/60" Value="20/60" />
                                <asp:ListItem Text="20/70" Value="20/70" />
                                <asp:ListItem Text="20/80" Value="20/80" />
                                <asp:ListItem Text="20/100" Value="20/100" />
                                <asp:ListItem Text="20/200" Value="20/200" />
                                <asp:ListItem Text="20/300" Value="20/300" />
                                <asp:ListItem Text="20/400" Value="20/400" />
                                <asp:ListItem Text="20/400+" Value="20/400+" />
                                <asp:ListItem Text="Unknown" Value="Unknown" />
                            </asp:DropDownList>
                        </td>

                        <td>
                            <asp:DropDownList ID="ddlOdOsCorrected" runat="server" ClientIDMode="Static">
                                <asp:ListItem Text="20/10" Value="20/10" Selected="True" />
                                <asp:ListItem Text="20/15" Value="20/15" />
                                <asp:ListItem Text="20/17" Value="20/17" />
                                <asp:ListItem Text="20/20" Value="20/20" />
                                <asp:ListItem Text="20/25" Value="20/25" />
                                <asp:ListItem Text="20/30" Value="20/30" />
                                <asp:ListItem Text="20/40" Value="20/40" />
                                <asp:ListItem Text="20/50" Value="20/50" />
                                <asp:ListItem Text="20/60" Value="20/60" />
                                <asp:ListItem Text="20/70" Value="20/70" />
                                <asp:ListItem Text="20/80" Value="20/80" />
                                <asp:ListItem Text="20/100" Value="20/100" />
                                <asp:ListItem Text="20/200" Value="20/200" />
                                <asp:ListItem Text="20/300" Value="20/300" />
                                <asp:ListItem Text="20/400" Value="20/400" />
                                <asp:ListItem Text="20/400+" Value="20/400+" />
                                <asp:ListItem Text="Unknown" Value="Unknown" />
                            </asp:DropDownList>
                        </td>
                    </tr>
                </table>
                <br />
                <div style="margin-bottom: 10px;">
                    Comments:<br />
                    <asp:TextBox ID="tbComments" runat="server" TextMode="MultiLine" Width="500px" Height="80px" ToolTip="Enter any comments, valid characters include + - . , ! ? # ' / ( )" ClientIDMode="Static"></asp:TextBox>
                </div>
                Provider:<br />
                <asp:DropDownList ID="ddlExamProviders" runat="server" DataTextField="NameLFMi" DataValueField="ID" Width="150px" ClientIDMode="Static" onchange="DdlHelper('ddlExamProviders', 'divExamData', 'divExamError', 'Provider name')">
                    <asp:ListItem Text="-Select-" Value="X" Selected="True" />
                </asp:DropDownList>
                <br />
                <div style="text-align: center">
                    <asp:Button ID="bAddUpdateExam" runat="server" Text="Save" ToolTip="Accept the exam data entered" CssClass="srtsButton"
                        OnClientClick="return ExamSaveUpdateButtonClick()" OnClick="bAddUpdateExam_Click" ClientIDMode="Static" />
                </div>
            </div>
        </div>

        <div class="padding">
            <div class="BeigeBoxContainer" style="width: 100%">
                 <div class="BeigeBoxHeader" style="min-height: 25px; max-height: 25px; text-align: left;"></div>
                <div class="BeigeBoxContent" style="margin-left: 10px;">
                   <div style="clear: both; float: left; margin-top: 15px; width: 100%;">
                        <img src="../../Styles/images/ExpandOrderNotifications.gif"
                            data-swap="../../Styles/images/CollapseOrderNotifications.gif"
                            data-src="../../Styles/images/ExpandOrderNotifications.gif"
                            id="imgOrderNotifications"
                            onclick="DoToggle('divOrderNotification', 'imgOrderNotifications')"
                            style="margin: 0px 0px 5px -5px; float: left; cursor:pointer"
                            alt="Click to expand/collapse order notifications" />
                        <div id="divOrderNotification" style="float: left; display: none; clear: left;">
                            <a href="#" onclick="DoUpdateEmailAddressDialog()" style="float: left;" class="AddScript">Update Email Address</a>
                            <asp:GridView ID="gvOrderNotifications" CssClass="ScriptHist" runat="server" AutoGenerateColumns="false" RowStyle-CssClass="ScriptHistPri"
                                AlternatingRowStyle-CssClass="ScriptHistAlt">
                                <Columns>
                                    <asp:BoundField DataField="OrderNumber" HeaderText="Order Number"  ReadOnly="true" />
                                    <asp:BoundField DataField="EmailAddress" HeaderText="Email Address" ReadOnly="true" />
                                    <asp:BoundField DataField="EmailMsg" HeaderText="Email Message" ReadOnly="true" />
                                    <asp:BoundField DataField="EmailDate" HeaderText="Email Date" DataFormatString="{0:d}" ReadOnly="true" />
                                </Columns>
                            </asp:GridView>
    </div>
                       </div>
                </div>
                <div class="BeigeBoxFooter" style="clear: both;"></div>
            </div>
        </div>

    </div>

    <div id="divOrderHistory" style="display: none;">
        <asp:UpdatePanel ID="upHist" runat="server" ChildrenAsTriggers="true">
            <ContentTemplate>
                <table id="tblOrderHist" class="OrderHist">
                    <thead>
                        <tr>
                            <th></th>
                            <th>Frame</th>
                            <th>Date Last Modified</th>
                            <th>Current Status</th>
                        </tr>
                    </thead>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>

    <div id="divIncompleteOrderHistory" style="display: none;">
        <asp:UpdatePanel ID="upIncHist" runat="server" ChildrenAsTriggers="true">
            <ContentTemplate>
                <table id="tblIncOrderHist" class="OrderHist">
                    <thead>
                        <tr>
                            <th></th>
                            <th>OrderNumber</th>
                            <th>Date Last Modified</th>
                            <th>Current Status</th>
                        </tr>
                    </thead>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>

    <div id="divOrderStatusHistoryDialog" style="display: none;">
        <%-- Add Gridview to show status history --%>
        <div style="float: left;">
            <asp:GridView ID="gvStatusHistory" runat="server" AutoGenerateColumns="false" CssClass="mGrid" GridLines="None">
                <RowStyle BackColor="#ccffff" />
                <AlternatingRowStyle BackColor="#ffffcc" />
                <Columns>
                    <asp:BoundField HeaderText="Order Status" DataField="OrderStatusType" />
                    <asp:BoundField HeaderText="Status Comment" DataField="StatusComment" />
                    <asp:BoundField HeaderText="Lab Site Code" DataField="LabCode" />
                    <asp:BoundField HeaderText="Modified By" DataField="ModifiedBy" />
                    <asp:BoundField HeaderText="Date Added" DataField="DateLastModified" />
                </Columns>
            </asp:GridView>
        </div>
    </div>

    <div id="divLab" runat="server" class="padding">
        <div style="margin-bottom: 15px;">
            <h1>
                 <asp:LinkButton ID="lnkPatientDetail" runat="server"></asp:LinkButton>
               <%-- <a href="../SrtsPerson/PersonDetails.aspx?id='<%#Eval("PatientId") %>'&isP=true">
                <asp:Label ID="lblLabDemo" runat="server" /></a>--%>
                </h1>
            <span>Next Eligible FOC Date: </span>
            <asp:Label ID="lblLabNextEligFoc" runat="server"></asp:Label>
        </div>

        <asp:GridView ID="gvLabOrders" runat="server" AutoGenerateColumns="false" OnRowCommand="gvLabOrders_RowCommand" DataKeyNames="OrderNumber" CssClass="mGrid" AlternatingRowStyle-CssClass="alt">
            <Columns>
                <asp:CommandField SelectText="Select" ButtonType="Link" ShowSelectButton="true" HeaderText=" " />
                <asp:BoundField DataField="OrderNumber" HeaderText="Order Number" />
                <asp:BoundField DataField="Frame" HeaderText="Frame" />
                <asp:BoundField DataField="DateLastModified" HeaderText="Order Date" ControlStyle-Width="100px" />
                <asp:BoundField DataField="CurrentStatus" HeaderText="Current Status" ItemStyle-HorizontalAlign="Left" />
            </Columns>
        </asp:GridView>
    </div>

    <div id="divOrder" style="display: none; height:auto; overflow-y: auto;">
        <asp:UpdatePanel ID="upOrder" runat="server" ChildrenAsTriggers="true" UpdateMode="Conditional">
            <ContentTemplate>
                <asp:HiddenField ID="hfMaxPair" runat="server" />
                <asp:HiddenField ID="hfIsFocFrame" runat="server" />
                <asp:HiddenField ID="hfIsFocEligible" runat="server" />
                <asp:HiddenField ID="hfIsPrevOrderFOC" runat="server" />
                <asp:HiddenField ID="hfOrderIsPrefill" runat="server" />
                <asp:HiddenField ID="hfLabPreference" runat="server" ClientIDMode="Static" />
                <asp:HiddenField ID="hfOrderStatus" runat="server" ClientIDMode="Static" />

                <div style="background: #f0f0f0; padding-bottom: 20px;">
                    <div style="min-height: 30px; max-height: 200px;">
                        <div id="divOrderError">
                        </div>
                    </div>
                    <div class="padding" style="display: block;">
                        <div class="w3-row" style="padding:0px">
                            <div class="w3-half">
                             <div class="left">
                                <h1>
                                    <asp:Label ID="labelOrderNumber" runat="server" Text="Order Number :  "></asp:Label>
                                    <asp:Label ID="lblOrderNumber" runat="server" Text="NEW" onmouseover="if (this.value != 'NEW') DoStatusHistoryDialog();" onmouseout="if (this.value != 'NEW') DoStatusHistoryDialogClose();" Font-Underline="true" ClientIDMode="Static"></asp:Label>
                                </h1>
                             </div>
                            </div>
                            <div class="w3-half" style="margin-left:0px">
                                <div id="divViewPrescriptionDoc" runat="server" visible="false" class="w3-row">
                                 <div style="float:right;margin-right:30px">
                                     <div> <asp:ImageButton ID="btnViewDoc" runat="server" ClientIDMode="Static"
                                    ImageUrl="~/Styles/images/img_headline.png" ToolTip="View this order's prescription document." Width="20px" Height="20px" 
                                        CommandName="ViewDocument" OnCommand="btnViewDoc_Command" OnClientClick="return DoShowDoc()"  /></div>
                                     <div style="float:left;font-weight:normal;font-style:italic;margin-left:25px;margin-top:-23px"> (Click image to view precription document.)</div>
                                 </div>
                             </div>
                            </div>
                        </div>
                       

                        <div id="divOrderOptions">
                        <div class="right" style="margin-right: 5px;">
                            <asp:CheckBox ID="cbReturnToStock" runat="server" Text="Return to Stock" onclick="ToggleOrderUpdateButton('cbReturnToStock')" ClientIDMode="Static" />
                        </div>
                        <div class="right" style="margin-right: 5px;">
                            <asp:CheckBox ID="cbDispense" runat="server" Text="Dispense" onclick="ToggleOrderUpdateButton('cbDispense')" ClientIDMode="Static" />
                        </div>
                        <div class="right" style="margin-right: 5px;">
                            <asp:CheckBox ID="cbDelete" runat="server" Text="Delete" onclick="DeleteCheckToggle();" Visible="False" ClientIDMode="Static" />
                        </div>
                        </div>
                    </div>
        
                    <div class="right" style="clear: both; margin: 5px 55px 20px 0px; font-size: 14px;">
                        <asp:Label ID="lblTechnician" runat="server" Text="Technician:"></asp:Label>&nbsp;
                        <asp:Label ID="lblOrderingTech" runat="server"></asp:Label>
                    </div>

                    <!-- LAB REDIRECT OR REJECT STUFF -->
                    <div id="divLabRedirectReject" class="marginOnly" visible="false" style="display: none;">
                        <div style="margin-bottom: 10px;">
                            <div class="w3-half">
                            <asp:RadioButtonList ID="rblRejectRedirect" runat="server" RepeatDirection="Horizontal" TabIndex="90" ClientIDMode="Static">
                                <asp:ListItem Text="Redirect" Value="redirect" Selected="True"></asp:ListItem>
                                <asp:ListItem Text="Reject" Value="reject"></asp:ListItem>
                                <asp:ListItem Text="Hold for Stock" Value="hfs"></asp:ListItem>
                            </asp:RadioButtonList>
                            </div>
                             <div class="w3-half" style="margin-top:-40px;margin-left:275px">
                                <asp:Button ID="bLabStatusChange" runat="server" TabIndex="93" OnClientClick="return DoRejectRedirectVal()" 
                                            OnClick="bLabStatusChange_Click" Text="Submit" CssClass="srtsButton" ClientIDMode="Static" />
                             </div>
                          
                           
                        </div>

                        <div class="w3-row">
                            <div class="w3-row">
                                <div id="divRedirect" style="float:left;width:97%;margin-left:10px">
                                    <div class="w3-half">
                                        <asp:Label ID="Label17" runat="server" Text="Select lab to redirect order to:"></asp:Label><br />
                                        <asp:DropDownList ID="ddlRedirectLab" runat="server" Width="350px" onchange="DoRejectRedirectVal()" TabIndex="91"></asp:DropDownList><br />
                                    </div>
                                   <div class="w3-half" style="float:right;margin-top:0px">
                                        <asp:Label ID="Label1" runat="server" Text="Enter justification for redirecting order:"></asp:Label>
                                   </div>                                  
                                </div>
                            </div>
                              <div class="w3-row">
                                    <div class="w3-half">&nbsp;                          
                                    </div>
                                    <div class="w3-half">
                                        <div id="divReject" style="margin-bottom:25px">
                                            <asp:Label ID="Label18" runat="server" Text="Enter justification for rejecting order:"></asp:Label><br />
                                        </div>
                                        <div id="divLabJust" style="margin-top: -22px;">
                                        <asp:HiddenField ID="hfRedirectJust" runat="server" ClientIDMode="Static" />
                                        <asp:HiddenField ID="hfRejectJust" runat="server" ClientIDMode="Static" />
                                        <asp:TextBox ID="tbLabJust" runat="server" TextMode="MultiLine" TabIndex="92" Width="85%" Height="30px" onchange="DoRejectRedirectVal()" ClientIDMode="Static"></asp:TextBox>
                                       
                                        </div>
                                    </div>
                              </div>
                        </div>
                       


                        <div id="divHoldForStock">
                            <div class="w3-row" style="margin-left:20px">
                                <div class="w3-half">
                                    <div>
                                        <asp:HiddenField ID="hfStockDate" runat="server" Value="" ClientIDMode="Static" />
                                        <asp:Label ID="Label111" runat="server" Text="Select Estimated Stock Date:"></asp:Label>
                                        <asp:TextBox ID="tbHfsDate" runat="server" TabIndex="5" Width="85px" CssClass="srtsTextBox_medium" ClientIDMode="Static" ReadOnly="true" onchange="DoStatusDateVal();" />
                                        <span class="calRight" style="margin-left: 8px;">
                                            <asp:Image runat="server" ID="imgHoldForStock" AlternateText="Calendar" ImageUrl="~/Styles/images/Calendar_scheduleHS.png" ClientIDMode="Static" />
                                            <ajaxToolkit:CalendarExtender ID="ceHfsDate" runat="server" TargetControlID="tbHfsDate" Format="MM/dd/yyyy" PopupButtonID="imgHoldForStock" ClientIDMode="Static">
                                            </ajaxToolkit:CalendarExtender>
                                        </span>
                                    </div>
                                    <div style="margin-top: 10px;">
                                        <asp:Label ID="Label112" runat="server" Text="Select Out of Stock Reason:"></asp:Label>
                                        <asp:DropDownList ID="ddlStockReason" runat="server" onchange="DoStockReason();" ClientIDMode="Static">
                                            <asp:ListItem Text="-Select-" Value="X" Selected="True"></asp:ListItem>
                                            <asp:ListItem Text="Frame Unavailable" Value="f"></asp:ListItem>
                                            <asp:ListItem Text="Lens Unavailable" Value="l"></asp:ListItem>
                                            <asp:ListItem Text="Other" Value="o"></asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>
                                <div class="w3-half">
                                    <div id="divHoldForStockHistory" style="margin-top: 10px;">
                                        <asp:Label ID="Label113" runat="server" Text="Current Hold for Stock Status:"></asp:Label><br />
                                        <asp:Label ID="lblCurrentHoldForStock" runat="server" ForeColor="#782E1E"></asp:Label>
                                    </div>
                                    <div style="margin-top: 10px;">
                                        <asp:CheckBox ID="cbCheckInHfsOrder" runat="server" Text="Check-In Hold for Stock Order " TextAlign="Left" ClientIDMode="Static" onchange="DoCheckinCheck();" />
                                    </div>
                                </div>
                            </div>                    
                        </div>
                    </div>
                    <!-- END LAB REDIRECT OR REJECT STUFF -->





 <asp:Panel ID="pnlAddOrder" runat="server">
                    <div class="w3-row padding">
                        <div class="w3-half">
                            <asp:Label ID="lblShipTo" runat="server" Text="Dispense Method"></asp:Label>
                             <asp:DropDownList ID="ddlShipTo" runat="server" TabIndex="1" CssClass="tabable" ClientIDMode="Static" AutoPostBack="true">
                                                <asp:ListItem Text="Clinic Distribution" Value="CD"></asp:ListItem>
                                                <asp:ListItem Text="Clinic Ship to Patient" Value="C2P"></asp:ListItem>
                                                <asp:ListItem Text="Lab Ship to Patient" Value="L2P"></asp:ListItem>
                             </asp:DropDownList>
                        </div>
                        <div class="w3-half">
                         
                            <span>Dispensing comments </span>
                            <asp:TextBox ID="tbDispComment" runat="server" MaxLength="45" Width="350px" TabIndex="2" CssClass="tabable" ToolTip="Maximum characters allowed is 45." ClientIDMode="Static"></asp:TextBox>
                  
                        </div>
                        
                   <asp:Label ID="lblDateVerified" runat="server" ClientIDMode="Static" Text="" ForeColor="Transparent" BorderWidth="0" />
                   <asp:Label ID="lblExpireDays" runat="server" ClientIDMode="Static" Text="" ForeColor="Transparent" BorderWidth="0" />
                       <%-- Address Validation--%>
                   <div id="divValidateAddress" class="w3-row" style="float:right;clear:both;display:none;padding:5px 30px">
                       <div id="msgValidateAddress" class="msgValidateAddress"></div>
                    <asp:Button ID="btnValidateAddress" runat="server" CssClass="srtsButton" Text="Validate Address" OnClick="btnValidateAddress_Click" CausesValidation="false" ClientIDMode="Static" />                  
                   </div>
                                 
                    
                    
                    
                    </div>
                    <%--////////////////////////////////////////////////////////////////--%>      

                 <div id="divEyeOrderInformation" class="w3-row padding">
                     <%-- Eyewear Order Information--%>


                    
                    <div class="marginOnly">
                        <table class="widthfull">
                            <tr class="widthfull">
                                <td class="widthhalf">
                                    <asp:CheckBox ID="cbEmailPatient" runat="server" AutoPostBack="true" Text="Patient requests email when order is complete" OnCheckedChanged="cbEmailPatient_Clicked" ></asp:CheckBox>
                                </td>
                                <td class="widthhalf">
                                </td>
                            </tr>
                            <tr class="widthfull">
                                <td class="width80">
                                   <asp:Label ID="lblPatientEmail" runat="server" Text="Patient Email"></asp:Label>
                                   <div style="display: inline-block; padding-left: 10px; font-size: 0.8em;">(This is the email address where notifications will be received. Please verify accuracy.)</div>
                                </td>
                                <td class="width20">
                                </td>
                            </tr>
                            </table>
                     </div>
                     <div class="marginOnly">
                        <table class="widthfull">
                             <tr class="widthfull">
                                <td class="widthhalf">
                                     <asp:TextBox ID="tbOrderEmailAddress" runat="server" TabIndex="3" Width="330px" /> 
                                </td>
                                <td class="widthhalf">
                                    <asp:CheckBox ID="cbPermanentEmailAddressChange" runat="server" Text="Permanent Email Address Change?"></asp:CheckBox>
                                </td>
                            </tr>
                        </table>
                    </div>
                     <br />
                    <div class="marginOnly">
                        <table class="widthfull">
                            <tr class="widthfull">
                                <td class="widthquarter">
                                    <asp:Label ID="lblOrderPriority" runat="server" Text="Priority"></asp:Label>
                                </td>
                                <td class="width75">
                                    <asp:Label ID="lblFrame" runat="server" Text="Frame"></asp:Label>
                                    <div style="display: inline-block; padding-left: 10px; font-size: 0.8em;">(Visible options limited by policy and regulation.)</div>
                                </td>
                            </tr>
                            <tr class="widthfull">
                                <td class="widthquarter">
                                    <div id="divOrderPriority">
                                        <asp:DropDownList ID="ddlOrderPriority" runat="server" TabIndex="4" ClientIDMode="Static" AutoPostBack="true" 
                                            OnSelectedIndexChanged="ddlOrderPriority_SelectedIndexChanged"></asp:DropDownList>
                                    </div>
                                </td>
                                <td class="width75" style="width:700px">
                                    <asp:DropDownList ID="ddlFrame" runat="server" TabIndex="5" ClientIDMode="Static" AutoPostBack="true" OnSelectedIndexChanged="ddlFrame_SelectedIndexChanged"></asp:DropDownList>
                                </td>
                            </tr>
                        </table>
                    </div>

                    <div class="marginOnly">
                        <asp:UpdatePanel ID="upTmpLimitEBT" runat="server" ChildrenAsTriggers="false" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:HiddenField ID="hfTmpNext" runat="server" Value="" ClientIDMode="Static" />
                                <table class="widthfull">
                                    <tr class="widthfull">
                                        <td class="width20">
                                            <asp:Label ID="lblFrameColor" runat="server" Text="Color" CssClass="widthfull"></asp:Label></td>
                                        <td class="width20">
                                            <asp:Label ID="lblEye" runat="server" Text="Eye" CssClass="widthfull"></asp:Label></td>
                                        <td class="width20">
                                            <asp:Label ID="lblBridge" runat="server" Text="Bridge" CssClass="widthfull"></asp:Label></td>
                                        <td class="width40">
                                            <asp:Label ID="lblTemple" runat="server" Text="Temple" CssClass="widthfull"></asp:Label></td>
                                    </tr>
                                    <tr class="widthfull">
                                        <td id="tdColor" class="width20">
                                            <asp:DropDownList ID="ddlColor" runat="server" TabIndex="6" ClientIDMode="Static" AutoPostBack="true" OnSelectedIndexChanged="ddlColor_SelectedIndexChanged"></asp:DropDownList>
                                        </td>
                                        <td id="tdEye" class="width20">
                                            <asp:DropDownList ID="ddlEye" runat="server" TabIndex="7" ClientIDMode="Static" AutoPostBack="true" OnSelectedIndexChanged="ddlEye_SelectedIndexChanged"></asp:DropDownList>
                                        </td>
                                        <td id="tdBridge" class="width20">
                                            <asp:DropDownList ID="ddlBridge" runat="server" TabIndex="8" ClientIDMode="Static"></asp:DropDownList>
                                        </td>
                                        <td id="tdTemple" class="width40">
                                            <asp:DropDownList ID="ddlTemple" runat="server" TabIndex="9" ClientIDMode="Static"></asp:DropDownList>
                                        </td>
                                    </tr>
                                </table>
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="ddlColor" EventName="SelectedIndexChanged" />
                                <asp:AsyncPostBackTrigger ControlID="ddlEye" EventName="SelectedIndexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>
                    </div>
                    <div class="marginOnly">
                        <table class="widthfull">
                            <tr class="widthfull">
                                <td class="widthquarter">
                                    <asp:Label ID="lblLensType" runat="server" Text="Lens" CssClass="widthfull"></asp:Label></td>
                                <td class="widthquarter">
                                    <asp:Label ID="LblLenseTint" runat="server" Text="Tint" CssClass="widthfull"></asp:Label></td>
                                <td class="widthquarter">
                                    <asp:Label ID="lblCoating" runat="server" Text="Coating" CssClass="widthfull"></asp:Label></td>
                                <td class="widthquarter">
                                    <asp:Label ID="lblFrameMat" runat="server" Text="Material" CssClass="widthfull"></asp:Label></td>
                            </tr>
                            <tr class="widthfull">
                                <td id="tdLens" class="widthquarter">
                                    <asp:DropDownList ID="ddlLens" runat="server" TabIndex="10" ClientIDMode="Static"></asp:DropDownList>
                                </td>
                                <td id="tdTint" class="widthquarter">
                                    <asp:DropDownList ID="ddlTint" runat="server" TabIndex="11" ClientIDMode="Static" AutoPostBack="true" OnSelectedIndexChanged="ddlTint_SelectedIndexChanged"></asp:DropDownList>
                                </td>
                                <td id="tdCoating">
                                    <asp:CheckBoxList ID="ddlCoating" runat="server" ClientIDMode="AutoID" AutoPostBack="true" OnSelectedIndexChanged="ddlCoating_SelectedIndexChanged"> 
                                    </asp:CheckBoxList>
                                </td>
                                <td id="tdMaterial" class="widthquarter">
                                    <asp:DropDownList ID="ddlMaterial" runat="server" TabIndex="12" ClientIDMode="Static" AutoPostBack="true" OnSelectedIndexChanged="ddlMaterial_SelectedIndexChanged"></asp:DropDownList>
                                </td>
                            </tr>
                        </table>
                    </div>
                    <div class="marginOnly left">
                        <table class="widthfull">
                            <tr class="widthfull">
                                <td class="widthquarter aligncenter" colspan="2">
                                    <asp:Label ID="lblSegHeight" runat="server" Text="Segment Height"></asp:Label></td>
                                <td class="width15 aligncenter">
                                    <asp:Label ID="lblPairs" runat="server" Text="Pair"></asp:Label></td>
                                <td class="width15 aligncenter">
                                   <%-- <asp:Label ID="lblShipTo" runat="server" Text="Dispense Method"></asp:Label>--%></td>
                                <td class="width15 alirncenter">
                                    <asp:Label ID="lblProdLab" runat="server" Text="Prod Lab" CssClass="widthfixed50px"></asp:Label>
       
                                </td>
                            </tr>

                            <tr class="widthfull">
                                <td class="align_right" style="padding-right: 10px">
                                    <asp:Label ID="lblSegHeightOD" runat="server" Text="Right(OD)"></asp:Label></td>
                                <td class="alignleft">
                                    <asp:TextBox ID="tbOdSegHt" runat="server" Width="50px" onchange="SegHtHelper('tbOdSegHt', 'tbOsSegHt')" CssClass="tabable" /></td>
                                <td class="aligncenter">
                                    <asp:TextBox ID="tbPair" runat="server" TabIndex="13" Width="50px" Text="1" onchange="DoMaxPairVal('tbPair')" CssClass="tabable" /></td>
                                <td>
             
                                       <%-- <asp:DropDownList ID="ddlShipTo" runat="server" TabIndex="13" CssClass="tabable" ClientIDMode="Static">
                                            <asp:ListItem Text="Clinic Distribution" Value="CD"></asp:ListItem>
                                            <asp:ListItem Text="Clinic Ship to Patient" Value="C2P"></asp:ListItem>
                                            <asp:ListItem Text="Lab Ship to Patient" Value="L2P"></asp:ListItem>
                                        </asp:DropDownList>
                                    <asp:Label ID="lblDateVerified" runat="server" ClientIDMode="Static" Text="test" ForeColor="Transparent" BorderWidth="0" />
                                    <asp:Label ID="lblExpireDays" runat="server" ClientIDMode="Static" Text="test" ForeColor="Transparent" BorderWidth="0" />--%>
                        
                                </td>
                                <td id="tdProdLab" class="alignleft">
                                    <asp:TextBox ID="txtProdLab" runat="server" ClientIDMode="Static" ReadOnly="true" Enabled="false"></asp:TextBox>
                                    <asp:DropDownList ID="ddlProdLab" runat="server" TabIndex="14" ClientIDMode="Static" Visible="false"></asp:DropDownList>
                                   <%-- <asp:HiddenField ID="hdfProdLabs" runat="server" ClientIDMode="Static" />--%>
                                </td>
                            </tr>
                            <tr>
                                <td class="align_right" style="padding-right: 10px">
                                    <asp:Label ID="lblSegHeightOS" runat="server" Text="Left(OS)"></asp:Label></td>
                                <td class="alignleft">
                                    <asp:TextBox ID="tbOsSegHt" runat="server" TabIndex="15" Width="50px" onchange="SegHtHelper('tbOsSegHt', 'tbOdSegHt')" CssClass="tabable" /></td>
                                <td></td>
                                <td></td>
                                <td class="alignleft" style="font-size: 14px;display:none">
                                    <asp:Label ID="lblCurrentLabHead" runat="server" Text="Current Lab:"></asp:Label>&nbsp;
                                    <asp:Label ID="lblCurrentLab" runat="server"></asp:Label></td>
                            </tr>
                        </table>
                    </div>

                     
      

                    <div class="w3-row orderStuff" style="width:95%">
                        <div class="w3-half" style="">
                            <asp:Label ID="lblComment1" runat="server" Text="Comment 1"></asp:Label><br />
                            <asp:TextBox ID="tbComment1" runat="server" TabIndex="16" Width="300px" TextMode="MultiLine" CssClass="tabable" />
                        </div>
                        <div class="w3-half" >
                            <asp:Label ID="lblComment2" runat="server" Text="Comment 2"></asp:Label><br />
                            <asp:TextBox ID="tbComment2" runat="server" TabIndex="17" Width="300px" TextMode="MultiLine" CssClass="tabable" />
                            <asp:HiddenField ID="hfOppositeSign" runat="server" ClientIDMode="Static" />
                        </div>
                    </div>

                    <div id="divJustBlock" class="" style="display: none; clear: both;">
                        <div id="divFocJust" class="orderStuff" style="display: none; ">
                            <asp:Label ID="lblFOCJust" runat="server" Text="FOC Justification"></asp:Label><br />
                            <asp:TextBox ID="tbFocJust" runat="server" Width="300px" TextMode="MultiLine" onchange="DoGenericCommentVal('tbFocJust', 'FOC');" CssClass="tabable" />
                        </div>
                        <div id="divMatJust" class="orderStuff" style="display: none; clear: both;">
                            <asp:Label ID="lblMatJust" runat="server" Text="Material Justification"></asp:Label><br />
                            <asp:TextBox ID="tbMaterialJust" runat="server" Width="300px" TextMode="MultiLine" onchange="DoMatVal('tbMaterialJust')" CssClass="tabable" />
                        </div>
                    </div>
                    <div id="hrJustBlock" class="marginOnly">
                    </div>
                    <div id="divCoatingJust" class="orderStuff" style="display: none;"> 
                            <asp:Label ID="lblCoatingJust" runat="server" Text="Coating Justification"></asp:Label><br />
                            <asp:TextBox ID="tbCoatingJust" runat="server" Width="300px" TextMode="MultiLine" onchange="DoCoatingVal('tbCoatingJust')" CssClass="tabable" />
                    </div>
                    <div id="divRejectBlock" visible="false" runat="server" style="clear: left;">
                        <div class="orderStuff" id="divRejectLab" style="float: left">
                            <asp:Label ID="lblRejectLabReason" runat="server" Text="Lab Comment"></asp:Label><br />
                            <asp:TextBox ID="tbRejectLabReason" runat="server" Width="300px" TextMode="MultiLine" ReadOnly="true" CssClass="tabable" />
                        </div>
                        <div class="orderStuff" id="div1divRejectClinic">
                            <asp:Label ID="lblRejectClinicReply" runat="server" Text="Clinic Justification"></asp:Label><br />
                            <asp:TextBox ID="tbRejectClinicReply" runat="server" Width="300px" TextMode="MultiLine" onchange="DoGenericCommentVal('tbRejectClinicReply', 'Clinic');" CssClass="tabable" />
                        </div>
                    </div>
                    <div id="hrRejectBlock" class="marginOnly" runat="server">
                    </div>


                    </div>
                    </asp:Panel>



                </div>
                <input type="hidden" runat="server" id="hfNextCombo" value="" />
                <div id="divUpdateEmailAddress" style="display: none;">
                    <asp:Label ID="lblEmailAddress" runat="server" Text="Email Address"></asp:Label>
                    <asp:TextBox ID="tbEmailAddress" runat="server" Width="95%" MaxLength="45"  ClientIDMode="Static" onchange="IsValidEmailAddress();"></asp:TextBox>
                    <div id="divUpdateEmailAddressMsg" style="color: red; width: 90%;"></div>
                    <div style="float: right; margin-top: 5px;">
                        <asp:Button ID="bUpdateEmailAddress" runat="server" CssClass="srtsButton" Text="Update" OnClientClick="return UpdateEmailAddressClick();" OnClick="bUpdateEmailAddress_Click" CausesValidation="false" />
                    </div>
                </div>
                <div id="divReorder" style="display: none;">
                    <asp:Label ID="lblReorderReason" runat="server" CssClass="srtsLabel_medium" Text="Select Reason For Re-Order" AssociatedControlID="ddlReorderReason"></asp:Label>
                    <asp:DropDownList ID="ddlReorderReason" runat="server"></asp:DropDownList>
                    <asp:TextBox ID="tbOther" runat="server" TextMode="MultiLine" Width="95%" Height="100px" MaxLength="45"></asp:TextBox>
                    <div style="float: right; margin-top: 5px;">
                        <asp:Button ID="bSaveReorder" runat="server" CssClass="srtsButton" Text="Save" OnClientClick="return DoSaveReorder()" />
                    </div>
                </div>

                <div id="divAddressDialog" style="display: none; overflow: hidden;">
                    <asp:UpdatePanel ID="upAddress" runat="server" ChildrenAsTriggers="false" UpdateMode="Conditional">
                        <ContentTemplate>
                            <div id="divAddresses" style="clear: both;">
                                <input type="button" id="bSpecialChars" value="Special Characters" class="srtsButton" onclick="DoDialog();" />
                                <div id="divAddressMsg" style="color: red; width: 90%;"></div>
                                <div style="height: auto">
                                    <div id="addressMessage"></div>
                                </div>

                                <!-- Address 1, Address 2 -->
                                <div class="w3-row">
                                    <!-- Address 1, Address 2 -->
                                    <div class="w3-half">
                                        <!-- Address 1 -->
                                        <div class="padding">
                                            <asp:Label ID="lblPrimaryAddAddress1" runat="server" Text="Address 1" CssClass="srtsLabel_medium" /><br />
                                            <asp:TextBox ID="tbPrimaryAddress1" runat="server" MaxLength="100" CssClass="srtsTextBox_medium ascii" ClientIDMode="Static"
                                                ToolTip="Enter the patient house and street address." Width="220px" />
                                            <asp:CustomValidator ID="cvPrimaryAddress1" runat="server" ControlToValidate="tbPrimaryAddress1" ClientValidationFunction="ValidateAddress1" ValidateEmptyText="true" />
                                        </div>
                                    </div>
                                    <div class="w3-half">
                                        <!-- Address 2 -->
                                        <div class="padding">
                                            <asp:Label ID="lblPrimaryAddAddress2" runat="server" Text="Address 2" CssClass="srtsLabel_medium" /><br />
                                            <asp:TextBox ID="tbPrimaryAddress2" runat="server" MaxLength="100" CssClass="srtsTextBox_medium ascii" ClientIDMode="Static" ToolTip="Continuation of patient address." Width="220px" />
                                            <asp:CustomValidator ID="cvPrimaryAddress2" runat="server" ControlToValidate="tbPrimaryAddress2" ClientValidationFunction="ValidateAddress2" ValidateEmptyText="true" />
                                        </div>
                                    </div>
                                </div>

                                <!-- City, State -->
                                <div class="w3-row" style="margin-right: 35px">
                                    <!-- Zip -->
                                    <div class="w3-third">
                                        <!-- Zip -->
                                        <div style="margin: 0px 0px 10px 0px;">
                                            <!-- Zip -->
                                            <div class="padding">
                                                <asp:Label ID="lblPrimaryZip" runat="server" Text="Zip" CssClass="srtsLabel_medium" /><br />
                                                <asp:TextBox ID="tbPrimaryZipCode" runat="server" CssClass="srtsTextBox_medium" ClientIDMode="Static"
                                                    ToolTip="Enter patient residence zip code" Width="150px" onkeydown="return (event.keyCode!=13);" onchange="javascript:DoZipLookup()" />
                                                <asp:CustomValidator ID="cvPrimaryZipCode" runat="server" ControlToValidate="tbPrimaryZipCode"
                                                    ClientValidationFunction="ValidateZip" ValidateEmptyText="true" />
                                            </div>
                                        </div>
                                    </div>

                                    <!-- City -->
                                    <div class="w3-third">
                                        <div style="margin: 0px 0px 10px 0px;">
                                            <!-- City -->
                                            <div class="padding">
                                                <asp:Label ID="lblPrimaryCity" runat="server" Text="City" CssClass="srtsLabel_medium" /><br />
                                                <asp:TextBox ID="tbPrimaryCity" runat="server" MaxLength="100" CssClass="srtsTextBox_medium ascii" ClientIDMode="Static" CausesValidation="true"
                                                    onclick="DoTbCityClick();" onblur="DoTbCityBlur();" ToolTip="Enter city name from patient address" Width="150px" />
                                                <asp:CustomValidator ID="cvPrimaryCity" runat="server" ControlToValidate="tbPrimaryCity" ClientValidationFunction="ValidateCity" ValidateEmptyText="true" />
                                            </div>
                                        </div>
                                    </div>

                                    <!-- Foreign City -->
                                    <div class="w3-third">
                                        <div style="margin: 0px 0px 0px 20px">
                                            &nbsp;&nbsp;<asp:RadioButtonList ID="rblCity" runat="server" TabIndex="8" RepeatDirection="Horizontal" ToolTip="Select Area"
                                                CausesValidation="true" ClientIDMode="Static" onchange="DoRblCityChange();">
                                                <asp:ListItem Text="APO" Value="APO" />
                                                <asp:ListItem Text="FPO" Value="FPO" />
                                                <asp:ListItem Text="DPO" Value="DPO" />
                                            </asp:RadioButtonList>
                                        </div>
                                    </div>
                                </div>

                                <!-- State, Country, UIC -->
                                <div class="w3-row">
                                    <!-- State -->
                                    <div class="w3-third">
                                        <div style="margin-left: 20px">
                                            <asp:Label ID="lblPrimaryState" runat="server" Text="State" CssClass="srtsLabel_medium" /><br />
                                            <asp:DropDownList ID="ddlPrimaryState" runat="server" ToolTip="Select patient residence state." ClientIDMode="Static"
                                                DataTextField="Value" DataValueField="Key" Width="160px">
                                            </asp:DropDownList>
                                            <asp:CustomValidator ID="cvPrimaryState" runat="server" ControlToValidate="ddlPrimaryState" ClientValidationFunction="ValidateState" ValidateEmptyText="true" />
                                        </div>
                                    </div>

                                    <!-- Country -->
                                    <div class="w3-third">
                                        <div class="padding" style="padding-top: 0px">
                                            <asp:Label ID="lblPrimaryCountry" runat="server" Text="Country" CssClass="srtsLabel_medium" /><br />
                                            <asp:DropDownList ID="ddlPrimaryCountry" runat="server" ToolTip="Select patient residence country."
                                                ClientIDMode="Static" onchange="DoDdlCountryChange();" DataTextField="Text" DataValueField="Value" Width="170px">
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                    <!-- UIC -->
                                    <div class="w3-third">
                                        <div style="margin: 0px 0px 10px 20px;">
                                            <asp:Label ID="lblPrimaryUIC" runat="server" Text="UIC" CssClass="srtsLabel_medium" /><br />
                                            <asp:TextBox ID="tbPrimaryUIC" runat="server" CssClass="srtsTextBox_medium" Width="125px" />
                                        </div>
                                    </div>
                                </div>

                                <!-- Special Character User Control -->
                                <srts:SpecialCharacters id="uSpecialCharacters" runat="server"></srts:SpecialCharacters>
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="bSaveAddress" EventName="Click" />
                        </Triggers>
                    </asp:UpdatePanel>
                    <!-- Address - Save Button -->
                    <div class="padding" style="position: relative; top: 25px; padding-bottom: 0px; padding-top: 0px; text-align: right">
                        <asp:Button ID="bSaveAddress" runat="server" CssClass="srtsButton" CausesValidation="false" Text="Update" OnClick="bSaveAddress_Click" Enabled="true" />
                    </div>
                </div>
                </div>
        <div style="clear: both; text-align: center; padding: 20px 0px; margin-top: 5px;">
            <asp:Button ID="bSaveCreateNewOrder" runat="server" CssClass="srtsButton tabable" TabIndex="18" Text="Submit and Create New" 
                OnClientClick="return OrderSaveUpdateButtonClick(true)" OnClick="bSaveCreateNewOrder_Click" ClientIDMode="Static" />
            <asp:Button ID="bSaveOrder" runat="server" CssClass="srtsButton tabable" TabIndex="19" Text="Submit and Close" 
                OnClientClick="return OrderSaveUpdateButtonClick(true)" OnClick="bSaveOrder_Click" ClientIDMode="Static" />
            <asp:Button ID="bSaveIncompleteOrder" runat="server" CssClass="srtsButton tabable" TabIndex="20" Text="Save Incomplete" 
                OnClick="bSaveIncompleteOrder_Click" CausesValidation="false" ClientIDMode="Static" />
            <asp:Button ID="bSaveNewOrder" runat="server" CssClass="srtsButton tabable" TabIndex="21" Text="Save As New" OnClientClick="return OrderSaveUpdateButtonClick(true)" OnClick="bSaveNewOrder_Click"  ClientIDMode="Static" />
            <asp:Button ID="bReprint771" runat="server" CssClass="srtsButton tabable" TabIndex="22" Text="Reprint 771" OnClientClick="return SetIsReprint();" OnClick="bReprint771_Click" CausesValidation="false" ClientIDMode="Static" />         
          <%--  <asp:Button ID="bVerifyAddress" runat="server" CssClass="srtsButton" Text="Verify Address" OnClick="bVerifyAddress_Click" CausesValidation="false" ClientIDMode="Static" />--%>
                </div>
                    <iframe id="hiddenDownload" runat="server" width="1" height="1"></iframe>
            </ContentTemplate>
            <Triggers>
                <%--<asp:AsyncPostBackTrigger ControlID="bLabStatusChange" EventName="Click" />--%>
                <asp:AsyncPostBackTrigger ControlID="bSaveCreateNewOrder" EventName="Click" />
                <asp:AsyncPostBackTrigger ControlID="bSaveOrder" EventName="Click" />
                <asp:AsyncPostBackTrigger ControlID="bSaveIncompleteOrder" EventName="Click" />
                <asp:AsyncPostBackTrigger ControlID="bSaveNewOrder" EventName="Click" />
                <asp:AsyncPostBackTrigger ControlID="ddlShipTo" EventName = "SelectedIndexChanged" /> 
               <asp:AsyncPostBackTrigger ControlID="bReprint771" EventName = "Click" /> 
              <%--  <asp:AsyncPostBackTrigger ControlID="bVerifyAddress" EventName="Click" />--%>
                <asp:AsyncPostBackTrigger ControlID="ddlOrderPriority" EventName="SelectedIndexChanged" />
                <asp:AsyncPostBackTrigger ControlID="ddlFrame" EventName="SelectedIndexChanged" />
                <asp:AsyncPostBackTrigger ControlID="ddlTint" EventName="SelectedIndexChanged" />
                <asp:AsyncPostBackTrigger ControlID="ddlCoating" EventName="SelectedIndexChanged" />
                <asp:AsyncPostBackTrigger ControlID="ddlMaterial" EventName="SelectedIndexChanged" />
                <asp:AsyncPostBackTrigger ControlID="ddlProdlab" EventName="SelectedIndexChanged" />
            </Triggers>
        </asp:UpdatePanel>


    </div>
       <%--PrescriptionDocumentDialog Modal --%>
               <%-- ///////////////////////////////////////////////////////////////////--%>
               <div id="PrescriptionDocumentDialog" class="w3-modal" style="z-index: 30000">
                <div class="w3-modal-content">
                    <div class="w3-container">
                        <div class="PrescriptionDocumentDialog">
                            <div class="BeigeBoxContainer shadow" style="width:550px">
                                <div style="background-color: #fff">
                                 <div class="BeigeBoxHeader" style="text-align:left;padding: 12px 10px 3px 15px">
                                     <div id="PrescriptionDocumentDialogheader" class="header_info">
                                            <span onclick="document.getElementById('PrescriptionDocumentDialog').style.display='none'"
                                                class="w3-closebtn">&times;</span>
                                             <span class="label">Order Management</span> - Prescription Document View
                                        </div>
                                 </div>
                                    <div class="BeigeBoxContent" style="margin-left: 10px; padding-top: 0px; height: 430px">
                                        <asp:UpdatePanel ID="uplPrescriptionDocumentView" runat="server" ChildrenAsTriggers="false" UpdateMode="Conditional">
                                            <ContentTemplate>  
                                                <div class="row padding">
                                                    <iframe id="ifrDocumentView" runat="server" src="PrescriptionDocumentView.aspx" width="500px" height="400px" />                                
                                                </div>                                         
                                            </ContentTemplate>
                                            <Triggers>
                                                  <asp:AsyncPostBackTrigger ControlID="btnViewDoc" EventName="Click" />
                                            </Triggers>
                                        </asp:UpdatePanel>

                                    </div> 
                                      <div class="BeigeBoxFooter" style="border-top:1px solid #E7CFAD;"></div>     
                                </div>
                            </div>

                        </div>
                    </div>
                </div>
    </div>
               <%--////////////////////////////////////////////////////////////////--%>     
  
</asp:Content>
