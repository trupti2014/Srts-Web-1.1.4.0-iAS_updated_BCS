using SrtsWeb.BusinessLayer.Concrete;
using SrtsWeb.DataLayer.Repositories;
using SrtsWeb.Entities;
using SrtsWeb.ExtendersHelpers;
using SrtsWeb.Views.JSpecs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SrtsWeb.Presenters.JSpecs
{
    public class JSpecsNewOrderPresenter
    {
        private IJSpecsNewOrderView _view;

        public JSpecsNewOrderPresenter(IJSpecsNewOrderView view)
        {
            _view = view;
        }
        /// <summary>
         /// Initialize view on page load.
         /// </summary>
        public void InitView()
        {
            GetUsersAddresses();
            GetUsersEmailAddresses();
            GetUsersActivePrescriptions();
            GetStatesAndCountries();
            // GetFrameFamilies();
        }

        public void GetAllFrames()
        {
            string siteCode = "008094";
            FrameRepository glassesRepository = new FrameRepository();
            List<FrameEntity> frames = glassesRepository.GetAllFrames(siteCode);
            _view.UserFrames = frames;
        }
        /// <summary>
        /// Get All user email addresses, and set the view.
        /// </summary>
        private void GetUsersEmailAddresses()
        {
            EMailAddressRepository emailRepository = new EMailAddressRepository();
            List<EMailAddressEntity> emailAddresses = emailRepository.GetEmailAddressesByIndividualID(_view.userInfo.Patient.Individual.ID, HttpContext.Current.User.Identity.Name);
            _view.UserEmailAddresses = emailAddresses;
        }

        /// <summary>
        /// Get all user addresses, and set the view.
        /// </summary>
        private void GetUsersAddresses()
        {
            AddressRepository addressRepository = new AddressRepository();
            List<AddressEntity> addresses = addressRepository.GetAddressesByIndividualID(_view.userInfo.Patient.Individual.ID, HttpContext.Current.User.Identity.Name);
            _view.UserAddresses = addresses;
        }

        /// <summary>
        /// GetUsersPrescriptions() sets the view for all prescriptions
        /// that are <= two years old.
        /// </summary>
        private void GetUsersActivePrescriptions()
        {
            IndividualPrescriptionRepository prescriptionRepository = new IndividualPrescriptionRepository();
            DateTime today = DateTime.UtcNow;
            int twoYears = 730;

            List<PrescriptionEntity> prescriptions = prescriptionRepository.GetPrescriptionsByIndividualID(_view.userInfo.Patient.Individual.ID, HttpContext.Current.User.Identity.Name);
            _view.UserPrescriptions = prescriptions.Where(prescription => (today - prescription.PrescriptionDate).TotalDays < twoYears).OrderByDescending(prescription => prescription.PrescriptionDate).ToList();
        }

        /// <summary>
        /// Get all of the states, and countries data.
        /// </summary>
        private void GetStatesAndCountries()
        {
            _view.CountryData = _view.LookupCache.GetByType(LookupType.CountryList.ToString());
            _view.StateData = _view.LookupCache.GetByType(LookupType.StateList.ToString());
        }

        /// <summary>
        /// Retrieve Patients order by order id.
        /// </summary>
        /// <param name="orderNumber"></param>
        public void GetPatientsOrderByOrderId(string orderNumber)
        {
            var _orderReposity = new OrderRepository.GEyesOrderRepository();
            _view.userInfo.JSpecsOrder.OrderID = orderNumber;
            _view.userInfo.JSpecsOrder.IsReOrder = true;

            _view.userInfo.JSpecsOrder.PatientOrder = _orderReposity.GetOrderByOrderNumber(orderNumber, HttpContext.Current.User.Identity.Name);
        }

        /// <summary>
        /// Insert a new email address for the patient.
        /// </summary>
        /// <param name="emailAddr">email address to be added.</param>
        public void InsertNewEmailAddress(EMailAddressEntity emailAddr)
        {
            EMailAddressRepository emailRepository = new EMailAddressRepository();
            emailAddr.ModifiedBy = "JSPECS-SELF";
            emailAddr.IsActive = true;
            _view.UserEmailAddresses = emailRepository.InsertEMailAddress(emailAddr);
        }

        /// <summary>
        /// Check if patient already has an email address associated with there account.
        /// </summary>
        /// <param name="individualID">id of the individual</param>
        /// <param name="email">email being checked</param>
        /// <returns></returns>
        public bool PatientHasEmailAddress(int individualID, string email)
        {
            EMailAddressRepository emailRepository = new EMailAddressRepository();
            List<EMailAddressEntity> emails = emailRepository.GetEmailAddressesByIndividualID(individualID, _view.userInfo.Patient.Individual.NameFiL);

            if (emails.Any(e => e.EMailAddress.ToLower() == email.ToLower()))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Check to see if address is already in database
        /// </summary>
        public bool PatientHasAddress(int individualID, AddressEntity address)
        {
            AddressRepository addressRepository = new AddressRepository();
            List<AddressEntity> addresses = addressRepository.GetAddressesByIndividualID(individualID, _view.userInfo.Patient.Individual.NameFiL);
            if (addresses.Any(e => e.FormattedAddress == address.FormattedAddress))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Insert a new address for the patient.
        /// </summary>
        /// <param name="address">An address</param>
        public void InsertNewAddress(AddressEntity address)
        {
            AddressRepository addressRepository = new AddressRepository();

            address.IndividualID = _view.userInfo.Patient.Individual.ID;
            address.ModifiedBy = "JSPECS-SELF";
            address.IsActive = true;

            _view.UserAddresses = addressRepository.InsertAddress(address);
        }

        /// <summary>
        /// Get the rx frame restions by frame code
        /// </summary>
        /// <param name="frameCode">a string of a framecode</param>
        /// <returns></returns>
        public FrameRxRestrictionsEntity GetFrameRxRestrictionsByFrameCode(string frameCode)
        {
            FrameRepository.FrameRxRestrictions frameRepository = new FrameRepository.FrameRxRestrictions();
            FrameRxRestrictionsEntity frameRestrictions = new FrameRxRestrictionsEntity();
            frameRestrictions = frameRepository.GetFrameRxRestrictionsByFrameCode(frameCode);

            return frameRestrictions;
        }

        /// <summary>
        /// Get USPS recommended address from address supplied.
        /// </summary>
        /// <param name="address">A AddressEntity to check with USPS service</param>
        /// <returns>USPS recommended address</returns>
        public AddressEntity GetUSPSRecommendedAddress(AddressEntity address)
        {
            USPSService uspsService = new USPSService();
            return uspsService.ValidateAddress(address);
        }
        public JSpecsOrder NewJSpecsOrder()
        {
            JSpecsOrder jo = new JSpecsOrder();
            jo.PatientOrder = new OrderEntity();
            jo.OrderAddress = new AddressEntity();
            jo.OrderPrescription = new PrescriptionEntity();
            jo.OrderEmailAddress = new EMailAddressEntity();
            jo.OrderID = "";
            jo.IsReOrder = false;
            return jo;
        }
        public OrderEntity NewOrderEntity()
        {
            OrderEntity oe = new OrderEntity();
            oe.ClinicSiteCode = "008094";
            oe.DateLastModified = new DateTime();
            oe.PrescriptionID = 0;
            oe.IndividualID_Patient = 0;
            oe.IndividualID_Tech = 5767;
            oe.PatientPhoneID = 0;
            oe.LabSiteCode = "";
            oe.LensMaterial = "PLAS";
            oe.LensType = "SVD";
            oe.LensTypeLong = "SINGLE VISION DISTANCE";
            oe.LensTint = "CLEAR";
            oe.LensCoating = "";
            oe.LocationCode = "";
            oe.ModifiedBy = "JSPECS-SELF";
            oe.NumberOfCases = 1;
            oe.Pairs = 1;
            oe.OrderNumber = "";
            oe.ShipAddress1 = "";
            oe.ShipAddress2 = "";
            oe.ShipAddress3 = "";
            oe.ShipCity = "";
            oe.ShipState = "";
            oe.ShipZipCode = "";
            oe.ShipAddressType = "";
            oe.ShipCountry = "";
            oe.OrderDisbursement = "L2P";
            oe.Tint = "CL";
            oe.UserComment1 = "";
            oe.UserComment2 = "";
            oe.FrameBridgeSize = "18";
            oe.FrameCode = "";
            oe.FrameColor = "SLV";
            oe.FrameFamily = "Fallback";
            oe.FrameCategory = "PMI";
            oe.FrameEyeSize = "51";
            oe.FrameTempleType = "NONE";
            oe.FrameDescription = "";

            oe.Demographic = "O09M11N";
            oe.IsGEyes = false;
            oe.IsActive = true;
            oe.IsMultivision = false;
            oe.ODSegHeight = "";
            oe.OSSegHeight = "";
            oe.VerifiedBy = 0;
            oe.CorrespondenceEmail = "";
            oe.OnholdForConfirmation = false;
            oe.MedProsDispense = false;
            oe.PimrsDispense = false;
            return oe;
        }
    }
}
