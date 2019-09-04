using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Conversion.Model;

namespace Conversion.Formatters
{
    public abstract class BaseFormatter
    {
        public abstract string FormatMeasurement(Measurement m, int significantDigits);

        public abstract bool CanFormat(Measurement m);

        protected string FormatAmount(double amount, Unit unit)
        {
            string strAmount;
            if (double.IsPositiveInfinity(amount))
            {
                strAmount = "Infinity";
            }
            else
            {
                strAmount = amount.ToString("#.############################", CultureInfo.InvariantCulture);
            }

            return strAmount + " " + (strAmount == "1" ? unit.Singular : unit.Plural);
        }
    }
}
