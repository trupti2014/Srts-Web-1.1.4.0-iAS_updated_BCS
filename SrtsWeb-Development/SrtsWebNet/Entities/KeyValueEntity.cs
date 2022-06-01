using System;

namespace SrtsWeb.Entities
{
    [Serializable]
    public class KeyValueEntity
    {
        public object Key { get; set; }

        public object Value { get; set; }
    }
}