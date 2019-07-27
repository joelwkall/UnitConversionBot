using System;
using System.Linq;
using Conversion.Scanners;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tests.Helpers;

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
                ("This is 5' not 7\" supposed to match.", new int[]{}),
                ("That dude is 5'3\" tall! And again 5'3\"", new[]{63}),
            };

            ScannerUtils.Test(new FeetAndInchesScanner(), strs, (expected, actual) =>
            {
                return actual.Amount == expected;
            });
        }
    }
}
