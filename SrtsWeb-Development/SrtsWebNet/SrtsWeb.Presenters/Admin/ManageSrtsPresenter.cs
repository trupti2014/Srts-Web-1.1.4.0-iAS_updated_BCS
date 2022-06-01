using SrtsWeb.DataLayer.Repositories;
using SrtsWeb.Entities;
using SrtsWeb.ExtendersHelpers;
using SrtsWeb.Views.Admin;

namespace SrtsWeb.Presenters.Admin
{
    public sealed class ManageSrtsPresenter
    {
        private IManageSrtsView _view;

        public ManageSrtsPresenter(IManageSrtsView view)
        {
            _view = view;
        }

        public void InitView(bool isUpdate)
        {
            if (isUpdate)
            {
                FillData();
            }
            else
            {
                NewEntries();
            }
        }

        public void NewEntries()
        {
            _view.Address1 = string.Empty;
            _view.Address2 = string.Empty;
            _view.BOSData = _view.LookupCache.GetByType(LookupType.BOSType.ToString());
            _view.City = string.Empty;
            _view.CountryData = _view.LookupCache.GetByType(LookupType.CountryList.ToString());
            _view.DSNPhoneNumber = string.Empty;
            _view.EMailAddress = string.Empty;
            _view.IsActive = true;
            _view.IsAPOCompatible = false;
            _view.IsConus = true;
            _view.IsMultivision = false;
            _view.MaxEyeSize = 0;
            _view.MaxFramesPerMonth = 0;
            _view.MaxPower = 0;
            _view.RegPhoneNumber = string.Empty;
            _view.SiteCode = string.Empty;
            _view.SiteDescription = string.Empty;
            _view.SiteName = string.Empty;
            _view.SiteType = "CLINIC";
            _view.StateData = _view.LookupCache.GetByType(LookupType.StateList.ToString());
            _view.ZipCode = string.Empty;
            _view.StateSelected = "AL";
            _view.CountrySelected = "US";
        }

        public void FillData()
        {
            var _repository = new SiteRepository.SiteCodeRepository();
            var sce = new SiteCodeEntity();
            sce = _repository.GetSiteBySiteID(_view.mySession.MySite.SiteCode)[0];
            _view.Address1 = sce.Address1;
            _view.Address2 = sce.Address2;
            _view.BOSData = _view.LookupCache.GetByType(LookupType.BOSType.ToString());
            _view.City = sce.City;
            _view.CountryData = _view.LookupCache.GetByType(LookupType.CountryList.ToString());
            _view.DSNPhoneNumber = sce.DSNPhoneNumber;
            _view.EMailAddress = sce.EMailAddress;
            _view.IsActive = sce.IsActive;
            _view.IsAPOCompatible = sce.IsAPOCompatible;
            _view.IsConus = sce.IsConus;
            _view.IsMultivision = sce.IsMultivision;
            _view.MaxEyeSize = sce.MaxEyeSize;
            _view.MaxFramesPerMonth = sce.MaxFramesPerMonth;
            _view.MaxPower = sce.MaxPower;
            _view.RegPhoneNumber = sce.RegPhoneNumber;
            _view.SiteCode = sce.SiteCode;
            _view.SiteDescription = sce.SiteDescription;
            _view.SiteName = sce.SiteName;
            _view.SiteType = sce.SiteType;
            _view.StateData = _view.LookupCache.GetByType(LookupType.StateList.ToString());
            _view.ZipCode = sce.ZipCode.ToZipCodeDisplay();
            _view.StateSelected = sce.State;
            _view.CountrySelected = sce.Country;
        }

        public void UpdateSite()
        {
            var sce = new SiteCodeEntity();
            var _repository = new SiteRepository.SiteCodeRepository();
            sce.Address1 = _view.Address1;
            sce.Address2 = _view.Address2;
            sce.BOS = _view.BOSSelected;
            sce.City = _view.City;
            sce.Country = _view.CountrySelected;
            sce.DSNPhoneNumber = _view.DSNPhoneNumber;
            sce.EMailAddress = _view.EMailAddress;
            sce.IsActive = _view.IsActive;
            sce.IsAPOCompatible = _view.IsAPOCompatible;
            sce.IsConus = _view.IsConus;
            sce.IsMultivision = _view.IsMultivision;
            sce.MaxEyeSize = _view.MaxEyeSize;
            sce.MaxFramesPerMonth = _view.MaxFramesPerMonth;
            sce.MaxPower = _view.MaxPower;
            sce.ModifiedBy = _view.mySession.ModifiedBy;
            sce.RegPhoneNumber = _view.RegPhoneNumber;
            sce.SiteCode = _view.SiteCode;
            sce.SiteDescription = _view.SiteDescription;
            sce.SiteName = _view.SiteName;
            sce.SiteType = _view.SiteType;
            sce.State = _view.StateSelected;
            sce.ZipCode = _view.ZipCode.ToZipCodeDisplay();
            _repository.UpdateSite(sce);
        }

        public void InsertSite()
        {
            var sce = new SiteCodeEntity();
            var _repository = new SiteRepository.SiteCodeRepository();
            sce.Address1 = _view.Address1;
            sce.Address2 = _view.Address2;
            sce.BOS = _view.BOSSelected;
            sce.City = _view.City;
            sce.Country = _view.CountrySelected;
            sce.DSNPhoneNumber = _view.DSNPhoneNumber;
            sce.EMailAddress = _view.EMailAddress;
            sce.IsActive = _view.IsActive;
            sce.IsAPOCompatible = _view.IsAPOCompatible;
            sce.IsConus = _view.IsConus;
            sce.IsMultivision = _view.IsMultivision;
            sce.MaxEyeSize = _view.MaxEyeSize;
            sce.MaxFramesPerMonth = _view.MaxFramesPerMonth;
            sce.MaxPower = _view.MaxPower;
            sce.ModifiedBy = _view.mySession.ModifiedBy;
            sce.RegPhoneNumber = _view.RegPhoneNumber;
            sce.SiteCode = _view.SiteCode;
            sce.SiteDescription = _view.SiteDescription;
            sce.SiteName = _view.SiteName;
            sce.SiteType = _view.SiteType;
            sce.State = _view.StateSelected;
            sce.ZipCode = _view.ZipCode.ToZipCodeDisplay();
            _repository.InsertSite(sce);
        }
    }
}