using System.Security.Principal;

namespace SrtsWeb.Reports
{
    public class CustomReportCredentials : Microsoft.Reporting.WebForms.IReportServerCredentials
    {
        private string _UserName;
        private string _Fact2;
        private string _DomainName;

        #region IReportServerCredentials Members

        public CustomReportCredentials(string userName, string fact2, string domainName)
        {
            _UserName = userName;
            _Fact2 = fact2;
            _DomainName = domainName;
        }

        public WindowsIdentity ImpersonationUser
        {
            get
            {
                return null;
            }
        }

        public System.Net.ICredentials NetworkCredentials
        {
            get
            {
                return new System.Net.NetworkCredential(_UserName, _Fact2, _DomainName);
            }
        }

        public bool GetFormsCredentials(out System.Net.Cookie authCookie, out string user, out string fact2, out string authority)
        {
            authCookie = null;
            user = fact2 = authority = null;

            return false;
        }

        #endregion IReportServerCredentials Members
    }
}