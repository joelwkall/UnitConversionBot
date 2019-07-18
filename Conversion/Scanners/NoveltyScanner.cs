using System;
using System.Collections.Generic;
using Conversion.Model;

namespace Conversion.Scanners
{
    public class NoveltyScanner : BaseScanner
    {
        public override (string remaining, IEnumerable<DetectedMeasurement> foundMeasurements) FindMeasurements(string str)
        {
            var phrases = new[]
            {
                ("banana for scale", "banana"),
                ("doggo", "doggo"),
                ("puppy", "puppy"),
                ("pupper", "pupper")
            };

            var measurements = new List<DetectedMeasurement>();

            foreach (var (phrase, result) in phrases)
            {
                var pos = str.IndexOf(phrase, StringComparison.InvariantCultureIgnoreCase);

                if (pos != -1)
                {
                    measurements.Add(new DetectedMeasurement(new Unit(result, result + "s"), 1, 1, "1 " + result));
                    str = str.Replace(phrase, "", StringComparison.InvariantCultureIgnoreCase);
                }

            }

            return (str, measurements);
        }
    }
}
