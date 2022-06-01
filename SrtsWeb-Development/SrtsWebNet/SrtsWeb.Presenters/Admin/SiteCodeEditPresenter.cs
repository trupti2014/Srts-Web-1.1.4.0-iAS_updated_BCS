using SrtsWeb.DataLayer.Repositories;
using SrtsWeb.Entities;
using SrtsWeb.ExtendersHelpers;
using SrtsWeb.Views.Admin;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SrtsWeb.Presenters.Admin
{
    public sealed class SiteCodeEditPresenter
    {
        private ISiteCodeEditView _view;

        public SiteCodeEditPresenter(ISiteCodeEditView view)
        {
            _view = view;
        }

        public void InitView()
        {
            _view.BOSData = _view.LookupCache.GetByType(LookupType.BOSType.ToString());
            _view.CountryData = _view.LookupCache.GetByType(LookupType.CountryList.ToString());
            _view.StateData = _view.LookupCache.GetByType(LookupType.StateList.ToString());
            _view.SiteTypes = _view.LookupCache.GetByType(LookupType.SiteType.ToString());

            var _repository = new SiteRepository.SiteCodeRepository();

            var sCodes = _repository.GetSiteBySiteID(_view.SiteCode);
            string _siteType = sCodes.Select(a => a.SiteType).FirstOrDefault().ToString();

            GetSites(_siteType, sCodes.FirstOrDefault());

            FillData();
        }

        public void FillLabReportInfo(string _type)
        {
            var _repository = new SiteRepository.SiteCodeRepository();

            if (_type.StartsWith("CLI"))
            {
                var lLabs = _repository.GetSitesByType("LAB");

                var mLabs = lLabs.Where(x => x.IsMultivision == true).Select(x => x.SiteCode).ToList().ConvertAll<KeyValuePair<String, String>>(StringToKeyValuePair);
                var sLabs = lLabs.Where(x => x.IsMultivision == false).Select(x => x.SiteCode).ToList().ConvertAll<KeyValuePair<String, String>>(StringToKeyValuePair);

                _view.MPrimary = mLabs;
                _view.SPrimary = sLabs;
            }
            else
            {
                _view.MPrimary = new List<KeyValuePair<string, string>>();
                _view.MSecondary = new List<KeyValuePair<string, string>>();
                _view.SPrimary = new List<KeyValuePair<string, string>>();
                _view.SSecondary = new List<KeyValuePair<string, string>>();
            }
        }

        public static KeyValuePair<string, string> StringToKeyValuePair(String strIn)
        {
            return new KeyValuePair<string, string>(strIn, strIn);
        }

        public void GetSites(string _type, SiteCodeEntity _site)
        {
            var _repository = new SiteRepository.SiteCodeRepository();

            if (_type.StartsWith("CLI"))
            {
                var lLabs = _repository.GetSitesByType("LAB");

                var mLabs = lLabs.Where(x => x.IsMultivision == true).Select(x => x.SiteCode).ToList().ConvertAll<KeyValuePair<String, String>>(StringToKeyValuePair);
                var sLabs = lLabs.Where(x => x.IsMultivision == false).Select(x => x.SiteCode).ToList().ConvertAll<KeyValuePair<String, String>>(StringToKeyValuePair);
                sLabs.AddRange(lLabs.Where(x => x.IsMultivision == true).Select(x => x.SiteCode).ToList().ConvertAll<KeyValuePair<String, String>>(StringToKeyValuePair));

                _view.MPrimary = mLabs;
                _view.SPrimary = sLabs;
                _view.SinglePrimary = string.IsNullOrEmpty(_site.SinglePrimary) ? string.Empty : _site.SinglePrimary;
                _view.SingleSecondary = string.IsNullOrEmpty(_site.SingleSecondary) ? string.Empty : _site.SingleSecondary;
                _view.MultiPrimary = string.IsNullOrEmpty(_site.MultiPrimary) ? string.Empty : _site.MultiPrimary;
                _view.MultiSecondary = string.IsNullOrEmpty(_site.MultiSecondary) ? string.Empty : _site.MultiSecondary;
            }
            else
            {
                _view.MPrimary = new List<KeyValuePair<string, string>>();
                _view.MSecondary = new List<KeyValuePair<string, string>>();
                _view.SPrimary = new List<KeyValuePair<string, string>>();
                _view.SSecondary = new List<KeyValuePair<string, string>>();
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
                    if (!string.IsNullOrEmpty(sce.MultiPrimary))
                        _view.MultiPrimary = sce.MultiPrimary;
                    if (!string.IsNullOrEmpty(sce.MultiSecondary))
                        _view.MultiSecondary = sce.MultiSecondary;
                    if (!string.IsNullOrEmpty(sce.SinglePrimary))
                        _view.SinglePrimary = sce.SinglePrimary;
                    if (!string.IsNullOrEmpty(sce.SingleSecondary))
                        _view.SingleSecondary = sce.SingleSecondary;
                }
                _view.Region = 0;
                _view.HasLMS = sce.HasLMS;
                _view.ShipToPatientLab = sce.ShipToPatientLab;
            }

            var addRep = new SiteRepository.SiteAddressRepository();
            var mAddresses = addRep.GetSiteAddressBySiteID(_view.SiteCode);
            var addr = mAddresses.FirstOrDefault(p => p.AddressType == "MAIL");

            if (addr.IsNull())
            {
                _view.MailAddress1 = String.Empty;
                _view.MailAddress2 = String.Empty;
                _view.MailAddress3 = String.Empty;
                _view.MailCity = String.Empty;
                _view.MailState = "AL";
                _view.MailCountry = "US";
                _view.MailZipCode = String.Empty;
                _view.IsConusMail = true;
            }
            else
            {
                _view.MailAddress1 = string.IsNullOrEmpty(addr.Address1) ? string.Empty : addr.Address1;
                _view.MailAddress2 = string.IsNullOrEmpty(addr.Address2) ? string.Empty : addr.Address2;
                _view.MailAddress3 = string.IsNullOrEmpty(addr.Address3) ? string.Empty : addr.Address3;
                _view.MailZipCode = string.IsNullOrEmpty(addr.ZipCode) ? string.Empty : addr.ZipCode.ToZipCodeDisplay();
                _view.MailState = string.IsNullOrEmpty(addr.State) ? "AL" : addr.State;
                _view.MailCountry = string.IsNullOrEmpty(addr.Country) ? "US" : addr.Country;
                _view.MailCity = string.IsNullOrEmpty(addr.City) ? string.Empty : addr.City;
                _view.IsConusMail = addr.IsConus;
            }

            addr = mAddresses.FirstOrDefault(p => p.AddressType == "SITE");

            if (addr.IsNull())
            {
                _view.Address1 = String.Empty;
                _view.Address2 = String.Empty;
                _view.Address3 = String.Empty;
                _view.City = String.Empty;
                _view.State = "AL";
                _view.Country = "US";
                _view.ZipCode = String.Empty;
                _view.IsConus = true;
            }
            else
            {
                _view.Address1 = string.IsNullOrEmpty(addr.Address1) ? string.Empty : addr.Address1;
                _view.Address2 = string.IsNullOrEmpty(addr.Address2) ? string.Empty : addr.Address2;
                _view.Address3 = string.IsNullOrEmpty(addr.Address3) ? string.Empty : addr.Address3;
                _view.ZipCode = string.IsNullOrEmpty(addr.ZipCode) ? string.Empty : addr.ZipCode.ToZipCodeDisplay();
                _view.State = string.IsNullOrEmpty(addr.State) ? "AL" : addr.State;
                _view.Country = string.IsNullOrEmpty(addr.Country) ? "US" : addr.Country;
                _view.City = string.IsNullOrEmpty(addr.City) ? string.Empty : addr.City;
                _view.IsConus = addr.IsConus;
            }
            //FillLabParameters();
        }

        //public void FillLabParameters()
        //{
        //    var fpr = new FabricationParametersRepository();
        //    var fir = new FrameItemsRepository();
        //    var l = fir.GetFrameItems();

        //    _view.FabricationParameterData = fpr.GetAllParametersBySiteCode(_view.SiteCode);
        //    _view.LensMaterial = l.Where(x => x.TypeEntry.ToLower() == "material").Select(x => new { Key = x.Text, Value = x.Value }).Distinct().OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);

        //}

        public void UpdateSite()
        {
            _view.ErrMessage = string.Empty;
            var sce = new SiteCodeEntity();
            var _repository = new SiteRepository.SiteCodeRepository();
            var ModifiedBy = string.IsNullOrEmpty(_view.mySession.ModifiedBy) ? Globals.ModifiedBy : _view.mySession.ModifiedBy;

            sce.BOS = _view.BOS;
            sce.DSNPhoneNumber = _view.DSNPhoneNumber;
            sce.EMailAddress = _view.EMailAddress;
            sce.IsActive = _view.IsActive;
            sce.IsMultivision = _view.IsMultivision;
            sce.MaxEyeSize = _view.MaxEyeSize;
            sce.MaxFramesPerMonth = _view.MaxFramesPerMonth;
            sce.MaxPower = _view.MaxPower;
            sce.ModifiedBy = ModifiedBy;
            //sce.ModifiedBy = string.IsNullOrEmpty(_view.mySession.ModifiedBy) ? string.Empty : _view.mySession.ModifiedBy;
            sce.RegPhoneNumber = _view.RegPhoneNumber;
            sce.SiteCode = _view.SiteCode;
            sce.SiteDescription = _view.SiteDescription;
            sce.SiteName = _view.SiteName;
            sce.SiteType = _view.SiteType;
            sce.MultiPrimary = _view.MultiPrimary;
            sce.MultiSecondary = string.IsNullOrEmpty(_view.MultiSecondary) ? string.Empty : _view.MultiSecondary;
            sce.SinglePrimary = _view.SinglePrimary;
            sce.SingleSecondary = string.IsNullOrEmpty(_view.SingleSecondary) ? string.Empty : _view.SingleSecondary;
            sce.Region = string.IsNullOrEmpty(_view.Region.ToString()) ? 0 : _view.Region;
            sce.HasLMS = _view.HasLMS;
            sce.ShipToPatientLab = _view.ShipToPatientLab;

            if (_repository.UpdateSite(sce))
            {
                FillData();
            }
            else
            {
                _view.ErrMessage = "Site was not updated, please inform Help Desk or try again!";
            }
        }

        public void UpdateAddress()
        {
            var sae = new SiteAddressEntity();
            _view.ErrMessage = string.Empty;
            var _repository = new SiteRepository.SiteAddressRepository();

            sae.Address1 = _view.Address1;
            sae.Address2 = _view.Address2;
            sae.Address3 = _view.Address3;
            sae.AddressType = "SITE";
            sae.City = _view.City;
            sae.Country = _view.Country;
            sae.State = _view.State;
            sae.ZipCode = _view.ZipCode.ToZipCodeDisplay();
            sae.IsConus = _view.IsConus;
            sae.SiteCode = _view.SiteCode;
            sae.ModifiedBy = _view.mySession.ModifiedBy;
            sae.IsActive = true;

            if (!_repository.UpdateSiteAddress(sae))
            {
                _view.ErrMessage = "Site Address was not updated, please try again or inform Help Desk";
                return;
            }

            if (_view.UseAddress)
            {
                sae.AddressType = "MAIL";
            }
            else
            {
                sae.Address1 = _view.MailAddress1;
                sae.Address2 = _view.MailAddress2;
                sae.Address3 = _view.MailAddress3;
                sae.AddressType = "MAIL";
                sae.City = _view.MailCity;
                sae.Country = _view.MailCountry;
                sae.State = _view.MailState;
                sae.ZipCode = _view.MailZipCode.ToZipCodeDisplay();
                sae.IsConus = _view.IsConusMail;
                sae.SiteCode = _view.SiteCode;
                sae.ModifiedBy = _view.mySession.ModifiedBy;
                sae.IsActive = true;
            }

            if (_repository.UpdateSiteAddress(sae))
            {
                FillData();
            }
            else
            {
                _view.ErrMessage = "Mail Address was not updated, please try again or inform Help Desk";
                return;
            }
        }

        public void DoUpdate()
        {
            _view.ErrMessage = string.Empty;
            var sce = new SiteCodeEntity();
            var siteAE = new SiteAddressEntity();
            var mailAE = new SiteAddressEntity();
            var _repository = new SiteRepository.SiteAddressRepository();

            sce.BOS = _view.BOS;
            sce.DSNPhoneNumber = _view.DSNPhoneNumber;
            sce.EMailAddress = _view.EMailAddress;
            sce.IsActive = _view.IsActive;
            sce.IsMultivision = _view.IsMultivision;
            sce.IsReimbursable = _view.IsReimbursable;
            sce.MaxEyeSize = _view.MaxEyeSize;
            sce.MaxFramesPerMonth = _view.MaxFramesPerMonth;
            sce.MaxPower = _view.MaxPower;
            sce.ModifiedBy = _view.mySession.ModifiedBy;
            sce.RegPhoneNumber = _view.RegPhoneNumber;
            sce.SiteCode = _view.SiteCode;
            sce.SiteDescription = _view.SiteDescription;
            sce.SiteName = _view.SiteName;
            sce.SiteType = _view.SiteType;
            sce.MultiPrimary = _view.MultiPrimary;
            sce.MultiSecondary = string.IsNullOrEmpty(_view.MultiSecondary) ? string.Empty : _view.MultiSecondary;
            sce.SinglePrimary = _view.SinglePrimary;
            sce.SingleSecondary = string.IsNullOrEmpty(_view.SingleSecondary) ? string.Empty : _view.SingleSecondary;
            sce.Region = string.IsNullOrEmpty(_view.Region.ToString()) ? 0 : _view.Region;
            sce.HasLMS = _view.HasLMS;
            sce.ShipToPatientLab = _view.ShipToPatientLab;

            siteAE.Address1 = _view.Address1;
            siteAE.Address2 = _view.Address2;
            siteAE.Address3 = _view.Address3;
            siteAE.AddressType = "SITE";
            siteAE.City = _view.City;
            siteAE.Country = _view.Country;
            siteAE.State = _view.State;
            siteAE.ZipCode = _view.ZipCode.ToZipCodeDisplay();
            siteAE.IsConus = _view.IsConus;
            siteAE.SiteCode = _view.SiteCode;
            siteAE.ModifiedBy = _view.mySession.ModifiedBy;
            siteAE.IsActive = true;

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
                mailAE.AddressType = "MAIL";
                mailAE.City = _view.MailCity;
                mailAE.Country = _view.MailCountry;
                mailAE.State = _view.MailState;
                mailAE.ZipCode = _view.MailZipCode.ToZipCodeDisplay();
                mailAE.IsConus = _view.IsConusMail;
                mailAE.SiteCode = _view.SiteCode;
                mailAE.ModifiedBy = _view.mySession.ModifiedBy;
                mailAE.IsActive = true;
            }

            var siteRep = new SiteRepository.SiteCodeRepository();
            if (!siteRep.UpdateSite(sce))
            {
                _view.ErrMessage = "Site was not updated, please inform Help Desk or try again!";
                return;
            }

            if (!_repository.UpdateSiteAddress(siteAE))
            {
                _view.ErrMessage = "Site Address was not updated, please try again or inform Help Desk";
                return;
            }

            if (!_repository.UpdateSiteAddress(mailAE))
            {
                _view.ErrMessage = "Mail Address was not updated, please try again or inform Help Desk";
                return;
            }

            FillData();
        }

        public void InsertAddress()
        {
            SiteAddressEntity sae = new SiteAddressEntity();
            _view.ErrMessage = string.Empty;
            var _repository = new SiteRepository.SiteAddressRepository();

            sae.Address1 = _view.Address1;
            sae.Address2 = _view.Address2;
            sae.Address3 = _view.Address3;
            sae.AddressType = "SITE";
            sae.City = _view.City;
            sae.Country = _view.Country;
            sae.State = _view.State;
            sae.ZipCode = _view.ZipCode.ToZipCodeDisplay();
            sae.IsConus = _view.IsConus;
            sae.SiteCode = _view.SiteCode;
            sae.ModifiedBy = _view.mySession.ModifiedBy;
            sae.IsActive = true;
            if (_repository.InsertSiteAddress(sae))
            {
                FillData();
            }
            else
            {
                _view.ErrMessage = "Site Address was not added, please try again or inform Help Desk";
                return;
            }
            if (_view.UseAddress)
            {
                _view.MailAddress1 = _view.Address1;
                _view.MailAddress2 = _view.Address2;
                _view.MailAddress3 = _view.Address3;
                _view.MailCity = _view.City;
                _view.MailCountry = _view.Country;
                _view.MailState = _view.State;
                _view.MailZipCode = _view.ZipCode;
                _view.IsConusMail = _view.IsConus;
            }
            sae.Address1 = _view.MailAddress1;
            sae.Address2 = _view.MailAddress2;
            sae.Address3 = _view.MailAddress3;
            sae.AddressType = "MAIL";
            sae.City = _view.MailCity;
            sae.Country = _view.MailCountry;
            sae.State = _view.MailState;
            sae.ZipCode = _view.MailZipCode.ToZipCodeDisplay();
            sae.IsConus = _view.IsConusMail;
            sae.SiteCode = _view.SiteCode;
            sae.ModifiedBy = _view.mySession.ModifiedBy;
            sae.IsActive = true;

            if (_repository.InsertSiteAddress(sae))
            {
                FillData();
            }
            else
            {
                _view.ErrMessage = "Mail Address was not added, please try again or inform Help Desk";
            }
        }

        //public bool InsertParameter()
        //{
        //    var pe = new FabricationParameterEntitiy();

        //    pe.Material = _view.Material;
        //    pe.Cylinder = _view.Cylinder;
        //    pe.MaxPlus = _view.MaxPlus;
        //    pe.MaxMinus = _view.MaxMinus;
        //    pe.IsStocked = _view.IsStocked.ToBoolean();
        //    pe.SiteCode = _view.SiteCode;
        //    pe.ModifiedBy = _view.mySession.ModifiedBy;

        //    var r = new FabricationParametersRepository();
        //    return r.InsertParameter(pe);
        //}

        //public bool UpdateParameter(int id)
        //{
        //    var pe = new FabricationParameterEntitiy();

        //    pe.ID = id;
        //    pe.Material = _view.Material;
        //    pe.Cylinder = _view.Cylinder;
        //    pe.MaxPlus = _view.MaxPlus;
        //    pe.MaxMinus = _view.MaxMinus;
        //    pe.IsStocked = _view.IsStocked.ToBoolean();
        //    pe.SiteCode = _view.SiteCode;
        //    pe.ModifiedBy = _view.mySession.ModifiedBy;
        //    pe.IsActive = true;

        //    var r = new FabricationParametersRepository();
        //    return r.UpdateParameter(pe);
        //}

        //public void DeleteParameter(int id)
        //{
        //    var r = new FabricationParametersRepository();
        //    r.DeleteParameter(id);
        //}
    }
}