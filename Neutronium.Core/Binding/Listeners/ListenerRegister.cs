﻿using System;

namespace Neutronium.Core.Binding.Listeners
{
    public class ListenerRegister<T>: IEntityUpdater<T> where T:class
    {
        private readonly Action<T> _On;
        private readonly Action<T> _Off;

        public ListenerRegister(Action<T> on, Action<T> off)
        {
            _On = on;
            _Off = off;
        }

        public void OnEnter(T item) => _On(item);

        public void OnExit(T item) => _Off(item);
    }
}
