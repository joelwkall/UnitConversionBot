using Conversion.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Conversion.Converters
{
    public class NoveltyConverter : BaseConverter
    {
        private static Random _random = new Random();

        private double _randomThreshHold;

        //only for testing
        public void SetRandomThreshHold(double threshHold)
        {
            _randomThreshHold = threshHold;
        }

        public NoveltyConverter(double randomThreshHold = 0.1)
        {
            _randomThreshHold = randomThreshHold;
        }

        public override void Convert(ConversionCollection collection)
        {
            foreach (var m in collection.AllValidMeasurements.ToList())
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
                    if (_random.NextDouble() < _randomThreshHold)
                        collection.ConvertedMeasurements.Add(new Measurement(
                            new Unit("goodness", "goodness"),
                            double.PositiveInfinity
                        ));
                }
                else if (m.Unit.Singular == "washing machine")
                {
                    //dont convert every time
                    if (_random.NextDouble() < _randomThreshHold)
                        collection.ConvertedMeasurements.Add(new Measurement(
                            UnitFamily.Meters.GetUnit("centimeter"),
                            m.Amount * 60
                        ));
                }
                else if (m.Unit.UnitFamily == UnitFamily.Meters)
                {
                    var amount = m.Amount * m.Unit.Ratio / 0.6;

                    //dont convert every time
                    if (amount > 1 && amount < 100 && _random.NextDouble() < _randomThreshHold)
                        collection.ConvertedMeasurements.Add(new Measurement(
                            new Unit("washing machine", "washing machines"),
                            amount
                        ));
                }
            }
        }
    }
}
