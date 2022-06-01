<%@ Page Language="C#" MasterPageFile="~/SrtsMaster.Master" AutoEventWireup="true" CodeBehind="ManageEventLogTracing.aspx.cs" Inherits="SrtsWeb.WebForms.Admin.ManageEventLogTracing" %>


<%@ MasterType VirtualPath="~/srtsMaster.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
 <style>
     .hexead9c8 {
     background-color:#ead9c8;
     }
     .hexf8f2ec {
     background-color:#f8f2ec;
     }
     .hexfff {
     background-color:#fff;
     }
     .inline-rb input[type="radio"] {
        width: auto;
        }

    .inline-rb label {
        display: inline;
        padding-left:4px;
}

     .pnl {
        padding:20px 0px;
     }

     .adminlevel, .loginlevel, .clinicManagementlevel, .clinicOrderlevel, .labManagementlevel, .labOrderlevel, .examlevel, .prescriptionlevel, .personlevel{
        position:relative;
     }
     .adminlevel {top:0px;}
     .loginlevel {top:-35px;}
     .clinicManagementlevel {top:-70px;}
     .clinicOrderlevel {top:-105px;}
     .labManagementlevel {top:-140px;}
     .labOrderlevel {top:-175px;}
     .examlevel {top:-210px;}
     .prescriptionlevel {top:-245px;}
     .personlevel {top: -280px;}
     .log {}
     .logName {float:left;clear:both;width:200px;}
     .logcontent{padding-left:100px;}


     #divSingleColumns a {
     text-decoration:none;
     width:250px;
     }

     h4 {
      height:35px;
      font-variant: normal;
     }


     h4:hover {
     font-weight:bold;
     color:#782E1E;

     }

     .active {
       font-weight:bold;
       color:#782E1E!important;
     }
 </style>
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="MainContent" runat="server">
     <asp:ScriptManagerProxy ID="smpProxy" runat="server">
        <Scripts>
            <asp:ScriptReference Path="~/Scripts/EventLogs/ManageEventLogTracing.js" />
            <asp:ScriptReference Path="~/Scripts/Global/PassFailConfirm.js" />
            <asp:ScriptReference Path="~/Scripts/Global/GlobalVal.js" />
        </Scripts>
    </asp:ScriptManagerProxy>

