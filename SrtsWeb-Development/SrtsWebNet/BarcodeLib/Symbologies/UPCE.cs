using System;

namespace BarcodeLib.Symbologies
{
    internal sealed class UPCE : BarcodeCommon, IBarcode
    {
        private string[] EAN_CodeA = { "0001101", "0011001", "0010011", "0111101", "0100011", "0110001", "0101111", "0111011", "0110111", "0001011" };
        private string[] EAN_CodeB = { "0100111", "0110011", "0011011", "0100001", "0011101", "0111001", "0000101", "0010001", "0001001", "0010111" };
        private string[] UPCE_Code_0 = { "bbbaaa", "bbabaa", "bbaaba", "bbaaab", "babbaa", "baabba", "baaabb", "bababa", "babaab", "baabab" };
        private string[] UPCE_Code_1 = { "aaabbb", "aababb", "aabbab", "aabbba", "abaabb", "abbaab", "abbbaa", "ababab", "ababba", "abbaba" };

        public UPCE(string input)
        {
            Raw_Data = input;
        }

        private string Encode_UPCE()
        {
            if (Raw_Data.Length != 6 && Raw_Data.Length != 8 && Raw_Data.Length != 12) Error("EUPCE-1: Invalid data length. (8 or 12 numbers only)");

            if (!CheckNumericOnly(Raw_Data)) Error("EUPCE-2: Numeric only.");

            int CheckDigit = Int32.Parse(Raw_Data[Raw_Data.Length - 1].ToString());
            int NumberSystem = Int32.Parse(Raw_Data[0].ToString());

            if (Raw_Data.Length == 12)
            {
                string UPCECode = "";

                string Manufacturer = Raw_Data.Substring(1, 5);
                string ProductCode = Raw_Data.Substring(6, 5);

                if (NumberSystem != 0 && NumberSystem != 1)
                    Error("EUPCE-3: Invalid Number System (only 0 & 1 are valid)");

                if (Manufacturer.EndsWith("000") || Manufacturer.EndsWith("100") || Manufacturer.EndsWith("200") && Int32.Parse(ProductCode) <= 999)
                {
                    UPCECode += Manufacturer.Substring(0, 2);

                    UPCECode += ProductCode.Substring(2, 3);

                    UPCECode += Manufacturer[2].ToString();
                }
                else if (Manufacturer.EndsWith("00") && Int32.Parse(ProductCode) <= 99)
                {
                    UPCECode += Manufacturer.Substring(0, 3);

                    UPCECode += ProductCode.Substring(3, 2);

                    UPCECode += "3";
                }
                else if (Manufacturer.EndsWith("0") && Int32.Parse(ProductCode) <= 9)
                {
                    UPCECode += Manufacturer.Substring(0, 4);

                    UPCECode += ProductCode[4];

                    UPCECode += "4";
                }
                else if (!Manufacturer.EndsWith("0") && Int32.Parse(ProductCode) <= 9 && Int32.Parse(ProductCode) >= 5)
                {
                    UPCECode += Manufacturer;

                    UPCECode += ProductCode[4];
                }
                else
                    Error("EUPCE-4: Illegal UPC-A entered for conversion.  Unable to convert.");

                Raw_Data = UPCECode;
            }

            string pattern = "";

            if (NumberSystem == 0) pattern = UPCE_Code_0[CheckDigit];
            else pattern = UPCE_Code_1[CheckDigit];

            string result = "101";

            int pos = 0;
            foreach (char c in pattern)
            {
                int i = Int32.Parse(Raw_Data[pos++].ToString());
                if (c == 'a')
                {
                    result += EAN_CodeA[i];
                }
                else if (c == 'b')
                {
                    result += EAN_CodeB[i];
                }
            }

            result += "01010";

            result += "1";

            return result;
        }

        #region IBarcode Members

        public string Encoded_Value
        {
            get { return Encode_UPCE(); }
        }

        #endregion IBarcode Members
    }
}