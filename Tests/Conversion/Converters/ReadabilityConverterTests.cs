using Conversion.Converters;
using Conversion.Model;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Conversion.Converters
{
    [TestClass]
    public class ReadabilityConverterTests
    {
        [TestMethod]
        public void Tests()
        {
            var measurements = new[]
            {
                (new Measurement(UnitFamily.Meters.GetUnit("meter"), 0.03), new Measurement(UnitFamily.Meters.GetUnit("centimeter"), 3)),
                (new Measurement(UnitFamily.Meters.GetUnit("centimeter"), 300), new Measurement(UnitFamily.Meters.GetUnit("meter"), 3)),
                (new Measurement(UnitFamily.Meters.GetUnit("meter"), 3500), new Measurement(UnitFamily.Meters.GetUnit("kilometer"), 3.5)),
                (new Measurement(UnitFamily.Meters.GetUnit("meter"), 0.25), new Measurement(UnitFamily.Meters.GetUnit("centimeter"), 25)),
                //TODO: test other units
            };

            foreach (var (input, expectedResult) in measurements)
            {
                var result = new ReadabilityConverter().Convert(input);

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
