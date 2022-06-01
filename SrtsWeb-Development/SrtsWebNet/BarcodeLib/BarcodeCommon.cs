using System;
using System.Collections.Generic;

namespace BarcodeLib
{
    internal abstract class BarcodeCommon
    {
        public String Raw_Data { get; set; }

        protected List<string> _Errors = new List<string>();

        public string RawData
        {
            get { return this.Raw_Data; }
        }

        public List<string> Errors
        {
            get { return this._Errors; }
        }

        public void Error(string ErrorMessage)
        {
            this._Errors.Add(ErrorMessage);
            throw new Exception(ErrorMessage);
        }

        internal static bool CheckNumericOnly(string Data)
        {
            long value = 0;
            if (Data != null)
            {
                if (Int64.TryParse(Data, out value))
                    return true;
            }
            else
            {
                return false;
            }

            int STRING_LENGTHS = 18;

            string temp = Data;
            string[] strings = new string[(Data.Length / STRING_LENGTHS) + ((Data.Length % STRING_LENGTHS == 0) ? 0 : 1)];

            int i = 0;
            while (i < strings.Length)
            {
                if (temp.Length >= STRING_LENGTHS)
                {
                    strings[i++] = temp.Substring(0, STRING_LENGTHS);
                    temp = temp.Substring(STRING_LENGTHS);
                }
                else
                    strings[i++] = temp.Substring(0);
            }

            foreach (string s in strings)
            {
                if (!Int64.TryParse(s, out value))
                    return false;
            }

            return true;
        }
    }
}