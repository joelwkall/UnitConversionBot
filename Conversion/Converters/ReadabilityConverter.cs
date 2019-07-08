using Conversion.Model;
using System.Globalization;
using System.Linq;

namespace Conversion.Converters
{
    /// <summary>
    /// Converts an expression to another expression of the same unit, if it results in a shorter number.
    /// </summary>
    public class ReadabilityConverter : BaseConverter
    {
        public override Measurement Convert(Measurement m)
        {
            //only normal numbers
            if (double.IsSubnormal(m.Amount))
                return null;

            //ignore custom wacky units
            if (m.UnitExpression.Unit == null)
                return null;

            //dont translate stuff that was not converted
            if (m is DetectedMeasurement)
                return null;

            var baseAmount = m.Amount * m.UnitExpression.Ratio;

            var shortest = (expression: m.UnitExpression, amount: m.Amount);

            foreach (var expr in m.UnitExpression.Unit.Expressions)
            {
                var convertedAmount = baseAmount / expr.Ratio;

                if (NumberOfZeroes(convertedAmount) < NumberOfZeroes(shortest.amount))
                    shortest = (expr, convertedAmount);
            }

            //dont convert if not changed
            if (shortest.expression == m.UnitExpression)
                return null;

            return new Measurement(shortest.expression, shortest.amount);
        }

        private int NumberOfZeroes(double value)
        {
            return value.ToString("0.############################", CultureInfo.InvariantCulture).Where(c => c == '0').Count();
        }
    }
}
