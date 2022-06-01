using SrtsWeb.Entities;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using SrtsWeb.BusinessLayer.Concrete;
using SrtsWeb.BusinessLayer.Abstract;

namespace SrtsWeb.Base
{
    public class UserControlBase : System.Web.UI.UserControl, ICustomEventLogger
    {
        public SRTSSession mySession
        {
            get { return (SRTSSession)Session["SRTSSession"]; }
            set
            {
                if (Session["SRTSSession"] == null)
                {
                    Session.Add("SRTSSession", value);
                }
                else
                {
                    Session["SRTSSession"] = value;
                }
            }
        }
        private void LoadLookupTable()
        {
            SrtsWeb.BusinessLayer.Abstract.ILookupService _service = new LookupService();
            Cache["SRTSLOOKUP"] = _service.GetAllLookups();
        }
        public List<LookupTableEntity> LookupCache
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

        override protected void OnInit(EventArgs e)
        {
            base.OnInit(e);

            if (Context.Session == null)
            {
                Response.Redirect("../WebForms/Account/Login.aspx");
            }

            if (mySession == null) return;

            if (mySession.LogTriggers != null)
                mySession.LogTriggers.Clear();

            var p = Request.Path.Substring(Request.Path.LastIndexOf('/') + 1);

            mySession.LogTriggers = CustomLogger.GetLogTriggers(p);
        }

        protected void CurrentModule(string module)
        {
            if (Session["SRTSSession"] == null)
            {
                SRTSSession mySession = new SRTSSession();
                mySession.CurrentModule = module;
            }
            else
            {
                mySession.CurrentModule = module;
            }
        }

        protected void CurrentModule_Sub(string module)
        {
            if (Session["SRTSSession"] == null)
            {
                SRTSSession mySession = new SRTSSession();
                mySession.CurrentModule_Sub = module;
            }
            else
            {
                mySession.CurrentModule_Sub = module;
            }
        }

        #region Shared Methods

        protected void rbNewPatient_Click(object sender, EventArgs e)
        {
            mySession.AddOrEdit = "ADD";
            Response.Redirect("~/WebForms/SrtsPerson/AddPerson.aspx");
        }

        protected void rbNewIndividual_Click(object sender, EventArgs e)
        {
            mySession.AddOrEdit = "ADD";
            Response.Redirect("~/WebForms/SrtsPerson/AddPerson.aspx");
        }

        protected void rbPatientSearch_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/WebForms/SrtsWebClinic/Patients/ManagePatients.aspx/search");
        }

        protected void rbIndividualSearch_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/WebForms/SrtsWebClinic/Individuals/IndividualSearch.aspx");
        }

        protected void rbOrderCheckin_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/WebForms/SrtsWebClinic/Orders/ManageOrders.aspx/checkin");
        }

        protected void rbOrderDispense_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/WebForms/SrtsWebClinic/Orders/ManageOrders.aspx/dispense");
        }

        protected void btnEditPatient_Click(object sender, EventArgs e)
        {
            mySession.TempID = mySession.Patient.Individual.ID;
            Response.Redirect("~/WebForms/SrtsWebClinic/Patients/PatientManagementEdit.aspx");
            mySession.CurrentModule = "Manage Patients - Edit Patient Information";
        }

        protected Control GetControlThatCausedPostBack(Page page)
        {
            Control control = null;
            string ctrlname = page.Request.Params.Get("__EVENTTARGET");
            if (ctrlname != null && ctrlname != string.Empty)
            {
                control = page.FindControl(ctrlname);
            }
            else
            {
                foreach (string ctl in page.Request.Form)
                {
                    Control c = page.FindControl(ctl);
                    if (c is System.Web.UI.WebControls.Button || c is System.Web.UI.WebControls.ImageButton)
                    {
                        control = c;
                        break;
                    }
                }
            }
            return control;
        }

        #endregion Shared Methods

        public void LogEvent(string message)
        {
            var cel = new CustomEventLogger();
            cel.LogEvent(message);
        }
        public void LogEvent(string messageFormat, object[] formatParams)
        {
            var cel = new CustomEventLogger();
            cel.LogEvent(messageFormat, formatParams);
        }

        protected void ShowConfirmDialog(String msg)
        {
            /// Show global confirm dialog
            ScriptManager.RegisterStartupScript(this, GetType(), "DisplayDialogMessage", "displaySrtsMessage('Success!','" + msg + "', 'success');", true);
        }


        protected void ShowErrorDialog(String msg)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "DisplayDialogMessage", "displaySrtsMessage('Error!','" + msg + "', 'error');", true);
        }
    }
}