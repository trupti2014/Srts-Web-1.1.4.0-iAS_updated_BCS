using System;

namespace SrtsWeb.BusinessLayer.TypeExtendersAndHelpers.Helpers
{
    public static partial class SrtsHelper
    {
        public static DateTime? ParseValue(object DateTimeObject)
        {
            DateTime retVal = new DateTime();

            if (!DateTime.TryParse(DateTimeObject.ToString(), out retVal)
                && DateTimeObject.ToString() != string.Empty)
            {
                try
                {
                    string sDate = DateTimeObject.ToString().Trim().Replace(" ", "-");
                    string[] dateArray = sDate.Split('-');
                    int day = Convert.ToInt32(dateArray[0]);
                    int mon = 0;
                    switch (dateArray[1].ToUpper())
                    {
                        case "JAN": mon = 1; break;
                        case "FEB": mon = 2; break;
                        case "MAR": mon = 3; break;
                        case "APR": mon = 4; break;
                        case "MAY": mon = 5; break;
                        case "JUN": mon = 6; break;
                        case "JUL": mon = 7; break;
                        case "AUG": mon = 8; break;
                        case "SEP": mon = 9; break;
                        case "OCT": mon = 10; break;
                        case "NOV": mon = 11; break;
                        case "DEC": mon = 12; break;
                    }
                    int year = Convert.ToInt32(dateArray[2]);
                    retVal = new DateTime(year, mon, day);
                }
                catch { }
            }
            if (retVal == new DateTime())
                return null;
            else
                return retVal;
        }

        public static bool CheckDateForGoodDate(DateTime? _checkDate)
        {
            if (!_checkDate.HasValue || _checkDate == DateTime.Parse("01/01/1900"))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static DateTime SetDateOrDefault(DateTime? _checkDate)
        {
            if (_checkDate.HasValue && _checkDate != DateTime.Parse("01/01/1900"))
            {
                return (DateTime)_checkDate;
            }
            else
            {
                return DateTime.Parse("01/01/1900");
            }
        }
    }
}