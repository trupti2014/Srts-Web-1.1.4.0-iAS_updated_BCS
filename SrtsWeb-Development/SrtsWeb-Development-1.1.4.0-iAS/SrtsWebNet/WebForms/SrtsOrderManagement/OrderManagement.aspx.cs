using SrtsWeb.Base;
using SrtsWeb.BusinessLayer.Concrete;
using SrtsWeb.Entities;
using SrtsWeb.ExtendersHelpers;
using SrtsWeb.Presenters.Orders;
using SrtsWeb.Views.Orders;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Permissions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Web.Security;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;

namespace SrtsWeb.SrtsOrderManagement
{
    [PrincipalPermission(SecurityAction.Demand, Role = "ClinicTech")]
    [PrincipalPermission(SecurityAction.Demand, Role = "ClinicClerk")]
    [PrincipalPermission(SecurityAction.Demand, Role = "ClinicProvider")]
    [PrincipalPermission(SecurityAction.Demand, Role = "HumanTech")]
    [PrincipalPermission(SecurityAction.Demand, Role = "LabTech")]
    [PrincipalPermission(SecurityAction.Demand, Role = "LabClerk")]
    public partial class OrderManagement : PageBase, IOrderManagementView
    {
        private OrderManagementPresenter p;

        public IOrderManagementView ViewToPass { get { return this; } }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
#if DEBUG
                using (MethodTracer.Trace(SrtsTraceSource.LabOrderSource, "OrderManagement_Page_Load", mySession.MyUserID))
#endif
                {
                    this.p = new OrderManagementPresenter(this);
                    var r = Roles.GetRolesForUser();
                    var isClinic = r.Any(x => x.ToLower().Contains("clinic"));

                    this.divClinic.Visible = isClinic;
                    this.divLab.Visible = !isClinic;

                    this.mySession.MyIndividualID = SrtsWeb.Account.CustomProfile.GetProfile(HttpContext.Current.User.Identity.Name).IndividualId;

                    var dto = Session["PatientOrderDTO"] as PatientOrderDTO;
                    if (dto == null) return;

                    if (Session["IsDtoRedirect"].Equals(true))
                    {
                        this.PatientId = dto.IndividualId;
                        this.Demographic = dto.Demographic;
                        this.SiteCode = this.mySession.MyClinicCode;//dto.PatientSiteCode; //CR DEF 0059875 - Frame Restrictions Not Working
                        Session.Add("IsDtoRedirect", false);
                        mySession.SelectedPatientID = this.PatientId;
                    }

                    // Set hidden fields for AJAX usage
                    this.hfPatientId.Value = this.PatientId.ToString();
                    this.hfUserId.Value = this.mySession.ModifiedBy;
                    this.hdfPatientEmailAddress.Value = this.p.GetEmailAddressByPatientID().EMailAddress;

                    lnkPatientDetail.PostBackUrl = "../SrtsPerson/PersonDetails.aspx?id='" + this.PatientId + "'&isP=true";
                    BuildPageTitle();

                    // if (this.mySession.Patient.Addresses == null)
                    // {
                    this.p.FillPatientAddressData();
                    // }
                    if (this.mySession.Patient.Addresses != null && this.mySession.Patient.Addresses.Count > 0)
                    {
                        AddressEntity patientAddress = new AddressEntity();
                        patientAddress = this.mySession.Patient.Addresses.FirstOrDefault(a => a.IsActive == true);
                        if (patientAddress != null)
                        {
                            if (patientAddress.DateVerified.ToString() != "1/1/0001" && patientAddress.DateVerified != null && patientAddress.DateVerified.ToString() != "")
                            {
                                this.hdfVerifiedDate.Value = patientAddress.DateVerified.ToShortDateString();
                                this.hdfExpireDays.Value = patientAddress.ExpireDays.ToString();
                                lblDateVerified.Text = this.hdfVerifiedDate.Value;
                                lblExpireDays.Text = this.hdfExpireDays.Value;
                            }
                        }
                    }
                   // CheckLabMailToPatientStatus();

                    if (!this.IsPostBack)
                    {
                        if (HttpContext.Current.Session["CurrentProcess"] != null)
                        {
                            var cp = Session["CurrentProcess"] as string;
                            Session["CurrentProcess"] = null;
                            switch (cp)
                            {
                                case "Order Management":
                                    ShowOrder(Session["PatientOrderDTO"] as PatientOrderDTO);
                                    Session["PatientOrderDTO"] = null;
                                    break;
                                case "Do New Order":
                                    DoOrderWork(Session["CurrentOrderNumber"].ToInt32());
                                    Session["CurrentOrderNumber"] = null;
                                    break;
                            }
                        }


                        if (r.Any(x => x.ToLower().Contains("clinic")))
                        {
                            if (Session["CONFIRM"] != null && !String.IsNullOrEmpty(Session["CONFIRM"].ToString()))
                            {
                                ShowConfirmDialog(Session["CONFIRM"].ToString());

                                Session.Remove("CONFIRM");
                            }
                            else if (Session["FAIL"] != null && !String.IsNullOrEmpty(Session["FAIL"].ToString()))
                            {
                                ScriptManager.RegisterStartupScript(this, this.GetType(), "Fail", String.Format("Confirm('{0}', 'statusMessage', true, false);", Session["FAIL"].ToString()), true);
                                Session.Remove("FAIL");
                            }

                            // Set the initial value to false.  This only becomes true if the user clicks the "Save and Create New" button.
                            this.OrderIsPrefill = false;

                            //this.lblCurrentDate.Text = DateTime.Today.ToShortDateString();

                            p.GetPatientData();

                            var tsk1 = Task.Run(() => { p.IsPatientAddressActive(this.PatientId); });
                            //var tsk2 = Task.Run(() => {  });

                            Task.WhenAll(new Task[] { tsk1 });

                            // Get preferences
                            p.GetOrderPreferences();
                            p.GetFrameItemPreferences();
                            p.GetRxPreferences();

                            SetRxPrefs();

                            // If patient is guard/reserve then show the make active duty div
                            var st = this.Demographic.ToPatientStatusKey();
                            this.divConvertToActive.Visible = st.Equals("12") || st.Equals("15");

                            if (String.IsNullOrEmpty(dto.OrderNumber))
                            {
                                GetLoadDataAsync();
                                return;
                            }
                            else
                                GetLoadData();

                            if (!GetSelectedOrder(dto.OrderNumber, false))
                            {
                                if (Session["FAIL"] != null && !String.IsNullOrEmpty(Session["FAIL"].ToString()))
                                {
                                    ScriptManager.RegisterStartupScript(this, this.GetType(), "Fail", String.Format("Confirm('{0}', 'statusMessage', true, false);", Session["FAIL"].ToString()), true);
                                    Session.Remove("FAIL");
                                    dto.OrderNumber = String.Empty;
                                    return;
                                }
                            }

                            ScriptManager.RegisterStartupScript(this, this.GetType(), "OrderDialog", "DoSelectedOrder('false');", true);
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "hideReject", "DoRejectRedirect('none');", true);
                            dto.OrderNumber = String.Empty;
                        }
                        else if (r.Any(x => x.ToLower().Contains("lab")))
                        {
                            p.GetPatientData();
                            p.GetAllPrescriptions();
                            p.GetAllOrders();
                            BindLabOrderHistoryGrid();

                            if (String.IsNullOrEmpty(dto.OrderNumber)) return;

                            if (!GetSelectedOrderForLab(dto.OrderNumber))
                            {
                                ScriptManager.RegisterStartupScript(this, this.GetType(), "ErrorAlert", "alert('You do not have permissions to view this order.');", true);
                                return;
                            }

                            ScriptManager.RegisterStartupScript(this, this.GetType(), "OrderDialog", "DoSelectedOrder('true');", true);
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "hideReject", "DoRejectRedirect('redirect');", true);

                            dto.OrderNumber = String.Empty;

                            this.ceHfsDate.StartDate = DateTime.Now;
                        }
                    }
                    else
                    {
                        var et = Request.Params.Get("__EVENTTARGET");

                        switch (et.ToLower())
                        {
                            case "showorder":
                                ShowOrder(dto);
                                break;
                        }
                    }
                }
            }
            catch (Exception ex) { ex.TraceErrorException(); }
        }

        private void ShowOrder(PatientOrderDTO dto)
        {
            var ea = "";
            if (HttpContext.Current.Session["CurrentOrderNumber"] != null && HttpContext.Current.Session["CurrentOrderNumber"].ToString() != "")
            {
                var on = Session["CurrentOrderNumber"] as string;
                if (!string.IsNullOrEmpty(on))
                {
                    ea = on;
                }
                else
                {
                    ea = Request.Params.Get("__EVENTARGUMENT");
                }
            }
            else
            {
                ea = Request.Params.Get("__EVENTARGUMENT");
            }
            if (!GetSelectedOrder(ea, false))
            {
                if (Session["FAIL"] != null && !String.IsNullOrEmpty(Session["FAIL"].ToString()))
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "Fail", String.Format("Confirm('{0}', 'statusMessage', true, false);", Session["FAIL"].ToString()), true);
                    Session.Remove("FAIL");
                    dto.OrderNumber = String.Empty;
                    return;
                }
            }
            hdfVerifiedDate.Value = this.PatientAddress.DateVerified.ToString();
            hdfExpireDays.Value = this.PatientAddress.ExpireDays.ToString();
            ScriptManager.RegisterStartupScript(this, this.GetType(), "OrderDialog", "DoSelectedOrder('false');", true);

        }


        private void BuildPageTitle()
        {
            try
            {
                Master.CurrentModuleTitle = "Manage Orders";
                Master.uplCurrentModuleTitle.Update();
            }
            catch (NullReferenceException)
            {
                CurrentModule("Manage Orders");
                CurrentModule_Sub(string.Empty);
            }
        }

        protected void bConvertToActive_Click(object sender, EventArgs e)
        {
            try
            {
#if DEBUG
                using (MethodTracer.Trace(SrtsTraceSource.LabOrderSource, "OrderManagement_bConvertToActive_Click", mySession.MyUserID))
#endif
                {
                    var demo = String.Format("{0}{1}11{2}N", this.Demographic.ToRankKey(), this.Demographic.ToBOSKey(), this.Demographic.ToGenderKey());
                    this.Demographic = demo;
                    if (this.p.IsNull())
                        this.p = new OrderManagementPresenter(this);
                    this.p.MakePatientActiveDuty();
                    this.divConvertToActive.Visible = false;
                    var dto = Session["PatientOrderDTO"] as PatientOrderDTO;
                    if (dto == null) return;
                    dto.Demographic = demo;
                }
            }
            catch (Exception ex) { ex.TraceErrorException(); }
        }

        #region EXAMS

        protected void gvExamHist_SelectedIndexChanged(object sender, EventArgs e)
        {
#if DEBUG
            using (MethodTracer.Trace(SrtsTraceSource.ExamSource, "OrderManagement_gvExamHist_SelectedIndexChanged", mySession.MyUserID))
#endif
            {
                var i = default(Int32);
                if (!Int32.TryParse(this.gvExamHist.DataKeys[this.gvExamHist.SelectedRow.RowIndex].Value.ToString(), out i)) return;

                // Do edit prescription stuff
                DoExamWork(i);
                SetExamButtons();
            }
        }

        protected void gvExamHist_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow) return;

            e.Row.ToolTip = "Click to select row";
            e.Row.Attributes["onclick"] = this.Page.ClientScript.GetPostBackClientHyperlink(this.gvExamHist, "Select$" + e.Row.RowIndex, true);
        }

        protected void bAddUpdateExam_Click(object sender, EventArgs e)
        {
#if DEBUG
            using (MethodTracer.Trace(SrtsTraceSource.ExamSource, "OrderManagement_bAddUpdateExam_Click", mySession.MyUserID))
#endif
            {
                var confirm = String.Empty;
                var fail = String.Empty;

                if (this.bAddUpdateExam.Text.Equals("Save") || this.hfButtonText.Value.Equals("Save"))
                {
                    if (p.InsertExam())
                    {
                        confirm = "Exam saved successfully";
                    }
                    else
                    {
                        fail = "Error saving exam";
                    }
                }
                else
                {
                    if (p.UpdateExam())
                    {
                        confirm = "Exam updated successfully";
                    }
                    else
                    {
                        fail = "Error updating exam";
                    }
                }

                this.hfButtonText.Value = String.Empty;

                if (!String.IsNullOrEmpty(confirm))
                {
                    LogEvent(String.Format("{0} by user {1} at {2}", confirm, mySession.MyUserID, DateTime.Now));
                    Session["CONFIRM"] = confirm;
                }
                else if (!String.IsNullOrEmpty(fail))
                {
                    Session["FAIL"] = fail;
                }

                if (!CustomRedirect.SanitizeRedirect(this.Request.Url.ToString(), false))
                {
                    ShowErrorDialog("An error occurred after updating the exam.");
                }
            }
        }

        private void BindExamGrid()
        {
            this.gvExamHist.DataKeyNames = new[] { "ExamId" };
            this.gvExamHist.DataSource = this.ExamList;
            this.gvExamHist.DataBind();
        }

        private void BindExamProviders()
        {
            this.ddlExamProviders.DataSource = this.ProviderList;
            this.ddlExamProviders.DataTextField = "NameLFMi";
            this.ddlExamProviders.DataValueField = "ID";
            this.ddlExamProviders.DataBind();
            this.ddlExamProviders.Items.Insert(0, new ListItem("-Select-", "X"));
            this.ddlExamProviders.SelectedIndex = 0;
        }

        private void BindOrderNotificationsGrid()
        {
            this.gvOrderNotifications.DataKeyNames = new[] { "OrderNumber" };;
            this.gvOrderNotifications.DataSource = this.OrderEmailList.Where(x => x.EmailSent == true).OrderByDescending(d => d.EmailDate);
            this.gvOrderNotifications.DataBind();
        }

        private Boolean GetSelectedExamById(Int32 key)
        {
#if DEBUG
            using (MethodTracer.Trace(SrtsTraceSource.ExamSource, "OrderManagement_GetSelectedExamById", mySession.MyUserID))
#endif
            {
                var e = this.ExamList.FirstOrDefault(x => x.ExamId == key);
                if (e == null) return false;
                this.Exam = e;

                // Highlight the selected row
                this.gvExamHist.Rows[this.ExamList.IndexOf(e)].BackColor = System.Drawing.ColorTranslator.FromHtml("#A1DCF2");

                return true;
            }
        }

        private void DoExamWork(Int32 key)
        {
            if (!GetSelectedExamById(key)) return;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ScriptDialog", "DoExamDialog();", true);
        }

        private void SetExamButtons()
        {
            this.bAddUpdateExam.Text = this.Exam.ExamId == default(Int32) ? "Save" : "Update";
        }

        #endregion EXAMS

        #region PRESCRIPTIONS

        protected void SetRxPrefs()
        {
            if (!SitePrefsRX.IsNull())
            {
                this.hfRxName.Value = SitePrefsRX.RxType.IsNullOrEmpty() ? "FTW" : SitePrefsRX.RxType;
                this.hfOdPdDistCombo.Value = SitePrefsRX.PDDistance.IsNull() ? null : Convert.ToString(SitePrefsRX.PDDistance);
                this.hfOdPdNearCombo.Value = SitePrefsRX.PDNear.IsNull() ? null : Convert.ToString(SitePrefsRX.PDNear);
                this.hfProviderId.Value = SitePrefsRX.ProviderId.IsNull() ? "X" : Convert.ToString(SitePrefsRX.ProviderId);
                //this.hfOdPdDistCombo.Value = SitePrefsRX.PDDistance.IsNull() ? "63.00" : Convert.ToString(SitePrefsRX.PDDistance);
                //this.hfOdPdNearCombo.Value = SitePrefsRX.PDNear.IsNull() ? "60.00" : Convert.ToString(SitePrefsRX.PDNear);
            }
            else
            {
                this.hfRxName.Value = "FTW";
                this.hfOdPdDistCombo.Value = null;
                this.hfOdPdNearCombo.Value = null;
                //this.hfOdPdDistCombo.Value = "63.00";
                //this.hfOdPdNearCombo.Value = "60.00";
            }
        }

        protected void gvPrescriptionHist_SelectedIndexChanged(object sender, EventArgs e)
        {
#if DEBUG
            using (MethodTracer.Trace(SrtsTraceSource.RxSource, "OrderManagement_gvPrescriptionHist_SelectedIndexChanged", mySession.MyUserID))
#endif
            {
                var i = default(Int32);
                if (!Int32.TryParse(gvPrescriptionHist.DataKeys[gvPrescriptionHist.SelectedRow.RowIndex].Value.ToString(), out i)) return;

                // Do edit prescription stuff
                DoEditWork(i);
                SetPrescriptionButtons();
                if (!this.Prescription.IsUsed) cbDeletePrescription.Visible = true;
                if (IsRxScan(this.Prescription.PrescriptionScanId))
                {
                    divAttachPrescription.Visible = false;
                    divDeletePrescription.Visible = true;
                    btnDeleteScan.Visible = true;
                }
                else
                {
                    divAttachPrescription.Visible = true;
                    divDeletePrescription.Visible = false;
                    btnDeleteScan.Visible = false;
                }
            }
        }


        protected Boolean IsRxScan(Int32 rxScanId)
        {
            if (rxScanId == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        protected void gvPrescriptionHist_RowCommand(object sender, GridViewCommandEventArgs e)
        {
#if DEBUG
            using (MethodTracer.Trace(SrtsTraceSource.RxSource, "OrderManagement_gvPrescriptionHist_RowCommand", mySession.MyUserID))
#endif
            {
                string commandArgument = null;
                string[] arguments = null;
                var i = default(Int32);
                if (!Int32.TryParse(e.CommandArgument.ToString(), out i))
                {
                    try
                    {
                        commandArgument = e.CommandArgument.ToString();
                        arguments = commandArgument.Split(',');

                        // if first argument = 0 (there is no prescription scan document), then just get prescription
                        if (Convert.ToInt32(arguments[0]) == 0)
                        {
                            // Do edit prescription stuff
                            DoEditWork(Convert.ToInt32(arguments[0]));
                            SetPrescriptionButtons();
                            if (!this.Prescription.IsUsed) cbDeletePrescription.Visible = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        return;
                    }
                }
                switch (e.CommandName.ToLower())
                {
                    case "order":
                        var r = ((GridView)sender).DataKeys[i];

                        // Do add order stuff
                        Session["CurrentProcess"] = "New Order";
                        Session["CurrentOrderNumber"] = r.Value;
                        DoOrderWork(Convert.ToInt32(r.Value));
                        SetOrderControlState();
                        break;
                    case "viewdocument":
                        int PrescriptionScanId = Convert.ToInt32(arguments[0]);
                        int PrescriptionId = Convert.ToInt32(arguments[1]);
                        Session["Argument"] = commandArgument;
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "DisplayPrescriptionDocumentDialog", "DisplayPrescriptionDocumentDialog();", true);
                        break;
                }
            }
        }

        protected void gvPrescriptionHist_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow) return;

            e.Row.Attributes.Add("onmouseover", "this.style.backgroundColor='#EFD3A5'; this.style.cursor='pointer';");
            e.Row.Attributes.Add("onmouseout", "this.style.backgroundColor=''; this.style.textDecoration='none';");
            e.Row.ToolTip = "Click to select row";

            Prescription drv = (Prescription)e.Row.DataItem;
            int rxScanId = Convert.ToInt32(drv.PrescriptionScanId);
            int rxId = Convert.ToInt32(drv.PrescriptionId);
            string arg = rxScanId + "," + rxId;

            for (var i = 1; i < e.Row.Cells.Count; i++)
            {
                if (i > 1 && i != 6) 
                {
                    e.Row.Cells[i].Attributes["onclick"] = this.Page.ClientScript.GetPostBackClientHyperlink(this.gvPrescriptionHist, "Select$" + e.Row.RowIndex, true);
                }
                if (i == 6 && rxScanId != 0) 
                {
                    e.Row.Cells[i].Attributes["onclick"] = this.Page.ClientScript.GetPostBackClientHyperlink(this.gvPrescriptionHist, "ViewDocument$" + arg, true); 
                }
                if (i == 6 && rxScanId == 0)
                {
                    e.Row.Cells[i].Attributes.Remove("onclick");
                    e.Row.Cells[i].Attributes.Remove("onmouseover");
                    e.Row.Cells[i].Attributes.Add("onmouseover", "this.style.cursor='none';");
                    e.Row.Cells[i].ToolTip = "";
                }
            }
        }


        protected void bSaveUpdatePrescription_Click(object sender, EventArgs e)
        {
#if DEBUG
            using (MethodTracer.Trace(SrtsTraceSource.RxSource, "OrderManagement_bSaveUpdatePrescription_Click", mySession.MyUserID))
#endif
            {
                var confirm = String.Empty;
                var fail = String.Empty;

                if (this.bSaveUpdatePrescription.Text.Equals("Save") || this.hfButtonText.Value.Equals("Save"))
                {
                    if (p.InsertPrescription())
                        confirm = "Prescription saved successfully";
                    else
                        fail = "Error saving prescription";
                }
                else
                {
                    if (this.cbDeletePrescription.Checked)
                    {
                        if (p.DeletePrescription())
                            confirm = "Prescription successfully deleted";
                        else
                            fail = "Error deleting prescription";
                    }
                    else
                    {
                        var pe = this.Prescription;
                        if (pe.PrescriptionDocument != null && pe.PrescriptionDocument.Id == 0)
                        {
                            pe.PrescriptionDocument.PrescriptionId = pe.PrescriptionId;
                            pe.PrescriptionDocument.DocumentName = Path.GetFileName(pe.PrescriptionDocument.DocumentName);
                            if (pe.PrescriptionDocument.DocumentImage != null)
                            {
                                var result = p.InsertScannedPrescription(pe);
                                if (result.GreaterThan(0))
                                {
                                    pe.PrescriptionScanId = result;
                                }
                            }
                            pe.PrescriptionDocument = null;
                        }
                        if (p.UpdatePrescription(pe))
                            confirm = "Prescription successfully updated";
                        else
                            fail = "Error updating prescription";
                    }
                }

                var m = String.Empty;
                if (!String.IsNullOrEmpty(confirm))
                {
                    m = confirm;
                    Session["CONFIRM"] = confirm;
                }
                else if (!String.IsNullOrEmpty(fail))
                {
                    m = fail;
                    Session["FAIL"] = fail;
                }

                LogEvent(String.Format("{0} by user {1} at {2}", m, mySession.MyUserID, DateTime.Now));
                if (!CustomRedirect.SanitizeRedirect(this.Request.Url.ToString(), false))
                {
                    ShowErrorDialog("Error updating prescription.");
                }
            }
        }

        protected void bSaveRxOpenOrder_Click(object sender, EventArgs e)
        {
#if DEBUG
            using (MethodTracer.Trace(SrtsTraceSource.RxSource, "OrderManagement_bSaveRxOpenOrder_Click", mySession.MyUserID))
#endif
            {
                var confirm = String.Empty;
                var fail = String.Empty;
                var rxId = default(Int32);

                if (p.InsertPrescription(out rxId))
                    confirm = "Prescription saved successfully.";
                else
                    fail = "Error saving prescription";

                var m = String.Empty;
                if (!String.IsNullOrEmpty(confirm))
                {
                    m = confirm;
                    ShowConfirmDialog(confirm);
                }
                else if (!String.IsNullOrEmpty(fail))
                {
                    m = fail;
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "Fail", String.Format("Confirm('{0}', 'statusMessage', true, false);", fail), true);
                }

                LogEvent(String.Format("{0} by user {1} at {2}", m, mySession.MyUserID, DateTime.Now));

                p.GetAllPrescriptions();
                BindPrescriptionGrid();

                this.hfButtonText.Value = "Update";

                DoOrderWork(Convert.ToInt32(rxId));
                SetOrderControlState();
            }
        }

        protected void bSaveNewPrescription_Click(object sender, EventArgs e)
        {
#if DEBUG
            using (MethodTracer.Trace(SrtsTraceSource.RxSource, "OrderManagement_bSaveNewPrescription_Click", mySession.MyUserID))
#endif
            {
                var confirm = String.Empty;
                var fail = String.Empty;

                if (p.InsertPrescription())
                {
                    confirm = "Prescription saved successfully";
                }
                else
                {
                    fail = "Error saving prescription";
                }

                var m = string.Empty;
                if (!String.IsNullOrEmpty(confirm))
                {
                    m = confirm;
                    Session["CONFIRM"] = confirm;
                }
                else if (!String.IsNullOrEmpty(fail))
                {
                    m = fail;
                    Session["FAIL"] = fail;
                }

                LogEvent("{0} by user {1} at {2}", new Object[] { m, mySession.MyUserID, DateTime.Now });
                //Response.Redirect(this.Request.Url.ToString(), false);
                if (!CustomRedirect.SanitizeRedirect(this.Request.Url.ToString(), false))
                {
                    ShowErrorDialog("An error occurred after saving the prescription.");
                }
                //this.Redirect(this.Request.Url.ToString(), false);

            }
        }

        private void BindPrescriptionGrid()
        {
            this.gvPrescriptionHist.DataKeyNames = new[] { "PrescriptionId" };
            this.gvPrescriptionHist.DataSource = this.PrescriptionList;
            this.gvPrescriptionHist.DataBind();
        }

        private void BindPrescriptionProviders()
        {
            try
            {
                this.ddlPrescriptionProvider.DataSource = this.ProviderList;
                this.ddlPrescriptionProvider.DataTextField = "NameLFMi";
                this.ddlPrescriptionProvider.DataValueField = "ID";
                this.ddlPrescriptionProvider.DataBind();
                this.ddlPrescriptionProvider.Items.Insert(0, new ListItem("-Select-", "X"));
                this.ddlPrescriptionProvider.SelectedIndex = 0;

                //if (!SitePrefsRX.ProviderId.IsNull())
                //    this.ddlPrescriptionProvider.SelectedValue = SitePrefsRX.ProviderId;
                //else
                //    this.ddlPrescriptionProvider.SelectedIndex = 0;
            }
            finally { }
        }

        private Boolean GetSelectedPrescription(Int32 key)
        {
#if DEBUG
            using (MethodTracer.Trace(SrtsTraceSource.RxSource, "OrderManagement_GetSelectedPrescription", mySession.MyUserID))
#endif
            {
                //if (this.PrescriptionList.IsNull()) return false;
                if (this.PrescriptionList.IsNull())
                {
                    if (Session["CurrentPrescriptionList"] != null)
                    {
                        this.PrescriptionList = Session["CurrentPrescriptionList"] as List<Prescription>;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    Session["CurrentPrescriptionList"] = this.PrescriptionList;
                }

                var p = this.PrescriptionList.FirstOrDefault(x => x.PrescriptionId == key);
                if (p == null) return false;
                this.Prescription = p;

                // Highlight the selected row
                this.gvPrescriptionHist.Rows[this.PrescriptionList.IndexOf(p)].BackColor = System.Drawing.ColorTranslator.FromHtml("#A1DCF2");

                return true;
            }
        }

        private void DoEditWork(Int32 key)
        {
#if DEBUG
            using (MethodTracer.Trace(SrtsTraceSource.RxSource, "OrderManagement_DoEditWork", mySession.MyUserID))
#endif
            {
                if (!GetSelectedPrescription(key)) return;
                // Get the selected prescription based on the Prescription Name
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ScriptDialog", "DoSelectedPrescription();", true);

                if (this.Prescription.OdHPrism.Equals(0) && this.Prescription.OsHPrism.Equals(0) &&
                    this.Prescription.OdVPrism.Equals(0) && this.Prescription.OsVPrism.Equals(0)) return;

                ScriptManager.RegisterStartupScript(this, this.GetType(), "Toggle1", "DoToggle('divPrismBase', 'imgPrismBase');", true);
            }
        }

        private void SetPrescriptionButtons()
        {
            if (this.Prescription.IsUsed)
            {
                this.bSaveUpdatePrescription.Style.Add("display", "none");
                this.bSaveRxOpenOrder.Visible = false;
                this.bSaveNewPrescription.Visible = true;
            }
            else
            {
                this.bSaveNewPrescription.Visible = true;
                this.bSaveUpdatePrescription.Text = "Update";
                this.bSaveUpdatePrescription.Style.Remove("display");
                this.bSaveRxOpenOrder.Visible = true;
            }
        }

        #endregion PRESCRIPTIONS

        #region ORDERS

        protected void bSaveCreateNewOrder_Click(object sender, EventArgs e)
        {
            var confirm = String.Empty;
            var fail = String.Empty;
            this.Order.IsComplete = true;
            var leaveEvent = false;

            try
            {
#if DEBUG
                using (MethodTracer.Trace(SrtsTraceSource.ClinicOrderSource, "OrderManagement_bSaveCreateNewOrder_Click", mySession.MyUserID))
#endif
                {
                    if ( /*this.OrderIsPrefill && p.CompareOrders(this.OriginalOrder, this.Order)*/ p.IsReorderWithin30Days() ) //Aldela 03/07/2018
                    {
                        this.Order.IsReOrder = true;
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ReOrderDialog", "DoReOrderDialog();", true);
                        SingletonOM.StaticOrderManagement = this;
                        leaveEvent = true;
                        return;
                    }

                    if (p.InsertOrder())
                    {
                        this.Order.FocJustification = string.Empty;
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ClearComments", "ClearComments();", true);
                        confirm = "Order successfully saved";
                    }
                    else
                    {
                        fail = "Error saving order record";
                    }
                }
            }
            catch (Exception ex)
            {
                fail = "There is an error with the order";
                ex.TraceErrorException(ex.InnerException.IsNull() ? String.Empty : ex.InnerException.Message);
            }
            finally
            {
                var m = String.Empty;
                if (!String.IsNullOrEmpty(confirm))
                {
                    m = confirm;
                    Session["CONFIRM"] = confirm;
                    ShowConfirmDialog(Session["CONFIRM"].ToString());
                }
                else if (!String.IsNullOrEmpty(fail))
                {
                    m = fail;
                    Session["FAIL"] = fail;
                }

                LogEvent(String.Format("{0} by user {1} at {2}", m, mySession.MyUserID, DateTime.Now));
                this.bSaveOrder.Attributes.Remove("disabled");
            }

            if (leaveEvent) return;

            // Clean the order object so only the data necessary to populate the dialog is left.
            this.OriginalOrder = Order;
            var o = Order;

            o.CurrentStatus = String.Empty;
            o.IsComplete = false;
            o.IsReOrder = false;
            o.LinkedId = String.Empty;
            o.OrderNumber = String.Empty;
            o.OrderNumberBarCode = null;

            this.Order = o;

            this.OrderIsPrefill = true;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "CloseOrderDialog", "CloseOrderDialog();", true);
            GetSelectedOrder(String.Empty, true);
            ScriptManager.RegisterStartupScript(this, this.GetType(), "OrderDialog", "DoSelectedOrder('false');", true);
            this.bSaveCreateNewOrder.Enabled = false;
        }

        protected void bSaveOrder_Click(object sender, EventArgs e)
        {
            var leaveEvent = false;
            var confirm = String.Empty;
            var fail = String.Empty;
            this.Order.IsComplete = true;

            try
            {
#if DEBUG
                using (MethodTracer.Trace(SrtsTraceSource.ClinicOrderSource, "OrderManagement_bSaveOrder_Click", mySession.MyUserID))
#endif
                {
                    if (this.bSaveOrder.Text.Equals("Submit and Close"))
                    {
                        if ( /*this.OrderIsPrefill && p.CompareOrders(this.OriginalOrder, this.Order)) */ p.IsReorderWithin30Days()) //Aldela 3/5/2018
                        {
                            this.Order.IsReOrder = true;
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ReOrderDialog", "DoReOrderDialog();", true);
                            SingletonOM.StaticOrderManagement = this;
                            leaveEvent = true;
                            return;
                        }

                        if (p.InsertOrder())
                        {
                            confirm = "Order successfully saved";
                        }
                        else
                        {
                            fail = "Error saving order record";
                        }
                    }
                    else if (this.bSaveOrder.Text.Equals("Update"))
                    {
                        if (this.cbReturnToStock.Checked)
                        {
                            if (p.ReturnOrderToStock())
                            {
                                confirm = "Order successfully returned to stock";
                            }
                            else
                            {
                                fail = "Error returning order to stock";
                            }
                        }
                        else if (this.cbDelete.Checked)
                        {
                            if (p.DeleteOrder())
                            {
                                confirm = "Order successfully deleted";
                            }
                            else
                            {
                                fail = "Error deleting order record";
                            }
                        }
                        else if (this.cbDispense.Checked)
                        {
                            if (p.DispenseOrderFromStock())
                            {
                                confirm = "Order successfully dispensed";
                            }
                            else
                            {
                                fail = "Error dispensing order record";
                            }
                        }
                        else
                        {
                            if (p.UpdateOrder())
                            {
                                confirm = "Order successfully updated";
                            }
                            else
                            {
                                fail = "Error updating order record";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                fail = "There is an error with the order";
                ex.TraceErrorException(ex.InnerException.IsNull() ? String.Empty : ex.InnerException.Message);
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex.InnerException.IsNull() ? ex : ex.InnerException);
            }
            finally
            {
                var m = String.Empty;
                if (!String.IsNullOrEmpty(confirm))
                {
                    m = confirm;
                    Session["CONFIRM"] = confirm;
                }
                else if (!String.IsNullOrEmpty(fail))
                {
                    m = fail;
                    Session["FAIL"] = fail;
                }

                LogEvent(String.Format("{0} by user {1} at {2}", m, mySession.MyUserID, DateTime.Now));

                if (!leaveEvent)
                {
                    //Response.Redirect(this.Request.Url.ToString(), false);
                    if(!CustomRedirect.SanitizeRedirect(this.Request.Url.ToString(),false))
                    {
                        ShowErrorDialog("An error occcurred after processing the order.");
                    }
                   // this.Redirect(this.Request.Url.ToString(), false);

                    this.bSaveOrder.Attributes.Remove("disabled");
                }
            }
        }

        protected void bSaveNewOrder_Click(object sender, EventArgs e)
        {
            try
            {
#if DEBUG
                using (MethodTracer.Trace(SrtsTraceSource.ClinicOrderSource, "OrderManagement_bSaveNewOrder_Click", mySession.MyUserID))
#endif
                {
                    // Check to see if this is a re-order
                    if (p.IsReorder())
                    {
                        this.Order.IsReOrder = true;
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ReOrderDialog", "DoReOrderDialog();", true);
                        SingletonOM.StaticOrderManagement = this;
                    }
                    else
                    {
                        this.Order.IsReOrder = false;
                        if (p.InsertOrder())
                        {
                            Session["CONFIRM"] = "New order successfully saved";
                            LogEvent(String.Format("New order successfully saved by user {0} at {1}", mySession.MyUserID, DateTime.Now));
                        }
                        else
                        {
                            Session["FAIL"] = "Error saving new order record";
                            LogEvent(String.Format("Error saving new order record by user {0} at {1}", mySession.MyUserID, DateTime.Now));
                        }
                        //Response.Redirect(this.Request.Url.ToString(), false);
                        if (!CustomRedirect.SanitizeRedirect(this.Request.Url.ToString(), false))
                        {
                            ShowErrorDialog("There was a problem printing this report.  Please report this error.");            
                        }

                    }
                }
            }
            catch (Exception ex) { ex.TraceErrorException(); }
        }

        protected void bSaveIncompleteOrder_Click(object sender, EventArgs e)
        {
            try
            {
#if DEBUG
                using (MethodTracer.Trace(SrtsTraceSource.ClinicOrderSource, "OrderManagement_bSaveIncompleteOrder_Click", mySession.MyUserID))
#endif
                {
                    this.Order.IsComplete = false;
                    if (p.InsertOrder())
                    {
                        Session["CONFIRM"] = "Incomplete order successfully saved";
                        LogEvent(String.Format("Incomplete order successfully saved by user {0} at {1}", mySession.MyUserID, DateTime.Now));
                    }
                    else
                    {
                        Session["FAIL"] = "Error saving incomplete order record";
                    }
                    //Response.Redirect(this.Request.Url.ToString(), false);
                    if(!CustomRedirect.SanitizeRedirect(this.Request.Url.ToString(),false))
                    {
                        ShowErrorDialog("An error occurred after saving the incomplete order record.");
                    }
                   // this.Redirect(this.Request.Url.ToString(), false);
                }
            }
            catch (Exception ex) { ex.TraceErrorException(); }
        }

        protected void bReprint771_Click(object sender, EventArgs e)
        {
            HttpContext.Current.Session["OrderNbrs"] = this.Order.OrderNumber;
            hiddenDownload.Src = "~/WebForms/Reports/rptViewDD771.aspx";
            
        }

        protected void bUpdateEmailAddress_Click(object sender, EventArgs e)
        {
#if DEBUG
            using (MethodTracer.Trace(SrtsTraceSource.ExamSource, "OrderManagement_bUpdateEmailAddress_Click", mySession.MyUserID))
#endif
            {
                var confirm = String.Empty;
                var fail = String.Empty;

                EMailAddressEntity entity;

                var bHasEmail = p.GetEmailAddressByPatientID();
                if (bHasEmail == null) //Insert Email
                {
                    entity = new EMailAddressEntity();
                    entity.EMailAddress = this.PatientEmailAddress;
                    if (p.InsertEmailAddress(entity))
                    {
                        confirm = "Email Address updated successfully";
                        this.hdfPatientEmailAddress.Value = this.PatientEmailAddress;
                    }
                    else
                    {
                        fail = "Error updating Email Address";
                    }
                }
                else //Update Email
                {
                    entity = p.GetEmailAddressByPatientID();
                    entity.EMailAddress = this.PatientEmailAddress;
                    if (p.UpdateEmailAddress(entity))
                    {
                        confirm = "Email Address updated successfully";
                        this.hdfPatientEmailAddress.Value = this.PatientEmailAddress;
                    }
                    else
                    {
                        fail = "Error updating Email Address";
                    }
                }
                
            //    this.hfButtonText.Value = String.Empty;

                if (!String.IsNullOrEmpty(confirm))
                {
                    LogEvent(String.Format("{0} by user {1} at {2}", confirm, mySession.MyUserID, DateTime.Now));
                    Session["CONFIRM"] = confirm;
                }
                else if (!String.IsNullOrEmpty(fail))
                {
                    Session["FAIL"] = fail;
                }

                if(!CustomRedirect.SanitizeRedirect(this.Request.Url.ToString(),false))
                {
                    ShowErrorDialog("An error occurred after updating the email address.");
                }
            }
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string AjaxInsertOrder(String comment1, String comment2)
        { 
#if DEBUG
            using (MethodTracer.Trace(SrtsTraceSource.ClinicOrderSource, "OrderManagement_AjaxInsertOrder", "User ID not available in static method."))
#endif
            {
                var om = SingletonOM.StaticOrderManagement;
                om.tbComment1.Text = comment1;
                om.tbComment2.Text = comment2;

                var p = new OrderManagementPresenter(om);
                if (p.InsertOrder())
                {
                    HttpContext.Current.Session["CONFIRM"] = "New order successfully saved";
                }
                else
                    HttpContext.Current.Session["FAIL"] = "Error saving new order record";

                var u = HttpContext.Current.Request.Url.ToString();
                return u.Remove(u.LastIndexOf('/'));
            }
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static IEnumerable<Order> AjaxOrderHistory(String PatientId, String UserId)
        {
#if DEBUG
            using (MethodTracer.Trace(SrtsTraceSource.ClinicOrderSource, "OrderManagement_AjaxOrderHistory", UserId))
#endif
            {
                var p = new OrderManagementPresenterAjax();
                var s = System.Web.HttpContext.Current.Session["SRTSSession"] as SRTSSession;
                var clinicCode = string.IsNullOrEmpty(s.MyClinicCode) ? Globals.SiteCode : s.MyClinicCode;
                var o = p.GetAllOrders(Convert.ToInt32(PatientId), UserId).Where(x => x.IsComplete == true && x.IsActive == true);

                return o;
            }
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static IEnumerable<Order> AjaxIncompleteOrderHistory(String PatientId, String UserId)
        {
#if DEBUG
            using (MethodTracer.Trace(SrtsTraceSource.ClinicOrderSource, "OrderManagement_AjaxIncompleteOrderHistory", UserId))
#endif
            {
                var p = new OrderManagementPresenterAjax();
                var o = p.GetAllOrders(Convert.ToInt32(PatientId), UserId).Where(x => x.IsComplete == false && x.IsActive == true);

                return o;
            }
        }

        protected void cbEmailPatient_Clicked(object sender, EventArgs e)
        {
            if (cbEmailPatient.Checked && tbOrderEmailAddress.Text == String.Empty)
            {
                tbOrderEmailAddress.Text = p.GetEmailAddressByPatientID().EMailAddress;
            }
        }

        protected void ddlOrderPriority_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.SelectedPriority.Equals("X")) return;

            try
            {
#if DEBUG
                using (MethodTracer.Trace(SrtsTraceSource.ClinicOrderSource, "OrderManagement_ddlOrderPriority_SelectedIndexChanged", mySession.MyUserID))
#endif
                {
                    doPriorityLoad();
                }
            }
            catch (Exception ex) { ex.TraceErrorException(); }
        }

        private void doPriorityLoad()
        {
            ClearOrderDdls();

            this.hfIsPrevOrderFOC.Value = string.Empty;
            this.hfIsFocEligible.Value = IsEligibleFocOrder() ? "Y" : "N";
            var demo = String.Format("{0}{1}", this.Demographic.Substring(0, 7), this.Order.Priority);
            p.GetFrames(demo, this.SiteCode, this.Prescription.PrescriptionId);
            BindFrames();

            this.hfNextCombo.Value = this.ddlFrame.ID;

            if (this.OrderPreferences.PriorityFrameCombo.IsNullOrEmpty()) return;
            if (!this.OrderPreferences.PriorityFrameCombo.Any(x => x.PriorityCode == this.Order.Priority)) return;

            // If a frame preference for the selected priority exists then set it and load the defaults.
            var fc = this.OrderPreferences.PriorityFrameCombo.FirstOrDefault(x => x.PriorityCode == Order.Priority).FrameCode;

            if (!this.OrderDdlData.FrameList.Any(x => x.FrameCode == fc)) return;

            this.ddlFrame.SelectedValue = fc;
            p.GetFrameItems(demo, fc);
            p.GetLabs(fc);
            BindOrderDdls();
        }

        protected void ddlFrame_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.Order.Frame.Equals("X")) return;

            try
            {
#if DEBUG
                using (MethodTracer.Trace(SrtsTraceSource.ClinicOrderSource, "OrderManagement_ddlFrame_SelectedIndexChanged", mySession.MyUserID))
#endif
                {
                    DoFrameLoad();
                }
            }
            catch (Exception ex) { ex.TraceErrorException(); }
        }

        private void DoFrameLoad()
        {
            this.OrderIsPrefill = this.OrderIsPrefill && this.OriginalOrder.Frame.Equals(this.Order.Frame);

            p.GetFrameItems(String.Format("000000B{0}", this.Order.Priority), this.Order.Frame);
            p.GetLabs(this.Order.Frame);
            BindOrderDdls();
            this.hfMaxPair.Value = String.Empty;
            this.hfMaxPair.Value = this.OrderDdlData.FrameList.Where(x => x.FrameCode == this.Order.Frame).Select(m => m.MaxPair).FirstOrDefault().ToString();
            this.hfIsFocFrame.Value = String.Empty;
            this.hfIsFocFrame.Value = this.OrderDdlData.FrameList.Where(x => x.FrameCode == this.Order.Frame).Select(m => m.IsFoc).FirstOrDefault().ToString().ToLower();
            ScriptManager.RegisterStartupScript(this, this.GetType(), "OrderVal", "OrderSaveUpdateButtonClick(false);", true);
            ScriptManager.RegisterStartupScript(this, this.GetType(), "DoSegHt", "ToggleSegHt();", true);

            this.hfNextCombo.Value = this.ddlColor.ID;
        }

        protected void ddlMaterial_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
#if DEBUG
                using (MethodTracer.Trace(SrtsTraceSource.ClinicOrderSource, "OrderManagement_ddlMaterial_SelectedIndexChanged", mySession.MyUserID))
#endif
                {
                    // If the priority is Wounded Worrior and the selected material is plas then show the special tints.
                    this.p.AddRemoveSpecialTints(this.SelectedPriority.ToLower().Equals("w") && this.Order.Material.ToLower().Equals("plas"));
                    BindSpecialTints();
                    BindLabDdl();

                    ScriptManager.RegisterStartupScript(this, this.GetType(), "OrderMatVal", "OrderRequiresMatJust();", true);
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "Reprint771", "EnableSaveIncompleteReprint771();", true);

                    this.hfNextCombo.Value = this.ddlMaterial.ID;
                }
            }
            catch (Exception ex) { ex.TraceErrorException(); }
        }

        protected void ddlTint_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
