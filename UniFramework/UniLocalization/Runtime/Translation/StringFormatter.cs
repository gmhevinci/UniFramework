using System;
using System.Linq;
using System.Collections.Generic;

namespace UniFramework.Localization
{
    public static class StringFormatter
    {
        public static string FormateString(string text, IFormatProvider formatProvider, params object[] args)
        {
            if (args == null || args.Length == 0)
                return text;

            try
            {
                if (formatProvider == null)
                    return string.Format(text, args);
                else
                    return string.Format(formatProvider, text, args);
            }
            catch (FormatException e)
            {
                UniLogger.Warning($"Input string was not in the correct format for String.Format ! : {text} {e}");
                return text;
            }
        }
    }
}