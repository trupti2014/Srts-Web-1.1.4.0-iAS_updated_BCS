<%@ Page Title="About Us" Language="C#" MasterPageFile="~/srtsMaster.master" AutoEventWireup="true"
    CodeBehind="About.aspx.cs" Inherits="SrtsWeb.About" %>

<%@ MasterType VirtualPath="~/srtsMaster.Master" %>

<asp:Content ID="Content6" ContentPlaceHolderID="MainContent_Public" runat="server">
     <div class="box_full_top" style="margin-top:30px;padding-top:10px;text-align:center;margin-bottom:-10px">
        <h1 style="font-size: 1.4em;margin-bottom:0px">About SRTSweb</h1>
    </div>
    <div class="box_full_content" style="min-height: 300px">
        <div class="padding">
            <div class="padding" style="margin: 0px 30px">
                <p style="font-size:1.1em">Why SRTSweb....</p>
                <br />
                <p>
                    SRTSweb provides the only Tri-Service solution to support the need for a clinically integrated, secure web-based application to fulfill the Optical Fabrication Enterprise objectives.
                </p>
                <br />
                <p>
                    SRTSweb provides the capability for authorized clinics worldwide to record, store, retrieve, and transmit spectacle request information to optical fabrication labs as needed.  Remote users have the capability to utilize the GEyes functionality to re-order an existing srts order and have that order sent directly to the lab for fabrication.
                </p>
                <br />
                <p>
                    SRTSweb eliminates the need for client software, satisfies stringent Tri-Service Information Assurance (IA) requirements, and provides the user with a more state-of-the-art tool for ordering spectacles, inserts and masks.
                </p>
            </div>
        </div>
    </div>
    <div class="box_full_bottom"></div>
</asp:Content>