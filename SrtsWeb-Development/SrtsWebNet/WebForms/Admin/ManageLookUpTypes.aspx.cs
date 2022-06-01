using BarcodeLib;
using SrtsWeb.Base;
using SrtsWeb.BusinessLayer.Concrete;
using SrtsWeb.Entities;
using SrtsWeb.ExtendersHelpers;
using SrtsWeb.Presenters.Admin;
using SrtsWeb.Views.Admin;
using System;
using System.Collections.Generic;
using System.Security.Permissions;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SrtsWeb.Admin
{
    [PrincipalPermission(SecurityAction.Demand, Role = "MgmtDataMgmt")]
    [PrincipalPermission(SecurityAction.Demand, Role = "MgmtEnterprise")]
    public partial class ManageLookUpTypes : PageBase, IManageLookUpTypesView
    {
        private ManageLookUpTypesPresenter _presenter;

        public ManageLookUpTypes()
        {
            _presenter = new ManageLookUpTypesPresenter(this);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
#if DEBUG
            using (MethodTracer.Trace(SrtsTraceSource.AdminSource, "ManageLookUpTypes_Page_Load", mySession.MyUserID))
#endif
            {
                if (!IsPostBack)
                {
                    _presenter.InitView();
                    ddlLookUpTable.SelectedIndex = 0;
                    if (Roles.IsUserInRole("MgmtEnterprise"))
                    {
                        pnlGenBarcodes.Visible = true;
                    }
                }
            }
        }
        private void LoadLookupTable()
        {
            SrtsWeb.BusinessLayer.Abstract.ILookupService _service = new LookupService();
            Cache["SRTSLOOKUP"] = _service.GetAllLookups();
        }

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

        protected void ddlLookUpTable_SelectedIndexChanged(object sender, EventArgs e)
        {
#if DEBUG
            using (MethodTracer.Trace(SrtsTraceSource.AdminSource, "ManageLookUpTypes_ddlLookUpTable_SelectedIndexChanged", mySession.MyUserID))
#endif
            {
                if (ddlLookUpTable.SelectedIndex > 0)
                {
                    _presenter.LoadSelectedTypes();
                }
            }
        }

        protected void grdLookUpTables_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("Insert"))
            {
#if DEBUG
                using (MethodTracer.Trace(SrtsTraceSource.AdminSource, "ManageLookUpTypes_grdLookUpTables_RowCommand", mySession.MyUserID))
#endif
                {
                    GridViewRow gvr = (GridViewRow)(((LinkButton)e.CommandSource).NamingContainer);

                    CodeInput = ((TextBox)gvr.FindControl("txtLookUpTableCode")).Text;
                    TextInput = ((TextBox)gvr.FindControl("txtLookUpTableName")).Text;
                    ValueInput = ((TextBox)gvr.FindControl("txtLookUpTableValue")).Text;
                    DescriptionInput = ((TextBox)gvr.FindControl("txtLookUpTableDescription")).Text;
                    IsActiveInput = Convert.ToBoolean(((RadioButtonList)gvr.FindControl("rblIsActive")).SelectedValue);
                    CustomEventTracer.TraceVerbose(SrtsTraceSource.AdminSource, "grdLookUpTables_RowCommand", mySession.MyUserID, "Insert Lookup.");
                    _presenter.InsertLookup();
                    LogEvent("User {0} added new lookup value for text: {1} on {2}", new Object[] { mySession.MyUserID, TextInput, DateTime.Now });
                    grdLookUpTables.EditIndex = -1;
                    _presenter.LoadSelectedTypes();
                }
            }
        }

        protected void grdLookUpTables_RowEditing(object sender, GridViewEditEventArgs e)
        {
#if DEBUG
            using (MethodTracer.Trace(SrtsTraceSource.AdminSource, "ManageLookUpTypes_grdLookUpTables_RowEditing", mySession.MyUserID))
#endif
            {
                grdLookUpTables.EditIndex = e.NewEditIndex;
                _presenter.LoadSelectedTypes();
            }
        }

        protected void grdLookUpTables_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
#if DEBUG
            using (MethodTracer.Trace(SrtsTraceSource.AdminSource, "ManageLookUpTypes_grdLookUpTables_RowCancelingEdit", mySession.MyUserID))
#endif
            {
                grdLookUpTables.EditIndex = -1;
                _presenter.LoadSelectedTypes();
            }
        }

        protected void grdLookUpTables_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
#if DEBUG
            using (MethodTracer.Trace(SrtsTraceSource.AdminSource, "ManageLookUpTypes_grdLookUpTables_RowUpdating", mySession.MyUserID))
#endif
            {
                IDInput = (Int32)grdLookUpTables.DataKeys[e.RowIndex].Values[0];
                CodeInput = Convert.ToString(((TextBox)grdLookUpTables.Rows[e.RowIndex].FindControl("txtLookUpTableCode")).Text);
                TextInput = Convert.ToString(((TextBox)grdLookUpTables.Rows[e.RowIndex].FindControl("txtLookUpTableName")).Text);
                ValueInput = Convert.ToString(((TextBox)grdLookUpTables.Rows[e.RowIndex].FindControl("txtLookUpTableValue")).Text); ;
                DescriptionInput = Convert.ToString(((TextBox)grdLookUpTables.Rows[e.RowIndex].FindControl("txtLookUpTableDescription")).Text);
                IsActiveInput = Convert.ToBoolean(((RadioButtonList)grdLookUpTables.Rows[e.RowIndex].FindControl("rblIsActive")).SelectedValue);
                CustomEventTracer.TraceVerbose(SrtsTraceSource.AdminSource, "grdLookUpTables_RowCommand", mySession.MyUserID, "Update Lookup.");
                _presenter.UpdateLookup();
                LogEvent("User {0} updated lookup value for text: {1} on {2}", new Object[] { mySession.MyUserID, TextInput, DateTime.Now });
                grdLookUpTables.EditIndex = -1;
                _presenter.LoadSelectedTypes();
            }
        }

        protected void grdLookUpTables_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