<div id="divSingleColumns" style="margin: 5px 0px 20px 0px;padding-bottom:20px;border-bottom:1px solid #ebd9c7">
    <div class="w3-row">
                <div class="w3-container">          
                <div class="padding" style="text-align:left">
               <div class="w3-col" style="width:800px">
                    <h1 style="border-bottom:solid 1px #ead9c8" class="w3-large">Trace Logs</h1>
                    <h1 style="margin-left:300px;margin-top:-25px" class="w3-large">Trace Log Current Level<span id="logLevelTitle"></span></h1>
                    <div style="margin:20px 0px 30px 10px">                                          
                        <div class="log">
                            <!-- Panel - Admin Events Log Settings  -->
                            <div class="logName w3-half"><a href="#"><h4>Admin Events</h4></a></div>
                            <div id="pnlAdmin" class="logcontent adminlevel w3-half">
                                <h2 style="padding-bottom:5px">Admin Events Current Log Level:</h2>
                                <asp:TextBox ID="txtAdmin" runat="server" CssClass="srtsTextBox_medium"></asp:TextBox>
                            </div>
                            <!-- Panel - Login Log Settings  -->
                            <div class="logName w3-half"><a href="#"><h4>Login Events</h4></a></div>
                            <div id="pnlLogin" class="logcontent loginlevel w3-half">
                                <h2 style="padding-bottom:5px">Login Events Current Log Level:</h2>
                                <asp:TextBox ID="txtLogin" runat="server" CssClass="srtsTextBox_medium"></asp:TextBox>
                            </div>
                            <!-- Panel - Clinic Management Trace Log Settings -->       
                            <div class="logName w3-half"><a href="#"><h4>Clinic Management Events</h4></a></div>
                            <div id="pnlClinicManagement" class="logcontent clinicManagementlevel w3-half">
                                <h2 style="padding-bottom:5px">Clinic Management Events Current Log Level:</h2>
                                <asp:TextBox ID="txtClinicManagement" runat="server" CssClass="srtsTextBox_medium"></asp:TextBox>
                            </div>
                            <!-- Panel - Clinic Orders Trace Log Settings -->       
                            <div class="logName w3-half"><a href="#"><h4>Clinic Orders Events</h4></a></div>
                            <div id="pnlClinicOrders" class="logcontent clinicOrderlevel w3-half">
                                <h2 style="padding-bottom:5px">Clinic Orders Events Current Log Level:</h2>
                                <asp:TextBox ID="txtClinicOrders" runat="server" CssClass="srtsTextBox_medium"></asp:TextBox>
                            </div>  
                             <!-- Panel - Lab Management Trace Log Settings -->       
                            <div class="logName w3-half"><a href="#"><h4>Lab Management Events</h4></a></div>
                            <div id="pnlLabManagement" class="logcontent labManagementlevel w3-half">
                                <h2 style="padding-bottom:5px">Lab Management Events Current Log Level:</h2>
                                <asp:TextBox ID="txtLabManagement" runat="server" CssClass="srtsTextBox_medium"></asp:TextBox>
                            </div>                            
                             <!-- Panel - Lab Orders Trace Log Settings -->       
                            <div class="logName w3-half"><a href="#"><h4>Lab Orders Events</h4></a></div>
                            <div id="pnlLabOrders" class="logcontent labOrderlevel w3-half">
                                <h2 style="padding-bottom:5px">Lab Orders Events Current Log Level:</h2>
                                <asp:TextBox ID="txtLabOrders" runat="server" CssClass="srtsTextBox_medium"></asp:TextBox>
                            </div>    
                             <!-- Panel - Exam Events Trace Log Settings -->       
                            <div class="logName w3-half"><a href="#"><h4>Exam Events</h4></a></div>
                            <div id="pnlExam" class="logcontent examlevel w3-half">
                                <h2 style="padding-bottom:5px">Exam Events Current Log Level:</h2>
                                <asp:TextBox ID="txtExams" runat="server" CssClass="srtsTextBox_medium"></asp:TextBox>
                            </div>
                             <!-- Panel - Prescription Events Trace Log Settings -->  
                            <div class="logName w3-half"><a href="#"><h4>Prescription Events</h4></a></div>
                            <div id="pnlPrescription" class="logcontent prescriptionlevel w3-half">
                                <h2 style="padding-bottom:5px">Prescription Events Current Log Level:</h2>
                                <asp:TextBox ID="txtPrescriptions" runat="server" CssClass="srtsTextBox_medium"></asp:TextBox>
                            </div>
                            <!-- Panel - Person Events Trace Log Settings -->  
                            <div class="logName w3-half"><a href="#"><h4>Person Events</h4></a></div>
                            <div id="pnlPerson" class="logcontent personlevel w3-half">
                                <h2 style="padding-bottom:5px">Person Events Current Log Level:</h2>
                                <asp:TextBox ID="txtPerson" runat="server" CssClass="srtsTextBox_medium"></asp:TextBox>
                            </div>                                                                          
                        </div>

                    </div>       
               </div>
               <div class="w3-rest">
                   <h1 style="border-bottom:solid 1px #ead9c8" class="w3-large">Modify Trace Log Level</h1>                   
                     <div style="margin:20px 0px 0px 10px">
                     <h2>Select an option below to change the current trace level.</h2>      
                      <div style="margin:20px 0px 0px 30px">
                       <asp:RadioButtonList ID="rblLoginSwitchLevel" runat="server" RepeatDirection="Vertical" ToolTip="Select Trace Level" 
                          TextAlign="Right" CssClass="inline-rb" CausesValidation="false" ClientIDMode="Static" onchange="ShowSaveSwitchLevel();">
                            <asp:ListItem Text="  Off" Value="Off" />
                            <asp:ListItem Text="  Error" Value="Error" />
                            <asp:ListItem Text="  Warning" Value="Warning" />
                            <asp:ListItem Text="  Information" Value="Information" />
                            <asp:ListItem Text="  Verbose" Value="Verbose" />
                      </asp:RadioButtonList>
                      </div>
                        <div style="float:left;padding:30px 0px 0px 0px;">
                            <asp:HiddenField ID="hdfLogName" runat="server" ClientIDMode="Static" Value="Admin Events" /><br />
                            <asp:Button ID="btnSaveNewLevel" runat="server" ClientIDMode="Static" Text="Save"  CssClass="srtsButton" OnClientClick="SaveTraceSwitchLevel()" />
                        </div>
                     </div>
               </div>                        
           </div>

   
           </div>
    </div>
</div>
    <script>
        var logPanels = $('.log > .logcontent').hide();
        var hdfLogName = document.getElementById('<%= hdfLogName.ClientID %>');




     </script>
</asp:Content>
