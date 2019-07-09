using Conversion.Model;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using Conversion.Scanners;
using System;

namespace Conversion.Scanners
{
    public class SingleRegexScanner: BaseScanner
    {
        private static string _wordSeparators = "[<>.,;:_!#¤%&()=?`@£${}+´¨~*'\"]";
        private static string _regex = $"(?:\\s|^|{_wordSeparators})(([\\d,\\.]+)PATTERN)(?:\\s|$|{_wordSeparators})";

        public override (string remaining, IEnumerable<DetectedMeasurement> foundMeasurements) FindMeasurements(string str)
        {
            var found = new List<DetectedMeasurement>();

            foreach (var u in UnitFamily.AllUnits)
            {
                var matchOptions = u.MatchOptions;

                //construct the pattern to inject based on options
                var pattern = "(?:";
                
                if (matchOptions.HasFlag(MatchOptions.AllowSpace))
                    pattern = "\\s?" + pattern;

                pattern += u.Singular;

                if (matchOptions.HasFlag(MatchOptions.AllowPlural))
                    pattern += "|" + u.Plural;

                pattern += ")";

                pattern = _regex.Replace("PATTERN", pattern, true, CultureInfo.InvariantCulture);

                //loop to find all instances in the input string
                while (true)
                {
                    var regex = Regex.Match(str, pattern);

                    var culture = CultureInfo.InvariantCulture;
                    if (regex.Success && regex.Groups.Count > 0)
                    {
                        var foundStr = regex.Groups[1].Captures[0].Value;
                        var amountStr = regex.Groups[2].Captures[0].Value;

                        //TODO: smarter number parser that doesn't parse "3,5" as "35"
                        if (double.TryParse(amountStr,
                            NumberStyles.Number | NumberStyles.AllowThousands, CultureInfo.InvariantCulture,
                            out var amount))
                        {
                            var digits = Utils.DetectSignificantDigits(amountStr);

                            found.Add(new DetectedMeasurement(u, amount, digits, foundStr));
                        }
                        else
                        {
                            //TODO: log this
                            //we matched on a string that was not a number. 
                        }

                        //remove the match from the string so we dont find it again
                        str = str.Replace(foundStr, "");
                    }
                    //if we didnt find a match, that means there are no more instances
                    else
                    {
                        break;
                    }
                }
                
            }

            return (str, found);
        }
    }
}
