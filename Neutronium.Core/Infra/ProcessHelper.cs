using System.Diagnostics;

namespace Neutronium.Core.Infra 
{
    public static class ProcessHelper 
    {
        public static void OpenFileWinstalledApplication(string url) 
        {
            Process.Start(url);
        }

        public static void OpenLocalUrlInBrowser(int port) 
        {
            OpenUrlInBrowser($"http://localhost:{port}/");
        }

        public static void OpenUrlInBrowser(string url)
        {
            OpenFileWinstalledApplication(url);
        }
    }
}
