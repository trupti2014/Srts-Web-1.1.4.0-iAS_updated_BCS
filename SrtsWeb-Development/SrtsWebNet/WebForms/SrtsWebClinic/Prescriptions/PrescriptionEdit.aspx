<%@ Page Title="" Language="C#" MasterPageFile="~/SrtsMaster.Master" AutoEventWireup="True"
    MaintainScrollPositionOnPostback="true" EnableViewState="true" CodeBehind="PrescriptionEdit.aspx.cs"
    Inherits="SrtsWebClinic.Prescriptions.PrescriptionEdit" %>

<%@ MasterType VirtualPath="~/srtsMaster.Master" %>

<asp:Content ID="Head" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content10" ContentPlaceHolderID="MainContent" runat="server">

    <asp:HiddenField ID="hfODSphereCalc" runat="server" />
    <asp:HiddenField ID="hfODCylinderCalc" runat="server" />
    <asp:HiddenField ID="hfODAxisCalc" runat="server" />
    <asp:HiddenField ID="hfOSSphereCalc" runat="server" />
    <asp:HiddenField ID="hfOSCylinderCalc" runat="server" />
    <asp:HiddenField ID="hfOSAxisCalc" runat="server" />
    <asp:HiddenField ID="hfODHPrism" runat="server" />
    <asp:HiddenField ID="hfODHBase" runat="server" />
    <asp:HiddenField ID="hfODVPrism" runat="server" />
    <asp:HiddenField ID="hfODVBase" runat="server" />
    <asp:HiddenField ID="hfOSHPrism" runat="server" />
    <asp:HiddenField ID="hfOSHBase" runat="server" />
    <asp:HiddenField ID="hfOSVPrism" runat="server" />
    <asp:HiddenField ID="hfOSVBase" runat="server" />

    <asp:ValidationSummary ID="vsErrors" runat="server" DisplayMode="BulletList" ShowSummary="true" CssClass="validatorSummary" />

    <div class="patientnameheader" style="padding-top: 20px; padding-bottom: 30px;">
        <asp:Literal ID="litPatientNameHeader" runat="server"></asp:Literal>
    </div>

    <div class="padding">
        <div id="divPrescriptionDetails" class="prescriptionDDLs" runat="server">
            <table id="prescriptionDetails" runat="server">
                <tr class="prescriptionLabel">
                    <td style="text-align: right;"></td>
                    <td>
                        <label>Sphere</label></td>
                    <td>
                        <label>Cylinder</label></td>
                    <td>
                        <label>Axis</label></td>
                    <td>
                        <label>H-Prism</label></td>
                    <td>
                        <label>H-Base</label></td>
                    <td>
                        <label>V-Prism</label></td>
                    <td>
                        <label>V-Base</label></td>
                    <td>
                        <label>Add</label></td>
                    <td>
                        <label>PD Dist</label></td>
                    <td>
                        <label>PD Near</label></td>
                </tr>
                <tr>
                    <td style="text-align: right;" class="prescriptionLabel">
                        <label>Right(OD)</label>
                    </td>
                    <td>
                        <asp:ListBox ID="lboxODSphere" runat="server" Rows="1" CssClass="listbox" onchange="ODSphere()" onclick="setVertPos()" TabIndex="1"></asp:ListBox>
                        <ajaxToolkit:ListSearchExtender runat="server" TargetControlID="lboxODSphere" ID="LSE_lboxODSphere" Enabled="True" PromptText="" QueryTimeout="2" PromptCssClass="listSearchPrompt" QueryPattern="Contains"></ajaxToolkit:ListSearchExtender>
                    </td>
                    <td>
                        <asp:ListBox ID="lboxODCylinder" runat="server" Rows="1" CssClass="listbox" TabIndex="2" onchange="ODCylinder()" onclick="setVertPos()"></asp:ListBox>
                        <ajaxToolkit:ListSearchExtender runat="server" TargetControlID="lboxODCylinder" ID="LSE_lboxODCylinder" Enabled="True" PromptText="" QueryTimeout="2" PromptCssClass="listSearchPrompt" QueryPattern="Contains"></ajaxToolkit:ListSearchExtender>
                    </td>
                    <td>
                        <asp:ListBox ID="lboxODAxis" runat="server" Rows="1" CssClass="listbox" onchange="ODAxis()" onclick="setVertPos()" TabIndex="3"></asp:ListBox>
                        <ajaxToolkit:ListSearchExtender runat="server" TargetControlID="lboxODAxis" ID="LSE_lboxODAxis" Enabled="True" PromptText="" QueryTimeout="2" PromptCssClass="listSearchPrompt" QueryPattern="Contains"></ajaxToolkit:ListSearchExtender>
                    </td>
                    <td>
                        <asp:ListBox ID="lboxODHPrism" runat="server" Rows="1" CssClass="listbox" OnSelectedIndexChanged="lboxODHPrism_SelectedIndexChanged" onclick="setVertPos()" onchange="getPrismVals()" TabIndex="4"></asp:ListBox>
                        <ajaxToolkit:ListSearchExtender runat="server" TargetControlID="lboxODHPrism" ID="LSE_lboxODHPrism" Enabled="True" PromptText="" QueryTimeout="2" PromptCssClass="listSearchPrompt"></ajaxToolkit:ListSearchExtender>
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlODHBase" runat="server" CssClass="listbox" OnSelectedIndexChanged="ddlODHBase_SelectedIndexChanged" onclick="setVertPos()" onchange="getPrismVals()" TabIndex="5"
                            ToolTip="Must have selection if associated prism has numeric value">
                            <asp:ListItem Text="N/A" Value="" Selected="True"></asp:ListItem>
                            <asp:ListItem Text="IN" Value="I"></asp:ListItem>
                            <asp:ListItem Text="OUT" Value="O"></asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td>
                        <asp:ListBox ID="lboxODVPrism" runat="server" Rows="1" CssClass="listbox" OnSelectedIndexChanged="lboxODVPrism_SelectedIndexChanged" onclick="setVertPos()" onchange="getPrismVals()" TabIndex="6"></asp:ListBox>
                        <ajaxToolkit:ListSearchExtender runat="server" TargetControlID="lboxODVPrism" ID="LSE_lboxODVPrism" Enabled="True" PromptText="" QueryTimeout="2" PromptCssClass="listSearchPrompt"></ajaxToolkit:ListSearchExtender>
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlODVBase" runat="server" CssClass="listbox" OnSelectedIndexChanged="ddlODVBase_SelectedIndexChanged" onclick="setVertPos()" onchange="getPrismVals()" TabIndex="7"
                            ToolTip="Must have selection if associated prism has numeric value">
                            <asp:ListItem Text="N/A" Value="" Selected="True"></asp:ListItem>
                            <asp:ListItem Text="UP" Value="U"></asp:ListItem>
                            <asp:ListItem Text="DOWN" Value="D"></asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td>
                        <asp:ListBox ID="lboxODAdd" runat="server" Rows="1" CssClass="listbox" onchange="setAddPower('OD')" onclick="setVertPos()" TabIndex="8"></asp:ListBox>
                        <ajaxToolkit:ListSearchExtender runat="server" TargetControlID="lboxODAdd" ID="LSE_lboxODAdd" Enabled="True" PromptText="" QueryTimeout="2" PromptCssClass="listSearchPrompt"></ajaxToolkit:ListSearchExtender>
                    </td>
                    <td id="tablePDTotal" runat="server" class="PDTotal" rowspan="2" style="padding-top: 23px">
                        <asp:ListBox ID="lboxPDTotal" runat="server" Rows="1" CssClass="listbox" TabIndex="17" onchange="setPDNear('T')"></asp:ListBox>
                        <ajaxToolkit:ListSearchExtender runat="server" TargetControlID="lboxPDTotal" ID="LSE_lboxPDTotal" Enabled="True" PromptText="" QueryTimeout="2" PromptCssClass="listSearchPrompt"></ajaxToolkit:ListSearchExtender>
                    </td>
                    <td id="tablePDOD" runat="server" class="PDMono">
                        <asp:ListBox ID="lboxPDOD" runat="server" Rows="1" CssClass="listbox" TabIndex="19" onchange="setPDNear('M')"></asp:ListBox>
                        <ajaxToolkit:ListSearchExtender runat="server" TargetControlID="lboxPDOD" ID="LSE_lboxPDOD" Enabled="True" PromptText="" QueryTimeout="2" PromptCssClass="listSearchPrompt"></ajaxToolkit:ListSearchExtender>
                    </td>
                    <td id="tablePDTotalNear" runat="server" class="PDTotal" rowspan="2" style="padding-top: 23px">
                        <asp:ListBox ID="lboxPDTotalNear" runat="server" Rows="1" CssClass="listbox" TabIndex="18"></asp:ListBox>
                        <ajaxToolkit:ListSearchExtender runat="server" TargetControlID="lboxPDTotalNear" ID="LSE_lboxPDTotalNear" Enabled="True" PromptText="" QueryTimeout="2" PromptCssClass="listSearchPrompt"></ajaxToolkit:ListSearchExtender>
                    </td>
                    <td id="tablePDODNear" runat="server" class="PDMono">
                        <asp:ListBox ID="lboxPDODNear" runat="server" Rows="1" CssClass="listbox" TabIndex="20"></asp:ListBox>
                        <ajaxToolkit:ListSearchExtender runat="server" TargetControlID="lboxPDODNear" ID="LSE_lboxPDODNear" Enabled="True" PromptText="" QueryTimeout="2" PromptCssClass="listSearchPrompt"></ajaxToolkit:ListSearchExtender>
                    </td>
                </tr>
                <tr>
                    <td style="text-align: right;">
                        <label class="prescriptionLabel">Left(OS)</label>
                    </td>
                    <td>
                        <asp:ListBox ID="lboxOSSphere" runat="server" Rows="1" CssClass="listboxBottom" onchange="OSSphere()" onclick="setVertPos()" TabIndex="9"></asp:ListBox>
                        <ajaxToolkit:ListSearchExtender runat="server" TargetControlID="lboxOSSphere" ID="LSE_lboxOSSphere" Enabled="True" PromptText="" QueryTimeout="2" PromptCssClass="listSearchPrompt" QueryPattern="Contains"></ajaxToolkit:ListSearchExtender>
                    </td>
                    <td>
                        <asp:ListBox ID="lboxOSCylinder" runat="server" Rows="1" CssClass="listboxBottom" onchange="OSCylinder()" onclick="setVertPos()" TabIndex="10"></asp:ListBox>
                        <ajaxToolkit:ListSearchExtender runat="server" TargetControlID="lboxOSCylinder" ID="LSE_lboxOSCylinder" Enabled="True" PromptText="" QueryTimeout="2" PromptCssClass="listSearchPrompt" QueryPattern="Contains"></ajaxToolkit:ListSearchExtender>
                    </td>
                    <td>
                        <asp:ListBox ID="lboxOSAxis" runat="server" Rows="1" CssClass="listboxBottom" onchange="OSAxis()" onclick="setVertPos()" TabIndex="11"></asp:ListBox>
                        <ajaxToolkit:ListSearchExtender runat="server" TargetControlID="lboxOSAxis" ID="LSE_lboxOSAxis" Enabled="True" PromptText="" QueryTimeout="2" PromptCssClass="listSearchPrompt" QueryPattern="Contains"></ajaxToolkit:ListSearchExtender>
                    </td>
                    <td>
                        <asp:ListBox ID="lboxOSHPrism" runat="server" Rows="1" CssClass="listboxBottom" OnSelectedIndexChanged="lboxOSHPrism_SelectedIndexChanged" onclick="setVertPos()" onchange="getPrismVals()" TabIndex="12"></asp:ListBox>
                        <ajaxToolkit:ListSearchExtender runat="server" TargetControlID="lboxOSHPrism" ID="LSE_lboxOSHPrism" Enabled="False" PromptText="" QueryTimeout="2" PromptCssClass="listSearchPrompt"></ajaxToolkit:ListSearchExtender>
                        <div class="prescriptionPrismEnable">
                            <asp:CheckBox ID="cbEnablePrism" runat="server" onclick="enablePrism()" Text="Enable Prism" />
                        </div>
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlOSHBase" runat="server" CssClass="listboxBottom" OnSelectedIndexChanged="ddlOSHBase_SelectedIndexChanged" onclick="setVertPos()" onchange="getPrismVals()" TabIndex="13"
                            ToolTip="Must have selection if associated prism has numeric value">
                            <asp:ListItem Text="N/A" Value="" Selected="True"></asp:ListItem>
                            <asp:ListItem Text="IN" Value="I"></asp:ListItem>
                            <asp:ListItem Text="OUT" Value="O"></asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td>
                        <asp:ListBox ID="lboxOSVPrism" runat="server" Rows="1" CssClass="listboxBottom" OnSelectedIndexChanged="lboxOSVPrism_SelectedIndexChanged" onclick="setVertPos()" onchange="getPrismVals()" TabIndex="14"></asp:ListBox>
                        <ajaxToolkit:ListSearchExtender runat="server" TargetControlID="lboxOSVPrism" ID="LSE_lboxOSVPrism" Enabled="False" PromptText="" QueryTimeout="2" PromptCssClass="listSearchPrompt"></ajaxToolkit:ListSearchExtender>
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlOSVBase" runat="server" CssClass="listboxBottom" OnSelectedIndexChanged="ddlOSVBase_SelectedIndexChanged" onclick="setVertPos()" onchange="getPrismVals()" TabIndex="15"
                            ToolTip="Must have selection if associated prism has numeric value">
                            <asp:ListItem Text="N/A" Value="" Selected="True"></asp:ListItem>
                            <asp:ListItem Text="UP" Value="U"></asp:ListItem>
                            <asp:ListItem Text="DOWN" Value="D"></asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td>
                        <asp:ListBox ID="lboxOSAdd" runat="server" Rows="1" CssClass="listboxBottom" onchange="setAddPower('OS')" onclick="setVertPos()" TabIndex="16"></asp:ListBox>
                        <ajaxToolkit:ListSearchExtender runat="server" TargetControlID="lboxOSAdd" ID="LSE_lboxOSAdd" Enabled="True" PromptText="" QueryTimeout="2" PromptCssClass="listSearchPrompt"></ajaxToolkit:ListSearchExtender>
                        <div id="divPDrbl" runat="server" class="prescriptionPDrbl">
                            <asp:RadioButtonList ID="rblPDMode" runat="server" ClientIDMode="AutoID" RepeatDirection="Horizontal" onclick="setPDFieldsToDisplay();" CssClass="rblPDMode">
                                <asp:ListItem Text="Total" Selected="True" Value="T" />
                                <asp:ListItem Text="Mono" Value="M" />
                            </asp:RadioButtonList>
                        </div>
                    </td>
                    <td id="tablePDOS" runat="server" class="PDMono">
                        <asp:ListBox ID="lboxPDOS" runat="server" Rows="1" CssClass="listboxBottom" TabIndex="21" onchange="setPDNear('M')"></asp:ListBox>
                        <ajaxToolkit:ListSearchExtender runat="server" TargetControlID="lboxPDOS" ID="LSE_lboxPDOS" Enabled="True" PromptText="" QueryTimeout="2" PromptCssClass="listSearchPrompt"></ajaxToolkit:ListSearchExtender>
                    </td>
                    <td id="tablePDOSNear" runat="server" class="PDMono">
                        <asp:ListBox ID="lboxPDOSNear" runat="server" Rows="1" CssClass="listboxBottom" TabIndex="22"></asp:ListBox>
                        <ajaxToolkit:ListSearchExtender runat="server" TargetControlID="lboxPDOSNear" ID="LSE_lboxPDOSNear" Enabled="True" PromptText="" QueryTimeout="2" PromptCssClass="listSearchPrompt"></ajaxToolkit:ListSearchExtender>
                    </td>
                </tr>
            </table>
        </div>
        <asp:Panel ID="pnlCalcValues" Height="140px" runat="server" HorizontalAlign="Center">
            <div class="prescriptionCalcValues" id="CalcValuesDiv" style="display: none;">
                <fieldset>
                    <legend>Calculated Values</legend>
                    <table style="width: 95%;">
                        <tr>
                            <td style="background-color: #FFFFFF"></td>
                            <td>
                                <label>Sphere</label></td>
                            <td>
                                <label>Cylinder</label></td>
                            <td>
                                <label>Axis</label></td>
                        </tr>
                        <tr>
                            <td style="text-align: right;">
                                <label>Right(OD)&nbsp;&nbsp;</label></td>
                            <td class="noColor">
                                <asp:Label ID="lblODSphere_calc" runat="server" Text="Plano" CssClass="prescriptionCalcValuesLabels"></asp:Label>
                            </td>
                            <td class="noColor">
                                <asp:Label ID="lblODCylinder_calc" runat="server" Text="Sphere" CssClass="prescriptionCalcValuesLabels"></asp:Label>
                            </td>
                            <td class="noColor">
                                <asp:Label ID="lblODAxis_calc" runat="server" Text="N/A" CssClass="prescriptionCalcValuesLabels"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td style="text-align: right;">
                                <label>
                                    Left(OS)&nbsp;&nbsp;</label>
                            </td>
                            <td class="noColor">
                                <asp:Label ID="lblOSSphere_calc" runat="server" Text="Plano" CssClass="prescriptionCalcValuesLabels"></asp:Label>
                            </td>
                            <td class="noColor">
                                <asp:Label ID="lblOSCylinder_calc" runat="server" Text="Sphere" CssClass="prescriptionCalcValuesLabels"></asp:Label>
                            </td>
                            <td class="noColor">
                                <asp:Label ID="lblOSAxis_calc" runat="server" Text="N/A" CssClass="prescriptionCalcValuesLabels"></asp:Label>
                            </td>
                        </tr>
                    </table>
                </fieldset>
            </div>
        </asp:Panel>
    </div>
    <div style="text-align: center" class="srtsLabel_medium">
        <div style="padding-left: 30px; text-align: left;">
            <asp:CheckBox ID="chkboxRemove" runat="server" Text=" Remove this prescription" />
        </div>
        <asp:Literal ID="litDoctors" runat="server" Text="Select Provider: "></asp:Literal>
        <asp:DropDownList ID="ddlDoctors" runat="server" TabIndex="23"
            ToolTip="Select the Provider who wrote the prescription" DataTextField="NameLFMi"
            DataValueField="ID">
        </asp:DropDownList>
        <asp:RequiredFieldValidator ID="rfvDoctor" runat="server" InitialValue="-1" Display="None"
            ControlToValidate="ddlDoctors" ErrorMessage="Selection of Provider is required">*</asp:RequiredFieldValidator>
        <br />
        <br />
        <br />
        <asp:Button ID="bSave" runat="server" CssClass="srtsButton" Text="Save As New" OnClick="bSave_Click" TabIndex="24" />&nbsp;
        <asp:Button ID="bUpdate" runat="server" CssClass="srtsButton" Text="Update" OnClick="bUpdate_Click" TabIndex="25" />&nbsp;
        <asp:Button ID="bCancel" runat="server" CssClass="srtsButton" Text="Cancel" OnClick="bCancel_Click" CausesValidation="False" TabIndex="26" />
    </div>
    <br />
    <asp:ScriptManagerProxy ID="smpProxy" runat="server">
        <Scripts>
            <asp:ScriptReference Path="~/Scripts/prescription.js" />
            <asp:ScriptReference Path="~/JavaScript/jsValidators.js" />
        </Scripts>
    </asp:ScriptManagerProxy>
    <script type="text/javascript">
        $(document).ready(function () {
            setPDFieldsToDisplay();
        });
    </script>
</asp:Content>