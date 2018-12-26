using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Application.SetUp.Script
{
    public static class ProcessExtension
    {
        private const int CTRL_C_EVENT = 0;
        [DllImport("kernel32.dll")]
        internal static extern bool GenerateConsoleCtrlEvent(uint dwCtrlEvent, uint dwProcessGroupId);
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool AttachConsole(uint dwProcessId);
        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        internal static extern bool FreeConsole();
        [DllImport("kernel32.dll")]
        static extern bool SetConsoleCtrlHandler(ConsoleCtrlDelegate HandlerRoutine, bool Add);
        // Delegate type to be used as the Handler Routine for SCCH
        delegate Boolean ConsoleCtrlDelegate(uint CtrlType);

        public static bool SendControlC(this Process process)
        {
            if (!AttachConsole((uint)process.Id))
                return false;

            SetConsoleCtrlHandler(null, true);
            try
            {
                if (!GenerateConsoleCtrlEvent(CTRL_C_EVENT, 0))
                    return false;
            }
            finally
            {
                FreeConsole();
                SetConsoleCtrlHandler(null, false);
            }
            return true;
        }
    }
}
