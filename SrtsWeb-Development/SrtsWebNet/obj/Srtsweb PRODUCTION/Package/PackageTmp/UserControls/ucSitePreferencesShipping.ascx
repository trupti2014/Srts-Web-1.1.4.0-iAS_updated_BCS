<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ucSitePreferencesShipping.ascx.cs" Inherits="SrtsWeb.UserControls.ucSitePreferencesShipping" %>

<asp:UpdatePanel ID="upShippingPreferences" runat="server" UpdateMode="conditional" ChildrenAsTriggers="true">
    <ContentTemplate>
        <asp:HiddenField ID="hfSuccessShipping" runat="server" Value="0" ClientIDMode="Static" />
        <asp:HiddenField ID="hfMsgShipping" runat="server" Value="" ClientIDMode="Static" />
        <div class="w3-row" style="margin-top: -30px">
            <div class="BeigeBoxContainer" style="margin: 10px 0px 0px 0px">
                <div class="BeigeBoxHeader" style="text-align: left; position: relative; top: 0px; padding: 12px 10px 3px 15px">
                    <span class="label">Shipping Site Preferences</span>
                </div>
                <div class="BeigeBoxContent padding" style="margin-top: 0px; margin-left: 10px; height: auto">
                         <div id="divDefaultShippingProvider" class="w3-row padding" style="padding-left: 0px">
                            <asp:Label ID="lblDefaultShippingProvider" runat="server" CssClass="srtsLabel_medium" Text="Default Shipping Provider:"></asp:Label><br />
                            <asp:DropDownList ID="ddlShippingProvider" runat="server" CssClass="srtsDropDown_medium" ClientIDMode="Static" Width="300px"></asp:DropDownList>
                        </div>
                </div>
                <div class="BeigeBoxFooter"></div>
            </div>
        </div>
        <div style="position: relative; top: 125px; left: 385px">
            <asp:Button ID="bUpdateShippingPref" runat="server" OnClick="bUpdateShippingPref_Click" Text="Submit" CssClass="srtsButton" CausesValidation="false" />
        </div>

       <asp:ScriptManagerProxy ID="smpShippingPref" runat="server">
            <Scripts>
                <asp:ScriptReference Path="~/Scripts/Preferences/ShippingSitePreferences.js" />
            </Scripts>
        </asp:ScriptManagerProxy>
    </ContentTemplate>
    <Triggers>
        <asp:AsyncPostBackTrigger ControlID="bUpdateShippingPref" EventName="Click" />
    </Triggers>
</asp:UpdatePanel>
