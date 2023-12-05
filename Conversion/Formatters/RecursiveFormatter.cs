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
        private double _childRatio;

        public RecursiveFormatter(BaseFormatter childFormatter, Func<Measurement, bool> canFormatFunc, Unit childUnit, double childRatio)
        {
            _childFormatter = childFormatter;
            _canFormatFunc = canFormatFunc;
            _childUnit = childUnit;
            _childRatio = childRatio;
        }

        public RecursiveFormatter(BaseFormatter childFormatter, Func<Measurement, bool> canFormatFunc, Unit childUnit)
            :this(childFormatter, canFormatFunc, childUnit, childUnit.Ratio)
        {
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

            //if it was rounded up, there is no fractions or child unit to add
            if (Math.Abs(integerPart) > Math.Abs(m.Amount))
                return FormatAmount(integerPart, m.Unit);

            var integerPartDigits = Utils.CountDigits(integerPart);
            if (integerPart == 0)
                integerPartDigits = 0;

            //did we deplete the specified significant digits?
            if (integerPartDigits >= significantDigits)
                return FormatAmount(rounded, m.Unit);

            //calculate child part
            var decimalPart = Math.Abs(m.Amount) - Math.Abs(integerPart);

            if (decimalPart <= 0)
                return FormatAmount(integerPart, m.Unit);

            var remainingDigits = Math.Max(0, significantDigits - integerPartDigits);

            //if there is a child unit and formatter, use it
            if (_childUnit != null && _childFormatter != null)
            {
                var childAmount = decimalPart / _childRatio;

                var childFormatted = _childFormatter.FormatMeasurement(
                    new Measurement(_childUnit, childAmount),
                    remainingDigits);

                //avoid rounding up to a "full" child
                if (childFormatted.StartsWith((1 / _childRatio).ToString()))
                    return FormatAmount(integerPart + 1, m.Unit);

                //use child formatter for remaining decimals
                if (integerPart == 0)
                    return childFormatted;

                return FormatAmount(integerPart, m.Unit) + ", " + childFormatted;
            }

            //if there is no child, display using fractions
            //TODO: support 1/16, 1/32 etc if there are enough decimals and remaining significant digits
            //for now, just round it to nearest integer
            var denominator = 8;
            var fractions = Math.Round(decimalPart * denominator);

            //avoid rounding up to 1/1
            if (fractions == denominator)
                return FormatAmount(integerPart + 1, m.Unit);

            return integerPart + " " + fractions + "/" + denominator + " " + m.Unit.Plural;
        }
    }
}
