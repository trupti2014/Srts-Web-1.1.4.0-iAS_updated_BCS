<%@ Page Title="" Language="C#" MasterPageFile="~/GEyes/GEyesMasterResponsive.Master" AutoEventWireup="true"
    CodeBehind="IndividualInfo.aspx.cs" Inherits="GEyes.Forms.IndividualInfo" %>

<asp:Content ID="Content6" ContentPlaceHolderID="MainContent_Public" runat="server">

<%--Panel - Important Information--%>
<asp:Panel ID="pnlMain" runat="server" Visible="False">
    <div class="w3-row">
        <div class="text-left" style="padding-left:100px">
            <a href="https://www.med.navy.mil/sites/nostra/Pages/Spectacles.aspx">Click here to see pictures of the frames</a><br />
            <%--<a href="https://srtsweb.amedd.army.mil/WebForms/Public/CheckOrderStatus.aspx">Click here for order status</a>--%>
            <a runat="server" href="~/WebForms/Public/CheckOrderStatus.aspx">Click here for order status</a>
        </div>
    </div>
<div class="container align_center">
<div class="card cardHeader">
     <h5 class="card-header">User Identification</h5>
     <div class="card-body bg-white text-left w3-padding-24">
     <p class="card-title w3-large w3-padding-large">Please provide an Id, email address and your current location zip code.</p>
       
         
         
         
         <div class="w3-padding-large">

   <%--  Caution Panel--%>
        <div style="margin:-20px 20px -5px 20px">
        <div class="w3-panel w3-pale-yellow w3-border w3-border-yellow" style="height:auto;font-size:12px;padding-top:15px">
        <blockquote class="G-Eyes_warning">
            CAUTION:  The email address you enter will be used to confirm your order.  You will be sent a confirmation email to the email address listed.  Ensure your email address is correct before you advance to the next page.
        </blockquote>
        </div>
        </div>


        <div id="G-EyesUserIdentification">
                     <div class="G-Eyes_errorsummary w3-text-red">
                           <span style="color:red!important"><asp:ValidationSummary ID="vsErrors" runat="server" ForeColor="Red" ShowSummary="false" CssClass="G-Eyes_ErrorSummary" ValidationGroup="IndAddr" />
                            <asp:ValidationSummary ID="ErrorMessage" runat="server" ForeColor="Red" ShowSummary="true" CssClass="G-Eyes_ErrorSummary" ValidationGroup="ErrorMessage" /></span>
                        </div>

     
                    
                         <div class="w3-row-padding">
                            <div class="w3-half w3-padding">
                                 <div id="divIdNumber">
                                    <label>ID Number</label>
                                    <asp:TextBox ID="tbIDNumber" runat="server" CssClass="form-control" ToolTip="Enter your identification number." TextMode="Password" MaxLength="10"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvIDNumber" runat="server" ControlToValidate="tbIDNumber"
                                    Display="Dynamic" ErrorMessage="ID Number is required" ValidationGroup="IndAddr" SetFocusOnError="False">* ID Number is required.</asp:RequiredFieldValidator>
                                </div>
                              
                             </div>
                            <div class="w3-half">
                                <div id="divIdType" style="margin-top:7px;margin-left:8px">
                                   <label>ID Type</label>
                                    <div class="form-control" style="width:98%;height:42px;padding-top:3px">
                                        &nbsp;
                                        <input id="rbDoD" runat="server" class="w3-radio" type="radio" name="IDNumberType" value="DoD" checked>
                                        <label>DoD</label>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                        <input id="rbSSN" runat="server" class="w3-radio" type="radio" name="IDNumberType" value="SSN">
                                        <label>SSN</label>
                                   </div>
                                </div>
                             </div>
                          </div>

                         <div class="w3-row-padding">
                                   <div class="w3-padding">
                              <label>Email Address</label>
                             <asp:TextBox ID="tbEmail" runat="server" CssClass="form-control" ToolTip="Enter your email address."> </asp:TextBox>
                               <asp:RequiredFieldValidator ID="rfvEmail" runat="server" ControlToValidate="tbEmail"
                                    Display="Dynamic" ErrorMessage="Email Address is required" ValidationGroup="IndAddr">* Email Address is required.
                                       </asp:RequiredFieldValidator>
                                <asp:RegularExpressionValidator ID="RegExValidatorEmail1" runat="server" ErrorMessage="Not a valid email format" Display="Dynamic" ControlToValidate="tbEmail" ValidationExpression="^([a-zA-Z][\w\.-]*[a-zA-Z0-9]@[a-zA-Z0-9][\w\.-]*[a-zA-Z0-9]\.[a-zA-Z][a-zA-Z\.]*[a-zA-Z]){1,70}$" ValidationGroup="InAddr" CssClass="G-Eyes_validator">* Invalid Email format.</asp:RegularExpressionValidator>
                              </div>
                          </div>
                  
                        <div class="w3-row-padding">
                             <div class="w3-padding">
                              <label">Email Address (confirm)</label>
                            <asp:TextBox ID="tbEmailConfirm" runat="server" CssClass="form-control" ToolTip="Enter email address again."></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvEmailConfirm" runat="server" ControlToValidate="tbEmailConfirm"
                                Display="Dynamic" ErrorMessage="Confirmation Email Address is required" ValidationGroup="IndAddr">* Confirmation Email Address is required.</asp:RequiredFieldValidator>
                            <asp:CompareValidator ID="cvEMail" runat="server" ControlToCompare="tbEmail" ControlToValidate="tbEmailConfirm"
                                Display="Dynamic" ErrorMessage="Email Addresses are not identical." BorderStyle="None" ValidationGroup="IndAddr">* Email Addresses are not identical.</asp:CompareValidator>
                               </div>
                         </div>

                        <div class="w3-row-padding">
                            <div class="w3-padding">
                                <label>Zip Code</label>
                                <asp:TextBox ID="tbZipCode" runat="server"  CssClass="form-control" ToolTip="Enter the zip code.  Must be five numbers or nine numbers with a hyphen (e.g., 55555 or 55555-4444)." Text=""></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvZipCode" runat="server" ControlToValidate="tbZipCode"
                                    Display="Dynamic" ErrorMessage="Zip code is required.  Enter five numbers or nine numbers with a hyphen (i.e. 55555 or 55555-4444)." ValidationGroup="IndAddr">* Zip Code is required.</asp:RequiredFieldValidator>
                                <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="tbZipCode"
                                    ErrorMessage="Zip code must be five numbers or nine numbers with a hyphen (i.e. 55555 or 55555-4444)." ValidationExpression="^\d{5}(\-\d{4})?$"
                                    Display="Static" ValidationGroup="IndAddr" CssClass="G-Eyes_validator">* Invalid Zip Code format.</asp:RegularExpressionValidator>
                            </div>
                         </div>

                    </div>

                   <asp:Panel ID="pnlConfirm" runat="server" Visible="true">
                       <div class="text-center">
                        <asp:LinkButton ID="lnkSubmit" class="btn bg-secondary text-white" runat="server" ValidationGroup="IndAddr" OnCommand="btnSubmit_Click">Next</asp:LinkButton>&nbsp;&nbsp;
                        <asp:LinkButton ID="lnkCancel" class="btn bg-secondary text-white" runat="server" CausesValidation="False" OnCommand="btnCancel_Click">Cancel</asp:LinkButton>
                        </div>
                    </asp:Panel>

        </div>
       </div>
</div>
</div>
</asp:Panel>
</asp:Content>