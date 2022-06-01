using SrtsWeb.Base;
using SrtsWeb.BusinessLayer.Concrete;
using SrtsWeb.Entities;
using SrtsWeb.ExtendersHelpers;
using SrtsWeb.Presenters.Admin;
using SrtsWeb.Views.Admin;
using System;
using System.Collections.Generic;
using System.Security.Permissions;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SrtsWeb.Admin
{
    [PrincipalPermission(SecurityAction.Demand, Role = "HumanAdmin")]
    [PrincipalPermission(SecurityAction.Demand, Role = "LabAdmin")]
    [PrincipalPermission(SecurityAction.Demand, Role = "ClinicAdmin")]
    [PrincipalPermission(SecurityAction.Demand, Role = "MgmtAdmin")]
    [PrincipalPermission(SecurityAction.Demand, Role = "MgmtEnterprise")]
    [PrincipalPermission(SecurityAction.Demand, Role = "MgmtDataMgmt")]
    [PrincipalPermission(SecurityAction.Demand, Role = "TrainingAdmin")]
    public partial class SiteCodeEdit : PageBase, ISiteCodeEditView
    {
        private SiteCodeEditPresenter _presenter;

        public SiteCodeEdit()
        {
            _presenter = new SiteCodeEditPresenter(this);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.mySession.MyIndividualID = SrtsWeb.Account.CustomProfile.GetProfile(HttpContext.Current.User.Identity.Name).IndividualId;

            if (!IsPostBack)
            {
                SiteCode = Request.QueryString["id"].ToString();
                _presenter.InitView();
                Master.CurrentModuleTitle = string.Empty;
                CurrentModule("Administration - Facilities Manager - Edit");
                CurrentModule_Sub(string.Empty);
                BuildPageTitle();
            }
            if (ddlSiteType.SelectedValue.ToUpper() == "LAB" || ddlSiteType.SelectedValue.ToUpper() == "ADMIN")
            {
                pnlLabs.Visible = false;
                pnlLabsOnly.Visible = true;

                //if (ddlSiteType.SelectedValue.ToUpper() == "LAB")
                //{
                //    pnlLabParams.Visible = true;
                //    BindFabricationParameters();
                //}
            }
            else
            {
                pnlLabsOnly.Visible = false;
                pnlLabs.Visible = true;
            }
            if (Roles.IsUserInRole("MgmtDataMgmt") || Roles.IsUserInRole("MgmtEnterprise") || Roles.IsUserInRole("MgmtAdmin") || Roles.IsUserInRole("TrainingAdmin"))
            {
                ddlMPrimary.Enabled = true;

                ddlSPrimary.Enabled = true;
            }
            else
            {
                tbSiteCode.Enabled = false;

                ddlMPrimary.Enabled = false;

                ddlSPrimary.Enabled = false;
            }
        }

        //protected override void Render(HtmlTextWriter writer)
        //{
        //    using (MethodTracer.Trace(SrtsTraceSource.AdminSource, "SiteCodeEdit_Render", mySession.MyUserID))
        //    {
        //        foreach (GridViewRow r in gvLabParameters.Rows)
        //        {
        //            if (r.RowType == DataControlRowType.DataRow)
        //            {
        //                r.Attributes["onclick"] = this.Page.ClientScript.GetPostBackClientHyperlink(this.gvLabParameters, "Select$" + r.RowIndex, true);
        //            }
        //        }

        //        base.Render(writer);
        //    }
        //}

        private void ShowMessage()
        {
            CustomValidator cv = new CustomValidator();
            cv.IsValid = false;
            cv.ErrorMessage = ErrMessage;
            this.Page.Validators.Add(cv);
            return;
        }

        protected void btnAddAddr_Click(object sender, EventArgs e)
        {
#if DEBUG
            using (MethodTracer.Trace(SrtsTraceSource.AdminSource, "SiteCodeEdit_btnAddAddr_Click", mySession.MyUserID))
#endif
            {
                ErrMessage = string.Empty;
                _presenter.InsertAddress();
                if (!string.IsNullOrEmpty(ErrMessage))
                {
                    ShowMessage();
                    LogEvent(String.Format("User {0} unsuccessfully added new address to site {1} at {2}.", mySession.MyUserID, this.SiteCode, DateTime.Now));
                }
                else
                {
                    LogEvent(String.Format("User {0} added new address to site {1} at {2}.", mySession.MyUserID, this.SiteCode, DateTime.Now));
                    Response.Redirect("~/WebForms/Admin/SiteCodeManagement.aspx");
                }
            }
        }

        protected void rblUseAddress_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (UseAddress)
            {
                MailZipCode = ZipCode.ToZipCodeDisplay();
                MailState = State;
                MailCountry = Country;
                MailAddress1 = Address1;
                MailAddress2 = Address2;
                MailAddress3 = Address3;
                MailCity = City;
                IsConusMail = IsConus;
            }
        }

        protected void ddlSiteType_SelectedIndexChanged(object sender, EventArgs e)
        {
#if DEBUG
            using (MethodTracer.Trace(SrtsTraceSource.AdminSource, "SiteCodeEdit_ddlSiteType_SelectedIndexChanged", mySession.MyUserID))
#endif
            {
                if (ddlSiteType.SelectedValue.ToUpper().StartsWith("CLI"))
                {
                    ddlMPrimary.Visible = true;
                    ddlSPrimary.Visible = true;
                    _presenter.FillLabReportInfo(ddlSiteType.SelectedValue);
                }
                else
                {
                    ddlMPrimary.Visible = false;
                    ddlSPrimary.Visible = false;
                }
            }
        }

        private void BuildPageTitle()
        {
            try
            {
                Master.CurrentModuleTitle = string.Format("{0} {1}", mySession.CurrentModule, mySession.CurrentModule_Sub);
                Master.uplCurrentModuleTitle.Update();
            }
            catch (NullReferenceException)
            {
                CurrentModule("Administration - Facilities Manager - Edit");
                CurrentModule_Sub(string.Empty);
            }
        }

        #region Accessors
        public List<LookupTableEntity> CacheData
        {
            get
            {
                if (Cache["SRTSLOOKUP"] == null)
                {
                   Master.LoadLookupTable();
                }
                return Cache["SRTSLOOKUP"] as List<LookupTableEntity>;
            }
            set { Cache["SRTSLOOKUP"] = value; }
        }
      
        //public List<FabricationParameterEntitiy> FabricationParameterData
        //{
        //    get { return (List<FabricationParameterEntitiy>)Session["FabricationParameterData"]; }
        //    set { Session.Add("FabricationParameterData", value); }
        //}

        //private void BindFabricationParameters()
        //{
        //    gvLabParameters.DataKeyNames = new[] { "ID" };
        //    gvLabParameters.DataSource = FabricationParameterData;
        //    gvLabParameters.DataBind();
        //    //gvLabParameters.SelectedIndex = -1;
        //}

        private string _errMessage;

        public string ErrMessage
        {
            get { return _errMessage; }
            set { _errMessage = value; }
        }

        public bool UseAddress
        {
            get { return Convert.ToBoolean(rblUseAddress.SelectedValue); }
            set { rblUseAddress.SelectedValue = value.ToString(); }
        }

        public bool HasLMS
        {
            get
            {
                if (rblHasLMS.SelectedValue.ToString().ToUpper() == "TRUE")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            set { rblHasLMS.SelectedValue = value.ToString(); }
        }

        public bool ShipToPatientLab
        {
            get
            {
                if (rblShipToPatientLab.SelectedValue.ToString().ToUpper() == "TRUE")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            set
            {
                rblShipToPatientLab.SelectedValue = value.ToString();
            }
        }

        public string MultiPrimary
        {
            get { return ddlMPrimary.SelectedValue; }
            set { ddlMPrimary.SelectedValue = value; }
        }

        public string MultiSecondary
        {
            get;

            set;
        }

        public string SinglePrimary
        {
            get { return ddlSPrimary.SelectedValue; }
            set { ddlSPrimary.SelectedValue = value; }
        }

        public string SingleSecondary
        {
            get;

            set;
        }

        public List<KeyValuePair<string, string>> MPrimary
        {
            get { return (List<KeyValuePair<string, string>>)ddlMPrimary.DataSource; }
            set
            {
                ddlMPrimary.Items.Clear();
                ddlMPrimary.DataSource = value;
                ddlMPrimary.DataValueField = "Key";
                ddlMPrimary.DataTextField = "Value";
                ddlMPrimary.DataBind();
            }
        }

        private List<KeyValuePair<string, string>> _sPrimary;

        public List<KeyValuePair<string, string>> SPrimary
        {
            get { return _sPrimary; }
            set
            {
                _sPrimary = value;
                ddlSPrimary.Items.Clear();
                ddlSPrimary.DataSource = _sPrimary;
                ddlSPrimary.DataTextField = "Value";
                ddlSPrimary.DataValueField = "Key";
                ddlSPrimary.DataBind();
            }
        }

        public List<KeyValuePair<string, string>> MSecondary
        {
            get;
            set;
        }

        public List<KeyValuePair<string, string>> SSecondary
        {
            get;
            set;
        }

        public int Region
        {
            get;
            set;
        }

        public string SiteCode
        {
            get { return tbSiteCode.Text; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    tbSiteCode.Text = value;
                }
                else
                {
                    tbSiteCode.Text = string.Empty;
                }
            }
        }

        public string SiteCodetb
        {
            get { return tbSiteCode.Text; }
            set { tbSiteCode.Text = value; }
        }

        public string SiteName
        {
            get { return tbSiteName.Text; }
            set { tbSiteName.Text = value; }
        }

        public string SiteType
        {
            get { return ddlSiteType.SelectedValue; }
            set { ddlSiteType.SelectedValue = value; }
        }

        public string SiteDescription
        {
            get { return tbSiteDescription.Text; }
            set { tbSiteDescription.Text = value; }
        }

        public List<LookupTableEntity> SiteTypes
        {
            get { return (List<LookupTableEntity>)ddlSiteType.DataSource; }
            set
            {
                ddlSiteType.DataSource = value;
                ddlSiteType.DataBind();
            }
        }

        public string BOS
        {
            get { return ddlBOS.SelectedValue; }
            set { ddlBOS.SelectedValue = value; }
        }

        public bool IsMultivision
        {
            get { return Convert.ToBoolean(rblIsMultivision.SelectedValue); }
            set { rblIsMultivision.SelectedValue = value.ToString(); }
        }

        public string Address1
        {
            get { return tbAddress1.Text; }
            set { tbAddress1.Text = value; }
        }

        public string Address2
        {
            get { return tbAddress2.Text; }
            set { tbAddress2.Text = value; }
        }

        public string Address3
        {
            get { return tbAddress3.Text; }
            set { tbAddress3.Text = value; }
        }

        public string City
        {
            get { return tbCity.Text; }
            set { tbCity.Text = value; }
        }

        public string State
        {
            get { return ddlState.SelectedValue; }
            set { ddlState.SelectedValue = value; }
        }

        public string Country
        {
            get { return ddlCountry.SelectedValue; }
            set { ddlCountry.SelectedValue = value; }
        }

        public string ZipCode
        {
            get { return tbZipCode.Text; }
            set { tbZipCode.Text = value; }
        }

        public string MailAddress1
        {
            get { return tbMailAddress1.Text; }
            set { tbMailAddress1.Text = value; }
        }

        public string MailAddress2
        {
            get { return tbMailAddress2.Text; }
            set { tbMailAddress2.Text = value; }
        }

        public string MailAddress3
        {
            get { return tbMailAddress3.Text; }
            set { tbMailAddress3.Text = value; }
        }

        public string MailCity
        {
            get { return tbMailCity.Text; }
            set { tbMailCity.Text = value; }
        }

        public string MailState
        {
            get { return ddlMailState.SelectedValue; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    ddlMailState.SelectedValue = value;
                }
                else
                {
                    ddlMailState.SelectedIndex = -1;
                }
            }
        }

        public string MailCountry
        {
            get { return ddlMailCountry.SelectedValue; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    ddlMailCountry.SelectedValue = value;
                }
                else
                {
                    ddlMailCountry.SelectedIndex = -1;
                }
            }
        }

        public string MailZipCode
        {
            get { return tbMailZipCode.Text; }
            set { tbMailZipCode.Text = value; }
        }

        public bool IsConus
        {
            get { return Convert.ToBoolean(rblIsConus.SelectedValue); }
            set { rblIsConus.SelectedValue = value.ToString(); }
        }

        public bool IsConusMail
        {
            get { return Convert.ToBoolean(rblMailIsConus.SelectedValue); }
            set { rblMailIsConus.SelectedValue = value.ToString(); }
        }

        public string EMailAddress
        {
            get { return tbEmail.Text; }
            set { tbEmail.Text = value; }
        }

        public string DSNPhoneNumber
        {
            get { return tbDSNPhoneNumber.Text; }
            set { tbDSNPhoneNumber.Text = value; }
        }

        public string RegPhoneNumber
        {
            get { return tbRegPhoneNumber.Text; }
            set { tbRegPhoneNumber.Text = value; }
        }

        public int MaxEyeSize
        {
            get { return Convert.ToInt32(tbMaxEyeSize.Text); }
            set { tbMaxEyeSize.Text = value.ToString(); }
        }

        public int MaxFramesPerMonth
        {
            get { return Convert.ToInt32(tbMaxFrames.Text); }
            set { tbMaxFrames.Text = value.ToString(); }
        }

        public double MaxPower
        {
            get { return Convert.ToDouble(tbMaxPower.Text); }
            set { tbMaxPower.Text = value.ToString(); }
        }

        public bool IsActive
        {
            get { return Convert.ToBoolean(rblIsActive.SelectedValue); }
            set { rblIsActive.SelectedValue = value.ToString(); }
        }

        private List<LookupTableEntity> _stateData;

        public List<LookupTableEntity> StateData
        {
            get
            {
                return _stateData;
            }
            set
            {
                _stateData = value;
                ddlState.Items.Clear();
                ddlState.DataSource = _stateData;
                ddlState.DataTextField = "ValueTextCombo";
                ddlState.DataValueField = "Value";
                ddlState.Items.Insert(0, "-Select-");
                ddlState.SelectedIndex = 0;

                ddlMailState.Items.Clear();
                ddlMailState.DataSource = _stateData;
                ddlMailState.DataTextField = "ValueTextCombo";
                ddlMailState.DataValueField = "Value";
                ddlMailState.Items.Insert(0, "-Select-");
                ddlMailState.SelectedIndex = 0;

                ddlState.DataBind();
                ddlMailState.DataBind();
            }
        }

        private List<LookupTableEntity> _countryData;

        public List<LookupTableEntity> CountryData
        {
            get
            {
                return _countryData;
            }
            set
            {
                _countryData = value;
                ddlCountry.Items.Clear();
                ddlCountry.DataSource = _countryData;
                ddlCountry.DataTextField = "Text";
                ddlCountry.DataValueField = "Value";
                ddlCountry.Items.Insert(0, "-Select-");
                ddlCountry.SelectedIndex = 0;

                ddlMailCountry.Items.Clear();
                ddlMailCountry.DataSource = _countryData;
                ddlMailCountry.DataTextField = "Text";
                ddlMailCountry.DataValueField = "Value";
                ddlMailCountry.Items.Insert(0, "-Select-");
                ddlMailCountry.SelectedIndex = 0;

                ddlMailCountry.DataBind();
                ddlCountry.DataBind();
            }
        }

        private List<LookupTableEntity> _bosData;

        public List<LookupTableEntity> BOSData
        {
            get { return _bosData; }
            set
            {
                _bosData = value;
                ddlBOS.Items.Clear();
                ddlBOS.DataSource = _bosData;
                ddlBOS.DataTextField = "Text";
                ddlBOS.DataValueField = "Value";
                ddlBOS.Items.Insert(0, "-Select-");
                ddlBOS.SelectedIndex = 0;
                ddlBOS.DataBind();
            }
        }

        public bool IsReimbursable
        {
            get
            {
                return Convert.ToBoolean(this.rblIsReimbursable.SelectedValue);
            }
            set
            {
                this.rblIsReimbursable.SelectedValue = value.ToString();
            }
        }

        //public decimal Cylinder
        //{
        //    get { return Convert.ToDecimal(tbCyl.Text); }
        //    set { tbCyl.Text = value.ToString(); }
        //}

        //public decimal MaxPlus
        //{
        //    get { return Convert.ToDecimal(tbMaxPlus.Text); }
        //    set { tbMaxPlus.Text = value.ToString(); }
        //}

        //public decimal MaxMinus
        //{
        //    get { return Convert.ToDecimal(tbMaxMinus.Text); }
        //    set { tbMaxMinus.Text = value.ToString(); }
        //}

        //public string Material
        //{
        //    get { return ddlMatType.SelectedValue; }
        //    set
        //    {
        //        var i = this.ddlMatType.Items.IndexOf(ddlMatType.Items.FindByValue(value));
        //        if (i != -1)
        //        {
        //            ddlMatType.SelectedValue = value;
        //        }
        //    }
        //}

        //public int MatParamID
        //{
        //    get { return hfMatParamID.Value.ToInt32(); }
        //    set { hfMatParamID.Value = value.ToString(); }
        //}

        //public string IsStocked
        //{
        //    get { return ddlIsStocked.SelectedValue; }
        //    set { ddlIsStocked.SelectedValue = value; }
        //}



        #endregion Accessors

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
#if DEBUG
            using (MethodTracer.Trace(SrtsTraceSource.AdminSource, "SiteCodeEdit_btnUpdate_Click", mySession.MyUserID))
