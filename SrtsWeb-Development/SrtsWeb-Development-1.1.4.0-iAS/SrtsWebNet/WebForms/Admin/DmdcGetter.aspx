<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DmdcGetter.aspx.cs" Inherits="SrtsWeb.Admin.DmdcGetter" MasterPageFile="~/srtsMaster.Master" %>

<%@ MasterType VirtualPath="~/srtsMaster.Master" %>

<asp:Content ID="HeadContent1" runat="server" ContentPlaceHolderID="HeadContent">
    <style type="text/css">
        .block {
            float: left;
            margin: 5px;
            display: inline-block;
            width: 240px;
        }

        .indentDetail {
            margin-left: 8px;
            color: #4d0303;
            font-size: larger;
        }
    </style>
</asp:Content>
<asp:Content ID="MainContent2" runat="server" ContentPlaceHolderID="MainContent">
    <div>
        <asp:UpdatePanel ID="upDmdc" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
            <ContentTemplate>
                <table style="width: 100%;">
                    <tr>
                        <td style="text-align: center;">
                            <asp:Button ID="bGet" runat="server" OnClick="bGet_Click" Text="Get" Width="70px" CssClass="qsButton" />
                            <asp:Button ID="bClear" runat="server" OnClick="bClear_Click" Text="Clear" Width="70px" CssClass="qsButton" />
                        </td>
                        <td></td>
                    </tr>
                    <tr>
                        <td style="vertical-align: top; width: 145px; margin: 5px 5px 5px 0px;">
                            <asp:TextBox ID="tbIds" runat="server" TextMode="MultiLine" Height="300px" Width="100%"></asp:TextBox>
                        </td>
                        <td style="vertical-align: top;">
                            <div style="height: 300px; margin-left: 5px; width: 850px; overflow-x: auto;">
                                <asp:GridView ID="gvDmdcResults" runat="server" GridLines="Both"
                                    AllowPaging="false" AutoGenerateColumns="false" CssClass="mGrid" AllowSorting="true"
                                    AlternatingRowStyle-CssClass="alt" HeaderStyle-HorizontalAlign="Left" RowStyle-HorizontalAlign="Left"
                                    OnRowCommand="gvDmdcResults_RowCommand">
                                    <Columns>
                                        <asp:TemplateField>
                                            <ItemTemplate>
                                                <asp:LinkButton ID="lnbIdNumber" Text="Select" runat="server" CommandName="Select"></asp:LinkButton>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField HeaderText="Last Name" DataField="PnLastName" />
                                        <asp:BoundField HeaderText="First Name" DataField="PnFirstName" />
                                        <asp:BoundField HeaderText="Middle Name" DataField="PnMiddleName" />
                                        <asp:BoundField HeaderText="ID Number 1" DataField="PnId1" />
                                        <asp:BoundField HeaderText="ID Type 1" DataField="PnIdType1" />
                                        <asp:BoundField HeaderText="ID Number 2" DataField="PnId2" />
                                        <asp:BoundField HeaderText="ID Type 2" DataField="PnIdType2" />
                                    </Columns>
                                </asp:GridView>
                            </div>
                        </td>
                    </tr>
                </table>
                <div style="border: 1px solid lightgray; height: 100%;">
                    <asp:Panel ID="pnlPersonDetail" runat="server">
                        <table>
                            <tr>
                                <td>
                                    <div class="block">
                                        First Name:
                            <br />
                                        <asp:Label ID="lblFirstName" runat="server" CssClass="indentDetail" Text="txt"></asp:Label>
                                    </div>
                                </td>
                                <td>
                                    <div class="block">
                                        Match Reason Cd 2:
                            <br />
                                        <asp:Label ID="lblMatchReasonCd2" runat="server" CssClass="indentDetail" Text="txt"></asp:Label>
                                    </div>
                                </td>
                                <td>
                                    <div class="block">
                                        Mailing Zip Ext:
                            <br />
                                        <asp:Label ID="lblZipExt" runat="server" CssClass="indentDetail" Text="txt"></asp:Label>
                                    </div>
                                </td>
                                <td>
                                    <div class="block">
                                        Rank:
                            <br />
                                        <asp:Label ID="lblRank" runat="server" CssClass="indentDetail" Text="txt"></asp:Label>
                                    </div>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <div style="clear: both;" class="block">
                                        Middle Name:
                            <br />
                                        <asp:Label ID="lblMiddleName" runat="server" CssClass="indentDetail" Text="txt"></asp:Label>
                                    </div>
                                </td>
                                <td>
                                    <div class="block">
                                        Email:
                            <br />
                                        <asp:Label ID="lblEmail" runat="server" CssClass="indentDetail" Text="txt"></asp:Label>
                                    </div>
                                </td>
                                <td>
                                    <div class="block">
                                        Mailing Country:
                            <br />
                                        <asp:Label ID="lblCountry" runat="server" CssClass="indentDetail" Text="txt"></asp:Label>
                                    </div>
                                </td>
                                <td>
                                    <div class="block">
                                        Service Cd:
                            <br />
                                        <asp:Label ID="lblServiceCd" runat="server" CssClass="indentDetail" Text="txt"></asp:Label>
                                    </div>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <div style="clear: both;" class="block">
                                        Last Name:
                            <br />
                                        <asp:Label ID="lblLastName" runat="server" CssClass="indentDetail" Text="txt"></asp:Label>
                                    </div>
                                </td>
                                <td>
                                    <div class="block">
                                        Phone Number:
                            <br />
                                        <asp:Label ID="lblPhNumber" runat="server" CssClass="indentDetail" Text="txt"></asp:Label>
                                    </div>
                                </td>
                                <td>
                                    <div class="block">
                                        Category Cd:
                            <br />
                                        <asp:Label ID="lblCatCd" runat="server" CssClass="indentDetail" Text="txt"></asp:Label>
                                    </div>
                                </td>
                                <td>
                                    <div class="block">
                                        Attached Unit ID Cd:
                            <br />
                                        <asp:Label ID="lblUnitId" runat="server" CssClass="indentDetail" Text="txt"></asp:Label>
                                    </div>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <div style="clear: both;" class="block">
                                        ID Number 1:
                            <br />
                                        <asp:Label ID="lblIdNum1" runat="server" CssClass="indentDetail" Text="txt"></asp:Label>
                                    </div>
                                </td>
                                <td>
                                    <div class="block">
                                        Mailing Address 1:
                            <br />
                                        <asp:Label ID="lblAddress1" runat="server" CssClass="indentDetail" Text="txt"></asp:Label>
                                    </div>
                                </td>
                                <td>
                                    <div class="block">
                                        Entitlement Type Cd:
                            <br />
                                        <asp:Label ID="lblEntitlementCd" runat="server" CssClass="indentDetail" Text="txt"></asp:Label>
                                    </div>
                                </td>
                                <td>
                                    <div class="block">
                                        DOB:
                            <br />
                                        <asp:Label ID="lblDob" runat="server" CssClass="indentDetail" Text="txt"></asp:Label>
                                    </div>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <div style="clear: both;" class="block">
                                        ID Type 1:
                            <br />
                                        <asp:Label ID="lblIdType1" runat="server" CssClass="indentDetail" Text="txt"></asp:Label>
                                    </div>
                                </td>
                                <td>
                                    <div class="block">
                                        Mailing Address 2:
                            <br />
                                        <asp:Label ID="lblAddress2" runat="server" CssClass="indentDetail" Text="txt"></asp:Label>
                                    </div>
                                </td>
                                <td>
                                    <div class="block">
                                        Organization Cd:
                            <br />
                                        <asp:Label ID="lblOrgCd" runat="server" CssClass="indentDetail" Text="txt"></asp:Label>
                                    </div>
                                </td>
                                <td>
                                    <div class="block">
                                        Death Calendar Dt:
                            <br />
                                        <asp:Label ID="lblDeathDt" runat="server" CssClass="indentDetail" Text="txt"></asp:Label>
                                    </div>
                                </td>
                            </tr>

                            <tr>
                                <td>
                                    <div style="clear: both;" class="block">
                                        Match Reason Cd 1:
                            <br />
                                        <asp:Label ID="lblMatchReasonCd1" runat="server" CssClass="indentDetail" Text="txt"></asp:Label>
                                    </div>
                                </td>
                                <td>
                                    <div class="block">
                                        Mailing City:
                            <br />
                                        <asp:Label ID="lblCity" runat="server" CssClass="indentDetail" Text="txt"></asp:Label>
                                    </div>
                                </td>
                                <td>
                                    <div class="block">
                                        Projected End Dt:
                            <br />
                                        <asp:Label ID="lblEndDt" runat="server" CssClass="indentDetail" Text="txt"></asp:Label>
                                    </div>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <div style="clear: both;" class="block">
                                        Id Number 2:
                            <br />
                                        <asp:Label ID="lblIdNum2" runat="server" CssClass="indentDetail" Text="txt"></asp:Label>
                                    </div>
                                </td>
                                <td>
                                    <div class="block">
                                        Mailing State:
                            <br />
                                        <asp:Label ID="lblState" runat="server" CssClass="indentDetail" Text="txt"></asp:Label>
                                    </div>
                                </td>
                                <td>
                                    <div class="block">
                                        Pay Plan Cd:
                            <br />
                                        <asp:Label ID="lblPayPlanCd" runat="server" CssClass="indentDetail" Text="txt"></asp:Label>
                                    </div>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <div style="clear: both;" class="block">
                                        Id Type 2:
                            <br />
                                        <asp:Label ID="lblIdType2" runat="server" CssClass="indentDetail" Text="txt"></asp:Label>
                                    </div>
                                </td>
                                <td>
                                    <div class="block">
                                        Mailing Zip:
                            <br />
                                        <asp:Label ID="lblZip" runat="server" CssClass="indentDetail" Text="txt"></asp:Label>
                                    </div>
                                </td>
                                <td>
                                    <div class="block">
                                        Pay Grade:
                            <br />
                                        <asp:Label ID="lblPayGrade" runat="server" CssClass="indentDetail" Text="txt"></asp:Label>
                                    </div>
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                </div>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="gvDmdcResults" EventName="SelectedIndexChanged" />
                <asp:AsyncPostBackTrigger ControlID="bClear" EventName="Click" />
                <asp:AsyncPostBackTrigger ControlID="bGet" EventName="Click" />
            </Triggers>
        </asp:UpdatePanel>
    </div>
</asp:Content>