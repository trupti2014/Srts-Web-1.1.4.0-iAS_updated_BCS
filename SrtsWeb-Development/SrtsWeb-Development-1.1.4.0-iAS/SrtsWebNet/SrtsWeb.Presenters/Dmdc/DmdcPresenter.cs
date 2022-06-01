using SrtsWeb.BusinessLayer.Abstract;
using SrtsWeb.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SrtsWeb.Presenters.Dmdc
{
    public class DmdcPresenter
    {
        private IDmdcService service;

        public DmdcPresenter(IDmdcService service)
        {
            this.service = service;
        }

        public IEnumerable<DmdcPerson> Get(IEnumerable<String> idListIn)
        {
            var ld = new List<DmdcPerson>();
            idListIn.ToList().ForEach(x => ld.Add(Get(x)));
            return ld;
        }

        public DmdcPerson Get(String idIn)
        {
            if (String.IsNullOrEmpty(idIn)) return new DmdcPerson();

            if (idIn.Length.Equals(10))
                return this.service.DoDmdcByDodId(idIn).FirstOrDefault();
            else if (idIn.Length.Equals(9))
            {
                if (idIn.StartsWith("9"))
                    return this.service.DoDmdcByFsId(idIn).FirstOrDefault();
                else
                    return this.service.DoDmdcBySsn(idIn).FirstOrDefault();
            }

            return new DmdcPerson();
        }
    }
}