using SrtsWeb.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SrtsWeb.ExtendersHelpers
{
    /// <summary>
    /// A custom partial class to make entity extension methods available.
    /// </summary>
    public static partial class Extenders
    {
        /// <summary>
        /// Static method to get a list of lookup table items by type.
        /// </summary>
        /// <param name="lIn">LookupTableEntity list of get items from.</param>
        /// <param name="typeToGet">Lookup table type to search for.</param>
        /// <returns>LookupTableEntity list of items matching the type.</returns>
        public static List<LookupTableEntity> GetByType(this List<LookupTableEntity> lIn, String typeToGet)
        {
            string sortField = "Text";
            List<LookupTableEntity> resultList = new List<LookupTableEntity>();
            try
            {
                if (typeToGet == "AcuityType")
                {
                    sortField = "Description";
                }

                resultList = lIn.Where(x => x.Code == typeToGet).OrderBy(x => sortField).ToList();
                return resultList;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                LogException(ex);
            }
           return resultList;
        }
    }
}