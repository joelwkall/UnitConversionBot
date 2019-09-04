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
        public static string WordSeparatorRegex = "[<>.,;:\\-_!#¤&()=?`@{}+´¨~*'\"]";

        public static bool Parse(string str, out double amount)
        {
            //remove spaces since they are only valid as thousands separators
            //TODO: only do this if it is actually a thousands separator. So we dont catch "I ran 2 5km trips today" as 25.
            str = str.Replace(" ", "");

            bool decimalComma = false;

            //TODO: the below algorithm is not very robust. Find a better way
            //if there are both commas and points, then the decimal marker is the last of them
            if (str.Contains(',') && str.Contains('.'))
            {
                if (str.LastIndexOf(',') > str.LastIndexOf('.'))
                    decimalComma = true;
            }
            //no points, and only one comma, then probably decimal comma 
            //except if it is a clear thousands separator comma (TODO: always exclude when 3, 6, 9 decimals?)
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

        protected static IEnumerable<DetectedMeasurement> MatchLoop(string str, string pattern, bool ignoreCase, Func<Match, string, DetectedMeasurement> createFunc)
        {
            while (true)
            {
                var regex = Regex.Match(str, pattern, ignoreCase ? RegexOptions.IgnoreCase : RegexOptions.None);

                var culture = CultureInfo.InvariantCulture;
                if (regex.Success && regex.Groups.Count > 0)
                {
                    var created = createFunc(regex, str);

                    //find out if there is a minus sign before the detected string
                    //but only if preceeded by a space
                    //TODO: this doesnt work if there is a space between the sign and the number
                    var index = str.IndexOf(created.DetectedString);
                    var preceededByMinus = index > 0 && str[index - 1] == '-';
                    var spaceBeforeMinus = index == 1 || (index > 2 && str[index - 2] == ' ');
                    if (preceededByMinus && spaceBeforeMinus)
                    {
                        created.DetectedString = "-" + created.DetectedString;
                        created.Invert();
                    }

                    //remove the match from the string so we dont find it again
                    str = str.Replace(created.DetectedString, "[DETECTED]");

                    //only return it if it was actually found
                    if (!(created is NonDetectedMeasurement))
                        yield return created;
                }
                //if we didnt find a match, that means there are no more instances
                else
                {
                    break;
                }
            }
        }
    }
}
