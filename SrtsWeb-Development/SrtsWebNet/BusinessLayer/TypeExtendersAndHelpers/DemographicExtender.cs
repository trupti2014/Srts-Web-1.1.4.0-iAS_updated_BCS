namespace SrtsWeb.BusinessLayer.TypeExtendersAndHelpers.Extenders
{
    public static partial class SrtsExtender
    {
        public static string ToRankKey(this string profile)
        {
            if (string.IsNullOrEmpty(profile))
            {
                return string.Empty;
            }
            else
            {
                string str = profile.Substring(0, 3);
                return str;
            }
        }

        public static string ToRankValue(this string profile)
        {
            if (string.IsNullOrEmpty(profile))
            {
                return string.Empty;
            }
            else
            {
                string str = profile.Substring(0, 3).Replace("*", "").Trim();
                return str;
            }
        }

        public static string ToPatientStatusKey(this string profile)
        {
            if (string.IsNullOrEmpty(profile))
            {
                return string.Empty;
            }
            else
            {
                string str = profile.Substring(4, 2);
                return str;
            }
        }

        public static string ToPatientStatusValue(this string profile)
        {
            if (string.IsNullOrEmpty(profile))
            {
                return string.Empty;
            }
            else
            {
                string str = profile.Substring(4, 2).ToUpper().Trim();
                switch (str)
                {
                    case "11":
                        return "Active Duty";

                    case "12":
                        return "Reserve";

                    case "14":
                        return "Cadet";

                    case "15":
                        return "National Guard";

                    case "21":
                        return "ROTC";

                    case "31":
                        return "Retired";

                    case "32":
                        return "PDRL";

                    case "36":
                        return "Former POW";

                    case "41":
                        return "DEP Active Duty";

                    case "43":
                        return "DEP Retired";

                    case "51":
                        return "State Dept Employee - Overseas";

                    case "52":
                        return "State Dept. Dependent - Overseas";

                    case "53":
                        return "Other Fed Agency Employee";

                    case "54":
                        return "Other Fed Agency Dependent";

                    case "55":
                        return "DOD Remote Area Employee - CONUS";

                    case "56":
                        return "DOD Remote Area Dependent - CONUS";

                    case "57":
                        return "DOD Occupational Health";

                    case "59":
                        return "Other Employee and Dep(USO, RED CROSS)";

                    case "61":
                        return "VA Beneficiary";

                    case "62":
                        return "OFF Workman's Comp Program (OWCP)";

                    case "63":
                        return "Service Home - Other Than Retired";

                    case "64":
                        return "Other Federal Agency";

                    case "65":
                        return "Contract Employee";

                    case "66":
                        return "Federal Prisoner";

                    case "67":
                        return "American Indian, ALEUT, Eskimo";

                    case "68":
                        return "MICRONESIA, SAMOA, Trust Territory";

                    case "69":
                        return "Other US Government Beneficiary";

                    case "71":
                        return "IMET/SALES";

                    case "72":
                        return "NATO Military";

                    case "74":
                        return "Non-NATO Military";

                    case "76":
                        return "Foreign Civilian";

                    case "78":
                        return "Foreign POW/Internee";

                    case "00":
                        return "DOD Civilian Employee";

                    case "99":
                        return "Not Applicable";

                    default:
                        return "Undetermined";
                }
            }
        }

        public static string ToOrderPriorityKey(this string profile)
        {
            if (string.IsNullOrEmpty(profile))
            {
                return string.Empty;
            }
            else
            {
                string str = profile.Substring(profile.Length - 1);
                return str;
            }
        }

        public static string ToOrderPriorityValue(this string profile)
        {
            if (string.IsNullOrEmpty(profile))
            {
                return string.Empty;
            }
            else
            {
                string tmpStr = profile.Substring(profile.Length - 1).ToUpper();
                switch (tmpStr)
                {
                    case "S":
                        return "Standard";

                    case "V":
                        return "VIP";

                    case "W":
                        return "Wounded Warrior";

                    case "F":
                        return "Frame of Choice";

                    case "P":
                        return "Pilot";

                    case "C":
                        return "Crew Member";

                    case "R":
                        return "Readiness";

                    case "H":
                        return "Humanirarian";

                    case "T":
                        return "Trainee";

                    case "N":
                        return "Not Applicable";

                    default:
                        return "Unknown";
                }
            }
        }

        public static string ToBOSKey(this string profile)
        {
            if (string.IsNullOrEmpty(profile))
            {
                return string.Empty;
            }
            else
            {
                string str = profile.Substring(3, 1);
                return str;
            }
        }

        public static string ToBOSValue(this string profile)
        {
            if (string.IsNullOrEmpty(profile))
            {
                return string.Empty;
            }
            else
            {
                string str = string.Empty;
                if (profile.Length > 1)
                {
                    str = profile.Substring(3, 1).ToUpper().Trim();
                }
                else
                {
                    str = profile;
                }
                switch (str)
                {
                    case "A":
                        return "Army";

                    case "B":
                        return "NOAA";

                    case "C":
                        return "Coast Guard";

                    case "F":
                        return "Air Force";

                    case "K":
                        return "Other";

                    case "M":
                        return "Marines";

                    case "N":
                        return "Navy";

                    case "P":
                        return "PHS";

                    default:
                        return "Other";
                }
            }
        }

        public static string ToGenderKey(this string profile)
        {
            if (string.IsNullOrEmpty(profile))
            {
                return string.Empty;
            }
            else
            {
                string str = profile.Substring(6, 1);
                return str;
            }
        }

        public static string ToGenderValue(this string profile)
        {
            if (string.IsNullOrEmpty(profile))
            {
                return string.Empty;
            }
            else
            {
                string str = profile.Substring(6, 1).ToUpper().Trim();
                switch (str)
                {
                    case "M":
                        return "Male";

                    case "F":
                        return "Female";

                    case "B":
                        return "Both";

                    default:
                        return "Not Identified";
                }
            }
        }
    }
}