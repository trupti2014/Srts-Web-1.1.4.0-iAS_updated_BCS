using Elmah;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SrtsWeb.BusinessLayer.Concrete
{
    /// <summary>
    /// -----------------------------------------------------------------
    /// NEED TO DEPRECATE THIS AND REPLACE WITH CUSTOM EVENT LOGGER CLASS
    /// -----------------------------------------------------------------
    /// Custom class used to add a log to the ELMAH log
    /// </summary>
    public class CustomLogger
    {
        private List<String> _LogTriggers { get; set; }

        /// <summary>
        /// Default Ctor that sets the parameter: LogTriggers
        /// </summary>
        /// <param name="LogTriggers"></param>
        public CustomLogger(List<String> LogTriggers) { this._LogTriggers = LogTriggers; }

        public void Log(List<String> logTriggers, String logMessage)
        {
            // If no log trigger(s) are sent to the log method then leave.
            if (!this._LogTriggers.Any(x => logTriggers.Contains(x))) return;
            if (String.IsNullOrEmpty(logMessage)) return;
            Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception(logMessage));
        }

        public static void Log(List<String> methodTriggers, List<String> logTriggers, String logMessage)
        {
            // If no log trigger(s) are sent to the log method then leave.
            if (!methodTriggers.Any(x => logTriggers.Contains(x))) return;
            if (String.IsNullOrEmpty(logMessage)) return;
            Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception(logMessage));
        }
        public static void Log(String methodTrigger, List<String> logTriggers, String logMessage)
        {
            // If no log trigger(s) are sent to the log method then leave.
            if (!logTriggers.Contains(methodTrigger)) return;
            if (String.IsNullOrEmpty(logMessage)) return;
            Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception(logMessage));
        }

        public static List<String> GetLogTriggers(String triggerToFind)
        {
            var ttf = String.Format("{0}:", triggerToFind);
            var l = ConfigurationManager.AppSettings.AllKeys
                .Where(key => key.ToLower().Contains(triggerToFind.ToLower()) && ConfigurationManager.AppSettings[key] == "1")
                .Select(s => s.Substring(s.IndexOf(':') + 1)).ToList();

            var al = new List<String>();
            l.ForEach(x => al.AddRange(x.Split(new[] { ',' })));
            return al;
        }
    }
}
