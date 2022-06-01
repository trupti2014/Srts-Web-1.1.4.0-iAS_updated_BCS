using SrtsWeb.Base;
using SrtsWeb.Entities;
using SrtsWeb.ExtendersHelpers;
using SrtsWeb.Presenters.JSpecs;
using SrtsWeb.Views.JSpecs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace JSpecs.Forms
{
    public partial class JSpecsNewOrder : PageBaseJSpecs, IJSpecsNewOrderView
    {
        private JSpecsNewOrderPresenter _presenter;

        public JSpecsNewOrder()
        {
            _presenter = new JSpecsNewOrderPresenter(this);
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["userInfo"] != null)
            {
                if (!IsPostBack)
                {
                    if (userInfo.JSpecsOrder != null)
                    {
                        string orderId = userInfo.JSpecsOrder.OrderID;

                        if (!orderId.IsNullOrEmpty())
                        {
                            _presenter.GetPatientsOrderByOrderId(orderId);

                            if (userInfo.JSpecsOrder.PatientOrder.IsNull())
                            {
                                // Add alert message saying cannot find orderid
                                Response.Redirect("~/WebForms/JSpecs/Forms/JSpecsOrders.aspx");
                            }
                        }
                        else
                        {
                            
                            // Currently this is where starting a new order would go, and not supported in v1.0
                            // Response.Redirect("~/WebForms/JSpecs/Forms/JSpecsOrders.aspx");
                        }
                    }

                     _presenter.InitView();

                    try
                    {
                        lbtnSubmitDetails.Text = userInfo.JSpecsOrder.IsReOrder ? "All of my details are correct" : "Select Glasses";
                    }
                    catch
                    {
                        lbtnSubmitDetails.Text = "Select Glasses";
                    }

                    // Prevent buttons from being double clicked, and adding duplicate records
                    // This code is causing bugs with making events fire twice
                    // lbtnUSPSAddAddress.Attributes.Add("onclick", " this.disabled = true; " + ClientScript.GetPostBackEventReference(lbtnUSPSAddAddress, null) + ";");
                    // lbtnUserAddAddress.Attributes.Add("onclick", " this.disabled = true; " + ClientScript.GetPostBackEventReference(lbtnUserAddAddress, null) + ";");
                    //lbtnAddEmail.Attributes.Add("onclick", " this.disabled = true; " + ClientScript.GetPostBackEventReference(lbtnAddEmail, null) + ";");
                }
            }
            else
            {
                Response.Redirect("~/WebForms/JSpecs/Forms/JSpecsLogin.aspx");
            }
        }

        /// <summary>
        /// Submit address, prescription, and email for order
        /// and redirect to next part of the order.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSubmitDetails_Click(object sender, EventArgs e)
        {
            var test = userInfo; 
            string[] frameData = hfieldFramesSelected.Value.Split(';');
            if (userShippingAddresses.SelectedIndex != 0 && userPrescriptions.SelectedIndex != 0 && userEmailAddresses.SelectedIndex != 0 && userGlassesSelection.SelectedIndex != 0)
            {
                // if (userGlassesSelection.SelectedIndex == 3)
                if (userGlassesSelection.SelectedIndex == 2 || userGlassesSelection.SelectedIndex == 4)
                {
                    if (hfieldFramesSelected.Value != "UPLC;REVISION UPLC INSERT" && hfieldFramesSelected.Value != "")
                    {
                        userInfo.JSpecsOrder = _presenter.NewJSpecsOrder();
                        userInfo.JSpecsOrder.PatientOrder = _presenter.NewOrderEntity();
                        userInfo.JSpecsOrder.PatientOrder.FrameCode = frameData[0];
                        userInfo.JSpecsOrder.PatientOrder.FrameDescription = frameData[1];
                        userInfo.JSpecsOrder.PatientOrder.IndividualID_Patient = userInfo.Patient.Individual.ID.ToInt32();
                        userInfo.JSpecsOrder.PatientOrder.IndividualID_Tech = userInfo.Patient.Individual.ID.ToInt32();
                        userInfo.JSpecsOrder.OrderAddress = userInfo.Patient.Addresses.Find(address => address.ID.ToString() == userShippingAddresses.SelectedValue);
                        userInfo.JSpecsOrder.OrderPrescription = userInfo.Patient.Prescriptions.Find(prescription => prescription.ID.ToString() == userPrescriptions.SelectedValue);
                        userInfo.JSpecsOrder.OrderEmailAddress = userInfo.Patient.EMailAddresses.Find(emailAddress => emailAddress.ID.ToString() == userEmailAddresses.SelectedValue);
                        Response.Redirect("~/WebForms/JSpecs/Forms/JSpecsConfirmOrder.aspx");
                    }
                }
                else //if (userGlassesSelection.SelectedIndex ==1)
                {
                    userInfo.JSpecsOrder = _presenter.NewJSpecsOrder();
                    userInfo.JSpecsOrder.PatientOrder = _presenter.NewOrderEntity();
                    userInfo.JSpecsOrder.PatientOrder.FrameCode = frameData[0];
                    userInfo.JSpecsOrder.PatientOrder.FrameDescription = frameData[1];
                    userInfo.JSpecsOrder.PatientOrder.IndividualID_Patient = userInfo.Patient.Individual.ID.ToInt32();
                    userInfo.JSpecsOrder.PatientOrder.IndividualID_Tech = userInfo.Patient.Individual.ID.ToInt32();
                    userInfo.JSpecsOrder.OrderAddress = userInfo.Patient.Addresses.Find(address => address.ID.ToString() == userShippingAddresses.SelectedValue);
                    userInfo.JSpecsOrder.OrderPrescription = userInfo.Patient.Prescriptions.Find(prescription => prescription.ID.ToString() == userPrescriptions.SelectedValue);
                    userInfo.JSpecsOrder.OrderEmailAddress = userInfo.Patient.EMailAddresses.Find(emailAddress => emailAddress.ID.ToString() == userEmailAddresses.SelectedValue);
                    Response.Redirect("~/WebForms/JSpecs/Forms/JSpecsConfirmOrder.aspx");
                }
                
            }
            else
            {
                // Error message saying they have to select an option that isn't --select--
            }
        }

        #region Glasses modal

        /// <summary>
        /// This function will add the glasses if it is possible,
        /// and display the appropriate validation errors.
        /// </summary>
        /// <param name="sender">Add glasses selection</param>
        /// <param name="e"></param>
        protected void AddGlasses(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// When user input on glasses make sure those glasses are orderable.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void inputGlasses_Changed(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Display the Add Glasses Modal.
        /// </summary>
        /// <param name="sender">Button to open add glasses</param>
        /// <param name="e"></param>
        protected void btnDisplayAddGlasses_click(object sender, EventArgs e)
        {
            if (userGlassesSelection.SelectedIndex != 1 && userGlassesSelection.SelectedIndex != 0)
            {
                _presenter.GetAllFrames();
                MPEAddGlasses.Show();
            }
            /*
            if (userGlassesSelection.SelectedIndex == 3)
            {
                _presenter.GetAllFrames();
                MPEAddGlasses.Show();
            }
            else if(userGlassesSelection.SelectedIndex == 2)
            {
                _presenter.GetAllFrames();
                MPEFOCGlasses.Show();
            }
            else if (userGlassesSelection.SelectedIndex == 1)
            {
                 _presenter.GetAllFrames();
                MPEAddGlasses.Show();
            }
              */
        }
        /// <summary>
        /// Close the add Glasses Modal
        /// </summary>
        /// <param name="sender">Close button on Add glasses modal</param>
        /// <param name="e"></param>
        protected void btnCloseAddGlasses_Click(object sender, EventArgs e)
        {
            MPEAddGlasses.Hide();
        }

        /// <summary>
        
            /*
        /// Close the add Glasses Modal
        /// </summary>
        /// <param name="sender">Close button on Add glasses modal</param>
        /// <param name="e"></param>
        protected void btnCloseFOCGlasses_Click(object sender, EventArgs e)
        {
            MPEFOCGlasses.Hide();
        }

        */

        /// <summary>
        /// Add selected frames
        /// </summary>
        protected void btnAddProtectedMaskInsert(object sender, EventArgs e)
        {
            var frameData = (sender as LinkButton).CommandArgument.ToString();
            string[] frameDataSplt = frameData.Split(';');
            hfieldFramesSelected.Value = frameData;
            labelFieldFrameDescription.Text = "Selected Frame: " + frameDataSplt[1];
            MPEAddGlasses.Hide();
        }

        protected string processFrameImage(object imgURL)
        {
            if (imgURL == null || imgURL.ToString() == "" )
            {
                return "/JSpecs/imgs/Fallback/Frame_Not_Available.png";
            }
            return imgURL.ToString();
        }
        /// <summary>
        /// Reset the add glasses modal fields
        /// </summary>
        protected void ResetAddGlassesFields()
        {

        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/WebForms/JSpecs/Forms/JSpecsOrders.aspx");
        }
        #endregion Glasses modal

        #region Email Address modal
        /// <summary>
        /// This function will add an email address if the email is valid,
        /// and display the appropriate validation errors.
        /// </summary>
        /// <param name="sender">Add email address button</param>
        /// <param name="e"></param>
        protected void AddEmailAddress(object sender, EventArgs e)
        {
            string email, confirmEmail;
            email = inputEmail.Text;
            confirmEmail = inputConfirmEmail.Value;

            Page.Validate("EmailValidationGroup");

            if (Page.IsValid)
            {
                if (!_presenter.PatientHasEmailAddress(userInfo.Patient.Individual.ID.ToInt32(), email))
                {
                    EMailAddressEntity emailAddress = new EMailAddressEntity();
                    emailAddress.IndividualID = userInfo.Patient.Individual.ID;
                    emailAddress.EMailAddress = email;

                    // insert new email
                    _presenter.InsertNewEmailAddress(emailAddress);


                    // update view
                    ScriptManager.RegisterStartupScript(this, GetType(), "ValidateCntinue", "ValidateContinueOrderBtn();", true);
                    UpdatePanelEmailSection.Update();
                    ResetAddEmailFields();
                    userEmailAddresses.SelectedIndex = _userEmailAddresses.Count;

                }
                else
                {
                    // Message saying patient already has email address
                    fvCheckInputEmailAddress.Visible = true;
                }
            }
        }

        /// <summary>
        /// When user input on email changes do a database check if email exists.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void inputEmail_Changed(object sender, EventArgs e)
        {
            if (inputEmail.Text != "" && _presenter.PatientHasEmailAddress(userInfo.Patient.Individual.ID.ToInt32(), inputEmail.Text))
            {
                fvCheckInputEmailAddress.Visible = true;
            }
            else
            {
                Page.Validate("EmailValidationGroup");
                fvCheckInputEmailAddress.Visible = false;
            }

        }

        /// <summary>
        /// Display the Add Email Modal.
        /// </summary>
        /// <param name="sender">Button to open add email</param>
        /// <param name="e"></param>
        protected void btnDisplayAddEmail_click(object sender, EventArgs e)
        {
            MPEAddEmail.Show();
        }

        /// <summary>
        /// Close the add Email Modal
        /// </summary>
        /// <param name="sender">Close button on Add email modal</param>
        /// <param name="e"></param>
        protected void btnCloseAddEmail_Click(object sender, EventArgs e)
        {
            ResetAddEmailFields();
            MPEAddEmail.Hide();
        }

        /// <summary>
        /// Reset the add email modal fields
        /// </summary>
        protected void ResetAddEmailFields()
        {
            inputEmail.Text = string.Empty;
            inputConfirmEmail.Value = string.Empty;
            fvCheckInputEmailAddress.Visible = false;
        }
        #endregion Email Address modal

        #region Address modal

        /// <summary>
        /// Verify user address, and update modal with results
        /// </summary>
        /// <param name="snder">A object from the button clicked</param>
        /// <param name="e"></param>
        protected void btnVerifyAddress_click(object snder, EventArgs e)
        {
            AddressEntity address = new AddressEntity();
            address.Address1 = inputStreetAddress.Value;
            address.Address2 = inputStreetAddress2.Value;
            address.City = inputCity.Value;
            address.State = stateInput.SelectedValue;
            address.Country = countryInput.SelectedValue;
            address.ZipCode = inputZipcode.Value;

            // check to see if the address already exists
            if (_presenter.PatientHasAddress(userInfo.Patient.Individual.ID.ToInt32(), address))
            {
                fvCheckAddressExists.Visible = true;
            }
            else
            {
                //clear address already exists message if it is showing.
                clearValidation();
                // Get Usps recommended address
                AddressEntity recommendedAddress = _presenter.GetUSPSRecommendedAddress(address);

                // Store address in session
                Session["UserEnteredAddress"] = address;
                Session["USPSRecommenedAddress"] = recommendedAddress;

                // Reset side results, and set new results
                ResetAddAddressModalResults();
                SetUserAddressResultFields(address);
                SetUserUSPSAddressResultFields(recommendedAddress);
            }
        }

        protected void btnAddAddress_Click(object sender, EventArgs e)
        {
            LinkButton button = (LinkButton)sender;
            string buttonId = button.ID;
            AddressEntity address = null;


            if (buttonId == "lbtnUserAddAddress")
            {
                address = (AddressEntity)Session["UserEnteredAddress"];
            }
            else if (buttonId == "lbtnUSPSAddAddress")
            {
                address = (AddressEntity)Session["USPSRecommenedAddress"];
            }

            if (address != null)
            {
                //insert a new address
                _presenter.InsertNewAddress(address);
                ScriptManager.RegisterStartupScript(this, GetType(), "ValidateContinue", "ValidateContinueOrderBtn();", true);
                UpdatePanelAddressSection.Update();
                userShippingAddresses.SelectedIndex = _userAddresses.Count;
                CloseAddAddressModal();
            }
        }

        private void SetUserAddressResultFields(AddressEntity address)
        {
            userEnteredAddressResultContainer.Visible = true;
            userAddress.InnerText = address.Address1;
            userAddress2.InnerText = address.Address2;
            userAddress3.InnerText = address.City + ", " + address.State + " " + address.ZipCode;
        }

        private void SetUserUSPSAddressResultFields(AddressEntity uspsRecommendedaddress)
        {
            uspsAddressResultContainer.Visible = true;
            if (uspsRecommendedaddress.ZipCode != "-")
            {
                uspsAddress.InnerText = uspsRecommendedaddress.Address1 + " " + uspsRecommendedaddress.Address2;
                uspsAddress2.InnerText = uspsRecommendedaddress.City + ", " + uspsRecommendedaddress.State + " " + uspsRecommendedaddress.ZipCode;
                lbtnUSPSAddAddress.Visible = true;
            }
            else
            {
                uspsAddress.InnerText = "Address not found.";
                lbtnUSPSAddAddress.Visible = false;
            }
        }

        /// <summary>
        /// Remove validation from address modal.
        /// </summary>
        protected void clearValidation()
        {
            //clear address already exists message if it is showing.
            if (fvCheckAddressExists.Visible)
            {
                fvCheckAddressExists.Visible = false;
            }
        }

        /// <summary>
        /// Display the add address modal.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnDisplayAddAddress_Click(object sender, EventArgs e)
        {
            clearValidation();
            MPEAddAddress.Show();
        }

        /// <summary>
        /// Close the add address modal
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnCloseAddAddress_Click(object sender, EventArgs e)
        {
            CloseAddAddressModal();
        }

        /// <summary>
        /// Clear and close add address modal
        /// </summary>
        protected void CloseAddAddressModal()
        {
            MPEAddAddress.Hide();
            ResetAddAddressModalInput();
            ResetAddAddressModalResults();
        }

        /// <summary>
        /// Reset the add email modal fields, and results
        /// </summary>
        private void ResetAddAddressModalInput()
        {
            inputStreetAddress.Value = string.Empty;
            inputStreetAddress2.Value = string.Empty;
            inputCity.Value = string.Empty;
            inputZipcode.Value = string.Empty;
            stateInput.SelectedValue = "--Select State--";
            countryInput.SelectedValue = "US";
            Session.Remove("UserEnteredAddress");
            Session.Remove("USPSRecommenedAddress");
        }

        /// <summary>
        /// Reset the Add address modal result fields.
        /// </summary>
        private void ResetAddAddressModalResults()
        {
            userEnteredAddressResultContainer.Visible = false;
            userAddress.InnerText = string.Empty;
            userAddress2.InnerText = string.Empty;

            uspsAddressResultContainer.Visible = false;
            uspsAddress.InnerText = string.Empty;
            uspsAddress2.InnerText = string.Empty;

        }
        #endregion Address Modal

        #region Prescription modal

        #endregion Prescription Modal

        #region Data Filters
        public bool CheckPrescriptionAgainstFrameRxRestrictions(PrescriptionEntity prescription, FrameRxRestrictionsEntity frameRestrictions)
        {
            return prescription.ODSphere.Between(frameRestrictions.MaxSphere, frameRestrictions.MinSphere)
                && prescription.OSSphere.Between(frameRestrictions.MaxSphere, frameRestrictions.MinSphere)
                && prescription.ODCylinder.Between(frameRestrictions.MaxCylinder, frameRestrictions.MinCylinder)
                && prescription.OSCylinder.Between(frameRestrictions.MaxCylinder, frameRestrictions.MinCylinder);
        }
        #endregion
        #region JSpecsDetailsView members
        public JSpecsSession userInfo
        {
            get { return (JSpecsSession)Session["userInfo"]; }
            set { Session.Add("userInfo", value); }
        }

        private List<AddressEntity> _userAddresses;
        public List<AddressEntity> UserAddresses
        {
            get { return _userAddresses; }
            set
            {
                _userAddresses = value;
                userInfo.Patient.Addresses = _userAddresses;
                userShippingAddresses.DataSource = _userAddresses;
                userShippingAddresses.DataTextField = "Address1";
                userShippingAddresses.DataValueField = "ID";
                userShippingAddresses.DataBind();
                userShippingAddresses.Items.Insert(0, new ListItem("--Select Address--", "0"));

                // Default to select address
                try
                {
                    userShippingAddresses.SelectedValue = userInfo.JSpecsOrder.OrderAddress.IsNull() ? "0" : userInfo.JSpecsOrder.OrderAddress.ID.ToString();
                }
                catch
                {
                    userShippingAddresses.SelectedValue = "0";
                }
            }
        }

        private List<PrescriptionEntity> _userPrescriptions;
        public List<PrescriptionEntity> UserPrescriptions
        {
            get { return _userPrescriptions; }
            set
            {
                _userPrescriptions = value;
                
                var datasource = from prescription in _userPrescriptions
                                    select new
                                    {
                                        prescription.ID,
                                        DisplayField = string.Concat(prescription.PrescriptionDate.Date.ToString("d"), ", ", prescription.RxName.ToRxUserFriendlyName())
                                    };

                userPrescriptions.DataSource = datasource;
                userPrescriptions.DataTextField = "DisplayField";
                userPrescriptions.DataValueField = "ID";
                userPrescriptions.DataBind();
                userPrescriptions.Items.Insert(0, new ListItem("--Select Prescription--", "0"));

                // Default to original prescription 
                // userPrescriptions.SelectedValue = userInfo.JSpecsOrder.OrderPrescription.IsNull() ? userInfo.JSpecsOrder.PatientOrder.PrescriptionID.ToString() : //userInfo.JSpecsOrder.OrderPrescription.ID.ToString();
                
            }
        }

        private List<FrameEntity> _userFrames;

        public List<FrameEntity> UserFrames
        {
            get { return _userFrames; }
            set
            {
                _userFrames = value;
                jspecsFrames.DataSource = from userFrame in _userFrames
                                          where userFrame.IsInsert && userFrame.IsActive && userFrame.FrameCode != "UPLC"
                                          select new
                                          {
                                              FrameCode = userFrame.FrameCode,
                                              DisplayField = userFrame.FrameDescription,
                                              ImageUrl = File.Exists(Server.MapPath("/JSpecs/imgs/" + userFrame.FrameCode + ".jpg")) ? "~/Jspecs/imgs/" + userFrame.FrameCode + ".jpg" : ""
                                          };

                jspecsFrames.DataBind();

                /*
                jspecsFramesFOC.DataSource = from userFrames in _userFrames
                                             where userFrames.IsInsert && userFrames.IsActive && userFrames.FrameCode != "UPLC"
                                             select new
                                             {
                                                 FrameCode = userFrames.FrameCode,
                                                 DisplayField = userFrames.FrameDescription,
                                                 ImageUrl = File.Exists(Server.MapPath("/JSpecs/imgs/" + userFrames.FrameCode + ".jpg")) ? "~/Jspecs/imgs/" + userFrames.FrameCode + ".jpg" : ""
                                             };

                jspecsFramesFOC.DataBind();
            } */

            }
        }

        /*
        private List<FrameEntity> _userFramesFOC;

        public List<FrameEntity> UserFramesFOC
        {
            get { return _userFrames;  }
            set
            {
                _userFrames = value;
                jspecsFramesFOC.DataSource = from userFrames in _userFrames
                                 where userFrames.IsInsert && userFrames.IsActive && userFrames.FrameCode != "UPLC"
                                 select new
                                 {
                                     FrameCode = userFrames.FrameCode,
                                     DisplayField = userFrames.FrameDescription,
                                     ImageUrl = File.Exists(Server.MapPath("/JSpecs/imgs/"+ userFrames.FrameCode + ".jpg")) ? "~/Jspecs/imgs/" + userFrames.FrameCode + ".jpg" : "" 
                                 };

                jspecsFramesFOC.DataBind();
            }
                
        }
        */

        private List<EMailAddressEntity> _userEmailAddresses;

        public List<EMailAddressEntity> UserEmailAddresses
        {
            get { return _userEmailAddresses; }
            set
            {
                _userEmailAddresses = value;
                userInfo.Patient.EMailAddresses = _userEmailAddresses;
                userEmailAddresses.DataSource = _userEmailAddresses;
                userEmailAddresses.DataTextField = "EMailAddress";
                userEmailAddresses.DataValueField = "ID";
                userEmailAddresses.DataBind();
                userEmailAddresses.Items.Insert(0, new ListItem("--Select Email--", "0"));

                // Default to select email
                try
                {
                    userEmailAddresses.SelectedValue = userInfo.JSpecsOrder.OrderEmailAddress.IsNull() ? "0" : userInfo.JSpecsOrder.OrderEmailAddress.ID.ToString();
                }
                catch
                {
                    userEmailAddresses.SelectedValue = "0";
                }
            }
        }

        private List<LookupTableEntity> _countryData;

        public List<LookupTableEntity> CountryData
        {
            get { return _countryData; }
            set
            {
                _countryData = value;
                countryInput.DataSource = _countryData;
                countryInput.DataTextField = "Text";
                countryInput.DataValueField = "Value";
                countryInput.DataBind();
                countryInput.Items.Insert(0, "--Select Country--");
                countryInput.SelectedValue = "US";
            }
        }

        private List<LookupTableEntity> _stateData;

        public List<LookupTableEntity> StateData
        {
            get { return _stateData; }
            set
            {
                _stateData = value;
                stateInput.DataSource = _stateData;
                stateInput.DataTextField = "Text";
                stateInput.DataValueField = "value";
                stateInput.DataBind();
                stateInput.Items.Insert(0, "--Select State--");
            }
        }

        public string ErrorMessage
        {
            set
            {
                ShowMessage_Redirect(this.Page, value, "/WebForms/JSpecs/Forms/JSpecsOrders.aspx");
            }
        }
        #endregion JSpecsDetailsView members
    }
}