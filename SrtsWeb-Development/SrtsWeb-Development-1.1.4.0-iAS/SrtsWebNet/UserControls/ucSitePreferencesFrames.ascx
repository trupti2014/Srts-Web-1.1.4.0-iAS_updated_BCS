<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ucSitePreferencesFrames.ascx.cs" Inherits="SrtsWeb.UserControls.ucSitePreferencesFrames" %>
<!-- Site Preferences - Frames Defaults -->


<asp:UpdatePanel ID="upframeitempreferences" runat="server" UpdateMode="conditional" ChildrenAsTriggers="true">
    <ContentTemplate>
        <asp:HiddenField ID="hfSuccessFrames" runat="server" Value="0" ClientIDMode="Static" />
        <asp:HiddenField ID="hfMsgFrames" runat="server" Value="" ClientIDMode="Static" />
        <div class="w3-row alignLeft" style="margin-top: -30px">
            <div class="BeigeBoxContainer" style="margin: 10px 0px 0px 0px">
                <div class="BeigeBoxHeader" style="text-align: left; position: relative; top: 0px; padding: 12px 10px 3px 15px">
                    <span class="label">Frames Preferences</span>
                </div>
                <div class="BeigeBoxContent padding" style="margin-top: -20px; margin-left: 10px; height: auto">
                    <div id="divFramePreference">
                        <%-- frames--%>
                        <div id="divFrames" class="w3-row padding" style="padding-left: 0px">
                            <asp:Label ID="lblFrames" runat="server" CssClass="srtsLabel_medium" Text="Frame:"></asp:Label><br />
                            <asp:DropDownList ID="ddlFrames" runat="server" CssClass="srtsDropDown_medium" ClientIDMode="Static" OnSelectedIndexChanged="ddlFrames_SelectedIndexChanged"
                                AutoPostBack="true" Width="784px">
                            </asp:DropDownList>
                        </div>

                        <%-- color, eye, bridge, temple--%>
                        <div id="divSection1" class="w3-row padding" style="padding-left: 0px">
                            <div class="w3-col" style="width: 45%">
                                <%-- color--%>
                                <div class="w3-half">
                                    <asp:Label ID="lblColor" runat="server" CssClass="srtsLabel_medium" Text="Color:"></asp:Label><br />
                                    <asp:DropDownList ID="ddlColor" runat="server" ClientIDMode="Static" CssClass="srtsDropDown_medium" Width="170px"></asp:DropDownList>
                                </div>
                                <%-- eye--%>
                                <div class="w3-half">
                                    <asp:Label ID="lblEye" runat="server" CssClass="srtsLabel_medium" Text="Eye:"></asp:Label><br />
                                    <asp:DropDownList ID="ddlEye" runat="server" ClientIDMode="Static" CssClass="srtsDropDown_medium" Width="170px"></asp:DropDownList>
                                </div>
                            </div>

                            <div class="w3-rest">
                                <%--  bridge--%>
                                <div class="w3-col" style="width: 39%">
                                    <asp:Label ID="lblBridge" runat="server" CssClass="srtsLabel_medium" Text="Bridge:"></asp:Label><br />
                                    <asp:DropDownList ID="ddlBridge" runat="server" CssClass="srtsDropDown_medium" ClientIDMode="Static" Width="160px"></asp:DropDownList>
                                </div>
                                <%--   temple--%>
                                <div class="w3-rest">
                                    <asp:Label ID="lblTemple" runat="server" CssClass="srtsLabel_medium" Text="Temple:"></asp:Label><br />
                                    <asp:DropDownList ID="ddlTemple" runat="server" ClientIDMode="Static" CssClass="srtsDropDown_medium" Width="255px"></asp:DropDownList>
                                </div>
                            </div>
                        </div>

                        <%-- lens, tint, coating, material--%>
                        <div id="divSection2" class="w3-row padding" style="padding-left: 0px; display: block; overflow:auto" >
                            <div class="w3-col" style="width: 45%">
                            <%--lens--%>
                            <div class="w3-half">
                                <asp:Label ID="lblLens" runat="server" CssClass="srtsLabel_medium" Text="Lens:"></asp:Label><br />
                                <asp:DropDownList ID="ddlLens" runat="server" ClientIDMode="Static" CssClass="srtsDropDown_medium" onchange="SetSetHts();" Width="170px"></asp:DropDownList>
                            </div>
                            <%-- tint--%>
                            <div class="w3-half">
                                <asp:Label ID="lblTint" runat="server" CssClass="srtsLabel_medium" Text="Tint:"></asp:Label><br />
                                <asp:DropDownList ID="ddlTint" runat="server" CssClass="srtsDropDown_medium" ClientIDMode="Static" Width="170px"></asp:DropDownList>
                            </div>
                            </div>
                             <div class="w3-rest">
                            <%-- coating--%>
                            <div class="w3-col" style="width: 39%">
                                <asp:Label ID="lblCoating" runat="server" CssClass="srtsLabel_medium" Text="Coating:"></asp:Label><br />
                                <asp:CheckBoxList  ID="ddlCoating" runat="server" ClientIDMode="Static" Width="160px">
                                </asp:CheckBoxList>
                            </div>
                            <%-- material--%>
                            <div class="w3-rest">
                                <asp:Label ID="lblMaterial" runat="server" CssClass="srtsLabel_medium" Text="Material:"></asp:Label><br />
                                <asp:DropDownList ID="ddlMaterial" runat="server" CssClass="srtsDropDown_medium" ClientIDMode="Static" Width="255px"></asp:DropDownList>
                            </div>
                        </div>
                        </div>

                        <%-- set seg height--%>
                        <div id="divSection3" class="w3-row padding" style="padding-left: 0px; width: 400px">
                            <%--set seg height--%>
                            <div class="w3-col" style="width: 30%">
                                <asp:Label ID="lblSetSegHeight" runat="server" CssClass="srtsLabel_medium" Text="Set Seg Height:"></asp:Label>
                            </div>

                            <div class="w3-rest">
                                <%-- seg right--%>
                                <div class="w3-half">
                                    <asp:Label ID="lblSegRight" runat="server" CssClass="srtsLabel_medium" Text="Right (OD):"></asp:Label>
                                    <asp:TextBox ID="tbOdSegHt" runat="server" Width="45px" CssClass="srtsTextBox_small" ClientIDMode="Static" onchange="SegHtVal('tbOdSegHt');"></asp:TextBox>
                                </div>
                                <div class="w3-half">
                                    <%--   seg left--%>
                                    <asp:Label ID="lblSegLeft" runat="server" CssClass="srtsLabel_medium" Text="Left (OS):"></asp:Label>
                                    <asp:TextBox ID="tbOsSegHt" runat="server" Width="45px" CssClass="srtsTextBox_small" ClientIDMode="Static" onchange="SegHtVal('tbOsSegHt');"></asp:TextBox>
                                </div>

                            </div>
                        </div>
                    </div>
                </div>
                <div class="BeigeBoxFooter"></div>
            </div>
            <%--    messages--%>
            <div class="customValidators padding" style="height: 15px; max-height: 15px; width: 80%">
                <div id="divFramePreferenceError"></div>
            </div>

            <div style="position: relative; top: -5px; left: 776px">
                <asp:Button ID="bSubmit" runat="server" Text="Submit" CssClass="srtsButton" OnClick="bSubmit_Click" CausesValidation="false" />
            </div>
        </div>

        <asp:ScriptManagerProxy ID="smpFramePref" runat="server">
            <Scripts>
                <asp:ScriptReference Path="~/Scripts/Preferences/FrameItemPreferences.js" />
            </Scripts>
        </asp:ScriptManagerProxy>

    </ContentTemplate>
    <Triggers>
        <asp:AsyncPostBackTrigger ControlID="ddlFrames" EventName="SelectedIndexChanged" />
        <asp:AsyncPostBackTrigger ControlID="bSubmit" EventName="Click" />
    </Triggers>
</asp:UpdatePanel>

