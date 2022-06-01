using System;
using System.ComponentModel.DataAnnotations;

namespace SrtsWeb.Entities
{
    public sealed class DmdcPersonnel
    {
        private String _PnCategoryCode;

        [MaxLength(1)]
        public String PnCategoryCode
        {
            get
            {
                switch (_PnCategoryCode)
                {
                    /*
                    A	Active duty member
                    B	Presidential Appointees of all Federal Government agencies
                    C	DoD and Uniformed Service civil service employee, except Presidential appointee
                    D	Disabled American veteran
                    E	DoD and Uniformed Service contract employee
                    F	Former member (Reserve service, discharged from RR or SR following notification of retirement eligibility
                    H	Medal of Honor recipient
                    I	Non-DoD civil service employee, except Presidential appointee
                    J	Academy student
                    K	Non-appropriated fund DoD and Uniformed Service employee (NAF)
                    L	Lighthouse service
                    M	Non-federal Agency civilian associates
                    N	National Guard member
                    O	Non-DoD contract employee
                    Q	Reserve retiree not yet eligible for retired pay (“Grey Area Retiree”)
                    R	Retired military member eligible for retired pay
                    T	Foreign Affiliate
                    U	DoD OCONUS Hires
                    V	Reserve member
                    W	DoD Beneficiary, a person who receives benefits from the DoD based on prior association, condition or authorization, an example is a former spouse
                    Y	Civilian Retirees
                    */
                    case "A":
                        return "11"; // Active Duty
                    case "C":
                        return "00"; // Civ
                    case "D":
                        return "32";

                    case "E":
                    case "O":
                        return "65"; // Contract Employee
                    case "J":
                        return "14"; // Academy Student (Cadet)
                    case "N":
                        return "15"; // National Guard
                    case "R":
                        return "31"; // Retired
                    case "V":
                        return "12"; // Reserve
                    default:
                        return String.Empty;
                }
            }
            set { _PnCategoryCode = value; }
        }

        [MaxLength(2)]
        public String PnEntitlementTypeCode { get; set; }

        [MaxLength(2)]
        public String OrganizationCode { get; set; }

        public DateTime PnProjectedEndDate { get; set; }

        [MaxLength(2)]
        public String PayPlanCode { get; set; }

        [MaxLength(2)]
        public String PayGrade { get; set; }

        [MaxLength(6)]
        public String Rank { get; set; }

        private String _ServiceCode;

        [MaxLength(1)]
        public String ServiceCode
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

        [MaxLength(8)]
        public String AttachedUnitIdCode { get; set; }
    }
}