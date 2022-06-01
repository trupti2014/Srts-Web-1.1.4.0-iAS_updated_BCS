using SrtsWeb.Base;
using SrtsWeb.BusinessLayer.Concrete;
using SrtsWeb.Entities;
using SrtsWeb.ExtendersHelpers;
using SrtsWeb.Presenters.Patients;
using SrtsWeb.Views.Patients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Web;
using System.Web.Security;
using System.Web.UI;

namespace SrtsWebClinic.Patients
{
    [PrincipalPermission(SecurityAction.Demand, Role = "ClinicClerk")]
    [PrincipalPermission(SecurityAction.Demand, Role = "ClinicTech")]
    public partial class BMTUpload : PageBase, IBmtView
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!HttpContext.Current.User.Identity.IsAuthenticated)
            {
                FormsAuthentication.RedirectToLoginPage();
            }
#if DEBUG
            using (MethodTracer.Trace(SrtsTraceSource.ClinicManageSource, "BMTUpload_", mySession.MyUserID))
#endif
            {
                if (!IsPostBack)
                {
                    try
                    {
                        this.lblLoadError.Text = String.Empty;
                        BuildPageTitle();
                    }
                    catch (Exception ex) { ex.TraceErrorException(); }
                }
            }
        }
        private void BuildPageTitle()
        {
            try
            {
                Master.CurrentModuleTitle = "Manage Patients - BMT Upload";
                Master.uplCurrentModuleTitle.Update();
            }
            catch (NullReferenceException)
            {
                CurrentModule("Manage Patients - BMTUpload");
                CurrentModule_Sub(string.Empty);
            }
        }
        protected void btnProcess_Click(object sender, EventArgs e)
        {
            var fileLoc = String.Empty;
            try
            {
#if DEBUG
                using (MethodTracer.Trace(SrtsTraceSource.ClinicManageSource, "BMTUpload_btnProcess_Click", mySession.MyUserID))
#endif
                {
                    this.lblLoadError.Text = String.Empty;

                    // Clear the bad grid
                    this.gvBmtOutput.DataSource = null;
                    this.gvBmtOutput.DataBind();

                    if (!this.fUpload.HasFile)
                    {
                        Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("No file to upload"));
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "DoConfirm", "Confirm('There was a problem accessing the file for upload.', 'divBmtMessage', true, true);", true);

                        return;
                    }

                    //SAVE THE FILE TO THE APP_DATA\FILEUPLOADS FOLDER ON THE WEB SERVER
                    fileLoc = String.Format(@"{0}App_Data\FileUploads\{1}", Request.PhysicalApplicationPath, this.fUpload.FileName.Insert(this.fUpload.FileName.LastIndexOf('.'),
                        String.Format("{0}{1}{2}{3}", DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute)));

                    this.fUpload.SaveAs(fileLoc);

                    var ew = new BmtPresenter(this);
                    var traineeList = ew.GetBmtData(fileLoc);

                    var good = default(Int32);
                    var bad = ew.ProcessTrainees(traineeList, out good).ToList();

                    if (good > 0)
                    {
                        // Show global confirm dialog
                        ShowConfirmDialog(String.Format("Successfully added {0} new patient(s).", good));
                        LogEvent(String.Format("User {0} successfully added {1} patients at {2}", mySession.MyUserID, good, DateTime.Now));
                    }

                    if (bad.Count.Equals(0)) return;

                    this.lblLoadError.Text = String.Format("Error(s) were encountered while attempting to load {0} patient(s)", bad.Count);
                    LogEvent(String.Format("User {0} unsuccessfully added {1} patients at {2}", mySession.MyUserID, bad.Count, DateTime.Now));
                    this.gvBmtOutput.DataSource = bad;
                    this.gvBmtOutput.DataBind();
                }
            }
            catch (Exception ex)
            {
                ex.TraceErrorException();
                ScriptManager.RegisterStartupScript(this, this.GetType(), "DoConfirm", "Confirm('There was an error adding new patient(s).', 'divBmtMessage', true, true);", true);
            }
            finally
            {
                if (!String.IsNullOrEmpty(fileLoc))
                    System.IO.File.Delete(fileLoc);
            }
        }

        protected void lbGetFile_Click(object sender, EventArgs e)
        {
            Response.ClearHeaders();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AppendHeader("Content-Disposition", "attachment; filename=BMTTemplate.xlsx");
            Response.TransmitFile(Server.MapPath("~/App_Data/FileUploads/BMT/bmt_file.xlsx"));
            Response.End();
        }

        #region INTERFACE PROPERTIES

        public List<BmtEntity> BasicMilitaryTrainee
        {
            get
            {
                return Session["BasicMilitaryTrainee"] as List<BmtEntity>;
            }
            set
            {
                Session["BasicMilitaryTrainee"] = value;
            }
        }

        #endregion INTERFACE PROPERTIES

        private void ShowConfirmDialog(String msg)
        {
            /// Show global confirm dialog
            ScriptManager.RegisterStartupScript(this, GetType(), "DisplayDialogMessage", "displaySrtsMessage('Success!','" + msg + "', 'success');", true);
        }
    }
}