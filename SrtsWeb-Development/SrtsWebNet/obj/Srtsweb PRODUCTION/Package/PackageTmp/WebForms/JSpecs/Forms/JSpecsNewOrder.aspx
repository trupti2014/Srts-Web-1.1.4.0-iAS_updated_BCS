<%@ Page Title="" Language="C#" MasterPageFile="~/JSpecs/JSpecsMaster.Master" AutoEventWireup="true" CodeBehind="JSpecsNewOrder.aspx.cs" Inherits="JSpecs.Forms.JSpecsNewOrder" %>
<asp:Content ID="contentHeader" ContentPlaceHolderID="contentHeader" runat="server">
<style>
    .modal{
        padding-left: 40px;
    }
    .frames{
        float:left;
        padding: 0 0 30px 0;
        max-height: 600px;
        overflow: scroll;
    }
    .frames li{
        list-style: none;
        width: 20%;
        margin: 0 4% 20px 0;
        float: left;
        border: 1px solid black;
        position:relative;
        min-height: 300px;
    }
    .frames .frame_details{
        padding: 15px;
    }
    .frame__button {
        width: 200px;
        margin: 0 auto;
        position: absolute;
        .0
        bottom: 30px;
        left: 55px;
    }
    .profile__button {
        bottom : 25px;
    }
    #MainContent_UpdatePanelAddressSection,
    #MainContent_UpdatePrescriptionPanel,
    #MainContent_UpdatePanelEmailSection{
        float:left;
    }
    #lbtnSubmitDetails{
        float:left;
        width:98%;
        margin-top: 30px;
    }
    @media only screen and (max-width:1580px){
        .frame__button {
            left: 30px;
        }
    }
    @media only screen and (max-width:1380px){
        .frames li{
            width:27%;
        }
        .frame__button {
            left: 55px;
        }
    }
    @media only screen and (max-width:1220px){
        .frames li{
            width:40%;
        }
    }
    @media only screen and (max-width:880px){
        .frames li{
            width:60%;
        }
    }
    @media only screen and (max-width:650px){
        .frames li{
            width:70%;
        }
    }
    
    @media only screen and (max-height:768px){
        .modal{
           top: 10px;
        }
    }
</style>

