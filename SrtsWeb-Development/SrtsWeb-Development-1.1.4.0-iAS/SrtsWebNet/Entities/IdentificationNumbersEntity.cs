using System;

namespace SrtsWeb.Entities
{
    [Serializable]
    public sealed class IdentificationNumbersEntity
    {
        public int ID { get; set; }

        public int IndividualID { get; set; }

        public string IDNumber { get; set; }

        public string IDNumberType { get; set; }

        public string IDNumberTypeDescription { get; set; }

        public bool IsActive { get; set; }

        public string ModifiedBy { get; set; }

        public DateTime DateLastModified { get; set; }

        public string IDNumberFilter
        {
            get { return string.Format("{0}", IDNumber.Substring(IDNumber.Length - 4, 4).PadLeft(9, '*')); }
        }
    }
}