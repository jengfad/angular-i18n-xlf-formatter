using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace XlfFormatter.Extensions
{
    public static class StringExtension
    {
        public static string RemoveTabsLineBreaks(this string value)
        {
            return Regex.Replace(value.Trim(), @"\r\n?|\n|\t|[ ]{3,}", string.Empty);
        }

        public static string RemoveWhitespaces(this string value)
        {
            return Regex.Replace(value, @"\s+", string.Empty);
        }

        public static string RemoveCommas(this string value)
        {
            return value.Replace(",", "");
        }

        public static string CsvFriendly(this string value)
        {
            return value.RemoveTabsLineBreaks().RemoveCommas();
        }

        public static string ToLowerNoWhitespaces(this string value)
        {
            return value.ToLower().RemoveWhitespaces();
        }
    }
}
