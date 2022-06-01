using System;

namespace SrtsWeb.DataLayer.Repositories
{
    /// <summary>
    /// This class is for shared methods/functions between the repostories.
    /// </summary>
    public static class SharedOperations
    {
        /// <summary>
        /// Static method gets a string value for order disbursement based on two bool values from the database.  
        /// </summary>
        /// <param name="Lab">Lab ship to clinic True/False.</param>
        /// <param name="Clinic">Clinic ship to patient True/False.</param>
        /// <returns>String value for order disbursement.</returns>
        public static String ExtractOrderDisbursement(Boolean Lab, Boolean Clinic)
        {
            if (!Clinic && !Lab) return "CD";
            if (Clinic && !Lab) return "C2P";
            if (!Clinic && Lab) return "L2P";
            return "CD";
        }
    }
}