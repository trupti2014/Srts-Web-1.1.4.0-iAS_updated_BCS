using System;

namespace SrtsWeb.BusinessLayer.TypeExtendersAndHelpers.Extenders
{
    public static partial class SrtsExtender
    {
        public static int ToFiscalYear(this DateTime dt)
        {
            return (dt.Month < 10) ? dt.Year : dt.Year + 1;
        }

        public static string ToMilDateString(this DateTime dt)
        {
            return dt.ToString("MM/dd/yyyy").ToUpper();
        }

        public static string ToMilTimeString(this DateTime dt)
        { return dt.ToString("HHmm") + " HRS"; }

        public static string ToMilDateTimeString(this DateTime dt)
        { return dt.ToString("dd MMM yyyy HHmm".ToUpper()) + " HRS"; }

        public static string ToMilDateString(this DateTime? ndt)
        {
            if (ndt.HasValue)
                return ndt.Value.ToString("dd MMM yyyy").ToUpper();
            else
                return string.Empty;
        }

        public static string ToMilTimeString(this DateTime? ndt)
        {
            if (ndt.HasValue) { return ndt.Value.ToString("HHmm") + " HRS"; }
            else { return string.Empty; }
        }

        public static string ToMilDateTimeString(this DateTime? ndt)
        {
            if (ndt.HasValue)
            {
                string datePart = ndt.ToMilDateString();
                string timePart = ndt.ToMilTimeString();
                return string.Format("{0} {1}", datePart, timePart);
            }
            else
                return string.Empty;
        }

        public static Double DateDiff(this DateTime dt, DateTime dtCompare)
        {
            TimeSpan ts;
            if (dtCompare > dt)
                ts = dtCompare - dt;
            else
                ts = dt - dtCompare;
            return Math.Round(ts.TotalDays);
        }
    }
}