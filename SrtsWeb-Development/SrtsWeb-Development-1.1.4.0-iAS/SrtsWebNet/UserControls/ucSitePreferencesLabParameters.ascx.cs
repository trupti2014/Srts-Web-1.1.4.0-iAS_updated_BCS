using SrtsWeb.Account;
using SrtsWeb.Base;
using SrtsWeb.BusinessLayer.Concrete;
using SrtsWeb.Entities;
using SrtsWeb.ExtendersHelpers;
using SrtsWeb.Presenters.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using SrtsWeb.Views.Admin;

namespace SrtsWeb.UserControls
{
    [PrincipalPermission(SecurityAction.Demand, Role = "MgmtEnterprise")]
    [PrincipalPermission(SecurityAction.Demand, Role = "MgmtAdmin")]
    [PrincipalPermission(SecurityAction.Demand, Role = "ClinicAdmin")]
    [PrincipalPermission(SecurityAction.Demand, Role = "LabAdmin")]
    [PrincipalPermission(SecurityAction.Demand, Role = "TrainingAdmin")]
    public partial class ucSitePreferencesLabParameters : UserControlBase, ISitePreferencesLabParametersView
    {
        private SitePreferencesPresenter.LabParametersPresenter p;

        public ucSitePreferencesLabParameters()
        {
            p = new SitePreferencesPresenter.LabParametersPresenter(this);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.mySession.MyIndividualID = SrtsWeb.Account.CustomProfile.GetProfile(HttpContext.Current.User.Identity.Name).IndividualId;
            ((SrtsWeb.WebForms.Admin.SitePreferences)this.Page).RefreshUserControl += ucSitePreferences_RefreshUserControl;
            if (!IsPostBack)
            {
                this.p = new SitePreferencesPresenter.LabParametersPresenter(this);
                this.p.InitView();
                BindFabricationParameters();
                                
            }
            this.btnSaveLabParams.Text = this.p.HasLensLabParameters() ? "Update" : "Save";
            if (!this.mySession.MySite.IsMultivision) //When a SV lab is configuring their lens capabilites, unable to select MV.
            {
                //ddlCapabilityType.Items.Remove(ddlCapabilityType.Items.FindByValue("MV")); 
                ListItem i = ddlCapabilityType.Items.FindByValue("MV");
                i.Attributes.Add("style", "color:gray;");
                i.Attributes.Add("disabled", "true");
                //i.Value = "-1";
            }
        }

        private void ucSitePreferences_RefreshUserControl(object sender, EventArgs e)
        {
            this.p = new SitePreferencesPresenter.LabParametersPresenter(this);
            this.p.InitView();
            BindFabricationParameters();
        }

        protected override void Render(HtmlTextWriter writer)
        {
            //using (MethodTracer.Trace(SrtsTraceSource.AdminSource, "SiteCodeEdit_Render", mySession.MyUserID))
            //{
            foreach (GridViewRow r in gvLabParameters.Rows)
            {
                if (r.RowType == DataControlRowType.DataRow)
                {
                    //r.Attributes["onclick"] = this.Page.ClientScript.GetPostBackClientHyperlink(this.gvLabParameters, "Select$" + r.RowIndex, true);
                }
            }

            base.Render(writer);
            //}
        }
        protected void gvLabParameters_RowCommand(object sender, GridViewCommandEventArgs e)
        {
#if DEBUG
            using (MethodTracer.Trace(SrtsTraceSource.AdminSource, "SiteCodeEdit_gvLabParameters_RowCommand", mySession.MyUserID))
#endif
            {
                var i = default(Int32);
                if (!Int32.TryParse(e.CommandArgument.ToString(), out i)) return;

                switch (e.CommandName.ToLower())
                {
                    case "delete":
                        var id = ((GridView)sender).DataKeys[i];
                        p.DeleteParameter(id.Value.ToInt32());
                        LogEvent("User {0} deleted a lab parameter on {1}", new Object[] { mySession.MyUserID, DateTime.Now });
                        break;
                }
            }
        }

        //protected void gvLabParameters_RowCreated(object sender, GridViewRowEventArgs e)
        protected void gvLabParameters_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow) return;

