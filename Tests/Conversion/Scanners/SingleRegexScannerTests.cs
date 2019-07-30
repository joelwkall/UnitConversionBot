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
                }),
                ("3in in depth and 10in wide.", new[]
                {
                    (3.0, "in", 1),
                    (10.0, "in", 1)
                }),
                ("41,000 Kg / 90,000 pounds", new[]
                {
                    (41000.0, "kg", 2),
                    (90000.0, "pound", 1)
                }),
                ("40 celsius, 41 C, 42C, 43 degrees fahrenheit, 44F, 45 kelvin", new[]
                {
                    (40.0, "celsius", 1),
                    (41.0, "C", 2),
                    (42.0, "C", 2),
                    (43.0, "degrees fahrenheit", 2),
                    (44.0, "F", 2),
                    (45.0, "Kelvin", 2)
                })
            };

            ScannerUtils.Test(new SingleRegexScanner(), strs, (expected, actual) =>
            {
                return actual.Amount == expected.Item1 &&
                       (actual.Unit.Singular == expected.Item2 || actual.Unit.Plural == expected.Item2) &&
                       actual.SignificantDigits == expected.Item3;
            });
        }
    }
}
