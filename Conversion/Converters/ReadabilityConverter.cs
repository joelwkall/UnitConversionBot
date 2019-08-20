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
        public override void Convert(ConversionCollection collection)
        {
            foreach (var m in collection.ConvertedMeasurements.ToList())
            {
                //only normal numbers
                if (double.IsSubnormal(m.Amount))
                    continue;

                //ignore custom wacky units
                if (m.Unit.UnitFamily == null)
                    continue;

                //dont translate stuff that was not converted
                if (m is DetectedMeasurement)
                    continue;

                var baseAmount = m.Amount * m.Unit.Ratio;

                var preferred = (unit: m.Unit, amount: m.Amount,
                    numberOfZeroes: Utils.CountLeadingTrailingZeroes(m.Amount));

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
                    continue;

                //replace the old with the new
                collection.ConvertedMeasurements.Remove(m);
                collection.ConvertedMeasurements.Add(new Measurement(preferred.unit, preferred.amount));
            }

        }
    }
}
