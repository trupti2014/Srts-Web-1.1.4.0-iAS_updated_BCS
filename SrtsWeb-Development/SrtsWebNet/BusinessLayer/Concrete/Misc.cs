using SrtsWeb.DataLayer.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

namespace SrtsWeb.BusinessLayer.Concrete
{
    /// <summary>
    /// A custom class to handle miscellaneous operations.
    /// </summary>
    public static class Misc
    {
        public const String GEYES_SITE_CODE = "009900";
        public const String JSPECS_SITE_CODE = "008094";

        /// <summary>
        /// Gets a value indicating if a site is a clinic site type
        /// </summary>
        /// <param name="_siteCode">Site to get site type for.</param>
        /// <returns>Flag of weather or not the site is a clinic site type.</returns>
        public static Boolean IsSiteClinic(String _siteCode)
        {
            var r = new SiteRepository.SiteCodeRepository();
            return r.GetSiteBySiteID(_siteCode).Any(x => x.SiteType.ToLower() == "clinic");
        }

        /// <summary>
        /// Gets a sorted list based on a supplied property and sort direction applied to a generic class T list.
        /// </summary>
        /// <typeparam name="T">Generic class.</typeparam>
        /// <param name="tIn">List of generic type T to be sorted.</param>
        /// <param name="propName">Class property to sort.</param>
        /// <param name="sortDirection">Ascending or Descending.</param>
        /// <returns>Sorted list of generic type T.</returns>
        public static List<T> DoSort<T>(List<T> tIn, String propName, SortDirection sortDirection) where T : class
        {
            if (tIn == null) return new List<T>();
            var l = new List<T>();
            switch (sortDirection)
            {
                case SortDirection.Descending:
                    l = tIn.OrderBy(x => x.GetType().GetProperty(propName).GetValue(x, null)).ToList();
                    break;

                case SortDirection.Ascending:
                    l = tIn.OrderByDescending(x => x.GetType().GetProperty(propName).GetValue(x, null)).ToList();
                    break;
            }
            return l;
        }
    }
}