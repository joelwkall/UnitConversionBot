using System;
using System.Linq;
using Conversion.Scanners;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Conversion.Scanners
{
    [TestClass]
    public class FeetAndInchesScannerTests
    {
        [TestMethod]
        public void Tests()
        {
            var strs = new[]
            {
                ("That dude is 5'2\" tall!", new[]{62}),
                ("I don't think you can fit 12' 15\" in that house. The ceiling is only 11'5\"!", new[]{159, 137}),
                ("This is 5' not 7\" supposed to match.", new int[]{})
            };

            //TODO: this is very similar to SingleRegexScannerTests
            foreach (var (input, expectedResults) in strs)
            {
                var (_,results) = new FeetAndInchesScanner().FindMeasurements(input);

                if (expectedResults != null)
                {
                    Assert.AreEqual(expectedResults.Length, results.Count(), $"{input} should have given {expectedResults.Length} matches.");

                    foreach (var expected in expectedResults)
                    {
                        var actual = results.FirstOrDefault(f =>
                        {
                            return f.Amount == expected;
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
