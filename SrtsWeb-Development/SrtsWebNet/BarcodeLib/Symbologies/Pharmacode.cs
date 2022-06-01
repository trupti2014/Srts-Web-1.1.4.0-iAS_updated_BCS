using System;

namespace BarcodeLib.Symbologies
{
    internal sealed class Pharmacode : BarcodeCommon, IBarcode
    {
        private string _thinBar = "1";
        private string _gap = "00";
        private string _thickBar = "111";

        public Pharmacode(string input)
        {
            Raw_Data = input;

            if (!CheckNumericOnly(Raw_Data))
            {
                Error("EPHARM-1: Data contains invalid  characters (non-numeric).");
            }
            else if (Raw_Data.Length > 6)
            {
                Error("EPHARM-2: Data too long (invalid data input length).");
            }
        }

        private string Encode_Pharmacode()
        {
            int num;

            if (!Int32.TryParse(Raw_Data, out num))
            {
                Error("EPHARM-3: Input is unparseable.");
            }
            else if (num < 3 || num > 131070)
            {
                Error("EPHARM-4: Data contains invalid  characters (invalid numeric range).");
            }

            int startIndex = 0;

            for (int index = 15; index >= 0; index--)
            {
                if (Math.Pow(2, index) < num / 2)
                {
                    startIndex = index;
                    break;
                }
            }

            double sum = Math.Pow(2, startIndex + 1) - 2;
            string[] encoded = new string[startIndex + 1];
            int i = 0;

            for (int index = startIndex; index >= 0; index--)
            {
                double power = Math.Pow(2, index);
                double diff = num - sum;
                if (diff > power)
                {
                    encoded[i++] = _thickBar;
                    sum += power;
                }
                else
                {
                    encoded[i++] = _thinBar;
                }
            }

            string result = String.Empty;
            foreach (string s in encoded)
            {
                if (result != String.Empty)
                {
                    result += _gap;
                }

                result += s;
            }

            return result;
        }

        #region IBarcode Members

        public string Encoded_Value
        {
            get { return Encode_Pharmacode(); }
        }

        #endregion IBarcode Members
    }
}