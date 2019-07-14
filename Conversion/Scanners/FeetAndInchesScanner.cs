using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using Conversion.Model;

namespace Conversion.Scanners
{
    public class FeetAndInchesScanner : BaseScanner
    {
        private static string _pattern = "(\\d+)'\\s?(\\d+)\"";

        public override (string remaining, IEnumerable<DetectedMeasurement> foundMeasurements) FindMeasurements(
            string str)
        {
            var found = new List<DetectedMeasurement>();

            //TODO: this is very similar to SingleRegexScanner
            //loop to find all instances in the input string
            while (true)
            {
                var regex = Regex.Match(str, _pattern);

                var culture = CultureInfo.InvariantCulture;
                if (regex.Success && regex.Groups.Count > 0)
                {
                    var foundStr = regex.Groups[0].Captures[0].Value;
                    var amountStr1 = regex.Groups[1].Captures[0].Value;
                    var amountStr2 = regex.Groups[2].Captures[0].Value;
                    
                    if (Parse(amountStr1, out var amount1) && Parse(amountStr2, out var amount2))
                    {
                        var digits1 = Utils.DetectSignificantDigits(amountStr1);
                        var digits2 = Utils.DetectSignificantDigits(amountStr2);

                        var amount = amount2 + 12 * amount1;

                        found.Add(new DetectedMeasurement(UnitFamily.ImperialDistances.GetUnit("inch"), amount, digits1 + digits2, foundStr));

                        
                    }
                    else
                    {
                        //TODO: log this
                        //we matched on a string that was not a number. 
                    }

                    str = str.Replace(foundStr, "");
                }
                //if we didnt find a match, that means there are no more instances
                else
                {
                    break;
                }
            }

            return (str, found);
        }
    }
}
