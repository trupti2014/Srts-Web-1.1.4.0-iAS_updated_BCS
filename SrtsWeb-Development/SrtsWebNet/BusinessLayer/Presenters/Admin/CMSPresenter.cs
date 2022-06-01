using SrtsWeb.ExtendersHelpers;
using SrtsWeb.Views.Admin;
using SrtsWeb.DataLayer.Repositories;
using SrtsWeb.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace SrtsWeb.BusinessLayer.Presenters.Admin
{
    public sealed class CMSPresenter
    {
        private ICMSContentMessageView _view;

        public CMSPresenter(ICMSContentMessageView view)
        {
            _view = view;
        }

        public void InitView()
        {
            HttpContext.Current.Session.Add("action", "add");

            FillSelectLists();

            _view.ContentDescription = String.Empty;
            _view.ContentTitle = String.Empty;
        }

        public void InitView(Int32 contentId)
        {
            HttpContext.Current.Session.Add("action", "update");

            FillSelectLists();
            FillForm(contentId);
        }

        public void AddNewContent()
        {
            var c = new CMSEntity();
            var v = this._view;
            var _cmsRepository = new CMSRepository.CmsEntityRepository();

            c.cmsContentTitle = v.ContentTitle;
            c.cmsContentBody = v.ContentDescription;
            c.cmsContentTypeID = v.SelectedContentTypeId;
            c.cmsContentAuthorID = v.CurrentContentAuthorId;
            c.cmsContentRecipientTypeID = v.SelectedContentRecipientTypeId.Equals("0") ? "R001" : v.SelectedContentRecipientTypeId;
            c.cmsContentRecipientIndividualID = 0;
            c.cmsContentRecipientSiteID = v.SelectedRecipientSiteId;
            c.cmsContentRecipientGroupID = v.SelectedRecipientGroupTypeId;
            c.cmsContentDisplayDate = v.ContentStartDate;
            c.cmsContentExpireDate = v.ContentExpirationDate;

            _cmsRepository.InsertMessage(c);

            ResetView();
        }

        public void UpdateExistingContent()
        {
            var c = new CMSEntity();
            var v = this._view;
            var _cmsRepository = new CMSRepository.CmsEntityRepository();

            c.cmsContentID = v.ContentId;
            c.cmsContentTitle = v.ContentTitle;
            c.cmsContentBody = v.ContentDescription;
            c.cmsContentTypeID = v.SelectedContentTypeId;
            c.cmsContentAuthorID = v.CurrentContentAuthorId;
            c.cmsContentRecipientTypeID = v.SelectedContentRecipientTypeId;
            c.cmsContentRecipientIndividualID = 0;
            c.cmsContentRecipientSiteID = v.SelectedRecipientSiteId;
            c.cmsContentRecipientGroupID = v.SelectedRecipientGroupTypeId;
            c.cmsContentDisplayDate = v.ContentStartDate;
            c.cmsContentExpireDate = v.ContentExpirationDate;

            _cmsRepository.UpdateMessage(c);

            ResetView();
        }

        private void FillSelectLists()
        {
            var _cmsRepository = new CMSRepository.CmsEntityRepository();

            var ct = _cmsRepository.GetCMS_ContentTypes();

            if (!Roles.GetRolesForUser().Contains("MgmtEnterprise"))
            {
                var idx = new List<int>();
                var i = 0;
                foreach (var r in ct)
                {
                    if (r.cmsContentTypeName.Equals("Alert") || r.cmsContentTypeName.Equals("Public Announcement"))
                        idx.Add(i);
                    i++;
                }

                for (var j = 0; j < idx.Count; j++)
                    ct.RemoveAt(idx[j]);

                _view.ContentType = ct;
            }
            else
                _view.ContentType = ct;

            _view.ContentRecipientType = _cmsRepository.GetCMS_RecipientTypes();

            _view.RecipientGroupType = _cmsRepository.GetCMS_RecipientGroupTypes();

            var sr = new CMSRepository.CmsSiteRepository();
            var sl = sr.GetCMS_UserFacilities(_view.mySession.MySite.SiteCode);
            _view.FacilityClinic = sl;
            _view.FacilityLab = sl;

            _view.ContentStartDate = DateTime.MinValue;
            _view.ContentExpirationDate = DateTime.MinValue;
        }

        private void FillForm(Int32 contentId)
        {
            var e = GetEntityData(contentId);

            var v = this._view;

            if (e == null)
            {
                v.Message = "There was an error in the message and cannot be displayed.";
                return;
            }

            v.ContentId = contentId;

            v.SelectedContentAuthorId = e.cmsContentAuthorID;
            v.SelectedContentRecipientTypeId = String.IsNullOrEmpty(e.cmsContentRecipientTypeID) ? "0" : e.cmsContentRecipientTypeID;
            v.SelectedContentTypeId = String.IsNullOrEmpty(e.cmsContentTypeID) ? "0" : e.cmsContentTypeID;

            if (e.cmsContentRecipientTypeID.Equals("R002"))
                v.SelectedFacilityClinicId = String.IsNullOrEmpty(e.cmsContentRecipientSiteID) ?
                "0" : e.cmsContentRecipientSiteID;
            else if (e.cmsContentRecipientTypeID.Equals("R003"))
                v.SelectedFacilityLabId = String.IsNullOrEmpty(e.cmsContentRecipientSiteID) ?
                "0" : e.cmsContentRecipientSiteID;
            v.SelectedRecipientGroupTypeId = String.IsNullOrEmpty(e.cmsContentRecipientGroupID) ?
                "0" : e.cmsContentRecipientGroupID;

            v.ContentDescription = e.cmsContentBody;
            v.ContentTitle = e.cmsContentTitle;

            v.ContentStartDate = e.cmsContentDisplayDate;
            v.ContentExpirationDate = e.cmsContentExpireDate;
        }

        private CMSEntity GetEntityData(Int32 contentId)
        {
            var _cmsRepository = new CMSRepository.CmsEntityRepository();

            var l = _cmsRepository.GetMessagesByContentID(contentId);

            if (l.IsNull()) return null;

            return l[0];
        }

        private void ResetView()
        {
            this._view = this._view.ResetViewProperties();
            InitView();
        }
    }
}