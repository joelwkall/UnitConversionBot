using Conversion.Converters;
using Conversion.Model;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
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
                (new Measurement(Unit.Mph.GetExpression("mph"), 10), new Measurement(Unit.Kmph.GetExpression("kilometers per hour"), 16.09344)),
                (new Measurement(Unit.Kmph.GetExpression("km/h"), 10), new Measurement(Unit.Mph.GetExpression("miles per hour"), 6.21371192237334)),

                (new Measurement(Unit.Feet.GetExpression("feet"), 10), new Measurement(Unit.Meters.GetExpression("meter"), 3.048)),
                (new Measurement(Unit.Feet.GetExpression("inches"), 10), new Measurement(Unit.Meters.GetExpression("meter"), 0.254)),
                (new Measurement(Unit.Meters.GetExpression("meters"), 10), new Measurement(Unit.Feet.GetExpression("foot"), 32.8083989501312)),
                (new Measurement(Unit.Meters.GetExpression("cm"), 10), new Measurement(Unit.Feet.GetExpression("foot"), 0.328083989501312)),
                (new Measurement(Unit.Meters.GetExpression("millimetres"), 10), new Measurement(Unit.Feet.GetExpression("foot"), 0.0328083989501312)),
            };

            foreach (var (input, expectedResult) in measurements)
            {
                var result = new ImperialMetricConverter().Convert(input);

                if (expectedResult != null)
                {
                    Assert.IsNotNull(result, $"{input} was not converted correctly. Result was null.");
                    
                    Assert.AreEqual(expectedResult.Amount, result.Amount, 0.00000001, $"{input} was not converted correctly regarding Amount.");
                    Assert.AreEqual(expectedResult.UnitExpression.Singular, result.UnitExpression.Singular, $"{input} was not converted correctly regarding UnitExpression.");
                }
                else
                {
                    Assert.IsNull(expectedResult, input + " was not supposed to be converted.");
                }
            }
        }
    }
}
