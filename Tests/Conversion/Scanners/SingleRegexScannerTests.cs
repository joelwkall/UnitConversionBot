using System;
using System.Linq;
using Conversion.Scanners;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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

            foreach (var (input, expectedResults) in strs)
            {
                var (_,results) = new SingleRegexScanner().FindMeasurements(input);

                if (expectedResults != null)
                {
                    Assert.AreEqual(expectedResults.Length, results.Count(), $"{input} should have given {expectedResults.Length} matches.");

                    foreach (var expected in expectedResults)
                    {
                        var actual = results.FirstOrDefault(f =>
                        {
                            return f.Amount == expected.Item1 &&
                                   f.Unit.Singular == expected.Item2 &&
                                   f.SignificantDigits == expected.Item3;
                        });

                        if(actual==null)
                            throw new Exception($"Could not find expected conversion {expected}");
                    }
                }
                else
                {
                    Assert.AreEqual(0, results.Count(), input + " was not supposed to match.");
                }
            }
        }
    }
}
