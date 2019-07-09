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
            (UnitFamily.Feet, 0.3048, UnitFamily.Meters),
            (UnitFamily.Pounds, 0.45359237, UnitFamily.Kilograms)
        };

        public override Measurement Convert(Measurement m)
        {
            foreach (var (from, ratio, to) in Conversions)
            {
                if (m.Unit.UnitFamily == from)
                {
                    return new Measurement(
                        to.Units.First(e => e.Ratio == 1.0),
                        m.Amount * ratio * m.Unit.Ratio
                    );
                }
                else if (m.Unit.UnitFamily == to)
                {
                    return new Measurement(
                        from.Units.First(e => e.Ratio == 1.0),
                        m.Amount * (1/ratio) * m.Unit.Ratio
                    );
                }
            }

            return null;
        }
    }
}
