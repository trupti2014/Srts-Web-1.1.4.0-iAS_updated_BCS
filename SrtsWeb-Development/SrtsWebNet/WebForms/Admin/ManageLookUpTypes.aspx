<%@ Page Language="C#" AutoEventWireup="True" MasterPageFile="~/SrtsMaster.master"
    CodeBehind="ManageLookUpTypes.aspx.cs" Inherits="SrtsWeb.Admin.ManageLookUpTypes" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <div id="divpagecontent" runat="server">
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <div>
                    <div>
                        <asp:Label ID="lblSelectType" runat="server" Text="Please select a lookup type to manage. " />
                        <ajaxToolkit:ComboBox ID="ddlLookUpTable" runat="server" DropDownStyle="DropDownList"
                            AutoPostBack="true" AutoCompleteMode="SuggestAppend" OnSelectedIndexChanged="ddlLookUpTable_SelectedIndexChanged" />
                    </div>
                    <asp:GridView ID="grdLookUpTables" runat="server" AutoGenerateColumns="false" DataKeyNames="ID"
                        GridLines="None" AllowPaging="false" CssClass="mGrid" PagerStyle-CssClass="pgr"
                        AlternatingRowStyle-CssClass="alt" ShowFooter="True" OnRowCancelingEdit="grdLookUpTables_RowCancelingEdit"
                        OnRowCommand="grdLookUpTables_RowCommand" OnRowDeleting="grdLookUpTables_RowDeleting"
                        OnRowEditing="grdLookUpTables_RowEditing" OnRowUpdating="grdLookUpTables_RowUpdating">
                        <PagerSettings Mode="NextPreviousFirstLast" Position="TopAndBottom" FirstPageText="<< First"
                            NextPageText="Next >" PreviousPageText="< Previous" LastPageText="Last >>" />
                        <Columns>
                            <asp:BoundField DataField="ID" Visible="false" />
                            <asp:TemplateField HeaderText="Code" HeaderStyle-HorizontalAlign="Left" ControlStyle-Width="50px">
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtLookUpTableCode" runat="server" Text='<%# Bind("Code") %>' Width="50px"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvLookUpTableCode" ValidationGroup="Update" runat="server"
                                        ControlToValidate="txtLookUpTableCode" ErrorMessage="Please Enter Lookup Type Code"
                                        ToolTip="Please Enter Lookup Type Code" SetFocusOnError="false" ForeColor="Red">*</asp:RequiredFieldValidator>
                                </EditItemTemplate>
                                <FooterTemplate>
                                    <asp:TextBox ID="txtLookUpTableCode" runat="server" MaxLength="50" Width="50px"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvtxtLookUpTableCode" ValidationGroup="Insert" runat="server"
                                        ControlToValidate="txtLookUpTableCode" ErrorMessage="Please Enter Lookup Type Code"
                                        ToolTip="Please Enter Lookup Type Code" SetFocusOnError="true" ForeColor="Red">*</asp:RequiredFieldValidator>
                                </FooterTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblCode" runat="server" Text='<%# Eval("Code")%>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Type Value" HeaderStyle-HorizontalAlign="Left" ControlStyle-Width="50px">
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtLookUpTableValue" runat="server" Text='<%# Bind("Value") %>'
                                        Width="90px"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvLookUpTableValue" ValidationGroup="Update" runat="server"
                                        ControlToValidate="txtLookUpTableValue" ErrorMessage="Please enter a value for this lookup type."
                                        ToolTip="Please enter a value for this lookup type." SetFocusOnError="true" ForeColor="Red">*</asp:RequiredFieldValidator>
                                </EditItemTemplate>
                                <FooterTemplate>
                                    <asp:TextBox ID="txtLookUpTableValue" runat="server" Width="50px"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvLookUpTableValue" ValidationGroup="Insert" runat="server"
                                        ControlToValidate="txtLookUpTableValue" ErrorMessage="Please enter a value for this lookup type."
                                        ToolTip="Please enter a value for this lookup type." SetFocusOnError="true" ForeColor="Red">*</asp:RequiredFieldValidator>
                                </FooterTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblValue" runat="server" Text='<%# Eval("Value") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Type Name" HeaderStyle-HorizontalAlign="Left" ControlStyle-Width="50px">
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtLookUpTableName" runat="server" Text='<%# Bind("Text") %>' MaxLength="50"
                                        Width="50px"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvLookUpTableName" ValidationGroup="Update" runat="server"
                                        ControlToValidate="txtLookUpTableName" ErrorMessage="Please Enter Name" ToolTip="Please Enter Lookup Type Name"
                                        SetFocusOnError="true" ForeColor="Red">*</asp:RequiredFieldValidator>
                                </EditItemTemplate>
                                <FooterTemplate>
                                    <asp:TextBox ID="txtLookUpTableName" runat="server" Width="50px"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvLookUpTableName" ValidationGroup="Insert" runat="server"
                                        ControlToValidate="txtLookUpTableName" ErrorMessage="Please Enter Lookup Type Name"
                                        ToolTip="Please Enter Lookup Type Name" SetFocusOnError="true" ForeColor="Red">*</asp:RequiredFieldValidator>
                                </FooterTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblText" runat="server" Text='<%# Eval("Text") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Type Description" HeaderStyle-HorizontalAlign="Left"
                                ControlStyle-Width="200px">
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtLookUpTableDescription" runat="server" Text='<%# Bind("Description") %>'
                                        MaxLength="200" Width="200px"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvLookUpTableDescription" ValidationGroup="Update"
                                        runat="server" ControlToValidate="txtLookUpTableDescription" ErrorMessage="Please enter a description for this lookup type."
                                        ToolTip="Please enter a description for this lookup type." SetFocusOnError="true"
                                        ForeColor="Red">*</asp:RequiredFieldValidator>
                                </EditItemTemplate>
                                <FooterTemplate>
                                    <asp:TextBox ID="txtLookUpTableDescription" runat="server" Width="200px"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvLookUpTableDescription" ValidationGroup="Insert"
                                        runat="server" ControlToValidate="txtLookUpTableDescription" ErrorMessage="Please enter a description for this lookup type."
                                        ToolTip="Please enter a description for this lookup type." SetFocusOnError="true"
                                        ForeColor="Red">*</asp:RequiredFieldValidator>
                                </FooterTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblDescription" runat="server" Text='<%# Eval("Description") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Is Active" HeaderStyle-HorizontalAlign="Left" ControlStyle-Width="25px">
                                <EditItemTemplate>
                                    <asp:RadioButtonList ID="rblIsActive" runat="server" RepeatDirection="Horizontal">
                                        <asp:ListItem Selected="True" Text="True" Value="True"></asp:ListItem>
                                        <asp:ListItem Text="False" Value="False"></asp:ListItem>
                                    </asp:RadioButtonList>
                                </EditItemTemplate>
                                <FooterTemplate>
                                    <asp:RadioButtonList ID="rblIsActive" runat="server" RepeatDirection="Horizontal">
                                        <asp:ListItem Selected="True" Text="True" Value="True"></asp:ListItem>
                                        <asp:ListItem Text="False" Value="False"></asp:ListItem>
                                    </asp:RadioButtonList>
                                </FooterTemplate>
                                <ItemTemplate>
                                    <%# Eval("IsActive") %>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Edit" ShowHeader="False" HeaderStyle-HorizontalAlign="Left">
                                <EditItemTemplate>
                                    <asp:LinkButton ID="lnkUpdate" runat="server" CausesValidation="True" CommandName="Update"
                                        Text="Update" OnClientClick="return confirm('Update?')" ValidationGroup="Update"></asp:LinkButton>
                                    <asp:ValidationSummary ID="vsUpdate" runat="server" ShowMessageBox="true" ShowSummary="false"
                                        ValidationGroup="Update" Enabled="true" HeaderText="Validation Summary..." />
                                    <asp:LinkButton ID="lnkCancel" runat="server" CausesValidation="False" CommandName="Cancel"
                                        Text="Cancel"></asp:LinkButton>
                                </EditItemTemplate>
                                <FooterTemplate>
                                    <asp:LinkButton ID="lnkAdd" runat="server" CausesValidation="True" CommandName="Insert"
                                        ValidationGroup="Insert" Text="Insert"></asp:LinkButton>
                                    <asp:ValidationSummary ID="vsInsert" runat="server" ShowMessageBox="true" ShowSummary="false"
                                        ValidationGroup="Insert" Enabled="true" HeaderText="Validation..." />
                                </FooterTemplate>
                                <ItemTemplate>
                                    <asp:LinkButton ID="lnkEdit" runat="server" CausesValidation="False" CommandName="Edit"
                                        Text="Edit"></asp:LinkButton>
                                </ItemTemplate>
                                <HeaderStyle HorizontalAlign="Left" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Delete" ShowHeader="False" HeaderStyle-HorizontalAlign="Left">
                                <ItemTemplate>
                                    <asp:LinkButton ID="lnkDelete" runat="server" CausesValidation="False" CommandName="Delete"
                                        Text="Delete" OnClientClick="return confirm('Delete?')"></asp:LinkButton>
                                </ItemTemplate>
                                <HeaderStyle HorizontalAlign="Left" />
                            </asp:TemplateField>
                        </Columns>
                        <EmptyDataTemplate>
                            <table class="grid" cellspacing="0" rules="all" border="1" id="gvEG" style="border-collapse: collapse;">
                                <tr>
                                    <th align="left" scope="col">Type Code
                                    </th>
                                    <th align="left" scope="col">Type Name
                                    </th>
                                    <th align="left" scope="col">Type Value
                                    </th>
                                    <th align="left" scope="col">Type Description
                                    </th>
                                    <th align="left" scope="col">Edit
                                    </th>
                                    <th scope="col">Delete
                                    </th>
                                </tr>
                                <tr class="gridRow">
                                    <td colspan="6">No Records found...
                                    </td>
                                </tr>
                                <tr class="gridFooterRow">
                                    <td>
                                        <asp:TextBox ID="txtLookUpTableCode" runat="server" MaxLength="6"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtLookUpTableName" runat="server"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtLookUpTableValue" runat="server"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtLookUpTableDescription" runat="server"></asp:TextBox>
                                    </td>
                                    <td colspan="2" style="text-align: justify; vertical-align: middle">
                                        <asp:LinkButton ID="lnkAdd" runat="server" CausesValidation="false" CommandName="emptyInsert"
                                            Text="emptyInsert"></asp:LinkButton>
                                    </td>
                                </tr>
                            </table>
                        </EmptyDataTemplate>
                    </asp:GridView>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
        <br />
        <br />
        <br />
        <asp:UpdatePanel ID="pnlGenBarcodes" runat="server" Visible="false">
            <ContentTemplate>
                <asp:Button ID="btnGenBarCodes" runat="server" CssClass="srtsButton" Text="Generate Barcodes" OnClick="btnGenBarcodes_Click" />
                <asp:TextBox ID="tbCountAffectedRows" runat="server" BorderStyle="None" BorderWidth="0px"></asp:TextBox>
            </ContentTemplate>
        </asp:UpdatePanel>

        <hr style="width: 75%; text-align: center;" />

        <div style="margin-top: 15px;">
            <asp:Panel ID="pnlPwdReset" runat="server" DefaultButton="btnResetPw">
                <asp:UpdatePanel ID="upPwdReset" runat="server" ChildrenAsTriggers="true" UpdateMode="Conditional">
                    <ContentTemplate>
                        <div style="float: left; margin-left: 10px; text-align: left;" class="colorRed">
                            <asp:Label ID="lblPwdError" runat="server"></asp:Label>
                        </div>

                        <div style="clear: both; float: left; margin-top: 5px;">
                            <div style="width: 150px; text-align: right; float: left; margin-right: 10px;">
                                <asp:Label ID="lblUserName" runat="server" Text='Username:' />
                            </div>
                            <div style="float: left;">
                                <asp:TextBox ID="tbUserName" runat="server" Width="225px"></asp:TextBox>
                            </div>
                        </div>

                        <div style="clear: both; float: left; margin-top: 5px;">
                            <div style="width: 150px; text-align: right; float: left; margin-right: 10px;">
                                <asp:Label ID="lblPassword" runat="server" Text='Enter New Password:' />
                            </div>
                            <div style="float: left;">
                                <asp:TextBox ID="tbPassword" runat="server" TextMode="Password" Width="225px"></asp:TextBox>
                            </div>
                        </div>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="btnResetPw" EventName="Click" />
                    </Triggers>
                </asp:UpdatePanel>
                <div style="clear: both; float: left; margin top 8px;">
                    <asp:Button ID="btnResetPw" runat="server" CssClass="srtsButton" Text="Submit" OnClick="btnResetPw_Click" />
                </div>
            </asp:Panel>
        </div>
    </div>
</asp:Content>
