using SrtsWeb.Base;
using SrtsWeb.BusinessLayer.Concrete;
using SrtsWeb.Entities;
using SrtsWeb.ExtendersHelpers;
using SrtsWeb.Presenters.Admin;
using SrtsWeb.Views.Admin;
using System;
using System.Collections.Generic;
using System.Security.Permissions;

namespace SrtsWeb.Admin
{
    [PrincipalPermission(SecurityAction.Demand, Role = "MgmtEnterprise")]
    public partial class LmsFileGenerator : PageBase, ILmsFileGeneratorView
    {
        protected void Page_Load(object sender, EventArgs e)
        {
#if DEBUG
            using (MethodTracer.Trace(SrtsTraceSource.AdminSource, "LmsFileGenerator_Page_Load", mySession.MyUserID))
#endif
                if (!Page.IsPostBack)
                {
                    var p = new LmsFileGeneratorPresenter(this);
                    p.GetLmsSites();
                }
        }

        protected void bGetData_Click(object sender, EventArgs e)
        {
#if DEBUG
            using (MethodTracer.Trace(SrtsTraceSource.AdminSource, "LmsFileGenerator_bGetData_Click", mySession.MyUserID))
#endif
            {
                DoFileGenerator();
                LogEvent(String.Format("User {0} attempted to generate LMS file data at {1}.", mySession.MyUserID, DateTime.Now));
            }
        }

        private void DoFileGenerator()
        {
#if DEBUG
            using (MethodTracer.Trace(SrtsTraceSource.AdminSource, "LmsFileGenerator_DoFileGenerator", mySession.MyUserID))
#endif
            {
                var p = new LmsFileGeneratorPresenter(this);
                var badOrders = new List<String>();

                try
                {
                    p.GetLmsFileData();
                }
                catch (Exception ex)
                {
                    ex.LogException();
                }
            }
        }

        public List<string> BadOrders
        {
            get
            {
                return ViewState["BadOrders"] as List<String>;
            }
            set
            {
                ViewState["BadOrders"] = value;
                this.gvUnprocessedOrders.DataSource = value;
                this.gvUnprocessedOrders.DataBind();
            }
        }

        public List<string> GoodOrders
        {
            get
            {
                return ViewState["GoodOrders"] as List<string>;
            }
            set
            {
                ViewState["GoodOrders"] = value;
                this.gvProcessedOrders.DataSource = value;
                this.gvProcessedOrders.DataBind();
            }
        }

        public string ErrorMessage
        {
            get
            {
                return ViewState["ErrorOrders"].ToString();
            }
            set
            {
                ViewState["ErrorOrders"] = value;
                this.litError.Text = value;
            }
        }

        public List<SiteCodeEntity> LabList
        {
            set
            {
                this.ddlLabList.DataSource = value;
                this.ddlLabList.DataTextField = "SiteCode";
                this.ddlLabList.DataValueField = "SiteCode";
                this.ddlLabList.DataBind();
            }
        }

        public string SiteCode
        {
            get
            {
                return this.ddlLabList.SelectedValue;
            }
        }

        public bool MarkComplete
        {
            get
            {
                return this.rblMarkComplete.SelectedValue.Equals("Y");
            }
        }
    }
}