using Neutronium.Core.Binding.GlueObject;
using System.Collections.Generic;

namespace Neutronium.Core.Binding.Listeners
{
    public interface IEntityUpdater<T>
    {
        void OnEnter(T item);

        void OnExit(T item);
    }
}
