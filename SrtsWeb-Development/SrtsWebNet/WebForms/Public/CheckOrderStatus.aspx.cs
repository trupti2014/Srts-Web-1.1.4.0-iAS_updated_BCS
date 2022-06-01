using System;
using System.Text;
using System.Globalization;
using System.Threading;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SrtsWeb.Entities;
using SrtsWeb.Views.Public;
using SrtsWeb.Presenters.Public;
using SrtsWeb.ExtendersHelpers;

namespace SrtsWeb.WebForms.Public
{
    public partial class CheckOrderStatus : System.Web.UI.Page, ICheckOrderStatusView
    {
        const string colorAlternate = "#e6f2ff";
        protected void Page_Load(object sender, EventArgs e)
        {
            Master.HideMainContent();

            if (!this.IsPostBack)
            {
                this.divIdEntry.Visible = true;
                this.divOrderStatus.Visible = false;
                this.tbIdNum.Focus();
            }
        }

        protected void CaptchaValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            //validate the Captcha to check we're not dealing with a bot
            args.IsValid = CheckOrderStatusCaptcha.Validate(args.Value);

            CaptchaCode.Text = null;
        }

        protected void bSubmit_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                // Find patient by id in srts database
                var p = new CheckOrderStatusPresenter(this);
                if (p.GetIndividualInfo())
                {
                    this.divIdEntry.Visible = false;
                    this.divOrderStatus.Visible = true;
                }
                else
                {
                    this.divIdEntry.Visible = true;
                    this.divMessage.Style.Add("visibility", "visible");
                    this.litMessage.Text = "Sorry, there is no data available for the number you entered.";
                }
            }
        }

        public string IdNumber
        {
            get { return this.tbIdNum.Text; }
        }

        public List<CheckOrderStatusEntity> PatientStatuses
        {
            get
            {
                return ViewState["PatientStatuses"] as List<CheckOrderStatusEntity>;
            }
            set
            {
                ViewState["PatientStatuses"] = value;
                this.gvOrders.DataSource = value;
                this.gvOrders.DataBind();
            }
        }

        public List<IndividualEntity> PatientInfo
        {
            get
            {
                return ViewState["PatientInfo"] as List<IndividualEntity>;
            }
            set
            {
                ViewState["PatientInfo"] = value;
                IndividualEntity p = new IndividualEntity();
                p = value.FirstOrDefault();
                this.divPatientNameHeader.Attributes.Add("class", "patientnameheader_CheckOrderStatus");
                this.litPatientNameHeader.Text = string.Format("Name:  {0}", p.NameFML);
            }
        }

        protected override void Render(HtmlTextWriter writer)
        {
            foreach (GridViewRow r in gvOrders.Rows)
            {
                if (r.RowType == DataControlRowType.DataRow)
                {
                    // on row hover, change color
                    r.Attributes["onmouseover"] = "this.style.cursor='pointer';this.style.backgroundColor='#c8e4b6';";

                    // when row not hovered, set alternating colors
                    if (r.RowType == DataControlRowType.DataRow &&
                        r.RowState == DataControlRowState.Normal)
                        r.Attributes["onmouseout"] = "this.style.backgroundColor='white';";
                    if (r.RowType == DataControlRowType.DataRow &&
                        r.RowState == DataControlRowState.Alternate)
                        r.Attributes["onmouseout"] = "this.style.backgroundColor='" + colorAlternate + "';";


                    r.ToolTip = "Click to View This Order's Detail";
                    r.Attributes["onclick"] = this.Page.ClientScript.GetPostBackClientHyperlink(this.gvOrders, "Select$" + r.RowIndex, true);

                }
            }

            base.Render(writer);
        }

        protected void gvOrders_SelectedIndexChanged(object sender, EventArgs e)
        {
            // remove selected row highlight and reset row colors to alternating 
            foreach (GridViewRow row in gvOrders.Rows)
            {
                if (row.RowType == DataControlRowType.DataRow && row.RowState == DataControlRowState.Normal)
                    row.BackColor = ColorTranslator.FromHtml("#FFFFFF");
                if (row.RowType == DataControlRowType.DataRow && row.RowState == DataControlRowState.Alternate)
                    row.BackColor = ColorTranslator.FromHtml('"' + colorAlternate + '"');
            }
            // highlight selected row
            GridViewRow rowSelected = gvOrders.SelectedRow;
            rowSelected.BackColor = ColorTranslator.FromHtml("#c8e4b6");
        }
        protected void gvOrders_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            var lOn = new List<CheckOrderStatusEntity>() { this.PatientStatuses[e.CommandArgument.ToInt32()] };
            this.gvOrderStatus.DataSource = lOn;
            this.gvOrderStatus.DataBind();
            this.divOrderDetail.Visible = true;
        }

        protected void gvOrders_RowCreated(object sender, GridViewRowEventArgs e)
        {
            if (e == null)
            {
                throw new ArgumentNullException("e");
            }

            if (e.Row.RowType == DataControlRowType.Header)
            {
                e.Row.Cells[6].Text = GetColumnHeader("Status");
                e.Row.Cells[6].Attributes.Add("onmouseover", "GetCurrentOrderStatusToolTip()");
                e.Row.Cells[6].Attributes.Add("onmouseout", "Hide('divCurrentOrderStatusToolTip')");

                e.Row.Cells[5].Text = GetColumnHeader("Lab");
                e.Row.Cells[5].Attributes.Add("onmouseover", "GetLabToolTip()");
                e.Row.Cells[5].Attributes.Add("onmouseout", "Hide('divLabToolTip')");

                e.Row.Cells[4].Text = GetColumnHeader("Tint");
                e.Row.Cells[4].Attributes.Add("onmouseover", "GetTintToolTip()");
                e.Row.Cells[4].Attributes.Add("onmouseout", "Hide('divTintToolTip')");

            }
            if (e.Row.RowType == DataControlRowType.DataRow && e.Row.RowState == DataControlRowState.Normal)
                e.Row.BackColor = ColorTranslator.FromHtml("#FFFFFF");
            if (e.Row.RowType == DataControlRowType.DataRow && e.Row.RowState == DataControlRowState.Alternate)
                e.Row.BackColor = ColorTranslator.FromHtml('"' + colorAlternate + '"');
        }

        protected void gvOrders_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e == null)
            {
                throw new ArgumentNullException("e");
            }

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // convert color to title case
                string color = e.Row.Cells[2].Text;
                if (!string.IsNullOrEmpty(color))
                {
                    e.Row.Cells[2].Text = ConvertToTitleCase(color.ToLower());
                }

                // convert lens to title case
                string lens = e.Row.Cells[3].Text;
                if (!string.IsNullOrEmpty(lens))
                {
                    e.Row.Cells[3].Text = ConvertToTitleCase(lens.ToLower());
                }

                // get friendly names for InitialLoadFrame
                string frame = e.Row.Cells[1].Text;
                string framefriendlyName = GetFriendlyNames_Frame(frame);
                if (!string.IsNullOrEmpty(framefriendlyName))
                {
                    e.Row.Cells[1].Text = framefriendlyName;
                }

                // get friendly names for Tint
                string tint = e.Row.Cells[4].Text;
                string tintfriendlyName = GetFriendlyNames_Tint(tint);
                if (!string.IsNullOrEmpty(tintfriendlyName))
                {
                    e.Row.Cells[4].Text = tintfriendlyName;
                }
            }
        }

        protected string ConvertToTitleCase(string text)
        {
            string title = string.Empty;
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            title = textInfo.ToTitleCase(text);
            return title;
        }
        protected string GetFriendlyNames_Tint(string type)
        {
            string name = string.Empty;
            switch (type)
            {
                case "CL":
                    name = "Clear";// CL
                    break;
            }
            return name;
        }
        protected string GetFriendlyNames_Frame(string type)
        {
            string name = string.Empty;
            switch (type)
            {
                case "350":
                case "350NS":
                case "COV51":
                case "COV53":
                case "DELTA":
                case "ELI52":
                case "ELITE":
                case "ELL50":
                case "ELL51":
                case "ELL52":
                case "ELL53":
                case "ENCOR":
                case "ESSOK":
                case "EXC51":
                case "EXCEL":
                case "KEE52":
                case "KEE55":
                case "KEE58":
                case "KIN50":
                case "KIN51":
                case "KIN52":
                case "KIN53":
                case "KIN54":
                case "KIN55":
                case "LIB50":
                case "LIB52":
                case "LIB54":
                case "MUG50":
                case "MUG52":
                case "MUG54":
                case "OSA53":
                case "OSA55":
                case "SID54":
                case "SIDES":
                case "THU52":
                case "THU54":
                case "VIS51":
                case "VISTA":
                case "WIL50":
                case "WIL51":
                case "WIL52":
                case "WIL53":
                case "WIL54":
                case "WIL55":
                    name = "InitialLoadFrame of Choice";// FOC
                    break;
                case "5AL":
                case "5AL54":
                case "5AM":
                case "5AM50":
                case "5AM52":
                case "5AM54":
                case "5AP52":
                case "5APL":
                case "5APM":
                case "5APS":
                case "5AS":
                case "HLF":
                case "HLF50":
                    name = "Standard Issue"; // SI
                    break;
                case "EAB43":
                case "EAB45":
                case "M40":
                case "M45":
                case "M50":
                case "MC":
                case "MSOI":
                    name = "Protective Mask Insert"; // PMI
                    break;
                case "AFD":
                case "AFF":
                case "AFJS":
                case "AH64":
                case "AV":
                case "AVS":
                case "FGG":
                case "FGS":
                    name = "Aviator Flight Frames"; // Aviator
                    break;
                case "UPLC":
                    name = "Combat Eye Protection Inserts"; // CEP
                    break;
            }
            return name;
        }
        protected string GetColumnHeader(string column)
        {
            string name = string.Empty;
            switch (column)
            {
                case "Status":
                    name = "<p class='pointer'>Current Order Status</p>";
                    break;
                case "Tint":
                    name = "<p class='pointer_small' style='width:100px'>Tint</p>";
                    break;
                case "Lab":
                    name = "<p class='pointer_small' style='width:100px'>Lab</p>";
                    break;
            }
            return name;
        }


        protected void Exit_Command(object sender, CommandEventArgs e)
        {
            Response.Redirect("~/WebForms/default.aspx");
        }
    }
}