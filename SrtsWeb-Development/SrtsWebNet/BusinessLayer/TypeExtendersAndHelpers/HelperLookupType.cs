using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using SrtsWeb.Entities;

namespace SrtsWeb.BusinessLayer.TypeExtendersAndHelpers.Helpers
{
    public static partial class SrtsHelper
    {
        public static Dictionary<string, string> GetLookupTypes(List<LookupTableEntity> dt)
        {
            var query = (from a in dt
                         select new
                         {
                             Code = a.Code,
                             Value = a.Code
                         }).GroupBy(x => x.Code).Select(x => x.FirstOrDefault());
            Dictionary<string, string> _data = new Dictionary<string, string>();
            foreach (var dr in query)
            {
                _data.Add(dr.Code, dr.Value);
            }
            return _data;
        }

        public static DataTable GetLookupTypesSelected(List<LookupTableEntity> dt, string _type)
        {
            string sortField = "Text";

            var d = new DataTable();
            foreach (var p in typeof(Entities.LookupTableEntity).GetProperties())
                d.Columns.Add(p.Name, p.PropertyType);

            if (_type == "AcuityType")
            {
                sortField = "Description";
            }

            var query = from a in dt
                        where a.Code == _type
                        orderby sortField
                        select new Entities.LookupTableEntity
                        {
                            Code = a.Code,
                            DateLastModified = a.DateLastModified,
                            Description = a.Description,
                            Id = a.Id,
                            IsActive = a.IsActive,
                            ModifiedBy = a.ModifiedBy,
                            Text = a.Text,
                            Value = a.Value
                        };

            foreach (var a in query)
            {
                DataRow row = d.NewRow();
                foreach (DataColumn c in d.Columns)
                    row[c.ColumnName] = a.GetType().GetProperty(c.ColumnName).GetValue(a, null);
                d.Rows.Add(row);
            }

            return d;
        }
    }
}