using System.Linq;
using Conversion;
using Conversion.Converters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Conversion
{
    [TestClass]
    public class TextAnalyzerTests
    {
        private TextAnalyzer _regularAnalyzer = CreateAnalyzer(0.0);
        private TextAnalyzer _noveltyAnalyzer = CreateAnalyzer(1.0);

        private static TextAnalyzer CreateAnalyzer(double threshHold)
        {
            var a = TextAnalyzer.CreateNewDefault();
            foreach(var nc in a.Converters.OfType<NoveltyConverter>())
                nc.SetRandomThreshHold(threshHold);
            return a;
        }

        [TestMethod]
        public void BeginningAndEnd()
        {
            var str = "12 lbs is the most thing close to 18 mph";

            var results = _regularAnalyzer.FindConversions(str);

            CollectionAssert.AreEquivalent(new[]{ "12 lbs ≈ 5.44 kilograms or .857 stone", "18 mph ≈ 29 kilometres per hour" }, results.ToList(), "Results were " + string.Join(", ",results));
        }

        [TestMethod]
        public void InputDecimalComma()
        {
            var str = "This thing weighs 15,88 lbs";

            var results = _regularAnalyzer.FindConversions(str);

            Assert.AreEqual(1, results.Count());
            Assert.AreEqual("15,88 lbs ≈ 7.203 kilograms or 1.1343 stone", results.First());
        }

        [TestMethod]
        public void InputDecimalPoint()
        {
            var str = "This thing weighs 15.88 lbs";

            var results = _regularAnalyzer.FindConversions(str);

            Assert.AreEqual(1, results.Count());
            Assert.AreEqual("15.88 lbs ≈ 7.203 kilograms or 1.1343 stone", results.First());
        }

        [TestMethod]
        public void OutputDecimal()
        {
            var str = "This thing goes 150 kph";

            var results = _regularAnalyzer.FindConversions(str);

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
            var results = _regularAnalyzer.FindConversions(str);

            Assert.AreEqual(expected, results.Count(), $"Results were: {string.Join(',',results)}");
        }

        [TestMethod]
        public void NoCascades()
        {
            var strings =new[]{"3 inches", "4 km/h"};

            foreach (var str in strings)
            {
                var results = _regularAnalyzer.FindConversions(str);

                Assert.AreEqual(1, results.Count(), str + " caused the wrong number of results.");
            }
        }

        [DataTestMethod]
        
        //spaces and plurals
        [DataRow("10 foot", "10 foot ≈ 3 metres", false)]
        [DataRow("10 feet", "10 feet ≈ 3 metres", false)]
        [DataRow("10'", "10' ≈ 3 metres", false)]
        [DataRow("10 '", null, false)]
        [DataRow("10's", null, false)]
        [DataRow("10 inches", "10 inches ≈ 25 centimetres", false)]
        [DataRow("10 inch", "10 inch ≈ 25 centimetres", false)]
        [DataRow("10in", "10in ≈ 25 centimetres", false)]
        [DataRow("10\"", "10\" ≈ 25 centimetres", false)]
        [DataRow("10 \"", null, false)]
        [DataRow("10\"s", null, false)]
        [DataRow("10 in", null, false)]
        [DataRow("10ins", null, false)]

        //some well chosen units
        [DataRow("This forest is 10 square miles", "10 square miles ≈ 26 square kilometres", false)]
        [DataRow("My apartment is about 92 m^2", "92 m^2 ≈ 990 square feet", false)]
        [DataRow("This boat does 35 knots", "35 knots ≈ 64.8 kilometres per hour or 40.3 miles per hour", false)]
        [DataRow("I can run 10 m/s", "10 m/s ≈ 22 miles per hour", false)]
        [DataRow("This hole is 5 yards", "5 yards ≈ 4.6 metres", false)]

        //line breaks
        [DataRow("something \n450 lbs down", "450 lbs ≈ 204 kilograms or 32.1 stone", false)]
        [DataRow("something \r450 lbs down", "450 lbs ≈ 204 kilograms or 32.1 stone", false)]
        [DataRow("something <br/>450 lbs down", "450 lbs ≈ 204 kilograms or 32.1 stone", false)]
        [DataRow("450 lbs down\n", "450 lbs ≈ 204 kilograms or 32.1 stone", false)]
        [DataRow("450 lbs down\r", "450 lbs ≈ 204 kilograms or 32.1 stone", false)]
        [DataRow("450 lbs down<br/>", "450 lbs ≈ 204 kilograms or 32.1 stone", false)]

        //special chars
        [DataRow("of reaching 185 lbs, however", "185 lbs ≈ 83.91 kilograms or 13.21 stone", false)]
        [DataRow("God imgurs at 5gb's.", null, false)]
        [DataRow("Does it handle the .55 lbs format ?", ".55 lbs ≈ .0393 stone or 249 grams", false)]
        [DataRow("It should handle heights like 6'10\"", "6'10\" ≈ 2.08 metres", false)]

        //novelty stuff
        [DataRow("I added a banana for scale just in case. And banana again.", "1 banana ≈ 18 centimeters or 7 inches", true)]
        [DataRow("This is a nice doggo", "1 doggo ≈ Infinity goodness", true)]
        [DataRow("This hole is 20 meters across", "20 meters ≈ 66 feet or 33 washing machines", true)]
        [DataRow("This hole is 66 feet across", "66 feet ≈ 20.1 metres or 33.5 washing machines", true)] //both ways
        [DataRow("This thing weighs 300 pounds", "300 pounds ≈ 140 kilograms or 21 stone or 1.7 washing machines", true)]
        [DataRow("This thing weighs 140 kilograms", "140 kilograms ≈ 309 pounds or 22 stone or 1.75 washing machines", true)] //both ways
        [DataRow("My washing machine broke down yesterday", "1 washing machine ≈ 60 centimeters or 2 feet", true)]
        [DataRow("I would walk 1500 miles for this", "1500 miles ≈ 2410 kilometres or 3 proclaimer walks", true)]
        [DataRow("The road was 805 km long", "805 km ≈ 500.2 miles or 1 proclaimer walk", true)]

        //converter interactions
        [DataRow("120 meters should not be converted to inches", "120 meters ≈ 394 feet", false)]
        [DataRow("Do not convert 10 kilometers to feet", "10 kilometers ≈ 6.2 miles", false)]
        [DataRow("It was 100 degrees F yesterday", "100 degrees F ≈ 38 ° Celsius or 310 Kelvin", false)]
        [DataRow("It was 45C yesterday", "45C ≈ 113 ° Fahrenheit or 318 Kelvin", false)]
        [DataRow("Liquid nitrogen is 77 Kelvin or colder", "77 Kelvin ≈ -196 ° Celsius or -321 ° Fahrenheit", false)]
        [DataRow("It was -20C yesterday", "-20C ≈ -4 ° Fahrenheit or 250 Kelvin", false)] //minus sign works
        [DataRow("It was -40F yesterday", "-40F ≈ -40 ° Celsius or 230 Kelvin", false)] //minus sign works
        [DataRow("-20C yesterday", "-20C ≈ -4 ° Fahrenheit or 250 Kelvin", false)] //even if first in string
        [DataRow("SKV-0MG-87g", "87g ≈ .0137 stone or 3.07 ounces", false)] //dont convert 0, also dont take minus sign if no space before
        [DataRow("This affects the weight by -20 lbs", "-20 lbs ≈ -9.1 kilograms or -1.4 stone", false)] //minus signs work with readability converter
        [DataRow("This thing is about 5.01 m long", "5.01 m ≈ 16 feet, 5 2/8 inches", false)] //feet formatter works

        //quotes
        [DataRow("A simple check for 12\" should work", "12\" ≈ 30.5 centimetres", false)]
        [DataRow("When it is \"quoted as 12\" it \" should also work", "12\" ≈ 30.5 centimetres", false)]
        [DataRow("But a \"quote that happens to end with 12\" should not work", null, false)]
        [DataRow("A phrase with a \"quote\" and also a 12\" should work", "12\" ≈ 30.5 centimetres", false)]
        [DataRow("A phrase with a \"quote\" and also a 12\" should work, even with \"quotes\" after", "12\" ≈ 30.5 centimetres", false)]
        public void ConvertSingle(string input, string expected, bool novelty)
        {
            var results = (novelty ? _noveltyAnalyzer : _regularAnalyzer).FindConversions(input);

            if (expected != null)
            {
                Assert.AreEqual(1, results.Count(), input + " should have given 1 match. Results were [" + string.Join(',', results) + "]");
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
            var results = _regularAnalyzer.FindConversions("I added a banana for scale just in case", "And banana again.");
            Assert.AreEqual(1, results.Count());
        }

        [TestMethod]
        public void NoUneccessaryConversions()
        {
            var results = _regularAnalyzer.FindConversions("It varied from 12 ft. to about", "And then another text with no measurements.");
            Assert.AreEqual(1, results.Count());
            Assert.AreEqual("12 ft ≈ 3.66 metres", results.First());
        }
    }
}
