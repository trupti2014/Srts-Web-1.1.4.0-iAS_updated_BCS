using SrtsWeb.Entities;
using System.Collections.Generic;

namespace SrtsWeb.Views.Admin
{
    public interface IEmailMessageView
    {
        SRTSSession mySession { get; set; }

        string EmailType { get; set; }
        string MessagePart1 { get; set; }
        string MessagePart2 { get; set; }
        string MessagePart3 { get; set; }
        string MessagePart4 { get; set; }

        string EmailAddress { get; set; }

        string EmailSubject { get; set;}
        string EmailBody { get; set; }
        EmailMessageEntity EmailMessageType { get; set; }

        List<EmailMessageEntity> EmailMessageTypes { get; set; }

        List<string> EmailAddresses { get; set; }


    }
}