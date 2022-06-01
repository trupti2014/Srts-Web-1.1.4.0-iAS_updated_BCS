using SrtsWeb.Entities;
using System.Collections.Generic;
using System.Data;

namespace SrtsWeb.BusinessLayer.Views.Patients
{
    public interface IComboAddView
    {
        SRTSSession mySession { get; set; }

        List<LookupTableEntity> LookupCache { get; set; }

        List<LookupTableEntity> IDTypeDDL { set; }

        string IDNumber { get; set; }

        string IDNumberType { get; set; }

        string IDNumberMessage { get; set; }

        string Address1 { get; set; }

        string Address2 { get; set; }

        string City { get; set; }

        string CountrySelected { get; set; }

        List<LookupTableEntity> CountryDDL { set; }

        string StateSelected { get; set; }

        List<LookupTableEntity> StateDDL { set; }

        string AddressTypeSelected { get; set; }

        List<LookupTableEntity> AddressTypeDDL { set; }

        string ZipCode { get; set; }

        string UIC { get; set; }

        DataTable UICDDL { set; }

        string UICSelected { get; set; }

        string AddrMessage { get; set; }

        string EMailAddress { get; set; }

        string TypeEMailSelected { get; set; }

        List<LookupTableEntity> TypeEmailDDL { set; }

        string EmailMessage { get; set; }

        string Extension { get; set; }

        string PhoneNumber { get; set; }

        List<LookupTableEntity> PhoneTypeDDL { set; }

        string TypePhoneSelected { get; set; }

        string PhoneMessage { get; set; }

        string IndividualTypeMessage { get; set; }
    }
}