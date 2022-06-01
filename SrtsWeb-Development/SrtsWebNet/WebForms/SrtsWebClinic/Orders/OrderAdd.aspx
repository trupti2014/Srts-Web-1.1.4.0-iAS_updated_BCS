<%@ Page Title="" Language="C#" MasterPageFile="~/SrtsMaster.master" AutoEventWireup="True"
    CodeBehind="OrderAdd.aspx.cs" Inherits="SrtsWebClinic.Orders.OrderAdd" %>

<%@ MasterType VirtualPath="~/srtsMaster.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="PatientInformation" ContentPlaceHolderID="ContentTop_Title_Right"
    runat="server">

    <asp:Literal ID="litContentTop_Title_Right" runat="server"></asp:Literal>
    <br />
    <asp:Literal ID="litBoS" runat="server"></asp:Literal>
    <br />
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="MainContent" runat="server">
    <div class="pageMenu_Sub" style="margin-top: -25px">
        <div class="rightcorner">
        </div>
        <div class="leftcorner">
        </div>
    </div>
    <div class="padding_header">
        <div style="width: 975px; margin-right: auto; margin-left: auto;">
            <div id="divContainerBeige3">
                <div class="containertop">
                    <div class="containertop_right">
                    </div>
                    <div class="containertop_left">
                    </div>
                </div>
                <div class="container_content">
                    <div class="container_inner">
                        <div id="divAddOrderWarnings" class="divWarnings" runat="server"></div>
                        <asp:ValidationSummary ID="vsErrors" runat="server" DisplayMode="BulletList" ForeColor="Red" CssClass="validatorSummary" />
                        <h1>Prescription</h1>
                        <div class="padding">
                            <table>
                                <colgroup>
                                    <col style="width: 85px" />
                                    <col style="width: 85px" />
                                    <col style="width: 85px" />
                                    <col style="width: 85px" />
                                    <col style="width: 85px" />
                                    <col style="width: 85px" />
                                    <col style="width: 85px" />
                                    <col style="width: 85px" />
                                    <col style="width: 85px" />
                                    <col style="width: 85px" />
                                    <col style="width: 85px" />
                                </colgroup>
                                <tr>
                                    <td>&nbsp;</td>
                                    <td>
                                        <label class="srtsLabel_medium">Sphere</label></td>
                                    <td>
                                        <label class="srtsLabel_medium">Cylinder</label>
                                    </td>
                                    <td>
                                        <label class="srtsLabel_medium">Axis</label></td>
                                    <td>
                                        <label class="srtsLabel_medium">H-Prism</label></td>
                                    <td>
                                        <label class="srtsLabel_medium">H-Base</label></td>
                                    <td>
                                        <label class="srtsLabel_medium">V-Prism</label></td>
                                    <td>
                                        <label class="srtsLabel_medium">V-Base</label></td>
                                    <td>
                                        <label class="srtsLabel_medium">Add</label></td>
                                    <td>
                                        <label class="srtsLabel_medium">PD</label></td>
                                    <td>
                                        <label class="srtsLabel_medium">Near</label></td>
                                </tr>
                                <tr>
                                    <td style="text-align: right;">
                                        <label class="srtsLabel_medium">Right(OD)&nbsp;</label></td>
                                    <td>
                                        <asp:TextBox ID="tbODSphere" runat="server" Width="75px" Enabled="false"></asp:TextBox></td>
                                    <td>
                                        <asp:TextBox ID="tbODCylinder" runat="server" Width="75px" Enabled="false"></asp:TextBox></td>
                                    <td>
                                        <asp:TextBox ID="tbODAxis" runat="server" Width="75px" Enabled="false"></asp:TextBox></td>
                                    <td>
                                        <asp:TextBox ID="tbODHPrism" runat="server" Width="75px" Enabled="false"></asp:TextBox></td>
                                    <td>
                                        <asp:DropDownList ID="ddlODHBase" runat="server" Width="75px" Enabled="false">
                                            <asp:ListItem Text="N/A" Value="" Selected="True"></asp:ListItem>
                                            <asp:ListItem Text="IN" Value="I"></asp:ListItem>
                                            <asp:ListItem Text="OUT" Value="O"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="tbODVPrism" runat="server" Width="75px" Enabled="false"></asp:TextBox></td>
                                    <td>
                                        <asp:DropDownList ID="ddlODVBase" runat="server" Width="75px" Enabled="false">
                                            <asp:ListItem Text="N/A" Value="" Selected="True"></asp:ListItem>
                                            <asp:ListItem Text="UP" Value="U"></asp:ListItem>
                                            <asp:ListItem Text="DOWN" Value="D"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="tbODAdd" runat="server" Width="75px" Enabled="false"></asp:TextBox></td>
                                    <td id="tablePDTotal" runat="server" rowspan="2" style="padding-top: 12px">
                                        <asp:TextBox ID="tbPDTotal" runat="server" Width="75px" Enabled="false"></asp:TextBox></td>
                                    <td id="tablePDOD" runat="server">
                                        <asp:TextBox ID="tbPDOD" runat="server" Width="75px" Enabled="false"></asp:TextBox></td>
                                    <td id="tablePDTotalNear" runat="server" rowspan="2" style="padding-top: 12px">
                                        <asp:TextBox ID="tbPDTotalNear" runat="server" Width="75px" Enabled="false"></asp:TextBox></td>
                                    <td id="tablePDODNear" runat="server">
                                        <asp:TextBox ID="tbPDODNear" runat="server" Width="75px" Enabled="false"></asp:TextBox></td>
                                </tr>
                                <tr>
                                    <td style="text-align: right;">
                                        <label class="srtsLabel_medium">Left(OS)&nbsp;</label></td>
                                    <td>
                                        <asp:TextBox ID="tbOSSphere" runat="server" Width="75px" Enabled="false"></asp:TextBox></td>
                                    <td>
                                        <asp:TextBox ID="tbOSCylinder" runat="server" Width="75px" Enabled="false"></asp:TextBox></td>
                                    <td>
                                        <asp:TextBox ID="tbOSAxis" runat="server" Width="75px" Enabled="false"></asp:TextBox></td>
                                    <td>
                                        <asp:TextBox ID="tbOSHPrism" runat="server" Width="75px" Enabled="false"></asp:TextBox></td>
                                    <td>
                                        <asp:DropDownList ID="ddlOSHBase" runat="server" Width="75px" Enabled="false">
                                            <asp:ListItem Text="N/A" Value="" Selected="True"></asp:ListItem>
                                            <asp:ListItem Text="IN" Value="I"></asp:ListItem>
                                            <asp:ListItem Text="OUT" Value="O"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="tbOSVPrism" runat="server" Width="75px" Enabled="false"></asp:TextBox></td>
                                    <td>
                                        <asp:DropDownList ID="ddlOSVBase" runat="server" Width="75px" Enabled="false">
                                            <asp:ListItem Text="N/A" Value="" Selected="True"></asp:ListItem>
                                            <asp:ListItem Text="UP" Value="U"></asp:ListItem>
                                            <asp:ListItem Text="DOWN" Value="D"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="tbOSAdd" runat="server" Width="75px" Enabled="false"></asp:TextBox></td>
                                    <td id="tablePDOS" runat="server">
                                        <asp:TextBox ID="tbPDOS" runat="server" Width="75px" Enabled="false"></asp:TextBox></td>
                                    <td id="tablePDOSNear" runat="server">
                                        <asp:TextBox ID="tbPDOSNear" runat="server" Width="75px" Enabled="false"></asp:TextBox></td>
                                </tr>
                            </table>
                        </div>
                        <hr />
                        <h1>Eyeware Information</h1>
                        <div class="padding">
                            <div>
                                <div class="div200pxleft">
                                    <label class="srtsLabel_medium">This Order Priority:</label>
                                    <br />
                                    <asp:DropDownList ID="ddlPriority" runat="server" TabIndex="1" AutoPostBack="True"
                                        OnSelectedIndexChanged="ddlPriority_SelectedIndexChanged" DataTextField="Value"
                                        DataValueField="Key">
                                    </asp:DropDownList>
                                </div>

                                <div class="div200pxleft">
                                    <label class="srtsLabel_medium">Next FOC Date:</label><br />
                                    <asp:Label ID="lblFOCDate" runat="server" CssClass="srtsLabel_medium" Style="padding-left: 20px;" />
                                </div>

                                <div class="div200pxleft" style="padding-left: 25px;">
                                    <label class="srtsLabel_medium">Production Lab:</label><br />
                                    <%--<asp:TextBox ID="tbLab" runat="server" Enabled="false" CssClass="srtsDateTextBox_medium"></asp:TextBox>--%>
                                    <asp:DropDownList ID="ddlLab" runat="server" OnSelectedIndexChanged="ddlLab_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                                </div>

                                <div class="div200pxleft" style="padding-left: 25px;">
                                    <label class="srtsLabel_medium">Technician:</label><br />
                                    <%--<asp:DropDownList ID="ddlTechnician" runat="server" Width="200px" DataTextField="NameLF" DataValueField="ID" Enabled="false"></asp:DropDownList>--%>
                                    <asp:TextBox ID="tbTechnician" runat="server" ReadOnly="true"></asp:TextBox>
                                </div>

                                <div class="div200pxleft">
                                    <asp:Label ID="lblVerification" runat="server" Text="Eligibility Verified By:" CssClass="srtsLabel_medium"></asp:Label>
                                    <asp:DropDownList ID="ddlVerification" runat="server" Width="200px" DataTextField="NameLF" DataValueField="ID" TabIndex="2"></asp:DropDownList>
                                </div>
                            </div>
                            <div id="divDeployment" runat="server" visible="false">
                                <h1>Deployment</h1>
                                <table style="margin-left: 25px;">
                                    <colgroup>
                                        <col style="width: 150px" />
                                        <col style="width: 200px" />
                                    </colgroup>
                                    <tr>
                                        <td>
                                            <label class="srtsLabel_medium">Location:</label>
                                        </td>
                                        <td>
                                            <label class="srtsLabel_medium">Start Date:</label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:DropDownList ID="ddlDeployLocation" runat="server" TabIndex="3">
                                            </asp:DropDownList>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="tbDeployStartDate" runat="server" TabIndex="4"></asp:TextBox>
                                            <asp:Image runat="server" ID="calImage1" AlternateText="Calendar" ImageUrl="~/Styles/images/Calendar_scheduleHS.png" />
                                            <ajaxToolkit:CalendarExtender ID="ceExamDate" runat="server" TargetControlID="tbDeployStartDate"
                                                Format="MM/dd/yyyy" PopupButtonID="calImage1">
                                            </ajaxToolkit:CalendarExtender>
                                        </td>
                                    </tr>
                                </table>
                            </div>
                            <br />
                            <table class="orderaddtable" style="margin-left: 0px; width: 900px">
                                <tr>
                                    <td>
                                        <label class="srtsLabel_medium">Frame:</label>
                                    </td>
                                    <td>
                                        <label class="srtsLabel_medium" style="padding-left: 10px">Color:</label>
                                    </td>
                                    <td>
                                        <label class="srtsLabel_medium" style="padding-left: 10px">Temple:</label>
                                    </td>
                                    <td>
                                        <label class="srtsLabel_medium" style="padding-left: 10px">Eye:</label>
                                    </td>
                                    <td>
                                        <label class="srtsLabel_medium" style="padding-left: 10px">Bridge:</label>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="text-align: left;">
                                        <asp:DropDownList ID="ddlFrame" runat="server" TabIndex="6" AutoPostBack="True" OnSelectedIndexChanged="ddlFrame_SelectedIndexChanged"
                                            DataTextField="FrameLongDescription" DataValueField="FrameCode" Width="300px">
                                        </asp:DropDownList>
                                        <asp:RequiredFieldValidator ID="rfvFrame" runat="server" Display="None" Text="*" ControlToValidate="ddlFrame"
                                            InitialValue="X" ErrorMessage="Frame is required"></asp:RequiredFieldValidator>
                                    </td>
                                    <td style="text-align: left; padding-left: 10px">
                                        <asp:DropDownList ID="ddlColor" runat="server" DataTextField="Text" DataValueField="Value"
                                            TabIndex="7" Width="200px">
                                        </asp:DropDownList>
                                        <asp:RequiredFieldValidator ID="rfvColor" runat="server" Display="None" Text="*" ControlToValidate="ddlColor"
                                            InitialValue="X" ErrorMessage="Color is required"></asp:RequiredFieldValidator>
                                    </td>
                                    <td style="text-align: left; padding-left: 10px">
                                        <asp:DropDownList ID="ddlTemple" runat="server" DataTextField="Text" DataValueField="Value"
                                            TabIndex="8" Width="275px">
                                        </asp:DropDownList>
                                        <asp:RequiredFieldValidator ID="rfvTemple" runat="server" Display="None" Text="*" ControlToValidate="ddlTemple"
                                            InitialValue="X" ErrorMessage="Temple is required"></asp:RequiredFieldValidator>
                                    </td>
                                    <td style="text-align: left; padding-left: 10px">
                                        <asp:DropDownList ID="ddlEye" runat="server" DataTextField="Text" DataValueField="Value"
                                            TabIndex="9" Width="50px">
                                        </asp:DropDownList>
                                        <asp:RequiredFieldValidator ID="rfvEye" runat="server" Display="None" Text="*" ControlToValidate="ddlEye"
                                            InitialValue="X" ErrorMessage="Eye is required"></asp:RequiredFieldValidator>
                                    </td>
                                    <td style="text-align: left; padding-left: 10px">
                                        <asp:DropDownList ID="ddlBridge" runat="server" DataTextField="Text" DataValueField="Value"
                                            TabIndex="10" Width="50px">
                                        </asp:DropDownList>
                                        <asp:RequiredFieldValidator ID="rfvBridge" runat="server" Display="None" Text="*" ControlToValidate="ddlBridge"
                                            InitialValue="X" ErrorMessage="Bridge is required"></asp:RequiredFieldValidator>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <label class="srtsLabel_medium">Lens:</label>
                                    </td>
                                    <td>
                                        <label class="srtsLabel_medium" style="padding-left: 10px">Tint:</label>
                                    </td>
                                    <td>
                                        <label class="srtsLabel_medium" style="padding-left: 10px">Material:</label>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="text-align: left;">
                                        <asp:DropDownList ID="ddlLens" runat="server" DataTextField="Text" DataValueField="Value"
                                            TabIndex="11" AutoPostBack="True" OnSelectedIndexChanged="ddlLens_SelectedIndexChanged" Width="300px">
                                        </asp:DropDownList>
                                        <asp:RequiredFieldValidator ID="rfvLens" runat="server" Display="None" Text="*" ControlToValidate="ddlLens"
                                            InitialValue="X" ErrorMessage="Lens is required"></asp:RequiredFieldValidator>
                                    </td>
                                    <td style="text-align: left; padding-left: 10px">
                                        <asp:DropDownList ID="ddlTint" runat="server" DataTextField="Text" DataValueField="Value"
                                            TabIndex="12" Width="200px">
                                        </asp:DropDownList>
                                        <asp:RequiredFieldValidator ID="rfvTint" runat="server" Display="None" Text="*" ControlToValidate="ddlTint"
                                            InitialValue="X" ErrorMessage="Tint is required"></asp:RequiredFieldValidator>
                                    </td>
                                    <td style="text-align: left; padding-left: 10px">
                                        <asp:DropDownList ID="ddlMaterial" runat="server" DataTextField="Text" DataValueField="Value" AutoPostBack="True"
                                            OnSelectedIndexChanged="ddlMaterial_SelectedIndexChanged" TabIndex="13" Width="275px">
                                        </asp:DropDownList>
                                        <asp:RequiredFieldValidator ID="rfvMaterial" runat="server" Display="None" Text="*" ControlToValidate="ddlMaterial"
                                            InitialValue="X" ErrorMessage="Material is required"></asp:RequiredFieldValidator>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <label class="srtsLabel_medium">Segment Height:</label>
                                    </td>
                                    <td>
                                        <div style="width: 35%; float: left; padding-left: 10px;">
                                            <label class="srtsLabel_medium">Pair:</label>
                                        </div>
                                        <div style="width: 50%; float: left; text-align: right; padding-right: 15px;">
                                            <label class="srtsLabel_medium">Cases:</label>
                                        </div>
                                    </td>
                                    <td>
                                        <label class="srtsLabel_medium" style="padding-left: 10px">Ship To:</label>
                                    </td>
                                </tr>
                            </table>
                            <div class="fullwidth">
                                <div class="framecolumnleft">
                                    <div style="float: left; padding-left: 25px; margin-top: 5px">
                                        <label class="srtsLabel_LeftRed" style="padding-top: 5px">Right(OD):</label>
                                        <asp:TextBox ID="tbODSegHeight" runat="server" TabIndex="14" MaxLength="3" Width="25px" onchange="setSegHt()"
                                            ToolTip="Seg Height valid values are 10 - 35 or 3B or 4B" CssClass="srtsTextBox_small"></asp:TextBox>
                                    </div>
                                    <div style="float: left; padding-left: 55px; margin-top: 5px">
                                        <label class="srtsLabel_LeftRed" style="padding-top: 5px">Left(OS):</label>
                                        <asp:TextBox ID="tbOSSegHeight" runat="server" MaxLength="3" TabIndex="15" Width="25px"
                                            ToolTip="Seg Height valid values are 10 - 35 or 3B or 4B" CssClass="srtsTextBox_small"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="framecolumnmid">
                                    <div class="left">
                                        <asp:TextBox ID="tbPair" runat="server" MaxLength="1" TabIndex="16" CssClass="addorder_tbsmall"></asp:TextBox>
                                    </div>
                                    <div class="right">
                                        <asp:TextBox ID="tbCases" runat="server" MaxLength="1" TabIndex="17" CssClass="addorder_tbsmall"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="framecolumnright">
                                    <div style="float: left;">
                                        <asp:RadioButtonList ID="rblShipTo" runat="server" onchange="setShipTo()" RepeatDirection="Horizontal" ForeColor="#782E1E" TabIndex="18">
                                            <asp:ListItem Text="Clinic" Value="C" Selected="True"></asp:ListItem>
                                            <asp:ListItem Text="Patient" Value="P"></asp:ListItem>
                                        </asp:RadioButtonList>
                                    </div>
                                </div>
                            </div>
                            <asp:CustomValidator ID="cvOdSegHeight"
                                runat="server"
                                ControlToValidate="tbODSegHeight"
                                ClientValidationFunction="ValidateSegHt"
                                ErrorMessage="OD Seg Height valid values are 10 - 35 or 3B or 4B"
                                Display="None"></asp:CustomValidator>
                            <asp:CustomValidator ID="cvOsSegHeight"
                                runat="server"
                                ControlToValidate="tbOSSegHeight"
                                ClientValidationFunction="ValidateSegHt"
                                ErrorMessage="OS Seg Height valid values are 10 - 35 or 3B or 4B"
                                Display="None"></asp:CustomValidator>
                        </div>
                        <br />
                        <hr />
                        <div style="text-align: center;">
                            <asp:Label ID="lblCharRemain" Style="color: #782E1E;" ClientIDMode="Static" runat="server"></asp:Label>
                        </div>
                        <div id="divOrderJust" runat="server" class="padding" visible="false" style="width: 100%">
                            <div id="divFocJust" runat="server" style="width: 33%; float: left; text-align: left;" visible="false">
                                <label style="color: #782E1E; font-weight: bold;">FOC Justification</label><br />
                                <asp:TextBox ID="tbFocJust" runat="server" ClientIDMode="Static" TabIndex="30" Width="90%" onKeyDown="return textboxMaxCommentSize(this, 180, event, 'lblCharRemain', 'tbFocJust')"
                                    ToolTip="Enter justification for 2nd pair of FOC within the last year, valid characters include + - . , ! ? # ' / ( )"></asp:TextBox>
                                <asp:CustomValidator ID="cvFocJust" runat="server" ErrorMessage="FOC order placed in the last year, justify another FOC order."
                                    Text="*" ControlToValidate="tbFocJust" OnServerValidate="ValidateFocJust" ValidateEmptyText="True" />
                            </div>
                            <div id="divMaterialJust" runat="server" style="width: 33%; float: left; text-align: left;" visible="false">
                                <label style="color: #782E1E; font-weight: bold;">Material Justification</label><br />
                                <asp:TextBox ID="tbMaterialJust" runat="server" ClientIDMode="Static" TabIndex="31" Width="90%" onKeyDown="return textboxMaxCommentSize(this, 180, event, 'lblCharRemain', 'tbMaterialJust')"
                                    ToolTip="Enter justification for non-standard material, valid characters include + - . , ! ? # ' / ( )"></asp:TextBox>
                                <asp:CustomValidator ID="cvMaterialJust" runat="server" ErrorMessage="FOC order placed in the last year, justify another FOC order."
                                    Text="*" ControlToValidate="tbMaterialJust" OnServerValidate="ValidateMaterialJust" ValidateEmptyText="True" />
                            </div>
                        </div>
                        <div class="padding">
                            <div style="width: 50%; height: 15px; float: left; text-align: left;">
                                <label style="color: #782E1E">Comment 1:</label><br />
                            </div>
                            <div style="width: 50%; height: 15px; float: left; text-align: left; padding-left: 10px;">
                                <label style="color: #782E1E">Comment 2:</label><br />
                            </div>
                            <div style="width: 50%; float: left; clear: left">
                                <asp:TextBox ID="tbComment1" runat="server" ClientIDMode="Static" TextMode="MultiLine" Height="50px" Width="95%" TabIndex="32"
                                    onKeyDown="return textboxMaxCommentSize(this, 180, event, 'lblCharRemain', 'tbComment1')" ToolTip="Enter any comments, valid characters include + - . , ! ? # ' / ( )"></asp:TextBox>
                                <asp:CustomValidator ID="cvComment1" runat="server" ErrorMessage="Invalid character(s) in Comment 1" ClientValidationFunction="textboxCommentValidation" OnServerValidate="ValidateComment1Format" ControlToValidate="tbComment1" Display="Static" Text="*"></asp:CustomValidator>
                            </div>
                            <div style="width: 50%; float: left;">
                                <asp:TextBox ID="tbComment2" runat="server" ClientIDMode="Static" TextMode="MultiLine" Height="50px" Width="95%" TabIndex="33"
                                    onKeyDown="return textboxMaxCommentSize(this, 180, event, 'lblCharRemain', 'tbComment2')" ToolTip="Enter any comments, valid characters include + - . , ! ? # ' / ( )"></asp:TextBox>
                                <asp:CustomValidator ID="cvComment2" runat="server" ErrorMessage="Invalid character(s) in Comment 2" ClientValidationFunction="textboxCommentValidation" OnServerValidate="ValidateComment2Format" ControlToValidate="tbComment2" Text="*"></asp:CustomValidator>
                            </div>
                        </div>
                        <br />
                        <div style="text-align: center">
                            <asp:Button ID="btnAdd" runat="server" CssClass="srtsButton" TabIndex="25" Text='Submit Order' OnClick="btnAdd_Click" CausesValidation="true" />
                            <asp:Button ID="btnCancel" runat="server" CssClass="srtsButton" TabIndex="26" Text="Cancel" CausesValidation="false" OnClick="btnCancel_Click" />
                            <asp:Button ID="btnIncomplete" runat="server" CssClass="srtsButton" TabIndex="27" Text="Save Incomplete Order" CausesValidation="false" OnClick="btnIncomplete_Click" />
                        </div>
                    </div>
                </div>
                <div class="containerbottom">
                    <div class="containerbottom_right">
                    </div>
                    <div class="containerbottom_left">
                    </div>
                </div>
            </div>
        </div>
    </div>
    <asp:ScriptManagerProxy ID="smpProxy" runat="server">
        <Scripts>
            <asp:ScriptReference Path="~/Scripts/order.js" />
            <asp:ScriptReference Path="~/JavaScript/jsValidators.js" />
        </Scripts>
    </asp:ScriptManagerProxy>
</asp:Content>