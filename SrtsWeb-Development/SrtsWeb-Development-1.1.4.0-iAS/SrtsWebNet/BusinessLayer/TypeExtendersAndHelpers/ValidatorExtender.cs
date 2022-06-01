using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace SrtsWeb.BusinessLayer.TypeExtendersAndHelpers.Extenders
{
    public static partial class SrtsExtender
    {
        public static Boolean ValidatePasswordComplexity(this String password, out String ErrorMsg)
        {
            var err = new StringBuilder();
            var p = password.Trim();
            var valList = new Dictionary<Regex, String>()
            {
                {new Regex(@"^.*([a-z]).*([a-z]).*$", RegexOptions.CultureInvariant), "Password does not contain at least two lower case letters<br />"},
                {new Regex(@"^.*([A-Z]).*([A-Z]).*$", RegexOptions.CultureInvariant), "Password does not contain at least two upper case letters<br />"},
                {new Regex(@"^.*([0-9]).*([0-9]).*$", RegexOptions.CultureInvariant), "Password does not contain at least two numbers<br />"},
                {new Regex(@"^.*([\&!@#\$%\^\*\(\)]).*([\&!@#\$%\^\*\(\)]).*$", RegexOptions.CultureInvariant), "Password does not contain at least two special characters<br />"}
            };

            Regex antiWhiteList = new Regex(@"^(?=.*[^a-zA-Z0-9\&!@#\$%\^\*\(\)]).*$", RegexOptions.CultureInvariant);

            if (p.Length < 15) err.AppendLine("Password is not at least 15 characters long<br />");
            if (antiWhiteList.IsMatch(p)) err.AppendLine("Password contains unapproved characters<br />");
            foreach (var r in valList)
                if (!r.Key.IsMatch(p)) err.AppendLine(r.Value);

            ErrorMsg = err.ToString();
            return String.IsNullOrEmpty(ErrorMsg);
        }

        public static Boolean ValidatePasswordCharacters(this String password)
        {
            var p = password.Trim();

            Regex antiWhiteList = new Regex(@"^(?=.*[^a-zA-Z0-9\&!@#\$%\^\*\(\)]).*$", RegexOptions.CultureInvariant);

            if (p.Length < 15) return false;
            if (antiWhiteList.IsMatch(p)) return false;

            return true;
        }

        public static bool ValidateNameLength(this string len)
        {
            const int min = 1;
            const int max = 40;

            if (len.Length >= min && len.Length <= max)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool ValidateNameFormat(this string name)
        {
            if (Regex.IsMatch(name, @"^[a-zA-Z'\s-]{1,40}$"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool ValidateUnitNameFormat(this string name)
        {
            if (Regex.IsMatch(name, @"^[a-zA-Z0-9'\s-]{1,40}$"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool ValidateDOBFormat(this string date)
        {
            if (Regex.IsMatch(date, @"^(0[1-9]|1[012])/(0[1-9]|[12][0-9]|3[01])/(19|20)\d\d$"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool ValidateDOBIsValid(this string date)
        {
            DateTime value;
            if (DateTime.TryParse(date, out value))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool ValidateDOBNotFuture(this string date)
        {
            if (date.GetDateTimeVal() > DateTime.Today)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static bool ValidateDateInRange(this string date, DatePart datePart, Int32 boundLimit, bool isUpperBound)
        {
            var d = new DateTime();
            if (DateTime.TryParse(date, out d))
            {
                switch (datePart)
                {
                    case DatePart.Day:
                        if (isUpperBound)
                        {
                            if (d <= DateTime.Today.Date.AddDays(boundLimit)) return true;
                        }
                        else
                            if (d >= DateTime.Today.Date.AddDays(-boundLimit)) return true;
                        break;

                    case DatePart.Month:
                        if (isUpperBound)
                        {
                            if (d <= DateTime.Today.Date.AddMonths(boundLimit)) return true;
                        }
                        else
                            if (d >= DateTime.Today.Date.AddMonths(-boundLimit)) return true;
                        break;

                    case DatePart.Year:
                        if (isUpperBound)
                        {
                            if (d <= DateTime.Today.Date.AddYears(boundLimit)) return true;
                        }
                        else
                            if (d >= DateTime.Today.Date.AddYears(-boundLimit)) return true;
                        break;
                }

                return false;
            }
            else
                return false;
        }

        public static bool ValidateIDNumLength(this string number, int len)
        {
            if (number.Length == len)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static string ValidateIDNumLength_forModals(this string number, string idtype)
        {
            int idLenLimit = 0;
            string idType = "";

            switch (idtype)
            {
                case "DIN":
                    idLenLimit = 10;
                    idType = "DoD ID";
                    break;

                case "SSN":
                    idLenLimit = 9;
                    idType = "Social Security";
                    break;

                case "PIN":
                    idLenLimit = 11;
                    idType = "Provider ID";
                    break;

                case "DBN":
                    idLenLimit = 11;
                    idType = "DoD Benifits";
                    break;

                default:
                    idLenLimit = 11;
                    break;
            }

            if (number.Length == idLenLimit)
            {
                return string.Empty;
            }
            else
            {
                return string.Format("{0} Number must be {1} digits", idType, idLenLimit);
            }
        }

        public static bool ValidateIDNumFormat(this string number)
        {
            if (Regex.IsMatch(number, @"^\d{9,11}$"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool ValidateCommentLength(this string input, int limit)
        {
            if (input.Length > limit)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static bool ValidateCommentFormat(this string input)
        {
            List<String> badCombos = new List<String>();
            badCombos.Add("''");
            badCombos.Add("//");

            badCombos.Add("--");
            badCombos.Add("' '");

            bool stop = false;

            if (input.Length > 0)
            {
                foreach (var item in badCombos)
                {
                    if (input.Contains(item))
                    {
                        stop = true;
                        return false;
                    }
                }
                if (!stop)
                {
                    if (Regex.IsMatch(input, @"^[a-zA-Z0-9\s+-.,!?#'/():]*$"))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public static bool ValidateAddressFormat(this string name)
        {
            return Regex.IsMatch(name, @"^[a-zA-Z0-9'.\s\-\\/#]{1,40}$");
        }

        public static bool ValidateCityFormat(this string name)
        {
            if (Regex.IsMatch(name, @"^[a-zA-Z'.\s-]{1,40}$"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool ValidateWorkPhoneFormat(this string name)
        {
            if (Regex.IsMatch(name, @"^[0-9-\-]{7,15}$"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool ValidateDSNPhoneFormat(this string name)
        {
            if (Regex.IsMatch(name, @"^[0-9-\-]{7,15}$"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool ValidateFaxFormat(this string name)
        {
            if (Regex.IsMatch(name, @"^[0-9+\(\)#\.\s\/-]{8,20}$"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool ValidateEmailFormat(this string name)
        {
            if (Regex.IsMatch(name, @"^([a-zA-Z][\w\.-]*[a-zA-Z0-9]@[a-zA-Z0-9][\w\.-]*[a-zA-Z0-9]\.[a-zA-Z][a-zA-Z\.]*[a-zA-Z]){1,70}$"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool ValidateZipCodeFormat(this string name)
        {
            if (Regex.IsMatch(name, @"^\d{5}(\-\d{4})?$"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public enum DatePart { Year, Month, Day };
}