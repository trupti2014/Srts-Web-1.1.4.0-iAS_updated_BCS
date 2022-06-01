<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ucCms.ascx.cs" Inherits="SrtsWeb.UserControls.ucCms" %>
<hr style="width: 100%; color: #E7CFAD; margin-bottom: 5px; text-align: center;" />

<div style="float: right; width: 75%; border: solid 1px #E7CFAD; padding: 5px 10px; height: 380px; display: none" id="CreateAnnouncement">
    <p class="srtsLabel_medium noindent">Announcement Headline</p>
    <asp:TextBox ID="txtContentTitle" runat="server" CssClass="srtsTextBox_medium" Style="width: 680px"></asp:TextBox>
    <br />
    <br />
    <p class="srtsLabel_medium noindent">Announcement Text</p>
    <asp:TextBox ID="txtContentDescription"
        TextMode="MultiLine"
        Rows="15"
        Width="680px"
        CssClass="srtsTextBox_medium_multi"
        runat="server">
    </asp:TextBox>
    <p style="position: relative; top: 260px; text-align: right">
        <asp:Button ID="btnSave" runat="server" CssClass="srtsButton" CausesValidation="false" Text="Save" OnClick="btnSave_Click" />
        <asp:Button ID="btnCancel" runat="server" CssClass="srtsButton" CausesValidation="false" Text="Cancel" OnClick="btnCancel_Click" />
    </p>
</div>

<div style="float: left; width: 20%; border: solid 1px #E7CFAD; padding: 5px 10px; height: 380px">
    <p class="srtsLabel_medium noindent" style="text-align: left">Start Date</p>
    <asp:TextBox ID="txtDisplayStartDate" runat="server" CssClass="srtsTextBox_small" TextMode="Date"></asp:TextBox>
    <ajaxToolkit:CalendarExtender ID="calStart" runat="server" TargetControlID="txtDisplayStartDate" Format="MM/dd/yyyy"></ajaxToolkit:CalendarExtender>
    <br />
    <br />
    <p class="srtsLabel_medium noindent" style="text-align: left">Expiration Date</p>
    <asp:TextBox ID="txtDisplayEndDate" runat="server" CssClass="srtsTextBox_small" TextMode="Date"></asp:TextBox>
    <ajaxToolkit:CalendarExtender ID="calEnd" runat="server" TargetControlID="txtDisplayEndDate" Format="MM/dd/yyyy"></ajaxToolkit:CalendarExtender>
    <br />
    <br />
    <p class="srtsLabel_medium noindent" style="text-align: left">Content Type</p>
    <asp:DropDownList ID="ddlSelContentType" runat="server" Width="180px" ClientIDMode="Static"></asp:DropDownList>
    <br />
    <br />
    <div id="divSelRecipientType" style="display: none">
        <p class="srtsLabel_medium noindent" style="text-align: left">Recipient Type</p>
        <asp:DropDownList ID="ddlSelRecipientType" runat="server" Width="180px" ClientIDMode="Static"></asp:DropDownList>
    </div>
    <br />
    <div id="divSelGroupType" style="display: none">
        <p class="srtsLabel_medium noindent" style="text-align: left">Recipient Group Type</p>
        <asp:DropDownList ID="ddlSelRecipientGroupType" runat="server" Width="180px" ClientIDMode="Static"></asp:DropDownList>
    </div>
    <br />
    <div id="divSelFacilityName_Clinic" style="display: none">
        <p class="srtsLabel_medium noindent" style="text-align: left">Clinic Name</p>
        <asp:DropDownList ID="ddlSelFacilityName_Clinic" runat="server" Width="180px" ClientIDMode="Static"></asp:DropDownList>
    </div>
    <br />
    <div id="divSelFacilityName_Lab" style="display: none">
        <p class="srtsLabel_medium noindent" style="text-align: left">Lab Name</p>
        <asp:DropDownList ID="ddlSelFacilityName_Lab" runat="server" Width="180px" ClientIDMode="Static"></asp:DropDownList>
    </div>
    <br />
    <span style="float: left; color: red">
        <asp:Literal ID="lblErr" runat="server"></asp:Literal></span>
    <asp:HiddenField ID="hfCId" runat="server" />
