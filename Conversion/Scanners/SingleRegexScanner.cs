﻿using Conversion.Model;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Conversion.Scanners
{
    public class SingleRegexScanner: BaseScanner
    {
        private static string _regex = $"(?:\\s|^|{WordSeparatorRegex})({NumberRegex}PATTERN)(?:\\s|$|{WordSeparatorRegex})";

        private static Unit[] _exactCaseUnits = new[]
        {
            UnitFamily.Meters.GetUnit("m"),
            UnitFamily.Kilograms.GetUnit("g"),
            UnitFamily.Liters.GetUnit("l"),
        };

        public override (string remaining, IEnumerable<DetectedMeasurement> foundMeasurements) FindMeasurements(string str)
        {
            var found = new List<DetectedMeasurement>();

            foreach (var u in UnitFamily.AllUnits)
            {
                //construct the pattern to inject based on options
                var pattern = "(?:";

                //"in" is also a word so we require it to be without space
                if (u.Singular != "in")
                    pattern = "\\s?" + pattern; //TODO: make sure line breaks are not allowed between

                pattern += Escape(u.Singular) + "|" + Escape(u.Plural) + ")";

                pattern = _regex.Replace("PATTERN", pattern, true, CultureInfo.InvariantCulture);

                var ignoreCase = _exactCaseUnits.Contains(u) ? false : true;

                //loop to find all instances in the input string
                var detected = MatchLoop(str, pattern, ignoreCase, (match, _) => CreateMeasurement(match, u));

                foreach (var d in detected)
                    str = str.Replace(d.DetectedString, "");

                found.AddRange(detected);
            }

            return (str, found);
        }

        private string Escape(string unit)
        {
            return unit.Replace("*", "\\*").Replace("^", "\\^");
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
