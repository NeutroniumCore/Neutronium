using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Neutronium.Core.Infra 
{
    public static class ProcessHelper 
    {
        public static void OpenFileWithInstalledApplication(string url) 
        {
            //Process.Start(url);
            OpenBrowser(url);
        }

        private static void OpenBrowser(string url) {
            try {
                Process.Start(url);
            } catch {
                // hack because of this: https://github.com/dotnet/corefx/issues/10361
                
                if (Environment.OSVersion.Platform == PlatformID.Win32Windows || Environment.OSVersion.Platform == PlatformID.Win32S || Environment.OSVersion.Platform == PlatformID.Win32NT) {
                    url = url.Replace("&", "^&");
                    Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
                } else if (Environment.OSVersion.Platform == PlatformID.Unix) {
                    Process.Start("xdg-open", url);
                } else if (Environment.OSVersion.Platform == PlatformID.MacOSX) {
                    Process.Start("open", url);
                } else {
                    throw;
                }
            }
        }


        public static void OpenLocalUrlInBrowser(int port) 
        {
            OpenUrlInBrowser($"http://localhost:{port}/");
        }

        public static void OpenUrlInBrowser(string url)
        {
            OpenFileWithInstalledApplication(url);
        }
    }
}
