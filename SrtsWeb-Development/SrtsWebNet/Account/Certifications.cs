using System;

namespace SrtsWeb.Account
{
    [Serializable]
    public class Certifications
    {
        public DateTime HIPPACertifiedOn { get; set; }

        public DateTime HIPPACertificateExpires { get; set; }

        public DateTime IASOCertifiedOn { get; set; }

        public DateTime IASOCertificateExpires { get; set; }
    }
}