using SrtsWeb.BusinessLayer.Abstract;
using SrtsWeb.BusinessLayer.Enumerators;
using SrtsWeb.Views.Public;
using SrtsWeb.ExtendersHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SrtsWeb.BusinessLayer.Presenters.Public
{
    public sealed class FacilityAccountRequestPresenter
    {
        private IFacilityAccountRequestView _view;

        public FacilityAccountRequestPresenter(IFacilityAccountRequestView view)
        {
            _view = view;
        }

        public void InitView()
        {
            FillDDLs();
        }

        private void FillDDLs()
        {
            _view.StateDDL = _view.LookupCache.GetByType(LookupType.StateList.ToString());
            _view.CountryDDL = _view.LookupCache.GetByType(LookupType.CountryList.ToString());
            _view.FacilityTypeDDL = _view.LookupCache.GetByType(LookupType.SiteType.ToString());
            _view.BOS_DDL = _view.LookupCache.GetByType(LookupType.BOSType.ToString());
        }

        public bool SendEmail(IMailService mailService)
        {
            try
            {
                var sb = new StringBuilder();
                sb.AppendFormat("The following individual submitted a request for a new facility record:{0}", Environment.NewLine);
                sb.AppendFormat("Requestors Name:  {0} {1}{2}", _view.RequestorsTitle, _view.RequestorsName, Environment.NewLine);
                sb.AppendFormat("Requestors Phone: {0}{1}", _view.RequestorsWorkPhone, Environment.NewLine);
                sb.AppendFormat("Requestors DSN:   {0}{1}", _view.RequestorsDSNPhone, Environment.NewLine);
                sb.AppendFormat("Requestors Email: {0}{1}", _view.RequestorsEmail, Environment.NewLine);
                sb.AppendFormat("Requestors Fax:   {0}{1}", _view.RequestorsFax, Environment.NewLine);
                sb.AppendFormat("{0}Please see the listed items pertaining to the requested facility:{1}", Environment.NewLine, Environment.NewLine);
                sb.AppendFormat("{0}Unit Name:   {1}{2}", Environment.NewLine, _view.UnitName, Environment.NewLine);
                sb.AppendFormat("Unit Address1:  {0}{1}", _view.UnitAddress1, Environment.NewLine);
                sb.AppendFormat("Unit Address2:  {0}{1}", _view.UnitAddress2, Environment.NewLine);
                sb.AppendFormat("Unit Address3:  {0}{1}", _view.UnitAddress3, Environment.NewLine);
                sb.AppendFormat("Unit City:      {0}{1}", _view.UnitCity, Environment.NewLine);
                sb.AppendFormat("Unit State:     {0}{1}", _view.UnitState, Environment.NewLine);
                sb.AppendFormat("Unit ZipCode:   {0}{1}", _view.UnitZipCode, Environment.NewLine);
                sb.AppendFormat("Facility Type:  {0}{1}", _view.UnitFacilityType, Environment.NewLine);
                sb.AppendFormat("Facility BOS:   {0}{1}", _view.FacilityBOS, Environment.NewLine);
                sb.AppendFormat("Component:      {0}{1}", _view.FacilityComponent, Environment.NewLine);

                var fromEmail = ("usarmy.jbsa.medcom-usamitc.mbx.usamitc-srts@mail.mil");
                var toEmail = new List<String> { _view.RequestorsEmail, "nostra-customerservice@med.navy.mil" };
                var subject = "SRTS New Facility Request";
                mailService.SendEmail(sb.ToString(), fromEmail, toEmail, subject);
                return true;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Mail Service Error"));
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return false;
            }
        }
    }
}