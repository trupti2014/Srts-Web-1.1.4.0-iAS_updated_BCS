<%@ Page Title="" Language="C#" MasterPageFile="~/JSpecs/JSpecsMaster.Master" AutoEventWireup="true"
    CodeBehind="JSpecsHomePage.aspx.cs" Inherits="JSpecs.Forms.JSpecsHomePage" %>

<asp:Content ID="Content6" ContentPlaceHolderID="MainContent" runat="server">
    <asp:Panel ID="pnlSecurityMessage" runat="server" CssClass="pnlSecurityMessage" Visible="true">
        <div class="align_center">

            <div class="contentTitleleft" style="text-align: center; margin-top: 20px">
                <h2 style="text-align: center">
                    <asp:Literal ID="litModuleTitle" runat="server" Text="JSpecs User Agreement" />
                </h2>
                <div border-bottom: 10px solid #E7CFAD; width: 100%">
                    <h1 class="headerBlue">Welcome to JSpecs - Please Read and Accept the User Agreement to Proceed</h1>
                </div>
            </div>
                <div id="SecurityMessage_MainContent" margin: 0 auto">
                    <p>You are accessing a U.S. Government (USG) information system (IS) (which includes any device attached to this information system) that is provided for U.S. Government authorized use only.</p>
                    <p>You consent to the following conditions: </p>
                    <p>
                        The U.S. Government routinely intercepts and monitors communications on this information system for purposes including, but not limited to, penetration testing, communications security (COMSEC) monitoring, network operations and defense, personnel misconduct (PM), law enforcement (LE), and counterintelligence (CI) investigations.
                    </p>
                    <p>
                        At any time, the U.S. Government may inspect and seize data stored on this information system. Communications using, or data stored on, this information system are not private, are subject to routine monitoring, interception, and search, and may be disclosed or used for any U.S. Government-authorized purpose. .....
                    </p>
                </div>
                    <asp:LinkButton ID="acceptEULA" class="btn btn--large" runat="server" OnClick="GoToLogin" Text="I Have Read and Agree to Terms" style="max-width: 300px; margin: 0 auto" /> 
            </div>
    </asp:Panel>
</asp:Content>
