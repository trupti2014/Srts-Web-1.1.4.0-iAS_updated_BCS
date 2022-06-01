using SrtsWeb.DataLayer.Repositories;
using SrtsWeb.Entities;
using SrtsWeb.ExtendersHelpers;
using SrtsWeb.Views.Admin;
using System.Linq;

namespace SrtsWeb.Presenters.Admin
{
    public sealed class SiteCodeAddPresenter
    {
        private ISiteCodeAddView _view;

        public SiteCodeAddPresenter(ISiteCodeAddView view)
        {
            _view = view;
        }

        public void InitView()
        {
            _view.BOSData = _view.LookupCache.GetByType(LookupType.BOSType.ToString());
            _view.CountryData = _view.LookupCache.GetByType(LookupType.CountryList.ToString());
            _view.StateData = _view.LookupCache.GetByType(LookupType.StateList.ToString());
            _view.SiteCodes = _view.LookupCache.GetByType(LookupType.SiteType.ToString());
            GetSites("CLINIC");
            FillBlanks();
        }

        public void GetSites(string _type)
        {
            var _repository = new SiteRepository.SiteCodeRepository();
            if (_type.StartsWith("CLI"))
            {
                var lLabs = _repository.GetSitesByType("LAB");

                _view.MPrimary = lLabs.Where(x => x.IsMultivision == true).Select(a => a.SiteCode).ToList();

                var sLabs = lLabs.Where(x => x.IsMultivision == false).Select(a => a.SiteCode).ToList();
                sLabs.AddRange(lLabs.Where(x => x.IsMultivision == true).Select(a => a.SiteCode).ToList());
                _view.SPrimary = sLabs;
            }
        }

