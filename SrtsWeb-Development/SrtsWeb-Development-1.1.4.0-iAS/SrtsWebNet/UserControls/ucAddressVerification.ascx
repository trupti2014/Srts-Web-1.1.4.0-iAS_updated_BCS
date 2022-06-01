<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ucAddressVerification.ascx.cs" Inherits="SrtsWeb.UserControls.ucAddressVerification" %>


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
                                        <asp:UpdatePanel ID="UpdatePanel1" runat="server" ChildrenAsTriggers="false" UpdateMode="Conditional">
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
                                                     <%--  <div class="rightArrow"><asp:ImageButton ID="btnSaveVerifiedAddress" CommandName="SaveVerifiedAddress" runat="server" 
                                                           ImageUrl="~/Styles/images/Arrow_blue_right.gif" Width="25px" CausesValidation="false" OnClick="SaveAddress" ToolTip="I would like to use the Verified Mailing Address."  /></div>--%>
                                                </div>
                                                  <div>
                                                        <!-- Address 1, Address 2 -->
                                                        <div class="w3-row">
                                                            <!-- Address 1, Address 2 -->
                                                   
                                                                <!-- Address 1 -->
                                                                <div class="padding">
                                                                    <asp:Label ID="lblAddress1Verified" runat="server" Text="Address 1" CssClass="srtsLabel_medium" /><br />
                                                                    <asp:TextBox ID="txtAddress1Verified" runat="server" MaxLength="100" CssClass="srtsTextBox_medium" 
                                                                        ClientIDMode="Static" Width="255px" ReadOnly="true" />                                                    
                                                                </div>
                                                            
                                                                <!-- Address 2 -->
                                                                <div  class="padding">
                                                                    <asp:Label ID="lblAddress2Verified" runat="server" Text="Address 2" CssClass="srtsLabel_medium" /><br />
                                                                    <asp:TextBox ID="txtAddress2Verified" runat="server" MaxLength="100" CssClass="srtsTextBox_medium" 
                                                                        ClientIDMode="Static" width="255px" ReadOnly="true" />        
                                                                </div>
                                                        
                                                        </div>

                                                        <!-- City, State -->
                                                   
                                                        <div class="w3-row">
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
                                                                    <asp:TextBox ID="txtZipCodeVerified" runat="server" CssClass="srtsTextBox_medium" ClientIDMode="Static" Width="75px" ReadOnly="true" />        
                                                                    </div>
                                                                </div>
                                                            </div>                                             
                                                        </div>

                                           
                                                    
                                                        <!-- State, Country -->
                                                        <div class="w3-row" style="padding-top:20px">
                                                        <!-- State -->
                                                            <div id="div3" class="w3-half">
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
                                  
                                         
                                                        </div>
                                                    <br /><br />
                                                    <div style="float:right">
                                                        <asp:Button ID="btnSaveVerifiedAddress" runat="server" Text="Save" CssClass="srtsButton" CommandName="SaveVerifiedAddress" CausesValidation="false" 
                                                            OnClick="SaveAddress" ToolTip="I would like to use the Verified Mailing Address." />
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
                                                <asp:AsyncPostBackTrigger ControlID="btnAddressVerify" EventName="click" />
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