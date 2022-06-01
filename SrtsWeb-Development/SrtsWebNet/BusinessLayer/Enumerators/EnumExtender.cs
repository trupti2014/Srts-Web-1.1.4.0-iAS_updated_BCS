using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Reflection;

namespace EligibilityAndFrames
{
    public static partial class SrtsExtender
    {
        public static string GetDescription(Enum value)
        {
            FieldInfo fieldInfo = value.GetType().GetField(value.ToString());
            DescriptionAttribute[] attributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return (attributes.Length > 0) ? attributes[0].Description : value.ToString();
        }

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

        public static List<string> GetEnumList(Type _type)
        {
            List<string> typeList = new List<string>();

            foreach (var value in Enum.GetValues(_type))
            {
                typeList.Add(value.ToString());
            }
            return typeList;
        }
    }
}