            e.Row.Attributes.Add("onmouseover", "this.style.backgroundColor='#CCCCCC'; this.style.cursor='pointer';");
            e.Row.Attributes.Add("onmouseout", "this.style.backgroundColor=''; this.style.textDecoration='none';");
            //e.Row.ToolTip = "Click to row to edit";

            e.Row.Cells[1].ToolTip = "Click to delete parameter";

            // e.Row.Attributes["onclick"] = this.Page.ClientScript.GetPostBackClientHyperlink(this.gvLabParameters, "Select$" + e.Row.RowIndex);//, true);

        }

        protected void gvLabParameters_RowEditing(object sender, GridViewEditEventArgs e)
        {
            //Set the edit index.
            //gvLabParameters.EditIndex = e.NewEditIndex;
            //gvLabParameters_Edit(e.NewEditIndex);

            var i = default(Int32);
            if (!Int32.TryParse(gvLabParameters.DataKeys[e.NewEditIndex/*gvLabParameters.SelectedRow.RowIndex*/].Value.ToString(), out i)) return;

            GridViewRow row = gvLabParameters.Rows[e.NewEditIndex];//gvLabParameters.SelectedRow;
            var mat = (Label)row.FindControl("lblMat");
            var stock = (Label)row.FindControl("lblIsStocked");
            var cyl = (Label)row.FindControl("lblCyl");
            var plus = (Label)row.FindControl("lblMaxPlus");
            var minus = (Label)row.FindControl("lblMaxMinus");
            var captype = (Label)row.FindControl("lblCapabilityType");

            this.Material = mat.Text;
            this.IsStocked = stock.Text == "YES" ? "TRUE" : "FALSE";
            this.tbCyl.Text = cyl.Text;
            this.tbMaxPlus.Text = plus.Text;
            this.tbMaxMinus.Text = minus.Text;
            this.CapabilityType = captype.Text;
            this.btnSaveParams.Text = "Update";
            this.MatParamID = i;

            //Bind data to the GridView control.
            //BindData();
        }

        protected void btnSaveParams_Click(object sender, EventArgs e)
        {
            //using (MethodTracer.Trace(SrtsTraceSource.AdminSource, "SiteCodeEdit_btnSaveParams_Click", mySession.MyUserID))
            //{
            var m = string.Empty;

            if (this.btnSaveParams.Text.Equals("Update"))
            {
                this.btnSaveParams.Text = "Save";
                if (!p.UpdateParameter(this.MatParamID))
                {
                    this.hfMsgLabPref.Value = "There was an error attempting to update the lab parameter";
                    m = this.hfMsgLabPref.Value;
                    return;
                }
                this.hfSuccessLabPref.Value = "1";
                this.hfMsgLabPref.Value = "Successfully updated the lab parameter.";
            }
            else if (this.btnSaveParams.Text.Equals("Save"))
            {
                if (!p.InsertParameter())
                {
                    this.hfMsgLabPref.Value = "There was an error attempting to add the lab parameter";
                    m = this.hfMsgLabPref.Value;
                    return;
                }
                this.hfSuccessLabPref.Value = "1";
                this.hfMsgLabPref.Value = "Successfully added the lab parameter.";
               
            }
            m = this.hfMsgLabPref.Value;
            LogEvent("User {0} {1} parameter restrictions to site {2} at {3}.", new Object[] { mySession.MyUserID, m, this.SiteCode, DateTime.Now });
            p.FillLabParameters();
            BindFabricationParameters();
            ClearInputs();
            //}
        }

        protected void gvLabParameters_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
#if DEBUG
            using (MethodTracer.Trace(SrtsTraceSource.AdminSource, "SiteCodeEdit_gvLabParameters_RowDeleting", mySession.MyUserID))
