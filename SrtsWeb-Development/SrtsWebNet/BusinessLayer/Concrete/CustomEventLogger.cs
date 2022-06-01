using SrtsWeb.BusinessLayer.Abstract;
using SrtsWeb.ExtendersHelpers;
using System;
using System.Configuration;
using System.Diagnostics;

namespace SrtsWeb.BusinessLayer.Concrete
{
    /// <summary>
    /// Custom class that implements EventLog to do event logging operations.
    /// </summary>
    public class CustomEventLogger : EventLog, ICustomEventLogger
    {
        /// <summary>
        /// Default Ctor that sets the source (SRTSWeb_App_Event), and the log name (Application) from the app settings.  If the source does not already exist then it is created.
        /// </summary>
        public CustomEventLogger()
            : base(ConfigurationManager.AppSettings["EventLogSourceName"])
        {
            Source = ConfigurationManager.AppSettings["EventLogSourceName"];
            var LogName = ConfigurationManager.AppSettings["EventLogName"];

            if (SourceExists(Source)) return;
            CreateEventSource(Source, LogName);
        }

        /// <summary>
        /// Event for adding a log entry.
        /// </summary>
        /// <param name="message">String</param>
        public void LogEvent(String message)
        {
            WriteEntry(Source, message, EventLogEntryType.SuccessAudit);
        }

        /// <summary>
        /// Event for adding a log entry.
        /// </summary>
        /// <param name="messageFormat">String - Format string used to generate the event message.</param>
        /// <param name="formatParams">Object array - parameter list for format string.</param>
        public void LogEvent(string messageFormat, object[] formatParams)
        {
            WriteEntry(Source, String.Format(messageFormat, formatParams), EventLogEntryType.SuccessAudit);
        }
    }
}