using System;
using System.Collections.Generic;

namespace SrtsWeb.BusinessLayer.Abstract
{
    public interface IMailService
    {
        void SendEmail(String body, String fromEmail, List<String> toEmailAddress, String subject);
    }
}