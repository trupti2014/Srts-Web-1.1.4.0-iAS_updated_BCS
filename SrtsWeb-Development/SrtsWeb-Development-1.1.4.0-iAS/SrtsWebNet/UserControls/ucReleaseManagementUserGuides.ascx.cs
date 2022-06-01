using SrtsWeb.Base;
using SrtsWeb.Entities;
using SrtsWeb.BusinessLayer.Concrete;
using SrtsWeb.ExtendersHelpers;
using SrtsWeb.Presenters.Admin;
using SrtsWeb.Views.Admin;
using System.IO;
using System;
using System.Collections.Generic;
using System.Security.Permissions;
using System.Web;
using System.Web.UI.WebControls;
using System.Linq;
using System.Web.UI;
using System.Web.Services;

namespace SrtsWeb.UserControls
{
    [PrincipalPermission(SecurityAction.Demand, Role = "MgmtEnterprise")]
    [PrincipalPermission(SecurityAction.Demand, Role = "MgmtAdmin")]


    public partial class ucReleaseManagementUserGuides : UserControlBase, IReleaseManagementUserGuidesView
    {
        private ReleaseManagementPresenter.UserGuidesPresenter p;

        public ucReleaseManagementUserGuides()
        {
            p = new ReleaseManagementPresenter.UserGuidesPresenter(this);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.mySession.MyIndividualID = SrtsWeb.Account.CustomProfile.GetProfile(HttpContext.Current.User.Identity.Name).IndividualId;
            if (!IsPostBack)
            {
                this.p = new ReleaseManagementPresenter.UserGuidesPresenter(this);
                this.p.InitView();
                //BindUserGuideData();
            }
            BindUserGuideData();
        }



        protected void customValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if ( fuUploadUserGuide.PostedFile.ContentLength > 67108864 )  //Check to see if file is more than 64 MB
            {   
                customValidator1.ErrorMessage = "File exceeds the 64 MB size limit.";   
                args.IsValid = false;   
            } 
            else
            {
                args.IsValid = true;
            }
        }   

        protected void btnUploadUserGuide_Click(object sender, EventArgs e)
        {
            if (this.ReleaseManagementUserGuideEntity != null  && Page.IsValid )
            {
                var m = string.Empty;

                if (!p.InsertUpdateUserGuide(this.ReleaseManagementUserGuideEntity))
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "", "alert('Error uploading user guide!');", true);
                    return;
                }
                p.FillUserGuides();
                BindUserGuideData();
            }

        }

        protected void gvUploadedUserGuides_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow) return;

            LinkButton lbn = e.Row.FindControl("lnkUserGuideName") as LinkButton;
            ScriptManager.GetCurrent(this.Page).RegisterPostBackControl(lbn);
        }

        protected void gvUploadedUserGuides_RowCommand(object sender, GridViewCommandEventArgs e)
        {
#if DEBUG
            using (MethodTracer.Trace(SrtsTraceSource.AdminSource, "ReleaseManagmentUserGuides_gvUploadedUserGuides_RowCommand", mySession.MyUserID))
#endif
            {
                switch (e.CommandName.ToLower())
                {
                    
                    case "delete":
                        var i = default(Int32);
                        if (!Int32.TryParse(e.CommandArgument.ToString(), out i)) return;
                        var guide = ((GridView)sender).DataKeys[i];
                        p.DeleteUserGuide(guide.Value.ToString());
                    break;

                    case "downloadfile":
                        string userguideName = e.CommandArgument.ToString();
                        var d = p.GetUserGuide(userguideName.ToString());
                        if (d != null)
                        {
                            Response.ClearHeaders();
                            Response.ContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                            Response.AppendHeader("Content-Disposition", "attachment; filename=" + userguideName.ToString());
                            Response.Clear();
                            Response.BinaryWrite(d.UserGuideDocument);
                            Response.End();
                        }
                        else
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "", "alert('This user guide is not available!');", true);
                            return;
                        }
                    break;
                }

            }
            
        }

        protected void gvUploadedUserGuides_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
#if DEBUG
            using (MethodTracer.Trace(SrtsTraceSource.AdminSource, "ReleaseManagmentUserGuides_gvUploadedUserGuides_RowDeleting", mySession.MyUserID))
#endif
            {
                p.FillUserGuides();
                BindUserGuideData();
            }
        }

        public static List<string> CheckUserGuideExists()
        {
            return ReleaseManagementPresenter.UserGuidesPresenter.GetAllGuides().Select(x => x.GuideName).ToList();
        }

        //#region INTERFACES

        public List<ReleaseManagementUserGuideEntity> UserGuideData
        {
            get {
                return (List<ReleaseManagementUserGuideEntity>)Session["UserGuideData"];
                
            }
            set { Session.Add("UserGuideData", value); }
        }

        private void BindUserGuideData()
        {
            gvUploadedUserGuides.DataKeyNames = new[] { "GuideName" };
            gvUploadedUserGuides.DataSource = UserGuideData;
            gvUploadedUserGuides.DataBind();
        }


        private ReleaseManagementUserGuideEntity _ReleaseManagementUserGuideEntity;
        public ReleaseManagementUserGuideEntity ReleaseManagementUserGuideEntity
        {
            get
            {
                this._ReleaseManagementUserGuideEntity = Session["ReleaseManagementUserGuide"] as ReleaseManagementUserGuideEntity;
                this._ReleaseManagementUserGuideEntity = new ReleaseManagementUserGuideEntity();

                if (fuUploadUserGuide.HasFile)
                {
                      this._ReleaseManagementUserGuideEntity.GuideName = Path.GetFileName(fuUploadUserGuide.PostedFile.FileName);
                      byte[] fileBytes = fuUploadUserGuide.FileBytes;
                      this._ReleaseManagementUserGuideEntity.UserGuideDocument = fileBytes;
                      this._ReleaseManagementUserGuideEntity.ModifiedBy = this.mySession.ModifiedBy;
                }

                this.ReleaseManagementUserGuideEntity = _ReleaseManagementUserGuideEntity;

                return this._ReleaseManagementUserGuideEntity;
            }
            set
            {
               Session.Add("ReleaseManagementUserGuide", value);
            }

        }



    }
}