using SrtsWeb;
using SrtsWeb.BusinessLayer.Presenters.Exams;
using SrtsWeb.BusinessLayer.TypeExtendersAndHelpers.Extenders;
using SrtsWeb.BusinessLayer.Views.Exams;
using SrtsWeb.CustomErrors;
using SrtsWeb.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Permissions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SrtsWebClinic.Exams
{
    [PrincipalPermission(SecurityAction.Demand, Role = "ClinicTech")]
    [PrincipalPermission(SecurityAction.Demand, Role = "ClinicClerk")]
    [PrincipalPermission(SecurityAction.Demand, Role = "ClinicProvider")]
    [PrincipalPermission(SecurityAction.Demand, Role = "HumanTech")]
    [PrincipalPermission(SecurityAction.Demand, Role = "MgmtEnterprise")]
    public partial class ExamManagementEdit : PageBase, IExamManagementEditView, ISiteMapResolver
    {
        private ExamManagementEditPresenter _presenter;

        public ExamManagementEdit()
        {
            _presenter = new ExamManagementEditPresenter(this);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            mySession.MainContentTitle = "Edit Patients Exam";
            Master.CurrentModuleTitle = "Edit Exam";
            if (!Page.IsPostBack)
            {
                _presenter.InitView();
                tbExamDate.Focus();
                this.ceExamDate.EndDate = DateTime.Today;

                if (User.IsInRole("ClinicTech"))
                {
                    btnAdd.Enabled = true;
                }
                else
                {
                    btnAdd.Enabled = false;
                }
            }
            else
            {
                WebControl wcICausedPostBack = (WebControl)GetControlThatCausedPostBack(sender as Page);
                int indx = wcICausedPostBack.TabIndex;
                var ctrl = from control in wcICausedPostBack.Parent.Controls.OfType<WebControl>()
                           where control.TabIndex > indx
                           select control;
                ctrl.DefaultIfEmpty(wcICausedPostBack).First().Focus();
            }
        }

        public SiteMapNode BuildBreadCrumbs(object sender, SiteMapResolveEventArgs e)
        {
            SiteMapNode parent = new SiteMapNode(e.Provider, "1", "~/Default.aspx", "My SRTSWeb");
            SiteMapNode child = new SiteMapNode(e.Provider, "2", "~/SrtsWebClinic/Patients/ManagePatients.aspx/search", "Manage Patients Search");
            child.ParentNode = parent;
            SiteMapNode child2 = new SiteMapNode(e.Provider, "3", "~/SrtsWebClinic/Patients/PatientDetails.aspx", "Patients Details");
            child2.ParentNode = child;
            SiteMapNode child3 = new SiteMapNode(e.Provider, "4", "~/SrtsWebClinic/Exams/ExamManagementEdit.aspx", "Edit Exam");
            child3.ParentNode = child2;
            return child3;
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            _presenter.SaveData();
            Response.Redirect("../Patients/PatientDetails.aspx?view=exam&tab=2");
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("../Patients/PatientDetails.aspx");
        }

        #region Input Validators

        protected void ValidateCommentFormat(object source, ServerValidateEventArgs args)
        {
            if (args.Value.Length > 0)
            {
                int limit = 256;
                if (args.IsValid = args.Value.ValidateCommentLength(limit))
                {
                    if (args.IsValid = args.Value.ValidateCommentFormat())
                    {
                    }
                    else
                    {
                        cvComment.ErrorMessage = "Invalid characters in Comments";
                    }
                }
                else
                {
                    cvComment.ErrorMessage = string.Format("Limit is {0} characters in Comments", limit.ToString());
                }
            }
        }

        #endregion Input Validators

        #region Accessors Exam

        public DateTime ExamDate
        {
            get
            {
                DateTime d2;
                bool success = DateTime.TryParse(tbExamDate.Text, out d2);
                if (success)
                { return d2; }
                else
                { return DateTime.Now; }
            }
            set { tbExamDate.Text = value.ToMilDateString(); }
        }

        public string ODUncorrected
        {
            get { return ddlODUncorrected.SelectedValue; }
            set { ddlODUncorrected.SelectedValue = value; }
        }

        private DataTable _acuityValues;

        public DataTable AcuityValues
        {
            get { return _acuityValues; }
            set
            {
                _acuityValues = value;
                ddlODCorrected.Items.Clear();
                ddlODCorrected.DataSource = _acuityValues;
                ddlODCorrected.DataTextField = "Text";
                ddlODCorrected.DataValueField = "Value";
                ddlODCorrected.DataBind();

                ddlODOSCorrected.Items.Clear();
                ddlODOSCorrected.DataSource = _acuityValues;
                ddlODOSCorrected.DataTextField = "Text";
                ddlODOSCorrected.DataValueField = "Value";
                ddlODOSCorrected.DataBind();

                ddlODOSUnCorrected.Items.Clear();
                ddlODOSUnCorrected.DataSource = _acuityValues;
                ddlODOSUnCorrected.DataTextField = "Text";
                ddlODOSUnCorrected.DataValueField = "Value";
                ddlODOSUnCorrected.DataBind();

                ddlODUncorrected.Items.Clear();
                ddlODUncorrected.DataSource = _acuityValues;
                ddlODUncorrected.DataTextField = "Text";
                ddlODUncorrected.DataValueField = "Value";
                ddlODUncorrected.DataBind();

                ddlOSCorrected.Items.Clear();
                ddlOSCorrected.DataSource = _acuityValues;
                ddlOSCorrected.DataTextField = "Text";
                ddlOSCorrected.DataValueField = "Value";
                ddlOSCorrected.DataBind();

                ddlOSUnCorrected.Items.Clear();
                ddlOSUnCorrected.DataSource = _acuityValues;
                ddlOSUnCorrected.DataTextField = "Text";
                ddlOSUnCorrected.DataValueField = "Value";
                ddlOSUnCorrected.DataBind();
            }
        }

        public string OSUncorrected
        {
            get { return ddlOSUnCorrected.SelectedValue; }
            set { ddlOSUnCorrected.SelectedValue = value; }
        }

        public string ODOSUncorrected
        {
            get { return ddlODOSUnCorrected.SelectedValue; }
            set { ddlODOSUnCorrected.SelectedValue = value; }
        }

        public string ODCorrected
        {
            get { return ddlODCorrected.SelectedValue; }
            set { ddlODCorrected.SelectedValue = value; }
        }

        public string OSCorrected
        {
            get { return ddlOSCorrected.SelectedValue; }
            set { ddlOSCorrected.SelectedValue = value; }
        }

        public string ODOSCorrected
        {
            get { return ddlODOSCorrected.SelectedValue; }
            set { ddlODOSCorrected.SelectedValue = value; }
        }

        public string ExamComments
        {
            get { return tbComments.Text; }
            set { tbComments.Text = value; }
        }

        public List<PersonnelEntity> TechData
        {
            set
            {
                ddlTechnician.Items.Clear();
                ddlTechnician.DataSource = value;
                ddlTechnician.DataBind();
            }
        }

        public List<PersonnelEntity> DoctorsData
        {
            set
            {
                ddlDoctors.Items.Clear();
                ddlDoctors.DataSource = value;
                ddlDoctors.DataBind();
            }
        }

        public int TechID
        {
            get { return SrtsExtender.GetIntVal(ddlTechnician.SelectedValue); }
            set { ddlTechnician.SelectedValue = value.ToString(); }
        }

        public int DoctorID
        {
            get { return SrtsExtender.GetIntVal(ddlDoctors.SelectedValue); }
            set { ddlDoctors.SelectedValue = value.ToString(); }
        }

        #endregion Accessors Exam
    }
}