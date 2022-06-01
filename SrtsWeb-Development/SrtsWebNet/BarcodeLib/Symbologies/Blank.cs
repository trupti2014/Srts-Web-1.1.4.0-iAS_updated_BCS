using System;

namespace BarcodeLib.Symbologies
{
    internal sealed class Blank : BarcodeCommon, IBarcode
    {
        #region IBarcode Members

        public string Encoded_Value
        {
            get { throw new NotImplementedException(); }
        }

        #endregion IBarcode Members
    }
}