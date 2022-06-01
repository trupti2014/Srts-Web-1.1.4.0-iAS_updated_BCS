<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ucSitePreferencesLabMailToPatient.ascx.cs" Inherits="SrtsWeb.UserControls.ucSitePreferencesLabMailToPatient" %>

<!-- Site Preferences - Lab MailToPatient -->
<style>
    .selected {
    color: darkgreen;
    font-weight: bold;
    }

    /* Checkbox element, when checked */
input[type="checkbox"]:checked {
  /*box-shadow: 0 0 0 3px hotpink;*/
}

</style>

<asp:UpdatePanel ID="uplabMailToPatient" runat="server" UpdateMode="conditional" ChildrenAsTriggers="true">
    <ContentTemplate>
        <div class="w3-row alignLeft" style="margin-top: -30px; height: auto">
                <div class="BeigeBoxContainer" style="float:left;margin: 10px 0px 20px 0px; height: auto;width:100%">
                <div class="BeigeBoxHeader" style="text-align: left; position: relative; top: 0px; padding: 12px 10px 3px 15px">
                    <span class="label">Mail-to-Patient</span>
                </div>
                <div class="BeigeBoxContent padding" style="padding-right:0px;margin-top: 0px; margin-left: 0px; height: auto">                 
                     <div class="w3-row alignLeft padding" style="padding-top:0px">                       
                        <div class="w3-half">
                             <h1 style="text-align:left">Update Lab's Mail-to-Patient Capability</h1>                            
                            <div style="padding-top:0px;padding-left:5px;padding-right:10px;margin-top:-15px">                               
                                    <% // Cability Status -  Set Lab Capability %>
                                    <div class="sitepreferencegroupbox" style="padding:10px;margin-left:-5px">
                                    <asp:Label ID="lblMailToPatient" runat="server" Text="Is this lab mail-to-patient capable?" CssClass="srtsLabel_medium" /><br />                            
                                    <asp:RadioButtonList ID="rblMailToPatient" runat="server" TabIndex="1" RepeatDirection="Vertical" ToolTip="Is this lab mail-to-patient capable."
                                        CausesValidation="false" ClientIDMode="Static">                  
                                        <asp:ListItem Text="This lab is mail-to-patient capable." Value="true" />
                                        <asp:ListItem Text="This lab is not mail-to-patient capable." Value="false" />
                                    </asp:RadioButtonList>
                                    </div>  


                                      <% // Disabled - Clinic Action Required-  %>
                                      <div id="divClinicDisableNotified" style="display:none;margin-left:-5px;margin-top:0px" >
                                            <h1 style="text-align:left;padding-left:5px">Clinic Action Required (Disabled Clinics)</h1> 
                                         <%-- // Clinics disabled and notified of action required --%>
                                            <div class="sitepreferencegroupbox" style="padding:10px;margin-top:0px;margin-left:5px;min-height:185px">
                                             <div id="divClinicActionRequired">
                                             <asp:Label ID="lblDisableClinicsNotified" runat="server" CssClass="srtsLabel_medium" 
                                                  Text="The following clinics have been disabled.  Select a clinic to re-enable their 'Lab Ship to Patient' capability." 
                                                  CausesValidation="false" ClientIDMode="Static" />
                                            <br />
                                            <span id="spnchkAllDisabledClinics" class="srtsLabel_medium"> </span>
                                                <br />
                                                <asp:CheckBox ID="chkAllDisabledClinics" runat="server" CssClass="Master" ClientIDMode="Static" Text="Select/unselect all clinics"/><br /><br />
                                                <div style="max-height:100px;overflow-y:auto">
                                                <asp:CheckBoxList ID="chkDisabledList" runat="server" ClientIDMode="Static"></asp:CheckBoxList>
                                                </div>
                                                </div>
                                            </div>
                                        </div>

                                
                                    <% // Enable - Clinics To Enable -  %>
                                      <div id="divClinicsToEnable" style="">
                                      <h1 style="text-align:left">Clinics To Enable </h1>    
                                       <div class="sitepreferencegroupbox" style="padding:10px;margin-top:0px;margin-left:0px;min-height:95px">
                                           <%--// Clinics to be enabled--%>
                                           <div id="divClinicRestartDate" style="display:none">
                                           <asp:Label ID="lblClinicRestartDate" runat="server" ClientIDMode="Static" class="srtsLabel_medium" style="text-align: left">Resume Date </asp:Label>
                                            <span id="spnClinicRestartDate" class="srtsLabel_medium" style="font-size:small;color:darkgreen">(optional)</span>
                                            <br />        
                                            <asp:TextBox ID="txtClinicRestartDate" runat="server" ClientIDMode="Static" CssClass="srtsTextBox_small" TextMode="Date" Width="130px"></asp:TextBox> 
                                            <br />
                                            <ajaxToolkit:CalendarExtender ID="CalendarExtender2" runat="server" TargetControlID="txtClinicRestartDate" Format="MM/dd/yyyy" OnClientDateSelectionChanged="ValidateClinicRestartDate"></ajaxToolkit:CalendarExtender>                                           
                                            </div><br />
                                             <asp:Label ID="lblClinicsToEnable" runat="server" CssClass="srtsLabel_medium" Text="" CausesValidation="false" ClientIDMode="Static" /><br /> 
                                            <div style="width:354px;margin-left:-2px;margin-top:20px;margin-left:0px;height:auto;max-height:85px;overflow-y:auto">                                                                                          
                                                <ul id="ulClinicsToEnable" runat="server" ClientIDMode="Static"></ul>                 
                                            </div>
                                       </div>
                                       </div>
                            

                                       <% // Capability Change - On to Off - Reason %>
                                     <div id="divNoCapabilityReason">
                                         <div class="sitepreferencegroupbox" style="padding:10px;margin-left:-5px;height:75px">
                                       <asp:Label ID="lblMailToPatientReason" runat="server" Text="Select a reason for no capability." CssClass="srtsLabel_medium" />
                                           <span id="spnNoCapabilityReason" class="srtsLabel_medium" style="display:none;font-size:smaller;color:red">(selection required)</span><br />                                
                                       <asp:RadioButtonList ID="rblNoMailToPaientReason" runat="server" RepeatDirection="Vertical" ToolTip="Select reason for no availability." 
                                            CausesValidation="false" ClientIDMode="Static">                  
                                            <asp:ListItem Text="Lab capacity not available." Value="NoCapacity"/>
                                            <asp:ListItem Text="Other" Value="Other"/>
                                        </asp:RadioButtonList>
                                          </div>
                                
                                       </div>
                            </div>
                        </div>
                        <div class="w3-half">
                                     <% // Enable/Disable -Clinics Assigned to Lab -  %>
                                     <div id="divClinicEnableDisable" style="padding-top:0px;padding-left:0px;display:none">
                                      <h1 style="text-align:left">Clinics Assigned to Lab </h1>          
                                        <div class="sitepreferencegroupbox" style="padding:10px;margin-top:0px;margin-left:0px;height:306px">                                                              
                                        <asp:Label ID="lblClinicEnableDisable" runat="server" CssClass="srtsLabel_medium"
                                         Text="Select a clinic and/or clinics to disable. The clinic will be notified that an action is required in order to use the Lab Ship to Patient option."
                                        ToolTip="Select a Clinic to enable or disable." CausesValidation="false" ClientIDMode="Static" />
                                         <br /> <br />   
                                                <span class="srtsLabel_medium">
                                                    <asp:CheckBox ID="chkAllClinics" runat="server" CssClass="Master" ClientIDMode="Static" Text="Select/unselect all clinics" /></span>
                                                <div style="width:354px;margin-left:-2px;margin-top:10px;height:206px;overflow-y:auto">
                                                    <asp:CheckBoxList id="chkClinics" runat="server" ClientIDMode="Static"></asp:CheckBoxList>
                                                </div>                                              
                                        </div>
                                    </div>



                                      <% // Disable - Clinics To Disable -  %>
                                      <div id="divClinicsToDisable" style="">
                                      <h1 style="text-align:left">Clinics To Disable </h1>    
                                      <div class="sitepreferencegroupbox" style="padding:10px;margin-top:0px;margin-left:0px;min-height:95px">
                                            <%--// Clinics to be disabled--%>                                                 
                                  
                                            <div id="divClinicStopDate" style="margin:0px;padding:0px;display:none">
                                            <div class="w3-quarter" style="padding:0px">
                                                <asp:Label ID="lblClinicStopDate" runat="server" ClientIDMode="Static" class="srtsLabel_medium" style="text-align: left">Stop Date </asp:Label>
                                                <span id="spnClinicStoptDate" style="font-size:small;color:red">(*)</span>
                                                <br />     
                                                <asp:TextBox ID="txtClinicStopDate" runat="server" ClientIDMode="Static" CssClass="srtsTextBox_small" TextMode="Date" Width="75px"></asp:TextBox> 
                                                <br />
                                                <ajaxToolkit:CalendarExtender ID="CalendarExtender3" runat="server" TargetControlID="txtClinicStopDate" Format="MM/dd/yyyy" OnClientHidden="ValidateClinicStopDate" OnClientDateSelectionChanged="ValidateClinicStopDate"></ajaxToolkit:CalendarExtender>                                           
                                            </div>
                                            <div class="w3-threequarter" style="padding:0px">
                                                <span class="srtsLabel_medium">&nbsp;&nbsp;Comments</span> <span id="spnClinicStopComments" style="font-size:small;color:darkgreen">(optional)</span>
                                                <br />
                                                &nbsp;&nbsp;<asp:TextBox ID="txtCClinicStopComment" runat="server" ClientIDMode="Static" CssClass="srtsTextBox_medium" TextMode="Date" Width="255px"></asp:TextBox> 
                                            </div>
                                           </div><br /><br /><br />
                                            <asp:Label ID="lblClinicsToDisable" runat="server" CssClass="srtsLabel_medium" Text="" CausesValidation="false" ClientIDMode="Static" /><br /><br />
                                            <div style="width:350px;margin-left:15px;margin-top:5px;margin-left:0px;height:auto;max-height:85px;overflow-y:auto">  
                                                <ul id="ulClinicsDisabled" runat="server" ClientIDMode="Static"></ul>
                                            </div>                   
                                      </div>
                                      </div>
                                 

                                            <asp:HiddenField ID="hdfClinicsToDisable" runat="server" ClientIDMode="Static" />
                                            <asp:HiddenField ID="hdfClinicsToEnable" runat="server" ClientIDMode="Static" />
                                            <asp:HiddenField ID="hdfClinicsDisabled" runat="server" ClientIDMode="Static" />
                                            <asp:HiddenField ID="hdfLabMTPStatus" runat="server" ClientIDMode="Static" />
                            

                                      <% // Capability Change - Date Range %>
                                      <div id="divCapabilityDateRange" style="padding-top:0px;display:none">
                                        <div style="text-align:right;margin-top:-20px;margin-bottom:-18px"><span id="spnRequired" style="display:none;font-size:small;color:red">(* = required)</span></div>
                                         <h1 style="text-align:left;margin-bottom:0px;margin-left:10px;margin-top:20px">Capability Availability Dates  </h1> 
                                              <div class="sitepreferencegroupbox" style="padding:10px;height:60px;margin-top:5px">                  
                                            <div id="divEndDate" style="margin:0px 0px 10px 0px">
                                                <div class="w3-half">
                                                    <asp:Label ID="lblStopDate" runat="server" ClientIDMode="Static" class="srtsLabel_medium" style="text-align: left">Availability End Date </asp:Label>
                                                     <span id="spnStopDate" class="srtsLabel_medium" style="display:none;font-size:small;color:red">(*)</span>
                                                   <br />        
                                                    <asp:TextBox ID="txtStopDate" runat="server" ClientIDMode="Static" CssClass="srtsTextBox_small" TextMode="Date"></asp:TextBox>
                                                    <ajaxToolkit:CalendarExtender ID="CalendarExtender1" runat="server" TargetControlID="txtStopDate" Format="MM/dd/yyyy" OnClientDateSelectionChanged="ValidateStopDate"></ajaxToolkit:CalendarExtender>
                                                </div>
                                                <div class="w3-half"><div id="divRestartDate"style="">
                                                    <asp:Label ID="lblRestartDate" runat="server" ClientIDMode="Static" class="srtsLabel_medium" style="text-align: left">Resume Date </asp:Label>
                                                    <span id="spnRestartDate" class="srtsLabel_medium" style="display:none;font-size:small;color:red">(*)</span>
                                                   <br />        
                                                    <asp:TextBox ID="txtRestartDate" runat="server" ClientIDMode="Static" CssClass="srtsTextBox_small" TextMode="Date" Width="150px"></asp:TextBox> 
                                                    <br />
                                                    <ajaxToolkit:CalendarExtender ID="calEnd" runat="server" TargetControlID="txtRestartDate" Format="MM/dd/yyyy" OnClientDateSelectionChanged="ValidateRestartDate"></ajaxToolkit:CalendarExtender>
                                                </div>
                                                </div>
                                            </div>
                                          

                                 
                                            <div id="divStartDate" style="display:none">
                                            <asp:Label ID="lblStartDate" runat="server" ClientIDMode="Static" class="srtsLabel_medium" style="text-align: left">Start Date </asp:Label>
                                               <br />
                                            <asp:TextBox ID="txtStartDate" runat="server" ClientIDMode="Static" CssClass="srtsTextBox_small" TextMode="Date"></asp:TextBox>
                                            <ajaxToolkit:CalendarExtender ID="calStart" runat="server" TargetControlID="txtStartDate" Format="MM/dd/yyyy" OnClientDateSelectionChanged="ValidateStartDate"></ajaxToolkit:CalendarExtender>
                                            </div>
                                        </div>
                             
                                        <div id="divOtherComment" class="sitepreferencegroupbox" style="padding:10px;height:75px">
                                      <asp:Label ID="lblComments" runat="server" Text="Comments" CssClass="srtsLabel_medium" />    
                                          <span id="spnComments" class="srtsLabel_medium" style="display:none;font-size:smaller;color:red">(reason required if  'other' is selected)</span><br />      
                                       <asp:TextBox ID="txtOtherComments" runat="server" ClientIDMode="Static" CssClass="srtsTextBox_multi" Width="95%" Height="55px" MaxLength="500" TextMode="MultiLine" BorderStyle="None"></asp:TextBox>
                                       </div>
                                        </div>
                        </div>
                    </div>
                    <div id="divSubmit" class="w3-row alignLeft padding" style="padding-bottom:0px;margin-right:0px;margin-top:-40px">
                        <div class="w3-half" style="text-align:right">
                        <asp:Button ID="btnSaveClinicMTP" runat="server" OnClientClick="javascript:return ValidateComments();" OnClick="btnSaveLabMTPPref_Click" Text="Save Changes" CssClass="srtsButton" CausesValidation="false" />
                            &nbsp </div>
                        <div class="w3-half" style="text-align:left">                                                       
                          
                             <asp:Button ID="btnCancelClinicMTP" runat="server" OnClick="btnCancelClinicMTP_Click" Text="Cancel Changes" CssClass="srtsButton" CausesValidation="false" />
                        </div> 
                     </div>
                 

                 </div>
                 <div class="BeigeBoxFooter"></div>
                </div>
              <asp:HiddenField ID="hdfNotificationValue" runat="server" ClientIDMode="Static" />
             <asp:HiddenField ID="hdfNotificationMessage" runat="server" ClientIDMode="Static"/>
         </div>


        <asp:ScriptManagerProxy ID="smpLabMailToPatient" runat="server">
            <Scripts>
                <asp:ScriptReference Path="~/Scripts/Preferences/LabMailToPatient.js" />
            </Scripts>
        </asp:ScriptManagerProxy>
        
    </ContentTemplate>
      <Triggers>
        <asp:AsyncPostBackTrigger ControlID="btnSaveClinicMTP" EventName="Click" />
    </Triggers>
</asp:UpdatePanel>




