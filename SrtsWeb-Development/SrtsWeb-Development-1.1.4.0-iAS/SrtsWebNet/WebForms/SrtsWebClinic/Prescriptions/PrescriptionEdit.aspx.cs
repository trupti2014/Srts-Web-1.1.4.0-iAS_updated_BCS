using SrtsWeb;
using SrtsWeb.BusinessLayer.Presenters.Prescriptions;
using SrtsWeb.BusinessLayer.Views.Prescriptions;
using SrtsWeb.CustomErrors;
using SrtsWeb.Entities;
using System;
using System.Collections.Generic;
using System.Security.Permissions;
using System.Web;
using System.Web.Security;
using System.Web.UI.WebControls;

namespace SrtsWebClinic.Prescriptions
{
    [PrincipalPermission(SecurityAction.Demand, Role = "ClinicTech")]
    [PrincipalPermission(SecurityAction.Demand, Role = "ClinicClerk")]
    [PrincipalPermission(SecurityAction.Demand, Role = "ClinicProvider")]
    [PrincipalPermission(SecurityAction.Demand, Role = "HumanTech")]
    public partial class PrescriptionEdit : PageBase, IPrescriptionEditView, ISiteMapResolver
    {
        private PrescriptionEditPresenter _presenter;

        public PrescriptionEdit()
        {
            _presenter = new PrescriptionEditPresenter(this);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            if (!IsPostBack)
            {
                _presenter.InitView();
                CommitSave = false;

                bSave.Enabled = false;
                bUpdate.Enabled = false;

                if (!string.IsNullOrEmpty(msg))
                {
                    CustomValidator cv = new CustomValidator();
                    cv.IsValid = false;
                    cv.ErrorMessage = msg;
                    this.Page.Validators.Add(cv);
                    return;
                }
                else
                {
                    if (Roles.IsUserInRole("ClinicTech"))
                    {
                        bSave.Enabled = true;
                        this.bUpdate.Enabled = !this.IsUsed;
                        this.chkboxRemove.Enabled = this.IsDeletable;
                    }
                }
            }
            //Master.CurrentModuleTitle = "Edit Prescription";
            litPatientNameHeader.Text = string.Format("{0}", mySession.Patient.Individual.NameFMiL);
        }

        public SiteMapNode BuildBreadCrumbs(object sender, SiteMapResolveEventArgs e)
        {
            SiteMapNode parent = new SiteMapNode(e.Provider, "1", "~/Default.aspx", "My SRTSWeb");
            SiteMapNode child = new SiteMapNode(e.Provider, "2", "~/SrtsWebClinic/Patients/ManagePatients.aspx/search", "Manage Patients Search");
            child.ParentNode = parent;
            SiteMapNode child2 = new SiteMapNode(e.Provider, "3", "~/SrtsWebClinic/Patients/PatientDetails.aspx", "Patients Details");
            child2.ParentNode = child;
            SiteMapNode child3 = new SiteMapNode(e.Provider, "4", "~/SrtsWebClinic/Prescriptions/PrescriptionEdit.aspx", "Edit Prescription");
            child3.ParentNode = child2;
            return child3;
        }

        protected void bSave_Click(object sender, EventArgs e)
        {
            mySession.TempID = 0;
            IsMonoCalculation = rblPDMode.SelectedIndex == 0 ? false : true;

            _presenter.SaveData();
            if (!string.IsNullOrEmpty(msg))
            {
                CustomValidator cv = new CustomValidator();
                cv.IsValid = false;
                cv.ErrorMessage = msg;
                this.Page.Validators.Add(cv);
                return;
            }
            else
            {
                Response.Redirect("../Patients/PatientDetails.aspx?tab=2");
            }
        }

        protected void bCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("../Patients/PatientDetails.aspx?tab=2");
        }

        protected void bUpdate_Click(object sender, EventArgs e)
        {
            IsMonoCalculation = rblPDMode.SelectedIndex == 0 ? false : true;
            _presenter.SaveData();
            if (!string.IsNullOrEmpty(msg))
            {
                CustomValidator cv = new CustomValidator();
                cv.IsValid = false;
                cv.ErrorMessage = msg;
                this.Page.Validators.Add(cv);
                return;
            }
            else
            {
                Response.Redirect("../Patients/PatientDetails.aspx?tab=2");
            }
        }

        #region PageEvents

