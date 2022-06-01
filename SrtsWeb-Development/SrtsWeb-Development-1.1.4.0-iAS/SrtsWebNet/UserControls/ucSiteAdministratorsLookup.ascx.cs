using SrtsWeb.Base;
using SrtsWeb.Entities;
using SrtsWeb.ExtendersHelpers;
using SrtsWeb.Presenters.Admin;
using SrtsWeb.Views.Admin;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Permissions;
using System.Web.UI;
using System.Web.UI.WebControls;
using SrtsWeb.BusinessLayer.Concrete;

namespace SrtsWeb.UserControls
{
    [PrincipalPermission(SecurityAction.Demand, Role = "MgmtEnterprise")]
    [PrincipalPermission(SecurityAction.Demand, Role = "MgmtAdmin")]


    public partial class ucSiteAdministratorsLookup : UserControlBase, ISiteAdministratorsView
    {
        private SiteAdministratorsPresenter p;

        public ucSiteAdministratorsLookup()
        {
            p = new SiteAdministratorsPresenter(this);
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                p.GetSites(AdminSiteType);
            }
        }


        protected void ddlSiteCode_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.SiteCodeTxtBox = String.Empty;
            lblError.Visible = false;
            lvSiteAdminSearchResults.DataSource = null;
            lvSiteAdminSearchResults.DataBind();

            List<SiteAdministratorEntity> sae;
            sae = p.SearchSiteAdministrators(this.SiteCodeDDL);

            lvSiteAdminSearchResults.DataSource = sae;
            lvSiteAdminSearchResults.DataBind();
        }

        //protected void cvtbSiteCode_ServerValidate(object source, ServerValidateEventArgs args)
        //{

        //        if (String.IsNullOrEmpty(this.SiteCodeTxtBox))
        //            args.IsValid = false;
        //        else if (!SiteCodes.Any(s => s.SiteCode == this.SiteCodeTxtBox))
        //            args.IsValid = false;
        //        else
        //            args.IsValid = true;

        //}

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                this.SiteCodeDDL = "X";
                lblError.Visible = false;
                lvSiteAdminSearchResults.DataSource = null;
                lvSiteAdminSearchResults.DataBind();

                var ModifiedBy = string.IsNullOrEmpty(mySession.MyUserID) ? Globals.ModifiedBy : mySession.MyUserID;
#if DEBUG
                using (MethodTracer.Trace(SrtsTraceSource.AdminSource, "ucSiteAdministratorsLookup_btnSearch_Click", ModifiedBy))
#endif
                {

                    if (!String.IsNullOrEmpty(this.SiteCodeTxtBox) && SiteCodes.Any(s => s.SiteCode.Equals(this.SiteCodeTxtBox, StringComparison.OrdinalIgnoreCase)) )
                    {

                        List<SiteAdministratorEntity> sae;
                        sae = p.SearchSiteAdministrators(this.SiteCodeTxtBox);


                        lvSiteAdminSearchResults.DataSource = sae;
                        lvSiteAdminSearchResults.DataBind();

                    }
                    else
                    {
                        lblError.Visible = true;
                    }
                    
                }
            }
            catch (Exception ex) { ex.TraceErrorException(); }

        }

        #region INTERFACE PROPERTIES

        public SiteType AdminSiteType { get; set; }


        public IEnumerable<SiteCodeEntity> SiteCodes
        {
            get
            {
                return ViewState["SiteCodes"] as List<SiteCodeEntity> ?? new List<SiteCodeEntity>();
            }
            set
            {
                ViewState["SiteCodes"] = value;
                this.ddlSiteCode.DataSource = value;
                this.ddlSiteCode.DataTextField = "SiteCombinationProfile";
                this.ddlSiteCode.DataValueField = "SiteCode";
                this.ddlSiteCode.DataBind();
                this.ddlSiteCode.Items.Insert(0, new ListItem("-Select-", "X"));
            }
        }

        public string SiteCodeDDL
        {
            get
            {
                return this.ddlSiteCode.SelectedValue;
            }
            set
            {
                this.ddlSiteCode.SelectedValue = value;
            }
        }

        public string SiteCodeTxtBox
        {
            get
            {
                int i;
                char pad = '0';
                bool result = int.TryParse(this.tbSiteCode.Text, out i);
                if (result && this.tbSiteCode.Text.Length < 6)
                    return this.tbSiteCode.Text.PadLeft(6, pad);
                else if(this.tbSiteCode.Text.Any(char.IsLower))
                    return this.tbSiteCode.Text.ToUpper();
                else
                    return this.tbSiteCode.Text;
            }
            set
            {
                this.tbSiteCode.Text = value;
            }
        }

        #endregion INTERFACE PROPERTIES

    }
}