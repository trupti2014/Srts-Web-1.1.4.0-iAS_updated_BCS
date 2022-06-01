using SrtsWeb.DataLayer.Repositories;
using SrtsWeb.Entities;
using System;
using System.Collections.Generic;

namespace SrtsWeb.BusinessLayer.Concrete
{
    /// <summary>
    /// Custom class used to perform person related operations.
    /// </summary>
    public class PatientsService
    {
        /// <summary>
        /// Gets a list of IndividualEntities.
        /// </summary>
        /// <param name="typeSearch">Global or Local search.  Values: "G" or "S"</param>
        /// <param name="SearchId">ID number; SSN, DODID, or FSN.</param>
        /// <param name="LastName">Last Name</param>
        /// <param name="FirstName">First Name</param>
        /// <param name="SiteCode">If search is local then search in this site code.</param>
        /// <param name="ActiveOnly">Active individuals only.</param>
        /// <param name="ModifiedBy">User performing the search.</param>
        /// <returns></returns>
        public static List<IndividualEntity> SearchIndividual(String typeSearch, String SearchId, String LastName, String FirstName, String SiteCode, Boolean ActiveOnly, String ModifiedBy)
        {
            var r = new IndividualRepository();
            var cnt = default(Int32);

            if (typeSearch.ToUpper() == "S")
                return r.FindIndividualByLastnameOrLastFour(SearchId, LastName, FirstName, SiteCode, "PATIENT", ActiveOnly, ModifiedBy, out cnt);
            else
                return r.FindIndividualByLastnameOrLastFour(SearchId, LastName, FirstName, null, "PATIENT", ActiveOnly, ModifiedBy, out cnt);
        }

        /// <summary>
        /// Gets a list of IndividualEntities by individual ID.  Since IndividualId a primary key in the DB this will actully return no more than one record.
        /// </summary>
        /// <param name="IndividualId">ID number to search for.</param>
        /// <param name="ModifiedBy">User performing the search.</param>
        /// <returns>Individual entity list by individual id.</returns>
        public static List<IndividualEntity> SearchIndividual(Int32 IndividualId, String ModifiedBy)
        {
            var r = new IndividualRepository();
            var iel = r.GetIndividualByIndividualID(IndividualId, ModifiedBy);
            return iel;
        }
    }
}