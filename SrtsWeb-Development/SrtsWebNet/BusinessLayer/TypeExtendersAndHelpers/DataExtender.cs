using SrtsWeb.BusinessLayer.TypeExtendersAndHelpers.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace SrtsWeb.BusinessLayer.TypeExtendersAndHelpers.Extenders
{
    public static partial class SrtsExtender
    {
        public static T ToTypeDefault<T>(this T tIn)
        {
            if (tIn == null) return default(T);
            return tIn;
        }

        public static Boolean IsDirty<T>(this T original, T compare)
        {
            return !original.Equals(compare);
        }

        public static string ToSSNRemoveDash(this string ssn)
        {
            if (string.IsNullOrEmpty(ssn))
            {
                return string.Empty;
            }
            if (ssn.Contains("-"))
            {
                return ssn.Replace("-", "");
            }
            else
            {
                return ssn;
            }
        }

        public static string ToSSNAddDash(this string ssn)
        {
            if (ssn.Length == 9)
            {
                return string.Format("{0}-{1}-{3}", ssn.Substring(0, 3), ssn.Substring(3, 2), ssn.Substring(5, 3));
            }
            else
            {
                return ssn;
            }
        }

        public static string ToZipCodeDisplay(this string zipCode)
        {
            if (string.IsNullOrEmpty(zipCode))
            {
                return string.Empty;
            }
            if (!zipCode.Contains("-"))
            {
                if (zipCode.Length >= 8)
                {
                    return zipCode.Insert(5, "-");
                }
                else
                {
                    return string.Format("{0}-0000", zipCode);
                }
            }
            else
                return zipCode;
        }

        public static string GetStringVal(this string columnName)
        {
            if (string.IsNullOrEmpty(columnName))
            {
                return string.Empty;
            }
            else
            {
                return columnName;
            }
        }

        public static int GetIntVal(this string columnName)
        {
            int? val = GetNullableIntVal(columnName);
            return (val.HasValue) ? val.Value : 0;
        }

        public static int? GetNullableIntVal(this string columnName)
        {
            int val = 0;
            if (Int32.TryParse(columnName, out val))
                return val;
            else
                return null;
        }

        public static double? GetNullableDoubleVal(this string columnName)
        {
            double val = 0.0;
            if (double.TryParse(columnName, out val))
                return val;
            else
                return null;
        }

        public static decimal GetDecimalVal(this string columnName)
        {
            decimal? val = GetNullableDecimalVal(columnName);
            return (val.HasValue) ? val.Value : 0;
        }

        public static decimal? GetNullableDecimalVal(this string columnName)
        {
            decimal val = 0;
            if (decimal.TryParse(columnName, out val))
                return decimal.Parse(columnName, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands);
            else
                return null;
        }

        public static double GetDoubleVal(this string columnName)
        {
            double? val = GetNullableDoubleVal(columnName);
            return (val.HasValue) ? val.Value : 0.0;
        }

        public static DateTime GetDateTimeVal(this string columnName)
        {
            DateTime? dtNullable = SrtsHelper.ParseValue(columnName);
            return (dtNullable.HasValue) ? dtNullable.Value : new DateTime();
        }

        public static DateTime? GetNullableDateTimeVal(this string columnName)
        {
            return SrtsHelper.ParseValue(columnName);
        }

        public static Boolean IsTextAfter(this String sourceString, String stringToFindTextAfter)
        {
            try
            {
                var idx = 0;

                if (!sourceString.Contains(stringToFindTextAfter)) return false;

                idx = sourceString.IndexOf(stringToFindTextAfter) + stringToFindTextAfter.Length;

                if (idx < stringToFindTextAfter.Length) return false;

                return sourceString.Substring(idx).Trim().Length > 0;
            }
            catch
            {
                return false;
            }
        }

        public static Boolean ContainsKey(this List<Tuple<String, String>> tupleListIn, String key)
        {
            var lKey = key.ToLower();

            return tupleListIn.Any(x => x.Item1.ToLower() == lKey);
        }

        public static Boolean TryGetValues(this List<Tuple<String, String>> tupleListIn, String key, out List<String> ValueListOut)
        {
            try
            {
                var lKey = key.ToLower();
                var nList = tupleListIn.Where(x => x.Item1.ToLower() == lKey).ToList();

                if (nList == null || nList.Count.Equals(0)) { ValueListOut = new List<string>(); return false; }

                var r = new List<String>();
                nList.ForEach(x => r.Add(x.Item2));

                ValueListOut = r;

                return true;
            }
            catch
            {
                ValueListOut = new List<string>();
                return false;
            }
        }

        public static String ToHtmlDecodeString<T>(this T tIn)
        {
            if ((tIn is object && tIn.GetType().IsValueType) || tIn is String)
                return tIn == null ? "" : HttpUtility.HtmlDecode(tIn.ToString());
            else
                return "";
        }
        public static String ToHtmlEncodeString<T>(this T tIn)
        {
            if ((tIn is object && tIn.GetType().IsValueType) || tIn is String)
                return tIn == null ? "" : HttpUtility.HtmlEncode(tIn.ToString());
            else
                return "";
        }
        public static Boolean IsNullOrEmpty<T>(this IEnumerable<T> listIn)
        {
            return listIn == null || listIn.Count().Equals(0);
        }
        public static Boolean IsNull<T>(this T tIn)
        {
            return tIn == null;
        }
        public static Boolean IsNull<T>(this IEnumerable<T> listIn)
        {
            return listIn == null;
        }
    }
}