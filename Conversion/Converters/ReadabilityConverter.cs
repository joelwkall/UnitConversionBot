using Conversion.Model;
using System.Globalization;
using System.Linq;

namespace Conversion.Converters
{
    /// <summary>
    /// Converts a unit to another unit of the same family, if it results in a shorter number.
    /// </summary>
    public class ReadabilityConverter : BaseConverter
    {
        public override Measurement Convert(Measurement m)
        {
            //only normal numbers
            if (double.IsSubnormal(m.Amount))
                return null;

            //ignore custom wacky units
            if (m.Unit.UnitFamily == null)
                return null;

            //dont translate stuff that was not converted
            if (m is DetectedMeasurement)
                return null;

            var baseAmount = m.Amount * m.Unit.Ratio;

            var shortest = (unit: m.Unit, amount: m.Amount);

            foreach (var u in m.Unit.UnitFamily.Units)
            {
                var convertedAmount = baseAmount / u.Ratio;

                if (NumberOfZeroes(convertedAmount) < NumberOfZeroes(shortest.amount))
                    shortest = (unit: u, convertedAmount);
            }

            //dont convert if not changed
            if (shortest.unit == m.Unit)
                return null;

            return new Measurement(shortest.unit, shortest.amount);
        }

        private int NumberOfZeroes(double value)
        {
            return value.ToString("0.############################", CultureInfo.InvariantCulture).Where(c => c == '0').Count();
        }
    }
}