#if DEBUG
                using (MethodTracer.Trace(SrtsTraceSource.ClinicOrderSource, "OrderManagement_ddlTint_SelectedIndexChanged", mySession.MyUserID))
#endif
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "OrderMatVal", "OrderRequiresMatJust();", true);
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "Reprint771", "EnableSaveIncompleteReprint771();", true);

                    if (!this.p.SetTintLab(this.Order.Tint)) return; 

                    BindLabDdl();

                    this.hfNextCombo.Value = this.ddlTint.ID;
                }
            }
            catch (Exception ex) { ex.TraceErrorException(); }
        }

        protected void ddlCoating_SelectedIndexChanged(object sender, EventArgs e) 
        {
            try
            {
#if DEBUG
                using (MethodTracer.Trace(SrtsTraceSource.ClinicOrderSource, "OrderManagement_ddlCoating_SelectedIndexChanged", mySession.MyUserID))
#endif
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "OrderCoatingVal", "OrderRequiresCoatingJust();", true);
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "OrderMatVal", "OrderRequiresMatJust();", true);
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "OrderFocVal", "OrderRequiresFocJust();", true);
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "Reprint771", "EnableSaveIncompleteReprint771();", true);

                    BindLabDdl();

                    this.hfNextCombo.Value = this.ddlCoating.ID;
                }
            }
            catch (Exception ex) { ex.TraceErrorException(); }
        }

        protected void ddlColor_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
