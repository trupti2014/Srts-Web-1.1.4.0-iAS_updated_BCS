using SrtsWeb.Entities;
using System;
using System.Collections.Generic;
using System.Data;

namespace SrtsWeb.BusinessLayer.Views.Admin
{
    public interface ICMSContentMessageView
    {
        SRTSSession mySession { get; set; }

        List<CmsSite> FacilityClinic { set; }

        String SelectedFacilityClinicId { get; set; }

        List<CmsSite> FacilityLab { set; }

        String SelectedFacilityLabId { get; set; }

        String Message { get; set; }

        Int32 CurrentContentAuthorId { get; }

        String SelectedRecipientSiteId { get; }

        List<CMSEntity> ContentType { set; }

        String SelectedContentTypeId { get; set; }

        List<CMSEntity> ContentAuthors { set; }

        Int32 SelectedContentAuthorId { get; set; }

        List<CMSEntity> ContentRecipientType { set; }

        String SelectedContentRecipientTypeId { get; set; }

        List<CMSEntity> RecipientGroupType { set; }

        String SelectedRecipientGroupTypeId { get; set; }

        Int32 ContentId { get; set; }

        Int32 ContentRecipientIndividualId { get; set; }

        string ContentTitle { get; set; }

        string ContentDescription { get; set; }

        DateTime ContentStartDate { get; set; }

        DateTime ContentExpirationDate { get; set; }
    }
}