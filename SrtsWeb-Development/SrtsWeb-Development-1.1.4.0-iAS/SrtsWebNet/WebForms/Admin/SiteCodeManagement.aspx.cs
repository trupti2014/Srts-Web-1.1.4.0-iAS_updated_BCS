using SrtsWeb.Base;
using SrtsWeb.BusinessLayer.Abstract;
using SrtsWeb.BusinessLayer.Concrete;
using SrtsWeb.Entities;
using SrtsWeb.ExtendersHelpers;
using SrtsWeb.Presenters.Admin;
using SrtsWeb.Views.Admin;
using System;
using System.Collections.Generic;
using System.Security.Permissions;
using System.Web.Security;

namespace SrtsWeb.Admin
{
    [PrincipalPermission(SecurityAction.Demand, Role = "HumanAdmin")]
    [PrincipalPermission(SecurityAction.Demand, Role = "LabAdmin")]
    [PrincipalPermission(SecurityAction.Demand, Role = "ClinicAdmin")]
    [PrincipalPermission(SecurityAction.Demand, Role = "MgmtAdmin")]
    [PrincipalPermission(SecurityAction.Demand, Role = "MgmtEnterprise")]
    [PrincipalPermission(SecurityAction.Demand, Role = "MgmtDataMgmt")]
    [PrincipalPermission(SecurityAction.Demand, Role = "TrainingAdmin")]
    public partial class SiteCodeManagement : PageBase, ISiteCodeManagementView
    {
        private SiteCodeManagementPresenter _presenter;

        public SiteCodeManagement()
        {
            _presenter = new SiteCodeManagementPresenter(this);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
#if DEBUG
            using (MethodTracer.Trace(SrtsTraceSource.AdminSource, "SiteCodeManagement_Page_Load", mySession.MyUserID))
#endif
            {
                if (!IsPostBack)
                {
                    _presenter.InitView();
                    SetViewBySiteType();
                    Master.CurrentModuleTitle = string.Empty;
                    CurrentModule("Administration - Facilities Manager");
                    CurrentModule_Sub(string.Empty);
                    BuildPageTitle();
                }
                if (Roles.IsUserInRole("MgmtEnterprise") || Roles.IsUserInRole("MgmtDataMgmt") || Roles.IsUserInRole("MgmtAdmin") || Roles.IsUserInRole("TrainingAdmin"))
                {
                    ddlSiteCode.Enabled = true;
                    btnAdd.Visible = true;
                }
                else
                {
                    ddlSiteCode.Enabled = false;
                    btnAdd.Visible = false;
                }
            }
        }

        protected void ddlSiteCode_SelectedIndexChanged(object sender, EventArgs e)
        {
#if DEBUG
            using (MethodTracer.Trace(SrtsTraceSource.AdminSource, "SiteCodeManagement_ddlSiteCode_SelectedIndexChanged", mySession.MyUserID))
#endif
            {
                _presenter.FillData();

                SetViewBySiteType();
            }
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/WebForms/Admin/SiteCodeAdd.aspx");
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            string url = (string.Format("~/WebForms/Admin/SiteCodeEdit.aspx?id={0}", SiteCode));
            if (!CustomRedirect.SanitizeRedirect(url, false))
            {
                ShowErrorDialog("An Error has occurred.");
            };

           // CustomRedirect.SanitizeRedirect(url, false);
            
            //if(IsLocalUrl(url)  && !Helpers.isAbsolute(url))
            //{
            //Response.Redirect(url);
            //}
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/WebForms/Default.aspx");
        }

        private void SetViewBySiteType()
        {
            if (SiteType.ToUpper() == "LAB")
            {
                pnlLabs.Visible = false;
                pnlLabOnly.Visible = true;
            }
            else if (SiteType.ToUpper() == "ADMIN")
            {
                pnlLabs.Visible = false;
                pnlLabOnly.Visible = false;
            }
            else
            {
                pnlLabs.Visible = true;
                pnlLabOnly.Visible = false;
            }
        }

        private void BuildPageTitle()
        {
            try
            {
                Master.CurrentModuleTitle = string.Format("{0} {1}", mySession.CurrentModule, mySession.CurrentModule_Sub);
                Master.uplCurrentModuleTitle.Update();
            }
            catch (NullReferenceException)
            {
                CurrentModule("Administration - Facilities Manager");
                CurrentModule_Sub(string.Empty);
            }
        }
        
