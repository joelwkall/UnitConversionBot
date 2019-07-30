using System.Collections.Generic;
using System.Linq;
using Conversion.Converters;
using Conversion.Filters;
using Conversion.Model;
using Conversion.Scanners;

namespace Conversion
{
    public class TextAnalyzer
    {
        public static TextAnalyzer Default { get; private set; } = new TextAnalyzer(
            new BaseScanner[]
            {
                new NoveltyScanner(),
                new FeetAndInchesScanner(),
                new FeetOrInchesScanner(),
                new SingleRegexScanner(),
            },
            new BaseConverter[]
            {
                new ImperialMetricConverter(),
                new NoveltyConverter(0.1),
                new ReadabilityConverter(),
                new TemperatureConverter(), 
            },
            new BaseFilter[]
            {
                new DuplicatesFilter(), 
            }
        );

        private IEnumerable<BaseScanner> _scanners;
        private IEnumerable<BaseConverter> _converters;
        private IEnumerable<BaseFilter> _filters;

        public TextAnalyzer(IEnumerable<BaseScanner> scanners, IEnumerable<BaseConverter> converters, IEnumerable<BaseFilter> filters)
        {
            _scanners = scanners;
            _converters = converters;
            _filters = filters;
        }

        public IEnumerable<string> FindConversions(params string[] strs)
        {
            if (strs == null || !strs.Any())
                yield break;

            var foundMeasurements = new Dictionary<DetectedMeasurement, List<Measurement>>();

            //ignore empty strings
            foreach (var str in strs.Where(s => !string.IsNullOrWhiteSpace(s)))
            {
                var currentStr = str;

                //detect all measurements in all texts
                foreach (var scanner in _scanners)
                {
                    var (newStr, measurements) = scanner.FindMeasurements(currentStr);
                    currentStr = newStr;

                    foreach (var measurement in measurements)
                    {
                        if (!foundMeasurements.ContainsKey(measurement))
                            foundMeasurements.Add(measurement, new List<Measurement>(){measurement});
                    }
                }
            }

            //run them through converters
            foreach (var converter in _converters)
            {
                foreach (var pair in foundMeasurements.ToDictionary(o => o.Key, o => o.Value))
                {
                    foreach (var value in pair.Value.ToList())
                    {
                        foreach (var converted in converter.Convert(value))
                        {
                            if (converted != null)
                            {
                                foundMeasurements[pair.Key].Remove(value);
                                foundMeasurements[pair.Key].Add(converted);
                            }
                        }
                    }
                }
            }
            
            var successful = foundMeasurements.Where(o => o.Value.Count>1 || o.Key != o.Value.First());

            if(successful.Count() < foundMeasurements.Count())
            {
                //TODO: log this
            }

            //run filters
            var originalMeasurements = foundMeasurements.ToList();
            foreach (var filter in _filters)
            {
                foreach (var pair in originalMeasurements)
                {
                    if (!filter.Keep(pair, originalMeasurements))
                    {
                        //TODO: log this
                        foundMeasurements.Remove(pair.Key);
                    }
                }
            }
        
            //create output
            //TODO: here we can have a cascading list of formatters,
            //if we want to format some measurements in a special way
            foreach (var pair in successful)
            {
                //format the output with 1 more significant digit to balance accuracy and readability
                yield return pair.Key.DetectedString + " ≈ " + string.Join(" or ", pair.Value.Select(v=>v.ToString(pair.Key.SignificantDigits + 1)));
            }
        }
    }
}
