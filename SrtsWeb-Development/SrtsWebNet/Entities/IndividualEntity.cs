using System;

namespace SrtsWeb.Entities
{
    [Serializable]
    public sealed class IndividualEntity
    {
        public int ID { get; set; }

        public string IDNumber { get; set; }

        public string IDNumberType { get; set; }

        public string IDNumberTypeDescription { get; set; }

        public string PersonalType { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string MiddleName { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public string Demographic { get; set; }

        public DateTime? EADStopDate { get; set; }

        public DateTime? NextFocDate { get; set; }

        public bool IsPOC { get; set; }

        public string SiteCodeID { get; set; }

        public string Comments { get; set; }

        public bool IsActive { get; set; }

        public string TheaterLocationCode { get; set; }

        public string ModifiedBy { get; set; }

        public DateTime DateLastModified { get; set; }

        public string BOSDescription { get; set; }

        public string StatusDescription { get; set; }

        public Boolean IsNewPatient { get; set; }

        public string IDNumberDisplay
        {
            get { return string.Format("******{0}", IDNumber.Substring(IDNumber.Length - 4, 4)); }
        }

        public string Gender
        {
            get
            {
                return Demographic.Substring(6, 1);
            }
            set
            {
            }
        }

        public string Status
        {
            get { return Demographic.Substring(4, 2); }
        }

        public string BOS
        {
            get { return Demographic.Substring(3, 1); }
        }

        public string Rank
        {
            get
            {
                { return Demographic.Substring(0, 3); }
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
                    return string.Format("{0}, {1} {2}.", LastName, FirstName, MiddleName.Substring(0, 1));
                }
            }
        }

        public string NameFMiL
        {
            get
            {
                if (string.IsNullOrEmpty(MiddleName))
                {
                    return string.Format("{0} {1}", FirstName, LastName);
                }
                else
                {
                    return string.Format("{0} {1}. {2}", FirstName, MiddleName.Substring(0, 1), LastName);
                }
            }
        }

        public string NameFiL
        {
            get
            {
                return string.Format("{0}. {1}", FirstName.Substring(0, 1), LastName);
            }
        }

        public string NameFML
        {
            get
            {
                if (string.IsNullOrEmpty(MiddleName))
                {
                    return string.Format("{0} {1}", FirstName, LastName);
                }
                else
                {
                    return string.Format("{0} {1} {2}", FirstName, MiddleName, LastName);
                }
            }
        }
    }
}