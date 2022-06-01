using SrtsWeb.BusinessLayer.Concrete;
using SrtsWeb.Base;
using SrtsWeb.Entities;
using SrtsWeb.ExtendersHelpers;
using SrtsWeb.Presenters.Dmdc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Web;
using System.Web.Security;
using System.Web.UI.WebControls;

namespace SrtsWeb.Admin
{
    [PrincipalPermission(SecurityAction.Demand, Role = "MgmtEnterprise")]
    public partial class DmdcGetter : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (mySession == null)
                mySession = new SRTSSession();

            if (!HttpContext.Current.User.Identity.IsAuthenticated)
            {
                FormsAuthentication.RedirectToLoginPage();
                return;
            }
#if DEBUG
            using (MethodTracer.Trace(SrtsTraceSource.AdminSource, "DmdcGetter_Page_Load", mySession.MyUserID))
#endif
            {
                if (!IsPostBack)
                {
                    try
                    {
                        mySession.CurrentModule = "DMDC Getter";
                        Master.CurrentModuleTitle = mySession.CurrentModule;
                        ClearDetailLabels();
                    }
                    catch { }
                }
            }
        }

        protected void bGet_Click(object sender, EventArgs e)
        {
#if DEBUG
            using (MethodTracer.Trace(SrtsTraceSource.AdminSource, "DmdcGetter_bGet_Click", mySession.MyUserID))
#endif
            {
                try
                {
                    if (String.IsNullOrEmpty(this.tbIds.Text)) return;

                    var idList = this.tbIds.Text.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();

                    var p = new DmdcPresenter(new DmdcService());
                    var lp = p.Get(idList);
                    this.DmdcPeople = DmdcService.FlattenDmdcPerson(lp);

                    this.gvDmdcResults.DataSource = this.DmdcPeople;
                    this.gvDmdcResults.DataBind();

                    LogEvent(String.Format("User {0} attempted to search for {1} individuals from DMDC at {2}", mySession.MyUserID, idList.Count, DateTime.Now));

                    if (lp.Count().Equals(0)) return;

                    SetDetails(0);
                }
                catch (Exception ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                }
            }
        }

        protected void bClear_Click(object sender, EventArgs e)
        {
#if DEBUG
            using (MethodTracer.Trace(SrtsTraceSource.AdminSource, "DmdcGetter_bClear_Click", mySession.MyUserID))
#endif
            {
                this.tbIds.Text = "";
                ClearDetailLabels();
                this.gvDmdcResults.DataSource = null;
                this.gvDmdcResults.DataBind();
                this.CurrentRowIdx = -1;
                this.tbIds.Focus();
            }
        }

        protected void gvDmdcResults_RowCommand(object sender, GridViewCommandEventArgs e)
        {
#if DEBUG
            using (MethodTracer.Trace(SrtsTraceSource.AdminSource, "DmdcGetter_gvDmdcResults_RowCommand", mySession.MyUserID))
#endif
            {
                var gvR = ((LinkButton)e.CommandSource).NamingContainer as GridViewRow;
                SetDetails(gvR.RowIndex);
            }
        }

        private void ClearDetailLabels()
        {
#if DEBUG
            using (MethodTracer.Trace(SrtsTraceSource.AdminSource, "DmdcGetter_ClearDetailLabels", mySession.MyUserID))
#endif
            {
                // Clear text on labels
                foreach (var c in this.pnlPersonDetail.Controls)
                {
                    if ((c is Label) == false) continue;
                    var l = c as Label;
                    if (!l.ID.StartsWith("lbl")) continue;
                    l.Text = "";
                }
                this.tbIds.Focus();
            }
        }

        private void SetDetails(Int32 rowIdx)
        {
#if DEBUG
            using (MethodTracer.Trace(SrtsTraceSource.AdminSource, "DmdcGetter_SetDetails", mySession.MyUserID))
#endif
            {
                ClearDetailLabels();
                var p = this.DmdcPeople.ToList()[rowIdx];
                this.lblFirstName.Text = p.PnFirstName;
                this.lblMiddleName.Text = p.PnMiddleName;
                this.lblLastName.Text = p.PnLastName;
                this.lblIdNum1.Text = p.PnId1;
                this.lblIdType1.Text = p.PnIdType1;
                this.lblMatchReasonCd1.Text = p.MatchReasonCode1;
                this.lblIdNum2.Text = p.PnId2;
                this.lblIdType2.Text = p.PnIdType2;
                this.lblMatchReasonCd2.Text = p.MatchReasonCode2;
                this.lblEmail.Text = p.Email;
                this.lblPhNumber.Text = p.PhoneNumber;
                this.lblAddress1.Text = p.MailingAddress1;
                this.lblAddress2.Text = p.MailingAddress2;
                this.lblCity.Text = p.MailingCity;
                this.lblState.Text = p.MailingState;
                this.lblZip.Text = p.MailingZip;
                this.lblZipExt.Text = p.MailingZipExtension;
                this.lblCountry.Text = p.MailingCountry;
                this.lblCatCd.Text = p.PnCategoryCode;
                this.lblEntitlementCd.Text = p.PnEntitlementTypeCode;
                this.lblOrgCd.Text = p.OrganizationCode;
                this.lblEndDt.Text = p.PnProjectedEndDate.ToShortDateString();
                this.lblPayPlanCd.Text = p.PayPlanCode;
                this.lblPayGrade.Text = p.PayGrade;
                this.lblRank.Text = p.Rank;
                this.lblServiceCd.Text = p.ServiceCode;
                this.lblUnitId.Text = p.AttachedUnitIdCode;
                this.lblDob.Text = p.PnDateOfBirth.ToShortDateString();
                this.lblDeathDt.Text = p.PnDeathCalendarDate.ToShortDateString();
            }
        }

        public IEnumerable<DmdcPerson_Flat> DmdcPeople
        {
            get { return ViewState["DmdcPeople"] as IEnumerable<DmdcPerson_Flat>; }
            set { ViewState["DmdcPeople"] = value; }
        }

        public int CurrentRowIdx { get; set; }
    }
}