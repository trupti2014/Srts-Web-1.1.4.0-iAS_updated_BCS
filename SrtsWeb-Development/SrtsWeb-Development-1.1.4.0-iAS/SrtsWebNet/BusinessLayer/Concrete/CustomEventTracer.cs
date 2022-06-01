using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SrtsWeb.ExtendersHelpers;

namespace SrtsWeb.BusinessLayer.Concrete
{
    /// <summary>
    /// Custom class used to perform trace operations.
    /// </summary>
    public sealed class CustomEventTracer
    {
        private String _UserId { get; set; }
        private String _Source { get; set; }
        private TraceSource _TraceSource;
        private String DEFAULT_MESSAGE { get { return String.Format("User/Modifier: {0}{1}Date: {2}{3}", this._UserId, NewLine, DateTime.Now, NewLine); } }
        private String NewLine { get { return Environment.NewLine; } }

        /// <summary>
        /// Default Ctor to set class parameters: source, userId, and SrtsTraceSource.
        /// </summary>
        /// <param name="source">Identifies the mothod or event that the trace is generated from.</param>
        /// <param name="userId">Identifies the individual adding the trace.</param>
        /// <param name="srtsTraceSource">SrtsWeb.ExtendersHelpers.SrtsTraceSource - Indentifies the location within the application that generated the trace.</param>
        public CustomEventTracer(String source, String userId, SrtsTraceSource srtsTraceSource)
        {
            this._UserId = userId;
            this._Source = source;
            this._TraceSource = new TraceSource(srtsTraceSource.ToString());
        }

        /// <summary>
        /// Method to write a 'Error' message to a trace log.
        /// </summary>
        /// <param name="message">Message to be written to the trace.</param>
        public void TraceError(String message)
        {
            if (!_TraceSource.Switch.ShouldTrace(TraceEventType.Error)) return;
            if (String.IsNullOrEmpty(message)) { LogNullOrEmptyMessage(); return; }
            _TraceSource.TraceData(TraceEventType.Error, 1, String.Format("{0}Message: {1}{2}Trace Description: {3}", new Object[] { DEFAULT_MESSAGE, message, NewLine, String.Format("{0}_Error", _Source) }));
        }

        /// <summary>
        /// Method to write a 'Warning' message to a trace log.
        /// </summary>
        /// <param name="message">Message to be written to the trace.</param>
        public void TraceWarning(String message)
        {
            if (!_TraceSource.Switch.ShouldTrace(TraceEventType.Warning)) return;
            if (String.IsNullOrEmpty(message)) { LogNullOrEmptyMessage(); return; }
            _TraceSource.TraceData(TraceEventType.Warning, 1, String.Format("{0}Message: {1}{2}Trace Description: {3}", new Object[] { DEFAULT_MESSAGE, message, NewLine, String.Format("{0}_Warning", _Source) }));
        }

        /// <summary>
        /// Method to write a 'Information' message to a trace log.
        /// </summary>
        /// <param name="message">Message to be written to the trace.</param>
        public void TraceInfo(String message)
        {
            if (!_TraceSource.Switch.ShouldTrace(TraceEventType.Information)) return;
            if (String.IsNullOrEmpty(message)) { LogNullOrEmptyMessage(); return; }
            _TraceSource.TraceData(TraceEventType.Information, 1, String.Format("{0}Message: {1}{2}Trace Description: {3}", new Object[] { DEFAULT_MESSAGE, message, NewLine, String.Format("{0}_Info", _Source) }));
        }

        /// <summary>
        /// Method to write a 'Verbose' message to a trace log.
        /// </summary>
        /// <param name="message">Message to be written to the trace.</param>
        public void TraceVerbose(String message)
        {
            if (!_TraceSource.Switch.ShouldTrace(TraceEventType.Verbose)) return;
            if (String.IsNullOrEmpty(message)) { LogNullOrEmptyMessage(); return; }
            _TraceSource.TraceData(TraceEventType.Verbose, 1, String.Format("{0}Message: {1}{2}Trace Description: {3}", new Object[] { DEFAULT_MESSAGE, message, NewLine, String.Format("{0}_Verbose", _Source) }));
        }


