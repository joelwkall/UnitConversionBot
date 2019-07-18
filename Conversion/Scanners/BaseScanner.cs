using Conversion.Model;
using System.Collections.Generic;
using System.Globalization;

namespace Conversion.Scanners
{
    public abstract class BaseScanner
    {
        //TODO: it's ugly that this method is expected to remove the occurences.
        //Better if DetectedMeasurement had the index of the found string instead or something
        public abstract (string remaining, IEnumerable<DetectedMeasurement> foundMeasurements) FindMeasurements(string str);

        public static string NumberRegex = "((?:(?:\\.|,)?\\d+(?:[\\.,\\s]\\d+)*(?:(?:\\.|,)\\d+)?))";

        //Omitting front slash from here because we dont want it to catch fractions
        public static string WordSeparatorRegex = "[<>.,;:\\-_!#¤%&()=?`@£${}+´¨~*'\"]";

        protected bool Parse(string str, out double amount)
        {
            //TODO: smarter number parser that doesn't parse "3,5" as "35"
            if (double.TryParse(str,
                NumberStyles.Number | NumberStyles.AllowThousands, CultureInfo.InvariantCulture,
                out amount))
            {
                return true;
            }

            return false;
        }
    }
}
