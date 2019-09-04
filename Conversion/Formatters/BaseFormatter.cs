using System;
using System.Collections.Generic;
using System.Text;
using Conversion.Model;

namespace Conversion.Formatters
{
    public abstract class BaseFormatter
    {
        public abstract string FormatMeasurement(Measurement m, int significantDigits);

        public abstract bool CanFormat(Measurement m);
    }
}
