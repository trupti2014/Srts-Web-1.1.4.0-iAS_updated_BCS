using System;

namespace SrtsWeb.Entities
{
    [Serializable]
    public sealed class OrderPriorityEntity
    {
        public string OrderPriorityText { get; set; }

        public string OrderPriorityValue { get; set; }
    }
}