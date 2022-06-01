using DataBaseAccessLayer;
using SrtsWeb.DataLayer.Interfaces;
using SrtsWeb.DataLayer.RepositoryBase;
using SrtsWeb.Entities;
using SrtsWeb.ExtendersHelpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace SrtsWeb.DataLayer.Repositories
{
    public sealed class CMSRepository
    {
        /// <summary>
        /// A custom repository class used to perform CMS message operations.
        /// </summary>
        public sealed class CmsMessageRepository : RepositoryBase<CmsMessage>, ICmsMessageRepository
        {
            /// <summary>
            /// Default ctor.
            /// </summary>
            public CmsMessageRepository()
                : base(DbFactory.GetDbObject(DataBaseType.SQL, Globals.ConnStrNm))
            {
            }

            /// <summary>
            /// Ctor.
            /// </summary>
            /// <param name="CatalogName"></param>
            public CmsMessageRepository(String CatalogName)
                : base(DbFactory.GetDbObject(DataBaseType.SQL, Globals.DefaultConnStrNm), CatalogName)
            { }

            /// <summary>
            /// Gets a list of messages based on content ID.
            /// </summary>
            /// <param name="contentID">Content ID to search with.</param>
            /// <returns>CmsMessage list of messages.</returns>
            public List<CmsMessage> GetMessagesByContentID(int contentID)
            {
                var cmd = this.DAL.GetCommandObject();

                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.CommandText = "GetCMSMessageByContentID";
                cmd.Parameters.Add(this.DAL.GetParamenter("@ContentID", contentID));

                return GetRecords(cmd).ToList();
            }

            /// <summary>
            /// Gets a list of messages by content type id.
            /// </summary>
            /// <param name="contentTypeID">Content type ID to search with.</param>
            /// <returns>CmsMessage list of messages.</returns>
            public List<CmsMessage> GetMessageByContentTypeID(string contentTypeID)
            {
                var cmd = this.DAL.GetCommandObject();

                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.CommandText = "GetCMSMessageByContentTypeID";
                cmd.Parameters.Add(this.DAL.GetParamenter("@ContentTypeID", contentTypeID));

                return GetRecords(cmd).ToList();
            }

            /// <summary>
            /// Gets a list of messages for either the Message Center, or Applications Announcements sections of the application.
            /// </summary>
            /// <param name="contentTypeId">Content type ID to search with.</param>
            /// <param name="siteId">Site code where messages can belong.</param>
            /// <param name="siteTypeId">Site type ID (clinic/lab) where messages can belong.</param>
            /// <param name="individualId">Individual ID of person who created the messages.</param>
            /// <param name="groupId">Group ID where messages can belong.</param>
            /// <param name="mySiteId">Site code of user performing operation.</param>
            /// <returns>CmsMessage list of messages.</returns>
            public List<CmsMessage> GetCms_MessageCenter_ApplicationAnnouncement_Content(
                String contentTypeId,
                string siteId = default(String),
                string siteTypeId = default (String),
                int individualId = default(Int32),
                string groupId = default(String),
                string mySiteId = default(String))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandText = "GetCms_MessageCenter_ApplicationAnnouncement_Content";
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add(this.DAL.GetParamenter("@contentTypeId", contentTypeId));

                    if (siteId != null && !siteId.Trim().Equals(default(String)))
                        cmd.Parameters.Add(this.DAL.GetParamenter("@siteId", siteId));

                    if (siteTypeId != null && !siteTypeId.Trim().Equals(default(String)))
                        cmd.Parameters.Add(this.DAL.GetParamenter("@siteTypeId", siteTypeId));

                    if (!individualId.Equals(default(Int32)))
                        cmd.Parameters.Add(this.DAL.GetParamenter("@individualId", individualId));

                    if (groupId != null && !groupId.Trim().Equals(default(String)))
                        cmd.Parameters.Add(this.DAL.GetParamenter("@groupId", groupId));

                    if (!String.IsNullOrEmpty(mySiteId))
                        cmd.Parameters.Add(this.DAL.GetParamenter("@SiteCode", mySiteId));

                    return GetRecords(cmd).ToList();
                }
            }

            /// <summary>
            /// Gets a list of messges by an individual.
            /// </summary>
            /// <param name="individualId">Individual ID number to search with.</param>
            /// <returns>CmsMessage list of messages.</returns>
            public List<CmsMessage> GetCms_IndividualsMessagesById(Int32 individualId)
            {
                var cmd = this.DAL.GetCommandObject();

                cmd.CommandText = "GetCms_IndividualsMessagesById";
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add(this.DAL.GetParamenter("@individualId", individualId));

                return GetRecords(cmd).ToList();
            }

            /// <summary>
            /// Gets a list of messages based on an authorId.
            /// </summary>
            /// <param name="authorId">ID number used to search with.</param>
            /// <returns>CmsMessage list of messages.</returns>
            public List<CmsMessage> GetCMS_ContentByAuthorId(Int32 authorId)
            {
                var cmd = this.DAL.GetCommandObject();

                cmd.CommandText = "GetCMS_ContentByAuthorId";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(this.DAL.GetParamenter("@authorId", authorId));

                return GetRecords(cmd).ToList();
            }

            protected override CmsMessage FillRecord(IDataReader dr)
            {
                var c = dr.GetColumnNameList();
                return new CmsMessage()
                {
                    cmsContentId = dr.ToInt32("cmsContentID", c),
                    cmsContentTitle = dr.AsString("cmsContentTitle", c),
                    cmsContentBody = dr.AsString("cmsContentBody", c)
                };
            }
        }

        /// <summary>
        /// A custom repository class to handle CMS message operations.
        /// </summary>
        public sealed class CmsEntityRepository : RepositoryBase<CMSEntity>, ICmsEntityRepository
        {
            /// <summary>
            /// Default ctor.
            /// </summary>
            public CmsEntityRepository()
                : base(DbFactory.GetDbObject(DataBaseType.SQL, Globals.ConnStrNm))
            {
            }

            /// <summary>
            /// Ctor.
            /// </summary>
            /// <param name="CatalogName"></param>
            public CmsEntityRepository(String CatalogName)
                : base(DbFactory.GetDbObject(DataBaseType.SQL, Globals.DefaultConnStrNm), CatalogName)
            { }

            /// <summary>
            /// Gets a list of messages based on content ID.
            /// </summary>
            /// <param name="contentID">Content ID to search with.</param>
            /// <returns>CMSEntity list of messages.</returns>
            public List<CMSEntity> GetMessagesByContentID(int contentID)
            {
                var cmd = this.DAL.GetCommandObject();

                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.CommandText = "GetCMSMessageByContentID";
                cmd.Parameters.Add(this.DAL.GetParamenter("@ContentID", contentID));

                return GetRecords(cmd).ToList();
            }

            /// <summary>
            /// Gets a list of messages by content type id.
            /// </summary>
            /// <param name="contentTypeID">Content type ID to search with.</param>
            /// <returns>CMSEntity list of messages.</returns>
            public List<CMSEntity> GetMessageByContentTypeID(string contentTypeID)
            {
                var cmd = this.DAL.GetCommandObject();

                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.CommandText = "GetCMSMessageByContentTypeID";
                cmd.Parameters.Add(this.DAL.GetParamenter("@ContentTypeID", contentTypeID));

                return GetRecords(cmd).ToList();
            }

            /// <summary>
            /// Gets a list of messages for either the Message Center, or Applications Announcements sections of the application.
            /// </summary>
            /// <param name="contentTypeId">Content type ID to search with.</param>
            /// <param name="siteId">Site code where messages can belong.</param>
            /// <param name="siteTypeId">Site type ID (clinic/lab) where messages can belong.</param>
            /// <param name="individualId">Individual ID of person who created the messages.</param>
            /// <param name="groupId">Group ID where messages can belong.</param>
            /// <param name="mySiteId">Site code of user performing operation.</param>
            /// <returns>CMSEntity list of messages.</returns>
            public List<CMSEntity> GetCms_MessageCenter_ApplicationAnnouncement_Content(
                String contentTypeId,
                string siteId = default(String),
                string siteTypeId = default (String),
                int individualId = default(Int32),
                string groupId = default(String),
                string mySiteId = default(String))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandText = "GetCms_MessageCenter_ApplicationAnnouncement_Content";
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add(this.DAL.GetParamenter("@contentTypeId", contentTypeId));

                    if (siteId != null && !siteId.Trim().Equals(default(String)))
                        cmd.Parameters.Add(this.DAL.GetParamenter("@siteId", siteId));

                    if (siteTypeId != null && !siteTypeId.Trim().Equals(default(String)))
                        cmd.Parameters.Add(this.DAL.GetParamenter("@siteTypeId", siteTypeId));

                    if (!individualId.Equals(default(Int32)))
                        cmd.Parameters.Add(this.DAL.GetParamenter("@individualId", individualId));

                    if (groupId != null && !groupId.Trim().Equals(default(String)))
                        cmd.Parameters.Add(this.DAL.GetParamenter("@groupId", groupId));

                    if (!String.IsNullOrEmpty(mySiteId))
                        cmd.Parameters.Add(this.DAL.GetParamenter("@SiteCode", mySiteId));

                    return GetRecords(cmd).ToList();
                }
            }

            /// <summary>
            /// Gets a list of CMS content types.
            /// </summary>
            /// <returns>CMSEntity list of content types.</returns>
            public List<CMSEntity> GetCMS_ContentTypes()
            {
                var cmd = this.DAL.GetCommandObject();
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.CommandText = "GetCMS_ContentTypes";
                return GetRecords(cmd).ToList();
            }

            /// <summary>
            /// Gets a list of recipient types.
            /// </summary>
            /// <returns>CMSEntity list of recipient types.</returns>
            public List<CMSEntity> GetCMS_RecipientTypes()
            {
                var cmd = this.DAL.GetCommandObject();
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.CommandText = "GetCMS_RecipientTypes";
                return GetRecords(cmd).ToList();
            }

            /// <summary>
            /// Geets a list of recipient group types.
            /// </summary>
            /// <returns>CMSEntity list of recipient group types.</returns>
            public List<CMSEntity> GetCMS_RecipientGroupTypes()
            {
                var cmd = this.DAL.GetCommandObject();
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.CommandText = "GetCMS_RecipientGroupTypes";
                return GetRecords(cmd).ToList();
            }

            /// <summary>
            /// Adds a new CMS message to the database.
            /// </summary>
            /// <param name="entity">CMSEntity that contains the message to be added.</param>
            public void InsertMessage(CMSEntity entity)
            {
                var cmd = this.DAL.GetCommandObject();

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "CMSInsertMessage";
                cmd.Parameters.Add(this.DAL.GetParamenter("@ContentTitle", entity.cmsContentTitle));
                cmd.Parameters.Add(this.DAL.GetParamenter("@ContentBody", entity.cmsContentBody));
                cmd.Parameters.Add(this.DAL.GetParamenter("@ContentTypeID", entity.cmsContentTypeID));
                cmd.Parameters.Add(this.DAL.GetParamenter("@AuthorID", entity.cmsContentAuthorID));
                cmd.Parameters.Add(this.DAL.GetParamenter("@RecipientTypeID", entity.cmsContentRecipientTypeID));
                cmd.Parameters.Add(this.DAL.GetParamenter("@RecipientIndividualID", entity.cmsContentRecipientIndividualID));
                cmd.Parameters.Add(this.DAL.GetParamenter("@RecipientSiteID", entity.cmsContentRecipientSiteID));
                cmd.Parameters.Add(this.DAL.GetParamenter("@RecipientGroupID", entity.cmsContentRecipientGroupID));
                cmd.Parameters.Add(this.DAL.GetParamenter("@DisplayDate", entity.cmsContentDisplayDate));
                cmd.Parameters.Add(this.DAL.GetParamenter("@ExpireDate", entity.cmsContentExpireDate));

                InsertData(cmd);
            }

            /// <summary>
            /// Updates an existing CMS message in the database.
            /// </summary>
            /// <param name="entity">CMSEntity that contains the message to be updated.</param>
            public void UpdateMessage(CMSEntity entity)
            {
                var cmd = this.DAL.GetCommandObject();
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.CommandText = "CMSUpdateMessage";
                cmd.Parameters.Add(this.DAL.GetParamenter("@ContentID", entity.cmsContentID));
                cmd.Parameters.Add(this.DAL.GetParamenter("@ContentTitle", entity.cmsContentTitle));
                cmd.Parameters.Add(this.DAL.GetParamenter("@ContentBody", entity.cmsContentBody));
                cmd.Parameters.Add(this.DAL.GetParamenter("@ContentTypeID", entity.cmsContentTypeID));
                cmd.Parameters.Add(this.DAL.GetParamenter("@AuthorID", entity.cmsContentAuthorID));
                cmd.Parameters.Add(this.DAL.GetParamenter("@RecipientTypeID", entity.cmsContentRecipientTypeID));
                cmd.Parameters.Add(this.DAL.GetParamenter("@RecipientIndividualID", entity.cmsContentRecipientIndividualID));
                cmd.Parameters.Add(this.DAL.GetParamenter("@RecipientSiteID", entity.cmsContentRecipientSiteID));
                cmd.Parameters.Add(this.DAL.GetParamenter("@RecipientGroupID", entity.cmsContentRecipientGroupID));
                cmd.Parameters.Add(this.DAL.GetParamenter("@DisplayDate", entity.cmsContentDisplayDate));
                cmd.Parameters.Add(this.DAL.GetParamenter("@ExpireDate", entity.cmsContentExpireDate));
                UpdateData(cmd);
            }

            /// <summary>
            /// Deletes a message from the database.
            /// </summary>
            /// <param name="contentId">Content ID of the message to be deleted.</param>
            public void DeleteMessage(Int32 contentId)
            {
                var cmd = this.DAL.GetCommandObject();

                cmd.CommandText = "CMSDeleteMessage";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(this.DAL.GetParamenter("@ContentId", contentId));

                DeleteData(cmd);
            }

            protected override CMSEntity FillRecord(IDataReader dr)
            {
                var c = dr.GetColumnNameList();
                return new CMSEntity()
                {
                    AuthorFirstName = dr.AsString("AuthorFirstName", c),
                    AuthorLastName = dr.AsString("AuthorLastName", c),
                    AuthorMiddleName = dr.AsString("AuthorMiddleName", c),
                    cmsContentAuthorID = dr.ToInt32("cmsContentAuthorID", c),
                    cmsContentBody = dr.AsString("cmsContentBody", c),
                    cmsContentDescription = dr.AsString("cmsContentDescription", c),
                    cmsContentDisplayDate = dr.ToDateTime("cmsContentDisplayDate", c),
                    cmsContentExpireDate = dr.ToDateTime("cmsContentExpireDate", c),
                    cmsCreatedDate = dr.ToDateTime("cmsCreatedDate", c),
                    cmsContentID = dr.ToInt32("cmsContentID", c),
                    cmsContentRecipientGroupID = dr.AsString("cmsRecipientGroupID", c),
                    cmsContentRecipientIndividualID = dr.ToInt32("cmsContentRecipientIndividualID", c),
                    cmsContentRecipientSiteID = dr.AsString("cmsContentRecipientSiteID", c),
                    cmsContentRecipientTypeID = dr.AsString("cmsRecipientTypeID", c),
                    cmsContentTitle = dr.AsString("cmsContentTitle", c),
                    cmsContentTypeID = dr.AsString("cmsContentTypeID", c),
                    cmsContentTypeName = dr.AsString("cmsContentTypeName", c),
                    cmsRecipientGroupDescription = dr.AsString("cmsRecipientGroupDescription", c),
                    cmsRecipientGroupname = dr.AsString("cmsRecipientGroupName", c),
                    cmsRecipientTypeDescription = dr.AsString("cmsRecipientTypeDescription", c),
                    cmsRecipientTypeName = dr.AsString("cmsRecipientTypeName", c),
                    RecipientFristName = dr.AsString("RecipientFirstName", c),
                    RecipientLastName = dr.AsString("RecipientLastName", c),
                    RecipientMiddleName = dr.AsString("RecipientMiddleName", c)
                };
            }
        }

        /// <summary>
        /// A custom repository class for performing cms site/facility operations.
        /// </summary>
        public sealed class CmsSiteRepository : RepositoryBase<CmsSite>, ICmsSiteRepository
        {
            /// <summary>
            /// Default ctor.
            /// </summary>
            public CmsSiteRepository()
                : base(DbFactory.GetDbObject(DataBaseType.SQL, Globals.ConnStrNm))
            {
            }

            /// <summary>
            /// Ctor.
            /// </summary>
            /// <param name="CatalogName"></param>
            public CmsSiteRepository(String CatalogName)
                : base(DbFactory.GetDbObject(DataBaseType.SQL, Globals.DefaultConnStrNm), CatalogName)
            { }

            /// <summary>
            /// Gets a list of sites that a accept CMS messages based on their site code.
            /// </summary>
            /// <param name="siteCode">Site code to search with.</param>
            /// <returns>CmsSite list of sites that can accept CMS messages</returns>
            public List<CmsSite> GetCMS_UserFacilities(String siteCode)
            {
                var cmd = this.DAL.GetCommandObject();
                cmd.CommandText = "GetCMS_GetUserFacilities";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(this.DAL.GetParamenter("@SiteCode", siteCode));

                return GetRecords(cmd).ToList();
            }

            protected override CmsSite FillRecord(IDataReader dr)
            {
                var s = new CmsSite();
                s.IsActive = dr.ToBoolean("IsActive");
                s.SiteCode = dr.AsString("SiteCode");
                s.SiteName = dr.AsString("SiteName");
                s.SiteType = dr.AsString("SiteType");

                return s;
            }
        }
    }
}