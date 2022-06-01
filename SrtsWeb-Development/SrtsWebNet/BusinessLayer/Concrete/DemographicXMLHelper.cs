using SrtsWeb.BusinessLayer.Abstract;
using SrtsWeb.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace SrtsWeb.BusinessLayer.Concrete
{
    /// <summary>
    /// Custom class to perform operations against the Demographic.xml file.
    /// </summary>
    public class DemographicXMLHelper : IDemographicXMLHelper
    {
        private XElement xdoc;

        /// <summary>
        /// Default Ctor
        /// </summary>
        public DemographicXMLHelper()
        {
            xdoc = DemographicXml.Instance.XmlDocument;
        }

        /// <summary>
        /// Gets a list of all branches of service.
        /// </summary>
        /// <returns>BOSEntity List</returns>
        public List<BOSEntity> GetALLBOS()
        {
            List<BOSEntity> lInfo = xdoc.Elements("BOS").
                Select(l => new BOSEntity()
                {
                    BOSText = l.Attribute("bosText").Value,
                    BOSValue = l.Attribute("bosValue").Value,
                    BOSCombo = l.Attribute("bosCombo").Value
                }).ToList();

            return lInfo;
        }

        /// <summary>
        /// Gets a list of all statuses by branch of service.
        /// </summary>
        /// <param name="_bos">Branch of servce used for comparison.</param>
        /// <returns>StatusEntity List</returns>
        public List<StatusEntity> GetStatusByBOS(string _bos)
        {
            List<StatusEntity> tmpStatus = xdoc.Elements("BOS").Where(e => e.Attribute("bosValue").Value == _bos).
                Elements("Status").
                Select(l => new StatusEntity()
                {
                    StatusText = l.Attribute("statusText").Value,
                    StatusValue = l.Attribute("statusValue").Value
                }).ToList();

            return tmpStatus;
        }

        /// <summary>
        /// Gets a list of all ranks by branch of service and status.
        /// </summary>
        /// <param name="_bos">Branch of servce used for comparison.</param>
        /// <param name="_status">Status user for comparison.</param>
        /// <returns>RankEntity List</returns>
        public List<RankEntity> GetRanksByBOSAndStatus(string _bos, string _status)
        {
            List<RankEntity> tmpRanks = xdoc.Elements("BOS").Where(e => e.Attribute("bosValue").Value == _bos).
                Elements("Status").Where(a => a.Attribute("statusValue").Value == _status).
                Elements("Rank").
                Select(l => new RankEntity()
                {
                    RankText = l.Attribute("rankText").Value,
                    RankValue = l.Attribute("rankValue").Value
                }).ToList();

            return tmpRanks;
        }

        /// <summary>
        /// Gets a list of all order priorities by branch of service, status, and rank.
        /// </summary>
        /// <param name="_bos">Branch of servce used for comparison.</param>
        /// <param name="_status">Status user for comparison.</param>
        /// <param name="_rank">Rank user for comparison.</param>
        /// <returns>OrderPriorityEntity List</returns>
        public List<OrderPriorityEntity> GetOrderPrioritiesByBOSStatusAndRank(string _bos, string _status, string _rank)
        {
            var tmpPriorities = xdoc.Elements("BOS").Where(e => e.Attribute("bosValue").Value == _bos).
                Elements("Status").Where(a => a.Attribute("statusValue").Value == _status).
                Elements("Rank").Where(r => r.Attribute("rankValue").Value == _rank).
                Elements("Priority").
                Select(l => new OrderPriorityEntity()
                {
                    OrderPriorityText = l.Attribute("priorityText").Value,
                    OrderPriorityValue = l.Attribute("priorityValue").Value
                }).ToList();

            return tmpPriorities;
        }

        /// <summary>
        /// Gets a list of all priorities
        /// </summary>
        /// <returns>System.Collections.Generic.List of OrderPriorityEntity - List of all order priorities</returns>
        public List<OrderPriorityEntity> GetAllPriorities()
        {
            var p = xdoc.Elements("Priority").
                Select(a => new
                {
                    pText = a.Attribute("priorityText").Value,
                    pValue = a.Attribute("priorityValue").Value
                }).Distinct()
                .Select(x => new OrderPriorityEntity()
                {
                    OrderPriorityText = x.pText,
                    OrderPriorityValue = x.pValue
                }).ToList();
            return p;
        }
    }

    /// <summary>
    /// This is a singleton class to give access to the DemographicXML file to the above helper class
    /// </summary>
    internal sealed class DemographicXml
    {
        public XElement XmlDocument { get; set; }

        private static DemographicXml _instance;

        public static DemographicXml Instance 
        { 
            get 
            { 
                if (_instance == null) 
                    _instance = new DemographicXml(); 
                return _instance; 
            } 
        }

        private DemographicXml()
        {
            string DemoPath = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + "DemographicXML.xml");
            this.XmlDocument = XElement.Load(DemoPath);
        }
    }
}