using System;

namespace SrtsWeb.Entities
{
    [Serializable]
    public sealed class RankEntity
    {
        public string RankText { get; set; }

        public string RankValue { get; set; }
    }
}