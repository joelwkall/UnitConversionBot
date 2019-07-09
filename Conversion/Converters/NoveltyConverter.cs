﻿using Conversion.Model;
using System;

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

        public override Measurement Convert(Measurement m)
        {
            //TODO: also return in inches when possible
            if (m.Unit.Singular == "banana")
            {
                return new Measurement(
                    UnitFamily.Meters.GetUnit("centimeters"),
                    18
                );
            }
            else if (m.Unit.Singular == "doggo" || m.Unit.Singular == "puppy" || m.Unit.Singular == "pupper")
            {
                //dont convert this every time
                if (_random.NextDouble() < _doggoThreshHold)
                    return new Measurement(
                        new Unit("goodness", "goodness"), 
                        double.PositiveInfinity
                    );
            }

            return null;
        }
    }
}