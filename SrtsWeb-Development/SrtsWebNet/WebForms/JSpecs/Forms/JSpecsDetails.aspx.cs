using SrtsWeb.Base;
using SrtsWeb.Entities;
using SrtsWeb.ExtendersHelpers;
using SrtsWeb.Presenters.JSpecs;
using SrtsWeb.Views.JSpecs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
namespace JSpecs.Forms
{
    public partial class JSpecsDetails : PageBaseJSpecs, IJSpecsDetailsView
    {
        private JSpecsDetailsPresenter _presenter;

        public JSpecsDetails()
        {
            _presenter = new JSpecsDetailsPresenter(this);
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

                        /*
                        sphereErr.ToolTip = "Sphere can only be ....";
                        cylinderErr.ToolTip = "Cylinder must be ...";
                        axisErr.ToolTip = "ToolTIp for Axis Error ...";
                        addErr.ToolTip = "Add Error Tool Tip, making this one a bit longer for test....";
                        */

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
                            Response.Redirect("~/WebForms/JSpecs/Forms/JSpecsOrders.aspx");
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
            if (userShippingAddresses.SelectedIndex != 0 && userPrescriptions.SelectedIndex != 0 && userEmailAddresses.SelectedIndex != 0)
            {
                userInfo.JSpecsOrder.OrderAddress = userInfo.Patient.Addresses.Find(address => address.ID.ToString() == userShippingAddresses.SelectedValue);
                userInfo.JSpecsOrder.OrderPrescription = userInfo.Patient.Prescriptions.Find(prescription => prescription.ID.ToString() == userPrescriptions.SelectedValue);
                userInfo.JSpecsOrder.OrderEmailAddress = userInfo.Patient.EMailAddresses.Find(emailAddress => emailAddress.ID.ToString() == userEmailAddresses.SelectedValue);
                Response.Redirect("~/WebForms/JSpecs/Forms/JSpecsConfirmOrder.aspx");
            }
            else
            {
                // Error message saying they have to select an option that isn't --select--
            }
        }

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
            //clear address already exists message if it is showing. jkl jk
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
        protected void addPrescriptionDropDownValues()
        {
            // sphere
            for ( int i = 10; i >= -10; i--)
            {
                string itrSphere = (i > 0) ? "+" + i.ToString() : i.ToString();
                ListItem li = new ListItem(itrSphere, itrSphere);
                rightSphereDD.Items.Add(li);
                leftSphereDD.Items.Add(li);
            }
            // cylinder
            for (int i = 6; i >= -6; i--)
            {
                string itrCyl = (i > 0) ? "+" + i.ToString() : i.ToString();
                ListItem li = new ListItem(itrCyl, itrCyl);
                rightCylinderDD.Items.Add(li);
                leftCylinderDD.Items.Add(li);
            }
            // axis
            for (int i = 1; i <= 180; i++)
            {
                string itrAxis = (i < 10) ? "00" + i.ToString() : (i < 100) ? "0" + i.ToString() : i.ToString();
                ListItem li = new ListItem(itrAxis, itrAxis);
                rightAxisDD.Items.Add(li);
                leftAxisDD.Items.Add(li);
            }
            // add
            for(double i = 0.75; i<= 4.00; i += .25)
            {
                // add some logic to make sure string is three characters i.e 1.50 instead of 1.5, also add + sign in front.
                string initial = i.ToString();
                string itrAdd = (initial.Length == 1) ? initial + ".00" : (initial.Length == 3 ) ? initial + "0" : initial;
                ListItem li = new ListItem( "+" + itrAdd, itrAdd);
                rightAddDD.Items.Add(li);
                leftAddDD.Items.Add(li);
            }
            //onePD
            for (int i = 50; i<= 80; i++)
            {
                ListItem li = new ListItem(i.ToString(), i.ToString());
                onePDDD.Items.Add(li);
            }
            //TwoPDs
            for (int i = 25; i <= 40; i++)
            {
                ListItem li = new ListItem(i.ToString(), i.ToString());
                twoPDRightDD.Items.Add(li);
                twoPDLeftDD.Items.Add(li);
            }
            // prism horizontal
            // prism vertical
            for (int i = 0; i <= 7; i++)
            {
                ListItem li = new ListItem(i.ToString(), i.ToString());
                rightPrismHorizontalDD.Items.Add(li);
                rightPrismVerticalDD.Items.Add(li);
                leftPrismHorizontalDD.Items.Add(li);
                leftPrismVerticalDD.Items.Add(li);
            }
            ListItem InItem = new ListItem("In","In");
            ListItem OutItem = new ListItem("Out", "Out");
            ListItem BIItem = new ListItem("BI", "BI");
            ListItem BOItem = new ListItem("BO", "BO");
            rightPrismHorizontalBaseDirectionDD.Items.Add(InItem);
            rightPrismHorizontalBaseDirectionDD.Items.Add(OutItem);
            rightPrismHorizontalBaseDirectionDD.Items.Add(BIItem);
            rightPrismHorizontalBaseDirectionDD.Items.Add(BOItem);
            leftPrismHorizontalBaseDirectionDD.Items.Add(InItem);
            leftPrismHorizontalBaseDirectionDD.Items.Add(OutItem);
            leftPrismHorizontalBaseDirectionDD.Items.Add(BIItem);
            leftPrismHorizontalBaseDirectionDD.Items.Add(BOItem);
            ListItem UpItem = new ListItem("Up", "Up");
            ListItem DownItem = new ListItem("Down", "Down");
            ListItem BUItem = new ListItem("BU", "BU");
            ListItem BDItem = new ListItem("BD", "BD");
            rightPrismVerticalBaseDirectionDD.Items.Add(UpItem);
            rightPrismVerticalBaseDirectionDD.Items.Add(DownItem);
            rightPrismVerticalBaseDirectionDD.Items.Add(BUItem);
            rightPrismVerticalBaseDirectionDD.Items.Add(BDItem);
            leftPrismVerticalBaseDirectionDD.Items.Add(UpItem);
            leftPrismVerticalBaseDirectionDD.Items.Add(DownItem);
            leftPrismVerticalBaseDirectionDD.Items.Add(BUItem);
            leftPrismVerticalBaseDirectionDD.Items.Add(BDItem);
        }
        protected void btnOpenAddPrescription_Click(object sender, EventArgs e)
        {

            addPrescriptionDropDownValues();
            MPEAddPrescription.Show();
            ScriptManager.RegisterStartupScript(this,this.GetType(),"none", "initModalJavaScript();", true);
        }
        protected void btnCloseAddPrescription_Click(object sender, EventArgs e)
        {
            resetPrescriptionFields();
            MPEAddPrescription.Hide();

        }
        protected void resetPrescriptionFields()
        {

        }

