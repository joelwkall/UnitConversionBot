using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Conversion
{
    public class Utils
    {
        public static int DetectSignificantDigits(string str)
        {
            int significant = 0;
            int zeroStreak = 0;
            bool started = false;

            foreach (var c in str)
            {
                if (!char.IsDigit(c))
                    continue;

                var digit = int.Parse(c.ToString());

                if (digit > 0)
                {
                    started = true;
                    significant += 1 + zeroStreak;
                    zeroStreak = 0;
                }
                else if (started)
                {
                    zeroStreak++;
                }
            }

            return significant;
        }

        public static double RoundToSignificantDigits(double d, int digits)
        {
            if (d == 0)
                return 0;

            double scale = Math.Pow(10, Math.Floor(Math.Log10(Math.Abs(d))) + 1);
            return scale * Math.Round(d / scale, digits, MidpointRounding.ToEven);
        }

        /// <summary>
        /// Counts the number of leading and trailing zeroes.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int CountLeadingTrailingZeroes(double value)
        {
            var str = value.ToString("0.############################", CultureInfo.InvariantCulture);

            if (str.Length == 1)
                return str == "0" ? 1 : 0;

            int count(IEnumerable<char> s)
            {
                var num = 0;
                foreach (var c in s)
                {
                    if (c == '0')
                        num++;
                    else if(c != '.')
                        break;
                }

                return num;
            }

            return count(str) + count(str.Reverse());
        }
    }
}
