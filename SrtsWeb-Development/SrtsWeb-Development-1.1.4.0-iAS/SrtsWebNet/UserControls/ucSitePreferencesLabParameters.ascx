<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ucSitePreferencesLabParameters.ascx.cs" Inherits="SrtsWeb.UserControls.ucSitePreferencesLabParameters" %>

<!-- Site Preferences - Lab Parameters -->


<asp:UpdatePanel ID="uplabParameters" runat="server" UpdateMode="conditional" ChildrenAsTriggers="true">
    <ContentTemplate>
        <asp:HiddenField ID="hfSuccessLabPref" runat="server" Value="0" ClientIDMode="Static" />
        <asp:HiddenField ID="hfMsgLabPref" runat="server" Value="" ClientIDMode="Static" />
        <div class="w3-row alignLeft" style="margin-top: -30px; height: auto;" >
            <div class="BeigeBoxContainer" style="margin: 10px 0px 0px 0px; height: auto;">
                <div class="BeigeBoxHeader" style="text-align: left; position: relative; top: 0px; padding: 12px 10px 3px 15px">
                    <span class="label">Lab Parameters</span>
                </div>
            <%--    messages--%>
            <div class="customValidators" style="min-height: 15px; max-height: 100px; width: 100%; padding-top: 0px; padding-bottom: 0px; margin-top: 0px; padding-left: 0px;"> 
                <div id="errorMessage"></div>
            </div>
                <div class="BeigeBoxContent padding" style=" padding-top: 0px; height: auto"><%--margin-top: -20px; margin-left: 10px;--%>
                    <div id="divMaterialCylinderParams">
                    <asp:HiddenField ID="hfMatParamID" runat="server" />
                    <div class="w3-row alignLeft">
                        <div class="sitepreferencegroupbox"> <%---Border around Material/Cylinder---%>
                        <div class="padding" style="padding: 5px 0px;">
                            <h1>Add New Material/Cylinder:</h1>
                            <div class="w3-row">
                                <div class="w3-col s2 w3-left" style="width: 18%;" >
                                    <asp:Label ID="Label1" runat="server" Text="Material" CssClass="srtsLabel_medium" /><br />
                                    <asp:DropDownList ID="ddlMatType" runat="server" CssClass="srtsDropDown_medium" Width="120px"></asp:DropDownList>
                                </div>
                                <div class="w3-col s2 w3-left" style="width: 12%;">
                                    <asp:Label ID="Label2" runat="server" Text="In Stock" CssClass="srtsLabel_medium" /><br />
                                    <asp:DropDownList ID="ddlIsStocked" runat="server" CssClass="srtsDropDown_medium" Width="70px"
                                        OnSelectedIndexChanged="ddlIsStocked_SelectedIndexChanged" AutoPostBack="True">
                                        <asp:ListItem>-Select-</asp:ListItem>
                                        <asp:ListItem Value="TRUE" Text="YES"></asp:ListItem>
                                        <asp:ListItem Value="FALSE" Text="NO"></asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                                <div class="w3-col s2 w3-left" style="width: 14%;">
                                    <asp:Label ID="Label3" runat="server" Text="Cylinder" CssClass="srtsLabel_medium" /><br />
                                    <asp:TextBox ID="tbCyl" runat="server" CssClass="srtsTextBox_medium" Width="90px" onchange="ParamInput('tbCyl')" ClientIDMode="Static" TabIndex="2" />
                                </div>
                                <div class="w3-col s2 w3-left" style="width: 14%;">
                                    <asp:Label ID="Label4" runat="server" Text="Max Plus" CssClass="srtsLabel_medium" /><br />
                                    <asp:TextBox ID="tbMaxPlus" runat="server" CssClass="srtsTextBox_medium" Width="90px" onchange="ParamInput('tbMaxPlus')" ClientIDMode="Static" TabIndex="3" />
                                </div>
                                <div class="w3-col s2 w3-left" style="width: 14%;">
                                    <asp:Label ID="Label5" runat="server" Text="Max Minus" CssClass="srtsLabel_medium" />
                                    <br />
                                    <asp:TextBox ID="tbMaxMinus" runat="server" CssClass="srtsTextBox_medium" Width="90px" onchange="ParamInput('tbMaxMinus')"
                                        ClientIDMode="Static" TabIndex="4" />
                                </div>
                                <div class="w3-col s2 w3-left" style="width: 14%;">
                                    <asp:Label ID="Label7" runat="server" Text="Type" CssClass="srtsLabel_medium" /><br />
                                    <asp:DropDownList ID="ddlCapabilityType" runat="server" CssClass="srtsDropDown_medium" Width="90px"
                                         AutoPostBack="True" TabIndex="5">
                                        <asp:ListItem>-Select-</asp:ListItem>
                                        <asp:ListItem Text="SV"></asp:ListItem>
                                        <asp:ListItem Text="MV"></asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                                <div class="w3-col s2 w3-left" style="padding-top: 6px; width: 14%;">
                                    <asp:Button ID="btnSaveParams" runat="server" OnClientClick="return IsGoodToSave();" OnClick="btnSaveParams_Click"
                                        CssClass="srtsButton" CausesValidation="false" Text="Save" Enabled="true" />
                                </div>
                            </div>
                        </div>
                        <div class="w3-row" style="margin-top: 0px">
                            <div class="" style="width: 100%; margin-left: auto; margin-right: auto; height: 200px; max-height: 200px; overflow-y: scroll; overflow-x: hidden;">
                                <asp:GridView ID="gvLabParameters" runat="server" AutoGenerateColumns="False"
                                    OnRowCommand="gvLabParameters_RowCommand"
                                    OnRowDataBound="gvLabParameters_RowDataBound"
                                    OnRowEditing="gvLabParameters_RowEditing"
                                    OnRowDeleting="gvLabParameters_RowDeleting"
                                    EmptyDataText="No Parameters Exist"
                                    GridLines="Both"
                                    CssClass="mGrid width75"
                                    AlternatingRowStyle-CssClass="alt"
                                    HeaderStyle-HorizontalAlign="Left"
                                    RowStyle-HorizontalAlign="Left"
                                    ShowHeaderWhenEmpty="true"
                                    HorizontalAlign="Center">
                                    <Columns>
                                        <asp:ButtonField Text="Edit" ButtonType="Link" CommandName="Edit" />
                                        <asp:ButtonField Text="Delete" ButtonType="Link" CommandName="delete" />
                                        <asp:TemplateField HeaderText="Material">
                                            <ItemStyle HorizontalAlign="Center" />
                                            <ItemTemplate>
                                                <asp:Label ID="lblMat" runat="server" Text='<%# Eval("Material") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="In Stock">
                                            <ItemStyle HorizontalAlign="Center" />
                                            <ItemTemplate>
                                                <asp:Label ID="lblIsStocked" runat="server" Text='<%# Eval("IsStocked").Equals(true) ? "YES" : "NO"%>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Cylinder">
                                            <ItemStyle HorizontalAlign="Center" />
                                            <ItemTemplate>
                                                <asp:Label ID="lblCyl" runat="server" Text='<%# Eval("Cylinder").ToString().Equals("0.00") || Eval("Cylinder").ToString().StartsWith("-") ? 
                                        Eval("Cylinder") : String.Format("+{0}", Eval("Cylinder")) %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Max plus">
                                            <ItemStyle HorizontalAlign="Center" />
                                            <ItemTemplate>
                                                <asp:Label ID="lblMaxPlus" runat="server" Text='<%# Eval("MaxPLus").ToString().Equals("0.00") || Eval("MaxPLus").ToString().StartsWith("-") ? 
                                        Eval("MaxPLus") : String.Format("+{0}", Eval("MaxPLus")) %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Max Minus">
                                            <ItemStyle HorizontalAlign="Center" />
                                            <ItemTemplate>
                                                <div>
                                                    <asp:Label ID="lblMaxMinus" runat="server" Text='<%# Eval("MaxMinus").ToString().Equals("0.00") || Eval("MaxMinus").ToString().StartsWith("-") ? 
                                        Eval("MaxMinus") : String.Format("+{0}", Eval("MaxMinus")) %>'></asp:Label>
                                                </div>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Type">
                                            <ItemStyle HorizontalAlign="Center" />
                                            <ItemTemplate>
                                                <div>
                                                    <asp:Label ID="lblCapabilityType" runat="server" Text='<%# Eval("CapabilityType") %>'>></asp:Label>
                                                </div>
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
                       <%-- BEGIN FS390--%>
                        <div class="sitepreferencegroupbox">
                            <div id="divLabParams">
                            <div class="w3-row" style="padding: 20px 20px;" >
                                <div class="w3-col s3 w3-left">
                                    <asp:Label ID="lblPrism" runat="server" style="color:#004994">Prism:</asp:Label><br />
                                    <asp:Label ID="Label6" runat="server" Text="Max Prism" CssClass="srtsLabel_medium" /><br />
                                    <asp:TextBox ID="tbMaxPrism" runat="server" style="z-index: auto !important; position:absolute;" CssClass="srtsTextBox_medium" Width="150px" onchange="ValLabParam('tbMaxPrism')" ClientIDMode="Static" TabIndex="6" />
                                </div>
                                <div class="w3-col s3 w3-left" style="margin-left: 0px">
                                    <asp:Label ID="lblDecentration" runat="server" style="color:#004994">Decentration:</asp:Label><br />
                                    <asp:Label ID="Label9" runat="server" Text="Max Decentration - Plus" CssClass="srtsLabel_medium" /><br />
                                    <asp:TextBox ID="tbMaxDecentrationPlus" runat="server" style="z-index: auto !important; position:absolute;" CssClass="srtsTextBox_medium" Width="150px" onchange="ValLabParam('tbMaxDecentrationPlus')" ClientIDMode="Static" TabIndex="7" />
                                </div>
                                <div class="w3-col s3 w3-left">
                                    <br />
                                    <asp:Label ID="Label10" runat="server" Text="Max Decentration - Minus" CssClass="srtsLabel_medium" />
                                    <br />
                                    <asp:TextBox ID="tbMaxDecentrationMinus" runat="server" style="z-index: auto !important; position:absolute;" CssClass="srtsTextBox_medium" Width="150px" onchange="ValLabParam('tbMaxDecentrationMinus')"
                                        ClientIDMode="Static" TabIndex="8" />
                                </div>
                                <div class="w3-col s2 w3-left" style="padding-top: 21px" > <%--OnClientClick="return IsGoodToSave();" --%>
                                    <asp:Button ID="btnSaveLabParams" runat="server" 
                                        CssClass="srtsButton" CausesValidation="false" Text="Save" Enabled="true" OnClick="btnSaveLabParams_Click" />
                                </div>
                            </div>
                           </div>
                        </div>
                        <%--END FS390--%>
                    </div>
                   
                   </div>
                <div class="BeigeBoxFooter"></div>
            </div>

            <%--    messages--%>
<%--            <div class="customValidators padding" style="height: 50px; max-height: 50px; width: 80%">
                <div id="errorMessage"></div>
            </div>--%>

            <div style="position: relative; top: -5px; left: 776px">
            </div>
        </div>

        <asp:ScriptManagerProxy ID="smpLabParams" runat="server">
            <Scripts>
                <asp:ScriptReference Path="~/Scripts/Preferences/LabParameters.js" />
            </Scripts>
        </asp:ScriptManagerProxy>

    </ContentTemplate>
    <Triggers>
        <asp:AsyncPostBackTrigger ControlID="btnSaveParams" EventName="Click" />
        <asp:AsyncPostBackTrigger ControlID="ddlIsStocked" EventName="SelectedIndexChanged" />
    </Triggers>
</asp:UpdatePanel>



