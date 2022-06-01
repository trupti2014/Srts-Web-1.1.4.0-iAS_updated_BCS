using System;

namespace SrtsWeb.Entities
{
    [Serializable]
    public sealed class UserAccountEntity
    {
        public string LowerUserName { get; set; }

        public bool IsApproved { get; set; }

        public bool IsLockedOut { get; set; }

    }
}