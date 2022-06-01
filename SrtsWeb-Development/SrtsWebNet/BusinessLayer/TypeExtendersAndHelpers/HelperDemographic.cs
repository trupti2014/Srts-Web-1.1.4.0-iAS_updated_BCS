using SrtsWeb.BusinessLayer.TypeExtendersAndHelpers.Extenders;
using System;
using System.Data;
using System.Linq;

namespace SrtsWeb.BusinessLayer.TypeExtendersAndHelpers.Helpers
{
    public partial class SrtsHelper
    {
        public static string SetLocationCode(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return "00000";
            }
            else
            {
                return str;
            }
        }

        public static string DisplayLocationCode(string str)
        {
            if (str == "00000")
            {
                return string.Empty;
            }
            else
            {
                return str;
            }
        }

        public static string BuildProfile(string rank, string bos, string jobStatus, string gender, string orderPriority)
        {
            if (rank.Length == 2)
            {
                return string.Format("*{0}{1}{2}{3}{4}", rank, bos, jobStatus, gender, orderPriority);
            }
            else
            {
                return string.Format("{0}{1}{2}{3}{4}", rank, bos, jobStatus, gender, orderPriority);
            }
        }

        //public static string GetDescriptionFromLookup(DataTable _dt, string _lookupType, string _key)
        //{
        //    DataTable dt2 = GetLookupTypesSelected(_dt, _lookupType);
        //    var str = from a in dt2.AsEnumerable()
        //              where a.Field<string>("Value") == _key
        //              select new
        //              {
        //                  Text = a.Field<string>("Text")
        //              };

        //    return str.FirstOrDefault().ToString();
        //}

        public static bool CheckDemographic(string demographic)
        {
            bool cont = true;
            string _status = demographic.ToPatientStatusKey();
            string _oPriority = demographic.ToOrderPriorityKey();
            string _bos = demographic.ToBOSKey();
            string _rank = demographic.ToRankKey();
            switch (_rank.ToUpper())
            {
                case "*E1":
                case "*E2":
                case "*E3":
                case "*E4":
                case "*E5":
                case "*E6":
                case "*E7":
                case "*E8":
                case "*E9":
                case "*O1":
                case "*O2":
                case "*O3":
                case "*O4":
                case "*O5":
                case "*O6":
                case "*O7":
                case "*O8":
                case "*O9":
                case "O10":
                case "O11":
                case "*W1":
                case "*W2":
                case "*W3":
                case "*W4":
                case "*W5":
                case "CDT":
                    cont = true;
                    if (CheckBOS(_bos))
                    {
                        cont = true;
                    }
                    else
                    {
                        return false;
                    }
                    break;

                case "FOR":
                case "CIV":
                case "DEP":
                case "PHS":
                case "OTH":

                case "NA":
                default:
                    cont = false;
                    if (CheckBOS(_bos))
                    {
                        return false;
                    }
                    else
                    {
                        cont = false;
                    }
                    break;
            }

            if (cont)
            {
                cont = IsMilitaryStatus(_status);
            }
            else
            {
                if (!IsMilitaryStatus(_status))
                {
                    cont = true;
                }
            }

            if (_oPriority == "T")
            {
                if (_rank == "*E1")
                {
                    cont = true;
                }
                else
                {
                    return false;
                }
            }

            if (_oPriority == "P")
            {
                if (CheckPilot(_rank))
                {
                    cont = true;
                }
                else
                {
                    return false;
                }
            }

            if (_oPriority == "C" || _oPriority == "R")
            {
                if (CheckIsMilitary(_rank))
                {
                    cont = true;
                }
                else
                {
                    return false;
                }
            }

            if (_oPriority == "W")
            {
                if (CheckWoundedWarrior(_rank))
                {
                    cont = true;
                }
                else
                {
                    return false;
                }
            }

            if (_oPriority == "V")
            {
                if (CheckVIP(_rank))
                {
                    cont = true;
                }
                else
                {
                    return false;
                }
            }
            return cont;
        }

        public static bool CheckIsMilitary(string _rank)
        {
            switch (_rank.ToUpper())
            {
                case "*E1":
                case "*E2":
                case "*E3":
                case "*E4":
                case "*E5":
                case "*E6":
                case "*E7":
                case "*E8":
                case "*E9":
                case "*O1":
                case "*O2":
                case "*O3":
                case "*O4":
                case "*O5":
                case "*O6":
                case "*O7":
                case "*O8":
                case "*O9":
                case "O10":
                case "O11":
                case "*W1":
                case "*W2":
                case "*W3":
                case "*W4":
                case "*W5":
                case "CDT":
                    return true;

                default:
                    return false;
            }
        }

        public static bool CheckWoundedWarrior(string _rank)
        {
            switch (_rank.ToUpper())
            {
                case "*E1":
                case "*E2":
                case "*E3":
                case "*E4":
                case "*E5":
                case "*E6":
                case "*E7":
                case "*E8":
                case "*E9":
                case "*O1":
                case "*O2":
                case "*O3":
                case "*O4":
                case "*O5":
                case "*O6":
                case "*O7":
                case "*O8":
                case "*O9":
                case "O10":
                case "O11":
                case "*W1":
                case "*W2":
                case "*W3":
                case "*W4":
                case "*W5":
                    return true;

                default:
                    return false;
            }
        }

        public static bool CheckPilot(string _rank)
        {
            switch (_rank.ToUpper())
            {
                case "*O1":
                case "*O2":
                case "*O3":
                case "*O4":
                case "*O5":
                case "*O6":
                case "*O7":
                case "*O8":
                case "*O9":
                case "O10":
                case "O11":
                case "*W1":
                case "*W2":
                case "*W3":
                case "*W4":
                case "*W5":
                    return true;

                default:
                    return false;
            }
        }

        public static bool CheckVIP(string _rank)
        {
            switch (_rank.ToUpper())
            {
                case "*O7":
                case "*O8":
                case "*O9":
                case "O10":
                case "O11":
                    return true;

                default:
                    return false;
            }
        }

        public static bool IsMilitaryStatus(string _status)
        {
            switch (_status)
            {
                case "11":

                case "12":

                case "14":

                case "15":

                case "21":

                case "31":

                case "41":

                case "43":

                    return true;

                case "51":

                case "52":

                case "53":

                case "54":

                case "55":

                case "56":

                case "57":

                case "59":

                case "61":

                case "63":

                case "64":

                case "65":

                case "66":

                case "67":

                case "68":

                case "69":

                case "71":

                case "72":

                case "74":

                case "76":

                case "78":

                case "00":

                case "99":

                default:
                    return false;
            }
        }

        public static bool CheckBOS(string _rank)
        {
            switch (_rank)
            {
                case "A":

                case "F":

                case "M":

                case "N":

                case "C":

                    return true;

                case "P":

                case "O":

                case "K":

                case "B":

                default:
                    return false;
            }
        }

        public static Boolean IsBosValid(String _bosToCheck)
        {
            switch (_bosToCheck.ToUpper())
            {
                case "A":
                case "F":

                case "M":

                case "N":

                case "C":

                case "P":

                case "O":

                case "K":

                case "B":

                    return true;

                default:
                    return false;
            }
        }
    }
}