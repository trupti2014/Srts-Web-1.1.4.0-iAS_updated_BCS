using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using SrtsWeb.BusinessLayer.Abstract;
using SrtsWeb.BusinessLayer.Concrete;
using SrtsWeb.Entities;

namespace SrtsWeb.Base
{
    public class MasterBase : MasterPage, ICustomEventLogger
    {
        public SRTSSession mySession
        {
            get
            {
                if (Session["SRTSSession"] == null)
                {
                    SRTSSession ss = new SRTSSession();
                    Session.Add("SRTSSession", ss);
                }
                return (SRTSSession)Session["SRTSSession"];
            }
            set
            {
                if (Session["SRTSSession"] == null)
                {
                    Session.Add("SRTSSession", value);
                }
                else
                {
                    Session["SRTSSession"] = value;
                }
            }
        }
        private void LoadLookupTable()
        {
            SrtsWeb.BusinessLayer.Abstract.ILookupService _service = new LookupService();
            Cache["SRTSLOOKUP"] = _service.GetAllLookups();
        }

        public List<LookupTableEntity> LookupCache
        {
            get
            {
                if (Cache["SRTSLOOKUP"] == null)
                {
                    LoadLookupTable();
                }
                return Cache["SRTSLOOKUP"] as List<LookupTableEntity>;
            }
            set { Cache["SRTSLOOKUP"] = value; }
        }

        public void LogEvent(string message)
        {
            var cel = new CustomEventLogger();
            cel.LogEvent(message);
        }

        public void LogEvent(string messageFormat, object[] formatParams)
        {
            var cel = new CustomEventLogger();
            cel.LogEvent(messageFormat, formatParams);
        }
    }
}