#if DEBUG
                using (MethodTracer.Trace(SrtsTraceSource.ClinicOrderSource, "OrderManagement_ddlColor_SelectedIndexChanged", mySession.MyUserID))
#endif
                {
                    TempFrameRestrictions();
                    this.hfNextCombo.Value = this.ddlEye.ID;
                    this.hfTmpNext.Value = this.ddlEye.ID;
                }
            }
            catch (Exception ex) { ex.TraceErrorException(); }
        }

        protected void ddlEye_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
#if DEBUG
                using (MethodTracer.Trace(SrtsTraceSource.ClinicOrderSource, "OrderManagement_ddlEye_SelectedIndexChanged", mySession.MyUserID))
#endif
                {
                    TempFrameRestrictions();
                    this.hfNextCombo.Value = this.ddlBridge.ID;
                    this.hfTmpNext.Value = this.ddlBridge.ID;
                }
            }
            catch (Exception ex) { ex.TraceErrorException(); }
        }

        private void BindOrderPriorities()
        {
            if (this.OrderDdlData.PriorityList != null)
            {
                this.ddlOrderPriority.DataSource = this.OrderDdlData.PriorityList;
                this.ddlOrderPriority.DataTextField = "Key";
                this.ddlOrderPriority.DataValueField = "Value";
                this.ddlOrderPriority.DataBind();
                this.ddlOrderPriority.Items.Insert(0, new ListItem("-Select-", "X"));

                if (this.OrderIsPrefill)
                    this.ddlOrderPriority.SelectedValue = this.Order.Priority;
                else
                    this.ddlOrderPriority.SelectedIndex = 0;
            }
        }

        private void BindFrames()
        {
            if (this.OrderDdlData == null) return;
            if (this.OrderDdlData.FrameList == null || this.OrderDdlData.FrameList.Count.Equals(0)) return;

            var l = this.OrderDdlData.FrameList.Where(x => (this.IsSingleVisionRx ? x.FrameType.ToLower() == "s" : x.FrameType.ToLower() == "m") || x.FrameType.ToLower() == "b");

            this.ddlFrame.Items.Clear();
            this.ddlFrame.DataSource = l;
            this.ddlFrame.DataTextField = "FrameCodeAndDescription";
            this.ddlFrame.DataValueField = "FrameCode";
            this.ddlFrame.DataBind();
            this.ddlFrame.Items.Insert(0, new ListItem("-Select-", "X"));

            if (this.OrderIsPrefill)
                this.ddlFrame.SelectedValue = this.Order.Frame;
            else
                this.ddlFrame.SelectedIndex = 0;
        }

        private void ClearOrderDdls()
        {
            this.ddlBridge.Items.Clear();
            this.ddlColor.Items.Clear();
            this.ddlEye.Items.Clear();
            this.ddlFrame.Items.Clear();
            this.ddlLens.Items.Clear();
            this.ddlMaterial.Items.Clear();
            this.ddlTemple.Items.Clear();
            this.ddlTint.Items.Clear();
            this.ddlProdLab.Items.Clear();
            this.ddlCoating.Items.Clear();
        }

        private void BindOrderDdls()
        {
            if (this.OrderDdlData == null) return;
            var l = this.OrderDdlData;
            var pref = this.FramePreferences.Clone();

#region BRIDGE
            try
            {
                this.ddlBridge.ClearSelection();
                this.ddlBridge.DataSource = null;
                this.ddlBridge.DataBind();
                if (l.BridgeList != null && l.BridgeList.Count > 0)
                {
                    this.ddlBridge.DataSource = l.BridgeList;
                    this.ddlBridge.DataTextField = "Key";
                    this.ddlBridge.DataValueField = "Value";
                    this.ddlBridge.DataBind();
                    this.ddlBridge.Items.Insert(0, new ListItem("-Select-", "X"));

                    if (this.OrderIsPrefill)
                    {
                        this.ddlBridge.SelectedValue = this.Order.Bridge;
                    }
                    else
                    {
                        if (this.ddlBridge.Items.Count == 2)
                        {
                            this.ddlBridge.SelectedIndex = 1;
                        }
                        else
                        {
                            if (!pref.Bridge.IsNullOrEmpty() && !pref.Bridge.Equals("G"))
                            {
                                this.ddlBridge.SelectedValue = pref.Bridge.Equals("N") ? "X" : pref.Bridge;
                            }
                            else if (this.DefaultFrameItems != null)
                            {
                                this.ddlBridge.SelectedValue = this.DefaultFrameItems.DefaultBridge;
                            }
                            else
                            {
                                this.ddlBridge.SelectedIndex = 0;
                            }
                        }
                    }
                }
            }
            catch
            {
                this.ddlBridge.Items.Insert(0, new ListItem("-Select-", "X"));
                this.ddlBridge.SelectedIndex = 0;
            }
#endregion

#region COLOR
            try
            {
                this.ddlColor.ClearSelection();
                this.ddlColor.DataSource = null;
                this.ddlColor.DataBind();
                if (l.ColorList != null && l.ColorList.Count > 0)
                {
                    this.ddlColor.DataSource = l.ColorList;
                    this.ddlColor.DataTextField = "Key";
                    this.ddlColor.DataValueField = "Value";
                    this.ddlColor.DataBind();
                    this.ddlColor.Items.Insert(0, new ListItem("-Select-", "X"));

                    if (this.OrderIsPrefill)
                    {
                        this.ddlColor.SelectedValue = this.Order.Color;
                    }
                    else
                    {
                        if (!pref.Color.IsNullOrEmpty() && !pref.Color.Equals("G"))
                        {
                            this.ddlColor.SelectedValue = pref.Color.Equals("N") ? "X" : pref.Color;
                        }
                        else
                        {
                            var i = this.ddlColor.Items.IndexOf(ddlColor.Items.FindByValue("BLK"));

                            if (i != -1)
                            {
                                ddlColor.SelectedIndex = i;
                            }
                            else if (this.ddlColor.Items.Count == 2)
                            {
                                this.ddlColor.SelectedIndex = 1;
                            }
                            else
                            {
                                this.ddlColor.SelectedIndex = 0;
                            }
                        }
                    }
                }
            }
            catch
            {
                this.ddlColor.Items.Insert(0, new ListItem("-Select-", "X"));
                this.ddlColor.SelectedIndex = 0;
            }
#endregion

#region EYE
            try
            {
                this.ddlEye.ClearSelection();
                this.ddlEye.DataSource = null;
                this.ddlEye.DataBind();
                if (l.EyeList != null && l.EyeList.Count > 0)
                {
                    this.ddlEye.DataSource = l.EyeList;
                    this.ddlEye.DataTextField = "Key";
                    this.ddlEye.DataValueField = "Value";
                    this.ddlEye.DataBind();
                    this.ddlEye.Items.Insert(0, new ListItem("-Select-", "X"));

                    if (this.OrderIsPrefill)
                    {
                        this.ddlEye.SelectedValue = this.Order.Eye;
                    }
                    else
                    {
                        if (this.ddlEye.Items.Count == 2)
                        {
                            this.ddlEye.SelectedIndex = 1;
                        }
                        else
                        {
                            if (!pref.Eye.IsNullOrEmpty() && !pref.Eye.Equals("G"))
                            {
                                this.ddlEye.SelectedValue = pref.Eye.Equals("N") ? "X" : pref.Eye;
                            }
                            else if (this.DefaultFrameItems != null)
                            {
                                this.ddlEye.SelectedValue = this.DefaultFrameItems.DefaultEyeSize;
                            }
                            else
                            {
                                this.ddlEye.SelectedIndex = 0;
                            }
                        }
                    }
                }
            }
            catch
            {
                this.ddlEye.Items.Insert(0, new ListItem("-Select-", "X"));
                this.ddlEye.SelectedIndex = 0;
            }
#endregion

#region LENS
            try
            {
                this.ddlLens.ClearSelection();
                this.ddlLens.DataSource = null;
                this.ddlLens.DataBind();
                if (l.LensTypeList != null && l.LensTypeList.Count > 0)
                {
                    //If the prescription has no add power then remove all multivision lenses from the list.
                    if (this.Prescription.OsAdd.Equals(default(Decimal)) && this.Prescription.OdAdd.Equals(default(Decimal)))
                    {
                        var lns = l.LensTypeList.Where(x => x.Value.Substring(0, 2) == "SV").ToList();
                        if (lns.Count.Equals(0))
                        {
                            // The frame selected is for multivision lens types only.
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "DoFail", "Fail(Lens, 'divOrder', 'divOrderError', 'The frame selected is for multivision lens types and the prescription is for single vision eyewear.');", true);
                        }
                        this.ddlLens.DataSource = lns;
                    }
                    else
                        this.ddlLens.DataSource = l.LensTypeList;
                    this.ddlLens.DataTextField = "Key";
                    this.ddlLens.DataValueField = "Value";
                    this.ddlLens.DataBind();
                    this.ddlLens.Items.Insert(0, new ListItem("-Select-", "X"));

                    if (this.OrderIsPrefill)
                    {
                        this.ddlLens.SelectedValue = this.Order.LensType;
                    }
                    else
                    {
                        if (!pref.Lens.IsNullOrEmpty() && !pref.Lens.Equals("G"))
                        {
                            if (pref.Lens.Equals("N"))
                                this.ddlLens.SelectedValue = "X";
                            else if (this.ddlLens.Items.Cast<ListItem>().ToList().Any(x => x.Value == pref.Lens))
                                this.ddlLens.SelectedValue = pref.Lens;
                            else
                                this.ddlLens.SelectedValue = "X";
                        }
                        else
                        {
                            var i = this.ddlLens.Items.IndexOf(ddlLens.Items.FindByValue("SVD"));
                            if (i != -1)
                            {
                                ddlLens.SelectedIndex = i;
                            }
                            else if (this.ddlLens.Items.Count == 2)
                            {
                                this.ddlLens.SelectedIndex = 1;
                            }
                            else
                            {
                                this.ddlLens.SelectedIndex = 0;
                            }
                        }
                    }
                }
            }
            catch
            {
                this.ddlLens.Items.Insert(0, new ListItem("-Select-", "X"));
                this.ddlLens.SelectedIndex = 0;
            }
