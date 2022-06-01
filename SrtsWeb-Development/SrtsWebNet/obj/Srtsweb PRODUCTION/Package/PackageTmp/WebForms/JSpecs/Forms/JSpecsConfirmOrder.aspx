<%@ Page Language="C#" MasterPageFile="~/JSpecs/JSpecsMaster.Master" AutoEventWireup="true"
    CodeBehind="JSpecsConfirmOrder.aspx.cs" Inherits="SrtsWeb.WebForms.JSpecs.Forms.JSpecsConfirmOrder" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="Server">

    <script src="../../../Scripts/JSpecs/JSpecsValidateOrder.js"></script>

    <div class="title title--border title--margin-bottom">
        <h1 class="title__name">Confirm Your Order</h1>
    </div>

    <section class="confirm-order">
        <article class="confirm-order__form">
            <div class="confirm-order__form__row">
                <label>Selected Device:</label>
                <div class="confirm-order__form__row__cell">
                    <p runat="server" id="frameDetails"></p>
                    <img runat="server" id="frameImg" src="/JSpecs/svg/glasses.svg" />
                    <a></a>
                </div>
            </div>

            <div class="confirm-order__form__row">
                <label>Shipping Address:</label>
                <div class="confirm-order__form__row__cell">
                    <input runat="server" id="orderAddress" style="width: 100%" ClientIDMode="Static" readonly />
                </div>
            </div>
            <div class="confirm-order__form__row">
                <label>Prescription:</label>
                <div class="confirm-order__form__row__cell">
                    <input runat="server" id="orderPrescription" style="width: 100%" ClientIDMode="Static" readonly />
                </div>
            </div>
            <div class="confirm-order__form__row">
                <label>Email Address:</label>
                <div class="confirm-order__form__row__cell">
                    <input runat="server" id="orderEmailAddress" style="width: 100%" ClientIDMode="Static" readonly />
                </div>
            </div>

            <div style="text-align: center">
                <asp:LinkButton runat="server" id="lbtnVerifyInformation" ClientIDMode="Static" onClientClick="return false;" CssClass="btn" Height="100%" Text="Verify Information" />
                <asp:LinkButton runat="server" CssClass="confirm-order-edit" OnClick="btnEditInformation_Click" Height="100%" Text="EDIT Information" />
            </div>
        </article>

       
        <article runat="server" id="confirmOrderAgreement" class="confirm-order__agreement">
            <div>
                <span>Required</span>
                <input id="inputUserAgreementCheckbox" runat="server" ClientIdMode="Static" type="checkbox" />
            </div>
            <div class="confirm-order__agreement__text">
                <p runat="server" id="pUserAgreement">
                </p>
            </div>
        </article>
        <asp:LinkButton runat="server" ID="lbtnSubmitOrder" ClientIDMode="Static" OnClick="SubmitOrder" disabled="true" class="btn btn--medium btn--margin-center">Submit Order</asp:LinkButton>
    </section>
</asp:Content>
