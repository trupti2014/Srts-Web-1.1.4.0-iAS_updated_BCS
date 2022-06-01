using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace SrtsWeb.BusinessLayer.TypeExtendersAndHelpers.Extenders
{
    public static partial class SrtsExtender
    {
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

        public static DataTable ToDataTable<T>(List<T> items)
        {
            var tb = new DataTable(typeof(T).Name);
            PropertyInfo[] props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in props)
            {
                Type t = GetCoreType(prop.PropertyType);
                tb.Columns.Add(prop.Name, t);
            }
            foreach (T item in items)
            {
                var values = new object[props.Length];
                for (int i = 0; i < props.Length; i++)
                {
                    values[i] = props[i].GetValue(item, null);
                }
                tb.Rows.Add(values);
            }
            return tb;
        }

        public static bool IsNullable(Type t)
        {
            return !t.IsValueType || (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>));
        }

        public static Type GetCoreType(Type t)
        {
            if (t != null && IsNullable(t))
            {
                if (!t.IsValueType)
                {
                    return t;
                }
                else
                {
                    return Nullable.GetUnderlyingType(t);
                }
            }
            else
            {
                return t;
            }
        }
    }
}