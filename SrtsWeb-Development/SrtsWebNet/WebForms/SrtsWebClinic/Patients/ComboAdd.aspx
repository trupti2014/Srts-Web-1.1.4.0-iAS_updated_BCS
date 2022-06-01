<%@ Page Title="" Language="C#" MasterPageFile="~/SrtsMaster.Master" AutoEventWireup="true"
    CodeBehind="ComboAdd.aspx.cs" Inherits="SrtsWebClinic.Patients.ComboAdd" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="PatientInformation" ContentPlaceHolderID="ContentTop_Title_Right"
    runat="server">
    <asp:Literal ID="litContentTop_Title_Right" runat="server"></asp:Literal>
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="MainContent" runat="server">   
        <%--Patient Personal Information--%>
<srts:PatientDemographics ID="PatientDemographics2" runat="server"></srts:PatientDemographics>
        <div>
            <%--Patient Contact Information--%>
            <h1>&nbsp;&nbsp; Add New Identification and Contact Information</h1>
                       
                            <div>
                                <asp:ValidationSummary ID="vsErrors" runat="server" ShowMessageBox="true" ShowSummary="true"
                                    DisplayMode="BulletList" />
                            </div>
                            <div style="margin-top:-30px">
                            <srts:BeigeContainerHeader ID="srtsAddPatientInformationHeader" runat="server" ></srts:BeigeContainerHeader>
                            <div class="padding" style="margin-top:10px;height:320px">
                            <h1>Select an Option</h1>
                          
                                            <%--       update forms --%>  
                                 
                          <div class="padding" style="height:240px;float:right;margin:-55px 0px 0px 155px;width:560px">                                                                                   <srts:BeigeContainerHeader ID="srtsAddInformationHeader" runat="server" />

                           <div style="width:185px;margin:65px 0px 0px -185px">
                                <asp:Panel ID="pnlIDNumberClick" runat="server">
                                                <div class="addpatientinfo">
                                                    <asp:Label ID="lblIDNumberMessage" runat="server" />
                                                </div>
                                </asp:Panel>
                                <asp:Panel ID="pnlAddressClick" runat="server">
                                                <div class="addpatientinfo">
                                                    <asp:Label ID="lblMessageClick" runat="server" />
                                                </div>
                                            </asp:Panel>
                                <asp:Panel ID="pnlEmailClick" runat="server">
                                                <div class="addpatientinfo">
                                                    <asp:Label ID="lblEmailMessage" runat="server" />
                                                </div>
                                            </asp:Panel>
                                <asp:Panel ID="pnlPhoneClick" runat="server">
                                                <div class="addpatientinfo">
                                                    <asp:Label ID="lblPhoneMessage" runat="server" />
                                                </div>
                                </asp:Panel>
                           </div>


                                    <div class="padding" style="position:absolute;margin-top:-240px;height:230px;width:500px">
                                        <asp:Panel ID="pnlIDNumber" runat="server">
                                        <h1 style="border-bottom:1px solid #C6A252">Add New Identification Number</h1>
                                            <div class="padding" style="float:right;margin-top:20px">
                                                <table>
                                                    <tr>
                                                        <td style="text-align: left;">
                                                            <label>
                                                                <strong>Identification Number</strong></label>
                                                        </td>
                                                        <td style="text-align: left;">
                                                            <label>
                                                                <strong>Identification Type</strong></label><br />
                                                        </td>
                                                 
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <asp:TextBox ID="tbIDNumber" runat="server" MaxLength="100" TabIndex="1" Width="250px"
                                                                ToolTip="Enter the individuals Identification Number">
                                                            </asp:TextBox>
                                                            <asp:RequiredFieldValidator ID="rfvIdentificationNumbers" ControlToValidate="tbIDNumber"
                                                                ValidationGroup="idnumb" runat="server" Display="None" ErrorMessage="Identification Number is a required field"></asp:RequiredFieldValidator>
                                                        </td>
                                                        <td>
                                                            <asp:DropDownList ID="ddlIDNumberType" runat="server" TabIndex="2" DataTextField="Value"
                                                                DataValueField="Key" ToolTip="Select patient identification type.">
                                                            </asp:DropDownList>
                                                        </td>
                                                    </tr>
                                                </table>
                                                <div>
                                                    <asp:Label ID="lblCompleteIDNumber" runat="server" Font-Bold="true" Text=""></asp:Label><br />
                                                    <br />
                                                    <asp:Button ID="btnSaveIDNumber" runat="server" Text="Save Data" TabIndex="3" CausesValidation="true"
                                                        ValidationGroup="idnumb" OnClick="btnSaveIDNumber_Click" />&nbsp;&nbsp;
                                                    <asp:LinkButton ID="lnkCancelID" runat="server" Text="Cancel Operation" CausesValidation="false"
                                                        PostBackUrl="~/SrtsWebClinic/Patients/PatientDetails.aspx"></asp:LinkButton>
                                                    <asp:LinkButton ID="lblReturenID" runat="server" Text="Return to Details Page" CausesValidation="false"
                                                        PostBackUrl="~/SrtsWebClinic/Patients/PatientDetails.aspx"></asp:LinkButton>
                                                </div>
                                            </div>
                                            <div style="float:left;width:175px;position:absolute;z-index:200px;height:200px;margin-left:-200px;background-color:#fff">
                                                        <div class="padding" style="margin-top:20px;border-top:1px solid #E7CFAD;border-bottom:1px solid #E7CFAD;color:#004994">
                                            Add new information.  Select 'Save Data' to complete operation or 'Cancel'.
                                            </div>
                                            </div> 
                                        </asp:Panel>
                                        <ajaxToolkit:CollapsiblePanelExtender ID="cpeIDNumber" runat="server" TargetControlID="pnlIDNumber"
                                            ExpandControlID="pnlIDNumberClick" CollapseControlID="pnlIDNumberClick" Collapsed="true"
                                            CollapsedImage="~/Styles/images/img_expan.png" ExpandedImage="~/Styles/images/img_collapse.png"
                                            TextLabelID="lblIDNumberMessage" ExpandedText="Hide Identification Number" CollapsedText="Add Identification Number"
                                            ExpandDirection="Horizontal" ScrollContents="false">
                                        </ajaxToolkit:CollapsiblePanelExtender>
                         
                               
                                        <asp:Panel ID="pnlAddress" runat="server">
                                           <h1 style="border-bottom:1px solid #C6A252">Add New Mailing Address</h1>
                                            <div class="padding" style="float:right;margin-top:20px">
                                            <div>
                                                <table>
                                                    <tr>
                                                        <td style="text-align: left;">
                                                            <label>
                                                                <strong>Address 1</strong></label>
                                                        </td>
                                                        <td style="text-align: left;">
                                                            <label>
                                                                <strong>Address 2</strong></label>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <asp:TextBox ID="tbAddress1" runat="server" MaxLength="100" TabIndex="4" Width="250px"
                                                                ToolTip="Enter the patient house and street address.">
                                                            </asp:TextBox>
                                                            <asp:RequiredFieldValidator ID="rfvAddress1" ControlToValidate="tbAddress1" ErrorMessage="Address1 is a required field"
                                                                Display="None" runat="server" ValidationGroup="addr"></asp:RequiredFieldValidator>
                                                            <asp:RegularExpressionValidator ID="revAddress1" runat="server" ControlToValidate="tbAddress1"
                                                                ErrorMessage="Invalid characters in Address 1" ValidationExpression="^[0-9a-zA-Z '-]+$"
                                                                Display="Dynamic" ValidationGroup="addr"></asp:RegularExpressionValidator>
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="tbAddress2" runat="server" MaxLength="100" TabIndex="5" Width="250px"
                                                                ToolTip="Continuation of patient address.">
                                                            </asp:TextBox>
                                                            <asp:RegularExpressionValidator ID="revAddress2" runat="server" ControlToValidate="tbAddress2"
                                                                ErrorMessage="Invalid characters in Address 2" ValidationExpression="^[0-9a-zA-Z '-]+$"
                                                                Display="Dynamic" ValidationGroup="addr"></asp:RegularExpressionValidator>
                                                        </td>
                                                    </tr>
                                                </table>
                                                <table>
                                                    <tr>
                                                        <td style="text-align: left;">
                                                            <label>
                                                                <strong>City</strong></label>
                                                        </td>
                                                        <td style="text-align: left;">
                                                            <label>
                                                                <strong>State</strong></label>
                                                        </td>
                                                        <td style="text-align: left;">
                                                            <label>
                                                                <strong>Country</strong></label>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <asp:TextBox ID="tbCity" runat="server" MaxLength="100" TabIndex="6" ToolTip="Enter city name from patient address">
                                                            </asp:TextBox>
                                                            <asp:RequiredFieldValidator ID="rfvCity" runat="server" ErrorMessage="City is a required field"
                                                                ControlToValidate="tbCity" Display="None" ValidationGroup="addr"></asp:RequiredFieldValidator>
                                                        </td>
                                                        <td>
                                                            <asp:DropDownList ID="ddlState" runat="server" TabIndex="7" ToolTip="Select patient residence state."
                                                                DataTextField="Value" DataValueField="Key">
                                                            </asp:DropDownList>
                                                        </td>
                                                        <td>
                                                            <asp:DropDownList ID="ddlCountry" runat="server" TabIndex="8" ToolTip="Select patient residence country."
                                                                DataTextField="Value" DataValueField="Key">
                                                            </asp:DropDownList>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td style="text-align: left;">
                                                            <label>
                                                                <strong>Zip Code</strong></label>
                                                        </td>
                                                        <td style="text-align: left;">
                                                            <label>
                                                                <strong>Address Type</strong></label>
                                                        </td>
                                                        <td>
                                                            &nbsp;
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <asp:TextBox ID="tbZipCode" runat="server" TabIndex="9" ToolTip="Enter patient residence zip code">
                                                            </asp:TextBox>
                                                            <asp:RequiredFieldValidator ID="rfvZipCode" runat="server" Display="None" ErrorMessage="ZipCode is a required field"
                                                                ControlToValidate="tbZipCode" ValidationGroup="addr"></asp:RequiredFieldValidator>
                                                        </td>
                                                        <td>
                                                            <asp:DropDownList ID="ddlAddressType" runat="server" TabIndex="10" ToolTip="Select patient address type."
                                                                DataTextField="Value" DataValueField="Key">
                                                            </asp:DropDownList>
                                                        </td>
                                                        <td>
                                                            &nbsp;
                                                        </td>
                                                    </tr>
                                                </table>
                                                <div>
                                                    <asp:Label ID="lblAddrComplete" runat="server" Text="" Font-Bold="true"></asp:Label><br />
                                                    <br />
                                                    <asp:Button ID="btnSaveAddress" Text="Save Data" runat="server" CausesValidation="true"
                                                        ValidationGroup="addr" TabIndex="11" OnClick="btnSaveAddress_Click" />&nbsp;&nbsp;
                                                    <asp:LinkButton ID="lbAddrCancel" runat="server" PostBackUrl="~/SrtsWebClinic/Patients/PatientDetails.aspx"
                                                        CausesValidation="false" TabIndex="12" Text="Cancel Operation"></asp:LinkButton>
                                                    <asp:LinkButton ID="lbAddressReturn" runat="server" PostBackUrl="~/SrtsWebClinic/Patients/PatientDetails.aspx"
                                                        CausesValidation="false" Text="Return to Details Page"></asp:LinkButton>
                                                </div>
                                            </div>
                                            </div> 
                                        
                                        </asp:Panel>
                                        <ajaxToolkit:CollapsiblePanelExtender ID="cpeAddr" runat="server" TargetControlID="pnlAddress"
                                            ExpandControlID="pnlAddressClick" CollapseControlID="pnlAddressClick" Collapsed="true"
                                            TextLabelID="lblMessageClick" ExpandedText="Hide Address" CollapsedText="Add New Mailing Address"
                                            ExpandDirection="Horizontal" ScrollContents="false">
                                        </ajaxToolkit:CollapsiblePanelExtender>
                       
                        
                                        <asp:Panel ID="pnlEmail" runat="server">
                                            <div>
                                                <table>
                                                    <tr>
                                                        <td style="text-align: left;">
                                                            <label>
                                                                <strong>EMail Address</strong></label>
                                                        </td>
                                                        <td style="text-align: left;">
                                                            <label>
                                                                <strong>Email Type</strong></label>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <asp:TextBox ID="tbEMailAddress" runat="server" TabIndex="13" Width="300px" ToolTip="Enter the patient EMail Address">
                                                            </asp:TextBox>
                                                            <asp:RequiredFieldValidator ID="rfvEmailAddress" runat="server" ControlToValidate="tbEMailAddress"
                                                                ValidationGroup="email" Display="None" ErrorMessage="Email Address is a required field"></asp:RequiredFieldValidator>
                                                        </td>
                                                        <td>
                                                            <asp:DropDownList ID="ddlEMailType" runat="server" TabIndex="14" ToolTip="Select patient email type."
                                                                DataTextField="Key" DataValueField="Value">
                                                            </asp:DropDownList>
                                                        </td>
                                                    </tr>
                                                </table>
                                                <div>
                                                    <asp:Label ID="lblEmailComplete" runat="server" Font-Bold="true" Text=""></asp:Label><br />
                                                    <br />
                                                    <asp:Button ID="btnEmailSave" Text="Save Data" runat="server" CausesValidation="true"
                                                        ValidationGroup="email" TabIndex="15" OnClick="btnEmailSave_Click" />&nbsp;&nbsp;
                                                    <asp:LinkButton ID="lbEmailCancel" runat="server" CausesValidation="false" Text="Cancel Operation"
                                                        PostBackUrl="~/SrtsWebClinic/Patients/PatientDetails.aspx"></asp:LinkButton>
                                                    <asp:LinkButton ID="lbEmailReturn" runat="server" CausesValidation="false" PostBackUrl="~/SrtsWebClinic/Patients/PatientDetails.aspx"
                                                        Text="Return to Details Page"></asp:LinkButton>
                                                </div>
                                            </div>
                                        </asp:Panel>
                                        <ajaxToolkit:CollapsiblePanelExtender ID="cpeEmail" runat="server" TargetControlID="pnlEmail"
                                            ExpandControlID="pnlEmailClick" CollapseControlID="pnlEmailClick" Collapsed="true"
                                            TextLabelID="lblEmailMessage" ExpandedText="Hide Email Address" CollapsedText="Add New Email Address"
                                            ExpandDirection="Horizontal" ScrollContents="false">
                                        </ajaxToolkit:CollapsiblePanelExtender>
                         
                              
                                        <asp:Panel ID="pnlPhone" runat="server">
                                            <div>
                                                <table>
                                                    <tr>
                                                        <td style="text-align: left;">
                                                            <label>
                                                                <strong>Area Code</strong></label>
                                                        </td>
                                                        <td style="text-align: left;">
                                                            <label>
                                                                <strong>Phone Number</strong></label>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <asp:TextBox ID="tbAreaCode" runat="server" MaxLength="100" TabIndex="16" Width="250px"
                                                                ToolTip="Enter the area code">
                                                            </asp:TextBox>
                                                            <asp:RequiredFieldValidator ID="rfvAreaCode" ControlToValidate="tbAreaCode" Display="None"
                                                                ErrorMessage="Area Code is a required field" runat="server"></asp:RequiredFieldValidator>
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="tbPhoneNumber" runat="server" MaxLength="100" TabIndex="17" Width="250px"
                                                                ValidationGroup="phone" ToolTip="Continuation of patient Phone Number.">
                                                            </asp:TextBox>
                                                            <asp:RequiredFieldValidator ID="rfvPhoneNumber" ControlToValidate="tbPhoneNumber"
                                                                ValidationGroup="phone" ErrorMessage="Phone Number is a required field" Display="None"
                                                                runat="server"></asp:RequiredFieldValidator>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td style="text-align: left;">
                                                            <label>
                                                                <strong>Extension</strong></label>
                                                        </td>
                                                        <td style="text-align: left;">
                                                            <label>
                                                                <strong>Phone Type</strong></label>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <asp:TextBox ID="tbExtension" runat="server" MaxLength="100" TabIndex="18" ToolTip="Enter patient extension">
                                                            </asp:TextBox>
                                                        </td>
                                                        <td>
                                                            <asp:DropDownList ID="ddlPhoneType" runat="server" TabIndex="19" ToolTip="Select patient phone type."
                                                                DataTextField="Key" DataValueField="Value">
                                                            </asp:DropDownList>
                                                        </td>
                                                    </tr>
                                                </table>
                                                <div>
                                                    <asp:Label ID="lblPhoneComplete" runat="server" Text=""></asp:Label><br />
                                                    <br />
                                                    <asp:Button ID="btnPhoneSave" Text="Save Data" runat="server" CausesValidation="true"
                                                        ValidationGroup="phone" TabIndex="20" OnClick="btnPhoneSave_Click" />&nbsp;&nbsp;
                                                    <asp:LinkButton ID="btnCancel" Text="Cancel Operation" runat="server" CausesValidation="false"
                                                        PostBackUrl="~/SrtsWebClinic/Patients/PatientDetails.aspx" />
                                                    <asp:LinkButton id="lnkReturnPhone" runat="server" CausesValidation="false" Text="Return to Details Page"
                                                        PostBackUrl="~/SrtsWebClinic/Patients/PatientDetails.aspx"></asp:LinkButton>
                                                </div>
                                            </div>
                                        </asp:Panel>
                                        <ajaxToolkit:CollapsiblePanelExtender ID="cpePhone" runat="server" TargetControlID="pnlPhone"
                                            ExpandControlID="pnlPhoneClick" CollapseControlID="pnlPhoneClick" Collapsed="true"
                                            TextLabelID="lblPhoneMessage" ExpandedText="Hide Phone Numbers" CollapsedText="Add New Phone Number"
                                            ExpandDirection="Horizontal" ScrollContents="false">
                                        </ajaxToolkit:CollapsiblePanelExtender>
                                    </div>
                                    <srts:BeigeContainerFooter ID="srtsAddInformationFooter" runat="server" />      
                                    </div>  
                            </div>

                          
                            
                            <srts:BeigeContainerFooter ID="srtsAddPatientInformationFooter" runat="server"/>
                       </div>                   
        </div>
</asp:Content>