        protected Boolean validateForm()
        {
            Boolean valid = true;
            if (rightSphereDD.SelectedValue == "na")
            {
                valid = false;
            } else if(leftSphereDD.SelectedValue == "na")
            {
                valid = false;
            } else if (hfieldPrescriptionImageSelected.Value == "na" || hfieldPrescriptionImageName.Value == "na")
            {
                valid = false;
            } else if (onePDDD.SelectedValue =="na" && twoPDLeftDD.SelectedValue =="na" && twoPDRightDD.SelectedValue == "na")
            {
                valid = false;
            }
           
            return valid;
        }

        protected String generateMissingList()
        {
            String missingValues = "alert('Please enter values for ";
            if (rightSphereDD.SelectedValue == "na")
            {
                missingValues += "Right Sphere, ";
            }
            if (leftSphereDD.SelectedValue == "na")
            {
                missingValues += "Left Sphere, ";
            }
            if (hfieldPrescriptionImageSelected.Value == "na" || hfieldPrescriptionImageName.Value == "na")
            {
                missingValues += "Prescription Image, ";
            }
            if (onePDDD.SelectedValue == "na" && twoPDLeftDD.SelectedValue == "na" && twoPDRightDD.SelectedValue == "na")
            {
                missingValues += "One PD or Two PD";
            }

            missingValues += " ')";
            return missingValues;
        }


        protected decimal calcSphere(String startingSphere, String startingCylinder)
        {

            Decimal calcValue = 0;
            Decimal cylinder = Decimal.Parse(startingCylinder);
            Decimal sphere = Decimal.Parse(startingSphere);

            calcValue = sphere + cylinder;

            return calcValue;
        } 

        protected decimal calcCylinder(String startingCylinder)
        {

            Decimal startingCyl = Decimal.Parse(startingCylinder);
            Decimal calcValue = startingCyl;
            if (startingCyl > 0)
            {
                calcValue = startingCyl * 2;
                calcValue = startingCyl - calcValue;
            }

            return calcValue;
        }

        protected int calcAxis(String startingAxis)
        {
            int startingAx = int.Parse(startingAxis);
            int calcValue = startingAx + 90;

            if (calcValue > 180)
            {
                calcValue -= 180;
            }


            return calcValue;
        }

        protected void validatePrescription()
        {
            if (validateForm())
            {
                btnAddPrescription.Enabled = true;
            }
        }

