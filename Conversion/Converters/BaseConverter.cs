using System;
using System.Collections.Generic;
using System.Text;
using Conversion.Model;

namespace Conversion.Converters
{
    public abstract class BaseConverter
    {
        public abstract IEnumerable<Measurement> Convert(Measurement m);
    }
}
