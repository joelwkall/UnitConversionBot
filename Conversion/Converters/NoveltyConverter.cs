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

                    collection.ConvertedMeasurements.Add(new Measurement(
                        UnitFamily.ImperialDistances.GetUnit("inch"),
                        7
                    ));
                }
                else if (m.Unit.Singular == "doggo" || m.Unit.Singular == "puppy" || m.Unit.Singular == "pupper")
                {
                    if (SillyTime())
                        collection.ConvertedMeasurements.Add(new Measurement(
                            new Unit("goodness", "goodness"),
                            double.PositiveInfinity
                        ));
                }
                else if (m.Unit.Singular == "washing machine")
                {
                    if (SillyTime(2))
                    {
                        collection.ConvertedMeasurements.Add(new Measurement(
                            UnitFamily.Meters.GetUnit("centimeter"),
                            m.Amount * 60
                        ));
                        collection.ConvertedMeasurements.Add(new Measurement(
                            UnitFamily.ImperialDistances.GetUnit("feet"),
                            m.Amount * 2
                        ));
                    }
                }
                else if (m.Unit.UnitFamily == UnitFamily.Kilograms && SillyTime(2))
                {
                    var amount = m.Amount * m.Unit.Ratio / 80.0;

                    if (amount > 1 && amount < 100)
                        collection.ConvertedMeasurements.Add(new Measurement(
                            new Unit("washing machine", "washing machines"),
                            amount
                        ));
                }
                else if (m.Unit.UnitFamily == UnitFamily.Meters && SillyTime(2))
                {
                    var alternatives = new (double length, string name)[]
                    {
                        (0.6, "washing machine"),
                        (1.5, "small boulder"),
                        (2.5, "half giraffe")
                    };

                    var chosenAlt = alternatives[_random.Next(alternatives.Length)];

                    var amount = m.Amount * m.Unit.Ratio / chosenAlt.length;

                    if (amount > 1 && amount < 100)
                        collection.ConvertedMeasurements.Add(new Measurement(
                            new Unit(chosenAlt.name, chosenAlt.name + "s"),
                            amount
                        ));
                }
                else if (ProclaimerMultiple(m) && SillyTime(3))
                {
                    collection.ConvertedMeasurements.Add(new Measurement(
                        new Unit("proclaimer walk", "proclaimer walks"),
                        m.Amount/500.0
                    ));
                }
            }
        }

        private bool SillyTime(int silliness = 1)
        {
            return _random.NextDouble() < (_randomThreshHold * silliness);
        }

        private static bool ProclaimerMultiple(Measurement m)
        {
            //only operate on miles
            if(m.Unit != UnitFamily.ImperialDistances.GetUnit("mile") && m.Unit != UnitFamily.ImperialDistances.GetUnit("mi"))
                return false;
                    
            //if its a multiple of 500 and not too large
            return Math.Round(m.Amount) % 500 == 0 && m.Amount < 10000;
        }
    }
}
