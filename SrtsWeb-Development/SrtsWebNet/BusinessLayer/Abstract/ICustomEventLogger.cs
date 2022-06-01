using SrtsWeb.ExtendersHelpers;
using System;

namespace SrtsWeb.BusinessLayer.Abstract
{
    public interface ICustomEventLogger
    {
        void LogEvent(string message);

        void LogEvent(string messageFormat, object[] formatParams);

        //void LogEvent(String message, SrtsLogPriority priority);
    }
}