        public void FillData()
        {
            var _repository = new SiteRepository.SiteCodeRepository();

            var lsce = _repository.GetSiteBySiteID(_view.SiteCode);
            foreach (SiteCodeEntity sce in lsce)
            {
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

                if (_view.mySession.MySite.SiteType != "LAB")
                {
                    _view.MultiPrimary = string.IsNullOrEmpty(sce.MultiPrimary) ? string.Empty : sce.MultiPrimary;

                    _view.SinglePrimary = string.IsNullOrEmpty(sce.SinglePrimary) ? string.Empty : sce.SinglePrimary;
                }
                _view.Region = sce.Region;
                _view.HasLMS = sce.HasLMS;
                _view.ShipToPatientLab = sce.ShipToPatientLab;
            }

            var addRep = new SiteRepository.SiteAddressRepository();
            var mAddresses = addRep.GetSiteAddressBySiteID(_view.SiteCode);
            var addr = from p in mAddresses.Where(p => p.AddressType == "MAIL")
                       select (p);
            foreach (var s in addr)
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
            addr = from p in mAddresses.Where(p => p.AddressType == "SITE")
                   select (p);
            foreach (var s in addr)
            {
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

        public void FillBlanks()
        {
            _view.DSNPhoneNumber = string.Empty;
            _view.EMailAddress = string.Empty;
            _view.IsActive = true;
            _view.IsConus = true;
            _view.IsMultivision = true;
            _view.MaxEyeSize = 0;
            _view.MaxFramesPerMonth = 0;
            _view.MaxPower = 0;
            _view.RegPhoneNumber = string.Empty;
            _view.SiteCode = string.Empty;
            _view.SiteDescription = string.Empty;
            _view.SiteName = string.Empty;
            _view.SiteType = "CLINIC";
            _view.Address1 = string.Empty;
            _view.Address2 = string.Empty;
            _view.Address3 = string.Empty;
            _view.ZipCode = string.Empty;
            _view.State = "AL";
            _view.Country = "US";
            _view.City = string.Empty;
            _view.MailAddress1 = string.Empty;
            _view.MailAddress2 = string.Empty;
            _view.MailAddress3 = string.Empty;
            _view.MailZipCode = string.Empty;
            _view.MailState = "AL";
            _view.MailCountry = "US";
            _view.MailCity = string.Empty;
            _view.HasLMS = false;
            _view.ShipToPatientLab = false;
            _view.IsConusMail = true;
        }

        public void InsertSite()
        {
            _view.ErrMessage = string.Empty;
            SiteCodeEntity sce = new SiteCodeEntity();
            var _repository = new SiteRepository.SiteCodeRepository();
            var tmpSite = _repository.GetTempSiteBySiteID(_view.SiteCode);
            if (tmpSite != null && tmpSite.Count > 0)
            {
                _view.ErrMessage = "This is a duplicate site, the site code already exists!";
                return;
            }

            sce.DSNPhoneNumber = _view.DSNPhoneNumber;
            sce.EMailAddress = _view.EMailAddress;
            sce.IsActive = _view.IsActive;
            sce.IsConus = _view.IsConus;
            sce.IsMultivision = _view.IsMultivision;
            sce.IsReimbursable = _view.IsReimbursable;
            sce.MaxEyeSize = string.IsNullOrEmpty(_view.MaxEyeSize.ToString()) ? 0 : _view.MaxEyeSize;
            sce.MaxFramesPerMonth = string.IsNullOrEmpty(_view.MaxFramesPerMonth.ToString()) ? 0 : _view.MaxFramesPerMonth;
            sce.MaxPower = string.IsNullOrEmpty(_view.MaxPower.ToString()) ? 0.0 : _view.MaxPower;
            sce.ModifiedBy = _view.mySession.ModifiedBy;
            sce.RegPhoneNumber = _view.RegPhoneNumber;
            sce.SiteCode = _view.SiteCodetb;
            sce.SiteDescription = _view.SiteDescription;
            sce.SiteName = _view.SiteName;
            sce.SiteType = _view.SiteType;
            sce.Region = 0;
            sce.BOS = _view.BOS;
            if (_view.SiteType == "LAB")
            {
                sce.MultiSecondary = string.Empty;
                sce.MultiPrimary = string.Empty;
                sce.SinglePrimary = string.Empty;
                sce.SingleSecondary = string.Empty;
            }
            else
            {
                sce.MultiSecondary = string.Empty;
                sce.MultiPrimary = _view.MultiPrimary;
                sce.SingleSecondary = string.Empty;
                sce.SinglePrimary = _view.SinglePrimary;
            }
            if (_view.SiteType.StartsWith("LAB"))
            {
                sce.HasLMS = _view.HasLMS;
                sce.ShipToPatientLab = _view.ShipToPatientLab;
            }
            else
            {
                sce.HasLMS = false;
                sce.ShipToPatientLab = false;
            }
            _repository.InsertSite(sce);
            InsertAddress();
        }

        public void InsertAddress()
        {
            SiteAddressEntity sae = new SiteAddressEntity();
            SiteAddressEntity mailAE = new SiteAddressEntity();
            _view.ErrMessage = string.Empty;
            var _repository = new SiteRepository.SiteAddressRepository();

            sae.Address1 = _view.Address1;
            sae.Address2 = _view.Address2;
            sae.Address3 = _view.Address3;
            sae.City = _view.City;
            sae.Country = _view.Country;
            sae.State = _view.State;
            sae.ZipCode = _view.ZipCode.ToZipCodeDisplay();
            sae.IsConus = _view.IsConus;
            sae.SiteCode = _view.SiteCode;
            sae.ModifiedBy = _view.mySession.ModifiedBy;
            sae.IsActive = true;
            sae.AddressType = "SITE";

            if (_view.UseAddress)
            {
                mailAE.Address1 = _view.Address1;
                mailAE.Address2 = _view.Address2;
                mailAE.Address3 = _view.Address3;
                mailAE.City = _view.City;
                mailAE.Country = _view.Country;
                mailAE.State = _view.State;
                mailAE.ZipCode = _view.ZipCode.ToZipCodeDisplay();
                mailAE.IsConus = _view.IsConus;
                mailAE.SiteCode = _view.SiteCode;
                mailAE.ModifiedBy = _view.mySession.ModifiedBy;
                mailAE.IsActive = true;
                mailAE.AddressType = "MAIL";
            }
            else
            {
                mailAE.Address1 = _view.MailAddress1;
                mailAE.Address2 = _view.MailAddress2;
                mailAE.Address3 = _view.MailAddress3;
                mailAE.City = _view.MailCity;
                mailAE.Country = _view.MailCountry;
                mailAE.State = _view.MailState;
                mailAE.ZipCode = _view.MailZipCode.ToZipCodeDisplay();
                mailAE.IsConus = _view.IsConusMail;
                mailAE.SiteCode = _view.SiteCode;
                mailAE.ModifiedBy = _view.mySession.ModifiedBy;
                mailAE.AddressType = "MAIL";
                mailAE.IsActive = true;
            }

            if (!_repository.InsertSiteAddress(sae))
            {
                _view.ErrMessage = "Site Address was not updated, please try again or inform Help Desk";
                return;
            }

            if (!_repository.InsertSiteAddress(mailAE))
            {
                _view.ErrMessage = "Mail Address was not updated, please try again or inform Help Desk";
                return;
            }

            FillData();
        }
    }
}