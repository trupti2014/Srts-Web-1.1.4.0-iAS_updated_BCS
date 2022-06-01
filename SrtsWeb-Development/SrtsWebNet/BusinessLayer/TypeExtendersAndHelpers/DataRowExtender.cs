using SrtsWeb.BusinessLayer.TypeExtendersAndHelpers.Helpers;
using System;
using System.Data;

namespace SrtsWeb.BusinessLayer.TypeExtendersAndHelpers.Extenders
{
    public static partial class SrtsExtender
    {
        public static string GetStringVal(this DataRow row, string columnName)
        {
            if (!row.Table.Columns.Contains(columnName)) return String.Empty;
            if (string.IsNullOrEmpty(row[columnName].ToString())) return string.Empty;

            return row[columnName].ToString();
        }

        public static string GetStringVal(this DataRow row, int columnIndex)
        {
            if (string.IsNullOrEmpty(row[columnIndex].ToString()))
            {
                return string.Empty;
            }
            else
            {
                return row[columnIndex].ToString();
            }
        }

        public static bool GetBoolVal(this DataRow row, string columnName)
        {
            if (!row.Table.Columns.Contains(columnName)) return false;  // Added for IsDeletable field on edit prescription
            string sVal = row[columnName].ToString().ToUpper();
            return (sVal == "Y" || sVal == "YES" || sVal == "1" || sVal == "T" || sVal == "TRUE" || sVal == "ON");
        }

        public static bool GetBoolVal(this DataRow row, int columnIndex)
        {
            if (row.Table.Columns.Count < columnIndex) return false;
            string sVal = row[columnIndex].ToString().ToUpper();
            return (sVal == "Y" || sVal == "YES" || sVal == "1" || sVal == "T" || sVal == "TRUE" || sVal == "ON");
        }

        public static bool? GetNullableBoolVal(this DataRow row, string columnName)
        {
            string val = row[columnName].ToString().ToUpper();
            if (val == "Y" || val == "YES" || val == "1" || val == "T" || val == "TRUE" || val == "ON")
                return true;
            if (val == "N" || val == "NO" || val == "0" || val == "F" || val == "FALSE" || val == "OFF")
                return false;
            return null;
        }

        public static bool? GetNullableBoolVal(this DataRow row, int columnIndex)
        {
            string val = row[columnIndex].ToString().ToUpper();
            if (val == "Y" || val == "YES" || val == "1" || val == "T" || val == "TRUE" || val == "ON")
                return true;
            if (val == "N" || val == "NO" || val == "0" || val == "F" || val == "FALSE" || val == "OFF")
                return false;
            return null;
        }

        public static int GetIntVal(this DataRow row, string columnName)
        {
            if (!row.Table.Columns.Contains(columnName)) return default(int);
            int? val = GetNullableIntVal(row, columnName);
            return (val.HasValue) ? val.Value : 0;
        }

        public static int GetIntVal(this DataRow row, int columnIndex)
        {
            int? val = GetNullableIntVal(row, columnIndex);
            return (val.HasValue) ? val.Value : 0;
        }

        public static int? GetNullableIntVal(this DataRow row, string columnName)
        {
            int val = 0;
            if (Int32.TryParse(row[columnName].ToString(), out val))
                return val;
            else
                return null;
        }

        public static double? GetNullableDoubleVal(this DataRow row, string columnName)
        {
            double val = 0.0;
            if (double.TryParse(row[columnName].ToString(), out val))
                return val;
            else
                return null;
        }

        public static decimal GetDecimalVal(this DataRow row, string columnName)
        {
            decimal? val = GetNullableDecimalVal(row, columnName);
            return (val.HasValue) ? val.Value : 0;
        }

        public static decimal? GetNullableDecimalVal(this DataRow row, string columnName)
        {
            decimal val = 0;
            if (decimal.TryParse(row[columnName].ToString(), out val))
                return val;
            else
                return null;
        }

        public static int? GetNullableIntVal(this DataRow row, int columnIndex)
        {
            int val = 0;
            if (Int32.TryParse(row[columnIndex].ToString(), out val))
                return val;
            else
                return null;
        }

        public static double GetDoubleVal(this DataRow row, string columnName)
        {
            double? val = GetNullableDoubleVal(row, columnName);
            return (val.HasValue) ? val.Value : 0.0;
        }

        public static double GetDoubleVal(this DataRow row, int columnIndex)
        {
            double val = 0.0;
            if (row[columnIndex] != DBNull.Value)
                Double.TryParse(row[columnIndex].ToString(), out val);
            return val;
        }

        public static DateTime GetDateTimeVal(this DataRow row, string columnName)
        {
            if (!row.Table.Columns.Contains(columnName)) return default(DateTime);
            DateTime? dtNullable = SrtsHelper.ParseValue(row[columnName].ToString());
            return (dtNullable.HasValue) ? dtNullable.Value : new DateTime();
        }

        public static DateTime GetDateTimeVal(this DataRow row, int columnIndex)
        {
            DateTime? dtNullable = SrtsHelper.ParseValue(row[columnIndex].ToString());
            return (dtNullable.HasValue) ? dtNullable.Value : new DateTime();
        }

        public static DateTime? GetNullableDateTimeVal(this DataRow row, string columnName)
        {
            if (!row.Table.Columns.Contains(columnName)) return default(DateTime?);
            return SrtsHelper.ParseValue(row[columnName]);
        }

        public static DateTime? GetNullableDateTimeVal(this DataRow row, int columnIndex)
        {
            return SrtsHelper.ParseValue(row[columnIndex]);
        }
    }
}