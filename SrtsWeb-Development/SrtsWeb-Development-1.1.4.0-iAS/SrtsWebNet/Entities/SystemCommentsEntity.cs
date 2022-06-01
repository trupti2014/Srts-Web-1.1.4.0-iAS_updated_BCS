using System;

namespace SrtsWeb.Entities
{
    [Serializable]
    public sealed class SystemCommentsEntity
    {
        public int ID { get; set; }

        public string Comment { get; set; }
    }
}