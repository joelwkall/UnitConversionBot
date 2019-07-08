using System;
using System.Collections.Generic;
using Conversion.Model;

namespace Conversion.Scanners
{
    public class NoveltyScanner : BaseScanner
    {
        public override (string remaining, IEnumerable<DetectedMeasurement> foundMeasurements) FindMeasurements(string str)
        {
            string[] phrases = new[] {"banana", "doggo", "puppy", "pupper"};

            var measurements = new List<DetectedMeasurement>();

            foreach (var phrase in phrases)
            {
                var pos = str.IndexOf(phrase, StringComparison.InvariantCultureIgnoreCase);

                if (pos != -1)
                {
                    measurements.Add(new DetectedMeasurement(new UnitExpression(phrase, phrase + "s"), 1, 1, "1 " + phrase));
                    str = str.Replace(phrase, "", StringComparison.InvariantCultureIgnoreCase);
                }

            }

            return (str, measurements);
        }
    }
}
