using SrtsWeb.Entities;
using SrtsWebTrainingAdmin.Repositories;
using System;
using System.Collections.Generic;
using System.Data;

namespace SrtsWebTrainingAdmin.Admin
{
    public sealed class ClassMaintenancePresenter
    {
        private Dictionary<String, Int32> nameId = new Dictionary<String, Int32>();

        private IClassMaintenanceView _view;

        public ClassMaintenancePresenter(IClassMaintenanceView view)
        {
            _view = view;
        }

        public Boolean IsClassNameAvailable(String _name)
        {
            ISiteCodeRepository r = new SiteCodeRepository();
            return r.IsSiteCodeAvailable(_name);
        }

        public void InitView()
        {
        }

        //public void DeleteClass()
        //{
        //    ISiteCodeRepository scr = new SiteCodeRepository();
        //    scr.DeleteClass(_view.DeleteClassID);
        //}

        public void AddSites()
        {
            ISiteCodeRepository scr = new SiteCodeRepository();
            SiteCodeEntity sae = new SiteCodeEntity();
            sae.SiteCode = string.Format("{0}{1}", _view.ClassID, "L");
            sae.SiteName = string.Format("Class {0} laboratory", _view.ClassID);
            sae.SiteType = "LAB";
            sae.SiteDescription = string.Format("Laboratory for class {0}", _view.ClassID);
            sae.BOS = "N";
            sae.IsMultivision = true;
            sae.EMailAddress = "someaddress@mail.mil";
            sae.DSNPhoneNumber = "234.567.8901";
            sae.RegPhoneNumber = "345.678.9012";
            sae.IsAPOCompatible = true;
            sae.MaxEyeSize = 6;
            sae.MaxFramesPerMonth = 500;
            sae.MaxPower = 6;
            sae.HasLMS = false;
            sae.Region = 0;
            sae.MultiPrimary = string.Empty;
            sae.MultiSecondary = string.Empty;
            sae.SinglePrimary = string.Empty;
            sae.SingleSecondary = string.Empty;
            sae.IsActive = true;
            sae.ModifiedBy = "SYSTEM";

            scr.InsertSite(sae);

            SiteAddressEntity ae = new SiteAddressEntity();
            ae.SiteCode = string.Format("{0}{1}", _view.ClassID, "L");
            ae.Address1 = "123 Some Street";
            ae.Address2 = string.Empty;
            ae.Address3 = string.Empty;
            ae.AddressType = "SITE";
            ae.City = "ThisCity";
            ae.Country = "US";
            ae.IsActive = true;
            ae.IsConus = true;
            ae.ModifiedBy = "SYSTEM";
            ae.State = "VA";
            ae.ZipCode = "555550000";

            scr.InsertSiteAddress(ae);

            ae.AddressType = "MAIL";

            scr.InsertSiteAddress(ae);

            sae.SiteCode = string.Format("{0}{1}", _view.ClassID, "C");
            sae.SiteName = string.Format("Class {0} Clinic", _view.ClassID);
            sae.SiteType = "CLINIC";
            sae.SiteDescription = string.Format("Clinic for class {0}", _view.ClassID);
            sae.BOS = "N";
            sae.IsMultivision = true;
            sae.EMailAddress = "someaddress@mail.mil";
            sae.DSNPhoneNumber = "234.567.8901";
            sae.RegPhoneNumber = "345.678.9012";
            sae.IsAPOCompatible = true;
            sae.MaxEyeSize = 6;
            sae.MaxFramesPerMonth = 500;
            sae.MaxPower = 6;
            sae.HasLMS = false;
            sae.Region = 0;
            sae.MultiPrimary = string.Format("{0}{1}", _view.ClassID, "L");
            sae.MultiSecondary = string.Empty;
            sae.SinglePrimary = string.Format("{0}{1}", _view.ClassID, "L");
            sae.SingleSecondary = string.Empty;
            sae.IsActive = true;
            sae.ModifiedBy = "SYSTEM";

            scr.InsertSite(sae);

            ae.SiteCode = string.Format("{0}{1}", _view.ClassID, "C");
            ae.Address1 = "123 Some Street";
            ae.Address2 = string.Empty;
            ae.Address3 = string.Empty;
            ae.AddressType = "SITE";
            ae.City = "ThisCity";
            ae.Country = "US";
            ae.IsActive = true;
            ae.IsConus = true;
            ae.ModifiedBy = "SYSTEM";
            ae.State = "VA";
            ae.ZipCode = "555550000";

            scr.InsertSiteAddress(ae);

            ae.AddressType = "MAIL";

            scr.InsertSiteAddress(ae);
        }

