using System;

namespace SrtsWeb.Entities
{
    [Serializable]
    public sealed class StatusEntity
    {
        public string StatusText { get; set; }

        public string StatusValue { get; set; }
    }
}