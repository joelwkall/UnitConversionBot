using Conversion.Model;
using System.Collections.Generic;
using System.Linq;

namespace Conversion.Converters
{
    public class ImperialMetricConverter : BaseConverter
    {
        private static List<(Unit, double, Unit)> Conversions = new List<(Unit, double, Unit)>()
        {
            (Unit.Mph, 1.609344, Unit.Kmph),
            (Unit.Feet, 0.3048, Unit.Meters),
            (Unit.Pounds, 0.45359237, Unit.Kilograms)
        };

        public override Measurement Convert(Measurement m)
        {
            foreach (var (from, ratio, to) in Conversions)
            {
                if (m.UnitExpression.Unit == from)
                {
                    return new Measurement(
                        to.Expressions.First(e => e.Ratio == 1.0),
                        m.Amount * ratio * m.UnitExpression.Ratio
                    );
                }
                else if (m.UnitExpression.Unit == to)
                {
                    return new Measurement(
                        from.Expressions.First(e => e.Ratio == 1.0),
                        m.Amount * (1/ratio) * m.UnitExpression.Ratio
                    );
                }
            }

            return null;
        }
    }
}
