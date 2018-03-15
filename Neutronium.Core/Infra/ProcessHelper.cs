using System.Diagnostics;

namespace Neutronium.Core.Infra 
{
    public static class ProcessHelper 
    {
        public static void OpenFileWithInstalledApplication(string url) 
        {
            Process.Start(url);
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
