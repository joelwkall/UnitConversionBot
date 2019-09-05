using System;
using System.Collections.Generic;
using System.Linq;
using Conversion.Converters;
using Conversion.Filters;
using Conversion.Formatters;
using Conversion.Model;
using Conversion.Scanners;

namespace Conversion
{
    public class ConversionCollection
    {
        public DetectedMeasurement DetectedMeasurement;
        public List<Measurement> ConvertedMeasurements;

        public IEnumerable<Measurement> AllValidMeasurements => ConvertedMeasurements.Prepend(DetectedMeasurement).Where(v=>v.Amount != 0);

        public ConversionCollection(DetectedMeasurement detectedMeasurement) : this(detectedMeasurement, new List<Measurement>())
        {
        }

        public ConversionCollection(DetectedMeasurement detectedMeasurement, IEnumerable<Measurement> convertedMeasurements)
        {
            DetectedMeasurement = detectedMeasurement;
            ConvertedMeasurements = convertedMeasurements.ToList();
        }
    }

    public class TextAnalyzer
    {
        public static TextAnalyzer Default { get; private set; } = CreateNewDefault();

        public static BaseFormatter InchFormatter = new RecursiveFormatter(m =>
            m.Unit.UnitFamily == UnitFamily.ImperialDistances && m.Unit.Ratio == 1.0 / 12);

        public static BaseFormatter FootFormatter = new RecursiveFormatter(
            InchFormatter,
            m => m.Unit.UnitFamily == UnitFamily.ImperialDistances && m.Unit.Ratio == 1,
        UnitFamily.ImperialDistances.GetUnit("inch"));

        public static TextAnalyzer CreateNewDefault()
        {
            return new TextAnalyzer(
                new BaseScanner[]
                {
                    new FeetAndInchesScanner(),
                    new FeetOrInchesScanner(),
                    new SingleRegexScanner(),
                    new NoveltyScanner(),
                },
                new BaseConverter[]
                {
                    new ImperialMetricConverter(),
                    new ReadabilityConverter(),
                    new TemperatureConverter(),
                    new NoveltyConverter(0.1),
                },
                new BaseFilter[]
                {
                    new DuplicatesFilter(),
                },
                new BaseFormatter[]
                {
                    InchFormatter, 
                    FootFormatter, 
                    new DefaultFormatter(), 
                }
            );
        }

        //TODO: these could all be interfaces instead
        public IEnumerable<BaseScanner> Scanners { get; private set; }
        public IEnumerable<BaseConverter> Converters { get; private set; }
        public IEnumerable<BaseFilter> Filters { get; private set; }
        public IEnumerable<BaseFormatter> Formatters { get; private set; }

        public TextAnalyzer(IEnumerable<BaseScanner> scanners, IEnumerable<BaseConverter> converters, IEnumerable<BaseFilter> filters, IEnumerable<BaseFormatter> formatters)
        {
            Scanners = scanners;
            Converters = converters;
            Filters = filters;
            Formatters = formatters;
        }

        public IEnumerable<string> FindConversions(params string[] strs)
        {
            if (strs == null || !strs.Any())
                yield break;

            var foundMeasurements = new List<ConversionCollection>();

            //ignore empty strings
            foreach (var str in strs.Where(s => !string.IsNullOrWhiteSpace(s)))
            {
                var currentStr = str;

                //detect all measurements in all texts
                foreach (var scanner in Scanners)
                {
                    var (newStr, measurements) = scanner.FindMeasurements(currentStr);
                    currentStr = newStr;

                    foreach (var measurement in measurements)
                    {
                        if (!foundMeasurements.Any(c=>c.DetectedMeasurement==measurement))
                            foundMeasurements.Add(new ConversionCollection(measurement));
                    }
                }
            }

            //run them through converters
            foreach (var converter in Converters)
            {
                foreach (var collection in foundMeasurements)
                {
                    converter.Convert(collection);
                }
            }

            foundMeasurements = foundMeasurements.Where(o => o.ConvertedMeasurements.Count>0).ToList();

            //run filters
            var originalMeasurements = foundMeasurements.ToList(); //clone so we can modify it
            foreach (var filter in Filters)
            {
                foreach (var collection in originalMeasurements)
                {
                    if (!filter.Keep(collection, originalMeasurements))
                    {
                        //TODO: log this
                        foundMeasurements.Remove(collection);
                    }
                }
            }
        
            //create output
            foreach (var collection in foundMeasurements)
            {
                yield return collection.DetectedMeasurement.DetectedString + " ≈ " + 
                             string.Join(" or ", collection.ConvertedMeasurements.Select(v=>Format(v, collection.DetectedMeasurement.SignificantDigits)));
            }
        }

        private string Format(Measurement m, int significantDigits)
        {
            foreach (var formatter in Formatters)
            {
                //format the output with 1 more significant digit to balance accuracy and readability
                if (formatter.CanFormat(m))
                    return formatter.FormatMeasurement(m, significantDigits + 1);
            }

            return m.ToString();
        }
    }
}
