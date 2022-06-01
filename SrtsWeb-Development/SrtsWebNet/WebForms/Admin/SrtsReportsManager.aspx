<%@ Page Title="" Language="C#" MasterPageFile="~/srtsMaster.Master" AutoEventWireup="true" EnableEventValidation="false" CodeBehind="SrtsReportsManager.aspx.cs" Inherits="SrtsWeb.Admin.SrtsReportsManager" %>

<%@ MasterType VirtualPath="~/srtsMaster.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
<style>
    #divSelectAddress td {
    border:none;
    width:33px; 
    }
    .lblHide {
    display:none;
    width:0px;
    }

     .EditDialogMessage {
            position: absolute;
            top: 10px;
            left: 100px;
            height: auto;
            min-height: 120px;
            min-width: 600px;
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

        .success {
            width: 95%;
            padding: 0px 5px;
            margin-left: 20px;
            text-align: left;
            font-size: 20px!important;
            color: #2A7713;
        }

        .error {
            width: 95%;
            padding: 0px 5px;
            margin-left: 20px;
            text-align: left;
            font-size: 20px!important;
            color: #cc0000;
        }
 .lblIsValid {
                text-align: left!important;
                font-size: 12px;
                line-height: 1.5;
                color:#cc0000;
                visibility: hidden;
            }

        .descriptionItem {
            margin-bottom: 10px;
        }

        .colorBlue {
            color: #1252a0;
        }


         .AddressVerificationDialog {
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


        .AddressVerificationDialog .header_info {
            font-size: 15px;
            color: #004994;
            padding: 5px 10px;
            background-color: transparent;
        }

        .AddressVerificationDialog .content {
            background-color: #fff;
            padding: 10px 10px;
            text-align: left;
        }

        .AddressVerificationDialog .title {
            width: 95%;
            padding: 10px 10px;
            text-align: center;
            font-size: 17px!important;
            color: #006600;
        }

        .AddressVerificationDialog .message {
            margin: 5px;
            padding: 5px 10px;
            text-align: center;
            font-size: 13px!important;
            color: #000;
        }

        .AddressVerificationDialog .w3-closebtn {
            margin-top: -3px;
        }


</style>
<script type="text/javascript">
    var addr1 = '';
    var addr2 = '';
    var city = '';
    var tState = '';
    var state = tState == undefined || tState == '' ? 'X' : tState;
    var c = '';
    var s = '';
    var zip = '';
    var ddlCountry = '';
    var addressSet = false;
    function GetAddressValues() {
        // removing 'Unknown' and 'Non Applicable' as options in the State dropdown List
        ddlState = $('#ddlPrimaryState');
        ddlState.find('option[value=UN]').remove();
        ddlState.find('option[value=NA]').remove();
        ddlState.find('option[value=AA]').remove();
        ddlState.find('option[value=AE]').remove();
        ddlState.find('option[value=AP]').remove();
        getLocalStateData();
        getForeignCountryData();
        if (!addressSet) {
            addr1 = $('#tbPrimaryAddress1').val();
            addr2 = $('#tbPrimaryAddress2').val();
            city = $('#tbPrimaryCity').val();
            tState = $('#hdfState').val();
            state = tState == undefined || tState == '' ? 'X' : tState;
            c = $('#ddlPrimaryCountry').val();
            s = $('#ddlPrimaryState').val();
            zip = $('#tbPrimaryZipCode').val();
            ddlCountry = $('#ddlPrimaryCountry');
            country = $('#ddlPrimaryCountry').val();
            addressSet = true;
        }
        DoRblAddressTypeChangeEdit();
    }


    function DoRblAddressTypeChangeEdit() {

        rblAddressType = $('#rblAddressType').find('input:checked').val();
        if (rblAddressType == undefined) {
            //load saved address
            if (tState == 'NA') {
                isForeign = 'true';
                $('#rblAddressType').find('input[value=' + fn + ']').prop('checked', true);
                DoAddressTypeChangeEdit(fn, false);
            }
            else if (tState != 'NA') {
                isForeign = 'false';
                $('#rblAddressType').find('input[value=' + us + ']').prop('checked', true);
                DoAddressTypeChangeEdit(us, false);
            }
            else {
                DoAddressTypeChangeEdit(us, true);
            }
        }
        else {
            // do foreign address
            if (rblAddressType == 'FN') {
                isForeign = 'true';
                if (tState == 'NA') {
                    //load current foreign address
                    DoAddressTypeChangeEdit(fn, false);
                }
                else {
                    //load new foreign address
                    DoAddressTypeChangeEdit(fn, true);
                }
            }


            // do US address
            if (rblAddressType == 'US') {
                isForeign = 'false';
                if (tState != 'NA') {
                    //load current us address
                    DoAddressTypeChangeEdit(us, false);
                }
                else {
                    //load new us address
                    DoAddressTypeChangeEdit(us, true);
                }
            }
        }
    }

    function DoAddressTypeChangeEdit(t, isnew) {
        var usAddress = $('#divUsAddress');
        var uicAddress = $('#divUIC');
        var divState = $('#divState');
        var divAddress1 = $('#divAddress1');
        var divAddress2 = $('#divAddress2');
        ClearMsgs('divAddresses', 'divAddressMsg');
        // show selected address type form
        switch (t) {
            case (fn):
                isForeign = 'true';
                //set radio button to foreign
                $('#rblAddressType').find('input[value=' + fn + ']').prop('checked', true);

                switch (isnew) {
                    case (true):
                        //set country to 'select'
                        $('#ddlPrimaryCountry').val('X');
                        // clear address 1
                        $('#tbPrimaryAddress1').val('');
                        //clear address2
                        $('#tbPrimaryAddress2').val('');
                        break;
                    case (false):
                        $('#ddlPrimaryCountry').val(c);
                        $('#tbPrimaryAddress1').val(addr1);
                        $('#tbPrimaryAddress2').val(addr2);
                        break;
                }

                // clear and hide city, state and zip
                SetStatetoNA();
                //ddlState.hide();

                // clear city, and zip values
                $('#tbPrimaryCity').val("");
                $('#tbPrimaryZipCode').val("");
                $('#rblCity').find('input').removeAttr('checked');

                // show Foreign address form
                usAddress.hide();
                // uicAddress.hide();
                divState.hide();
                getCountriesForeign();
                break;
            case (us):
                isForeign = 'false';
                //set radio button to US
                $('#rblAddressType').find('input[value=' + us + ']').prop('checked', true);
                getCountriesUS();
                switch (isnew) {
                    case (true):
                        //set country to 'select'
                        $('#ddlPrimaryCountry').val('X');
                        // clear address 1
                        $('#tbPrimaryAddress1').val('');
                        //clear address2
                        $('#tbPrimaryAddress2').val('');
                        //set city value 
                        $('#tbPrimaryCity').val('');
                        SetPOCheckBox();
                        //set zip code value
                        $('#tbPrimaryZipCode').val('');
                        //set state value
                        ddlState.prop("disabled", false);
                        $('#ddlPrimaryState').val('X');
                        //set country value
                        $('#ddlPrimaryCountry').val(us);
                        break;
                    case (false):
                        // set to saved values
                        //set address1 value
                        $('#tbPrimaryAddress1').val(addr1);
                        //set address2 value
                        $('#tbPrimaryAddress2').val(addr2);
                        //set city value 
                        $('#tbPrimaryCity').val(city);
                        SetPOCheckBox();
                        //set zip code value
                        $('#tbPrimaryZipCode').val(zip);
                        //set state value
                        ddlState.prop("disabled", false);
                        if (s != 'X') {
                            SetPOCheckBox();
                            $('#ddlPrimaryState').val(s);
                        }
                        else {
                            $('#ddlPrimaryState').val('X');
                        }
                        //set country value
                        $('#ddlPrimaryCountry').val(us);
                        break;
                }

                // show US address form
                usAddress.show();
                // uicAddress.show();
                divState.show();

                break;
        }
    }
    function showfile(strpath) {
        window.open(strpath);
    }
    </script>
<%--    <link rel="stylesheet" type="text/css" href="../../../Styles/w3.css" />
     <link rel="stylesheet" type="text/css" href="../../../Styles/ui.jqgrid.css" />--%>
</asp:Content>
<asp:Content ID="contentClinicName" ContentPlaceHolderID="ContentTop_Title_Right" runat="server">
<%--    <asp:UpdatePanel ID="UpdatePanel2" runat="server">
        <ContentTemplate>
            <asp:Literal ID="litContentTop_Title_Right" runat="server"></asp:Literal>
        </ContentTemplate>
    </asp:UpdatePanel>--%>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">


    <div id="divSingleColumns" style="margin: 15px 0px 0px 0px; padding: 10px 40px">
        <%--   View Reports--%>
        <div class="BeigeBoxContainer" id="divPrintReports" runat="server"  style="margin: 0px">
            <div class="BeigeBoxHeader" style="padding: 12px 10px 3px 15px;text-align:left">
                <span class="label">View/Print Reports</span>
            </div>
            <div class="BeigeBoxContent" style="padding-top: 0px; height: auto">
            <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
                <ContentTemplate>
                    <asp:UpdatePanel ID="uplViewReports" runat="server">
                        <ContentTemplate>
                            <div class="tabContent">
                                 <div class="padding">
                                    <div class="countHeader">
                                        <div style="float: left; width: 375px">
                                            Select Report to View:&nbsp;&nbsp;
                                            <asp:DropDownList ID="ddlReportSelection" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlReportSelection_SelectedIndexChanged">
                                                <asp:ListItem>-- Select a Report to View --</asp:ListItem>
                                            </asp:DropDownList>&nbsp;&nbsp;
                                        </div>
                                        <div id="divReprint" runat="server" style="clear: both; float: left; width: 55%; margin-top: 15px;" visible="false">
                                            Select Item to Reprint:&nbsp;&nbsp;
                                            <asp:DropDownList ID="ddlReprint" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlReprint_SelectedIndexChanged">
                                            </asp:DropDownList>
                                        </div>
                                        <div id="divLblFormat" runat="server" style="margin: 15px 0px 0px 10px; float: left;" visible="false">
                                            Select Label Format:&nbsp;&nbsp;
                                                <asp:DropDownList ID="ddlLabelFormat" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlLabelFormat_SelectedIndexChanged">
                                                    <asp:ListItem Text="-Select-" Value="X"></asp:ListItem>
                                                    <asp:ListItem Text="Print To Label Avery 5160" Value="Avery 5160"></asp:ListItem>
                                                    <asp:ListItem Text="Print To Single Label" Value="Single Label"></asp:ListItem>
                                                </asp:DropDownList>
                                        </div>
                                    </div>
                                </div>
                                <asp:Label ID="Label2" runat="server"></asp:Label><br />
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </ContentTemplate>
            </asp:UpdatePanel>
            </div>
         <div class="BeigeBoxFooter"></div>       
         </div>

        
        <%-- Print Labels   --%> 
        <div  id="divPrintLabels" runat="server"  style="margin: -20px 0px 0px 0px">
        <div class="BeigeBoxContent" style="padding-top: 10px; min-height: 330px">
            <div id="divInstruction" style="margin-left: 0px; width: 100%; color: #004994; font-size: 1em; text-align: left; float: left; padding-bottom: 10px;">
                        <%--This text is set in the page load of the .js--%>
            </div>
            <br />

                  
            <asp:UpdatePanel ID="uplOrderAddresses" runat="server" ChildrenAsTriggers="true" UpdateMode="Conditional">
            <ContentTemplate>
                     <iframe id="hiddenDownload" runat="server" width="1" height="1" visible="false"></iframe>

                
                   <div style="float:left">
                    <!-- Submit, Clear, Cancel Buttons - Top -->
                    <div id="divLabelFunctions" style="float: right; margin-top: 1px">
                        <asp:Button ID="btnSubmitTop11" runat="server" CssClass="srtsButton submit" ToolTip="Print Labels for Grid Items." Text="Print_12" OnClick="btnSubmitTop_Click" />
                        <asp:Button ID="btnReprint" runat="server" CssClass="srtsButton submit" ToolTip="Load List of Print History." Text="History" 
                           OnClick="btnSubmit_Click" />
                        <asp:Button ID="btnClearTop" runat="server" CssClass="srtsButton clear" ToolTip="Remove All Grid Items." Text="Clear" 
                           OnClick="clearOrderAddresses_Click" />
                      <%--  <asp:Button ID="btnCancelTop" runat="server" CssClass="srtsButton" ToolTip="Cancel out of page" Text="Cancel" OnClick="btnCancel_Click" />--%>
                    <%-- <iframe id="hiddenDownload" runat="server" width="1" height="1"></iframe>--%>
                    </div>

                     <div class="countHeader" style="float: left; margin-left: 0px">
                        <!-- Single Order, Bulk Input -->
                       <asp:Panel ID="Panel1" DefaultButton="btnAddToGrid" runat="server">
                         <div style="float:left;margin: 10px 0px 0px 0px; min-width: 260px; max-width: 300px; text-align: left">
                            <asp:Label ID="lblSingleOrder" Text='Single Order: ' runat="server" CssClass="srtsLabel_medium_text"></asp:Label>
                            <asp:TextBox ID="tbSingleReadScan" runat="server" CssClass="srtsTextBox_medium" Width="150px"></asp:TextBox><br />
                             
                        </div>
                        
                        
                        <div style="float: left; margin-top: 1px;">
                            <asp:Button ID="btnAddToGrid" runat="server" CssClass="srtsButton submit" ToolTip="Add this order to the list" Text="Add Order" 
                            OnClientClick="return IsFunctionValid('add');" OnClick="btnAddOrderToGrid_Click"  CommandName="Single" />
                        <%--    <button id="btnBulkInput" onclick="return DisplayBulkInputDialog()" class="srtsButton bulk">Bulk Input</button>--%>
                        </div>
                           
                        
                     
                        
                      </asp:Panel>

                         <asp:Panel ID="Panel2" DefaultButton="btnBulk" runat="server">
                          <div style="float: left; margin: 0px 0px 0px 5px" class="print">
                             <asp:Button ID="btnBulk" runat="server" Text="Bulk Input" CssClass="srtsButton bulk" OnClientClick="return DisplayBulkInputDialog()" />
                         
                              <asp:Label ID="lblPrintFormat" Text='Label Format:  ' runat="server" CssClass="srtsLabel_medium_text"></asp:Label>
                            <asp:DropDownList ID="ddlSelectLabel" runat="server" ClientIDMode="Static">
                                <asp:ListItem Text="Print To Label Avery 5160" Value="Avery5160"></asp:ListItem>
                                <asp:ListItem Text="Print To Single Label" Value="SingleLabel"></asp:ListItem>
                            </asp:DropDownList>
                             
                        </div>  
                             
                                                   
                                 <%--Bulk Input Modal --%>                            
                                 <div id="OrderNumberBulkInput" class="w3-modal" style="z-index: 30000">
                                    <div class="w3-modal-content">
                                        <div class="w3-container">
                                            <div class="EditDialogMessage">
                                                <div class="BeigeBoxContainer shadow" style="width:400px">
                                                    <div style="background-color: #fff">
                                                     <div class="BeigeBoxHeader" style="text-align:left;padding: 12px 10px 3px 15px">
                                                         <div id="Div1" class="header_info">
                                                                <span onclick="document.getElementById('OrderNumberBulkInput').style.display='none'"
                                                                    class="w3-closebtn">&times;</span>
                                                                Bulk Order Number Input
                                                            </div>
                                                     </div>
                                                        <div class="BeigeBoxContent" style="padding: 10px 20px 20px 25px; min-height: 200px">
                                                            <p style="text-align:left;text-indent:0px">Scan or enter all order numbers you want to print labels for, then click &quot;Add&quot;</p>
                                                            <div class="w3-row" style="text-align:center">
                                                                <div class="w3-col" style="margin-left:30px;width:375px;text-align:left;padding:10px 10px">
                                                                    <asp:Label ID="lblOrderScanCount" runat="server"></asp:Label>
                                                                    <asp:TextBox ID="tbOrderNumbers" runat="server" TextMode="MultiLine" Width="60%" Height="200px" CssClass="srtsTextBox"/><br />
                                                                    <div style="float:right;margin-right:50px">
                                                                        <asp:Button ID="btnBulkDone" runat="server" CssClass="srtsButton" Text="Add" CommandName="Bulk"
                                                                        OnClientClick="return BtnBulkDone();" OnClick="btnAddOrderToGrid_Click" /></div>
                                                                </div>
                                                            </div>
                                                        </div>                                             
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                 </div>
                        
                         </asp:Panel>
                    </div>
                    </div>
                         <div style="float:left">                   
                            <div id="divHistoryItems" runat="server" visible="false" style="float:left;clear:both;margin-bottom:10px;margin-left:0px">
                                <asp:Label ID="lblHistoryItems" Text='Select History Item:  ' runat="server" CssClass="srtsLabel_medium_text"></asp:Label>
                                <asp:DropDownList ID="ddlHistoryItems" runat="server" AutoPostBack="true" ClientIDMode="Static"  
                                        OnSelectedIndexChanged="ddlHistoryItems_SelectedIndexChanged">
                                </asp:DropDownList>
                            </div>
                        </div>
                
                    <br />
                      
                    <asp:UpdatePanel ID="upGvOrderAddresses" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                    <asp:GridView ID="gvOrderAddresses" runat="server" AutoGenerateColumns="false" CssClass="mGrid"  DataKeyNames="PatientId, OrderNumber"
                        AlternatingRowStyle-CssClass="alt" ShowFooter="true" OnRowDataBound="gvOrderAddresses_RowDataBound" OnRowCreated="gvOrderAddresses_RowCreated" ClientIDMode="Static">               
                        <Columns>                         
                                <asp:TemplateField HeaderText="Order Number">
                                    <ItemTemplate>
                                        <asp:Label ID="OrderNumber" runat="server" Text='<%# Eval("OrderNumber") %>'></asp:Label><br />
                                        </ItemTemplate>
                                </asp:TemplateField>                                      
                                 <asp:TemplateField HeaderText="Patient Name">
                                    <ItemTemplate>
                                        <asp:Label ID="Patient" runat="server" Text='<%# string.Concat(Eval("FirstName"), " ", Eval("MiddleName"), ". ", Eval("LastName"))%>'></asp:Label><br />
                                        </ItemTemplate>
                                     <ItemStyle HorizontalAlign="Left" />
                                 </asp:TemplateField>
                                <asp:TemplateField HeaderText="Order Mailing Address">
                                    <ItemTemplate>
                                        <p id="pShipAddress1" runat="server" style="text-indent:0px"><%# Eval("ShipAddress1") %></p>
                                        <p id="pShipAddress2" runat="server" style="text-indent:0px"><%# Eval("ShipAddress2") %></p>                
                                        <p id="pShipAddress3" runat="server" style="text-indent:0px"><%# Eval("ShipAddress3") %></p>   
                                        <asp:Label ID="ShipCity" runat="server" Text='<%# Eval("ShipCity") %>'></asp:Label> 
                                        <asp:Label ID="ShipState" runat="server" Text='<%# Eval("ShipState") %>'></asp:Label>
                                        <asp:Label ID="ShipZipCode" runat="server" Text='<%# Eval("ShipZipCode") %>'></asp:Label>
                                        <br></br />
                                        <asp:Label ID="ShipCountryName" runat="server" Text='<%# Eval("ShipCountryName") %>'></asp:Label>
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                </asp:TemplateField>                     
                                <asp:TemplateField HeaderText="Patient Mailing Address">
                                    <ItemTemplate>
                                        <div>
                                            <p id="pAddress1" runat="server" style="text-indent:0px"><%# Eval("Address1") %></p>
                                            <p id="pAddress2" runat="server" style="text-indent:0px"><%# Eval("Address2") %></p>                
                                            <p id="pAddress3" runat="server" style="text-indent:0px"><%# Eval("Address3") %></p>   
                                            <asp:Label ID="City" runat="server" Text='<%# Eval("City") %>'></asp:Label> 
                                            <asp:Label ID="State" runat="server" Text='<%# Eval("State") %>'></asp:Label>
                                            <asp:Label ID="ZipCode" runat="server" Text='<%# Eval("ZipCode") %>'></asp:Label>
                                            <br></br />
                                            <asp:Label ID="CountryName" runat="server" Text='<%# Eval("CountryName") %>'></asp:Label>
                                            <asp:Label ID="lblIsValid" runat="server" CssClass="lblIsValid" ClientIDMode="Static"  ></asp:Label>
                                        </div>                                                                    
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Address Actions">
                                    <ItemTemplate>

                                        <div class="w3-third">
                                         <asp:CheckBox ID="chkUseMailingAddress" runat="server" ToolTip="Select to use the patient's mailing address instead of the order address."
                                            CausesValidation="false" ClientIDMode="Static">                
                                        </asp:CheckBox>                                     
                                        </div>
                                        <div class="w3-third">
                                         <asp:ImageButton ID="btnEdit" runat="server" OnClick="grdAddressesUpdate_Click" ClientIDMode="Static"
                                             ImageUrl="~/Styles/images/img_edit_pencil.png" ToolTip="Edit patient mailing address" Width="20px" Height="20px" CommandName="Modify"
                                             CommandArgument='<%#Eval("OrderNumber")+","+ Eval("PatientId")%>'/>
                                        </div>
                                        <div class="w3-third">
                                        <asp:ImageButton ID="btnRemove" runat="server" 
                                            ImageUrl="~/Styles/images/img_delete_x.png" ToolTip="Remove order address from grid" Width="15px" Height="15px"
                                             OnClick="grdAddressesUpdate_Click" CommandName="Delete" CommandArgument='<%#Eval("OrderNumber")+","+ Eval("PatientId")%>'/>
                                        </div>
                                           <%--<div class="w3-row" id="divAddressToolTip" style="visibility: visible">
                                           <div class="BeigeBoxContainer shadow" style="">
                                                <div style="background-color: #fff">
                                                    <div class="BeigeBoxHeader" style="padding: 12px 10px 5px 15px"><span class='colorBlue'>Address Information</span></div>
                                                    <div class="BeigeBoxContent">
                                                        <div id="divAddressContent" class="content" style="padding: 5px 20px">test</div>
                                                    </div>
                                                </div>
                                            </div>
                                            </div>--%>
                                    </ItemTemplate>
                                 </asp:TemplateField>

                        </Columns>
                    </asp:GridView>

                 <%--Edit Mailing Address Modal --%>
                     <div id="EditDialogMessage" class="w3-modal" style="z-index: 30000">
                <div class="w3-modal-content">
                    <div class="w3-container">
                        <div class="EditDialogMessage">
                            <div class="BeigeBoxContainer shadow" style="">
                                <div style="background-color: #fff">
                                 <div class="BeigeBoxHeader" style="text-align:left;padding: 12px 10px 3px 15px">
                                <%--        <span class="label">Edit Mailing Address</span>--%>
                                     <div id="srtsMessageheader" class="header_info">
                                            <span onclick="document.getElementById('EditDialogMessage').style.display='none'"
                                                class="w3-closebtn">&times;</span>
                                            Edit Mailing Address
                                        </div>
                                 </div>
                                    <div class="BeigeBoxContent" style="margin-left: 10px; padding-top: 0px; min-height: 200px">
                                        <asp:UpdatePanel ID="upAddresses" runat="server" ChildrenAsTriggers="false" UpdateMode="Conditional">
                                            <ContentTemplate>
                                                <div id="divAddresses" style="clear: both;">
                                                       <div class="w3-row" style="width:535px">
                                                       <div class="w3-col" style="width:375px">
                                                       <asp:RadioButtonList ID="rblAddressType" runat="server" TabIndex="0" RepeatDirection="Horizontal" 
                                                           ToolTip="Select Address Type" CausesValidation="false" ClientIDMode="Static" CssClass="srtsLabel_medium" 
                                                           onchange="GetAddressValues();">
                                                                    <asp:ListItem Text="US Address" Value="US"/>
                                                                    <asp:ListItem Text="Foreign Address" Value="FN" />
                                                       </asp:RadioButtonList>  
                                                       </div>
                                                  
                                                    
                                                       <!-- UIC -->                                              
                                                       <div class="w3-rest" style="display:none">
                                                            <div id="divUIC" style="margin: 5px 0px 10px 0px;padding-right:30px;display:none">
                                                                <asp:Label ID="lblPrimaryUIC" runat="server" Text="UIC" CssClass="srtsLabel_medium" />
                                                                <asp:TextBox ID="tbPrimaryUIC" runat="server" CssClass="srtsTextBox_medium" Width="120px" />
                                                            </div>
                                                       </div>
                           
                                                      </div>




                                                    <div style="height:165px">
                                                    <!-- Address 1, Address 2 -->
                                                    <div class="w3-row">
                                                        <!-- Address 1, Address 2 -->
                                                        <div id="divAddress1" class="w3-half">
                                                            <!-- Address 1 -->
                                                            <div class="padding">
                                                                <asp:Label ID="lblPrimaryAddAddress1" runat="server" Text="Address 1" CssClass="srtsLabel_medium" /><br />
                                                                <asp:TextBox ID="tbPrimaryAddress1" runat="server" MaxLength="100" CssClass="srtsTextBox_medium" ClientIDMode="Static"
                                                                    ToolTip="Enter the patient house and street address." Width="220px" />
                                                               <%-- <asp:CustomValidator ID="cvPrimaryAddress1" runat="server" ClientIDMode="Static" ControlToValidate="tbPrimaryAddress1" ClientValidationFunction="ValidateAddress1" ValidateEmptyText="true" />--%>
                                                            </div>
                                                        </div>
                                                        <div id="divAddress2" class="w3-half">
                                                            <!-- Address 2 -->
                                                            <div  class="padding" style="margin-left:0px">
                                                                <asp:Label ID="lblPrimaryAddAddress2" runat="server" Text="Address 2" CssClass="srtsLabel_medium" /><br />
                                                                <asp:TextBox ID="tbPrimaryAddress2" runat="server" MaxLength="100" CssClass="srtsTextBox_medium" ClientIDMode="Static" ToolTip="Continuation of patient address." Width="220px" />
                                                               <%-- <asp:CustomValidator ID="cvPrimaryAddress2" runat="server" ControlToValidate="tbPrimaryAddress2" ClientValidationFunction="ValidateAddress2" ValidateEmptyText="true" />--%>
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <!-- City, State -->
                                                    <div id="divUsAddress">
                                                    <div class="w3-row" style="margin-right: 35px">
                                                        <!-- Zip -->
                                                        <div class="w3-third">
                                                            <!-- Zip -->
                                                            <div style="margin: 0px 0px 10px 0px;">
                                                                <!-- Zip -->
                                                                <div class="padding">
           <asp:Label ID="lblPrimaryZip" runat="server" Text="Zip" CssClass="srtsLabel_medium" /><br />
                                                                <asp:TextBox ID="tbPrimaryZipCode" runat="server" CssClass="srtsTextBox_medium" ClientIDMode="Static"
                                                                        ToolTip="Enter patient residence zip code" Width="150px" onkeydown = "return (event.keyCode!=13);" onchange="javascript:DoZipLookup()"  />
                                                                <%--<asp:CustomValidator ID="cvPrimaryZipCode" runat="server" ControlToValidate="tbPrimaryZipCode"
                                                                    ClientValidationFunction="ValidateZip" ValidateEmptyText="true" />--%>
                                                           
                                                                </div>
                                                            </div>
                                                        </div>

                                                        <!-- City -->
                                                        <div class="w3-third">
                                                            <div style="margin: 0px 0px 10px 0px;">
                                                                <!-- City -->
                                                                <div class="padding">
                                                                    <asp:Label ID="lblPrimaryCity" runat="server" Text="City" CssClass="srtsLabel_medium" /><br />
                                                                    <asp:TextBox ID="tbPrimaryCity" runat="server" MaxLength="100" CssClass="srtsTextBox_medium" ClientIDMode="Static" CausesValidation="true"
                                                                        onclick="DoTbCityClick();" onblur="DoTbCityBlur();" ToolTip="Enter city name from patient address" Width="150px" />
                                                                   <%-- <asp:CustomValidator ID="cvPrimaryCity" runat="server" ControlToValidate="tbPrimaryCity" ClientValidationFunction="ValidateCity" ValidateEmptyText="true" />--%>
                                                                </div>
                                                            </div>
                                                        </div>

                                                        <!-- APOCity -->
                                                        <div class="w3-third">
                                                            <div style="margin: 0px 0px 0px 20px;padding-top:20px">
                                                                &nbsp;&nbsp;<asp:RadioButtonList ID="rblCity" runat="server" TabIndex="8" RepeatDirection="Horizontal" ToolTip="Select Area"
                                                                    CausesValidation="true" ClientIDMode="Static" onchange="DoRblCityChange();">
                                                                    <asp:ListItem Text="APO" Value="APO" />
                                                                    <asp:ListItem Text="FPO" Value="FPO" />
                                                                    <asp:ListItem Text="DPO" Value="DPO" />
                                                                </asp:RadioButtonList>
                                                            </div>
                                                        </div>
                                                    </div>

                                           
                                                    </div>
                                                        <!-- State, Country -->
                                                        <div class="w3-row" style="padding-top:20px">
                                                        <!-- State -->
                                                         <div id="divState" class="w3-third">
                                                            <div class="padding" style="padding-top: 0px">
                                                                <asp:Label ID="lblPrimaryState" runat="server" Text="State" CssClass="srtsLabel_medium" /><br />
                                                                    <asp:DropDownList ID="ddlPrimaryState" runat="server" ToolTip="Select patient residence state."
                                                                         ClientIDMode="Static"
                                                                        DataTextField="Value" DataValueField="Key" Width="160px">
                                                                    </asp:DropDownList>
                                                                   <%-- <asp:CustomValidator ID="cvPrimaryState" runat="server" ControlToValidate="ddlPrimaryState" ClientValidationFunction="ValidateState" ValidateEmptyText="true" />--%>
                                                            </div>
                                                        </div>

                                                        <!-- Country -->
                                                        <div class="w3-third">
                                                            <div class="padding" style="padding-top: 0px">
                                                                <asp:Label ID="lblPrimaryCountry" runat="server" Text="Country" CssClass="srtsLabel_medium" /><br />
                                                                <asp:DropDownList ID="ddlPrimaryCountry" runat="server" ToolTip="Select patient residence country."
                                                                    ClientIDMode="Static" onchange="DoDdlCountryChange();" DataTextField="Text" DataValueField="Value" Width="325px">
                                                                </asp:DropDownList>
                                                            </div>
                                                        </div>
                                         
                                                        <div class="w3-third"></div>
                                                        </div>
                                  
                                         
                                                    </div>

                                         
                                          
                                                </div>
                                            <%--    Validation Messages--%>
                                                <div class="w3-row" style="text-align:left">
                                                <div class="w3-col" style="width:300px">
                                                 <div id="divAddressMsg" style="color:red;width:90%;text-align:left;padding-left:20px"></div>
                                                 <div style="height: auto"><div id="addressMessage"></div></div>
                                                </div>
                                                    <div class="w3-rest" style="float:right">
                                                    <!-- Address - Save Button -->
                                                    <div class="padding" style="text-align: right">
                                                        <asp:Button ID="bSaveAddress" runat="server" CssClass="srtsButton" CausesValidation="false" Text="Validate" 
                                                            OnClientClick="return IsValidAddress();" OnClick  ="bSaveAddress_Click"  Enabled="true" />
                                                    </div>

                                                    </div>
                                                    </div>
                                            </ContentTemplate>
                                            <Triggers>
                                                <asp:AsyncPostBackTrigger ControlID="bSaveAddress" EventName="click" />
                                                <asp:AsyncPostBackTrigger ControlID="btnSaveVerifiedAddress" EventName="click" />
                                            </Triggers>
                                        </asp:UpdatePanel>
                                    </div> 
                                            
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>                 
                        
               <%--Address Verification Modal --%>
               <%-- ///////////////////////////////////////////////////////////////////--%>
               <div id="AddressVerificationDialog" class="w3-modal" style="z-index: 30000">
                <div class="w3-modal-content">
                    <div class="w3-container">
                        <div class="AddressVerificationDialog">
                            <div class="BeigeBoxContainer shadow" style="width:550px">
                                <div style="background-color: #fff">
                                 <div class="BeigeBoxHeader" style="text-align:left;padding: 12px 10px 3px 15px">
                                     <div id="AddressVerificationDialogheader" class="header_info">
                                            <span onclick="document.getElementById('AddressVerificationDialog').style.display='none'"
                                                class="w3-closebtn">&times;</span>
                                             <span class="label">Address Information</span> - Address Validation
                                        </div>
                                 </div>
                                    <div class="BeigeBoxContent" style="margin-left: 10px; padding-top: 0px; height: 430px">
                                        <asp:UpdatePanel ID="UpdatePanel2" runat="server" ChildrenAsTriggers="false" UpdateMode="Conditional">
                                            <ContentTemplate>  
                                                <div class="row padding">
                                                    <div id="divAddressMessage" class="header_info"><span style="text-align:left;font-size:smaller">The United States Postal service has found and returned the below address. </span></div>
                                                </div> 
                                            <div class="row padding" style="padding-left:15px;padding-top:0px;margin-left:50px;margin-top:-25px">
                                            <%-- Address as Entered--%>
                                            <div class="w3-col" style="width:50%">
                                                <div id="divAddressEntered" style="height:300px">
                                                    <div class="header_info">
                                                        <div class="rightArrow"><asp:ImageButton ID="btnSaveEnteredAddress" CommandName="SaveEnteredAddress" runat="server" ImageUrl="~/Styles/images/Arrow_blue_right.gif" Width="25px"
                                                    CausesValidation="false" OnClick="SaveAddress" ToolTip="I would like to use the Mailing Address as Entered." /></div>
                                                    <span style="font-size:smaller">&nbsp;&nbsp;&nbsp;Mailing Address as Entered</span>
                                                    </div>
                                                    <div>
                                                        <!-- Address 1, Address 2 -->
                                                        <div class="w3-row">
                                                            <!-- Address 1, Address 2 -->
                                                   
                                                                <!-- Address 1 -->
                                                                <div class="padding">
                                                                    <asp:Label ID="lblAddress1" runat="server" Text="Address 1" CssClass="srtsLabel_medium" /><br />
                                                                    <asp:TextBox ID="txtAddress1" runat="server" MaxLength="100" CssClass="srtsTextBox_medium" 
                                                                        ClientIDMode="Static" Width="255px" ReadOnly="true" />                                                
                                                                </div>
                                                            
                                                                <!-- Address 2 -->
                                                                <div  class="padding">
                                                                    <asp:Label ID="lblAddress2" runat="server" Text="Address 2" CssClass="srtsLabel_medium" /><br />
                                                                    <asp:TextBox ID="txtAddress2" runat="server" MaxLength="100" CssClass="srtsTextBox_medium" 
                                                                        ClientIDMode="Static" width="255px" ReadOnly="true" />        
                                                                </div>
                                                        
                                                        </div>

                                                        <!-- City, State -->
                                                        <div id="div6">
                                                        <div class="w3-row">
                                                            <!-- City -->
                                                            <div class="w3-half">
                                                                <div style="margin: 0px 0px 10px 0px;">
                                                                    <!-- City -->
                                                                    <div class="padding">
                                                                        <asp:Label ID="lblCity" runat="server" Text="City" CssClass="srtsLabel_medium" /><br />
                                                                        <asp:TextBox ID="txtCity" runat="server" MaxLength="100" CssClass="srtsTextBox_medium"
                                                                             ClientIDMode="Static" Width="140px" ReadOnly="true" />        
                                                                    </div>
                                                                </div>
                                                            </div> 
                                                            <!-- Zip -->
                                                            <div class="w3-half">
                                                                <!-- Zip -->
                                                                <div style="margin: 0px 0px 10px 30px;">
                                                                    <!-- Zip -->
                                                                    <div class="padding">
                                                                    <asp:Label ID="lblZipCode" runat="server" Text="Zip" CssClass="srtsLabel_medium" /><br />
                                                                    <asp:TextBox ID="txtZipCode" runat="server" CssClass="srtsTextBox_medium" ClientIDMode="Static" Width="75px" ReadOnly="true" />        
                                                                    </div>
                                                                </div>
                                                            </div>

                                                                                                             
                                                        </div>

                                           
                                                        </div>
                                                        <!-- State, Country -->
                                                        <div class="w3-row" style="padding-top:20px">
                                                        <!-- State -->
                                                            <div id="div7" class="w3-half">
                                                            <div class="padding" style="padding-top: 0px">
                                                                <asp:Label ID="lblState" runat="server" Text="State" CssClass="srtsLabel_medium" /><br />
                                                                    <asp:TextBox ID="txtState" runat="server" MaxLength="100" CssClass="srtsTextBox_medium"
                                                                             ClientIDMode="Static" Width="120px" ReadOnly="true" />        
                                                                   
                                                            </div>
                                                        </div>

                                                        <!-- Country -->
                                                        <div class="w3-half">
                                                            <div class="padding" style="margin: 0px 0px 0px 10px; padding-top: 0px">
                                                                <asp:Label ID="lblCountry" runat="server" Text="Country" CssClass="srtsLabel_medium" /><br />
                                                                <asp:TextBox ID="txtCountry" runat="server" MaxLength="100" CssClass="srtsTextBox_medium"
                                                                             ClientIDMode="Static" Width="96px" ReadOnly="true" />        
                                                            </div>
                                                        </div>
                              
                                                        </div>
                                  
                                         
                                                        </div>
                                                </div>
                                            </div>

                  
                                             
                                            <%--   Verified Address--%>
                                            <div class="w3-col" style="width:50%">
                                                <div id="divAddressVerified">
                                                <div class="header_info">
                                                   
                                                </div>
                                              
                                                        <!-- Address 1, Address 2 -->
                                                        <div class="w3-row" style="width:400px">
                                                            <!-- Address 1, Address 2 -->
                                                   
                                                                <!-- Address 1 -->
                                                                <div class="padding">
                                                                    <asp:Label ID="lblAddress1Verified" runat="server" Text="Address 1" CssClass="srtsLabel_medium" /><br />
                                                                    <asp:TextBox ID="txtAddress1Verified" runat="server" MaxLength="100" CssClass="srtsTextBox_medium" 
                                                                        ClientIDMode="Static" Width="302px" ReadOnly="true" />                                                    
                                                                </div>
                                                            
                                                                <!-- Address 2 -->
                                                                <div  class="padding">
                                                                    <asp:Label ID="lblAddress2Verified" runat="server" Text="Address 2" CssClass="srtsLabel_medium" /><br />
                                                                    <asp:TextBox ID="txtAddress2Verified" runat="server" MaxLength="100" CssClass="srtsTextBox_medium" 
                                                                        ClientIDMode="Static" Width="302px" ReadOnly="true" />        
                                                                </div>
                                                        
                                                        </div>

                                                        <!-- City, State -->
                                                   
                                                        <div class="w3-row" style="width:400px">
                                                            <!-- City -->
                                                            <div class="w3-half">
                                                                <div style="margin: 0px 0px 10px 0px;">
                                                                    <!-- City -->
                                                                    <div class="padding">
                                                                        <asp:Label ID="lblCityVerified" runat="server" Text="City" CssClass="srtsLabel_medium" /><br />
                                                                        <asp:TextBox ID="txtCityVerified" runat="server" MaxLength="135" CssClass="srtsTextBox_medium"
                                                                             ClientIDMode="Static" Width="150px" ReadOnly="true" />        
                                                                    </div>
                                                                </div>
                                                            </div>     
                                                            <!-- Zip -->
                                                            <div class="w3-half" style="text-align:right">
                                                                <!-- Zip -->
                                                                <div style="margin: 0px 0px 10px 10px;">
                                                                    <!-- Zip -->
                                                                    <div class="padding">
                                                                    <asp:Label ID="lblZipCodeVerified" runat="server" Text="Zip" CssClass="srtsLabel_medium" /><br />
                                                                    <asp:TextBox ID="txtZipCodeVerified" runat="server" CssClass="srtsTextBox_medium" ClientIDMode="Static" Width="95px" ReadOnly="true" />        
                                                                    </div>
                                                                </div>
                                                            </div>                                             
                                                        </div>

                                           
                                                    
                                                        <!-- State, Country -->
                                                        <div class="w3-row" style="padding-top:20px;width:400px">
                                                        <!-- State -->
                                                            <div id="div2" class="w3-half">
                                                            <div class="padding" style="padding-top: 0px">
                                                                <asp:Label ID="lblStateVerified" runat="server" Text="State" CssClass="srtsLabel_medium" /><br />
                                                                    <asp:TextBox ID="txtStateVerified" runat="server" MaxLength="100" CssClass="srtsTextBox_medium"
                                                                             ClientIDMode="Static" Width="170px" ReadOnly="true" />        
                                                            </div>
                                                            </div>

                                                        <!-- Country -->
                                                        <div class="w3-half">
                                                            <div class="padding" style="margin: 0px 0px 0px 10px; padding-top: 0px">
                                                                <asp:Label ID="lblCountryVerified" runat="server" Text="Country" CssClass="srtsLabel_medium" /><br />
                                                                <asp:TextBox ID="txtCountryVerified" runat="server" MaxLength="100" CssClass="srtsTextBox_medium"
                                                                             ClientIDMode="Static" Width="96px" ReadOnly="true" />        
                                                            </div>
                                                        </div>
                              
                                                        </div>
                                  
                                         
                                                      
                                                    <br /><br /><br />
                                                    <div class="w3-row" style="width:400px;text-align:right">
                                                        <asp:Button ID="btnSaveVerifiedAddress" runat="server" Text="Save" CssClass="srtsButton" CommandName="SaveVerifiedAddress" CausesValidation="false" 
                                                            OnClick="SaveAddress" ToolTip="I would like to use the Verified Mailing Address." />
                                                      <%--    <asp:Button ID="btnAddressVerify" runat="server" CssClass="srtsButton" CausesValidation="false" Text="Validate"
                                                        OnClientClick="return IsValidAddress();" OnClick="bSaveAddress_Click" Enabled="true" ClientIDMode="Static" />--%>
                                                   <asp:Button ID="btnSaveVerifiedAddress_Cancel" runat="server" Text="Cancel" CssClass="srtsButton" OnClick="btnCancelAddressSave_Click" 
                                                         Enabled="true" ClientIDMode="Static" UseSubmitBehavior="false" />
                                                    </div>

                                                </div>
                                            </div>
                                            </div>
                                                 <div id="divAddressSubmit">
                                                      <div style="text-align:center">
                                                         <asp:Button ID="btnAddressSave" runat="server" Text="Save Address as Entered" CssClass="srtsButton" OnClick="SaveAddress" CausesValidation="false" CommandName="SaveEnteredAddress" />                                                               <br />
                                                            (Please note:  The address will only remain valid<br /> for a period of 30 days.)
                                                     </div>
                         
                                                    

                                                     <br /><br />
                                                        <div style="text-align:center">
                                                         <asp:Button ID="btnCancelAddressSave" runat="server" Text="Cancel" CssClass="srtsButton" OnClick="btnCancelAddressSave_Click" 
                                                         Enabled="true" ClientIDMode="Static" UseSubmitBehavior="false" CausesValidation="false" /><br />
                                                         Edit the address and try the validation again.
                                                        </div>

                                                 </div>
                                            </ContentTemplate>
                                            <Triggers>
                                                <asp:AsyncPostBackTrigger ControlID="btnSaveVerifiedAddress" EventName="click" />
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
                
                    </ContentTemplate>
                    </asp:UpdatePanel>

                     

                    <asp:HiddenField ID="hdfBulkOrders" runat="server" ClientIDMode="Static" />
                    <asp:HiddenField ID="hdfIsValid" runat="server" ClientIDMode="Static" />
                    <asp:HiddenField ID="hdfState" runat="server" ClientIDMode="Static" />
                    <asp:HiddenField ID="hdfCity" runat="server" ClientIDMode="Static" />
                    <asp:HiddenField ID="HiddenField1" runat="server" ClientIDMode="Static" />
                    <asp:HiddenField ID="hdfDateVerified" runat="server" ClientIDMode="Static" />
                    <asp:HiddenField ID="hdfVerifiedExpiry" runat="server" ClientIDMode="Static" />







        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="btnAddToGrid" EventName="click" />
            <asp:AsyncPostBackTrigger ControlID="gvOrderAddresses" EventName="RowCommand" /> 
            <asp:AsyncPostBackTrigger ControlID="ddlHistoryItems" EventName="SelectedIndexChanged" />   

        </Triggers>
        </asp:UpdatePanel>

        </div>
      
        </div>


     </div>







        <asp:ScriptManagerProxy ID="smpProxy" runat="server">
        <Scripts>
<%--           <asp:ScriptReference Path="~/Scripts/Person/PersonDetails.js" />
            <asp:ScriptReference Path="~/Scripts/Person/PersonDetailVal.js" />--%>
<%--           --%>
      <%--      <asp:ScriptReference Path="~/Scripts/Global/GlobalVal.js" />--%>
<%--            --%>
            <asp:ScriptReference Path="~/Scripts/Global/SharedAddress.js" />
            <asp:ScriptReference Path="~/Scripts/ManageReports/PrintShippingLabels.js" />
        <%--    <asp:ScriptReference Path="~/Scripts/Global/SrtsCustomValidators.js" />--%>
           
<%-- <asp:ScriptReference Path="~/Scripts/Global/PassFailConfirm.js" />--%>

        </Scripts>
    </asp:ScriptManagerProxy>

</asp:Content>
