using Conversion.Model;
using System.Collections.Generic;

namespace Conversion.Scanners
{
    public abstract class BaseScanner
    {
        //TODO: it's ugly that this method is expected to remove the occurences.
        //Better if DetectedMeasurement had the index of the found string instead or something
        public abstract (string remaining, IEnumerable<DetectedMeasurement> foundMeasurements) FindMeasurements(string str);
    }
}
