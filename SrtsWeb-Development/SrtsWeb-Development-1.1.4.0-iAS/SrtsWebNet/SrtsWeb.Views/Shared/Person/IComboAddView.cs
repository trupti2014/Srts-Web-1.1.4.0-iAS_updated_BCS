using SrtsWeb.Entities;
using System;
using System.Collections.Generic;
using System.Data;

namespace SrtsWeb.Views.Shared.Person
{
    public interface IComboAddView : IAddress, IEmailAddress, IIdNumber, IIndividualType, IPhoneNumber
    {
        SRTSSession mySession { get; set; }

        List<LookupTableEntity> LookupCache { get; set; }

        DataTable IDTypeDDL { set; }

        String IDNumberMessage { get; set; }

        DataTable CountryDDL { set; }

        DataTable StateDDL { set; }

        DataTable AddressTypeDDL { set; }

        DataTable UICDDL { set; }

        String AddrMessage { get; set; }

        DataTable TypeEmailDDL { set; }

        String EmailMessage { get; set; }

        DataTable PhoneTypeDDL { set; }

        String PhoneMessage { get; set; }

        DataTable IndividualTypeDDL { set; }

        String IndividualTypeMessage { get; set; }
    }
}