</div>
<script type="text/javascript">
    $(function () {
        if ($('#<%=ddlSelContentType.ClientID%> option:selected').val() != 0)
            DoSelContentType($('#<%=ddlSelContentType.ClientID%> option:selected').val());

        if ($('#<%=ddlSelRecipientType.ClientID%> option:selected').val() != 0)
            DoSelRecipientType($('#<%=ddlSelRecipientType.ClientID%> option:selected').val());

        if ($('#<%=ddlSelFacilityName_Clinic.ClientID%> option:selected').val() != 0)
            DoSelClinic($('#<%=ddlSelFacilityName_Clinic.ClientID%> option:selected').val());

        if ($('#<%=ddlSelFacilityName_Lab.ClientID%> option:selected').val() != 0)
            DoSelLab($('#<%=ddlSelFacilityName_Lab.ClientID%> option:selected').val());

        if ($('#<%=ddlSelRecipientGroupType.ClientID%> option:selected').val() != 0)
            DoSelGroupType($('#<%=ddlSelRecipientGroupType.ClientID%> option:selected').val());

    }).on('change', '#<%=ddlSelContentType.ClientID%>', function () {
        DoSelContentType($('#<%=ddlSelContentType.ClientID%> option:selected').val());
    }).on('change', '#<%=ddlSelRecipientType.ClientID%>', function () {
        DoSelRecipientType($('#<%=ddlSelRecipientType.ClientID%> option:selected').val());
    }).on('change', '#<%=ddlSelFacilityName_Clinic.ClientID%>', function () {
        DoSelClinic($('#<%=ddlSelFacilityName_Clinic.ClientID%> option:selected').val());
    }).on('change', '#<%=ddlSelFacilityName_Lab.ClientID%>', function () {
        DoSelLab($('#<%=ddlSelFacilityName_Lab.ClientID%> option:selected').val());
    }).on('change', '#<%=this.ddlSelRecipientGroupType.ClientID%>', function () {
        DoSelGroupType($('#<%=ddlSelRecipientGroupType.ClientID%> option:selected').val());
    });

    function DoSelContentType(val) {
        $('div[id$="divSelRecipientType"]').hide();
        $('div[id$="divSelGroupType"]').hide();
        $('div[id$="divSelFacilityName_Clinic"]').hide();
        $('div[id$="divSelFacilityName_Lab"]').hide();
        $('div[id$="CreateAnnouncement"]').hide();

        switch (val) {
            case 'C000':
            case 'C003':
            case 'C004':
            case 'C005':
                $('div[id$="CreateAnnouncement"]').show();
                break;
            case 'C001':
            case 'C002':
                $('div[id$="divSelRecipientType"]').show();
                break;
            default:
                $('div[id$="CreateAnnouncement"]').hide();
                $('div[id$="divSelRecipientType"]').hide();
                break;
        };
    };
    function DoSelRecipientType(val) {
        $('div[id$="CreateAnnouncement"]').hide();
        $('div[id$="divSelFacilityName_Clinic"]').hide();
        $('div[id$="divSelFacilityName_Lab"]').hide();
        $('div[id$="divSelGroupType"]').hide();

        $('#<%= this.ddlSelFacilityName_Clinic.ClientID %>').val('0');
        $('#<%= this.ddlSelFacilityName_Lab.ClientID %>').val('0');

        switch (val) {
            case 'R001':
                $('div[id$="CreateAnnouncement"]').show();
                break;
            case 'R002':
                $('div[id$="CreateAnnouncement"]').hide();
                $('div[id$="divSelFacilityName_Clinic"]').show();
                break;
            case 'R003':
                $('div[id$="CreateAnnouncement"]').hide();
                $('div[id$="divSelFacilityName_Lab"]').show();
                break;
            case 'R004':
                $('div[id$="CreateAnnouncement"]').hide();
                break;
            case 'R005':
                $('div[id$="CreateAnnouncement"]').hide();
                $('div[id$="divSelGroupType"]').show();
                break;
        };

    }
    function DoSelClinic(val) {
        $('div[id$="divSelFacilityName_Lab"]').hide();
        $('div[id$="divSelGroupType"]').hide();

        if (val == null || val == '0')
            $('div[id$="CreateAnnouncement"]').hide();
        else
            $('div[id$="CreateAnnouncement"]').show();
    }
    function DoSelLab(val) {
        $('div[id$="divSelFacilityName_Clinic"]').hide();
        $('div[id$="divSelGroupType"]').hide();

        if (val == null || val == '0')
            $('div[id$="CreateAnnouncement"]').hide();
        else
            $('div[id$="CreateAnnouncement"]').show();
    }
    function DoSelGroupType(val) {
        $('div[id$="divSelFacilityName_Clinic"]').hide();
        $('div[id$="divSelFacilityName_Lab"]').hide();
        $('div[id$="CreateAnnouncement"]').hide();

        if (val == 'G000' ||
            val == 'G001' ||
            val == 'G002' ||
            val == 'G003' ||
            val == 'G004' ||
            val == 'G007') {
            $('div[id$="CreateAnnouncement"]').show();
        }
    }
</script>