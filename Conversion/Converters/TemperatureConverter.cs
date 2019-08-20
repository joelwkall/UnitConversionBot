using System.Linq;
using Conversion.Model;

namespace Conversion.Converters
{
    public class TemperatureConverter : BaseConverter
    {
        public override void Convert(ConversionCollection collection)
        {
            foreach (var m in collection.AllValidMeasurements.ToList())
            {
                if (m.Unit.UnitFamily == UnitFamily.Fahrenheit)
                {
                    collection.ConvertedMeasurements.Add(new Measurement(UnitFamily.Celcius.PrimaryUnit, (m.Amount - 32) / 1.8));
                    collection.ConvertedMeasurements.Add(new Measurement(UnitFamily.Kelvin.PrimaryUnit, (m.Amount - 32) / 1.8 + 273.15));
                }
                else if (m.Unit.UnitFamily == UnitFamily.Celcius)
                {
                    collection.ConvertedMeasurements.Add(new Measurement(UnitFamily.Fahrenheit.PrimaryUnit, (m.Amount * 1.8) + 32.0));
                    collection.ConvertedMeasurements.Add(new Measurement(UnitFamily.Kelvin.PrimaryUnit, m.Amount + 273.15));
                }
                else if (m.Unit.UnitFamily == UnitFamily.Kelvin)
                {
                    collection.ConvertedMeasurements.Add(new Measurement(UnitFamily.Celcius.PrimaryUnit, m.Amount - 273.15));
                    collection.ConvertedMeasurements.Add(new Measurement(UnitFamily.Fahrenheit.PrimaryUnit, ((m.Amount - 273.15) * 1.8) + 32.0));
                }
            }
        }
    }
}