#if DEBUG
            using (MethodTracer.Trace(SrtsTraceSource.AdminSource, "ManageLookUpTypes_grdLookUpTables_RowDeleting", mySession.MyUserID))
#endif
            {
                IDInput = (Int32)grdLookUpTables.DataKeys[e.RowIndex].Values[0];
                GridViewRow row = grdLookUpTables.Rows[e.RowIndex];

                CodeInput = ((Label)row.FindControl("lblCode")).Text;
                TextInput = ((Label)row.FindControl("lblText")).Text;
                ValueInput = ((Label)row.FindControl("lblValue")).Text;
                DescriptionInput = ((Label)row.FindControl("lblDescription")).Text;
                IsActiveInput = false;
                CustomEventTracer.TraceVerbose(SrtsTraceSource.AdminSource, "grdLookUpTables_RowCommand", mySession.MyUserID, "Update Lookup.");
                _presenter.UpdateLookup();
                LogEvent("User {0} deleted lookup value for text: {1} on {2}", new Object[] { mySession.MyUserID, TextInput, DateTime.Now });
                grdLookUpTables.EditIndex = -1;
                _presenter.LoadSelectedTypes();
            }
        }

        protected void btnGenBarcodes_Click(object sender, EventArgs e)
        {
#if DEBUG
            using (MethodTracer.Trace(SrtsTraceSource.AdminSource, "ManageLookUpTypes_btnGenBarcodes_Click", mySession.MyUserID))
#endif
            {
                int CountAffectedRows = _presenter.GenerateLegacyOrderBarcodes(new GenerateBarCodes(new Barcode()));
                tbCountAffectedRows.Text = CountAffectedRows.ToString() + " Rows Updated";
                LogEvent(String.Format("User {0} generated barcodes for {1} order(s) at {2}.", mySession.MyUserID, CountAffectedRows, DateTime.Now));
            }
        }

        protected void btnResetPw_Click(object sender, EventArgs e)
        {
#if DEBUG
            using (MethodTracer.Trace(SrtsTraceSource.AdminSource, "ManageLookUpTypes_btnResetPw_Click", mySession.MyUserID))
#endif
            {
                var good = true;
                var msg = new System.Text.StringBuilder();

                if (String.IsNullOrEmpty(this.tbUserName.Text))
                {
                    good = false;
                    msg.AppendLine("User name is a required field.");
                }
                if (String.IsNullOrEmpty(this.tbPassword.Text))
                {
                    good = false;
                    msg.AppendLine("Password is a required field.");
                }

                if (!good)
                {
                    this.lblPwdError.Text = msg.ToString().Replace(Environment.NewLine, "<br />");
                    return;
                }

                var m = new CustomProviders.SrtsMembership();

                bool pwdUpdated = m.AdminSetPassword(tbUserName.Text, tbPassword.Text);

                if (!pwdUpdated)
                {
                    this.lblPwdError.Text = Server.HtmlEncode(String.Format("An error occured while attempting to reset password for {0}.", this.tbUserName.Text));
                    LogEvent("User {0} recieved an error attempting to reset a password for {1} at {2}.", new Object[] { mySession.MyUserID, tbUserName.Text, DateTime.Now });
                    this.tbPassword.Text = String.Empty;
                }
                else
                {
                    this.lblPwdError.Text = String.Empty;
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "", "alert('Users password reset successfully!');", true);
                    tbUserName.Text = string.Empty;
                    tbPassword.Text = string.Empty;
                    LogEvent(String.Format("User {0} reset the password for {1} at {2}.", mySession.MyUserID, tbUserName.Text, DateTime.Now));
                }
            }
        }

        #region Accessors

        public Dictionary<string, string> LookupTypes
        {
            get { return (Dictionary<string, string>)ddlLookUpTable.DataSource; }
            set
            {
                ddlLookUpTable.Items.Clear();
                ddlLookUpTable.DataSource = value;
                ddlLookUpTable.DataTextField = "Value";
                ddlLookUpTable.DataValueField = "Key";
                ddlLookUpTable.DataBind();
                ddlLookUpTable.Items.Insert(0, "-- select a lookup type to manage --");
            }
        }

        public string SelectedType
        {
            get { return ddlLookUpTable.SelectedValue; }
            set { ddlLookUpTable.SelectedValue = value; }
        }

        public List<LookupTableEntity> LookupsBind
        {
            get { return grdLookUpTables.DataSource as List<LookupTableEntity>; }
            set
            {
                grdLookUpTables.DataSource = value;
                grdLookUpTables.DataBind();
            }
        }

        private string _codeInput;

        public string CodeInput
        {
            get { return _codeInput; }
            set { _codeInput = value; }
        }

        private string _textInput;

        public string TextInput
        {
            get { return _textInput; }
            set { _textInput = value; }
        }

        private string _valueInput;

        public string ValueInput
        {
            get { return _valueInput; }
            set { _valueInput = value; }
        }

        public string _descriptionInput;

        public string DescriptionInput
        {
            get { return _descriptionInput; }
            set { _descriptionInput = value; }
        }

        private bool _isActiveInput;

        public bool IsActiveInput
        {
            get { return _isActiveInput; }
            set { _isActiveInput = value; }
        }

        private int _idInput;

        public int IDInput
        {
            get { return _idInput; }
            set { _idInput = value; }
        }

        #endregion Accessors
    }
}