<%@ Page Title="" Language="C#" MasterPageFile="~/JSpecs/JSpecsMaster.Master" AutoEventWireup="true"
    CodeBehind="JSpecsLogin.aspx.cs" Inherits="JSpecs.Forms.JSpecsLogin" %>
<%@ MasterType VirtualPath="~/JSpecs/JSpecsMaster.Master" %>

<asp:Content ID="contentHeader" ContentPlaceHolderID="contentHeader" runat="server">
<style>
.box {
  width: 40%;
  margin: 0 auto;
  background: rgba(255,255,255,0.2);
  padding: 35px;
  border: 2px solid #fff;
  border-radius: 20px/50px;
  background-clip: padding-box;
  text-align: center;
}

.overlay {
  position: fixed;
  top: 0;
  bottom: 0;
  left: 0;
  right: 0;
  background: rgba(0, 0, 0, 0.1);
  transition: opacity 500ms;
  visibility: hidden;
  opacity: 0;
}
.overlay:target {
  visibility: visible;
  opacity: 10;
}

.popup {
  margin: 90px auto;
  padding: 20px;
  border-radius: 5px;
  color: #ba3939;
  background: #ffe0e0;
  border: 1px solid #a33a3a;
  width: 30%;
  position: relative;
  z-index:1000;
  transition: all 5s ease-in-out;
}

.popup h2 {
  margin-top: 0;
  color: #333;
  font-family: Tahoma, Arial, sans-serif;
}
.popup .close {
  position: absolute;
  top: 20px;
  right: 30px;
  transition: all 200ms;
  font-size: 30px;
  font-weight: bold;
  text-decoration: none;
  color: #333;
}
.popup .close:hover {
  color: #ff0000;
}
.popup .content {
text-align:center;
padding:0px 20px 20px;
}
</style>

</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="Server" style="display: inline-flex">
<%--<asp:ScriptManagerProxy ID="smpLogin" runat="server">
<Scripts>
    <asp:ScriptReference Path="~/Scripts/Global/jquery-1.11.1.min.js" />
    <asp:ScriptReference Path="~/Scripts/Global/jquery-ui.min.js" />
</Scripts>
</asp:ScriptManagerProxy>--%>
    <article class="login wrap">
        <div class="login__module">
            <h1 class="login__module__title">CAC Log On</h1>
            <img src="../../../../JSpecs/imgs/CAC/cac-img.png" />
            <br /><br /><p>Please select your PIV authentication certificate when prompted.</p>
        </div>
        <div class="login__module login__module--right login__module--disabled">
            <h1 class="login__module__title">DS Log On</h1>
            <div class="input">
                <label>Username</label>
                <input />
                <span class="input__recovery-link">Forgot Username?</span>
            </div>
            <div class="input">
                <label>Password</label>
                <input />
                <span class="input__recovery-link">Forgot Password?</span>
            </div>
        </div>
    </article>
    <asp:LinkButton ID="lnkCacLogin" runat="server" class="btn btn--heavy btn--large"
            OnClick="btnCacYes_Click" style="max-width: 300px; margin: 0 auto;">Log On</asp:LinkButton>

<%--Begin PIV Authentication Message Handling--%>
<div id="errorMessage" runat="server" ClientIdMode="Static" class="overlay">
    <div class="popup">
        <h2>Oops!</h2>
        <a onClick="CloseMe(); return true;" class="close" href="#">&times;</a>
        <div id="messageContent" runat="server" class="content">                          
        </div>
    </div>
</div>
<div>
    <a id="showMsg" class="button" href="#errorMessage"></a>
</div>
<script type="text/javascript">
    function ShowMessage(message) {
        window.location=document.getElementById('showMsg').href;
        return false;
    }
    function CloseMe() {
        document.execCommand("ClearAuthenticationCache");
    }
</script>
<%--End PIV Authentication Message Handling--%>

</asp:Content>