#endregion

#region MATERIAL
            try
            {
                this.ddlMaterial.ClearSelection();
                this.ddlMaterial.DataSource = null;
                this.ddlMaterial.DataBind();
                if (l.MaterialList != null && l.MaterialList.Count > 0)
                {
                    this.ddlMaterial.DataSource = l.MaterialList;
                    this.ddlMaterial.DataTextField = "Key";
                    this.ddlMaterial.DataValueField = "Value";
                    this.ddlMaterial.DataBind();
                    this.ddlMaterial.Items.Insert(0, new ListItem("-Select-", "X"));

                    if (this.OrderIsPrefill)
                    {
                        this.ddlMaterial.SelectedValue = this.Order.Material;
                    }
                    else
                    {
                        //New for hi-index stuff
                        if (!pref.Material.IsNullOrEmpty() && !pref.Material.Equals("G"))
                            this.ddlMaterial.SelectedValue = pref.Material.Equals("N") ? "X" : pref.Material;
                        else
                        {
                            var i = -1;
                            if (this.ddlFrame.SelectedValue != "UPLC")
                            {
                                if (!p.IsHighPowerLensRx())
                                {
                                    i = this.ddlMaterial.Items.IndexOf(ddlMaterial.Items.FindByValue("PLAS"));
                                }
                                else
                                {
                                    i = this.ddlMaterial.Items.IndexOf(ddlMaterial.Items.FindByValue("HI"));
                                }
                            }
                            else
                            {
                                i = this.ddlMaterial.Items.IndexOf(ddlMaterial.Items.FindByValue("POLY"));
                            }
                            if (i != -1)
                            {
                                ddlMaterial.SelectedIndex = i;
                            }
                            else if (this.ddlMaterial.Items.Count == 2)
                            {
                                this.ddlMaterial.SelectedIndex = 1;
                            }
                            else
                            {
                                this.ddlMaterial.SelectedIndex = 0;
                            }
                        }
                        //End of New for hi-index stuff
                    }

                    // Determine if the material is in stock --- MAYBE
                }
            }
            catch
            {
                this.ddlMaterial.Items.Insert(0, new ListItem("-Select-", "X"));
                this.ddlMaterial.SelectedIndex = 0;
            }
#endregion

#region TEMPLE
            try
            {
                this.ddlTemple.ClearSelection();
                this.ddlTemple.DataSource = null;
                this.ddlTemple.DataBind();
                l.TempleList = (l.TempleList.OrderBy(x => (x.Value.Substring(0, 3)))).ToDictionary((Key) => Key.Key, (Val) => Val.Value);
                if (l.TempleList != null && l.TempleList.Count > 0)
                {
                    this.ddlTemple.DataSource = l.TempleList;
                    this.ddlTemple.DataTextField = "Key";
                    this.ddlTemple.DataValueField = "Value";
                    this.ddlTemple.DataBind();
                    this.ddlTemple.Items.Insert(0, new ListItem("-Select-", "X"));

                    if (this.OrderIsPrefill)
                    {
                        this.ddlTemple.SelectedValue = this.Order.Temple;
                    }
                    else
                    {
                        if (this.ddlTemple.Items.Count == 2)
                        {
                            this.ddlTemple.SelectedIndex = 1;
                        }
                        else
                        {
                            if (!pref.Temple.IsNullOrEmpty() && !pref.Temple.Equals("G"))
                            {
                                this.ddlTemple.SelectedValue = pref.Temple.Equals("N") ? "X" : pref.Temple;
                            }
                            else if (this.DefaultFrameItems != null)
                            {
                                this.ddlTemple.SelectedValue = this.DefaultFrameItems.DefaultTemple;
                            }
                            else
                            {
                                this.ddlTemple.SelectedIndex = 0;
                            }
                        }
                    }
                }
            }
            catch
            {
                this.ddlTemple.Items.Insert(0, new ListItem("-Select-", "X"));
                this.ddlTemple.SelectedIndex = 0;
            }
#endregion

#region TINT
            try
            {
                this.ddlTint.ClearSelection();
                this.ddlTint.DataSource = null;
                this.ddlTint.DataBind();
                if (l.TintList != null && l.TintList.Count > 0)
                {
                    this.ddlTint.DataSource = l.TintList;
                    this.ddlTint.DataTextField = "Key";
                    this.ddlTint.DataValueField = "Value";
                    this.ddlTint.DataBind();
                    this.ddlTint.Items.Insert(0, new ListItem("-Select-", "X"));

                    if (this.OrderIsPrefill)
                    {
                        this.ddlTint.SelectedValue = this.Order.Tint;
                    }
                    else
                    {
                        if (!pref.Tint.IsNullOrEmpty() && !pref.Tint.Equals("G"))
                        {
                            this.ddlTint.SelectedValue = pref.Tint.Equals("N") ? "X" : pref.Tint;
                        }
                        else
                        {
                            var i = this.ddlTint.Items.IndexOf(ddlTint.Items.FindByValue("CL"));
                            if (i != -1)
                            {
                                ddlTint.SelectedIndex = i;
                            }
                            else if (this.ddlTint.Items.Count == 2)
                            {
                                this.ddlTint.SelectedIndex = 1;
                            }
                            else
                            {
                                this.ddlTint.SelectedIndex = 0;
                            }
                        }
                    }
                }
            }
            catch
            {
                this.ddlTint.Items.Insert(0, new ListItem("-Select-", "X"));
                this.ddlTint.SelectedIndex = 0;
            }
#endregion

#region COATING
            try
            {
                this.ddlCoating.ClearSelection();
                this.ddlCoating.Items.Clear();
                this.ddlCoating.DataSource = null;
                this.ddlCoating.DataBind();
                if (l.CoatingList != null && l.CoatingList.Count > 0)
                {
                    this.ddlCoating.DataSource = l.CoatingList;
                    this.ddlCoating.DataTextField = "Key";
                    this.ddlCoating.DataValueField = "Value";


                    this.ddlCoating.DataBind();
                   // this.ddlCoating.Items.Insert(0, new ListItem("-Select-", "X"));

                    if (this.OrderIsPrefill)
                    {
                        //this.ddlCoating.SelectedValue = this.Order.Coatings;
                        if (!this.Order.Coatings.IsNullOrEmpty())
                        {
                            string[] coatings = this.Order.Coatings.Split(new[] { "," }, StringSplitOptions.None);
                            foreach (string s in coatings)
                                ddlCoating.Items.FindByValue(s).Selected = true;
                        }
                    }
                    else
                    {
                        if (!pref.Coatings.IsNullOrEmpty() /*&& !pref.Coating.Equals("G")*/)
                        {
                            string[] coatings = pref.Coatings.Split(new[] { "," }, StringSplitOptions.None);
                            foreach (string s in coatings)
                                ddlCoating.Items.FindByValue(s).Selected = true;
                            //this.ddlCoating.SelectedValue = /*pref.Coating.Equals("N") ? "X" :*/ pref.Coatings;
                        }
                        else
                        {
                            if (!this.Order.Coatings.IsNullOrEmpty())
                            {
                                string[] coatings = this.Order.Coatings.Split(new[] { "," }, StringSplitOptions.None);
                                foreach (string s in coatings)
                                    ddlCoating.Items.FindByValue(s).Selected = true;
                            }

                    //        var i = this.ddlCoating.Items.IndexOf(ddlCoating.Items.FindByValue("UV"));
                    //        if (i != -1)
                    //        {
                    //            ddlCoating.SelectedIndex = i;
                    //        }
                    //        else if (this.ddlCoating.Items.Count == 2)
                    //        {
                    //            this.ddlCoating.SelectedIndex = 1;
                    //        }
                    //        else
                    //        {
                    //            this.ddlCoating.SelectedIndex = 0;
                    //        }
                        }
                    }
                }
            }
            catch
            {
                //this.ddlCoating.Items.Insert(0, new ListItem("-Select-", "X"));
                //this.ddlCoating.SelectedIndex = 0;
            }
#endregion


#region SEG HT
            if (!this.ddlLens.SelectedValue.StartsWith("SV") && this.ddlLens.SelectedIndex != 0)
            {
                if (this.OrderIsPrefill)
                {
                    this.tbOdSegHt.Text = this.Order.OdSegHeight;
                    this.tbOsSegHt.Text = this.Order.OsSegHeight;
                }
                else
                {
                    // Set seg ht preferences
                    if (!pref.OdSegHt.IsNullOrEmpty())
                        this.tbOdSegHt.Text = pref.OdSegHt;
                    if (!pref.OsSegHt.IsNullOrEmpty())
                        this.tbOsSegHt.Text = pref.OsSegHt;
                }
            }