        private void LoadLookupTable()
        {
            ILookupService _service = new LookupService();
            Cache["SRTSLOOKUP"] = _service.GetAllLookups();
        }

        #region Accessors

        public List<LookupTableEntity> CacheData
        {
            get 
            {
                if (Cache["SRTSLOOKUP"] == null)
                {
                    LoadLookupTable();
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
            get;

            set;
        }

        public bool NewSite
        {
            get;

            set;
        }

        public bool HasLMS
        {
            get
            {
                if (tbHasLMS.Text.ToUpper() == "TRUE")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            set { tbHasLMS.Text = value.ToString(); }
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

        private List<string> _siteCodes;

        public List<string> SiteCodes
        {
            get { return _siteCodes; }
            set
            {
                _siteCodes = value;
                ddlSiteCode.Items.Clear();
                ddlSiteCode.DataSource = _siteCodes;
                ddlSiteCode.DataBind();
            }
        }

        public string MultiPrimary
        {
            get { return tbMPrimary.Text; }
            set { tbMPrimary.Text = value.ToString(); }
        }

        public string MultiSecondary
        {
            get;

            set;
        }

        public string SinglePrimary
        {
            get { return tbSPrimary.Text; }
            set { tbSPrimary.Text = value.ToString(); }
        }

        public string SingleSecondary
        {
            get;

            set;
        }

        public List<string> MPrimary
        {
            get;

            set;
        }

        public List<string> SPrimary
        {
            get;

            set;
        }

        public List<string> MSecondary
        {
            get;

            set;
        }

        public List<string> SSecondary
        {
            get;

            set;
        }

        public int Region
        {
            get;

            set;
        }

        public string SiteCode
        {
            get { return ddlSiteCode.SelectedValue; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    ddlSiteCode.SelectedValue = value;
                    SiteCodetb = value;
                }
                else
                {
                    ddlSiteCode.SelectedIndex = -1;
                }
            }
        }

        public string SiteCodetb
        {
            get;

            set;
        }

        public string SiteName
        {
            get { return tbSiteName.Text; }
            set { tbSiteName.Text = value; }
        }

        public string SiteType
        {
            get { return tbSiteType.Text; }
            set { tbSiteType.Text = value; }
        }

        public string SiteDescription
        {
            get { return tbSiteDescription.Text; }
            set { tbSiteDescription.Text = value; }
        }

        public string BOS
        {
            get { return tbBOS.Text; }
            set { tbBOS.Text = value; }
        }

        public bool IsMultivision
        {
            get { return Convert.ToBoolean(tbMultivision.Text); }
            set { tbMultivision.Text = value.ToString(); }
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
            get { return tbState.Text; }
            set { tbState.Text = value; }
        }

        public string Country
        {
            get { return tbCountry.Text; }
            set { tbCountry.Text = value; }
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
            get { return tbMailState.Text; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    tbMailState.Text = value;
                }
                else
                {
                    tbMailState.Text = string.Empty;
                }
            }
        }

        public string MailCountry
        {
            get { return tbMailCountry.Text; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    tbMailCountry.Text = value;
                }
                else
                {
                    tbMailCountry.Text = string.Empty;
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
            get { return Convert.ToBoolean(tbIsConus.Text); }
            set { tbIsConus.Text = value.ToString(); }
        }

        public bool IsConusMail
        {
            get { return Convert.ToBoolean(tbMailIsConus.Text); }
            set { tbMailIsConus.Text = value.ToString(); }
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
            get { return Convert.ToInt32(tbMaxEyeSize.Text); }
            set { tbMaxEyeSize.Text = value.ToString(); }
        }

        public int MaxFramesPerMonth
        {
            get { return Convert.ToInt32(tbMaxFrames.Text); }
            set { tbMaxFrames.Text = value.ToString(); }
        }

        public double MaxPower
        {
            get { return Convert.ToDouble(tbMaxPower.Text); }
            set { tbMaxPower.Text = value.ToString(); }
        }

        public bool IsActive
        {
            get { return Convert.ToBoolean(tbIsActive.Text); }
            set { tbIsActive.Text = value.ToString(); }
        }

        public List<LookupTableEntity> StateData
        {
            get;

            set;
        }

        public List<LookupTableEntity> CountryData
        {
            get;
            set;
        }

        public List<LookupTableEntity> BOSData
        {
            get;

            set;
        }

        #endregion Accessors
    }
}