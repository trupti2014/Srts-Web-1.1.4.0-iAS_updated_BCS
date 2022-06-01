<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ucSitePreferencesRoutingOrders.ascx.cs" Inherits="SrtsWeb.UserControls.ucSitePreferencesRoutingOrders" %>

<asp:UpdatePanel ID="upRoutingOrders" runat="server" UpdateMode="conditional" ChildrenAsTriggers="true">
    <ContentTemplate>
        <asp:HiddenField ID="hfSuccessRO" runat="server" Value="0" ClientIDMode="Static" />
        <asp:HiddenField ID="hfMsgRO" runat="server" Value="" ClientIDMode="Static" />
        <div class="w3-row alignLeft" style="margin-top: -30px; height: auto;" >
        <div class="BeigeBoxContainer" style="margin: 10px 0px 20px 0px; height: auto">
                <div class="BeigeBoxHeader" style="text-align: left; position: relative; top: 0px; padding: 12px 10px 3px 15px">
                    <span class="label">Routing Orders</span>
                </div>
                <div class="BeigeBoxContent padding" style="margin-top: 0px; margin-left: 10px; height: auto">
                    <div id="divCurrentLabCapacity">
                    <%--<asp:HiddenField ID="hfMatParamID" runat="server" />--%>
                    <div class="sitepreferencegroupbox"> <%---Border around Material/Cylinder---%>
                    <div class="w3-row alignLeft">
                        <div class="padding" style="padding: 5px 0px;">
                            <h1>Current Lab Capacity:</h1>
                            <div class="w3-row w3-padding" >
                                <div class="w3-col s2 w3-left" style="width: 40%;padding-left:20px">
                                    <asp:Label ID="Label3" runat="server" Text="Daily Capacity" CssClass="srtsLabel_medium" /><br />
                                    <asp:TextBox ID="tbDailyCapacity" runat="server" CssClass="srtsTextBox_medium" Width="90px" ClientIDMode="Static" TabIndex="2" /><%--onchange="ParamInput('tbCyl')" --%>
                                </div>
                                <div class="w3-col s2 w3-left" style="width: 40%;">
                                    <asp:Label ID="Label4" runat="server" Text="Fabricating Patient-Directed Orders" CssClass="srtsLabel_medium" /><br />
                                    <asp:Checkbox ID="cbParticipatePDO" runat="server" ClientIDMode="Static" Text="Participate in PDO" />
                                </div>
                                <div class="w3-col s2 w3-left" style="padding-top: 6px; width: 14%;">
                                    <asp:Button ID="btnSaveRoutingOrders" runat="server" OnClick="btnSaveRoutingOrdersPref_Click" Text="Submit" CssClass="srtsButton" CausesValidation="false" /> <%--OnClientClick="return IsGoodToSave();" --%>
                                </div>
                            </div>
                        </div>
                        <div class="w3-row" style="margin-top: 20px">
                            <h1>Lab Capacity History:</h1>
                            <div class="" style="width: 100%; margin-left: auto; margin-right: auto; height: 200px; max-height: 200px; overflow-y: auto; overflow-x: hidden;">
                                <asp:GridView ID="gvLabCapacityHistory" runat="server" AutoGenerateColumns="False"
                                    OnRowDataBound="gvLabCapacityHistory_RowDataBound"
                                    EmptyDataText="No Parameters Exist"
                                    GridLines="Both"
                                    CssClass="mGrid width75"
                                    AlternatingRowStyle-CssClass="alt"
                                    HeaderStyle-HorizontalAlign="Left"
                                    RowStyle-HorizontalAlign="Left"
                                    ShowHeaderWhenEmpty="true"
                                    HorizontalAlign="Center">
                                    <Columns>
                                        <asp:TemplateField HeaderText="Date">
                                            <ItemStyle HorizontalAlign="Center" />
                                            <ItemTemplate>
                                                <asp:Label ID="lblDate" runat="server" Text='<%# Convert.ToDateTime( Eval("DateLastModified")).ToShortDateString() %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Lab Capacity">
                                            <ItemStyle HorizontalAlign="Center" />
                                            <ItemTemplate>
                                                <asp:Label ID="lblLabCapacity" runat="server" Text='<%# Eval("Capacity")%>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Participate in PDO">
                                            <ItemStyle HorizontalAlign="Center" />
                                            <ItemTemplate>
                                                <asp:Label ID="lblIsPDO" runat="server" Text='<%# Eval("PDO").Equals(true) ? "YES" : "NO"%>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                    <HeaderStyle BackColor="#004994" ForeColor="White" Height="20px" />
                                    <RowStyle BorderColor="#CCCCCC" BorderStyle="Solid" BorderWidth="1px" Height="18px" />
                                </asp:GridView>
                            </div>
                        </div>
                       </div>
                   </div>
    <%-- 
                    <div style="margin-top: 0px; color: #782E1E">
                        <h1>Daily Capacity:</h1>
                        <div class="padding" style="padding-top: 0px">
                            <asp:TextBox type="number" ID="tbCapacity" runat="server" ClientIDMode="Static"  /><%--Text="Current Capacity"--%>
<%--                        </div>
                    </div>
                    <div style="margin-top: 0px; color: #782E1E">
                        <h1>Fabricating Patient-Directed Orders:</h1>
                        <div class="padding" style="padding-top: 0px">
                            <asp:Checkbox ID="cbParticipatePDO" runat="server" ClientIDMode="Static" Text="Participate in PDO" />
                        </div>
                    </div>--%>
                </div>             
            </div>
        <div class="BeigeBoxFooter"></div>
        </div>
          
        <div style="position: relative; top: 125px; left: 385px">
        </div>

    <asp:ScriptManagerProxy ID="smpRoutingOrdersPref" runat="server">
            <Scripts>
                <asp:ScriptReference Path="~/Scripts/Preferences/RoutingOrdersPreferences.js" />
            </Scripts>
        </asp:ScriptManagerProxy>
    </ContentTemplate>
    <Triggers>
        <asp:AsyncPostBackTrigger ControlID="btnSaveRoutingOrders" EventName="Click" />
    </Triggers>
       
</asp:UpdatePanel>