#endregion

            BindLabDdl();
            BindReOrderDdl();
        }

        private void BindSpecialTints()
        {
            var currTint = this.ddlTint.SelectedValue;
            this.ddlTint.DataSource = this.OrderDdlData.TintList;
            this.ddlTint.DataBind();
            this.ddlTint.Items.Insert(0, new ListItem("-Select-", "X"));
            if ((ddlTint.Items.Cast<ListItem>().Select(i => i.Value).ToList()).Any(x => x.Contains(currTint)))
            {
                this.ddlTint.SelectedValue = currTint;
            }
            else
            {
                // Clear is the default selection.  If no clear tint then set index to 0.
                var t = this.OrderDdlData.TintList.FirstOrDefault(x => x.Key.ToLower() == "clear");
                if (t.IsNull())
                    this.ddlTint.SelectedIndex = 0;
                else
                    this.ddlTint.SelectedValue = t.Value;
            }
        }

        private void BindLabDdl()
        {
            if (this.OrderDdlData == null) return;

            try
            {
                var labList = this.OrderDdlData.LabSiteCodeList;

                var l = new Dictionary<String, String>();
                // Determine if any of the labs associated with the site can fabricate the order based on the material
                foreach (var a in labList)
                {
                    if (p.CanLabFabricate(a.Value.Substring(1)).Equals(false)) continue;
                    l.Add(a.Key, a.Value);
                }

                switch (l.Count)
                {
                    case 0:
                        l.Add("MV - MNOST1", "0MNOST1");
                        break;
                    case 1:
                        // If the lens multivision and only the single vision lab is avaiable then use nostra
                        if (this.Order.LensType.StartsWith("SV").Equals(false) && l.FirstOrDefault().Key.StartsWith("SV"))
                        {
                            var ld = l.Cast<KeyValuePair<String, String>>().ToList();
                            ld.Insert(0, new KeyValuePair<String, String>("MV - MNOST1", "0MNOST1"));
                            l = ld.ToDictionary(x => x.Key, x => x.Value);
                        }
                        break;
                }

                // Set the drop down list
                this.ddlProdLab.ClearSelection();
                this.ddlProdLab.DataSource = null;
                this.ddlProdLab.DataBind();
                if (l != null && l.Count > 0)
                {
                    this.ddlProdLab.DataSource = l;
                    this.ddlProdLab.DataTextField = "Key";
                    this.ddlProdLab.DataValueField = "Value";
                    this.ddlProdLab.DataBind();

                    if (l.Count.Equals(1)) { this.ddlProdLab.SelectedIndex = 0; return; }

                    this.ddlProdLab.Items.Insert(0, new ListItem("-Select-", "X"));

                    if (String.IsNullOrEmpty(ddlLens.SelectedValue)) return;
                    switch (ddlLens.SelectedValue.Substring(0, 1))
                    {
                        case "S":
                            if (this.ddlProdLab.Items[2] == null) break;
                            // If a lab preference exists then use it.
                            if (!this.OrderPreferences.Lab.IsNullOrEmpty())
                            {
                                this.ddlProdLab.SelectedValue = this.OrderPreferences.Lab;
                                // Set the hidden value so the java script function SetLab is aware of a potential preference.
                                this.hfLabPreference.Value = this.OrderPreferences.Lab.StartsWith("0M") ? "MV" : "SV";
                                break;
                            }
                            this.ddlProdLab.SelectedIndex = 2;
                            break;

                        case "B":
                        case "D":
                        case "Q":
                        case "T":
                            if (this.ddlProdLab.Items[1] == null) break;
                            this.ddlProdLab.SelectedIndex = 1;
                            break;

                        default:
                            this.ddlProdLab.SelectedIndex = 0;
                            break;
                    }


                    if (this.ddlProdLab.SelectedIndex == 0)
                    {
                        this.txtProdLab.Text = "";
                    }
                    else
                    {
                        this.txtProdLab.Text = ddlProdLab.SelectedItem.Text;
                    }
                }
            }
            catch
            {
                this.ddlProdLab.Items.Insert(0, new ListItem("-Select-", "X"));
                this.ddlProdLab.SelectedIndex = 0;
                this.txtProdLab.Text = "";
            }
            
        }

        private void BindReOrderDdl()
        {
            try
            {
                this.ddlReorderReason.ClearSelection();
                this.ddlReorderReason.DataSource = null;
                this.ddlReorderReason.DataBind();
                if (this.OrderDdlData.ReorderList.IsNullOrEmpty()) throw new Exception("No Re-Order Reasons available for load.");

                this.ddlReorderReason.DataSource = this.OrderDdlData.ReorderList;
                this.ddlReorderReason.DataTextField = "Key";
                this.ddlReorderReason.DataValueField = "Value";
                this.ddlReorderReason.DataBind();
                this.ddlReorderReason.Items.Insert(0, new ListItem("-Select-", "X"));
                this.ddlReorderReason.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                this.ddlReorderReason.Items.Insert(0, new ListItem("-Select-", "X"));
                this.ddlReorderReason.SelectedIndex = 0;
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex.InnerException.IsNull() ? ex : ex.InnerException);
            }
        }

        private void BindLabOrderHistoryGrid()
        {
            if (Roles.GetRolesForUser().Any(x => x.ToLower().Contains("lab")))
            {
                this.gvLabOrders.DataSource = this.OrderList.Where(x => x.IsComplete == true && x.IsActive == true);
                this.gvLabOrders.DataBind();
            }
        }

        private Boolean GetSelectedOrder(String key, Boolean forPrefill)
        {
#if DEBUG
            using (MethodTracer.Trace(SrtsTraceSource.ClinicOrderSource, "OrderManagement_GetSelectedOrder", mySession.MyUserID))
#endif
            {
                Order o = null;
                if (!forPrefill)
                    o = p.GetOrderByOrderNumber(key);
                else
                    o = this.Order.Clone();

                if (o == null) { Session["FAIL"] = String.Format("Error retrieving order: {0}", key); return false; }

                if (o.IsGEyes)
                {
                    DoGeyesOrder(o);
                    p.GetOrderStatusHistory(o.OrderNumber);
                    return true;
                }
                hfOrderStatus.Value = o.CurrentStatus.ToString();
                GetSelectedPrescription(o.PrescriptionId);

                if (!forPrefill)
                    p.GetOrderStatusHistory(o.OrderNumber);

                p.GetPriorities();
                BindOrderPriorities();

                p.GetFrames(o.Demographic, o.ClinicSiteCode, o.PrescriptionId);
                BindFrames();

                p.GetFrameItems(String.Format("000000B{0}", o.Priority), o.Frame);
                p.GetLabs(o.Frame, o.Tint);
                BindOrderDdls();

                this.Order = o;

                SetGeyesOrLabOrderState(false);

                SetOrderControlState();

                SetShipToPatient();

                this.divRejectBlock.Visible = forPrefill ? false : this.OrderStateHistory.FirstOrDefault(x => x.IsActive == true).OrderStatusTypeID.Equals(3);
                this.hrRejectBlock.Visible = forPrefill ? false : this.OrderStateHistory.FirstOrDefault(x => x.IsActive == true).OrderStatusTypeID.Equals(3);

                this.hfMaxPair.Value = String.Empty;
                this.hfMaxPair.Value = this.OrderDdlData.FrameList.Where(x => x.FrameCode == this.Order.Frame).Select(m => m.MaxPair).FirstOrDefault().ToString();

                this.hfIsFocFrame.Value = String.Empty;
                this.hfIsFocFrame.Value = this.OrderDdlData.FrameList.Where(x => x.FrameCode == this.Order.Frame).Select(m => m.IsFoc).FirstOrDefault().ToString().ToLower();
                //this.hfIsFocEligible.Value = IsEligibleFocOrder() ? "Y" : "N";
                this.hfIsFocEligible.Value = IsEligibleFocOrder() && string.IsNullOrEmpty(o.FocJustification) ? "N" : "Y";

                return true;
            }
        }

        private void SetPriorityFramePreferences(OrderManagementPresenter p)
        {
            if (this.OrderPreferences.InitialLoadPriority.IsNullOrEmpty()) return;

            if (this.OrderDdlData.PriorityList.Any(x => x.Value == this.OrderPreferences.InitialLoadPriority).Equals(false)) return;
            this.ddlOrderPriority.SelectedValue = this.OrderPreferences.InitialLoadPriority;

            doPriorityLoad();

            var demo = String.Format("{0}{1}", this.Demographic.Substring(0, 7), this.OrderPreferences.InitialLoadPriority);

            if (this.OrderPreferences.InitialLoadFrame.IsNullOrEmpty() && !this.OrderPreferences.PriorityFrameCombo.Any(x => x.PriorityCode == this.OrderPreferences.InitialLoadPriority)) { return; }

            var fc = this.OrderPreferences.InitialLoadFrame.IsNullOrEmpty() ?
                this.OrderPreferences.PriorityFrameCombo.FirstOrDefault(x => x.PriorityCode == this.OrderPreferences.InitialLoadPriority).FrameCode : this.OrderPreferences.InitialLoadFrame;

            if (!this.OrderDdlData.FrameList.Any(x => x.FrameCode == fc)) return;

            this.ddlFrame.SelectedValue = fc;

            DoFrameLoad();
        }

        private void DoOrderWork(Int32 key)
        {
            if (!this.OrderStateHistory.IsNull())
                this.OrderStateHistory.Clear();

            if (!GetSelectedPrescription(key)) return;
#if DEBUG
            using (MethodTracer.Trace(SrtsTraceSource.ClinicOrderSource, "OrderManagement_DoOrderWork", mySession.MyUserID))
#endif
            {
                ClearOrderDdls();
                this.p = new OrderManagementPresenter(this);
                if (!this.OrderIsPrefill)
                    this.Order = new Order();
                p.GetPriorities();
                BindOrderPriorities();
                // Set order preferences for priority and frame.
                SetPriorityFramePreferences(p);
                SetGeyesOrLabOrderState(false);

                // disable/enable seg ht when RX has no Add power
                var enable = this.Prescription.OdAdd != default(Decimal) && this.Prescription.OsAdd != default(Decimal);
                this.tbOdSegHt.Enabled = enable;
                this.tbOsSegHt.Enabled = enable;

                SetShipToPatient();

                ScriptManager.RegisterStartupScript(this, this.GetType(), "OrderDialog", "DoAddOrder();", true);
                ScriptManager.RegisterStartupScript(this, this.GetType(), "hideReject", "DoRejectRedirect('none');", true);

                // Determine if the sphere or cylander signs (+-) are the same.  If one of the numbers is 0 then false.
                this.hfOppositeSign.Value = String.Empty;
                var ods = this.Prescription.OdSphere.ToStringWithSign();
                var oss = this.Prescription.OsSphere.ToStringWithSign();
                var odc = this.Prescription.OdCylinder.ToStringWithSign();
                var osc = this.Prescription.OsCylinder.ToStringWithSign();

                if (this.Prescription.OdSphere != 0 && this.Prescription.OsSphere != 0)
                    if (ods.Substring(0, 1).Equals("+") && oss.Substring(0, 1).Equals("-") || (ods.Substring(0, 1).Equals("-") && oss.Substring(0, 1).Equals("+")))
                    {
                        this.hfOppositeSign.Value = "Opposite signs verified.";
                        return;
                    }

                if (this.Prescription.OdCylinder != 0 && this.Prescription.OsCylinder != 0)
                    if (odc.Substring(0, 1).Equals("+") && osc.Substring(0, 1).Equals("-") || (odc.Substring(0, 1).Equals("-") && osc.Substring(0, 1).Equals("+")))
                        this.hfOppositeSign.Value = "Opposite signs verified.";
            }
        }

        private void SetOrderControlState()
        {
#if DEBUG
            using (MethodTracer.Trace(SrtsTraceSource.ClinicOrderSource, "OrderManagement_SetOrderControlState", mySession.MyUserID))
#endif
            {
                var text = String.IsNullOrEmpty(this.Order.OrderNumber) ? "Submit and Close" : "Update";
                var isSave = text.ToLower().Equals("submit and close");
                var isUpdateable = this.OrderStateHistory == null || this.OrderStateHistory.Count == 0 ? true : this.OrderStateHistory.Any(x => (x.IsActive == true &&
                    (x.OrderStatusTypeID == 1 || x.OrderStatusTypeID == 3 || x.OrderStatusTypeID == 9 || x.OrderStatusTypeID == 15 || x.OrderStatusTypeID == 21)));

                if (isUpdateable)
                    this.bSaveOrder.Style.Remove("display");
                else
                    this.bSaveOrder.Style.Add("display", "none");

                this.bSaveCreateNewOrder.Visible = isSave;
                this.bSaveOrder.Text = text;
                this.bSaveNewOrder.Visible = !isSave && this.Order.IsComplete;
                this.bSaveIncompleteOrder.Visible = isSave;
                this.bReprint771.Visible = !isSave && this.Order.IsComplete;

                this.ddlProdLab.Enabled = false;//isUpdateable;

                this.cbDelete.Visible = !isSave && isUpdateable;
                this.labelOrderNumber.Visible = !isSave;
                this.lblTechnician.Visible = !isSave;

                // Show the recreturn to stock checkbox when there is order history present and the currently active status is either 11 or 16
                this.cbReturnToStock.Visible = this.OrderStateHistory == null || this.OrderStateHistory.Count == 0 ? 
                    false : 
                    this.OrderStateHistory.Any(x => (x.IsActive == true && (x.OrderStatusTypeID == 11 || x.OrderStatusTypeID == 16)));
                this.cbDispense.Visible = this.OrderStateHistory == null || this.OrderStateHistory.Count == 0 ? false : this.OrderStateHistory.Any(x => (x.IsActive == true && x.OrderStatusTypeID == 17));
                this.divRejectBlock.Visible = this.OrderStateHistory == null || this.OrderStateHistory.Count == 0 ? false : this.OrderStateHistory.Any(x => (x.IsActive == true && x.OrderStatusTypeID == 3));
            }
        }

        private void SetGeyesOrLabOrderState(Boolean isGeyes)
        {
            this.ddlOrderPriority.Enabled = !isGeyes;
            this.ddlFrame.Enabled = !isGeyes;
            this.ddlBridge.Enabled = !isGeyes;
            this.ddlColor.Enabled = !isGeyes;
            this.ddlEye.Enabled = !isGeyes;
            this.ddlLens.Enabled = !isGeyes;
            this.ddlMaterial.Enabled = !isGeyes;
            this.ddlTemple.Enabled = !isGeyes;
            this.ddlTint.Enabled = !isGeyes;
            this.ddlCoating.Enabled = !isGeyes;
            this.ddlProdLab.Enabled = false;//!isGeyes;
            this.tbOdSegHt.Enabled = !isGeyes;
            this.tbOsSegHt.Enabled = !isGeyes;
            this.tbComment1.Enabled = !isGeyes;
            this.tbComment2.Enabled = !isGeyes;
            this.bSaveIncompleteOrder.Visible = !isGeyes;
            this.bSaveNewOrder.Visible = !isGeyes;
            this.bSaveOrder.Visible = !isGeyes;
            this.bSaveCreateNewOrder.Visible = !isGeyes;
            //this.bVerifyAddress.Visible = !isGeyes;
            this.cbDelete.Visible = !isGeyes;
            this.cbReturnToStock.Visible = !isGeyes;
            this.cbDispense.Visible = !isGeyes;
            this.tbPair.Enabled = !isGeyes;
            this.ddlShipTo.Enabled = !isGeyes;
        }

        private void SetShipToPatient()
        {
#if DEBUG
            using (MethodTracer.Trace(SrtsTraceSource.ClinicOrderSource, "OrderManagement_SetShipToPatient", mySession.MyUserID))
#endif
            {
                //var od = this.Order.OrderDisbursement;
                var od = "";
                if (Session["CurrentOrderDisbursement"] != null)
                {
                    od = Session["CurrentOrderDisbursement"].ToString();
                }
                else
                {
                    od = this.Order.OrderDisbursement;
                }
                p.IsPatientAddressActive(this.PatientId);

                if (this.PatientHasAddress && !String.IsNullOrEmpty(od))
                {
                    if (this.OrderPreferences.DistributionMethod.IsNullOrEmpty())
                    {
                        // Get preferences
                        p.GetOrderPreferences();
                    }
                    // If a distribution method preference exists then use it.
                    this.ddlShipTo.SelectedValue = this.Order.OrderNumber.IsNullOrEmpty() ? this.OrderPreferences.DistributionMethod.IsNullOrEmpty() ? od : this.OrderPreferences.DistributionMethod : od;
                    Session["CurrentOrderDisbursement"] = this.ddlShipTo.SelectedValue;
                    ScriptManager.RegisterStartupScript(upTmpLimitEBT, upTmpLimitEBT.GetType(), "DoAddressValidation", "DoAddressValidation();", true);
                }
                else
                {
                    this.ddlShipTo.Items.Clear();
                    this.ddlShipTo.Items.Insert(0, new ListItem("Clinic Distribution", "CD"));
                    this.ddlShipTo.SelectedValue = "CD";
                    this.lblShipTo.ToolTip = "Patient address is required to see ship to patient options";
                }     
            }

            //// check lab mtp status
            //GetLabMTPOptions();

        }

        //protected void GetLabMTPOptions()  //Aldela 05/03/2019: Commented this function 
        //{
        //    var status =  p.IsLabMTP().ToList();
        //    bool statusShow = true;
        //    List<string> labsNotMTP = new List<string>();
        //    Session["labsNotMTP"] = "";
        //    foreach (DataRow row in status)
        //    {
        //        dynamic siteLab = new ExpandoObject();
        //        siteLab.Name = row["SiteCode"];
        //        siteLab.Status = row["ShipToPatientLab"];
        //        statusShow = siteLab.Status;

        //        if (statusShow == false)
        //            labsNotMTP.Add(siteLab.Name);
        //    }

        //    // check lab mtp status
        //    GetLabMTPOptions();

        //}

        //protected void GetLabMTPOptions()
        //{
        //    var status =  p.IsLabMTP().ToList();
        //    bool statusShow = true;
        //    List<string> labsNotMTP = new List<string>();
        //    Session["labsNotMTP"] = "";
        //    foreach (DataRow row in status)
        //    {
        //        dynamic siteLab = new ExpandoObject();
        //        siteLab.Name = row["SiteCode"];
        //        siteLab.Status = row["ShipToPatientLab"];
        //        statusShow = siteLab.Status;

        //        if (statusShow == false)
        //            labsNotMTP.Add(siteLab.Name);
        //    }
        //   // Session["labsNotMTP"] = labsNotMTP;
        //    hdfProdLabs.Value = String.Join(",", labsNotMTP);
        //}

        private void DoGeyesOrder(Order o)
        {
            // Need to get InitialLoadFrame, Color, Lens, Tint, Coating, and Material descriptions from DB.
            this.lblOrderNumber.Text = o.OrderNumber;

            this.ddlOrderPriority.Items.Clear();
            this.ddlOrderPriority.Items.Insert(0, new ListItem(o.Demographic.ToOrderPriorityValue(), o.Demographic.ToOrderPriorityKey()));
            this.ddlOrderPriority.SelectedIndex = 0;

            this.ddlFrame.Items.Clear();
            this.ddlFrame.Items.Insert(0, new ListItem(o.Frame, o.Frame));
            this.ddlFrame.SelectedIndex = 0;

            this.ddlBridge.Items.Clear();
            this.ddlBridge.Items.Insert(0, new ListItem(o.Bridge, o.Bridge));
            this.ddlBridge.SelectedIndex = 0;

            this.ddlColor.Items.Clear();
            this.ddlColor.Items.Insert(0, new ListItem(o.Color, o.Color));
            this.ddlColor.SelectedIndex = 0;

            this.ddlEye.Items.Clear();
            this.ddlEye.Items.Insert(0, new ListItem(o.Eye, o.Eye));
            this.ddlEye.SelectedIndex = 0;

            this.ddlLens.Items.Clear();
            this.ddlLens.Items.Insert(0, new ListItem(o.LensType, o.LensType));
            this.ddlLens.SelectedIndex = 0;

            this.ddlMaterial.Items.Clear();
            this.ddlMaterial.Items.Insert(0, new ListItem(o.Material, o.Material));
            this.ddlMaterial.SelectedIndex = 0;

            this.ddlTemple.Items.Clear();
            this.ddlTemple.Items.Insert(0, new ListItem(o.Temple, o.Temple));
            this.ddlTemple.SelectedIndex = 0;

            this.ddlTint.Items.Clear();
            this.ddlTint.Items.Insert(0, new ListItem(o.Tint, o.Tint));
            this.ddlTint.SelectedIndex = 0;

            this.ddlCoating.Items.Clear();
            this.ddlCoating.Items.Insert(0, new ListItem(o.Coatings, o.Coatings));
         
            this.ddlProdLab.Items.Clear();
            this.ddlProdLab.Items.Insert(0, new ListItem(o.LabSiteCode, o.LabSiteCode));
            this.ddlProdLab.SelectedIndex = 0;

            this.tbOdSegHt.Text = o.OdSegHeight;
            this.tbOsSegHt.Text = o.OsSegHeight;
            this.tbComment1.Text = o.Comment1;
            this.tbComment2.Text = o.Comment2;

            SetGeyesOrLabOrderState(true);
        }

        private Boolean IsEligibleFocOrder()
        {
            if (this.Patient.NextFocDate == null) return true;
            if (this.Patient.NextFocDate <= DateTime.Now) return true;

            return false;
        }

        private void TempFrameRestrictions()
        {
            if (!this.Order.Frame.In("350", "EXCEL", "EXC51")) return;
#if DEBUG
            using (MethodTracer.Trace(SrtsTraceSource.ClinicOrderSource, "OrderManagement_TempFrameRestrictions", mySession.MyUserID))
#endif
            {
                var reload = false;
                var ee = this.Order.Eye;

                switch (this.Order.Frame)
                {
                    case "350":
                        {
                            if (this.Order.Color.Equals("GLD") && ee.Equals("50"))
                            {
                                // Remove the 140 and 145 temple sizes
                                this.ddlTemple.Items.Remove(this.ddlTemple.Items.FindByValue("145SKL"));
                                this.ddlTemple.Items.Remove(this.ddlTemple.Items.FindByValue("140SKL"));
                                break;
                            }

                            if (this.ddlTemple.Items.FindByValue("140SKL") != null) break;
                            reload = true;
                            break;
                        }
                    case "EXC51":
                        {
                            var br = this.Order.Bridge;
                            if (this.Order.Color.Equals("BRN") && ee.Equals("51") && br.Equals("20"))
                            {
                                this.ddlTemple.Items.Remove(this.ddlTemple.Items.FindByValue("145SKL"));
                                this.ddlTemple.Items.Remove(this.ddlTemple.Items.FindByValue("140SKL"));
                                break;
                            }

                            if (this.ddlTemple.Items.FindByValue("145SKL") != null) break;
                            reload = true;
                            break;
                        }
                }

                if (reload)
                {
                    this.ddlTemple.DataSource = OrderDdlData.TempleList;
                    this.ddlTemple.DataBind();
                    this.ddlTemple.Items.Insert(0, new ListItem("-Select-", "X"));
                    this.ddlTemple.SelectedIndex = 0;
                }

                ScriptManager.RegisterStartupScript(upTmpLimitEBT, upTmpLimitEBT.GetType(), "ColorVal", "ComboChangedGeneric(Color, 'Color');", true);
                ScriptManager.RegisterStartupScript(upTmpLimitEBT, upTmpLimitEBT.GetType(), "EyeVal", "ComboChangedGeneric(Eye, 'Eye');", true);
                ScriptManager.RegisterStartupScript(upTmpLimitEBT, upTmpLimitEBT.GetType(), "BridgeVal", "ComboChangedGeneric(Bridge, 'Bridge');", true);
                ScriptManager.RegisterStartupScript(upTmpLimitEBT, upTmpLimitEBT.GetType(), "TempleVal", "ComboChangedGeneric(Temple, 'Temple');", true);
                if (!this.cbDelete.Checked) return;
                ScriptManager.RegisterStartupScript(upTmpLimitEBT, upTmpLimitEBT.GetType(), "DoDeleteAction", "DoDeleteCheckAction();", true);
            }
        }

        public bool OrderIsPrefill
        {
            get { return ViewState["OrderIsPrefill"].ToBoolean(); }
            set { ViewState["OrderIsPrefill"] = value; this.hfOrderIsPrefill.Value = value.ToString(); }
        }

        public Order OriginalOrder
        {
            get { return ViewState["OriginalOrder"] as Order; }
            set { ViewState["OriginalOrder"] = value; }
        }

