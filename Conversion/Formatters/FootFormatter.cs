using System;
using System.Collections.Generic;
using System.Text;
using Conversion.Model;

namespace Conversion.Formatters
{
    public class FootFormatter : BaseFormatter
    {
        private InchesFormatter InchesFormatter;

        public FootFormatter(InchesFormatter inchesFormatter)
        {
            InchesFormatter = inchesFormatter;
        }

        public override bool CanFormat(Measurement m)
        {
            return m.Unit.UnitFamily == UnitFamily.ImperialDistances && m.Unit.Ratio==1; //only format feet
        }

        public override string FormatMeasurement(Measurement m, int significantDigits)
        {
            var rounded = Utils.RoundToSignificantDigits(m.Amount, significantDigits);

            var integerPart = (int)Math.Truncate(rounded);
            var integerPartDigits = Utils.CountDigits(integerPart);

            //did we deplete the specified significant digits?
            if (integerPartDigits >= significantDigits)
                return FormatAmount(integerPart, m.Unit);

            //calculate inches part
            var decimalPart = Math.Abs(rounded) - Math.Abs(integerPart);

            if (decimalPart == 0)
                return FormatAmount(integerPart, m.Unit);

            var remainingDigits = Math.Max(0, significantDigits - integerPartDigits);
            var inches = decimalPart * 12;

            //avoid rounding up to 12
            if (Utils.RoundToSignificantDigits(inches, remainingDigits) == 12)
                return FormatAmount(integerPart + 1, m.Unit);

            return FormatAmount(integerPart, m.Unit) + ", " + InchesFormatter.FormatMeasurement(
                       new Measurement(UnitFamily.ImperialDistances.GetUnit("inch"), inches), 
                       remainingDigits);
        }
    }
}
