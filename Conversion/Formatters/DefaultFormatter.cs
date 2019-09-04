using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Conversion.Model;

namespace Conversion.Formatters
{
    public class DefaultFormatter : BaseFormatter
    {
        public override bool CanFormat(Measurement m)
        {
            return true;
        }

        public override string FormatMeasurement(Measurement m, int significantDigits)
        {
            var roundedAmount = Utils.RoundToSignificantDigits(m.Amount, significantDigits);

            return FormatAmount(roundedAmount, m.Unit);
        }
    }
}
