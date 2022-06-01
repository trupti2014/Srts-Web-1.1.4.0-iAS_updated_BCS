<%@ Page Title="DoD Spectacle Request Transmission System" Language="C#" MasterPageFile="srtsMaster.Master" AutoEventWireup="true" CodeBehind="srtsSelfService.aspx.cs" Inherits="srtsSelfService" %>


<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">


</asp:Content>


<asp:Content ID="MainContent" runat="server" ContentPlaceHolderID="MainContent">
<div id="tabHomeMenu">

<div class="content">
<div class="contentinner">
<div class="right"></div>
<div class="left"></div>
<%--<ul id="functionShortcuts">
<li class="findpatient">
<div id="findpatient">
<p class="navigation">
<a href="/SrtsWebClinic/Patients/PatientSearch.aspx" target="_self">
&nbsp;&nbsp;Patient Search
<span>
Choose this option to search for a patient.
</span>
</a>
</p>
</div>
</li>

<li class="adduser">
<div id="adduser">
<p class="navigation">
<a href="/SrtsWebClinic/Patients/PatientManagementAdd.aspx?Key=0" target="_self">
&nbsp;&nbsp;Add New Patient
<span>
Choose this option to add a patient you know is not currently in the system.  If you are not sure, you should perform a Patient Search first.
</span>
</a>
</p>
</div>
</li>

<li class="ordercheckin">
<div id="ordercheckin">
<p class="navigation">
<a href="/SrtsWebClinic/CheckInandDispense/CheckIn.aspx?key=1" target="_self">
Order Check-In
<span>
Choose this option if you have an order to check in.
</span>
</a>
</p>
</div>
</li>

<li class="orderdispense">
<div id="orderdispense">
<p class="navigation">
<a href="/SrtsWebClinic/CheckInAndDispense/CheckIn.aspx?Key=2" target="_self">
Dispense Order
<span>
Choose this option to dispense an order.
</span>
</a>
</p>
</div>
</li>
</ul> --%>      
</div>     
</div>




</div>

<div class="padding">
<srts:BeigeContainer ID="SRTSWebResources" runat="server" Title="SRTS Resources">
<asp:PlaceHolder ID="plhSRTSWebResources" runat="server" >
<div style="padding-top:20px;">
<div id="divcenterBottom">
<div id="centerright_bottom">
<br />
<h1>Documentation</h1>
<srts:WhiteBoxBlueHeader ID="WhiteBoxBlueHeader1" runat="server">
<ul>
<li><a href="PageTemplate.aspx" >Clinic Documentation</a></li>
<li><a href="PageTemplate.aspx" >Lab Documentation</a></li>
</ul>
</srts:WhiteBoxBlueHeader> 


</div>

<div id="centerleft_bottom">

</div>

<div id="centermiddle_bottom">
<div id="divSoftwareDownloads" >
<img src="../Styles/images/img_softwaredownloads.png" width="222px" height="52px" alt="Image - Software Downloads Section" />

<ul>
<li>
<a href="https://srts.amedd.army.mil/Download/Instructions.asp?session=12%3A17%3A41+PM&segment=ordering&type=upgrade" target="_blank">
Ordering Facility Upgrade</a>
</li>
<li>
<a href="https://srts.amedd.army.mil/Download/Instructions.asp?session=12%3A17%3A41+PM&segment=ordering&type=full1" target="_blank">
Ordering Facility Full Installation(one file)</a>
</li>
<li>
<a href="https://srts.amedd.army.mil/Download/Instructions.asp?session=12%3A17%3A41+PM&segment=ordering&type=ordmanual" target="_blank">
Ordering Facility Manual</a>
</li> 
</ul>

</div>
</div>


</div>
</div>
</asp:PlaceHolder>
</srts:BeigeContainer>

<srts:BeigeContainer ID="SRTSWebSupport" runat="server" Title="SRTS Technical Support">
<asp:PlaceHolder ID="plhSRTSWebSupport" runat="server" >
<div style="float:left;width:70%;margin-right:20px">


<img src="../Styles/images/img_knownproblems.png" width="211x" height="36px" alt="Image - Known Problems" /> 
<div>
<b class="rnd_top"><b class="rnd_b1"></b><b class="rnd_b2"></b>
<b class="rnd_b3"></b><b class="rnd_b4"></b></b>
<div class="rnd_content" style="text-align:left;color:#000000"> 

<ul>
<li style="border-bottom:1px solid #C6E7FF">
known problem 1
</li>
<li style="border-bottom:1px solid #C6E7FF">
known problem 2
</li>
</ul>
</div>
<b class="rnd_bottom"><b class="rnd_b4"></b><b class="rnd_b3"></b>
<b class="rnd_b2"></b><b class="rnd_b1"></b></b>
</div>

</div> 
<div style="width:25%">

<div id="divTechSupport" runat="server">

</div>
</div>
</asp:PlaceHolder>
</srts:BeigeContainer>

<srts:BeigeContainer ID="srtsTeamCommunication" runat="server" Title="SRTS Team Communication">
<asp:PlaceHolder ID="plhSrtsCommunication" runat="server" >
<srts:WhatsNew ID="srtsWhatsNew" runat="server"></srts:WhatsNew>
</asp:PlaceHolder> 
</srts:BeigeContainer>
</div>
</asp:Content>


