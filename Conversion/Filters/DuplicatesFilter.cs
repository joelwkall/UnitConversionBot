using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Conversion.Model;

namespace Conversion.Filters
{
    /// <summary>
    /// Removes instances of measurements where the converted value is already present
    /// as a detected value, and vice versa. The purpose is to not convert measurements
    /// where a conversion was already in the post.
    /// Example "The tree was 30 ft (10 meters) tall"
    /// </summary>
    public class DuplicatesFilter : BaseFilter
    {
        internal override bool Keep(KeyValuePair<DetectedMeasurement, List<Measurement>> pair, IEnumerable<KeyValuePair<DetectedMeasurement, List<Measurement>>> originalMeasurements)
        {
            //if the detected measurement is included in any of the other converted measurements
            if (originalMeasurements.Any(otherPair =>
                otherPair.Value.Any(otherValue => RepresentsTheSame(pair.Key, otherValue))))
                return false;

            //if any of the converted measurements is already one of the detected measurements
            if (originalMeasurements.Any(otherPair => 
                pair.Value.Any(value => RepresentsTheSame(otherPair.Key, value))))
                return false;

            return true;
        }

        public static bool RepresentsTheSame(Measurement a, Measurement b)
        {
            if (a.Unit.UnitFamily != b.Unit.UnitFamily)
                return false;

            if (a.BaseAmount > b.BaseAmount * 1.05 || b.BaseAmount > a.BaseAmount * 1.05)
                return false;

            return true;
        }
    }
}
