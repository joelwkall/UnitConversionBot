using System;
using System.Collections.Generic;
using System.Text;
using Conversion.Model;

namespace Conversion.Formatters
{
    public class InchesFormatter : BaseFormatter
    {
        public override bool CanFormat(Measurement m)
        {
            return m.Unit.UnitFamily == UnitFamily.ImperialDistances && m.Unit.Ratio== 1.0 / 12; //only format inches
        }

        public override string FormatMeasurement(Measurement m, int significantDigits)
        {
            //TODO: this is very duplicated from FootFormatter
            var rounded = Utils.RoundToSignificantDigits(m.Amount, significantDigits);

            var integerPart = (int)Math.Truncate(rounded);
            var integerPartDigits = Utils.CountDigits(integerPart);

            var formatted = FormatAmount(integerPart, m.Unit);

            //did we deplete the specified significant digits?
            if (integerPartDigits >= significantDigits)
                return formatted;

            //calculate fractional part
            var decimalPart = Math.Abs(rounded) - Math.Abs(integerPart);

            if (decimalPart == 0)
                return formatted;

            
            var remainingDigits = Math.Max(0, significantDigits - integerPartDigits);

            //TODO: support 1/16, 1/32 etc if there are enough decimals and remaining significant digits
            //for now, just hard code 1 remaining sigdig
            remainingDigits = 1;

            var eights = decimalPart * 8;

            //avoid rounding up to 8
            if (Utils.RoundToSignificantDigits(eights, remainingDigits) == 8)
                return FormatAmount(integerPart + 1, m.Unit);

            return integerPart + " " + Utils.RoundToSignificantDigits(eights, remainingDigits) + "/8 inches";
        }
    }
}
