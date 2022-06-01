using System;

namespace SrtsWeb.Entities
{
    [Serializable]
    public sealed class PersonnelEntity
    {
        public int ID { get; set; }

        public string PersonalType { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string MiddleName { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public string Demographic { get; set; }

        public DateTime? EADStopDate { get; set; }

        public bool IsPOC { get; set; }

        public string SiteCodeID { get; set; }

        public string Comments { get; set; }

        public bool IsActive { get; set; }

        public string TheaterLocationCode { get; set; }

        public string ModifiedBy { get; set; }

        public DateTime DateLastModified { get; set; }

        public string MyRole { get; set; }

        public string NameLF
        {
            get { return string.Format("{0}, {1}", LastName, FirstName); }
        }

        public string NameLFM
        {
            get
            {
                if (string.IsNullOrEmpty(MiddleName))
                {
                    return string.Format("{0}, {1}", LastName, FirstName);
                }
                else
                {
                    return string.Format("{0}, {1} {2}", LastName, FirstName, MiddleName);
                }
            }
        }

        public string NameLFMi
        {
            get
            {
                if (string.IsNullOrEmpty(MiddleName))
                {
                    return string.Format("{0}, {1}", LastName, FirstName);
                }
                else
                {
                    return string.Format("{0}, {1} {2}", LastName, FirstName, MiddleName.Substring(0, 1));
                }
            }
        }

        public string Initials
        {
            get
            {
                if (string.IsNullOrEmpty(MiddleName))
                {
                    return string.Format("{0}{1}", FirstName.Substring(0, 1), LastName.Substring(0, 1));
                }
                else
                {
                    return string.Format("{0}{1}{2}", FirstName.Substring(0, 1), MiddleName.Substring(0, 1), LastName.Substring(0, 1));
                }
            }
        }
    }
}