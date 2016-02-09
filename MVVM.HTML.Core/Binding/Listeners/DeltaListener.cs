using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVVM.HTML.Core.Infra;

namespace MVVM.HTML.Core.Binding.Listeners
{
    internal class DeltaListener<T> where T:class
    {
        private HashSet<T> _Old = new HashSet<T>();
        private HashSet<T> _New = new HashSet<T>();
        private ListenerRegister<T> _ListenerRegister;
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
