using SrtsWeb.BusinessLayer.Concrete;
using SrtsWeb.DataLayer.Repositories;
using SrtsWeb.Entities;
using SrtsWeb.Views.Patients;
using System.Collections.Generic;

namespace SrtsWeb.Presenters.Patients
{
    public sealed class ManagePatientsPresenter
    {
        private IManagePatientsView _view;

        public ManagePatientsPresenter(IManagePatientsView view)
        {
            _view = view;
        }

        public void InitView()
        {
            PopulateDropDowns();
        }

        public List<IndividualEntity> SearchIndividual(string typeSearch)
        {
            var sId = string.IsNullOrEmpty(_view.SearchID) ? null : _view.SearchID;
            var sLNm = string.Empty;
            var sFNm = string.Empty;

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

            return PatientsService.SearchIndividual(typeSearch, sId, sLNm, sFNm, _view.SelectedSiteValue, _view.ActiveOnly, _view.mySession.ModifiedBy);
        }

        private void PopulateDropDowns()
        {
            var _siteCodeRepository = new SiteRepository.SiteCodeRepository();

            if (_view.mySession == null || _view.mySession.MySite == null || _view.mySession.MySite.SiteType == null) return;

            var siteType = _view.mySession.MySite.SiteType;
            _view.SiteCodes = _siteCodeRepository.GetSitesByType(siteType);
        }
    }
}