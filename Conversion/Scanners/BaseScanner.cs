using Conversion.Model;
using System.Collections.Generic;

namespace Conversion.Scanners
{
    public abstract class BaseScanner
    {
        public abstract (string remaining, IEnumerable<DetectedMeasurement> foundMeasurements) FindMeasurements(string str);
    }
}
