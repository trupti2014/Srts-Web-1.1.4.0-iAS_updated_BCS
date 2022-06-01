using SrtsWeb.BusinessLayer.TypeExtendersAndHelpers.Helpers;
using System;
using System.Data.Common;

namespace SrtsWeb.BusinessLayer.TypeExtendersAndHelpers.Extenders
{
    public static partial class SrtsExtender
    {
        public static string GetStringVal(this DbParameterCollection dbParameters, string parmName)
        {
            return dbParameters[parmName].Value.ToString();
        }

        public static int GetIntVal(this DbParameterCollection dbParameters, string parmName)
        {
            int? val = GetNullableIntVal(dbParameters, parmName);
            return (val.HasValue) ? val.Value : 0;
        }

        public static int? GetNullableIntVal(this DbParameterCollection dbParameters, string parmName)
        {
            int val = 0;
            if (Int32.TryParse(dbParameters[parmName].Value.ToString(), out val))
                return val;
            else
                return null;
        }

        public static double GetDoubleVal(this DbParameterCollection dbParameters, string parmName)
        {
            double val = 0.0;
            double.TryParse(dbParameters[parmName].Value.ToString(), out val);
            return val;
        }

        public static bool GetBoolVal(this DbParameterCollection dbParameters, string parmName)
        {
            string sVal = dbParameters[parmName].Value.ToString().ToUpper();
            return (sVal == "Y" || sVal == "1" || sVal == "TRUE");
        }

        public static bool? GetNullableBoolVal(this DbParameterCollection dbParameters, string parmName)
        {
            string val = dbParameters[parmName].Value.ToString().ToUpper();
            if (val == "Y" || val == "YES" || val == "1" || val == "T" || val == "TRUE")
                return true;
            if (val == "N" || val == "NO" || val == "0" || val == "F" || val == "FALSE")
                return false;
            return null;
        }

        public static DateTime GetDateTimeVal(this DbParameterCollection dbParameters, string parmName)
        {
            DateTime? dtNullable = SrtsHelper.ParseValue(dbParameters[parmName].Value);
            if (dtNullable.HasValue)
                return dtNullable.Value;
            else
                return new DateTime();
        }

        public static DateTime? GetNullableDateTimeVal(this DbParameterCollection dbParameters, string parmName)
        {
            return SrtsHelper.ParseValue(dbParameters[parmName].Value);
        }
    }
}