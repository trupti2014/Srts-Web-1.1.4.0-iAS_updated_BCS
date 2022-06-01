using System;
using System.Data;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SrtsWeb.Entities;
using SrtsWeb.Views.Admin;
using SrtsWeb.ExtendersHelpers;
using SrtsWeb.DataLayer.Repositories;
using SrtsWeb.BusinessLayer.Concrete;

namespace SrtsWeb.Presenters.Admin
{
    public class ReportsManagerPresenter
    {
        IReportsManagerView v;
        public ReportsManagerPresenter(IReportsManagerView view)
        { this.v = view; }

        public void GetOrderCountItems(String status, String siteCode) 
        {
            var r = new ReportsManagerRepository();
            this.v.ReprintItems = r.GetReprintCountList(status, siteCode);
        }

        public void GetReprint771(String siteCode, DateTime batchDate)
        {
            var r = new ReportsManagerRepository.ReprintReturnRepository();
            var o = r.GetReprint771("2", siteCode, batchDate);
            this.v.ReprintOrderNumbers = String.IsNullOrEmpty(o) ? String.Empty : o.Substring(0, o.Length - 1);
        }

        public void GetReprintLabels(String status, String siteCode, DateTime batchDate)
        {
            if (status == "od")
            {
                var odr = new OnDemandLabelsRepository.ReprintOnDemandReturnRepository();
                this.v.ReprintOnDemandLabels = odr.GetOnDemandReprintLabel(siteCode, batchDate);
            }
            else
            {
                var r = new ReportsManagerRepository.ReprintReturnRepository();
                this.v.ReprintLabels = r.GetReprintLabel(status, siteCode, batchDate);
            }
        }

        public void GetOnDemandCountItems(String modifiedBy, String siteCode)
        {
            HttpContext.Current.Session["IsHistory"] = "false";
            var r = new ReportsManagerRepository();
            this.v.ReprintItems = r.GetReprintOnDemandCountList(siteCode);
            if (this.v.ReprintItems.Count > 0)
            {
                HttpContext.Current.Session["IsHistory"] = "true";
            }
        }

        public bool InsertOnDemandReprintLabel(ReprintOnDemandInsertEntity ent, String modifiedby)
        {
            var r = new OnDemandLabelsRepository.ReprintOnDemandRepository();
            var result = r.InsertOnDemandLabel(ent, modifiedby);
            return result;
        }


        public void GetOrderAddresses(String orderNumber)
        {
            var r = new OnDemandLabelsRepository.OrderLabelAddressesRepository();
            this.v.OrderAddresses = r.GetAddressesByOrderNumber(orderNumber);
        }

        public bool PrintLabels(List<OrderLabelAddresses> orderLabelAddresses, DataTable LabelTable)
        {
            // If the site preference for the alpha sort is set to true then do sort on the datatable.
            var r = new SitePreferencesRepository.GeneralPreferencesRepostiory();
            var sortAlpha = r.GetPreferences(this.v.mySession.MySite.SiteCode);
            if (sortAlpha)
            {
                List<OrderLabelAddresses> sorted_orderLabelAddresses = new List<OrderLabelAddresses>();
                sorted_orderLabelAddresses = orderLabelAddresses.OrderBy(ob => ob.LastName).ThenBy(ob => ob.FirstName).ToList<OrderLabelAddresses>();
                for (var i = 0; i < sorted_orderLabelAddresses.Count; i++)
                {
                    LabelTable.Rows.Add(BuildLabel(sorted_orderLabelAddresses[i], LabelTable));
                }
            }
            else
            {
                for (var i = 0; i < orderLabelAddresses.Count; i++)
                {
                    LabelTable.Rows.Add(BuildLabel(orderLabelAddresses[i], LabelTable));
                }
            }
            HttpContext.Current.Session["LabelTable"] = LabelTable;
            HttpContext.Current.Session["lType"] = v.SelectedLabel;
            return true;
        }


        protected DataRow BuildLabel(OrderLabelAddresses orderlabeladdress, DataTable LabelTable)
        {
            var dr = LabelTable.NewRow();
            //Create data table that matches the what the xml headers/elements are
            bool useMailingAddress = orderlabeladdress.UseMailingAddress;
            switch (useMailingAddress)
            {
                case true:
                    dr = returnMailingAddressLabel(orderlabeladdress, LabelTable);
                    break;
                case false:
                    dr = returnOrderAddressLabel(orderlabeladdress, LabelTable);
                    break;
            }

            return dr;
        }

