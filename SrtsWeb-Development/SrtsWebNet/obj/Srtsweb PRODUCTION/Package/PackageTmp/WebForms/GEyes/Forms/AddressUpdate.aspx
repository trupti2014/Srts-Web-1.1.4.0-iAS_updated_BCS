<%@ Page Title="" Language="C#" MasterPageFile="~/GEyes/GeyesMasterResponsive.Master" AutoEventWireup="true"
    CodeBehind="AddressUpdate.aspx.cs" Inherits="GEyes.Forms.AddressUpdate" %>

<asp:Content ID="Content6" ContentPlaceHolderID="MainContent_Public" runat="server">
    <asp:ScriptManagerProxy ID="smpGEyes" runat="server">
        <Scripts>
            <asp:ScriptReference Path="~/Scripts/GEyes/GEyes.js" />
        </Scripts>
    </asp:ScriptManagerProxy>
    <asp:HiddenField ID="hfSuccessGE" runat="server" Value="0" ClientIDMode="Static" />
    <asp:HiddenField ID="hfMsgGE" runat="server" Value="" ClientIDMode="Static" />


  

    <asp:ValidationSummary ID="vsMessage" runat="server" DisplayMode="BulletList" ForeColor="Red" ValidationGroup="addr" CssClass="G-Eyes_ErrorSummary" Enabled="False" />
    <div>
                <asp:Panel ID="pnlAddress" runat="server" Visible="false" CssClass="G-Eyes">
                <div class="container align_center">
                <div class="card cardHeader">
                <h5 class="card-header">
                <span>Shipping Address</span>
                </h5>
                <div class="card-body bg-white text-left">
                      <p class="card-title w3-large w3-padding-large">Enter your shipping address information, then select the &#39;Submit&#39; button to continue.</p>
                        <div id="divSelectedOrder" class="w3-padding-large">
                            <div class="container">
                                <div class="row">
                                  <div id="Address" class="col-sm-6 w3-padding">
                                    <asp:Label ID="lblAddress1" runat="server" Text="Address*"></asp:Label>
                                    <asp:TextBox ID="tbAddress1" runat="server" MaxLength="100" TabIndex="1" ToolTip="Enter the street address." CssClass="form-control"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvAddress1" ControlToValidate="tbAddress1" ErrorMessage="" runat="server" ValidationGroup="addr" Display="Dynamic">* Address is required</asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="revAddress1" runat="server" ControlToValidate="tbAddress1"
                                        ErrorMessage="" ValidationExpression="^[a-zA-Z0-9'.\s-\/]{1,40}$"
                                        Display="Dynamic" ValidationGroup="addr">* Invalid characters in Address</asp:RegularExpressionValidator><br />
                                  </div>

                                  <div id="Address2" class="col-sm-6 w3-padding">
                                        <asp:Label ID="lblAddress2" runat="server" Text="Address (cont'd):"></asp:Label>
                                        <asp:TextBox ID="tbAddress2" runat="server" MaxLength="100" TabIndex="2" ToolTip="Continuation of address." CssClass="form-control"></asp:TextBox>
                                        <asp:RegularExpressionValidator ID="revAddress2" runat="server" ControlToValidate="tbAddress2"
                                            ErrorMessage="" ValidationExpression="^[a-zA-Z0-9'.\s-\/]{1,40}$"
                                            Display="Dynamic" ValidationGroup="addr" CssClass="G-Eyes_validator">* Invalid characters in Address (cont'd)</asp:RegularExpressionValidator>
                                  </div>
                                </div>


                                <div class="row">
                                    <div id="City" class="col-sm-3 w3-padding">
                                         <asp:Label ID="lblCity" runat="server" Text="City:*"></asp:Label>
                                        <asp:TextBox ID="txtCity" runat="server" TabIndex="3" ToolTip="Enter City" CssClass="form-control">
                                        </asp:TextBox>
                                        <asp:RequiredFieldValidator ID="rfvCity" runat="server" ErrorMessage="Must be FPO, APO, or DPO"
                                            ControlToValidate="txtCity" Display="Dynamic" ValidationGroup="addr">* City is required</asp:RequiredFieldValidator>
                                    </div>
                                    <div id="State" class="col-sm-3 w3-padding">
                                         <asp:Label ID="lblState" runat="server" Text="State:*"></asp:Label>
                                        <asp:DropDownList ID="ddlState" runat="server" TabIndex="4" ToolTip="Select State" CssClass="form-control">
                                        </asp:DropDownList>
                                        <asp:RequiredFieldValidator ID="rfvState" runat="server" ErrorMessage=""
                                            ControlToValidate="ddlState" Display="Dynamic" ValidationGroup="addr" InitialValue="-Select-">* State is required</asp:RequiredFieldValidator>
                                    </div>
                                    <div id="Country" class="col-sm-3 w3-padding">
                                         <asp:Label ID="lblCountry" runat="server" Text="Country:*"></asp:Label>
                                        <asp:DropDownList ID="ddlCountry" runat="server" TabIndex="5" ToolTip="Select country." DataTextField="Value" DataValueField="Key" CssClass="form-control" AutoPostBack="False">
                                        </asp:DropDownList>
                                        <asp:RequiredFieldValidator ID="rfvCountry" runat="server" ErrorMessage=""
                                            ControlToValidate="ddlCountry" Display="Dynamic" ValidationGroup="addr" InitialValue="-Select-">* Country is required</asp:RequiredFieldValidator>
                                    </div>
                                    <div id="Zip" class="col-sm-3 w3-padding">
                                          <asp:Label ID="lblZipCode" runat="server" Text="Zip Code:"></asp:Label>
                                            <asp:TextBox ID="tbZipCode" runat="server" TabIndex="6" CssClass="form-control" CausesValidation="False" Enabled="False"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="rfvZipCode" runat="server" Display="Dynamic" ErrorMessage="Zip code is a required.  Enter five numbers or nine numbers with a hyphen (i.e. 55555 or 55555-4444)."
                                                ControlToValidate="tbZipCode" ValidationGroup="addr">* Zip code is a required</asp:RequiredFieldValidator>
                                            <asp:RegularExpressionValidator ID="revZipCode" runat="server" ControlToValidate="tbZipCode"
                                                ErrorMessage="Zip code must be five numbers or nine numbers with a hyphen (i.e. 55555 or 55555-4444)." ValidationExpression="^\d{5}(\-\d{4})?$"
                                                Display="Static" ValidationGroup="addr">* Invalid entry</asp:RegularExpressionValidator>
                                    </div>
                                </div>
                            </div>


                    <div class="w3-center w3-padding">
                        <asp:Button ID="btnNext" runat="server" CssClass="btn btn-secondary text-white" Text="Next" OnClick="btnNext_Click" />
                        <asp:Button ID="btnSubmit" runat="server" CssClass="btn btn-secondary text-white" Text="Submit" OnClick="btnSubmit_Click" ValidationGroup="addr" />&nbsp;
                        <asp:Button ID="btnCancel" runat="server" CssClass="btn btn-secondary text-white" Text="Cancel" OnClick="btnCancel_Click" /><br />
                    </div>
                        </div>
                </div>
                </div>
                </div>
            </asp:Panel>
    </div>



    <asp:Panel ID="pnlGVAddresses" runat="server">
        <br />
        <blockquote style="font-size: medium">
            Please select an existing address to ship this order to, or you can
            <asp:LinkButton ID="lbntAddAddress" runat="server" OnClick="lbntAddAddress_Click" Font-Bold="True">add a new address</asp:LinkButton>&nbsp;if needed.
        </blockquote>
        <br />

        <asp:GridView ID="gvAddresses" runat="server" AutoGenerateColumns="False" GridLines="None" AllowPaging="false" CssClass="mGrid" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
            EmptyDataText="No Addresses On Record" DataKeyNames="ID" CellSpacing="0" Width="100%" OnRowCommand="gvAddresses_RowCommand" AutoGenerateSelectButton="True">
            <PagerSettings Mode="NextPreviousFirstLast" Position="TopAndBottom" FirstPageText="<< First" NextPageText="Next >" PreviousPageText="< Previous" LastPageText="Last >>" />
            <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
            <Columns>

                <asp:TemplateField HeaderText="Type">
                    <ItemTemplate>
                        <asp:Label ID="lblAddressType" runat="server" Text='<%# Eval("AddressType") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Address">
                    <ItemTemplate>
                        <asp:Label ID="lblAddress1" runat="server" Text='<%# Eval("Address1") %>'></asp:Label>
                        <asp:Label ID="lblAddress2" runat="server" Text='<%# Eval("Address2") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="City,State(Country)">
                    <ItemTemplate>
                        <asp:Label ID="lblCity" runat="server" Text='<%# Eval("City") %>'></asp:Label>,
                    &nbsp;
                    <asp:Label ID="lblState" runat="server" Text='<%# Eval("State") %>'></asp:Label>&nbsp;
                    (<asp:Label ID="lblCountry" runat="server" Text='<%# Eval("Country") %>'></asp:Label>)
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Zip Code">
                    <ItemTemplate>
                        <asp:Label ID="lblZipCode" runat="server" Text='<%# Eval("ZipCode") %>' Width="100px"></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="IsActive">
                    <ItemTemplate>
                        <asp:Label ID="lblIsActiveAddr" runat="server" Text='<%# Eval("IsActive") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
            <HeaderStyle HorizontalAlign="Left"></HeaderStyle>

            <PagerStyle CssClass="pgr"></PagerStyle>
            <RowStyle HorizontalAlign="Center" />
        </asp:GridView>
        <br />

        <div align="center">
            <asp:Button ID="btnExit" runat="server" CssClass="srtsButton" Text="Cancel"
                PostBackUrl="~/WebForms/GEyes/Forms/GEyesHomePage.aspx" OnClick="btnCancel_Click" />
            <br />
            <br />
        </div>
    </asp:Panel>




</asp:Content>
