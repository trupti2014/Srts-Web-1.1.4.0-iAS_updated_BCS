using System;

namespace SrtsWeb.Entities
{
    [Serializable]
    public sealed class EMailAddressEntity
    {
        public int ID { get; set; }

        public int IndividualID { get; set; }

        public string EMailType { get { return "MILITARY"; } }

        public string EMailAddress { get; set; }

        public bool IsActive { get; set; }

        public string ModifiedBy { get; set; }

        public DateTime DateLastModified { get; set; }

        public string EMailDisplay { get { return String.Format("{0} - {1}", EMailType, EMailAddress); } }
    }
}