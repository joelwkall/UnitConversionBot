using System;
using System.Linq;
using Conversion.Scanners;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tests.Helpers;

namespace Tests.Conversion.Scanners
{
    [TestClass]
    public class FeetOrInchesScannerTests
    {
        [TestMethod]
        public void Tests()
        {
            var strs = new[]
            {
                ("usually 15'-16' average", new[]{15,16}),
                ("I have a 5' table", new[]{5}),
                ("2016 to 2018.\"", new int[]{}),
            };

            ScannerUtils.Test(new FeetOrInchesScanner(), strs, (expected, actual) =>
            {
                return actual.Amount == expected;
            });
        }
    }
}
