using System;

namespace SrtsWeb.Entities
{
    [Serializable]
    public sealed class CACInfo
    {
        public string CACSubject { get; set; }

        public string CACSubjectKeyIdentifier { get; set; }

        public int CACVersion { get; set; }

        public string CACHexSerialNumber { get; set; }

        public string CACIssuer { get; set; }

        public string CACAuthorityKeyIdentifier { get; set; }

        public DateTime CACValidFrom { get; set; }

        public DateTime CACValidTo { get; set; }

        public string CACHexThumbPrint { get; set; }
    }
}