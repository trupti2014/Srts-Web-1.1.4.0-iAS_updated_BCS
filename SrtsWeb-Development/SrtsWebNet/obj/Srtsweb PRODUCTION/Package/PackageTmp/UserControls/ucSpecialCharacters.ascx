<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ucSpecialCharacters.ascx.cs" Inherits="SrtsWeb.UserControls.ucSpecialCharacters" %>
<div id="divAsciiChooser" style="display: none;">
    <asp:ScriptManagerProxy ID="smpAscii" runat="server">
        <Scripts>
            <asp:ScriptReference Path="~/Scripts/Global/Ascii.js" />
        </Scripts>
    </asp:ScriptManagerProxy>
    <div style="margin: 10px;">
        <select id="selLetterGroup" style="width: 100px;" onchange="DoDdlChange();">
            <option value="X">Select</option>
            <option value="A">A</option>
            <option value="E">E</option>
            <option value="I">I</option>
            <option value="O">O</option>
            <option value="U">U</option>
            <option value="Y">Y</option>
            <option value="N">N</option>
            <option value="Misc">Misc</option>
        </select>
    </div>
    <div id="divAsciiChars" style="margin: 10px;">
        <div id="divA" style="display: none;">
            <asp:RadioButtonList ID="rblLetterA" runat="server" RepeatColumns="5" ClientIDMode="Static">
                <asp:ListItem Text="&Aacute;" Value="u193"></asp:ListItem>
                <asp:ListItem Text="&aacute;" Value="l225"></asp:ListItem>
                <asp:ListItem Text="&Auml;" Value="u196"></asp:ListItem>
                <asp:ListItem Text="&auml;" Value="l228"></asp:ListItem>
                <asp:ListItem Text="&Acirc;" Value="u194"></asp:ListItem>
                <asp:ListItem Text="&acirc;" Value="l226"></asp:ListItem>
                <asp:ListItem Text="&Agrave;" Value="u192"></asp:ListItem>
                <asp:ListItem Text="&agrave;" Value="l224"></asp:ListItem>
                <asp:ListItem Text="&Aring;" Value="u197"></asp:ListItem>
                <asp:ListItem Text="&aring;" Value="l229"></asp:ListItem>
                <asp:ListItem Text="&Atilde;" Value="u195"></asp:ListItem>
                <asp:ListItem Text="&atilde;" Value="l227"></asp:ListItem>
            </asp:RadioButtonList>
        </div>
        <div id="divE" style="display: none;">
            <asp:RadioButtonList ID="rblLetterE" runat="server" RepeatColumns="4" ClientIDMode="Static">
                <asp:ListItem Text="&Eacute;" Value="u201"></asp:ListItem>
                <asp:ListItem Text="&eacute;" Value="l233"></asp:ListItem>
                <asp:ListItem Text="&Euml;" Value="u203"></asp:ListItem>
                <asp:ListItem Text="&euml;" Value="l235"></asp:ListItem>
                <asp:ListItem Text="&Ecirc;" Value="u202"></asp:ListItem>
                <asp:ListItem Text="&ecirc;" Value="l234"></asp:ListItem>
                <asp:ListItem Text="&Egrave;" Value="u200"></asp:ListItem>
                <asp:ListItem Text="&egrave;" Value="l232"></asp:ListItem>
            </asp:RadioButtonList>
        </div>
        <div id="divI" style="display: none;">
            <asp:RadioButtonList ID="rblLetterI" runat="server" RepeatColumns="4" ClientIDMode="Static">
                <asp:ListItem Text="&Iacute;" Value="u205"></asp:ListItem>
                <asp:ListItem Text="&iacute;" Value="l237"></asp:ListItem>
                <asp:ListItem Text="&Iuml;" Value="u207"></asp:ListItem>
                <asp:ListItem Text="&iuml;" Value="l239"></asp:ListItem>
                <asp:ListItem Text="&Icirc;" Value="u206"></asp:ListItem>
                <asp:ListItem Text="&icirc;" Value="l238"></asp:ListItem>
                <asp:ListItem Text="&Igrave;" Value="u204"></asp:ListItem>
                <asp:ListItem Text="&igrave;" Value="l236"></asp:ListItem>
            </asp:RadioButtonList>
        </div>
        <div id="divO" style="display: none;">
            <asp:RadioButtonList ID="rblLetterO" runat="server" RepeatColumns="4" ClientIDMode="Static">
                <asp:ListItem Text="&Oacute;" Value="u211"></asp:ListItem>
                <asp:ListItem Text="&oacute;" Value="l243"></asp:ListItem>
                <asp:ListItem Text="&Ouml;" Value="u214"></asp:ListItem>
                <asp:ListItem Text="&ouml;" Value="l246"></asp:ListItem>
                <asp:ListItem Text="&Ocirc;" Value="u212"></asp:ListItem>
                <asp:ListItem Text="&ocirc;" Value="l244"></asp:ListItem>
                <asp:ListItem Text="&Ograve;" Value="u210"></asp:ListItem>
                <asp:ListItem Text="&ograve;" Value="l242"></asp:ListItem>
                <asp:ListItem Text="&Otilde;" Value="u213"></asp:ListItem>
                <asp:ListItem Text="&otilde;" Value="l245"></asp:ListItem>
                <asp:ListItem Text="&Oslash;" Value="u216"></asp:ListItem>
                <asp:ListItem Text="&oslash;" Value="l248"></asp:ListItem>
            </asp:RadioButtonList>
        </div>
        <div id="divU" style="display: none;">
            <asp:RadioButtonList ID="rblLetterU" runat="server" RepeatColumns="4" ClientIDMode="Static">
                <asp:ListItem Text="&Uacute;" Value="u218"></asp:ListItem>
                <asp:ListItem Text="&uacute;" Value="l250"></asp:ListItem>
                <asp:ListItem Text="&Uuml;" Value="u220"></asp:ListItem>
                <asp:ListItem Text="&uuml;" Value="l252"></asp:ListItem>
                <asp:ListItem Text="&Ucirc;" Value="u219"></asp:ListItem>
                <asp:ListItem Text="&ucirc;" Value="l251"></asp:ListItem>
                <asp:ListItem Text="&Ugrave;" Value="u217"></asp:ListItem>
                <asp:ListItem Text="&ugrave;" Value="l249"></asp:ListItem>
            </asp:RadioButtonList>
        </div>
        <div id="divY" style="display: none;">
            <asp:RadioButtonList ID="rblLetterY" runat="server" RepeatColumns="2" ClientIDMode="Static">
                <asp:ListItem Text="&Yacute;" Value="u221"></asp:ListItem>
                <asp:ListItem Text="&yacute;" Value="l253"></asp:ListItem>
                <asp:ListItem Text="&Yuml;" Value="u159"></asp:ListItem>
                <asp:ListItem Text="&yuml;" Value="l255"></asp:ListItem>
            </asp:RadioButtonList>
        </div>
        <div id="divN" style="display: none;">
            <asp:RadioButtonList ID="rblLetterN" runat="server" RepeatColumns="2" ClientIDMode="Static">
                <asp:ListItem Text="&Ntilde;" Value="u209"></asp:ListItem>
                <asp:ListItem Text="&ntilde;" Value="l241"></asp:ListItem>
            </asp:RadioButtonList>
        </div>
        <div id="divMisc" style="display: none;">
            <asp:RadioButtonList ID="rblLetterMisc" runat="server" RepeatColumns="4" ClientIDMode="Static">
                <asp:ListItem Text="&AElig;" Value="u198"></asp:ListItem>
                <asp:ListItem Text="&aelig;" Value="l230"></asp:ListItem>
                <asp:ListItem Text="&Ccedil;" Value="u199"></asp:ListItem>
                <asp:ListItem Text="&ccedil;" Value="l231"></asp:ListItem>
                <asp:ListItem Text="&ETH;" Value="u208"></asp:ListItem>
                <asp:ListItem Text="&eth;" Value="l240"></asp:ListItem>
                <asp:ListItem Text="&THORN;" Value="u222"></asp:ListItem>
                <asp:ListItem Text="&thorn;" Value="l254"></asp:ListItem>
            </asp:RadioButtonList>
        </div>
    </div>
    <div id="divButtons" style="margin: 8px 0px; display: none;">
        <input type="button" id="bReplace" class="srtsButton" value="Replace" onclick="DoReplace();" />
        <input type="button" id="bReplaceAll" class="srtsButton" value="Replace All" onclick="DoReplaceAll();" />
    </div>
</div>
