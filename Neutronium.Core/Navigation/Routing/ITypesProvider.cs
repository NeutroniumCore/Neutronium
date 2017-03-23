using System;
using System.Collections.Generic;

namespace Neutronium.Core.Navigation.Routing
{
    /// <summary>
    /// provider of type enumerable
    /// </summary>
    public interface ITypesProvider
    {
        /// <summary>
        /// collection of types
        /// </summary>
        IEnumerable<Type> Types { get; }
    }
}
