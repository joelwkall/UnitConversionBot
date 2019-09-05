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
        [DataRow(757.995, 6, "757 feet, 11 7/8 inches")]
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
    }
}
