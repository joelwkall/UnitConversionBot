using System.Collections.Generic;
using Conversion;
using Conversion.Converters;
using Conversion.Model;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

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
                (new Measurement(UnitFamily.Meters.GetUnit("meter"), 0.03), new Measurement(UnitFamily.Meters.GetUnit("centimetre"), 3)),
                (new Measurement(UnitFamily.Meters.GetUnit("centimeter"), 300), new Measurement(UnitFamily.Meters.GetUnit("metre"), 3)),
                (new Measurement(UnitFamily.Meters.GetUnit("meter"), 3500), new Measurement(UnitFamily.Meters.GetUnit("kilometre"), 3.5)),
                (new Measurement(UnitFamily.Meters.GetUnit("meter"), 0.25), new Measurement(UnitFamily.Meters.GetUnit("centimetre"), 25)),
                //TODO: test other units
            };

            foreach (var (input, expectedResult) in measurements)
            {
                var collection = new ConversionCollection(null, new List<Measurement>(){input});
                new ReadabilityConverter().Convert(collection);
                var results = collection.ConvertedMeasurements.Select(m => m.ToString(10)).ToList();

                if (expectedResult != null)
                {
                    Assert.IsTrue(results.Contains(expectedResult.ToString(10)), $"Results did not contain {expectedResult}. Results were: [{string.Join(',', results)}]");
                }
                else
                {
                    Assert.IsNull(expectedResult, input + " was not supposed to be converted.");
                }
            }
        }
    }
}
