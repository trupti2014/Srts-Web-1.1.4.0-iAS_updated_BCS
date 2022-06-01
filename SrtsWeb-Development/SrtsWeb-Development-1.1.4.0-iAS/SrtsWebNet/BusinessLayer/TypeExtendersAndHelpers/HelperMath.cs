using System;

namespace SrtsWeb.BusinessLayer.TypeExtendersAndHelpers.Helpers
{
    public static partial class SrtsHelper
    {
        public static decimal RoundInteger(decimal x)
        {
            return Math.Round(x * 4, 2, MidpointRounding.ToEven) / 4;
        }

        public static double RoundDouble(double x)
        {
            return Math.Round(x * 4, 2, MidpointRounding.ToEven) / 4;
        }

        public static bool TestSigns(decimal? firstValue, decimal? secondValue)
        {
            if (firstValue == 0 || secondValue == 0)
            {
                return true;
            }
            if (string.IsNullOrEmpty(firstValue.ToString()) && string.IsNullOrEmpty(secondValue.ToString()))
            {
                return true;
            }
            if ((string.IsNullOrEmpty(firstValue.ToString()) && !string.IsNullOrEmpty(secondValue.ToString())) ||
                (!string.IsNullOrEmpty(firstValue.ToString()) && string.IsNullOrEmpty(secondValue.ToString())))
            {
                return false;
            }
            decimal aDecimal = Convert.ToDecimal(firstValue);
            decimal bDecimal = Convert.ToDecimal(secondValue);
            decimal testNumber = aDecimal * bDecimal;
            if (Math.Sign(testNumber) > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}