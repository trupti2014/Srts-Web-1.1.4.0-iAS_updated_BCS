using System;
using System.ComponentModel.DataAnnotations;

namespace SrtsWeb.Entities
{
    public sealed class DmdcIdentifiers
    {
        [MaxLength(3)]
        public String MatchReasonCode { get; set; }

        public String PnId { get; set; }

        public String PnIdType { get; set; }
    }
}