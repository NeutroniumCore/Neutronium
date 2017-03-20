using System.Collections.Generic;
using System.Linq;
using MoreCollection.Extensions;

namespace Neutronium.Core.Binding.Listeners
{
    internal class DeltaListener<T> where T:class
    {
        private readonly HashSet<T> _Old = new HashSet<T>();
        private readonly HashSet<T> _New = new HashSet<T>();

        public void VisitOld(T old)
        {
            _Old.Add(old);
        }

        public void VisitNew(T old)
        {
            _New.Add(old);
        }

        public void Apply(ListenerRegister<T> listenerRegister)
        {
            _Old.Where(o => !_New.Contains(o)).ForEach(listenerRegister.Off);
            _New.Where(o => !_Old.Contains(o)).ForEach(listenerRegister.On);
        }
    }
}
