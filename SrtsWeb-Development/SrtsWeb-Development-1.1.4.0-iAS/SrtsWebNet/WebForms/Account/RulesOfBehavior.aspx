<%@ Page Title="" Language="C#" MasterPageFile="~/srtsMaster.Master" AutoEventWireup="true" CodeBehind="RulesOfBehavior.aspx.cs" Inherits="SrtsWeb.Account.RulesOfBehavior" %>

<%@ MasterType VirtualPath="~/srtsMaster.Master" %>
<asp:Content ID="Content5" ContentPlaceHolderID="MainContent_Public" runat="server">

    <div class="box_full_top" style="margin-top:30px;padding-top:5px;text-align:center;margin-bottom:-10px">
        <h1 style="font-size: 1.4em;margin-bottom:0px">Rules of Behavior / Acceptable Use Policy</h1>
    </div>
    <div class="box_full_content" style="min-height: 300px; height: auto;">
        <div class="padding">
            <h2 style="text-align: center">
                <asp:Literal ID="litModuleTitle" runat="server" Text="Please accept the Rules of Behavior / Acceptable Use Policy by clicking 'I Accept' below" />
            </h2>
            <div class="padding general" style="height: auto;">
                <asp:BulletedList ID="blcRulesOfBehavior" runat="server" BulletStyle="Disc"  DataTextField="RuleBehavior"> <%-- ContentCssClass="accordianContent" --%>
                </asp:BulletedList>
            </div>
            <div style="text-align: center; padding-top: 60px;">
                <asp:Button ID="btnAccept" runat="server" Text="I Accept" CssClass="srtsButton" OnClick="btnAccept_Click" TabIndex="1" />
                <asp:Button ID="btnDoNotAccept" runat="server" Text="I DO NOT Accept" CssClass="srtsButton" OnClick="btnDoNotAccept_Click" TabIndex="2" />
                <%--<input type="submit" value="I DO NOT Accept" class="srtsButton" />--%> <%-- onclick="$('[id$=divSecurityMessage]').dialog('close');"--%>
            </div>
        </div>
    </div>
    <div class="box_full_bottom"></div>
</asp:Content>