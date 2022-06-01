using SrtsWeb.BusinessLayer.Enumerators;
using SrtsWeb.ExtendersHelpers;
using SrtsWeb.Views.Admin;
using SrtsWeb.DataLayer.Repositories;
using SrtsWeb.Entities;
using System.Collections.Generic;
using System.Linq;

namespace SrtsWeb.BusinessLayer.Presenters.Admin
{
    public sealed class SiteCodeManagementPresenter
    {
        private ISiteCodeManagementView _view;

        public SiteCodeManagementPresenter(ISiteCodeManagementView view)
        {
            _view = view;
        }

        public void InitView()
        {
            _view.BOSData = _view.LookupCache.GetByType(LookupType.BOSType.ToString());
            _view.CountryData = _view.LookupCache.GetByType(LookupType.CountryList.ToString());
            _view.StateData = _view.LookupCache.GetByType(LookupType.StateList.ToString());

            var _repository = new SiteRepository.SiteCodeRepository();

            _view.SiteCodes = _repository.GetAllSiteCodes();
            _view.SiteCode = _view.mySession.MySite.SiteCode;
            GetSites(_view.mySession.MySite.Region, _view.mySession.MySite.SiteType);
            FillData();
        }

        public void GetSites(int region, string _type)
        {
            var _repository = new SiteRepository.SiteCodeRepository();
            if (_type.StartsWith("CLI"))
            {
                var sCodes = _repository.GetSitesByTypeAndRegion("LAB", region);
                _view.MPrimary = sCodes;
                _view.MSecondary = sCodes;
                _view.SPrimary = sCodes;
                _view.SSecondary = sCodes;
                _view.SinglePrimary = string.IsNullOrEmpty(_view.mySession.MySite.SinglePrimary) ? string.Empty : _view.mySession.MySite.SinglePrimary;
                _view.SingleSecondary = string.IsNullOrEmpty(_view.mySession.MySite.SingleSecondary) ? string.Empty : _view.mySession.MySite.SingleSecondary;
                _view.MultiPrimary = string.IsNullOrEmpty(_view.mySession.MySite.MultiPrimary) ? string.Empty : _view.mySession.MySite.MultiPrimary;
                _view.MultiSecondary = string.IsNullOrEmpty(_view.mySession.MySite.MultiSecondary) ? string.Empty : _view.mySession.MySite.MultiSecondary;
            }
            else
            {
                _view.MPrimary = new List<string>();
                _view.MSecondary = new List<string>();
                _view.SPrimary = new List<string>();
                _view.SSecondary = new List<string>();
            }
        }

        public void FillData()
        {
            var _repository = new SiteRepository.SiteCodeRepository();

            var sce = _repository.GetSiteBySiteID(_view.SiteCode).FirstOrDefault();

            _view.DSNPhoneNumber = sce.DSNPhoneNumber;
            _view.EMailAddress = sce.EMailAddress;
            _view.IsActive = string.IsNullOrEmpty(sce.IsActive.ToString()) ? true : sce.IsActive;
            _view.IsConus = sce.IsConus;
            _view.IsMultivision = string.IsNullOrEmpty(sce.IsMultivision.ToString()) ? true : sce.IsMultivision;
            _view.MaxEyeSize = sce.MaxEyeSize;
            _view.MaxFramesPerMonth = sce.MaxFramesPerMonth;
            _view.MaxPower = sce.MaxPower;
            _view.RegPhoneNumber = sce.RegPhoneNumber;
            _view.SiteCode = sce.SiteCode;
            _view.SiteDescription = sce.SiteDescription;
            _view.SiteName = sce.SiteName;
            _view.SiteType = sce.SiteType;
            _view.BOS = sce.BOS;
            if (_view.SiteType != "LAB")
            {
                _view.MultiPrimary = string.IsNullOrEmpty(sce.MultiPrimary) ? string.Empty : sce.MultiPrimary;
                _view.MultiSecondary = string.IsNullOrEmpty(sce.MultiSecondary) ? string.Empty : sce.MultiSecondary;
                _view.SinglePrimary = string.IsNullOrEmpty(sce.SinglePrimary) ? string.Empty : sce.SinglePrimary;
                _view.SingleSecondary = string.IsNullOrEmpty(sce.SingleSecondary) ? string.Empty : sce.SingleSecondary;
            }
            _view.Region = 0;
            _view.HasLMS = sce.HasLMS;
            _view.ShipToPatientLab = sce.ShipToPatientLab;

            var addRep = new SiteRepository.SiteAddressRepository();
            var addrs = addRep.GetSiteAddressBySiteID(_view.SiteCode);

            //Clear address data
            _view.MailAddress1 = string.Empty;
            _view.MailAddress2 = string.Empty;
            _view.MailAddress3 = string.Empty;
            _view.MailZipCode = string.Empty;
            _view.MailState = string.Empty;
            _view.MailCountry = string.Empty;
            _view.MailCity = string.Empty;
            _view.IsConusMail = true;
            _view.Address1 = string.Empty;
            _view.Address2 = string.Empty;
            _view.Address3 = string.Empty;
            _view.ZipCode = string.Empty;
            _view.State = string.Empty;
            _view.Country = string.Empty;
            _view.City = string.Empty;
            _view.IsConus = true;

            var s = addrs.Where(p => p.AddressType == "MAIL").FirstOrDefault();
            if (s != null)
            {
                _view.MailAddress1 = string.IsNullOrEmpty(s.Address1) ? string.Empty : s.Address1;
                _view.MailAddress2 = string.IsNullOrEmpty(s.Address2) ? string.Empty : s.Address2;
                _view.MailAddress3 = string.IsNullOrEmpty(s.Address3) ? string.Empty : s.Address3;
                _view.MailZipCode = string.IsNullOrEmpty(s.ZipCode) ? string.Empty : s.ZipCode.ToZipCodeDisplay();
                _view.MailState = string.IsNullOrEmpty(s.State) ? string.Empty : s.State;
                _view.MailCountry = string.IsNullOrEmpty(s.Country) ? string.Empty : s.Country;
                _view.MailCity = string.IsNullOrEmpty(s.City) ? string.Empty : s.City;
                _view.IsConusMail = s.IsConus;
            }
            
            s = addrs.Where(p => p.AddressType == "SITE").FirstOrDefault();
            if (s == null) return;

            _view.Address1 = string.IsNullOrEmpty(s.Address1) ? string.Empty : s.Address1;
            _view.Address2 = string.IsNullOrEmpty(s.Address2) ? string.Empty : s.Address2;
            _view.Address3 = string.IsNullOrEmpty(s.Address3) ? string.Empty : s.Address3;
            _view.ZipCode = string.IsNullOrEmpty(s.ZipCode) ? string.Empty : s.ZipCode.ToZipCodeDisplay();
            _view.State = string.IsNullOrEmpty(s.State) ? string.Empty : s.State;
            _view.Country = string.IsNullOrEmpty(s.Country) ? string.Empty : s.Country;
            _view.City = string.IsNullOrEmpty(s.City) ? string.Empty : s.City;
            _view.IsConus = s.IsConus;
        }
    }
}