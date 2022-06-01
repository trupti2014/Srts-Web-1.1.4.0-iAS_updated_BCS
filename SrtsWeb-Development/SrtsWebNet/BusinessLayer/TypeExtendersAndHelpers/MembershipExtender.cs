using SrtsWeb.BusinessLayer.Concrete;
using SrtsWeb.BusinessLayer.Enumerators;
using System;
using System.Web.Profile;
using System.Web.Security;

namespace SrtsWeb.BusinessLayer.TypeExtendersAndHelpers.Extenders
{
    public static partial class SrtsExtender
    {
        public static MembershipUserCollection GetFilteredUsers(this MembershipUserCollection collIn,
            String filterPropertyName, Object filterValue)
        {
            MembershipUserCollection mc = new MembershipUserCollection();
            foreach (MembershipUser mu in collIn)
            {
                var fv = filterValue;

                var tv = mu.GetType().GetProperty(filterPropertyName,
                    System.Reflection.BindingFlags.IgnoreCase |
                    System.Reflection.BindingFlags.Public |
                    System.Reflection.BindingFlags.Instance).GetValue(mu, null);

                if (!tv.ToString().ToLower().Equals(fv.ToString().ToLower())) continue;
                mc.Add(mu);
            }
            return mc;
        }

        public static SiteType GetUserSiteType(this ProfileBase profileIn)
        {
            var pers = profileIn.GetPropertyValue("Personal");
            var scOb = pers.GetType().GetProperty("SiteCode").GetValue(pers, null);

            if (scOb == null) return SiteType.OTHER;

            if (scOb.ToString().Contains("ADM")) return SiteType.ADMINSTRATIVE;
            if (scOb.ToString().Contains("OTHER")) return SiteType.ADMINSTRATIVE;

            return Misc.IsSiteClinic(scOb.ToString()) ? SiteType.CLINIC : SiteType.LABORATORY;
        }

        public static MembershipUserCollection GetUsersWithProfiles(this MembershipUserCollection collIn)
        {
            MembershipUserCollection m = new MembershipUserCollection();
            foreach (MembershipUser u in collIn)
            {
                var p = ProfileBase.Create(u.UserName);
                var pers = p.GetPropertyValue("Personal");
                if (pers.GetType().GetProperty("SiteCode").GetValue(pers, null) == null) continue;
                m.Add(u);
            }

            return m;
        }
    }
}