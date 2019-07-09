using System.Collections.Generic;
using System.Linq;
using Conversion.Converters;
using Conversion.Model;
using Conversion.Scanners;

namespace Conversion
{
    public class TextAnalyzer
    {
        private IEnumerable<BaseScanner> _scanners;
        private IEnumerable<BaseConverter> _converters;

        public TextAnalyzer(IEnumerable<BaseScanner> scanners, IEnumerable<BaseConverter> converters)
        {
            _scanners = scanners;
            _converters = converters;
        }

        public IEnumerable<string> FindConversions(string str)
        {
            if (str == null)
                yield break;

            var foundMeasurements = new Dictionary<DetectedMeasurement, Measurement>();

            //detect all measurements
            foreach (var scanner in _scanners)
            {
                var (newStr, measurements) = scanner.FindMeasurements(str);
                str = newStr; 

                foreach (var measurement in measurements)
                {
                    foundMeasurements.Add(measurement,measurement);
                }
            }

            //run them through converters
            foreach (var converter in _converters)
            {
                foreach (var pair in foundMeasurements.ToDictionary(o=>o.Key, o=>o.Value))
                {
                    var converted = converter.Convert(pair.Value);

                    if(converted != null)
                        foundMeasurements[pair.Key] = converted;
                }
            }

            var successful = foundMeasurements.Where(o => o.Key != o.Value);

            if(successful.Count() < foundMeasurements.Count())
            {
                //TODO: log this
            }

            //TODO: run filters (to remove duplicates and unnecessary conversions)

            foreach (var pair in successful)
            {
                //format the output with 1 more significant digit to balance accuracy and readability
                yield return pair.Key.DetectedString + " ≈ " + pair.Value.ToString(pair.Key.SignificantDigits + 1);
            }
        }
    }
}
