 <%@ Page Title="" Language="C#" MasterPageFile="~/JSpecs/JSpecsMaster.Master" AutoEventWireup="true"
    CodeBehind="JSpecsOrders.aspx.cs" Inherits="JSpecs.Forms.JSpecsOrders" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="Server">
    <script src="../../../Scripts/JSpecs/JSpecsLoaderHandler.js"></script>
    <asp:ScriptManager ID="OrdersScriptManager" runat="server" EnablePartialRendering="true"></asp:ScriptManager>

    <div class="title title--order title--mobile-border">
        <h1 class="title__name">Your Orders</h1>
        <!-- <div class="title__orderBtn">
            <a runat="server" class="btn" disabled='true'>I want to order something different</a>
        </div> -->
    </div>

    <asp:UpdatePanel ID="UpdatePanelOrdersSection" runat="server" UpdateMode="Conditional" style="display: block;">
        <ContentTemplate>
            <article>
                <ul class="orders">
                    <asp:Repeater ID="jspecsOrders" runat="server">
                        <ItemTemplate>
                            <li class="order">
                                <h3 class="order__date">Ordered <%#Eval("DateCreated", "{0:M/d/yyyy}") %></h3>
                                <img runat="server" src='<%# !String.IsNullOrEmpty(Eval("FrameImgName").ToString()) ? setThumbnailImage(String.Format("/JSpecs/{0}{1}.{2}", Eval("FrameImgPath"), Eval("FrameImgName"), Eval("FrameImgType"))) : "/JSpecs/imgs/Fallback/Frame_Not_Available.png" %>' class="order__frame-img" />
                                <p class="order__status">
                                    <%#Eval("OrderStatus")%>
                                </p>
                                <p class="order__frame-info">
                                    <%# Eval("FrameCategory").ToString() != "PMI" ? Eval("RxNameUserFriendly") + ", " + Eval("LensTint").ToString().ToLower() + "<br />" : "" %>
                                    <%# Eval("FrameUserFriendlyName") %>
                                </p>
                                <asp:LinkButton runat="server" ID="lbtnOrderAgain" CssClass=<%# Convert.ToBoolean(Eval("EligibleOrder")) ? "btn btn--small order__button " : "btn--small btn--disabled order__button" %> OnClick="btnReOrder_Click" CommandArgument='<%# Eval("OrderNumber") + ";" + Eval("EligibleOrder") %>' Text="Order Again"></asp:LinkButton>
                                <asp:LinkButton runat="server" ID="btnDisplayOrderDetails" ClientIDMode="AutoID" OnClick="btnDisplayOrderDetails_click" CausesValidation="false" CommandArgument='<%# Eval("OrderNumber") + ";" + Eval("EligibleOrder")%>' class="btn btn--small order__button" Text="Details" />
                            </li>
                        </ItemTemplate>
                    </asp:Repeater>
                </ul>

                <div class="modal__background" id="cantOrderMsg" runat="server">
                    <div class="modal--centered" >
                        <asp:LinkButton runat = "server" ID="lbtnCloseErrorMessage" class="close__button">Close<span class="close__button__circle">X</span></asp:LinkButton>
                    <p class="modal__error-message">It appears that you're not eligible to order this item with the app at this time. Have a question? Check out our FAQ page or contact your Optometry Clinic.</p>
                        <div class="test" style="width: 100%; text-align: center">
                            <asp:LinkButton runat = "server" ID="lbtnOkErrorMessage" CssClass="btn btn--heavy" Height="100%" Text="Ok" />
                        </div>
                        
                    </div>
                </div>
                

                <asp:Button ID="btnOpenOrderDetailsMPEDummy" runat="server" Style="display: none" />
                <ajaxToolkit:ModalPopupExtender ID="MPEOrderDetails" runat="server"
                    TargetControlID="btnOpenOrderDetailsMPEDummy" PopupControlID="UpdatePanelOrderDetails"
                    BackgroundCssClass="Background">
                </ajaxToolkit:ModalPopupExtender>


                <asp:Panel runat="server">
                    <asp:UpdatePanel runat="server" ID="UpdatePanelOrderDetails" UpdateMode="Conditional" Style="display: none;">
                        <ContentTemplate>
                            <div class="modal">
                                <asp:LinkButton runat="server" ID="lbtnCloseOrderDetails" OnClick="btnCloseOrderDetails_click" class="close__button">Close <span class="close__button__circle">X</span></asp:LinkButton>
                                <div>
                                    <div class="modal__order">
                                        <h3>Ordered
                                            <asp:Label runat="server" ID="orderPlaced"></asp:Label></h3>
                                        <img runat="server" id="modalImg" src="/JSpecs/imgs/Fallback/Frame_Not_Available.png" class="modal__order__img" />
                                        <h3 runat="server" id="modalFrameDetails"></h3>
                                    </div>
                                </div>
                                <span class="modal__split"></span>
                                <div>
                                    <div class="modal__order-details">
                                        <h3>Order Details</h3>
                                        <p>
                                            <b>Order Placed:</b>
                                            <asp:Label runat="server" ID="orderPlaced2"></asp:Label>
                                        </p>
                                        <p>
                                            <b>Prescription Date:</b>
                                            <asp:Label runat="server" ID="prescriptionDate"></asp:Label>
                                        </p>
                                        <p>
                                            <b>Prescription:</b>
                                            <asp:Label runat="server" ID="rxName"></asp:Label>
                                        </p>
                                        <table>
                                            <tr>
                                                <th>RX</th>
                                                <th>Sphere (SPH)</th>
                                                <th>Cylinder (Cyl)</th>
                                                <th>Axis</th>
                                                <th>Addition (ADD)</th>
                                            </tr>
                                            <tr>
                                                <th>OD (Right Eye)</th>
                                                <td runat="server" id="odSph"></td>
                                                <td runat="server" id="odCyl"></td>
                                                <td runat="server" id="odAxis"></td>
                                                <td runat="server" id="odAdd"></td>
                                            </tr>
                                            <tr>
                                                <th>OS (Left Eye)</th>
                                                <td runat="server" id="osSph"></td>
                                                <td runat="server" id="osCyl"></td>
                                                <td runat="server" id="osAxis"></td>
                                                <td runat="server" id="osAdd"></td>
                                            </tr>
                                        </table>
                                        <p>
                                            <b>PD:</b>
                                            <asp:Label runat="server" ID="pd"></asp:Label></p>
                                        <p>
                                            <b>Mailed/dispensed:</b>
                                            <asp:Label runat="server" ID="orderStatus"></asp:Label></p>
                                        <p>
                                            <b>Mailing Address:</b>
                                            <asp:Label runat="server" ID="mailingAddress">N/A</asp:Label></p>
                                        <p>
                                            <b>Tracking Number</b> : N/A</p>
                                        <div class="modal__btn-container">
                                            <asp:LinkButton runat="server" ID="lbtnModalOrderAgain" OnClick="btnReOrder_Click" CssClass="btn btn--heavy" Height="100%" Text="Order Again" />
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </asp:Panel>
            </article>
        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>
