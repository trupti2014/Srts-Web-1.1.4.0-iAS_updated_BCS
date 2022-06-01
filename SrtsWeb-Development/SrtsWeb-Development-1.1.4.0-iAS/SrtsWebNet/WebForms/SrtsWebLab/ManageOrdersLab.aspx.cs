using SrtsWeb;
using SrtsWeb.Base;
using SrtsWeb.BusinessLayer.Concrete;
using SrtsWeb.Entities;
using SrtsWeb.ExtendersHelpers;
using SrtsWeb.Presenters.Lab;
using SrtsWeb.Views.Lab;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Web;
using System.Web.Script.Services;
using System.Web.Security;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using System.Drawing;
using System.Linq.Expressions;
using System.Data;

namespace SrtsWebLab
{
    [PrincipalPermission(SecurityAction.Demand, Role = "LabTech")]
    [PrincipalPermission(SecurityAction.Demand, Role = "LabClerk")]
    [PrincipalPermission(SecurityAction.Demand, Role = "LabMail")]
    [PrincipalPermission(SecurityAction.Demand, Role = "HumanTech")]
    public partial class ManageOrdersLab : PageBase, IManageOrdersLabView, ISiteMapResolver
    {
        private ManageOrdersLabPresenter _presenter;
        //List<ManageOrderEntity> manageOrderEntity;

        public ManageOrdersLab()
        {
            _presenter = new ManageOrdersLabPresenter(this);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!HttpContext.Current.User.Identity.IsAuthenticated)
            {
                FormsAuthentication.RedirectToLoginPage();
            }

            if (mySession == null)
            {
                mySession = new SRTSSession();
            }
            BuildPageTitle();

            hdfSiteHasLMS.Value = mySession.MySite.HasLMS.ToString();

            Control c = GetPostBackControl(this.Page);
            if (c != null || hdfCurrentModule.Value == "holdforstock")
            {
                switch (c.ID)
                {
                    case "lbHoldStock":
                    case "btnNo":
                        uplHoldforStock.Visible = true;
                        GetHoldforStock("holdforstock");
                        break;
                    case "gvStockonHold":
                        GetHoldforStock("holdforstock");
                        uplHoldforStock.Visible = true;
                        break;
                    default:
                        break;
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(Request.QueryString["id"]))
                {
                    var re = Request.QueryString["id"];
                    switch (re)
                    {
                        case "holdforstock":

                            GetHoldforStock("holdforstock");
                            uplHoldforStock.Visible = true;
                            break;
                    }
                }
            }



            if (gvPending.Rows.Count < 1 && gvStockonHold.Rows.Count < 1)
            {
                divHoldOptions.Visible = false;
            }
            else
            {
                divHoldOptions.Visible = true;
            }

#if DEBUG
            using (MethodTracer.Trace(SrtsTraceSource.LabOrderSource, "ManageOrdersLab_PageBase", mySession.MyUserID))
#endif
            {
                this.ceHfsDate.StartDate = DateTime.Now;
                if (IsPostBack)
                {
                    hiddenDownload.Src = "";
                }
                else
                {
                    try
                    {
                        // DO NOT CHANGE! Critical functions in orderlab.js strip values from this string
                        litContentTop_Title_Right.Text = string.Format("{0} - {1}", mySession.MySite.SiteName, mySession.MyClinicCode);
                        // DO NOT CHANGE! Critical functions in orderlab.js strip values from this string

                        BuildPageTitle();
                        BindRedirectLabs();
                    }
                    catch (NullReferenceException ex)
                    {
                        ex.TraceErrorException();
                        Response.Redirect(FormsAuthentication.LoginUrl);
                    }
                }
                //if (mySession.MySite.HasLMS) //Moved this code to run client side in ManageOrdersLab.js
                //{
                //    this.btnSubmitTop.Visible = false;
                //    this.btnSubmitBottom.Visible = false;
                //}

            }
            Bind_gvOrderPrintPriority();
        }

        protected void Page_LoadComplete(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Request.QueryString["id"]))
            {
                var re = Request.QueryString["id"];
                switch (re)
                {
                    case "holdforstock":
                        GetHoldforStock("holdforstock");
                        uplHoldforStock.Visible = true;
                        break;
                }
            }
            var priorities = hdfCheckInPriorities.Value;
        }
        private void BuildPageTitle()
        {
            try
            {
                Master.CurrentModuleTitle = "Manage Orders - Lab";
                Master.uplCurrentModuleTitle.Update();
            }
            catch (NullReferenceException)
            {
                CurrentModule("Manage Orders - Lab");
                CurrentModule_Sub(string.Empty);
            }
        }
        public SiteMapNode BuildBreadCrumbs(object sender, SiteMapResolveEventArgs e)
        {
            SiteMapNode parent = new SiteMapNode(e.Provider, "1", "~/Default.aspx", "My SRTSWeb");
            SiteMapNode child = new SiteMapNode(e.Provider, "2", "~/SrtsWebLab/ManageOrdersLab.aspx#checkin", "Manage Orders");
            child.ParentNode = parent;
            return child;
        }

        #region Shared Methods

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
#if DEBUG
            using (MethodTracer.Trace(SrtsTraceSource.LabOrderSource, "ManageOrdersLab_btnSubmit_Click", mySession.MyUserID))
