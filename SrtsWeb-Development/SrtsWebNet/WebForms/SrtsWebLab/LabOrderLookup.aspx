<%@ Page Title="" Language="C#" MasterPageFile="~/srtsMaster.Master" AutoEventWireup="True" EnableViewState="true" EnableEventValidation="false" CodeBehind="LabOrderLookup.aspx.cs" Inherits="SrtsWebLab.LabOrderLookup" %>

<%@ MasterType VirtualPath="~/srtsMaster.Master" %>

<asp:Content ContentPlaceHolderID="HeadContent" ID="headContent" runat="server">
   <style>
        .hdr_searchResults {
        float:left;
        position:relative;
        top: 20px;
        left: 40px;
        color:#0f621e;
        margin:0px;
        padding:0px;
        font-size:12px;
        }
        #selectOrderInformation li {
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

        #selectOrderInformation li.active {
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

        #selectOrderInformation li a {
                text-decoration: none;
                padding-left: 5px;
            }

        .defaultPanels {
            margin: 0px 0px;
            padding: 5px;        }

        .alignLeft {
            text-align: left;
        }

    </style>
</asp:Content>
<asp:Content ID="contentLabName" ContentPlaceHolderID="ContentTop_Title_Right" runat="server">
    <div style="position: relative; top: -20px; float: right;" class="patientnameheader_Patient">
        <span id="spanWrapper">
            <asp:Literal ID="litContentTop_Title_Right" runat="server"></asp:Literal>
        </span>
    </div>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    
     <div style="min-width: 1096px;margin-top:20px">
        <div class="w3-row">
                  <!--Search Buttons -->
            <asp:Panel ID="pnlSearch" runat="server" DefaultButton="btnSearchGlobal">
                <div class="headerBlue" style="font-size:14px;height:45px;padding:0px;text-align:left;margin:0px 0px 0px 40px">Enter a search criteria below to search for order detail information. Search by patient name, ID/SSN number, or order number.</div>                       
                <div style="margin: 10px 0px 0px 40px">
                    <div class="w3-row" style="text-align:left">
                    <div class="w3-col l5">
                        <div class="w3-col l4">
                            <span class="srtsLabel_medium" style="margin-left:0px;">Last Name:</span><br />
                            <asp:TextBox ID="tbLastName" runat="server" MaxLength="25" Width="110px" CssClass="srtsTextBox_small"
                                ToolTip="Enter as much of the patients last name as you know." TabIndex="1" ClientIDMode="Static"></asp:TextBox><br />
                            <ajaxToolkit:FilteredTextBoxExtender ID="ftb_tbLastName" runat="server" FilterType="Custom, UppercaseLetters, LowercaseLetters" ValidChars="'- " TargetControlID="tbLastName" Enabled="True" />
                        </div>
                        <div class="w3-col l4">
                            <span class="srtsLabel_medium" style="margin-left:0px;">First Name:</span><br />
                            <asp:TextBox ID="tbFirstName" runat="server" MaxLength="25" Width="110px" CssClass="srtsTextBox_small"
                                ToolTip="Optional Enter as much of the patients first name as you know." TabIndex="2"></asp:TextBox>
                           
                             <br />
                            <ajaxToolkit:FilteredTextBoxExtender ID="ftb_tbFirstName" runat="server" FilterType="Custom, UppercaseLetters, LowercaseLetters" ValidChars="'- " TargetControlID="tbFirstName" Enabled="True" />
                        </div>
                        <div class="w3-col l4" style="padding-top:7px;margin-left:-20px">
                                <asp:Button ID="btnSearchGlobal" runat="server" CssClass="srtsButton"  ClientIDMode="Static" OnClientClick="return SearchNameHasText($('#tbLastName').val());" 
                                    OnCommand="rbSearch_Click" CommandArgument="G" CausesValidation="False" Text="Name Search" />   
                        </div>
                    </div> 
                    <div class="w3-col l7">
                         <div class="w3-col l5"  style="padding-left:10px">
                            <span class="srtsLabel_medium" style="margin-left:0px;">ID/SSN Number or Order Number:</span><br />
                            <asp:TextBox ID="tbID" runat="server" MaxLength="16" ToolTip="Enter the patients full ID, SSN or the last four digits of the users SSN or an order number"
                                TabIndex="3" CssClass="srtsTextBox_small" Width="200px" onkeypress="return GetKeyCode(event)" ClientIDMode="Static"></asp:TextBox><br />
                            <ajaxToolkit:FilteredTextBoxExtender ID="ftb_tbID" runat="server" FilterType="Custom, Numbers" ValidChars="-" TargetControlID="tbID" Enabled="True" />
                        </div>
                        <div class="w3-col l3" style="padding-top:6px;margin-left:-25px">
                            <asp:Button ID="bQuickSearch" runat="server" Text="Number Search" CssClass="srtsButton" ClientIDMode="Static" OnClientClick="return QuickSearchHasText($('#tbID').val());" OnClick="bQuickSearch_Click" />
                        </div>
                        <div class="w3-col l2" style="padding-top:6px">
                            <asp:Button ID="btnClear" runat="server" CssClass="srtsButton clear" ToolTip="Remove All Grid Items." Text="Clear All" 
                           OnClick="clearGrids_Click" ClientIDMode="Static" /> 
                             <asp:Button ID="btnEnter" runat="server" Text="" OnClick="bQuickSearch_Click" ClientIDMode="Static" BackColor="Transparent" BorderColor="Transparent" /> 
                           
                            <%-- <div class="w3-col padding" style="width:15%">
                              <span class="srtsLabel_medium" style="margin-left:0px;"> Order Date:</span><br />
                            <asp:TextBox ID="tbOrderDate" runat="server" CssClass="srtsTextBox_medium" Width="110px" TabIndex="5"></asp:TextBox><br />       
                         </div>--%>
                        </div>
                    </div> 
                    </div>
                        
              <asp:Literal ID="litPageMessage" runat="server" Visible="False"></asp:Literal>   
               </div>
            </asp:Panel>
            <div id="srtsMessage"></div>
            </div>
        <div class="w3-row">
                        <h1 class="w3-large" style="position:relative;top:0px;left:0px;right:60px;padding: 0px 50px 0px 5px; text-align: right;color:#0f621e;font-size:1em;font-style:italic">
                            <asp:Literal ID="litPatientName" runat="server" /></h1>
                <div id="divSearchResults" runat="server" style="clear: both; width: 100%; margin: -20px auto 0px auto; text-align: left;" visible="False">
           <div class="padding" style="padding-right:0px;height:400px;overflow-y:auto;width:1000px;margin:0px 0px 0px 15px">
                          
                <asp:GridView ID="gvSearch" runat="server" ClientIDMode="Static" AllowSorting="True" AllowPaging="false"
                    AutoGenerateColumns="False" GridLines="None" DataKeyNames="ID" OnSorting="gvSearch_Sorting"
                     Width="100%" ViewStateMode="Enabled"
                    CssClass="mGrid" AlternatingRowStyle-CssClass="alt" ShowHeaderWhenEmpty="false"
                    OnRowDataBound="gvSearch_RowDataBound"
                    OnRowCreated="gvSearch_RowCreated"
                    OnRowCommand="gvSearch_RowCommand">
                    <Columns>
                        <asp:ButtonField CommandName="Orders" ButtonType="Button" Text="Orders" Visible="false">
                            <ItemStyle Width="50px" />
                        </asp:ButtonField>
                        <asp:BoundField DataField="LastName" HeaderText="Last Name" SortExpression="LastName">
                            <HeaderStyle HorizontalAlign="Left" />
                            <ItemStyle Width="150px" />
                        </asp:BoundField>
                        <asp:BoundField DataField="FirstName" HeaderText="First Name" SortExpression="FirstName">
                            <HeaderStyle HorizontalAlign="Left" />
                            <ItemStyle Width="150px" />
                        </asp:BoundField>
                        <asp:BoundField DataField="IDNumber" HeaderText="ID Number(s) *Last Four" SortExpression="IDNumber">
                            <HeaderStyle HorizontalAlign="Left" />
                            <ItemStyle Width="100px" HorizontalAlign="Center" />
                        </asp:BoundField>
                          <asp:BoundField DataField="SiteCodeID" HeaderText="Clinic ID" SortExpression="SiteCodeID">
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle Width="50px" HorizontalAlign="Center" />
                        </asp:BoundField>
                    </Columns>
                </asp:GridView>
           </div>
        </div>

              <div id="divLab" runat="server" visible="false" class="padding" style="margin:0px 0px 0px 15px">
                <div style="height:400px; overflow-y:auto">
                <asp:GridView ID="gvLabOrders" runat="server" AutoGenerateColumns="false" DataKeyNames="OrderNumber" 
                    OnRowDataBound="gvLabOrders_RowDataBound" CssClass="mGrid" AlternatingRowStyle-CssClass="alt" GridLines="none">
                    <Columns>
                        <asp:TemplateField HeaderText="Select">
                            <ItemTemplate>
                                 <asp:ImageButton ID="btnEdit" runat="server" OnClick="gvLabOrders_Click" ClientIDMode="Static"
                                             ImageUrl="~/Styles/images/img_edit_pencil.png" ToolTip="Select this Order Number" Width="20px" Height="20px" CommandName="Select"
                                             CommandArgument='<%#Eval("OrderNumber")%>'/>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="OrderNumber" HeaderText="Order Number" />
                        <asp:BoundField DataField="DateLastModified" HeaderText="Order Date" />
                        <asp:BoundField DataField="CurrentStatus" HeaderText="Current Status" />
                    </Columns>
                </asp:GridView>
                    </div>
            </div>
            
       </div>
    </div>               

    <div id="divOrderInformation" runat="server" ClientIDMode="Static" style="float:left;width:100%;margin: 10px 0px 20px 0px; padding-bottom: 0px; padding-top: 0px" visible="false">
        <div class="w3-row" style="margin: 0px;padding-right:40px">

            <div class="w3-col" style="width: 150px">
                <div style="font-size: 11px; padding: 0px 10px 20px 15px; text-align: left">Select an option below to view order information.</div>
                <%--Menu Options--%>
                <div id="selectOrderInformation">
                    <ul>
                        <li id="lstPatientInfo"><a href="#" id="lnkPatientInfo">Patient Information</a></li>
                        <li id="lstOrderInfo"><a href="#" id="lnkOrderInfo">Order Information</a></li>
                        <li id="lstOrderStatus"><a href="#" id="lnkOrderStatus">Status History</a></li>
                        <li id="lstPrescription" class="active"><a href="#" id="lnkPrescription">Prescription</a></li>
                    </ul>
                </div>
            </div>
 
            <div class="w3-rest" style="padding:0px;border-left: 1px solid #ebd9c7;border-top: 1px solid #ebd9c7;border-bottom: 1px solid #ebd9c7;border-right: 1px solid #ebd9c7;padding-top:20px;
                border-top-left-radius: 6px;border-bottom-left-radius: 6px;border-top-right-radius: 6px;border-bottom-right-radius: 6px;">
                
                
                <div id="pnlPatientInfo" style="height: auto; min-height:380px;display: block">
                    <h1 style="padding: 0px 0px 0px 20px; text-align: left" class="w3-large">Patient Information</h1>
                    <div class="defaultPanels">
                     <!-- Patient Information -->
                   <div style="margin:0px 0px 0px 20px">
                        <asp:GridView ID="gvPatientInfo" runat="server" ClientIDMode="Static" AutoGenerateColumns="false" GridLines="None" DataKeyNames="ID" OnRowDataBound="gvPatientInfo_RowDataBound"
                               Width="95%" ViewStateMode="Enabled" CssClass="mGrid" AlternatingRowStyle-CssClass="alt">
                        <Columns>
                        <asp:TemplateField HeaderText ="Patient Information">
                            <ItemTemplate>
                                <div class="w3-row padding" style="text-align:left">
                                <div class="w3-col l4">   <!-- Patient Name -->
                                        <asp:Label ID="lblPatientName" runat="server" Text="Patient Name" CssClass="srtsLabel_medium"></asp:Label><br />
                                        <asp:TextBox ID="txtPatientName" runat="server" CssClass="srtsTextBox" 
                                            Text='<%# string.Concat(Eval("FirstName"), " ", Eval("MiddleName"), ". ", Eval("LastName"))%>' Width="200" ReadOnly="true" 
                                            ClientIDMode="Static"></asp:TextBox><br />
                                       <%-- <asp:Label ID="lblPatientID" runat="server" Text="ID Number" CssClass="srtsLabel_medium"></asp:Label><br />
                                        <asp:TextBox ID="txtPatientID" runat="server" CssClass="srtsTextBox" 
                                            Text='<%# Eval("IDNumberDisplay")%>' Width="200" ReadOnly="true" 
                                            ClientIDMode="Static"></asp:TextBox><br />
                                        <asp:Label ID="lblNextFOC" runat="server" Text="Next FOC Date" CssClass="srtsLabel_medium"></asp:Label><br />
                                        <asp:TextBox ID="txtNextFOC" runat="server" CssClass="srtsTextBox" 
                                            Text='<%# Eval("NextFOCDate")%>' Width="200" ReadOnly="true" 
                                            ClientIDMode="Static"></asp:TextBox>--%>
                                </div>
                                <div class="w3-col l4">   <!-- Mailing Address -->
                                        <asp:Label ID="Label1" runat="server" Text="Mailing Address" CssClass="srtsLabel_medium"></asp:Label><br />
                                        <asp:TextBox ID="txtAddress1" runat="server" CssClass="srtsTextBox" 
                                            Text='<%# Eval("Address1")%>' Width="200" ReadOnly="true" ClientIDMode="Static"></asp:TextBox><br />
                                        <asp:TextBox ID="txtAddress2" runat="server" CssClass="srtsTextBox" Text='<%# Eval("Address2")%>' 
                                            Width="200" ReadOnly="true" ClientIDMode="Static"></asp:TextBox> <br />
                                       <asp:TextBox ID="txtCityStateZip" runat="server" CssClass="srtsTextBox" 
                                           Text='<%# string.Concat(Eval("City"), " ", Eval("State"),  ", ", Eval("ZipCode"))%>' Width="200" ReadOnly="true" 
                                            ClientIDMode="Static"></asp:TextBox>
                                     <asp:TextBox ID="txtCountry" runat="server" CssClass="srtsTextBox" 
                                            Text='<%# Eval("CountryName")%>' Width="200" ReadOnly="true" 
                                            ClientIDMode="Static"></asp:TextBox>
                                </div>
                                <div class="w3-col l4">   <!-- Order Address -->
                                        <asp:Label ID="lblOrderAddress" runat="server" Text="Order Address" CssClass="srtsLabel_medium"></asp:Label><br />
                                        <asp:TextBox ID="txtShipAddress1" runat="server" CssClass="srtsTextBox" Text='<%# Eval("ShipAddress1")%>' 
                                            Width="200" ReadOnly="true" ClientIDMode="Static"></asp:TextBox><br />
                                     <asp:TextBox ID="txtShipAddress2" runat="server" CssClass="srtsTextBox" Text='<%# Eval("ShipAddress2")%>' Width="200" ReadOnly="true" 
                                            ClientIDMode="Static"></asp:TextBox><br />
                                       <asp:TextBox ID="txtShipCityStateZip" runat="server" CssClass="srtsTextBox" 
                                            Text='<%# string.Concat(Eval("ShipCity"), " ", Eval("ShipState"),  ", ", Eval("ShipZipCode"))%>' Width="200" ReadOnly="true" 
                                            ClientIDMode="Static"></asp:TextBox>
                                    <asp:TextBox ID="txtShipCountry" runat="server" CssClass="srtsTextBox" 
                                            Text='<%# Eval("ShipCountryName")%>' Width="200" ReadOnly="true" 
                                            ClientIDMode="Static"></asp:TextBox>
                                </div>
                                </div>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                        </asp:GridView>
                       </div>
                    </div>
                </div>
                               
                    <!-- Order Information -->
                <div id="pnlOrderInfo" style="height: auto;min-height:380px;display: block">
                    <h1 style="padding: 0px 0px 0px 20px; text-align: left" class="w3-large">Order Information</h1>
                    <div class="defaultPanels">
                   <div style="margin:0px 0px 0px 20px">
                        <asp:GridView ID="gvOrderInfo" runat="server" ClientIDMode="Static" AutoGenerateColumns="false" GridLines="None" DataKeyNames="OrderNumber" OnRowDataBound="gvOrderInfo_RowDataBound"
                               Width="95%" ViewStateMode="Enabled" CssClass="mGrid" AlternatingRowStyle-CssClass="alt">
                        <Columns>
                        <asp:TemplateField HeaderText ="Order Information">
                            <ItemTemplate>
                                <div class="w3-row padding" style="text-align:left">
                                    <div class="w3-col l2">   <!-- Order Number -->
                                        <asp:Label ID="lblOrderNumber" runat="server" Text="Order Number" CssClass="srtsLabel_medium"></asp:Label><br />
                                        <asp:TextBox ID="txtOrderNumber" runat="server" CssClass="srtsTextBox" Text='<%# Eval("OrderNumber") %>' Width="115" ReadOnly="true" 
                                            ClientIDMode="Static"></asp:TextBox>
                                    </div>
                                    <div class="w3-col l2">   <!-- Order Date -->
                                        <asp:Label ID="lblOrderDate" runat="server" Text="Order Date" CssClass="srtsLabel_medium"></asp:Label><br />
                                        <asp:TextBox ID="txtOrderDate" runat="server" CssClass="srtsTextBox" Text='<%# Eval("DateLastModified", "{0:d}") %>' Width="110" ReadOnly="true" 
                                            ClientIDMode="Static"></asp:TextBox>
                                    </div>
                                       <div class="w3-col l1">   <!-- Clinic Site Code -->
                                        <asp:Label ID="lblClinicCode" runat="server" Text="Clinic" CssClass="srtsLabel_medium"></asp:Label><br />
                                        <asp:TextBox ID="txtClinicCode" runat="server" CssClass="srtsTextBox" Text='<%# Eval("ClinicSiteCode") %>' Width="50" ReadOnly="true" 
                                            ClientIDMode="Static"></asp:TextBox>
                                    </div>
                                      <div class="w3-col l2">   <!-- Technician Name -->
                                        <asp:Label ID="lblTechnician" runat="server" Text="Technician" CssClass="srtsLabel_medium"></asp:Label><br />
                                        <asp:TextBox ID="txtTechnician" runat="server" CssClass="srtsTextBox" Text='<%# Eval("TechnicianName") %>' Width="115" ReadOnly="true" 
                                            ClientIDMode="Static"></asp:TextBox>
                                    </div>
                                    <div class="w3-col l5"> 
                                    <asp:Label ID="lblCurrentStatus" runat="server" Text="Current Status" CssClass="srtsLabel_medium"></asp:Label><br />
                                        <asp:TextBox ID="txtCurrentStatus" runat="server" CssClass="srtsTextBox" Text='<%# Eval("CurrentStatus") %>' Width="300" ReadOnly="true" 
                                            ClientIDMode="Static"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="w3-row padding" style="text-align:left">
                                    <div class="w3-col l1"> 
                                    <asp:Label ID="lblPairs" runat="server" Text="Pairs" CssClass="srtsLabel_medium"></asp:Label><br />
                                        <asp:TextBox ID="txtPairs" runat="server" CssClass="srtsTextBox" Text='<%# Eval("Pairs") %>' Width="45" ReadOnly="true" 
                                            ClientIDMode="Static"></asp:TextBox>
                                    </div>
                                    <div class="w3-col l3"> 
                                    <asp:Label ID="lblDispenseMethod" runat="server" Text="Dispense Method" CssClass="srtsLabel_medium"></asp:Label><br />
                                        <asp:TextBox ID="txtDispenseMethod" runat="server" CssClass="srtsTextBox" Text='<%# Eval("OrderDisbursement") %>' Width="175" ReadOnly="true" 
                                            ClientIDMode="Static"></asp:TextBox>
                                    </div>
                                    <div class="w3-col l8"> 
                                    <asp:Label ID="lblDispenseComments" runat="server" Text="Dispense Comments" CssClass="srtsLabel_medium"></asp:Label><br />
                                        <asp:TextBox ID="txtDispenseComments" runat="server" CssClass="srtsTextBox" Text='<%# Eval("DispenseComments") %>' Width="495" ReadOnly="true" 
                                            ClientIDMode="Static"></asp:TextBox>
                                    </div>                                 
                                  </div>
                                <div class="w3-row padding" style="text-align:left">
                                    <div class="w3-col l2"> 
                                    <asp:Label ID="lblPriority" runat="server" Text="Priority" CssClass="srtsLabel_medium"></asp:Label><br />
                                        <asp:TextBox ID="txtPriority" runat="server" CssClass="srtsTextBox" Text='<%# Eval("Priority") %>' Width="100" ReadOnly="true" 
                                            ClientIDMode="Static"></asp:TextBox>
                                    </div>
                                    <div class="w3-col l8"> 
                                    <asp:Label ID="lblFrame" runat="server" Text="Frame" CssClass="srtsLabel_medium"></asp:Label><br />
                                        <asp:TextBox ID="txtFrame" runat="server" CssClass="srtsTextBox" Text='<%# Eval("Frame") %>' Width="495" ReadOnly="true" 
                                            ClientIDMode="Static"></asp:TextBox>
                                    </div>
                                    <div class="w3-col l2"> 
                                    <asp:Label ID="lblProdLab" runat="server" Text="Prod Lab" CssClass="srtsLabel_medium"></asp:Label><br />
                                        <asp:TextBox ID="txtProdLab" runat="server" CssClass="srtsTextBox" Text='<%# Eval("LabSiteCode") %>' Width="105" ReadOnly="true" 
                                            ClientIDMode="Static"></asp:TextBox>
                                    </div>                                 
                                  </div>
                                <div class="w3-row padding" style="text-align:left">
                                    <div class="w3-col l2"> 
                                    <asp:Label ID="lblColor" runat="server" Text="Color" CssClass="srtsLabel_medium"></asp:Label><br />
                                        <asp:TextBox ID="txtColor" runat="server" CssClass="srtsTextBox" Text='<%# Eval("Color") %>' Width="100" ReadOnly="true" 
                                            ClientIDMode="Static"></asp:TextBox>
                                    </div>
                                    <div class="w3-col l2"> 
                                    <asp:Label ID="lblEye" runat="server" Text="Eye" CssClass="srtsLabel_medium"></asp:Label><br />
                                        <asp:TextBox ID="txtEye" runat="server" CssClass="srtsTextBox" Text='<%# Eval("Eye") %>' Width="100" ReadOnly="true" 
                                            ClientIDMode="Static"></asp:TextBox>
                                    </div>
                                    <div class="w3-col l2"> 
                                    <asp:Label ID="lblBridge" runat="server" Text="Bridge" CssClass="srtsLabel_medium"></asp:Label><br />
                                        <asp:TextBox ID="txtBridge" runat="server" CssClass="srtsTextBox" Text='<%# Eval("Bridge") %>' Width="100" ReadOnly="true" 
                                            ClientIDMode="Static"></asp:TextBox>
                                    </div> 
                                      <div class="w3-col l6"> 
                                    <asp:Label ID="lblTemple" runat="server" Text="Temple" CssClass="srtsLabel_medium"></asp:Label><br />
                                        <asp:TextBox ID="txtTemple" runat="server" CssClass="srtsTextBox" Text='<%# Eval("Temple") %>' Width="365" ReadOnly="true" 
                                            ClientIDMode="Static"></asp:TextBox>
                                    </div>                                 
                                  </div>
                                <div class="w3-row padding" style="text-align:left">
                                    <div class="w3-col l4"> 
                                    <asp:Label ID="lblLens" runat="server" Text="Lens" CssClass="srtsLabel_medium"></asp:Label><br />
                                        <asp:TextBox ID="txtLens" runat="server" CssClass="srtsTextBox" Text='<%# Eval("LensType") %>' Width="240" ReadOnly="true" 
                                            ClientIDMode="Static"></asp:TextBox>
                                    </div>
                                    <div class="w3-col l3"> 
                                    <asp:Label ID="lblTint" runat="server" Text="Tint" CssClass="srtsLabel_medium"></asp:Label><br />
                                        <asp:TextBox ID="txtTine" runat="server" CssClass="srtsTextBox" Text='<%# Eval("Tint") %>' Width="170" ReadOnly="true" 
                                            ClientIDMode="Static"></asp:TextBox>
                                    </div>
                                      <div class="w3-col l2"> 
                                    <asp:Label ID="lblCoating" runat="server" Text="Coating" CssClass="srtsLabel_medium"></asp:Label><br />
                                        <asp:TextBox ID="txtCoating" runat="server" CssClass="srtsTextBox" Text='<%# Eval("Coatings") %>' Width="100" ReadOnly="true" 
                                            ClientIDMode="Static"></asp:TextBox>
                                    </div>
                                    <div class="w3-col l3"> 
                                    <asp:Label ID="lblMaterial" runat="server" Text="Material" CssClass="srtsLabel_medium"></asp:Label><br />
                                        <asp:TextBox ID="txtMaterial" runat="server" CssClass="srtsTextBox" Text='<%# Eval("Material") %>' Width="170" ReadOnly="true" 
                                            ClientIDMode="Static"></asp:TextBox>
                                    </div>                                  
                                  </div>
                            </ItemTemplate>
                        </asp:TemplateField>
                        </Columns>
                        </asp:GridView>
                       </div>
                    </div>
                    </div>

                     <!-- Order Status History -->
                <div id="pnlOrderStatusHistory" style="height: auto; min-height:380px; display: block">
                    <h1 style="padding: 0px 0px 0px 20px; text-align: left" class="w3-large">Order Status History</h1>
                    <div class="defaultPanels">
                     <div style="margin:0px 0px 20px 20px; height:400px;overflow-y:auto">
                        <asp:GridView ID="gvOrderStatusHistory" runat="server" ClientIDMode="Static" AutoGenerateColumns="false" GridLines="None" DataKeyNames="ID" 
                            OnRowDataBound="gvOrderStatusHistory_RowDataBound"
                               Width="95%" ViewStateMode="Enabled" CssClass="mGrid">
                        <Columns>
                        <asp:TemplateField HeaderText ="Order Status History">
                            <ItemTemplate>
                                <div class="w3-row padding" style="text-align:left">
                                      <div class="w3-col l2">   <!-- Status Date -->
                                        <asp:Label ID="Label2" runat="server" Text="Order Date" CssClass="srtsLabel_medium"></asp:Label><br />
                                        <asp:TextBox ID="TextBox1" runat="server" CssClass="srtsTextBox" Text='<%# Eval("DateLastModified", "{0:d}") %>' Width="100" ReadOnly="true" 
                                            ClientIDMode="Static"></asp:TextBox>
                                    </div>
                                     <div class="w3-col l4">   <!-- Status Type -->
                                        <asp:Label ID="lblOrderStatusType" runat="server" Text="Status" CssClass="srtsLabel_medium"></asp:Label><br />
                                        <asp:TextBox ID="txtOrderStatusType" runat="server" CssClass="srtsTextBox" Text='<%# Eval("OrderStatusType") %>' Width="235" ReadOnly="true" 
                                            ClientIDMode="Static"></asp:TextBox>
                                    </div>
                                    <div class="w3-col l3">   <!-- Status Comment -->
                                        <asp:Label ID="lblStatusComment" runat="server" Text="Status Comment" CssClass="srtsLabel_medium"></asp:Label><br />
                                        <asp:TextBox ID="txtStatusComment" runat="server" CssClass="srtsTextBox" Text='<%# Eval("StatusComment") %>' Width="180" ReadOnly="true" 
                                            ClientIDMode="Static"></asp:TextBox>
                                    </div>
                                    
                                       <div class="w3-col l1">   <!-- Lab Site Code -->
                                        <asp:Label ID="lblLabStieCode" runat="server" Text="Clinic" CssClass="srtsLabel_medium"></asp:Label><br />
                                        <asp:TextBox ID="txtClinicCode" runat="server" CssClass="srtsTextBox" Text='<%# Eval("LabCode") %>' Width="50" ReadOnly="true" 
                                            ClientIDMode="Static"></asp:TextBox>
                                    </div>
                                      <div class="w3-col l2">   <!-- Technician Name -->
                                        <asp:Label ID="lblTechnician" runat="server" Text="Technician" CssClass="srtsLabel_medium"></asp:Label><br />
                                        <asp:TextBox ID="txtTechnician" runat="server" CssClass="srtsTextBox" Text='<%# Eval("ModifiedBy") %>' Width="120" ReadOnly="true" 
                                            ClientIDMode="Static"></asp:TextBox>
                                    </div>
                                    
                                </div>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                        </asp:GridView>
                       </div>
                    </div>
                    </div>

                    <!-- Order Prescription -->
                <div id="pnlOrderPrescription" style="height: auto; min-height:380px; display: block">
                    <h1 style="padding: 0px 0px 0px 20px; text-align: left" class="w3-large">Prescription</h1>
                    <div class="defaultPanels">
                   <div style="margin:0px 0px 0px 20px">
                           <asp:GridView ID="gvPrescription" runat="server" ClientIDMode="Static" AutoGenerateColumns="false" GridLines="None" DataKeyNames="ID" OnRowDataBound="gvPrescription_RowDataBound"
                                Width="95%" ViewStateMode="Enabled" CssClass="mGrid" AlternatingRowStyle-CssClass="alt" EmptyDataText="Prescription not found for this order." ShowHeaderWhenEmpty="true">
                    <Columns>
                        <asp:TemplateField HeaderText ="Prescription Information">
                            <ItemTemplate>
                                <div class="w3-row padding" style="margin-left:45px;text-align:left">
                                    <div class="w3-col l2">   <!-- Prescription Date -->
                                        <asp:Label ID="lblDate" runat="server" Text="Date" CssClass="srtsLabel_medium"></asp:Label><br />
                                        <asp:TextBox ID="txtDate" runat="server" CssClass="srtsTextBox_small" Text='<%# Eval("DateLastModified", "{0:d}") %>' Width="100" ReadOnly="true" ClientIDMode="Static"></asp:TextBox>
                                    </div>
                                     <div class="w3-col l3">   <!-- Prescription Provider -->
                                          <asp:Label ID="lblProvider" runat="server" Text="Provider" CssClass="srtsLabel_medium"></asp:Label><br />
                                        <asp:TextBox ID="txtProvider" runat="server" CssClass="srtsTextBox_small" Text='<%# Eval("ProviderID") %>' Width="155" ReadOnly="true" ClientIDMode="Static"></asp:TextBox>
                                    </div>
                                     <div class="w3-col l3" style="margin-top:-2px">   <!-- Prescription Type-->
                                            <asp:Label ID="lblType" runat="server" Text="Type" CssClass="srtsLabel_medium"></asp:Label><br />
                                            <asp:TextBox ID="txtType" runat="server" CssClass="srtsTextBox" Width="100" Text='<%# Eval("PrescriptionName") %>' ReadOnly="true" ClientIDMode="Static"></asp:TextBox>
                                         </div>
                                </div>

                                <!-- Prescription Values -->
                                <div class="w3-row padding">
                                   <div class="w3-col l2">
                                      <div style="padding:20px 20px 0px 0px;text-align:right">
                                          <asp:Label ID="lblRightOD" runat="server" Text="Right (OD)" CssClass="srtsLabel_medium"></asp:Label><br /><br />
                                          <asp:Label ID="lblLeftOS" runat="server" Text="Left (OS)" CssClass="srtsLabel_medium"></asp:Label>
                                      </div>
                                   </div>
                                   <div class="w3-col l2">
                                       <asp:Label ID="lblSph" runat="server" Text="Sph" CssClass="srtsLabel_medium"></asp:Label><br />
                                       <asp:TextBox ID="txtSphRight" runat="server" CssClass="srtsTextBox" Width="100" Text='<%# Eval("OdSphere") %>' ReadOnly="true" ClientIDMode="Static"></asp:TextBox>
                                       <asp:TextBox ID="txtSphLeft" runat="server" CssClass="srtsTextBox" Width="100" Text='<%# Eval("OsSphere") %>' ReadOnly="true" ClientIDMode="Static"></asp:TextBox>
                                   </div>
                                    <div class="w3-col l2">
                                       <asp:Label ID="lblCyl" runat="server" Text="Cyl" CssClass="srtsLabel_medium"></asp:Label><br />
                                       <asp:TextBox ID="txtCylRight" runat="server" CssClass="srtsTextBox" Width="100" Text='<%# Eval("OdCylinder") %>' ReadOnly="true" ClientIDMode="Static"></asp:TextBox>
                                       <asp:TextBox ID="txtCylLeft" runat="server" CssClass="srtsTextBox" Width="100" Text='<%# Eval("OsCylinder") %>' ReadOnly="true" ClientIDMode="Static"></asp:TextBox>
                                   </div>
                                    <div class="w3-col l2">
                                       <asp:Label ID="lblAxis" runat="server" Text="Axis" CssClass="srtsLabel_medium"></asp:Label><br />
                                       <asp:TextBox ID="txtAxisRight" runat="server" CssClass="srtsTextBox" Width="100" Text='<%# Eval("OdAxis") %>' ReadOnly="true" ClientIDMode="Static"></asp:TextBox>
                                       <asp:TextBox ID="txtAxisLeft" runat="server" CssClass="srtsTextBox" Width="100" Text='<%# Eval("OsAxis") %>' ReadOnly="true" ClientIDMode="Static"></asp:TextBox>
                                   </div>
                                    <div class="w3-col l2">
                                       <asp:Label ID="lblAdd" runat="server" Text="Add" CssClass="srtsLabel_medium"></asp:Label><br />
                                       <asp:TextBox ID="txtAddRight" runat="server" CssClass="srtsTextBox" Width="100" Text='<%# Eval("OdAdd") %>' ReadOnly="true" ClientIDMode="Static"></asp:TextBox>
                                       <asp:TextBox ID="txtAddLeft" runat="server" CssClass="srtsTextBox" Width="100" Text='<%# Eval("OsAdd") %>' ReadOnly="true" ClientIDMode="Static"></asp:TextBox>
                                   </div>
                                    <div class="w3-col l1">
                                       <asp:Label ID="lblPD" runat="server" Text="PD" CssClass="srtsLabel_medium"></asp:Label><br />
                                       <asp:TextBox ID="txtOdPd" runat="server" CssClass="srtsTextBox" Width="50" Text='<%# Eval("OdPdDist") %>' ReadOnly="true" ClientIDMode="Static"></asp:TextBox>
                                        <asp:TextBox ID="txtOsPd" runat="server" CssClass="srtsTextBox" Width="50" Text='<%# Eval("OsPdDist") %>' ReadOnly="true" ClientIDMode="Static"></asp:TextBox>
                                   </div>
                                    <div class="w3-col l1">
                                       <asp:Label ID="lblNearPd" runat="server" Text="Near PD" CssClass="srtsLabel_medium"></asp:Label><br />
                                       <asp:TextBox ID="txtOdNearPd" runat="server" CssClass="srtsTextBox" Width="50" Text='<%# Eval("OdPdNear") %>' ReadOnly="true" ClientIDMode="Static"></asp:TextBox>
                                       <asp:TextBox ID="txtOsNearPd" runat="server" CssClass="srtsTextBox" Width="50" Text='<%# Eval("OsPdNear") %>' ReadOnly="true" ClientIDMode="Static"></asp:TextBox>
                                   </div>
                                </div>


                                <!-- Prescription Prism Values -->
                                 <div class="w3-row padding">
                                     <div id="divPrismValues" runat="server" style="padding: 0px 0px 20px 75px">
                                         <asp:Label ID="lblPrismValues" runat="server" CssClass="srtsLabel_medium_text" Text="Prism Values"></asp:Label>
                                     </div>
                                   <div class="w3-col l2">
                                      <div style="padding:20px 20px 0px 0px;text-align:right">
                                          <asp:Label ID="lblPrismODRight" runat="server" Text="Right (OD)" CssClass="srtsLabel_medium"></asp:Label><br /><br />
                                          <asp:Label ID="lblPrismOSLeft" runat="server" Text="Left (OS)" CssClass="srtsLabel_medium"></asp:Label>
                                      </div>
                                   </div>
                                   <div class="w3-col l2">
                                       <asp:Label ID="lblHPrism" runat="server" Text="H-Prism" CssClass="srtsLabel_medium"></asp:Label><br />
                                       <asp:TextBox ID="txtHPrismRight" runat="server" CssClass="srtsTextBox" Width="100" Text='<%# Eval("OdHPrism") %>' ReadOnly="true" ClientIDMode="Static"></asp:TextBox>
                                       <asp:TextBox ID="txtHPrismLeft" runat="server" CssClass="srtsTextBox" Width="100" Text='<%# Eval("OsHPrism") %>' ReadOnly="true" ClientIDMode="Static"></asp:TextBox>
                                   </div>
                                    <div class="w3-col l2">
                                       <asp:Label ID="lblBase" runat="server" Text="H-Base" CssClass="srtsLabel_medium"></asp:Label><br />
                                       <asp:TextBox ID="txtHBaseRight" runat="server" CssClass="srtsTextBox" Width="100" Text='<%# Eval("OdHBase") %>' ReadOnly="true" ClientIDMode="Static"></asp:TextBox>
                                       <asp:TextBox ID="txtHBaseLeft" runat="server" CssClass="srtsTextBox" Width="100" Text='<%# Eval("OsHBase") %>' ReadOnly="true" ClientIDMode="Static"></asp:TextBox>
                                   </div>
                                    <div class="w3-col l2">
                                       <asp:Label ID="lblVPrism" runat="server" Text="V-Prism" CssClass="srtsLabel_medium"></asp:Label><br />
                                       <asp:TextBox ID="txtVPrismRight" runat="server" CssClass="srtsTextBox" Width="100" Text='<%# Eval("OdVPrism") %>' ReadOnly="true" ClientIDMode="Static"></asp:TextBox>
                                       <asp:TextBox ID="txtVPrismLeft" runat="server" CssClass="srtsTextBox" Width="100" Text='<%# Eval("OsVPrism") %>' ReadOnly="true" ClientIDMode="Static"></asp:TextBox>
                                   </div>
                                    <div class="w3-col l2">
                                       <asp:Label ID="lblVBase" runat="server" Text="V-Base" CssClass="srtsLabel_medium"></asp:Label><br />
                                       <asp:TextBox ID="txtVBaseRight" runat="server" CssClass="srtsTextBox" Width="100" Text='<%# Eval("OdVBase") %>' ReadOnly="true" ClientIDMode="Static"></asp:TextBox>
                                       <asp:TextBox ID="txtVBaseLeft" runat="server" CssClass="srtsTextBox" Width="100" Text='<%# Eval("OsVBase") %>' ReadOnly="true" ClientIDMode="Static"></asp:TextBox>
                                   </div>
                                </div>

                                  <!-- Calculated Values -->
                                 <div class="w3-row padding">
                                     <div id="divCalculatedValues" runat="server" style="padding: 0px 0px 20px 75px">
                                         <asp:Label ID="lblCalculatedValues" runat="server" CssClass="srtsLabel_medium_text" Text="Calculated Values"></asp:Label>
                                     </div>
                                   <div class="w3-col l2">
                                      <div style="padding:20px 20px 0px 0px;text-align:right">
                                          <asp:Label ID="lblCalculatedRight" runat="server" Text="Right (OD)" CssClass="srtsLabel_medium"></asp:Label><br /><br />
                                          <asp:Label ID="lblCalculatedLeft" runat="server" Text="Left (OS)" CssClass="srtsLabel_medium"></asp:Label>
                                      </div>
                                   </div>
                                   <div class="w3-col l2">
                                       <asp:Label ID="lblCalculatedSph" runat="server" Text="Sph" CssClass="srtsLabel_medium"></asp:Label><br />
                                       <asp:TextBox ID="txtCalculatedSphRight" runat="server" CssClass="srtsTextBox" Width="100" Text='<%# Eval("OdSphereCalc") %>' ReadOnly="true" ClientIDMode="Static"></asp:TextBox>
                                       <asp:TextBox ID="txtCalculatedSphLeft" runat="server" CssClass="srtsTextBox" Width="100" Text='<%# Eval("OsSphereCalc") %>' ReadOnly="true" ClientIDMode="Static"></asp:TextBox>
                                   </div>
                                    <div class="w3-col l2">
                                       <asp:Label ID="lblCalculatedCyl" runat="server" Text="Cyl" CssClass="srtsLabel_medium"></asp:Label><br />
                                       <asp:TextBox ID="txtCalculatedCylRight" runat="server" CssClass="srtsTextBox" Width="100" Text='<%# Eval("OdCylinderCalc") %>' ReadOnly="true" ClientIDMode="Static"></asp:TextBox>
                                       <asp:TextBox ID="txtCalculatedCylLeft" runat="server" CssClass="srtsTextBox" Width="100" Text='<%# Eval("OsCylinderCalc") %>' ReadOnly="true" ClientIDMode="Static"></asp:TextBox>
                                   </div>
                                    <div class="w3-col l2">
                                       <asp:Label ID="lblCalculatedAxis" runat="server" Text="Axis" CssClass="srtsLabel_medium"></asp:Label><br />
                                       <asp:TextBox ID="txtCalculatedAxisRight" runat="server" CssClass="srtsTextBox" Width="100" Text='<%# Eval("OdAxisCalc") %>' ReadOnly="true" ClientIDMode="Static"></asp:TextBox>
                                       <asp:TextBox ID="txtCalculatedAxisLeft" runat="server" CssClass="srtsTextBox" Width="100" Text='<%# Eval("OsAxisCalc") %>' ReadOnly="true" ClientIDMode="Static"></asp:TextBox>
                                   </div>
                                </div>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
                    </div>
                        </div>
                    </div>
                
                
            </div>

         </div>
    </div>


    <asp:ScriptManagerProxy ID="smpProxy" runat="server">
        <Scripts>
<%--            <asp:ScriptReference Path="~/Scripts/Patient/ManagePatients.js" />--%>
             <asp:ScriptReference Path="~/Scripts/LabManagement/LabOrderLookup.js" />
            <asp:ScriptReference Path="~/Scripts/Global/SrtsCustomValidators.js" />
        </Scripts>
    </asp:ScriptManagerProxy>
</asp:Content>