#region LAB ORDERS

        protected void bLabStatusChange_Click(object sender, EventArgs e)
        {
            try
            {
#if DEBUG
                using (MethodTracer.Trace(SrtsTraceSource.LabOrderSource, "OrderManagement_bLabStatusChange_Click", mySession.MyUserID))
#endif
                {
                    p.EditStatusForLab(this.LabAction, this.cbCheckInHfsOrder.Checked);
                    var dto = Session["PatientOrderDTO"] as PatientOrderDTO;
                    dto.OrderNumber = String.Empty;
                    Session["PatientOrderDTO"] = dto;

                    LogEvent(String.Format("Order was {0} by user {1} at {2}", this.LabAction.Equals("redirect") ? "redirected" : "rejected", mySession.MyUserID, DateTime.Now));
                    if(!CustomRedirect.SanitizeRedirect(this.Request.Url.ToString(),false))
                    {
                        ShowErrorDialog("An error has occurred with the status change.");
                    }
                }
            }
            catch (Exception ex) { ex.TraceErrorException(); }
        }

        protected void gvLabOrders_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            var i = default(Int32);
            if (!Int32.TryParse(e.CommandArgument.ToString(), out i)) return;
             try
                {
#if DEBUG
                    using (MethodTracer.Trace(SrtsTraceSource.LabOrderSource, "OrderManagement_gvLabOrders_RowCommand", mySession.MyUserID))
#endif
                    {
                        var k = ((GridView)sender).DataKeys[i];

                        if (!GetSelectedOrderForLab(k.Value.ToString()))
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ErrorAlert", "alert('You do not have permissions to view this order.');", true);
                            return;
                        }

                        ScriptManager.RegisterStartupScript(this, this.GetType(), "OrderDialog", "DoSelectedOrder('true');", true);
                    }
                }
                catch (Exception ex) { ex.TraceErrorException(); }

         }
          

        private Boolean GetSelectedOrderForLab(String key)
        {
#if DEBUG
            using (MethodTracer.Trace(SrtsTraceSource.LabOrderSource, "OrderManagement_GetSelectedOrderForLab", mySession.MyUserID))
#endif
            {
                var o = this.OrderList.Where(x => x.OrderNumber == key).FirstOrDefault();
                if (o == null) return false;

                GetSelectedPrescription(o.PrescriptionId);
                if (this.Prescription.PrescriptionScanId > 0)
                {
                    divViewPrescriptionDoc.Visible = true;
                }
                p.GetOrderStatusHistory(o.OrderNumber);

                p.GetPriorities();
                BindOrderPriorities();
                p.GetFrames(o.Demographic, o.ClinicSiteCode, o.PrescriptionId);
                BindFrames();
                p.GetFrameItems(String.Format("000000B{0}", o.Priority), o.Frame);
                p.GetLabs(o.Frame, o.Tint);
                BindOrderDdls();

                this.Order = o;

                p.GetLabListForRedirect();
                BindRedirectLabs();

                p.GetLabJustificationPreferences();

                SetGeyesOrLabOrderState(true);

                var curOrderState = this.OrderStateHistory.FirstOrDefault(x => x.IsActive == true);
                switch (curOrderState.OrderStatusTypeID)
                {
                    case 1:
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "hideReject", "DoRejectRedirect('none');", true);
                        this.divRejectBlock.Visible = false;
                        this.hrRejectBlock.Visible = false;
                        this.bReprint771.Enabled = false;
                        break;

                    case 2:
                    case 4:
                        if (curOrderState.LabCode.ToLower() == mySession.MySite.SiteCode.ToLower())
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "hideReject", "DoRejectRedirect('redirect');", true);
                            this.bReprint771.Enabled = true;
                        }
                        else
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "hideReject", "DoRejectRedirect('none');", true);
                            this.bReprint771.Enabled = false;
                        }
                        break;

                    case 3:
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "hideReject", "DoRejectRedirect('none');", true);
                        this.divRejectBlock.Visible = true;
                        this.hrRejectBlock.Visible = true;
                        this.tbRejectLabReason.Enabled = false;
                        this.tbRejectClinicReply.Enabled = false;
                        this.bReprint771.Enabled = true;
                        break;

                    case 5:
                        if (curOrderState.LabCode.ToLower() == mySession.MySite.SiteCode.ToLower())
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "hideReject", "DoRejectRedirect('hfs');", true);
                            this.rblRejectRedirect.SelectedIndex = 2;
                            // Set the current hold for stock status
                            this.lblCurrentHoldForStock.Text = curOrderState.StatusComment;
                            this.bReprint771.Enabled = true;
                        }
                        else
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "hideReject", "DoRejectRedirect('none');", true);
                            this.bReprint771.Enabled = false;
                        }
                        break;

                    case 9:
                        if (curOrderState.LabCode.ToLower() == mySession.MySite.SiteCode.ToLower())
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "hideReject", "DoRejectRedirect('redirect');", true);
                        }
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "hideReject", "DoRejectRedirect('none');", true);
                        }
                        this.divRejectBlock.Visible = true;
                        this.hrRejectBlock.Visible = true;
                        this.tbRejectLabReason.Enabled = false;
                        this.tbRejectClinicReply.Enabled = false;
                        this.bReprint771.Enabled = false;
                        break;

                    default:
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "hideReject", "DoRejectRedirect('none');", true);
                        this.divRejectBlock.Visible = false;
                        this.hrRejectBlock.Visible = false;
                        this.bReprint771.Enabled = true;
                        break;
                }

                return true;
            }
        }

        private void BindRedirectLabs()
        {
            try
            {
                this.ddlRedirectLab.DataSource = this.Order.LensType.StartsWith("SV", StringComparison.CurrentCultureIgnoreCase) ?
                    this.RedirectLabList : this.RedirectLabList.Where(x => x.IsMultivision == true);
                this.ddlRedirectLab.DataTextField = "SiteCombinationProfile";
                this.ddlRedirectLab.DataValueField = "SiteCode";
                this.ddlRedirectLab.DataBind();
                this.ddlRedirectLab.Items.Insert(0, new ListItem("-Select-", "X"));
                this.ddlRedirectLab.SelectedIndex = 0;
            }
            catch
            {
                this.ddlRedirectLab.SelectedIndex = -1;
            }
        }

        private String GetJustificationPreference(String justificationReason)
        {
            return this.LabJustificationPreferences.FirstOrDefault(x => x.JustificationReason == justificationReason).Justification ?? String.Empty;
        }

#endregion LAB ORDERS

#endregion ORDERS

#region ADDRESS VERIFY/UPDATE

        protected void bSaveAddress_Click(object sender, EventArgs e)
        {
            try
            {
                var ModifiedBy = string.IsNullOrEmpty(mySession.MyUserID) ? Globals.ModifiedBy : mySession.MyUserID;
#if DEBUG
                using (MethodTracer.Trace(SrtsTraceSource.PersonSource, "OrderManagement_bSaveAddress_Click", ModifiedBy))
#endif
                {
                    var good = p.DoSaveAddress(this.PatientAddress);
                    if (!good)
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "Fail", String.Format("Confirm('{0}', 'statusMessage', true, false);", "An error occurred saving the address."), true);
                        return;
                    }

                    // Add clinic and lab disbursement methods to shipTo ddl.  This is also done in javascript on the close address dialog.
                    this.ddlShipTo.Items.Insert(1, new ListItem("Clinic Ship to Patient", "C2P"));
                    this.ddlShipTo.Items.Insert(2, new ListItem("Lab Ship to Patient", "L2P"));

                    ShowConfirmDialog("Address saved successfully.");
                    LogEvent("Address saved successfully by user {0} at {1}.", new Object[] { ModifiedBy, DateTime.Now });
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "Close", "CloseAddressDialog();", true);
                }
            }
            catch (Exception ex) { ex.TraceErrorException(); }
        }

        //protected void bVerifyAddress_Click(object sender, EventArgs e)
        //{
        //    this.p.FillPatientAddressData();
        //    ScriptManager.RegisterStartupScript(this, this.GetType(), "AddressDialog", "DoAddressDialog();", true);
        //}

        protected void btnValidateAddress_Click(object sender, EventArgs e)
        {
            // store current order number
            hdfOrderNumber.Value = this.Order.OrderNumber;

            // close order dialog
            ScriptManager.RegisterStartupScript(this, this.GetType(), "CloseOrderDialog", "CloseOrderDialog();", true);

            if (Session["CurrentProcess"] != null && Session["CurrentProcess"].ToString() != "New Order")
            {
                HttpContext.Current.Session["CurrentProcess"] = "Order Management";
                HttpContext.Current.Session["CurrentOrderNumber"] = this.Order.OrderNumber;
            }
            else if (Session["CurrentProcess"] != null && Session["CurrentProcess"].ToString() == "New Order")
            {
                HttpContext.Current.Session["CurrentProcess"] = "Do New Order";
            }
            //redirect to patient details page
            var redirectURL = "../SrtsPerson/PersonDetails.aspx?id=" + this.PatientId + "&isP=true";
            this.Redirect(redirectURL, false);  
        }


        /// <summary>
        /// THIS CODE IS DUPLICATED FROM PersonDetails.aspx.cs
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [System.Web.Services.WebMethod]
        public static string GetUSPSAddressbyZip(string data)
        {
            //////////////////////////////////////////////////////////////////////////////////////////////////
            //expected response sample
            //<?xml version="1.0" encoding="UTF-8"?>
            //<CityStateLookupResponse>
            //<ZipCode ID="0">
            //<Zip5>90210</Zip5>
            //<City>BEVERLY HILLS</City>
            //<State>CA</State>
            //</ZipCode>
            //</CityStateLookupResponse>
            ////////////////////////////////////////////////////////////////////////////////////////////////////////
            try
            {
                var ss = HttpContext.Current.Session["SRTSSession"] as SRTSSession;
#if DEBUG
                using (MethodTracer.Trace(SrtsTraceSource.PersonSource, "PersonDetails_GetUSPSAddressbyZip", ss.MyUserID))
#endif
                {
                    // build the xml request string
                    string Zip5 = data.Substring(0, 5);
                    string userID = ConfigurationManager.AppSettings["uspsUsername"];
                    string requestBaseURL = ConfigurationManager.AppSettings["uspsAPICallBase"];
                    string requestStartAPICall = "?API=CityStateLookup&XML=<CityStateLookupRequest";
                    string requestUserID = " USERID=" + '"' + userID + '"' + ">";
                    string requestQueryID = "<ZipCode ID= " + '"' + "0" + '"' + ">";
                    string requestQueryItem = "<Zip5>" + Zip5 + "</Zip5>";
                    string requestEndAPICall = "</ZipCode></CityStateLookupRequest>";
                    string requestSend = String.Format("{0}{1}{2}{3}{4}{5}",
                        requestBaseURL,
                        requestStartAPICall,
                        requestUserID,
                        requestQueryID,
                        requestQueryItem,
                        requestEndAPICall);

                    // send the request
                    ServicePointManager.ServerCertificateValidationCallback += new System.Net.Security.RemoteCertificateValidationCallback((s, ce, ch, ssl) => true);
                    XmlDocument responseDoc = new XmlDocument();
                    responseDoc.Load(requestSend);

                    // parse the xmldocument response to get the address info
                    string result;

                    // check response for error
                    XmlNodeList error = responseDoc.GetElementsByTagName("Error");
                    XmlNodeList errordescription = responseDoc.GetElementsByTagName("Description");

                    // if response is good
                    if (error.Count == 0)
                    {
                        XmlNodeList city = responseDoc.GetElementsByTagName("City");
                        XmlNodeList state = responseDoc.GetElementsByTagName("State");
                        XmlNodeList zip = responseDoc.GetElementsByTagName("Zip5");

                        // return address info
                        JavaScriptSerializer serializer = new JavaScriptSerializer();
                        AddressEntity uspsAddress = new AddressEntity();
                        uspsAddress.City = city[0].InnerText;
                        uspsAddress.State = state[0].InnerText;

                        result = serializer.Serialize(uspsAddress).ToString();
                    }
                    else
                    {
                        // 0 means error
                        result = "error:  " + errordescription[0].InnerText;
                    }
                    return result;
                }
            }
            catch (Exception ex) { ex.TraceErrorException(); return String.Empty; }
        }

#endregion ADDRESS VERIFY/UPDATE

