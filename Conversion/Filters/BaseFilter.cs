using System.Collections.Generic;
using Conversion.Model;

namespace Conversion.Filters
{
    public abstract class BaseFilter
    {
        internal abstract bool Keep(ConversionCollection collection, IEnumerable<ConversionCollection> originalCollections);
    }
}
