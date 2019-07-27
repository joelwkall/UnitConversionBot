using Conversion.Model;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Conversion.Scanners
{
    public class SingleRegexScanner: BaseScanner
    {
        private static string _regex = $"(?:\\s|^|{WordSeparatorRegex})({NumberRegex}PATTERN)(?:\\s|$|{WordSeparatorRegex})";

        public override (string remaining, IEnumerable<DetectedMeasurement> foundMeasurements) FindMeasurements(string str)
        {
            var found = new List<DetectedMeasurement>();

            foreach (var u in UnitFamily.AllUnits)
            {
                //construct the pattern to inject based on options
                var pattern = "(?:";

                //"in" is also a word so we require it to be without space
                if (u.Singular != "in")
                    pattern = "\\s?" + pattern;

                pattern += u.Singular + "|" + u.Plural + ")";

                pattern = _regex.Replace("PATTERN", pattern, true, CultureInfo.InvariantCulture);

                //loop to find all instances in the input string
                var detected = MatchLoop(str, pattern, (match, _) => CreateMeasurement(match, u));

                foreach (var d in detected)
                    str = str.Replace(d.DetectedString, "");

                found.AddRange(detected);
            }

            return (str, found);
        }

        private DetectedMeasurement CreateMeasurement(Match regex, Unit u)
        {
            var foundStr = regex.Groups[1].Captures[0].Value;
            var amountStr = regex.Groups[2].Captures[0].Value;

            if (Parse(amountStr, out var amount))
            {
                var digits = Utils.DetectSignificantDigits(amountStr);

                return new DetectedMeasurement(u, amount, digits, foundStr);
            }

            return new NonDetectedMeasurement(foundStr);
        }
    }
}
