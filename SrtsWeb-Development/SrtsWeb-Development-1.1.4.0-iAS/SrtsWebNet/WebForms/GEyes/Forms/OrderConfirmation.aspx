<%@ Page Title="" Language="C#" MasterPageFile="~/GEyes/GeyesMasterResponsive.Master" AutoEventWireup="true" CodeBehind="OrderConfirmation.aspx.cs" Inherits="GEyes.Forms.OrderConfirmation" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        function hideSubmitBtn(btn) {
            btn.disabled = true;
        }
    </script>
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="MainContent_Public" runat="server">
      <asp:UpdatePanel ID="uplOrderConfirmation" runat="server" ChildrenAsTriggers="true">
          <ContentTemplate>
        <asp:Panel ID="pnlConfirm" runat="server" Visible="false" CssClass="G-Eyes">
        <div class="container align_center">
        <div class="card cardHeader">
        <h5 class="card-header"><span>Order Submission</span></h5>
        <div class="card-body bg-white text-center">
                <p class="card-title w3-large w3-padding-large">Your order is ready to be submitted!</p>
          <div id="captchaOrderConfirmation"  style="margin-top: 30px !important; margin-bottom: 12px !important;">
                 <table style="width: 100%; margin-top: 30px;">
                    <tr>
                    <td style="width: 365px"></td>
                    <td style="text-align: left;" >
                            <asp:Label ID="CaptchaLabel" runat="server" AssociatedControlID="OrderConfirmationCaptchaCode">Retype the characters from the picture:</asp:Label>
                        
                    </td>
                    <td></td>
                    </tr>
                    <tr>
                    <td></td>
                    <td style="align-content: left;" >
                        <BotDetect:WebFormsCaptcha ID="OrderConfirmationCaptcha" runat="server" UserInputID="OrderConfirmationCaptchaCode"  />
                    </td>
                    <td></td>
                    </tr>
                    <tr>
                    <td></td>
                    <td style="text-align: left;">
                        <div>
                            <asp:TextBox ID="OrderConfirmationCaptchaCode" runat="server" Width="250px"/>
                        </div>
                        </td>
                    <td>
                    </td>
                    </tr>
                    <tr>
                    <td colspan="3" style="text-align: center;">
                        <asp:RequiredFieldValidator ID="rfvCaptchaCode" runat="server" ControlToValidate="OrderConfirmationCaptchaCode" CssClass="requestValidator" ErrorMessage="Retyping the code from the picture is required" />
                    </td>
                    <td></td>
                    <td></td>
                    </tr>
                   <tr>
                    <td colspan="3" style="text-align: center;">
                        <div>
                            <asp:CustomValidator ID="cvOrderConfirmationCaptcha" runat="server" EnableClientScript="False" ControlToValidate="OrderConfirmationCaptchaCode" ErrorMessage="Incorrect CAPTCHA code. Please retype the code from the picture." OnServerValidate="OrderConfirmationCaptchaValidator_ServerValidate" />
                        </div>
                    </td>
                    <td></td>
                    <td></td>
                    </tr>
                    </table>
            </div>
              
                      <p class="card-title w3-large w3-padding-large">Click the &#39;Submit&#39; button to confirm submission or &#39;Cancel&#39; to cancel this order.</p>
           
                     <div class="w3-padding" style="text-align:center">
                            <div id="Submit" class="w3-padding">
                                <asp:Button ID="btnSubmit" runat="server" CssClass="btn btn-secondary text-white" Text="Submit" OnClick="btnSubmit_Click" UseSubmitBehavior="False" /> <%--OnClientClick="hideSubmitBtn(this)" --%>&nbsp;&nbsp;
                                <asp:Button ID="btnCancel" runat="server" CssClass="btn btn-secondary text-white" Text="Cancel" OnClick="btnCancel_Click" />
                            </div>
                     </div>
        </div>
        </div>
        </div>
        </asp:Panel>

        <asp:Panel ID="pnlSuccess" runat="server" Visible="false" CssClass="G-Eyes">
        <div class="container align_center">
        <div class="card cardHeader">
        <h5 class="card-header"><span>Order Submitted for Approval - Important - Please Read!</span></h5>
        <div class="card-body bg-white text-left">
                <div class="w3-center"><h3>IMPORTANT INFORMATION:</h3>
                <blockquote class="G-Eyes_warning">
                    READ BEFORE YOU FINISH!
                </blockquote></div>
                <div id="divSubmitedComplete" class="w3-padding-large">
       <%--             <div class="container">--%>
                        <div class="row">
                            <div class="w3-padding w3-center">
                                <p> Your order has been saved and once approved will be submitted for production.
                                <asp:Label ID="lblMessage" runat="server" Font-Bold="true" Visible="False"></asp:Label> </p>
                                <p>Your order number is: <asp:Label ID="lblOrderNumber" runat="server" Font-Bold="true"></asp:Label>.
                                    Please either write the order number down or print this page for reference.
                                </p>
                                <p>Thank you for using the G-Eyes order process.</p>
                            </div>
                        </div>
                <%--    </div>--%>
                </div>
        </div>
        </div>
        </div>
            <br /><br />
            <div class="w3-enter">
            <asp:Button ID="btnEixt" runat="server" CssClass="btn btn-secondary text-white" Text="Exit" OnClick="btnEixt_Click" />
            <br />
            <br />
        </div>

        </asp:Panel>
                  </ContentTemplate>
                    <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="btnSubmit" EventName="Click" />
                    </Triggers>
                    </asp:UpdatePanel>

</asp:Content>