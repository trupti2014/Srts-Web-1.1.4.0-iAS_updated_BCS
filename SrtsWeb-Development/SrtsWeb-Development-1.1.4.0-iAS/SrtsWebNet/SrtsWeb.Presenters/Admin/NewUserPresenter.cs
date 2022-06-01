using SrtsWeb.DataLayer.Repositories;
using SrtsWeb.ExtendersHelpers;
using SrtsWeb.Views.Admin;
using System;
using System.Linq;

namespace SrtsWeb.Presenters.Admin
{
    public class NewUserPresenter
    {
        private INewUser v;

        public NewUserPresenter(INewUser view)
        {
            this.v = view;
        }

        public void GetIndividual(string idNumber, string modifiedBy)
        {
            if (String.IsNullOrEmpty(idNumber) || idNumber.Length < 9) { this.v.IndividualList = null; return; }
            var idType = idNumber.Length.Equals(10) ? "DIN" : "SSN";

            var r = new IndividualRepository();
            var ModifiedBy = string.IsNullOrEmpty(modifiedBy) ? Globals.ModifiedBy : modifiedBy;
            var l = r.GetIndividualByIDNumberIDNumberType(idNumber, idType, ModifiedBy);
            if (l.Count.Equals(0)) { this.v.IndividualList = null; return; }

            var itr = new IndividualTypeRepository();
            var canBeUser = itr.GetIndividualTypesByIndividualId(l[0].ID).Any(x => x.TypeDescription.ToLower().In(new[] { "technician", "provider", "other" }));

            if (canBeUser)
                this.v.IndividualList = l;
            else
                this.v.IndividualList = null;
        }

        public void GetAllSites()
        {
            var r = new SiteRepository.SiteCodeRepository();
            var scs = r.GetAllSites().Where(x => x.IsActive == true).Distinct().ToList();

            if (scs == null || scs.Count.Equals(0)) return;

            this.v.DestinationSiteCodes = scs;
        }

        public void SyncUserToIndividual(String userName, Int32 individualId, Boolean forRemove = false)
        {
            if (userName == null || individualId == default(Int32)) return;
            var r = new IndividualRepository();
            r.SyncUserToIndividual(userName, individualId, forRemove);
        }

        public Boolean GetUserDodId(out String DodId)
        {
            var r = new IdentificationNumbersRepository();
            var ModifiedBy = string.IsNullOrEmpty(this.v.mySession.ModifiedBy) ? Globals.ModifiedBy : this.v.mySession.ModifiedBy;
            var ids = r.GetIdentificationNumbersByIndividualID(this.v.IndividualList[0].ID, ModifiedBy);

            var d = ids.Where(x => x.IDNumberType == "DIN" && x.IsActive == true).FirstOrDefault();
            if (d.IsNull()) { DodId = String.Empty; return false; }
            DodId = d.IDNumber;
            return true;
        }
    }
}