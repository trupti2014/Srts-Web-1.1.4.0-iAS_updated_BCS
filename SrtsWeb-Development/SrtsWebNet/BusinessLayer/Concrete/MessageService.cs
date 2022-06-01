using SrtsWeb.BusinessLayer.Abstract;
using SrtsWeb.DataLayer.Repositories;
using SrtsWeb.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SrtsWeb.BusinessLayer.Concrete
{
    /// <summary>
    /// Custom class to perform CMS message 'get' operations.
    /// </summary>
    public sealed class MessageService : IMessageService
    {
        /// <summary>
        /// Gets all alert type (C003) messages.
        /// </summary>
        /// <returns>HTML formatted message string.</returns>
        public String GetAlerts()
        {
            var _repository = new CMSRepository.CmsEntityRepository();
            var l = _repository.GetMessageByContentTypeID("C003");
            var sb = new StringBuilder();
            foreach (var s in l)
                sb.AppendFormat("{0}<br /><br />", s.cmsContentBody);

            return String.IsNullOrEmpty(sb.ToString()) ? "No Alerts at This Time!" : sb.ToString();
        }

        /// <summary>
        /// Gets all public announcement type (C000) messages.
        /// </summary>
        /// <returns>Message list for public announcements.</returns>
        public List<CmsMessage> GetPublicAnnouncements()
        {
            var _repository = new CMSRepository.CmsMessageRepository();
            return _repository.GetMessageByContentTypeID("C000");
        }

        /// <summary>
        /// Gets all message center type (C002) messages.
        /// </summary>
        /// <param name="siteId">Site to get messages for.</param>
        /// <param name="siteTypeId">Site type to get messages for.</param>
        /// <param name="individualId">ID of individual performing look up.</param>
        /// <param name="groupId">Group id to get messages for.</param>
        /// <returns>List of messages to appear in the message center for a specific site.</returns>
        public List<CmsMessage> GetMessageCenterMessages(
            string siteId = default(String),
            string siteTypeId = default(String),
            int individualId = default(Int32),
            string groupId = default(String))
        {
            var _repository = new CMSRepository.CmsMessageRepository();
            return _repository.GetCms_MessageCenter_ApplicationAnnouncement_Content("C002", siteId, siteTypeId, individualId, groupId);
        }

        /// <summary>
        /// Gets all application announcement type (C001) messages.
        /// </summary>
        /// <param name="siteId">Site to get messages for.</param>
        /// <param name="siteTypeId">Site type to get messages for.</param>
        /// <param name="individualId">ID of individual performing look up.</param>
        /// <param name="groupId">Group id to get messages for.</param>
        /// <returns>List of messages that appear in the application announcements section.</returns>
        public List<CmsMessage> GetApplicationAnnouncements(
            string siteId = default(String),
            string siteTypeId = default(String),
            int individualId = default(Int32),
            string groupId = default(String))
        {
            var _repository = new CMSRepository.CmsMessageRepository();
            return _repository.GetCms_MessageCenter_ApplicationAnnouncement_Content("C001", siteId, siteTypeId, individualId, groupId);
        }

        /// <summary>
        /// Gets all help type (C005) messages.
        /// </summary>
        /// <returns>HTML formatted message string.</returns>
        public string GetHelpMessages()
        {
            var _repository = new CMSRepository.CmsEntityRepository();
            var l = _repository.GetMessageByContentTypeID("C005");
            var s = new StringBuilder();
            foreach (var a in l)
                s.AppendFormat("{0}<br />", a.cmsContentBody);

            return s.ToString();
        }

        /// <summary>
        /// Gets all body type (C004) messages.
        /// </summary>
        /// <returns>HTML formatted message string.</returns>
        public string GetBodyMessages()
        {
            var _repository = new CMSRepository.CmsEntityRepository();
            var l = _repository.GetMessageByContentTypeID("C004");
            var s = new StringBuilder();
            foreach (var a in l)
                s.AppendFormat("{0}<br />", a.cmsContentBody);

            return s.ToString();
        }

        /// <summary>
        /// Gets all message for an individual.
        /// </summary>
        /// <param name="individualId">Individual id to get messages for.</param>
        /// <returns>List of messages for a particular individual.</returns>
        public List<CmsMessage> GetMessagesById(int individualId)
        {
            var _repository = new CMSRepository.CmsMessageRepository();
            return _repository.GetCms_IndividualsMessagesById(individualId);
        }
    }
}