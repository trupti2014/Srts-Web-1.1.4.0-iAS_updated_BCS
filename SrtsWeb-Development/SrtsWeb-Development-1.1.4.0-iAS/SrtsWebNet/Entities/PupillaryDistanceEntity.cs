using System;

namespace SrtsWeb.Entities
{
    [Serializable]
    public sealed class PupillaryDistanceEntity
    {
        public int DistantLeft { get; set; }

        public int DistantRight { get; set; }

        public int NearLeft { get; set; }

        public int NearRight { get; set; }
    }
}