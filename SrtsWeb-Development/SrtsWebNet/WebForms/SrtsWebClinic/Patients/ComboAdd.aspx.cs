using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SrtsWeb.CustomErrors;
using SrtsWeb.BusinessLayer.Views.Patients;
using SrtsWeb.BusinessLayer.Presenters.Patients;
using System.Data;

namespace SrtsWebClinic.Patients
{
    public partial class ComboAdd : PageBase, IComboAddView
    {
        private ComboAddPresenter _presenter;

        public ComboAdd()
        {
            _presenter = new ComboAddPresenter(this);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            ////////////////////////////////////// shows apppropriate page title
            CurrentModule("Manage Patients - Add Patient Information");
            //////////////////////////////////////
            srtsAddPatientInformationHeader.HeaderImage.Visible = false;
            srtsAddInformationHeader.HeaderImage.Visible = false;
            if (!IsPostBack)
            {
                _presenter.InitView();
                try
                {
                    litContentTop_Title_Right.Text = string.Format("{0} - DOB: {1}", mySession.Patient.Individual.NameLFMi, mySession.Patient.Individual.DateOfBirth != null ? mySession.Patient.Individual.DateOfBirth.Value.ToShortDateString() : string.Empty);
                }
                catch (NullReferenceException)
                {
                    Response.Redirect("PatientDetails.aspx");
                }

            }
        }


        protected void btnSaveIDNumber_Click(object sender, EventArgs e)
        {
            _presenter.SaveIDNumbers();
        }

        protected void btnSaveAddress_Click(object sender, EventArgs e)
        {
            _presenter.SaveAddress();
        }

        protected void btnEmailSave_Click(object sender, EventArgs e)
        {
            _presenter.SaveEmail();
        }

        protected void btnPhoneSave_Click(object sender, EventArgs e)
        {
            _presenter.SavePhone();
        }

        #region IDNumber Accessors

        public string IDNumberMessage
        {
            get { return lblCompleteIDNumber.Text; }
            set { lblCompleteIDNumber.Text = value; }
        }

        public DataTable IDTypeDDL
        {
            set
            {
                ddlIDNumberType.DataSource = value;
                ddlIDNumberType.DataTextField = "Text";
                ddlIDNumberType.DataValueField = "Value";
                ddlIDNumberType.DataBind();
            }
        }

        public string IDNumber
        {
            get { return tbIDNumber.Text; }
            set { tbIDNumber.Text = value; }
        }

        public string IDNumberType
        {
            get { return ddlIDNumberType.SelectedValue; }
            set { ddlIDNumberType.SelectedValue = value; }
        }

        #endregion

        #region Address Accessors


        public string Address1
        {
            get
            {
                return tbAddress1.Text;
            }
            set
            {
                tbAddress1.Text = value;
            }
        }

        public string Address2
        {
            get
            {
                return tbAddress2.Text;
            }
            set
            {
                tbAddress2.Text = value;
            }
        }

        public string City
        {
            get
            {
                return tbCity.Text;
            }
            set
            {
                tbCity.Text = value;
            }
        }

        public string CountrySelected
        {
            get
            {
                return ddlCountry.SelectedValue;
            }
            set
            {
                ddlCountry.SelectedValue = value;
            }
        }

        public DataTable CountryDDL
        {
            set
            {
                ddlCountry.DataSource = value;
                ddlCountry.DataTextField = "Text";
                ddlCountry.DataValueField = "Value";
                ddlCountry.DataBind();
            }
        }

        public string StateSelected
        {
            get
            {
                return ddlState.SelectedValue;
            }
            set
            {
                ddlState.SelectedValue = value;
            }
        }

        public DataTable StateDDL
        {
            set
            {
                ddlState.DataSource = value;
                ddlState.DataTextField = "Text";
                ddlState.DataValueField = "Value";
                ddlState.DataBind();
            }
        }

        public string AddressTypeSelected
        {
            get
            {
                return ddlAddressType.SelectedValue;
            }
            set
            {
                ddlAddressType.SelectedValue = value;
            }
        }

        public DataTable AddressTypeDDL
        {
            set
            {
                ddlAddressType.DataSource = value;
                ddlAddressType.DataTextField = "Text";
                ddlAddressType.DataValueField = "Value";
                ddlAddressType.DataBind();
            }
        }

        public string ZipCode
        {
            get
            {
                return tbZipCode.Text;
            }
            set
            {
                tbZipCode.Text = value;
            }
        }

        public string AddrMessage
        {
            get { return lblAddrComplete.Text; }
            set { lblAddrComplete.Text = value; }
        }

        #endregion

        #region Email Accessors


        public string EMailAddress
        {
            get
            {
                return tbEMailAddress.Text;
            }
            set
            {
                tbEMailAddress.Text = value;
            }
        }

        public string TypeEMailSelected
        {
            get
            {
                return ddlEMailType.SelectedValue;
            }
            set
            {
                ddlEMailType.SelectedValue = value;
            }
        }

        public DataTable TypeEmailDDL
        {
            set
            {
                ddlEMailType.DataSource = value;
                ddlEMailType.DataTextField = "Text";
                ddlEMailType.DataValueField = "Value";
                ddlEMailType.DataBind();
            }
        }

        public string EmailMessage
        {
            get
            {
                return lblEmailComplete.Text;
            }
            set
            {
                lblEmailComplete.Text = value;
            }
        }

        #endregion

        #region Phone Accessors

        public string PhoneMessage
        {
            get { return lblPhoneComplete.Text; }
            set { lblPhoneComplete.Text = value; }
        }

        public string AreaCode
        {
            get
            {
                return tbAreaCode.Text;
            }
            set
            {
                tbAreaCode.Text = value;
            }
        }

        public string Extension
        {
            get
            {
                return tbExtension.Text;
            }
            set
            {
                tbExtension.Text = value;
            }
        }

        public string PhoneNumber
        {
            get
            {
                return tbPhoneNumber.Text;
            }
            set
            {
                tbPhoneNumber.Text = value;
            }
        }

        public DataTable PhoneTypeDDL
        {
            set
            {
                ddlPhoneType.DataSource = value;
                ddlPhoneType.DataTextField = "Text";
                ddlPhoneType.DataValueField = "Value";
                ddlPhoneType.DataBind();
            }
        }

        public string TypePhoneSelected
        {
            get
            {
                return ddlPhoneType.SelectedValue;
            }
            set
            {
                ddlPhoneType.SelectedValue = value;
            }
        }

        #endregion
    }
}