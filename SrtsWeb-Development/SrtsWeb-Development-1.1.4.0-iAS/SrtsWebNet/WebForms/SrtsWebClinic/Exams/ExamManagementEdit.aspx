<%@ Page Title="" Language="C#" MasterPageFile="~/SrtsMaster.master" AutoEventWireup="true"
    CodeBehind="ExamManagementEdit.aspx.cs" Inherits="SrtsWebClinic.Exams.ExamManagementEdit" %>

<%@ MasterType VirtualPath="~/srtsMaster.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script src="<%=ResolveUrl("~/JavaScript/jsValidators.js") %>"></script>
    <script type="text/javascript">
        function getlblRemainingID() {
            var lblID = '<%=lblRemaining.ClientID%>';
            return lblID;
        }
        function gettbCommentID() {
            var tbID = '<%=tbComments.ClientID%>';
            return tbID;
        }
    </script>
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="MainContent" runat="server">
    <asp:ValidationSummary ID="vsErrors" runat="server" DisplayMode="BulletList" ForeColor="Red"
        CssClass="validatorSummary" />
    <asp:CustomValidator ID="cvSaveExam" runat="server" ErrorMessage="There was an error saving the exam.  Please try again later." Display="None"></asp:CustomValidator>
    <div class="padding">
        <label class="srtsLabel_medium">Exam Date (mm/dd/yyyy):</label><br />
        <asp:TextBox ID="tbExamDate" runat="server" TabIndex="1" CssClass="srtsDateTextBox_medium"></asp:TextBox>&nbsp
            <asp:Image runat="server" ID="calImage1" AlternateText="Calendar" ImageUrl="~/Styles/images/Calendar_scheduleHS.png" />
        <ajaxToolkit:CalendarExtender ID="ceExamDate" runat="server" TargetControlID="tbExamDate"
            Format="MM/dd/yyyy" PopupButtonID="calImage1">
        </ajaxToolkit:CalendarExtender>
        <br />
        <br />
        <table style="width: 300px">
            <colgroup>
                <col class="srtsLabel_medium" style="width: 100px; text-align: right" />
                <col style="width: 100px" />
                <col style="width: 100px" />
            </colgroup>
            <thead>
                <tr class="srtsLabel_medium">
                    <td>
                        <label>&nbsp;</label>
                    </td>
                    <td>
                        <label>Uncorrected</label>
                    </td>
                    <td>
                        <label>Corrected</label>
                    </td>
                </tr>
            </thead>
            <tbody>
                <tr>
                    <td>
                        <label>Right(OD</label>&nbsp
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlODUncorrected" runat="server" DataTextField="Value" DataValueField="Key" TabIndex="2">
                        </asp:DropDownList>
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlODCorrected" runat="server" DataTextField="Value" DataValueField="Key" TabIndex="3">
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td>
                        <label>Left(OS)</label>&nbsp
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlOSUnCorrected" runat="server" DataTextField="Value" DataValueField="Key" TabIndex="4">
                        </asp:DropDownList>
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlOSCorrected" runat="server" DataTextField="Value" DataValueField="Key" TabIndex="5">
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td>
                        <label>Both</label>&nbsp
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlODOSUnCorrected" runat="server" DataTextField="Value" DataValueField="Key" TabIndex="6">
                        </asp:DropDownList>
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlODOSCorrected" runat="server" DataTextField="Value" DataValueField="Key" TabIndex="7">
                        </asp:DropDownList>
                    </td>
                </tr>
            </tbody>
        </table>
        <br />
        <div style="height: 85px">
            <div style="width: 100px; text-align: right; float: left">
                <label class="srtsLabel_medium">Comments:</label>&nbsp
            </div>
            <asp:TextBox ID="tbComments" runat="server" TextMode="MultiLine" Width="500px" Height="80px" ToolTip="Enter any comments, valid characters include + - . , ! ? # ' / ( )" onKeyDown="return textboxMaxCommentSize(this, 256, event, getlblRemainingID(), gettbCommentID())" TabIndex="8" CssClass="srtsTextBox_medium_multi"></asp:TextBox>
            <asp:CustomValidator ID="cvComment" runat="server" ErrorMessage="Invalid character(s) in Comment" ClientValidationFunction="textboxCommentValidation" OnServerValidate="ValidateCommentFormat" ControlToValidate="tbComments" ValidateEmptyText="True" CssClass="asterisk_500TB" Text="*"></asp:CustomValidator><br />
        </div>
        <div style="clear: left; padding-left: 100px; width: 500px; height: 20px; text-align: center;">
            <asp:Label ID="lblRemaining" runat="server" CssClass="srtsLabel_medium"></asp:Label>
        </div>
    </div>
    <div class="padding">
        <div style="width: 100px; text-align: right; float: left">
            <label class="srtsLabel_medium">Provider:</label>&nbsp
        </div>
        <asp:DropDownList ID="ddlDoctors" runat="server" DataTextField="NameLFMi" DataValueField="ID" TabIndex="9" Width="150px">
            <asp:ListItem Text="-Select-" Value="X" Selected="True" />
        </asp:DropDownList>
        <asp:RequiredFieldValidator ID="rfvDoc" runat="server" ControlToValidate="ddlDoctors"
            Display="None" ErrorMessage="Provider is required." Text="*" InitialValue="X"></asp:RequiredFieldValidator><br />
        <br />
        <asp:DropDownList ID="ddlTechnician" runat="server" DataTextField="NameLFMi" DataValueField="ID"
            TabIndex="10" Width="150px" Visible="False">
            <asp:ListItem Text="-Select-" Value="X" Selected="True" />
        </asp:DropDownList>
        <div style="text-align: center">
            <asp:Button ID="btnAdd" runat="server" CssClass="srtsButton" Text="Save Data" TabIndex="11" ToolTip="Accept the exam data entered"
                OnClick="btnAdd_Click" />
            <asp:Button ID="btnCancel" runat="server" CssClass="srtsButton" Text="Cancel" CausesValidation="False"
                TabIndex="12" ToolTip="Close this window.  Any unsaved changes will NOT be saved."
                OnClick="btnCancel_Click" />
        </div>
    </div>
</asp:Content>