using System;
using System.Collections.Generic;

namespace Neutronium.Core.Navigation.Routing
{
    public interface ITypesProvider
    {
        IEnumerable<Type> Types { get; }
    }

    public class TypesProvider : ITypesProvider
    {
        public IEnumerable<Type> Types { get; }
        public TypesProvider(IEnumerable<Type> types)
        {
            Types = types;
        }  
    }
}
