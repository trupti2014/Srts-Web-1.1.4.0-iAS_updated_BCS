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

    public partial class ucSitePreferencesShipping : UserControlBase, ISitePreferencesShipping
    {
        private SitePreferencesPresenter.ShippingPreferencesPresenter p;

        public ucSitePreferencesShipping()
        {
            this.p = new SitePreferencesPresenter.ShippingPreferencesPresenter(this);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            ((SrtsWeb.WebForms.Admin.SitePreferences)this.Page).RefreshUserControl += ucSitePreferencesShipping_RefreshUserControl;

            if (!IsPostBack)
            {
                //this.p = new SitePreferencesPresenter.ShippingPreferencesPresenter(this);
                //this.p.GetShippingProviders();
                p.InitView();
            }
        }

        private void ucSitePreferencesShipping_RefreshUserControl(object sender, EventArgs e)
        {
            //this.p = new SitePreferencesPresenter.ShippingPreferencesPresenter(this);
            //this.p.GetShippingProviders();
            p.InitView();
        }

        protected void bUpdateShippingPref_Click(object sender, EventArgs e)
        {
            this.p = new SitePreferencesPresenter.ShippingPreferencesPresenter(this);
            if (!this.p.SetShippingPreferences()) return;
            this.hfMsgShipping.Value = String.Format("Successfully updated shipping preferences.");
            this.hfSuccessShipping.Value = "1";
        }



        #region Interface Members


        public string SiteCode
        {
            get
            {
                var pg = ((ISitePreferencesView)this.Page);
                return pg.SiteCode;
            }
        }

        public string Shipper
        {
            get
            {
                return this.ddlShippingProvider.SelectedValue.Equals("X") ? String.Empty : this.ddlShippingProvider.SelectedValue;
            }
            set
            {
                this.ddlShippingProvider.SelectedValue = value;
            }
        }

        public IEnumerable<LookupTableEntity> ShippingProviderList
        {
            get
            {
                return ViewState["ShippingProviderList"] as List<LookupTableEntity> ?? new List<LookupTableEntity>();
            }
            set
            {
                ViewState["ShippingProviderList"] = value;

                this.ddlShippingProvider.DataTextField = "Text";
                this.ddlShippingProvider.DataValueField = "Value";

                this.ddlShippingProvider.DataSource = value;
                this.ddlShippingProvider.DataBind();
                this.ddlShippingProvider.Items.Insert(0, new ListItem("-Select-", "X"));


                //BindDdl(this.ddlShippingProvider, value);
            }
        }

        #endregion Interface Members
    }
}