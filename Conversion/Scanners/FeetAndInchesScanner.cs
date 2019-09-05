using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using Conversion.Model;

namespace Conversion.Scanners
{
    public class FeetAndInchesScanner : BaseScanner
    {
        //TODO: accept double apostrophe, and “” style quotes.
        private static string _pattern = $"{NumberRegex}'\\s?{NumberRegex}\"";

        public override (string remaining, IEnumerable<DetectedMeasurement> foundMeasurements) FindMeasurements(
            string str)
        {
            var found = new List<DetectedMeasurement>();

            //loop to find all instances in the input string
            var detected = MatchLoop(str, _pattern, true, (match, _) => CreateMeasurement(match));

            foreach (var d in detected)
                str = str.Replace(d.DetectedString, "");

            found.AddRange(detected);
             
            return (str, found);
        }

        private DetectedMeasurement CreateMeasurement(Match regex)
        {
            var foundStr = regex.Groups[0].Captures[0].Value;
            var amountStr1 = regex.Groups[1].Captures[0].Value;
            var amountStr2 = regex.Groups[2].Captures[0].Value;

            if (Parse(amountStr1, out var amount1) && Parse(amountStr2, out var amount2))
            {
                var digits1 = Utils.DetectSignificantDigits(amountStr1);
                var digits2 = Utils.DetectSignificantDigits(amountStr2);

                var amount = amount2 + 12 * amount1;

                return new DetectedMeasurement(UnitFamily.ImperialDistances.GetUnit("inch"), amount, digits1 + digits2, foundStr);
            }

            return new NonDetectedMeasurement(foundStr);
        }
    }
}
