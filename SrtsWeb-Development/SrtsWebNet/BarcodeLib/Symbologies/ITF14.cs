using System;

namespace BarcodeLib.Symbologies
{
    internal sealed class ITF14 : BarcodeCommon, IBarcode
    {
        private string[] ITF14_Code = { "NNWWN", "WNNNW", "NWNNW", "WWNNN", "NNWNW", "WNWNN", "NWWNN", "NNNWW", "WNNWN", "NWNWN" };

        public ITF14(string input)
        {
            Raw_Data = input;

            CheckDigit();
        }

        private string Encode_ITF14()
        {
            if (Raw_Data.Length > 14 || Raw_Data.Length < 13)
                Error("EITF14-1: Data length invalid. (Length must be 13 or 14)");

            if (!CheckNumericOnly(Raw_Data))
                Error("EITF14-2: Numeric data only.");

            string result = "1010";

            for (int i = 0; i < Raw_Data.Length; i += 2)
            {
                bool bars = true;
                string patternbars = ITF14_Code[Int32.Parse(Raw_Data[i].ToString())];
                string patternspaces = ITF14_Code[Int32.Parse(Raw_Data[i + 1].ToString())];
                string patternmixed = "";

                while (patternbars.Length > 0)
                {
                    patternmixed += patternbars[0].ToString() + patternspaces[0].ToString();
                    patternbars = patternbars.Substring(1);
                    patternspaces = patternspaces.Substring(1);
                }

                foreach (char c1 in patternmixed)
                {
                    if (bars)
                    {
                        if (c1 == 'N')
                            result += "1";
                        else
                            result += "11";
                    }
                    else
                    {
                        if (c1 == 'N')
                            result += "0";
                        else
                            result += "00";
                    }

                    bars = !bars;
                }
            }

            result += "1101";
            return result;
        }

        private void CheckDigit()
        {
            if (Raw_Data.Length == 13)
            {
                int total = 0;

                for (int i = 0; i <= Raw_Data.Length - 1; i++)
                {
                    int temp = Int32.Parse(Raw_Data.Substring(i, 1));
                    total += temp * ((i == 0 || i % 2 == 0) ? 3 : 1);
                }

                int cs = total % 10;
                cs = 10 - cs;
                if (cs == 10)
                    cs = 0;

                this.Raw_Data += cs.ToString();
            }
        }

        #region IBarcode Members

        public string Encoded_Value
        {
            get { return this.Encode_ITF14(); }
        }

        #endregion IBarcode Members
    }
}