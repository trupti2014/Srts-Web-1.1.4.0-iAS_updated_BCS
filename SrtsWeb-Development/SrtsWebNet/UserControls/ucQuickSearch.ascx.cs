using SrtsWeb.BusinessLayer.Concrete;
using SrtsWeb.Base;
using SrtsWeb.Entities;
using SrtsWeb.ExtendersHelpers;
using System;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SrtsWeb.UserControls
{
    public partial class ucQuickSearch : UserControlBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.mySession == null)
                this.mySession = new SRTSSession();

            this.tbQuickSearch.Enabled = true;
            this.bQuickSearch.Enabled = true;
            this.bChangeStatus.Enabled = true;
            this.bView.Enabled = true;
        }

        protected void bQuickSearch_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(this.tbQuickSearch.Text.Trim())) return;
            var t = this.tbQuickSearch.Text.Trim();

            switch (t.Length)
            {
                case 9:
                case 10:
                    {
                        var ModifiedBy = string.IsNullOrEmpty(this.mySession.ModifiedBy) ? Globals.ModifiedBy : this.mySession.ModifiedBy;
                        var dto = MasterService.DoQuickSearchPatient(t, ModifiedBy);
                        if (dto == null)
                        {
                            Session.Add("qsId", t);
                            Response.Redirect("~/WebForms/SrtsPerson/AddPerson.aspx", true);
                        }
                        else
                        {
                            DoPatientQuickSearch(dto);
                        }

                        break;
                    }
                case 16:
                    {
                        var st = StatusType.VIEW;
                        var ModifiedBy = string.IsNullOrEmpty(this.mySession.ModifiedBy) ? Globals.ModifiedBy : this.mySession.ModifiedBy;

                        var o = MasterService.DoQuickSearchOrder(t, ModifiedBy, out st);

                        //var o = MasterService.DoQuickSearchOrder(t, this.mySession.ModifiedBy, out st);
                       
                        if (o == null)
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "StatusAlert", String.Format("alert('No results found for order number {0}.');", t), true);
                            this.tbQuickSearch.Text = String.Empty;
                            break;
                        }

                        if (Roles.IsUserInRole("ClinicTech") || Roles.IsUserInRole("ClinicClerk"))
                        {
                            // Get patient information for use in the OrderManagement page
                            DoClinicQuickSearch(o.ClinicSiteCode, st);
                        }
                        else if (Roles.IsUserInRole("LabTech") || Roles.IsUserInRole("LabClerk"))
                            DoLabQuickSearch(o.LabSiteCode, st);

                        break;
                    }
                default:
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "StatusAlert", "alert('Invalid search criteria or no results found.');", true);
                    this.tbQuickSearch.Text = String.Empty;
                    break;
            }
        }

        protected void bView_Click(object sender, EventArgs e)
        {
            var cc = ProcessDtoForOrderManagement();
            if (Roles.IsUserInRole("ClinicTech") || Roles.IsUserInRole("ClinicClerk"))
                DoClinicQuickSearch(cc, StatusType.VIEW);
            else if (Roles.IsUserInRole("LabTech") || Roles.IsUserInRole("LabClerk"))
                DoLabQuickSearch(cc, StatusType.VIEW);
        }

        private void DoPatientQuickSearch(PatientOrderDTO dto)
        {
            Session.Add("PatientOrderDTO", dto);
            this.bDetails.Enabled = true;
            this.bOrders.Enabled = true;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "PatientDialog", "$(function(){DoPatientDialog();});", true);
        }

        protected void bDetails_Click(object sender, EventArgs e)
        {
            var dto = Session["PatientOrderDTO"] as PatientOrderDTO;
            Session.Remove("PatientOrderDTO");
            Response.Redirect(String.Format("~/WebForms/SrtsPerson/PersonDetails.aspx?id={0}&isP=true", dto.IndividualId), true);
        }

        protected void bOrders_Click(object sender, EventArgs e)
        {
            Session.Add("IsDtoRedirect", true);
            Response.Redirect("~/WebForms/SrtsOrderManagement/OrderManagement.aspx", true);
        }

        private void DoClinicQuickSearch(String siteCode, StatusType st)
        {
            ProcessDtoForOrderManagement();
            // If the orders clinic site code and the logged in users site code don't match then just view the order.
            if (!siteCode.Equals(this.mySession.MySite.SiteCode))
                st = StatusType.VIEW;

            switch (st)
            {
                case StatusType.CREATED:
                case StatusType.REJECTED:
                case StatusType.RESUBMITTED:
                case StatusType.VIEW:
                case StatusType.RETURN_TO_STOCK:
                    Session.Add("IsDtoRedirect", true);
                    Response.Redirect("~/WebForms/SrtsOrderManagement/OrderManagement.aspx", true);
                    break;

                case StatusType.LAB_RECEIVED:
                    Session.Remove("PatientOrderDTO");
                    this.bChangeStatus.Text = "Force Check In";
                    this.bChangeStatus.ToolTip = "Forces check in of the order to the clinic.";
                    this.bChangeStatus.CommandArgument = "F";
                    break;

                case StatusType.LAB_DISPENSED:
                    Session.Remove("PatientOrderDTO");
                    this.bChangeStatus.Text = "Check In";
                    this.bChangeStatus.ToolTip = "Checks in the order to the clinic.";
                    this.bChangeStatus.CommandArgument = "C";
                    break;

                case StatusType.RECEIVED:
                case StatusType.FORCEDRECEIVED:
                    Session.Remove("PatientOrderDTO");
                    this.bChangeStatus.Text = "Dispense";
                    this.bChangeStatus.ToolTip = "Dispenses the eyewear to the patient.";
                    this.bChangeStatus.CommandArgument = "D";
                    break;
            }

            ScriptManager.RegisterStartupScript(this, this.GetType(), "StatusDialog", "$(function(){DoStatusDialog();});", true);
        }

        private void DoLabQuickSearch(String siteCode, StatusType st)
        {
            var t = this.tbQuickSearch.Text.Trim();
            ProcessDtoForOrderManagement();
            Session.Add("IsDtoRedirect", true);
            Response.Redirect("~/WebForms/SrtsOrderManagement/OrderManagement.aspx", true);
        }

        private String ProcessDtoForOrderManagement()
        {
            var st = StatusType.VIEW;
            var ModifiedBy = string.IsNullOrEmpty(this.mySession.ModifiedBy) ? Globals.ModifiedBy : this.mySession.ModifiedBy;

            var o = MasterService.DoQuickSearchOrder(this.tbQuickSearch.Text.Trim(), ModifiedBy, out st);
            var dto = MasterService.DoQuickSearchPatient(o.IndividualID_Patient, ModifiedBy);

            //var o = MasterService.DoQuickSearchOrder(this.tbQuickSearch.Text.Trim(), this.mySession.ModifiedBy, out st);
            //var dto = MasterService.DoQuickSearchPatient(o.IndividualID_Patient, this.mySession.ModifiedBy);


            dto.OrderNumber = o.OrderNumber;
            Session.Add("PatientOrderDTO", dto);

            return o.ClinicSiteCode;
        }

        protected void bChangeStatus_Click(object sender, EventArgs e)
        {
            var b = sender as Button;
            var act = b.CommandArgument;
            var ModifiedBy = string.IsNullOrEmpty(this.mySession.ModifiedBy) ? Globals.ModifiedBy : this.mySession.ModifiedBy;


            if (MasterService.DoStatusUpdate(this.tbQuickSearch.Text.Trim(), act, ModifiedBy))
            {
                var st = StatusType.VIEW;
                var o = MasterService.DoQuickSearchOrder(this.tbQuickSearch.Text.Trim(), ModifiedBy, out st);
                var dto = MasterService.DoQuickSearchPatient(o.IndividualID_Patient, ModifiedBy);
                dto.OrderNumber = o.OrderNumber;
                Session.Add("PatientOrderDTO", dto);
                DoClinicQuickSearch(o.ClinicSiteCode, StatusType.VIEW);
            }
            Elmah.ErrorSignal.FromCurrentContext().Raise(
                new Exception(
                    String.Format("Location: MasterPage  |||  Method: bChangeStatus_Click  |||  Message: Error updating order status for order number {0}",
                    this.tbQuickSearch.Text.Trim()
                    )));
        }
    }
}