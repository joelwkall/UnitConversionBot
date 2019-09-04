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
            string strAmount;
            if (double.IsPositiveInfinity(m.Amount))
            {
                strAmount = "Infinity";
            }
            else
            {
                var roundedAmount = Utils.RoundToSignificantDigits(m.Amount, significantDigits);

                strAmount = roundedAmount.ToString("#.############################", CultureInfo.InvariantCulture);
            }

            return strAmount + " " + (strAmount == "1" ? m.Unit.Singular : m.Unit.Plural);
        }
    }
}
