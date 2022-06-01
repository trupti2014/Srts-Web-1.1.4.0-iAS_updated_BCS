using System;

namespace SrtsWeb.DataLayer.Interfaces
{
    internal interface ICmsEntityRepository
    {
        void DeleteMessage(int contentId);

        global::System.Collections.Generic.List<global::SrtsWeb.Entities.CMSEntity> GetCMS_ContentTypes();

        global::System.Collections.Generic.List<global::SrtsWeb.Entities.CMSEntity> GetCms_MessageCenter_ApplicationAnnouncement_Content(string contentTypeId, string siteId = default(String), string siteTypeId = default (String), int individualId = default(Int32), string groupId = default(String), string mySiteId = default(String));

        global::System.Collections.Generic.List<global::SrtsWeb.Entities.CMSEntity> GetCMS_RecipientGroupTypes();

        global::System.Collections.Generic.List<global::SrtsWeb.Entities.CMSEntity> GetCMS_RecipientTypes();

        global::System.Collections.Generic.List<global::SrtsWeb.Entities.CMSEntity> GetMessageByContentTypeID(string contentTypeID);

        global::System.Collections.Generic.List<global::SrtsWeb.Entities.CMSEntity> GetMessagesByContentID(int contentID);

        void InsertMessage(global::SrtsWeb.Entities.CMSEntity entity);

        void UpdateMessage(global::SrtsWeb.Entities.CMSEntity entity);
    }

    internal interface ICmsMessageRepository
    {
        System.Collections.Generic.List<SrtsWeb.Entities.CmsMessage> GetCMS_ContentByAuthorId(int authorId);

        System.Collections.Generic.List<SrtsWeb.Entities.CmsMessage> GetCms_IndividualsMessagesById(int individualId);

        System.Collections.Generic.List<SrtsWeb.Entities.CmsMessage> GetCms_MessageCenter_ApplicationAnnouncement_Content(string contentTypeId, string siteId = default(String), string siteTypeId = default (String), int individualId = default(Int32), string groupId = default(String), string mySiteId = default(String));

        System.Collections.Generic.List<SrtsWeb.Entities.CmsMessage> GetMessageByContentTypeID(string contentTypeID);

        System.Collections.Generic.List<SrtsWeb.Entities.CmsMessage> GetMessagesByContentID(int contentID);
    }

    internal interface ICmsSiteRepository
    {
        System.Collections.Generic.List<SrtsWeb.Entities.CmsSite> GetCMS_UserFacilities(string siteCode);
    }
}