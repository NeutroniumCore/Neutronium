using System.Diagnostics;

namespace Neutronium.Core.Infra 
{
    public static class ProcessHelper 
    {
        public static void OpenFileWinstalledApplication(string url) 
        {
            Process.Start(string.Format(url));
        }

        public static void OpenLocalUrlInBrowser(int port) 
        {
            OpenFileWinstalledApplication($"http://localhost:{port}/");
        }
    }
}
