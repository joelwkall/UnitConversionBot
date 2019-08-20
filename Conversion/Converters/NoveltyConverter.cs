using Conversion.Model;
using System;
using System.Collections.Generic;

namespace Conversion.Converters
{
    public class NoveltyConverter : BaseConverter
    {
        private static Random _random = new Random();

        private double _doggoThreshHold;

        public NoveltyConverter(double doggoThreshHold = 0.1)
        {
            _doggoThreshHold = doggoThreshHold;
        }

        public override void Convert(ConversionCollection collection)
        {
            foreach (var m in collection.AllValidMeasurements)
            {
                if (m.Unit.Singular == "banana")
                {
                    collection.ConvertedMeasurements.Add(new Measurement(
                        UnitFamily.Meters.GetUnit("centimeters"),
                        18
                    ));

                    //collection.ConvertedMeasurements.Add(new Measurement(
                    //    UnitFamily.ImperialDistances.GetUnit("inch"),
                    //    7
                    //));
                }
                else if (m.Unit.Singular == "doggo" || m.Unit.Singular == "puppy" || m.Unit.Singular == "pupper")
                {
                    //dont convert every time
                    if (_random.NextDouble() < _doggoThreshHold)
                        collection.ConvertedMeasurements.Add(new Measurement(
                            new Unit("goodness", "goodness"),
                            double.PositiveInfinity
                        ));
                }
            }
        }
    }
}
