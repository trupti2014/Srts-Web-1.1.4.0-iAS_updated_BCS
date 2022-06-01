using SrtsWeb.BusinessLayer.mil.osd.dmdc.sadr;
using System.Collections.Generic;

namespace SrtsWeb.BusinessLayer.Abstract
{
    public interface IDmdcService
    {
        IEnumerable<SrtsWeb.Entities.DmdcPerson> DoDmdcByDodId(string dodId);

        IEnumerable<SrtsWeb.Entities.DmdcPerson> DoDmdcByFsId(string fsId);

        IEnumerable<SrtsWeb.Entities.DmdcPerson> DoDmdcBySsn(string ssn);

        //RecordGeneratorWebService GetServiceObject();

        //IEnumerable<SrtsWeb.Entities.DmdcPerson> ProcessXml(System.Xml.XmlNodeList xList, System.Xml.XmlNamespaceManager xMgr);
    }
}