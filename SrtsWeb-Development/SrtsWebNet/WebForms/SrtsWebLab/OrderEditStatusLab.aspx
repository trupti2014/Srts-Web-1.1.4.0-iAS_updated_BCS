<%@ Page Title="" Language="C#" MasterPageFile="~/SrtsMaster.master" AutoEventWireup="true"
    CodeBehind="OrderEditStatusLab.aspx.cs" Inherits="SrtsWebLab.OrderEditStatusLab"
    EnableViewState="true" %>

<%@ MasterType VirtualPath="~/srtsMaster.Master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        function DoDialog() {
            var dialogOpts = {
                autoOpen: false,
                modal: true,
                width: 750,
                height: 400,
                title: 'Order History',
                dialogClass: 'generic'
            };
            var doh = $('#divOrderHistory').dialog(dialogOpts);
            doh.parent().appendTo($('form:first'));

            doh.dialog('open');
        }
    </script>
</asp:Content>
<asp:Content ID="contentOrderInformation" ContentPlaceHolderID="ContentTop_Title_Right"
    runat="server">
    <asp:Literal ID="litContentTop_Title_Right" runat="server"></asp:Literal>
</asp:Content>
<asp:Content ID="contentMenuLab" runat="server" ContentPlaceHolderID="contentSubMenuLab">
    <div class="button">
        <ul>
            <li>
                <asp:LinkButton ID="lnkOrderSearch" runat="server" PostBackUrl="~/SrtsWebLab/ManageOrdersLab.aspx?path=search"
                    CausesValidation="false" ToolTip="Select to perform a new search." Text="Order Search" />
            </li>
            <li>
                <asp:LinkButton ID="lnkOrderCheckin" runat="server" PostBackUrl="~/SrtsWebLab/ManageOrdersLab.aspx?path=checkin"
                    CausesValidation="false" ToolTip="" Text="Order Check-In" />
            </li>
            <li>
                <asp:LinkButton ID="lnkOrderDispense" runat="server" PostBackUrl="~/SrtsWebLab/ManageOrdersLab.aspx?path=dispense"
                    CausesValidation="false" ToolTip="" Text="Order Dispense" />
            </li>
        </ul>
    </div>
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="MainContent" runat="server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <div class="divSingleColumn" style="margin-top: 20px;">
                <div class="box_fullinner_top">
                </div>
                <div class="box_fullinner_content">
                    <div>
                        <table style="width: 100%;">
                            <tr>
                                <td style="width: 95%;">
                                    <srts:PatientDemographics ID="PatientDemographics" runat="server" />
                                </td>
                            </tr>
                        </table>
                    </div>
                    <asp:ValidationSummary ID="vsErrors" runat="server" DisplayMode="BulletList" ForeColor="Red"
                        ShowSummary="true" />
                    <div>
                        <p>
                            <asp:Label ID="lblPriority" runat="server" CssClass="srtsLabel_medium" Width="100px"
                                Text="This Order Priority:"></asp:Label>
                            <asp:DropDownList ID="ddlPriority" runat="server" TabIndex="1" DataTextField="Value"
                                DataValueField="Key" Enabled="false">
                            </asp:DropDownList>
                        </p>
                        <p>
                            <asp:Label ID="lblFOC" runat="server" CssClass="srtsLabel_medium" Width="100px" Text="Next FOC Date:"></asp:Label>
                            <asp:TextBox ID="tbFOCDate" runat="server" ReadOnly="true" CssClass="srtsTextBox_medium"></asp:TextBox>
                        </p>
                    </div>
                    <hr />
                    <div>
                        <h3>Deployment</h3>
                        <p>
                            <asp:Label ID="lblLocation" runat="server" CssClass="srtsLabel_medium" Width="100px"
                                Text="Location:"></asp:Label>
                            <asp:TextBox ID="tbLocation" runat="server" CssClass="srtsTextBox_medium" ReadOnly="true"></asp:TextBox>
                        </p>
                    </div>
                    <hr />
                    <div>
                        <h3>Frame</h3>
                        <asp:UpdatePanel ID="upSingleMulti" runat="server" UpdateMode="Always">
                            <ContentTemplate>
                                <p>
                                    <asp:RadioButtonList ID="rblSingleMulti" runat="server" TabIndex="4" Enabled="false">
                                        <asp:ListItem Text="Single Vision" Value="false"></asp:ListItem>
                                        <asp:ListItem Text="Multi Vision" Value="true"></asp:ListItem>
                                    </asp:RadioButtonList>
                                </p>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        <p>
                            <asp:Label ID="lblFrame" runat="server" CssClass="srtsLabel_medium" Width="100px"
                                Text="Frame:"></asp:Label>
                            <asp:TextBox ID="tbFrameCode" runat="server" CssClass="srtsTextBox_medium" ReadOnly="true"></asp:TextBox>
                        </p>
                        <p>
                            <asp:Label ID="lblColor" runat="server" CssClass="srtsLabel_medium" Width="100px"
                                Text="Color:"></asp:Label>
                            <asp:TextBox ID="tbFrameColor" runat="server" CssClass="srtsTextBox_medium" ReadOnly="true"></asp:TextBox>
                        </p>
                        <p>
                            <asp:Label ID="lblTemple" runat="server" CssClass="srtsLabel_medium" Width="100px"
                                Text="Temple:"></asp:Label>
                            <asp:TextBox ID="tbTempleSize" runat="server" CssClass="srtsTextBox_medium" ReadOnly="true"></asp:TextBox>
                        </p>
                        <p>
                            <asp:Label ID="lblEye" runat="server" CssClass="srtsLabel_medium" Width="100px" Text="Eye:"></asp:Label>
                            <asp:TextBox ID="tbEyeSize" runat="server" CssClass="srtsTextBox_medium" ReadOnly="true"></asp:TextBox>
                        </p>
                        <p>
                            <asp:Label ID="lblBridge" runat="server" CssClass="srtsLabel_medium" Width="100px"
                                Text="Bridge:"></asp:Label>
                            <asp:TextBox ID="tbBridgeSize" runat="server" CssClass="srtsTextBox_medium" ReadOnly="true"></asp:TextBox>
                        </p>
                        <p>
                            <asp:Label ID="lblLens" runat="server" CssClass="srtsLabel_medium" Width="100px"
                                Text="Lens:"></asp:Label>
                            <asp:TextBox ID="tbLens" runat="server" CssClass="srtsTextBox_medium" ReadOnly="true"></asp:TextBox>
                        </p>
                        <p>
                            <asp:Label ID="lblTint" runat="server" CssClass="srtsLabel_medium" Width="100px"
                                Text="Tint:"></asp:Label>
                            <asp:TextBox ID="tbTint" runat="server" CssClass="srtsTextBox_medium" ReadOnly="true"></asp:TextBox>
                        </p>
                        <p>
                            <asp:Label ID="lblMaterial" runat="server" CssClass="srtsLabel_medium" Width="100px"
                                Text="Material:"></asp:Label>
                            <asp:TextBox ID="tbMaterial" runat="server" CssClass="srtsTextBox_medium" ReadOnly="true"></asp:TextBox>
                        </p>
                        <p>
                            <asp:Label ID="lblPair" runat="server" CssClass="srtsLabel_medium" Width="100px"
                                Text="Pair:"></asp:Label>
                            <asp:TextBox ID="tbPair" runat="server" Width="100px" CssClass="srtsTextBox_medium"
                                TabIndex="13" ReadOnly="true"></asp:TextBox>
                        </p>
                        <p>
                            <asp:Label ID="lblCases" runat="server" CssClass="srtsLabel_medium" Width="100px"
                                Text="Cases:"></asp:Label>
                            <asp:TextBox ID="tbCases" runat="server" Width="100px" CssClass="srtsTextBox_medium"
                                TabIndex="14" ReadOnly="true"></asp:TextBox>
                    </div>
                    <hr />
                    <div>
                        <h3>Prescription</h3>
                        <asp:CheckBox ID="cbNewPrescription" runat="server" Visible="false" />
                        <table>
                            <colgroup>
                                <col style="width: 75px;" />
                                <col style="width: 50px;" />
                                <col style="width: 50px;" />
                                <col style="width: 50px;" />
                                <col style="width: 50px;" />
                                <col style="width: 50px;" />
                                <col style="width: 50px;" />
                                <col style="width: 50px;" />
                                <col style="width: 50px;" />
                                <col style="width: 50px;" />
                                <col style="width: 50px;" />
                            </colgroup>
                            <tr>
                                <th>&nbsp;</th>
                                <th>
                                    <label><strong>Sphere</strong></label></th>

                                <th>
                                    <label><strong>Cylinder</strong></label></th>

                                <th>
                                    <label><strong>Axis</strong></label></th>

                                <th>
                                    <label><strong>H-Prism</strong></label></th>

                                <th>
                                    <label><strong>H-Base</strong></label></th>

                                <th>
                                    <label><strong>V-Prism</strong></label></th>

                                <th>
                                    <label><strong>V-Base</strong></label></th>

                                <th>
                                    <label><strong>Add</strong></label></th>
                            </tr>
                            <tr>
                                <td style="text-align: right;">
                                    <label>
                                        <strong>Right(OD)</strong></label>
                                </td>
                                <td>
                                    <asp:TextBox ID="tbODSphere" runat="server" Width="50px" TabIndex="15" ReadOnly="true"></asp:TextBox></td>
                                <td>
                                    <asp:TextBox ID="tbODCylinder" runat="server" Width="50px" TabIndex="16"></asp:TextBox></td>
                                <td>
                                    <asp:TextBox ID="tbODAxis" runat="server" Width="50px" TabIndex="17" ReadOnly="true"></asp:TextBox></td>
                                <td>
                                    <asp:TextBox ID="tbODHPrism" runat="server" Width="50px" TabIndex="18" ReadOnly="true"></asp:TextBox></td>
                                <td>
                                    <asp:DropDownList ID="ddlODHBase" runat="server" TabIndex="19" Enabled="false">
                                        <asp:ListItem Text="N/A" Value=""></asp:ListItem>
                                        <asp:ListItem Text="IN" Value="I"></asp:ListItem>
                                        <asp:ListItem Text="OUT" Value="O"></asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                                <td>N
                                    <asp:TextBox ID="tbODVPrism" runat="server" Width="50px" TabIndex="20" ReadOnly="true"></asp:TextBox></td>
                                <td>
                                    <asp:DropDownList ID="ddlODVBase" runat="server" AutoPostBack="false" Enabled="false"
                                        TabIndex="21">
                                        <asp:ListItem Text="N/A" Value=""></asp:ListItem>
                                        <asp:ListItem Text="UP" Value="U"></asp:ListItem>
                                        <asp:ListItem Text="DOWN" Value="D"></asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                                <td>
                                    <asp:TextBox ID="tbODAdd" runat="server" Width="50px" TabIndex="22" ReadOnly="true"></asp:TextBox></td>

                                <td id="tablePDTotal" runat="server" rowspan="2" style="padding-top: 12px">
                                    <asp:TextBox ID="tbPDTotal" runat="server" Width="75px" Enabled="false" TabIndex="22"></asp:TextBox></td>

                                <td id="tablePDOD" runat="server">
                                    <asp:TextBox ID="tbPDOD" runat="server" Width="75px" Enabled="false" TabIndex="22"></asp:TextBox></td>

                                <td id="tablePDTotalNear" runat="server" rowspan="2" style="padding-top: 12px">
                                    <asp:TextBox ID="tbPDTotalNear" runat="server" Width="75px" Enabled="false" TabIndex="22"></asp:TextBox></td>

                                <td id="tablePDODNear" runat="server">
                                    <asp:TextBox ID="tbPDODNear" runat="server" Width="75px" Enabled="false" TabIndex="22"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <td style="text-align: right;">
                                    <label>
                                        <strong>Left(OS)</strong></label>
                                </td>
                                <td>
                                    <asp:TextBox ID="tbOSSphere" runat="server" Width="50px" TabIndex="23" ReadOnly="true"></asp:TextBox>
                                </td>
                                <td>
                                    <asp:TextBox ID="tbOSCylinder" runat="server" Width="50px" TabIndex="24" ReadOnly="true"></asp:TextBox>
                                </td>
                                <td>
                                    <asp:TextBox ID="tbOSAxis" runat="server" Width="50px" TabIndex="25" ReadOnly="true"></asp:TextBox>
                                </td>
                                <td>
                                    <asp:TextBox ID="tbOSHPrism" runat="server" Width="50px" TabIndex="26" ReadOnly="true"></asp:TextBox>
                                </td>
                                <td>
                                    <asp:DropDownList ID="ddlOSHBase" runat="server" AutoPostBack="false" Enabled="false"
                                        TabIndex="27">
                                        <asp:ListItem Text="N/A" Value=""></asp:ListItem>
                                        <asp:ListItem Text="IN" Value="I"></asp:ListItem>
                                        <asp:ListItem Text="OUT" Value="O"></asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                                <td>
                                    <asp:TextBox ID="tbOSVPrism" runat="server" Width="50px" TabIndex="28" ReadOnly="true"></asp:TextBox></td>
                                <td>
                                    <asp:DropDownList ID="ddlOSVBase" runat="server" AutoPostBack="false" Enabled="false"
                                        TabIndex="29">
                                        <asp:ListItem Text="N/A" Value=""></asp:ListItem>
                                        <asp:ListItem Text="UP" Value="U"></asp:ListItem>
                                        <asp:ListItem Text="DOWN" Value="D"></asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                                <td>
                                    <asp:TextBox ID="tbOSAdd" runat="server" Width="50px" TabIndex="30" ReadOnly="true"></asp:TextBox></td>
                                <td id="tablePDOS" runat="server">
                                    <asp:TextBox ID="tbPDOS" runat="server" Width="75px" Enabled="false" TabIndex="22"></asp:TextBox></td>

                                <td id="tablePDOSNear" runat="server">
                                    <asp:TextBox ID="tbPDOSNear" runat="server" Width="75px" Enabled="false" TabIndex="22"></asp:TextBox></td>
                            </tr>
                        </table>
                        <hr />
                        <table>
                            <colgroup>
                                <col style="width: 75px;" />
                                <col style="width: 50px;" />
                                <col style="width: 50px;" />
                            </colgroup>
                            <tr>
                                <th style="text-align: right;">&nbsp;
                                </th>
                                <th style="text-align: left;">
                                    <label>
                                        <strong>Right(OD):</strong></label>
                                </th>
                                <th style="text-align: left;">
                                    <label>
                                        <strong>Left(OS):</strong></label>
                                </th>
                            </tr>
                            <tr>
                                <td style="text-align: right;">
                                    <label>
                                        <strong>Seg Ht:</strong></label>
                                </td>
                                <td>
                                    <asp:TextBox ID="tbODSegHeight" runat="server" TabIndex="31" MaxLength="3" ReadOnly="true"></asp:TextBox>
                                </td>
                                <td>
                                    <asp:TextBox ID="tbOSSegHeight" runat="server" MaxLength="3" TabIndex="32" ReadOnly="true"></asp:TextBox>
                                </td>
                            </tr>
                        </table>
                    </div>
                    <hr />
                    <div>
                        <h3>Shipping Information</h3>
                    </div>
                    <div>
                        <p>
                            <asp:Label ID="lblAddrType" runat="server" CssClass="srtsLabel_medium" Width="100px"
                                Text="Address Type:"></asp:Label>
                            <asp:TextBox ID="tbAddressType" runat="server" CssClass="srtsTextBox_medium" ReadOnly="true"></asp:TextBox>
                        </p>
                        <p>
                            <asp:Label ID="lblAddr1" runat="server" CssClass="srtsLabel_medium" Width="100px"
                                Text="Address 1:"></asp:Label>
                            <asp:TextBox ID="tbStreet1" runat="server" MaxLength="100" TabIndex="42" Width="250px"
                                CssClass="srtsTextBox_medium" ReadOnly="true"></asp:TextBox>
                        </p>
                        <p>
                            <asp:Label ID="lblAddr2" runat="server" CssClass="srtsLabel_medium" Width="100px"
                                Text="Address 2:"></asp:Label>
                            <asp:TextBox ID="tbStreet2" runat="server" MaxLength="100" TabIndex="43" Width="250px"
                                CssClass="srtsTextBox_medium" ReadOnly="true"></asp:TextBox>
                        </p>
                        <p>
                            <asp:Label ID="lblZipCode" runat="server" CssClass="srtsLabel_medium" Width="100px"
                                Text="Zip Code:"></asp:Label>
                            <asp:TextBox ID="tbZipCode" runat="server" TabIndex="44" CssClass="srtsLabel_medium"
                                Width="250px" ReadOnly="true"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvZipCode" runat="server" Display="None" ErrorMessage="ZipCode is a required field"
                                ControlToValidate="tbZipCode"></asp:RequiredFieldValidator>
                            <asp:RegularExpressionValidator ID="revZipCode" runat="server" ControlToValidate="tbZipCode"
                                Display="None" ErrorMessage="ZipCode Is Not Formatted Correctly"
                                ValidationExpression="^\d{5}(\-\d{4})?$"></asp:RegularExpressionValidator>
                        </p>
                        <p>
                            <asp:Label ID="lblCity" runat="server" CssClass="srtsLabel_medium" Width="100px"
                                Text="City:"></asp:Label>
                            <asp:TextBox ID="tbCity" runat="server" MaxLength="100" TabIndex="45" CssClass="srtsTextBox_medium"
                                Width="250px" ReadOnly="true"></asp:TextBox>
                        </p>
                        <p>
                            <asp:Label ID="lblState" runat="server" CssClass="srtsLabel_medium" Width="100px"
                                Text="State:"></asp:Label>
                            <asp:TextBox ID="tbState" runat="server" CssClass="srtsTextBox_medium" ReadOnly="true"></asp:TextBox>
                        </p>
                        <p>
                            <asp:Label ID="lblCountry" runat="server" CssClass="srtsLabel_medium" Width="100px"
                                Text="Country:"></asp:Label>
                            <asp:TextBox ID="tbCountry" runat="server" CssClass="srtsTextBox_medium" ReadOnly="true"></asp:TextBox>
                        </p>
                    </div>
                    <hr />
                    <div>
                        <p>
                            <asp:Literal ID="litInfo" runat="server">Do Not Change the Lab selection unless this is being Resubmitted or Redirected"</asp:Literal><br />
                            <asp:Label ID="lblLab" runat="server" CssClass="srtsLabel_medium" Width="100px" Text="Laboratory:"></asp:Label>
                            <asp:DropDownList ID="ddlLab" runat="server" OnSelectedIndexChanged="ddlLab_SelectedIndexChanged"
                                TabIndex="49">
                            </asp:DropDownList>
                        </p>
                    </div>
                    <p>
                        <asp:Label ID="lblInfo" runat="server" CssClass="srtsLabel_medium" Width="100px"
                            Text="Prior Justification"></asp:Label><br />
                        <asp:Label ID="lblJustificationInfo" runat="server"></asp:Label><br />
                        <asp:Literal ID="litComment" runat="server"><strong>Justification:</strong></asp:Literal>
                        <asp:TextBox ID="tbComment" runat="server" TextMode="MultiLine" MaxLength="256" Width="500px"
                            TabIndex="50"></asp:TextBox><br />
                        <asp:RequiredFieldValidator ID="rfvJustification" runat="server" ControlToValidate="tbComment"
                            ErrorMessage="Justification is required" Display="None"></asp:RequiredFieldValidator>
                    </p>
                    <p>
                        <asp:Label ID="lblAction" runat="server" CssClass="srtsLabel_medium" Width="100px"
                            Text="Action To Take:"></asp:Label>
                        <asp:RadioButtonList ID="rblAction" runat="server" AutoPostBack="true" OnSelectedIndexChanged="rblAction_SelectedIndexChanged">
                            <asp:ListItem Selected="True" Text="Reject" Value="R"></asp:ListItem>
                            <asp:ListItem Text="Cancel" Value="C"></asp:ListItem>
                            <asp:ListItem Text="Redirect" Value="D"></asp:ListItem>
                        </asp:RadioButtonList>
                    </p>
                    <div>
                        <asp:Button ID="btnAdd" runat="server" CssClass="srtsButton" TabIndex="53" Text="Submit Data" OnClick="btnAdd_Click"
                            CausesValidation="true" />
                        <asp:Button ID="btnCancel" runat="server" CssClass="srtsButton" TabIndex="54" Text="Cancel Operation" OnClick="btnCancel_Click"
                            CausesValidation="false" />
                        <br />
                    </div>
                </div>
                <div class="box_fullinner_bottom">
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>


</asp:Content>
