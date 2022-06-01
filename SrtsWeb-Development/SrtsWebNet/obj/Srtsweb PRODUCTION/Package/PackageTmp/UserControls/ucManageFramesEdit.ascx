<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ucManageFramesEdit.ascx.cs" Inherits="SrtsWeb.UserControls.ucManageFramesEdit" %>


<style>
    .FrameImageDialog {
        position: absolute;
        top: 10px;
        left: 95px;
        height: auto;
        min-height: 120px;
        min-width: 650px;
        padding: 0px;
        background: transparent;
        border-radius: 4px;
    }

    .shadow {
        -webkit-box-shadow: 10px 10px 5px 0px rgba(0,0,0,0.12);
        -moz-box-shadow: 10px 10px 5px 0px rgba(0,0,0,0.12);
        box-shadow: 10px 10px 5px 0px rgba(0,0,0,0.12);
    }


    .FrameImageDialog .header_info {
        font-size: 15px;
        color: #004994;
        padding: 5px 10px;
        background-color: transparent;
    }

    .FrameImageDialog .content {
        background-color: #fff;
        padding: 10px 10px;
        text-align: left;
    }

    .FrameImageDialog .title {
        width: 95%;
        padding: 10px 10px;
        text-align: center;
        font-size: 17px !important;
        color: #006600;
    }

    .FrameImageDialog .message {
        margin: 5px;
        padding: 5px 10px;
        text-align: center;
        font-size: 13px !important;
        color: #000;
    }

    .FrameImageDialog .w3-closebtn {
        margin-top: -3px;
    }

    .imageInfo {
        color: darkblue;
    }

    .captionLeft {
        clear: both;
        float: left;
        margin-left: 0px;
        margin-top: 10px;
    }

    .rblCaptionLeft {
        float: left;
        margin-left: 15px;
        margin-top: 5px;
    }
</style>



<%--Image Modal --%>
<%-- ///////////////////////////////////////////////////////////////////--%>
<div id="FrameImageDialog" class="w3-modal" style="z-index: 30000">
    <div class="w3-modal-content">
        <div class="w3-container">
            <div class="FrameImageDialog">
                <div class="BeigeBoxContainer shadow" style="width: 550px">
                    <asp:UpdatePanel ID="uplImageModalView" runat="server" ChildrenAsTriggers="true" UpdateMode="Conditional">
                        <ContentTemplate>
                            <div style="background-color: #fff">
                                <div class="BeigeBoxHeader" style="text-align: left; padding: 12px 10px 3px 15px">
                                    <div id="FrameImageDialogheader" class="header_info">
                                        <span onclick="document.getElementById('FrameImageDialog').style.display='none'"
                                            class="w3-closebtn">&times;</span>
                                        <span class="label">Manage Frame Images</span> - Image View
                                    </div>
                                </div>
                                <div class="BeigeBoxContent" style="margin-left: 0px; padding-top: 0px; height: 430px">
                                    <div id="divFrameImage" runat="server" class="row padding" style="height: 400px"></div>
                                </div>
                                <div class="BeigeBoxFooter" style="border-top: 1px solid #E7CFAD;">
                                    <span class="label">
                                        <asp:Literal ID="litFooterInfo" runat="server" Text=""></asp:Literal></span>
                                </div>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </div>
        </div>
    </div>
</div>
<%--////////////////////////////////////////////////////////////////--%>

