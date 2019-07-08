using System;
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

    public class Unit
    {
        public static Unit Mph { get; private set; } = new Unit(
            new UnitExpression("mile per hour", "miles per hour"), 
            "mph");

        public static Unit Kmph { get; private set; } = new Unit(
            new UnitExpression("kilometer per hour", "kilometers per hour"), 
            "km/h", 
            "kph", 
            "kmph");

        //TODO: add meters per second

        public static Unit Meters { get; private set; } = new Unit( 
            new UnitExpression("meter", "meters"),
            new UnitExpression("metre", "metres"), 
            "m",

            new UnitExpression("kilometer", "kilometers", 1000), 
            new UnitExpression("kilometre", "kilometres", 1000),
            new UnitExpression("km", 1000),

            new UnitExpression("centimeter", "centimeters", 0.01), 
            new UnitExpression("centimetre", "centimetres", 0.01), 
            new UnitExpression("cm", 0.01),

            new UnitExpression("millimeter", "millimeters", 0.001),
            new UnitExpression("millimetre", "millimetres", 0.001),
            new UnitExpression("mm", 0.001)
            );

        public static Unit Feet { get; private set; } = new Unit( 
            new UnitExpression("foot", "feet"), 
            new UnitExpression("'", MatchOptions.AllowNone), //TODO: AllowNone are so few so could probably implemented as special cases in the converter instead
            "ft",

            new UnitExpression("inch", "inches", 1.0/12),
            new UnitExpression("in", MatchOptions.AllowNone, 1.0/12), 
            new UnitExpression("\"", MatchOptions.AllowNone, 1.0/12)
            );

        public static Unit Kilograms { get; private set; } = new Unit(
            new UnitExpression("kilogram", "kilograms"),
            "kg",

            new UnitExpression("gram", "grams", 0.001),
            new UnitExpression("g", 0.001),
            new UnitExpression("milligram", "milligrams", 0.000001),
            new UnitExpression("mg", 0.000001),
            new UnitExpression("microgram", "micrograms", 0.000000001),
            new UnitExpression("µg", 0.000000001),

            new UnitExpression("ton", "tons", 1000),
            new UnitExpression("tonne", "tonnes", 1000)
        );

        public static Unit Pounds { get; private set; } = new Unit(
            new UnitExpression("pound", "pounds"),
            new UnitExpression("lb", "lbs"),

            new UnitExpression("ounce", "ounces", 1.0 / 16),
            new UnitExpression("oz", 1.0 / 16),

            new UnitExpression("imperial ton", "imperial tons", 2240)
        );

        public static List<Unit> AllUnits => new List<Unit>
        {
            Mph,
            Kmph,
            Meters,
            Feet,
            Kilograms,
            Pounds
        };

        public static IEnumerable<UnitExpression> AllExpressions => AllUnits.SelectMany(u => u.Expressions);

        public UnitExpression GetExpression(string expression)
        {
            return Expressions.First(e => e.Singular == expression || e.Plural == expression);
        }

        static Unit()
        {
            //make all expressions aware of their parent
            foreach (var unit in AllUnits)
            {
                foreach (var expr in unit.Expressions)
                {
                    expr.Unit = unit;
                }
            }
        }
        
        public IEnumerable<UnitExpression> Expressions { get; private set; }

        public Unit(params UnitExpression[] alternativeExpressions)
        {
            Expressions = new List<UnitExpression>(alternativeExpressions);
        }
    }
    
    public class UnitExpression
    {
        public string Singular { get; private set; }
        public string Plural { get; private set; }
        public MatchOptions MatchOptions { get; private set; }
        public double Ratio { get; private set; } = 1;
        public Unit Unit { get; set; }

        public UnitExpression(string singular, string plural, MatchOptions matchOptions, double ratio = 1.0)
        {
            Singular = singular;
            Plural = plural;
            MatchOptions = matchOptions;
            Ratio = ratio;
        }

        public UnitExpression(string singular, string plural, double ratio = 1.0) : this(singular, plural, MatchOptions.AllowAll, ratio)
        {
        }

        public UnitExpression(string singular, MatchOptions matchOptions, double ratio = 1.0) : this(singular, singular, matchOptions, ratio)
        { }

        public UnitExpression(string singular, double ratio = 1.0) : this(singular, singular, MatchOptions.AllowAll, ratio)
        { }

        public static implicit operator UnitExpression(string singular)
        {
            return new UnitExpression(singular, singular);
        }

        public static bool operator ==(UnitExpression expression1, UnitExpression expression2)
        {
            return EqualityComparer<UnitExpression>.Default.Equals(expression1, expression2);
        }

        public static bool operator !=(UnitExpression expression1, UnitExpression expression2)
        {
            return !(expression1 == expression2);
        }

        public override bool Equals(object obj)
        {
            var expression = obj as UnitExpression;
            return expression != null &&
                   Singular == expression.Singular &&
                   Plural == expression.Plural &&
                   MatchOptions == expression.MatchOptions &&
                   Ratio == expression.Ratio &&
                   EqualityComparer<Unit>.Default.Equals(Unit, expression.Unit);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Singular, Plural, MatchOptions, Ratio, Unit);
        }
    }

    public class Measurement
    {
        public UnitExpression UnitExpression;
        public double Amount;
        
        public Measurement(UnitExpression unitExpression, double amount)
        {
            UnitExpression = unitExpression;
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

            return strAmount + " " + (Amount == 1 ? UnitExpression.Singular : UnitExpression.Plural);
        }
    }

    public class DetectedMeasurement:Measurement
    {
        public string DetectedString;
        public int SignificantDigits;

        public DetectedMeasurement(UnitExpression unitExpression, double amount, int significantDigits, string detectedString):base(unitExpression, amount)
        {
            DetectedString = detectedString;
            SignificantDigits = significantDigits;
        }
    }
}
