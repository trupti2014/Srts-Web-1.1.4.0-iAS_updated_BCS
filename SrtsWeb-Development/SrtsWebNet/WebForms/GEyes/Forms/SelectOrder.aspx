<%@ Page Title="" Language="C#" MasterPageFile="~/GEyes/GEyesMasterResponsive.Master" AutoEventWireup="true"
    CodeBehind="SelectOrder.aspx.cs" Inherits="GEyes.Forms.SelectOrder" EnableEventValidation="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script src="../../../Scripts/Global/SrtsCustomValidators.js"></script>
    <script type="text/javascript">
        function getlblRemainingID() {
            var lblID = '<%=lblRemaining.ClientID%>';
            return lblID;
        }
        function gettbCommentID() {
            var tbID = '<%=tbComment.ClientID%>';
            return tbID;
        }
    </script>
<style>
* {
  box-sizing: border-box;
}

body {
  font-family: Arial, Helvetica, sans-serif;
  min-width:600px;
}

/* Responsive columns */
@media screen and (max-width: 600px) {
  .columnmin {
    width: 100%;
    display: block;
    margin-bottom: 20px;
    text-align:left;
  }
}

/* Style the cards */
.card {
  box-shadow: 0 4px 8px 0 rgba(0, 0, 0, 0.2);
  padding: 0px;
  text-align: left;
  background-color: #f1f1f1;
  width:100%;

}

.card_orders {
  box-shadow: 0 4px 8px 0 rgba(0, 0, 0, 0.2);
  padding: 0px;
  text-align: left;
  background-color: #f1f1f1;


}

.w3-container {
    margin-top:20px;
    background-color:#fff;
    min-height:100px;
    padding:5px;
    font-size:12px;
    width:100%;
}

.cardLabelHeader{
  font-weight:bold;
  margin-top:10px;
  font-size:12px;

}
.cardLabel{
  margin-left:10px;
}

.content{
    text-align:left;
    padding:0px 20px 10px 20px;
    font-size:12px!important;   
}


</style>
    
</asp:Content>

