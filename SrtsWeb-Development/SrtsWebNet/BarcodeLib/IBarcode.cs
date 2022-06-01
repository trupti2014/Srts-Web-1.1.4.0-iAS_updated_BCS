using System.Collections.Generic;

namespace BarcodeLib
{
    internal interface IBarcode
    {
        string Encoded_Value
        {
            get;
        }

        string RawData
        {
            get;
        }

        List<string> Errors
        {
            get;
        }
    }
}