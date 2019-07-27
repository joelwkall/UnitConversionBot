using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using Conversion.Model;

namespace Conversion.Scanners
{
    public class FeetOrInchesScanner : BaseScanner
    {
        private static string _pattern = $"(?:\\s|^|{WordSeparatorRegex})({NumberRegex}(\"|'))(?:\\s|$|{WordSeparatorRegex})";

        public override (string remaining, IEnumerable<DetectedMeasurement> foundMeasurements) FindMeasurements(
            string str)
        {
            var found = new List<DetectedMeasurement>();

            //loop to find all instances in the input string
            var detected = MatchLoop(str, _pattern, (match, s) => CreateMeasurement(match, s));

            foreach (var d in detected)
                str = str.Replace(d.DetectedString, "");

            found.AddRange(detected);

            return (str, found);
        }

        private DetectedMeasurement CreateMeasurement(Match regex, string str)
        {
            var foundStr = regex.Groups[1].Captures[0].Value;
            var amountStr = regex.Groups[2].Captures[0].Value;
            var unitStr = regex.Groups[3].Captures[0].Value;

            if (Parse(amountStr, out var amount))
            {
                var digits = Utils.DetectSignificantDigits(amountStr);

                var unit = unitStr == "\""
                    ? UnitFamily.ImperialDistances.GetUnit("inch")
                    : UnitFamily.ImperialDistances.GetUnit("foot");

                if (!IsQuoting(str, foundStr, unitStr[0]))
                     return new DetectedMeasurement(unit, amount, digits, foundStr);
            }

            return new NonDetectedMeasurement(foundStr);
        }

        private bool IsQuoting(string baseStr, string subStr, char quoteType)
        {
            var pos = baseStr.IndexOf(subStr);

            if(pos==-1)
                throw new ArgumentException($"{nameof(baseStr)} must contain {nameof(subStr)}");

            //loop through each char
            var quotesBefore = 0;
            var quotesAfter = 0;
            for (var i = 0; i < baseStr.Length; i++)
            {
                //ignore the chars inside the substring
                if (i >= pos && i <= pos+subStr.Length)
                    continue;

                var c = baseStr[i];

                //check if we are inside a quote
                if (c == quoteType)
                {
                    var prevC = i > 0 ? baseStr[i - 1] : 'a';
                    //if the previous char is a digit, then it's not the start or end of a quote
                    if (!char.IsDigit(prevC))
                    {
                        if (i < pos)
                            quotesBefore++;
                        else
                            quotesAfter++;
                    }
                    
                }
            }

            //if there is an odd number of quotes before, the substring might be the ending quote
            if (quotesBefore % 2 == 1)
            {
                //if there is an even number after, the quotes dont match up so the substring is the ending quote
                if (quotesAfter % 2 == 0)
                    return true;
            }

            return false;
        }
    }
}
