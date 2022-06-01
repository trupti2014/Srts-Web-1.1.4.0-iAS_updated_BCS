<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ucAdminDeleteUser.ascx.cs" Inherits="SrtsWeb.UserControls.ucAdminDeleteUser" EnableViewState="true" %>
<div style="height: 334px; margin: 10px 100px 10px 20px; padding: 10px 0px; border-bottom: 1px solid #E7CFAD;">
    <h1 style="font-weight:bold;">Delete Users Account</h1>
    <div style="float: left; width: 100%; margin-bottom: 15px;">
        <p class="srtsLabel_medium" style="font-weight:bold;">Select a user account to delete.</p>
    </div>
    <div style="float: left; width: 80%;">
        <fieldset>
            <legend class="srtsLabel_medium">User Names</legend>
            <div style="margin: 8px;">
                <asp:ListBox runat="server" ID="lbUsers" Height="150px" Width="100%" ClientIDMode="Predictable"></asp:ListBox>
            </div>
        </fieldset>
    </div>
    <div style="float: left; width: 100%;">
    </div>
    <div style="float: left; width: 100%;" id="confirmDelete">
        <asp:Button ID="bSubmitChanges"
            runat="server"
            Text="Delete User"
            OnClientClick="return ConfirmDelete();"
            OnClick="bSubmitChanges_Click"
            ClientIDMode="Predictable" />
    </div>
</div>
<script type="text/javascript">
    function ConfirmDelete() {
        var u = $($('#<%=this.lbUsers.ClientID%>')).val();
        if (u == null || u == '<%=this.defaultText%>') return false;

        if ($('#<%=this.lbUsers.ClientID%>').val() != null)
            return confirm('Are you sure you want to delete this user?');
        else
            return false;
    };
</script>
