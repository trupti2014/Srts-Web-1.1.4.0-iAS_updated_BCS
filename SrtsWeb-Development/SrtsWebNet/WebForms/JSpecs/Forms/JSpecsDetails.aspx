<%@ Page Title="" Language="C#" MasterPageFile="~/JSpecs/JSpecsMaster.Master" AutoEventWireup="true"
    CodeBehind="JSpecsDetails.aspx.cs" Inherits="JSpecs.Forms.JSpecsDetails" %>
<asp:Content ID="contentHeader" ContentPlaceHolderID="contentHeader" runat="server">
    <script src="../../../Scripts/JSpecs/JSpecsNewPrescription.js"></script>
    <script src="../../../Scripts/JSpecs/pickaday.js"></script>
    <link rel="stylesheet" type="text/css" href="../../../Styles/pickaday.css" />
    <script>
        function initModalJavaScript() {
            var picker = new Pikaday({ field: document.getElementById('MainContent_tbPrescriptionDate') });
            var yesbtn = document.getElementById("MainContent_rbtnYes");
            var nobtn = document.getElementById("MainContent_rbtnNo");
            var modal = document.getElementById('MainContent_PanelAddPrescription');
            var frameHeight =  ( window.innerHeight < 865 ) ? window.innerHeight : 865;
            yesbtn.onclick = function () {
                var prismSection = document.getElementById("prism_table");
                prismSection.style.display = "block";
            }
            nobtn.onclick = function () {
                var prismSection = document.getElementById("prism_table");
                prismSection.style.display = "none";
            }
            modal.style.height = frameHeight * .75 + "px";

            modal.style.overflow = "scroll";
        }
    </script>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="Server">
        <script src="../../../Scripts/JSpecs/JSpecsLoaderHandler.js"></script>
        
    <%--Title--%>
    <div class="title title--margin-bottom">
        <h1 class="title__name">Your Profile</h1>
        <h2 class="title__subtitle">Please provide the details for your order.</h2>
    </div>

    <asp:ScriptManager ID="DetailsScriptManager" AsyncPostBackTimeout ="0" runat="server" EnablePartialRendering="true"></asp:ScriptManager>
   <!-- Content -->
    <article class="profile">
        <asp:UpdatePanel ID="UpdatePanelAddressSection" runat="server" UpdateMode="Conditional" style="display: inline-block;">
            <ContentTemplate>

                <p class="profile__required-text">Required</p>
                <section class="profile__module">
                    <div class="profile__module__heading__container">
                        <h1 class="profile__module__heading">Shipping Address <span class="profile__module__heading__number">1</span></h1>
                    </div>
                    <div class="profile__module__content">
                        <p class="profile__module__content__info">Select current address from drop down menu or you can add a new address.</p>
                        <div class="select">
                            <label class="select__text" required >Select your shipping address:</label>
                            <asp:DropDownList runat="server" ID="userShippingAddresses" onChange="ValidateContinueOrderBtn();" ClientIDMode="Static">
                            </asp:DropDownList>
                        </div>
                    </div>
                    <asp:LinkButton runat="server" ID="btnDisplayAddAddress" OnClick="btnDisplayAddAddress_Click" CausesValidation="false" class="profile__button btn btn--heavy" Text="ADD a new address" />
                </section>

                <!-- Address pop up modal -->
                <asp:Button runat="server" ID="btnOpenAddressMPEDummy" Style="display: none;" />
                <ajaxToolkit:ModalPopupExtender ID="MPEAddAddress" runat="server"
                    PopupControlID="PanelAddAddress" TargetControlID="btnOpenAddressMPEDummy"
                    BackgroundCssClass="Background">
                </ajaxToolkit:ModalPopupExtender>

                <asp:Panel runat="server">
                    <asp:UpdatePanel  class="address__modal" runat="server" ID="PanelAddAddress" UpdateMode="Conditional" Style="display: none;">
                        <ContentTemplate>
                            <div>
                                <asp:LinkButton runat="server" ID="lbtnCloseAddAddress" OnClick="btnCloseAddAddress_Click" class="close__button">Close <span class="close__button__circle">X</span></asp:LinkButton>
                                <div class="modal__left">
                                    <span runat="server" id="fvCheckAddressExists" class="error-message" visible="false"      style="color: red;">Address already exists.</span>
                                    <h2 class="modal__title">Enter your new address:</h2>
                                    <div class="modal__address-form">
                                        <div class="input">
                                            <label>Address</label>
                                            <input runat="server" id="inputStreetAddress" placeholder="Street Address" />
                                            <input runat="server" id="inputStreetAddress2" placeholder="Apt, Suite, Bldg, Gate Code. (Optional)" />
                                            <asp:RequiredFieldValidator ID="rfvStreetAddress" runat="server" ErrorMessage="Street Address cannot be blank." CssClass="error-message" Display="Dynamic" ControlToValidate="inputStreetAddress" ValidationGroup="AddressValidationGroup"></asp:RequiredFieldValidator>
                                        </div>

                                        <div class="input--group">
                                            <div class="input inline group-2">
                                                <label>City</label>
                                                <input runat="server" id="inputCity" placeholder="City" />
                                                <asp:RequiredFieldValidator ID="rfvCity" runat="server" ErrorMessage="City cannot be blank." CssClass="error-message" Display="Dynamic" ControlToValidate="inputCity" ValidationGroup="AddressValidationGroup"></asp:RequiredFieldValidator>
                                            </div>
                                            <div class="input inline group-2 ">
                                                <label>Zip Code</label>
                                                <input runat="server" id="inputZipcode" placeholder="Zip Code" />
                                                <asp:RequiredFieldValidator ID="rfvZipcode" runat="server" ErrorMessage="Zip Code cannot be blank." CssClass="error-message" Display="Dynamic" ControlToValidate="inputZipcode" ValidationGroup="AddressValidationGroup"></asp:RequiredFieldValidator>
                                                <asp:RegularExpressionValidator ID="revZipcode" runat="server" ErrorMessage="Invalid Zip Code format." CssClass="error-message" Display="Dynamic" ControlToValidate="inputZipcode" ValidationExpression="\d{5}-?(\d{4})?$" ValidationGroup="AddressValidationGroup"></asp:RegularExpressionValidator>
                                            </div>
                                        </div>

                                        <div class="select">
                                            <label>State</label>
                                            <asp:DropDownList runat="server" ID="stateInput">
                                            </asp:DropDownList>
                                            <asp:RequiredFieldValidator
                                                ID="rfvState"
                                                runat="server"
                                                ControlToValidate="stateInput"
                                                ValidationGroup="AddressValidationGroup"
                                                InitialValue="--Select State--"
                                                ErrorMessage="Please select a state."
                                                CssClass="error-message">
                                            </asp:RequiredFieldValidator>
                                        </div>
                                        <div class="select">
                                            <label>Country</label>
                                            <asp:DropDownList runat="server" ID="countryInput">
                                            </asp:DropDownList>
                                            <asp:RequiredFieldValidator
                                                ID="rfvCountry"
                                                runat="server"
                                                ControlToValidate="countryInput"
                                                ValidationGroup="AddressValidationGroup"
                                                InitialValue="--Select Country--"
                                                ErrorMessage="Please select a country."
                                                CssClass="error-message">
                                            </asp:RequiredFieldValidator>
                                        </div>
                                        <div class="modal__btn-container">
                                            <asp:LinkButton runat="server" ID="btnVerifyAddress" OnClick="btnVerifyAddress_click" CausesValidation="true" ValidationGroup="AddressValidationGroup" CssClass="btn btn--heavy" Height="100%" Text="Verify" />
                                        </div>
                                    </div>
                                </div>
                                <span class="modal__split__vertically"></span>
                                <div class="modal__right">
                                    <div runat="server" id="userEnteredAddressResultContainer" class="modal__address-result" visible="false">
                                        <h3>You Entered:</h3>
                                        <p runat="server" id="userAddress"></p>
                                        <p runat="server" id="userAddress2"></p>
                                        <p runat="server" id="userAddress3"></p>
                                        <asp:LinkButton runat="server" ID="lbtnUserAddAddress" CssClass="btn btn--heavy" style="min-width: unset;" OnClick="btnAddAddress_Click" Height="100%" Text="Use address as entered" />
                                    </div>
                                    <div runat="server" id="uspsAddressResultContainer" class="modal__address-result" visible="false">
                                        <h3>USPS recommends:</h3>
                                        <p runat="server" id="uspsAddress"></p>
                                        <p runat="server" id="uspsAddress2"></p>
                                        <asp:LinkButton runat="server" ID="lbtnUSPSAddAddress" OnClick="btnAddAddress_Click" CssClass="btn btn--heavy" Height="100%" Text="Use USPS recommendation" />
                                    </div>
                                </div>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </asp:Panel>
                <!-- Address pop up modal end -->
            </ContentTemplate>
        </asp:UpdatePanel>

        <asp:UpdatePanel ID="UpdatePrescriptionPanel" runat="server" UpdateMode="Conditional" style="display: inline-block;">
            <ContentTemplate>
                <section class="profile__module">
                    <div class="profile__module__heading__container">
                        <h1 class="profile__module__heading">Prescription<span class="profile__module__heading__number">2</span></h1>
                    </div>
                    <div class="profile__module__content">
                        <p class="profile__module__content__info">Select current prescription from drop down menu or you can add a new prescription.</p>
                        <div class="select">
                            <label class="select__text" required>Select your prescription:</label>
                            <asp:DropDownList runat="server" ID="userPrescriptions" onChange="ValidateContinueOrderBtn();" ClientIDMode="Static" DataTextFormatString="{0:MM/dd/yyyy}" AutoPostBack="true">
                            </asp:DropDownList>
                        </div>
                        <%--Removing this feature for now since it's functionality is disabled--%>
                        <%--<div class="select">
                            <label class="select__text">Select type of glasses:</label>
                            <asp:DropDownList runat="server" ID="userGlassesSelection" disabled ClientIDMode="Static">
                                <asp:ListItem>N/A</asp:ListItem>
                            </asp:DropDownList>
                        </div>--%>
                    </div>
                    
                    <asp:LinkButton runat="server" ID="btnOpenAddPrescription" OnClick="btnOpenAddPrescription_Click" CausesValidation="false" class="profile__button btn btn--heavy" Text="ADD a new prescription" Style=""></asp:LinkButton>
                    
                </section>

                <%-- New Prescription pop up modal--%>
                <asp:Button runat="server" ID="btnOpenPrescriptionMPEDummy" Style="display: none;" />
                <ajaxToolkit:ModalPopupExtender ID="MPEAddPrescription" runat="server"
                    PopupControlID="PanelAddPrescription" TargetControlID="btnOpenPrescriptionMPEDummy"
                    BackgroundCssClass="Background">
                </ajaxToolkit:ModalPopupExtender>

                <asp:Panel runat="server">
                    <asp:UpdatePanel runat="server" ID="PanelAddPrescription" UpdateMode="Conditional" Style="display: none;">
                        
                        <ContentTemplate>
                            <div class="modal" style="float:left;">
                                
                                <asp:LinkButton runat="server" ID="lnkbtnCloseAddPrescriptionPanel" OnClick="btnCloseAddPrescription_Click" CausesValidation="false" CssClass="close__button">Close <span class="close__button__circle">X</span></asp:LinkButton>
                                
                                <br />
                                <h2 class="modal__title">Add Prescription:</h2>
                                <div class="modal__prescription-form">
                                    <h3>Please enter your prescription.</h3>
                                    <div style="text-align:left; margin-bottom:25px;">
                                        <Label>Prescription Date:</Label> <asp:TextBox ID="tbPrescriptionDate" ValidationGroup="PrescriptionValidationGroup" runat="server"></asp:TextBox>
                                    </div>

                                    <h5 style="text-align:left;">Add values left to right i.e. Sphere, Cylinder, Axis and Add<br /><span style="color:red; display:none;">*Cylinder is required to filed before Axis*</span></h5>
                                    

                                    <table class="prescription_form">
                                        <thead>
                                            <tr>
                                                <th></th>
                                                <th>Sphere (SPH)</th>
                                                <th>Cylinder (CYL)</th>
                                                <th>Axis</th>
                                                <th>Add</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            <tr>
                                                <th>OD-Right</th>
                                                <td><asp:DropDownList runat="server" ID="rightSphereDD" onchange="validateForm()"><asp:ListItem Value="na">--Select Value--</asp:ListItem></asp:DropDownList></td>
                                                <td><asp:DropDownList runat="server" ID="rightCylinderDD" onchange="noAxisIfCylIsZero(event)"><asp:ListItem Value="0">--Select Value--</asp:ListItem></asp:DropDownList></td>
                                                <td><asp:DropDownList runat="server" ID="rightAxisDD" onchange="resetAxisIfCylIsZero(event)" disabled="disabled"><asp:ListItem Value="0">--Select Value--</asp:ListItem></asp:DropDownList></td>
                                                <td><asp:DropDownList runat="server" ID="rightAddDD" onchange="autoPopulateOSFromOD(event)" ><asp:ListItem Value="0">--Select Value--</asp:ListItem></asp:DropDownList></td>
                                            </tr> 
                                            <tr>
                                                <th>OS-Left</th>
                                                <td><asp:DropDownList runat="server" ID="leftSphereDD" onchange="validateForm()"><asp:ListItem Value="na">--Select Value--</asp:ListItem></asp:DropDownList></td>
                                                <td><asp:DropDownList runat="server" ID="leftCylinderDD" onchange="noAxisIfCylIsZero(event)"><asp:ListItem Value="0">--Select Value--</asp:ListItem></asp:DropDownList></td>
                                                <td><asp:DropDownList runat="server" ID="leftAxisDD" onchange="resetAxisIfCylIsZero(event)" disabled="disabled"><asp:ListItem Value="0">--Select Value--</asp:ListItem></asp:DropDownList></td>
                                                <td><asp:DropDownList runat="server" ID="leftAddDD" onchange="autoPopulateOSFromOD(event)"><asp:ListItem Value="0">--Select Value--</asp:ListItem></asp:DropDownList></td>
                                            </tr>
                                        </tbody>
                                    </table>

                                    <h4>PD-Pupillary distance</h4>
                                    <table class="prescription_form" style="width:100%;">
                                        <thead>
                                            <tr>
                                                <th>One PD</th>
                                                <th>Two PDs</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            <tr>
                                                <td><asp:DropDownList runat="server" ID="onePDDD" onchange="resetTwoPDs()"><asp:ListItem Value="0">--Select Value--</asp:ListItem></asp:DropDownList></td>
                                                <td>Right: <asp:DropDownList runat="server" ID="twoPDRightDD" onchange="resetOnePD()"><asp:ListItem Value="0">--Select Value--</asp:ListItem></asp:DropDownList><br />
                                                    Left:  <asp:DropDownList runat="server" ID="twoPDLeftDD" onchange="resetOnePD()"><asp:ListItem Value="0">--Select Value--</asp:ListItem></asp:DropDownList>
                                                </td>
                                            </tr>
                                        </tbody>
                                    </table>
                                    
                                    <div style="text-align:left; margin:25px 0;">
                                        <Label for="MainContent_rbtnYes">Prism Values:</Label> 
                                        <asp:RadioButton ID="rbtnYes" runat="server" GroupName="PrismValues" Text="Yes" />
                                        <asp:RadioButton ID="rbtnNo" runat="server" GroupName="PrismValues" Text="No" checked="true"/>
                                    </div>
                                    

                                     <table class="prescription_form" id="prism_table" style="display:none;">
                                        <thead>
                                            <tr>
                                                <th></th>
                                                <th>Prism Horizontal</th>
                                                <th>Base Direction</th>
                                                <th>Prism Vertical</th>
                                                <th>Base Direction</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            <tr>
                                                <th>OD-Right</th>
                                                <td><asp:DropDownList runat="server" ID="rightPrismHorizontalDD" onchange="validateForm()"><asp:ListItem Value="0">--Select Value--</asp:ListItem></asp:DropDownList></td>
                                                <td><asp:DropDownList runat="server" ID="rightPrismHorizontalBaseDirectionDD" onchange="validateForm()"><asp:ListItem Value="In">--Select Value--</asp:ListItem></asp:DropDownList></td>
                                                <td><asp:DropDownList runat="server" ID="rightPrismVerticalDD" onchange="validateForm()"><asp:ListItem Value="0">--Select Value--</asp:ListItem></asp:DropDownList></td>
                                                <td><asp:DropDownList runat="server" ID="rightPrismVerticalBaseDirectionDD" onchange="validateForm()"><asp:ListItem Value="In">--Select Value--</asp:ListItem></asp:DropDownList></td>
                                            </tr>
                                            <tr>
                                                <th>OS-Left</th>
                                                <td><asp:DropDownList runat="server" ID="leftPrismHorizontalDD" onchange="validateForm()"><asp:ListItem Value="0">--Select Value--</asp:ListItem></asp:DropDownList></td>
                                                <td><asp:DropDownList runat="server" ID="leftPrismHorizontalBaseDirectionDD" onchange="validateForm()"><asp:ListItem Value="In">--Select Value--</asp:ListItem></asp:DropDownList></td>
                                                <td><asp:DropDownList runat="server" ID="leftPrismVerticalDD" onchange="validateForm()"><asp:ListItem Value="0">--Select Value--</asp:ListItem></asp:DropDownList></td>
                                                <td><asp:DropDownList runat="server" ID="leftPrismVerticalBaseDirectionDD" onchange="validateForm()"><asp:ListItem Value="Out">--Select Value--</asp:ListItem></asp:DropDownList></td>
                                            </tr>
                                        </tbody>
                                    </table>
                                    <div style="width:95%;max-width:660px">
                                        <p>Please upload a picture of your prescription.  Ensure your name, the provider's name and address, date of prescription and provider signature are clear.</p>
                                        <img id="previewImageRX" src=""/>
                                        <input id="choosePrescription" type="file" accept="image/*" onchange="addPrescriptionPhoto(event)"/>
                                        <asp:HiddenField runat="server" ID="hfieldPrescriptionImageSelected" Value="na" />
                                        <asp:HiddenField runat="server" ID="hfieldPrescriptionImageName" Value="na" />
                                        <div id="divCenter" runat="server" class="divhead">

                                        </div>
                                    </div>
                                    <div class="modal__btn-container">
                                        <asp:LinkButton runat="server" ID="btnAddPrescription" OnClick="AddPrescription" OnClientClick="if(!validateForm()){return false;}" ValidationGroup="PrescriptionValidationGroup" disabled="true" CausesValidation="true" CssClass="btn btn--heavy" Height="100%" Text="Add new prescription" style="" AutoPostBack="false"/>
                                    </div>
                                </div>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </asp:Panel>
                <%-- New Prescription pop up modal end--%>
            </ContentTemplate>
        </asp:UpdatePanel>

        <asp:UpdatePanel ID="UpdatePanelEmailSection" runat="server" UpdateMode="Conditional" style="display: inline-block;">
            <ContentTemplate>
                <section class="profile__module">
                    <div class="profile__module__heading__container">
                        <h1 class="profile__module__heading">Email Address<span class="profile__module__heading__number">3</span></h1>
                    </div>
                    <div class="profile__module__content">
                        <p class="profile__module__content__info">Select current email address from drop down menu or you can add a new email address.</p>
                        <div class="select">
                            <label class="select__text" required>Select your email:</label>
                            <asp:DropDownList runat="server" ID="userEmailAddresses" onChange="ValidateContinueOrderBtn();"  ClientIDMode="Static">
                            </asp:DropDownList>
                        </div>
                    </div>
                    <asp:LinkButton runat="server" ID="btnDisplayAddEmail" OnClick="btnDisplayAddEmail_click" CausesValidation="false" class="profile__button btn btn--heavy" Text="ADD a new email" />
                </section>

                <%-- Email pop up modal--%>
                <asp:Button runat="server" ID="btnOpenEmailMPEDummy" Style="display: none;" />
                <ajaxToolkit:ModalPopupExtender ID="MPEAddEmail" runat="server"
                    PopupControlID="PanelAddEmail" TargetControlID="btnOpenEmailMPEDummy"
                    BackgroundCssClass="Background">
                </ajaxToolkit:ModalPopupExtender>

                <asp:Panel runat="server">
                    <asp:UpdatePanel runat="server" ID="PanelAddEmail" UpdateMode="Conditional" Style="display: none;">
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="inputEmail" EventName="TextChanged" />
                        </Triggers>
                        <ContentTemplate>
                            <div class="modal">
                                <asp:LinkButton runat="server" ID="lnkbtnCloseAddEmailPanel" OnClick="btnCloseAddEmail_Click" CausesValidation="false" CssClass="close__button">Close <span class="close__button__circle">X</span></asp:LinkButton>
                                <br />
                                <h2 class="modal__title">Enter your new email address:</h2>
                                <div class="modal__email-form">
                                    <div class="input">
                                        <label>Email address</label>
                                        <asp:TextBox runat="server" ID="inputEmail" ClientIDMode="Static" placeholder="Enter email address" />
                                        <span runat="server" id="fvCheckInputEmailAddress" class="error-message" visible="false" style="color: red;">Email address already exists.</span>
                                        <asp:RequiredFieldValidator runat="server" ID="rfvEmailAddress" ErrorMessage="Email Address cannot be blank." CssClass="error-message" Display="Dynamic" ControlToValidate="inputEmail" ValidationGroup="EmailValidationGroup"></asp:RequiredFieldValidator>
                                        <asp:RegularExpressionValidator runat="server" ID="revEmailAddress" ErrorMessage="Invalid Email Address format." CssClass="error-message" Display="Dynamic" ControlToValidate="inputEmail" ValidationExpression="\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" ValidationGroup="EmailValidationGroup"></asp:RegularExpressionValidator>
                                    </div>
                                    <div class="input">
                                        <label>Re enter Email address</label>
                                        <input runat="server" id="inputConfirmEmail" placeholder="Re enter email address" />
                                        <asp:RequiredFieldValidator runat="server" ID="rfvValidateEmailAddress" ErrorMessage="Confirmation Email Address cannot be blank." CssClass="error-message" Display="Dynamic" ControlToValidate="inputConfirmEmail" ValidationGroup="EmailValidationGroup"></asp:RequiredFieldValidator>
                                        <asp:CompareValidator runat="server" ID="cvValidateEmailAddress" ErrorMessage="Confirmation Email Address must match your Email Address." CssClass="error-message" Display="Dynamic" ControlToValidate="inputConfirmEmail" ControlToCompare="inputEmail" Operator="Equal" Type="String" ValidationGroup="EmailValidationGroup"></asp:CompareValidator>
                                    </div>
                                    <div class="modal__btn-container">
                                        <asp:LinkButton runat="server" ID="lbtnAddEmail" OnClick="AddEmailAddress" ValidationGroup="EmailValidationGroup" CausesValidation="true" CssClass="btn btn--heavy" Height="100%" Text="Add new email address" />
                                    </div>
                                </div>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </asp:Panel>
                <%-- Email pop up modal end--%>
            </ContentTemplate>
        </asp:UpdatePanel>
        <br /><br /><br />
        <%--<nav class="profile__nav">
            <a id="stateBackBtn" class="btn btn--small">BACK</a>
            <div id="stateNav" class="profile__nav__state">
                <span class="profile__nav__state__circle active"></span>
                <span class="profile__nav__state__circle"></span>
                <span class="profile__nav__state__circle"></span>
            </div>
            <a id="stateNextBtn" class="btn btn--small">NEXT</a>--%>
            <asp:LinkButton runat="server" ID="lbtnSubmitDetails" ClientIDMode="Static" OnClick="btnSubmitDetails_Click" Disabled="true" class="btn">Select Glasses</asp:LinkButton>
<%--        </nav>--%>

    </article>
    <!--End Of Content-->

    <script src="../../../Scripts/JSpecs/JSpecsDetailsValidation.js"></script>
</asp:Content>
