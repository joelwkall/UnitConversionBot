using System.Linq;
using Conversion;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Conversion
{
    [TestClass]
    public class TextAnalyzerTests
    {
        private TextAnalyzer _analyzer = TextAnalyzer.Default;

        [TestMethod]
        public void BeginningAndEnd()
        {
            var str = "12 lbs is the most thing close to 18 mph";

            var results = _analyzer.FindConversions(str);

            CollectionAssert.AreEquivalent(new[]{ "12 lbs ≈ 5.44 kilograms or .857 stone", "18 mph ≈ 29 kilometres per hour" }, results.ToList(), "Results were " + string.Join(", ",results));
        }

        [TestMethod]
        public void InputDecimalComma()
        {
            var str = "This thing weighs 15,88 lbs";

            var results = _analyzer.FindConversions(str);

            Assert.AreEqual(1, results.Count());
            Assert.AreEqual("15,88 lbs ≈ 7.203 kilograms or 1.1343 stone", results.First());
        }

        [TestMethod]
        public void InputDecimalPoint()
        {
            var str = "This thing weighs 15.88 lbs";

            var results = _analyzer.FindConversions(str);

            Assert.AreEqual(1, results.Count());
            Assert.AreEqual("15.88 lbs ≈ 7.203 kilograms or 1.1343 stone", results.First());
        }

        [TestMethod]
        public void OutputDecimal()
        {
            var str = "This thing goes 150 kph";

            var results = _analyzer.FindConversions(str);

            Assert.AreEqual(1, results.Count());
            Assert.AreEqual("150 kph ≈ 93.2 miles per hour", results.First());
        }

        [DataTestMethod]
        [DataRow("It works with regular 90 pound spaces between 12 meter things.", 2)]
        [DataRow("It works with (90 pound parenthesis 12 meter) too.", 2)]
        [DataRow("The smallest subspecies of the reindeer. Males average 90 kg in weight, females 70 kg or so", 2)]
        [DataRow("For reference here, the chinook is barely 40 feet in the air, " +
                 "but it produced enough wind to send that half panel flying. That " +
                 "panel itself is 12 feet across and weighs upwards of a 100 pounds. " +
                 "This is to let you all know to be careful when dealing with " +
                 "helicopters. I was to the left of the person filming this.", 3)]
        [DataRow("I started the oven opening which is 19in wide and 11in in height. " +
                 "The vent opening will be 3in in depth and 10in wide.", 4)]
        [DataRow("5 foot wingspan through an 8 inch gap.", 2)]
        [DataRow("The smallest subspecies of the reindeer. Males average 90 kg in weight, females 70 kg", 2)]
        [DataRow("It also works with volumes such as 3 gallons, 5 liters, and 5 fl oz.", 3)]
        [DataRow("usually 15'-16' average", 2)]
        [DataRow("And a 10 km Run (6.2 miles).", 0)] //duplicate measurement
        [DataRow("And a 10 km Run (not at all 5.2 miles).", 2)] //outside duplicate threshhold
        [DataRow("60 mph. (97 kph)", 0)] //duplicate measurement
        [DataRow("60 mph is definitely not equal to 120 kph", 2)] //outside duplicate threshhold
        [DataRow("With the Apollo Space Capsule on top, it was 111 meters / 364 ft tall and " +
                 "without a stabilizer was 10 meters / 33 ft in diameter. Fully fueled, " +
                 "the Saturn V had a weight of 2,950 tons and had a load-carrying capacity " +
                 "in a low Earth’s orbit, originally estimated at 118,000 kg / 260,000 pounds, " +
                 "but designed to ship at least 41,000 Kg / 90,000 pounds to Moon. " +
                 "Later upgrades increased this capacity…", 0)] //all duplicates
        public void ConvertMultiple(string str, int expected)
        {
            var results = _analyzer.FindConversions(str);

            Assert.AreEqual(expected, results.Count(), $"Results were: {string.Join(',',results)}");
        }

        [TestMethod]
        public void NoCascades()
        {
            var strings =new[]{"3 inches", "4 km/h"};

            foreach (var str in strings)
            {
                var results = _analyzer.FindConversions(str);

                Assert.AreEqual(1, results.Count(), str + " caused the wrong number of results.");
            }
        }

        [DataTestMethod]
        //spaces and plurals
        [DataRow("10 foot", "10 foot ≈ 3 metres")]
        [DataRow("10 feet", "10 feet ≈ 3 metres")]
        [DataRow("10'", "10' ≈ 3 metres")]
        [DataRow("10 '", null)]
        [DataRow("10's", null)]
        [DataRow("10 inches", "10 inches ≈ 25 centimetres")]
        [DataRow("10 inch", "10 inch ≈ 25 centimetres")]
        [DataRow("10in", "10in ≈ 25 centimetres")]
        [DataRow("10\"", "10\" ≈ 25 centimetres")]
        [DataRow("10 \"", null)]
        [DataRow("10\"s", null)]
        [DataRow("10 in", null)]
        [DataRow("10ins", null)]

        //TODO: copy the above to lbs, since ' and " are special scanners

        //line breaks
        [DataRow("something \n450 lbs down", "450 lbs ≈ 204 kilograms or 32.1 stone")]
        [DataRow("something \r450 lbs down", "450 lbs ≈ 204 kilograms or 32.1 stone")]
        [DataRow("something <br/>450 lbs down", "450 lbs ≈ 204 kilograms or 32.1 stone")]
        [DataRow("450 lbs down\n", "450 lbs ≈ 204 kilograms or 32.1 stone")]
        [DataRow("450 lbs down\r", "450 lbs ≈ 204 kilograms or 32.1 stone")]
        [DataRow("450 lbs down<br/>", "450 lbs ≈ 204 kilograms or 32.1 stone")]

        //special chars
        [DataRow("of reaching 185 lbs, however", "185 lbs ≈ 83.91 kilograms or 13.21 stone")]
        [DataRow("God imgurs at 5gb's.", null)]
        [DataRow("Does it handle the .55 lbs format ?", ".55 lbs ≈ .0393 stone or 249 grams")]
        [DataRow("It should handle heights like 6'10\"", "6'10\" ≈ 2.08 metres")]

        //novelty stuff
        [DataRow("I added a banana for scale just in case. And banana again.", "1 banana ≈ 18 centimeters or 7 inches")]
        //TODO: test puppy conversions using a 1.0 threshhold noveltyconverter

        //converter interactions
        [DataRow("120 meters should not be converted to inches", "120 meters ≈ 394 feet")]
        [DataRow("Do not convert 10 kilometers to feet", "10 kilometers ≈ 6.2 miles")]
        [DataRow("It was 100 degrees F yesterday", "100 degrees F ≈ 38 ° Celsius or 310 Kelvin")]
        [DataRow("It was 45C yesterday", "45C ≈ 113 ° Fahrenheit or 318 Kelvin")]
        [DataRow("Liquid nitrogen is 77 Kelvin or colder", "77 Kelvin ≈ -196 ° Celsius or -321 ° Fahrenheit")]
        [DataRow("It was -20C yesterday", "-20C ≈ 68 ° Fahrenheit or 290 Kelvin")] //minus sign works
        [DataRow("-20C yesterday", "-20C ≈ 68 ° Fahrenheit or 290 Kelvin")] //even if first in string
        [DataRow("SKV-0MG-87G", "87G ≈ .0137 stone or 3.07 ounces")] //dont convert 0, also dont take minus sign if no space before

        //quotes
        [DataRow("A simple check for 12\" should work", "12\" ≈ 30.5 centimetres")]
        [DataRow("When it is \"quoted as 12\" it \" should also work", "12\" ≈ 30.5 centimetres")]
        [DataRow("But a \"quote that happens to end with 12\" should not work", null)]
        [DataRow("A phrase with a \"quote\" and also a 12\" should work", "12\" ≈ 30.5 centimetres")]
        [DataRow("A phrase with a \"quote\" and also a 12\" should work, even with \"quotes\" after", "12\" ≈ 30.5 centimetres")]
        [DataRow("A phrase with a \"quote\" and also a 12\" should work, even with \"quotes\" and /1\" after", "12\" ≈ 30.5 centimetres")]
        public void ConvertSingle(string input, string expected)
        {
            var results = _analyzer.FindConversions(input);

            if (expected != null)
            {
                Assert.AreEqual(1, results.Count(), input + " should have given 1 match.");
                Assert.AreEqual(expected, results.First());
            }
            else
            {
                Assert.AreEqual(0, results.Count(), input + " was not supposed to match.");
            }
        }

        [TestMethod]
        public void NoDuplicates()
        {
            var results = _analyzer.FindConversions("I added a banana for scale just in case", "And banana again.");
            Assert.AreEqual(1, results.Count());
        }

        [TestMethod]
        public void NoUneccessaryConversions()
        {
            var results = _analyzer.FindConversions("It varied from 12 ft. to about", "And then another text with no measurements.");
            Assert.AreEqual(1, results.Count());
            Assert.AreEqual("12 ft ≈ 3.66 metres", results.First());
        }
    }
}
