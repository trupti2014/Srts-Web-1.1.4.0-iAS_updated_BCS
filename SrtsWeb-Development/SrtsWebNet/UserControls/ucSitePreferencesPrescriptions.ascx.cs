using SrtsWeb.Account;
using SrtsWeb.Base;
using SrtsWeb.BusinessLayer.Concrete;
using SrtsWeb.Entities;
using SrtsWeb.ExtendersHelpers;
using SrtsWeb.Presenters.Admin;
using SrtsWeb.Views.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SrtsWeb.UserControls
{
    [PrincipalPermission(SecurityAction.Demand, Role = "MgmtEnterprise")]
    [PrincipalPermission(SecurityAction.Demand, Role = "MgmtAdmin")]
    [PrincipalPermission(SecurityAction.Demand, Role = "LabAdmin")]
    [PrincipalPermission(SecurityAction.Demand, Role = "ClinicAdmin")]
    [PrincipalPermission(SecurityAction.Demand, Role = "TrainingAdmin")]

    public partial class ucSitePreferencesPrescriptions : UserControlBase, IPrescriptionPreferencesView
    {
        private SitePreferencesPresenter.PrescriptionPreferencesPresenter p;

        public ucSitePreferencesPrescriptions()
        {
            p = new SitePreferencesPresenter.PrescriptionPreferencesPresenter(this);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            //if (!IsPostBack)
            //    p.InitView();

            ((SrtsWeb.WebForms.Admin.SitePreferences)this.Page).RefreshUserControl += ucSitePreferencesPrescriptions_RefreshUserControl;

            if (!IsPostBack)
            {
                p.InitView();
            }
        }

        private void ucSitePreferencesPrescriptions_RefreshUserControl(object sender, EventArgs e)
        {
            p.InitView();
        }

        private void BindPrescriptionProviders()
        {
            this.ddlPrescriptionProvider.DataSource = this.ProviderList;
            this.ddlPrescriptionProvider.DataTextField = "NameLFMi";
            this.ddlPrescriptionProvider.DataValueField = "ID";
            this.ddlPrescriptionProvider.DataBind();
            this.ddlPrescriptionProvider.Items.Insert(0, new ListItem("-Select-", "X"));
            this.ddlPrescriptionProvider.Items.Insert(1, new ListItem("-No Default-", "N"));
        }

        public List<IndividualEntity> ProviderList
        {
            get { return ViewState["ProviderList"] as List<IndividualEntity>; }
            set { ViewState["ProviderList"] = value; BindPrescriptionProviders(); }
        }

        public SitePrefRxEntity SitePrefsRX
        {
            get { return ViewState["SitePrefsRX"] as SitePrefRxEntity; }
            set { ViewState["SitePrefsRX"] = value; }
        }

        protected void bUpdateRxPref_Click(object sender, EventArgs e)
        {
            var msg = String.Empty;

            SitePrefsRX.ModifiedBy = mySession.MyUserID;
            SitePrefsRX.SiteCode = SiteCode;
            SitePrefsRX.PDDistance = PDDistance;
            SitePrefsRX.PDNear = PDNear;

            msg = p.UpdateRxDefaults(SitePrefsRX) ? "Succes" : "Fail";

            if (msg == "Fail")
                lblMessage.Text = "There was an error updating your prescription defaults.";
            else
                ShowConfirmDialog("Your prescription defaults were successfully updated!");

            p.FillView();
        }

        private void ShowConfirmDialog(String msg)
        {
            /// Show global confirm dialog
            ScriptManager.RegisterStartupScript(this, GetType(), "DisplayDialogMessage", "displaySrtsMessage('Success!','" + msg + "', 'success');", true);
        }

        #region Interface Members

        public decimal? PDDistance
        {
            get
            {
                if (String.IsNullOrEmpty(this.tbPD.Text))
                    return null;
                else
                    return Convert.ToDecimal(tbPD.Text);
            }
            set
            {
                tbPD.Text = value.IsNull() ? String.Empty : String.Format("{0:0.00}", value);

                if (!value.IsNull())
                {
                    rblPDDistance.SelectedValue = "DEFAULT";
                }
                else
                {
                    rblPDDistance.SelectedValue = "REQUIRE";
                }
            }
        }

        public decimal? PDNear
        {
            get
            {
                if (String.IsNullOrEmpty(this.tbPDNear.Text))
                    return null;
                else
                    return Convert.ToDecimal(tbPDNear.Text);
            }
            set
            {
                tbPDNear.Text = value.IsNull() ? String.Empty : String.Format("{0:0.00}", value);
                if (!value.IsNull())
                {
                    rblPDNear.SelectedValue = "DEFAULT";
                }
                else
                {
                    rblPDNear.SelectedValue = "REQUIRE";
                }
            }
        }

        public int? Provider
        {
            get
            {
                if (ddlPrescriptionProvider.SelectedValue.Equals("N"))
                    return null;
                else
                    return Convert.ToInt32(ddlPrescriptionProvider.SelectedValue);
            }
            set { ddlPrescriptionProvider.SelectedValue = value.Equals(null) ? "N" : value.ToString(); }
        }

        public string RxType
        {
            get { return ddlPrescriptionName.SelectedValue; }
            set { ddlPrescriptionName.SelectedValue = value.IsNullOrEmpty() ? "X" : value; }
        }

        public string SiteCode
        {
            get
            {
                var pg = ((ISitePreferencesView)this.Page);
                return pg.SiteCode;
            }
        }

        #endregion Interface Members

        protected void ddlPrescriptionName_SelectedIndexChanged(object sender, EventArgs e)
        {
            SitePrefsRX.RxType = RxType;
        }

        protected void ddlPrescriptionProvider_SelectedIndexChanged(object sender, EventArgs e)
        {
            SitePrefsRX.ProviderId = Provider;
        }
    }
}