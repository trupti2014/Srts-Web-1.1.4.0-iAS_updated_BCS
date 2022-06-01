using Elmah;
using System;

namespace SrtsWeb.CustomErrors
{
    public sealed class ExceptionUtility
    {
        public static void LogException(Exception ex, string source = "")
        {
            try
            {
                if (!String.IsNullOrEmpty(source))
                    ErrorSignal.FromCurrentContext().Raise(new Exception(source));

                ErrorSignal.FromCurrentContext().Raise(ex);
            }
            catch
            {
            }
        }
    }
}