#region Interface Members

       // private String _PatientEmailAddress;

        public String PatientEmailAddress
        {
            get
            {
                //this._PatientEmailAddress = Session["PatientEmailAddress"] as EMailAddressEntity;
                //if (this._PatientEmailAddress == null) this._PatientEmailAddress = new EMailAddressEntity();
                return this.tbEmailAddress.Text;
            }
            set
            {
                if(value.IsNull()) return;
                //Session.Add("PatientEmailAddress", value);
                this.tbEmailAddress.Text = value;
            }
        }


        private Exam _Exam;

        public Exam Exam
        {
            get
            {
                this._Exam = Session["Exam"] as Exam;
                if (this._Exam == null) this._Exam = new Exam();

                this._Exam.ExamDate = String.IsNullOrEmpty(this.tbExamDate.Text) ? DateTime.MinValue : Convert.ToDateTime(this.tbExamDate.Text);
                this._Exam.ExamComments = this.tbComments.Text;
                this._Exam.OdCorrected = this.ddlOdCorrected.SelectedValue;
                this._Exam.OdUncorrected = this.ddlOdUncorrected.SelectedValue;
                this._Exam.OsCorrected = this.ddlOsCorrected.SelectedValue;
                this._Exam.OsUncorrected = this.ddlOsUnCorrected.SelectedValue;
                this._Exam.OdOsCorrected = this.ddlOdOsCorrected.SelectedValue;
                this._Exam.OdOsUncorrected = this.ddlOdOsUnCorrected.SelectedValue;
                this._Exam.DoctorId = this.ddlExamProviders.SelectedValue.Equals("X") ? 0 : Convert.ToInt32(this.ddlExamProviders.SelectedValue);
                this._Exam.PatientId = this.PatientId;

                return this._Exam;
            }
            set
            {
                Session.Add("Exam", value);
                this.ddlOdCorrected.SelectedValue = value.OdCorrected;
                this.ddlOdUncorrected.SelectedValue = value.OdUncorrected;
                this.ddlOsCorrected.SelectedValue = value.OsCorrected;
                this.ddlOsUnCorrected.SelectedValue = value.OsUncorrected;
                this.ddlOdOsCorrected.SelectedValue = value.OdOsCorrected;
                this.ddlOdOsUnCorrected.SelectedValue = value.OdOsUncorrected;
                this.tbComments.Text = value.ExamComments;
                this.tbExamDate.Text = value.ExamDate.ToShortDateString();
                this.ddlExamProviders.SelectedValue = this.ProviderList.Any(x => x.ID == value.DoctorId) ? value.DoctorId.ToString() : "X";
            }
        }

        public List<Exam> ExamList
        {
            get
            {
                return Session["ExamList"] as List<Exam>;
            }
            set
            {
                Session["ExamList"] = value;
                BindExamGrid();
            }
        }

        private Prescription _Prescription;

        public Prescription Prescription
        {
            get
            {
                this._Prescription = Session["Prescription"] as Prescription;
                if (this._Prescription == null) this._Prescription = new Prescription();

                PrescriptionDocument pd = new PrescriptionDocument();
                pd = this.PrescriptionDocument;
                if (pd != null) this.PrescriptionDocument.PrescriptionId = this._Prescription.PrescriptionId;
                this._Prescription.PrescriptionDocument = this.PrescriptionDocument;
                this.PrescriptionDocument = null;

                this._Prescription.PrescriptionDate = String.IsNullOrEmpty(this.tbPrescriptionDate.Text) ? DateTime.MinValue : Convert.ToDateTime(this.tbPrescriptionDate.Text);
                this._Prescription.PrescriptionName = this.ddlPrescriptionName.SelectedValue;
                this._Prescription.IsMonoCalculation = this.cbMonoOrComboPd.Checked;
                this._Prescription.OdAdd = String.IsNullOrEmpty(this.tbOdAdd.Text) ? default(Decimal) : Convert.ToDecimal(this.tbOdAdd.Text);
                this._Prescription.OsAdd = String.IsNullOrEmpty(this.tbOsAdd.Text) ? default(Decimal) : Convert.ToDecimal(this.tbOsAdd.Text);
                this._Prescription.OdAxis = String.IsNullOrEmpty(this.tbOdAxis.Text) ? default(Int32) : Convert.ToInt32(this.tbOdAxis.Text);
                this._Prescription.OsAxis = String.IsNullOrEmpty(this.tbOsAxis.Text) ? default(Int32) : Convert.ToInt32(this.tbOsAxis.Text);
                this._Prescription.OdCylinder = String.IsNullOrEmpty(this.tbOdCylinder.Text) ? default(Decimal) : Convert.ToDecimal(this.tbOdCylinder.Text);
                this._Prescription.OsCylinder = String.IsNullOrEmpty(this.tbOsCylinder.Text) ? default(Decimal) : Convert.ToDecimal(this.tbOsCylinder.Text);
                this._Prescription.OdSphere = String.IsNullOrEmpty(this.tbOdSphere.Text) ? default(Decimal) : Convert.ToDecimal(this.tbOdSphere.Text);
                this._Prescription.OsSphere = String.IsNullOrEmpty(this.tbOsSphere.Text) ? default(Decimal) : Convert.ToDecimal(this.tbOsSphere.Text);
                this._Prescription.OdHBase = this.tbOdHBase.Text;
                this._Prescription.OsHBase = this.tbOsHBase.Text;
                this._Prescription.OdHPrism = String.IsNullOrEmpty(this.tbOdHPrism.Text) ? default(Decimal) : Convert.ToDecimal(this.tbOdHPrism.Text);
                this._Prescription.OsHPrism = String.IsNullOrEmpty(this.tbOsHPrism.Text) ? default(Decimal) : Convert.ToDecimal(this.tbOsHPrism.Text);
                this._Prescription.PdDistTotal = String.IsNullOrEmpty(this.tbOdPdDistCombo.Text) ? default(Decimal) : Convert.ToDecimal(this.tbOdPdDistCombo.Text);
                this._Prescription.PdNearTotal = String.IsNullOrEmpty(this.tbOdPdNearCombo.Text) ? default(Decimal) : Convert.ToDecimal(this.tbOdPdNearCombo.Text);
                this._Prescription.OdPdDist = String.IsNullOrEmpty(this.tbOdPdDistMono.Text) ? default(Decimal) : Convert.ToDecimal(this.tbOdPdDistMono.Text);
                this._Prescription.OdPdNear = String.IsNullOrEmpty(this.tbOdPdNearMono.Text) ? default(Decimal) : Convert.ToDecimal(this.tbOdPdNearMono.Text);
                this._Prescription.OsPdDist = String.IsNullOrEmpty(this.tbOsPdDistMono.Text) ? default(Decimal) : Convert.ToDecimal(this.tbOsPdDistMono.Text);
                this._Prescription.OsPdNear = String.IsNullOrEmpty(this.tbOsPdNearMono.Text) ? default(Decimal) : Convert.ToDecimal(this.tbOsPdNearMono.Text);
                if (this._Prescription.IsMonoCalculation)
                {
                    this._Prescription.PdDistTotal = this._Prescription.OdPdDist + this._Prescription.OsPdDist;
                    this._Prescription.PdNearTotal = this._Prescription.OdPdNear + this._Prescription.OsPdNear;
                }
                else
                {
                    var d = this._Prescription.PdDistTotal / 2;
                    var n = this._Prescription.PdNearTotal / 2;
                    this._Prescription.OdPdDist = d;
                    this._Prescription.OsPdDist = d;
                    this._Prescription.OsPdNear = n;
                    this._Prescription.OdPdNear = n;
                }
                this._Prescription.OdVBase = this.tbOdVBase.Text;
                this._Prescription.OsVBase = this.tbOsVBase.Text;
                this._Prescription.OdVPrism = String.IsNullOrEmpty(this.tbOdVPrism.Text) ? default(Decimal) : Convert.ToDecimal(this.tbOdVPrism.Text);
                this._Prescription.OsVPrism = String.IsNullOrEmpty(this.tbOsVPrism.Text) ? default(Decimal) : Convert.ToDecimal(this.tbOsVPrism.Text);
                this._Prescription.ProviderId = String.IsNullOrEmpty(this.ddlPrescriptionProvider.SelectedValue) || this.ddlPrescriptionProvider.SelectedValue.Equals("X") ? 0 :
                    Convert.ToInt32(this.ddlPrescriptionProvider.SelectedValue);
                this._Prescription.PatientId = this.PatientId;

                this._Prescription.OdSphereCalc = this.hfODSphereCalc.Value;
                this._Prescription.OdCylinderCalc = this.hfODCylinderCalc.Value;
                this._Prescription.OdAxisCalc = this.hfODAxisCalc.Value;
                this._Prescription.OsSphereCalc = this.hfOSSphereCalc.Value;
                this._Prescription.OsCylinderCalc = this.hfOSCylinderCalc.Value;
                this._Prescription.OsAxisCalc = this.hfOSAxisCalc.Value;

                this._Prescription.ExtraRxTypes = this.divExtraRxs.Visible ? this.cblExtraRxTypes.Items.Cast<ListItem>().Where(x => x.Selected).Select(y => y.Value).ToList() : new List<String>();

                return this._Prescription;
            }
            set
            {
                Session.Add("Prescription", value);

                this.tbPrescriptionDate.Text = value.PrescriptionDate.ToShortDateString();
                this.ddlPrescriptionName.SelectedValue = String.IsNullOrEmpty(value.PrescriptionName) ? "X" : value.PrescriptionName;
                this.cbMonoOrComboPd.Checked = value.IsMonoCalculation;
                this.tbOdAdd.Text = value.OdAdd.ToString();
                this.tbOsAdd.Text = value.OsAdd.ToString();
                this.tbOdAxis.Text = value.OdAxis.ToString().PadLeft(3, '0');
                this.tbOsAxis.Text = value.OsAxis.ToString().PadLeft(3, '0');
                this.tbOdCylinder.Text = value.OdCylinder.ToString().StartsWith("-") || value.OdCylinder.ToString().Equals("0.00") ? value.OdCylinder.ToString() : String.Format("+{0}", value.OdCylinder);
                this.tbOsCylinder.Text = value.OsCylinder.ToString().StartsWith("-") || value.OsCylinder.ToString().Equals("0.00") ? value.OsCylinder.ToString() : String.Format("+{0}", value.OsCylinder);
                this.tbOdHBase.Text = value.OdHBase;
                this.tbOsHBase.Text = value.OsHBase;
                this.tbOdHPrism.Text = value.OdHPrism.Equals(default(decimal)) ? String.Empty : value.OdHPrism.ToString();
                this.tbOsHPrism.Text = value.OsHPrism.Equals(default(decimal)) ? String.Empty : value.OsHPrism.ToString();

                this.tbOdPdDistCombo.Text = value.PdDistTotal.ToString();
                this.tbOdPdNearCombo.Text = value.PdNearTotal.ToString();
                this.tbOdPdDistMono.Text = value.OdPdDist.ToString();
                this.tbOsPdDistMono.Text = value.OsPdDist.ToString();
                this.tbOdPdNearMono.Text = value.OdPdNear.ToString();
                this.tbOsPdNearMono.Text = value.OsPdNear.ToString();

                this.tbOdSphere.Text = value.OdSphere.ToString().StartsWith("-") || value.OdSphere.ToString().Equals("0.00") ? value.OdSphere.ToString() : String.Format("+{0}", value.OdSphere);
                this.tbOsSphere.Text = value.OsSphere.ToString().StartsWith("-") || value.OsSphere.ToString().Equals("0.00") ? value.OsSphere.ToString() : String.Format("+{0}", value.OsSphere);
                this.tbOdVBase.Text = value.OdVBase;
                this.tbOsVBase.Text = value.OsVBase;
                this.tbOdVPrism.Text = value.OdVPrism.Equals(default(decimal)) ? String.Empty : value.OdVPrism.ToString();
                this.tbOsVPrism.Text = value.OsVPrism.Equals(default(decimal)) ? String.Empty : value.OsVPrism.ToString();

                this.hfODSphereCalc.Value = value.OdSphereCalc;
                this.hfODCylinderCalc.Value = value.OdCylinderCalc;
                this.hfODAxisCalc.Value = value.OdAxisCalc;
                this.hfOSSphereCalc.Value = value.OsSphereCalc;
                this.hfOSCylinderCalc.Value = value.OsCylinderCalc;
                this.hfOSAxisCalc.Value = value.OsAxisCalc;

                if (this.ProviderList != null)
                {
                    try
                    {
                        this.ddlPrescriptionProvider.SelectedValue = this.ProviderList.Any(x => x.ID == value.ProviderId) ? value.ProviderId.ToString() : "X";
                    }
                    catch { this.ddlPrescriptionProvider.SelectedIndex = this.ddlPrescriptionProvider.Items.Count > 0 ? 0 : -1; }
                }
                this.cbDeletePrescription.Visible = !value.IsUsed;

                if (this.tbOdAdd.Text.Equals("0.00") && this.tbOsAdd.Text.Equals("0.00"))
                    this.divExtraRxs.Style.Add("display", "none");
                else
                    this.divExtraRxs.Style.Remove("display");
            }
        }

        private PrescriptionDocument _PrescriptionDocument;
        public PrescriptionDocument PrescriptionDocument
        {
            get
            {
                this._PrescriptionDocument = Session["PrescriptionDocument"] as PrescriptionDocument;

                // if there is no document in session and if the file upload has a file name in it...then see if we are adding a new document.               
                if (this._PrescriptionDocument == null)
                {
                    this._PrescriptionDocument = new PrescriptionDocument();


                    if (Session["fileupload"] != null)
                    {
                        FileUpload fileupload = new FileUpload();
                        fileupload = Session["fileupload"] as FileUpload;
                        if (fileupload.HasFile)
                        {
                            this._PrescriptionDocument.DocumentName = Path.GetFileName(fileupload.PostedFile.FileName);
                            this._PrescriptionDocument.DocumentType = fileupload.PostedFile.ContentType;
                            this._PrescriptionDocument.DocumentLength = fileupload.PostedFile.ContentLength;
                            this._PrescriptionDocument.IndividualId = this.PatientId;

                            byte[] fileBytes = fileupload.FileBytes;
                            this._PrescriptionDocument.DocumentImage = fileBytes;
                        }
                        Session["fileupload"] = null;
                    }
                }
                this.PrescriptionDocument = _PrescriptionDocument;
                return this._PrescriptionDocument;
            }
            set
            {
                Session.Add("PrescriptionDocument", value);
            }
        }


        public NewRxVals CalculatedRxValues
        {
            get
            {
                return Session["CalculatedRxValues"] as NewRxVals;
            }
            set
            {
                Session["CalculatedRxValues"] = value;
            }
        }

        public List<Prescription> PrescriptionList
        {
            get
            {
                return ViewState["PrescriptionList"] as List<Prescription>;
            }
            set
            {
                ViewState["PrescriptionList"] = value;
                BindPrescriptionGrid();
            }
        }



        private Order _Order;

        public Order Order
        {
            get
            {
                this._Order = Session["Order"] as Order;
                if (_Order == null) _Order = new Order();

                _Order.OrderNumber = this.lblOrderNumber.Text;
                _Order.Cases = 1;
                _Order.Comment1 = this.tbComment1.Text.ToHtmlEncodeString();
                _Order.Comment2 = this.tbComment2.Text.ToHtmlEncodeString();
                if (!String.IsNullOrEmpty(_Order.Demographic))
                    _Order.Demographic = String.Format("{0}{1}", _Order.Demographic.Substring(0, 7), this.ddlOrderPriority.SelectedValue);
                _Order.FocJustification = this.tbFocJust.Text.ToHtmlEncodeString();
                _Order.MaterialJustification = this.tbMaterialJust.Text.ToHtmlEncodeString();
                _Order.CoatingJustification = this.tbCoatingJust.Text.ToHtmlEncodeString();
                _Order.OdSegHeight = this.tbOdSegHt.Text.ToHtmlEncodeString();
                _Order.OsSegHeight = this.tbOsSegHt.Text.ToHtmlEncodeString();
                _Order.Pairs = String.IsNullOrEmpty(this.tbPair.Text) ? 1 : Convert.ToInt32(this.tbPair.Text.ToHtmlEncodeString());
                _Order.IsGEyes = false;
                _Order.IsEmailPatient = this.cbEmailPatient.Checked;
                _Order.IsPermanentEmailAddressChange = this.cbPermanentEmailAddressChange.Checked;
                _Order.OrderEmailAddress = this.tbOrderEmailAddress.Text.ToHtmlEncodeString();
                _Order.Bridge = this.ddlBridge.SelectedValue;
                _Order.Color = this.ddlColor.SelectedValue;
                _Order.Eye = this.ddlEye.SelectedValue;
                _Order.Frame = this.ddlFrame.SelectedValue;
                _Order.LensType = this.ddlLens.SelectedValue;
                _Order.Material = this.ddlMaterial.SelectedValue;
                _Order.Priority = this.ddlOrderPriority.SelectedValue;
                _Order.LabSiteCode = this.ddlProdLab.SelectedValue.Length.Equals(7) ? this.ddlProdLab.SelectedValue.Substring(1) : this.ddlProdLab.SelectedValue;
                _Order.TechnicianId = this.mySession.MyIndividualID;
                _Order.Temple = this.ddlTemple.SelectedValue;
                _Order.Tint = this.ddlTint.SelectedValue;
                List<string> selectedValues = this.ddlCoating.Items.Cast<ListItem>()
                                                                        .Where(li => li.Selected)
                                                                        .Select(li => li.Value)
                                                                        .ToList();
                _Order.Coatings = string.Join(",", selectedValues);

                //_Order.ShipToPatient = new List<String>() { "C2P", "L2P" }.Contains(this.ddlShipTo.SelectedValue);
                _Order.OrderDisbursement = this.ddlShipTo.SelectedValue;
                _Order.DispenseComments = this.tbDispComment.Text;
                return _Order;
            }
            set
            {
                Session.Add("Order", value);
                this.lblOrderNumber.Text = value.OrderNumber;
                this.lblOrderingTech.Text = value.TechnicianName;
                this.tbComment1.Text = value.Comment1.ToHtmlDecodeString();
                this.tbComment2.Text = value.Comment2.ToHtmlDecodeString();
                this.tbFocJust.Text = value.FocJustification.ToHtmlDecodeString();
                this.tbMaterialJust.Text = value.MaterialJustification.ToHtmlDecodeString();
                this.tbCoatingJust.Text = value.CoatingJustification.ToHtmlDecodeString();
                this.tbOdSegHt.Text = value.OdSegHeight.ToHtmlDecodeString();
                this.tbOsSegHt.Text = value.OsSegHeight.ToHtmlDecodeString();
                this.tbPair.Text = value.Pairs.ToHtmlDecodeString();
                this.tbPair.Enabled = String.IsNullOrEmpty(value.LinkedId);
                this.cbEmailPatient.Checked = value.IsEmailPatient;
                this.cbPermanentEmailAddressChange.Checked = value.IsPermanentEmailAddressChange;
                this.tbOrderEmailAddress.Text = value.OrderEmailAddress.ToHtmlDecodeString();

                try
                {
                    this.ddlBridge.SelectedValue = value.Bridge;
                }
                catch { this.ddlBridge.SelectedIndex = this.ddlBridge.Items.Count > 0 ? 0 : -1; }

                try
                {
                    this.ddlColor.SelectedValue = value.Color;
                }
                catch { this.ddlColor.SelectedIndex = this.ddlColor.Items.Count > 0 ? 0 : -1; }

                try
                {
                    this.ddlEye.SelectedValue = value.Eye;
                }
                catch { this.ddlEye.SelectedIndex = this.ddlEye.Items.Count > 0 ? 0 : -1; }

                try
                {
                    this.ddlFrame.SelectedValue = value.Frame;
                }
                catch { this.ddlFrame.SelectedIndex = this.ddlFrame.Items.Count > 0 ? 0 : -1; }

                try
                {
                    this.ddlLens.SelectedValue = value.LensType;
                }
                catch { this.ddlLens.SelectedIndex = this.ddlLens.Items.Count > 0 ? 0 : -1; }

                try
                {
                    this.ddlMaterial.SelectedValue = value.Material;
                }
                catch { this.ddlMaterial.SelectedIndex = this.ddlMaterial.Items.Count > 0 ? 0 : -1; }

                try
                {
                    this.ddlOrderPriority.SelectedValue = value.Priority;
                }
                catch { this.ddlOrderPriority.SelectedIndex = this.ddlOrderPriority.Items.Count > 0 ? 0 : -1; }

                // See if there is a selected lab
                this.lblCurrentLab.Text = String.IsNullOrEmpty(value.LabSiteCode) || value.LabSiteCode.Equals("X") ? "N/A" : Server.HtmlEncode(value.LabSiteCode);

                try
                {
                    this.ddlProdLab.ClearSelection();
                    if (String.IsNullOrEmpty(value.LabSiteCode) || value.LabSiteCode.Equals("X"))
                    {
                        this.ddlProdLab.SelectedIndex = -1;
                        this.txtProdLab.Text = "";
                    }
                    else
                    {
                        var l = this.ddlProdLab.Items.Cast<ListItem>().Where(x => x.Value.Contains(value.LabSiteCode)).FirstOrDefault();
                        if (l == null)
                        {
                            if (value.LabSiteCode != null)
                            {
                                this.txtProdLab.Text = value.LabSiteCode;
                                ListItem lsc = new ListItem(value.LabSiteCode);
                                this.ddlProdLab.Items.Add(lsc);
                                lsc.Selected = true;
                            }
                            else
                            {
                                this.ddlProdLab.SelectedIndex = 0;
                                this.txtProdLab.Text = "";
                            }
                        }
                        else
                        {
                            this.ddlProdLab.SelectedIndex = this.ddlProdLab.Items.IndexOf(l);
                            this.txtProdLab.Text = ddlProdLab.SelectedItem.Text;
                        }
                    }
                }
                catch
                {
                    this.ddlProdLab.SelectedIndex = this.ddlProdLab.Items.Count > 0 ? 0 : -1;
                    this.txtProdLab.Text = "";
                }

                try
                {
                    this.ddlTemple.SelectedValue = value.Temple;
                }
                catch { this.ddlTemple.SelectedIndex = this.ddlTemple.Items.Count > 0 ? 0 : -1; }

                try
                {
                    this.ddlTint.SelectedValue = value.Tint;
                }
                catch { this.ddlTint.SelectedIndex = this.ddlTint.Items.Count > 0 ? 0 : -1; }

                try
                {
                    if (String.IsNullOrEmpty(value.Coatings))
                        this.ddlCoating.ClearSelection();
                    else
                    {
                        string[] coatings = value.Coatings.Split(new[] { "," }, StringSplitOptions.None);
                        foreach (string s in coatings)
                            this.ddlCoating.Items.FindByValue(s).Selected = true; 
                    }
                }
                catch { this.ddlCoating.ClearSelection(); }

                //this.ddlShipTo.SelectedValue = value.ShipToPatient ? "P" : "C";
                this.ddlShipTo.SelectedValue = value.OrderDisbursement;
                Session["CurrentOrderDisbursement"] = value.OrderDisbursement;
                this.tbDispComment.Text = value.DispenseComments;
            }
        }

        public List<Order> OrderList
        {
            get
            {
                return ViewState["OrderList"] as List<Order>;
            }
            set
            {
                ViewState["OrderList"] = value;
            }
        }

        public List<OrderEmail> OrderEmailList
        {
            get
            {
                return Session["OrderEmailList"] as List<OrderEmail>;
            }
            set
            {
                Session["OrderEmailList"] = value;
                BindOrderNotificationsGrid();
            }
        }

        public List<IndividualEntity> ProviderList
        {
            get
            {
                return ViewState["ProviderList"] as List<IndividualEntity>;
            }
            set
            {
                ViewState["ProviderList"] = value;
            }
        }

        public IndividualEntity Patient
        {
            get
            {
                return Session["Patient"] as IndividualEntity;
            }
            set
            {
                Session["Patient"] = value;
                this.lblDemo.Text = String.Format("{0} - ({1} {2})", value.NameFMiL, value.BOSDescription, value.Rank);
                this.lblNextEligFoc.Text = value.NextFocDate == null ? DateTime.Now.ToShortDateString() : value.NextFocDate.Value.ToShortDateString();
                //  this.lblLabDemo.Text = String.Format("{0} {1} {2}", value.NameLFMi, value.BOSDescription, value.Rank);
                lnkPatientDetail.Text = String.Format("{0} {1} {2}", value.NameLFMi, value.BOSDescription, value.Rank);
                this.lblLabNextEligFoc.Text = value.NextFocDate == null ? DateTime.Now.ToShortDateString() : value.NextFocDate.Value.ToShortDateString();
            }
        }

        public bool PatientHasAddress
        {
            get
            {
                return Convert.ToBoolean(Session["PatientHasAddress"]);
            }
            set
            {
                Session["PatientHasAddress"] = value;
            }
        }

        public OrderDropDownData OrderDdlData
        {
            get
            {
                return ViewState["OrderDdlData"] as OrderDropDownData;
            }
            set
            {
                ViewState["OrderDdlData"] = value;
            }
        }

        public List<LookupData> LookupDataList
        {
            get
            {
                return ViewState["LookupDataList"] as List<LookupData>;
            }
            set
            {
                ViewState["LookupDataList"] = value;
            }
        }

        public String SelectedPriority
        {
            get 
            { 
                return this.ddlOrderPriority.SelectedValue;
            }
            set 
            { 
                this.ddlOrderPriority.SelectedValue = value;
            }
        }

        public List<OrderStateEntity> OrderStateHistory
        {
            get { return ViewState["OrderStateHistory"] as List<OrderStateEntity>; }
            set
            {
                ViewState["OrderStateHistory"] = value;
                this.gvStatusHistory.DataSource = value;
                this.gvStatusHistory.DataBind();
            }
        }

        public String ClinicJustification
        {
            get
            {
                return tbRejectClinicReply.Text.ToHtmlEncodeString();
            }
            set
            {
                tbRejectClinicReply.Text = value.ToHtmlDecodeString();
            }
        }

        public String LabJustification
        {
            get
            {
                return tbRejectLabReason.Text.ToHtmlEncodeString();
            }
            set
            {
                tbRejectLabReason.Text = value.ToHtmlDecodeString();
            }
        }

        public Int32 PatientId
        {
            get { return Convert.ToInt32(Session["PatientId"]); }
            set { Session["PatientId"] = value; }
        }

        public String Demographic
        {
            get { return Session["Demographic"] == null ? String.Empty : Session["Demographic"].ToString(); }
            set { Session["Demographic"] = value; }
        }

        public String SiteCode
        {
            get { return Session["SiteCode"].ToString(); }
            set { Session["SiteCode"] = value; }
        }

        public string LabAction
        {
            get
            {
                return this.rblRejectRedirect.SelectedValue.ToLower();
            }
            set
            {
                this.rblRejectRedirect.SelectedValue = value.ToLower();
            }
        }

        public string RejectRedirectJustification
        {
            get
            {
                return this.tbLabJust.Text.ToHtmlEncodeString();
            }
            set
            {
                this.tbLabJust.Text = value.ToHtmlDecodeString();
            }
        }

        public string RedirectLab
        {
            get
            {
                return this.ddlRedirectLab.SelectedValue;
            }
            set
            {
                try
                {
                    this.ddlRedirectLab.SelectedValue = value;
                }
                catch
                {
                    this.ddlRedirectLab.SelectedIndex = -1;
                }
            }
        }

        public List<SiteCodeEntity> RedirectLabList
        {
            get
            {
                return Session["RedirectLabList"] as List<SiteCodeEntity>;
            }
            set
            {
                Session["RedirectLabList"] = value;
            }
        }

        public Dictionary<string, string> SpecialLabTintList
        {
            get
            {
                return ViewState["SpecialLabTintList"] as Dictionary<String, String>;
            }
            set
            {
                ViewState["SpecialLabTintList"] = value;
            }
        }

        public FrameItemDefaultEntity DefaultFrameItems
        {
            get;
            set;
        }

        public AddressEntity PatientAddress
        {
            get
            {
                //var a = Session["orderAddress"] as AddressEntity;
                var a = ViewState["PatientAddress"] as AddressEntity;
                if (a.IsNull()) a = new AddressEntity();

                a.Address1 = this.tbPrimaryAddress1.Text;
                a.Address2 = this.tbPrimaryAddress2.Text;
                a.City = this.tbPrimaryCity.Text;
                a.Country = this.ddlPrimaryCountry.SelectedValue;
                a.IndividualID = this.PatientId;
                a.IsActive = true;
                a.State = this.ddlPrimaryState.SelectedValue;
                a.UIC = this.tbPrimaryUIC.Text;
                a.ZipCode = this.tbPrimaryZipCode.Text;

                //Session.Remove("orderAddress");

                return a;
            }
            set
            {
                if (value.IsNull()) return;

                ViewState["PatientAddress"] = value;

                this.tbPrimaryAddress1.Text = value.Address1;
                this.tbPrimaryAddress2.Text = value.Address2;
                this.tbPrimaryCity.Text = value.City;
                //this.ddlPrimaryCountry.SelectedValue = String.IsNullOrEmpty(value.Country) ? "US" : value.Country;
                //this.ddlPrimaryState.SelectedValue = value.State;
                if (this.ddlPrimaryCountry.Items.FindByText(value.Country) != null)
                {
                    this.ddlPrimaryCountry.SelectedValue = value.Country;
                }
                else
                {
                    this.ddlPrimaryCountry.SelectedValue = "US";
                }
                if (this.ddlPrimaryState.Items.FindByText(value.State) != null)
                {
                    this.ddlPrimaryState.SelectedValue = value.State;
                }
                else
                {
                    this.ddlPrimaryState.SelectedValue = "TX";
                }
                this.tbPrimaryUIC.Text = value.UIC;
                this.tbPrimaryZipCode.Text = value.ZipCode;
            }
        }

        public List<LookupTableEntity> Countries
        {
            get
            {
                return ViewState["Countries"] as List<LookupTableEntity>;
            }
            set
            {
                ViewState["Countries"] = value;
                this.ddlPrimaryCountry.DataTextField = "ValueTextCombo";
                this.ddlPrimaryCountry.DataValueField = "Value";
                this.ddlPrimaryCountry.DataSource = value;
                this.ddlPrimaryCountry.DataBind();
                this.ddlPrimaryCountry.Items.Insert(0, new ListItem("-Select-", "X"));
                this.ddlPrimaryCountry.SelectedIndex = 0;
            }
        }

        public List<LookupTableEntity> States
        {
            get
            {
                return ViewState["States"] as List<LookupTableEntity>;
            }
            set
            {
                ViewState["States"] = value;
                this.ddlPrimaryState.DataTextField = "ValueTextCombo";
                this.ddlPrimaryState.DataValueField = "Value";
                this.ddlPrimaryState.DataSource = value;
                this.ddlPrimaryState.DataBind();
                this.ddlPrimaryState.Items.Insert(0, new ListItem("-Select-", "X"));
                this.ddlPrimaryState.SelectedIndex = 0;
            }
        }

        public SitePrefFrameItemEntity FramePreferences
        {
            get
            {
                return this.FramePreferencesList.FirstOrDefault(x => x.Frame == this.Order.Frame) ?? new SitePrefFrameItemEntity();
            }
        }

        public List<SitePrefFrameItemEntity> FramePreferencesList
        {
            get
            {
                return ViewState["FramePreferencesList"] as List<SitePrefFrameItemEntity> ?? new List<SitePrefFrameItemEntity>();
            }
            set
            {
                ViewState["FramePreferencesList"] = value;
            }
        }

        public SitePrefRxEntity SitePrefsRX
        {
            get
            {
                return ViewState["SitePrefsRX"] as SitePrefRxEntity;
            }
            set
            {
                ViewState["SitePrefsRX"] = value;
            }
        }

        public SitePrefOrderEntity OrderPreferences
        {
            get
            {
                return ViewState["OrderPreferences"] as SitePrefOrderEntity ?? new SitePrefOrderEntity();
            }
            set
            {
                ViewState["OrderPreferences"] = value;
            }
        }

        public DateTime HoldForStockDate
        {
            get
            {
                return this.hfStockDate.Value.ToDateTime();
            }
            set
            {
                this.hfStockDate.Value = value.ToString();
                this.ceHfsDate.SelectedDate = value;
                this.tbHfsDate.Text = value.ToString();
            }
        }

        public List<SitePrefLabJustification> LabJustificationPreferences
        {
            get { return ViewState["LabJustificationPreferences"] as List<SitePrefLabJustification>; }
            set
            {
                ViewState["LabJustificationPreferences"] = value;
                var rd = value.FirstOrDefault(x => x.JustificationReason == "redirect");
                var rj = value.FirstOrDefault(x => x.JustificationReason == "reject");
                this.RedirectJustificationPreference = rd.IsNull() ? String.Empty : rd.Justification;
                this.RejectJustificationPreference = rj.IsNull() ? String.Empty : rj.Justification;
                rd = null;
                rj = null;
            }
        }
