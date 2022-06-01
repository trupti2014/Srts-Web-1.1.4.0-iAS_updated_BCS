using System;

namespace BarcodeLib.Symbologies
{
    internal sealed class UPCSupplement2 : BarcodeCommon, IBarcode
    {
        private string[] EAN_CodeA = { "0001101", "0011001", "0010011", "0111101", "0100011", "0110001", "0101111", "0111011", "0110111", "0001011" };
        private string[] EAN_CodeB = { "0100111", "0110011", "0011011", "0100001", "0011101", "0111001", "0000101", "0010001", "0001001", "0010111" };
        private string[] UPC_SUPP_2 = { "aa", "ab", "ba", "bb" };

        public UPCSupplement2(string input)
        {
            Raw_Data = input;
        }

        private string Encode_UPCSupplemental_2()
        {
            if (Raw_Data.Length != 2) Error("EUPC-SUP2-1: Invalid data length. (Length = 2 required)");

            if (!CheckNumericOnly(Raw_Data))
                Error("EUPC-SUP2-2: Numeric Data Only");

            string pattern = "";

            try
            {
                pattern = this.UPC_SUPP_2[Int32.Parse(Raw_Data.Trim()) % 4];
            }
            catch { Error("EUPC-SUP2-3: Invalid Data. (Numeric only)"); }

            string result = "1011";

            int pos = 0;
            foreach (char c in pattern)
            {
                if (c == 'a')
                {
                    result += EAN_CodeA[Int32.Parse(Raw_Data[pos].ToString())];
                }
                else if (c == 'b')
                {
                    result += EAN_CodeB[Int32.Parse(Raw_Data[pos].ToString())];
                }

                if (pos++ == 0) result += "01";
            }

            return result;
        }

        #region IBarcode Members

        public string Encoded_Value
        {
            get { return Encode_UPCSupplemental_2(); }
        }

        #endregion IBarcode Members
    }
}