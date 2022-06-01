using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using SrtsWeb.Entities;
using SrtsWeb.ExtendersHelpers;
using SrtsWeb.DataLayer.Interfaces;
using SrtsWeb.DataLayer.Repositories;

namespace SrtsWeb.BusinessLayer.Concrete
{
    /// <summary>
    /// A custom class to perform address save/update operations.
    /// </summary>
    public class SharedAddressService
    {
        private AddressEntity _address;
        private SRTSSession _mySession;

        /// <summary>
        /// Default Ctor.
        /// </summary>
        /// <param name="address">Address to save/update</param>
        /// <param name="mySession">SRTSSession session state object.</param>
        public SharedAddressService(AddressEntity address, SRTSSession mySession)
        {
            this._address = address;
            this._mySession = mySession;
        }

        #region ADDRESS OPS

        /// <summary>
        /// Method does a insert/update operation on an address.  Returns a true if successful.
        /// </summary>
        /// <param name="message">Output string for an error message if applicable.</param>
        /// <returns>True if the save actions is successful.</returns>
        public Boolean DoSaveAddress(out String message)
        {
            if (_address.IsNull()) { message = "No address to save"; return false; }
            // If there is no original entity then this is an add
            if (_mySession.Patient.Addresses.IsNull())
            {
                var ar = new AddressRepository();
                _mySession.Patient.Addresses = ar.GetAddressesByIndividualID(_mySession.Patient.Individual.ID, _mySession.ModifiedBy);
            }
            if (_mySession.Patient.Addresses.FirstOrDefault(x => x.AddressType == _address.AddressType && x.IsActive).IsNull())
            {
                if (String.IsNullOrEmpty(_address.Address1) && String.IsNullOrEmpty(_address.City) && _address.State.Equals("X") && _address.Country.Equals("X") && String.IsNullOrEmpty(_address.ZipCode)) { message = String.Empty; return true; }
                return SaveAddress(_address, new AddressRepository(), out message);
            }
            if (!IsAddressEntityDirty(_mySession.Patient.Addresses.FirstOrDefault(x => x.AddressType == _address.AddressType && x.IsActive), _address)) { message = String.Empty; return true; }
            // if the address fields are blank then it is to be removed
            _address.IsActive = !_address.Address1.IsNull() && !_address.Address2.IsNull() && !_address.City.IsNull() && !_address.ZipCode.IsNull() && !_address.UIC.IsNull();

            return UpdateAddress(_address, new AddressRepository(), out message);
        }

        private Boolean IsAddressEntityDirty(AddressEntity original, AddressEntity compare)
        {
            if (!original.AddressType.ToLower().Equals(compare.AddressType.ToLower())) return true;
            if (!original.Address1.ToLower().Equals(compare.Address1.ToLower())) return true;
            if (!original.Address2.ToLower().Equals(compare.Address2.ToLower())) return true;
            if (!original.City.ToLower().Equals(compare.City.ToLower())) return true;
            if (!original.State.ToLower().Equals(compare.State.ToLower())) return true;
            if (!original.Country.ToLower().Equals(compare.Country.ToLower())) return true;
            if (!original.ZipCode.ToLower().Equals(compare.ZipCode.ToLower())) return true;
            if (!original.DateLastModified.Equals(compare.DateLastModified)) return true;
            if (!original.DateVerified.Equals(compare.DateVerified)) return true;
            if (!original.ExpireDays.Equals(compare.ExpireDays)) return true;
            if (!original.UIC.ToLower().Equals(compare.UIC.ToLower())) return true;
            HttpContext.Current.Session["PrimaryAddress"] = original;
            return false;
        }

        private Boolean SaveAddress(AddressEntity ae, IAddressRepository repository, out String message)
        {
            ae = FillAddressElements(ae);

            var dt = repository.InsertAddress(ae);
            List<AddressEntity> addressSave = dt;
            if (dt.Count > 0)
            {
                message = "Your Address Data was Saved";
                _mySession.Patient.Addresses.Clear();
                _mySession.Patient.Addresses.AddRange(dt.ToArray());

                if (null != HttpContext.Current && null != HttpContext.Current.Session)
                {
                    AddressEntity newAddress = new AddressEntity();
                    newAddress = addressSave.FirstOrDefault(a => a.IsActive == true); 
                    HttpContext.Current.Session["PrimaryAddress"] = newAddress;
                }
                return true;
            }
            else
            {
                message = "Your Address Data was NOT Saved, please try again later or contact the SRTS team to report the problem";
                return false;
            }
        }

        private Boolean UpdateAddress(AddressEntity addr, IAddressRepository repository, out String message)
        {
            addr = FillAddressElements(addr);
            List<AddressEntity> addressUpdate = repository.UpdateAddress(addr);
            //  var addressUpdate = repository.UpdateAddress(addr);

            if (!addressUpdate.IsNullOrEmpty())
            {
                message = "Your Address Data was Updated";
                _mySession.Patient.Addresses.Clear();
                _mySession.Patient.Addresses.AddRange(addressUpdate.ToArray());
                if (null != HttpContext.Current && null != HttpContext.Current.Session)
                {
                    AddressEntity newAddress = new AddressEntity();
                    newAddress = addressUpdate.FirstOrDefault(a => a.IsActive == true);   
                    HttpContext.Current.Session["PrimaryAddress"] = newAddress;
                }
                return true;
            }
            else
            {
                message = "Your Address Data was NOT Saved, please try again later or contact the SRTS team to report the problem";
                return false;
            }
        }

        private AddressEntity FillAddressElements(AddressEntity addr)
        {
            addr.IndividualID = _mySession.Patient.Individual.ID;
            addr.ModifiedBy = _mySession.ModifiedBy;
            return addr;
        }

        #endregion ADDRESS OPS
    }
}