#endif
            {
                DoOrderStatusOperations(false);
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/WebForms/Default.aspx/home");
        }

        private void DoOrderStatusOperations(Boolean checkInFromHold)
        {
#if DEBUG
            using (MethodTracer.Trace(SrtsTraceSource.LabManageSource, "ManageOrdersLab_DoOrderStatusOperations", mySession.MyUserID))
#endif
            {
                var ordersInGrid = new List<String>();

                var printLabels = false;

                if (!hfOrdersInGrid.Value.IsNullOrEmpty()) ordersInGrid = hfOrdersInGrid.Value.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();

                var pageModule = -1;
                var fVal = hfPageModule.Value.ToLower();

                switch (hfPageModule.Value.ToLower())
                {
                    case "checkin":
                        pageModule = 0;
                        break;

                    case "dispense":
                        pageModule = 1;
                        break;

                    case "holdforstock":
                        if (checkInFromHold)
                            pageModule = 3;
                        else
                        {
                            pageModule = 2;
                        }
                        break;
                    case "redirectreject":
                        if (this.LabAction.Equals("redirect"))
                            pageModule = 4;
                        else
                            pageModule = 5;
                        break;

                    default:
                        pageModule = -1;
                        break;
                }

                try
                {
                    if ((pageModule != -1) && (ordersInGrid.Count > 0))
                    {
                        printLabels = _presenter.UpdateOrders(ordersInGrid, pageModule, SrtsWeb.Reports.ReportCommon.GetTableTemplateForLabels());

                        if (printLabels) { hiddenDownload.Src = "~/WebForms/PrintForms/LabelRptViewer.aspx"; }
                    }

                    Page.ClientScript.RegisterStartupScript(this.GetType(), "SetPageHashValue", "SetPageHashValue('" + fVal + "');", true);
                }
                catch (Exception ex)
                {
                    ex.TraceErrorException();
                }

                if (!(String.IsNullOrEmpty(this.Message)))
                {
                    ShowMessage();
                    if (pageModule == 4)
                    {
                        if (!(this.Message.Contains("error"))) ShowConfirmDialog(this.Message);
                        else ShowErrorDialog(this.Message);
                    }

                }
                else if (pageModule == 0)
                {
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "DisplayProgress", "DisplayProgress();", true);
                    hiddenDownload.Src = "~/WebForms/Reports/rptViewDD771.aspx";
                }
            }
        }

        private void ShowMessage()
        {
            CustomValidator cv = new CustomValidator();
            cv.IsValid = false;
            cv.ErrorMessage = Message;
            this.Page.Validators.Add(cv);
            return;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string UpdateOrderStatuses(String OrdersInGrid, String CurrentModule, String SelectedLabel)
        {
            var mySession = HttpContext.Current.Session["SRTSSession"] as SRTSSession;
#if DEBUG
            using (MethodTracer.Trace(SrtsTraceSource.LabOrderSource, "ManageOrdersLab_btnSubmit_Click", mySession.MyUserID))
#endif
            {
                var ordersInGrid = new List<String>();
                var printLabels = false;

                if (!OrdersInGrid.IsNullOrEmpty()) ordersInGrid = OrdersInGrid.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();

                int pageModule = -1;
                switch (CurrentModule.ToLower())
                {
                    case "checkin":
                        pageModule = 0;
                        break;

                    case "dispense":
                        pageModule = 1;
                        break;

                    default:
                        pageModule = -1;
                        break;
                }

                var view = new ManageOrdersLab();
                try
                {
                    if ((pageModule != -1) && (ordersInGrid.Count > 0))
                    {
                        var fVal = string.Empty;
                        view.mySession = mySession;
                        view.SelectedLabel = SelectedLabel;
                        var p = new ManageOrdersLabPresenter(view);

                        printLabels = p.UpdateOrders(ordersInGrid, pageModule, SrtsWeb.Reports.ReportCommon.GetTableTemplateForLabels());
                        if (pageModule == 0)
                        {
                            fVal = "checkin";
                        }
                        else
                        {
                            fVal = "dispense";
                        }

                        if (printLabels) return "~/WebForms/PrintForms/LabelRptViewer.aspx";
                    }
                }
                catch (Exception ex)
                {
                    ex.TraceErrorException();
                    return "";
                }

                if (String.IsNullOrEmpty(view.Message) && pageModule == 0)
                    return "~/WebForms/Reports/rptViewDD771.aspx";
                return "";
            }
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static List<object> GetLabOrderData(string pageMode, string siteCode)
        {
            List<object> gridDataList = new List<object>();
            var ss = HttpContext.Current.Session["SRTSSession"] as SRTSSession;
            try
            {
#if DEBUG
                using (MethodTracer.Trace(SrtsTraceSource.LabOrderSource, "ManageOrdersLab_GetLabOrderData", ss.MyUserID))
#endif
                {
                    var moe = ManageOrdersLabPresenter.GetManageLabGridOrders(pageMode, siteCode, HttpContext.Current.User.Identity.Name);

                    foreach (var m in moe)
                    {
                        var hfsDate = "";
                        if (m.OrderStatusTypeID.Equals(5))
                        {
                            //var rx = new Regex(@"(\d{2}|\d{1})[^\w\d\r\n:](\d{2}|\d{1})[^\w\d\r\n:](\d{4}|\d{2})");
                            //hfsDate = rx.Match(m.StatusComment).Value.ToDateTime().ToString();
                            var searchStr = "Hold: Anticipated stock date is ";
                            var ix = m.StatusComment.LastIndexOf(searchStr);
                            hfsDate = ix.GreaterThan(-1) ? m.StatusComment.Substring(searchStr.Length, m.StatusComment.IndexOf(".") - searchStr.Length).ToDateTime().ToString() : String.Empty;

                            var idx = m.StatusComment.IndexOf("Justification:");
                            var ccc = idx.GreaterThan(-1) ? m.StatusComment.Substring(m.StatusComment.IndexOf("Justification:")) : m.StatusComment;
                            m.StatusComment = ccc;
                        }

                        gridDataList.AddRange(new object[]{
                            new {
                                OrderNumber = m.OrderNumber,
                                LastName = m.LastName,
                                FirstName = m.FirstName,
                                MiddleName = m.MiddleName,
                                Priority = m.Priority,
                                FrameCode = m.FrameCode,
                                IndividualId = m.IndividualId,
                                LensType = m.LensType,
                                LensMaterial = m.LensMaterial,
                                OrderDate = m.DateLastModified.ToString(),
                                ClinicCode = m.ClinicSiteCode,
                                CurrentStatusId = m.OrderStatusTypeID,
                                HfsDate = hfsDate,
                                StatusComment = m.StatusComment
                            }
                        });
                    }
                }
            }
            catch (Exception ex) { ex.TraceErrorException(); }

            return gridDataList;
        }

        //// Get Hold for Stock Orders ///////
        public void GetHoldforStock(string pageMode)
        {
            string siteCode = mySession.MyClinicCode;
            var moe = ManageOrdersLabPresenter.GetManageLabGridOrders(pageMode, siteCode, HttpContext.Current.User.Identity.Name);
            if (moe != null)
            {
                var searchStr = "Hold: Anticipated stock date is ";
                foreach (ManageOrderEntity o in moe)
                {
                    var ix = o.StatusComment.LastIndexOf(searchStr);
                    string endDate = ix.GreaterThan(-1) ? o.StatusComment.Substring(searchStr.Length, o.StatusComment.IndexOf(".") - searchStr.Length).ToDateTime().ToShortDateString() : String.Empty;
                    DateTime DateHoldStockEnd = new DateTime();
                    if (DateTime.TryParse(endDate.ToString(), out DateHoldStockEnd))
                    {
                        o.DateHoldStockEnd = DateHoldStockEnd;
                    }
                }
                HoldForStockOrders = moe;
                List<String> LensTypes = new List<String>();
                LensTypes = HoldForStockOrders.Select(l => l.LensMaterial).Distinct().OrderBy(l => l).ToList();
                List<String> FrameTypes = new List<String>();
                FrameTypes = HoldForStockOrders.Select(l => l.FrameCode).Distinct().OrderBy(f => f).ToList();
            }
        }
        //// end Get Hold for Stock Orders ///////


        #endregion Shared Methods


        #region Hold for Stock Methods

        /// <summary>
        /// /Gets the total selected items in the OrdersPending gridview
        /// </summary>
        protected void GetTotalSelectedOnHoldItems(string itemType, string Item)
        {
            int totalOrdersChecked = 0;
            List<string> orderNumbers = new List<string>();
            foreach (GridViewRow row in gvStockonHold.Rows)
            {
                Label Status = (Label)row.FindControl("Status");
                Label OrderNumber = (Label)row.FindControl("OrderNumber");
                CheckBox chkCheckInHold = (CheckBox)row.FindControl("chkCheckInHold");
                Label frame = (Label)row.FindControl("Frame");
                Label lens = (Label)row.FindControl("lens");

                string hfsStatusType = hdfStatusType.Value;

                if (hdfCommandName.Value == "SelectAllCheckinItems")
                {
                    string statusCol = string.Empty;
                    string itemStatus = string.Empty;
                    int indexJustification = Status.Text.IndexOf("Justification: ");
                    if (indexJustification != -1)
                    {
                        itemStatus = Status.Text.Substring(indexJustification + "Justification: ".Length);
                    }

                    if (!String.IsNullOrEmpty(itemType) && !String.IsNullOrEmpty(hfsStatusType) && !String.IsNullOrEmpty(Item))
                    {
                        if (itemStatus.StartsWith(hfsStatusType) && Item.StartsWith("Lens") && lens.Text == itemType)
                        {
                            chkCheckInHold.Checked = true;
                            totalOrdersChecked += 1;
                            orderNumbers.Add(OrderNumber.Text);
                        }
                        else if (itemStatus.StartsWith(hfsStatusType) && Item.StartsWith("Frame") && frame.Text == itemType)
                        {
                            chkCheckInHold.Checked = true;
                            totalOrdersChecked += 1;
                            orderNumbers.Add(OrderNumber.Text);

                        }

                    }
                }
            }
            if (hdfOrdersSelected.Value == "")
            {
                hdfOrdersSelected.Value = String.Join(",", orderNumbers);
                hdftotalOrdersChecked.Value = totalOrdersChecked.ToString();

            }
            hdftotalOrdersChecked.Value = totalOrdersChecked.ToString();
            hfOrdersInGrid.Value = String.Join(",", orderNumbers);
        }

        /// </summary>
        protected void GetTotalSelectedPendingItems()
        {
            int totalLensItemsChecked = 0;
            int totalFrameItemsChecked = 0;
            List<string> orderNumbers = new List<string>();
            foreach (GridViewRow row in gvPending.Rows)
            {
                CheckBox chkHoldLens = (CheckBox)row.FindControl("chkHoldLens");
                CheckBox chkHoldFrames = (CheckBox)row.FindControl("chkHoldFrame");
                Label OrderNumber = (Label)row.FindControl("OrderNumber");

                if (chkHoldLens != null && chkHoldLens.Checked)
                {
                    totalLensItemsChecked += 1;
                    orderNumbers.Add(OrderNumber.Text);
                }
                if (chkHoldFrames != null && chkHoldFrames.Checked)
                {
                    totalFrameItemsChecked += 1;
                    orderNumbers.Add(OrderNumber.Text);
                }
            }
            if (hdfOrdersSelected.Value == "")
            {
                hdfOrdersSelected.Value = String.Join(",", orderNumbers);
            }
            hdftotalFrameItemsChecked.Value = totalFrameItemsChecked.ToString();
            hdftotalLensItemsChecked.Value = totalLensItemsChecked.ToString();

            hfOrdersInGrid.Value = hdfOrdersSelected.Value;
        }

        protected void BindGridviews(object sender, EventArgs e)
        {
            Button button = sender as Button;
            if (button.Text == "Cancel")
            {
                ClearCheckItems();
                ClearAllVariables();
                return;
            }
            else if (hdfCommandName.Value == "SelectAllFrameItemType" || hdfCommandName.Value == "SelectAllLensItemType" || hdfCommandName.Value == "SelectAllCheckinItems" && button.Text == "No")
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "ShowHoldforStock", "PerformFunction('SetPageSubmenu','holdforstock', '');", true);
            }
            else
            {
                string commandName = "";
                if (String.IsNullOrEmpty(hdfCommandName.Value))
                {
                    commandName = button.CommandName.ToString();
                }
                else
                {
                    commandName = hdfCommandName.Value.ToString();
                }
                switch (commandName)
                {
                    case "SelectAllCheckinItems":
                        Bind_gvStockonHold(sender, e);
                        break;
                    case "SelectAllFrameItemType":
                    case "SelectAllLensItemType":
                    case "SelectAllItemsOfType":
                        Bind_gvPending(sender, e);
                        break;
                    case "SubmitHoldItem":
                    case "SubmitHoldItems":
                        processHoldforStock();
                        break;
                    case "SubmitReleaseHoldItems":
                    case "SubmitReleaseHoldItem":
                        hdfCommandName.Value = "ReleaseFromHold";
                        Bind_gvStockonHold(sender, e);

                        break;
                    default:
                        Bind_gvPending(sender, e);
                        Bind_gvStockonHold(sender, e);
                        break;
                }
            }
        }

        protected void Bind_gvOrderPrintPriority()
        {
            DataTable dt = new DataTable();
            dt.Clear();
            dt.Columns.Add("PriorityId");
            dt.Columns.Add("Priority");
            dt.Columns.Add("Count");
            dt.Columns.Add("Date");

            var p = LookupCache.Where(x => x.Code.ToLower() == "orderprioritytype").ToList();

            if (p != null)
            {
                foreach (LookupTableEntity entity in p)
                {
                    if (entity.Value != "N")
                    {
                        //dt.Rows.Add(new object[] { entity.Value, Helpers.ToTitleCase(entity.Text), 0, DateTime.Now.ToString("MM/dd/yyyy") });
                        dt.Rows.Add(new object[] { entity.Value, Helpers.ToTitleCase(entity.Text), 0, new DateTime().ToString("MM/dd/yyyy") });
                    }
                }
            }
            dt.DefaultView.Sort = "Priority asc";
            gvOrderPrintPriority.DataSource = dt;
            gvOrderPrintPriority.DataBind();
        }


        protected void gvOrderPrintPriority_RowCreated(object sender, GridViewRowEventArgs e)
        {
            //cast the sender back to a gridview
            GridView gv = sender as GridView;

            //check if the row is the header row
            if (e.Row.RowType == DataControlRowType.Header)
            {
                //create a new row
                GridViewRow extraHeader = new GridViewRow(0, 0, DataControlRowType.Header, DataControlRowState.Insert);
                extraHeader.BackColor = Color.CadetBlue;

                TableCell HeaderCell = new TableCell();
                HeaderCell.ID = "hdrPrintPriority";
                HeaderCell.Text = "Check-In by Priority";
                HeaderCell.ColumnSpan = gv.Columns.Count;
                HeaderCell.CssClass = "grdHeader";
                HeaderCell.Style.Add("color", "green");
                HeaderCell.Style.Add("font-size", "15px");
                extraHeader.Cells.Add(HeaderCell);

                gv.Controls[0].Controls.AddAt(0, extraHeader);
            }
        }

        protected void Bind_gvStockonHold(object sender, EventArgs e)
        {
            if (this.HoldForStockOrders != null)
            {
                // get orders on hold
                List<ManageOrderEntity> Status_Hold = new List<ManageOrderEntity>();
                Status_Hold = (from h in HoldForStockOrders
                               where (h.StatusComment.Substring(0, 4).ToString() == "Hold")
                               select h).ToList();
                if (gvStockonHoldSortExpression != null) //Aldela: Added
                {
                    var paramStockOnHold = Expression.Parameter(typeof(ManageOrderEntity), gvStockonHoldSortExpression);
                    var sortExpressionStockOnHold = Expression.Lambda<Func<ManageOrderEntity, object>>(Expression.Convert(Expression.Property(paramStockOnHold, gvStockonHoldSortExpression), typeof(object)), paramStockOnHold);
                    if (gvStockOnHoldSortDirection == System.Web.UI.WebControls.SortDirection.Ascending)
                        Status_Hold = Status_Hold.AsQueryable<ManageOrderEntity>().OrderBy(sortExpressionStockOnHold).ToList();
                    else
                        Status_Hold = Status_Hold.AsQueryable<ManageOrderEntity>().OrderByDescending(sortExpressionStockOnHold).ToList();
                }
                this.gvStockonHold.DataSource = Status_Hold;
                this.gvStockonHold.DataBind();
            }
            Button button = sender as Button;
            string commandName = "";

            if (!String.IsNullOrEmpty(button.CommandName.ToString()))
            {
                commandName = button.CommandName.ToString();
            }
            else
            {
                commandName = hdfCommandName.Value.ToString();
            }

            switch (commandName)
            {
                case "ReleaseFromHold":
                    ScriptManager.RegisterStartupScript(this, GetType(), "CloseHoldStockDialog", "PerformFunction('CloseHoldStockDialog','', '');", true);
                    processCheckInFromHold();
                    DoOrderStatusOperations(true);
                    ScriptManager.RegisterStartupScript(this, GetType(), "CloseCheckInAllFromHoldDialog", "PerformFunction('toggle','View Orders on Hold','View Orders Pending Processing');", true);
                    break;
                case "SelectAllCheckinItems":
                    ScriptManager.RegisterStartupScript(this, GetType(), "CloseHoldStockInfoDialog", "PerformFunction('toggle','View Orders on Hold','View Orders Pending Processing');", true);
                    break;
                case "noSelectAll":
                    ScriptManager.RegisterStartupScript(this, GetType(), "CloseHoldStockDialog", "PerformFunction('CloseHoldStockDialog','', '');", true);
                    break;
                case "CancelHold":
                    //lblMessage.Text = string.Empty;
                    ClearCheckItems();
                    ClearAllVariables();
                    ScriptManager.RegisterStartupScript(this, GetType(), "CloseHoldStockInfoDialog", "PerformFunction('CloseHoldStockInfoDialog','', '');", true);
                    break;
            }

        }

        protected void ReleaseHoldItems(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "CloseHoldStockDialog", "PerformFunction('CloseHoldStockDialog','ReleaseHoldItems', '');", true);
            processCheckInFromHold();
            // DoOrderStatusOperations(true); //Aldela: Commented this line 2/23/2018
            ScriptManager.RegisterStartupScript(this, GetType(), "CloseCheckInAllFromHoldDialog", "PerformFunction('toggle','View Orders on Hold','View Orders Pending Processing');", true);
        }

        protected void gvStockonHold_RowCreated(object sender, GridViewRowEventArgs e)
        {
            //cast the sender back to a gridview
            GridView gv = sender as GridView;

            //check if the row is the header row
            if (e.Row.RowType == DataControlRowType.Header)
            {
                //create a new row
                GridViewRow extraHeader = new GridViewRow(0, 0, DataControlRowType.Header, DataControlRowState.Insert);
                extraHeader.BackColor = Color.CadetBlue;

                TableCell HeaderCell = new TableCell();
                HeaderCell.ID = "hdrOrdersonHold";
                HeaderCell.Text = "Orders on Hold";
                HeaderCell.ColumnSpan = gv.Columns.Count;
                HeaderCell.CssClass = "grdHeader";
                HeaderCell.Style.Add("color", "green");
                HeaderCell.Style.Add("font-size", "15px");
                extraHeader.Cells.Add(HeaderCell);

                gv.Controls[0].Controls.AddAt(0, extraHeader);

            }
        }

        protected void gvStockonHold_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e == null)
            {
                throw new ArgumentNullException("e");
            }
            string hfsItem = hdfHoldStockItem.Value.ToString();
            string hfsItemType = hdfHoldStockItemType.Value.ToString();
            //cast the sender back to a gridview
            GridView gv = sender as GridView;

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                ManageOrderEntity moeDataRow = (ManageOrderEntity)e.Row.DataItem;
                Label EndDate = (Label)e.Row.FindControl("EndDate");
                Label Status = (Label)e.Row.FindControl("Status");
                //Label OrderNumber = (Label)e.Row.FindControl("OrderNumber");
                //CheckBox chkCheckInHold = (CheckBox)e.Row.FindControl("chkCheckInHold");
                //Label frame = (Label)e.Row.FindControl("Frame");
                //Label lens = (Label)e.Row.FindControl("lens");

                //string hfsStatusType = hdfStatusType.Value;

                //if (hdfCommandName.Value == "SelectAllCheckinItems")
                //{
                //    List<string> orderNumbers = new List<string>();
                //    string statusCol = string.Empty;
                //    string itemStatus = string.Empty;
                //    int indexJustification = Status.Text.IndexOf("Justification: ");
                //    if (indexJustification != -1)
                //    {
                //        itemStatus = Status.Text.Substring(indexJustification + "Justification: ".Length);
                //    }

                //    if (!String.IsNullOrEmpty(hfsItemType) && !String.IsNullOrEmpty(hfsStatusType) && !String.IsNullOrEmpty(hfsItem))
                //    {
                //        if (itemStatus.StartsWith(hfsStatusType) && hfsItem.StartsWith("Lens") && lens.Text == hfsItemType)
                //        {
                //            chkCheckInHold.Checked = true;
                //            TotalOrdersChecked += 1;
                //            orderNumbers.Add(OrderNumber.Text);
                //        }
                //        else if (itemStatus.StartsWith(hfsStatusType) && hfsItem.StartsWith("Frame") && frame.Text == hfsItemType)
                //        {
                //            chkCheckInHold.Checked = true;
                //            TotalOrdersChecked += 1;                     
                //            orderNumbers.Add(OrderNumber.Text);

                //        }

                //    }


                //}
                //hdfOrdersSelected.Value += String.Join(",", orderNumbers);
                //hdftotalOrdersChecked.Value = TotalOrdersChecked.ToString();

                //var searchStr = "Hold: Anticipated stock date is ";
                //var ix = moeDataRow.StatusComment.LastIndexOf(searchStr);
                //var eDate = ix.GreaterThan(-1) ? moeDataRow.StatusComment.Substring(searchStr.Length, moeDataRow.StatusComment.IndexOf(".") - searchStr.Length).ToDateTime().ToShortDateString() : String.Empty;
                //EndDate.Text = ix.GreaterThan(-1) ? moeDataRow.StatusComment.Substring(searchStr.Length, moeDataRow.StatusComment.IndexOf(".") - searchStr.Length).ToDateTime().ToShortDateString() : String.Empty;
                //if (!String.IsNullOrEmpty(eDate))
                //{
                //    moeDataRow.DateHoldStockEnd = Convert.ToDateTime(eDate.ToString());
                //}

                var idx = moeDataRow.StatusComment.IndexOf("Justification:");
                var ccc = idx.GreaterThan(-1) ? moeDataRow.StatusComment.Substring(moeDataRow.StatusComment.IndexOf("Justification:")) : moeDataRow.StatusComment;
                Status.Text = Server.HtmlEncode(ccc);
            }
        }

        protected void gvStockonHold_DataBound(object sender, EventArgs e)
        {
            if (hdfCommandName.Value != "SubmitReleaseHoldItems" && hdfCommandName.Value != "SubmitReleaseHoldItem")
            {
                if (!String.IsNullOrEmpty(hdfHoldStockItemType.Value))
                {
                    string itemtype = hdfHoldStockItemType.Value;
                    string item = hdfHoldStockItem.Value;

                    if (hdfOrdersSelected.Value == "")
                    {
                        GetTotalSelectedOnHoldItems(itemtype, item);
                    }


                    if (hdftotalOrdersChecked.Value != "" && hdftotalOrdersChecked.Value != "0")
                    {
                        DisplayHoldStockDialogMsg(hdftotalOrdersChecked.Value, item, itemtype);

                    }
                }
            }
            ScriptManager.RegisterStartupScript(this, GetType(), "toggle", "PerformFunction('toggle','View Orders on Hold','View Orders Pending Processing');", true);
        }

        protected void Bind_gvPending(object sender, EventArgs e)
        {
            if (this.HoldForStockOrders != null)
            {
                // get orders not on hold
                List<ManageOrderEntity> Status_NonHold = new List<ManageOrderEntity>();
                Status_NonHold = (from h in HoldForStockOrders
                                  where (h.StatusComment.Substring(0, 4).ToString() != "Hold")
                                  select h).ToList();
                if (gvPendingSortExpression != null) //Aldela: Added
                {
                    var paramPending = Expression.Parameter(typeof(ManageOrderEntity), gvPendingSortExpression); //Aldela: Added
                    var sortExpressionPending = Expression.Lambda<Func<ManageOrderEntity, object>>(Expression.Convert(Expression.Property(paramPending, gvPendingSortExpression), typeof(object)), paramPending); //Aldela: Added
                    if (gvPendingSortDirection == System.Web.UI.WebControls.SortDirection.Ascending)
                        Status_NonHold = Status_NonHold.AsQueryable<ManageOrderEntity>().OrderBy(sortExpressionPending).ToList();
                    else
                        Status_NonHold = Status_NonHold.AsQueryable<ManageOrderEntity>().OrderByDescending(sortExpressionPending).ToList();
                }
                this.gvPending.DataSource = Status_NonHold;
                this.gvPending.DataBind();
            }

            Button button = sender as Button;
            string commandName = "";

            if (String.IsNullOrEmpty(hdfCommandName.Value))
            {
                commandName = button.CommandName.ToString();
            }
            else
            {
                commandName = hdfCommandName.Value.ToString();
            }
            switch (commandName)
            {
                case "noSelectAll":
                    ScriptManager.RegisterStartupScript(this, GetType(), "CloseHoldStockDialog", "PerformFunction('CloseHoldStockDialog','', '');", true);
                    break;
                case "CancelHold":
                    lblMessage.Text = string.Empty;
                    hdfCommandName.Value = string.Empty;
                    ClearCheckItems();
                    ClearAllVariables();
                    ScriptManager.RegisterStartupScript(this, GetType(), "CloseHoldStockInfoDialog", "PerformFunction('CloseHoldStockInfoDialog','', '');", true);
                    break;
                default:

                    break;
            }


        }

        protected void gvPending_RowCreated(object sender, GridViewRowEventArgs e)
        {
            //cast the sender back to a gridview
            GridView gv = sender as GridView;

            //check if the row is the header row
            if (e.Row.RowType == DataControlRowType.Header)
            {
                //create a new row
                GridViewRow extraHeader = new GridViewRow(0, 0, DataControlRowType.Header, DataControlRowState.Insert);
                extraHeader.BackColor = Color.CadetBlue;

                TableCell HeaderCell = new TableCell();
                HeaderCell.ID = "hdrOrdersNotonHold";
                HeaderCell.Text = "Orders Pending Processing";
                HeaderCell.ColumnSpan = gvPending.Columns.Count;
                HeaderCell.CssClass = "grdHeader";
                HeaderCell.Style.Add("color", "green");
                HeaderCell.Style.Add("font-size", "15px");
                extraHeader.Cells.Add(HeaderCell);

                //add the new row to the gridview
                gv.Controls[0].Controls.AddAt(0, extraHeader);
            }
        }

        protected void gvPending_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e == null)
            {
                throw new ArgumentNullException("e");
            }

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                ManageOrderEntity moeDataRow = (ManageOrderEntity)e.Row.DataItem;
                CheckBox chkHoldLens = (CheckBox)e.Row.FindControl("chkHoldLens");
                CheckBox chkHoldFrame = (CheckBox)e.Row.FindControl("chkHoldFrame");
                Label frame = (Label)e.Row.FindControl("Frame");
                Label lens = (Label)e.Row.FindControl("lens");

                if (moeDataRow.StatusComment.ToLower() != "clinic order created")
                {
                    string hfsItem = hdfHoldStockItemType.Value;
                    if (!String.IsNullOrEmpty(hfsItem))
                    {
                        if (lens.Text == hfsItem)
                        {
                            chkHoldLens.Checked = true;
                        }
                        if (frame.Text == hfsItem)
                        {
                            chkHoldFrame.Checked = true;
                        }
                    }
                }
                else
                {
                    chkHoldFrame.Enabled = false;
                    chkHoldLens.Enabled = false;
                }

                Label Status = (Label)e.Row.FindControl("Status");

                var idx = moeDataRow.StatusComment.IndexOf("Justification:");
                var ccc = idx.GreaterThan(-1) ? moeDataRow.StatusComment.Substring(moeDataRow.StatusComment.IndexOf("Justification:")) : moeDataRow.StatusComment;
                Status.Text = Server.HtmlEncode(ccc);
            }
        }

        protected void gvPending_DataBound(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(hdfHoldStockItemType.Value))
            {
                if (hdfOrdersSelected.Value == "")
                {
                    GetTotalSelectedPendingItems();
                }
                string itemtype = hdfHoldStockItemType.Value;
                string item = hdfHoldStockItem.Value;


                if (hdfCommandName.Value != "SubmitHoldItems" && hdfCommandName.Value != "SubmitHoldItem")
                {
                    if (item == "Lens Material")
                    {
                        if (hdftotalLensItemsChecked.Value != "")
                        {
                            DisplayHoldStockDialogMsg(hdftotalLensItemsChecked.Value, "Lens", itemtype);
                        }
                    }
                    if (item == "Frame")
                    {
                        if (hdftotalFrameItemsChecked.Value != "")
                        {
                            DisplayHoldStockDialogMsg(hdftotalFrameItemsChecked.Value, "Frame", itemtype);
                        }
                    }
                    if (hdftotalFrameItemsChecked.Value != "" || hdftotalLensItemsChecked.Value != "" && hdfCommandName.Value == "SelectAllItemsOfType")
                    {
                        hdfCommandName.Value = "SubmitHoldItems";
                    }
                }
            }
            ScriptManager.RegisterStartupScript(this, GetType(), "toggle", "PerformFunction('toggle', 'View Orders on Hold', 'View Orders Pending Processing');", true);
        }

        protected void gvPending_Sorting(object sender, GridViewSortEventArgs e)
        {
            // By default, set the sort direction to ascending.
            System.Web.UI.WebControls.SortDirection sortDirection = System.Web.UI.WebControls.SortDirection.Ascending;

            // Retrieve the last column that was sorted.
            string lastsortExpression = gvPendingSortExpression;

            if (lastsortExpression != null)
            {
                // Check if the same column is being sorted.
                // Otherwise, the default value can be returned.
                if (lastsortExpression == e.SortExpression)
                {
                    if (gvPendingSortDirection == System.Web.UI.WebControls.SortDirection.Ascending)
                    {
                        sortDirection = System.Web.UI.WebControls.SortDirection.Descending;
                    }
                }
            }

            // Save new values 
            gvPendingSortDirection = sortDirection;
            gvPendingSortExpression = e.SortExpression;
        }

        protected void gvStockonHold_Sorting(object sender, GridViewSortEventArgs e)
        {
            // By default, set the sort direction to ascending.
            System.Web.UI.WebControls.SortDirection sortDirection = System.Web.UI.WebControls.SortDirection.Ascending;

            // Retrieve the last column that was sorted.
            string lastsortExpression = gvStockonHoldSortExpression;

            if (lastsortExpression != null)
            {
                // Check if the same column is being sorted.
                // Otherwise, the default value can be returned.
                if (lastsortExpression == e.SortExpression)
                {
                    if (gvStockOnHoldSortDirection == System.Web.UI.WebControls.SortDirection.Ascending)
                    {
                        sortDirection = System.Web.UI.WebControls.SortDirection.Descending;
                    }
                }
            }

            // Save new values 
            gvStockOnHoldSortDirection = sortDirection;
            gvStockonHoldSortExpression = e.SortExpression;
            ScriptManager.RegisterStartupScript(this, GetType(), "toggle", "PerformFunction('toggle','View Orders on Hold','View Orders Pending Processing');", true);
        }


        public System.Web.UI.WebControls.SortDirection gvStockOnHoldSortDirection
        {
            get
            {
                return (System.Web.UI.WebControls.SortDirection)ViewState["gvStockOnHoldSortDirection"];
            }
            set { ViewState["gvStockOnHoldSortDirection"] = value; }
        }

        public System.Web.UI.WebControls.SortDirection gvPendingSortDirection
        {
            get
            {
                return (System.Web.UI.WebControls.SortDirection)ViewState["gvPendingSortDirection"];
            }
            set { ViewState["gvPendingSortDirection"] = value; }
        }



        public string gvStockonHoldSortExpression
        {
            get
            {
                return (string)ViewState["gvStockonHoldSortExpression"];
            }
            set
            {
                ViewState["gvStockonHoldSortExpression"] = value;
            }
        }

        public string gvPendingSortExpression
        {

            get
            {
                return (string)ViewState["gvPendingSortExpression"];
            }
            set
            {
                ViewState["gvPendingSortExpression"] = value;
            }
        }



        /// <summary>
        /// Gets all selected items selected to be placed on hold for stock in the 
        /// Hold for Stock Orders Pending gridview.
        /// </summary>
        protected void processHoldforStock()
        {
            hdfHoldForStockReason.Value = hdfHoldStockItem.Value + " - " + hdfHoldStockItemType.Value + " is unavailable.";
            if (hdfOrdersSelected.Value == "")
            {
                GetTotalSelectedPendingItems();
            }
            if (hdfOrdersSelected.Value != "")
            {
                hfOrdersInGrid.Value = String.Join(",", hdfOrdersSelected.Value);
            }
            if (!String.IsNullOrEmpty(hfOrdersInGrid.Value))
            {
                string msg = string.Empty;
                bool result = DoHoldforStockOperations(false);
                if (result)
                {
                    uplHoldforStock.Visible = true;
                    hdfOrdersSelected.Value = "";
                    GetHoldforStock("holdforstock");
                    msg = "The selected orders were successfully placed on hold for stock.";
                    lblMessage.Text = msg;
                    ShowConfirmDialog(msg);
                }
                else
                {
                    msg = "There was a problem processing this request.";
                    ShowErrorDialog(msg);
                    msg = string.Empty;
                }

                ClearCheckItems();
                ClearAllVariables();
            }

        }
        protected void processCheckInFromHold()
        {
            if (hdfOrdersSelected.Value != "")
            {
                hfOrdersInGrid.Value = String.Join(",", hdfOrdersSelected.Value);
            }
            if (!String.IsNullOrEmpty(hfOrdersInGrid.Value))
            {
                string msg = string.Empty;
                bool result = DoHoldforStockOperations(true);
                if (result)
                {
                    uplHoldforStock.Visible = true;
                    GetHoldforStock("holdforstock");
                    hdfOrdersSelected.Value = "";
                    msg = "The selected orders were successfully released from hold.";
                    lblMessage.Text = msg;
                    ShowConfirmDialog(msg);
                }
                else
                {
                    msg = "There was a problem processing this request.";
                    ShowErrorDialog(msg);
                    msg = string.Empty;
                }

                ClearCheckItems();
                ClearAllVariables();
            }

        }
        private void ClearAllVariables()
        {
            hdfCommandName.Value = string.Empty;
            hdfHoldForStockReason.Value = string.Empty;
            hdfHoldStockItem.Value = string.Empty;
            hdfHoldStockItemType.Value = string.Empty;
            hdfStatusType.Value = string.Empty;
            hdfOrdersSelected.Value = string.Empty;
            hdftotalFrameItemsChecked.Value = string.Empty;
            hdftotalLensItemsChecked.Value = string.Empty;
            hdftotalOrdersChecked.Value = string.Empty;
            hdfHoldForStockReason.Value = string.Empty;
            hfStockDate.Value = string.Empty;
            lblMessage.Text = string.Empty;
        }

        /// <summary>
        /// / Places selected items in the Orders Pending Gridview on hold.
        /// </summary>
        /// <param name="checkInFromHold"></param>
        private Boolean DoHoldforStockOperations(Boolean checkInFromHold)
        {
            var isGood = false;
            var pageModule = -1;
            var fVal = hfPageModule.Value.ToLower();
            var ordersInGrid = new List<String>();
            if (!hfOrdersInGrid.Value.IsNullOrEmpty()) ordersInGrid = hfOrdersInGrid.Value.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();

            if (checkInFromHold)
                pageModule = 3;
            else
            {
                pageModule = 2;
            }
            try
            {
                isGood = _presenter.UpdateOrders(ordersInGrid, pageModule, SrtsWeb.Reports.ReportCommon.GetTableTemplateForLabels());

                Page.ClientScript.RegisterStartupScript(this.GetType(), "SetPageHashValue", "SetPageHashValue('" + fVal + "');", true);
                return isGood;
            }
            catch (Exception ex)
            {
                ex.TraceErrorException();
                return isGood;
            }
        }

        protected void ClearCheckItems()
        {
            if (gvPending.Rows.Count > 1)
            {
                foreach (GridViewRow row in gvPending.Rows)
                {
                    CheckBox chkHoldLens = (CheckBox)row.FindControl("chkHoldLens");
                    CheckBox chkHoldFrame = (CheckBox)row.FindControl("chkHoldFrame");
                    Label frame = (Label)row.FindControl("Frame");
                    Label lens = (Label)row.FindControl("Lens");

                    string hfsItemType = hdfHoldStockItemType.Value;
                    if (!String.IsNullOrEmpty(hdfHoldStockItemType.Value))
                    {
                        if (lens.Text == hdfHoldStockItemType.Value)
                        {
                            chkHoldLens.Checked = false;
                        }
                        else if (frame.Text == hdfHoldStockItemType.Value)
                        {
                            chkHoldFrame.Checked = false;
                        }
                    }
                }
            }
            if (gvStockonHold.Rows.Count > 1)
            {
                foreach (GridViewRow row in gvStockonHold.Rows)
                {
                    CheckBox chkCheckInHold = (CheckBox)row.FindControl("chkCheckInHold");
                    chkCheckInHold.Checked = false;
                }
            }

        }

        private void ShowConfirmDialog(String msg)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "DisplayDialogMessage", "displaySrtsMessage('Success!','" + msg + "', 'success');", true);
        }

        //private void ShowErrorDialog(String msg)
        //{
        //    ScriptManager.RegisterStartupScript(this, GetType(), "DisplayDialogMessage", "displaySrtsMessage('Error!','" + msg + "', 'error');", true);
        //}

        protected void DisplayHoldStockDialogMsg(string totalSelected, string selectedType, string selectedTypeItem)
        {
            var msg = string.Empty;
            var msgStart = string.Empty;
            if (!String.IsNullOrEmpty(hdfCommandName.Value))
            {
                if (totalSelected != "1")
                {
                    var strOrders = "A total of " + totalSelected + " orders with a ";
                    msgStart = strOrders;
                }
                else if (totalSelected == "1")
                {
                    var strOrder = "A single(" + totalSelected + ") order with a ";
                    msgStart = strOrder;
                }
                switch (hdfCommandName.Value)
                {
                    case "SelectAllLensItemType":
                    case "SelectAllFrameItemType":
                    case "HoldAllSelectedItems":
                    case "SelectAllItemsOfType":
                        msg = msgStart + selectedType + " of type " + selectedTypeItem + " will be placed on hold for stock.  <br /><br />Please select an estimated stock date and select 'Submit'. <br /><br /><span style='text-align:center;color:red;font-size:smaller'>( If you choose 'Cancel', all selections will be cleared.)</span>";
                        lblMessage.Text = msg;
                        ScriptManager.RegisterStartupScript(this, GetType(), "DisplayHoldStockDialog", "PerformFunction('DisplayHoldStockDialog','', '');", true);
                        break;
                    case "SelectAllCheckinItems":
                        msg = msgStart + selectedType + " of type " + selectedTypeItem + " will be released from hold for stock. <br /><br />Select 'Submit' to continue.<br /><br /><span style='text-align:center;color:red;font-size:smaller'>( If you choose 'Cancel', all selections will be cleared.)</span>";
                        lblMessage.Text = msg;
                        hdfCommandName.Value = "SubmitReleaseHoldItems";
                        ScriptManager.RegisterStartupScript(this, GetType(), "DisplayHoldStockDialog", "PerformFunction('DisplayHoldStockDialog','', '');", true);
                        break;
                }
            }

        }
        #endregion Hold for Stock Methods

        private void BindRedirectLabs()
        {
            try
            {
                this.RedirectOrdersLabList = _presenter.GetLabListForRedirect();
                this.ddlRedirectLab.DataSource = //this.Order.LensType.StartsWith("SV", StringComparison.CurrentCultureIgnoreCase) ?
                    this.RedirectOrdersLabList;// : this.RedirectLabList.Where(x => x.IsMultivision == true);
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

        //protected void ddlRedirectLab_ServerValidate(object source, ServerValidateEventArgs args)
        //{
        //    if (this.ddlRedirectLab.SelectedIndex == 1)
        //        args.IsValid = false;
        //    else
        //        args.IsValid = true;
        //}


        #region Page Members

        public String SelectedOrderNumber { get { return this.hfOrderNumber.Value.ToString(); } }

        public int? TotalOrdersToCheckIn
        {
            get { return 0; }
            set { }
        }

        public int? TotalOrdersOnHold
        {
            get
            {
                return Convert.ToInt32(ViewState["TotalOrdersOnHold"]);
            }
            set
            {
                ViewState.Add("TotalOrdersOnHold", value);
            }
        }

        public List<ManageOrderEntity> CheckInOrderData
        {
            get { return (List<ManageOrderEntity>)ViewState["CheckInOrders"]; }
            set { ViewState.Add("CheckInOrders", value); }
        }

        private string _message;

        public string Message
        {
            get { return _message; }
            set { _message = value; }
        }

        public List<ManageOrderEntity> DispenseOrderData
        {
            get { return (List<ManageOrderEntity>)ViewState["DispenseOrderData"]; }
            set { ViewState.Add("DispenseOrderData", value); }
        }

        private string _orderNumberDispense;

        public string DispenseOrderNumber
        {
            get
            {
                return _orderNumberDispense;
            }
            set
            {
                _orderNumberDispense = value;
            }
        }

        private string _orderNumberCheckIn;

        public string OrderNumberCheckIn
        {
            get
            {
                return _orderNumberCheckIn;
            }
            set
            {
                _orderNumberCheckIn = value;
            }
        }

        public string _clinicCode;

        public string ClinicCode
        {
            get
            {
                return _clinicCode;
            }
            set
            {
                _clinicCode = value;
            }
        }

        private string _labCode;

        public string LabCode
        {
            get
            {
                return _labCode;
            }
            set
            {
                _labCode = value;
            }
        }

        private DateTime? _dateReceivedByLab;

        public DateTime? DateReceivedByLab
        {
            get
            {
                return _dateReceivedByLab;
            }
            set
            {
                _dateReceivedByLab = value.ToNullableDateTime();
            }
        }

        private DateTime? _dateProductionCompleted;

        public DateTime? DateProductionCompleted
        {
            get
            {
                return _dateProductionCompleted;
            }
            set
            {
                _dateProductionCompleted = value.ToNullableDateTime();
            }
        }

        private DateTime? _dateSentToClinic;

        public DateTime? DateSentToClinic
        {
            get
            {
                return _dateSentToClinic;
            }
            set
            {
                _dateSentToClinic = value.ToNullableDateTime();
            }
        }

        private DateTime? _dateLabDispensed;

        public DateTime? DateLabDispensed
        {
            get
            {
                return _dateLabDispensed;
            }
            set
            {
                _dateLabDispensed = value.ToNullableDateTime();
            }
        }

        private DateTime? _dateClinicReceived;

        public DateTime? DateClinicReceived
        {
            get
            {
                return _dateClinicReceived;
            }
            set
            {
                _dateClinicReceived = value.ToNullableDateTime();
            }
        }

        private DateTime? _dateClinicDispensed;

        public DateTime? DateClinicDispensed
        {
            get
            {
                return _dateClinicDispensed;
            }
            set
            {
                _dateClinicDispensed = value.ToNullableDateTime();
            }
        }

        private DateTime? _dateRejected;

        public DateTime? DateRejected
        {
            get
            {
                return _dateRejected;
            }
            set
            {
                _dateRejected = value.ToNullableDateTime();
            }
        }

        private DateTime? _dateCancelled;

        public DateTime? DateCancelled
        {
            get
            {
                return _dateCancelled;
            }
            set
            {
                _dateCancelled = value.ToNullableDateTime();
            }
        }

        private DateTime? _dateResubmitted;

        public DateTime? DateResubmitted
        {
            get
            {
                return _dateResubmitted;
            }
            set
            {
                _dateResubmitted = value.ToNullableDateTime();
            }
        }

        private bool _isActive;

        public bool IsActive
        {
            get
            {
                return _isActive;
            }
            set
            {
                _isActive = value;
            }
        }

        private XmlDocument _XmlDoc;

        public XmlDocument XmlDoc
        {
            get { return _XmlDoc; }
            set { _XmlDoc = value; }
        }

        public String SelectedLabel
        {
            get { return ViewState["SelectedLabel"].IsNull() ? this.ddlLabelFormat.SelectedValue : ViewState["SelectedLabel"].ToString(); }
            set { ViewState["SelectedLabel"] = value; }
        }

        public string HoldForStockReason
        {
            get { return String.Format("Hold: Anticipated stock date is {0}.  Justification: {1}", this.hfStockDate.Value, this.hdfHoldForStockReason.Value); }
            //get { return String.Format("Hold: Anticipated stock date is {0}.  Justification: {1}", this.hfStockDate.Value, this.tbHoldForStockReason.Text); }
        }

        public List<ManageOrderEntity> StatusPending
        {
            get
            {
                return ViewState["StatusPending"] as List<ManageOrderEntity>;
            }
            set
            {
                ViewState["StatusPending"] = value;
            }

        }
        public List<ManageOrderEntity> StatusOnHold
        {
            get
            {
                return ViewState["StatusOnHold"] as List<ManageOrderEntity>;
            }
            set
            {
                ViewState["StatusOnHold"] = value;
            }

        }

        public List<ManageOrderEntity> HoldForStockOrders
        {
            get
            {
                return ViewState["HoldForStockOrders"] as List<ManageOrderEntity>;
            }
            set
            {
                ViewState["HoldForStockOrders"] = value;
                // get orders not on hold
                List<ManageOrderEntity> Status_NonHold = new List<ManageOrderEntity>();
                Status_NonHold = (from h in value
                                  where (h.StatusComment.Substring(0, 4).ToString() != "Hold")
                                  select h).ToList();
                if (gvPendingSortExpression != null)
                {
                    var paramPending = Expression.Parameter(typeof(ManageOrderEntity), gvPendingSortExpression);
                    var sortExpressionPending = Expression.Lambda<Func<ManageOrderEntity, object>>(Expression.Convert(Expression.Property(paramPending, gvPendingSortExpression), typeof(object)), paramPending); //Aldela: Added
                    if (gvPendingSortDirection == System.Web.UI.WebControls.SortDirection.Ascending)
                        Status_NonHold = Status_NonHold.AsQueryable<ManageOrderEntity>().OrderBy(sortExpressionPending).ToList();
                    else
                        Status_NonHold = Status_NonHold.AsQueryable<ManageOrderEntity>().OrderByDescending(sortExpressionPending).ToList();
                }
                this.gvPending.DataSource = Status_NonHold;
                this.gvPending.DataBind();
                StatusPending = Status_NonHold;
                hdfTotalOrdersPending.Value = StatusPending.Count.ToString();

                // get orders on hold
                List<ManageOrderEntity> Status_Hold = new List<ManageOrderEntity>();
                Status_Hold = (from h in value
                               where (h.StatusComment.Substring(0, 4).ToString() == "Hold")
                               select h).ToList();
                if (gvStockonHoldSortExpression != null) //Aldela: Added
                {
                    var paramStockOnHold = Expression.Parameter(typeof(ManageOrderEntity), gvStockonHoldSortExpression);
                    var sortExpressionStockOnHold = Expression.Lambda<Func<ManageOrderEntity, object>>(Expression.Convert(Expression.Property(paramStockOnHold, gvStockonHoldSortExpression), typeof(object)), paramStockOnHold);
                    if (gvStockOnHoldSortDirection == System.Web.UI.WebControls.SortDirection.Ascending)
                        Status_Hold = Status_Hold.AsQueryable<ManageOrderEntity>().OrderBy(sortExpressionStockOnHold).ToList();
                    else
                        Status_Hold = Status_Hold.AsQueryable<ManageOrderEntity>().OrderByDescending(sortExpressionStockOnHold).ToList();
                }
                this.gvStockonHold.DataSource = Status_Hold;
                this.gvStockonHold.DataBind();
                StatusOnHold = Status_Hold;
                hdfTotalOrdersOnHold.Value = StatusOnHold.Count.ToString();
            }
        }

        public int TotalLensItemsSelected
        {
            get { return (int)ViewState["TotalLensItemsSelected"]; }
            set
            {
                ViewState.Add("TotalLensItemsSelected", value);
                TotalOrdersChecked = value;
            }
        }

        public int TotalFrameItemsSelected
        {
            get { return (int)ViewState["TotalFrameItemsSelected"]; }
            set
            {
                ViewState.Add("TotalFrameItemsSelected", value);
                TotalOrdersChecked = value;
            }
        }

        public int TotalOrdersChecked
        {
            get { return (int)ViewState["TotalOrdersChecked"]; }
            set { ViewState.Add("TotalOrdersChecked", value); }
        }

        public List<ManageOrderEntity> SearchData
        {
            get { return (List<ManageOrderEntity>)ViewState["SearchData"]; }
            set { ViewState.Add("SearchData", value); }
        }

        public string SearchSortDirection
        {
            get { return (string)ViewState["SearchSortDirection"]; }
            set { ViewState.Add("SearchSortDirection", value); }
        }

        public string CurrentSortField
        {
            get { return (string)ViewState["CurrentSortField"]; }
            set { ViewState.Add("CurrentSortField", value); }
        }

        #endregion Page Members


        public List<SiteCodeEntity> RedirectOrdersLabList
        {
            get
            {
                return ViewState["RedirectOrdersLabList"] as List<SiteCodeEntity>;
            }
            set
            {
                ViewState["RedirectOrdersLabList"] = value;
            }
        }

        public String RedirectLab
        {
            get
            {
                return ViewState["RedirectOrderLab"].IsNull() ? this.ddlRedirectLab.SelectedValue : ViewState["RedirectOrderLab"].ToString();
            }
            set
            {
                ViewState["RedirectOrderLab"] = value;

            }

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


    }
}