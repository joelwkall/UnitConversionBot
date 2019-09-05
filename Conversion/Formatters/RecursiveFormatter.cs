using System;
using System.Collections.Generic;
using System.Text;
using Conversion.Model;

namespace Conversion.Formatters
{
    public class RecursiveFormatter : BaseFormatter
    {
        private BaseFormatter _childFormatter;
        private Func<Measurement, bool> _canFormatFunc;
        private Unit _childUnit;

        public RecursiveFormatter(BaseFormatter childFormatter, Func<Measurement, bool> canFormatFunc, Unit childUnit)
        {
            _childFormatter = childFormatter;
            _canFormatFunc = canFormatFunc;
            _childUnit = childUnit;
        }

        public RecursiveFormatter(Func<Measurement, bool> canFormatFunc)
        {
            _canFormatFunc = canFormatFunc;
        }

        public override bool CanFormat(Measurement m)
        {
            return _canFormatFunc(m);
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

            //if there is a child unit and formatter, use it
            if (_childUnit != null && _childFormatter != null)
            {
                var childAmount = decimalPart / _childUnit.Ratio;

                //avoid rounding up to the same
                if (Utils.RoundToSignificantDigits(childAmount, remainingDigits) == 1 / _childUnit.Ratio)
                    return FormatAmount(integerPart + 1, m.Unit);

                //use child formatter for remaining decimals
                return FormatAmount(integerPart, m.Unit) + ", " + _childFormatter.FormatMeasurement(
                           new Measurement(_childUnit, childAmount),
                           remainingDigits);
            }

            //if there is no child, display using fractions
            //TODO: support 1/16, 1/32 etc if there are enough decimals and remaining significant digits
            var denominator = 8;

            var fractions = decimalPart * denominator;

            //avoid rounding up to 1/1
            if (Utils.RoundToSignificantDigits(fractions, remainingDigits) == denominator)
                return FormatAmount(integerPart + 1, m.Unit);

            return integerPart + " " + Utils.RoundToSignificantDigits(fractions, remainingDigits) + "/" + denominator + " " + m.Unit.Plural;
        }
    }
}
