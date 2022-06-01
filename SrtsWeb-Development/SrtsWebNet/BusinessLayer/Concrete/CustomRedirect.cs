using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using SrtsWeb.ExtendersHelpers;

namespace SrtsWeb.BusinessLayer.Concrete
{
    /// <summary>
    /// Custom class that is used to ensure that a redirect is performed on a local address.
    /// </summary>
    public sealed class CustomRedirect
    {
        /// <summary>
        /// Performs a check to determine if the redirect URL is local before doing a redirect.
        /// </summary>
        /// <param name="redirectUrl">URL to redirect to.</param>
        /// <param name="endResponse">Flag to determin weather or not to end execution of the response.</param>
        public static bool SanitizeRedirect(String redirectUrl, Boolean endResponse) 
        {
            if (!isLocalUrl(redirectUrl) && isAbsolute(redirectUrl))
            {
                SystemException ex = new SystemException();
                ex.LogException("..A problem occurred in the SanitizeRedirect() method..");
                return false;
            }
            else
            {
                HttpContext.Current.Response.Redirect(redirectUrl, endResponse);
                return true;
            }
        }

        private static Boolean isLocalUrl(String url)
        {
            if (String.IsNullOrEmpty(url)) return false;
            Uri absUri;
            if (Uri.TryCreate(url, UriKind.Absolute, out absUri))
                return String.Equals(absUri.Host, HttpContext.Current.Request.Url.Host, StringComparison.OrdinalIgnoreCase);
            else
                return !url.StartsWith("http:", StringComparison.OrdinalIgnoreCase) && !url.StartsWith("https:", StringComparison.OrdinalIgnoreCase) && Uri.IsWellFormedUriString(url, UriKind.Relative);
        }

        private static Boolean isAbsolute(string url)
        {
            bool isAbsolute = false;
            Uri result;
            string redirectURL = url;
            isAbsolute = Uri.TryCreate(redirectURL, UriKind.Absolute, out result);
            return isAbsolute;
        }

    }
}
