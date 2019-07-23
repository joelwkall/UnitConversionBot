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
                if (u.Singular!="in")
                    pattern = "\\s?" + pattern;

                pattern += u.Singular + "|" + u.Plural + ")";

                pattern = _regex.Replace("PATTERN", pattern, true, CultureInfo.InvariantCulture);

                //loop to find all instances in the input string
                while (true)
                {
                    var regex = Regex.Match(str, pattern, RegexOptions.IgnoreCase);

                    var culture = CultureInfo.InvariantCulture;
                    if (regex.Success && regex.Groups.Count > 0)
                    {
                        var foundStr = regex.Groups[1].Captures[0].Value;
                        var amountStr = regex.Groups[2].Captures[0].Value;
                        
                        if (Parse(amountStr, out var amount))
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
