namespace BarcodeLib.Symbologies
{
    internal sealed class Codabar : BarcodeCommon, IBarcode
    {
        private System.Collections.Hashtable Codabar_Code = new System.Collections.Hashtable();

        public Codabar(string input)
        {
            Raw_Data = input;
        }

        private string Encode_Codabar()
        {
            if (Raw_Data.Length < 2) Error("ECODABAR-1: Data format invalid. (Invalid length)");

            switch (Raw_Data[0].ToString().ToUpper().Trim())
            {
                case "A": break;
                case "B": break;
                case "C": break;
                case "D": break;
                default: Error("ECODABAR-2: Data format invalid. (Invalid START character)");
                    break;
            }

            switch (Raw_Data[Raw_Data.Trim().Length - 1].ToString().ToUpper().Trim())
            {
                case "A": break;
                case "B": break;
                case "C": break;
                case "D": break;
                default: Error("ECODABAR-3: Data format invalid. (Invalid STOP character)");
                    break;
            }

            this.init_Codabar();

            string temp = Raw_Data;

            foreach (char c in Codabar_Code.Keys)
            {
                if (!CheckNumericOnly(c.ToString()))
                {
                    temp = temp.Replace(c, '1');
                }
            }

            if (!CheckNumericOnly(temp))
                Error("ECODABAR-4: Data contains invalid  characters.");

            string result = "";

            foreach (char c in Raw_Data)
            {
                result += Codabar_Code[c].ToString();
                result += "0";
            }

            result = result.Remove(result.Length - 1);

            this.Codabar_Code.Clear();

            Raw_Data = Raw_Data.Trim().Substring(1, RawData.Trim().Length - 2);

            return result;
        }

        private void init_Codabar()
        {
            Codabar_Code.Clear();
            Codabar_Code.Add('0', "101010011");
            Codabar_Code.Add('1', "101011001");
            Codabar_Code.Add('2', "101001011");
            Codabar_Code.Add('3', "110010101");
            Codabar_Code.Add('4', "101101001");
            Codabar_Code.Add('5', "110101001");
            Codabar_Code.Add('6', "100101011");
            Codabar_Code.Add('7', "100101101");
            Codabar_Code.Add('8', "100110101");
            Codabar_Code.Add('9', "110100101");
            Codabar_Code.Add('-', "101001101");
            Codabar_Code.Add('$', "101100101");
            Codabar_Code.Add(':', "1101011011");
            Codabar_Code.Add('/', "1101101011");
            Codabar_Code.Add('.', "1101101101");
            Codabar_Code.Add('+', "101100110011");
            Codabar_Code.Add('A', "1011001001");
            Codabar_Code.Add('B', "1010010011");
            Codabar_Code.Add('C', "1001001011");
            Codabar_Code.Add('D', "1010011001");
            Codabar_Code.Add('a', "1011001001");
            Codabar_Code.Add('b', "1010010011");
            Codabar_Code.Add('c', "1001001011");
            Codabar_Code.Add('d', "1010011001");
        }

        #region IBarcode Members

        public string Encoded_Value
        {
            get { return Encode_Codabar(); }
        }

        #endregion IBarcode Members
    }
}