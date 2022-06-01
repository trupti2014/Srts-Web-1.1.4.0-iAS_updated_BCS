using System;
using System.Collections.Generic;

namespace SrtsWeb.Entities
{
    [Serializable]
    public class ReleaseManagementUserGuideEntity
    {
    
        public String GuideName { get; set; }

        public byte[] UserGuideDocument { get; set; }

        public DateTime DateLastModified { get; set; }

        public String ModifiedBy { get; set; }

    }
}