using System;

namespace BarcodeLib.Symbologies
{
    internal sealed class Postnet : BarcodeCommon, IBarcode
    {
        private string[] POSTNET_Code = { "11000", "00011", "00101", "00110", "01001", "01010", "01100", "10001", "10010", "10100" };

        public Postnet(string input)
        {
            Raw_Data = input;
        }

        private string Encode_Postnet()
        {
            Raw_Data = Raw_Data.Replace("-", "");

            switch (Raw_Data.Length)
            {
                case 5:
                case 6:
                case 9:
                case 11: break;
                default: Error("EPOSTNET-2: Invalid data length. (5, 6, 9, or 11 digits only)");
                    break;
            }

            string result = "1";
            int checkdigitsum = 0;

            foreach (char c in Raw_Data)
            {
                try
                {
                    int index = Convert.ToInt32(c.ToString());
                    result += POSTNET_Code[index];
                    checkdigitsum += index;
                }
                catch (Exception ex)
                {
                    Error("EPOSTNET-2: Invalid data. (Numeric only) --> " + ex.Message);
                }
            }

            int temp = checkdigitsum % 10;
            int checkdigit = 10 - (temp == 0 ? 10 : temp);

            result += POSTNET_Code[checkdigit];

            result += "1";

            return result;
        }

        #region IBarcode Members

        public string Encoded_Value
        {
            get { return Encode_Postnet(); }
        }

        #endregion IBarcode Members
    }
}