// This code is distributed under MIT license. 
// Copyright (c) 2015 George Mamaladze
// See license.txt or http://opensource.org/licenses/mit-license.php

using System.Windows.Forms;

namespace Gma.System.MouseKeyHook.Implementation
{
    internal class GlobalEventFacade :  IDisposableMouseEvents
    {
        private GlobalMouseListener m_MouseListenerCache;

        public event MouseEventHandler MouseMove
        {
            add { GetMouseListener().MouseMove += value; }
            remove { GetMouseListener().MouseMove -= value; }
        }

        public void Dispose()
        {
            m_MouseListenerCache?.Dispose();
        }

        private GlobalMouseListener GetMouseListener()
        {
            var target = m_MouseListenerCache;
            if (target != null) return target;
            target = new GlobalMouseListener();
            m_MouseListenerCache = target;
            return target;
        }
    }
}