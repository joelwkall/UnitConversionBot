using Conversion.Model;
using System.Collections.Generic;
using System.Linq;

namespace Conversion.Converters
{
    public class ImperialMetricConverter : BaseConverter
    {
        private static List<(UnitFamily, double, UnitFamily)> Conversions = new List<(UnitFamily, double, UnitFamily)>()
        {
            (UnitFamily.Mph, 1.609344, UnitFamily.Kmph),
            (UnitFamily.ImperialDistances, 0.3048, UnitFamily.Meters),
            (UnitFamily.Pounds, 0.45359237, UnitFamily.Kilograms),
            (UnitFamily.Stones, 6.35029318, UnitFamily.Kilograms), //TODO: maybe dont convert very small numbers to stones (a range filter)
            (UnitFamily.Stones, 14, UnitFamily.Pounds),
            (UnitFamily.USVolumes, 3.78541178, UnitFamily.Liters),
            (UnitFamily.ImperialVolumes, 4.54609188, UnitFamily.Liters),
            (UnitFamily.USVolumes, 0.83267384, UnitFamily.ImperialVolumes),
            (UnitFamily.MetricArea, 10.7639104, UnitFamily.ImperialArea),
        };

        public override void Convert(ConversionCollection collection)
        {
            foreach (var m in collection.AllValidMeasurements.ToList())
            {
                foreach (var (from, ratio, to) in Conversions)
                {
                    if (m.Unit.UnitFamily == from)
                    {
                        collection.ConvertedMeasurements.Add(new Measurement(
                            to.PrimaryUnit,
                            m.Amount * ratio * m.Unit.Ratio
                        ));
                    }
                    else if (m.Unit.UnitFamily == to)
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