        /// <summary>
        /// Method to write a 'Error' message to a trace log.
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="messageParams">The object list to format.</param>
        public void TraceError(String format, Object[] messageParams)
        {
            if (!_TraceSource.Switch.ShouldTrace(TraceEventType.Error)) return;
            if (String.IsNullOrEmpty(format)) { LogNullOrEmptyMessage(); return; }
            var m = String.Format(format, messageParams);
            _TraceSource.TraceData(TraceEventType.Error, 1, String.Format("{0}Message: {1}{2}Trace Description: {3}", new Object[] { DEFAULT_MESSAGE, m, NewLine, String.Format("{0}_Error", _TraceSource.Switch.Description) }));
        }

        /// <summary>
        /// Method to write a 'Warning' message to a trace log.
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="messageParams">The object list to format.</param>
        public void TraceWarning(String format, Object[] messageParams)
        {
            if (!_TraceSource.Switch.ShouldTrace(TraceEventType.Warning)) return;
            if (String.IsNullOrEmpty(format)) { LogNullOrEmptyMessage(); return; }
            var m = String.Format(format, messageParams);
            _TraceSource.TraceData(TraceEventType.Warning, 1, String.Format("{0}Message: {1}{2}Trace Description: {3}", new Object[] { DEFAULT_MESSAGE, m, NewLine, String.Format("{0}_Warning", _TraceSource.Switch.Description) }));
        }

        /// <summary>
        /// Method to write a 'Information' message to a trace log.
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="messageParams">The object list to format.</param>
        public void TraceInfo(String format, Object[] messageParams)
        {
            if (!_TraceSource.Switch.ShouldTrace(TraceEventType.Information)) return;
            if (String.IsNullOrEmpty(format)) { LogNullOrEmptyMessage(); return; }
            var m = String.Format(format, messageParams);
            _TraceSource.TraceData(TraceEventType.Information, 1, String.Format("{0}Message: {1}{2}Trace Description: {3}", new Object[] { DEFAULT_MESSAGE, m, NewLine, String.Format("{0}_Info", _TraceSource.Switch.Description) }));
        }

        /// <summary>
        /// Method to write a 'Verbose' message to a trace log.
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="messageParams">The object list to format.</param>
        public void TraceVerbose(String format, Object[] messageParams)
        {
            if (!_TraceSource.Switch.ShouldTrace(TraceEventType.Verbose)) return;
            if (String.IsNullOrEmpty(format)) { LogNullOrEmptyMessage(); return; }
            var m = String.Format(format, messageParams);
            _TraceSource.TraceData(TraceEventType.Verbose, 1, String.Format("{0}Message: {1}{2}Trace Description: {3}", DEFAULT_MESSAGE, m, NewLine, String.Format("{0}_Verbose", _TraceSource.Switch.Description)));
        }

        private void LogNullOrEmptyMessage()
        {
            if (!_TraceSource.Switch.ShouldTrace(TraceEventType.Error)) return;
            _TraceSource.TraceData(TraceEventType.Error, 1, "{0}{1}", new Object[] { DEFAULT_MESSAGE, "Cannot write an empty message to the trace log." });
        }

        #region STATIC MEMBERS/METHODS
        private static String _DEFAULT_MESSAGE { get { return "User/Modifier: {0}{1}Date: {2}{3}"; } }
        private static String NL { get { return Environment.NewLine; } }

        /// <summary>
        /// Static method to write a 'Error' message to a trace log.
        /// </summary>
        /// <param name="srtsTraceSource">SrtsWeb.ExtendersHelpers.SrtsTraceSource - Indentifies the location within the application that generated the trace.</param>
        /// <param name="source">Identifies the mothod or event that the trace is generated from.</param>
        /// <param name="userId">Identifies the individual adding the trace.</param>
        /// <param name="message">Message to be written to the trace.</param>
        public static void TraceError(SrtsTraceSource srtsTraceSource, String source, String userId, String message)
        {
            var traceSource = new TraceSource(srtsTraceSource.ToString()); //, String.Format("Tracing_{0}_Error", srtsTraceSource));
            if (!traceSource.Switch.ShouldTrace(TraceEventType.Error)) return;
            if (String.IsNullOrEmpty(message)) { LogNullOrEmptyMessage(srtsTraceSource, source, userId); return; }

            traceSource.TraceData(TraceEventType.Error, 1, String.Format("{0}Message: {1}{2}TraceSource: {3}", new Object[] { String.Format(_DEFAULT_MESSAGE, userId, NL, DateTime.Now, NL), message, NL, source }));
        }

