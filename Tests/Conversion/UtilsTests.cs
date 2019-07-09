using Conversion;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Conversion
{
    [TestClass]
    public class UtilsTests
    {
        [TestMethod]
        public void DetectSignificantDigits()
        {
            var cases = new[]
            {
                ("1000", 1),
                ("12", 2),
                ("128", 3),
                ("1280", 3),
                ("12800", 3),
                ("128001", 6),
                ("000128001000", 6),
                ("00012800100035", 11),
                ("0128", 3),

                ("128.1", 4),
                ("00128.1", 4),
                ("00128.10", 4),
                ("00128.101", 6),
                ("00128.10001", 8),
                ("00128.10007001", 11),
                ("00128.1000766001", 13),
            };

            foreach (var (str, expected) in cases)
            {
                var actual = Utils.DetectSignificantDigits(str);

                Assert.AreEqual(expected, actual, str + " wasnt detected correctly.");
            }
        }

        [TestMethod]
        public void CountLeadingTrailingZeroes()
        {
            var cases = new[]
            {
                (1000, 3),
                (12, 0),
                (128, 0),
                (1280, 1),
                (12800, 2),
                (128001, 0),
                (128001000, 3),
                (12800100035, 0),
                (128, 0),

                (128.1, 0),
                (128.101, 0),
                (1280.10001, 0),
                (128.10007001, 0),
                (0.000766001, 4),
                (0.0, 1),
                (1.0, 0),
            };

            foreach (var (d, expected) in cases)
            {
                var actual = Utils.CountLeadingTrailingZeroes(d);

                Assert.AreEqual(expected, actual, d + " wasnt counted correctly.");
            }
        }
    }
}
