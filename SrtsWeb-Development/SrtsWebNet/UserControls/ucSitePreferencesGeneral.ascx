<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ucSitePreferencesGeneral.ascx.cs" Inherits="SrtsWeb.UserControls.ucSitePreferencesGeneral" %>

<asp:UpdatePanel ID="upGeneralPreferences" runat="server" UpdateMode="conditional" ChildrenAsTriggers="true">
    <ContentTemplate>
        <asp:HiddenField ID="hfSuccessGeneral" runat="server" Value="0" ClientIDMode="Static" />
        <asp:HiddenField ID="hfMsgGeneral" runat="server" Value="" ClientIDMode="Static" />
        <div class="w3-row" style="margin-top: -30px">
            <div class="BeigeBoxContainer" style="margin: 10px 0px 0px 0px">
                <div class="BeigeBoxHeader" style="text-align: left; position: relative; top: 0px; padding: 12px 10px 3px 15px">
                    <span class="label">General Site Preferences</span>
                </div>
                <div class="BeigeBoxContent padding" style="margin-top: 0px; margin-left: 10px; height: auto">
                    <div style="margin-top: 0px; color: #782E1E">
                        <h1>Label Printing:</h1>
                        <div class="padding" style="padding-top:0px">
                        <asp:CheckBox ID="cbAlphaSort" runat="server" ClientIDMode="Static" Text="Sort Labels Alphabetically" />
                        </div>
                    </div>
                </div>
                <div class="BeigeBoxFooter"></div>
            </div>
        </div>
        <div style="position: relative; top: 125px; left: 385px">
            <asp:Button ID="bUpdateGeneralPref" runat="server" OnClick="bUpdateGeneralPref_Click" Text="Submit" CssClass="srtsButton" CausesValidation="false" />
        </div>

                <asp:ScriptManagerProxy ID="smpGeneralPref" runat="server">
            <Scripts>
                <asp:ScriptReference Path="~/Scripts/Preferences/GeneralSitePreferences.js" />
            </Scripts>
        </asp:ScriptManagerProxy>
    </ContentTemplate>
    <Triggers>
        <asp:AsyncPostBackTrigger ControlID="bUpdateGeneralPref" EventName="Click" />
    </Triggers>
</asp:UpdatePanel>
