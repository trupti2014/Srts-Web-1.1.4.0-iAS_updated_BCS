using SrtsWeb.BusinessLayer.Abstract;
using SrtsWeb.BusinessLayer.Concrete;
using SrtsWeb.DataLayer.Repositories;
using SrtsWeb.Entities;
using SrtsWeb.Views.Patients;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SrtsWeb.Presenters.Patients
{
    public class BmtPresenter
    {
        private IBmtView v;

        public BmtPresenter(IBmtView view)
        {
            this.v = view;
        }

        public IEnumerable<BmtEntity> GetBmtData(String fileLoc)
        {
            var r = new ExcelRepository("BMT", fileLoc);

            var lt = r.GetBmtFileData();

            return lt;
        }

        public Int32 GetPatientTypeId()
        {
            var r = new LookupRepository();
            var ts = r.GetLookupsByType("IndividualType");
            if (ts == null || ts.Count.Equals(0)) return default(Int32);
            return ts.FirstOrDefault(x => x.Value == "PATIENT").Id;
        }

        public IEnumerable<BmtEntity> ProcessTrainees(IEnumerable<BmtEntity> traineeList, out Int32 traineesAdded)
        {
            IBmtService bSvc = new BmtService();
            var good = default(Int32);
            IEnumerable<BmtEntity> bad = bSvc.UpdateBmt(traineeList, this.v.mySession.MySite.SiteCode, out good);
            traineesAdded = good;
            return bad;
        }
    }
}