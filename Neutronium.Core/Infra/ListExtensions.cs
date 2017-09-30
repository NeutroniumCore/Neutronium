using System.Collections.Generic;

namespace Neutronium.Core.Infra
{
    public static class ListExtensions
    {
        public static void Apply<TSource>(this IList<TSource> collection, IndexDescriptor insertDescriptor, TSource element)
        {
            if (insertDescriptor.Insert)
                collection.Insert(insertDescriptor.Index, element);
            else
                collection[insertDescriptor.Index] = element;
        }
    }
}
