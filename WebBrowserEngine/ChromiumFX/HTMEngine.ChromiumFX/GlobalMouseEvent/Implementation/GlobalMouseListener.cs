// This code is distributed under MIT license. 
// Copyright (c) 2015 George Mamaladze
// See license.txt or http://opensource.org/licenses/mit-license.php

using System.Windows.Forms;
using Gma.System.MouseKeyHook.WinApi;
using System.Runtime.InteropServices;

namespace Gma.System.MouseKeyHook.Implementation
{
    // Because it is a P/Invoke method, 'GetSystemMetrics(int)'
    // should be defined in a class named NativeMethods, SafeNativeMethods,
    // or UnsafeNativeMethods.
    // https://msdn.microsoft.com/en-us/library/windows/desktop/ms724385(v=vs.85).aspx
    internal static class NativeMethods
    {
        private const int SM_CXDRAG = 68;
        private const int SM_CYDRAG = 69;

        [DllImport("user32.dll")]
        private static extern int GetSystemMetrics(int index);

        public static int GetXDragThreshold()
        {
            return GetSystemMetrics(SM_CXDRAG);
        }

        public static int GetYDragThreshold()
        {
            return GetSystemMetrics(SM_CYDRAG);
        }
    }

    internal class GlobalMouseListener : IMouseEvents
    {
        private HookResult Handle { get; set; }
        private Point m_PreviousPosition;
        private readonly Point m_UninitialisedPoint = new Point(-1, -1);

        public GlobalMouseListener()
        {
            Handle = HookHelper.HookGlobalMouse(Callback);
            m_PreviousPosition = m_UninitialisedPoint;
        }
        
        public void Dispose()
        {
            Handle.Dispose();
        }

        private bool Callback(CallbackData data)
        {
            var e = GetEventArgs(data);
            if (HasMoved(e.Point))
            {
                ProcessMove(ref e);
            }

            return !e.Handled;
        }

        private MouseEventExtArgs GetEventArgs(CallbackData data)
        {
            return MouseEventExtArgs.FromRawDataGlobal(data);
        }


        private void ProcessMove(ref MouseEventExtArgs e)
        {
            m_PreviousPosition = e.Point;

            OnMove(e);
        }

        private bool HasMoved(Point actualPoint)
        {
            return m_PreviousPosition != actualPoint;
        }

        public event MouseEventHandler MouseMove;

        protected virtual void OnMove(MouseEventArgs e)
        {
            MouseMove?.Invoke(this, e);
        }
    }
}