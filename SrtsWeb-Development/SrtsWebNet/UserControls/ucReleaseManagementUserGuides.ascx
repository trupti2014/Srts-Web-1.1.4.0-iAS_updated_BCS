<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ucReleaseManagementUserGuides.ascx.cs" Inherits="SrtsWeb.UserControls.ucReleaseManagementUserGuides" %>

<asp:UpdatePanel ID="upUserGuides" runat="server" UpdateMode="conditional" ChildrenAsTriggers="true">
    <ContentTemplate>
        <div class="w3-row" style="margin-top: -30px">
            <div class="BeigeBoxContainer" style="margin: 10px 0px 0px 0px">
                <div class="BeigeBoxHeader" style="text-align: left; position: relative; top: 0px; padding: 12px 10px 3px 15px">
                    <span class="label">User Guides</span>
                </div>
                <div class="BeigeBoxContent padding" style="margin-top: 0px; margin-left: 10px; height: auto">
                    <div id="divUserGuides">
                    <div class="w3-row alignLeft">
                        <div class="padding" style="padding: 5px 0px;">
                            <h1>Upload User Guides:</h1>
                            <div class="w3-row">
                                    <asp:FileUpload ID="fuUploadUserGuide" style="width: 40%;" runat="server" TabIndex="1" />
                                    <asp:Button ID="btnUploadUserGuide" runat="server" Text="Upload" CssClass="srtsButton" CausesValidation="true" TabIndex="2"  OnClick="btnUploadUserGuide_Click" OnClientClick="return CheckFileExists()" />
                                    <br />
                                    <asp:RequiredFieldValidator ErrorMessage="No file selected" ControlToValidate="fuUploadUserGuide" runat="server" Display="Dynamic" ForeColor="Red" />
                                    <asp:RegularExpressionValidator ID="revUploadUserGuide" runat="server" ControlToValidate="fuUploadUserGuide" ErrorMessage="Only files with the following extensions are allowed: .docx, .doc" ForeColor="Red" ValidationExpression="([a-zA-Z0-9\s_\\.\-:])+(.doc|.docx)$" ValidationGroup="UploadUserGuideGroup" SetFocusOnError="true"></asp:RegularExpressionValidator>
                                    <br />
                                    <asp:CustomValidator runat="server" Display="Dynamic" ID="customValidator1" ControlToValidate="fuUploadUserGuide" ForeColor="Red" ErrorMessage="" OnServerValidate="customValidator_ServerValidate"></asp:CustomValidator>
                            </div>
                        </div>
                        <div class="w3-row" style="margin-top: 0px">
                            <h1>Uploaded User Guides:</h1>
                            <div class="" style="width: 100%; margin-left: auto; margin-right: auto; height: 200px; max-height: 200px; overflow-y: scroll; overflow-x: hidden;">
                                <asp:GridView ID="gvUploadedUserGuides" runat="server" AutoGenerateColumns="False"
                                    OnRowDataBound="gvUploadedUserGuides_RowDataBound"
                                    OnRowCommand="gvUploadedUserGuides_RowCommand"
                                    OnRowDeleting="gvUploadedUserGuides_RowDeleting"
                                    EmptyDataText="No User Guides Exist"
                                    GridLines="Both"
                                    CssClass="mGrid width75"
                                    AlternatingRowStyle-CssClass="alt"
                                    HeaderStyle-HorizontalAlign="Left"
                                    RowStyle-HorizontalAlign="Left"
                                    ShowHeaderWhenEmpty="true"
                                    HorizontalAlign="Center">
                                    <Columns>
                                        <asp:ButtonField Text="Delete" ButtonType="Link" CommandName="delete" />
                                        <asp:TemplateField HeaderText="Name">
                                            <ItemStyle HorizontalAlign="Center" />
                                            <ItemTemplate>
                                                <asp:LinkButton ID="lnkUserGuideName" runat="server" Text='<%# Eval("GuideName")%>' CommandName="downloadfile" ClientIDMode="AutoID" CommandArgument='<%# Eval("GuideName")%>' CausesValidation="false"></asp:LinkButton>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Last Modified Date">
                                            <ItemStyle HorizontalAlign="Center" />
                                            <ItemTemplate>
                                                <asp:Label ID="lblLastModifiedDate" runat="server" Text='<%# Convert.ToDateTime( Eval("DateLastModified")).ToShortDateString() %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Modified By">
                                            <ItemStyle HorizontalAlign="Center" />
                                            <ItemTemplate>
                                                <asp:Label ID="lblModifiedBy" runat="server" Text='<%# Eval("ModifiedBy")%>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                    <HeaderStyle BackColor="#004994" ForeColor="White" Height="20px" />
                                    <RowStyle BorderColor="#CCCCCC" BorderStyle="Solid" BorderWidth="1px" Height="18px" />
                                </asp:GridView>
                            </div>
                        </div>
                       </div>
                </div>
                <div class="BeigeBoxFooter"></div>
            </div>
        </div>
        <div style="position: relative; top: 125px; left: 385px">
        </div>

    <asp:ScriptManagerProxy ID="smpReleaseManagementUserGuide" runat="server">
            <Scripts>
                <asp:ScriptReference Path="~/Scripts/ReleaseManagement/UserGuidesManagement.js" />
            </Scripts>
    </asp:ScriptManagerProxy>
    </ContentTemplate>
    <Triggers>
        <asp:PostBackTrigger ControlID="btnUploadUserGuide" />
    </Triggers>
       
</asp:UpdatePanel>
