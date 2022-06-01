using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace SrtsWeb.ExtendersHelpers
{
    public static partial class Helpers
    {
        #region DATETIME

        public static bool CheckDateForGoodDate(DateTime? _checkDate)
        {
            if (!_checkDate.HasValue || _checkDate == DateTime.Parse("01/01/1900"))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static DateTime SetDateOrDefault(DateTime? _checkDate)
        {
            if (_checkDate.HasValue && _checkDate != DateTime.Parse("01/01/1900"))
            {
                return (DateTime)_checkDate;
            }
            else
            {
                return DateTime.Parse("01/01/1900");
            }
        }

        #endregion DATETIME

        #region DEMOGRAPHIC

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

        #endregion DEMOGRAPHIC

        #region ENUMS

        /// <summary>
        ///
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetDescription(Enum value)
        {
            FieldInfo fieldInfo = value.GetType().GetField(value.ToString());
            DescriptionAttribute[] attributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return (attributes.Length > 0) ? attributes[0].Description : value.ToString();
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumString"></param>
        /// <returns></returns>
        public static T ConvertStringToEnum<T>(string enumString)
        {
            try
            {
                return (T)Enum.Parse(typeof(T), enumString, true);
            }
            catch (Exception ex)
            {
                T temp = default(T);
                String s = String.Format("'{0}' is not a valid enumeration of '{1}'", enumString, temp.GetType().Name);
                throw new Exception(s, ex);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="_type"></param>
        /// <returns></returns>
        public static List<KeyValuePair<string, string>> GetEnumDictionaryDescText(Type _type)
        {
            List<KeyValuePair<string, string>> kvPairList = new List<KeyValuePair<string, string>>();

            foreach (var value in Enum.GetValues(_type))
            {
                FieldInfo info = value.GetType().GetField(value.ToString());
                var valueDescription = (DescriptionAttribute[])info.GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (valueDescription.Length == 1)
                {
                    kvPairList.Add(new KeyValuePair<string, string>(value.ToString(), valueDescription[0].Description));
                }
                else
                {
                    kvPairList.Add(new KeyValuePair<string, string>(value.ToString(), value.ToString()));
                }
            }
            return kvPairList;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="_type"></param>
        /// <returns></returns>
        public static DataTable GetEnumDictionaryDescValue(Type _type)
        {
            List<KeyValuePair<int, string>> kvPairList = new List<KeyValuePair<int, string>>();
            DataTable dt = new DataTable();
            dt.Columns.Add("Key");
            dt.Columns.Add("Value");
            foreach (Enum enumValue in Enum.GetValues(_type))
            {
                kvPairList.Add(new KeyValuePair<int, string>(Convert.ToInt32(enumValue), GetDescription(enumValue)));
                DataRow dr = dt.NewRow();
                dr["Value"] = GetDescription(enumValue);
                dr["Key"] = Convert.ToInt32(enumValue);
                dt.Rows.Add(dr);
            }
            return dt;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="_type"></param>
        /// <returns></returns>
        public static List<string> GetEnumList(Type _type)
        {
            List<string> typeList = new List<string>();

            foreach (var value in Enum.GetValues(_type))
            {
                typeList.Add(value.ToString());
            }
            return typeList;
        }

        #endregion ENUMS

        #region EXCEPTIONS

        public static void LogElmahMessage(String customMessage)
        {
            try
            {
                if (String.IsNullOrEmpty(customMessage)) return;
                Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception(customMessage));
            }
            catch
            {
                // Do Nothing
            }
        }

        #endregion EXCEPTIONS

        #region STRING

        public static String GetRandomPwd(Int32 totalStrLength)
        {
            var newP = new StringBuilder();

            var alpha = new List<Char>() { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };
            var numeric = new List<Int32>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            var special = new List<Char> { '&', '!', '@', '#', '$', '%', '^', '*', '(', ')' };

            var all = new List<String>();
            all.AddRange(alpha.ConvertAll<String>(x => x.ToString()));
            all.AddRange(numeric.ConvertAll<String>(x => x.ToString()));
            all.AddRange(alpha.ConvertAll<String>(x => x.ToString().ToUpper()));
            all.AddRange(special.ConvertAll<String>(x => x.ToString()));

            Random r;

            r = new Random();
            newP.AppendFormat("{0}{1}", alpha[r.Next(alpha.Count)], alpha[r.Next(alpha.Count)]);
            newP.AppendFormat("{0}{1}", numeric[r.Next(numeric.Count)], numeric[r.Next(numeric.Count)]);
            newP.AppendFormat("{0}{1}", alpha[r.Next(alpha.Count)].ToString().ToUpper(), alpha[r.Next(alpha.Count)].ToString().ToUpper());
            newP.AppendFormat("{0}{1}", special[r.Next(special.Count)], special[r.Next(special.Count)]);

            if (totalStrLength < newP.Length) throw new Exception("Total string length input argument must be greater than or equal to 8.");
            var len = totalStrLength - newP.Length;
            for (int i = 0; i < len; i++)
                newP.Append(all[r.Next(all.Count)]);

            var randomizableList = new SortedDictionary<Guid, String>();
            foreach (var c in newP.ToString())
                randomizableList.Add(Guid.NewGuid(), c.ToString());
            newP.Clear();
            foreach (var a in randomizableList)
                newP.Append(a.Value);

            return newP.ToString();
        }

        public static string ToTitleCase(string text)
        {
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            return textInfo.ToTitleCase(text.ToLower());
        }

        #endregion STRING

        #region VARIOUS

        public static string GetIDTypeDescription(string _key)
        {
            try
            {
                if (String.IsNullOrEmpty(_key)) return String.Empty;

                Dictionary<string, string> idTypes = GetIdentificationTypes();

                if (idTypes.IsNullOrEmpty()) return String.Empty;
                return idTypes[_key] ?? String.Empty;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                LogElmahMessage("Error in GetIDTypeDescription...");
                return string.Empty;
            }
        }

        public static Dictionary<string, string> GetIdentificationTypes()
        {
            if (System.Web.HttpContext.Current.Cache["SRTSLOOKUP"].IsNull()) return null;

            var lts = System.Web.HttpContext.Current.Cache["SRTSLOOKUP"] as List<SrtsWeb.Entities.LookupTableEntity>;

            return lts.Where(x => x.Code == LookupType.IDNumberType.ToString()).ToDictionary((Key) => Key.Value, (Value) => Value.Text);
        }

        public static string EncodePassword(string pass, string salt, string hashingAlgorithm)
        {
            var bytes = Encoding.Unicode.GetBytes(pass);
            var src = Convert.FromBase64String(salt);
            var dst = new byte[src.Length + bytes.Length];
            Buffer.BlockCopy(src, 0, dst, 0, src.Length);
            Buffer.BlockCopy(bytes, 0, dst, src.Length, bytes.Length);
            var algorithm = HashAlgorithm.Create(hashingAlgorithm);
            var inArray = algorithm.ComputeHash(dst);
            return Convert.ToBase64String(inArray);
        }

        public static String GenSha1String(String stringIn)
        {
            //byte[] bytes = Encoding.Unicode.GetBytes(pass);
            ////byte[] src = Encoding.Unicode.GetBytes(salt); Corrected 5/15/2013
            //byte[] src = Convert.FromBase64String(salt);
            //byte[] dst = new byte[src.Length + bytes.Length];
            //Buffer.BlockCopy(src, 0, dst, 0, src.Length);
            //Buffer.BlockCopy(bytes, 0, dst, src.Length, bytes.Length);
            //HashAlgorithm algorithm = HashAlgorithm.Create("SHA1");
            //byte[] inArray = algorithm.ComputeHash(dst);
            //return Convert.ToBase64String(inArray);

            var sha1 = System.Security.Cryptography.SHA1.Create();
            var ba = Encoding.Unicode.GetBytes(stringIn);
            var hashBa = sha1.ComputeHash(ba, 0, ba.Length);
            return Convert.ToBase64String(hashBa);
        }

        public static string GetCountryDescription(string countryCode)
        {
            if (String.IsNullOrEmpty(countryCode)) return String.Empty;
            if (System.Web.HttpContext.Current.Cache["SRTSLOOKUP"].IsNull()) return null;
            var lookupTypes = System.Web.HttpContext.Current.Cache["SRTSLOOKUP"] as List<SrtsWeb.Entities.LookupTableEntity>;
            var country = from a in lookupTypes
                          where a.Code.ToString() == "CountryList"
                          select a;
            return country.Where(x => x.Value == countryCode)
                .Select(x => x.Text)
                .First();
        }

        #endregion VARIOUS

        #region DATATABLE

        public static DataTable ToTable(this object obj)
        {
            DataTable dt = new DataTable();
            Type myType = obj.GetType();
            foreach (PropertyInfo prop in myType.GetProperties())
            {
                dt.Columns.Add(new DataColumn(prop.Name));
            }
            DataRow dr = dt.NewRow();
            foreach (PropertyInfo prop in myType.GetProperties())
            {
                dr[prop.Name] = prop.GetValue(obj, null);
            }
            dt.Rows.Add(dr);
            return dt;
        }

        #endregion DATATABLE
    }
}