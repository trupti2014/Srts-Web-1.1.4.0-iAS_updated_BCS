 using SrtsWeb.DataLayer.Repositories;
using SrtsWeb.Entities;
using SrtsWeb.ExtendersHelpers;
using SrtsWeb.Views.JSpecs;
using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using System.Web;

namespace SrtsWeb.Presenters.JSpecs
{
    public sealed class JSpecsLoginPresenter
    {
        private IJSpecsLoginView _view;

        public JSpecsLoginPresenter(IJSpecsLoginView view)
        {
            _view = view;
        }

        public void InitView()
        {
            _view.ClinicCode = "008094";  // todo: get from lookup or stored procedure (lmb 6/2/2020)
        }

        /// <summary>
        /// Stub login to login into the testing account
        /// </summary>
        public void StubLogin()
        {
            // Log into the main test account 332411497 ssn
            //GetPatientByID("1536141185", "DIN", _view.ClinicCode); // Lara test account
            GetPatientByID("332411497", "SSN", _view.ClinicCode);
        }

        /// <summary>
        /// Look up patient by there EDIPI
        /// </summary>
        /// <param name="id">EDIPI id from CAC</param>
        /// <param name="idType">idType that should be DIN</param>
        /// <returns></returns>
        /// 
        public bool GetPatientByID(string id, string idType, string clinicCode)
        {
            var _individualRepository = new IndividualRepository.PatientRepository();
            _view.userInfo = new JSpecsSession();
            _view.userInfo.Patient = new PatientEntity();
           
            var p = _individualRepository.GetAllPatientInfoByIDNumber(id, idType, clinicCode);
            if (!p.IsNull() && !p.Individual.IsNull() && !p.IDNumbers.IsNullOrEmpty())
            {
                _view.userInfo.Patient = p;
                return true;
            }
            else
            {
                // User not on record
                return false;
            }
        }
    }
}
