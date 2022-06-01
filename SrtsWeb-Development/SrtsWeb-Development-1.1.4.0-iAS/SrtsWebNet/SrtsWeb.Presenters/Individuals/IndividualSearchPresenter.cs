using SrtsWeb.DataLayer.Repositories;
using SrtsWeb.Entities;
using SrtsWeb.ExtendersHelpers;
using SrtsWeb.Views.Individuals;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SrtsWeb.Presenters.Individuals
{
    public sealed class IndividualSearchPresenter
    {
        private IIndividualSearchView _view;

        public IndividualSearchPresenter(IIndividualSearchView view)
        {
            _view = view;
        }

        public void InitView()
        {
            var _siteCodeRepository = new SiteRepository.SiteCodeRepository();
            _view.SiteCodes = _siteCodeRepository.GetAllSites();
            var tmp = _view.LookupCache.GetByType(LookupType.IndividualType.ToString());

            tmp.Remove(tmp.FirstOrDefault(x => x.Value == "PATIENT"));
            _view.IndividualTypeDDL = tmp;
            _view.IndividualTypeSelected = "TECHNICIAN";
        }

        public List<IndividualEntity> SearchIndividual(string typeSearch)
        {
            var sId = string.IsNullOrEmpty(_view.SearchID) ? null : _view.SearchID;
            var sLNm = string.Empty;
            var sFNm = string.Empty;
            var cnt = default(int);

            if (string.IsNullOrEmpty(sId) && string.IsNullOrEmpty(_view.LastName))
                sLNm = string.Empty;
            else if (!string.IsNullOrEmpty(sId) && string.IsNullOrEmpty(_view.LastName))
                sLNm = null;
            else if (!string.IsNullOrEmpty(_view.LastName))
                sLNm = _view.LastName;

            if (string.IsNullOrEmpty(sLNm) && string.IsNullOrEmpty(_view.FirstName))
                sFNm = null;
            else
                sFNm = _view.FirstName;

            var _individualRepository = new IndividualRepository();
            if (typeSearch.ToUpper() == "S")
            {
                return _individualRepository.FindIndividualByLastnameOrLastFour(sId, sLNm, sFNm, _view.SelectedSiteValue, _view.IndividualTypeSelected, true, HttpContext.Current.User.Identity.Name, out cnt);
            }
            else
            {
                return _individualRepository.FindIndividualByLastnameOrLastFour(sId, sLNm, sFNm, null, _view.IndividualTypeSelected, true, HttpContext.Current.User.Identity.Name, out cnt);
            }
        }
    }
}