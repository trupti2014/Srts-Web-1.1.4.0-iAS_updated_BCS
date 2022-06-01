<%@ Page Title="" Language="C#" MasterPageFile="~/srtsMaster.Master" AutoEventWireup="true" CodeBehind="FrameManagementAdd.aspx.cs" Inherits="SrtsWeb.Admin.FrameManagementAdd" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:Panel ID="pnlFrames" runat="server" Visible="false">
        <h2>Manage Frames</h2>
        <hr />
        <asp:UpdatePanel ID="upFrameInfo" runat="server" UpdateMode="Always">
            <ContentTemplate>
                <div>
                    <h3>Frame Info</h3>
                    <br />
                    <table>
                        <tr>
                            <td>
                                <asp:Label ID="litFrameCode" runat="server" CssClass="srtsLabel_medium"><strong>Frame Code:</strong></asp:Label>
                            </td>
                            <td>
                                <asp:DropDownList ID="ddlFrames" runat="server" DataTextField="FrameCode"
                                    DataValueField="FrameCode" AutoPostBack="true" Width="300px" OnSelectedIndexChanged="ddlFrames_SelectedIndexChanged">
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label ID="litFrameDescription" runat="server" CssClass="srtsLabel_medium"><strong>Frame Description:</strong></asp:Label>
                            </td>
                            <td>
                                <asp:TextBox ID="tbFrameDescription" runat="server" Width="300px"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label ID="litFrameNotes" runat="server" CssClass="srtsLabel_medium"><strong>Frame Notes:</strong></asp:Label>
                            </td>
                            <td>
                                <asp:TextBox ID="tbFrameNotes" runat="server" Width="300px"></asp:TextBox>
                            </td>
                        </tr>
                    </table>
                </div>
                <div>
                    <hr />
                    <h3>Eligibility</h3>
                    <hr />
                    <br />
                    <asp:Literal ID="litBOS" runat="server"><strong>Branch of Service:</strong></asp:Literal>
                    <br />
                    <asp:CheckBoxList ID="cblBOS" runat="server" TextAlign="Right" DataTextField="Value"
                        DataValueField="Key" RepeatColumns="4" CellSpacing="10">
                    </asp:CheckBoxList>
                    <hr />
                    <asp:Literal ID="litProfile" runat="server"><strong>Order Priority:</strong></asp:Literal>
                    <br />
                    <asp:CheckBoxList ID="cblProfile" runat="server" RepeatColumns="4" DataTextField="Value" DataValueField="Key"
                        CellSpacing="10">
                    </asp:CheckBoxList>
                    <hr />
                    <asp:Literal ID="litRank" runat="server"><strong>Rank:</strong></asp:Literal>
                    <br />
                    <asp:CheckBoxList ID="cblRank" runat="server" RepeatColumns="4" DataTextField="Value"
                        DataValueField="Key" CellSpacing="10">
                    </asp:CheckBoxList>
                    <hr />
                    <asp:Literal ID="litStatus" runat="server"><strong>Job Status:</strong></asp:Literal>
                    <br />
                    <asp:CheckBoxList ID="cblStatus" runat="server" TextAlign="Right" RepeatColumns="4"
                        CellSpacing="10" DataTextField="Value" DataValueField="Key">
                    </asp:CheckBoxList>
                    <hr />
                    <asp:Literal ID="litGender" runat="server"><strong>Gender:</strong></asp:Literal>
                    <br />
                    <asp:CheckBoxList ID="cblGender" runat="server">
                        <asp:ListItem Text="Male" Value="M"></asp:ListItem>
                        <asp:ListItem Text="Female" Value="F"></asp:ListItem>
                        <asp:ListItem Text="Both" Value="B"></asp:ListItem>
                    </asp:CheckBoxList>
                    <hr />
                </div>
                <asp:ListBox ID="results" runat="server" Width="300px"></asp:ListBox>
                <br />
                <asp:Button ID="btnSubmit" runat="server" CssClass="srtsButton" Text="Submit" OnClick="btnSubmit_Click" />
            </ContentTemplate>
        </asp:UpdatePanel>
    </asp:Panel>
    <asp:Panel ID="pnlFrameItems" runat="server">
    </asp:Panel>
</asp:Content>