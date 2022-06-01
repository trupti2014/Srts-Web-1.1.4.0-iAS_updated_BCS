using System;

namespace SrtsWeb.Entities
{
    public class ReleaseNote
    {
        public String VersionNumber { get; set; }

        public DateTime VersionDate { get; set; }

        public String ReleaseNotes { get; set; }
    }
}