using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using SrtsWeb.ExtendersHelpers;

namespace SrtsWeb.MembershipProviders
{
    public class SrtsMembershipProvider : SqlMembershipProvider
    {
        public override void Initialize(string name, NameValueCollection config)
        {
            if (HttpContext.Current.Session == null || HttpContext.Current.Session["userName"] == null)
            {
                base.Initialize(name, config);
                return;
            }

            var un = HttpContext.Current.Session["userName"].ToString().ToLower();
            var trainingSites = ConfigurationManager.AppSettings["trainingSites"].Split(new[] { ',' }).ToList();

            var cfgNm = trainingSites.FirstOrDefault(x => un.StartsWith(x.ToLower())) ?? "SRTS";
            
            config["connectionStringName"] = cfgNm;

            Globals.ConnStrNm = cfgNm;
            
            base.Initialize(name, config);
        }
    }
}