        protected DataRow returnOrderAddressLabel(OrderLabelAddresses orderlabeladdress, DataTable LabelTable)
        {
            var dr = LabelTable.NewRow();
            string country = string.Empty;
            string mInitial = string.IsNullOrEmpty(orderlabeladdress.MiddleName) ? " " : " " + orderlabeladdress.MiddleName.FirstOrDefault().ToString() + ". ";
            dr["Name"] = string.Format("{0} {1} {2}", orderlabeladdress.FirstName, mInitial, orderlabeladdress.LastName);
            dr["Address"] = string.IsNullOrEmpty(orderlabeladdress.ShipAddress2) ? orderlabeladdress.ShipAddress1 : string.Format("{0}{1}{2}", orderlabeladdress.ShipAddress1, Environment.NewLine, orderlabeladdress.ShipAddress2);
            dr["City"] = orderlabeladdress.ShipCity;
            dr["State"] = orderlabeladdress.ShipState;
            dr["PostalCode"] = orderlabeladdress.ShipZipCode.ToZipCodeLabelPrint();
            if (!string.IsNullOrEmpty(orderlabeladdress.ShipCountryCode))
            {
                country = Helpers.GetCountryDescription(orderlabeladdress.ShipCountryCode);
            }
            dr["Country"] = string.IsNullOrEmpty(country) ? "US" : country;
            return dr;
        }

        protected DataRow returnMailingAddressLabel(OrderLabelAddresses orderlabeladdress, DataTable LabelTable)
        {
            var dr = LabelTable.NewRow();
            string country = string.Empty;
            string mInitial = string.IsNullOrEmpty(orderlabeladdress.MiddleName) ? " " : " " + orderlabeladdress.MiddleName.FirstOrDefault().ToString() + ". ";
            dr["Name"] = string.Format("{0} {1} {2}", orderlabeladdress.FirstName, mInitial, orderlabeladdress.LastName);
            dr["Address"] = string.IsNullOrEmpty(orderlabeladdress.Address2) ? orderlabeladdress.Address1 : string.Format("{0}{1}{2}", orderlabeladdress.Address1, Environment.NewLine, orderlabeladdress.Address2);
            dr["City"] = orderlabeladdress.City;
            dr["State"] = orderlabeladdress.State;
            dr["PostalCode"] = orderlabeladdress.ZipCode.ToZipCodeLabelPrint();
            if (!string.IsNullOrEmpty(orderlabeladdress.CountryCode))
            {
                country = Helpers.GetCountryDescription(orderlabeladdress.CountryCode);
            }
            dr["Country"] = string.IsNullOrEmpty(country) ? "US" : country;
            return dr;
        }

        public void FillPatientAddressDdls()
        {
            // Address DDLs
            this.v.States = v.LookupCache.GetByType(LookupType.StateList.ToString());
            this.v.Countries = v.LookupCache.GetByType(LookupType.CountryList.ToString());
        }

        public void FillPatientData()
        {
            if (!string.IsNullOrEmpty(v.mySession.SelectedPatientID.ToString()) || v.mySession.SelectedPatientID.ToString() != "0")
            {
                v.mySession.Patient = GetAllPatientInfo(v.mySession.SelectedPatientID, v.mySession.ModifiedBy);
                FillPersonData();
            }
        }

        private PatientEntity GetAllPatientInfo(int individualID, string modifiedBy)
        {
            var _repository = new IndividualRepository.PatientRepository();
            var pe = _repository.GetAllPatientInfoByIndividualID(individualID, true, modifiedBy, v.mySession.MySite.SiteCode);
            return pe;
        }

        private void FillPersonData()
        {
            // Address DDLs
            this.v.States = v.LookupCache.GetByType(LookupType.StateList.ToString());
            this.v.Countries = v.LookupCache.GetByType(LookupType.CountryList.ToString());

            // Mailing Address
            var addy = v.mySession.Patient.Addresses;
            this.v.PatientAddress = addy.FirstOrDefault(x => x.IsActive == true);
        }

        //public Boolean DoSaveAddress(AddressEntity entity)
        //{
        //    var msg = String.Empty;
        //    var svc = new SharedAddressService(entity, v.mySession);
        //    return svc.DoSaveAddress(out msg);
        //}

        public Boolean DoSaveAddress(AddressEntity entity)
        {
            var msg = String.Empty;
            var svc = new SharedAddressService(entity, v.mySession);
            var good = svc.DoSaveAddress(out msg);
            return good;
        }


    }
}
