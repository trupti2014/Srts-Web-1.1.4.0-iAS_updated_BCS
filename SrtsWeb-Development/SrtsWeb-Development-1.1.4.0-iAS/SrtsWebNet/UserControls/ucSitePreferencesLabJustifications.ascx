<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ucSitePreferencesLabJustifications.ascx.cs"  Inherits="SrtsWeb.UserControls.ucSitePreferencesLabJustifications"  %>

<!-- Site Preferences - Lab Justifications -->

<asp:UpdatePanel ID="uplabParameters" runat="server" UpdateMode="conditional" ChildrenAsTriggers="true">
    <ContentTemplate>
        <asp:HiddenField ID="hfSuccessLabJust" runat="server" Value="0" ClientIDMode="Static" />
        <asp:HiddenField ID="hfMsgLabJust" runat="server" Value="" ClientIDMode="Static" />
        <div class="w3-row alignLeft" style="margin-top: -30px; height: auto">
            <div class="BeigeBoxContainer" style="margin: 10px 0px 0px 0px; height: auto">
                <div class="BeigeBoxHeader" style="text-align: left; position: relative; top: 0px; padding: 12px 10px 3px 15px">
                    <span class="label">Lab Justifications</span>
                </div>

                <div class="BeigeBoxContent padding" style="margin-top: -20px; margin-left: 10px; height: auto">
                    <div class="w3-row alignLeft">
                        <div class="padding">
                            <h1>Add Default Rejection Comment/Template:</h1>
                            <div class="w3-row">
                                <div class="s2 w3-left">
                                    <asp:TextBox ID="tbReject" runat="server" ClientIDMode="Static" CssClass="srtsTextBox" Width="450px" Height="75px" MaxLength="500" TextMode="MultiLine"></asp:TextBox>
                                </div>
                                <div>
                                    <asp:Button ID="bDeleteReject" runat="server" CssClass="srtsButton" Text="Delete" ClientIDMode="Static" OnClick="bDeleteReject_Click" CausesValidation="false" />
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="w3-row alignLeft">
                        <div class="padding">
                            <h1>Add Default Redirect Comment/Template:</h1>
                            <div class="w3-row">
                                <div class="s2 w3-left">
                                    <asp:TextBox ID="tbRedirect" runat="server" ClientIDMode="Static" CssClass="srtsTextBox" Width="450px" Height="75px" MaxLength="500" TextMode="MultiLine"></asp:TextBox>
                                </div>
                                <div>
                                    <asp:Button ID="bDeleteRedirect" runat="server" CssClass="srtsButton" Text="Delete" ClientIDMode="Static" OnClick="bDeleteRedirect_Click" CausesValidation="false"/>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div style="position: relative; top: -5px; left: 776px">
                    <asp:Button ID="bSubmit" runat="server" Text="Submit" CssClass="srtsButton" OnClick="bSubmit_Click" CausesValidation="false" />
                </div>

                <div class="BeigeBoxFooter"></div>
            </div>

            <%--messages--%>
            <div class="customValidators padding" style="height: 50px; max-height: 50px; width: 80%">
                <div id="errorMessage"></div>
            </div>

        </div>
        <asp:ScriptManagerProxy ID="smpJustifications" runat="server">
            <Scripts>
                <asp:ScriptReference Path="~/Scripts/Preferences/JustificationPreferences.js" />
            </Scripts>
        </asp:ScriptManagerProxy>
    </ContentTemplate>
    <Triggers>
        <asp:AsyncPostBackTrigger ControlID="bSubmit" EventName="Click" />
        <asp:AsyncPostBackTrigger ControlID="bDeleteReject" EventName="Click" />
        <asp:AsyncPostBackTrigger ControlID="bDeleteRedirect" EventName="Click" />
    </Triggers>
</asp:UpdatePanel>