#endregion Interface Members

#region Properties

        private bool IsSingleVisionRx
        {
            get
            {
                try
                {
                    var p = this.PrescriptionList.FirstOrDefault(x => x.PrescriptionId == this.Prescription.PrescriptionId);
                    if (p == null) return false;
                    return p.OdAdd.Equals(default(Decimal)) && p.OsAdd.Equals(default(Decimal));
                }
                catch { return false; }
            }
        }

        public String Bridge
        {
            get { return Session["EyeSize"].ToString(); }
            set { Session["EyeSize"] = value; }
        }

        public string EyeSize { get; set; }

        public string Temple { get; set; }

        public string RedirectJustificationPreference { set { this.hfRedirectJust.Value = value; } }

        public string RejectJustificationPreference { set { this.hfRejectJust.Value = value; } }

#endregion Properties

        protected async void GetLoadDataAsync()
        {
            var ModifiedBy = string.IsNullOrEmpty(mySession.MyUserID) ? Globals.ModifiedBy : mySession.MyUserID;
#if DEBUG
            using (MethodTracer.Trace(SrtsTraceSource.ClinicOrderSource, "OrderManagement_GetLoadDataAsync", ModifiedBy))
#endif
            {
                if (p.IsNull())
                    p = new OrderManagementPresenter(this);

                var examTask = p.GetAllExamsAsync();
                var prescriptionTask = p.GetAllPrescriptionsAsync();
                var providerTask = p.GetProviderListAsync();
                var orderemailTask = p.GetAllOrderEmailsAsync();

                await Task.WhenAll(examTask, providerTask, prescriptionTask, orderemailTask);

                this.ExamList = examTask.Result;
                this.PrescriptionList = prescriptionTask.Result;
                this.ProviderList = providerTask.Result;
                this.OrderEmailList = orderemailTask.Result;

                BindPrescriptionGrid();
                BindExamGrid();
                BindPrescriptionProviders();
                BindExamProviders();
                BindOrderNotificationsGrid();
            }
        }

        protected void GetLoadData()
        {
            var ModifiedBy = string.IsNullOrEmpty(mySession.MyUserID) ? Globals.ModifiedBy : mySession.MyUserID;
#if DEBUG
            using (MethodTracer.Trace(SrtsTraceSource.ClinicOrderSource, "OrderManagement_GetLoadData", ModifiedBy))
#endif
            {
                if (p.IsNull())
                    p = new OrderManagementPresenter(this);

                p.GetAllExams();
                p.GetAllPrescriptions();
                BindPrescriptionProviders();

                p.GetProviderList();
            }
        }

        private void ShowConfirmDialog(String msg)
        {
            /// Show global confirm dialog
            ScriptManager.RegisterStartupScript(this, GetType(), "DisplayDialogMessage", "displaySrtsMessage('Success!','" + msg + "', 'success');", true);
        }

        //protected void gvLabOrders_RowDataBound(object sender, GridViewRowEventArgs e)
        //{
        //    if (e.Row.RowType != DataControlRowType.DataRow) return;

        //    //e.Row.Attributes.Add("onmouseover", "this.style.backgroundColor='#EFD3A5'; this.style.cursor='pointer';");
        //    //e.Row.Attributes.Add("onmouseout", "this.style.backgroundColor=''; this.style.textDecoration='none';");
        //    //e.Row.ToolTip = "Click to select row";


        //    Order drv = (Order)e.Row.DataItem;
        //    Prescription pre = new Prescription();
        //    //int rxScanId = Convert.ToInt32(drv.PrescriptionScanId);
        //    int rxId = Convert.ToInt32(drv.PrescriptionId);
        //    pre = this.PrescriptionList.Where(x => x.PrescriptionId == drv.PrescriptionId).FirstOrDefault();

        //    int rxScanId = pre.PrescriptionScanId;
        //    string arg = rxScanId + "," + rxId;

        //    for (var i = 1; i < e.Row.Cells.Count; i++)
        //    {
        //        //if (i > 1 && i != 6)
        //        //{
        //        //    e.Row.Cells[i].Attributes["onclick"] = this.Page.ClientScript.GetPostBackClientHyperlink(this.gvLabOrders, "Select$" + e.Row.RowIndex, true);
        //        //}

        //        if (i == 2 && rxScanId != 0)
        //        {
        //            e.Row.Cells[i].Attributes["onclick"] = this.Page.ClientScript.GetPostBackClientHyperlink(this.gvLabOrders, "ViewDocument$" + arg, true);
        //        }
        //        if (i == 2 && rxScanId == 0)
        //        {
        //            e.Row.Cells[i].Attributes.Remove("onclick");
        //            e.Row.Cells[i].Attributes.Remove("onmouseover");
        //            e.Row.Cells[i].Attributes.Add("onmouseover", "this.style.cursor='none';");
        //            e.Row.Cells[i].ToolTip = "";
        //        }

        //    }

        //}


        protected void btnViewDoc_Command(object sender, CommandEventArgs e)
        {
            int PrescriptionScanId = Convert.ToInt32(this.Prescription.PrescriptionScanId);
            int PrescriptionId = Convert.ToInt32(this.Prescription.PrescriptionId);
            string commandArgument = PrescriptionScanId +","+ PrescriptionId;
            Session["Argument"] = commandArgument;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "DisplayPrescriptionDocumentDialog", "DisplayPrescriptionDocumentDialog();", true);
        }


        protected void btnDelete_Click(object sender, EventArgs e)
        {
            if (p.IsNull())
                p = new OrderManagementPresenter(this);
            int PrescriptionScanId = Convert.ToInt32(this.Prescription.PrescriptionScanId);
            var scanDelete = p.DeletePrescriptionScan(PrescriptionScanId);
            GetLoadData();
            ShowConfirmDialog("The attached scanned document was successfully deleted.");


        }

        //protected void ddlProdLab_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    CheckLabMailToPatientStatus();
        //}


        //protected void CheckLabMailToPatientStatus()
        //{
        //    if (!String.IsNullOrEmpty(hdfProdLabs.Value))
        //    {
        //        string noshiplabs = hdfProdLabs.Value;
        //        string selectedlab = ddlProdLab.SelectedItem.ToString().Substring(5);

        //        var isselected = noshiplabs.Contains(selectedlab);
        //        if (isselected)
        //        {
        //            this.ddlShipTo.Items.Clear();
        //            this.ddlShipTo.Items.Insert(0, new ListItem("Clinic Distribution", "CD"));
        //            this.ddlShipTo.Items.Insert(1, new ListItem("Clinic Ship to Patient", "C2P"));
        //            this.ddlShipTo.SelectedValue = "CD";
        //        }
        //        else
        //        {
        //            this.ddlShipTo.Items.Clear();
        //            this.ddlShipTo.Items.Insert(0, new ListItem("Clinic Distribution", "CD"));
        //            this.ddlShipTo.Items.Insert(1, new ListItem("Clinic Ship to Patient", "C2P"));
        //            this.ddlShipTo.Items.Insert(2, new ListItem("Lab Ship to Patient", "L2P"));
        //            this.ddlShipTo.SelectedValue = "CD";
        //        }
        //    }
        //}
    }

    public class SingletonOM
    {
        public static OrderManagement StaticOrderManagement { get; set; }
    }
}