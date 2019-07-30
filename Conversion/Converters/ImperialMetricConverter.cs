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
            (UnitFamily.Stones, 6.35029318, UnitFamily.Kilograms), //TODO: maybe dont convert very small numbers to stones (separate stone converter?)
            (UnitFamily.Stones, 14, UnitFamily.Pounds),
            (UnitFamily.USVolumes, 3.78541178, UnitFamily.Liters),
            (UnitFamily.ImperialVolumes, 4.54609188, UnitFamily.Liters),
            (UnitFamily.USVolumes, 0.83267384, UnitFamily.ImperialVolumes),
        };

        public override IEnumerable<Measurement> Convert(Measurement m)
        {
            foreach (var (from, ratio, to) in Conversions)
            {
                if (m.Unit.UnitFamily == from)
                {
                    yield return new Measurement(
                        to.Units.First(e => e.Ratio == 1.0),
                        m.Amount * ratio * m.Unit.Ratio
                    );
                }
                else if (m.Unit.UnitFamily == to)
                {
                    yield return new Measurement(
                        from.Units.First(e => e.Ratio == 1.0),
                        m.Amount * (1/ratio) * m.Unit.Ratio
                    );
                }
            }
        }
    }
}
