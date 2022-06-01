﻿using SrtsWeb.DataLayer.Repositories;
using SrtsWeb.Entities;
using SrtsWeb.ExtendersHelpers;
using SrtsWeb.Views.GEyes;
using System.Linq;

namespace SrtsWeb.Presenters.GEyes
{
    public sealed class IndividualInfoPresenter
    {
        private IIndividualInfoView _view;

        public IndividualInfoPresenter(IIndividualInfoView view)
        {
            _view = view;
        }

        public void InitView()
        {
            _view.IDNumber = string.Empty;
            _view.EmailAddress = string.Empty;
            _view.EmailAddressConfirm = string.Empty;
            _view.ZipCode = string.Empty;
        }

        public bool CheckZip()
        {
            bool cont = false;
            var tcr = new TheaterCodeRepository();
            var dt = tcr.GetActiveTheaterCodes();
            if (dt.Any(x => x.TheaterCode == _view.ZipCode.ToZipCodeRemoveDash()))
                cont = true;
            else
                _view.Message = "This is not a valid theater zip code.  Please check your information and try again.";
            return cont;
        }

        public bool CheckUser()
        {
            bool cont = false;
            var _individualRepository = new IndividualRepository.PatientRepository();
            _view.myInfo.Patient = new PatientEntity();
            _view.ClinicCode = "009900";  // to do get from sp
            var p = new PatientEntity();
            p = _individualRepository.GetAllPatientInfoByIDNumber(_view.IDNumber.ToSSNRemoveDash(), _view.IDType, _view.ClinicCode);
            if (!p.IsNull() && !p.Individual.IsNull() && !p.IDNumbers.IsNullOrEmpty())
            {
                _view.myInfo.Patient = p;
                cont = true;
            }
            else
            {
                _view.Message = "No record found.  Please check your information and try again.";
            }

            return cont;
        }

        public string Name()
        {
            string name = string.Empty;
            name = string.Format("{0} {1}", _view.myInfo.Patient.Individual.FirstName, _view.myInfo.Patient.Individual.LastName);

            return name;
        }
    }
}