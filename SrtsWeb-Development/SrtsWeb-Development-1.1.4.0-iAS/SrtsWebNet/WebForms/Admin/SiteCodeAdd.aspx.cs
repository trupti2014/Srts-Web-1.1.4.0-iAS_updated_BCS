using SrtsWeb.Base;
using SrtsWeb.BusinessLayer.Concrete;
using SrtsWeb.Entities;
using SrtsWeb.ExtendersHelpers;
using SrtsWeb.Presenters.Admin;
using SrtsWeb.Views.Admin;
using System;
using System.Collections.Generic;
using System.Security.Permissions;
using System.Web.UI.WebControls;

namespace SrtsWeb.Admin
{
    [PrincipalPermission(SecurityAction.Demand, Role = "ClinicAdmin")]
    [PrincipalPermission(SecurityAction.Demand, Role = "MgmtAdmin")]
    [PrincipalPermission(SecurityAction.Demand, Role = "MgmtEnterprise")]
    [PrincipalPermission(SecurityAction.Demand, Role = "MgmtDataMgmt")]
    [PrincipalPermission(SecurityAction.Demand, Role = "TrainingAdmin")]
    public partial class SiteCodeAdd : PageBase, ISiteCodeAddView
    {
        private SiteCodeAddPresenter _presenter;

        public SiteCodeAdd()
        {
            _presenter = new SiteCodeAddPresenter(this);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
#if DEBUG
            using (MethodTracer.Trace(SrtsTraceSource.AdminSource, "SiteCodeAdd_Page_Load", mySession.MyUserID))
#endif
            {
                if (!IsPostBack)
                {
                    _presenter.InitView();
                    ddlSiteType.SelectedValue = "CLINIC";
                    pnlLabsLabels.Visible = true;
                    pnlLabsInput.Visible = true;
                    pnlLabsOnlyLabels.Visible = false;
                    pnlLabsOnlyInput.Visible = false;
                }
            }
        }

        private void ShowMessage()
        {
            CustomValidator cv = new CustomValidator();
            cv.IsValid = false;
            cv.ErrorMessage = ErrMessage;
            this.Page.Validators.Add(cv);
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
#if DEBUG
            using (MethodTracer.Trace(SrtsTraceSource.AdminSource, "SiteCodeAdd_btnAdd_Click", mySession.MyUserID))
#endif
            {
                ErrMessage = string.Empty;
                _presenter.InsertSite();

                var m = string.Empty;
                if (!string.IsNullOrEmpty(ErrMessage))
                {
                    ShowMessage();
                    m = "unsuccessfully added new site";
                }
                else
                {
                    ErrMessage = "Data was Inserted!";
                    ShowMessage();
                    m = "successfully added new site";
                }

                LogEvent("User {0} {1} {2} at {3}", new Object[] { mySession.MyUserID, m, this.SiteCode, DateTime.Now });
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/WebForms/Admin/SiteCodeManagement.aspx");
        }

        protected void ddlSiteType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlSiteType.SelectedIndex == 3)
            {
                pnlLabsLabels.Visible = false;
                pnlLabsInput.Visible = false;
                pnlLabsOnlyLabels.Visible = true;
                pnlLabsOnlyInput.Visible = true;
            }
            else if (ddlSiteType.SelectedIndex == 0)
            {
                pnlLabsLabels.Visible = false;
                pnlLabsInput.Visible = false;
                pnlLabsOnlyLabels.Visible = false;
                pnlLabsOnlyInput.Visible = false;
            }
            else
            {
                pnlLabsLabels.Visible = true;
                pnlLabsInput.Visible = true;
                pnlLabsOnlyLabels.Visible = false;
                pnlLabsOnlyInput.Visible = false;
            }
        }

