using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SrtsWeb.Entities
{
    public sealed class FrameRxRestrictionsEntity
    {
        public string FrameCode { get; set; }
        public decimal MaxSphere { get; set; }
        public decimal MinSphere { get; set; }
        public decimal MaxCylinder { get; set; }
        public decimal MinCylinder { get; set; }
        public string Material { get; set; }
    }
}