#endif
            {
                ErrMessage = string.Empty;
                try
                {
                    _presenter.DoUpdate();

                    if (!string.IsNullOrEmpty(ErrMessage))
                    {
                        ShowMessage();
                        LogEvent(String.Format("User {0} unsuccessfully updated site {1} at {2}", mySession.MyUserID, this.SiteCode, DateTime.Now));
                    }
                    else
                    {
                        LogEvent(String.Format("User {0} updated site {1} at {2}", mySession.MyUserID, this.SiteCode, DateTime.Now));
                        Response.Redirect("~/WebForms/Admin/SiteCodeManagement.aspx");
                    }
                }
                catch (Exception ex)
                {
                    ex.LogException();
                    ErrMessage = "There was an error updating this facility.";
                    ShowMessage();
                }
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/WebForms/Admin/SiteCodeManagement.aspx");
        }

        //protected void gvLabParameters_RowCommand(object sender, GridViewCommandEventArgs e)
        //{
        //    using (MethodTracer.Trace(SrtsTraceSource.AdminSource, "SiteCodeEdit_gvLabParameters_RowCommand", mySession.MyUserID))
        //    {
        //        var i = default(Int32);
        //        if (!Int32.TryParse(e.CommandArgument.ToString(), out i)) return;

        //        switch (e.CommandName.ToLower())
        //        {
        //            case "delete":
        //                var id = ((GridView)sender).DataKeys[i];
        //                _presenter.DeleteParameter(id.Value.ToInt32());
        //                LogEvent("User {0} deleted a lab parameter on {1}", new Object[] { mySession.MyUserID, DateTime.Now });
        //                break;
        //        }
        //    }
        //}

        //protected void gvLabParameters_RowDataBound(object sender, GridViewRowEventArgs e)
        //{
        //    if (e.Row.RowType != DataControlRowType.DataRow) return;

        //    e.Row.Attributes.Add("onmouseover", "this.style.backgroundColor='#CCCCCC'; this.style.cursor='pointer';");
        //    e.Row.Attributes.Add("onmouseout", "this.style.backgroundColor=''; this.style.textDecoration='none';");
        //    e.Row.ToolTip = "Click to row to edit";

        //    e.Row.Cells[0].ToolTip = "Click to delete parameter";
        //}

        //protected void gvLabParameters_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    var i = default(Int32);
        //    if (!Int32.TryParse(gvLabParameters.DataKeys[gvLabParameters.SelectedRow.RowIndex].Value.ToString(), out i)) return;

        //    GridViewRow row = gvLabParameters.SelectedRow;
        //    var mat = (Label)row.FindControl("lblMat");
        //    var stock = (Label)row.FindControl("lblIsStocked");
        //    var cyl = (Label)row.FindControl("lblCyl");
        //    var plus = (Label)row.FindControl("lblMaxPlus");
        //    var minus = (Label)row.FindControl("lblMaxMinus");

        //    this.Material = mat.Text;
        //    this.IsStocked = stock.Text == "YES" ? "TRUE" : "FALSE";
        //    this.tbCyl.Text = cyl.Text;
        //    this.tbMaxPlus.Text = plus.Text;
        //    this.tbMaxMinus.Text = minus.Text;
        //    this.btnSaveParams.Text = "Update";
        //    this.MatParamID = i;
        //}

        //protected void btnSaveParams_Click(object sender, EventArgs e)
        //{
        //    using (MethodTracer.Trace(SrtsTraceSource.AdminSource, "SiteCodeEdit_btnSaveParams_Click", mySession.MyUserID))
        //    {
        //        var m = string.Empty;

        //        if (this.btnSaveParams.Text.Equals("Update"))
        //        {
        //            m = _presenter.UpdateParameter(this.MatParamID) ? "successfully updated" : "unsuccessfully updated";
        //            this.btnSaveParams.Text = "Save";
        //        }
        //        else if (this.btnSaveParams.Text.Equals("Save"))
        //        {
        //            m = _presenter.InsertParameter() ? "successfully added" : "unsuccessfully added";
        //        }

        //        LogEvent("User {0} {1} parameter restrictions to site {2} at {3}.", new Object[] { mySession.MyUserID, m, this.SiteCode, DateTime.Now });
        //        _presenter.FillLabParameters();
        //        BindFabricationParameters();
        //        ClearInputs();
        //    }
        //}

        //protected void gvLabParameters_RowDeleting(object sender, GridViewDeleteEventArgs e)
        //{
        //    using (MethodTracer.Trace(SrtsTraceSource.AdminSource, "SiteCodeEdit_gvLabParameters_RowDeleting", mySession.MyUserID))
        //    {
        //        _presenter.FillLabParameters();
        //        BindFabricationParameters();
        //        ClearInputs();
        //        this.btnSaveParams.Text = "Save";
        //    }
        //}

        //protected void ClearInputs()
        //{
        //    ddlMatType.SelectedIndex = 0;
        //    ddlIsStocked.SelectedIndex = 0;
        //    tbCyl.Text = string.Empty;
        //    tbMaxPlus.Text = string.Empty;
        //    tbMaxMinus.Text = string.Empty;
        //}

        //protected void AddNewParameter_Click(object sender, EventArgs e)
        //{
        //    ClearInputs();
        //    ddlMatType.Focus();
        //    this.btnSaveParams.Text = "Save";
        //}

        //protected void ddlIsStocked_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    if (IsStocked == "FALSE")
        //    {
        //        tbCyl.Text = "0.00";
        //        tbMaxPlus.Text = "0.00";
        //        tbMaxMinus.Text = "0.00";
        //    }
        //}

        //public Dictionary<string, string> LensMaterial
        //{
        //    set
        //    {
        //        this.ddlMatType.DataSource = value;
        //        this.ddlMatType.DataTextField = "Value";
        //        this.ddlMatType.DataValueField = "Value";
        //        this.ddlMatType.DataBind();
        //        this.ddlMatType.Items.Insert(0, new ListItem("-Select-", "X"));
        //    }
        //}
    }
}