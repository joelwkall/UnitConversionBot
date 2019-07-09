﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Conversion.Model
{
    [Flags]
    public enum MatchOptions
    {
        AllowNone = 0,
        AllowSpace = 1,
        AllowPlural = 2,
        AllowAll = 3
    }

    public class UnitFamily
    {
        public static UnitFamily Mph { get; private set; } = new UnitFamily(
            new Unit("mile per hour", "miles per hour"), 
            "mph");

        public static UnitFamily Kmph { get; private set; } = new UnitFamily(
            new Unit("kilometer per hour", "kilometers per hour"), 
            "km/h", 
            "kph", 
            "kmph");

        //TODO: add meters per second

        public static UnitFamily Meters { get; private set; } = new UnitFamily( 
            new Unit("meter", "meters"),
            new Unit("metre", "metres"), 
            "m",

            new Unit("kilometer", "kilometers", 1000), 
            new Unit("kilometre", "kilometres", 1000),
            new Unit("km", 1000),

            new Unit("centimeter", "centimeters", 0.01), 
            new Unit("centimetre", "centimetres", 0.01), 
            new Unit("cm", 0.01),

            new Unit("millimeter", "millimeters", 0.001),
            new Unit("millimetre", "millimetres", 0.001),
            new Unit("mm", 0.001)
            );

        public static UnitFamily Feet { get; private set; } = new UnitFamily( 
            new Unit("foot", "feet"), 
            new Unit("'", MatchOptions.AllowNone), //TODO: AllowNone are so few so could probably implemented as special cases in the converter instead
            "ft",

            new Unit("inch", "inches", 1.0/12),
            new Unit("in", MatchOptions.AllowNone, 1.0/12), 
            new Unit("\"", MatchOptions.AllowNone, 1.0/12)
            );

        public static UnitFamily Kilograms { get; private set; } = new UnitFamily(
            new Unit("kilogram", "kilograms"),
            "kg",

            new Unit("gram", "grams", 0.001),
            new Unit("g", 0.001),
            new Unit("milligram", "milligrams", 0.000001),
            new Unit("mg", 0.000001),
            new Unit("microgram", "micrograms", 0.000000001),
            new Unit("µg", 0.000000001),

            new Unit("ton", "tons", 1000),
            new Unit("tonne", "tonnes", 1000)
        );

        public static UnitFamily Pounds { get; private set; } = new UnitFamily(
            new Unit("pound", "pounds"),
            new Unit("lb", "lbs"),

            new Unit("ounce", "ounces", 1.0 / 16),
            new Unit("oz", 1.0 / 16),

            new Unit("imperial ton", "imperial tons", 2240)
        );

        public static UnitFamily ImperialVolumes { get; private set; } = new UnitFamily(
            new Unit("imperial gallon", "imperial gallons"),
            "imperial gal",

            new Unit("imperial quart", "imperial quarts", 0.25),
            new Unit("imperial qt", 0.25),
            new Unit("imperial pint", "imperial pints", 0.125),
            new Unit("imperial pt", 0.125),
            new Unit("imperial fluid ounce", "imperial fluid ounces", 0.00625),
            new Unit("imperial fl oz", "imperial fl oz", 0.00625)
        );

        public static UnitFamily USVolumes { get; private set; } = new UnitFamily(
            new Unit("gallon", "gallons"),
            "gal",

            new Unit("quart", "quarts", 0.25),
            new Unit("qt", 0.25), 
            new Unit("pint", "pints", 0.125),
            new Unit("pt", 0.125),
            new Unit("fluid ounce", "fluid ounces", 0.0078125),
            new Unit("fl oz", 0.0078125)
        );

        public static UnitFamily Liters { get; private set; } = new UnitFamily(
            new Unit("liter", "liters"),
            new Unit("litre", "litres"),
            "l",

            new Unit("deciliter", "deciliters", 0.1),
            new Unit("decilitre", "decilitres", 0.1),
            new Unit("dl", 0.1),

            new Unit("centiliter", "centiliters", 0.01),
            new Unit("centilitre", "centilitres", 0.01),
            new Unit("cl", 0.01),

            new Unit("milliliter", "milliliters", 0.001),
            new Unit("millilitre", "millilitres", 0.001),
            new Unit("ml", 0.001)
        );

        //TODO: it's hacky that we need to state these here
        public static List<UnitFamily> AllFamilies => new List<UnitFamily>
        {
            Mph,
            Kmph,
            Meters,
            Feet,
            Kilograms,
            Pounds,
            ImperialVolumes, //We need this to be first because USVolumes will also catch it
            USVolumes,
            Liters
        };

        public static IEnumerable<Unit> AllUnits => AllFamilies.SelectMany(u => u.Units);

        public Unit GetUnit(string unit)
        {
            var u = Units.FirstOrDefault(e => e.Singular == unit || e.Plural == unit);

            if (u != null)
                return u;

            throw new Exception($"Could not find unit '{unit}' in the '{this.Units.First().Singular}' family.");
        }

        static UnitFamily()
        {
            //TODO: This is hacky
            //make all units aware of their family
            foreach (var unit in AllFamilies)
            {
                foreach (var u in unit.Units)
                {
                    u.UnitFamily = unit;
                }
            }
        }
        
        public IEnumerable<Unit> Units { get; private set; }

        public UnitFamily(params Unit[] units)
        {
            Units = new List<Unit>(units);
        }
    }
    
    public class Unit
    {
        public string Singular { get; private set; }
        public string Plural { get; private set; }
        public MatchOptions MatchOptions { get; private set; }
        public double Ratio { get; private set; } = 1;
        public UnitFamily UnitFamily { get; set; }

        public Unit(string singular, string plural, MatchOptions matchOptions, double ratio = 1.0)
        {
            Singular = singular;
            Plural = plural;
            MatchOptions = matchOptions;
            Ratio = ratio;
        }

        public Unit(string singular, string plural, double ratio = 1.0) : this(singular, plural, MatchOptions.AllowAll, ratio)
        {
        }

        public Unit(string singular, MatchOptions matchOptions, double ratio = 1.0) : this(singular, singular, matchOptions, ratio)
        { }

        public Unit(string singular, double ratio = 1.0) : this(singular, singular, MatchOptions.AllowAll, ratio)
        { }

        public static implicit operator Unit(string singular)
        {
            return new Unit(singular, singular);
        }

        public static bool operator ==(Unit u1, Unit u2)
        {
            return EqualityComparer<Unit>.Default.Equals(u1, u2);
        }

        public static bool operator !=(Unit u1, Unit u2)
        {
            return !(u1 == u2);
        }

        public override bool Equals(object obj)
        {
            var u = obj as Unit;
            return u != null &&
                   Singular == u.Singular &&
                   Plural == u.Plural &&
                   MatchOptions == u.MatchOptions &&
                   Ratio == u.Ratio &&
                   EqualityComparer<UnitFamily>.Default.Equals(UnitFamily, u.UnitFamily);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Singular, Plural, MatchOptions, Ratio, UnitFamily);
        }
    }

    public class Measurement
    {
        public Unit Unit { get; private set; }
        public double Amount { get; private set; }

        public Measurement(Unit unit, double amount)
        {
            Unit = unit;
            Amount = amount;
        }

        public string ToString(int significantDigits)
        {
            string strAmount;
            if (double.IsPositiveInfinity(Amount))
            {
                strAmount = "Infinity";
            }
            else
            {
                var roundedAmount = Utils.RoundToSignificantDigits(Amount, significantDigits);

                strAmount = roundedAmount.ToString("#.############################", CultureInfo.InvariantCulture);
            }

            return strAmount + " " + (Amount == 1 ? Unit.Singular : Unit.Plural);
        }
    }

    public class DetectedMeasurement:Measurement
    {
        public string DetectedString;
        public int SignificantDigits;

        public DetectedMeasurement(Unit unit, double amount, int significantDigits, string detectedString):base(unit, amount)
        {
            DetectedString = detectedString;
            SignificantDigits = significantDigits;
        }
    }
}
