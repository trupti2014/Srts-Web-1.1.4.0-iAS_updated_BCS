using System;
using System.Configuration;
using System.Linq;
using System.Web;

namespace SrtsWeb.ExtendersHelpers
{
    public class Globals
    {
        public static readonly String DtFmt = "dd MMM yyyy";
        public static readonly String TitleDtFmt = "dddd, MMMM dd, yyyy";
        public static String ModifiedBy = "";
        public static String SiteCode = "";
        public static Int32? PatientID = null;
        public static Int32? IndividualID = null;

        public static String ConnStrNm
        {
            get
            {
                var cfgNm = String.Empty;
                if (!HttpContext.Current.User.Identity.IsAuthenticated)
                {
                    cfgNm = HttpContext.Current.Session.IsNull() || HttpContext.Current.Session["connStrNm"].IsNull() ?
                    ConfigurationManager.AppSettings["DefaultConnStrNm"] : HttpContext.Current.Session["connStrNm"].ToString();
                }
                else
                {
                    var un = HttpContext.Current.User.Identity.Name.ToLower();
                    var trainingSites = ConfigurationManager.AppSettings["trainingSites"].Split(new[] { ',' }).ToList();
                    cfgNm = trainingSites.FirstOrDefault(x => un.StartsWith(x.ToLower())) ?? "SRTS";
                }

                return cfgNm;
            }
            set { System.Web.HttpContext.Current.Session["connStrNm"] = value; }
        }

        public static String DefaultConnStrNm
        {
            get { return ConfigurationManager.AppSettings["DefaultConnStrNm"]; }
        }

        public static void ClearGlobals()
        {
            ModifiedBy = string.Empty;
            SiteCode = string.Empty;
            PatientID = null;
            IndividualID = null;
        }
    }
}