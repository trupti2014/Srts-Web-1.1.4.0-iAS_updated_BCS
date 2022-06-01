using SrtsWeb.Entities;
using System.Collections.Generic;

namespace SrtsWeb.BusinessLayer.Abstract
{
    public interface IDemographicXMLHelper
    {
        List<BOSEntity> GetALLBOS();

        List<StatusEntity> GetStatusByBOS(string _bos);

        List<RankEntity> GetRanksByBOSAndStatus(string _bos, string _status);

        List<OrderPriorityEntity> GetOrderPrioritiesByBOSStatusAndRank(string _bos, string _status, string _rank);
    }
}