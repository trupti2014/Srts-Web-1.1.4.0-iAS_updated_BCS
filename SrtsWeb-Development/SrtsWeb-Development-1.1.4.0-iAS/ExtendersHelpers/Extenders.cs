using Elmah;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Profile;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SrtsWeb.ExtendersHelpers
{
    public enum DatePart { Year, Month, Day };

    /// <summary>
    /// A custom partial class to make extension methods available.
    /// </summary>
    public static partial class Extenders
    {
        #region DATA

        /// <summary>
        /// Converts a value from a data reader by column name into int32.
        /// </summary>
        /// <param name="recIn">Data reader that contains the data to manipulate</param>
        /// <param name="columnName">Data column name of data to find in the data reader.</param>
        /// <param name="columnNames">Optional list of column names in the data reader used to determine if the provided column name exists.</param>
        /// <returns>Value of data item in the specified column in the reader as a int32</returns>
        public static Int32 ToInt32(this IDataReader recIn, String columnName, List<String> columnNames = null)
        {
            if (recIn == null) return default(Int32);
            if (columnNames != null)
                if (!columnNames.Contains(columnName)) return default(Int32);
            Int32 i;
            return Int32.TryParse(recIn[columnName].ToString(), out i) ? i : default(Int32);
        }

        /// <summary>
        /// Converts a value from a data reader by column name into decimal.
        /// </summary>
        /// <param name="recIn">Data reader that contains the data to manipulate</param>
        /// <param name="columnName">Data column name of data to find in the data reader.</param>
        /// <param name="columnNames">Optional list of column names in the data reader used to determine if the provided column name exists.</param>
        /// <returns>Value of data item in the specified column in the reader as a decimal.</returns>
        public static Decimal ToDecimal(this IDataReader recIn, String columnName, List<String> columnNames = null)
        {
            if (recIn == null) return default(Decimal);
            if (columnNames != null)
                if (!columnNames.Contains(columnName)) return default(Decimal);

            return recIn[columnName].ToDecimal();
        }

        /// <summary>
        /// Converts a value from a data reader by column name into double.
        /// </summary>
        /// <param name="recIn">Data reader that contains the data to manipulate</param>
        /// <param name="columnName">Data column name of data to find in the data reader.</param>
        /// <param name="columnNames">Optional list of column names in the data reader used to determine if the provided column name exists.</param>
        /// <returns>Value of data item in the specified column in the reader as a double.</returns>
        public static Double ToDouble(this IDataReader recIn, String columnName, List<String> columnNames = null)
        {
            if (recIn == null) return default(Double);
            if (columnNames != null)
                if (!columnNames.Contains(columnName)) return default(Double);

            return recIn[columnName].ToDouble();
        }

        /// <summary>
        /// Converts a value from a data reader by column name into date time.
        /// </summary>
        /// <param name="recIn">Data reader that contains the data to manipulate</param>
        /// <param name="columnName">Data column name of data to find in the data reader.</param>
        /// <param name="columnNames">Optional list of column names in the data reader used to determine if the provided column name exists.</param>
        /// <returns>Value of data item in the specified column in the reader as a date time.</returns>
        public static DateTime ToDateTime(this IDataReader recIn, String columnName, List<String> columnNames = null)
        {
            if (recIn == null) return default(DateTime);
            if (columnNames != null)
                if (!columnNames.Contains(columnName)) return default(DateTime);

            return recIn[columnName].ToDateTime();
        }

        /// <summary>
        /// Converts a value from a data reader by column name into boolean.
        /// </summary>
        /// <param name="recIn">Data reader that contains the data to manipulate</param>
        /// <param name="columnName">Data column name of data to find in the data reader.</param>
        /// <param name="columnNames">Optional list of column names in the data reader used to determine if the provided column name exists.</param>
        /// <returns>Value of data item in the specified column in the reader as a boolean.</returns>
        public static Boolean ToBoolean(this IDataReader recIn, String columnName, List<String> columnNames = null)
        {
            if (recIn == null) return default(Boolean);
            if (columnNames != null)
                if (!columnNames.Contains(columnName)) return default(Boolean);

            return recIn[columnName].ToBoolean();
        }

        /// <summary>
        /// Converts a value from a data reader by column name into string.
        /// </summary>
        /// <param name="recIn">Data reader that contains the data to manipulate</param>
        /// <param name="columnName">Data column name of data to find in the data reader.</param>
        /// <param name="columnNames">Optional list of column names in the data reader used to determine if the provided column name exists.</param>
        /// <returns>Value of data item in the specified column in the reader as a string.</returns>
        public static String AsString(this IDataReader recIn, String columnName, List<String> columnNames = null)
        {
            if (recIn == null) return String.Empty;
            if (columnNames != null)
                if (!columnNames.Contains(columnName)) return String.Empty;

            return recIn[columnName].ToString();
        }

        /// <summary>
        /// Converts a value from a data reader by column name into nullable date time.
        /// </summary>
        /// <param name="recIn">Data reader that contains the data to manipulate</param>
        /// <param name="columnName">Data column name of data to find in the data reader.</param>
        /// <param name="columnNames">Optional list of column names in the data reader used to determine if the provided column name exists.</param>
        /// <returns>Value of data item in the specified column in the reader as a nullable date time.</returns>
        public static DateTime? ToNullableDateTime(this IDataReader recIn, String columnName, List<String> columnNames = null)
        {
            if (recIn == null) return null;
            if (!columnNames.IsNull())
                if (!columnNames.Contains(columnName)) return null;

            return recIn[columnName].ToNullableDateTime();
        }

        /// <summary>
        /// Gets a list of column names from a supplied data reader.
        /// </summary>
        /// <param name="dr">Data reader to get column names from.</param>
        /// <returns>List of column names.</returns>
        public static List<String> GetColumnNameList(this IDataReader dr)
        {
            return Enumerable.Range(0, dr.FieldCount).Select(dr.GetName).ToList();
        }

        public static bool HasColumn(this IDataRecord dr, string columnName)
        {
            for (int i = 0; i < dr.FieldCount; i++)
            {
                if (dr.GetName(i).Equals(columnName, StringComparison.InvariantCultureIgnoreCase))
                    return true;
            }
            return false;
        }


        /// <summary>
        /// Check if data is null
        /// </summary>
        /// <param name="dataReader"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public static bool IsDBNull(this IDataReader dataReader, string columnName)
        {
            return dataReader[columnName] == DBNull.Value;
        }

        #region DATAROW - THESE WILL GET DELETED EVENTUALLY

        public static String AsString(this DataRow rowIn, String columnName)
        {
            if (rowIn == null) return String.Empty;

            if (!rowIn.Table.Columns.Contains(columnName)) return String.Empty;

            return rowIn[columnName].ToString();
        }

        public static Int32 ToInt32(this DataRow rowIn, String columnName)
        {
            if (rowIn == null) return default(Int32);
            if (!rowIn.Table.Columns.Contains(columnName)) return default(Int32);

            return rowIn[columnName].ToInt32();
        }

        public static Decimal ToDecimal(this DataRow rowIn, String columnName)
        {
            if (rowIn == null) return default(Decimal);
            if (!rowIn.Table.Columns.Contains(columnName)) return default(Decimal);

            return rowIn[columnName].ToDecimal();
        }

        public static Double ToDouble(this DataRow rowIn, String columnName)
        {
            if (rowIn == null) return default(Double);
            if (!rowIn.Table.Columns.Contains(columnName)) return default(Double);

            return rowIn[columnName].ToDouble();
        }

        public static DateTime ToDateTime(this DataRow rowIn, String columnName)
        {
            if (rowIn == null) return default(DateTime);
            if (!rowIn.Table.Columns.Contains(columnName)) return default(DateTime);

            return rowIn[columnName].ToDateTime();
        }

        public static Boolean ToBoolean(this DataRow rowIn, String columnName)
        {
            if (rowIn == null) return default(Boolean);
            if (!rowIn.Table.Columns.Contains(columnName)) return default(Boolean);

            return rowIn[columnName].ToBoolean();
        }

        public static DateTime? ToNullableDateTime(this DataRow rowIn, String columnName)
        {
            if (rowIn == null) return null;
            if (!rowIn.Table.Columns.Contains(columnName)) return null;

            return rowIn[columnName].ToNullableDateTime();
        }

        public static Decimal? ToNullableDecimal(this DataRow rowIn, String columnName)
        {
            if (rowIn == null) return null;
            if (!rowIn.Table.Columns.Contains(columnName)) return null;

            return rowIn[columnName].ToNullableDecimal();
        }

        public static Int32? ToNullableInt32(this DataRow rowIn, String columnName)
        {
            if (rowIn == null) return null;
            if (!rowIn.Table.Columns.Contains(columnName)) return null;

            return rowIn[columnName].ToNullableInt32();
        }

        #endregion DATAROW - THESE WILL GET DELETED EVENTUALLY

        #endregion DATA

        #region DATETIME
        /// <summary>
        /// Converts a date time to a military formatted string.
        /// </summary>
        /// <param name="dt">Date time to convert.</param>
        /// <returns>Military formatted string.</returns>
        public static string ToStringMil(this DateTime dt)
        {
            return dt.ToString(Globals.DtFmt).ToUpper();
        }

        /// <summary>
        /// Converts a nullable date time to a military formatted string.
        /// </summary>
        /// <param name="ndt">Nullable Date time to convert.</param>
        /// <returns>Military formatted string.</returns>
        public static string ToStringMil(this DateTime? ndt)
        {
            if (ndt.HasValue)
                return ndt.Value.ToString(Globals.DtFmt).ToUpper();
            else
                return string.Empty;
        }
        
        /// <summary>
        /// Compares two dates and returns the difference as a double.  The return value will always be positive.
        /// </summary>
        /// <param name="dt">Original date time value.</param>
        /// <param name="dtCompare">Date time to compare to.</param>
        /// <returns>Difference between the dates as a double.</returns>
        public static Double DateDiff(this DateTime dt, DateTime dtCompare)
        {
            TimeSpan ts;
            if (dtCompare > dt)
                ts = dtCompare - dt;
            else
                ts = dt - dtCompare;
            return Math.Round(ts.TotalDays);
        }

        #endregion DATETIME

        #region DEMOGRAPHIC

        /// <summary>
        /// Gets the rank key portion of the profile string.
        /// </summary>
        /// <param name="profile">Person's profile string.</param>
        /// <returns>Rank key string.</returns>
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

        /// <summary>
        /// Gets the rank value from the profile string.
        /// </summary>
        /// <param name="profile">Person's profile string.</param>
        /// <returns>Rank value string.</returns>
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

        /// <summary>
        /// Gets the status key portion of the profile string.
        /// </summary>
        /// <param name="profile">Person's profile string.</param>
        /// <returns>Status key string.</returns>
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

        /// <summary>
        /// Gets the status value from the profile string.
        /// </summary>
        /// <param name="profile">Person's profile string.</param>
        /// <returns>Status value string.</returns>
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

        /// <summary>
        /// Gets the order priority key portion of the profile string.
        /// </summary>
        /// <param name="profile">Person's profile string.</param>
        /// <returns>Order priority key string.</returns>
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

        /// <summary>
        /// Gets the order priority value from the profile string.
        /// </summary>
        /// <param name="profile">Person's profile string.</param>
        /// <returns>Order priority value string.</returns>
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

        /// <summary>
        /// Gets the BOS key portion of the profile string.
        /// </summary>
        /// <param name="profile">Person's profile string.</param>
        /// <returns>BOS key string.</returns>
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

        /// <summary>
        /// Gets the BOS value from the profile string.
        /// </summary>
        /// <param name="profile">Person's profile string.</param>
        /// <returns>BOS value string.</returns>
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

        /// <summary>
        /// Gets the gender key portion of the profile string.
        /// </summary>
        /// <param name="profile">Person's profile string.</param>
        /// <returns>Gender key string.</returns>
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

        /// <summary>
        /// Gets the gender value from the profile string.
        /// </summary>
        /// <param name="profile">Person's profile string.</param>
        /// <returns>Gender value string.</returns>
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

        /// <summary>
        /// Gets the Rx value from rxName
        /// </summary>
        /// <param name="rxName">rxName string</param>
        /// <returns>resultRxName string</returns>
        public static string ToRxUserFriendlyName(this string rxName)
        {
            string resultRxName = string.Empty;

            switch (rxName)
            {
                case "FTW":
                    resultRxName = "Full time Wear";
                    break;
                case "NVO":
                    resultRxName = "Near";
                    break;
                case "BI":
                    resultRxName = "Bifocal";
                    break;
                case "DVO":
                    resultRxName = "Distance";
                    break;
                default:
                    resultRxName = "N/A";
                    break;
            }

            return resultRxName;
        }

        /// <summary>
        /// Gets the frameName from the frame category
        /// </summary>
        /// <param name="frameCategory">frameCategory string</param>
        /// <param name="frameCode">frameCode string</param>
        /// <returns>userFriendlyName string</returns>
        public static string ToUserFriendlyFrameName(this string frameCategory, string frameCode)
        {
            string userFriendlyName = string.Empty;

            switch(frameCategory)
            {
                case "FOC":
                    userFriendlyName = "Frame of choice glasses";
                    break;
                case "Standard":
                    userFriendlyName = "Standard issue glasses";
                    break;
                case "A1000":
                    userFriendlyName = "A1000";
                    break;
                case "Aviator":
                    userFriendlyName = "Aviator glasses";
                    break;
                case "PMI":
                    userFriendlyName = "Protective mask insert";
                    break;
                case "ARS":
                    userFriendlyName = "ARS";
                    break;
                case "AV":
                    userFriendlyName = "AV";
                    break;
                case "UPLC":
                    userFriendlyName = "MCEP inserts";
                    break;
                default:
                    userFriendlyName = frameCode;
                    break;
            }

            return userFriendlyName;
        }

        #endregion DEMOGRAPHIC

        #region EXCEPTIONS

        /// <summary>
        /// Adds exception to trace log.
        /// </summary>
        /// <param name="exIn">Exception object.</param>
        public static void TraceErrorException(this Exception exIn)
        {
            try
            {
                TraceErrorException(exIn, String.Empty);
            }
            catch
            {
                // Do Nothing
            }
        }

        /// <summary>
        /// Adds exception and additional message to trace log.
        /// </summary>
        /// <param name="exIn">Exception object.</param>
        /// <param name="optionalMessage">Optional message to add to trace log.</param>
        public static void TraceErrorException(this Exception exIn, String optionalMessage)
        {
            try
            {
                var _TraceSource = new System.Diagnostics.TraceSource(SrtsTraceSource.ErrorSource.ToString());
                if (!_TraceSource.Switch.ShouldTrace(System.Diagnostics.TraceEventType.Error)) return;

                _TraceSource.TraceData(System.Diagnostics.TraceEventType.Error, 1, String.Format("Exception: {0}{1}Exception Message: {2}{3}Stack Trace: {4}{5}Optional Message: {6}",
                    exIn, Environment.NewLine, exIn.Message, Environment.NewLine, exIn.StackTrace, Environment.NewLine, optionalMessage));
            }
            catch
            {
                // Do Nothing
            }
        }

        /// <summary>
        /// Adds exception to ELMAH log.
        /// </summary>
        /// <param name="exIn">Exception object.</param>
        public static void LogException(this Exception exIn)
        {
            try
            {
                LogException(exIn, String.Empty);
            }
            catch
            {
                // Do Nothing
            }
        }

        /// <summary>
        /// Adds exception and additional message to ELMAH log.
        /// </summary>
        /// <param name="exIn">Exception object.</param>
        /// <param name="optionalMessage">Optional message to log.</param>
        public static void LogException(this Exception exIn, String optionalMessage)
        {
            try
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(exIn);
                if (String.IsNullOrEmpty(optionalMessage)) return;
                Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception(optionalMessage));
            }
            catch
            {
                // Do Nothing
            }
        }
        #endregion

        #region GENERIC

        /// <summary>
        /// Checks for a null object T and converts it to the default type value of the type T.  If not null then returns the input value.
        /// </summary>
        /// <typeparam name="T">Generic object of type T.</typeparam>
        /// <param name="tIn">Generic object of type T.</param>
        /// <returns>Default value of type T or input value.</returns>
        public static T ToDefault<T>(this T tIn)
        {
            if (tIn == null) return default(T);
            return tIn;
        }

        /// <summary>
        /// Converts an inbount object to an HttpUtility.HtmlDecode string.
        /// </summary>
        /// <typeparam name="T">Generic object of type T.</typeparam>
        /// <param name="tIn">Generic object of type T.</param>
        /// <returns>HttpUtility.HtmlDecode string.</returns>
        public static String ToHtmlDecodeString<T>(this T tIn)
        {
            if ((tIn is object && tIn.GetType().IsValueType) || tIn is String)
                return tIn == null ? "" : HttpUtility.HtmlDecode(tIn.ToString());
            else
                return "";
        }

        /// <summary>
        /// Converts an inbount object to an HttpUtility.HtmlEncode string.
        /// </summary>
        /// <typeparam name="T">Generic object of type T.</typeparam>
        /// <param name="tIn">Generic object of type T.</param>
        /// <returns>HttpUtility.HtmlEncode string.</returns>
        public static String ToHtmlEncodeString<T>(this T tIn)
        {
            if ((tIn is object && tIn.GetType().IsValueType) || tIn is String)
                return tIn == null ? "" : HttpUtility.HtmlEncode(tIn.ToString());
            else
                return "";
        }

        /// <summary>
        /// Checks if an inbound obect list is null or empty.
        /// </summary>
        /// <typeparam name="T">Generic object of type T.</typeparam>
        /// <param name="tIn">Generic object of type T.</param>
        /// <returns>True/false</returns>
        public static Boolean IsNullOrEmpty<T>(this IEnumerable<T> listIn)
        {
            return listIn == null || listIn.Count().Equals(0);
        }

        /// <summary>
        /// Checks if an inbound object is null.
        /// </summary>
        /// <typeparam name="T">Generic object of type T.</typeparam>
        /// <param name="tIn">Generic object of type T.</param>
        /// <returns>True/false</returns>
        public static Boolean IsNull<T>(this T tIn)
        {
            return tIn == null;
        }

        /// <summary>
        /// Checks if an inbound object list is null.
        /// </summary>
        /// <typeparam name="T">Generic list of type T.</typeparam>
        /// <param name="listIn">Generic list of type T.</param>
        /// <returns>True/false</returns>
        public static Boolean IsNull<T>(this IEnumerable<T> listIn)
        {
            return listIn == null;
        }

        /// <summary>
        /// Checks to see if an inbound value of T is in an obect array.
        /// </summary>
        /// <typeparam name="T">Generic object of type T.</typeparam>
        /// <param name="tIn">Generic object of type T.</param>
        /// <param name="paramList">List of object for comparison.</param>
        /// <returns>True/false</returns>
        public static Boolean In<T>(this T tIn, params object[] paramList)
        {
            return paramList.Contains(tIn);
        }

        /// <summary>
        /// Converts an inbound object into a SHA1 hash string.
        /// </summary>
        /// <typeparam name="T">Generic object of type T.</typeparam>
        /// <param name="tIn">Generic object of type T.</param>
        /// <returns>SHA1 hash string.</returns>
        public static String GetObjectHash<T>(this T tIn)
        {
            using (var ms = new System.IO.MemoryStream())
            {
                var bFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                bFormatter.Serialize(ms, tIn);
                var bArray = ms.ToArray();

                var algorithm = System.Security.Cryptography.HashAlgorithm.Create("SHA1");

                var inArray = algorithm.ComputeHash(bArray);
                return Convert.ToBase64String(inArray);
            }
        }

        #endregion GENERIC

        #region NUMBERS

        /// <summary>
        /// 
        /// </summary>
        /// <param name="IntIn"></param>
        /// <param name="lowBounds"></param>
        /// <param name="highBounds"></param>
        /// <param name="inclusive"></param>
        /// <returns></returns>
        public static Boolean Between(this int IntIn, int lowBounds, int highBounds, Boolean inclusive = true)
        {
            if (inclusive)
                return IntIn.GreaterThanET(lowBounds) && IntIn.LessThanET(highBounds);
            else
                return IntIn.GreaterThan(lowBounds) && IntIn.LessThan(highBounds);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="intIn"></param>
        /// <param name="compareInt"></param>
        /// <returns></returns>
        public static Boolean GreaterThan(this int intIn, int compareInt)
        {
            return intIn > compareInt;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="intIn"></param>
        /// <param name="compareInt"></param>
        /// <returns></returns>
        public static Boolean GreaterThanET(this int intIn, int compareInt)
        {
            return intIn >= compareInt;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="intIn"></param>
        /// <param name="compareInt"></param>
        /// <returns></returns>
        public static Boolean LessThan(this int intIn, int compareInt)
        {
            return intIn < compareInt;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="intIn"></param>
        /// <param name="compareInt"></param>
        /// <returns></returns>
        public static Boolean LessThanET(this int intIn, int compareInt)
        {
            return intIn <= compareInt;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="IntIn"></param>
        /// <param name="lowBounds"></param>
        /// <param name="highBounds"></param>
        /// <param name="inclusive"></param>
        /// <returns></returns>
        public static Boolean Between(this Decimal IntIn, Decimal lowBounds, Decimal highBounds, Boolean inclusive = true)
        {
            if (inclusive)
                return IntIn.GreaterThanET(lowBounds) && IntIn.LessThanET(highBounds);
            else
                return IntIn.GreaterThan(lowBounds) && IntIn.LessThan(highBounds);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="intIn"></param>
        /// <param name="compareInt"></param>
        /// <returns></returns>
        public static Boolean GreaterThan(this Decimal intIn, Decimal compareInt)
        {
            return intIn > compareInt;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="intIn"></param>
        /// <param name="compareInt"></param>
        /// <returns></returns>
        public static Boolean GreaterThanET(this Decimal intIn, Decimal compareInt)
        {
            return intIn >= compareInt;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="intIn"></param>
        /// <param name="compareInt"></param>
        /// <returns></returns>
        public static Boolean LessThan(this Decimal intIn, Decimal compareInt)
        {
            return intIn < compareInt;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="intIn"></param>
        /// <param name="compareInt"></param>
        /// <returns></returns>
        public static Boolean LessThanET(this Decimal intIn, Decimal compareInt)
        {
            return intIn <= compareInt;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="numIn"></param>
        /// <returns></returns>
        public static string ToStringWithSign(this decimal numIn)
        {
            var n = numIn.ToString();
            if (n.Substring(0, 1).Equals("-"))
                return n;
            return string.Format("+{0}", n);
        }

        #endregion INT

        #region LISTBOX

        public static ListBox Sort(this ListBox listIn)
        {
            var l = new SortedList();
            listIn.Items.Cast<ListItem>().ToList().ForEach(x => l.Add(x.Text, x.Value));
            listIn.Items.Clear();
            foreach (DictionaryEntry li in l)
                listIn.Items.Add(new ListItem(li.Key.ToString(), li.Value.ToString()));
            return listIn;
        }

        public static List<ListItem> GetItemsList(this ListBox listIn)
        {
            if (listIn.Items.Count.Equals(0)) return new List<ListItem>() { new ListItem() };
            return listIn.Items.Cast<ListItem>().ToList();
        }

        #endregion LISTBOX

        #region LIST<Tuple>

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

        #endregion LIST<Tuple>

        #region MEMBERSHIP

        public static MembershipUserCollection GetFilteredUsers(this MembershipUserCollection collIn, String filterPropertyName, Object filterValue)
        {
            MembershipUserCollection mc = new MembershipUserCollection();
            foreach (MembershipUser mu in collIn)
            {
                var fv = filterValue;

                var tv = mu.GetType().GetProperty(filterPropertyName,
                    System.Reflection.BindingFlags.IgnoreCase |
                    System.Reflection.BindingFlags.Public |
                    System.Reflection.BindingFlags.Instance).GetValue(mu, null);

                if (!tv.ToString().ToLower().Equals(fv.ToString().ToLower())) continue;
                mc.Add(mu);
            }
            return mc;
        }

        public static MembershipUserCollection GetUsersWithProfiles(this MembershipUserCollection collIn)
        {
            MembershipUserCollection m = new MembershipUserCollection();
            foreach (MembershipUser u in collIn)
            {
                var p = ProfileBase.Create(u.UserName);
                var pers = p.GetPropertyValue("Personal");
                if (pers.GetType().GetProperty("SiteCode").GetValue(pers, null) == null) continue;
                m.Add(u);
            }

            return m;
        }

        #endregion MEMBERSHIP

        #region OBJECT

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

        #endregion OBJECT

        #region STRING

        public static string ToSSNRemoveDash(this string ssn)
        {
            if (string.IsNullOrEmpty(ssn)) return string.Empty;
            return ssn.Replace("-", "");
        }

        public static string ToZipCodeDisplay(this string zipCode)
        {
            if (string.IsNullOrEmpty(zipCode)) return string.Empty;

            if (!zipCode.Contains("-"))
            {
                if (zipCode.Length >= 8)
                {
                    return zipCode.Insert(5, "-");
                }
                else
                {
                    //return string.Format("{0}-0000", zipCode);
                    return zipCode;
                }
            }
            else
                return zipCode;
        }

        public static string ToZipCodeRemoveDash(this string zip)
        {
            if (string.IsNullOrEmpty(zip)) return string.Empty;
            return zip.Replace("-", "");
        }

        public static string ToZipCodeLabelPrint(this string zipCode)
        {
            if (zipCode.Contains("-0000"))
            {
                return zipCode.Substring(0, 5);
            }
            else
            {
                return zipCode;
            }
        }

        #endregion STRING

        #region VALIDATION

        public static Boolean ValidatePasswordComplexity(this String stringToValidate, out String ErrorMsg)
        {
            var err = new StringBuilder();
            var p = stringToValidate.Trim();
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

        public static Boolean ValidatePasswordCharacters(this String stringtovalidate)
        {
            var p = stringtovalidate.Trim();

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
            if (date.ToDateTime() > DateTime.Today)
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

        #endregion VALIDATION

        #region VIEW

        public static T ResetViewProperties<T>(this T tIn)
        {
            if (tIn == null) return default(T);

            foreach (PropertyInfo pi in tIn.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (pi.GetSetMethod() != null &&
                    !pi.Name.ToLower().Equals("mysession") &&
                    !pi.Name.ToLower().Equals("lookupcache"))
                {
                    if (pi.GetMethod != null && pi.GetMethod.IsFinal)
                        try { pi.SetValue(tIn, null, null); }
                        catch { }
                }
            }

            return tIn;
        }

        public static T SetControlStateTbDdlRbl<T>(this T collectionIn, Boolean isEnabled)
            where T : ControlCollection
        {
            foreach (Control c in collectionIn)
            {
                if (c.HasControls()) c.Controls.SetControlStateTbDdlRbl(isEnabled);

                if (c is TextBox)
                    ((TextBox)c).Enabled = isEnabled;
                else if (c is DropDownList)
                    ((DropDownList)c).Enabled = isEnabled;
                else if (c is RadioButtonList)
                    ((RadioButtonList)c).Enabled = isEnabled;
            }

            return collectionIn;
        }

        public static T SetControlStateBtn<T>(this T collectionIn, Boolean isEnabled)
            where T : ControlCollection
        {
            foreach (Control c in collectionIn)
            {
                if (c.HasControls()) c.Controls.SetControlStateBtn(isEnabled);

                if (c is Button)
                    ((Button)c).Enabled = isEnabled;
            }

            return collectionIn;
        }

        #endregion VIEW
    }
}