        protected void rblUseAddress_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (bool.Parse(rblUseAddress.SelectedValue) == false)
            {
                MailAddress1 = "";
                MailAddress2 = "";
                MailAddress3 = "";
                MailCity = City;
                MailState = State;
                MailCountry = Country;
                MailZipCode = "";
                middleSiteAddMailAddress.Visible = true;
            }
            else if (bool.Parse(rblUseAddress.SelectedValue) == true)
            {
                middleSiteAddMailAddress.Visible = false;
                MailAddress1 = Address1;
                MailAddress2 = Address2;
                MailAddress3 = Address3;
                MailCity = City;
                MailState = State;
                MailCountry = Country;
                MailZipCode = ZipCode.ToZipCodeDisplay();

                IsConusMail = IsConus;
            }
        }
        //private void LoadLookupTable()
        //{
        //    SrtsWeb.BusinessLayer.Abstract.ILookupService _service = new LookupService();
        //    Cache["SRTSLOOKUP"] = _service.GetAllLookups();
        //}
        #region Accessors
        public List<LookupTableEntity> CacheData
        {
            get
            {
                if (Cache["SRTSLOOKUP"] == null)
                {
                    Master.LoadLookupTable();
                }
                return Cache["SRTSLOOKUP"] as List<LookupTableEntity>;
            }
            set { Cache["SRTSLOOKUP"] = value; }
        }
       
        private string _errMessage;

        public string ErrMessage
        {
            get { return _errMessage; }
            set { _errMessage = value; }
        }

        public bool UseAddress
        {
            get { return Convert.ToBoolean(rblUseAddress.SelectedValue); }
            set { rblUseAddress.SelectedValue = value.ToString(); }
        }

        public bool HasLMS
        {
            get
            {
                if (rblHasLMS.SelectedValue.ToString().ToUpper() == "TRUE")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            set { rblHasLMS.SelectedValue = value.ToString(); }
        }

        public bool ShipToPatientLab
        {
            get
            {
                if (rblShipToPatientLab.SelectedValue.ToString().ToUpper() == "TRUE")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            set
            {
                rblShipToPatientLab.SelectedValue = value.ToString();
            }
        }

        public string MultiPrimary
        {
            get { return ddlMPrimary.SelectedValue; }
            set { ddlMPrimary.SelectedValue = value; }
        }

        public string MultiSecondary
        {
            get;

            set;
        }

        public string SinglePrimary
        {
            get { return ddlSPrimary.SelectedValue; }
            set { ddlSPrimary.SelectedValue = value; }
        }

        public string SingleSecondary
        {
            get;

            set;
        }

        public List<string> MPrimary
        {
            get { return (List<string>)ddlMPrimary.DataSource; }
            set
            {
                ddlMPrimary.Items.Clear();
                ddlMPrimary.DataSource = value;
                ddlMPrimary.DataBind();
            }
        }

        private List<string> _sPrimary;

        public List<string> SPrimary
        {
            get { return _sPrimary; }
            set
            {
                _sPrimary = value;
                ddlSPrimary.Items.Clear();
                ddlSPrimary.DataSource = _sPrimary;
                ddlSPrimary.DataBind();
            }
        }

        private List<string> _mSecondary;

        public List<string> MSecondary
        {
            get { return _mSecondary; }
            set
            {
                _mSecondary = value;
            }
        }

        private List<string> _sSecondary;

        public List<string> SSecondary
        {
            get { return _sSecondary; }
            set
            {
                _sSecondary = value;
            }
        }

        public int Region
        {
            get;

            set;
        }

        public string SiteCode
        {
            get { return tbSiteCode.Text; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    tbSiteCode.Text = value;
                }
                else
                {
                    tbSiteCode.Text = string.Empty;
                }
            }
        }

        public string SiteCodetb
        {
            get { return tbSiteCode.Text; }
            set { tbSiteCode.Text = value; }
        }

        public string SiteName
        {
            get { return tbSiteName.Text; }
            set { tbSiteName.Text = value; }
        }

        public string SiteType
        {
            get { return ddlSiteType.SelectedValue; }
            set { ddlSiteType.SelectedValue = value; }
        }

        public string SiteDescription
        {
            get { return tbSiteDescription.Text; }
            set { tbSiteDescription.Text = value; }
        }

        public string BOS
        {
            get { return ddlBOS.SelectedValue; }
            set { ddlBOS.SelectedValue = value; }
        }

        public bool IsMultivision
        {
            get { return Convert.ToBoolean(rblIsMultivision.SelectedValue); }
            set { rblIsMultivision.SelectedValue = value.ToString(); }
        }

        public string Address1
        {
            get { return tbAddress1.Text; }
            set { tbAddress1.Text = value; }
        }

        public string Address2
        {
            get { return tbAddress2.Text; }
            set { tbAddress2.Text = value; }
        }

        public string Address3
        {
            get { return tbAddress3.Text; }
            set { tbAddress3.Text = value; }
        }

        public string City
        {
            get { return tbCity.Text; }
            set { tbCity.Text = value; }
        }

        public string State
        {
            get { return ddlState.SelectedValue; }
            set { ddlState.SelectedValue = value; }
        }

        public string Country
        {
            get { return ddlCountry.SelectedValue; }
            set { ddlCountry.SelectedValue = value; }
        }

        public string ZipCode
        {
            get { return tbZipCode.Text; }
            set { tbZipCode.Text = value; }
        }

        public string MailAddress1
        {
            get { return tbMailAddress1.Text; }
            set { tbMailAddress1.Text = value; }
        }

        public string MailAddress2
        {
            get { return tbMailAddress2.Text; }
            set { tbMailAddress2.Text = value; }
        }

        public string MailAddress3
        {
            get { return tbMailAddress3.Text; }
            set { tbMailAddress3.Text = value; }
        }

        public string MailCity
        {
            get { return tbMailCity.Text; }
            set { tbMailCity.Text = value; }
        }

        public string MailState
        {
            get { return ddlMailState.SelectedValue; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    ddlMailState.SelectedValue = value;
                }
                else
                {
                    ddlMailState.SelectedIndex = -1;
                }
            }
        }

        public string MailCountry
        {
            get { return ddlMailCountry.SelectedValue; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    ddlMailCountry.SelectedValue = value;
                }
                else
                {
                    ddlMailCountry.SelectedIndex = -1;
                }
            }
        }

        public string MailZipCode
        {
            get { return tbMailZipCode.Text; }
            set { tbMailZipCode.Text = value; }
        }

        public bool IsConus
        {
            get { return Convert.ToBoolean(rblIsConus.SelectedValue); }
            set { rblIsConus.SelectedValue = value.ToString(); }
        }

        public bool IsConusMail
        {
            get { return Convert.ToBoolean(rblMailIsConus.SelectedValue); }
            set { rblMailIsConus.SelectedValue = value.ToString(); }
        }

        public string EMailAddress
        {
            get { return tbEmail.Text; }
            set { tbEmail.Text = value; }
        }

        public string DSNPhoneNumber
        {
            get { return tbDSNPhoneNumber.Text; }
            set { tbDSNPhoneNumber.Text = value; }
        }

        public string RegPhoneNumber
        {
            get { return tbRegPhoneNumber.Text; }
            set { tbRegPhoneNumber.Text = value; }
        }

        public int MaxEyeSize
        {
            get { return string.IsNullOrEmpty(tbMaxEyeSize.Text) ? 0 : Convert.ToInt32(tbMaxEyeSize.Text); }
            set { tbMaxEyeSize.Text = value.ToString(); }
        }

        public int MaxFramesPerMonth
        {
            get { return string.IsNullOrEmpty(tbMaxFrames.Text) ? 0 : Convert.ToInt32(tbMaxFrames.Text); }
            set { tbMaxFrames.Text = value.ToString(); }
        }

        public double MaxPower
        {
            get { return string.IsNullOrEmpty(tbMaxPower.Text) ? 0.0 : Convert.ToDouble(tbMaxPower.Text); }
            set { tbMaxPower.Text = value.ToString(); }
        }

        public bool IsActive
        {
            get { return Convert.ToBoolean(rblIsActive.SelectedValue); }
            set { rblIsActive.SelectedValue = value.ToString(); }
        }

        private List<LookupTableEntity> _stateData;

        public List<LookupTableEntity> StateData
        {
            get
            {
                return _stateData;
            }
            set
            {
                _stateData = value;
                ddlState.Items.Clear();
                ddlState.DataSource = _stateData;
                ddlState.DataTextField = "ValueTextCombo";
                ddlState.DataValueField = "Value";

                ddlMailState.Items.Clear();
                ddlMailState.DataSource = _stateData;
                ddlMailState.DataTextField = "ValueTextCombo";
                ddlMailState.DataValueField = "Value";
                ddlState.DataBind();
                ddlMailState.DataBind();
            }
        }

        private List<LookupTableEntity> _countryData;

        public List<LookupTableEntity> CountryData
        {
            get
            {
                return _countryData;
            }
            set
            {
                _countryData = value;
                ddlCountry.Items.Clear();
                ddlCountry.DataSource = _countryData;
                ddlCountry.DataTextField = "Text";
                ddlCountry.DataValueField = "Value";

                ddlMailCountry.Items.Clear();
                ddlMailCountry.DataSource = _countryData;
                ddlMailCountry.DataTextField = "Text";
                ddlMailCountry.DataValueField = "Value";
                ddlMailCountry.DataBind();
                ddlCountry.DataBind();
            }
        }

        private List<LookupTableEntity> _bosData;

        public List<LookupTableEntity> BOSData
        {
            get { return _bosData; }
            set
            {
                _bosData = value;
                ddlBOS.Items.Clear();
                ddlBOS.DataSource = _bosData;
                ddlBOS.DataTextField = "Text";
                ddlBOS.DataValueField = "Value";
                ddlBOS.DataBind();
            }
        }

        public bool NewSite
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public List<LookupTableEntity> SiteCodes
        {
            get { return (List<LookupTableEntity>)ddlSiteType.DataSource; }
            set
            {
                ddlSiteType.DataSource = value;
                ddlSiteType.DataBind();
            }
        }

        public bool IsReimbursable
        {
            get
            {
                return Convert.ToBoolean(this.rblIsReimbursable.SelectedValue);
            }
            set
            {
                this.rblIsReimbursable.SelectedValue = value.ToString();
            }
        }

        #endregion Accessors
    }
}