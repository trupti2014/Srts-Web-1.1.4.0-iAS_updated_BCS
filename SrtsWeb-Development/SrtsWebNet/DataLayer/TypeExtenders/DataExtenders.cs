using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;

namespace SrtsWeb.DataLayer.TypeExtenders
{
    public static class DataExtenders
    {
        public static Int32 ToInt32(this IDataReader recIn, String columnName, List<String> columnNames = null)
        {
            if (recIn == null) return default(Int32);
            if (columnNames != null)
                if (!columnNames.Contains(columnName)) return default(Int32);
            Int32 i;
            return Int32.TryParse(recIn[columnName].ToString(), out i) ? i : default(Int32);
        }
        public static Decimal ToDecimal(this IDataReader recIn, String columnName, List<String> columnNames = null)
        {
            if (recIn == null) return default(Decimal);
            if (columnNames != null)
                if (!columnNames.Contains(columnName)) return default(Decimal);
            Decimal i;
            return Decimal.TryParse(recIn[columnName].ToString(), out i) ? i : default(Decimal);
        }
        public static Double ToDouble(this IDataReader recIn, String columnName, List<String> columnNames = null)
        {
            if (recIn == null) return default(Double);
            if (columnNames != null)
                if (!columnNames.Contains(columnName)) return default(Double);
            Double i;
            return Double.TryParse(recIn[columnName].ToString(), out i) ? i : default(Double);
        }
        public static DateTime ToDateTime(this IDataReader recIn, String columnName, List<String> columnNames = null)
        {
            if (recIn == null) return default(DateTime);
            if (columnNames != null)
                if (!columnNames.Contains(columnName)) return default(DateTime);
            DateTime i;
            return DateTime.TryParse(recIn[columnName].ToString(), out i) ? i : default(DateTime);
        }
        public static Boolean ToBoolean(this IDataReader recIn, String columnName, List<String> columnNames = null)
        {
            if (recIn == null) return default(Boolean);
            if (columnNames != null)
                if (!columnNames.Contains(columnName)) return default(Boolean);
            Boolean i;
            return Boolean.TryParse(recIn[columnName].ToString(), out i) ? i : default(Boolean);
        }
        public static String AsString(this IDataReader recIn, String columnName, List<String> columnNames = null)
        {
            if (recIn == null) return String.Empty;
            if (columnNames != null)
                if (!columnNames.Contains(columnName)) return String.Empty;

            return recIn[columnName].ToString();
        }

        public static Int32 ToInt32(this Object objIn)
        {
            if (objIn == null) return default(Int32);
            Int32 i;
            return Int32.TryParse(objIn.ToString(), out i) ? i : default(Int32);
        }
        public static Decimal ToDecimal(this Object objIn)
        {
            if (objIn == null) return default(Decimal);
            Decimal i;
            return Decimal.TryParse(objIn.ToString(), out i) ? i : default(Decimal);
        }
        public static Double ToDouble(this Object objIn)
        {
            if (objIn == null) return default(Double);
            Double i;
            return Double.TryParse(objIn.ToString(), out i) ? i : default(Double);
        }
        public static DateTime ToDateTime(this Object objIn)
        {
            if (objIn == null) return default(DateTime);
            DateTime i;
            return DateTime.TryParse(objIn.ToString(), out i) ? i : default(DateTime);
        }
        public static Boolean ToBoolean(this Object objIn)
        {
            if (objIn == null) return default(Boolean);
            Boolean i;
            return Boolean.TryParse(objIn.ToString(), out i) ? i : default(Boolean);
        }

        public static List<String> GetColumnNameList(this IDataReader dr)
        {
            return Enumerable.Range(0, dr.FieldCount).Select(dr.GetName).ToList();
        }

        // THESE ARE TEMPORARY!!!!!!  THERE ARE DUPLICATES OF EXISTING EXTENSION METHODS
        //public static string ToBOSValue(this string profile)
        //{
        //    if (string.IsNullOrEmpty(profile))
        //    {
        //        return string.Empty;
        //    }
        //    else
        //    {
        //        string str = string.Empty;
        //        if (profile.Length > 1)
        //        {
        //            str = profile.Substring(3, 1).ToUpper().Trim();
        //        }
        //        else
        //        {
        //            str = profile;
        //        }
        //        switch (str)
        //        {
        //            case "A":
        //                return "Army";

        //            case "B":
        //                return "NOAA";

        //            case "C":
        //                return "Coast Guard";

        //            case "F":
        //                return "Air Force";

        //            case "K":
        //                return "Other";

        //            case "M":
        //                return "Marines";

        //            case "N":
        //                return "Navy";

        //            case "P":
        //                return "PHS";

        //            default:
        //                return "Other";
        //        }
        //    }
        //}
        //public static string ToZipCodeDisplay(this string zipCode)
        //{
        //    if (string.IsNullOrEmpty(zipCode))
        //    {
        //        return string.Empty;
        //    }
        //    if (!zipCode.Contains("-"))
        //    {
        //        if (zipCode.Length >= 8)
        //        {
        //            return zipCode.Insert(5, "-");
        //        }
        //        else
        //        {
        //            return string.Format("{0}-0000", zipCode);
        //        }
        //    }
        //    else
        //        return zipCode;
        //}
    }
}