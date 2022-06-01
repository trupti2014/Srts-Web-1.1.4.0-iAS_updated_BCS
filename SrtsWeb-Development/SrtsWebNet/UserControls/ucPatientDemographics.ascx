<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ucPatientDemographics.ascx.cs" Inherits="SrtsWeb.UserControls.ucPatientDemographics" %>

<div id="divPersonalInfo" runat="server" class="padding" style="margin-top: 0px" clientidmode="Static">
    <div class="patientnameheader">
        <asp:Literal ID="litPatientNameHeader" runat="server"></asp:Literal>
    </div>
    <div style="padding: 5px 0px; margin-top: 5px; width: 900px; border-top: 1px solid #E7CFAD;">
        <span class="colorRed">
            <asp:Literal ID="litMessage" runat="server" Visible="false"></asp:Literal></span>
        <div style="float: right; width: 50%; margin-right: 20px">

            <p class="label" style="text-align: right; margin-right: 300px">
                Status
                    <span class="srtsTextBox_medium">
                        <asp:Literal ID="litStatus" runat="server"></asp:Literal>
                    </span>
            </p>
            <p class="label" style="text-align: right; margin-right: 300px">
                Branch - Grade
                    <span class="srtsTextBox_medium">
                        <asp:Literal ID="litBranch" runat="server"></asp:Literal>
                        -
                        <asp:Literal ID="litRank" runat="server"></asp:Literal>
                    </span>
            </p>
            <asp:Literal ID="litPriority" runat="server" Visible="false"></asp:Literal>
            <p class="label" style="text-align: right; margin-right: 300px">
                Theater Location
                    <span class="srtsTextBox_medium">
                        <asp:Literal ID="litTheater" runat="server"></asp:Literal>
                    </span>
            </p>
        </div>
        <div style="float: left; width: 50%; margin-left: -20px">
            <p class="label" style="text-align: right; margin-right: 300px">
                Name
                        <span class="srtsTextBox_medium">
                            <asp:Literal ID="litName" runat="server"></asp:Literal>
                        </span>
            </p>
            <p class="label" style="text-align: right; margin-right: 300px">
                Birth Date - Gender
                    <span class="srtsTextBox_medium">
                        <asp:Literal ID="litDOB" runat="server"></asp:Literal>
                        -
                        <asp:Literal ID="litGender" runat="server"></asp:Literal>
                    </span>
            </p>
            <p class="label" style="text-align: right; margin-right: 300px">
                ID Number
                    <span class="srtsTextBox_medium">
                        <asp:Literal ID="litID" runat="server"></asp:Literal>
                    </span>
            </p>
            <p class="label" style="text-align: right; margin-right: 300px">
                EAD Stop Date
                    <span class="srtsTextBox_medium">
                        <asp:Literal ID="litEAD" runat="server"></asp:Literal>
                    </span>
            </p>
        </div>
        <br />
        <div style="clear: both; margin: 20px 0px; height: 100px; width: 100%">
            <p class="label" style="text-align: right; width: 130px">Clinic Name</p>
            <div style="margin: -25px 0px 0px 135px">
                <asp:TextBox ID="tbClinicName" runat="server" MaxLength="256" ToolTip="Patient Clinic Assignment"
                    Width="700px" ReadOnly="true" CssClass="srtsTextBox_medium">
                </asp:TextBox>
            </div>
            <br />
            <br />
            <div class="label" style="text-align: right; width: 135px">Comments/Notes </div>
            <div style="margin: -25px 0px 0px 135px">
                <asp:TextBox ID="tbCommentsView" runat="server" MaxLength="256" ToolTip=""
                    Rows="3" TextMode="MultiLine" Width="690px" ReadOnly="true" CssClass="srtsTextBox_medium_multi">
                </asp:TextBox>
            </div>
        </div>
    </div>
</div>
