using System;
using System.Collections.Generic;

namespace SrtsWeb.BusinessLayer.TypeExtendersAndHelpers.Helpers
{
    public static partial class SrtsHelper
    {
        public static decimal SphereToDecimal(string sphere)
        {
            decimal valOut = 0;
            return decimal.TryParse(sphere, out valOut) ? valOut : 0;
        }

        public static string SphereToString(decimal? sphere)
        {
            string tmpStr = string.Empty;
            if (string.IsNullOrEmpty(sphere.ToString()))
            {
                return tmpStr;
            }
            if (sphere == 0)
            {
                return "PLANO";
            }
            else
            {
                tmpStr = sphere.ToString();
            }
            return tmpStr;
        }

        public static List<String> SphereValues()
        {
            double min = -20.00;
            double max = 20.00;
            double step = 0.25;
            double posStepCnt = max / step;
            double negStepCnt = (Math.Abs(min) / step);
            double number = 0;
            string formattedNumber = number.ToString("N2");
            List<String> values = new List<String>();
            values.Add(formattedNumber);

            number = step * -1;
            for (int i = 0; i < Convert.ToInt16(negStepCnt); i++)
            {
                formattedNumber = number.ToString("N2");
                values.Add(formattedNumber);
                number -= step;
            }
            number = step;
            for (int i = 0; i < Convert.ToInt16(posStepCnt); i++)
            {
                formattedNumber = "+" + number.ToString("N2");
                values.Add(formattedNumber);
                number += step;
            }
            return values;
        }

        public static List<String> CylinderValues()
        {
            double min = -20.00;
            double max = 20.00;
            double step = 0.25;
            double posStepCnt = max / step;
            double negStepCnt = (Math.Abs(min) / step);
            double number = 0;
            string formattedNumber = number.ToString("N2");
            List<String> values = new List<String>();
            values.Add(formattedNumber);

            number = step * -1;
            for (int i = 0; i < Convert.ToInt16(negStepCnt); i++)
            {
                formattedNumber = number.ToString("N2");
                values.Add(formattedNumber);
                number -= step;
            }

            number = step;
            for (int i = 0; i < Convert.ToInt16(posStepCnt); i++)
            {
                formattedNumber = "+" + number.ToString("N2");
                values.Add(formattedNumber);
                number += step;
            }

            return values;
        }

        public static List<String> PrismValues()
        {
            double min = 0.00;
            double max = 15.00;
            double step = 0.25;
            double stepCnt = ((Math.Abs(min) + max) / step) + 1;

            List<String> values = new List<String>();

            double number = min;

            for (int i = 0; i < Convert.ToInt16(stepCnt); i++)
            {
                values.Add(number.ToString("N2"));
                number += step;
            }
            return values;
        }

        public static List<String> AxisValues()
        {
            int min = 0;
            int max = 180;
            int step = 1;
            int stepCnt = ((Math.Abs(min) + max) / step) + 1;

            List<String> values = new List<String>();

            int number = min;
            string formattedNumber = string.Empty;
            for (int i = 0; i < stepCnt; i++)
            {
                if (number < 10)
                {
                    formattedNumber = "00" + number.ToString();
                }
                else if (number < 100)
                {
                    formattedNumber = "0" + number.ToString();
                }
                else
                {
                    formattedNumber = number.ToString();
                }
                values.Add(formattedNumber);
                number += step;
            }
            return values;
        }

        public static string FormatAxis(int number)
        {
           string formattedNum = Convert.ToString(number).PadLeft(3, '0');

           return formattedNum;
        }

        public static string AddPlusSign(decimal _num)
        {
            decimal num = _num;
            string formattedNum = string.Empty;

            if (num > 0)
            {
                formattedNum = string.Format("+{0}", num.ToString("N2"));
            }
            else if (num <= 0)
            {
                formattedNum = num.ToString("N2");
            }

            return formattedNum;
        }

        public static List<String> AddValues()
        {
            double min = 0.00;
            double max = 15.00;
            double step = 0.25;
            double stepCnt = ((Math.Abs(min) + max) / step) + 1;

            List<String> values = new List<String>();

            double number = min;

            for (int i = 0; i < Convert.ToInt16(stepCnt); i++)
            {
                values.Add(number.ToString("N2"));
                number += step;
            }
            return values;
        }

        public static List<String> PDTotalValues()
        {
            double min = 52;
            int max = 82;
            double step = 1;

            List<String> values = new List<String>();

            for (min = 52; min <= max; min += step)
            {
                values.Add(min.ToString());
            }
            return values;
        }

        public static List<String> PDMonoValues()
        {
            double min = 26;
            int max = 41;
            double step = 0.5;

            List<String> values = new List<String>();

            for (min = 26; min <= max; min += step)
            {
                values.Add(min.ToString());
            }
            return values;
        }

        public static string CheckAxis(string _cylinder, string _axis)
        {
            if (_cylinder.ToUpper().StartsWith("S") || _cylinder.StartsWith("0") || string.IsNullOrEmpty(_cylinder))
            {
                return "0";
            }
            else
            {
                return _axis;
            }
        }

        public static decimal CylinderToDecimal(string cylinder)
        {
            decimal valOut = 0;
            return decimal.TryParse(cylinder, out valOut) ? valOut : 0;
        }

        public static string CylinderToString(decimal? cylinder)
        {
            string tmpStr = string.Empty;
            if (string.IsNullOrEmpty(cylinder.ToString()))
            {
                return tmpStr;
            }
            if (cylinder == 0)
            {
                return "SPHERE";
            }
            else
            {
                tmpStr = cylinder.ToString();
            }
            return tmpStr;
        }

        public static decimal PrismToDecimal(string prism)
        {
            decimal valOut = 0;
            return decimal.TryParse(prism, out valOut) ? valOut : 0;
        }

        public static string PrismToString(decimal? prism)
        {
            return string.IsNullOrEmpty(prism.ToString()) ? "0" : prism.ToString();
        }

        public static int AxisToInt(string axis)
        {
            int valOut = 0;
            return int.TryParse(axis, out valOut) ? valOut : 0;
        }

        public static string AxisToString(decimal? axis)
        {
            return string.IsNullOrEmpty(axis.ToString()) ? "0" : axis.ToString();
        }

        public static decimal AddValueToDecimal(string addValue)
        {
            decimal valOut = 0;
            return decimal.TryParse(addValue, out valOut) ? valOut : 0;
        }

        public static string AddValueToString(decimal? addValue)
        {
            return string.IsNullOrEmpty(addValue.ToString()) ? "0" : addValue.ToString();
        }

        public static string BaseValueToDisplay(string baseValue)
        {
            if (baseValue == "N")
            {
                return string.Empty;
            }
            else
            {
                return baseValue;
            }
        }

        public static string BaseValueToSave(string baseValue)
        {
            if (string.IsNullOrEmpty(baseValue))
            {
                return "N";
            }
            else
            {
                return baseValue;
            }
        }

        public static string DecimalToString(decimal? sh)
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

        public static decimal StringToDecimal(string str)
        {
            decimal valOut = 0;
            return decimal.TryParse(str, out valOut) ? valOut : 0;
        }

        public static Double StringToDouble(String str)
        {
            Double valOut = 0d;
            return Double.TryParse(str, out valOut) ? valOut : default(Double);
        }
    }
}