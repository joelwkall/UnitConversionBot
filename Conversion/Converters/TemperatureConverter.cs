using Conversion.Model;
using System;
using System.Collections.Generic;

namespace Conversion.Converters
{
    public class TemperatureConverter : BaseConverter
    {
        public override IEnumerable<Measurement> Convert(Measurement m)
        {
            if (m.Unit.UnitFamily == UnitFamily.Fahrenheit)
            {
                yield return new Measurement(UnitFamily.Celcius.PrimaryUnit, (m.Amount-32)/1.8);
                yield return new Measurement(UnitFamily.Kelvin.PrimaryUnit, (m.Amount - 32) / 1.8 + 273.15);
            }
            else if (m.Unit.UnitFamily == UnitFamily.Celcius)
            {
                yield return new Measurement(UnitFamily.Fahrenheit.PrimaryUnit, (m.Amount * 1.8) + 32.0);
                yield return new Measurement(UnitFamily.Kelvin.PrimaryUnit, m.Amount + 273.15);
            }
            else if (m.Unit.UnitFamily == UnitFamily.Kelvin)
            {
                yield return new Measurement(UnitFamily.Celcius.PrimaryUnit, m.Amount - 273.15);
                yield return new Measurement(UnitFamily.Fahrenheit.PrimaryUnit, ((m.Amount - 273.15) * 1.8) + 32.0);
            }
        }
    }
}
