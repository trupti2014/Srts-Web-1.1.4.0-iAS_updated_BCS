using System;

namespace SrtsWeb.Entities
{
    public class PasswordHistory
    {
        public String PasswordHash { get; set; }

        public String Salt { get; set; }

        public DateTime HistoryDate { get; set; }
    }
}