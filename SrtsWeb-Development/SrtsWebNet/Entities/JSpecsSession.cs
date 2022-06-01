using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SrtsWeb.Entities
{
    [Serializable]
    public sealed class JSpecsSession
    {
        public PatientEntity Patient { get; set; }
        public JSpecsOrder JSpecsOrder { get; set; }
        public bool SecurityAcknowledged { get; set; }
    }
}
