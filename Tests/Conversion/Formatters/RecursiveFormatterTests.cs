using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Conversion.Formatters;
using Conversion.Model;
using Conversion;

namespace Tests.Conversion.Formatters
{
    [TestClass]
    public class RecursiveFormatterTests
    {
        [TestMethod]

        [DataRow(7.0, 15, "7 feet")]
        [DataRow(7.5, 2, "7 feet, 6 inches")]
        [DataRow(7.5, 1, "8 feet")]
        [DataRow(7.67, 1, "8 feet")]
        [DataRow(757.67, 3, "758 feet")]
        [DataRow(757.67, 4, "757 feet, 8 inches")]
        [DataRow(757.91, 5, "757 feet, 11 inches")]
        [DataRow(757.98, 4, "758 feet")] //make sure we dont get 758 feet, 12 inches
        [DataRow(757.98, 6, "757 feet, 11 6/8 inches")]
        [DataRow(757.992, 6, "757 feet, 11 7/8 inches")]
        [DataRow(757.9501, 5, "757 feet, 11 inches")]
        public void Feet(double amount, int digits, string expected)
        {
            var formatter = TextAnalyzer.FootFormatter;
            var formatted =
                formatter.FormatMeasurement(new Measurement(UnitFamily.ImperialDistances.PrimaryUnit, amount), digits);

            Assert.AreEqual(expected, formatted);

            var negativeFormatted =
                formatter.FormatMeasurement(new Measurement(UnitFamily.ImperialDistances.PrimaryUnit, -amount), digits);

            Assert.AreEqual("-" + expected, negativeFormatted, "Negative formatting failed.");
        }

        [TestMethod]
        [DataRow(7.0, 15, "7 pounds")]
        [DataRow(7.5, 2, "7 pounds, 8 ounces")]
        [DataRow(7.5, 1, "8 pounds")]
        [DataRow(7.67, 1, "8 pounds")]
        [DataRow(757.67, 3, "758 pounds")]
        [DataRow(757.67, 4, "757 pounds, 10 ounces")] //TODO: this should be 11 ounces but we run out of sigdigs
        [DataRow(757.91, 5, "757 pounds, 15 ounces")]
        [DataRow(757.98, 4, "758 pounds")] //make sure we dont get 758 pounds, 16 ounces
        [DataRow(757.98, 6, "757 pounds, 15 5/8 ounces")]
        [DataRow(757.995, 6, "757 pounds, 15 7/8 ounces")]
        [DataRow(757.9501, 5, "757 pounds, 15 ounces")]
        public void Pounds(double amount, int digits, string expected)
        {
            var formatter = TextAnalyzer.PoundFormatter;
            var formatted =
                formatter.FormatMeasurement(new Measurement(UnitFamily.Pounds.PrimaryUnit, amount), digits);

            Assert.AreEqual(expected, formatted);

            var negativeFormatted =
                formatter.FormatMeasurement(new Measurement(UnitFamily.Pounds.PrimaryUnit, -amount), digits);

            Assert.AreEqual("-" + expected, negativeFormatted, "Negative formatting failed.");
        }

        [TestMethod]
        [DataRow(7.0, 15, "7 stone")]
        [DataRow(7.5, 2, "7 stone, 7 pounds")]
        [DataRow(7.5, 1, "8 stone")]
        [DataRow(7.67, 1, "8 stone")]
        [DataRow(757.67, 3, "758 stone")]
        [DataRow(757.67, 4, "757 stone, 9 pounds")]
        [DataRow(757.91, 5, "757 stone, 13 pounds")]
        [DataRow(757.98, 4, "758 stone")] //make sure we dont get 758 stone, 14 pounds
        [DataRow(757.98, 6, "757 stone, 13 pounds, 10 ounces")] //TODO: should be 12 ounces but we ran out of sigdigs
        [DataRow(757.98, 7, "757 stone, 13 pounds, 12 ounces")]
        [DataRow(757.995, 8, "757 stone, 13 pounds, 14 7/8 ounces")]
        [DataRow(757.9501, 5, "757 stone, 13 pounds")]
        public void Stones(double amount, int digits, string expected)
        {
            var formatter = TextAnalyzer.StonesFormatter;
            var formatted =
                formatter.FormatMeasurement(new Measurement(UnitFamily.Stones.PrimaryUnit, amount), digits);

            Assert.AreEqual(expected, formatted);

            var negativeFormatted =
                formatter.FormatMeasurement(new Measurement(UnitFamily.Stones.PrimaryUnit, -amount), digits);

            Assert.AreEqual("-" + expected, negativeFormatted, "Negative formatting failed.");
        }
    }
}