#endif
            {
                p.FillLabParameters();
                BindFabricationParameters();
                ClearInputs();
                this.btnSaveParams.Text = "Save";
            }
        }
        protected void ClearInputs()
        {
            ddlMatType.SelectedIndex = 0;
            ddlIsStocked.SelectedIndex = 0;
            tbCyl.Text = string.Empty;
            tbMaxPlus.Text = string.Empty;
            tbMaxMinus.Text = string.Empty;
            ddlCapabilityType.SelectedIndex = 0;
        }

        protected void ddlIsStocked_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (IsStocked == "FALSE")
            {
                tbCyl.Text = "0.00";
                tbMaxPlus.Text = "0.00";
                tbMaxMinus.Text = "0.00";
            }
        }

        protected void btnSaveLabParams_Click(object sender, EventArgs e)
        {
            var m = string.Empty;

            if (!p.InsertUpdateLabParameter(this.SiteCode))
            {
                this.hfMsgLabPref.Value = "There was an error attempting to add the lab parameter";
                m = this.hfMsgLabPref.Value;
                return;
            }
            this.hfSuccessLabPref.Value = "1";
            this.hfMsgLabPref.Value = "Successfully added the lab parameter.";

            m = this.hfMsgLabPref.Value;
            LogEvent("User {0} {1} parameter restrictions to site {2} at {3}.", new Object[] { mySession.MyUserID, m, this.SiteCode, DateTime.Now });
            p.FillLensLabParameters();

        }

        public Dictionary<string, string> LensMaterial
        {
            set
            {
                this.ddlMatType.DataSource = value;
                this.ddlMatType.DataTextField = "Value";
                this.ddlMatType.DataValueField = "Value";
                this.ddlMatType.DataBind();
                this.ddlMatType.Items.Insert(0, new ListItem("-Select-", "X"));
            }
        }

        private void LoadLookupTable()
        {
            SrtsWeb.BusinessLayer.Abstract.ILookupService _service = new LookupService();
            Cache["SRTSLOOKUP"] = _service.GetAllLookups();
        }

       

        #region INTERFACE PROPERTIES
        public List<LookupTableEntity> CacheData
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

        public List<FabricationParameterEntitiy> FabricationParameterData
        {
            get { return (List<FabricationParameterEntitiy>)Session["FabricationParameterData"]; }
            set { Session.Add("FabricationParameterData", value); }
        }



        private void BindFabricationParameters()
        {
            gvLabParameters.DataKeyNames = new[] { "ID" };
            gvLabParameters.DataSource = FabricationParameterData;
            gvLabParameters.DataBind();
            //gvLabParameters.SelectedIndex = -1;
        }

        public string Material
        {
            get { return ddlMatType.SelectedValue; }
            set
            {
                var i = this.ddlMatType.Items.IndexOf(ddlMatType.Items.FindByValue(value));
                if (i != -1)
                {
                    ddlMatType.SelectedValue = value;
                }
            }
        }

        public int MatParamID
        {
            get { return hfMatParamID.Value.ToInt32(); }
            set { hfMatParamID.Value = value.ToString(); }
        }

        private string _errMessage;
        public string ErrMessage
        {
            get { return _errMessage; }
            set { _errMessage = value; }
        }

        public string SiteCode
        {
            get
            {
                var pg = ((ISitePreferencesView)this.Page);
                return pg.SiteCode;
            }
        }


        public string IsStocked
        {
            get { return ddlIsStocked.SelectedValue; }
            set { ddlIsStocked.SelectedValue = value; }
        }

        public decimal Cylinder
        {
            get { return Convert.ToDecimal(tbCyl.Text); }
            set { tbCyl.Text = value.ToString(); }
        }

        public decimal MaxPlus
        {
            get { return Convert.ToDecimal(tbMaxPlus.Text); }
            set { tbMaxPlus.Text = value.ToString(); }
        }

        public decimal MaxMinus
        {
            get { return Convert.ToDecimal(tbMaxMinus.Text); }
            set { tbMaxMinus.Text = value.ToString(); }
        }

        public string CapabilityType
        {
            get { return ddlCapabilityType.SelectedValue; }
            set { ddlCapabilityType.SelectedValue = value; }
        }


        public decimal MaxPrism
        {
            get { return Convert.ToDecimal(tbMaxPrism.Text); }
            set { tbMaxPrism.Text = value.ToString(); }
        }

        public decimal MaxDecentrationPlus
        {
            get { return Convert.ToDecimal(tbMaxDecentrationPlus.Text); }
            set { tbMaxDecentrationPlus.Text = value.ToString(); }
        }

        public decimal MaxDecentrationMinus
        {
            get { return Convert.ToDecimal(tbMaxDecentrationMinus.Text); }
            set { tbMaxDecentrationMinus.Text = value.ToString(); }
        }

        #endregion
    }
}