namespace BarcodeLib.Symbologies
{
    internal sealed class ISBN : BarcodeCommon, IBarcode
    {
        public ISBN(string input)
        {
            Raw_Data = input;
        }

        private string Encode_ISBN_Bookland()
        {
            if (!CheckNumericOnly(Raw_Data))
                Error("EBOOKLANDISBN-1: Numeric Data Only");

            string type = "UNKNOWN";
            if (Raw_Data.Length == 10 || Raw_Data.Length == 9)
            {
                if (Raw_Data.Length == 10) Raw_Data = Raw_Data.Remove(9, 1);
                Raw_Data = "978" + Raw_Data;
                type = "ISBN";
            }
            else if (Raw_Data.Length == 12 && Raw_Data.StartsWith("978"))
            {
                type = "BOOKLAND-NOCHECKDIGIT";
            }
            else if (Raw_Data.Length == 13 && Raw_Data.StartsWith("978"))
            {
                type = "BOOKLAND-CHECKDIGIT";
                Raw_Data = Raw_Data.Remove(12, 1);
            }

            if (type == "UNKNOWN") Error("EBOOKLANDISBN-2: Invalid input.  Must start with 978 and be length must be 9, 10, 12, 13 characters.");

            EAN13 ean13 = new EAN13(Raw_Data);
            return ean13.Encoded_Value;
        }

        #region IBarcode Members

        public string Encoded_Value
        {
            get { return Encode_ISBN_Bookland(); }
        }

        #endregion IBarcode Members
    }
}