using System;
using System.Data;

namespace SrtsWeb.DataLayer.TypeExtenders
{
    public static class DataExtender
    {
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

        public static DateTime? ToNullableDateTime(this Object objIn)
        {
            if (objIn == null) return null;
            DateTime i;
            return DateTime.TryParse(objIn.ToString(), out i) ? (DateTime?)i : null;
        }

        public static Int32? ToNullableInt32(this Object objIn)
        {
            if (objIn == null) return null;
            Int32 i;
            return Int32.TryParse(objIn.ToString(), out i) ? (Int32?)i : null;
        }

        public static Decimal? ToNullableDecimal(this Object objIn)
        {
            if (objIn == null) return null;
            Decimal i;
            return Decimal.TryParse(objIn.ToString(), out i) ? (Decimal?)i : null;
        }

        public static String AsString(this DataRow rowIn, String columnName)
        {
            if (rowIn == null) return String.Empty;

            if (!rowIn.Table.Columns.Contains(columnName)) return String.Empty;
            if (rowIn[columnName] == null) return String.Empty;

            return rowIn[columnName].ToString();
        }

        public static Int32 ToInt32(this DataRow rowIn, String columnName)
        {
            if (rowIn == null) return default(Int32);
            if (!rowIn.Table.Columns.Contains(columnName)) return default(Int32);
            if (rowIn[columnName] == null) return default(Int32);

            return rowIn[columnName].ToInt32();
        }

        public static Decimal ToDecimal(this DataRow rowIn, String columnName)
        {
            if (rowIn == null) return default(Decimal);
            if (!rowIn.Table.Columns.Contains(columnName)) return default(Decimal);
            if (rowIn[columnName] == null) return default(Decimal);

            return rowIn[columnName].ToDecimal();
        }

        public static Double ToDouble(this DataRow rowIn, String columnName)
        {
            if (rowIn == null) return default(Double);
            if (!rowIn.Table.Columns.Contains(columnName)) return default(Double);
            if (rowIn[columnName] == null) return default(Double);

            return rowIn[columnName].ToDouble();
        }

        public static DateTime ToDateTime(this DataRow rowIn, String columnName)
        {
            if (rowIn == null) return default(DateTime);
            if (!rowIn.Table.Columns.Contains(columnName)) return default(DateTime);
            if (rowIn[columnName] == null) return default(DateTime);

            return rowIn[columnName].ToDateTime();
        }

        public static Boolean ToBoolean(this DataRow rowIn, String columnName)
        {
            if (rowIn == null) return default(Boolean);
            if (!rowIn.Table.Columns.Contains(columnName)) return default(Boolean);
            if (rowIn[columnName] == null) return default(Boolean);

            return rowIn[columnName].ToBoolean();
        }

        public static DateTime? ToNullableDateTime(this DataRow rowIn, String columnName)
        {
            if (rowIn == null) return null;
            if (!rowIn.Table.Columns.Contains(columnName)) return null;
            if (rowIn[columnName] == null) return default(DateTime);

            return rowIn[columnName].ToNullableDateTime();
        }

        public static Decimal? ToNullableDecimal(this DataRow rowIn, String columnName)
        {
            if (rowIn == null) return null;
            if (!rowIn.Table.Columns.Contains(columnName)) return null;
            if (rowIn[columnName] == null) return default(Decimal);

            return rowIn[columnName].ToNullableDecimal();
        }

        public static Int32? ToNullableInt32(this DataRow rowIn, String columnName)
        {
            if (rowIn == null) return null;
            if (!rowIn.Table.Columns.Contains(columnName)) return null;
            if (rowIn[columnName] == null) return default(Int32);

            return rowIn[columnName].ToNullableInt32();
        }
    }
}