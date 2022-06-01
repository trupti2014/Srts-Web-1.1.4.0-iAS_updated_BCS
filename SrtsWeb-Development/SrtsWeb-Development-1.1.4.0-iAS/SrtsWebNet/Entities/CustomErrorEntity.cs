using System;

namespace SrtsWeb.Entities
{
    [Serializable]
    public sealed class CustomErrorEntity
    {
        public string InnerExceptionType { get; set; }

        public string InnerException { get; set; }

        public string InnerSource { get; set; }

        public string InnerStackTrace { get; set; }

        public string ExceptionType { get; set; }

        public string Exception { get; set; }

        public string Source { get; set; }

        public string StackTrace { get; set; }
    }
}