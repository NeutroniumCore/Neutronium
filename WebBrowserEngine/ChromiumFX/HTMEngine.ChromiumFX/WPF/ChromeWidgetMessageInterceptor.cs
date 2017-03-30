using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace Neutronium.WebBrowserEngine.ChromiumFx.WPF
{
    internal class ChromeWidgetMessageInterceptor : NativeWindow
    {
        public ChromeWidgetMessageInterceptor(IntPtr browserHandle)
        {
            IntPtr chromeWidgetHostHandle;
            if (ChromeWidgetHandleFinder.TryFindHandle(browserHandle, out chromeWidgetHostHandle)){
                AssignHandle(chromeWidgetHostHandle);
            }           
        }

        const int WM_MOUSEACTIVATE = 0x0021;
        const int WM_NCLBUTTONDOWN = 0x00A1;

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", SetLastError = true)]
        static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == WM_MOUSEACTIVATE)
            {
                // The default processing of WM_MOUSEACTIVATE results in MA_NOACTIVATE,
                // and the subsequent mouse click is eaten by Chrome.
                // This means any .NET ToolStrip or ContextMenuStrip does not get closed.
                // By posting a WM_NCLBUTTONDOWN message to a harmless co-ordinate of the
                // top-level window, we rely on the ToolStripManager's message handling
                // to close any open dropdowns:
                // http://referencesource.microsoft.com/#System.Windows.Forms/winforms/Managed/System/WinForms/ToolStripManager.cs,1249
                var topLevelWindowHandle = m.WParam;
                IntPtr lParam = IntPtr.Zero;
                PostMessage(topLevelWindowHandle, WM_NCLBUTTONDOWN, IntPtr.Zero, lParam);
            }
        }
    }


    internal static class ChromeWidgetHandleFinder
    {
        private delegate bool EnumWindowProc(IntPtr hwnd, IntPtr lParam);

        [DllImport("user32")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool EnumChildWindows(IntPtr window, EnumWindowProc callback, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        private class ClassDetails
        {
            public IntPtr DescendantFound { get; set; }
        }

        private static bool EnumWindow(IntPtr hWnd, IntPtr lParam)
        {
            const string chromeWidgetHostClassName = "Chrome_RenderWidgetHostHWND";

            var buffer = new StringBuilder(128);
            GetClassName(hWnd, buffer, buffer.Capacity);

            if (buffer.ToString() == chromeWidgetHostClassName)
            {
                var gcHandle = GCHandle.FromIntPtr(lParam);

                var classDetails = (ClassDetails)gcHandle.Target;

                classDetails.DescendantFound = hWnd;
                return false;
            }

            return true;
        }

        /// <summary>
        /// Chrome's message-loop Window isn't created synchronously, so this may not find it.
        /// If so, you need to wait and try again later.
        /// </summary>
        public static bool TryFindHandle(IntPtr browserHandle, out IntPtr chromeWidgetHostHandle)
        {
            var classDetails = new ClassDetails();
            var gcHandle = GCHandle.Alloc(classDetails);

            var childProc = new EnumWindowProc(EnumWindow);
            EnumChildWindows(browserHandle, childProc, GCHandle.ToIntPtr(gcHandle));

            chromeWidgetHostHandle = classDetails.DescendantFound;

            gcHandle.Free();

            return classDetails.DescendantFound != IntPtr.Zero;
        }
    }
}