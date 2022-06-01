using SrtsWeb.Base;
using SrtsWeb.Entities;
using SrtsWeb.ExtendersHelpers;
using SrtsWeb.Views.JSpecs;
using SrtsWeb.Presenters.JSpecs;
using System;

namespace SrtsWeb.WebForms.JSpecs.Forms
{
    public partial class JSpecsConfirmOrder : PageBaseJSpecs, IJSpecsConfirmOrderView
    {
        private JSpecsConfirmOrderPresenter _presenter;

        public JSpecsConfirmOrder()
        {
            _presenter = new JSpecsConfirmOrderPresenter(this);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["userInfo"] != null && userInfo.JSpecsOrder != null)
            {
                if (!IsPostBack)
                {
                    _presenter.InitView();
                    InitView();

                    // Prevent buttons from being double clicked, and adding duplicate records
                    lbtnSubmitOrder.Attributes.Add("onclick", " this.disabled = true; " + ClientScript.GetPostBackEventReference(lbtnSubmitOrder, null) + ";");
                }
            }
            else
            {
                // Response.Redirect("~/WebForms/JSpecs/Forms/JSpecsLogin.aspx");
                Response.Redirect("~/WebForms/JSpecs/Forms/JSpecsOrders.aspx");
            }
        }

        public void InitView()
        {

            frameDetails.InnerText = userInfo.JSpecsOrder.PatientOrder.FrameDescription;
            try
            {
                frameImg.Src = _presenter.GetFrameImage(userInfo.JSpecsOrder.PatientOrder.FrameFamily, userInfo.JSpecsOrder.PatientOrder.FrameColor);
                if (frameImg.Src == "/JSpecs/imgs/Fallback/glasses.svg")
                {
                    frameImg.Src = "/JSpecs/imgs/Fallback/Frame_Not_Available.png";
                }
            }
            catch(NullReferenceException nre)
            {
                frameImg.Src = "/JSpecs/imgs/Fallback/Frame_Not_Available.png";
            }

            orderAddress.Value = userInfo.JSpecsOrder.OrderAddress.Address1 + ", " + userInfo.JSpecsOrder.OrderAddress.Address2;
            orderPrescription.Value = userInfo.JSpecsOrder.OrderPrescription.PrescriptionDate.ToString("MM/dd/yyyy") + ", " + userInfo.JSpecsOrder.OrderPrescription.RxName.ToRxUserFriendlyName();
            orderEmailAddress.Value = userInfo.JSpecsOrder.OrderEmailAddress.EMailAddress;
            SetUserAgreement();
        }

        public void SetUserAgreement()
        {
            string userAgreement = string.Empty;

            if (userInfo.Patient.Individual.StatusDescription == "Active Duty")
            {
                if(userInfo.JSpecsOrder.PatientOrder.FrameCategory != "PMI" && userInfo.JSpecsOrder.PatientOrder.FrameCategory != "Standard")  //standard check added by Lucy 3/19/2020
                {
                    userAgreement = @"I understand I only get one Frame of Choice (FOC) per year. This order will be
                        my one Frame of Choice.I understand that I cannot order another Frame of
                        Choice in the next 12 months, even if I lose or break these glasses, the
                        prescription is wrong, my prescription changes, or they don’t fit right.
                        By clicking “Submit Order”
                        , I acknowledge accepting these terms. I
                        understand that if I wish to have a fitting done, I could go to my Optometry
                        Clinic and order through them.";
                }
                else
                {
                    // In the case it is not a FOC there is no need for a user agreement.
                    inputUserAgreementCheckbox.Checked = true;
                    confirmOrderAgreement.Attributes.Add("style", "visibility: hidden");
                }
            }
            else if (userInfo.Patient.Individual.StatusDescription == "Retired")
            {
                userAgreement = @"I understand I only get one pair of glasses per year. This order will be my pair of glasses.
                    I understand that I cannot order another Frame of Choice in the next 12 months,
                    even if I lose or break these glasses, the prescription is wrong, my prescription changes,
                    or they don't fit right. I affirm that I only have a currect prescription and
                    need no fitting assistance. By clicking “Submit Order”, I acknowledge accepthing these terms.";
            }
            else
            {
                ErrorMessage = @"We\'re sorry. You are not applicable to order from our application.";
            }

            pUserAgreement.InnerText = userAgreement;
        }

        protected void SubmitOrder(object sender, EventArgs e)
        {

           
            if (_presenter.SubmitOrder())
            {
                LogEvent(String.Format("Submit Order is true: successfully logged on at {0}.",
                 DateTime.Now));
                // Console.WriteLine("Address Used on Reorder: " + orderAddress.Value + " " + DateTime.Now);
                Response.Redirect("/WebForms/JSpecs/Forms/JSpecsOrders.aspx");
            }
            else
            {
                LogEvent(String.Format("Submit Order is false: unsuccessfully logged on at {0}.",
                 DateTime.Now));
                ErrorMessage = "Error submitting order.";
            }

        }

        protected void btnEditInformation_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/WebForms/JSpecs/Forms/JSpecsNewOrder.aspx");
        }

        public JSpecsSession userInfo
        {
            get { return (JSpecsSession)Session["userInfo"]; }
            set { Session.Add("userInfo", value); }
        }

        public string ErrorMessage
        {
            set
            {
                ShowMessage_Redirect(this.Page, value, "/WebForms/JSpecs/Forms/JSpecsOrders.aspx");
            }
        }
    }
}