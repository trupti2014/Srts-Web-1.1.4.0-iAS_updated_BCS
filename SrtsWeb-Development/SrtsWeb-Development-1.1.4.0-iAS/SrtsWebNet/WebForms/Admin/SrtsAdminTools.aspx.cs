using SrtsWeb.Base;
using SrtsWeb.Presenters.Admin;
using SrtsWeb.Views.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;

namespace SrtsWeb.Admin
{
    public partial class SrtsAdminTools : PageBase, ISrtsToolsView
    {
        private SrtsToolsPresenter _presenter;

        public SrtsAdminTools()
        {
            _presenter = new SrtsToolsPresenter(this);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            lblUDPChange.Text = "Page was loaded at " + DateTime.Now.ToString();
        }

        #region JQGrid

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static List<object> GetSiteOrdersFromDB(string SiteCode, string StartDate, string EndDate)
        {
            return SrtsToolsPresenter.GetAllActiveOrders(SiteCode, StartDate, EndDate, HttpContext.Current.User.Identity.Name);
        }

        #endregion JQGrid

        #region PageEvents

        [WebMethod]
        public static Dictionary<string, string> GetSiteDataFromDB()
        {
            var s = SrtsToolsPresenter.GetSites().Select(x => new
                    {
                        SiteCode = x.SiteCode,
                        SiteTitle = String.Format("{0} - {1}", x.SiteCode, x.SiteName)
                    }).Distinct().ToDictionary(x => x.SiteCode, x => x.SiteTitle);

            return s;
        }

        #endregion PageEvents

        #region Accessors

        #endregion Accessors
    }
}