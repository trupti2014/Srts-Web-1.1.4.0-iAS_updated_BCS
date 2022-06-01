using SrtsWeb.Views.Admin;
using SrtsWeb.DataLayer.Repositories;
using SrtsWeb.Entities;
using System.Collections.Generic;

namespace SrtsWeb.Presenters.Admin
{
    public sealed class SrtsToolsPresenter
    {
        private ISrtsToolsView _view;

        public SrtsToolsPresenter(ISrtsToolsView view)
        {
            _view = view;
        }

        public static List<object> GetAllActiveOrders(string siteCode, string startDate, string endDate, string myUserId)
        {
            var r = new AdminToolsRepository();
            return r.GetAllActiveOrders(siteCode, startDate, endDate, myUserId);
        }

        public static List<SiteCodeEntity> GetSites() {
            var r = new SiteRepository.SiteCodeRepository();
            return r.GetAllSites();
        }
    }
}