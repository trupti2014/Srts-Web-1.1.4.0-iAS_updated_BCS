using SrtsWeb.Entities;
using System;
using System.Collections.Generic;

namespace SrtsWeb.BusinessLayer.Abstract
{
    public interface IMessageService
    {
        String GetAlerts();

        List<CmsMessage> GetPublicAnnouncements();

        List<CmsMessage> GetMessagesById(Int32 individualId);

        List<CmsMessage> GetMessageCenterMessages(
            string siteId = default(String),
            string siteType = default(String),
            int individualId = default(Int32),
            string groupId = default(String));

        List<CmsMessage> GetApplicationAnnouncements(
            string siteId = default(String),
            string siteType = default(String),
            int individualId = default(Int32),
            string groupId = default(String));

        String GetHelpMessages();

        String GetBodyMessages();
    }
}