        public Dictionary<String, Int32> CreateStudents()
        {
            string _userName = string.Format("{0}INSTCA", _view.ClassID);
            CreateUser(_userName, string.Format("{0}{1}", _view.ClassID, "C"));
            _userName = string.Format("{0}INSTCT", _view.ClassID);
            CreateUser(_userName, string.Format("{0}{1}", _view.ClassID, "C"));
            _userName = string.Format("{0}INSTCC", _view.ClassID);
            CreateUser(_userName, string.Format("{0}{1}", _view.ClassID, "C"));
            _userName = string.Format("{0}INSTLA", _view.ClassID);
            CreateUser(_userName, string.Format("{0}{1}", _view.ClassID, "L"));
            _userName = string.Format("{0}INSTLT", _view.ClassID);
            CreateUser(_userName, string.Format("{0}{1}", _view.ClassID, "L"));
            _userName = string.Format("{0}INSTLC", _view.ClassID);
            CreateUser(_userName, string.Format("{0}{1}", _view.ClassID, "L"));
            _userName = string.Format("{0}INSTLM", _view.ClassID);
            CreateUser(_userName, string.Format("{0}{1}", _view.ClassID, "L"));

            for (int x = 1; x <= 35; x++)
            {
                _userName = string.Format("{0}{1}CA", _view.ClassID, x.ToString().PadLeft(2, '0'));
                CreateUser(_userName, string.Format("{0}{1}", _view.ClassID, "C"));
                _userName = string.Format("{0}{1}CT", _view.ClassID, x.ToString().PadLeft(2, '0'));
                CreateUser(_userName, string.Format("{0}{1}", _view.ClassID, "C"));
                _userName = string.Format("{0}{1}CC", _view.ClassID, x.ToString().PadLeft(2, '0'));
                CreateUser(_userName, string.Format("{0}{1}", _view.ClassID, "C"));
                _userName = string.Format("{0}{1}LA", _view.ClassID, x.ToString().PadLeft(2, '0'));
                CreateUser(_userName, string.Format("{0}{1}", _view.ClassID, "L"));
                _userName = string.Format("{0}{1}LT", _view.ClassID, x.ToString().PadLeft(2, '0'));
                CreateUser(_userName, string.Format("{0}{1}", _view.ClassID, "L"));
                _userName = string.Format("{0}{1}LC", _view.ClassID, x.ToString().PadLeft(2, '0'));
                CreateUser(_userName, string.Format("{0}{1}", _view.ClassID, "L"));
                _userName = string.Format("{0}{1}LM", _view.ClassID, x.ToString().PadLeft(2, '0'));
                CreateUser(_userName, string.Format("{0}{1}", _view.ClassID, "L"));
            }

            return this.nameId;
        }

        private void CreateUser(string _name, string _siteCode)
        {
            DataSet ds = new DataSet();
            PatientEntity pe = new PatientEntity();
            IndividualEntity ie = new IndividualEntity();

            ie.PersonalType = "OTHER";
            ie.FirstName = _name;
            ie.MiddleName = _name;
            ie.LastName = _name;
            ie.DateOfBirth = DateTime.Parse("1/1/1900");
            ie.EADStopDate = DateTime.Parse("1/1/1900");
            ie.Demographic = "E01A11BS";
            ie.SiteCodeID = _siteCode;
            ie.IsPOC = true;
            ie.Comments = "This is a student";
            ie.IsActive = true;
            ie.ModifiedBy = "SYSTEM";
            ie.TheaterLocationCode = "000000000";
            pe.Individual = ie;

            IdentificationNumbersEntity ine = new IdentificationNumbersEntity();
            ine.IDNumber = _name;
            ine.IDNumberType = "SSN";
            ine.IsActive = true;
            ine.ModifiedBy = "SYSTEM";
            pe.IDNumbers = new List<IdentificationNumbersEntity>();
            pe.IDNumbers.Add(ine);

            SrtsWeb.DataLayer.Repositories.IIndividualRepository ir = new SrtsWeb.DataLayer.Repositories.IndividualRepository();
            ds = ir.InsertIndividual(pe);

            int _id = (Int32)ds.Tables[2].Rows[0]["ID"];

            this.nameId.Add(_name, _id);

            if (_id > 0)
            {
                AddressEntity ae = new AddressEntity();
                ae.IndividualID = _id;
                ae.Address1 = "123 My Street";
                ae.Address2 = string.Empty;
                ae.Address3 = string.Empty;
                ae.City = "My City";
                ae.Country = "US";
                ae.State = "VA";
                ae.AddressType = "HOME";
                ae.IsActive = true;
                ae.ModifiedBy = "SYSTEM";
                ae.ZipCode = "555550000";
                ae.UIC = string.Empty;
                SrtsWeb.DataLayer.Repositories.IAddressRepository ar = new SrtsWeb.DataLayer.Repositories.AddressRepository();
                ar.InsertAddress(ae);

                PhoneNumberEntity pne = new PhoneNumberEntity();
                pne.IndividualID = _id;
                pne.AreaCode = "555";
                pne.Extension = string.Empty;
                pne.IsActive = true;
                pne.ModifiedBy = "SYSTEM";
                pne.PhoneNumber = "5555555";
                pne.PhoneNumberType = "UNIT";
                SrtsWeb.DataLayer.Repositories.IPhoneRepository pr = new SrtsWeb.DataLayer.Repositories.PhoneRepository();
                pr.InsertPhoneNumber(pne);

                EMailAddressEntity eae = new EMailAddressEntity();
                eae.IndividualID = _id;
                eae.EMailAddress = "personal@mail.mil";
                eae.EMailType = "PERSONAL";
                eae.IsActive = true;

                eae.ModifiedBy = "SYSTEM";
                SrtsWeb.DataLayer.Repositories.IEMailAddressRepository ear = new SrtsWeb.DataLayer.Repositories.EMailAddressRepository();
                ear.InsertEMailAddress(eae);
            }
        }
    }
}