        protected void AddPrescription(object sender, EventArgs e)
        {


        if (validateForm()){

                
            
            PrescriptionEntity prescription = new PrescriptionEntity();
            //prescription.ID = 37;
            //prescription.ExamID = 37;
            //prescription.PrescriptionDocumentID = 37;
            prescription.IndividualID_Patient = userInfo.Patient.Individual.ID.ToInt32();
            prescription.IndividualID_Doctor = userInfo.Patient.Individual.ID.ToInt32();
            prescription.ODAxis = calcAxis(rightAxisDD.SelectedValue) ; // 1 to 180
            prescription.OSAxis = calcAxis(leftAxisDD.SelectedValue); // 1 to 180
            prescription.EnteredODAxis = Int32.Parse(rightAxisDD.SelectedValue);
            prescription.EnteredOSAxis = Int32.Parse(leftAxisDD.SelectedValue);
            prescription.ODSphere = calcSphere(rightSphereDD.SelectedValue, rightCylinderDD.SelectedValue);//Plano or number -20 to 20
            prescription.OSSphere = calcSphere(leftSphereDD.SelectedValue, leftCylinderDD.SelectedValue); //Plano or Number -20 to 20
            prescription.ODCylinder = calcCylinder(rightCylinderDD.SelectedValue);//Sphere or number -20 to 20
            prescription.OSCylinder = calcCylinder(leftCylinderDD.SelectedValue);//Sphere or number -20 to 20
            prescription.ODHPrism = Int32.Parse(rightPrismHorizontalDD.SelectedValue);// -15 to 15
            prescription.OSHPrism = Int32.Parse(leftPrismHorizontalDD.SelectedValue);// -15 to 15
            prescription.ODVPrism = Int32.Parse(rightPrismVerticalDD.SelectedValue) ;// -15 to 15
            prescription.OSVPrism = Int32.Parse(leftPrismVerticalDD.SelectedValue) ;// -15 to 15
            prescription.ODAdd = Decimal.Parse(rightAddDD.SelectedValue);
            prescription.OSAdd = Decimal.Parse(leftAddDD.SelectedValue);
            prescription.EnteredODSphere = Int32.Parse(rightSphereDD.SelectedValue);
            prescription.EnteredOSSphere= Int32.Parse(leftSphereDD.SelectedValue); 
            prescription.EnteredODCylinder = Int32.Parse(rightCylinderDD.SelectedValue);
            prescription.EnteredOSCylinder = Int32.Parse(leftCylinderDD.SelectedValue);
            prescription.PDTotal = Int32.Parse(onePDDD.SelectedValue);
            //prescription.PDTotalNear = 0;
            prescription.PDOD = Int32.Parse(twoPDRightDD.SelectedValue);
            //prescription.PDODNear = 0;
            prescription.PDOS = Int32.Parse(twoPDLeftDD.SelectedValue);
            //prescription.PDOSNear = 0;
            prescription.ODHBase = rightPrismHorizontalBaseDirectionDD.SelectedValue;//In/Out
            prescription.OSHBase = leftPrismHorizontalBaseDirectionDD.SelectedValue;//In/Out
            prescription.ODVBase = rightPrismVerticalBaseDirectionDD.SelectedValue;//Up/Down
            prescription.OSVBase = leftPrismVerticalBaseDirectionDD.SelectedValue;//Up/Down
            //prescription.ModifiedBy { get; set; }
            prescription.IsMonoCalculation = true;
            //prescription.IsUsed { get; set; }
            //prescription.IsActive { get; set; }
            //prescription.IsDeletable { get; set; }
            //prescription.DateLastModified = DateTime.Now;
            prescription.PrescriptionDate = DateTime.Parse(tbPrescriptionDate.Text);
            prescription.RxName = "Prescription-"+DateTime.Now;
            _presenter.InsertNewPrescription(prescription);


            String imageBase64 = hfieldPrescriptionImageSelected.Value;
            String[] imageSplit = imageBase64.Split(',');
            byte[] imageBytes = Convert.FromBase64String(imageSplit[1]);
            Int32 scanId = 0;



            _presenter.InsertScannedRx(userInfo.Patient.Individual.ID.ToInt32(), hfieldPrescriptionImageName.Value, imageSplit[0], imageBytes, out scanId);


            ScriptManager.RegisterStartupScript(this, GetType(), "ValidateContinue", "ValidateContinueOrderBtn();", true);
            UpdatePrescriptionPanel.Update();
            userPrescriptions.SelectedIndex = _userPrescriptions.Count;
            MPEAddPrescription.Hide();


            }
            else
            {
                String missingValues = generateMissingList();
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage",missingValues, true);
            }
            

        }



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
                try { 
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
                try
                {
                    FrameRxRestrictionsEntity frameRxRestrictions = _presenter.GetFrameRxRestrictionsByFrameCode(userInfo.JSpecsOrder.PatientOrder.FrameCode);
                    // Filter prescriptions by frame restrictions if frame restrictions are found based on framecode
                    if (!frameRxRestrictions.IsNull())
                    {
                        _userPrescriptions.Where(prescription => CheckPrescriptionAgainstFrameRxRestrictions(prescription, frameRxRestrictions));
                    }

                    // If there are no valid prescriptions, then redirect.
                    if (_userPrescriptions.Count < 0)
                    {
                        ErrorMessage = "You do not have a current prescription on file. Please go to your local Optometry Clinic for support.";
                    }

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
                catch
                {

                }
                

                
            }
        }

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