using System;

namespace MVVM.HTML.Core.Binding.Listeners
{
    internal struct ListenerRegister<T> where T:class
    {
        public ListenerRegister(Action<T> on, Action<T> off) : this()
        {
            On = on;
            Off = off;
        }

        public Action<T> On { get; private set; }

        public Action<T> Off { get; private set; }
    }
}
