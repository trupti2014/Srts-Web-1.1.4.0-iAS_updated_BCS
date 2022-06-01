using SrtsWeb.ExtendersHelpers;
using System;
using System.Collections;
using System.Configuration;

namespace SrtsWeb.CustomElmah
{
    /// <summary>
    /// Custom implementation of the Elmah.SqlErrorLog class.
    /// </summary>
    public class SrtsElmah : Elmah.SqlErrorLog
    {
        /// <summary>
        /// Overridden ConnectionString property to return a SRTS connection string to ELMAH.
        /// </summary>
        public override string ConnectionString
        {
            get
            {
                return ConfigurationManager.ConnectionStrings[Globals.ConnStrNm].ConnectionString;
            }
        }

        /// <summary>
        /// Default Ctor inherited from Elmah.SqlErrorLog
        /// </summary>
        /// <param name="connectionString">System.Collections.IDictionary</param>
        public SrtsElmah(IDictionary config)
            : base(config)
        {
        }

        /// <summary>
        /// Default Ctor inherited from Elmah.SqlErrorLog
        /// </summary>
        /// <param name="connectionString">String</param>
        public SrtsElmah(String connectionString)
            : base(connectionString)
        {
        }
    }
}