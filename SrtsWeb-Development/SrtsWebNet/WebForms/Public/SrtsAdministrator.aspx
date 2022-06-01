<%@ Page Title="About Us" Language="C#" MasterPageFile="~/srtsMaster.master" AutoEventWireup="true"
    CodeBehind="SrtsAdministrator.aspx.cs" Inherits="SrtsWeb.Public.SrtsAdministrator" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <h2>SRTSweb Membership Administrator</h2>
    <div>
        <asp:CreateUserWizard ID="CreateUserWizard1" runat="server">
            <WizardSteps>
                <asp:CreateUserWizardStep ID="CreateUserWizardStep1" runat="server">
                </asp:CreateUserWizardStep>
                <asp:CompleteWizardStep ID="CompleteWizardStep1" runat="server">
                </asp:CompleteWizardStep>
            </WizardSteps>
        </asp:CreateUserWizard>

        <br />
    </div>
</asp:Content>