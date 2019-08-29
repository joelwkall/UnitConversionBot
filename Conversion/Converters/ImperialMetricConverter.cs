using Conversion.Model;
using System.Collections.Generic;
using System.Linq;

namespace Conversion.Converters
{
    public class ImperialMetricConverter : BaseConverter
    {
        private static List<(UnitFamily, double, UnitFamily, bool)> Conversions = new List<(UnitFamily, double, UnitFamily, bool)>()
        {
            (UnitFamily.Mph, 1.609344, UnitFamily.Kmph, true),
            (UnitFamily.Knots, 1.85200, UnitFamily.Kmph, false),
            (UnitFamily.Knots, 1.15077945, UnitFamily.Mph, false),
            (UnitFamily.Mps, 2.23693629, UnitFamily.Mph, false),
            (UnitFamily.ImperialDistances, 0.3048, UnitFamily.Meters, true),
            (UnitFamily.Pounds, 0.45359237, UnitFamily.Kilograms, true),
            (UnitFamily.Stones, 6.35029318, UnitFamily.Kilograms, true), //TODO: maybe dont convert very small numbers to stones (a range filter)
            (UnitFamily.Stones, 14, UnitFamily.Pounds, true),
            (UnitFamily.USVolumes, 3.78541178, UnitFamily.Liters, true),
            (UnitFamily.ImperialVolumes, 4.54609188, UnitFamily.Liters, true),
            (UnitFamily.USVolumes, 0.83267384, UnitFamily.ImperialVolumes, true),
            (UnitFamily.MetricArea, 10.7639104, UnitFamily.ImperialArea, true),
        };

        public override void Convert(ConversionCollection collection)
        {
            foreach (var m in collection.AllValidMeasurements.ToList())
            {
                foreach (var (from, ratio, to, bothways) in Conversions)
                {
                    if (m.Unit.UnitFamily == from)
                    {
                        collection.ConvertedMeasurements.Add(new Measurement(
                            to.PrimaryUnit,
                            m.Amount * ratio * m.Unit.Ratio
                        ));
                    }
                    else if (bothways && m.Unit.UnitFamily == to)
                    {
                        collection.ConvertedMeasurements.Add(new Measurement(
                            from.PrimaryUnit,
                            m.Amount * (1 / ratio) * m.Unit.Ratio
                        ));
                    }
                }
            }
        }
    }
}
