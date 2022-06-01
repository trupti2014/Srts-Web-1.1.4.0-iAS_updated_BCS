using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SrtsWeb.Entities
{
    [Serializable]
    public class ReprintEntity
    {
        private String _ReportType;
        private Int32 _ReportCount;
        private String _Modifier;
        private DateTime _BatchDate;

        public String ReportType
        {
            private get
            {
                switch (_ReportType)
                {
                    case "2":
                        return "Lab Routing Form";
                    case "7":
                    case "11":
                        return "Shipping Labels";
                    default:
                        return String.Empty;
                }
            }
            set { this._ReportType = value; }
        }
        public Int32 ReportCount { private get { return this._ReportCount; } set { this._ReportCount = value; } }
        public String Modifier { private get { return this._Modifier; } set { this._Modifier = value; } }
        public DateTime BatchDate { get { return this._BatchDate; } set { this._BatchDate = value; } }

        public String CombinedKey
        {
            get
            {
                var key = String.Format("{0} - {1} {2}", ReportCount, Modifier, BatchDate.ToString());
                return key;
            }
        }
    }

    [Serializable]
    public class ReprintReturnEntity
    {
        public Int32 ID { get; set; }
        public String Name { get; set; }
        public String Address1 { get; set; }
        public String Address2 { get; set; }
        public String Address3 { get; set; }
        public String City { get; set; }
        public String State { get; set; }
        public String Country { get; set; }
        public String ZipCode { get; set; }
    }


    [Serializable]
    public class ReprintOnDemandInsertEntity
    {
        public String OrderNumber { get; set; }
        public String SiteCode { get; set; }
        public String AddressType { get; set; }
    }

    [Serializable]
    public class ReprintOnDemandReturnEntity
    {
        public String Ordernumber { get; set; }
        public String AddressType { get; set; }
        public String FirstName { get; set; }
        public String MiddleName { get; set; }
        public String LastName { get; set; }
        public String Address1 { get; set; }
        public String Address2 { get; set; }
        public String Address3 { get; set; }
        public String City { get; set; }
        public String State { get; set; }
        public String Country { get; set; }
        public String ZipCode { get; set; }
    }
}
