<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ucAdminUnlockUser.ascx.cs" Inherits="SrtsWeb.UserControls.ucAdminUnlockUser" %>
<!-- Unlock User Account -->
<div style="height: 250px; margin: 10px; padding: 10px 0px">
    <div style="text-align: left; width: 100%; margin-bottom: 15px;">
        <p class="srtsLabel_medium" style="font-weight: bold;">Select a user account to unlock.</p>
    </div>
    <div style="float: left; width: 100%;">
        <fieldset>
            <legend class="srtsLabel_medium">User Names</legend>
            <div>
                <asp:ListBox runat="server" ID="lbUsers" Height="150px" Width="100%" ClientIDMode="Predictable"
                    Style="border: 1px solid #E4CFAC; border-radius: 4px; padding: 5px; cursor: pointer"></asp:ListBox>
            </div>
        </fieldset>
    </div>
    <div style="float: left; width: 100%;">
    </div>
    <div style="float: left; width: 100%; text-align: right; margin-top: 15px" id="confirmDelete">
        <asp:Button ID="bSubmitChanges"
            runat="server" CssClass="srtsButton"
            Text="Unlock User"
            ClientIDMode="Predictable"
            OnClientClick="return ConfirmUnlock();"
            OnClick="bSubmitChanges_Click" />
    </div>
</div>
<script type="text/javascript">
    function ConfirmUnlock() {
        var u = $($('#<%=this.lbUsers.ClientID%>')).val();
        if (u == null || u == '<%=this.defaultText%>') return false;

        if ($('#<%=this.lbUsers.ClientID%>').val() != null)
            return confirm('Are you sure you want to unlock this user?');
        else
            return false;
    };
</script>