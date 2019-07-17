using System;
using System.Linq;
using Conversion.Scanners;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tests.Helpers;

namespace Tests.Conversion.Scanners
{
    [TestClass]
    public class SingleRegexScannerTests
    {
        [TestMethod]
        public void Tests()
        {
            var strs = new[]
            {
                ("This car goes 10 mph.", new[]
                {
                    (10.0, "mph", 1)
                }),
                ("This car goes 10 mph, and my goat goes 2.5 km/h.", new[]
                {
                    (10.0, "mph", 1),
                    (2.5, "km/h", 2)
                }),
                ("Does it handle the .55 lbs format?", new[]
                {
                    (0.55, "lb", 2)
                })
            };

            ScannerUtils.Test(new SingleRegexScanner(), strs, (expected, actual) =>
            {
                return actual.Amount == expected.Item1 &&
                       actual.Unit.Singular == expected.Item2 &&
                       actual.SignificantDigits == expected.Item3;
            });
        }
    }
}
