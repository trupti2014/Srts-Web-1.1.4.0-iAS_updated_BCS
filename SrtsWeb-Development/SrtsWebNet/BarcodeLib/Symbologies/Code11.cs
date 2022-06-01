using System;

namespace BarcodeLib.Symbologies
{
    internal sealed class Code11 : BarcodeCommon, IBarcode
    {
        private string[] C11_Code = { "101011", "1101011", "1001011", "1100101", "1011011", "1101101", "1001101", "1010011", "1101001", "110101", "101101", "1011001" };

        public Code11(string input)
        {
            Raw_Data = input;
        }

        private string Encode_Code11()
        {
            if (!CheckNumericOnly(Raw_Data.Replace("-", "")))
                Error("EC11-1: Numeric data and '-' Only");

            int weight = 1;
            int CTotal = 0;
            string Data_To_Encode_with_Checksums = Raw_Data;

            for (int i = Raw_Data.Length - 1; i >= 0; i--)
            {
                if (weight == 10) weight = 1;

                if (Raw_Data[i] != '-')
                    CTotal += Int32.Parse(Raw_Data[i].ToString()) * weight++;
                else
                    CTotal += 10 * weight++;
            }
            int checksumC = CTotal % 11;

            Data_To_Encode_with_Checksums += checksumC.ToString();

            if (Raw_Data.Length >= 1)
            {
                weight = 1;
                int KTotal = 0;

                for (int i = Data_To_Encode_with_Checksums.Length - 1; i >= 0; i--)
                {
                    if (weight == 9) weight = 1;

                    if (Data_To_Encode_with_Checksums[i] != '-')
                        KTotal += Int32.Parse(Data_To_Encode_with_Checksums[i].ToString()) * weight++;
                    else
                        KTotal += 10 * weight++;
                }
                int checksumK = KTotal % 11;
                Data_To_Encode_with_Checksums += checksumK.ToString();
            }

            string space = "0";
            string result = C11_Code[11] + space;

            foreach (char c in Data_To_Encode_with_Checksums)
            {
                int index = (c == '-' ? 10 : Int32.Parse(c.ToString()));
                result += C11_Code[index];

                result += space;
            }

            result += C11_Code[11];

            return result;
        }

        #region IBarcode Members

        public string Encoded_Value
        {
            get { return Encode_Code11(); }
        }

        #endregion IBarcode Members
    }
}