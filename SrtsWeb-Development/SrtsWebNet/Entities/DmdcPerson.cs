using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SrtsWeb.Entities
{
    public sealed class DmdcPerson
    {
        [MaxLength(26)]
        public String PnLastName { get; set; }

        [MaxLength(80)]
        public String Email { get; set; }

        public String PhoneNumber { get; set; }

        [MaxLength(106)]
        public String EnterpriseUserName { get; set; }

        [MaxLength(20)]
        public String PnFirstName { get; set; }

        [MaxLength(20)]
        public String PnMiddleName { get; set; }

        [MaxLength(4)]
        public String PnCadencyName { get; set; }

        public DateTime PnDateOfBirth { get; set; }

        public DateTime PnDeathCalendarDate { get; set; }

        [MaxLength(40)]
        public String MailingAddress1 { get; set; }

        [MaxLength(40)]
        public String MailingAddress2 { get; set; }

        [MaxLength(20)]
        public String MailingCity { get; set; }

        [MaxLength(2)]
        public String MailingState { get; set; }

        [MaxLength(5)]
        public String MailingZip { get; set; }

        [MaxLength(4)]
        public String MailingZipExtension { get; set; }

        [MaxLength(2)]
        public String MailingCountry { get; set; }

        public DmdcPersonnel _DmdcPersonnel;
        public List<DmdcIdentifiers> _DmdcIdentifier;
    }
}