<asp:UpdatePanel ID="upFrameImagesEdit" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
    <ContentTemplate>
        <asp:HiddenField ID="hfpanel" runat="server" Value="0" ClientIDMode="Static" />
        <asp:HiddenField ID="hfeSuccessFrames" runat="server" Value="0" ClientIDMode="Static" />
        <asp:HiddenField ID="hfeMsgFrames" runat="server" Value="" ClientIDMode="Static" />
        <asp:HiddenField ID="hfImage" runat="server" Value="" ClientIDMode="Static" />
        <asp:HiddenField ID="hfID" runat="server" Value="" ClientIDMode="Static" />

        <div class="w3-row" style="margin-top: -30px">
            <div class="BeigeBoxContainer" style="margin: 10px 0px 0px 0px">
                <div class="BeigeBoxHeader" style="text-align: left; position: relative; top: 0px; padding: 12px 10px 3px 15px">
                    <span class="label">Frame Images</span>
                </div>

                <div class="BeigeBoxContent padding" style="margin-top: 10px; margin-left: 10px; height: auto">
                    <%-- FrameFamliy--%>
                    <div id="divFramesFamily" class="w3-row padding" style="padding-left: 0px">
                        <asp:Label ID="lblFrameFamily" runat="server" CssClass="srtsLabel_medium" Text="Frame Family:"></asp:Label><br />
                        <asp:DropDownList ID="ddlFrameFamilyEdit" runat="server" CssClass="srtsDropDown_medium" ClientIDMode="Static" OnSelectedIndexChanged="ddlFrameFamily_SelectedIndexChanged"
                            AutoPostBack="true" Width="200px">
                        </asp:DropDownList>
                    </div>
                    <%-- Frames--%>
                    <div id="divFrames" class="w3-row padding" style="padding-left: 0px">
                        <asp:Label ID="lblFrames" runat="server" CssClass="srtsLabel_medium" Text="Frame:"></asp:Label><br />
                        <asp:DropDownList ID="ddlFrames" runat="server" CssClass="srtsDropDown_medium" ClientIDMode="Static" OnSelectedIndexChanged="ddlFrames_SelectedIndexChanged"
                            AutoPostBack="true" Width="650px">
                        </asp:DropDownList>
                        <div style="text-align: right; vertical-align: auto">
                            <asp:Button ID="bGetFrameImages" runat="server" align="right" Text="Submit" CssClass="srtsButton" Visible="false" CausesValidation="false" UseSubmitBehavior="false" Enabled="false" />
                        </div>
                    </div>
                    <br />
                    <br />
                    <asp:Panel ID="pnlEdit" runat="server" Visible="false">
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
                                            <td>
                                                <div class="w3-col" style="width: 300px; margin-top: 10px; margin-bottom: 5px">
                                                    <asp:Label ID="lblImageName" runat="server" CssClass="srtsLabel_medium" Text="Image Name:"></asp:Label><br />
                                                </div>
                                                <div class="w3-col" style="width: 300px; margin-bottom: 15px !important">
                                                    <asp:TextBox ID="txtImageName" runat="server" Width="300px" CssClass="srtsTextBox_medium" ReadOnly="False"></asp:TextBox>
                                                </div>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <div class="w3-col" style="width: 300px; margin-top: 10px; margin-bottom: 5px">
                                                    <asp:Label ID="lblMFGName" runat="server" CssClass="srtsLabel_medium" Text="Manufacturer Name:"></asp:Label><br />
                                                </div>
                                                <div class="w3-col" style="width: 300px; margin-bottom: 20px !important">
                                                    <asp:TextBox ID="txtMFGName" runat="server" Width="300px" CssClass="srtsTextBox_medium" ReadOnly="true"></asp:TextBox>
                                                </div>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <div class="captionLeft">
                                                    <asp:Label ID="lblIsActive" runat="server" CssClass="srtsLabel_medium" Text="Is Active"></asp:Label>
                                                </div>
                                                <div class="rblCaptionLeft">
                                                    <asp:RadioButtonList ID="rblIsActive" RepeatDirection="Horizontal" runat="server">
                                                        <asp:ListItem Text="Yes" Value="True"></asp:ListItem>
                                                        <asp:ListItem Text="No" Value="False"></asp:ListItem>
                                                    </asp:RadioButtonList>
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
                    </asp:Panel>
                    <br />
                    <br />
                    <div style="text-align: center">
                        <asp:Label ID="lblFound" Visible="false" runat="server" Font-Bold="true" Text=""></asp:Label>
                    </div>
                    <asp:GridView ID="gvFrameImage" runat="server" AllowPaging="true" AutoGenerateColumns="false"
                        CssClass="mGrid" AllowSorting="true" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                        HeaderStyle-HorizontalAlign="Left" RowStyle-HorizontalAlign="Center"
                        DataKeyNames="ID" PageSize="20" OnRowCommand="gvFrameImage_RowCommand" EnableViewState="true">
                        <PagerSettings Mode="NextPreviousFirstLast" Position="TopAndBottom" FirstPageText="<< First"
                            NextPageText="Next >" PreviousPageText="< Previous" LastPageText="Last >>" />
                        <Columns>
                            <asp:TemplateField HeaderText="MFG Name">
                                <ItemTemplate>
                                    <asp:LinkButton ID="lnbMFGName" CommandArgument='<%#Eval("ID") %>' Text='<%#Eval("MfgName") %>' runat="server" ToolTip="Edit this Frame Image." CommandName="EditRecord"></asp:LinkButton>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="IMGName" HeaderText="Image Name" />
                            <asp:BoundField DataField="EyeSize" HeaderText="Eye Size" />
                            <asp:BoundField DataField="Temple" HeaderText="Temple" />
                            <asp:BoundField DataField="BridgeSize" HeaderText="Bridge Size" />
                            <asp:BoundField DataField="Color" HeaderText="Color" />
                            <asp:BoundField DataField="ImgAngle" HeaderText="Image Angle" />
                            <asp:TemplateField HeaderText="Img">
                                <ItemTemplate>
                                    <asp:ImageButton ID="btnImageView" runat="server" ClientIDMode="Static" OnClientClick='<%# Eval("ID","return DisplayFrameImageDialog(\"{0}\")") %>'
                                        ImageUrl="~/Styles/images/GlassesIcon.png" ToolTip="View this frame image." Width="40px" Height="40px" CommandName="ViewImage"
                                        CommandArgument='<%#Eval("ID")+","+ Eval("FrameImage")%>' CausesValidation="false" />
                                </ItemTemplate>
                                <ItemStyle Width="25px" />
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>



                    <div class="BeigeBoxFooter"></div>
                </div>
            </div>
            <div style="text-align: right">
                <asp:Button ID="bUpdateFrameImages" runat="server" Text="Submit" CssClass="srtsButton" OnClick="btnApplyImage_Click" CausesValidation="false" UseSubmitBehavior="false" />
            </div>
            <asp:ScriptManagerProxy ID="smpFrameImagesEdit" runat="server">
                <Scripts>
                    <asp:ScriptReference Path="~/Scripts/ManagementEnterprise/FrameImages.js" />
                    <asp:ScriptReference Path="~/Scripts/ManagementEnterprise/FrameImagesEdit.js" />
                </Scripts>
            </asp:ScriptManagerProxy>
    </ContentTemplate>
    <Triggers>
        <asp:AsyncPostBackTrigger ControlID="bUpdateFrameImages" EventName="Click" />
        <asp:AsyncPostBackTrigger ControlID="ddlFrameFamilyEdit" EventName="SelectedIndexChanged" />
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
