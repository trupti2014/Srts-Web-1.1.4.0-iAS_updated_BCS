using SrtsWeb.Base;
using SrtsWeb.Entities;
using SrtsWeb.Presenters.Admin;
using SrtsWeb.Views.Admin;
using System;
using System.Collections.Generic;
using System.Security.Permissions;
using System.Web;
using System.Web.UI.WebControls;

namespace SrtsWeb.UserControls
{
    [PrincipalPermission(SecurityAction.Demand, Role = "MgmtEnterprise")]
    [PrincipalPermission(SecurityAction.Demand, Role = "MgmtAdmin")]
    [PrincipalPermission(SecurityAction.Demand, Role = "LabAdmin")]
    [PrincipalPermission(SecurityAction.Demand, Role = "TrainingAdmin")]
    [PrincipalPermission(SecurityAction.Demand, Role = "ClinicAdmin")]

    public partial class ucSitePreferencesRoutingOrders : UserControlBase, ISitePreferencesRoutingOrdersView
    {
        private SitePreferencesPresenter.RoutingOrdersPresenter p;

        public ucSitePreferencesRoutingOrders()
        {
            p = new SitePreferencesPresenter.RoutingOrdersPresenter(this);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.mySession.MyIndividualID = SrtsWeb.Account.CustomProfile.GetProfile(HttpContext.Current.User.Identity.Name).IndividualId;
            ((SrtsWeb.WebForms.Admin.SitePreferences)this.Page).RefreshUserControl += ucRoutingOrdersOrders_RefreshUserControl;
            if (!IsPostBack)
            {
                this.p = new SitePreferencesPresenter.RoutingOrdersPresenter(this);
                this.p.InitView();
                BindLabCapacityHistoryData();
            }
            this.btnSaveRoutingOrders.Text = this.p.HasCurrentLabCapacity() ? "Update" : "Save";
        }

        private void ucRoutingOrdersOrders_RefreshUserControl(object sender, EventArgs e)
        {
            this.p = new SitePreferencesPresenter.RoutingOrdersPresenter(this);
            this.p.InitView();
            BindLabCapacityHistoryData();

        }

        protected void gvLabCapacityHistory_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow) return;

            e.Row.Attributes.Add("onmouseover", "this.style.backgroundColor='#CCCCCC'; this.style.cursor='pointer';");
            e.Row.Attributes.Add("onmouseout", "this.style.backgroundColor=''; this.style.textDecoration='none';");
        }


        protected void btnSaveRoutingOrdersPref_Click(object sender, EventArgs e)
        {
            var m = string.Empty;
            if (!p.InsertRoutingOrdersPref())
            {
                    this.hfMsgRO.Value = "There was an error updating your Routing Orders preferences.";
                    m = this.hfMsgRO.Value;
                    return;
            }
            this.hfSuccessRO.Value = "1";
            this.hfMsgRO.Value = "Your Routing Orders preferences were successfully updated!";

            m = this.hfMsgRO.Value;
            LogEvent("User {0} {1} parameter restrictions to site {2} at {3}.", new Object[] { mySession.MyUserID, m, this.SiteCode, DateTime.Now });
            p.FillLabCapacityHistory();
            BindLabCapacityHistoryData();

        }

        #region INTERFACES

        public List<SitePrefRoutingOrdersEntity> LabCapacityHistoryData
        {
            get { return (List<SitePrefRoutingOrdersEntity>)Session["LabCapacityHistoryData"]; }
            set { Session.Add("LabCapacityHistoryData", value); }
        }



        private void BindLabCapacityHistoryData()
        {
            //gvLabCapacityHistory.DataKeyNames = new[] { "ID" };
            gvLabCapacityHistory.DataSource = LabCapacityHistoryData;
            gvLabCapacityHistory.DataBind();
        }

        private string _siteCode;
        public string SiteCode
        {
            get
            {
                var pg = ((ISitePreferencesView)this.Page);
                return pg.SiteCode;
            }
            set
            {
                _siteCode = value;
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

        private SitePrefRoutingOrdersEntity _sitePrefRoutingOrdersEntity;
        public SitePrefRoutingOrdersEntity SitePrefRoutingOrdersEntity
        {
            get
            {
                return _sitePrefRoutingOrdersEntity;
            }
            set
            {
                _sitePrefRoutingOrdersEntity = value;
            }
        }



        public int Capacity
        {
            get { return Convert.ToInt32(tbDailyCapacity.Text); }
            set { tbDailyCapacity.Text = value.ToString(); }
        }


        public bool PDO
        {
            get { return cbParticipatePDO.Checked; }
            set { cbParticipatePDO.Checked = Convert.ToBoolean(value); }
        }

        #endregion
    }
}