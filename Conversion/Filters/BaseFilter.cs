using System.Collections.Generic;
using Conversion.Model;

namespace Conversion.Filters
{
    public abstract class BaseFilter
    {
        internal abstract bool Keep(KeyValuePair<DetectedMeasurement, List<Measurement>> pair, IEnumerable<KeyValuePair<DetectedMeasurement, List<Measurement>>> originalMeasurements);
    }
}