<asp:Content ID="Content6" ContentPlaceHolderID="MainContent_Public" runat="server">
               <asp:UpdatePanel ID="uplOrders" runat="server" ChildrenAsTriggers="true">
                    <ContentTemplate>
        <asp:Panel ID="pnlDisplay" runat="server" Visible="true" style="text-align:left">
            <div class="w3-row" style="max-width:1180px">
                <div class="w3-half">
                <div class="text-left" style="padding-left:100px">
                    <a href="https://www.med.navy.mil/sites/nostra/Pages/Spectacles.aspx">Click here to see pictures of the frames</a><br />
                    <%--<a href="https://srtsweb.amedd.army.mil/WebForms/Public/CheckOrderStatus.aspx">Click here for order status</a>--%>
                    <a runat="server" href="~/WebForms/Public/CheckOrderStatus.aspx">Click here for order status</a>
                </div>

                </div>
                 <div class="w3-half text-right">
                      <asp:Button ID="btnCancelTop" runat="server" CssClass="btn bg-secondary text-white" Text="Cancel" OnClick="btnCancel_Click" PostBackUrl="~/WebForms/GEyes/Forms/GEyesHomePage.aspx" />

                 </div>
           
            </div>
            <div class="container">
             <div class="card cardHeader">
              <h5 class="card-header w3-center">Order History</h5>
                <div class="card-body bg-white text-center ">
                    <div class="w3-row">
                        <div class="text-center">
                            <p class="card-title w3-large w3-padding-small"> Welcome <asp:Label ID="lblWelcome" runat="server" Text=""></asp:Label>!<br />  We have found the following Order History for you. 
                                <asp:Literal ID="litCardTitle" runat="server" Text="Please select the item you would like to re-order."></asp:Literal>
                            </p> 
                             
                        </div>
                    </div>

                    <div id="divOrderCards" runat="server" class="container" style="margin-top:-20px;text-align:center;width:100%">
     
                   <asp:GridView ID="Gridview2" runat="server" AutoGenerateColumns="false" DataKeyNames="OrderNumber" AllowSorting ="true"  OnRowDataBound="Gridview2_RowDataBound" OnRowCommand="Gridview2_RowCommand" OnSorting="Orders_Sorting" Width="100%"  GridLines="None" EmptyDataText="No Orders On Record" Visible="true">
                       <Columns>
                           <asp:TemplateField>
                               <ItemTemplate>
                                 <div class="w3-col l12 w3-center w3-padding">
                                     <div class="card_orders" style="width:auto!important">
                                        <%-- // Item Header--%>
                                      <div class="w3-padding" style="height:20px">
                                          <div class="w3-half">
                                            <label class="cardLabelHeader"> 
                                            <asp:LinkButton ID="lnkItemOrderDate" runat="server" CommandName="Sort" 
                                                CommandArgument="DateCreated">Order Date:</asp:LinkButton>
                                            </label> <%# (Eval("DateCreated", "{0:d}"))%>
                                          </div>                                      
                                      </div>
                                          <%-- // Item Content--%>
                                      <div class="content w3-container text-left" style="padding:15px 20px 15px 20px">
                                          <div class="w3-row" style="margin-bottom:20px">
                                            <div class="w3-third text-left" style="margin-bottom:20px">
                                                 <span class="cardLabelHeader">  
                                                <asp:LinkButton ID="lnkOrderNumber" runat="server" CommandName="Sort" 
                                                CommandArgument="OrderNumber">Order Number</asp:LinkButton></span>
                                                <br /> <%#Eval("OrderNumber")%>
                                            </div>


                                          <div class="w3-twothird text-left"> 
                                              <span class="cardLabelHeader">
                                                <asp:LinkButton ID="lnkFrameDescription" runat="server" CommandName="Sort" 
                                                CommandArgument="FrameDescription">Frame Description</asp:LinkButton></span>
                                                <br /> <%#Eval("FrameDescription")%>
                                          </div>
                                          </div>
                                          <div class="w3-row">
                                          <div class="w3-third text-left" style="margin-bottom:20px">
                                              <span class="cardLabelHeader">
                                                <asp:LinkButton ID="lnkLensTint" runat="server" CommandName="Sort" 
                                                CommandArgument="LensTint">Lens Tint</asp:LinkButton></span>
                                                <br /> <%#Eval("LensTint")%>
                                          </div>
                                          <div class="w3-third text-left" style="margin-bottom:20px">
                                               <span class="cardLabelHeader">
                                                <asp:LinkButton ID="lnkLensCoating" runat="server" CommandName="Sort" 
                                                CommandArgument="LensCoating">Lens Coating</asp:LinkButton></span>
                                                <br /> <%#Eval("LensCoating")%>
                                          </div>
                                               <div class="w3-third text-left" style="margin-bottom:20px">
                                              <span class="cardLabelHeader">
                                                <asp:LinkButton ID="lnkLensType" runat="server" CommandName="Sort" 
                                                CommandArgument="LensTypeLong">Lens Type</asp:LinkButton></span>
                                                <br /> <%#Eval("LensTypeLong")%>
                                          </div>
                                          </div>

                                      </div>
                                          <%-- // Item Select--%>
                                         <div class="text-center">
                                          <asp:LinkButton ID="lnkButton" runat="server" CommandName="Select" CommandArgument='<%#Eval("OrderNumber") %>' Text="select" ></asp:LinkButton>
                                             </div>
                                      </div>
                                  </div>
                               </ItemTemplate>
                               
                           </asp:TemplateField>
                       </Columns>
                       </asp:GridView>
       
                    </div> 
               </div>
              </div> 
             </div>
 <%--            <asp:GridView ID="gvOrders" runat="server" GridLines="None" EmptyDataText="No Orders On Record"
                AutoGenerateColumns="False" HeaderStyle-HorizontalAlign="Left" RowStyle-HorizontalAlign="Left"
                CssClass="mGrid" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                AutoGenerateSelectButton="True" RowStyle-Wrap="false" DataKeyNames="OrderNumber"
                OnSelectedIndexChanged="gvOrders_SelectedIndexChanged" AllowSorting="True" OnSorting="Orders_Sorting" Visible="false">
                <HeaderStyle HorizontalAlign="Left" />
                <PagerSettings Mode="NextPreviousFirstLast" Position="TopAndBottom" FirstPageText="<< First"
                    NextPageText="Next >" PreviousPageText="< Previous" LastPageText="Last >>" />
                <AlternatingRowStyle CssClass="alt" />
                <Columns>
                    <asp:BoundField DataField="OrderNumber" HeaderText="Order Number" ReadOnly="True" SortExpression="OrderNumber" />
                    <asp:BoundField DataField="FrameDescription" HeaderText="Frame Description" ReadOnly="true" SortExpression="FrameDescription" />
                    <asp:BoundField DataField="LensTint" HeaderText="Lens Tint" ReadOnly="true" SortExpression="LensTint" />
                    <asp:BoundField DataField="LensCoating" HeaderText="Lens Coating" ReadOnly="true" SortExpression="LensCoating" />
                    <asp:BoundField DataField="LensTypeLong" HeaderText="Lens Type" ReadOnly="true" SortExpression="LensTypeLong" />
                    <asp:BoundField DataField="DateCreated" HeaderText="Order Date" ReadOnly="true" SortExpression="DateCreated" />
                </Columns>
       
                <PagerStyle CssClass="pgr" />
                <RowStyle HorizontalAlign="Left" Wrap="False" />
            </asp:GridView>--%>

           <div class="w3-padding-16" style="text-align: center">
                <asp:Button ID="btnCancel2" runat="server" CssClass="btn bg-secondary text-white" Text="Cancel" OnClick="btnCancel_Click" PostBackUrl="~/WebForms/GEyes/Forms/GEyesHomePage.aspx" />
            </div>
            <br />
            <br />
        </asp:Panel>




            <asp:Panel ID="pnlSelected" runat="server" Visible="false" CssClass="G-Eyes">
                  <div class="container align_center">
                    <div class="card cardHeader">
                <h5 class="card-header text-center">
                <span> Selected Order </span>
                </h5>
                <div class="card-body bg-white text-left">
                      <p class="card-title w3-large w3-padding-large">Your selected order is displayed below.  Enter a comment if desired, then select the 'Submit' button to continue.</p>
                        <div id="divSelectedOrder" class="w3-padding-large">
                

                          <div id="Frame" class="w3-padding">
                                    <label>Frame</label>
                                    <asp:TextBox ID="tbFrameDesc" runat="server" CssClass="form-control" ToolTip="Frame Desription" Enabled="false" MaxLength="10"></asp:TextBox>
                          </div>
                          <div id="LensTine" class="w3-padding">
                                    <label>Lens Tint</label>
                                    <asp:TextBox ID="tbLensTint" runat="server" CssClass="form-control" ToolTip="Lens Tint" Enabled="false" MaxLength="10"></asp:TextBox>
                          </div>
                         <div id="LensCoating" class="w3-padding">
                                    <label>Lens Coating</label>
                                    <asp:TextBox ID="tbLensCoating" runat="server" CssClass="form-control" ToolTip="Lens Coating" Enabled="false" MaxLength="10"></asp:TextBox>
                          </div>
                         <div id="Lens Type" class="w3-padding">
                                    <label>Lens Type</label>
                                    <asp:TextBox ID="tbLensType" runat="server" CssClass="form-control" ToolTip="Lens Coating" Enabled="false" MaxLength="10"></asp:TextBox>
                          </div>
 
                          <div id="Commments" class="w3-padding">
                            <asp:Label ID="lblComment" Style="padding-right: 10px" runat="server" Text="Comment"></asp:Label>
                            <span class="w3-small"><asp:Label ID="lblRemaining" Style="clear: left; text-align: right; padding-right: 10px" runat="server"></asp:Label></span>
                            <asp:TextBox ID="tbComment" runat="server" CssClass="form-control" TextMode="MultiLine" onKeyDown="return textboxMaxCommentSize(this, 90, event, getlblRemainingID(), gettbCommentID())" ToolTip="Valid characters incude ( ) , . ? # + -" ControlToValidate="tbComment"></asp:TextBox><br />
                            <div style="margin-left: 285px; text-align: center">
                            <asp:RegularExpressionValidator ID="RegExValidatorComment" runat="server" Display="Dynamic" ControlToValidate="tbComment" ValidationExpression="^[a-zA-Z0-9\.\s()\?#,+-]{1,90}$" ValidationGroup="allValidation">(Invalid or too many characters entered)</asp:RegularExpressionValidator>
                            <asp:CustomValidator ID="cvComment" runat="server" ErrorMessage="CustomValidator" EnableClientScript="False" OnServerValidate="ValidateCommentFormat" ValidationGroup="allValidation" Text="(Invalid or too many characters entered)" ControlToValidate="tbComment" ValidateEmptyText="True"></asp:CustomValidator>
                            </div>
                            </div>

                        <div style="text-align: center">
                            <asp:Button ID="btnAdd" runat="server" CssClass="btn bg-secondary text-white" Text="Submit" OnClick="btnAdd_Click" ValidationGroup="allValidation" />&nbsp;
                            <asp:Button ID="btnCancel" runat="server" CssClass="btn bg-secondary text-white" Text="Back" OnClick="btnCancel_Click" />
                        </div>

                        </div>
                </div>
            </div>
                  </div>
            </asp:Panel>

                  </ContentTemplate>
                    <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="Gridview2" EventName="RowCommand" />
                    </Triggers>
                    </asp:UpdatePanel>


</asp:Content>