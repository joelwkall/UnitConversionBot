using Conversion.Model;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Conversion.Converters
{
    /// <summary>
    /// Converts a unit to another unit of the same family, if it results in a shorter number.
    /// </summary>
    public class ReadabilityConverter : BaseConverter
    {
        public override IEnumerable<Measurement> Convert(Measurement m)
        {
            //only normal numbers
            if (double.IsSubnormal(m.Amount))
                yield break;

            //ignore custom wacky units
            if (m.Unit.UnitFamily == null)
                yield break;

            //dont translate stuff that was not converted
            if (m is DetectedMeasurement)
                yield break;

            var baseAmount = m.Amount * m.Unit.Ratio;

            var preferred = (unit: m.Unit, amount: m.Amount, numberOfZeroes: Utils.CountLeadingTrailingZeroes(m.Amount));

            foreach (var u in m.Unit.UnitFamily.Units)
            {
                var convertedAmount = baseAmount / u.Ratio;

                //if the new number has fewer zeroes
                //or the same number of zeroes but a smaller number
                //then the new unit is preferred
                var numberOfZeroesConverted = Utils.CountLeadingTrailingZeroes(convertedAmount);

                if (numberOfZeroesConverted < preferred.numberOfZeroes ||
                    numberOfZeroesConverted == preferred.numberOfZeroes && convertedAmount < preferred.amount)
                    preferred = (u, convertedAmount, numberOfZeroesConverted);
            }

            //dont convert if not changed
            if (preferred.unit == m.Unit)
                yield break;

            yield return new Measurement(preferred.unit, preferred.amount);
        }

        
    }
}
