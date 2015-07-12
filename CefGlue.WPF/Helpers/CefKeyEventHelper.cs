//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows.Input;

//namespace Xilium.CefGlue.WPF.Helpers
//{
//    public static class CefKeyEventHelper
//    {
//        private const int WM_SYSKEYDOWN = 0x104;
//        private const int WM_KEYDOWN = 0x100;
//        private const int WM_KEYUP = 0x101;
//        private const int WM_SYSKEYUP = 0x105;
//        private const int WM_CHAR = 0x102;
//        private const int WM_SYSCHAR = 0x106;
//        private const int VK_TAB = 0x9;

//        public static CefKeyEvent Get(int message, Key wParam, int lParam, CefEventFlags iCefEventFlags)
//        {
//            CefKeyEvent keyEvent = new CefKeyEvent();
//            keyEvent.WindowsKeyCode = (int)wParam;
//            keyEvent.NativeKeyCode = lParam;
//            keyEvent.IsSystemKey = message == WM_SYSCHAR ||
//                message == WM_SYSKEYDOWN ||
//                message == WM_SYSKEYUP;

//            if (message == WM_KEYDOWN || message == WM_SYSKEYDOWN)
//            {
//                keyEvent.EventType = CefKeyEventType.RawKeyDown;
//            }
//            else if (message == WM_KEYUP || message == WM_SYSKEYUP)
//            {
//                keyEvent.EventType = CefKeyEventType.KeyUp;
//            }
//            else
//            {
//                keyEvent.EventType = CefKeyEventType.Char;
//            }
//            keyEvent.Modifiers = GetCefKeyboardModifiers(wParam, lParam);


//            return keyEvent;
//        }

//        private static bool IsKeyDown(Key wparam)
//        {
//            return Keyboard.GetKeyStates(wparam) == KeyStates.Down;
//        }

//        private static CefEventFlags GetCefKeyboardModifiers(Key wparam, int lparam)
//        {
//            CefEventFlags modifiers = 0;
//            if ((IsKeyDown(Key.LeftShift)) || (IsKeyDown(Key.RightShift)))
//                modifiers |= CefEventFlags.ShiftDown;
//            if ((IsKeyDown(Key.LeftCtrl)) || (IsKeyDown(Key.RightCtrl)))
//                modifiers |= CefEventFlags.ControlDown;
//            if ((IsKeyDown(Key.LeftAlt)) || (IsKeyDown(Key.RightAlt)))
//                modifiers |= CefEventFlags.AltDown;

//            // Low bit set from GetKeyState indicates "toggled".
//            if (Keyboard.GetKeyStates(Key.NumLock) == KeyStates.Toggled)
//                modifiers |= CefEventFlags.NumLockOn;
//            if ( Keyboard.GetKeyStates(Key.Capital) == KeyStates.Toggled)
//                modifiers |= CefEventFlags.CapsLockOn;

//            switch (wparam)
//            {
//                case Key.Return:
//                    if ((lparam >> 16) & KF_EXTENDED)
//                        modifiers |= CefEventFlags.IsKeyPad;
//                    break;

//                case Key.Insert:
//                case Key.Delete:
//                case Key.Home:
//                case Key.End:
//                case Key.Prior:
//                case Key.Next:
//                case Key.Up:
//                case Key.Down:
//                case Key.Left:
//                case Key.Right:
//                    if (!((lparam >> 16) & KF_EXTENDED))
//                        modifiers |= CefEventFlags.IsKeyPad;
//                    break;

//                case Key.NumLock:
//                case Key.NumPad0:
//                case Key.NumPad1:
//                case Key.NumPad2:
//                case Key.NumPad3:
//                case Key.NumPad4:
//                case Key.NumPad5:
//                case Key.NumPad6:
//                case Key.NumPad7:
//                case Key.NumPad8:
//                case Key.NumPad9:
//                case Key.Divide:
//                case Key.Multiply:
//                case Key.Subtract:
//                case Key.Add:
//                case Key.Decimal:
//                case Key.Clear:
//                    modifiers |= CefEventFlags.IsKeyPad;
//                break;

//                case Key.LeftShift:
//                case Key.LeftCtrl:
//                case Key.LeftAlt:
//                    modifiers |= CefEventFlags.IsLeft;
//                break;

//                case Key.RightShift:
//                case Key.RightCtrl:
//                case Key.RightAlt:
//                    modifiers |= CefEventFlags.IsRight;
//                break;
//            }

//            return modifiers;
//        }
//    }
//}
