using SrtsWeb.BusinessLayer.Abstract;
using SrtsWeb.BusinessLayer.BmtSrDev;
using SrtsWeb.Entities;
using SrtsWeb.ExtendersHelpers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace SrtsWeb.BusinessLayer.Concrete
{
    /// <summary>
    /// Custom class used to perform BMT operations.
    /// </summary>
    public class BmtService : IBmtService
    {
        /// <summary>
        /// Method used to upload a trainee list to SRTSWeb via the BmtWebService and returns a list of failed trainee additions.  The output argument, 'traineesAdded' is the number of successfully added trainees.
        /// </summary>
        /// <param name="traineeList">System.Collections.Generic.IEnumerable of BmtEntity</param>
        /// <param name="siteCode">String</param>
        /// <param name="traineesAdded">Int32</param>
        /// <returns>System.Collections.Generic.IEnumerable of BmtEntity - Failed trainee additions</returns>
        public IEnumerable<BmtEntity> UpdateBmt(IEnumerable<BmtEntity> traineeList, String siteCode, out Int32 traineesAdded)
        {
            using (var svc = GetServiceObject())
            {
                if (svc.IsNull()) throw new Exception("Error creating a service reference to the BMT upload web service.");

                var tl = new List<BmtServiceEntity>();
                traineeList.ToList().ForEach(x => tl.Add(ConvertToServiceEntity(x)));

                // Add the site code to the trainee list
                tl.ToList().ForEach(x => x.SiteCode = siteCode);

                var result = svc.UploadBmtData(tl.ToArray());

                if (!result.Success) { traineesAdded = 0; return null; }

                traineesAdded = result.TraineesAdded;
                var bad = new List<BmtEntity>();
                result.FailedToAdd.ToList().ForEach(x => bad.Add(ConvertToSrtsEntity(x)));
                return bad;
            }
        }

        /// <summary>
        /// Converting the BmtWebService defined BmtServiceEntity into the SRTS defined custom class BmtEntity.
        /// </summary>
        /// <param name="entity">BmtServiceEntity - BMT web service class</param>
        /// <returns>BmtEntity - SRTSWeb custom entity</returns>
        public BmtEntity ConvertToSrtsEntity(BmtServiceEntity entity)
        {
            var b = new BmtEntity()
            {
                Address1 = entity.Address1,
                Address2 = entity.Address2,
                Address3 = entity.Address3,
                BOS = entity.BOS,
                City = entity.City,
                Country = entity.Country,
                DOB = entity.DOB,
                ErrorMessage = entity.ErrorMessage,
                FirstName = entity.FirstName,
                Gender = entity.Gender,
                Grade = entity.Grade,
                IdNumber = entity.IdNumber,
                IdNumberType = entity.IdNumberType,
                LastName = entity.LastName,
                MiddleName = entity.MiddleName,
                State = entity.State,
                UIC = entity.UIC,
                ZipCode = entity.ZipCode
            };

            return b;
        }

        /// <summary>
        /// Converting the SRTS defined custom class BmtEntity into the BmtWebService defined BmtServiceEntity.
        /// </summary>
        /// <param name="entity">BmtEntity - SRTSWeb custom entity</param>
        /// <returns>BmtServiceEntity - BMT web service class</returns>
        public BmtServiceEntity ConvertToServiceEntity(BmtEntity entity)
        {
            var b = new BmtServiceEntity()
            {
                Address1 = entity.Address1,
                Address2 = entity.Address2,
                Address3 = entity.Address3,
                BOS = entity.BOS,
                City = entity.City,
                Country = entity.Country,
                DOB = entity.DOB,
                ErrorMessage = entity.ErrorMessage,
                FirstName = entity.FirstName,
                Gender = entity.Gender,
                Grade = entity.Grade,
                IdNumber = entity.IdNumber,
                IdNumberType = entity.IdNumberType,
                LastName = entity.LastName,
                MiddleName = entity.MiddleName,
                State = entity.State,
                UIC = entity.UIC,
                ZipCode = entity.ZipCode
            };

            return b;
        }

        /// <summary>
        /// Method used to connect to the BMT web service and get the web service client.  Requires an X509 Certificate.
        /// </summary>
        /// <returns>BmtWsClient - Client returned by the web service.</returns>
        public BmtWsClient GetServiceObject()
        {
            X509Certificate2 c = null;

            try
            {
                var s = new X509Store(StoreName.My, StoreLocation.LocalMachine);
                s.Open(OpenFlags.OpenExistingOnly | OpenFlags.ReadOnly);

                var cs = s.Certificates.Find(X509FindType.FindBySubjectName, ConfigurationManager.AppSettings["certName"], true);
                if (cs.Count.Equals(0)) return null;

                c = cs[0];
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Certificate error"));
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return null;
            }

            try
            {
                BmtWsClient svc = new BmtWsClient();

                svc.ClientCredentials.ClientCertificate.Certificate = c;

                return svc;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("BMT Upload Service Creation Error"));
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return null;
            }
        }
    }
}