        protected void lboxODHPrism_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ODHPrism == "0.00")
            {
                ddlODHBase.SelectedValue = "";
            }
        }

        protected void lboxODVPrism_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ODVPrism == "0.00")
            {
                ddlODVBase.SelectedValue = "";
            }
        }

        protected void lboxOSHPrism_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (OSHPrism == "0.00")
            {
                ddlOSHBase.SelectedValue = "";
            }
        }

        protected void lboxOSVPrism_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (OSVPrism == "0.00")
            {
                ddlOSVBase.SelectedValue = "";
            }
        }

        protected void ddlOSVBase_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (OSVBase == "")
            {
                lboxOSVPrism.SelectedIndex = lboxOSVPrism.Items.IndexOf(lboxOSVPrism.Items.FindByValue("0.00"));
            }
        }

        protected void ddlOSHBase_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (OSHBase == "")
            {
                lboxOSHPrism.SelectedIndex = lboxOSHPrism.Items.IndexOf(lboxOSHPrism.Items.FindByValue("0.00"));
            }
        }

        protected void ddlODHBase_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ODHBase == "")
            {
                lboxODHPrism.SelectedIndex = lboxODHPrism.Items.IndexOf(lboxODHPrism.Items.FindByValue("0.00"));
            }
        }

        protected void ddlODVBase_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ODVBase == "")
            {
                lboxODVPrism.SelectedIndex = lboxODVPrism.Items.IndexOf(lboxODVPrism.Items.FindByValue("0.00"));
            }
        }

        #endregion PageEvents

        #region Accessors

        private bool _commitSave;

        public bool CommitSave
        {
            get { return _commitSave; }
            set { _commitSave = value; }
        }

        private string _msg;

        public string msg
        {
            get { return _msg; }
            set { _msg = value; }
        }

        public string ODSphere
        {
            get { return lboxODSphere.SelectedValue; }
            set { lboxODSphere.SelectedValue = value; }
        }

        public string ODSphere_calc
        {
            get { return hfODSphereCalc.Value; }
        }

        public string ODCylinder
        {
            get { return lboxODCylinder.SelectedValue; }
            set { lboxODCylinder.SelectedValue = value; }
        }

        public string ODCylinder_calc
        {
            get { return hfODCylinderCalc.Value; }
        }

        public string ODAxis
        {
            get { return lboxODAxis.SelectedValue; }
            set { lboxODAxis.SelectedValue = value; }
        }

        public string ODAxis_calc
        {
            get { return hfODAxisCalc.Value; }
        }

        public string ODHPrism
        {
            get { return lboxODHPrism.SelectedValue; }
            set { lboxODHPrism.SelectedValue = value; }
        }

        public string ODHBase
        {
            get { return ddlODHBase.SelectedValue; }
            set { ddlODHBase.SelectedValue = value; }
        }

        public string ODVPrism
        {
            get { return lboxODVPrism.SelectedValue; }
            set { lboxODVPrism.SelectedValue = value; }
        }

        public string ODVBase
        {
            get { return ddlODVBase.SelectedValue; }
            set { ddlODVBase.SelectedValue = value; }
        }

        public string ODAdd
        {
            get { return lboxODAdd.SelectedValue; }
            set { lboxODAdd.SelectedValue = value; }
        }

        public List<String> ODSphereValuesList
        {
            get
            {
                return (List<String>)lboxODSphere.DataSource;
            }
            set
            {
                lboxODSphere.DataSource = value;
                lboxODSphere.DataBind();
            }
        }

        public List<String> ODCylinderValuesList
        {
            get
            {
                return (List<String>)lboxODCylinder.DataSource;
            }
            set
            {
                lboxODCylinder.DataSource = value;
                lboxODCylinder.DataBind();
            }
        }

        public List<String> ODAxisValuesList
        {
            get
            {
                return (List<String>)lboxODAxis.DataSource;
            }
            set
            {
                lboxODAxis.DataSource = value;
                lboxODAxis.DataBind();
            }
        }

        public List<String> ODHPrismValuesList
        {
            get
            {
                return (List<String>)lboxODHPrism.DataSource;
            }
            set
            {
                lboxODHPrism.DataSource = value;
                lboxODHPrism.DataBind();
            }
        }

        public List<String> ODVPrismValuesList
        {
            get
            {
                return (List<String>)lboxODVPrism.DataSource;
            }
            set
            {
                lboxODVPrism.DataSource = value;
                lboxODVPrism.DataBind();
            }
        }

        public List<String> ODAddValuesList
        {
            get
            {
                return (List<String>)lboxODAdd.DataSource;
            }
            set
            {
                lboxODAdd.DataSource = value;
                lboxODAdd.DataBind();
            }
        }

        public List<String> OSSphereValuesList
        {
            get
            {
                return (List<String>)lboxOSSphere.DataSource;
            }
            set
            {
                lboxOSSphere.DataSource = value;
                lboxOSSphere.DataBind();
            }
        }

        public List<String> OSCylinderValuesList
        {
            get
            {
                return (List<String>)lboxOSCylinder.DataSource;
            }
            set
            {
                lboxOSCylinder.DataSource = value;
                lboxOSCylinder.DataBind();
            }
        }

        public List<String> OSAxisValuesList
        {
            get
            {
                return (List<String>)lboxOSAxis.DataSource;
            }
            set
            {
                lboxOSAxis.DataSource = value;
                lboxOSAxis.DataBind();
            }
        }

        public List<String> OSHPrismValuesList
        {
            get
            {
                return (List<String>)lboxOSHPrism.DataSource;
            }
            set
            {
                lboxOSHPrism.DataSource = value;
                lboxOSHPrism.DataBind();
            }
        }

        public List<String> OSVPrismValuesList
        {
            get
            {
                return (List<String>)lboxOSVPrism.DataSource;
            }
            set
            {
                lboxOSVPrism.DataSource = value;
                lboxOSVPrism.DataBind();
            }
        }

        public List<String> OSAddValuesList
        {
            get
            {
                return (List<String>)lboxOSAdd.DataSource;
            }
            set
            {
                lboxOSAdd.DataSource = value;
                lboxOSAdd.DataBind();
            }
        }

        public List<String> PDTotalValues
        {
            get
            {
                return (List<String>)lboxPDTotal.DataSource;
            }
            set
            {
                lboxPDTotal.Items.Clear();
                lboxPDTotal.DataSource = value;
                lboxPDTotal.DataBind();
                lboxPDTotal.Items.Insert(0, new ListItem("N/A", "X"));
                lboxPDTotal.SelectedIndex = -1;

                lboxPDTotalNear.Items.Clear();
                lboxPDTotalNear.DataSource = value;
                lboxPDTotalNear.DataBind();
                lboxPDTotalNear.Items.Insert(0, new ListItem("N/A", "X"));
                lboxPDTotalNear.SelectedIndex = -1;
            }
        }

        public List<String> PDMonoValues
        {
            get
            {
                return (List<String>)lboxPDOD.DataSource;
            }
            set
            {
                lboxPDOD.Items.Clear();
                lboxPDOD.DataSource = value;
                lboxPDOD.DataBind();
                lboxPDOD.Items.Insert(0, new ListItem("N/A", "X"));
                lboxPDOD.SelectedIndex = -1;

                lboxPDOS.Items.Clear();
                lboxPDOS.DataSource = value;
                lboxPDOS.DataBind();
                lboxPDOS.Items.Insert(0, new ListItem("N/A", "X"));
                lboxPDOS.SelectedIndex = -1;

                lboxPDODNear.Items.Clear();
                lboxPDODNear.DataSource = value;
                lboxPDODNear.DataBind();
                lboxPDODNear.Items.Insert(0, new ListItem("N/A", "X"));
                lboxPDODNear.SelectedIndex = -1;

                lboxPDOSNear.Items.Clear();
                lboxPDOSNear.DataSource = value;
                lboxPDOSNear.DataBind();
                lboxPDOSNear.Items.Insert(0, new ListItem("N/A", "X"));
                lboxPDOSNear.SelectedIndex = -1;
            }
        }

        public string OSSphere
        {
            get { return lboxOSSphere.SelectedValue; }
            set { lboxOSSphere.SelectedValue = value; }
        }

        public string OSSphere_calc
        {
            get { return hfOSSphereCalc.Value; }
        }

        public string OSCylinder
        {
            get { return lboxOSCylinder.SelectedValue; }
            set { lboxOSCylinder.SelectedValue = value; }
        }

        public string OSCylinder_calc
        {
            get { return hfOSCylinderCalc.Value; }
        }

        public string OSAxis
        {
            get { return lboxOSAxis.SelectedValue; }
            set { lboxOSAxis.SelectedValue = value; }
        }

        public string OSAxis_calc
        {
            get { return hfOSAxisCalc.Value; }
        }

        public string OSHPrism
        {
            get { return lboxOSHPrism.SelectedValue; }
            set { lboxOSHPrism.SelectedValue = value; }
        }

        public string OSHBase
        {
            get { return ddlOSHBase.SelectedValue; }
            set { ddlOSHBase.SelectedValue = value; }
        }

        public string OSVPrism
        {
            get { return lboxOSVPrism.SelectedValue; }
            set { lboxOSVPrism.SelectedValue = value; }
        }

        public string OSVBase
        {
            get { return ddlOSVBase.SelectedValue; }
            set { ddlOSVBase.SelectedValue = value; }
        }

        public string OSAdd
        {
            get { return lboxOSAdd.SelectedValue; }
            set { lboxOSAdd.SelectedValue = value; }
        }

        public string PDTotal
        {
            get { return lboxPDTotal.SelectedValue; }
            set
            {
                string setValue = value;

                if (setValue.Contains(".00"))
                {
                    lboxPDTotal.SelectedValue = value.Substring(0, 2);
                }
                else
                {
                    lboxPDTotal.SelectedValue = value.Substring(0, 4);
                }
            }
        }

        public string PDTotalNear
        {
            get { return lboxPDTotalNear.SelectedValue; }
            set
            {
                string setValue = value;

                if (setValue.Contains(".00"))
                {
                    lboxPDTotalNear.SelectedValue = value.Substring(0, 2);
                }
                else
                {
                    lboxPDTotalNear.SelectedValue = value.Substring(0, 4);
                }
            }
        }

        public string PDOD
        {
            get { return lboxPDOD.SelectedValue; }
            set
            {
                string setValue = value;

                if (setValue.Contains(".00"))
                {
                    lboxPDOD.SelectedValue = value.Substring(0, 2);
                }
                else
                {
                    lboxPDOD.SelectedValue = value.Substring(0, 4);
                }
            }
        }

        public string PDODNear
        {
            get { return lboxPDODNear.SelectedValue; }
            set
            {
                string setValue = value;

                if (setValue.Contains(".00"))
                {
                    lboxPDODNear.SelectedValue = value.Substring(0, 2);
                }
                else
                {
                    lboxPDODNear.SelectedValue = value.Substring(0, 4);
                }
            }
        }

        public string PDOS
        {
            get { return lboxPDOS.SelectedValue; }
            set
            {
                string setValue = value;

                if (setValue.Contains(".00"))
                {
                    lboxPDOS.SelectedValue = value.Substring(0, 2);
                }
                else
                {
                    lboxPDOS.SelectedValue = value.Substring(0, 4);
                }
            }
        }

        public string PDOSNear
        {
            get { return lboxPDOSNear.SelectedValue; }
            set
            {
                string setValue = value;

                if (setValue.Contains(".00"))
                {
                    lboxPDOSNear.SelectedValue = value.Substring(0, 2);
                }
                else
                {
                    lboxPDOSNear.SelectedValue = value.Substring(0, 4);
                }
            }
        }

        public bool IsMonoCalculation
        {
            get { return bool.Parse(ViewState["PDisMono"].ToString()); }
            set
            {
                ViewState.Add("PDisMono", value);
                rblPDMode.SelectedIndex = value ? 1 : 0;
            }
        }

        public List<PersonnelEntity> DoctorsList
        {
            get
            {
                return (List<PersonnelEntity>)ddlDoctors.DataSource;
            }
            set
            {
                ddlDoctors.Items.Clear();
                ddlDoctors.DataSource = value;
                ddlDoctors.DataBind();
            }
        }

        public int DoctorSelected
        {
            get
            {
                return Convert.ToInt32(ddlDoctors.SelectedValue);
            }
            set
            {
                ddlDoctors.SelectedValue = value.ToString();
            }
        }

        public bool IsActive
        {
            get
            {
                return Convert.ToBoolean(!chkboxRemove.Checked);
            }
        }

        public bool IsUsed
        {
            get
            {
                return Convert.ToBoolean(ViewState["IsUsed"]);
            }
            set
            {
                ViewState.Add("IsUsed", value);
            }
        }

        public bool IsDeletable
        {
            get
            {
                return Convert.ToBoolean(ViewState["IsDeletable"]);
            }
            set
            {
                ViewState.Add("IsDeletable", value);
            }
        }

        #endregion Accessors
    }
}