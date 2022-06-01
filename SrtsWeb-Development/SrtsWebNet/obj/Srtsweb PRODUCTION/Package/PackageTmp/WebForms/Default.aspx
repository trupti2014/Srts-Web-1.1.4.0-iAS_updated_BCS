<%@ Page Language="C#" MasterPageFile="~/srtsMaster.Master" AutoEventWireup="True" CodeBehind="Default.aspx.cs" Inherits="_Default" %>

<%@ MasterType VirtualPath="~/srtsMaster.Master" %>
<%@ Register TagPrefix="srts" TagName="UserProfile" Src="~/UserControls/ucUserProfile.ascx" %>
<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <style>
        .numbers {
            text-align: right!important;
            width: 40px!important;
            padding-right: 5px;
        }
        .announcement {
        padding: 10px 0px;
        }
    </style>

</asp:Content>
<asp:Content ID="contentClinicName" ContentPlaceHolderID="ContentTop_Title_Right" runat="server">
    <div style="margin: -25px -40px 0px 0px; float: right; text-align: left;">
        <asp:Literal ID="litContentTop_Title_Right" runat="server"></asp:Literal>
    </div>
</asp:Content>

<asp:Content ID="MainContent" runat="server" ContentPlaceHolderID="MainContent">
    <asp:ScriptManagerProxy ID="smpDefault" runat="server">
        <Scripts>
            <asp:ScriptReference Path="~/Scripts/Default/Default.js" />
        </Scripts>
    </asp:ScriptManagerProxy>

    <div id="divDefaultContainer" class="">
        <div class="" style="min-width: 1096px; max-width: 1096px; padding-top: 10px">
            <div class="w3-row">
            </div>

            <div class="w3-col" style="width: 600px">
                <!-- Facility Information -->
                <div id="divMyFacilityInfo" runat="server" style="margin-top: 5px">
                    <div class="BeigeBoxContainer" style="margin: 10px 10px 20px 20px">
                        <div class="BeigeBoxHeader" style="text-align: left; padding: 12px 10px 3px 15px">
                            <span class="label">Facility Information</span>
                        </div>
                        <div class="BeigeBoxContent" style="margin: 5px 10px 5px 0px; min-height: 120px;">
                            <!--Clinic Summary -->
                            <div id="divClinicSummary" runat="server" visible="false">
                                <br />
                                <h1 style="text-align: left; padding-left: 30px; color: #782E1E">
                                    <asp:Literal ID="litSiteName" runat="server" />&nbsp;Order Summaries:</h1>
                                <div id="divMyClinic">
                                    <div style="float: right; width: 35%; margin: 0px 10px 0px 0px; text-align: center;">
                                        <div style="margin: 0px 20px 0px 50px;">
                                            The average time from order creation date to dispense date is:<br />
                                            <span class="count">
                                                <asp:Literal ID="litAvgDispenseTime" runat="server"></asp:Literal>
                                                days</span>
                                        </div>
                                    </div>

                                    <div id="divSummaryWithLinks" runat="server" visible="false" style="width: 65%; text-align: left; margin: 5px 0px 5px 5px; padding-right: 0px">
                                        <ul>
                                            <li class="shopping_cart">
                                                <span class="w3-col count numbers">
                                                    <asp:Literal ID="litPending" runat="server"></asp:Literal></span> Orders
                                                <span class="w3-rest">
                                                    <asp:LinkButton ID="LinkButton5" runat="server" PostBackUrl="~/WebForms/SrtsWebClinic/Orders/ManageOrders.aspx#pending"
                                                        Text="Pending" ToolTip="" />
                                                </span>
                                            </li>

                                            <li class="shopping_cart">
                                                <span class="w3-col count numbers">
                                                    <asp:Literal ID="litRetrieval" runat="server"></asp:Literal></span> Orders ready for
                                                <span class="w3-rest">
                                                    <asp:LinkButton ID="LinkButton1" runat="server" PostBackUrl="~/WebForms/SrtsWebClinic/Orders/ManageOrders.aspx#checkin"
                                                        Text="Check-in" ToolTip="" />
                                                </span>
                                            </li>

                                            <li class="shopping_cart">
                                                <span class="w3-col count numbers">
                                                    <asp:Literal ID="litDispense" runat="server"></asp:Literal></span> Orders ready for
                                                <span class="w3-rest">
                                                    <asp:LinkButton ID="LinkButton3" runat="server" PostBackUrl="~/WebForms/SrtsWebClinic/Orders/ManageOrders.aspx#dispense"
                                                        Text="Dispense" ToolTip="" />
                                                </span>
                                            </li>

                                            <li class="shopping_cart">
                                                <span class="w3-col count numbers">
                                                    <asp:Literal ID="litRejected" runat="server"></asp:Literal></span>

                                                <span class="w3-rest">
                                                    <asp:LinkButton ID="LinkButton2" runat="server" PostBackUrl="~/WebForms/SrtsWebClinic/Orders/ManageOrders.aspx#problem"
                                                        Text="Problem" ToolTip="" />&nbsp;orders needing your attention
                                                </span>
                                            </li>

                                            <li class="shopping_cart">
                                                <span class="w3-col count numbers">
                                                    <asp:Literal ID="litOverdue" runat="server"></asp:Literal></span>
                                                <span class="w3-rest">
                                                    <asp:LinkButton ID="LinkButton4" runat="server" PostBackUrl="~/WebForms/SrtsWebClinic/Orders/ManageOrders.aspx#overdue"
                                                        Text="Outstanding" ToolTip="" />
                                                    orders</span><span style="font-size: 12px; color: #782E1E"> (more than 10 days old)</span></li>
                                        </ul>
                                    </div>
                                    <div id="divSummaryWithNoLinks" runat="server" visible="false" style="width: 60%; text-align: left; margin: 5px 15px">
                                        <ul>
                                            <li class="shopping_cart">
                                                <span class="w3-col count numbers">
                                                    <asp:Literal ID="litPending1" runat="server"></asp:Literal></span> Orders Pending
                                            </li>
                                            <li class="shopping_cart">
                                                <span class="w3-col count numbers">
                                                    <asp:Literal ID="litRetrieval1" runat="server"></asp:Literal></span> Orders ready for Check-in
                                            </li>
                                            <li class="shopping_cart">
                                                <span class="w3-col count numbers">
                                                    <asp:Literal ID="litDispense1" runat="server"></asp:Literal></span> Orders ready for Dispense
                                            </li>
                                            <li class="shopping_cart">
                                                <span class="w3-col count numbers">
                                                    <asp:Literal ID="litRejected1" runat="server"></asp:Literal></span> Problem orders needing your attention
                                            </li>
                                            <li class="shopping_cart">
                                                <span class="w3-col count numbers">
                                                    <asp:Literal ID="litOverdue1" runat="server"></asp:Literal></span> Outstanding orders<span style="font-size: 12px; color: #782E1E"> (more than 10 days old)</span></li>
                                        </ul>
                                    </div>
                                </div>
                            </div>
                            <!--Lab Summary -->
                            <div id="divLabSummary" runat="server" visible="false">
                                <br />
                                <h1 style="text-align: left; padding-left: 30px; color: #782E1E">
                                    <asp:Literal ID="litLabName" runat="server" />&nbsp;Lab summaries are as follows:</h1>
                                <div id="divMyLab">
                                    <!--Lab Average Time -->
                                    <div style="float: right; width: 35%; margin-right: 20px; text-align: center;">
                                        <div style="margin: -30px 20px 10px 10px;">
                                            <span class="count">
                                                <asp:Literal ID="litLabProdTime" runat="server"></asp:Literal>
                                                days</span> is the average production time from order creation date to production completion date on a rolling 14 day average.
                                        </div>
                                    </div>
                                    <%--<div>
                                        <br />
                                        <ul>
                                            <li class="shopping_cart"><span class="count">
                                                <asp:Literal ID="litLabRetrieval" runat="server"></asp:Literal></span> orders ready for
                                                                <asp:LinkButton ID="lnkLabCheckin" runat="server" PostBackUrl="~/WebForms/SrtsWebLab/ManageOrdersLab.aspx#checkin"
                                                                    Text="check-in" ToolTip="" />
                                            </li>
                                            <li class="shopping_cart"><span class="count">
                                                <asp:Literal ID="litLabDispense" runat="server"></asp:Literal></span> orders ready for
                                                                <asp:LinkButton ID="LinkButton6" runat="server" PostBackUrl="~/WebForms/SrtsWebLab/ManageOrdersLab.aspx#dispense"
                                                                    Text="check-out" ToolTip="" />
                                            </li>
                                        </ul>
                                    </div>--%>
                                    <div id="divLabSummaryWithLinks" runat="server" visible="false" style="width: 65%; text-align: left; margin: 5px 10px">
                                        <ul>
                                            <li class="shopping_cart">
                                                <span class="w3-col count numbers">
                                                    <asp:Literal ID="litLabRetrieval" runat="server"></asp:Literal></span> orders ready for
                                                <span class="w3-rest">
                                                    <asp:LinkButton ID="lnkLabCheckin" runat="server" PostBackUrl="~/WebForms/SrtsWebLab/ManageOrdersLab.aspx#checkin"
                                                        Text="check-in" ToolTip="" /></span></li>

                                            <li class="shopping_cart">
                                                <span class="w3-col count numbers">
                                                    <asp:Literal ID="litLabDispense" runat="server"></asp:Literal></span> orders ready for
                                                <span class="w3-rest">
                                                    <asp:LinkButton ID="LinkButton6" runat="server" PostBackUrl="~/WebForms/SrtsWebLab/ManageOrdersLab.aspx#dispense"
                                                        Text="check-out" ToolTip="" /></span></li>

                                            <li class="shopping_cart">
                                                <span class="w3-col count numbers">
                                                    <asp:Literal ID="litLabHoldForStock" runat="server"></asp:Literal></span> orders on 
                                                <span class="w3-rest">
                                                    <asp:LinkButton ID="LinkButton7" runat="server" PostBackUrl="~/WebForms/SrtsWebLab/ManageOrdersLab.aspx#holdforstock?id=holdforstock"
                                                        Text="hold for stock" ToolTip="" /></span></li>
                                        </ul>
                                    </div>

                                    <div id="divLabSummaryWithNoLinks" runat="server" visible="false">
                                        <br />
                                        <ul>
                                            <li class="shopping_cart"><span class="count numbers">
                                                <asp:Literal ID="litLabRetrieval1" runat="server"></asp:Literal></span> orders ready for check-in</li>
                                            <li class="shopping_cart"><span class="count numbers">
                                                <asp:Literal ID="litLabDispense1" runat="server"></asp:Literal></span> orders ready for check-out</li>
                                            <li class="shopping_cart"><span class="count numbers">
                                                <asp:Literal ID="litLabHoldForStock1" runat="server"></asp:Literal></span> orders on hold for stock</li>
                                        </ul>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <!-- Announcements -->
                <div id="divAnnouncements" runat="server" style="margin-top: 10px">
                    <div class="BeigeBoxContainer" style="margin: 0px 10px 0px 20px">
                        <div class="BeigeBoxHeader" style="text-align: left; padding: 12px 10px 3px 15px">
                            <span class="label">Announcements</span>
                        </div>
                        <div class="BeigeBoxContent" style="margin-left: 10px; min-height: 100px;">
                            <div id="divSrtsAnnouncements" runat="server" style="text-align:left;padding:5px 0px 0px 20px;font-size:12px">
                                <h2 style="font-size: 14px; text-align: left; color: #004994; margin: 10px 0px 0px 15px;">No important updates at this time!
                                </h2>
                            </div>
                        </div>
                    </div>
                </div>


                <div class="w3-row">
                    <!-- User Guides -->
                    <div class="w3-col" style="width: 100%">
                        <div id="divUserManuals" runat="server" style="margin-top: 10px">
                            <div class="BeigeBoxContainer" style="margin: 0px 10px 0px 20px">
                                <div class="BeigeBoxHeader" style="text-align: left; padding: 12px 10px 3px 15px">
                                    <span class="label">User Guides</span>
                                </div>
                                <div class="BeigeBoxContent" style="margin-top: 15px; min-height: 75px;">
                                    <div id="divUserManualsLinks" runat="server" style="text-align: left">
                                        <p>
                                            <asp:LinkButton ID="lnkCTGuide" runat="server" Text="Download Clinic Tech Guide"
                                                OnCommand="DownloadGuide" CommandArgument="CT" Visible="False"></asp:LinkButton>
                                        </p>
                                        <p>
                                            <asp:LinkButton ID="lnkCAGuide" runat="server" Text="Download Clinic Admin Guide"
                                                OnCommand="DownloadGuide" CommandArgument="CA" Visible="False"></asp:LinkButton>
                                        </p>
                                        <p>
                                            <asp:LinkButton ID="lnkLTGuide" runat="server" Text="Download Lab Tech Guide"
                                                OnCommand="DownloadGuide" CommandArgument="LT" Visible="False"></asp:LinkButton>
                                        </p>
                                        <p>
                                            <asp:LinkButton ID="lnkLAGuide" runat="server" Text="Download Lab Admin Guide"
                                                OnCommand="DownloadGuide" CommandArgument="LA" Visible="False"></asp:LinkButton>
                                        </p>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <!-- Did You Know? -->
                    <div class="w3-rest" style="display: none">
                        <div class="BeigeBoxContainer" style="margin: 10px 10px 0px 20px">
                            <div class="BeigeBoxHeader" style="text-align: left; padding: 12px 10px 3px 15px">
                                <span class="label">Did You Know...?</span>
                            </div>
                            <div class="BeigeBoxContent" style="margin-top: 15px; min-height: 75px;">
                                <div class="w3-content">
                                    <div class="mySlides w3-container">
                                        <p>These are very helpful</p>
                                        <p></p>
                                    </div>
                                    <div class="mySlides w3-container">
                                        <p>This is another did you know</p>
                                        <p></p>
                                    </div>
                                    <div class="mySlides w3-container">
                                        <p>did you know option 7</p>
                                        <p></p>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

            </div>
            <div class="w3-rest">
                <!-- User Information -->
                <div id="divUserData" style="margin-top: 5px; padding-bottom: 20px">
                    <div class="BeigeBoxContainer" style="margin: 10px 20px 10px 20px">
                        <div class="BeigeBoxHeader" style="text-align: left; padding: 12px 10px 3px 15px">
                            <span class="label">User Information</span>
                        </div>
                        <div class="BeigeBoxContent" style="margin: 0px 10px 10px 0px; min-height: 350px;">
                            <div style="margin: 20px 0px 0px 10px;">
                                <srts:UserProfile ID="srtsUserProfile" runat="server" />
                            </div>
                        </div>
                    </div>
                </div>





            </div>
        </div>
    </div>

    <!-- Selec a Site Dialog -->
    <div id="divSitesDialog" style="display: none; width: 320px; text-align: center; overflow-y: auto;">
        <asp:GridView ID="gvSites" runat="server" OnRowCommand="gvSites_RowCommand" OnRowDataBound="gvSites_RowDataBound" AutoGenerateColumns="false" CssClass="gvSitesDialog">
            <Columns>
                <asp:TemplateField HeaderText="Select site code to login to." HeaderStyle-CssClass="srtsLabel_medium">
                    <ItemTemplate>
                        <asp:Button ID="bSiteCode" CssClass="srtsButton" runat="server" CommandName="SrtsButton" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>

    <%--    <script>
        // script for Did You Know...?
        var slideIndex = 0;
        carousel();

        function carousel() {
            var i;
            var x = document.getElementsByClassName("mySlides");
            for (i = 0; i < x.length; i++) {
                x[i].style.display = "none";
            }
            slideIndex++;
            if (slideIndex > x.length) { slideIndex = 1 }
            x[slideIndex - 1].style.display = "block";
            setTimeout(carousel, 7000);
        }
    </script>--%>
</asp:Content>
