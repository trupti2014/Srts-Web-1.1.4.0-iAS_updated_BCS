using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SrtsWeb.Entities
{
    [Serializable]
    public class EmailMessageEntity
    {
        public String EmailType { get; set; }
        public String MessagePart1 { get; set; }
        public String MessagePart2 { get; set; }
        public String MessagePart3 { get; set; }
        public String MessagePart4 { get; set; }
        public string EmailSubject { get; set; }
        public string EmailBody { get; set; }
        public string EmailAddress { get; set; }
        public string EmailRecipient { get; set; }
        public string ClinicSiteCode { get; set; }
        public string LabSiteCode { get; set; }
        public List<string> EmailAddresses { get; set; }

        public List<EmailMessageEntity> EmailNotifications { get; set; }
    }


}