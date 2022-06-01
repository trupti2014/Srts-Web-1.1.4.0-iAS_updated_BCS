<%@ Page Title="" Language="C#" MasterPageFile="~/GEyes/GEyesMasterResponsive.Master" AutoEventWireup="true"
    CodeBehind="GEyesHomePage.aspx.cs" Inherits="GEyes.Forms.GEyesHomePage" %>

<asp:Content ID="Content6" ContentPlaceHolderID="MainContent_Public" runat="server">
<%--Panel - User Agreement--%>
    <asp:Panel ID="pnlSecurityMessage" runat="server" CssClass="pnlSecurityMessage" Visible="true">
        <div class="container align_center">
            <div class="card cardHeader">
              <h5 class="card-header">G-Eyes User Agreement</h5>
              <div class="card-body bg-white text-left w3-padding-24">
                <p class="card-title w3-large w3-padding-large">Please read and accept the G-Eyes User Agreement.</p>
                  <div id="SecurityMessage_MainContent" class="w3-padding-large">
                    <p>You are accessing a U.S. Government (USG) information system (IS) (which includes any device attached to this information system) that is provided for U.S. Government authorized use only.</p>
                  
                    <p>You consent to the following conditions: </p>
               
                    <p class="indent">
                        The U.S. Government routinely intercepts and monitors communications on this information system for purposes including, but not limited to, penetration testing, communications security (COMSEC) monitoring, network operations and defense, personnel misconduct (PM), law enforcement (LE), and counterintelligence (CI) investigations.
                    </p>
                
                    <p>
                        At any time, the U.S. Government may inspect and seize data stored on this information system. Communications using, or data stored on, this information system are not private, are subject to routine monitoring, interception, and search, and may be disclosed or used for any U.S. Government-authorized purpose.
                    </p>
                </div>

                <div class="w3-padding-24 text-center">
                      <asp:LinkButton ID="lnkSecurityAcknowledged" class="btn btn- bg-secondary text-white" runat="server" OnCommand="SecurityAcknowledged">I Accept</asp:LinkButton>
                                   &nbsp;&nbsp;
                     <asp:LinkButton ID="lnkSecurityAcknowledgedCancel" class="btn bg-secondary text-white" runat="server" CausesValidation="False" OnCommand="btnCancel_Click" ClientIDMode="Static">Cancel</asp:LinkButton>
                </div>
              </div>
            </div>
        </div>
    </asp:Panel>

<%--Panel - Important Information--%>
<asp:Panel ID="pnlMain" runat="server" Visible="False">
 <div class="container align_center">
            <div class="card cardHeader">
              <h5 class="card-header">Important Information to Note</h5>
              <div class="card-body bg-white text-left w3-padding-24">
                <p class="card-title w3-large w3-padding-large">Please note the following before continuing.</p>
                  <div class="w3-large w3-padding-large">
                        <div class="ulGEyes">
                        <ul>
                            <li>Please do not order unnecessary glasses or inserts.</li>
                            <li>Your spectacle frame may be changed due to inventory levels at the optical lab.</li>
                            <li>Temporarily available to all Active Duty.</li>
                            <li>You must have an optical order on file to use this application.</li>
                            <li>The optical lab will complete the order and send it to you.</li>
                            <li>It may take up to six(6) weeks to receive your order.</li>
                        </ul>
                        </div>
                    </div>
                    <%--panel buttons--%>
                        <div class="w3-padding-24 text-center">
                            <asp:LinkButton ID="lnkNext" class="btn bg-secondary text-white" runat="server" CausesValidation="False" OnCommand="btnNext_Click" ClientIDMode="Static">Next</asp:LinkButton>
                            &nbsp;&nbsp;
                            <asp:LinkButton ID="lnkCancel" class="btn bg-secondary text-white" runat="server" CausesValidation="False" OnCommand="btnCancel_Click" ClientIDMode="Static">Cancel</asp:LinkButton>
                         </div>
              </div>
            </div>
</div>
</asp:Panel>
</asp:Content>