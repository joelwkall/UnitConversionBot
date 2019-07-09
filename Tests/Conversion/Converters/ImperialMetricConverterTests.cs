using Conversion.Converters;
using Conversion.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Conversion.Converters
{
    [TestClass]
    public class ImperialMetricConverterTests
    {
        [TestMethod]
        public void Tests()
        {
            var measurements = new[]
            {
                (new Measurement(UnitFamily.Mph.GetUnit("mph"), 10), new Measurement(UnitFamily.Kmph.GetUnit("kilometers per hour"), 16.09344)),
                (new Measurement(UnitFamily.Kmph.GetUnit("km/h"), 10), new Measurement(UnitFamily.Mph.GetUnit("miles per hour"), 6.21371192237334)),

                (new Measurement(UnitFamily.Feet.GetUnit("feet"), 10), new Measurement(UnitFamily.Meters.GetUnit("meter"), 3.048)),
                (new Measurement(UnitFamily.Feet.GetUnit("inches"), 10), new Measurement(UnitFamily.Meters.GetUnit("meter"), 0.254)),
                (new Measurement(UnitFamily.Meters.GetUnit("meters"), 10), new Measurement(UnitFamily.Feet.GetUnit("foot"), 32.8083989501312)),
                (new Measurement(UnitFamily.Meters.GetUnit("cm"), 10), new Measurement(UnitFamily.Feet.GetUnit("foot"), 0.328083989501312)),
                (new Measurement(UnitFamily.Meters.GetUnit("millimetres"), 10), new Measurement(UnitFamily.Feet.GetUnit("foot"), 0.0328083989501312)),

                (new Measurement(UnitFamily.USVolumes.GetUnit("gallon"), 5), new Measurement(UnitFamily.Liters.GetUnit("liter"), 18.9270589)),
                (new Measurement(UnitFamily.ImperialVolumes.GetUnit("imperial gallon"), 5), new Measurement(UnitFamily.Liters.GetUnit("liter"), 22.7304594)),
            };

            //TODO: this is duplicate code from ImperialMetricConverterTests
            foreach (var (input, expectedResult) in measurements)
            {
                var result = new ImperialMetricConverter().Convert(input);

                if (expectedResult != null)
                {
                    Assert.IsNotNull(result, $"{input} was not converted correctly. Result was null.");
                    
                    Assert.AreEqual(expectedResult.Amount, result.Amount, 0.00000001, $"{input} was not converted correctly regarding Amount.");
                    Assert.AreEqual(expectedResult.Unit.Singular, result.Unit.Singular, $"{input} was not converted correctly regarding Unit.");
                }
                else
                {
                    Assert.IsNull(expectedResult, input + " was not supposed to be converted.");
                }
            }
        }
    }
}
