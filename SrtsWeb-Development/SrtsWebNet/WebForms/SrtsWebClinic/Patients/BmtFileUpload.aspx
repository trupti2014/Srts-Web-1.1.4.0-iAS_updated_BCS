<%@ Page Title="" Language="C#" MasterPageFile="~/srtsMaster.Master" AutoEventWireup="true" CodeBehind="BmtFileUpload.aspx.cs" 
    Inherits="SrtsWebClinic.Patients.BmtFileUpload" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script src="../../Scripts/jquery-1.9.1.min.js"></script>
    <%--<meta http-equiv="X-UA-Compatible" content="IE=EmulateIE7" />
    <title>DoD Spectacle Request Transmission System - BMT Upload</title>--%>
    <link rel="Stylesheet" href="~/Styles/srtsBaseStyles.css" type="text/css" />
    <script type="text/javascript">
        $(function () {
            $('#btnGetFile').on('click', function () {
                $('#<%=fUpload.ClientID%>').trigger('click');
            });
            $('#<%=fUpload.ClientID%>').on('change', function () {
                $('#<%=lblFileName.ClientID%>').html($(this).val());
                $('#<%=hfFile.ClientID%>').val($(this).val());
            });
        });
    </script>

</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="MainContent" runat="server">
    <asp:UpdatePanel ID="upFileLoad" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div class="box_full_top" style="height: 20px"></div>
            <div class="box_full_content">

                <div class="pageContent">

                    <div class="padding">
                        <p style="text-align: center" class="colorBlue">
                            Select a file to upload your data then select the <strong>Process File</strong> Button. 
                                                <br />
                            When complete, you will be given the number of records entered
                                                <br />
                            or you will receive a message indicating the individualss who were not entered into the database.
                        </p>
                        <br />
                        <br />
                        <div style="width: 50%; margin: 0px auto; text-align: center;">
                            <!-- the file upload control is hidden via css and the input button below actually calls the file upload controls click event. -->
                            <input type="button" id="btnGetFile" title="Get File" value="Get File" />
                            <asp:FileUpload ID="fUpload" runat="server" />
                            <asp:Button ID="btnProcess" runat="server" Text="Process File" OnClick="btnProcess_Click" ClientIDMode="Static" UseSubmitBehavior="true" />
                            <br />
                            <br />
                            <asp:Label ID="lblFileName" runat="server" CssClass="srtsLabel_medium"></asp:Label>
                        </div>
                    </div>
                    <br />
                    <asp:Label ID="lblInfo" runat="server"></asp:Label><br />
                    <asp:ValidationSummary ID="uploadSummary" DisplayMode="BulletList" ShowSummary="true" runat="server" />
                </div>
                <asp:HiddenField ID="hfFile" runat="server" />
            </div>
            <div class="box_full_bottom"></div>
        </ContentTemplate>
        <Triggers>
            <%--<asp:PostBackTrigger ControlID="btnProcess" />--%>
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>
