<%@ Page Title="" Language="C#" MasterPageFile="~/srtsMaster.Master" AutoEventWireup="true" CodeBehind="InformationCollected.aspx.cs" Inherits="SrtsWeb.Public.InformationCollected" %>

<%@ MasterType VirtualPath="~/srtsMaster.Master" %>

<asp:Content ID="Content6" ContentPlaceHolderID="MainContent_Public" runat="server">
         <div class="box_full_top" style="margin-top:30px;padding-top:10px;text-align:center;margin-bottom:-10px">
        <h1 style="font-size: 1.4em;margin-bottom:0px">Information Collected from the SRTS Homepage for Statistical Purposes</h1>
    </div>
    <div class="box_full_content" style="min-height: 300px">
        <div class="padding">
            <div class="padding">
                <h1>Below is an example of the information collected based on a standard
request for a World Wide Web document:</h1>
                <h2>xxx.yyy.com - - [28/Oct/19999:00:00:01 -0500] "GET /download/clinic1.exe
HTTP/1.0" 200 16704 Mozilla 3.0/www.altavista.digital.com</h2>
                <br />

                <h2>xxx.yyy.com (or 123.123.23.12)</h2>
                <blockquote>
                    This is the host name (or IP address) associated with the requester (you as the visitor). In this
    case, (....com) the requester is coming from a commercial address. Depending
    on the requester’s method of network connection, the host name (or IP address)
    may or may not identify a specific computer. Connections via many Internet
    Service Providers assign different IP addresses for each session, so the
    host name identifies only the ISP. The host name (or IP address) will identify
    a specific computer if that computer has a fixed IP address.
                </blockquote>
                <br />

                <h2>[28/Jan/1997:00:00:01 -0500]</h2>
                This is the date and time of the request
    <br />
                <br />

                <h2>"GET /download/clinic1.exe HTTP/1.0"</h2>
                This is the location of the requested file on the SRTS Homepage
    <br />
                <br />

                <h2>200</h2>
                This is the status code - 200 is OK - the request was filled
    <br />
                <br />

                <h2>16704</h2>
                This is the size of the requested file in bytes
    <br />
                <br />

                <h2>Mozilla 3.0</h2>
                <blockquote>
                    Identifies the type of browser software used to
    access the page, which indicates what design parameters to use in constructing
    the pages.
                </blockquote>
                <br />

                <h2>www.altavista.digital.com</h2>
                This indicates the last site the person visited, which indicates how people find the SRTS Homepage
    <br />
                <br />

                <blockquote>
                    Requests for other types of documents use similar information. No other
    user-identifying information is collected except within the registration
    and feedback forms where you will know what information we are collecting.
                </blockquote>
                <br />

                <br />
                <br />
            </div>
        </div>
    </div>
    <div class="box_full_bottom"></div>
</asp:Content>