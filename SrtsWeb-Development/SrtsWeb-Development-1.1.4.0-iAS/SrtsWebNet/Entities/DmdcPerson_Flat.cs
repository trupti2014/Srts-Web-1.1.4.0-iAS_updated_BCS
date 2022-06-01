using System;

namespace SrtsWeb.Entities
{
    [Serializable]
    public class DmdcPerson_Flat
    {
        public string PnLastName { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string EnterpriseUserName { get; set; }

        public string PnFirstName { get; set; }

        public string PnMiddleName { get; set; }

        public string PnCadencyName { get; set; }

        public DateTime PnDateOfBirth { get; set; }

        public DateTime PnDeathCalendarDate { get; set; }

        public string MailingAddress1 { get; set; }

        public string MailingAddress2 { get; set; }

        public string MailingCity { get; set; }

        public string MailingState { get; set; }

        public string MailingZip { get; set; }

        public string MailingZipExtension { get; set; }

        public string MailingCountry { get; set; }

        private string _PnCategoryCode;

        public string PnCategoryCode
        {
            get
            {
                return _PnCategoryCode;
                //switch (_PnCategoryCode)
                //{
                //    /*
                //    A	Active duty member
                //    B	Presidential Appointees of all Federal Government agencies
                //    C	DoD and Uniformed Service civil service employee, except Presidential appointee
                //    D	Disabled American veteran
                //    E	DoD and Uniformed Service contract employee
                //    F	Former member (Reserve service, discharged from RR or SR following notification of retirement eligibility
                //    H	Medal of Honor recipient
                //    I	Non-DoD civil service employee, except Presidential appointee
                //    J	Academy student
                //    K	Non-appropriated fund DoD and Uniformed Service employee (NAF)
                //    L	Lighthouse service
                //    M	Non-federal Agency civilian associates
                //    N	National Guard member
                //    O	Non-DoD contract employee
                //    Q	Reserve retiree not yet eligible for retired pay (“Grey Area Retiree”)
                //    R	Retired military member eligible for retired pay
                //    T	Foreign Affiliate
                //    U	DoD OCONUS Hires
                //    V	Reserve member
                //    W	DoD Beneficiary, a person who receives benefits from the DoD based on prior association, condition or authorization, an example is a former spouse
                //    Y	Civilian Retirees
                //    */
                //    case "A":
                //        return "11"; // Active Duty
                //    case "C":
                //        return "00"; // Civ
                //    case "E":
                //        return "65"; // Contract Employee
                //    case "J":
                //        return "14"; // Academy Student (Cadet)
                //    case "N":
                //        return "15"; // National Guard
                //    case "R":
                //        return "31"; // Retired
                //    case "V":
                //        return "12"; // Reserve
                //    default:
                //        return string.Empty;
                //}
            }
            set { _PnCategoryCode = value; }
        }

        public string PnEntitlementTypeCode { get; set; }

        public string OrganizationCode { get; set; }

        public DateTime PnProjectedEndDate { get; set; }

        public string PayPlanCode { get; set; }

        public string PayGrade { get; set; }

        public string Rank { get; set; }

        private string _ServiceCode;

        public string ServiceCode
        {
            get
            {
                switch (_ServiceCode)
                {
                    case "O": // The Commissioned Corps of the National Oceanic and Atmospheric Administration
                        return "B";

                    case "H": // The Commissioned Corps of the Public Health Service
                        return "P";

                    case "1": // Foreign Army
                    case "2": // Foreign Navy
                    case "3": // Foreign Marine Corps
                    case "4": // Foreign Air Force
                    case "D": // Office of the Secretary of Defense
                    case "X": // Not applicable
                    case "Z": // Unknown
                        return "K";

                    default:
                        return _ServiceCode;
                }
            }
            set { _ServiceCode = value; }
        }

        public string AttachedUnitIdCode { get; set; }

        public string MatchReasonCode1 { get; set; }

        public string PnId1 { get; set; }

        public string PnIdType1 { get; set; }

        public string MatchReasonCode2 { get; set; }

        public string PnId2 { get; set; }

        public string PnIdType2 { get; set; }
    }
}