using SrtsWeb.BusinessLayer.Abstract;
using SrtsWeb.BusinessLayer.Concrete;
using SrtsWeb.Base;
using SrtsWeb.Entities;
using SrtsWeb.ExtendersHelpers;
using SrtsWeb.Presenters.Public;
using SrtsWeb.Views.Public;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SrtsWeb.Public
{
    public partial class FacilityAccountRequest : PageBase, IFacilityAccountRequestView, ISystemAccessRequest, ISiteMapResolver
    {
        private FacilityAccountRequestPresenter _presenter;

        public FacilityAccountRequest()
        {
            _presenter = new FacilityAccountRequestPresenter(this);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            var c = Master.FindControl("pnlContentAuthenticated") as System.Web.UI.WebControls.Panel;
            c.Visible = false;

            var t = Request.QueryString["t"];

            if (!IsPostBack)
            {
                this.divNewSiteRequest.Visible = t.Equals("site");
                this.divSrtsAccessRequest.Visible = !t.Equals("site");
                this.btnSubmit.ValidationGroup = t.Equals("site") ? "allValidators" : "AccessReq";

                this.rfvCaptchaCode.ValidationGroup = t.Equals("site") ? "allValidators" : "AccessReq";
                this.cvCaptcha.ValidationGroup = t.Equals("site") ? "allValidators" : "AccessReq";

                CurrentModule("SRTSweb Resources");

                if (this.divNewSiteRequest.Visible)
                {
                    if (Cache["SRTSLOOKUP"] == null)
                    {
                        LoadLookupTable();
                    }

                    CurrentModule_Sub(" <br /> New Facility/Site Request");

                    _presenter.InitView();
                }
                else
                {
                    var p = new SystemAccessRequestPresenter(this);
                    p.InitView();
                    CurrentModule_Sub(" <br /> Systems Access Request");
                }

                mySession.SecurityAcknowledged = true;

                Master.CurrentModuleTitle = string.Format("{0} {1}", mySession.CurrentModule, mySession.CurrentModule_Sub);
                Master.uplCurrentModuleTitle.Update();
                litModuleTitle.Text = Master.CurrentModuleTitle;
            }
        }

        private void LoadLookupTable()
        {
            ILookupService _service = new LookupService();
            Cache["SRTSLOOKUP"] = _service.GetAllLookups();
        }

        public SiteMapNode BuildBreadCrumbs(object sender, SiteMapResolveEventArgs e)
        {
            SiteMapNode parent = new SiteMapNode(e.Provider, "1", "~/Default.aspx", "My SRTSWeb");
            SiteMapNode child = new SiteMapNode(e.Provider, "2", "~/Public/FacilityAccountRequest.aspx", "Facility Account Request Form");
            child.ParentNode = parent;
            return child;
        }

        protected void BuildUserInterface()
        {
            Master.CurrentModuleTitle = string.Format("{0} {1}", mySession.CurrentModule, mySession.CurrentModule_Sub);
        }

        protected void FacilityAccountRequestCaptchaValidator_ServerValidate(object source,  ServerValidateEventArgs args)
        {
            // validate the Captcha to check we're not dealing with a bot
            args.IsValid = FacilityAccountRequestCaptcha.Validate(args.Value);

            FacilityAccountRequestCaptchaCode.Text = null; // clear previous user input
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                if (this.divNewSiteRequest.Visible)
                {
                    if (_presenter.SendEmail(new MailService()))
                    {
                        pnlMain.Visible = false;
                        this.divcaptcha.Visible = false;
                        pnlNewSiteRequestSuccess.Visible = true;
                        btnSubmit.Visible = false;
                    }
                }
                else
                {
                    var p = new SystemAccessRequestPresenter(this);
                    if (p.SendEmail(new MailService()))
                    {
                        this.divSrtsAccessRequestForm.Visible = false;
                        this.divcaptcha.Visible = false;
                        pnlAccessRequestSuccess.Visible = true;
                        btnSubmit.Visible = false;
                    }
                }
            }
            else
            {
                FocusOnError();
            }
        }

        #region Accessors

        public string UnitName
        {
            get { return txtUnitName.Text; }
            set { txtUnitName.Text = value; }
        }

        public string UnitZipCode
        {
            get { return txtUnitZipCode.Text; }
            set { txtUnitZipCode.Text = value; }
        }

        public string UnitAddress1
        {
            get { return txtUnitAddress1.Text; }
            set { txtUnitAddress1.Text = value; }
        }

        public string UnitAddress2
        {
            get { return txtUnitAddress2.Text; }
            set { txtUnitAddress2.Text = value; }
        }

        public string UnitAddress3
        {
            get { return txtUnitAddress3.Text; }
            set { txtUnitAddress3.Text = value; }
        }

        public string UnitCity
        {
            get { return txtUnitCity.Text; }
            set { txtUnitCity.Text = value; }
        }

        public string UnitState
        {
            get { return ddlState.SelectedValue; }
            set { ddlState.SelectedValue = value; }
        }

        public List<LookupTableEntity> StateDDL
        {
            set
            {
                ddlState.Items.Clear();
                ddlState.DataSource = value;
                ddlState.DataTextField = "ValueTextCombo";
                ddlState.DataValueField = "Value";
                ddlState.DataBind();
                ddlState.Items.Insert(0, "-- Select --");
                ddlState.Items.Insert(1, "N/A");
                ddlState.SelectedIndex = 0;
            }
        }

        public string UnitCountry
        {
            get { return ddlCountry.SelectedValue; }
            set { ddlCountry.SelectedValue = value; }
        }

        public List<LookupTableEntity> CountryDDL
        {
            set
            {
                ddlCountry.Items.Clear();
                ddlCountry.DataSource = value;
                ddlCountry.DataTextField = "Text";
                ddlCountry.DataValueField = "Value";
                ddlCountry.DataBind();
                ddlCountry.Items.Insert(0, "-- Select --");
                ddlCountry.SelectedIndex = 0;
            }
        }

        public string Comments
        {
            get { return txtComments.Text; }
            set { txtComments.Text = value; }
        }

        public string RequestorsName
        {
            get { return txtRequesterName.Text; }
            set { txtRequesterName.Text = value; }
        }

        public string RequestorsTitle
        {
            get { return txtRequesterTitle.Text; }
            set { txtRequesterTitle.Text = value; }
        }

        public string RequestorsWorkPhone
        {
            get { return txtWorkPhone.Text; }
            set { txtWorkPhone.Text = value; }
        }

        public string RequestorsDSNPhone
        {
            get { return txtDSNPhone.Text; }
            set { txtDSNPhone.Text = value; }
        }

        public string RequestorsFax
        {
            get { return txtFax.Text; }
            set { txtFax.Text = value; }
        }

        public string RequestorsEmail
        {
            get { return txtEmail.Text; }
            set { txtEmail.Text = value; }
        }

        public string UnitFacilityType
        {
            get { return ddlFacilityType.SelectedValue; }
            set { ddlFacilityType.SelectedValue = value; }
        }

        public List<LookupTableEntity> FacilityTypeDDL
        {
            set
            {
                ddlFacilityType.Items.Clear();
                ddlFacilityType.DataSource = value;
                ddlFacilityType.DataTextField = "Text";
                ddlFacilityType.DataValueField = "Value";
                ddlFacilityType.DataBind();
                // Removes ADMINISTRATION value from the ddl
                var removeAdmin = new ListItem();
                removeAdmin = ddlFacilityType.Items.FindByValue("ADMIN");
                ddlFacilityType.Items.Remove(removeAdmin);
                ddlFacilityType.Items.Insert(0, "-- Select --");
                ddlFacilityType.SelectedIndex = 0;
            }
        }

        public string FacilityBOS
        {
            get { return ddlFaclityBOS.SelectedValue; }
            set { ddlFaclityBOS.SelectedValue = value; }
        }

        public List<LookupTableEntity> BOS_DDL
        {
            set
            {
                ddlFaclityBOS.Items.Clear();
                ddlFaclityBOS.DataSource = value;
                ddlFaclityBOS.DataTextField = "Text";
                ddlFaclityBOS.DataValueField = "Value";
                ddlFaclityBOS.DataBind();
                ddlFaclityBOS.Items.Insert(0, "-- Select --");
                ddlFaclityBOS.SelectedIndex = 0;
            }
        }

        public string FacilityComponent
        {
            get { return ddlFacilityComponent.SelectedValue; }
            set { ddlFacilityComponent.SelectedValue = value; }
        }

        #endregion Accessors

        #region Input Validators

        protected void ValidateRequestorName(object source, ServerValidateEventArgs args)
        {
            if (args.IsValid = args.Value.ValidateNameLength())
            {
                if (args.IsValid = args.Value.ValidateNameFormat())
                {
                }
                else
                {
                    cvRequesterName.ErrorMessage = "Invalid characters in Requetor Name";
                }
            }
            else
            {
                cvRequesterName.ErrorMessage = "Requestor Name is required (1-40 characters)";
            }
        }

        protected void ValidateRequestorTitle(object source, ServerValidateEventArgs args)
        {
            if (args.IsValid = args.Value.ValidateNameLength())
            {
                if (args.IsValid = args.Value.ValidateNameFormat())
                {
                }
                else
                {
                    cvRequesterTitle.ErrorMessage = "Invalid characters in Requetor Title";
                }
            }
            else
            {
                cvRequesterTitle.ErrorMessage = "Requestor Title is required (1-40 characters)";
            }
        }

        protected void ValidateWorkPhone(object source, ServerValidateEventArgs args)
        {
            if (!string.IsNullOrEmpty(args.Value))
            {
                if (args.IsValid = args.Value.ValidateWorkPhoneFormat())
                {
                }
                else
                {
                    cvWorkPhone.ErrorMessage = "Invalid characters or format in Work Phone";
                }
            }
            else
            {
                args.IsValid = false;
                cvWorkPhone.ErrorMessage = "Work Phone is required";
            }
        }

        protected void ValidateDSNPhone(object source, ServerValidateEventArgs args)
        {
            if (!string.IsNullOrEmpty(args.Value))
            {
                if (args.IsValid = args.Value.ValidateDSNPhoneFormat())
                {
                }
                else
                {
                    cvDSNPhone.ErrorMessage = "Invalid characters or format in DSN Phone";
                }
            }
        }

        protected void ValidateUnitName(object source, ServerValidateEventArgs args)
        {
            if (args.IsValid = args.Value.ValidateNameLength())
            {
                if (args.IsValid = args.Value.ValidateUnitNameFormat())
                {
                }
                else
                {
                    cvUnitName.ErrorMessage = "Invalid characters in Unit Name";
                }
            }
            else
            {
                cvUnitName.ErrorMessage = "Unit Name is required (1-40 characters)";
            }
        }

        protected void ValidateFax(object source, ServerValidateEventArgs args)
        {
            if (!string.IsNullOrEmpty(args.Value))
            {
                if (args.IsValid = args.Value.ValidateFaxFormat())
                {
                }
                else
                {
                    cvFax.ErrorMessage = "Invalid characters or format in Fax Number";
                }
            }
        }

        protected void ValidateAddress(object source, ServerValidateEventArgs args)
        {
            if (!string.IsNullOrEmpty(args.Value))
            {
                if (args.IsValid = args.Value.ValidateAddressFormat())
                {
                }
                else
                {
                    cvAddress.ErrorMessage = "Invalid characters or format in Address";
                }
            }
            else
            {
                args.IsValid = false;
                cvAddress.ErrorMessage = "Address1 is required";
            }
        }

        protected void ValidateAddress2(object source, ServerValidateEventArgs args)
        {
            if (!string.IsNullOrEmpty(args.Value))
            {
                if (args.IsValid = args.Value.ValidateAddressFormat())
                {
                }
                else
                {
                    cvAddress2.ErrorMessage = "Invalid characters or format in Address 2";
                    cvAddress3.ErrorMessage = "Invalid characters or format in Address 3";
                }
            }
        }

        protected void ValidateCity(object source, ServerValidateEventArgs args)
        {
            if (!string.IsNullOrEmpty(args.Value))
            {
                if (args.IsValid = args.Value.ValidateCityFormat())
                {
                }
                else
                {
                    cvCity.ErrorMessage = "Invalid characters or format in City";
                }
            }
            else
            {
                args.IsValid = false;
                cvCity.ErrorMessage = "City is required";
            }
        }

        protected void ValidateZipCode(object source, ServerValidateEventArgs args)
        {
            if (!string.IsNullOrEmpty(args.Value))
            {
                if (args.IsValid = args.Value.ValidateZipCodeFormat())
                {
                }
                else
                {
                    cvZipCode.ErrorMessage = "Invalid characters or format in Zip Code";
                }
            }
            else
            {
                args.IsValid = false;
                cvZipCode.ErrorMessage = "Zip Code is required";
            }
        }

        protected void ValidateCommentFormat(object source, ServerValidateEventArgs args)
        {
            if (!string.IsNullOrEmpty(args.Value))
            {
                if (args.IsValid = args.Value.ValidateCommentFormat())
                {
                }
                else
                {
                    cvComment.ErrorMessage = "Invalid characters in Comment";
                }
            }
        }

        protected void FocusOnError()
        {
            foreach (BaseValidator validator in Page.Validators)
            {
                if (!validator.IsValid)
                {
                    validator.FindControl(validator.ControlToValidate).Focus();
                    break;
                }
            }
        }

        protected void ddlCountry_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlCountry.SelectedValue != "US" && ddlCountry.SelectedIndex != 0)
            {
                ddlState.SelectedIndex = 1;
            }
            else
            {
                ddlState.SelectedIndex = 0;
            }
        }

        protected void ddlState_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlState.SelectedIndex > 1)
            {
                ddlCountry.SelectedValue = "US";
            }
            else
            {
                ddlCountry.SelectedIndex = 0;
            }
        }

        #endregion Input Validators

        #region ISystemAccessRequest Properties

        public string RequesterName
        {
            get
            {
                return this.tbName.Text;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public string RequesterTitle
        {
            get
            {
                return this.tbTitle.Text;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public string PhoneNumber
        {
            get
            {
                return this.tbPhone.Text;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public string Email
        {
            get
            {
                return this.tbEmail.Text;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public string SiteCode
        {
            get
            {
                return this.ddlSiteCode.SelectedValue;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public List<SiteCodeEntity> SiteList
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                var l = value.Select(x => x.SiteCode).Distinct().ToList();

                l.Remove("ADM001");
                l.Remove("ADM002");
                l.Remove("GEyes1");
                l.Remove("009900");

                this.ddlSiteCode.DataSource = l;
                this.ddlSiteCode.DataBind();

                this.ddlSiteCode.Items.Insert(0, new ListItem("-Select-", "X"));
                this.ddlSiteCode.SelectedIndex = 0;
            }
        }

        #endregion ISystemAccessRequest Properties
    }
}