</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="Server">
        <script src="../../../Scripts/JSpecs/JSpecsLoaderHandler.js"></script>
    <%--Title--%>
    <div class="title title--margin-bottom">
        <h1 class="title__name">New Order</h1>
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
                        <p class="profile__module__content__info">Select current prescription from drop down menu<%-- or you can add a new prescription--%>.</p>
                        <div class="select">
                            <label class="select__text" required>Select your prescription:</label>
                            <asp:DropDownList runat="server" ID="userPrescriptions" onChange="ValidateContinueOrderBtn();" ClientIDMode="Static" DataTextFormatString="{0:MM/dd/yyyy}">
                            </asp:DropDownList>
                        </div>
                        <%--Removing this feature for now since it's functionality is disabled--%>
                        <div class="select">
                            <label class="select__text" required>Select type of glasses:</label>
                            <asp:DropDownList runat="server" ID="userGlassesSelection" ClientIDMode="Static" onChange="ValidateAddFramesBtn();">
                                <asp:ListItem Value="0">--Select Glasses--</asp:ListItem>

                                <!--
                                <asp:ListItem Value="1">Standard Issue (Black) Glasses</asp:ListItem>
                                <asp:ListItem Value="2">Frame of Choice (Civilian) Glasses</asp:ListItem>
                                <asp:ListItem Value="3">Inserts for Protective Eyewear (MCEP)</asp:ListItem>
                                -->

                                <asp:ListItem Value="UPLC;REVISION UPLC INSERT">Inserts for Protective Eyewear (MCEP)</asp:ListItem>
                                <asp:ListItem Value="4">Inserts for Protective (Gas) Mask</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <asp:HiddenField runat="server" ID="hfieldFramesSelected" Value="" />
                        <asp:Label runat="server" ID="labelFieldFrameDescription" />
                        <asp:LinkButton runat="server" ID="btnDisplayAddGlasses" OnClick="btnDisplayAddGlasses_click" CausesValidation="false" class="profile__button btn btn--heavy" Text="Select Frames" Disabled="true"/>
                    </div>
                   <%-- <asp:LinkButton runat="server" class="profile__button btn btn--heavy disabled" Text="ADD a new prescription" Disabled="true"></asp:LinkButton>--%>
                </section>

                <!--

                <asp:Button runat="server" ID="btnOpenGlassesMPEFOC" Style="display: none;" />
                <ajaxToolkit:ModalPopupExtender ID="MPEFOCGlasses" runat="server"
                    PopupControlID="PanelAddGlassesFOC" TargetControlID="btnOpenGlassesMPEFOC"
                    BackgroundCssClass="Background">
                </ajaxToolkit:ModalPopupExtender>

                <asp:Panel runat="server">
                    <asp:UpdatePanel runat="server" ID="PanelAddGlassesFOC" UpdateMode="Conditional" Style="display: none;">
                        
                        <ContentTemplate>
                            <div class="modal" style="float: left;">
                            <asp:LinkButton runat="server" ID="lnkbtnCloseFOCGlassesPanel" OnClick="btnCloseFOCGlasses_Click" CausesValidation="false" CssClass="close__button">Close <span class="close__button__circle">X</span></asp:LinkButton>
                                <br />
                                <h2 class="modal__title">Select Frames:</h2>

                                <div style="width:50%;max-width:660px; float:left;">
                                        <p>Please upload a picture of your prescription.  Ensure your name, the provider's name and address, date of prescription and provider signature are clear.</p>
                                        <img id="previewImageFOC" src=""/>
                                        <input id="showFOCPic" type="file" accept="image/*" onchange="addPrescriptionPhoto(event)"/>
                                        <asp:HiddenField runat="server" ID="hfieldPrescriptionImageSelectedFOC" Value="na" />
                                        <asp:HiddenField runat="server" ID="hfieldPrescriptionImageNameFOC" Value="na" />
                                        <div id="divCenterFOC" runat="server" class="divhead">
                                            
                                        </div>
                                    </div>
                                <br />
                                <div style="width:50%;float:left;">
                                    <h2 class="modal__title">Select Frames:</h2>
                                    <div class="modal__glasses-form">
                                        <ul class="frames">
                                            <asp:Repeater ID="jspecsFramesFOC" runat="server">
                                                <ItemTemplate>
                                                    <li class="frame">
                                                        <div class="frame_details">
                                                            <asp:Image ID="frameImgFOC" runat="server" ImageUrl='<%# processFrameImage(Eval("ImageURL")) %>' class="order__frame-img"/>
                                                        
                                                            <h3 class="frame_name"><%# Eval("DisplayField") %></h3>
                                                            <asp:LinkButton runat="server" ID="btnAddProtectedMaskInsert" OnClick="btnAddProtectedMaskInsert" CommandArgument='<%# Eval("FrameCode") + ";" + Eval("DisplayField") %>' CausesValidation="false" class="frame__button btn btn--heavy" Text="Add Insert"></asp:LinkButton>
                                                        </div>
                                                    </li>
                                                </ItemTemplate>
                                            </asp:Repeater>
                                        </ul>
                                    </div>
                                 </div>
                            </div>


                        </ContentTemplate>
                    </asp:UpdatePanel>
                </asp:Panel>
                    -->

                <%-- Glasses pop up modal--%>
                <asp:Button runat="server" ID="btnOpenGlassesMPEDummy" Style="display: none;" />
                <ajaxToolkit:ModalPopupExtender ID="MPEAddGlasses" runat="server"
                    PopupControlID="PanelAddGlasses" TargetControlID="btnOpenGlassesMPEDummy"
                    BackgroundCssClass="Background">
                </ajaxToolkit:ModalPopupExtender>

                <asp:Panel runat="server">
                    <asp:UpdatePanel runat="server" ID="PanelAddGlasses" UpdateMode="Conditional" Style="display: none;">
                        
                        <ContentTemplate>
                            <div class="modal" style="float:left;">
                                <asp:LinkButton runat="server" ID="lnkbtnCloseAddGlassesPanel" OnClick="btnCloseAddGlasses_Click" CausesValidation="false" CssClass="close__button">Close <span class="close__button__circle">X</span></asp:LinkButton>
                                <br />
                                <h2 class="modal__title">Select Frames:</h2>
                                <div class="modal__glasses-form">
                                    <ul class="frames">
                                        <asp:Repeater ID="jspecsFrames" runat="server">
                                            <ItemTemplate>
                                                <li class="frame">
                                                    <div class="frame_details">
                                                        <asp:Image ID="frameImg" runat="server" ImageUrl='<%# processFrameImage(Eval("ImageURL")) %>' class="order__frame-img"/>
                                                        
                                                        <h3 class="frame_name"><%# Eval("DisplayField") %></h3>
                                                        <asp:LinkButton runat="server" ID="btnAddProtectedMaskInsert" OnClick="btnAddProtectedMaskInsert" CommandArgument='<%# Eval("FrameCode") + ";" + Eval("DisplayField") %>' CausesValidation="false" class="frame__button btn btn--heavy" Text="Add Insert"></asp:LinkButton>
                                                    </div>
                                                </li>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </ul>
                                </div>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </asp:Panel>
                <%-- Glasses pop up modal end--%>
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
            <%-- <asp:LinkButton runat="server" ID="lbtnCancel" ClientIDMode="Static" OnClick="btnCancel_Click" class="btn">Cancel</asp:LinkButton>
<%--        </nav>--%>

    </article>
    <!--End Of Content-->

    <script src="../../../Scripts/JSpecs/JSpecsNewOrderValidation.js"></script>
</asp:Content>