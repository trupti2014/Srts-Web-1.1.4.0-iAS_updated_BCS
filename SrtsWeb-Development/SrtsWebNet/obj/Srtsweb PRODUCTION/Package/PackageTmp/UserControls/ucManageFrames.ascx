<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ucManageFrames.ascx.cs" Inherits="SrtsWeb.UserControls.ucManageFrames" %>

<asp:UpdatePanel ID="upFrameImages" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
    <ContentTemplate>
        <asp:HiddenField ID="hfSuccessFrames" runat="server" Value="0" ClientIDMode="Static" />
        <asp:HiddenField ID="hfMsgFrames" runat="server" Value="" ClientIDMode="Static" />
        <asp:HiddenField ID="hdfFrameFamily" runat="server" Value="" ClientIDMode="Static" />
        <div class="w3-row" style="margin-top: -30px">
            <div class="BeigeBoxContainer" style="margin: 10px 0px 0px 0px">
                <div class="BeigeBoxHeader" style="text-align: left; position: relative; top: 0px; padding: 12px 10px 3px 15px">
                    <span class="label">Frame Images</span>
                </div>

                <div class="BeigeBoxContent padding" style="margin-top: 10px; margin-left: 10px; height: auto">
                    <%-- FrameFamliy--%>
                    <div id="divFramesFamily" class="w3-row padding" style="padding-left: 0px">
                        <asp:Label ID="lblFrameFamily" runat="server" CssClass="srtsLabel_medium" Text="Frame Family:"></asp:Label><br />
                        <asp:DropDownList ID="ddlFrameFamily" runat="server" CssClass="srtsDropDown_medium" ClientIDMode="Static" OnSelectedIndexChanged="ddlFrameFamily_SelectedIndexChanged"
                            AutoPostBack="true" Width="200px">
                        </asp:DropDownList>
                    </div>
                    <%-- Frames--%>
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
                    <br />
                    <hr />
                    <br />

                    <table>
                        <tr>

                            <td style="float: left; text-align: left">
                                <table style="width: 400px">
                                    <tr>
                                        <td style="width: auto; height: auto; vertical-align: bottom">
                    
                                         <div class="w3-col" style="width: 320px">
                                                <asp:FileUpload runat="server" ID="fileUpload" Width="300px" onchange="this.form.submit()" />
                                            </div>
                                            <div style="color: #900d0d">
                                                <asp:Label ID="lblFileUpload" runat="server" Width="280px" Visible="true" Style="color: #004994" /><br />
                                                <asp:Label runat="server" ID="lblInfo" />
                                            </div>
                       
                       
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="2">
                                            <div class="w3-col" style="width: 300px; margin-top: 10px; margin-bottom: 2px">
                                                <asp:Label ID="lblImageAngle" runat="server" CssClass="srtsLabel_medium" Text="Image Angle:"></asp:Label><br />
                                            </div>
                                            <div class="w3-col" style="width: 300px; margin-bottom: 15px !important">
                                                <asp:DropDownList runat="server" ID="ddlImageAngle" ClientIDMode="Static" CssClass="srtsDropDown_medium" Width="200px"></asp:DropDownList>
                                            </div>
                                        </td>
                                    </tr>

                                    <tr>
                                        <td colspan="2">
                                            <div class="w3-col" style="width: 300px; margin-top: 10px; margin-bottom: 2px">
                                                <asp:Label ID="lblImageName" runat="server" CssClass="srtsLabel_medium" Text="Image Name:"></asp:Label><br />
                                            </div>
                                            <div class="w3-col" style="width: 300px; margin-bottom: 15px !important">
                                                <asp:TextBox ID="txtImageName" runat="server" Width="300px" CssClass="srtsTextBox_medium" ReadOnly="False"></asp:TextBox>
                                            </div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="2">
                                            <div class="w3-col" style="width: 300px; margin-top: 10px; margin-bottom: 2px">
                                                <asp:Label ID="lblMFGName" runat="server" CssClass="srtsLabel_medium" Text="Manufacturer Name:"></asp:Label><br />
                                            </div>
                                            <div class="w3-col" style="width: 300px">
                                                <asp:TextBox ID="txtMFGName" runat="server" Width="300px" CssClass="srtsTextBox_medium" ReadOnly="true"></asp:TextBox>
                                            </div>
                                        </td>
                                    </tr>

                                </table>
                            </td>

                            <td>
                                <table style="width: auto">
                                    <tr>
                                        <td style="max-height: 150px; max-width: 350px">
                                            <div style="margin: 0px 0px 10px 0px">
                                                <asp:Image runat="server" ID="imgFrameImage" Height="150px" Width="350px" ImageUrl="~/Styles/images/DefaultGlasses.png" />
                                            </div>
                                        </td>
                                    </tr>
                                </table>
                            </td>

                        </tr>
                    </table>
                </div>

                <div class="BeigeBoxFooter"></div>
            </div>
        </div>
        <div style="text-align: right">
            <asp:Button ID="bUpdateFrameImages" runat="server" Text="Submit" CssClass="srtsButton" OnClick="btnApplyImage_Click" CausesValidation="false" UseSubmitBehavior="false" />
        </div>
        <asp:ScriptManagerProxy ID="smpFrameImages" runat="server">
            <Scripts>
                <asp:ScriptReference Path="~/Scripts/ManagementEnterprise/FrameImages.js" />
            </Scripts>
        </asp:ScriptManagerProxy>
    </ContentTemplate>
       <Triggers>
        <asp:AsyncPostBackTrigger ControlID="bUpdateFrameImages" EventName="Click" />
        <asp:AsyncPostBackTrigger ControlID="ddlFrameFamily" EventName="SelectedIndexChanged" />
        <asp:AsyncPostBackTrigger ControlID="ddlFrames" EventName="SelectedIndexChanged" />
    </Triggers>
</asp:UpdatePanel>
<script>
    function openPanel(evt, panelName) {
        var i, x, tablinks;
        x = document.getElementsByClassName("panel");
        for (i = 0; i < x.length; i++) {
            x[i].style.display = "none";
        }
        panellinks = document.getElementsByClassName("panelLink");
        for (i = 0; i < x.length; i++) {
            panellinks[i].className = panellinks[i].className.replace(" w3-blue", "");
        }
        document.getElementById(panelName).style.display = "block";
        evt.currentTarget.className += " w3-blue";
    }
</script>
