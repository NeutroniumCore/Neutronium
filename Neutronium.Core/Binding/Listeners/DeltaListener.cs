using System.Collections.Generic;
using System.Linq;
using Neutronium.Core.Infra;

namespace Neutronium.Core.Binding.Listeners
{
    internal class DeltaListener<T> where T:class
    {
        private readonly HashSet<T> _Old = new HashSet<T>();
        private readonly HashSet<T> _New = new HashSet<T>();
        private readonly ListenerRegister<T> _ListenerRegister;
        public DeltaListener(ListenerRegister<T> listenerRegister)
        {
            _ListenerRegister = listenerRegister;
        }

        public void VisitOld(T old)
        {
            _Old.Add(old);
        }

        public void VisitNew(T old)
        {
            _New.Add(old);
        }

        public void Apply()
        {
            _Old.Where(o => !_New.Contains(o)).ForEach(_ListenerRegister.Off);
            _New.Where(o => !_Old.Contains(o)).ForEach(_ListenerRegister.On);
        }
    }

    internal static class DeltaListener
    {
        public static DeltaListener<T> GetDeltaListener<T>(ListenerRegister<T> listenerRegister) where T : class
        {
            return new DeltaListener<T>(listenerRegister);
        }
    }
}
