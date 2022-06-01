<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ucSitePreferencesPrescriptions.ascx.cs" Inherits="SrtsWeb.UserControls.ucSitePreferencesPrescriptions" %>


           <div class="w3-row" style="margin-top: -30px">               
             <div class="BeigeBoxContainer" style="margin: 10px 0px 0px 0px">
                <div class="BeigeBoxHeader" style="text-align:left;position: relative; top: 0px; padding: 12px 10px 3px 15px">
                   <span class="label">Prescriptions Preferences</span>
                </div>
                <div class="BeigeBoxContent padding" style="padding-right:0px;margin-top: 0px; margin-left: 0px; height: auto">                 
                     <div class="w3-row alignLeft padding" style="padding-top:0px">
       
                        <div class="w3-half">
                        <%-- Provider--%>
                        <asp:Label ID="lblScriptProvider" runat="server" Text="Select Default Provider:" AssociatedControlID="ddlPrescriptionProvider" CssClass="srtsLabel_medium" /><br />
                               <div class="padding" style="padding-top:10px">
                        <asp:DropDownList ID="ddlPrescriptionProvider" runat="server" ClientIDMode="Static" CssClass="srtsDropDown_medium" TabIndex="1" Width="260px"
                             OnSelectedIndexChanged="ddlPrescriptionProvider_SelectedIndexChanged"></asp:DropDownList>
               </div>
                        </div>
                        <div class="w3-half">

                        <%-- Type--%>
                        <asp:Label ID="lblPName" runat="server" Text="Select Default Rx Type:" AssociatedControlID="ddlPrescriptionName" CssClass="srtsLabel_medium" /><br />
                        <div class="padding" style="padding-top:10px">
                            <asp:DropDownList ID="ddlPrescriptionName" runat="server" CssClass="srtsDropDown_medium" ClientIDMode="Static" TabIndex="2"  Width="260px"
                             OnSelectedIndexChanged="ddlPrescriptionName_SelectedIndexChanged">
                            <asp:ListItem Text="-Select-" Value="X"></asp:ListItem>
                            <asp:ListItem Text="-No Default-" Value="N"></asp:ListItem>
                            <asp:ListItem Text="DVO" Value="DVO"></asp:ListItem>
                            <asp:ListItem Text="NVO" Value="NVO"></asp:ListItem>
                            <asp:ListItem Text="FTW" Value="FTW"></asp:ListItem>
                            <asp:ListItem Text="Computer/Other" Value="CO"></asp:ListItem>
                        </asp:DropDownList>
                        </div>
                        </div>
         
                      

                    </div>
                    <div class="w3-row alignLeft padding" style="padding-bottom:0px">
                    <div class="w3-half">
                   <%-- Prescritpion Distance--%>   
                        <asp:Label ID="lblPD" runat="server" Text="Set Default PD Distance:" CssClass="srtsLabel_medium" /><br />
                           <div class="padding" style="padding-top:10px">
                        <asp:RadioButtonList ID="rblPDDistance" runat="server" TabIndex="8" RepeatDirection="Vertical" ToolTip="Select PD Default Value Option"
                            CausesValidation="false" ClientIDMode="Static" onchange="DoRblPDDistanceChange();">
                          <%--  <asp:ListItem Text="Use the global default value." Value="GLOBAL" />  --%>                     
                            <asp:ListItem Text="Leave blank and require a value be entered." Value="REQUIRE" />
                            <asp:ListItem Text="Use this default value:" Value="DEFAULT" />
                        </asp:RadioButtonList>
                        <div style="position:relative;top:-23px;left:170px">
                        <asp:TextBox ID="tbPD" runat="server" ClientIDMode="Static" TabIndex="3" CssClass="srtsTextBox_small" Width="80px"/>        
                       </div>
                        </div>
                        </div>
  
                        <div class="w3-half">
                        <%-- Prescritpion Near--%>
                        <asp:Label ID="lblPDNear" runat="server" Text="Set Default PD Near:" CssClass="srtsLabel_medium" /><br />
                              <div class="padding" style="padding-top:10px">
                             <asp:RadioButtonList ID="rblPDNear" runat="server" TabIndex="8" RepeatDirection="Vertical" ToolTip="Select PD Default Value Option"
                            CausesValidation="false" ClientIDMode="Static" onchange="DoRblPDNearChange();">
                           <%-- <asp:ListItem Text="Use the global default value." Value="GLOBAL" />      --%>                 
                            <asp:ListItem Text="Leave blank and require a value be entered." Value="REQUIRE" />
                            <asp:ListItem Text="Use this default value:" Value="DEFAULT" />
                            </asp:RadioButtonList>
                        <div style="position:relative;top:-23px;left:170px">
                        <asp:TextBox ID="tbPDNear" runat="server" ClientIDMode="Static" TabIndex="4" CssClass="srtsTextBox_small" Width="80px"></asp:TextBox>
                        </div> 
                        </div>
                        </div>
                    </div>
                </div>

                   
                     <div class="BeigeBoxFooter"></div>             
                </div>
                      <div class="customValidators">
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ErrorMessage="Error:  Provider requires a selection." 
                            ControlToValidate="ddlPrescriptionProvider" InitialValue="X"></asp:RequiredFieldValidator><br />
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="Error:  RX Type requires a selection." 
                            ControlToValidate="ddlPrescriptionName" InitialValue="X"></asp:RequiredFieldValidator><br />
                            <asp:CustomValidator ID="cvPD_Distance" runat="server" ControlToValidate="tbPD"
                            ErrorMessage="Error:  PD Distance value must be between 52 and 82." ClientValidationFunction="DoPdVal"></asp:CustomValidator><br />
                            <asp:CustomValidator ID="cvPD_Near" runat="server" ControlToValidate="tbPDNear" 
                            ErrorMessage="Error:  PD Near value must be between 52 and 82." ClientValidationFunction="DoPdVal"></asp:CustomValidator><br />
                            <asp:Label ID="lblMessage" runat="server"></asp:Label>
                        </div> 

              
             </div>
       
                    <div style="position:relative;top:-60px;left:385px">
                    <asp:Button ID="bUpdateRxPref" runat="server" OnClick="bUpdateRxPref_Click" Text="Submit" CssClass="srtsButton" />
                    </div>





<script type="text/javascript">
    function DoPdVal(oSrc, args) {

        var max = 82.00;
        var min = 52.00;
        var val = args.Value;

        args.IsValid = val >= min && val <= max;
    }
</script>

