using System;
using System.Collections.Generic;
using System.Text;

namespace SrtsWeb.BusinessLayer.TypeExtendersAndHelpers.Helpers
{
    public partial class SrtsHelper
    {
        public static string CheckString(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return string.Empty;
            }
            else
            {
                return str;
            }
        }

        public static string IntToString(int? sh)
        {
            if (string.IsNullOrEmpty(sh.ToString()))
            {
                return "0";
            }
            else
            {
                return sh.ToString();
            }
        }

        public static int StringToInt(string str)
        {
            int valOut = 0;
            return int.TryParse(str, out valOut) ? valOut : 0;
        }

        public static String GetRandomPwd(Int32 totalStrLength)
        {
            var newP = new StringBuilder();

            var alpha = new List<Char>() { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };
            var numeric = new List<Int32>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            var special = new List<Char> { '&', '!', '@', '#', '$', '%', '^', '*', '(', ')' };

            var all = new List<String>();
            all.AddRange(alpha.ConvertAll<String>(x => x.ToString()));
            all.AddRange(numeric.ConvertAll<String>(x => x.ToString()));
            all.AddRange(alpha.ConvertAll<String>(x => x.ToString().ToUpper()));
            all.AddRange(special.ConvertAll<String>(x => x.ToString()));

            Random r;

            r = new Random();
            newP.AppendFormat("{0}{1}", alpha[r.Next(alpha.Count)], alpha[r.Next(alpha.Count)]);
            newP.AppendFormat("{0}{1}", numeric[r.Next(numeric.Count)], numeric[r.Next(numeric.Count)]);
            newP.AppendFormat("{0}{1}", alpha[r.Next(alpha.Count)].ToString().ToUpper(), alpha[r.Next(alpha.Count)].ToString().ToUpper());
            newP.AppendFormat("{0}{1}", special[r.Next(special.Count)], special[r.Next(special.Count)]);

            if (totalStrLength < newP.Length) throw new Exception("Total string length input argument must be greater than or equal to 8.");
            var len = totalStrLength - newP.Length;
            for (int i = 0; i < len; i++)
                newP.Append(all[r.Next(all.Count)]);

            var randomizableList = new SortedDictionary<Guid, String>();
            foreach (var c in newP.ToString())
                randomizableList.Add(Guid.NewGuid(), c.ToString());
            newP.Clear();
            foreach (var a in randomizableList)
                newP.Append(a.Value);

            return newP.ToString();
        }
    }
}