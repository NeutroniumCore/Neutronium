using System.Diagnostics;

namespace MVVM.HTML.Core.Infra 
{
    public static class ProcessHelper 
    {
        public static void OpenFileWinstalledApplication(string url) 
        {
            Process.Start(string.Format(url));
        }

        public static void OpenLocalUrlInBrowser(int port) 
        {
            OpenFileWinstalledApplication(string.Format("http://localhost:{0}/", port));
        }
    }
}
