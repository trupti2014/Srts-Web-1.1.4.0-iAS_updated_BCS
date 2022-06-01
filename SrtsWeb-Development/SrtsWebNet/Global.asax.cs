using SrtsWeb;
using System;
using System.IO;
using System.Configuration;
using System.Web;
using System.Web.Configuration;
using System.Web.Hosting;

//using System.Configuration.ConfigurationSection;
//using System.Configuration.ProtectedConfigurationSection;

namespace SrtsHome
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            string path = Server.MapPath("~/log.txt");
            Console.SetOut(File.CreateText(path));

            //if (!System.Diagnostics.Debugger.IsAttached)
            if (System.Web.Configuration.WebConfigurationManager.AppSettings["DeployedTo"] == "PRODUCTION")
            {
                ProtectedSection("connectionStrings", "DataProtectionConfigurationProvider");
                ProtectedSection("appSettings", "DataProtectionConfigurationProvider");
                ProtectedSection("system.web/sessionstate", "DataProtectionConfigurationProvider");
                ProtectedSection("system.net/mailSettings/smtp", "DataProtectionConfigurationProvider");
            }

            SiteMap.SiteMapResolve += new SiteMapResolveEventHandler(Provider_BuildBreadCrumbs);
        }

        //protected void Application_EndRequest(object sender, EventArgs e)
        //{
        //    Console.Out.Flush();
        //}

        /// <summary>
        /// Set response cacheability to NoCache for the whole app.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Application_PreSendRequestHeaders(object sender, EventArgs e)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
        }

        /// <summary> Handles the BeginRequest event of the Application control. </summary>
        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            if (Request.IsSecureConnection)
            {
                //Response.AddHeader("Strict-Transport-Security", "max-age=31536000");
            }
        }

        protected SiteMapNode Provider_BuildBreadCrumbs(object sender, SiteMapResolveEventArgs e)
        {
            if (e.Context.CurrentHandler is ISiteMapResolver)
            {
                return ((ISiteMapResolver)e.Context.CurrentHandler).BuildBreadCrumbs(sender, e);
            }
            else
            {
                return null;
            }
        }

        private static void ProtectedSection(string strSectionName, string provider)
        {
            Configuration config = WebConfigurationManager.OpenWebConfiguration(HostingEnvironment.ApplicationVirtualPath);
            ConfigurationSection section = config.GetSection(strSectionName);
            if (section != null && !section.SectionInformation.IsProtected)
            {
                section.SectionInformation.ProtectSection(provider);
                config.Save();
            }
        }
    }
}