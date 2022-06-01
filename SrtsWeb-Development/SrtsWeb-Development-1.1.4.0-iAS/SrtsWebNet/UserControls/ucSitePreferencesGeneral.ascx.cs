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

    public partial class ucSitePreferencesGeneral : UserControlBase, ISitePreferencesGeneral
    {
        private SitePreferencesPresenter.GeneralPreferencesPresenter p;

        public ucSitePreferencesGeneral()
        {

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            ((SrtsWeb.WebForms.Admin.SitePreferences)this.Page).RefreshUserControl += ucSitePreferencesOrders_RefreshUserControl;

            if (!IsPostBack)
            {
                this.p = new SitePreferencesPresenter.GeneralPreferencesPresenter(this);
                this.p.GetPreferences();
            }
        }

        private void ucSitePreferencesOrders_RefreshUserControl(object sender, EventArgs e)
        {
            this.p = new SitePreferencesPresenter.GeneralPreferencesPresenter(this);
            this.p.GetPreferences();
        }

        protected void bUpdateGeneralPref_Click(object sender, EventArgs e)
        {
            this.p = new SitePreferencesPresenter.GeneralPreferencesPresenter(this);
            if (!this.p.SetPreferencesToDb()) return;
            this.hfMsgGeneral.Value = String.Format("Successfully updated general preferences.");
            this.hfSuccessGeneral.Value = "1";
        }



        #region Interface Members

        public bool LabelSortedAlpha
        {
            get
            {
                return this.cbAlphaSort.Checked;
            }
            set
            {
                this.cbAlphaSort.Checked = value;
            }
        }

        public string SiteCode
        {
            get
            {
                var pg = ((ISitePreferencesView)this.Page);
                return pg.SiteCode;
            }
        }

        public IEnumerable<SiteCodeEntity> SiteCodes
        {
            get
            {
                var pg = ((ISitePreferencesView)this.Page);
                return pg.SiteCodes;
            }
        }

        #endregion Interface Members
    }
}