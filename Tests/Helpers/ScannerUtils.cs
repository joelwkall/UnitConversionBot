using Conversion.Scanners;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using Conversion.Model;

namespace Tests.Helpers
{
    class ScannerUtils
    {
        public static void Test<T>(BaseScanner scanner, IEnumerable<(string,T[])> cases, Func<T, DetectedMeasurement,bool> validPredicate)
        {
            foreach (var (input, expectedResults) in cases)
            {
                var (_, results) = scanner.FindMeasurements(input);

                if (expectedResults != null)
                {
                    Assert.AreEqual(expectedResults.Length, results.Count(), $"{input} should have given {expectedResults.Length} matches.");

                    foreach (var expected in expectedResults)
                    {
                        var actual = results.FirstOrDefault(a=>validPredicate(expected, a));

                        if (actual == null)
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
