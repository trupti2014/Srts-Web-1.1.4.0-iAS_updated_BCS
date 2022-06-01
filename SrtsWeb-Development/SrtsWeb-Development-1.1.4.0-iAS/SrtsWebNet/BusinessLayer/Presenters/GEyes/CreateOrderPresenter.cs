using SrtsWeb.BusinessLayer.Enumerators;
using SrtsWeb.BusinessLayer.Views.GEyes;
using SrtsWeb.DataLayer.Repositories;
using SrtsWeb.Entities;
using SrtsWeb.ExtendersHelpers;
using System.Data;
using System.Linq;

namespace SrtsWeb.BusinessLayer.Presenters.GEyes
{
    public sealed class CreateOrderPresenter : PresenterBase
    {
        private ICreateOrderView _view;
        private IFrameRepository _frameRepository;
        private IPrescriptionRepository _rxRepository;

        public CreateOrderPresenter(ICreateOrderView view)
        {
            _view = view;
        }

        public void InitView()
        {
        }

        public void FillGrids(string _zipCode)
        {
            ITheaterCodeRepository _tcr = new TheaterCodeRepository();
            _view.LocationData = _tcr.GetActiveTheaterCodes();
            _view.LocationSelected = _zipCode;
            _view.PriorityData = Helpers.GetLookupTypesSelected(_view.LookupCache, LookupType.OrderPriorityType.ToString());
            _view.PrioritySelected = "W";

            string demo = _view.myInfo.Patient.Individual.Demographic;
            if (demo.Substring(5, 1) != "R")
            {
            }
            FillFrameData();
        }

        public void FillFrameData()
        {
            DataView dv;
            if (_view.FrameData == null)
            {
                _frameRepository = new FrameRepository();
                dv = new DataView(_frameRepository.GetFramesAndItemsByEligibility(_view.myInfo.Patient.Individual.Demographic, _view.myInfo.Patient.Individual.SiteCodeID));
                _view.myInfo.FrameData = dv;
                _view.FrameData = dv.ToTable(true, "FrameCode", "FrameDescription");
            }
            dv = _view.myInfo.FrameData;
            FillItemsData(dv);
        }

        public void FillItemsData(DataView dv)
        {
            dv.RowFilter = string.Format("TypeEntry = 'COLOR' AND FrameCode = '{0}'", _view.FrameSelected);
            _view.ColorData = dv.ToTable(true, "Text", "Value");
            dv.RowFilter = string.Format("TypeEntry = 'TINT' AND FrameCode = '{0}'", _view.FrameSelected);
            _view.TintData = dv.ToTable(true, "Text", "Value");
            _view.Pairs = 1;
            _view.Cases = 1;
        }

        public void SaveData()
        {
            PrescriptionEntity peOriginal = new PrescriptionEntity();
            OrderEntity _saveOE = new OrderEntity();
            _rxRepository = new PrescriptionRepository();
            peOriginal = Helpers.ProcessPresciptionRows(_rxRepository.GetPrescriptionsByIndividualID(_view.myInfo.Patient.Individual.ID, _view.myInfo.OrderToSave.ModifiedBy.ToString()).Rows[0]);

            _saveOE = new OrderEntity();
            _saveOE.PrescriptionID = peOriginal.ID;
            _saveOE.ClinicSiteCode = this.GeyesSiteCode;
            _saveOE.Demographic = _view.myInfo.Patient.Individual.Demographic;
            _saveOE.FrameBridgeSize = string.Empty;

            _saveOE.FrameCode = _view.FrameSelected;
            _saveOE.FrameColor = _view.ColorSelected;
            _saveOE.FrameEyeSize = string.Empty;

            _saveOE.FrameTempleType = string.Empty;

            _saveOE.IndividualID_Patient = _view.myInfo.Patient.Individual.ID;
            _saveOE.IndividualID_Tech = _view.myInfo.Patient.Individual.ID;
            _saveOE.IsGEyes = true;
            _saveOE.IsMultivision = false;
            _saveOE.LabSiteCode = "MNOST1";
            _saveOE.LocationCode = _view.LocationSelected;
            _saveOE.ModifiedBy = string.Format("SELF");
            _saveOE.NumberOfCases = _view.Cases;
            _saveOE.LensMaterial = string.Empty;
            _saveOE.LensType = string.Empty;
            _saveOE.ODSegHeight = "0";

            _saveOE.OSSegHeight = "0";
            _saveOE.Pairs = _view.Pairs;
            _saveOE.ShipToPatient = true;

            var type = "UNIT";

            var id = (from n in _view.myInfo.Patient.PhoneNumbers
                      where n.PhoneNumberType == type
                      select n.ID).FirstOrDefault();

            _saveOE.PatientPhoneID = id;

            _saveOE.Tint = _view.TintSelected;
            _saveOE.UserComment1 = _view.Comment;
            _saveOE.UserComment2 = string.Empty;
            _saveOE.VerifiedBy = 0;

            _view.myInfo.OrderToSave = _saveOE;
        }
    }
}