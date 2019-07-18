using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Conversion.Scanners;
using System.Globalization;

namespace Tests.Conversion.Scanners
{
    [TestClass]
    public class BaseScannerTests
    {
        [DataTestMethod]
        //decimal point
        [DataRow("Parsing the number 1 works", "1")]
        [DataRow("Parsing the number 1.0 works", "1.0")]
        [DataRow("Parsing the number .1 works", ".1")]
        [DataRow("Parsing the number 1works without space after", "1")]
        [DataRow("Parsing the number 1.0works without space after", "1.0")]
        [DataRow("Parsing the number .1works without space after", ".1")]
        [DataRow("Parsing the number1 works without space before", "1")]
        [DataRow("Parsing the number1.0 works without space before", "1.0")]
        [DataRow("Parsing the number.1 works without space before", ".1")]
        [DataRow("Parsing the number 1,000 works", "1,000")]
        [DataRow("Parsing the number 1,000.0 works", "1,000.0")]
        [DataRow("Parsing the number 1,000.0000 works", "1,000.0000")]
        [DataRow("Parsing the number 1,000,000.0000 works", "1,000,000.0000")]
        [DataRow("Parsing the number 1 000 works", "1 000")]
        [DataRow("Parsing the number 1 000.0 works", "1 000.0")]
        [DataRow("Parsing the number 1 000.0000 works", "1 000.0000")]
        [DataRow("Parsing the number 1 000 000.0000 works", "1 000 000.0000")]
        [DataRow("Parsing the number 1,00.0000 works", "1,00.0000")] //this is invalid but we allow it and let the number parser figure it out

        //decimal comma
        [DataRow("Parsing the number 1 works", "1")]
        [DataRow("Parsing the number 1,0 works", "1,0")]
        [DataRow("Parsing the number ,1 works", ",1")]
        [DataRow("Parsing the number 1works without space after", "1")]
        [DataRow("Parsing the number 1,0works without space after", "1,0")]
        [DataRow("Parsing the number ,1works without space after", ",1")]
        [DataRow("Parsing the number1 works without space before", "1")]
        [DataRow("Parsing the number1,0 works without space before", "1,0")]
        [DataRow("Parsing the number,1 works without space before", ",1")]
        [DataRow("Parsing the number 1.000 works", "1.000")]
        [DataRow("Parsing the number 1.000,0 works", "1.000,0")]
        [DataRow("Parsing the number 1.000,0000 works", "1.000,0000")]
        [DataRow("Parsing the number 1.000.000,0000 works", "1.000.000,0000")]
        [DataRow("Parsing the number 1 000 works", "1 000")]
        [DataRow("Parsing the number 1 000,0 works", "1 000,0")]
        [DataRow("Parsing the number 1 000,0000 works", "1 000,0000")]
        [DataRow("Parsing the number 1 000 000,0000 works", "1 000 000,0000")]
        [DataRow("Parsing the number 1.00,0000 works", "1.00,0000")] //this is invalid but we allow it and let the number parser figure it out
        public void NumberRegex(string input, string expected)
        {
            var regex = Regex.Match(input, BaseScanner.NumberRegex);

            if (regex.Success && regex.Groups.Count > 0)
            {
                var amountStr = regex.Groups[0].Captures[0].Value;
                if (expected != null)
                {
                    Assert.AreEqual(expected, amountStr);
                }
                else
                {
                    throw new Exception("Unexpected result " + amountStr);
                }
            }
            else if (expected != null)
                throw new Exception("No result.");
        }

        [DataTestMethod]
        //decimal point
        [DataRow("1.0", 1.0)]
        [DataRow("0.5", 0.5)]
        [DataRow(".5", 0.5)]
        [DataRow("1000.0", 1000.0)]
        [DataRow("1,000.0", 1000.0)]
        [DataRow("1 000.0", 1000.0)]
        [DataRow("1.000", 1000.0)]
        [DataRow("1.600", 1600.0)]
        [DataRow("1.120", 1.12)]
        [DataRow("1,000,000.0", 1000000.0)]
        [DataRow("1 000 000.0", 1000000.0)]
        //decimal comma
        [DataRow("1,0", 1.0)]
        [DataRow("0,5", 0.5)]
        [DataRow(",5", 0.5)]
        [DataRow("1000,0", 1000.0)]
        [DataRow("1.000,0", 1000.0)]
        [DataRow("1 000,0", 1000.0)]
        [DataRow("1,000", 1000.0)]
        [DataRow("1,600", 1600.0)]
        [DataRow("1,120", 1.12)]
        [DataRow("1.000.000,0", 1000000.0)]
        [DataRow("1 000 000,0", 1000000.0)]
        public void Parse(string input, double expected)
        {
            if(BaseScanner.Parse(input, out var output))
                Assert.AreEqual(expected, output);
            else
            {
                throw new Exception("Could not parse.");
            }
        }
    }
}
