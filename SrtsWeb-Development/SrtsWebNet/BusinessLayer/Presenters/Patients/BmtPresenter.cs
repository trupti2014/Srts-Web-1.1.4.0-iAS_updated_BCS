using SrtsWeb.BusinessLayer.Abstract;
using SrtsWeb.Views.Patients;
using SrtsWeb.DataLayer.Repositories;
using SrtsWeb.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SrtsWeb.BusinessLayer.Presenters.Patients
{
    public class BmtPresenter
    {
        private IBmtView v;

        public BmtPresenter(IBmtView view)
        {
            this.v = view;
        }

        public List<BmtEntity> GetBmtData(String fileLoc)
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

        public void CreateBmtFile(IExcelExporter excel, List<String> colNames)
        {
            using (excel)
            {
                var l = new List<String>();
                excel.CreateFile(colNames);
            }
        }
    }
}