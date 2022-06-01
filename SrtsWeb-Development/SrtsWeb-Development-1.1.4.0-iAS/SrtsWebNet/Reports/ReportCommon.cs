using System.Data;

namespace SrtsWeb.Reports
{
    public class ReportCommon
    {
        /// <summary>
        /// Static method used to return a typed empty dataset.  This is the expected format needed to fill the shipping labels with data.
        /// </summary>
        /// <returns>System.Data.DataTable</returns>
        public static DataTable GetTableTemplateForLabels()
        {
            var dt = new DataTable("personAddress");
            dt.Columns.AddRange(new[]{
                        new DataColumn("Name"),
                        new DataColumn("Address"),
                        new DataColumn("City"),
                        new DataColumn("State"),
                        new DataColumn("PostalCode"),
                        new DataColumn("Country")
                    });

            return dt;
        }
    }
}