using System;
using System.Text.RegularExpressions;

namespace ProjectHub.Util
{
    static class StringCharFilter
    {
        public static string Escape(string String, bool AllowBreaks = false)
        {
            char[] charsToTrim = { ' ', '\t' };
            String = String.Trim(charsToTrim);
            String = String.Replace(Convert.ToChar(1), ' ');
            String = String.Replace(Convert.ToChar(2), ' ');
            String = String.Replace(Convert.ToChar(3), ' ');
            String = String.Replace(Convert.ToChar(9), ' ');

            if (!AllowBreaks)
            {
                String = String.Replace(Convert.ToChar(10), ' ');
                String = String.Replace(Convert.ToChar(13), ' ');
            }

            String = Regex.Replace(String, "<(.|\\n)*?>", string.Empty);

            return String;
        }
    }
}
