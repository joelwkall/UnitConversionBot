using Microsoft.VisualStudio.TestTools.UnitTesting;
using Conversion.Model;
using Conversion.Filters;
using static Conversion.Model.UnitFamily;

namespace Tests.Conversion.Filters
{
    [TestClass]
    public class DuplicatesFilterTests
    {
        [TestMethod]
        public void RepresentsTheSame()
        {
            var tests = new[]
            {
                (new Measurement(Meters.GetUnit("m"), 10),new Measurement(ImperialDistances.GetUnit("ft"), 10), false),
                (new Measurement(Meters.GetUnit("m"), 10),new Measurement(Meters.GetUnit("m"), 10.5), true),
                (new Measurement(Meters.GetUnit("m"), 10),new Measurement(Meters.GetUnit("cm"), 1050), true),
                (new Measurement(Meters.GetUnit("m"), 10),new Measurement(Meters.GetUnit("cm"), 953), true),
                (new Measurement(Meters.GetUnit("m"), 10),new Measurement(Meters.GetUnit("m"), 10.51), false),
                (new Measurement(Meters.GetUnit("m"), 10),new Measurement(Meters.GetUnit("cm"), 952), false),
                (new Measurement(ImperialDistances.GetUnit("ft"), 10),new Measurement(ImperialDistances.GetUnit("in"), 126), true),
                (new Measurement(ImperialDistances.GetUnit("ft"), 10),new Measurement(ImperialDistances.GetUnit("in"), 127), false),
            };

            foreach (var (a, b, expected) in tests)
            {
                Assert.AreEqual(DuplicatesFilter.RepresentsTheSame(a, b), expected, $"Wrong result with measurements {a} and {b}.");
            }
        }
    }
}
