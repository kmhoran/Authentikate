using System;
using System.Globalization;

namespace Utils.Extentions
{
    public static class DateExtentions
    {
        public static string ToISOString(this DateTime date)
        {
            var DateTimeFormat = "{0}-{1}-{2}T{3}:{4}:{5}Z";
            return string.Format(
            DateTimeFormat,
            date.Year,
            PadLeft(date.Month),
            PadLeft(date.Day),
            PadLeft(date.Hour),
            PadLeft(date.Minute),
            PadLeft(date.Second));
        }
        private static string PadLeft(int number)
        {
            if (number >= 10) return number.ToString(CultureInfo.InvariantCulture);
            return $"0{number}";

        }
    }
}
