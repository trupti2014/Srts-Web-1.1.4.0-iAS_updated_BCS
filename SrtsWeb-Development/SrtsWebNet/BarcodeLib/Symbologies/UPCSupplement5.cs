using System;

namespace BarcodeLib.Symbologies
{
    internal sealed class UPCSupplement5 : BarcodeCommon, IBarcode
    {
        private string[] EAN_CodeA = { "0001101", "0011001", "0010011", "0111101", "0100011", "0110001", "0101111", "0111011", "0110111", "0001011" };
        private string[] EAN_CodeB = { "0100111", "0110011", "0011011", "0100001", "0011101", "0111001", "0000101", "0010001", "0001001", "0010111" };
        private string[] UPC_SUPP_5 = { "bbaaa", "babaa", "baaba", "baaab", "abbaa", "aabba", "aaabb", "ababa", "abaab", "aabab" };

        public UPCSupplement5(string input)
        {
            Raw_Data = input;
        }

        private string Encode_UPCSupplemental_5()
        {
            if (Raw_Data.Length != 5) Error("EUPC-SUP5-1: Invalid data length. (Length = 5 required)");

            if (!CheckNumericOnly(Raw_Data))
                Error("EUPCA-2: Numeric Data Only");

            int even = 0;
            int odd = 0;

            for (int i = 0; i <= 4; i += 2)
            {
                odd += Int32.Parse(Raw_Data.Substring(i, 1)) * 3;
            }

            for (int i = 1; i < 4; i += 2)
            {
                even += Int32.Parse(Raw_Data.Substring(i, 1)) * 9;
            }

            int total = even + odd;
            int cs = total % 10;

            string pattern = UPC_SUPP_5[cs];

            string result = "";

            int pos = 0;
            foreach (char c in pattern)
            {
                if (pos == 0) result += "1011";
                else result += "01";

                if (c == 'a')
                {
                    result += EAN_CodeA[Int32.Parse(Raw_Data[pos].ToString())];
                }
                else if (c == 'b')
                {
                    result += EAN_CodeB[Int32.Parse(Raw_Data[pos].ToString())];
                }

                pos++;
            }

            return result;
        }

        #region IBarcode Members

        public string Encoded_Value
        {
            get { return Encode_UPCSupplemental_5(); }
        }

        #endregion IBarcode Members
    }
}