using System.Linq;
using Conversion;
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
                (new DetectedMeasurement(UnitFamily.Mph.GetUnit("mph"), 10), new[]{new Measurement(UnitFamily.Kmph.GetUnit("kilometres per hour"), 16.09344)}),
                (new DetectedMeasurement(UnitFamily.Kmph.GetUnit("km/h"), 10), new[]{new Measurement(UnitFamily.Mph.GetUnit("miles per hour"), 6.21371192237334)}),

                (new DetectedMeasurement(UnitFamily.ImperialDistances.GetUnit("feet"), 10), new[]{new Measurement(UnitFamily.Meters.GetUnit("metre"), 3.048)}),
                (new DetectedMeasurement(UnitFamily.ImperialDistances.GetUnit("inches"), 10), new[]{new Measurement(UnitFamily.Meters.GetUnit("metre"), 0.254)}),
                (new DetectedMeasurement(UnitFamily.Meters.GetUnit("meters"), 10), new[]{new Measurement(UnitFamily.ImperialDistances.GetUnit("foot"), 32.8083989501312)}),
                (new DetectedMeasurement(UnitFamily.Meters.GetUnit("cm"), 10), new[]{new Measurement(UnitFamily.ImperialDistances.GetUnit("foot"), 0.328083989501312)}),
                (new DetectedMeasurement(UnitFamily.Meters.GetUnit("millimetres"), 10), new[]{new Measurement(UnitFamily.ImperialDistances.GetUnit("foot"), 0.0328083989501312)}),
                (new DetectedMeasurement(UnitFamily.Meters.GetUnit("kilometers"), 10), new[]{new Measurement(UnitFamily.ImperialDistances.GetUnit("foot"), 32808.3989501312)}),
                (new DetectedMeasurement(UnitFamily.Meters.GetUnit("lightyear"), 5), new[]{new Measurement(UnitFamily.ImperialDistances.GetUnit("foot"), 155192395000000000) }),

                (new DetectedMeasurement(UnitFamily.USVolumes.GetUnit("gallon"), 5), new[]
                {
                    new Measurement(UnitFamily.Liters.GetUnit("liter"), 18.9270589),
                    new Measurement(UnitFamily.ImperialVolumes.GetUnit("imperial gallon"), 4.1633692)
                }),
                (new DetectedMeasurement(UnitFamily.ImperialVolumes.GetUnit("imperial gallon"), 5), new[]
                {
                    new Measurement(UnitFamily.Liters.GetUnit("liter"), 22.7304594),
                    new Measurement(UnitFamily.USVolumes.GetUnit("us gallon"), 6.004752113)
                }),

                (new DetectedMeasurement(UnitFamily.Stones.GetUnit("stone"), 5), new[]
                {
                    new Measurement(UnitFamily.Kilograms.GetUnit("kilogram"), 31.7514659),
                    new Measurement(UnitFamily.Pounds.GetUnit("pound"), 70)
                })
            };
            
            foreach (var (input, expectedResults) in measurements)
            {
                var collection = new ConversionCollection(input);
                new ImperialMetricConverter().Convert(collection);
                var results = collection.ConvertedMeasurements.Select(m => m.ToString(10)).ToList();

                Assert.AreEqual(expectedResults.Length, results.Count(), $"Expected results [{string.Join(',', expectedResults.Select(m => m.ToString(10)))}], but got [{string.Join(',', results)}]");

                foreach (var expectedResult in expectedResults.Select(m => m.ToString(10)))
                {
                    Assert.IsTrue(results.Contains(expectedResult), $"Results did not contain {expectedResult}. Results were: [{string.Join(',', results)}]");
                }
            }
        }
    }
}
