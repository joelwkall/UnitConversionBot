using System;
using System.Collections.Generic;
using System.Text;
using Conversion.Model;

namespace Conversion.Converters
{
    public abstract class BaseConverter
    {
        //TODO: make this able to return multiple so that for instance stones or bananas can be translated to bouth pounds and kg
        public abstract Measurement Convert(Measurement m);
    }
}
