using System;
using SrtsWeb.ExtendersHelpers;

namespace SrtsWeb.BusinessLayer.Concrete
{
    /// <summary>
    /// Custom class to perform trace operations inside of methods/functions/events.
    /// </summary>
    public class MethodTracer : IDisposable
    {
        private SrtsTraceSource _srtsTraceSource;
        private String _methodName;
        private String _userId;

        /// <summary>
        /// Performs a 'Verbose' trace from within a method, function, or event.  The trace executes at the beginning and end of the method.
        /// </summary>
        /// <param name="srtsTraceSource">Location in app where trace occurs.</param>
        /// <param name="methodName">Method name where trace occurs.</param>
        /// <param name="userId">User performing the operation being traced.</param>
        /// <returns>IDisposable</returns>
        public static IDisposable Trace(SrtsTraceSource srtsTraceSource, String methodName, String userId)
        {
            return new MethodTracer(srtsTraceSource, methodName, userId);
        }

        private MethodTracer(SrtsTraceSource srtsTraceSource, String methodName, String userId)
        {
            _srtsTraceSource = srtsTraceSource;
            _methodName = methodName;
            _userId = userId;

            // Trace entry
            CustomEventTracer.TraceVerbose(srtsTraceSource, methodName, userId, String.Format("Begin execution of {0}.", methodName));
        }

        /// <summary>
        /// Performs the end of execution trace when the dispose method is called.  This is done by either calling the dispose method directly or at the end of a using statement.
        /// </summary>
        public void Dispose()
        {
            // Trace exit
            CustomEventTracer.TraceVerbose(_srtsTraceSource, _methodName, _userId, String.Format("End execution of {0}.", _methodName));
        }
    }
}