        /// <summary>
        /// Static method to write a 'Information' message to a trace log.
        /// </summary>
        /// <param name="srtsTraceSource">SrtsWeb.ExtendersHelpers.SrtsTraceSource - Indentifies the location within the application that generated the trace.</param>
        /// <param name="source">Identifies the mothod or event that the trace is generated from.</param>
        /// <param name="userId">Identifies the individual adding the trace.</param>
        /// <param name="message">Message to be written to the trace.</param>
        public static void TraceInfo(SrtsTraceSource srtsTraceSource, String source, String userId, String message)
        {
            var traceSource = new TraceSource(srtsTraceSource.ToString()); //, String.Format("Tracing_{0}_Info", srtsTraceSource));
            if (!traceSource.Switch.ShouldTrace(TraceEventType.Information)) return;
            if (String.IsNullOrEmpty(message)) { LogNullOrEmptyMessage(srtsTraceSource, source, userId); return; }

            traceSource.TraceData(TraceEventType.Information, 1, String.Format("{0}Message: {1}{2}TraceSource: {3}", new Object[] { String.Format(_DEFAULT_MESSAGE, userId, NL, DateTime.Now, NL), message, NL, source }));
        }

        /// <summary>
        /// Static method to write a 'Warning' message to a trace log.
        /// </summary>
        /// <param name="srtsTraceSource">SrtsWeb.ExtendersHelpers.SrtsTraceSource - Indentifies the location within the application that generated the trace.</param>
        /// <param name="source">Identifies the mothod or event that the trace is generated from.</param>
        /// <param name="userId">Identifies the individual adding the trace.</param>
        /// <param name="message">Message to be written to the trace.</param>
        public static void TraceWarning(SrtsTraceSource srtsTraceSource, String source, String userId, String message)
        {
            var traceSource = new TraceSource(srtsTraceSource.ToString()); //, String.Format("Tracing_{0}_Warning", srtsTraceSource));
            if (!traceSource.Switch.ShouldTrace(TraceEventType.Warning)) return;
            if (String.IsNullOrEmpty(message)) { LogNullOrEmptyMessage(srtsTraceSource, source, userId); return; }

            traceSource.TraceData(TraceEventType.Verbose, 1, String.Format("{0}Message: {1}{2}TraceSource: {3}", new Object[] { String.Format(_DEFAULT_MESSAGE, userId, NL, DateTime.Now, NL), message, NL, source }));
        }

        /// <summary>
        /// Static method to write a 'Verbose' message to a trace log.
        /// </summary>
        /// <param name="srtsTraceSource">SrtsWeb.ExtendersHelpers.SrtsTraceSource - Indentifies the location within the application that generated the trace.</param>
        /// <param name="source">Identifies the mothod or event that the trace is generated from.</param>
        /// <param name="userId">Identifies the individual adding the trace.</param>
        /// <param name="message">Message to be written to the trace.</param>
        public static void TraceVerbose(SrtsTraceSource srtsTraceSource, String source, String userId, String message)
        {
            var traceSource = new TraceSource(srtsTraceSource.ToString()); //, String.Format("Tracing_{0}_Verbose", srtsTraceSource));
            if (!traceSource.Switch.ShouldTrace(TraceEventType.Verbose)) return;
            if (String.IsNullOrEmpty(message)) { LogNullOrEmptyMessage(srtsTraceSource, source, userId); return; }

            traceSource.TraceData(TraceEventType.Verbose, 1, String.Format("{0}Message: {1}{2}TraceSource: {3}", String.Format(_DEFAULT_MESSAGE, userId, NL, DateTime.Now, NL), message, NL, source));
        }

        private static void LogNullOrEmptyMessage(SrtsTraceSource srtsTraceSource, String source, String userId)
        {
            var traceSource = new TraceSource(srtsTraceSource.ToString()); //, String.Format("Tracing_{0}_Error", srtsTraceSource));
            if (!traceSource.Switch.ShouldTrace(TraceEventType.Error)) return;
            
            Trace.TraceError("{0}Message: {1}{2}TraceSource: {3}", new Object[] { String.Format(_DEFAULT_MESSAGE, userId, NL, DateTime.Now, NL), "Cannot write an empty message to the trace log.", NL, source });
        }
        #endregion
    }
}
