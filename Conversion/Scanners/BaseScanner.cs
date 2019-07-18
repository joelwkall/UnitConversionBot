using System;
using Conversion.Model;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Conversion.Scanners
{
    public abstract class BaseScanner
    {
        //TODO: it's ugly that this method is expected to remove the occurences.
        //Better if DetectedMeasurement had the index of the found string instead or something
        public abstract (string remaining, IEnumerable<DetectedMeasurement> foundMeasurements) FindMeasurements(string str);

        public static string NumberRegex = "((?:(?:\\.|,)?\\d+(?:[\\.,\\s]\\d+)*(?:(?:\\.|,)\\d+)?))";

        //Omitting front slash from here because we dont want it to catch fractions
        public static string WordSeparatorRegex = "[<>.,;:\\-_!#¤%&()=?`@£${}+´¨~*'\"]";

        public static bool Parse(string str, out double amount)
        {
            //remove spaces since they are only valid as thousands separators
            str = str.Replace(" ", "");

            bool decimalComma = false;

            //if there are both commas and points, then the decimal marker is the last of them
            if (str.Contains(',') && str.Contains('.'))
            {
                if (str.LastIndexOf(',') > str.LastIndexOf('.'))
                    decimalComma = true;
            }
            //no points, and only one comma, then probably decimal comma
            //except if it is a clear thousands separator comma
            else if (str.Contains(',') && str.Count(c=>c==',')==1 && !Regex.Match(str,",\\d00").Success)
                decimalComma = true;
            //if there is a clear thousands separator point
            else if (Regex.Match(str, "\\.\\d00").Success)
                decimalComma = true;
        
            //if it's a decimal comma, we switch commas and periods
            if (decimalComma)
                str = str.Replace(",", "COMMA").Replace(".", ",").Replace("COMMA", ".");

            if (double.TryParse(str,
                NumberStyles.Number | NumberStyles.AllowThousands | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture,
                out amount))
            {
                return true;
            }

            return false;
        }
    }
}
